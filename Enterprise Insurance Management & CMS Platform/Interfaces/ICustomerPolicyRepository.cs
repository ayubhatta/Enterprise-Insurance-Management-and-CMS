using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface ICustomerPolicyRepository
    {
        Task<CustomerPolicy?> GetCustomerPolicyAsync(string customerId, Guid policyId);
        Task<bool> PurchasePolicyAsync(CustomerPolicy customerPolicy);
        Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid);
        Task<CustomerPolicy?> GetPurchaseByIdAsync(Guid id);
        Task<IEnumerable<CustomerPolicyDto>> GetPoliciesByCustomerAsync(string customerId);
        Task<Policy?> GetPolicyByIdAsync(Guid policyId);
    }
}