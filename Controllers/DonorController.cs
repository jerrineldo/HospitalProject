using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class DonorController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static DonorController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44349/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
        }

        // GET: Donor
        public ActionResult Index()
        {
            return View();
        }

        // GET: Donor/List
        public ActionResult List()
        {
            string url = "donordata/getdonors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DonorDto> SelectedDonors = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;
                return View(SelectedDonors);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Donor/Details/5
        public ActionResult Details(int? id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into donor data transfer object
                //Q: DO I NEED A VIEWMODEL HERE?
                DonorDto SelectedDonor = response.Content.ReadAsAsync<DonorDto>().Result;

                return View(SelectedDonor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Donor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Donor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Donor DonorInfo)
        {
            Debug.WriteLine(DonorInfo.Fname);
            string url = "donordata/adddonor";
            Debug.WriteLine(jss.Serialize(DonorInfo));
            HttpContent content = new StringContent(jss.Serialize(DonorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (ModelState.IsValid)
            {
                int donorid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = donorid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        
        // GET: Donor/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                DonorDto SelectedDonor = response.Content.ReadAsAsync<DonorDto>().Result;
                return View(SelectedDonor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Donor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Donor DonorInfo)
        {
            Debug.WriteLine(DonorInfo.Fname);
            string url = "donordata/updatedonor/" + id;
            Debug.WriteLine(jss.Serialize(DonorInfo));
            HttpContent content = new StringContent(jss.Serialize(DonorInfo));
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

        // GET: Donor/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                DonorDto SelectedDonor = response.Content.ReadAsAsync<DonorDto>().Result;
                return View(SelectedDonor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Donor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "donordata/deletedonor/" + id;
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
