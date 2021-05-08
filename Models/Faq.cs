using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Faq
    {
        [Key]
        public int faq_id { get; set; }

        public string question { get; set; }
        public string answer { get; set; }


        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual DepartmentsModel Department { get; set; }

    }

    public class FaqDto
    {
        public int faq_id { get; set; }

        public string question { get; set; }
        public string answer { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

    }
}