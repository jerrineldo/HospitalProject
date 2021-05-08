using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class ShowDepartment
    {
        //Conditionally render the page based on admin or non admin
        public bool isadmin { get; set; }

        public IEnumerable<DepartmentsDto> departments { get; set; }
    }
}