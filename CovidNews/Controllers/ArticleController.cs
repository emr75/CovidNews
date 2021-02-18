using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CovidNews.Models;
using CovidNews.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;


namespace CovidNews.Controllers
{
    public class ArticleController : Controller
    {

        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static ArticleController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:56807/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));




        }



        // GET: Article/List
        public ActionResult List()
        {
            string url = "articledata/getarticles";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ArticleDto> SelectedArticles = response.Content.ReadAsAsync<IEnumerable<ArticleDto>>().Result;
                return View(SelectedArticles);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Article/Details/5
        public ActionResult Details(int id)
        {
            ShowArticle ViewModel = new ShowArticle();
            string url = "articledata/findarticle/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into article data transfer object
                ArticleDto SelectedArticle = response.Content.ReadAsAsync<ArticleDto>().Result;
                ViewModel.article = SelectedArticle;


                url = "articledata/findcountryforarticle/" + id;
                response = client.GetAsync(url).Result;
                CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
                ViewModel.country = SelectedCountry;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Article/Create
        public ActionResult Create()
        {
            UpdateArticle ViewModel = new UpdateArticle();
            //get information about teams this article COULD play for.
            string url = "countrydata/getcountries";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CountryDto> PotentialCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
            ViewModel.allcountries = PotentialCountries;

            return View(ViewModel);
        }

        // POST: Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Article ArticleInfo)
        {
            Debug.WriteLine(ArticleInfo.ArticleName);
            string url = "articledata/addarticle";
            Debug.WriteLine(jss.Serialize(ArticleInfo));
            HttpContent content = new StringContent(jss.Serialize(ArticleInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int articleid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = articleid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Article/Edit/8
        public ActionResult Edit(int id)
        {
            UpdateArticle ViewModel = new UpdateArticle();

            string url = "articledata/findarticle/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into article data transfer object
                ArticleDto SelectedArticle = response.Content.ReadAsAsync<ArticleDto>().Result;
                ViewModel.article = SelectedArticle;

                //get information about teams this article COULD play for.
                url = "countrydata/getcountries";
                response = client.GetAsync(url).Result;
                IEnumerable<CountryDto> PotentialCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                ViewModel.allcountries = PotentialCountries;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Article/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Article ArticleInfo, HttpPostedFileBase ArticlePic)
        {
            Debug.WriteLine(ArticleInfo.ArticleName);
            string url = "articledata/updatearticle/" + id;
            Debug.WriteLine(jss.Serialize(ArticleInfo));
            HttpContent content = new StringContent(jss.Serialize(ArticleInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                //Send over image data for article
                url = "articledata/updateplayerpic/" + id;
                Debug.WriteLine("Received article picture " + ArticlePic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ArticlePic.InputStream);
                requestcontent.Add(imagecontent, "ArticlePic", ArticlePic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Article/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "articledata/findarticle/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into article data transfer object
                ArticleDto SelectedArticle = response.Content.ReadAsAsync<ArticleDto>().Result;
                return View(SelectedArticle);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Article/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "articledata/deletearticle/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}