using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    //The View Model required to update a donation
    public class UpdateDonation
    {
        //Information about the donation
        public DonationDto donation { get; set; }
        //Needed for a dropdownlist which presents the donation with a choice of donors.
        //(Do I need this???)
        public IEnumerable<DonorDto> alldonors { get; set; }
        //Needed for displaying a donor's name
        public DonorDto donor { get; set; }
    }
}