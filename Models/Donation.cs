using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool OneTime { get; set; }
        public string Msg { get; set; }
        //can be null?
        public string Dedication { get; set; }
        //can be null?
        public string DedicateName { get; set; }
        //can be null?
        public string Action { get; set; }
        [Required]
        public bool Anonymity { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string PaymentNumber { get; set; }
        //A donation is made by one donor
        [ForeignKey("Donor")]
        public int DonorId { get; set; }
        /*public string Fname { get; set; }
        public int Lname { get; set; }*/
        public virtual Donor Donor { get; set; }
    }

    //Data transferring vessel
    public class DonationDto
    {
        public int DonationId { get; set; }
        [DisplayName("Date")]
        public DateTime Date { get; set; }
        [DisplayName("Amount")]
        public decimal Amount { get; set; }
        [DisplayName("One Time")]
        public bool OneTime { get; set; }
        [DisplayName("Message")]
        public string Msg { get; set; }
        [DisplayName("Dedication")]
        public string Dedication { get; set; }
        [DisplayName("Dedicate Name")]
        public string DedicateName { get; set; }
        [DisplayName("Action")]
        public string Action { get; set; }
        [DisplayName("Anonymity")]
        public bool Anonymity { get; set; }
        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }
        [DisplayName("Payment Number")]
        public string PaymentNumber { get; set; }

        public int DonorId { get; set; }
        /*[DisplayName("Donor first name")]
        public string Fname { get; set; }
        [DisplayName("Donor last name")]
        public string Lname { get; set; }*/

    }
}