namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class PolicyPostResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int Version { get; set; }
    }
}
