 using System;
  using System.ComponentModel.DataAnnotations;
  
  namespace dotnetapp.Models
  {
      public class WorshipperBooking
      {
          [Key] // Marks this as the primary key, auto-incremented in the database
          public int BookingID { get; set; }
  
          [Required]
          [StringLength(255)]
          public string WorshipperName { get; set; }
  
          [Required]
          [StringLength(255)]
          public string ReligiousInstitutionName { get; set; } // Temple, Church, Mosque
  
          [Required]
          [StringLength(100)]
          public string ServiceType { get; set; } // Prayer, Religious , Festival Event
  
          [Required]
          [StringLength(15)]
          public string ContactNumber { get; set; }
  
          [Required]
          public string BookingDate { get; set; } // yyyy-MM-dd format
  
          [StringLength(500)]
          public string SpecialRequest { get; set; } // Additional request
      }
  }