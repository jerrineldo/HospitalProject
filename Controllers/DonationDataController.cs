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
    public class DonationDataController : ApiController
    {
        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Donation"
        //Choose context "ApplicationDbContext"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.

        //database access point
        private ApplicationDbContext db = new ApplicationDbContext();


        /* GetAllDonations FROM MS SAMPLE CODE
        // GET: api/DonationData
        public IQueryable<Donation> GetDonations()
        {
            return db.Donations;
        }
        */

        /// <summary>
        /// Gets a list of donations in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of donations including all the values of the columns.</returns>
        /// <example>
        /// GET: api/DonationData/GetDonations
        /// </example>

        [ResponseType(typeof(IEnumerable<DonationDto>))]
        public IHttpActionResult GetDonations()
        {
            //List<Donation> Donations = db.Donations.ToList();
            IEnumerable<Donation> Donations = db.Donations.ToList();
            List<DonationDto> DonationDtos = new List<DonationDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Donation in Donations)
            {
                DonationDto NewDonation = new DonationDto
                {
                    DonationId = Donation.DonationId,
                    Date = Donation.Date,
                    Amount = Donation.Amount,
                    OneTime = Donation.OneTime,
                    Msg = Donation.Msg,
                    Dedication = Donation.Dedication,
                    DedicateName = Donation.DedicateName,
                    Action = Donation.Action,
                    Anonymity = Donation.Anonymity,
                    PaymentMethod = Donation.PaymentMethod,
                    PaymentNumber = Donation.PaymentNumber,
                    DonorId = Donation.DonorId
                };
                DonationDtos.Add(NewDonation);
            }

            return Ok(DonationDtos);
        }



        // GET: api/DonationData/FindDonation/5
        [HttpGet]
        [ResponseType(typeof(DonationDto))]
        public IHttpActionResult FindDonation(int id)
        {
            //find donation data
            Donation Donation = db.Donations.Find(id);
            if (Donation == null)
            {
                return NotFound();
            }

            //Put into Dto form

            DonationDto DonationDto = new DonationDto
            {
                DonationId = Donation.DonationId,
                Date = Donation.Date,
                Amount = Donation.Amount,
                OneTime = Donation.OneTime,
                Msg = Donation.Msg,
                Dedication = Donation.Dedication,
                DedicateName = Donation.DedicateName,
                Action = Donation.Action,
                Anonymity = Donation.Anonymity,
                PaymentMethod = Donation.PaymentMethod,
                PaymentNumber = Donation.PaymentNumber,
                DonorId = Donation.DonorId
            };

            return Ok(DonationDto);
        }

        /// <summary>
        /// Finds a particular Donor in the database given a donation id with a 200 status code. If the Donor is not found, return 404.
        /// </summary>
        /// <param name="id">The donation id</param>
        /// <returns>Information about the Donor, including Donor id, and all the other columns in DonorDto</returns>
        // <example>
        // GET: api/DonorData/FindDonorForDonation/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(DonorDto))]
        public IHttpActionResult FindDonorForDonation(int id)
        {
            //Finds the first team which has any players
            //that match the input playerid
            Donor Donor = db.Donors
                .Where(dn => dn.Donations.Any(dnt => dnt.DonationId == id))
                .FirstOrDefault();
            //if not found, return 404 status code.
            if (Donor == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
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

            //pass along data as 200 status code OK response
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
        public IHttpActionResult UpdateDonation(int id, [FromBody] Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != donation.DonationId)
            {
                return BadRequest();
            }

            db.Entry(donation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationExists(id))
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
        /// Adds a donation to the database.
        /// </summary>
        /// <param name="donation">A donation object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/DonationData/AddDonation
        ///  FORM DATA: Donation JSON Object
        /// </example>
        [ResponseType(typeof(Donation))]
        [HttpPost]
        public IHttpActionResult AddDonation([FromBody] Donation donation)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Donations.Add(donation);
            db.SaveChanges();

            return Ok(donation.DonationId);
        }

        /// <summary>
        /// Deletes a donation in the database
        /// </summary>
        /// <param name="id">The id of the donation to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/DonationData/DeleteDonation/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteDonation(int id)
        {
            Donation donation= db.Donations.Find(id);
            if (donation == null)
            {
                return NotFound();
            }

            db.Donations.Remove(donation);
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
        /// Finds a donation in the system. Internal use only.
        /// </summary>
        /// <param name="id">The donation id</param>
        /// <returns>TRUE if the donation exists, false otherwise.</returns>
        private bool DonationExists(int id)
        {
            return db.Donations.Count(e => e.DonationId == id) > 0;
        }
    }
}