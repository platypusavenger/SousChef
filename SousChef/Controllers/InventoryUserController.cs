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
    /// InventoryUser Controller - Basic CRUD for InventoryUser
    /// </summary>
    public class InventoryUserController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/InventoryUser/
        /// <summary>
        /// An iQueryable InventoryUser lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of InventoryUser
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<InventoryUserModel> Get()
        {
            return _db.InventoryUsers.Select(o => new InventoryUserModel
            {
                id = o.id, 
                inventoryId = o.inventoryId,
                displayName = o.displayName,
                email = o.email,
                isVerified = o.isVerified,
                phoneNumber = o.phoneNumber
            });
        }

        // GET api/InventoryUser/5
        /// <summary>
        /// Retrieve a single InventoryUser from the database.
        /// </summary>
        /// <param name="id">The InventoryUserId of the InventoryUser to return.</param>
        /// <returns>
        /// 200 - Success + The requested InventoryUser.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(InventoryUserModel))]
		public IHttpActionResult Get(int id)
        {
            InventoryUserModel inventoryuser = Get().FirstOrDefault<InventoryUserModel>(o => o.id == id);
            if (inventoryuser == null)
            {
                return this.NotFound("InventoryUser not found.");
            }

            return Ok(inventoryuser);
        }

        // PUT api/InventoryUser/5
        /// <summary>
        /// Save changes to a single InventoryUser to the database.
        /// </summary>
        /// <param name="id">The InventoryUserId of the InventoryUser to save.</param>
        /// <param name="inventoryuserModel">The model of the edited InventoryUser</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, InventoryUserModel inventoryuserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryuserModel.id)
            {
                return BadRequest();
            }

            try
            {
				InventoryUser inventoryuser = _db.InventoryUsers.FirstOrDefault(o => o.id == id);
				if (inventoryuser == null)
                    throw new APIException("InventoryUser not found.", 404);
                inventoryuser.inventoryId = inventoryuserModel.inventoryId;
                inventoryuser.displayName = inventoryuserModel.displayName;
                inventoryuser.email = inventoryuserModel.email;
                inventoryuser.isVerified = inventoryuserModel.isVerified;
                inventoryuser.phoneNumber = inventoryuserModel.phoneNumber;

                _db.Entry(inventoryuser).State = EntityState.Modified;
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

        // POST api/InventoryUser/
        /// <summary>
        /// A new InventoryUser to be added.
        /// </summary>
        /// <param name="inventoryuserModel">The new InventoryUser</param>
        /// <returns>
        /// 201 - Created + The new InventoryUser
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(InventoryUserModel))]
		public IHttpActionResult Post(InventoryUserModel inventoryuserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                InventoryUser inventoryuser = new InventoryUser();
                inventoryuser.inventoryId = inventoryuserModel.inventoryId;
                inventoryuser.displayName = inventoryuserModel.displayName;
                inventoryuser.email = inventoryuserModel.email;
                inventoryuser.isVerified = inventoryuserModel.isVerified;
                inventoryuser.phoneNumber = inventoryuserModel.phoneNumber;
                
                _db.InventoryUsers.Add(inventoryuser);
                _db.SaveChanges();

                inventoryuserModel.id = inventoryuser.id;

                return CreatedAtRoute("DefaultApi", new { id = inventoryuser.id }, inventoryuserModel);
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

        // DELETE api/InventoryUser/5
        /// <summary>
        /// Delete a InventoryUser from the database.
        /// </summary>
        /// <param name="id">The InventoryUserId of the InventoryUser to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted InventoryUser 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(InventoryUserModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				InventoryUser inventoryuser = _db.InventoryUsers.Find(id);
				if (inventoryuser == null)
				{
					return this.NotFound("InventoryUser not found.");
				}

                InventoryUserModel returnModel = Get().FirstOrDefault<InventoryUserModel>(o => o.id == id);
                _db.InventoryUsers.Remove(inventoryuser);
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
