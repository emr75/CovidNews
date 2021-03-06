using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CovidNews.Models;
using CovidNews.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace CovidNews.Controllers
{
    public class CountryController : Controller
    {


        private readonly JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static CountryController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);

            client.BaseAddress = new Uri("http://localhost:56807/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


        }



        // GET: Country/List
        public ActionResult List()
        {
            string url = "countrydata/getcountries";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<CountryDto> SelectedCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                return View(SelectedCountries);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Country/Details/5
        public ActionResult Details(int id)
        {
            ShowCountry ViewModel = new ShowCountry();
            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.

            if (response.IsSuccessStatusCode)
            {
                //Country goes in Data Transfer Object
                CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
                ViewModel.country = SelectedCountry;

                //Find articles about this country
                url = "countrydata/getarticlesforcountry/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.

                IEnumerable<ArticleDto> SelectedArticles = response.Content.ReadAsAsync<IEnumerable<ArticleDto>>().Result;
                ViewModel.Countryarticles = SelectedArticles;


                url = "countrydata/getvariantsforcountry/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Country data transfer object
                IEnumerable<VariantDto> SelectedVariants = response.Content.ReadAsAsync<IEnumerable<VariantDto>>().Result;
                ViewModel.Countryvariants = SelectedVariants;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Country/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Country CountryInfo)
        {
            Debug.WriteLine(CountryInfo.CountryName);
            string url = "Countrydata/addCountry";
            Debug.WriteLine(jss.Serialize(CountryInfo));
            HttpContent content = new StringContent(jss.Serialize(CountryInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Countryid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Countryid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Country/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Country data transfer object
                CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
                return View(SelectedCountry);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Country CountryInfo)
        {
            Debug.WriteLine(CountryInfo.CountryName);
            string url = "countrydata/updatecountry/" + id;
            Debug.WriteLine(jss.Serialize(CountryInfo));
            HttpContent content = new StringContent(jss.Serialize(CountryInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Country/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                //Put data into Country data transfer object
                CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
                return View(SelectedCountry);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Country/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "countrydata/deletecountry/" + id;

            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

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