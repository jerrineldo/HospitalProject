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


namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class FaqDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET; api/FaqData/GetFaqs
        [ResponseType(typeof(IEnumerable<FaqDto>))]
        public IHttpActionResult GetFaqs()
        {
            IEnumerable<Faq> Faqs = db.Faqs.ToList();
            List<FaqDto> FaqDtos = new List<FaqDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Faq in Faqs)
            {
                FaqDto NewFaq = new FaqDto
                {
                    faq_id = Faq.faq_id,
                    question = Faq.question,
                    answer = Faq.answer,
                    DepartmentName = Faq.Department.DepartmentName,
                };
                FaqDtos.Add(NewFaq);
            }

            return Ok(FaqDtos);
        }

        // GET: api/FaqData/FindFaq/5
        //for finding Faqs
        [HttpGet]
        [ResponseType(typeof(FaqDto))]
        public IHttpActionResult FindFaq(int id)
        {
            //find the Faq data
            Faq Faq = db.Faqs.Find(id);
            //does db search and if results come back, 
            //We continue, if not, return error not found.
            if (Faq == null)
            {
                return NotFound();
            }

            //Put into Dto form

            FaqDto FaqDto = new FaqDto
            {
                faq_id = Faq.faq_id,
                question = Faq.question,
                answer = Faq.answer,
                DepartmentName = Faq.Department.DepartmentName,
            };

            return Ok(FaqDto);
        }

        // GET: api/FaqData/GetFaq/5
        [ResponseType(typeof(Faq))]
        public IHttpActionResult GetFaq(int id)
        {
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return NotFound();
            }


            return Ok(faq);
        }

        /// POST: api/FaqData/UpdateFaq/5
        /// FORM DATA: Faq JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateFaq(int id, [FromBody] Faq faq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Entry(faq).State = EntityState.Modified;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// POST: api/faqData/Addfaq
        ///  FORM DATA: faq JSON Object
        [ResponseType(typeof(Faq))]
        [HttpPost]
        public IHttpActionResult AddFaq([FromBody] Faq faq)
        {
            //If the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //add then save onto db 
            db.Faqs.Add(faq);
            db.SaveChanges();
            return Ok(faq.faq_id);
        }




        // DELETE: api/FaqData/5
        [ResponseType(typeof(Faq))]
        public IHttpActionResult DeleteFaq(int id)
        {
            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return NotFound();
            }

            db.Faqs.Remove(faq);
            db.SaveChanges();

            return Ok(faq);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FaqExists(int id)
        {
            return db.Faqs.Count(e => e.faq_id == id) > 0;
        }
    }
}