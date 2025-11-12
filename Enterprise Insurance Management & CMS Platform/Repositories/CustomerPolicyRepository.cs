using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class CustomerPolicyRepository(AppDbContext db) : ICustomerPolicyRepository
    {
        public async Task<IEnumerable<CustomerPolicyDto>> GetPoliciesByCustomerAsync(string customerId)
        {
            return await db.CustomerPolicies
                .Where(cp => cp.CustomerId == customerId)
                .Include(cp => cp.Policy)
                .Select(cp => new CustomerPolicyDto
                {
                    Id = cp.Id,
                    PolicyId = cp.PolicyId,
                    PolicyTitle = cp.Policy!.Title,
                    Category = cp.Policy.Category,
                    Description = cp.Policy.Description,
                    PurchasedAt = cp.PurchasedAt,
                    IsPaymentDone = cp.IsPaymentDone
                })
                .ToListAsync();
        }

        public async Task<CustomerPolicy?> GetCustomerPolicyAsync(string customerId, Guid policyId)
        {
            return await db.CustomerPolicies
                .FirstOrDefaultAsync(cp => cp.CustomerId == customerId && cp.PolicyId == policyId);
        }

        public async Task<bool> PurchasePolicyAsync(CustomerPolicy customerPolicy)
        {
            db.CustomerPolicies.Add(customerPolicy);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid)
        {
            var record = await db.CustomerPolicies.FindAsync(id);
            if (record == null) return false;

            record.IsPaymentDone = isPaid;
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<CustomerPolicy?> GetPurchaseByIdAsync(Guid id)
        {
            return await db.CustomerPolicies.FindAsync(id);
        }

        public async Task<Policy?> GetPolicyByIdAsync(Guid policyId)
        {
            return await db.Policies.FirstOrDefaultAsync(p => p.Id == policyId);
        }

    }
}