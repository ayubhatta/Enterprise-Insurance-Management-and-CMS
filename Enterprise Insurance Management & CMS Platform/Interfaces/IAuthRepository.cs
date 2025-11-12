using Enterprise_Insurance_Management___CMS_Platform.DTOs;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces;
public interface IAuthRepository
{
    Task<(bool Succeeded, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
    Task<bool> DeleteUserAsync(Guid id);
}
