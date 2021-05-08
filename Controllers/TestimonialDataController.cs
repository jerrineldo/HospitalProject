using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Red_Lake_Hospital_Redesign_Team6.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Diagnostics;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class TestimonialDataController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Finds the details of the testimonial in the database
        /// </summary>
        /// <param name="id">Id of the testimonial to be found</param>
        /// <returns>
        ///     Returns a testimonial object which contains information of the testimonial
        /// </returns>
        /// <example>
        ///     GET : api/TestimonialData/FindTestimonial/1
        /// </example>

        [HttpGet]
        [ResponseType(typeof(TestimonialDto))]
        public IHttpActionResult FindTestimonial(int id)
        {

            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            TestimonialDto NewTestimonial = new TestimonialDto
            {
                testimonial_Id = testimonial.testimonial_Id,
                testimonial_Content = testimonial.testimonial_Content,
                first_Name = testimonial.first_Name,
                last_Name = testimonial.last_Name,
                email = testimonial.email,
                phone_Number = testimonial.phone_Number,
                Has_Pic = testimonial.Has_Pic,
                Pic_Extension = testimonial.Pic_Extension,
                posted_Date = testimonial.posted_Date,
                Approved = testimonial.Approved,
                Id = testimonial.Id

            };
            return Ok(NewTestimonial);
        }

        /// curl -H "Content-Type:application/json" -d @NewTestimonial.json "https://localhost:44349/api/TestimonialData/AddTestimonial/2"
        /// <summary>
        /// Adds a testimonial to a department 
        /// </summary>
        /// <param name="Testimonial">A testimonial object.Sent as POST form data</param>
        /// <param name="DepartmentId">Id of the department to which the testimonial is to be added</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/TestimonialData/AddTestimonial/2
        /// FORM DATA: Testimonial JSON Object
        /// </example>

        [HttpPost]
        [ResponseType(typeof(Testimonial))]
        [Route("api/TestimonialData/AddTestimonial/{DepartmentId}")]
        [Authorize(Roles = "admin,user")]
        public IHttpActionResult AddTestimonial(int DepartmentId, Testimonial Testimonial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DepartmentsModel DepartmentSelected = db.Departments.Find(DepartmentId);

            if (DepartmentSelected == null)
            {
                return NotFound();
            }
            else
            {
                DepartmentSelected.Testimonials = new List<Testimonial>();
                db.Testimonials.Add(Testimonial);
                DepartmentSelected.Testimonials.Add(Testimonial);
                db.SaveChanges();
                return Ok(Testimonial.testimonial_Id);
            }
        }

        /// <summary>
        ///     Updates the information of the testimonial in the database
        /// </summary>
        /// <param name="DepartmentId">Id of the department</param>
        /// <param name="TestimonialId">Id of the testimonial</param>
        /// <param name="Testimonial">Testimonial object which contains the new information of the testimonial</param>
        /// <returns></returns>
        /// <example>
        ///     POST: api/TestimonialData/UpdateTestimonial/2/3
        /// </example>

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("api/TestimonialData/UpdateTestimonial/{DepartmentId}/{TestimonialId}")]
        [Authorize(Roles = "admin,user")]
        public IHttpActionResult UpdateTestimonial(int DepartmentId, int TestimonialId, [FromBody] Testimonial Testimonial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (TestimonialId != Testimonial.testimonial_Id)
            {
                return BadRequest();
            }

            if (!User.IsInRole("admin"))
            {
                if (User.Identity.GetUserId() != Testimonial.Id) return Unauthorized();
            }

            DepartmentsModel DepartmentSelected = db.Departments.Find(DepartmentId);

            if (DepartmentSelected == null)
            {
                return NotFound();
            }

            else
            {
                db.Entry(Testimonial).State = EntityState.Modified;

                db.Entry(Testimonial).Property(t => t.Has_Pic).IsModified = false;
                db.Entry(Testimonial).Property(t => t.Pic_Extension).IsModified = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(TestimonialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return StatusCode(HttpStatusCode.NoContent);

        }

        /// <summary>
        /// Receives testimonial picture data, uploads it to the webserver and updates the testimonial's Has_Pic option
        /// </summary>
        /// <param name="id">the testimonial id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F TestimonialPic=@file.jpg "https://localhost:xx/api/TestimonialData/UpdateTestimonialPic/2"
        /// POST: api/TestimonialData/UpdateTestimonialPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data
        [HttpPost]
        public IHttpActionResult UpdateTestimonialPic(int id)
        {
            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                int numfiles = HttpContext.Current.Request.Files.Count;

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var TestimonialPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (TestimonialPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(TestimonialPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Players/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Testimonials/"), fn);

                                //save the file
                                TestimonialPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the player haspic and picextension fields in the database
                                Testimonial SelectedTestimonial = db.Testimonials.Find(id);


                                SelectedTestimonial.Has_Pic = haspic;
                                SelectedTestimonial.Pic_Extension = extension;
                                db.Entry(SelectedTestimonial).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Player Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }

        /// <summary>
        /// Deletes a testimonial from the database
        /// </summary>
        /// <param name="id">Id of the testimonial to be deleted</param>
        /// <returns>
        ///     Testimonial object which contains the information of the testimonial which was deleted.
        /// </returns>
        /// <example>
        ///     POST: api/TestimonialData/DeleteTestimonial/2
        /// </example>

        [HttpPost]
        [ResponseType(typeof(DepartmentsModel))]
        [Authorize(Roles = "admin,user")]
        public IHttpActionResult DeleteTestimonial(int id)
        {
            Testimonial testimonial = db.Testimonials.Find(id);

            if (testimonial == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("admin"))
            {
                if (User.Identity.GetUserId() != testimonial.Id) return Unauthorized();
            }

            db.Testimonials.Remove(testimonial);
            db.SaveChanges();

            return Ok(testimonial);

        }

        /// <summary>
        ///     Checks if the Testimonial is present in the database.
        /// </summary>
        /// <param name="id">Id of the testimonial</param>
        /// <returns>
        ///     Boolean 1, if the testimonial is present in the database
        ///     Boolean 0, if the testimonial is not present in the database.
        /// </returns>

        private bool TestimonialExists(int id)
        {
            return db.Testimonials.Count(e => e.testimonial_Id == id) > 0;
        }
    }

}