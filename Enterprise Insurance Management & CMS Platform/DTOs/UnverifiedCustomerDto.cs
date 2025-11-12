namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class UnverifiedCustomerDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public bool IsVerified { get; set; }
        public DateTime RegisteredAt { get; set; }

        public UserInfoDto User { get; set; } = null!;
    }
}
