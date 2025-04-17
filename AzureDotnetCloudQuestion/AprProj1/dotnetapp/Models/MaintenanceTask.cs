using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class MaintenanceTask
    {
        [Key] // Marks this as the primary key, auto-incremented in the database
        public int TaskID { get; set; }

        [Required]
        [StringLength(255)]
        public string TaskName { get; set; }

        [Required]
        [StringLength(100)]
        public string HighwayName { get; set; }

        [Required]
        [StringLength(100)]
        public string MaintenanceType { get; set; } // Pothole Repair, Signboard Fixing, Tree Trimming, Grass Cutting, etc.

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; }

        [Required]
        public string ScheduledDate { get; set; } // yyyy-MM-dd format

        [StringLength(500)]
        public string AdditionalNotes { get; set; } // Optional notes
    }
}
