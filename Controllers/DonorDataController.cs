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
    //This code is mostly scaffolded from the base models and database context
    //New > WebAPIController with Entity Framework Read/Write Actions
    //Choose model "Donation"
    //Choose context "ApplicationDbContext"
    //Note: The base scaffolded code needs many improvements for a fully functioning MVP.

    //database access point
    public class DonorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Gets a list of donors in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of donors including all the values of the columns.</returns>
        /// <example>
        /// GET: api/DonorData/GetDonors
        /// </example>

        // GET: api/DonorData/5
        [ResponseType(typeof(IEnumerable<DonorDto>))]
        public IHttpActionResult GetDonors()
        {
            IEnumerable<Donor> Donors = db.Donors.ToList();
            List<DonorDto> DonorDtos = new List<DonorDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Donor in Donors)
            {
                DonorDto NewDonor = new DonorDto
                {
                    DonorId = Donor.DonorId,
                    Email = Donor.Email,
                    Type = Donor.Type,
                    OrgName = Donor.OrgName,
                    Fname = Donor.Fname,
                    Lname = Donor.Lname,
                    Addressl1 = Donor.Addressl1,
                    Addressl2 = Donor.Addressl2,
                    City = Donor.City,
                    Country = Donor.Country,
                    Province = Donor.Province,
                    PostalCode = Donor.PostalCode
                };
                DonorDtos.Add(NewDonor);
            }
            return Ok(DonorDtos);
        }

        //GET: api/DonorData/FindDonor/5
        [HttpGet]
        [ResponseType(typeof(DonorDto))]
        public IHttpActionResult FindDonor(int id)
        {
            //find donor data
            Donor Donor = db.Donors.Find(id);
            if (Donor == null)
            {
                return NotFound();
            }

            //Put into Dto form
            DonorDto DonorDto = new DonorDto
            {
                DonorId = Donor.DonorId,
                Email = Donor.Email,
                Type = Donor.Type,
                OrgName = Donor.OrgName,
                Fname = Donor.Fname,
                Lname = Donor.Lname,
                Addressl1 = Donor.Addressl1,
                Addressl2 = Donor.Addressl2,
                City = Donor.City,
                Country = Donor.Country,
                Province = Donor.Province,
                PostalCode = Donor.PostalCode
            };
            return Ok(DonorDto);
        }

        /// <summary>
        /// Updates a donation in the database given information about the donation.
        /// </summary>
        /// <param name="id">The donation id</param>
        /// <param name="donation">A donation object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/DonationData/UpdateDonation/5
        /// FORM DATA: Donation JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateDonor(int id, [FromBody] Donor donor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != donor.DonorId)
            {
                return BadRequest();
            }

            db.Entry(donor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonorExists(id))
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
        /// Adds a donor to the database.
        /// </summary>
        /// <param name="donor">A donor object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/DonorData/AddDonor
        /// FORM DATA: Donor JSON Object
        /// </example>
        [ResponseType(typeof(Donor))]
        [HttpPost]
        public IHttpActionResult AddDonor([FromBody] Donor donor)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Donors.Add(donor);
            db.SaveChanges();

            return Ok(donor.DonorId);
        }

        /// <summary>
        /// Deletes a donor in the database
        /// </summary>
        /// <param name="id">The id of the donor to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/DonorData/DeleteDonor/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteDonor(int id)
        {
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return NotFound();
            }

            db.Donors.Remove(donor);
            db.SaveChanges();

            return Ok();
        }

        //what is this for?
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a donor in the system. Internal use only.
        /// </summary>
        /// <param name="id">The donor id</param>
        /// <returns>TRUE if the donor exists, false otherwise.</returns>
        private bool DonorExists(int id)
        {
            return db.Donors.Count(e => e.DonorId == id) > 0;
        }
    }
}