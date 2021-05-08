using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Diagnostics;
using System.IO;
using System.Web;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using Microsoft.AspNet.Identity;



namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class JobsApiController : ApiController
    {

        // Create a new database context for interfacing with the db
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly string AUTHKEY = "26e554464af61cfccbc87c2beac84002"; //Create an authorization key to secure access between the JobsViewController and the JobsApiContoller; the key is simply the MD5 hashed value of the following string: "Hi Christine"





        /// <summary>
        /// The ListJobs method accesses the database to obtain a collection of all current job listings and return the collection.
        /// </summary>
        /// <returns>List of all current job postings</returns>
        /// <param name="key">MD5 key required to access this API end point.</param>
        // GET: api/JobsApi/ListJobs/{key}
        [HttpGet]
        [Route("api/jobsapi/listjobs/{key}")]
        [ResponseType(typeof(IEnumerable<JobPostingsViewModel>))]
        public IHttpActionResult ListJobs(string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            List<JobPostingsModel> allJobsList = db.JobPostings.ToList();
            List<JobPostingsViewModel> allJobsViewModelList = new List<JobPostingsViewModel> { };

            foreach (var element in allJobsList)
            {
                JobPostingsViewModel jobPost = new JobPostingsViewModel
                {
                    PostingId = element.PostingId,
                    ApplicationUserId = element.ApplicationUserId,
                    ContactEmail = db.Users.Find(element.ApplicationUserId).UserName,
                    PostingDate = element.PostingDate,
                    PostingExpiryDate = element.PostingExpiryDate,
                    PostingTitle = element.PostingTitle,
                    PostingDescription = element.PostingDescription,
                    PostingRemuneration = element.PostingRemuneration,
                    DepartmentId = element.DepartmentId,
                    Department = db.Departments.Find(element.DepartmentId).DepartmentName
                };

                allJobsViewModelList.Add(jobPost);

            }

            return Ok(allJobsViewModelList);
        }


        /// <summary>
        /// The JobDetails method retrieves a single job posting from the database based on the posting's ID number. The job posting is returned.
        /// </summary>
        /// <param name="id">ID number of the job posting to be retrived from the database.</param>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>A single job posting object</returns>
        // GET: api/JobsApi/JobDetails/5
        [HttpGet]
        [Route("api/jobsapi/jobdetails/{key}/{id}")]
        [ResponseType(typeof(JobPostingsViewModel))]
        public IHttpActionResult JobDetails(string key, int id)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            JobPostingsModel JobPosting = new JobPostingsModel();

            JobPosting = db.JobPostings.Find(id);

            JobPostingsViewModel JobPostingViewModel = new JobPostingsViewModel();

            JobPostingViewModel.PostingId = JobPosting.PostingId;
            JobPostingViewModel.ApplicationUserId = JobPosting.ApplicationUserId;
            JobPostingViewModel.ContactEmail = db.Users.Find(JobPosting.ApplicationUserId).UserName;
            JobPostingViewModel.PostingDate = JobPosting.PostingDate;
            JobPostingViewModel.PostingExpiryDate = JobPosting.PostingExpiryDate;
            JobPostingViewModel.PostingTitle = JobPosting.PostingTitle;
            JobPostingViewModel.PostingDescription = JobPosting.PostingDescription;
            JobPostingViewModel.PostingRemuneration = JobPosting.PostingRemuneration;
            JobPostingViewModel.DepartmentId = JobPosting.DepartmentId;
            JobPostingViewModel.Department = db.Departments.Find(JobPosting.DepartmentId).DepartmentName;

            return Ok(JobPostingViewModel);

        }



        /// <summary>
        /// The SaveCvPackage method expects to receive a PDF file in the request body. The method saves this file to the filesystem and then returns the full path for that file.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <param name="postingId">ID number of the posting that the received PDF file relates to.</param>
        /// <returns>Full file path where the received PDF is saved</returns>
        //POST: api/jobsapi/savecvpackage/{key}/{postingid}
        [HttpPost]
        [Route("api/jobsapi/savecvpackage/{key}/{postingid}")]
        public IHttpActionResult SaveCvPackage(string key, string postingId)
        {

            // Validate the authentication key that was provided with the request
            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            else
            {

                if (Request.Content.IsMimeMultipartContent())
                {
                    //Debug.WriteLine("Received multipart form data.");

                    int numfiles = HttpContext.Current.Request.Files.Count;
                    //Debug.WriteLine("Files Received: " + numfiles);

                    //Check if a file is posted
                    if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                    {

                        var newCvPackage = HttpContext.Current.Request.Files[0];
                        //Check if the file is empty
                        if (newCvPackage.ContentLength > 0)
                        {

                            //Create a new filename using a GUID. The view controller code arleady verified that the file is a PDF so we will give it a PDF extension. GUID was chosen for its ease of implementation, but one must be cognizant of potental collision issues in a production enviroment.
                            string newFileName = $"{Guid.NewGuid()}.pdf";

                            //Construct the absolute file path
                            string fullFilePath = Path.Combine(HttpContext.Current.Server.MapPath("~/CvPackages/"), $"PostingID-{postingId}-{newFileName}");


                            try
                            {
                                //Save the image file to disk
                                newCvPackage.SaveAs(fullFilePath);

                                return Ok(fullFilePath);

                            }
                            catch
                            {
                                return StatusCode(HttpStatusCode.InternalServerError);
                            }



                        }
                    }
                }

                return StatusCode(HttpStatusCode.InternalServerError);
            }

        }




        /// <summary>
        /// The createJobApplication method accepts a job application object as an input via the POST method which represents a new job application to be added to the database.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>200 status code if job application was successfully added</returns>
        //POST: api/jobsapi/createJobApplication/{key}
        [HttpPost]
        [Route("api/jobsapi/createjobapplication/{key}")]
        public IHttpActionResult createJobApplication([FromBody] JobApplicationViewModel newApplicationViewModel, string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            //Debug.WriteLine(NewAd.Title);
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            JobApplicationsModel newJobApplication = new JobApplicationsModel
            {
                ApplicantFirstName = newApplicationViewModel.ApplicantFirstName,
                ApplicantLastName = newApplicationViewModel.ApplicantLastName,
                ApplicantEmail = newApplicationViewModel.ApplicantEmail,
                ApplicantPhone = newApplicationViewModel.ApplicantPhone,
                ApplicationDate = newApplicationViewModel.ApplicationDate,
                PostingId = newApplicationViewModel.PostingId,
                CvPath = newApplicationViewModel.CvPath
            };


            try
            {
                db.JobApplications.Add(newJobApplication);
                db.SaveChanges();

                return Ok();
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }




        /// <summary>
        /// The ListApplications method accesses the database to obtain a collection of all current job applications and return the collection.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>List of ad listing objects containing all current ad listings</returns>
        // GET: api/jobsapi/listapplications/{key}
        [HttpGet]
        [Route("api/jobsapi/listapplications/{key}")]
        [ResponseType(typeof(IEnumerable<JobApplicationViewModel>))]
        public IHttpActionResult ListApplications(string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            List<JobApplicationsModel> allApplicationsList = db.JobApplications.OrderBy(a => a.PostingId).ThenByDescending(a => a.ApplicationDate).ToList();
            
            List<JobApplicationViewModel> allApplicationsViewModelList = new List<JobApplicationViewModel> { };

            foreach (var element in allApplicationsList)
            {
                JobApplicationViewModel jobApplication = new JobApplicationViewModel
                {
                    ApplicationId = element.ApplicationId,
                    PostingId = element.PostingId,
                    ApplicationDate = element.ApplicationDate,
                    ApplicantFirstName = element.ApplicantFirstName,
                    ApplicantLastName = element.ApplicantLastName,
                    ApplicantEmail = element.ApplicantEmail,
                    ApplicantPhone = element.ApplicantPhone == null || element.ApplicantPhone == "" ? "N/A" : element.ApplicantPhone,
                    CvPath = element.CvPath
                };

                allApplicationsViewModelList.Add(jobApplication);

            }

            return Ok(allApplicationsViewModelList);
        }




        /// <summary>
        /// The ApplicationDetails method accesses the database to obtain the particulars of one job application and returns the data in the form of an object.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <param name="id">ID of the job application being requested.</param>
        /// <returns>An object containing the particulars of a job application</returns>
        // GET: api/JobsApi/ApplicationDetails/{key}/{id}
        [HttpGet]
        [Route("api/jobsapi/applicationdetails/{key}/{id}")]
        [ResponseType(typeof(JobApplicationViewModel))]
        public IHttpActionResult ApplicationDetails(string key, int id)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            JobApplicationsModel applicationModel = new JobApplicationsModel();

            applicationModel = db.JobApplications.Find(id);

            JobApplicationViewModel applicationViewModel = new JobApplicationViewModel();

            applicationViewModel.ApplicationId = applicationModel.ApplicationId;
            applicationViewModel.PostingId = applicationModel.PostingId;
            applicationViewModel.ApplicationDate = applicationModel.ApplicationDate;
            applicationViewModel.ApplicantFirstName = applicationModel.ApplicantFirstName;
            applicationViewModel.ApplicantLastName = applicationModel.ApplicantLastName;
            applicationViewModel.ApplicantEmail = applicationModel.ApplicantEmail;
            applicationViewModel.ApplicantPhone = applicationModel.ApplicantPhone;
            applicationViewModel.CvPath = applicationModel.CvPath;


            return Ok(applicationViewModel);
        }



        /// <summary>
        /// The editApplication method accepts a JobApplicationViewModel object as an input via the POST method which represents the new data to be changed on an existing job application entity in the database.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>200 status code if ad listing was successfully added</returns>
        //POST: api/jobsapi/editapplication/{key}
        [HttpPost]
        [Route("api/jobsapi/editapplication/{key}")]
        public IHttpActionResult editApplication([FromBody] JobApplicationViewModel editedApplicationViewModel, string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            JobApplicationsModel targetJobApplicationModel = db.JobApplications.Find(editedApplicationViewModel.ApplicationId);


            targetJobApplicationModel.ApplicantFirstName = editedApplicationViewModel.ApplicantFirstName;
            targetJobApplicationModel.ApplicantLastName = editedApplicationViewModel.ApplicantLastName;
            targetJobApplicationModel.ApplicantEmail = editedApplicationViewModel.ApplicantEmail;
            targetJobApplicationModel.ApplicantPhone = editedApplicationViewModel.ApplicantPhone;
            targetJobApplicationModel.ApplicationDate = editedApplicationViewModel.ApplicationDate;
            targetJobApplicationModel.PostingId = editedApplicationViewModel.PostingId;
            targetJobApplicationModel.CvPath = editedApplicationViewModel.CvPath;


            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }




        /// <summary>
        /// The deleteApplication method deletes a job application entity from the database identified by its ID number.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <param name="id">ID number of the job application to be deleted.</param>
        /// <returns>200 status code if ad listing was successfully added</returns>
        //GET: api/jobsapi/deleteapplication/{key}
        [HttpGet]
        [Route("api/jobsapi/deleteapplication/{key}/{id}")]
        public IHttpActionResult deleteApplication(string key, int id)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            JobApplicationsModel targetJobApplicationModel = db.JobApplications.Find(id);

            // Check that the type exists in the database
            if (targetJobApplicationModel == null)
            {
                return NotFound();
            }


            try
            {
                // Delete the cover letter and resume attached to the job application if it exists
                if (File.Exists(targetJobApplicationModel.CvPath))
                {
                    File.Delete(targetJobApplicationModel.CvPath);
                }
                
                db.JobApplications.Remove(targetJobApplicationModel);
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }




        /// <summary>
        /// The GetAllDepartments method accesses the database to obtain a collection of all current hospital departments and returns the collection.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>List of all hospital departments.</returns>
        // GET: api/jobsapi/getalldepartments/{key}
        [HttpGet]
        [Route("api/jobsapi/getalldepartments/{key}")]
        [ResponseType(typeof(IEnumerable<DepartmentsModel>))]
        public IHttpActionResult GetAllDepartments(string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            List<DepartmentsModel> allDepartmentsList = db.Departments.OrderBy(d => d.DepartmentName).ToList();

            return Ok(allDepartmentsList);
        }




        /// <summary>
        /// The createPosting method accepts a JobPostingsModel object as an input via the POST method which represents a new job posting to be added to the database.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>200 status code if ad listing was successfully added</returns>
        //POST: api/jobsapi/createposting/{key}
        [HttpPost]
        [Route("api/jobsapi/createposting/{key}")]
        public IHttpActionResult createPosting([FromBody] JobPostingsModel newPosting, string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            db.JobPostings.Add(newPosting);
            db.SaveChanges();

            return Ok();


        }




        /// <summary>
        /// The editPosting method accepts a JobPostingsViewModel object as an input via the POST method which represents the new data to be inserted into a job posting already existing in the database.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>200 status code if ad listing was successfully added</returns>
        //POST: api/jobsapi/editposting/{key}
        [HttpPost]
        [Route("api/jobsapi/editposting/{key}")]
        public IHttpActionResult editPosting([FromBody] JobPostingsViewModel editedPostingViewModel, string key)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            JobPostingsModel targetPostingModel = db.JobPostings.Find(editedPostingViewModel.PostingId);


            targetPostingModel.PostingTitle = editedPostingViewModel.PostingTitle;
            targetPostingModel.PostingDescription = editedPostingViewModel.PostingDescription;
            targetPostingModel.PostingRemuneration = editedPostingViewModel.PostingRemuneration;
            targetPostingModel.PostingDate = editedPostingViewModel.PostingDate;
            targetPostingModel.PostingExpiryDate = editedPostingViewModel.PostingExpiryDate;
            targetPostingModel.DepartmentId = editedPostingViewModel.DepartmentId;
            targetPostingModel.ApplicationUserId = editedPostingViewModel.ApplicationUserId;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }



        /// <summary>
        /// The deletePosting deletes a job posting in the database that is identified by ID.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <param name="id">ID number of the job posting to be deleted.</param>
        /// <returns>200 status code if ad listing was successfully added</returns>
        //GET: api/jobsapi/deleteposting/{key}
        [HttpGet]
        [Route("api/jobsapi/deleteposting/{key}/{id}")]
        public IHttpActionResult deletePosting(string key, int id)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }


            JobPostingsModel targetJobPostingModel = db.JobPostings.Find(id);

            // Check that the type exists in the database
            if (targetJobPostingModel == null)
            {
                return NotFound();
            }


            try
            {
                db.JobPostings.Remove(targetJobPostingModel);
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }




        /// <summary>
        /// The searchPostings method accesses the database to obtain a collection of all current job postings where the title or desciption contains the query parameter, and then returns the collection.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <param name="searchParam">The search parameter string</param>
        /// <returns>List of all job postings matching the query parameter string</returns>
        // GET: api/jobsapi/searchpostings/{key}/{searchParam}
        [HttpGet]
        [Route("api/jobsapi/searchpostings/{key}/{searchParam}")]
        [ResponseType(typeof(IEnumerable<JobPostingsViewModel>))]
        public IHttpActionResult searchPostings(string key, string searchParam)
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            // Look for entities with titles or descriptions that contain the search parameter, then filter out entities with an expiry date that has already elapsed, and sort the results by the expiry dates
            List<JobPostingsModel> searchedJobsList = db.JobPostings.Where(p => p.PostingTitle.Contains(searchParam) || p.PostingDescription.Contains(searchParam)).Where(p => p.PostingExpiryDate > DateTime.Now).OrderBy(p => p.PostingExpiryDate).ToList();
            List<JobPostingsViewModel> searchedJobsViewModelList = new List<JobPostingsViewModel> { };

            foreach (var element in searchedJobsList)
            {
                JobPostingsViewModel jobPost = new JobPostingsViewModel
                {
                    PostingId = element.PostingId,
                    ApplicationUserId = element.ApplicationUserId,
                    ContactEmail = db.Users.Find(element.ApplicationUserId).UserName,
                    PostingDate = element.PostingDate,
                    PostingExpiryDate = element.PostingExpiryDate,
                    PostingTitle = element.PostingTitle,
                    PostingDescription = element.PostingDescription,
                    PostingRemuneration = element.PostingRemuneration,
                    DepartmentId = element.DepartmentId,
                    Department = db.Departments.Find(element.DepartmentId).DepartmentName
                };

                searchedJobsViewModelList.Add(jobPost);

            }

            return Ok(searchedJobsViewModelList);
        }












    }
}