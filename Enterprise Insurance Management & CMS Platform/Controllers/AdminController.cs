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

    [HttpGet("role-counts")]
    public async Task<IActionResult> GetUserRoleCounts()
    {
        return Ok(await _adminService.GetUserRoleCountsAsync());
    }

    [HttpGet("policies")]
    public async Task<IActionResult> GetAllPolicies()
    {
        return Ok(await _adminService.GetAllPoliciesAsync());
    }

    [HttpGet("policies/active")]
    public async Task<IActionResult> GetActivePolicies()
    {
        return Ok(await _adminService.GetActivePoliciesAsync());
    }

    [HttpGet("policies/inActive")]
    public async Task<IActionResult> GetInActivePolicies()
    {
        return Ok(await _adminService.GetInActivePoliciesAsync());
    }

    [HttpGet("policies/expired")]
    public async Task<IActionResult> GetExpiredPolicies()
    {
        return Ok(await _adminService.GetExpiredPoliciesAsync());
    }

    [HttpGet("policies/count")]
    public async Task<IActionResult> GetTotalPolicies()
    {
        return Ok(new { total = await _adminService.GetTotalPoliciesAsync() });
    }

    [HttpGet("claims/counts")]
    public async Task<IActionResult> GetClaimCounts()
    {
        return Ok(await _adminService.GetClaimCountsAsync());
    }

    [HttpGet("claims/submitted")]
    public async Task<IActionResult> GetSubmittedClaims()
    {
        return Ok(await _adminService.GetAllSubmittedClaimsAsync());
    }

    [HttpGet("claims/approved")]
    public async Task<IActionResult> GetApprovedClaims()
    {
        return Ok(await _adminService.GetAllApprovedClaimsAsync());
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await _adminService.GetAllCustomersAsync();
        return Ok(result);
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents()
    {
        var result = await _adminService.GetAllAgentsAsync();
        return Ok(result);
    }

    [HttpGet("editors")]
    public async Task<IActionResult> GetEditors()
    {
        var result = await _adminService.GetAllEditorsAsync();
        return Ok(result);
    }

    [HttpGet("claims-officers")]
    public async Task<IActionResult> GetClaimsOfficers()
    {
        var result = await _adminService.GetAllClaimsOfficersAsync();
        return Ok(result);
    }

}
