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
    /// Receipt Controller - Basic CRUD for Receipt
    /// </summary>
    public class ReceiptController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/Receipt/
        /// <summary>
        /// An iQueryable Receipt lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of Receipt
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<ReceiptModel> Get()
        {
            return _db.Receipts.Select(o => new ReceiptModel
            {
                id = o.id, 
                inventoryUserSourceId = o.inventoryUserSourceId,
                timestamp = o.timestamp
            });
        }

        // GET api/Receipt/5
        /// <summary>
        /// Retrieve a single Receipt from the database.
        /// </summary>
        /// <param name="id">The ReceiptId of the Receipt to return.</param>
        /// <returns>
        /// 200 - Success + The requested Receipt.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(ReceiptModel))]
		public IHttpActionResult Get(int id)
        {
            ReceiptModel receipt = Get().FirstOrDefault<ReceiptModel>(o => o.id == id);
            if (receipt == null)
            {
                return this.NotFound("Receipt not found.");
            }

            return Ok(receipt);
        }

        // PUT api/Receipt/5
        /// <summary>
        /// Save changes to a single Receipt to the database.
        /// </summary>
        /// <param name="id">The ReceiptId of the Receipt to save.</param>
        /// <param name="receiptModel">The model of the edited Receipt</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, ReceiptModel receiptModel)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // POST api/Receipt/
        /// <summary>
        /// A new Receipt to be added.
        /// </summary>
        /// <param name="receiptModel">The new Receipt</param>
        /// <returns>
        /// 201 - Created + The new Receipt
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(ReceiptModel))]
		public IHttpActionResult Post(ReceiptModel receiptModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Receipt receipt = new Receipt();
                receipt.inventoryUserSourceId = receiptModel.inventoryUserSourceId;
                receipt.timestamp = receiptModel.timestamp;
				
                _db.Receipts.Add(receipt);
                _db.SaveChanges();

                receiptModel.id = receipt.id;

                return CreatedAtRoute("DefaultApi", new { id = receipt.id }, receiptModel);
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

        // DELETE api/Receipt/5
        /// <summary>
        /// Delete a Receipt from the database.
        /// </summary>
        /// <param name="id">The ReceiptId of the Receipt to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted Receipt 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(ReceiptModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				Receipt receipt = _db.Receipts.Find(id);
				if (receipt == null)
				{
					return this.NotFound("Receipt not found.");
				}

                ReceiptModel returnModel = Get().FirstOrDefault<ReceiptModel>(o => o.id == id);
                _db.Receipts.Remove(receipt);
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
