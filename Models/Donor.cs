using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class Donor
    {
        [Key]
        public int DonorId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string OrgName { get; set; }

        [Required]
        public string Fname { get; set; }

        [Required]
        public string Lname { get; set; }

        [Required]
        public string Addressl1 { get; set; }

        [Required]
        public string Addressl2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        //can be null?
        public string Province { get; set; }

        [Required]
        public string PostalCode { get; set; }

        /* foreign key constraint
        //A donor can make many donations
        [ForeignKey("Donation")]
        public int DonationId { get; set; }
        public virtual Donation Donation { get; set; }
        */
        
        //A donor can make many donations
        // Establish a 1-to-many relationship with the Donors table
        public ICollection<Donation> Donations { get; set; }

    }

    public class DonorDto
    {
        public int DonorId { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; }
        [DisplayName("OrgName")]
        public string OrgName { get; set; }
        [DisplayName("Fname")]
        public string Fname { get; set; }
        [DisplayName("Lname")]
        public string Lname { get; set; }
        [DisplayName("Addressl1")]
        public string Addressl1 { get; set; }
        [DisplayName("Addressl2")]
        public string Addressl2 { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("Province")]
        public string Province { get; set; }
        [DisplayName("PostalCode")]
        public string PostalCode { get; set; }
    }
}