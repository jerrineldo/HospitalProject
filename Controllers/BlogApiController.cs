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
    public class BlogApiController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Finds all blogs in the Database.
        /// </summary>
        /// <param name="id">The Blog ID</param>
        /// <returns>All blog objects</returns>
        /// <example>/api/blogapi/getblogs -> {Module Object}</example>
        [ResponseType(typeof(IEnumerable<BlogDto>))]
        public IHttpActionResult GetBlogs()
        {
            List<BlogModel> Blogs = db.Blog.ToList();
            List<BlogDto> BlogDtos = new List<BlogDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Blog in Blogs)
            {
                BlogDto NewBlog = new BlogDto
                {
                    blog_id = Blog.blog_id,
                    date = Blog.date,
                    photo_path = Blog.photo_path,
                    title = Blog.title,
                    text = Blog.text,
                    email = Blog.email,
                    ApplicationUserId = Blog.ApplicationUserId
                };
                BlogDtos.Add(NewBlog);
            }
            return Ok(BlogDtos);
        }

        /// <summary>
        /// Finds a blog from the Database through an id.
        /// </summary>
        /// <param name="id">The Blog ID</param>
        /// <returns>Blog object containing information about the blog with a matching ID. Empty Blog Object if the ID does not match any Blogs in the system.</returns>
        /// <example>/api/blogapi/findblogs/2 -> {Module Object}</example>
        [ResponseType(typeof(BlogDto))]
        [HttpGet]
        public IHttpActionResult FindBlog(int id)
        {
            BlogModel Blog = db.Blog.Find(id);

            if (Blog == null)
            {
                return NotFound();
            }

            BlogDto BlogDto = new BlogDto
            {
                blog_id = Blog.blog_id,
                date = Blog.date,
                photo_path = Blog.photo_path,
                title = Blog.title,
                text = Blog.text,
                email = Blog.email
            };
            return Ok(BlogDto);
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
        public IHttpActionResult AddBlog([FromBody] BlogModel blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Blog.Add(blog);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = blog.blog_id }, blog);
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
        [ResponseType(typeof(BlogDto))]
        [HttpPost]
        public IHttpActionResult UpdateBlog(int id, [FromBody] BlogModel blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blog.blog_id)
            {
                return BadRequest();
            }

            db.Entry(blog).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
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
        public IHttpActionResult DeleteBlog(int id)
        {
            BlogModel blog = db.Blog.Find(id);
            if (blog == null)
            {
                return NotFound();
            }

            db.Blog.Remove(blog);
            db.SaveChanges();

            return Ok();
        }

        private bool BlogExists(int id)
        {
            return db.Blog.Count(e => e.blog_id == id) > 0;
        }
    }
}