using System.ComponentModel.DataAnnotations;

namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class ClaimDto
    {
        [Required]
        public Guid PolicyId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Claim reason cannot exceed 1000 characters.")]
        public string ClaimReason { get; set; } = null!;
    }
}
