using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class SpeakerSlot
    {
        [Key] // Primary key for the slot request
        public int SlotID { get; set; }

        [Required]
        [StringLength(100)]
        public string SpeakerName { get; set; }

        [Required]
        [StringLength(255)]
        public string TalkTitle { get; set; }

        [Required]
        [StringLength(100)]
        public string TalkCategory { get; set; } // e.g., Technology, Motivation, Education, etc.

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ContactEmail { get; set; }

        [Required]
        public string SlotDate { get; set; } // yyyy-MM-dd format

        [StringLength(500)]
        public string TalkSummary { get; set; } // Optional short description of the talk
    }
}
