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
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static CountryController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44334/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

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
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Country data transfer object
                CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
                ViewModel.country = SelectedCountry;

                //We don't need to throw any errors if this is null
                //A country not having any articles is not an issue.
                url = "countrydata/getarticlesforcountry/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<ArticleDto> SelectedArticles = response.Content.ReadAsAsync<IEnumerable<ArticleDto>>().Result;
                ViewModel.countryarticles = SelectedArticles;


                url = "countrydata/getvariantsforcountry/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Country data transfer object
                IEnumerable<VariantDto> SelectedVariants = response.Content.ReadAsAsync<IEnumerable<VariantDto>>().Result;
                ViewModel.countryvariants = SelectedVariants;

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
                return RedirectToAction("Details", new { id = id });
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

        // POST: Country/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "countrydata/deletecountry/" + id;
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