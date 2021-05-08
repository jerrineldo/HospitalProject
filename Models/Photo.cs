using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Photo
    {
        [Key]
        public int photo_id { get; set; }


        [Required]
        public string photo_path { get; set; }

        public string description { get; set; }
        public string photographer_fname { get; set; }

        public string photographer_lname { get; set; }
        
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual DepartmentsModel Department { get; set; }

    }

    public class PhotoDto
    {
        public int photo_id { get; set; }
        public string photo_path { get; set; }

        public string description { get; set; }
        public string photographer_fname { get; set; }

        public string photographer_lname { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

    }
}