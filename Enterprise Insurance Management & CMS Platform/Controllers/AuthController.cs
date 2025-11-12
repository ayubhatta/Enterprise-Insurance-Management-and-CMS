using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthRepository _authRepo, UserManager<ApplicationUser> _userManager, 
                            JobTriggerService _jobTriggerService, IDocumentService _documentService) 
                            : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterDto dto)
    {
        var (succeeded, errors) = await _authRepo.RegisterAsync(dto);
        if (!succeeded)
            return BadRequest(new { errors });

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { message = "User not found after registration." });

        // Handle document uploads if provided
        if (dto.Documents != null && dto.Documents.Count > 0)
        {
            var documents = await _documentService.SaveDocumentsAsync(
                dto.Documents,
                user.Id,
                "CustomerProfile",
                Guid.Parse(user.Id)
            );
        }

        _jobTriggerService.TriggerNewUserRegisteredJob(user.Id, TimeSpan.Zero);
        return Ok(new { message = "User registered successfully" });
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authRepo.LoginAsync(dto);
        if (token == null) return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new { token });
    }

    [Authorize(Roles = "Admin, Customer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _authRepo.DeleteUserAsync(id);
        if (!deleted) return NotFound(new { message = "User not found" });

        return Ok(new {success = true, message = "User Deleted Successfully." });
    }
}
