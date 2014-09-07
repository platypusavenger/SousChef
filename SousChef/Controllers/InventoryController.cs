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
using System.Reflection;

namespace SousChef.Controllers
{	
    /// <summary>
    /// Inventory Controller - Basic CRUD for Inventory
    /// </summary>
    public class InventoryController : ApiController
    {
        private SousChefEntities _db = new SousChefEntities();

        // GET api/Inventory/
        /// <summary>
        /// An iQueryable Inventory lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of Inventory
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<InventoryModel> Get()
        {
            return _db.Inventories.Select(o => new InventoryModel
            {
                id = o.id, 
                description = o.description, 
                version = o.version
            });
        }

        // GET api/Inventory/5
        /// <summary>
        /// Retrieve a single Inventory from the database.
        /// </summary>
        /// <param name="id">The InventoryId of the Inventory to return.</param>
        /// <returns>
        /// 200 - Success + The requested Inventory.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(InventoryModel))]
		public IHttpActionResult Get(int id)
        {
            InventoryModel inventory = Get().FirstOrDefault<InventoryModel>(o => o.id == id);
            if (inventory == null)
            {
                return this.NotFound("Inventory not found.");
            }

            return Ok(inventory);
        }

        // PUT api/Inventory/5
        /// <summary>
        /// Save changes to a single Inventory to the database.
        /// </summary>
        /// <param name="id">The InventoryId of the Inventory to save.</param>
        /// <param name="inventoryModel">The model of the edited Inventory</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, InventoryModel inventoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryModel.id)
            {
                return BadRequest();
            }

            try
            {
				Inventory inventory = _db.Inventories.FirstOrDefault(o => o.id == id);
				if (inventory == null)
                    throw new APIException("Inventory not found.", 404);
                inventory.description = inventoryModel.description;
                inventory.version = inventoryModel.version;

                _db.Entry(inventory).State = EntityState.Modified;
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

        // POST api/Inventory/
        /// <summary>
        /// A new Inventory to be added.
        /// </summary>
        /// <param name="inventoryModel">The new Inventory</param>
        /// <returns>
        /// 201 - Created + The new Inventory
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(InventoryModel))]
		public IHttpActionResult Post(InventoryModel inventoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				Inventory inventory = new Inventory();
                inventory.description = inventoryModel.description;
                var thisApp = Assembly.GetExecutingAssembly();
                AssemblyName name = new AssemblyName(thisApp.FullName);
                inventory.version = name.Version.ToString();

                _db.Inventories.Add(inventory);
                _db.SaveChanges();

                inventoryModel.id = inventory.id;

                return CreatedAtRoute("DefaultApi", new { id = inventory.id }, inventoryModel);
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

        // DELETE api/Inventory/5
        /// <summary>
        /// Delete a Inventory from the database.
        /// </summary>
        /// <param name="id">The InventoryId of the Inventory to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted Inventory 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(InventoryModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				Inventory inventory = _db.Inventories.Find(id);
				if (inventory == null)
				{
					return this.NotFound("Inventory not found.");
				}

                InventoryModel returnModel = Get().FirstOrDefault<InventoryModel>(o => o.id == id);
                _db.Inventories.Remove(inventory);
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
