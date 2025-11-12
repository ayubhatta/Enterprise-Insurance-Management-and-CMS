namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class PolicyResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int Version { get; set; }
        public List<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
