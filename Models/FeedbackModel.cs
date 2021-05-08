using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class FeedbackModel
    {
        [Key]
        public int feedback_id { get; set; }
        [Required]
        public DateTime date { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string text { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual DepartmentsModel Department { get; set; }
    }
    public class FeedbackDto
    {
        public int feedback_id { get; set; }
        public DateTime date { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public int DepartmentId { get; set; }

    }
}