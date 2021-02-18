using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CovidNews.Models;
using CovidNews.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

namespace CovidNews.Controllers
{
    public class VariantController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static VariantController()
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


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }



        // GET: Variant/List
        public ActionResult List()
        {
            string url = "variantdata/getvariants";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<VariantDto> SelectedVariants = response.Content.ReadAsAsync<IEnumerable<VariantDto>>().Result;
                return View(SelectedVariants);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Variant/Details/5
        public ActionResult Details(int id)
        {
            UpdateVariant ViewModel = new UpdateVariant();

            string url = "variantdata/findvariant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Variant data transfer object
                VariantDto SelectedVariant = response.Content.ReadAsAsync<VariantDto>().Result;
                ViewModel.variant = SelectedVariant;

                //find countries that are sponsored by this variant
                url = "variantdata/getcountriesforvariant/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Variant data transfer object
                IEnumerable<CountryDto> SelectedCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                ViewModel.variantincountries = SelectedCountries;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }

        }

        // GET: Variant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Variant/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Variant VariantInfo)
        {
            //Debug.WriteLine(VariantInfo.VariantName);
            string url = "variantdata/addvariant";
            //Debug.WriteLine(jss.Serialize(VariantInfo));
            HttpContent content = new StringContent(jss.Serialize(VariantInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Variantid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Variantid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Variant/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateVariant ViewModel = new UpdateVariant();

            string url = "variantdata/findvariant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Variant data transfer object
                VariantDto SelectedVariant = response.Content.ReadAsAsync<VariantDto>().Result;
                ViewModel.variant = SelectedVariant;

                //find countries that are sponsored by this variant
                url = "variantdata/getcountriesforvariant/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Variant data transfer object
                IEnumerable<CountryDto> SelectedCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                ViewModel.variantincountries = SelectedCountries;

                //find countries that are not sponsored by this variant
                url = "variantdata/getcountrieswithnovariant/" + id;
                response = client.GetAsync(url).Result;

                //put data into data transfer object
                IEnumerable<CountryDto> NoVariantCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                ViewModel.allcountries = NoVariantCountries;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }
        }

        // GET: Variant/Novariant/countryid/variantid
        [HttpGet]
        [Route("Variant/Novariant/{countryid}/{variantid}")]
        public ActionResult Novariant(int countryid, int variantid)
        {
            string url = "variantdata/novariant/" + countryid + "/" + variantid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = variantid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: variant/variant
        // First variant is the noun (the variant themselves)
        // second variant is the verb (the act of sponsoring)
        // The variant(1) variants(2) a country
        [HttpPost]
        [Route("Variant/variant/{countryid}/{variantid}")]
        public ActionResult Variant(int countryid, int variantid)
        {
            string url = "variantdata/variant/" + countryid + "/" + variantid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = variantid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Variant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Variant VariantInfo)
        {
            //Debug.WriteLine(VariantInfo.VariantName);
            string url = "variantdata/updatevariant/" + id;
            //Debug.WriteLine(jss.Serialize(VariantInfo));
            HttpContent content = new StringContent(jss.Serialize(VariantInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                //Debug.WriteLine("update variant request succeeded");
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                //Debug.WriteLine("update variant request failed");
                //Debug.WriteLine(response.StatusCode.ToString());
                return RedirectToAction("Error");
            }
        }

        // GET: Variant/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "variantdata/findvariant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Variant data transfer object
                VariantDto SelectedVariant = response.Content.ReadAsAsync<VariantDto>().Result;
                return View(SelectedVariant);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Variant/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "variantdata/deletevariant/" + id;
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