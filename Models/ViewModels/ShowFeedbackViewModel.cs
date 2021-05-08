using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Red_Lake_Hospital_Redesign_Team6.Models.ViewModels
{
    public class ShowFeedbackViewModel : Controller
    {
        public FeedbackDto Feedback { get; set; }
    }
}