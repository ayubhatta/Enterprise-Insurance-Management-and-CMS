using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Services;

public class AdminService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager) : IAdminService
{
    public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return false;

        // Prevent multiple admins
        if (dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!await _roleManager.RoleExistsAsync(dto.Role))
            return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return false;

        var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
        return addResult.Succeeded;
    }

    public async Task<IEnumerable<object>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new
            {
                user.Id,
                user.UserName,
                user.Email,
                Roles = roles
            });
        }

        return result;
    }

    public async Task<IEnumerable<object>> GetAllRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();
    }


    public async Task<(bool Succeeded, IEnumerable<string>? Errors)> RegisterStaffAsync(AdminRegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return (false, new[] { "Email already in use" });

        var user = new ApplicationUser
        {
            UserName = dto.Email.Split('@')[0],
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, dto.Role);

        return (true, null);
    }


}
