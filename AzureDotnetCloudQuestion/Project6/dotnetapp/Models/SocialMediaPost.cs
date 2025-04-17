using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class SocialMediaPost
    {
        [Key] // Marks this as the primary key, auto-incremented in the database
        public int PostID { get; set; }

        [Required] // Ensures the Title is required
        [StringLength(255)] // Sets the maximum length for the Title field
        public string Title { get; set; }

        [Required] // Ensures the Content is required
        [StringLength(255)] // Sets the maximum length for the Content field
        public string Content { get; set; }

        [Required] // Ensures the PublishDate is required
        public string PublishDate { get; set; }

        [Required] // Ensures the Platform is required
        [StringLength(100)] // Sets the maximum length for the Platform field
        public string Platform { get; set; }

        [Required] // Ensures the Status is required
        [StringLength(50)] // Sets the maximum length for the Status field
        public string Status { get; set; }
    }
}
