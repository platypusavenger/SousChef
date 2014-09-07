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
    /// ReceiptItem Controller - Basic CRUD for ReceiptItem
    /// </summary>
    public class ReceiptItemController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/ReceiptItem/
        /// <summary>
        /// An iQueryable ReceiptItem lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of ReceiptItem
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<ReceiptItemModel> Get()
        {
            return _db.ReceiptItems.Select(o => new ReceiptItemModel
            {
                id = o.id, 
                receiptId = o.receiptId, 
                itemId = o.itemId
            });
        }

        // GET api/ReceiptItem/5
        /// <summary>
        /// Retrieve a single ReceiptItem from the database.
        /// </summary>
        /// <param name="id">The ReceiptItemId of the ReceiptItem to return.</param>
        /// <returns>
        /// 200 - Success + The requested ReceiptItem.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(ReceiptItemModel))]
		public IHttpActionResult Get(int id)
        {
            ReceiptItemModel receiptitem = Get().FirstOrDefault<ReceiptItemModel>(o => o.id == id);
            if (receiptitem == null)
            {
                return this.NotFound("ReceiptItem not found.");
            }

            return Ok(receiptitem);
        }

        // PUT api/ReceiptItem/5
        /// <summary>
        /// Save changes to a single ReceiptItem to the database.
        /// </summary>
        /// <param name="id">The ReceiptItemId of the ReceiptItem to save.</param>
        /// <param name="receiptitemModel">The model of the edited ReceiptItem</param>
        /// <returns>
        /// 405 - Method Not Allowed
        /// </returns>
        public IHttpActionResult Put(int id, ReceiptItemModel receiptitemModel)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // POST api/ReceiptItem/
        /// <summary>
        /// A new ReceiptItem to be added.
        /// </summary>
        /// <param name="receiptitemModel">The new ReceiptItem</param>
        /// <returns>
        /// 201 - Created + The new ReceiptItem
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(ReceiptItemModel))]
		public IHttpActionResult Post(ReceiptItemModel receiptitemModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                ReceiptItem receiptitem = new ReceiptItem();
                receiptitem.itemId = receiptitemModel.itemId;
                receiptitem.receiptId = receiptitemModel.receiptId;

                _db.ReceiptItems.Add(receiptitem);
                _db.SaveChanges();

                receiptitemModel.id = receiptitem.id;

                return CreatedAtRoute("DefaultApi", new { id = receiptitem.id }, receiptitemModel);
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

        // DELETE api/ReceiptItem/5
        /// <summary>
        /// Delete a ReceiptItem from the database.
        /// </summary>
        /// <param name="id">The ReceiptItemId of the ReceiptItem to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted ReceiptItem 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(ReceiptItemModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				ReceiptItem receiptitem = _db.ReceiptItems.Find(id);
				if (receiptitem == null)
				{
					return this.NotFound("ReceiptItem not found.");
				}

                ReceiptItemModel returnModel = Get().FirstOrDefault<ReceiptItemModel>(o => o.id == id);
                _db.ReceiptItems.Remove(receiptitem);
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
