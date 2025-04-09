using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class SeasonalCrop
    {
        [Key]
        public int CropID { get; set; }

        [Required]
        [StringLength(150)]
        public string CropName { get; set; }

        [Required]
        [StringLength(50)]
        public string Season { get; set; } // e.g., Spring, Summer, Fall, Winter

        [Required]
        [StringLength(255)]
        public string StartDate { get; set; } // Format: yyyy-MM-dd

        [Required]
        [StringLength(255)]
        public string EndDate { get; set; } // Format: yyyy-MM-dd

        [StringLength(500)]
        public string Description { get; set; }
    }
}
