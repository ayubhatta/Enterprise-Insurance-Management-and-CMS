namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class ClaimEntity
    {
        public Guid Id { get; set; }
        public Guid PolicyId { get; set; } 
        public string CustomerId { get; set; } = null!;
        public string ClaimReason { get; set; } = null!;
        public string Status { get; set; } = "Submitted"; // Submitted, UnderReview, Approved, Rejected
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public Policy? Policy { get; set; }
        public ApplicationUser? Customer { get; set; }
        public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();
    }
}
