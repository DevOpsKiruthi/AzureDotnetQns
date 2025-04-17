using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class DigitalAds
    {
        [Key]
        public int DigitalAdID { get; set; }

        [Required]
        [StringLength(255)]
        public string AdTitle { get; set; }

        [Required]
        [StringLength(255)]
        public string AdContent { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayLocation { get; set; }

        [Required]
        [StringLength(50)]
        public string AdType { get; set; }
    }
}
