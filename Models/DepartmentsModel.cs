using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class DepartmentsModel
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        // Establish a 1-to-many relationship with the JobPostings table
        public ICollection<JobPostingsModel> JobPostings { get; set; }
        public ICollection<Testimonial> Testimonials { get; set; }
        public ICollection<Contact> Contact { get; set; }
        public ICollection<Ecards> Ecards { get; set; }

    }

    public class DepartmentsDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

    }
}