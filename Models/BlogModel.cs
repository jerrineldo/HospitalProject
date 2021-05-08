using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Red_Lake_Hospital_Redesign_Team6.Models
{
    public class BlogModel
    {
        [Key]
        public int blog_id { get; set; }
        [Required]
        public DateTime date { get; set; }
        public string photo_path { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string email { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class BlogDto
    {
        public int blog_id { get; set; }
        public DateTime date { get; set; }
        public string photo_path { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string email { get; set; }
        public string ApplicationUserId { get; set; }

    }
}