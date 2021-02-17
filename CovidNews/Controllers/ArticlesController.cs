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
    public class ArticlesController : ApiController
    {
        //This variable is our database access point
        private CovidDataContext db = new CovidDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Article"
        //Choose context "Covid Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.

        // GET: api/Articles/GetArticles
        // TODO: Searching Logic?
        public IEnumerable<ArticleDto> GetArticles()
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
                    Publisher = Article.Publisher
                };
                ArticleDtos.Add(NewArticle);
            }

            return ArticleDtos;
        }

        // GET: api/Articles/FindArticle/5
        [ResponseType(typeof(ArticleDto))]
        [HttpGet]
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
                Publisher = Article.Publisher
            };


            //pass along data as 200 status code OK response
            return Ok(ArticleDto);
        }

        // POST: api/Articles/UpdateArticle/5
        // FORM DATA: Article JSON Object
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

        // POST: api/Articles/AddArticle
        // FORM DATA: Article JSON Object
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

            return CreatedAtRoute("DefaultApi", new { id = article.ArticleID }, article);
        }

        // POST: api/Articles/DeleteArticle/5
        [HttpPost]
        public IHttpActionResult DeleteArticle(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return NotFound();
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

        private bool ArticleExists(int id)
        {
            return db.Articles.Count(e => e.ArticleID == id) > 0;
        }
    }
}
