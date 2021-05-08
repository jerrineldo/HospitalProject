using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Testimonial> Testimonials { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        //Define the relationship between models and tables in the DB using DbSets
        public DbSet<DepartmentsModel> Departments { get; set; }
        public DbSet<JobApplicationsModel> JobApplications { get; set; }
        public DbSet<JobPostingsModel> JobPostings { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<BlogModel> Blog { get; set; }
        public DbSet<FeedbackModel> Feedback { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public System.Data.Entity.DbSet<Red_Lake_Hospital_Redesign_Team6.Models.Ecards> Ecards { get; set; }
    }
}