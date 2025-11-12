using Enterprise_Insurance_Management___CMS_Platform.DTOs;

namespace Enterprise_Insurance_Management___CMS_Platform.Services;
public interface IAdminService
{
    Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto);
    Task<IEnumerable<object>> GetAllUsersAsync();
    Task<IEnumerable<object>> GetAllRolesAsync();
    Task<(bool Succeeded, IEnumerable<string>? Errors)> RegisterStaffAsync(AdminRegisterDto dto);
}
