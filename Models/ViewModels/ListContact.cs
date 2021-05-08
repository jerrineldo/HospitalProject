using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class ListContact
    {
        public bool isadmin { get; set; }

        public IEnumerable<ContactDto> Contacts { get; set; }
    }
}