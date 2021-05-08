using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Testimonial
    {
        [Key]
        public int testimonial_Id { get; set; }

        [Required(ErrorMessage = "Please Enter Testimonial Content.")]
        [AllowHtml]
        public string testimonial_Content { get; set; }

        [Required(ErrorMessage = "Please Enter a First Name.")]
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Please Enter a Last Name.")]
        public string last_Name { get; set; }

        [Required(ErrorMessage = "Please Enter an Email.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Please Enter a Phone Number.")]
        public int phone_Number { get; set; }
        public bool Has_Pic { get; set; }
        public string Pic_Extension { get; set; }

        [Required(ErrorMessage = "Please Enter a Posted Date.")]
        public DateTime posted_Date { get; set; }
        public bool Approved { get; set; }

        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public ICollection<DepartmentsModel> Departments { get; set; }

    }
    public class TestimonialDto
    {
        public int testimonial_Id { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please Enter Testimonial Content.")]
        public string testimonial_Content { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Enter a First Name.")]
        public string first_Name { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Enter a Last Name.")]
        public string last_Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Please Enter a Email.")]
        public string email { get; set; }

        [DisplayName("Phone Number")]
        [Required(ErrorMessage = "Please Enter a Phone Number.")]
        public int phone_Number { get; set; }

        public bool Has_Pic { get; set; }
        public string Pic_Extension { get; set; }
        public DateTime posted_Date { get; set; }
        public bool Approved { get; set; }
        public string Id { get; set; }
    }
}