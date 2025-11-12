using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService _adminService) : ControllerBase
{
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
