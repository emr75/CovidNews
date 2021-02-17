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
using CovidNews.Models;

namespace CovidNews.Controllers
{
    public class VariantsController : ApiController
    {
        private CovidDataContext db = new CovidDataContext();

        // GET: api/Variants
        public IQueryable<Variant> GetVariants()
        {
            return db.Variants;
        }

        // GET: api/Variants/5
        [ResponseType(typeof(Variant))]
        public IHttpActionResult GetVariant(int id)
        {
            Variant variant = db.Variants.Find(id);
            if (variant == null)
            {
                return NotFound();
            }

            return Ok(variant);
        }

        // PUT: api/Variants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVariant(int id, Variant variant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != variant.VariantID)
            {
                return BadRequest();
            }

            db.Entry(variant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Variants
        [ResponseType(typeof(Variant))]
        public IHttpActionResult PostVariant(Variant variant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Variants.Add(variant);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = variant.VariantID }, variant);
        }

        // DELETE: api/Variants/5
        [ResponseType(typeof(Variant))]
        public IHttpActionResult DeleteVariant(int id)
        {
            Variant variant = db.Variants.Find(id);
            if (variant == null)
            {
                return NotFound();
            }

            db.Variants.Remove(variant);
            db.SaveChanges();

            return Ok(variant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VariantExists(int id)
        {
            return db.Variants.Count(e => e.VariantID == id) > 0;
        }
    }
}