using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class TestimonialController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static TestimonialController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = false,
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44349//api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets the authentication credentials which are sent to the controller.
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

        /// <summary>
        ///  Renders the view for creating a new Testimonial
        /// </summary>
        /// <returns>Returns a view for creating a new Testimonial</returns>
        /// <example>
        /// GET : Testimonial/Create/2
        /// </example>

        [Authorize(Roles = "admin,user")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            string url = "DepartmentData/FindDepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Id = id;
                return View();
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Creates a new Testimonial in the database
        /// </summary>
        /// <param name="TestimonialInfo">Form data which has the details for the new Testimonial to be entered into the database</param>
        /// <returns>
        ///     If successfull , A new Testimonial would be entered into the database.
        ///     If not , then an error would be thrown 
        /// </returns>
        /// <example>
        /// POST: Testimonial/Create/1
        /// </example>

        [HttpPost]
        [Route("Testimonial/Create/{DepartmentId}")]
        [Authorize(Roles = "admin,user")]
        public ActionResult Create(int id, Testimonial TestimonialInfo, HttpPostedFileBase TestimonialPic)
        {
            GetApplicationCookie();
            TestimonialInfo.Id = User.Identity.GetUserId();
            string url = "TestimonialData/AddTestimonial/" + id;
            HttpContent content = new StringContent(jss.Serialize(TestimonialInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                int Id = response.Content.ReadAsAsync<int>().Result;
                //only attempt to send Movie picture data if we have it
                if (TestimonialPic != null)
                {
                    //Send over image data for movie
                    url = "TestimonialData/UpdateTestimonialPic/" + Id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(TestimonialPic.InputStream);
                    requestcontent.Add(imagecontent, "TestimonialPic", TestimonialPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                return RedirectToAction("DetailsofDepartment", "Department", new { Id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Renders the view to update a testimonial
        /// </summary>
        /// <param name="TestimonialId">Id of the testimonial to be updated</param>
        /// <param name="DepartmentId">Id of the department of which the testimonial is a part of.</param>
        /// <returns>
        ///     If successfull , returns a ViewModel which contains details about the testimonial and the department 
        ///     of which the testimonial is a part of.
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Testimonial/UpdateTestimonial/1/2
        /// </example>

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public ActionResult UpdateTestimonial(int TestimonialId, int DepartmentId)
        {
            TestimonialDetails Testimonial_Details = new TestimonialDetails();

            string url = "TestimonialData/FindTestimonial/" + TestimonialId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                TestimonialDto testimonial = response.Content.ReadAsAsync<TestimonialDto>().Result;
                Testimonial_Details.TestimonialDto = testimonial;

                url = "DepartmentData/FindDepartment/" + DepartmentId;
                response = client.GetAsync(url).Result;
                DepartmentsDto department = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                Testimonial_Details.DepartmentDto = department;

                return View(Testimonial_Details);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the information of a testimonial in the database.
        /// </summary>
        /// <param name="TestimonialDetails">View Model object which contains information of the testimonial 
        /// and the department</param>
        /// <returns>
        ///     If connection to the api is successfull, updates the contents of the existing testimonial and the control is 
        ///     redirected to the Action "DetailsofDepartment" which renders the view to display the details of the department
        ///     If not successfull, then an error message is thrown. 
        /// </returns>
        /// <example>
        ///     POST: Testimonial/UpdateTestimonial/{Form Data}
        /// </example>

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public ActionResult UpdateTestimonial(TestimonialDetails TestimonialDetails, HttpPostedFileBase TestimonialPic)
        {
            GetApplicationCookie();

            string url = "TestimonialData/UpdateTestimonial/" + TestimonialDetails.DepartmentDto.DepartmentId + "/" + TestimonialDetails.TestimonialDto.testimonial_Id;
            HttpContent content = new StringContent(jss.Serialize(TestimonialDetails.TestimonialDto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                if (TestimonialPic != null)
                {
                    //Send over image data for movie
                    url = "TestimonialData/UpdateTestimonialPic/" + TestimonialDetails.TestimonialDto.testimonial_Id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(TestimonialPic.InputStream);
                    requestcontent.Add(imagecontent, "TestimonialPic", TestimonialPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                return RedirectToAction("DetailsofDepartment", "Department", new { Id = TestimonialDetails.DepartmentDto.DepartmentId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Asks the user to confirm before deleting the Testimonial from the database permanently. Shows the information about the testimonial   
        /// </summary>
        /// <param name="DepartmentId">Id of the Department to be deleted.</param>
        /// <param name="TestimonialId">Id of the Testimonial to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the Testimonial to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Testimonial/DeleteConfirm/5/2
        /// </example>

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public ActionResult DeleteConfirm(int DepartmentId, int TestimonialId)
        {
            TestimonialDetails Testimonial_Details = new TestimonialDetails();
            string url = "TestimonialData/FindTestimonial/" + TestimonialId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                TestimonialDto Testimonial = response.Content.ReadAsAsync<TestimonialDto>().Result;
                Testimonial_Details.TestimonialDto = Testimonial;

                url = "DepartmentData/FindDepartment/" + DepartmentId;
                response = client.GetAsync(url).Result;

                DepartmentsDto Department = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                Testimonial_Details.DepartmentDto = Department;

                return View(Testimonial_Details);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///     Deletes the testimonial from the database
        /// </summary>
        /// <param name="TestimonialId">Id of the Testimonial to be deleted.</param>
        /// <param name="DepartmentId">Id of the Department to be deleted.</param>
        /// <returns>
        ///     If successfull, deletes the testimonial from the database  and the control is redirected to the 
        ///     Action "DetailsofDepartment" which renders the view to display the details of the department
        /// </returns>
        /// <example>
        ///     POST: Testimonial/DeleteTestimonial/5/2
        /// </example>

        [HttpPost]
        [Route("Testimonial/DeleteTestimonial/{TestimonialId}/{DepartmentId}")]
        [Authorize(Roles = "admin,user")]
        public ActionResult DeleteTestimonial(int TestimonialId, int DepartmentId)
        {
            GetApplicationCookie();
            string url = "TestimonialData/DeleteTestimonial/" + TestimonialId;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("DetailsofDepartment", "Department", new { Id = DepartmentId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Renders the view to display an error 
        /// </summary>
        /// <returns>
        /// </returns>

        public ActionResult Error()
        {
            return View();
        }

    }
}