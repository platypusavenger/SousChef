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
    /// InventoryUserSource Controller - Basic CRUD for InventoryUserSource
    /// </summary>
    public class InventoryUserSourceController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/InventoryUserSource/
        /// <summary>
        /// An iQueryable InventoryUserSource lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of InventoryUserSource
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<InventoryUserSourceModel> Get()
        {
            return _db.InventoryUserSources.Select(o => new InventoryUserSourceModel
            {
                id = o.id,
                inventoryUserId = o.inventoryUserId,
                sourceId = o.sourceId,
                accessKey = o.accessKey
            });
        }

        // GET api/InventoryUserSource/5
        /// <summary>
        /// Retrieve a single InventoryUserSource from the database.
        /// </summary>
        /// <param name="id">The InventoryUserSourceId of the InventoryUserSource to return.</param>
        /// <returns>
        /// 200 - Success + The requested InventoryUserSource.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(InventoryUserSourceModel))]
		public IHttpActionResult Get(int id)
        {
            InventoryUserSourceModel inventoryusersource = Get().FirstOrDefault<InventoryUserSourceModel>(o => o.id == id);
            if (inventoryusersource == null)
            {
                return this.NotFound("InventoryUserSource not found.");
            }

            return Ok(inventoryusersource);
        }

        // PUT api/InventoryUserSource/5
        /// <summary>
        /// Save changes to a single InventoryUserSource to the database.
        /// </summary>
        /// <param name="id">The InventoryUserSourceId of the InventoryUserSource to save.</param>
        /// <param name="inventoryusersourceModel">The model of the edited InventoryUserSource</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, InventoryUserSourceModel inventoryusersourceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryusersourceModel.id)
            {
                return BadRequest();
            }

            try
            {
				InventoryUserSource inventoryusersource = _db.InventoryUserSources.FirstOrDefault(o => o.id == id);
				if (inventoryusersource == null)
                    throw new APIException("InventoryUserSource not found.", 404);
                inventoryusersource.accessKey = inventoryusersourceModel.accessKey;

                _db.Entry(inventoryusersource).State = EntityState.Modified;
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

        // POST api/InventoryUserSource/
        /// <summary>
        /// A new InventoryUserSource to be added.
        /// </summary>
        /// <param name="inventoryusersourceModel">The new InventoryUserSource</param>
        /// <returns>
        /// 201 - Created + The new InventoryUserSource
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(InventoryUserSourceModel))]
		public IHttpActionResult Post(InventoryUserSourceModel inventoryusersourceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				InventoryUserSource inventoryusersource = new InventoryUserSource();
                inventoryusersource.inventoryUserId = inventoryusersourceModel.inventoryUserId;
                inventoryusersource.sourceId = inventoryusersourceModel.sourceId;
                inventoryusersource.accessKey = inventoryusersource.accessKey;

                _db.InventoryUserSources.Add(inventoryusersource);
                _db.SaveChanges();

                inventoryusersourceModel.id = inventoryusersource.id;

                return CreatedAtRoute("DefaultApi", new { id = inventoryusersource.id }, inventoryusersourceModel);
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

        // DELETE api/InventoryUserSource/5
        /// <summary>
        /// Delete a InventoryUserSource from the database.
        /// </summary>
        /// <param name="id">The InventoryUserSourceId of the InventoryUserSource to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted InventoryUserSource 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(InventoryUserSourceModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				InventoryUserSource inventoryusersource = _db.InventoryUserSources.Find(id);
				if (inventoryusersource == null)
				{
					return this.NotFound("InventoryUserSource not found.");
				}

                InventoryUserSourceModel returnModel = Get().FirstOrDefault<InventoryUserSourceModel>(o => o.id == id);
                _db.InventoryUserSources.Remove(inventoryusersource);
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
