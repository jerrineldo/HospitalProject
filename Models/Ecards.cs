using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Ecards
    {
        [Key]
        public int ecard_id { get; set; }

        
        public string photo_path { get; set; }

        
        public string message { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual DepartmentsModel Department { get; set; }

        //Establish a 1-to-many relationship with departments table
        public ICollection<DepartmentsModel> Departments { get; set; }
    }

    public class EcardsDto
    {
        public int ecard_id { get; set; }

        
        public string photo_path { get; set; }

        
        public string message { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}