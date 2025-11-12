namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class Policy
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = false;
        public int Version { get; set; } = 1;
        public ICollection<CustomerPolicy> CustomerPolicies { get; set; } = new List<CustomerPolicy>();
    }
}
