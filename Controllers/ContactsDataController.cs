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
    public class ContactsDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// The GetContacts method accesses the database to obtain a collection of all contact listings and return the collection.
        /// </summary>
        /// <returns>List of all contacts </returns>
        /// <param name="key">MD5 key required to access this API end point.</param>
        // GET: api/contactsdata/List/{key}
        [ResponseType(typeof(IEnumerable<ContactDto>))]
        public IHttpActionResult GetContacts()
        {
            List<Contact> Contacts = db.Contacts.ToList();
            List<ContactDto> ContactDtos = new List<ContactDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Contact in Contacts)
            {
                ContactDto NewContact = new ContactDto
                {
                    contact_id = Contact.contact_id,
                    Firstname = Contact.Firstname,
                    Lastname = Contact.Lastname,
                    Email = Contact.Email,
                    Phone = Contact.Phone,
                    Message = Contact.Message,
                    DepartmentName = Contact.Department.DepartmentName
                };
                ContactDtos.Add(NewContact);
            }

            return Ok(ContactDtos);
        }


        /// <summary>
        /// Finds a particular contact in the database with a 200 status code. If the player is not found, return 404.
        /// </summary>
        /// <param name="id">The contact id</param>
        /// <returns>Information about the contact, including contact id, first and last name, email, phone, message and departmentname</returns>
        // <example>
        // GET: api/ContactsData/FindContact/{key}
        [HttpGet]
        [ResponseType(typeof(ContactDto))]
        public IHttpActionResult FindContact(int id)
        {
            //Find the data
            Contact contact = db.Contacts.Find(id);
            //if not found, return 404 status code.
            if (contact == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            ContactDto ContactDtos = new ContactDto
            {
                contact_id = contact.contact_id,
                Firstname = contact.Firstname,
                Lastname = contact.Lastname,
                Email = contact.Email,
                Phone = contact.Phone,
                Message = contact.Message,
                DepartmentName = contact.Department.DepartmentName
            };


            //pass along data as 200 status code OK response
            return Ok(ContactDtos);
        }


        /// <summary>
        /// Finds a particular Department in the database given a contact id with a 200 status code. If the Department is not found, return 404.
        /// </summary>
        /// <param name="id">The contact id</param>
        /// <returns>Information about the Department, including Department id and Department Name</returns>
        // <example>
        // GET: api/DepartmentData/FindDepartmentForContact/{key}
        [HttpGet]
        [ResponseType(typeof(DepartmentsDto))]
        public IHttpActionResult FindDepartmentForContact(int id)
        {
            //Finds the first department which has any contacts
            //that match the input contactid
            DepartmentsModel departments = db.Departments
                .Where(d => d.Contact.Any(c => c.contact_id == id))
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
        /// Updates a contact in the database given information about the contact.
        /// </summary>
        /// <param name="id">The contact id</param>
        /// <param name="player">A contact object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/ContactsData/UpdateContact/{key}
        /// FORM DATA: Contact JSON Object
        // PUT: api/ContactsData/UpdateContact{key}
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateContact(int id, [FromBody] Contact contact)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contact.contact_id)
            {
                return BadRequest();
            }


            db.Entry(contact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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
        /// Adds a contact to the database.
        /// </summary>
        /// <param name="player">A contact object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/ContactsData/AddContact
        ///  FORM DATA: Contact JSON Object
        [ResponseType(typeof(Contact))]
        [HttpPost]
        [Route("api/contactsdata/addcontact")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddContact([FromBody] Contact contact)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Contacts.Add(contact);
            db.SaveChanges();

            return Ok(contact.contact_id);
        }


        /// <summary>
        /// Deletes a contact in the database
        /// </summary>
        /// <param name="id">The id of the contact to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/ContactsData/DeleteContact/{key}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteContact(int id)
        {
            Contact contacts = db.Contacts.Find(id);
            if (contacts == null)
            {
                return NotFound();
            }

            db.Contacts.Remove(contacts);
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
        private bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.contact_id == id) > 0;
        }
    }
}