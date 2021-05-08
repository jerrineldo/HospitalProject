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
using System.Diagnostics;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class EcardsDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// The GetEcards method accesses the database to obtain a collection of all ecards listings and return the collection.
        /// </summary>
        /// <returns>List of all ecards </returns>
        /// <param name="key">MD5 key required to access this API end point.</param>
        // GET: api/EcardsData/List/{key}
        // GET: api/EcardsData/{key}
        [HttpGet]
        [Route("api/ecardsdata/getecards")]
        [ResponseType(typeof(IEnumerable<EcardsDto>))]
        public IHttpActionResult GetEcards()
        {
            List<Ecards> Ecards = db.Ecards.ToList();
            List<EcardsDto> EcardsDtos = new List<EcardsDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Ecard in Ecards)
            {
                EcardsDto NewEcard = new EcardsDto
                {
                    ecard_id = Ecard.ecard_id,
                    photo_path = Ecard.photo_path,
                    message = Ecard.message,
                    DepartmentName = Ecard.Department.DepartmentName
                };
                EcardsDtos.Add(NewEcard);
            }

            return Ok(EcardsDtos);
        }

        /// <summary>
        /// Finds a particular ecard in the database with a 200 status code. If the player is not found, return 404.
        /// </summary>
        /// <param name="id">The ecard id</param>
        /// <returns>Information about the ecard, including ecard id, photo , message and departmentname</returns>
        // <example>
        // GET: api/EcardsData/FindEcard/{key}
        [HttpGet]
        [ResponseType(typeof(EcardsDto))]
        public IHttpActionResult FindEcard(int id)
        {
            //Find the data
            Ecards ecard = db.Ecards.Find(id);
            //if not found, return 404 status code.
            if (ecard == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            EcardsDto EcardsDtos = new EcardsDto
            {
                ecard_id= ecard.ecard_id,
                photo_path = ecard.photo_path,
                message = ecard.message,
                DepartmentName = ecard.Department.DepartmentName
            };


            //pass along data as 200 status code OK response
            return Ok(EcardsDtos);
        }


        /// <summary>
        /// Finds a particular Department in the database given a contact id with a 200 status code. If the Department is not found, return 404.
        /// </summary>
        /// <param name="id">The ecard id</param>
        /// <returns>Information about the Department, including Department id and Department Name</returns>
        // <example>
        // GET: api/DepartmentData/FindDepartmentForEcard/{key}
        [HttpGet]
        [ResponseType(typeof(DepartmentsDto))]
        public IHttpActionResult FindDepartmentForEcard(int id)
        {
            DepartmentsModel departments = db.Departments
                .Where(d => d.Ecards.Any(e => e.ecard_id == id))
                .FirstOrDefault();
            //if not found, return 404 status code.
            if (departments == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            DepartmentsDto DepartmentsDtos = new DepartmentsDto
            {
                DepartmentId = departments.DepartmentId,
                DepartmentName = departments.DepartmentName
            };


            //pass along data as 200 status code OK response
            return Ok(DepartmentsDtos);
        }

        /// <summary>
        /// Updates an ecard in the database given information about the ecard.
        /// </summary>
        /// <param name="id">The ecard id</param>
        /// <param name="player">A ecard object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/EcardsData/UpdateEcard/{key}
        /// FORM DATA: Ecard JSON Object
        // PUT: api/EcardsData/UpdateEcard{key}
        [HttpPost]
        [Route("api/ecardsdata/updateecards")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateEcard(int id, [FromBody] Ecards ecards)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ecards.ecard_id)
            {
                return BadRequest();
            }


            db.Entry(ecards).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EcardsExists(id))
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
        /// Adds an ecard to the database.
        /// </summary>
        /// <param name="player">An ecard object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/EcardsData/AddEcard
        ///  FORM DATA: Ecard JSON Object
        [ResponseType(typeof(Ecards))]
        [HttpPost]
        [Route("api/ecardsdata/addecard")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddEcard([FromBody] Ecards ecard)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ecards.Add(ecard);
            db.SaveChanges();

            return Ok(ecard.ecard_id);
        }


        /// <summary>
        /// Deletes an ecard in the database
        /// </summary>
        /// <param name="id">The id of an ecard to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/EcardsData/DeleteEcard/{key}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteEcard(int id)
        {
            Ecards ecards = db.Ecards.Find(id);
            if (ecards == null)
            {
                return NotFound();
            }

            db.Ecards.Remove(ecards);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a contact in the system. Internal use only.
        /// </summary>
        /// <param name="id">The contact id</param>
        /// <returns>TRUE if the contact exists, false otherwise.</returns>
        private bool EcardsExists(int id)
        {
            return db.Ecards.Count(e => e.ecard_id == id) > 0;
        }
    }
}