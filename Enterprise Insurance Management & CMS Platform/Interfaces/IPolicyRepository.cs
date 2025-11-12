using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<PolicyResponseDto>> GetAllAsync();
        Task<PolicyResponseDto?> GetByIdAsync(Guid id);
        Task<Policy> CreateAsync(Policy policy);
        Task<bool> DeleteAsync(Guid id);
        Task<Policy?> UpdateAsync(Guid id, Policy updatedPolicy);
        Task<Policy?> GetPolicyEntityByIdAsync(Guid id);
    }

}
