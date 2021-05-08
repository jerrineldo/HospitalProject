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
    public class DepartmentsDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET; api/DepartmentsData/GetDepartments
        [ResponseType(typeof(IEnumerable<DepartmentsDto>))]
        public IHttpActionResult GetDepartments()
        {
            IEnumerable<DepartmentsModel> Departments = db.Departments.ToList();
            List<DepartmentsDto> DepartmentsDtos = new List<DepartmentsDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var department in Departments)
            {
                DepartmentsDto NewDepartments = new DepartmentsDto
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName

                };
                DepartmentsDtos.Add(NewDepartments);
            }

            return Ok(DepartmentsDtos);
        }

        // GET: api/DepartmentsModels/5
        [ResponseType(typeof(DepartmentsModel))]
        public IHttpActionResult GetDepartmentsModel(int id)
        {
            DepartmentsModel departmentsModel = db.Departments.Find(id);
            if (departmentsModel == null)
            {
                return NotFound();
            }

            return Ok(departmentsModel);
        }

        // PUT: api/DepartmentsModels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDepartmentsModel(int id, DepartmentsModel departmentsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != departmentsModel.DepartmentId)
            {
                return BadRequest();
            }

            db.Entry(departmentsModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentsModelExists(id))
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

        // POST: api/DepartmentsModels
        [ResponseType(typeof(DepartmentsModel))]
        public IHttpActionResult PostDepartmentsModel(DepartmentsModel departmentsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Departments.Add(departmentsModel);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = departmentsModel.DepartmentId }, departmentsModel);
        }

        // DELETE: api/DepartmentsModels/5
        [ResponseType(typeof(DepartmentsModel))]
        public IHttpActionResult DeleteDepartmentsModel(int id)
        {
            DepartmentsModel departmentsModel = db.Departments.Find(id);
            if (departmentsModel == null)
            {
                return NotFound();
            }

            db.Departments.Remove(departmentsModel);
            db.SaveChanges();

            return Ok(departmentsModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepartmentsModelExists(int id)
        {
            return db.Departments.Count(e => e.DepartmentId == id) > 0;
        }
    }
}