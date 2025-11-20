namespace Enterprise_Insurance_Management___CMS_Platform.DTOs;

public class RegisterDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public IFormFileCollection? Documents { get; set; }

}
