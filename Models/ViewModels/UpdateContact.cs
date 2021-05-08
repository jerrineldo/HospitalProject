using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class UpdateContact
    {
        //Information about the player
        public ContactDto contact { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<DepartmentsDto> alldepartments { get; set; }
    }
}