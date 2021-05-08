using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class UpdatePhoto
    {
        //Information about the player
        public PhotoDto photo { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<DepartmentsDto> departments { get; set; }
    }
}