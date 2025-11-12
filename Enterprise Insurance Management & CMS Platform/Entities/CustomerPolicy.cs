namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class CustomerPolicy
    {
        public Guid Id { get; set; }
        public string? CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }

        public Guid PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public bool IsPaymentDone { get; set; } = false;
    }
}

