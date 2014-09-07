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
    /// Source Controller - Basic CRUD for Source
    /// </summary>
    public class SourceController : ApiController
    {
	
		private SousChefEntities _db = new SousChefEntities();

        // GET api/Source/
        /// <summary>
        /// An iQueryable Source lookup
        /// </summary>
        /// <returns>
        /// 200 - Success + A list of Source
        /// 401 - Not Authorized 
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IQueryable<SourceModel> Get()
        {
            return _db.Sources.Select(o => new SourceModel
            {
                id = o.id,
                description = o.description
            });
        }

        // GET api/Source/5
        /// <summary>
        /// Retrieve a single Source from the database.
        /// </summary>
        /// <param name="id">The SourceId of the Source to return.</param>
        /// <returns>
        /// 200 - Success + The requested Source.
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// </returns>
		[ResponseType(typeof(SourceModel))]
		public IHttpActionResult Get(int id)
        {
            SourceModel source = Get().FirstOrDefault<SourceModel>(o => o.id == id);
            if (source == null)
            {
                return this.NotFound("Source not found.");
            }

            return Ok(source);
        }

        // PUT api/Source/5
        /// <summary>
        /// Save changes to a single Source to the database.
        /// </summary>
        /// <param name="id">The SourceId of the Source to save.</param>
        /// <param name="sourceModel">The model of the edited Source</param>
        /// <returns>
        /// 204 - No Content
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Put(int id, SourceModel sourceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sourceModel.id)
            {
                return BadRequest();
            }

            try
            {
				Source source = _db.Sources.FirstOrDefault(o => o.id == id);
				if (source == null)
                    throw new APIException("Source not found.", 404);
                source.description = sourceModel.description;

                _db.Entry(source).State = EntityState.Modified;
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

        // POST api/Source/
        /// <summary>
        /// A new Source to be added.
        /// </summary>
        /// <param name="sourceModel">The new Source</param>
        /// <returns>
        /// 201 - Created + The new Source
        /// 400 - Bad Request + (Invalid Model State)
        /// 401 - Not Authorized 
        /// 404 - Not Found + Reason
        /// 500 - Internal Server Error + Exception
        /// </returns>
		[ResponseType(typeof(SourceModel))]
		public IHttpActionResult Post(SourceModel sourceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
				Source source = new Source();
                source.description = sourceModel.description;
                
                _db.Sources.Add(source);
                _db.SaveChanges();

                sourceModel.id = source.id;

                return CreatedAtRoute("DefaultApi", new { id = source.id }, sourceModel);
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

        // DELETE api/Source/5
        /// <summary>
        /// Delete a Source from the database.
        /// </summary>
        /// <param name="id">The SourceId of the Source to delete.</param>
        /// <returns>
		/// 200 - Success + The deleted Source 
		/// 401 - Not Authorized 
        /// 405 - Method Not Allowed
		/// 500 - Internal Server Error + the Exception
        /// </returns>
		[ResponseType(typeof(SourceModel))]
		public IHttpActionResult Delete(int id)
        {
            try {
				// For Objects which cannot be deleted:
				// return StatusCode(HttpStatusCode.MethodNotAllowed);  // Update Return Codes
				Source source = _db.Sources.Find(id);
				if (source == null)
				{
					return this.NotFound("Source not found.");
				}

                SourceModel returnModel = Get().FirstOrDefault<SourceModel>(o => o.id == id);
                _db.Sources.Remove(source);
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
