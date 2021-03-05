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
using System.Diagnostics;

namespace CovidNews.Controllers
{
    public class VariantDataController : ApiController
    {

        private CovidDataContext db = new CovidDataContext();

        /// <summary>Needs more
        /// Gets a list or Variants in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Variants including their ID and name.</returns>
        /// <example>
        /// GET: api/VariantData/GetVariants
        /// </example>
        [ResponseType(typeof(IEnumerable<VariantDto>))]
        public IHttpActionResult GetVariants()
        {
            List<Variant> Variants = db.Variants.ToList();
            List<VariantDto> VariantDtos = new List<VariantDto> { };

            foreach (var Variant in Variants)
            {
                VariantDto NewVariant = new VariantDto
                {
                    VariantID = Variant.VariantID,
                    VariantName = Variant.VariantName
                };
                VariantDtos.Add(NewVariant);
            }

            return Ok(VariantDtos);
        }

        /// <summary>
        /// Gets a list or Countries in the database associated with a particular variant. Returns a status code (200 OK)
        /// </summary>
        /// <param name="id">The input variant id</param>
        /// <returns>A list of Countries including their ID and name</returns>
        /// <example>
        /// GET: api/CountryData/GetCountriesPotentiallyWithVariant
        /// </example>
        [ResponseType(typeof(IEnumerable<CountryDto>))]
        public IHttpActionResult GetCountriesPotentiallyWithVariant(int id)
        {
            //sql equivalent
            //select * from countries
            //inner join variantcountries on variantcountries.countryid = countries.countryid
            //inner join variants on variants.variantid = variantcountries.variantid
            List<Country> Countries = db.Countries
                .Where(c => c.Variants.Any(v => v.VariantID == id))
                .ToList();
            List<CountryDto> CountryDtos = new List<CountryDto> { };


            foreach (var Country in Countries)
            {
                CountryDto NewCountry = new CountryDto
                {
                    CountryID = Country.CountryID,
                    CountryName = Country.CountryName,
                    Population = Country.Population,
                    Infected = Country.Infected,
                    Vaccinated = Country.Vaccinated,
                    Variants = Country.Variants
                };
                CountryDtos.Add(NewCountry);
            }

            return Ok(CountryDtos);
        }

        /// <summary>
        /// Gets a list or Countries in the database NOT associated with a variant. These could be potentially sponsored countries.
        /// </summary>
        /// <param name="id">The input variant id</param>
        /// <returns>A list of Countries including their ID and name</returns>
        /// <example>
        /// GET: api/CountryData/GetCountriesPotentiallyWithVariant
        /// </example>
        [ResponseType(typeof(IEnumerable<CountryDto>))]
        public IHttpActionResult GetCountriesWithNoVariant(int id)
        {
            List<Country> Countries = db.Countries
                .Where(c => !c.Variants.Any(v => v.VariantID == id))
                .ToList();
            List<CountryDto> CountryDtos = new List<CountryDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Country in Countries)
            {
                CountryDto NewCountry = new CountryDto
                {
                CountryID = Country.CountryID,
                CountryName = Country.CountryName,
                Population = Country.Population,
                Infected = Country.Infected,
                Vaccinated = Country.Vaccinated,
                Variants = Country.Variants
                };
                CountryDtos.Add(NewCountry);
            }

            return Ok(CountryDtos);
        }

        /// <summary>
        /// Finds a particular Variant in the database with a 200 status code. If the Variant is not found, return 404.
        /// </summary>
        /// <param name="id">The Variant id</param>
        /// <returns>Information about the Variant, including Variant and name</returns>
        // <example>
        // GET: api/VariantData/FindVariant/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(VariantDto))]
        public IHttpActionResult FindVariant(int id)
        {
            //Find the data
            Variant Variant = db.Variants.Find(id);
            //if not found, return 404 status code.
            if (Variant == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            VariantDto VariantDto = new VariantDto
            {
                VariantID = Variant.VariantID,
                VariantName = Variant.VariantName
            };


            //pass along data as 200 status code OK response
            return Ok(VariantDto);
        }

        /// <summary>
        /// Updates a Variant in the database given information about the Variant.
        /// </summary>
        /// <param name="id">The Variant id</param>
        /// <param name="Variant">A Variant object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/VariantData/UpdateVariant/5
        /// FORM DATA: Variant JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateVariant(int id, [FromBody] Variant Variant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Variant.VariantID)
            {
                return BadRequest();
            }

            db.Entry(Variant).State = EntityState.Modified;

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


        /// <summary>
        /// Adds a Variant to the database.
        /// </summary>
        /// <param name="Variant">A Variant object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/Variants/AddVariant
        ///  FORM DATA: Variant JSON Object
        /// </example>
        [ResponseType(typeof(Variant))]
        [HttpPost]
        public IHttpActionResult AddVariant([FromBody] Variant Variant)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Variants.Add(Variant);
            db.SaveChanges();

            return Ok(Variant.VariantID);
        }

