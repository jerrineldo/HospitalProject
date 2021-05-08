using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class EditEcard
    {
        [DisplayName("Ecards No.")]
        public int ecard_id { get; set; }

        [DisplayName("Photo")]
        public string photo_path { get; set; }

        [DisplayName("Message")]
        public string message { get; set; }

        [DisplayName("Department")]
        [ForeignKey("Department")]
        public string DepartmentName { get; set; }

        public IEnumerable<DepartmentsDto> alldepartments { get; set; }

        public EcardsDto ecards { get; set; }
    }
}