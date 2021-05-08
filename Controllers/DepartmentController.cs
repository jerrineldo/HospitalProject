using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class DepartmentController : Controller
    {

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static DepartmentController()
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
        /// Lists all the departments in the database
        /// </summary>
        /// <returns>
        ///     If successfull in retreiving the data from the database, then returns the data to the view
        ///     If not -> Calls the Error Action to invoke the error message.
        /// </returns>
        /// <example>
        /// GET: Department/ListDepartments
        /// </example>
        public ActionResult ListDepartments(int PageNum = 0)
        {
            ShowDepartment ViewModel = new ShowDepartment();
            ViewModel.isadmin = User.IsInRole("admin");

            string url = "DepartmentData/ListDepartments";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DepartmentsDto> ListOfDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;

                // -- Start of Pagination Algorithm --

                // Find the total number of players
                int DepartmentCount = ListOfDepartments.Count();
                // Number of players to display per page
                int PerPage = 4;
                // Determines the maximum number of pages (rounded up), assuming a page 0 start.
                int MaxPage = (int)Math.Ceiling((decimal)DepartmentCount / PerPage) - 1;

                // Lower boundary for Max Page
                if (MaxPage < 0) MaxPage = 0;
                // Lower boundary for Page Number
                if (PageNum < 0) PageNum = 0;
                // Upper Bound for Page Number
                if (PageNum > MaxPage) PageNum = MaxPage;

                // The Record Index of our Page Start
                int StartIndex = PerPage * PageNum;

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = PageNum;
                ViewData["PageSummary"] = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

                // -- End of Pagination Algorithm --

                url = "DepartmentData/getdepartmentspage/" + StartIndex + "/" + PerPage;
                response = client.GetAsync(url).Result;

                IEnumerable<DepartmentsDto> SelectedDepartmentsPage = response.Content.ReadAsAsync<IEnumerable<DepartmentsDto>>().Result;

                ViewModel.departments = SelectedDepartmentsPage;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        ///  Renders the view for creating a new Department
        /// </summary>
        /// <returns>Returns a view for creating a new Department</returns>
        /// <example>
        /// GET : Department/Create
        /// </example>

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a department in the database
        /// </summary>
        /// <param name="DepartmentInfo">Information on the new department to be entered into the database.</param>
        /// <returns>
        ///     If successfully connected to the api, the new department would be created in the database, and the control
        ///     is redirected to the action "ListDepartments" which displayes all the departments in the database
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     POST : Department/Create/{FORM Data}
        /// </example>

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(DepartmentsModel DepartmentInfo)
        {
            GetApplicationCookie();
            string url = "DepartmentData/AddDepartment";
            HttpContent content = new StringContent(jss.Serialize(DepartmentInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListDepartments");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Renders the view to update a Department
        /// </summary>
        /// <param name="id">Id of the department to be updated</param>
        /// <returns>
        ///     If successfull , returns a view which contains details about the department 
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Department/Update/1
        /// </example>

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Update(int id)
        {
            GetApplicationCookie();
            string url = "DepartmentData/FindDepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                DepartmentsDto UpdatedDepartment = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                return View(UpdatedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the information of a department in the database.
        /// </summary>
        /// <param name="DepartmentInfo">DepartmentsModel object which contains information of the department</param>
        /// <returns>
        ///     If connection to the api is successfull, updates the contents of the existing department and the control is 
        ///     redirected to the Action "ListDepartments" which renders the view to display all the departments
        ///     If not successfull, then an error message is thrown. 
        /// </returns>
        /// <example>
        ///     POST: Department/Update/2/{Form Data}
        /// </example>

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Update(int id, DepartmentsModel DepartmentInfo)
        {
            GetApplicationCookie();
            string url = "DepartmentData/UpdateDepartment/" + id;
            HttpContent content = new StringContent(jss.Serialize(DepartmentInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListDepartments");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Asks the user to confirm before deleting the department from the database permanently. Shows the information about the department   
        /// </summary>
        /// <param name="id">Id of the Department to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the department to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Department/DeleteConfirm/5
        /// </example>

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "DepartmentData/FindDepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                DepartmentsDto Department = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                return View(Department);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///     Deletes the department from the database
        /// </summary>
        /// <param name="id">Id of the Department to be deleted.</param>
        /// <returns>
        ///     If successfull, deletes the testimonial from the database  and the control is 
        ///     redirected to the Action "ListDepartments" which renders the view to display all the departments
        /// </returns>
        /// <example>
        ///     POST: Department/Delete/5
        /// </example>

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "DepartmentData/DeleteDepartment/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListDepartments");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Displays the details of the department including the testimonials for the department
        /// </summary>
        /// <param name="Id">Id of the department</param>
        /// <returns>
        ///     If successfull in fetching the data from the api controller, returns a "DepartmentDetails" View Model
        ///     which contains information on the details of the department and the testimonials of the department.
        /// </returns>
        /// <example>
        ///     GET: Department/DetailsofDepartment/5
        /// </example>

        public ActionResult DetailsofDepartment(int Id)
        {
            DepartmentDetails departmentdetails = new DepartmentDetails();
            departmentdetails.isadmin = User.IsInRole("admin");
            departmentdetails.isloggedinUser = User.IsInRole("user");
            departmentdetails.userid = User.Identity.GetUserId();

            string url = "DepartmentData/FindDepartment/" + Id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                DepartmentsDto department = response.Content.ReadAsAsync<DepartmentsDto>().Result;
                departmentdetails.DepartmentDto = department;

                url = "DepartmentData/FindTestimonialsForDepartment/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<TestimonialDto> testimonials = response.Content.ReadAsAsync<IEnumerable<TestimonialDto>>().Result;
                departmentdetails.Testimonials = testimonials;

                url = "DepartmentData/FindJobPostingsforDepartment/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<JobPostingsModel> jobPostings = response.Content.ReadAsAsync<IEnumerable<JobPostingsModel>>().Result;
                departmentdetails.JobPostings = jobPostings;

                return View(departmentdetails);
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