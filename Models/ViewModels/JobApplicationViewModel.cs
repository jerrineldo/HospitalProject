using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class JobApplicationViewModel
    {
        [DisplayName("Application No.")]
        public int ApplicationId { get; set; }

        [DisplayName("Originating Job Posting")]
        [ForeignKey("Posting")]
        public int PostingId { get; set; }

        [DisplayName("Submission Date")]
        public DateTime ApplicationDate { get; set; }

        [DisplayName("First Name")]
        public string ApplicantFirstName { get; set; }

        [DisplayName("Last Name")]
        public string ApplicantLastName { get; set; }

        [DisplayName("Email")]
        public string ApplicantEmail { get; set; }

        [DisplayName("Phone")]
        public string ApplicantPhone { get; set; }

        [DisplayName("CV Package")]
        public string CvPath { get; set; }

    }
}