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
    public class DonationController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static DonationController()
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
        
        // GET: Donation
        public ActionResult Index()
        {
            return View();
        }

        // GET: Donation/List
        public ActionResult List()
        {
            string url = "donationdata/getdonations";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DonationDto> SelectedDonations = response.Content.ReadAsAsync<IEnumerable<DonationDto>>().Result;
                return View(SelectedDonations);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        // GET: Donation/Details/5
        public ActionResult Details(int id)
        {
            ShowDonation ViewModel = new ShowDonation();
            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into donation data transfer object
                DonationDto SelectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
                ViewModel.donation = SelectedDonation;


                url = "donationdata/finddonorfordonation/" + id;
                response = client.GetAsync(url).Result;
                DonorDto SelectedDonor = response.Content.ReadAsAsync<DonorDto>().Result;
                ViewModel.donor = SelectedDonor;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Donation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Donation/Create
        [HttpPost]
        //authorized role needed
        public ActionResult Create(Donation DonationInfo)
        {
            Debug.WriteLine(DonationInfo.DonationId);
            string url = "donationdata/adddonation";
            Debug.WriteLine(jss.Serialize(DonationInfo));
            HttpContent content = new StringContent(jss.Serialize(DonationInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                int donationid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = donationid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        
        // GET: Donation/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateDonation ViewModel = new UpdateDonation();

            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into donation data transfer object
                DonationDto SelectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
                ViewModel.donation = SelectedDonation;

                //get information about donors which could make this donation.
                url = "donordata/getdonors";
                response = client.GetAsync(url).Result;
                IEnumerable<DonorDto> PotentialDonors = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;
                ViewModel.alldonors = PotentialDonors;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Donation/Edit/5
        [HttpPost]
        //validate token goes here
        public ActionResult Edit(int id, Donation DonationInfo)
        {
            Debug.WriteLine(DonationInfo.DonationId);
            string url = "donationdata/updatedontaion/" + id;
            Debug.WriteLine(jss.Serialize(DonationInfo));
            HttpContent content = new StringContent(jss.Serialize(DonationInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Donation/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                DonationDto SelectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
                return View(SelectedDonation);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Donation/Delete/5
        [HttpPost]
        //validate token
        public ActionResult Delete(int id)
        {
            string url = "donationdata/deletedonation/" + id;
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