        /// <summary>
        /// Deletes a Variant in the database
        /// </summary>
        /// <param name="id">The id of the Variant to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/Variants/DeleteVariant/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteVariant(int id)
        {
            Variant Variant = db.Variants.Find(id);
            if (Variant == null)
            {
                return NotFound();
            }

            db.Variants.Remove(Variant);
            db.SaveChanges();

            return Ok();
        }




        ///// <summary>
        ///// Deletes a relationship between a particular country and a variant
        ///// </summary>
        ///// <param name="countryid">The country id</param>
        ///// <param name="variantid">The Variant id</param>
        ///// <returns>status code of 200 OK</returns>
        //[HttpGet]
        //[Route("api/variantdata/novariant/{countryid}/{variantid}")]
        //public IHttpActionResult Novariant(int countryid, int variantid)
        //{
        //    //First select the variant (also loading in country data)
        //    Variant SelectedVariant = db.Variants
        //        .Include(v => v.Countries)
        //        .Where(v => v.VariantID == variantid)
        //        .FirstOrDefault();

        //    //Then select the country
        //    Country SelectedCountry = db.Countries.Find(countryid);

        //    //Debug.WriteLine("Selected Variant is.. " + SelectedVariant.VariantName);
        //    //Debug.WriteLine("Selected Country is.. " + SelectedCountry.CountryName);

        //    if (SelectedVariant == null || SelectedCountry == null || !SelectedVariant.Countries.Contains(SelectedCountry))
        //    {

        //        return NotFound();
        //    }
        //    else
        //    {
        //        //Remove the variant from the country
        //        SelectedVariant.Countries.Remove(SelectedCountry);
        //        db.SaveChanges();
        //        return Ok();
        //    }
        //}



        ///// <summary>
        ///// Adds a relationship between a particular country and a variant
        ///// </summary>
        ///// <param name="countryid">The country id</param>
        ///// <param name="variantid">The Variant id</param>
        ///// <returns>status code of 200 OK</returns>
        //[HttpGet]
        //[Route("api/variantdata/variant/{countryid}/{variantid}")]
        //public IHttpActionResult Variant(int countryid, int variantid)
        //{
        //    //First select the variant (also loading in country data)
        //    Variant SelectedVariant = db.Variants
        //        .Include(v => v.Countries)
        //        .Where(v => v.VariantID == variantid)
        //        .FirstOrDefault();

        //    //Then select the country
        //    Country SelectedCountry = db.Countries.Find(countryid);

        //    //Debug.WriteLine("Selected Variant Variant is.. " + SelectedVariant.VariantName);
        //    //Debug.WriteLine("Selected Country is.. " + SelectedCountry.CountryName);

        //    if (SelectedVariant == null || SelectedCountry == null || SelectedVariant.Countries.Contains(SelectedCountry))
        //    {

        //        return NotFound();
        //    }
        //    else
        //    {
        //        //Remove the variant from the country
        //        SelectedVariant.Countries.Add(SelectedCountry);
        //        db.SaveChanges();
        //        return Ok();
        //    }
        //}




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Variant in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Variant id</param>
        /// <returns>TRUE if the Variant exists, false otherwise.</returns>
        private bool VariantExists(int id)
        {
            return db.Variants.Count(e => e.VariantID == id) > 0;
        }

        private bool VariantAssociated(int countryid, int variantid)
        {
            return true;
        }
    }
}