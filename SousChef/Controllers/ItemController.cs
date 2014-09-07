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
    /// Item Controller - Basic CRUD for Item
    /// </summary>
    public class ItemController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/Item/
        /// <summary>
        /// An iQueryable Item lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of Item
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<ItemModel> Get()
        {
            return _db.Items.Select(o => new ItemModel
            {
                id = o.id, 
                description = o.description,
                upc = o.upc,
                isPerishable = o.isPerishable,
                expiration = o.expiration,
                image = o.image
            });
        }

        // GET api/Item/5
        /// <summary>
        /// Retrieve a single Item from the database.
        /// </summary>
        /// <param name="id">The ItemId of the Item to return.</param>
        /// <returns>
        /// 200 - Success + The requested Item.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(ItemModel))]
		public IHttpActionResult Get(int id)
        {
            ItemModel item = Get().FirstOrDefault<ItemModel>(o => o.id == id);
            if (item == null)
            {
                return this.NotFound("Item not found.");
            }

            return Ok(item);
        }

        // PUT api/Item/5
        /// <summary>
        /// Save changes to a single Item to the database.
        /// </summary>
        /// <param name="id">The ItemId of the Item to save.</param>
        /// <param name="itemModel">The model of the edited Item</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, ItemModel itemModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != itemModel.id)
            {
                return BadRequest();
            }

            try
            {
                Item item = _db.Items.FirstOrDefault(o => o.id == id);
				if (item == null)
                    throw new APIException("Item not found.", 404);
                item.description = itemModel.description;
                item.upc = itemModel.upc;
                item.isPerishable = itemModel.isPerishable;
                item.expiration = itemModel.expiration;
                item.image = itemModel.image;

                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
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

        // POST api/Item/
        /// <summary>
        /// A new Item to be added.
        /// </summary>
        /// <param name="itemModel">The new Item</param>
        /// <returns>
        /// 201 - Created + The new Item
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(ItemModel))]
		public IHttpActionResult Post(ItemModel itemModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Item item = new Item();
                item.description = itemModel.description;
                item.upc = itemModel.upc;
                item.isPerishable = itemModel.isPerishable;
                item.expiration = itemModel.expiration;
                item.image = itemModel.image;
                
                _db.Items.Add(item);
                _db.SaveChanges();

                itemModel.id = item.id;

                return CreatedAtRoute("DefaultApi", new { id = item.id }, itemModel);
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

        // DELETE api/Item/5
        /// <summary>
        /// Delete a Item from the database.
        /// </summary>
        /// <param name="id">The ItemId of the Item to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted Item 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(ItemModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				Item item = _db.Items.Find(id);
				if (item == null)
				{
					return this.NotFound("Item not found.");
				}

                ItemModel returnModel = Get().FirstOrDefault<ItemModel>(o => o.id == id);
                _db.Items.Remove(item);
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
