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
    public class CountryDataController : ApiController
    {
        //This variable is our database access point
        private CovidDataContext db = new CovidDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Country"
        //Choose context "Covid Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.


        /// <summary>
        /// Gets a list or Countries in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Countries including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/CountryData/GetCountries
        /// </example>
        [ResponseType(typeof(IEnumerable<CountryDto>))]
        public IHttpActionResult GetCountries()
        {
            List<Country> Countries = db.Countries.ToList();
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
        /// Gets a list of articles in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input countryid</param>
        /// <returns>A list of articles associated with the country</returns>
        /// <example>
        /// GET: api/CountryData/GetArticlesForCountry
        /// </example>
        [ResponseType(typeof(IEnumerable<ArticleDto>))]
        public IHttpActionResult GetArticlesForCountry(int id)
        {
            List<Article> Articles = db.Articles.Where(p => p.CountryID == id)
                .ToList();
            List<ArticleDto> ArticleDtos = new List<ArticleDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Article in Articles)
            {
                ArticleDto NewArticle = new ArticleDto
                {
                    ArticleID = Article.ArticleID,
                    ArticleName = Article.ArticleName,
                    Publisher = Article.Publisher,
                    ArticleHasPic = Article.ArticleHasPic,
                    PicExtension = Article.PicExtension,
                    CountryID = Article.CountryID
                };
                ArticleDtos.Add(NewArticle);
            }

            return Ok(ArticleDtos);
        }

        /// <summary>
        /// Gets a list or Variants in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input countryid</param>
        /// <returns>A list of Variants including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/VariantData/GetVariantsForCountry
        /// </example>
        [ResponseType(typeof(IEnumerable<VariantDto>))]
        public IHttpActionResult GetVariantsForCountry(int id)
        {
            List<Variant> Variants = db.Variants
                .Where(s => s.Countries.Any(t => t.CountryID == id))
                .ToList();
            List<VariantDto> VariantDtos = new List<VariantDto> { };

            //Here you can choose which information is exposed to the API
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
        /// Finds a particular Country in the database with a 200 status code. If the Country is not found, return 404.
        /// </summary>
        /// <param name="id">The Country id</param>
        /// <returns>Information about the Country, including Country id, bio, first and last name, and countryid</returns>
        // <example>
        // GET: api/CountryData/FindCountry/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(CountryDto))]
        public IHttpActionResult FindCountry(int id)
        {
            //Find the data
            Country Country = db.Countries.Find(id);
            //if not found, return 404 status code.
            if (Country == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            CountryDto CountryDto = new CountryDto
            {
                CountryID = Country.CountryID,
                CountryName = Country.CountryName,
                Population = Country.Population,
                Infected = Country.Infected,
                Vaccinated = Country.Vaccinated,
                Variants = Country.Variants
            };


            //pass along data as 200 status code OK response
            return Ok(CountryDto);
        }

        /// <summary>
        /// Updates a Country in the database given information about the Country.
        /// </summary>
        /// <param name="id">The Country id</param>
        /// <param name="Country">A Country object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/CountryData/UpdateCountry/5
        /// FORM DATA: Country JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCountry(int id, [FromBody] Country Country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Country.CountryID)
            {
                return BadRequest();
            }

            db.Entry(Country).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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
        /// Adds a Country to the database.
        /// </summary>
        /// <param name="Country">A Country object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/CountryData/AddCountry
        ///  FORM DATA: Country JSON Object
        /// </example>
        [ResponseType(typeof(Country))]
        [HttpPost]
        public IHttpActionResult AddCountry([FromBody] Country Country)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Countries.Add(Country);
            db.SaveChanges();

            return Ok(Country.CountryID);
        }

        /// <summary>
        /// Deletes a Country in the database
        /// </summary>
        /// <param name="id">The id of the Country to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/CountryData/DeleteCountry/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteCountry(int id)
        {
            Country Country = db.Countries.Find(id);
            if (Country == null)
            {
                return NotFound();
            }

            db.Countries.Remove(Country);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Country in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Country id</param>
        /// <returns>TRUE if the Country exists, false otherwise.</returns>
        private bool CountryExists(int id)
        {
            return db.Countries.Count(e => e.CountryID == id) > 0;
        }
    }
}