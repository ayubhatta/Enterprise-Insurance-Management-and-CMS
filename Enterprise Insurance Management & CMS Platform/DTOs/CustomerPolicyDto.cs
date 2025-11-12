namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class CustomerPolicyDto
    {
        public Guid Id { get; set; }
        public Guid PolicyId { get; set; }
        public string PolicyTitle { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PurchasedAt { get; set; }
        public bool IsPaymentDone { get; set; }
    }
}
