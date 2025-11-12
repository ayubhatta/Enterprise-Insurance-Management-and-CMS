namespace Enterprise_Insurance_Management___CMS_Platform.Entities
{
    public class AgentProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser? User { get; set; }
    }
}
