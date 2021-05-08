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
    public class JobsPostingsApiController : ApiController
    {

        // Create a new database context for interfacing with the db
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly string AUTHKEY = "10b41f57b7cb341ae096a9051849db96c8fd5134"; //Create an authorization key for accessing this API controller. This key is a combination of the server's public key and the client's private key, and then hashed with SHA1





        /// <summary>
        /// The ListJobs method accesses the database to obtain a collection of all current job listings and return the collection.
        /// </summary>
        /// <returns>List of all current job postings</returns>
        /// <param name="key">MD5 key required to access this API end point.</param>
        // GET: api/jobspostingsapi/ListJobs/{key}
        [HttpGet]
        [Route("api/jobspostingsapi/listjobs/{key}/{option?}")]
        [ResponseType(typeof(IEnumerable<JobPostingsViewModel>))]
        public IHttpActionResult ListJobs(string key, string option = "all")
        {

            if (key != AUTHKEY)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            List<JobPostingsModel> allJobsList = new List<JobPostingsModel> { };
                
            
            if (option == "all")
            {
                allJobsList = db.JobPostings.ToList();
            } 
            else if (option == "current")
            {
                allJobsList = db.JobPostings.Where(p => p.PostingExpiryDate > DateTime.Now).ToList();
            }
            else
            {
                return BadRequest();
            }
                
            
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
        // GET: api/jobspostingsapi/JobDetails/5
        [HttpGet]
        [Route("api/jobspostingsapi/jobdetails/{key}/{id}")]
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
        /// The GetAllDepartments method accesses the database to obtain a collection of all current hospital departments and returns the collection.
        /// </summary>
        /// <param name="key">MD5 key required to access this API end point.</param>
        /// <returns>List of all hospital departments.</returns>
        // GET: api/jobspostingsapi/getalldepartments/{key}
        [HttpGet]
        [Route("api/jobspostingsapi/getalldepartments/{key}")]
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
        //POST: api/jobspostingsapi/createposting/{key}
        [HttpPost]
        [Route("api/jobspostingsapi/createposting/{key}")]
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
        //POST: api/jobspostingsapi/editposting/{key}
        [HttpPost]
        [Route("api/jobspostingsapi/editposting/{key}")]
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
        //GET: api/jobspostingsapi/deleteposting/{key}
        [HttpGet]
        [Route("api/jobspostingsapi/deleteposting/{key}/{id}")]
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
        // GET: api/jobspostingsapi/searchpostings/{key}/{searchParam}
        [HttpGet]
        [Route("api/jobspostingsapi/searchpostings/{key}/{searchParam}")]
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