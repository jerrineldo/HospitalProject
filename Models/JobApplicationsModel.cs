using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class JobApplicationsModel
    {

        [Key]
        public int ApplicationId { get; set; }


        [ForeignKey("Posting")]
        public int PostingId { get; set; }
        public virtual JobPostingsModel Posting { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }

        [Required]
        public string ApplicantFirstName { get; set; }

        [Required]
        public string ApplicantLastName { get; set; }

        [Required]
        public string ApplicantEmail { get; set; }

        public string ApplicantPhone { get; set; }

        public string CvPath { get; set; }



    }
}