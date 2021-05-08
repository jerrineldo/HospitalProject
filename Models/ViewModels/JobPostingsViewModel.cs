using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class JobPostingsViewModel
    {

        public int PostingId { get; set; }

        public string ApplicationUserId { get; set; }

        [DisplayName("Contact Email")]
        public string ContactEmail { get; set; }

        [DisplayName("Posting Date")]
        public DateTime PostingDate { get; set; }

        [DisplayName("Application Deadline")]
        public DateTime PostingExpiryDate { get; set; }

        [DisplayName("Job Title")]
        public string PostingTitle { get; set; }

        [DisplayName("Job Description")]
        public string PostingDescription { get; set; }

        [DisplayName("Remuneration")]
        public string PostingRemuneration { get; set; }

        public int DepartmentId { get; set; }

        [DisplayName("Hiring Department")]
        public string Department { get; set; }

        public IEnumerable<DepartmentsModel> DepartmentsList { get; set; }

    }
}