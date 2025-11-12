using System;
using System.ComponentModel.DataAnnotations;

namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class CustomerProfile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string NationalId { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
    }
}
