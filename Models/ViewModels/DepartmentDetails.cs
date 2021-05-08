using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class DepartmentDetails
    {
        //Conditionally render the page based on admin or non admin
        public bool isadmin { get; set; }
        public bool isloggedinUser { get; set; }
        public string userid { get; set; }
        public DepartmentsDto DepartmentDto { get; set; }
        public IEnumerable<TestimonialDto> Testimonials { get; set; }
        public IEnumerable<JobPostingsModel> JobPostings { get; set; }
    }
}