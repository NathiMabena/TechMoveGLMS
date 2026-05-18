using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechMoveGLMS.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        [Required]
        public string Status { get; set; }

        [ValidateNever]
        public Contract Contract { get; set; }
    }
}