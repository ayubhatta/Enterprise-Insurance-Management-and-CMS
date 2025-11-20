using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;

namespace Enterprise_Insurance_Management___CMS_Platform.Services;

public class AdminService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, 
                          AppDbContext _context, IDocumentRepository _docRepo) 
                          : IAdminService
{
    public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return false;

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

    public async Task<object> GetUserRoleCountsAsync()
    {
        var roles = new[] { "Customer", "Agent", "Editor", "ClaimsOfficer" };

        var roleCounts = new Dictionary<string, int>();

        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            roleCounts[role] = users.Count;
        }

        return roleCounts;
    }

    public async Task<IEnumerable<PolicyResponseDto>> GetAllPoliciesAsync()
    {
        var policies = await _context.Policies.ToListAsync();

        var result = new List<PolicyResponseDto>();
        foreach (var p in policies)
        {
            var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);
            result.Add(new PolicyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ExpiryDate = p.ExpiryDate,
                IsActive = p.IsActive,
                Version = p.Version,
                Documents = docs.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Url = d.Url,
                    UploadedAt = d.UploadedAt
                }).ToList()
            });
        }

        return result;
    }


    public async Task<ActivePoliciesResultDto> GetActivePoliciesAsync()
    {
        var totalActivePolicies = await _context.Policies
            .Where(p => p.IsActive)
            .CountAsync();

        var activePolicies = await _context.Policies
            .Where(p => p.IsActive)
            .ToListAsync();

        var result = new List<PolicyResponseDto>();

        foreach (var p in activePolicies)
        {
            var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);

            result.Add(new PolicyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ExpiryDate = p.ExpiryDate,
                IsActive = p.IsActive,
                Version = p.Version,
                Documents = docs.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Url = d.Url,
                    UploadedAt = d.UploadedAt
                }).ToList()
            });
        }

        return new ActivePoliciesResultDto
        {
            TotalActivePolicies = totalActivePolicies,
            Policies = result
        };
    }



    public async Task<InActivePoliciesResultDto> GetInActivePoliciesAsync()
    {
        var today = DateTime.UtcNow;

        var totalInactivePolicies = await _context.Policies
            .Where(p => p.IsActive == false && p.ExpiryDate >= today)
            .CountAsync();

        var activePolicies = await _context.Policies
            .Where(p => p.IsActive == false && p.ExpiryDate >= today)
            .ToListAsync();


        var result = new List<PolicyResponseDto>();
        foreach (var p in activePolicies)
        {
            var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);
            result.Add(new PolicyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ExpiryDate = p.ExpiryDate,
                IsActive = p.IsActive,
                Version = p.Version,
                Documents = docs.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Url = d.Url,
                    UploadedAt = d.UploadedAt
                }).ToList()
            });
        }
        return new InActivePoliciesResultDto
        {
            TotalInActivePolicies = totalInactivePolicies,
            Policies = result
        };
    }


    public async Task<ExpiredPoliciesResultDto> GetExpiredPoliciesAsync()
    {
        var today = DateTime.UtcNow;

        var totalExpiredPolicies = await _context.Policies
            .Where(p => p.ExpiryDate <= today)
            .CountAsync();

        var expiredPolicies = await _context.Policies
            .Where(p => p.ExpiryDate <= today)
            .ToListAsync();

        var policyDTOs = new List<PolicyResponseDto>();

        foreach (var p in expiredPolicies)
        {
            var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);

            policyDTOs.Add(new PolicyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ExpiryDate = p.ExpiryDate,
                IsActive = p.IsActive,
                Version = p.Version,
                Documents = docs.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Url = d.Url,
                    UploadedAt = d.UploadedAt
                }).ToList()
            });
        }

        return new ExpiredPoliciesResultDto
        {
            TotalExpiredPolicies = totalExpiredPolicies,
            Policies = policyDTOs
        };
    }

    public async Task<int> GetTotalPoliciesAsync()
    {
        return await _context.Policies.CountAsync();
    }

    public async Task<object> GetClaimCountsAsync()
    {
        var submitted = _context.Claims
            .Where(c => c.Status == "Submitted");
        var approved = await _context.Claims
            .Where(c => c.Status == "Approved")
            .CountAsync();

        return new
        {
            TotalSubmitted = submitted,
            TotalApproved = approved
        };
    }

    public async Task<IEnumerable<ClaimEntity>> GetAllSubmittedClaimsAsync()
    {
        return await _context.Claims.ToListAsync();
    }

    public async Task<IEnumerable<ClaimEntity>> GetAllApprovedClaimsAsync()
    {
        return await _context.Claims
            .Where(c => c.Status == "Approved")
            .ToListAsync();
    }

    private async Task<IEnumerable<object>> GetUsersByRoleAsync(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

        var result = new List<object>();

        foreach (var user in usersInRole)
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

    public Task<IEnumerable<object>> GetAllCustomersAsync()
        => GetUsersByRoleAsync("Customer");

    public Task<IEnumerable<object>> GetAllAgentsAsync()
        => GetUsersByRoleAsync("Agent");

    public Task<IEnumerable<object>> GetAllEditorsAsync()
        => GetUsersByRoleAsync("Editor");

    public Task<IEnumerable<object>> GetAllClaimsOfficersAsync()
        => GetUsersByRoleAsync("ClaimsOfficer");


}
