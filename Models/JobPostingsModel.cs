using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class JobPostingsModel
    {
        [Key]
        public int PostingId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime PostingDate { get; set; }

        [Required]
        public DateTime PostingExpiryDate { get; set; }

        [Required]
        public string PostingTitle { get; set; }

        [Required]
        public string PostingDescription { get; set; }

        [Required]
        public string PostingRemuneration { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual DepartmentsModel Department { get; set; }


        // Establish a 1-to-many relationship with the JobApplications table
        public ICollection<JobApplicationsModel> JobApplications { get; set; }

    }
}