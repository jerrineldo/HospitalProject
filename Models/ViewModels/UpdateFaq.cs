using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class UpdateFaq
    {

        public FaqDto faq { get; set; }

        //information about possible departments this FAQ could be for
        public IEnumerable<DepartmentsDto> departments { get; set; }
    }
}