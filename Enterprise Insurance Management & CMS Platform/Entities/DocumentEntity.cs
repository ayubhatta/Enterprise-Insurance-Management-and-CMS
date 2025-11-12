namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class DocumentEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string UploadedById { get; set; } = null!;
        public string? LinkedToEntity { get; set; }  // Policy or CustomerProfile or Claim
        public Guid? LinkedEntityId { get; set; }    // PolicyId, ClaimId, etc.
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser? UploadedBy { get; set; }
    }
}