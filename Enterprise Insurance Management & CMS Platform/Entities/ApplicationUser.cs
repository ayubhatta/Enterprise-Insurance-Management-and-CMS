using Microsoft.AspNetCore.Identity;

namespace Enterprise_Insurance_Management___CMS_Platform.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
