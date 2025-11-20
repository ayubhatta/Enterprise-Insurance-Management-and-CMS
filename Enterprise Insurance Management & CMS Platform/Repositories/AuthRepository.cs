using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories;
public class AuthRepository(UserManager<ApplicationUser> _userManager, IConfiguration _config, ICustomerProfileRepository _profileRepo) : IAuthRepository
{
    public async Task<(bool Succeeded, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return (false, new[] { "Email already in use" });

        if (!dto.Email.EndsWith("@company.com", StringComparison.OrdinalIgnoreCase))
            return (false, new[] { "Only '@company.com' emails are allowed to register" });

        var user = new ApplicationUser
        {
            UserName = dto.Email.Split('@')[0],
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return (false, result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "Customer"); 

        var profile = new CustomerProfile
        {
            UserId = user.Id,
            Address = dto.Address,
            NationalId = dto.NationalId,
            DateOfBirth = DateTime.SpecifyKind(
                                            dto.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                                            DateTimeKind.Utc)
        };

        await _profileRepo.CreateAsync(profile);

        return (true, null);
    }

    public async Task<(string token, string role)?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return null;

        var token = await GenerateJwtToken(user);
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Customer";

        return (token, role);
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwt = _config.GetSection("Jwt");
        var secret = jwt["key"] ?? throw new InvalidOperationException("JWT Secret key not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("uid", user.Id)
            //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiryMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
