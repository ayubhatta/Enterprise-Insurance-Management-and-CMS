using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface IClaimRepository
    {
        Task<ClaimEntity> SubmitClaimAsync(ClaimEntity claim);
        Task<ClaimEntity?> GetClaimByIdAsync(Guid id);
        Task<IEnumerable<ClaimEntity>> GetClaimsByCustomerAsync(string customerId);
        Task<IEnumerable<ClaimEntity>> GetAllClaimsAsync();
        Task<bool> UpdateClaimStatusAsync(Guid id, string status);
        Task<ClaimEntity?> UpdateClaimAsync(Guid id, ClaimEntity updatedClaim);
        Task<bool> DeleteClaimAsync(Guid id);
    }
}