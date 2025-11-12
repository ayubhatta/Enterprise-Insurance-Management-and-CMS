using Microsoft.AspNetCore.Identity;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Data;
public static class IdentitySeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = new[] { "Admin", "Editor", "Agent", "Customer", "ClaimsOfficer" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }

    public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "admin@company.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Admin"
            };
            var result = await userManager.CreateAsync(admin, "Admin@1234!"); 
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
