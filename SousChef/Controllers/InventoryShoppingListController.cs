using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SousChef.Data;
using SousChef.Models;
using SousChef.Classes;

namespace SousChef.Controllers
{
    /// <summary>
    /// InventoryShoppingList Controller - Basic CRUD for InventoryShoppingList
    /// </summary>
    public class InventoryShoppingListController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/InventoryShoppingList/
        /// <summary>
        /// An iQueryable InventoryShoppingList lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of InventoryShoppingList
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<InventoryShoppingListModel> Get()
        {
            return _db.InventoryShoppingLists.Select(o => new InventoryShoppingListModel
            {
                id = o.id, 
                inventoryId = o.inventoryId, 
                itemId = o.itemId, 
                inventoryUserId = o.inventoryUserId
            });
        }

        // GET api/InventoryShoppingList/5
        /// <summary>
        /// Retrieve a single InventoryShoppingList from the database.
        /// </summary>
        /// <param name="id">The InventoryShoppingListId of the InventoryShoppingList to return.</param>
        /// <returns>
        /// 200 - Success + The requested InventoryShoppingList.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(InventoryShoppingListModel))]
		public IHttpActionResult Get(int id)
        {
            InventoryShoppingListModel inventoryshoppinglist = Get().FirstOrDefault<InventoryShoppingListModel>(o => o.id == id);
            if (inventoryshoppinglist == null)
            {
                return this.NotFound("InventoryShoppingList not found.");
            }

            return Ok(inventoryshoppinglist);
        }

        // PUT api/InventoryShoppingList/5
        /// <summary>
        /// Save changes to a single InventoryShoppingList to the database.
        /// </summary>
        /// <param name="id">The InventoryShoppingListId of the InventoryShoppingList to save.</param>
        /// <param name="inventoryshoppinglistModel">The model of the edited InventoryShoppingList</param>
        /// <returns>
        /// 405 - Method Not Allowed
        /// </returns>
        public IHttpActionResult Put(int id, InventoryShoppingListModel inventoryshoppinglistModel)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);  
        }

        // POST api/InventoryShoppingList/
        /// <summary>
        /// A new InventoryShoppingList to be added.
        /// </summary>
        /// <param name="inventoryshoppinglistModel">The new InventoryShoppingList</param>
        /// <returns>
        /// 201 - Created + The new InventoryShoppingList
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(InventoryShoppingListModel))]
		public IHttpActionResult Post(InventoryShoppingListModel inventoryshoppinglistModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				InventoryShoppingList inventoryshoppinglist = new InventoryShoppingList();
                inventoryshoppinglist.inventoryId = inventoryshoppinglistModel.inventoryId;
                inventoryshoppinglist.itemId = inventoryshoppinglistModel.itemId;
                inventoryshoppinglist.inventoryUserId = inventoryshoppinglistModel.inventoryUserId;
                
                _db.InventoryShoppingLists.Add(inventoryshoppinglist);
                _db.SaveChanges();

                inventoryshoppinglistModel.id = inventoryshoppinglist.id;

                return CreatedAtRoute("DefaultApi", new { id = inventoryshoppinglist.id }, inventoryshoppinglistModel);
            }
            catch (APIException ex)
            {
                if (ex.code == 404)
                    return this.NotFound(ex.message);
                else
                    return this.InternalServerError(ex);
            }
            catch (Exception e)
            {
                return this.InternalServerError(e);
            }
        }

        // DELETE api/InventoryShoppingList/5
        /// <summary>
        /// Delete a InventoryShoppingList from the database.
        /// </summary>
        /// <param name="id">The InventoryShoppingListId of the InventoryShoppingList to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted InventoryShoppingList 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(InventoryShoppingListModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				InventoryShoppingList inventoryshoppinglist = _db.InventoryShoppingLists.Find(id);
				if (inventoryshoppinglist == null)
				{
					return this.NotFound("InventoryShoppingList not found.");
				}

                InventoryShoppingListModel returnModel = Get().FirstOrDefault<InventoryShoppingListModel>(o => o.id == id);
                _db.InventoryShoppingLists.Remove(inventoryshoppinglist);
                _db.SaveChanges();

                return Ok(returnModel);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
