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
using System.Net;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class FeedbackController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static FeedbackController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                UseCookies = false
            };
            client = new HttpClient(handler)
            {
                //change this to match your own local port number
                BaseAddress = new Uri("https://localhost:44349/api/")
            };
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback List" Page. Gathers information from the database.
        /// </summary>
        /// <returns>A dynamic "Feedback List" webpage which provides a list of all blogs in the database.</returns>
        /// <example>GET : /Feedback/List</example>
        // GET: Blog/List
        public ActionResult List()
        {
            string url = "feedbackapi/getfeedback";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<FeedbackDto> SelectedFeedback = response.Content.ReadAsAsync<IEnumerable<FeedbackDto>>().Result;
                return View(SelectedFeedback);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback AdminList" Page. Gathers information from the database.
        /// </summary>
        /// <returns>A dynamic "Feedback List" webpage which provides a list of all blogs in the database and admin options.</returns>
        /// <example>GET : /Feedback/AdminList</example>
        // GET: Blog/List
        [Authorize(Roles = "Admin")]
        public ActionResult AdminList()
        {
            GetApplicationCookie();

            string url = "feedbackapi/getfeedback";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<FeedbackDto> SelectedFeedback = response.Content.ReadAsAsync<IEnumerable<FeedbackDto>>().Result;
                return View(SelectedFeedback);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback Show" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides detailes for a selected blog.</returns>
        /// <example>GET : /Feedback/Show/5</example>
        public ActionResult Show(int id)
        {
            ShowFeedbackViewModel ViewModel = new ShowFeedbackViewModel();
            string url = "feedbackapi/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                FeedbackDto SelectedFeedback = response.Content.ReadAsAsync<FeedbackDto>().Result;
                ViewModel.Feedback = SelectedFeedback;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback AdminShow" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides detailes for a selected Feedback.</returns>
        /// <example>GET : /Feedback/Show/5</example>
        [Authorize(Roles = "Admin")]
        public ActionResult AdminShow(int id)
        {
            GetApplicationCookie();

            ShowFeedbackViewModel ViewModel = new ShowFeedbackViewModel();
            string url = "feedbackapi/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                FeedbackDto SelectedBlogs = response.Content.ReadAsAsync<FeedbackDto>().Result;
                ViewModel.Feedback = SelectedBlogs;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback Create" Page. Adds information from the database.
        /// </summary>
        /// <returns>A dynamic "Feedback Create" webpage which can add a blog entry to the database.</returns>
        /// <example>GET : /Feedback/Create</example>
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            GetApplicationCookie();

            return View();
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback Create" Page. Adds information from the database.
        /// </summary>
        /// <returns>A dynamic "Feedback Create" webpage which can add a blog entry to the database.</returns>
        /// <example>GET : /Feedback/Create</example>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(FeedbackModel FeedbackInfo)
        {
            GetApplicationCookie();

            Debug.WriteLine(FeedbackInfo.title);
            string url = "feedbackapi/addfeedback";
            Debug.WriteLine(jss.Serialize(FeedbackInfo));
            HttpContent content = new StringContent(jss.Serialize(FeedbackInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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

        /// <summary>
        /// Routes to a dynamically generated "Feedback Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides a form to input new blog information.</returns>
        /// <example>GET : /Feedback/Update/5</example>
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id)
        {
            GetApplicationCookie();

            UpdateFeedbackViewModel ViewModel = new UpdateFeedbackViewModel();

            string url = "feedbackapi/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                FeedbackDto SelectedFeedback = response.Content.ReadAsAsync<FeedbackDto>().Result;
                ViewModel.Feedback = SelectedFeedback;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides a form to input new blog information.</returns>
        /// <example>GET : /Feedback/Update/5</example>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, FeedbackDto FeedbackInfo)
        {
            GetApplicationCookie();

            string url = "feedbackapi/updatefeedback/" + id;
            HttpContent content = new StringContent(jss.Serialize(FeedbackInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Show", new { id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        /// <summary>
        /// Routes to a dynamically generated "Feedback Delete" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides information on a blog that can be deleted.</returns>
        /// <example>GET : /Feedback/Delete/5</example>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();

            string url = "feedbackapi/findfeedback/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                FeedbackModel SelectedModule = response.Content.ReadAsAsync<FeedbackModel>().Result;
                return View(SelectedModule);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Feedback Delete" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Feedback</param>
        /// <returns>A dynamic "Feedback Show" webpage which provides information on a blog that can be deleted.</returns>
        /// <example>GET : /Feedback/Delete/5</example>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int id)
        {
            GetApplicationCookie();

            string url = "feedbackapi/deletefeedback/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("AdminList");
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