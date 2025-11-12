using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface IAgentRepository
    {        
        Task<bool> VerifyCustomerAsync(string userId);
        Task<CustomerProfile?> GetCustomerProfileByIdAsync(string userId);
        Task<IEnumerable<UnverifiedCustomerDto>> GetUnverifiedCustomersAsync();
    }
}
