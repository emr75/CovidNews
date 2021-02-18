using System;
using System.IO;
using System.Web;
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
    public class ArticleDataController : ApiController
    {
        //This variable is our database access point
        private CovidDataContext db = new CovidDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Article"
        //Choose context "Covid Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.


        /// <summary>
        /// Gets a list or articles in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of articles including their ID, bio, first name, last name, and countryid.</returns>
        /// <example>
        /// GET: api/ArticleData/GetArticles
        /// </example>
        [ResponseType(typeof(IEnumerable<ArticleDto>))]
        public IHttpActionResult GetArticles()
        {
            List<Article> Articles = db.Articles.ToList();
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
        /// Finds a particular article in the database with a 200 status code. If the article is not found, return 404.
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns>Information about the article, including article id, bio, first and last name, and countryid</returns>
        // <example>
        // GET: api/ArticleData/FindArticle/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(ArticleDto))]
        public IHttpActionResult FindArticle(int id)
        {
            //Find the data
            Article Article = db.Articles.Find(id);
            //if not found, return 404 status code.
            if (Article == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            ArticleDto ArticleDto = new ArticleDto
            {
                ArticleID = Article.ArticleID,
                ArticleName = Article.ArticleName,
                Publisher = Article.Publisher,
                ArticleHasPic = Article.ArticleHasPic,
                PicExtension = Article.PicExtension,
                CountryID = Article.CountryID
            };


            //pass along data as 200 status code OK response
            return Ok(ArticleDto);
        }

        /// <summary>
        /// Finds a particular Country in the database given a article id with a 200 status code. If the Country is not found, return 404.
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns>Information about the Country, including Country id, bio, first and last name, and countryid</returns>
        // <example>
        // GET: api/CountryData/FindCountryForArticle/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(CountryDto))]
        public IHttpActionResult FindCountryForArticle(int id)
        {
            //Finds the first country which has any articles
            //that match the input articleid
            Country Country = db.Countries
                .Where(t => t.Articles.Any(p => p.ArticleID == id))
                .FirstOrDefault();
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
        /// Updates a article in the database given information about the article.
        /// </summary>
        /// <param name="id">The article id</param>
        /// <param name="article">A article object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/ArticleData/UpdateArticle/5
        /// FORM DATA: Article JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateArticle(int id, [FromBody] Article article)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.ArticleID)
            {
                return BadRequest();
            }


            db.Entry(article).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
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
        /// Receives article picture data, uploads it to the webserver and updates the article's HasPic option
        /// </summary>
        /// <param name="id">the article id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F articlepic=@file.jpg "https://localhost:xx/api/articledata/updateplayerpic/2"
        /// POST: api/ArticleData/UpdateArticlePic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UpdateArticlePic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var ArticlePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (ArticlePic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(ArticlePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Articles/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Articles/"), fn);

                                //save the file
                                ArticlePic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the article haspic and picextension fields in the database
                                Article SelectedArticle = db.Articles.Find(id);
                                SelectedArticle.ArticleHasPic = haspic;
                                SelectedArticle.PicExtension = extension;
                                db.Entry(SelectedArticle).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Article Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }


        /// <summary>
        /// Adds a article to the database.
        /// </summary>
        /// <param name="article">A article object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/ArticleData/AddArticle
        ///  FORM DATA: Article JSON Object
        /// </example>
        [ResponseType(typeof(Article))]
        [HttpPost]
        public IHttpActionResult AddArticle([FromBody] Article article)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Articles.Add(article);
            db.SaveChanges();

            return Ok(article.ArticleID);
        }

        /// <summary>
        /// Deletes a article in the database
        /// </summary>
        /// <param name="id">The id of the article to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/ArticleData/DeleteArticle/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteArticle(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            //also delete image from path
            string path = HttpContext.Current.Server.MapPath("~/Content/Articles/" + id + "." + article.PicExtension);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine("File exists... preparing to delete!");
                System.IO.File.Delete(path);
            }

            db.Articles.Remove(article);
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
        /// Finds a article in the system. Internal use only.
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns>TRUE if the article exists, false otherwise.</returns>
        private bool ArticleExists(int id)
        {
            return db.Articles.Count(e => e.ArticleID == id) > 0;
        }
    }

}