using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechMoveGLMS.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string ServiceLevel { get; set; }

        public string? SignedAgreementPath { get; set; }

        [ValidateNever]
        public Client Client { get; set; }

        [ValidateNever]
        public ICollection<ServiceRequest> ServiceRequests { get; set; }
    }
}