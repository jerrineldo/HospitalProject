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
    public class ContactsController : Controller
    {

        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static ContactsController()
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
        /// This is NOT considered a proper authentication technique for the WebAPI. It piggybacks the existing authentication set up in the template for Individual User Accounts. Considering the existing scope and complexity of the course, it works for now.
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

        // GET: Contacts
        public ActionResult Index()
        {
            return View();
        }

        //GET: Contacts/List
        public ActionResult List()
        {

            // Grab all contacts
            string url = "contactsdata/getcontacts";

            // Send off an HTTP request
            // GET : /api/contactsdata/getcontacts
            // Retrieve response
            HttpResponseMessage response = client.GetAsync(url).Result;

            // If the response is a success, proceed
            if (response.IsSuccessStatusCode)
            {
                // Fetch the response content into IEnumerable<ContactDto>
                IEnumerable<ContactDto> SelectedContacts = response.Content.ReadAsAsync<IEnumerable<ContactDto>>().Result;

                //Return the list of contacts
                return View(SelectedContacts);
            }
            else
            {
                // If we reach here something went wrong with our list algorithm
                return RedirectToAction("Error");
            }
        }


        // GET: Contacts/Details/{key}
        public ActionResult Details(int id)
        {

            ShowContact ViewModel = new ShowContact();

            //Pass along to the view information about who is logged in
            ViewModel.isadmin = User.IsInRole("Admin");



            string url = "contactsdata/findcontact/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into contact data transfer object
                ContactDto SelectedContact = response.Content.ReadAsAsync<ContactDto>().Result;
                ViewModel.contact = SelectedContact;


                url = "contactsdata/finddepartmentforcontact/" + id;
                response = client.GetAsync(url).Result;
                DepartmentsDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                ViewModel.departments = SelectedDepartment;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Contact/Create
        // only administrators get to this page
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            UpdateContact ViewModel = new UpdateContact();
            //get information about department this person COULD contact.
            string url = "contactsdata/getcontacts";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DepartmentsDto> PotentialContacts = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;
            ViewModel.alldepartments = PotentialContacts;

            return View(ViewModel);
        }


        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Contact ContactInfo)
        {
            //pass along authentication credential in http request
            GetApplicationCookie();

            //Debug.WriteLine(ContactInfo.ContactFirstName);
            //Debug.WriteLine(jss.Serialize(ContactInfo));
            string url = "contactsdata/addcontact";
            HttpContent content = new StringContent(jss.Serialize(ContactInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                int contactid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = contactid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        // POST: Contacts/Edit/{key}
        public ActionResult Edit(int id)
        {
            UpdateContact ViewModel = new UpdateContact();

            string url = "contactsdata/findcontact" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                //Put data into contact data transfer object
                ContactDto SelectedContact = response.Content.ReadAsAsync<ContactDto>().Result;
                ViewModel.contact = SelectedContact;

                //get information about departments this contact belongs to.
                url = "departmentsdata/getdepartments";
                response = client.GetAsync(url).Result;
                IEnumerable<DepartmentsDto> PotentialContact = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;
                ViewModel.alldepartments = PotentialContact;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
            
        }

        // GET: Player/Delete/{key}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "contactsdata/findcontact/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into contact data transfer object
                ContactDto SelectedContact = response.Content.ReadAsAsync<ContactDto>().Result;
                return View(SelectedContact);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        // POST: Player/Delete/5
        [HttpGet]
        //[ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "contactsdata/deletecontact/" + id;
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
