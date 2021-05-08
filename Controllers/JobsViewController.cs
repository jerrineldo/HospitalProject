using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Numerics;
using EasyEncryption;


namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class JobsViewController : Controller
    {

        // Use an instance of HttpClient class to interface with our APIs and JavaScriptSerializer to parse JSON data passed between the view and API controllers.

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        private static readonly int CLIENTPRIVATEKEY = 8402; //Declare private key for API access

        private static string AUTHKEY;


        static JobsViewController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44349/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            AUTHKEY = Authentication.getAuthKey(CLIENTPRIVATEKEY); //Create the public-private key to access the APIs
            
        }



        /// <summary>
        /// The JobPostingsList method obtains a collection of all current job postings through a call to an API controller, then passes the returned data to the View for display.
        /// </summary>
        /// <returns>A view with all current job postings.</returns>
        // GET: JobsView/JobPostingsList
        [AllowAnonymous]
        public ActionResult JobPostingsList()
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);


            Debug.WriteLine(AUTHKEY);


            string url = $"jobspostingsapi/listjobs/{AUTHKEY}/current"; //Use the "current" option flag in the URL to get only current kob postings from the API; API was designed to accept "current" and "all" as possible flags and will return all job postings if no option flag is provided in the URL
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                List<JobPostingsViewModel> JobsPostingsList = response.Content.ReadAsAsync<List<JobPostingsViewModel>>().Result;


                ////Iterate over the job postings and remove any posting that has already expired so that they will not be passed to the View
                //for (int i = 0; i < JobsPostingsList.Count(); i++ )
                //{
                //    if (JobsPostingsList[i].PostingExpiryDate < DateTime.Now)
                //    {
                //        JobsPostingsList.RemoveAt(i);
                //    }
                //}


                return View(JobsPostingsList);
            }
            else
            {
                return View("Error");
            }
        }



        /// <summary>
        /// The JobPostingDetails method takes in an integer representing the Id of a job positng, and then obtains the data for that posting by making a call to the API controller. It then returns a View containing that data.
        /// </summary>
        /// <param name="id">Id of a job posting.</param>
        /// <returns>A View containing the details of a job posting.</returns>
        // GET: JobsView/JobPostingDetails/5
        [AllowAnonymous]
        [HttpGet]
        public ActionResult JobPostingDetails(int id)
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/jobdetails/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                JobPostingsViewModel JobPosting = response.Content.ReadAsAsync<JobPostingsViewModel>().Result;

                return View(JobPosting);

            }
            else
            {
                return View("Error");
            }

        }


        /// <summary>
        /// The JobApply method obtains the details about a job posting, then renders a view with it which allows a user to apply to that job posting.
        /// </summary>
        /// <param name="id">ID of the job posting that the user wants to apply for</param>
        /// <returns>A view with a form allowing a user to apply to a job</returns>
        // GET: JobsView/JobApply/5
        [AllowAnonymous]
        [HttpGet]
        public ActionResult JobApply(int id) 
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/jobdetails/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                JobPostingsViewModel JobPosting = response.Content.ReadAsAsync<JobPostingsViewModel>().Result;
                //ViewBag.JobPosting = JobPosting;

                //JobApplicationViewModel JobApplicationViewModel = new JobApplicationViewModel();

                return View(JobPosting);

            }
            else
            {
                return View("Error");
            }

        }




        /// <summary>
        /// The postJobApply method expects form data via the POST method relating to a job application plus a PDF resume file. This method verifies the received data and passes it to an API controller to be recorded in the database. 
        /// </summary>
        /// <returns>A view confirming receipt of the job application if successful.</returns>
        // POST: JobsView/postJobApply
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult postJobApply(JobApplicationViewModel JobApplicationViewModel, HttpPostedFileBase newCvPackage)
        {


            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            // Validate incoming form data to ensure that all necessary fields have been completed
            if (JobApplicationViewModel.ApplicantFirstName == ""
                || JobApplicationViewModel.ApplicantFirstName == null
                || JobApplicationViewModel.ApplicantLastName == "" 
                || JobApplicationViewModel.ApplicantLastName == null
                || JobApplicationViewModel.ApplicantEmail == ""
                || JobApplicationViewModel.ApplicantEmail == null
                || JobApplicationViewModel.PostingId <= 0
                || newCvPackage.ContentLength <= 0
                || newCvPackage == null
                || newCvPackage.ContentType != "application/pdf" )
            {
                return View("IncompleteForm");
            }

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string saveCvUrl = $"jobsapplicationsapi/savecvpackage/{AUTHKEY}/{JobApplicationViewModel.PostingId.ToString()}";
            MultipartFormDataContent requestcontent = new MultipartFormDataContent();
            HttpContent fileContent = new StreamContent(newCvPackage.InputStream);
            requestcontent.Add(fileContent, "newCvPackage", newCvPackage.FileName);
            HttpResponseMessage saveCvResponse = client.PostAsync(saveCvUrl, requestcontent).Result;

     
            if (saveCvResponse.IsSuccessStatusCode)
            {

                JobApplicationViewModel.CvPath = saveCvResponse.Content.ReadAsAsync<string>().Result;
                JobApplicationViewModel.ApplicationDate = DateTime.Now;

                //Debug.WriteLine("Saved File Path: " + JobApplicationViewModel.CvPath);


                // Stringify JSON data and send the create request to the API controller using POST method
                string createJobPostingUrl = $"jobsapplicationsapi/createjobapplication/{AUTHKEY}";
                HttpContent content = new StringContent(jss.Serialize(JobApplicationViewModel));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage createJobPostingResponse = client.PostAsync(createJobPostingUrl, content).Result;

                if (createJobPostingResponse.IsSuccessStatusCode)
                {
                    return View(JobApplicationViewModel);
                }
                else
                {
                    return View("Error");
                }

            }
            else
            {
                return View("Error");
            }

        }




        /// <summary>
        /// The adminJobApplicationsList allows an admin user to see all job applications on record.
        /// </summary>
        /// <returns>A view containing all of the job applications on record.</returns>
        /// 
        // GET: JobsView/adminJobApplicationsList
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobApplicationsList()
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobsapplicationsapi/listapplications/{AUTHKEY}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<JobApplicationViewModel> applicationsList = response.Content.ReadAsAsync<IEnumerable<JobApplicationViewModel>>().Result;
                return View(applicationsList);
            }
            else
            {
                return View("Error");
            }

        }




        /// <summary>
        /// The adminJobApplicationEdit renders a view for an admin user to edit an existing job application.
        /// </summary>
        /// <param name="id">ID number of the job application to be edited.</param>
        /// <returns>A view with a form to take in data for changing a job application on record.</returns>
        /// 
        // GET: JobsView/adminJobApplicationEdit/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobApplicationEdit(int id)
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobsapplicationsapi/applicationdetails/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                JobApplicationViewModel applicationViewModel = response.Content.ReadAsAsync<JobApplicationViewModel>().Result;

                return View(applicationViewModel);

            }
            else
            {
                return View("Error");
            }

        }



        /// <summary>
        /// The postAdminApplicationEdit method expects form data via the POST method containg changes to an existing job application entity in the database. The data is passed to an API controller that records the changes in the db.
        /// </summary>
        /// <returns>A view confirming the changes have been made if successful.</returns>
        /// 
        // POST: JobsView/postAdminJobApplicationEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult postAdminApplicationEdit(JobApplicationViewModel editedApplication)
        {

            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            // Validate incoming form data to ensure that all necessary fields have been completed
            if (editedApplication.ApplicantFirstName == ""
                || editedApplication.ApplicantFirstName == null
                || editedApplication.ApplicantLastName == ""
                || editedApplication.ApplicantLastName == null
                || editedApplication.ApplicantEmail == ""
                || editedApplication.ApplicantEmail == null
                || editedApplication.PostingId <= 0
                || Path.GetExtension(editedApplication.CvPath) != ".pdf")
            {
                return View("IncompleteForm");
            }


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobsapplicationsapi/editapplication/{AUTHKEY}";
            HttpContent content = new StringContent(jss.Serialize(editedApplication));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            if (response.IsSuccessStatusCode)
            {

                return View(editedApplication);

            }
            else
            {
                return View("Error");
            }

        }



        /// <summary>
        /// The adminJobApplicationDelete method renders a view to confirm deletion of a job application entity from the db identified by its ID number.
        /// </summary>
        /// <param name="id">ID number of the job application to be deleted.</param>
        /// <returns>A view to confirm deletion of the job application entity.</returns>
        /// 
        // GET: JobsView/adminJobApplicationDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobApplicationDelete(int id)
        {

            ViewData["id"] = id;
            return View();

        }



        /// <summary>
        /// The adminJobApplicationsDeleteConfirmed method deletes a job application entity in the database identified by the ID number by passing the request to an API controller method.
        /// </summary>
        /// <param name="id">ID number of the job application to be deleted.</param>
        /// <returns>A view confirming deletion of the selcted entity</returns>
        /// 
        // GET: JobsView/adminJobApplicationsDeleteConfirmed/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobApplicationsDeleteConfirmed(int id)
        {


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobsapplicationsapi/deleteapplication/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                ViewData["id"] = id;
                return View();

            }
            else
            {
                return View("Error");
            }

        }



        /// <summary>
        /// The adminJobPostingsList renders a view containing all of the job postings on record.
        /// </summary>
        /// <returns>A view containing all of the job postings on record.</returns>
        // GET: /JobsView/adminJobPostingsList
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobPostingsList()
        {

            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/listjobs/{AUTHKEY}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<JobPostingsViewModel> JobsPostingsList = response.Content.ReadAsAsync<IEnumerable<JobPostingsViewModel>>().Result;
                return View(JobsPostingsList);
            }
            else
            {
                return View("Error");
            }
        }




        /// <summary>
        /// The adminJobPostingCreate method renders a form allowing a user to input data for creating a new job posting.
        /// </summary>
        /// <returns>A view with a form allowing a user to input data for creating a new job posting.</returns>
        // GET: JobsView/adminJobPostingCreate
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobPostingCreate()
        {


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/getalldepartments/{AUTHKEY}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DepartmentsModel> allDepartmentsList = response.Content.ReadAsAsync<IEnumerable<DepartmentsModel>>().Result;
                return View(allDepartmentsList);
            }
            else
            {
                return View("Error");
            }

        }



        /// <summary>
        /// The postAdminJobPostingCreate method expects form data via the POST method for creating a new job posting. The data is passed to an API controller for insertion into the database.
        /// </summary>
        /// <returns>A confirmation page if successful</returns>
        /// 
        // POST: JobsView/postAdminJobPostingCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult postAdminJobPostingCreate(JobPostingsViewModel newPostingViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            // Validate incoming form data to ensure that all necessary fields have been completed
            if (newPostingViewModel.PostingExpiryDate == null
                || newPostingViewModel.PostingTitle == null
                || newPostingViewModel.PostingTitle == ""
                || newPostingViewModel.PostingDescription == null
                || newPostingViewModel.PostingDescription == ""
                || newPostingViewModel.DepartmentId <= 0)
            {
                return View("IncompleteForm");
            }


            JobPostingsModel newPostingModel = new JobPostingsModel
            {
                ApplicationUserId = User.Identity.GetUserId(),
                PostingDate = DateTime.Now,
                PostingExpiryDate = newPostingViewModel.PostingExpiryDate,
                PostingTitle = newPostingViewModel.PostingTitle,
                PostingDescription = newPostingViewModel.PostingDescription,
                PostingRemuneration = newPostingViewModel.PostingRemuneration,
                DepartmentId = newPostingViewModel.DepartmentId
            };


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/createposting/{AUTHKEY}";
            HttpContent content = new StringContent(jss.Serialize(newPostingModel));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                return View();

            }
            else
            {
                return View("Error");
            }

        }




        /// <summary>
        /// The adminJobPostingEdit method renders a view with a form for a user to input data to change an existing job posting entity.
        /// </summary>
        /// <param name="id">ID of the job posting to be edited.</param>
        /// <returns>A view with a form for a user to input data to change an existing job posting entity.</returns>
        /// 
        // GET: JobsView/adminJobPostingEdit/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobPostingEdit(int id)
        {


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/jobdetails/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                JobPostingsViewModel postingViewModel = response.Content.ReadAsAsync<JobPostingsViewModel>().Result;

                string departmentsUrl = $"jobspostingsapi/getalldepartments/{AUTHKEY}";
                HttpResponseMessage departmentsResponse = client.GetAsync(departmentsUrl).Result;
                if (departmentsResponse.IsSuccessStatusCode)
                {
                    postingViewModel.DepartmentsList = departmentsResponse.Content.ReadAsAsync<IEnumerable<DepartmentsModel>>().Result;
                    return View(postingViewModel);
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }

        }




        /// <summary>
        /// The postAdminJobPostingEdit method expects form data via the POST method representing the changes to be made to an exisitng job posting. The data is passed to an API controller method which is responsible for making the changes to the database entity.
        /// </summary>
        /// <returns>A view confirming that the changes have been made if successful</returns>
        /// 
        // POST: JobsView/postAdminJobPostingEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult postAdminJobPostingEdit(JobPostingsViewModel editedPostingViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            // Validate incoming form data to ensure that all necessary fields have been completed
            if (editedPostingViewModel.PostingId <= 0
                || editedPostingViewModel.ApplicationUserId == ""
                || editedPostingViewModel.ApplicationUserId == null
                || editedPostingViewModel.PostingExpiryDate == null
                || editedPostingViewModel.PostingTitle == null
                || editedPostingViewModel.PostingTitle == ""
                || editedPostingViewModel.PostingDescription == null
                || editedPostingViewModel.PostingDescription == ""
                || editedPostingViewModel.DepartmentId <= 0)
            {
                return View("IncompleteForm");
            }

            // Update information of user responsible for the job posting with the information of the user who edited the posting
            editedPostingViewModel.ApplicationUserId = User.Identity.GetUserId();



            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/editposting/{AUTHKEY}";
            HttpContent content = new StringContent(jss.Serialize(editedPostingViewModel));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                return View(editedPostingViewModel);

            }
            else
            {
                return View("Error");
            }

        }



        /// <summary>
        /// The adminJobPostingDelete renders a view for the user to confirm deletion of a job posting entity from the database.
        /// </summary>
        /// <param name="id">ID of the job posting to be deleted.</param>
        /// <returns>A view for the user to confirm deletion of a job posting entity from the database.</returns>
        /// 
        // GET: JobsView/adminJobPostingDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobPostingDelete(int id)
        {

            ViewData["id"] = id;
            return View();

        }


        /// <summary>
        /// The adminJobPostingDeleteConfirmed method will delete a job posting entity from the db.
        /// </summary>
        /// <param name="id">ID of the job posting to be deleted.</param>
        /// <returns>A view confirming that the entity has been deleted if successful.</returns>
        /// 
        // GET: JobsView/adminJobPostingDeleteConfirmed/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult adminJobPostingDeleteConfirmed(int id)
        {


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/deleteposting/{AUTHKEY}/{id}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                ViewData["id"] = id;
                return View();
            }
            else
            {
                return View("Error");
            }

        }


        /// <summary>
        /// The jobSearch method renders a view containing all of the job postings with a title or description that contains the user's search query paramters.
        /// </summary>
        /// <param name="searchParam">The user's search query parameters.</param>
        /// <returns>A view containing all of the job postings that match the user's query parameters.</returns>
        //GET: /JobsView/JobSearch?searchParam=receptionist
        [HttpGet]
        [AllowAnonymous]
        public ActionResult jobSearch(string searchParam)
        {


            //Get API authenication key by combining the server's public key with the client's private key and then hashing the result using SHA1
            //string AUTHKEY = getAuthKey(CLIENTPRIVATEKEY);

            string url = $"jobspostingsapi/searchpostings/{AUTHKEY}/{searchParam}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                List<JobPostingsViewModel> searchResults = response.Content.ReadAsAsync<List<JobPostingsViewModel>>().Result;
                return View(searchResults);
            }
            else
            {
                return View("Error");
            }

        }






    }
}