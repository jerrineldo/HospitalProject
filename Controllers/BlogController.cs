using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Red_Lake_Hospital_Redesign_Team6.Models;
using Red_Lake_Hospital_Redesign_Team6.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Net;

namespace Red_Lake_Hospital_Redesign_Team6.Controllers
{
    public class BlogController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static BlogController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                UseCookies = false
            };
            client = new HttpClient(handler)
            {
                //change this to match your own local port number
                BaseAddress = new Uri("https://localhost:44349/api/")
            };
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog List" Page. Gathers information from the database.
        /// </summary>
        /// <returns>A dynamic "Blog List" webpage which provides a list of all blogs in the database.</returns>
        /// <example>GET : /Blog/List</example>
        // GET: Blog/List
        public ActionResult List()
        {
            string url = "blogapi/getblogs";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<BlogDto> SelectedBlogs = response.Content.ReadAsAsync<IEnumerable<BlogDto>>().Result;
                return View(SelectedBlogs);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog AdminList" Page. Gathers information from the database.
        /// </summary>
        /// <returns>A dynamic "Blog List" webpage which provides a list of all blogs in the database and admin options.</returns>
        /// <example>GET : /Blog/AdminList</example>
        [Authorize(Roles = "Admin")]
        public ActionResult AdminList()
        {
            GetApplicationCookie();

            string url = "blogapi/getblogs";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<BlogDto> SelectedBlogs = response.Content.ReadAsAsync<IEnumerable<BlogDto>>().Result;
                return View(SelectedBlogs);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Show" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides detailes for a selected blog.</returns>
        /// <example>GET : /Blog/Show/5</example>
        public ActionResult Show(int id)
        {
            ShowBlogViewModel ViewModel = new ShowBlogViewModel();
            string url = "blogapi/findblog/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                BlogDto SelectedBlogs = response.Content.ReadAsAsync<BlogDto>().Result;
                ViewModel.Blog = SelectedBlogs;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog AdminShow" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides detailes for a selected blog.</returns>
        /// <example>GET : /Blog/Show/5</example>
        [Authorize(Roles = "Admin")]
        public ActionResult AdminShow(int id)
        {
            GetApplicationCookie();

            ShowBlogViewModel ViewModel = new ShowBlogViewModel();
            string url = "blogapi/findblog/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                BlogDto SelectedBlogs = response.Content.ReadAsAsync<BlogDto>().Result;
                ViewModel.Blog = SelectedBlogs;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Create" Page. Adds information from the database.
        /// </summary>
        /// <returns>A dynamic "Blog Create" webpage which can add a blog entry to the database.</returns>
        /// <example>GET : /Blog/Create</example>
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            GetApplicationCookie();

            return View();
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Create" Page. Adds information from the database.
        /// </summary>
        /// <returns>A dynamic "Blog Create" webpage which can add a blog entry to the database.</returns>
        /// <example>GET : /Blog/Create</example>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(BlogModel BlogInfo)
        {
            GetApplicationCookie();

            Debug.WriteLine(BlogInfo.title);
            string url = "blogapi/addblog";
            Debug.WriteLine(jss.Serialize(BlogInfo));
            HttpContent content = new StringContent(jss.Serialize(BlogInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides a form to input new blog information.</returns>
        /// <example>GET : /Blog/Update/5</example>
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id)
        {
            GetApplicationCookie();

            UpdateBlogViewModel ViewModel = new UpdateBlogViewModel();

            string url = "blogapi/findblog/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                BlogDto SelectedBlog = response.Content.ReadAsAsync<BlogDto>().Result;
                ViewModel.Blog = SelectedBlog;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides a form to input new blog information.</returns>
        /// <example>GET : /Blog/Update/5</example>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, BlogDto BlogInfo)
        {
            GetApplicationCookie();

            string url = "blogapi/updateblog/" + id;
            HttpContent content = new StringContent(jss.Serialize(BlogInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Show", new { id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        /// <summary>
        /// Routes to a dynamically generated "Blog Delete" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides information on a blog that can be deleted.</returns>
        /// <example>GET : /Blog/Delete/5</example>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();

            string url = "blogapi/findblog/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                BlogModel SelectedModule = response.Content.ReadAsAsync<BlogModel>().Result;
                return View(SelectedModule);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Routes to a dynamically generated "Blog Delete" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Blog</param>
        /// <returns>A dynamic "Blog Show" webpage which provides information on a blog that can be deleted.</returns>
        /// <example>GET : /Blog/Delete/5</example>
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int id)
        {
            GetApplicationCookie();

            string url = "blogapi/deleteblog/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}