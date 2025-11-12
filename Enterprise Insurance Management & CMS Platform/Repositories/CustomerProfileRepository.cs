using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class CustomerProfileRepository(AppDbContext _context) : ICustomerProfileRepository
    {
        public async Task<CustomerProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.CustomerProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
        public async Task<CustomerProfile> CreateAsync(CustomerProfile profile)
        {
            _context.CustomerProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }
        public async Task<IEnumerable<CustomerProfile>> GetAllAsync()
        {
            return await _context.CustomerProfiles.Include(p => p.User).ToListAsync();
        }
    }
}
