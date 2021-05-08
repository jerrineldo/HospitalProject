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
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class FeedbackApiController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Finds all blogs in the Database.
        /// </summary>
        /// <param name="id">The Blog ID</param>
        /// <returns>All blog objects</returns>
        /// <example>/api/blogapi/getblogs -> {Module Object}</example>
        [ResponseType(typeof(IEnumerable<FeedbackDto>))]
        public IHttpActionResult GetFeedback()
        {
            List<FeedbackModel> Feedbacks = db.Feedback.ToList();
            List<FeedbackDto> FeedbackDtos = new List<FeedbackDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Feedback in Feedbacks)
            {
                FeedbackDto NewFeedback = new FeedbackDto
                {
                    feedback_id = Feedback.feedback_id,
                    date = Feedback.date,
                    fname = Feedback.fname,
                    lname = Feedback.lname,
                    email = Feedback.email,
                    title = Feedback.title,
                    text = Feedback.text
                };
                FeedbackDtos.Add(NewFeedback);
            }
            return Ok(FeedbackDtos);
        }

        /// <summary>
        /// Finds a blog from the Database through an id.
        /// </summary>
        /// <param name="id">The Blog ID</param>
        /// <returns>Blog object containing information about the blog with a matching ID. Empty Blog Object if the ID does not match any Blogs in the system.</returns>
        /// <example>/api/blogapi/findblogs/2 -> {Module Object}</example>
        [ResponseType(typeof(BlogDto))]
        [HttpGet]
        public IHttpActionResult FindFeedback(int id)
        {
            FeedbackModel Feedback = db.Feedback.Find(id);

            if (Feedback == null)
            {
                return NotFound();
            }

            FeedbackDto FeedbackDto = new FeedbackDto
            {
                feedback_id = Feedback.feedback_id,
                date = Feedback.date,
                fname = Feedback.fname,
                lname = Feedback.lname,
                email = Feedback.email,
                title = Feedback.title,
                text = Feedback.text
            };
            return Ok(FeedbackDto);
        }

        /// <summary>
        /// Adds a Blog entry to the blog Database.
        /// </summary>
        /// <param name="AddBlog">An object with fields that map to the columns of the blog's table. </param>
        /// <example>
        /// POST api/BlogApi/AddBlog 
        /// FORM DATA / POST DATA / REQUEST BODY
        /// {
        ///	"blog_id":"1",
        ///	"title":"Covid Updates",
        /// }
        /// </example>
        [HttpPost]
        public IHttpActionResult AddFeedback([FromBody] FeedbackModel feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Feedback.Add(feedback);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = feedback.feedback_id }, feedback);
        }

        /// <summary>
        /// Updates a Blog in the Database.
        /// </summary>
        /// <param name="ModuleInfo">An object with fields that map to the columns of the blog's table.</param>
        /// <example>
        /// POST api/blogapi/updateblog/2
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"blog_id":"2",
        ///	"title":"New Covid Updates",
        /// }
        /// </example>
        [ResponseType(typeof(FeedbackDto))]
        [HttpPost]
        public IHttpActionResult UpdateFeedback(int id, [FromBody] FeedbackModel feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedback.feedback_id)
            {
                return BadRequest();
            }

            db.Entry(feedback).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }



        /// <summary>
        /// Deletes a Blog from the connected Database if the ID of that blog exists.
        /// </summary>
        /// <param name="id">The ID of the blog.</param>
        /// <example>POST /api/blogapi/deleteblog/3</example>
        [HttpPost]
        public IHttpActionResult DeleteFeedback(int id)
        {
            FeedbackModel feedback = db.Feedback.Find(id);
            if (feedback == null)
            {
                return NotFound();
            }

            db.Feedback.Remove(feedback);
            db.SaveChanges();

            return Ok();
        }

        private bool FeedbackExists(int id)
        {
            return db.Feedback.Count(e => e.feedback_id == id) > 0;
        }
    }
}