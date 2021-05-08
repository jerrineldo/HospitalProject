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
    public class FaqController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;
        static FaqController()
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
        /// <summary>
        /// Grabs the authentication credentials which are sent to the Controller.
        /// This is NOT considered a proper authentication technique for the WebAPI. It piggybacks the existing authentication set up in the template for Individual faq Accounts. Considering the existing scope and complexity of the course, it works for now.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: Faq
        public ActionResult Index()
        {
            return View();
        }

        // GET: Faq/List
        public ActionResult List()
        {
            string url = "faqdata/getfaqs";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<FaqDto> SelectedFaqs = response.Content.ReadAsAsync<IEnumerable<FaqDto>>().Result;
                return View(SelectedFaqs);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        // GET: Faq/Details/5
        public ActionResult Details(int id)
        {
            ShowFaq ViewModel = new ShowFaq();
            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                //Add this API data to faq DTO
                FaqDto SelectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
                ViewModel.faq = SelectedFaq;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        // only administrators get to this page
        [Authorize(Roles = "admin")]
        // GET: Post/Create
        public ActionResult Create()
        {
            UpdateFaq ViewModel = new UpdateFaq();
            //get information about departments this faq COULD be associated with.
            string url = "departmentsdata/getdepartments";

            HttpResponseMessage response = client.GetAsync(url).Result;
             IEnumerable<DepartmentsDto> PotentialDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;
            ViewModel.departments = PotentialDepartments;


            return View(ViewModel);
        }

        // POST: Faq/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Faq FaqInfo)
        {
            //pass along authentication credential in http request
            GetApplicationCookie();


            string url = "faqdata/addfaq";
            HttpContent content = new StringContent(jss.Serialize(FaqInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                int faq_id = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = faq_id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }





        // GET: Faq/Edit/5
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            UpdateFaq ViewModel = new UpdateFaq();

            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into faq data transfer object
                FaqDto SelectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
                ViewModel.faq = SelectedFaq;

                //get information about departments assoc w/ FAQ's.
                 url = "departmentsdata/getdepartments";
                 response = client.GetAsync(url).Result;
                 IEnumerable<DepartmentsDto> PotentialDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;
                 ViewModel.departments = PotentialDepartments;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Faq/Edit/5
        [HttpPost]
       
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id, Faq FaqInfo)
        {
            //pass along authentication credential in http request
            GetApplicationCookie();

            string url = "faqdata/updatefaq/" + id;
            Debug.WriteLine(jss.Serialize(FaqInfo));
            HttpContent content = new StringContent(jss.Serialize(FaqInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Faq/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "faqdata/findfaq/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into faq data transfer object
                FaqDto SelectedFaq = response.Content.ReadAsAsync<FaqDto>().Result;
                return View(SelectedFaq);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Faq/Delete/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            string url = "faqdata/deletefaq/" + id;
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
