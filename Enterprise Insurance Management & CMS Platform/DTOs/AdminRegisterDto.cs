using System.ComponentModel.DataAnnotations;

namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class AdminRegisterDto
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!; // Must be "Agent", "Editor", or "ClaimsOfficer"
    }
}