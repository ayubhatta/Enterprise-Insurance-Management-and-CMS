using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService _adminService, UserManager<ApplicationUser> _userManager) : ControllerBase
{

    [HttpPost("register-staff")]
    public async Task<IActionResult> RegisterStaff(AdminRegisterDto dto)
    {
        var allowedRoles = new[] { "Agent", "Editor", "ClaimsOfficer" };
        if (!allowedRoles.Contains(dto.Role))
            return BadRequest(new { message = "Invalid role. Allowed: Agent, Editor, ClaimsOfficer" });

        var (succeeded, errors) = await _adminService.RegisterStaffAsync(dto);
        if (!succeeded)
            return BadRequest(new { errors });

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { message = "User not found after registration." });

        return Ok(new { message = $"Staff registered successfully with role {dto.Role}" });
    }


    [HttpPatch("update-role")]
    public async Task<IActionResult> UpdateUserRole(UpdateUserRoleDto dto)
    {
        var success = await _adminService.UpdateUserRoleAsync(dto);
        if (!success) return BadRequest(new { message = "Cannot update role (maybe trying to assign admin or invalid role)." });
        return Ok(new { message = $"User role updated to {dto.Role}" });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _adminService.GetAllRolesAsync();
        return Ok(roles);
    }
}
