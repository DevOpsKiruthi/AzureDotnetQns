using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class EMIDetails
    {
        public int EMIID { get; set; }
        public string BorrowerName { get; set; }
        public string LoanType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EMIAmount { get; set; }
        public string Description { get; set; }
    }
}