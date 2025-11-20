using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Services;
public interface IAdminService
{
    Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto);
    Task<IEnumerable<object>> GetAllUsersAsync();
    Task<IEnumerable<object>> GetAllRolesAsync();
    Task<(bool Succeeded, IEnumerable<string>? Errors)> RegisterStaffAsync(AdminRegisterDto dto);
    Task<object> GetUserRoleCountsAsync();
    Task<IEnumerable<PolicyResponseDto>> GetAllPoliciesAsync();
    Task<int> GetTotalPoliciesAsync();
    Task<object> GetClaimCountsAsync();
    Task<IEnumerable<ClaimEntity>> GetAllSubmittedClaimsAsync();
    Task<IEnumerable<ClaimEntity>> GetAllApprovedClaimsAsync();
    Task<InActivePoliciesResultDto> GetInActivePoliciesAsync();
    Task<ActivePoliciesResultDto> GetActivePoliciesAsync();
    Task<ExpiredPoliciesResultDto> GetExpiredPoliciesAsync();
    Task<IEnumerable<object>> GetAllCustomersAsync();
    Task<IEnumerable<object>> GetAllAgentsAsync();
    Task<IEnumerable<object>> GetAllEditorsAsync();
    Task<IEnumerable<object>> GetAllClaimsOfficersAsync();
}
