using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface ICustomerProfileRepository
    {
        Task<CustomerProfile?> GetByUserIdAsync(string userId);
        Task<CustomerProfile> CreateAsync(CustomerProfile profile);
        Task<IEnumerable<CustomerProfile>> GetAllAsync();
    }
}
