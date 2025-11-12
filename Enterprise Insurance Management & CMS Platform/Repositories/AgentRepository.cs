using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class AgentRepository(AppDbContext _db, UserManager<ApplicationUser> _userManager) : IAgentRepository
    {
        public async Task<IEnumerable<UnverifiedCustomerDto>> GetUnverifiedCustomersAsync()
        {
            var customers = await _db.CustomerProfiles
                .Include(c => c.User)
                .Where(c => !c.IsVerified)
                .ToListAsync();

            var result = new List<UnverifiedCustomerDto>();

            foreach (var c in customers)
            {
                var roles = await _userManager.GetRolesAsync(c.User!);
                if (!roles.Contains("Customer")) continue;

                result.Add(new UnverifiedCustomerDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Address = c.Address,
                    NationalId = c.NationalId,
                    DateOfBirth = c.DateOfBirth,
                    IsVerified = c.IsVerified,
                    RegisteredAt = c.RegisteredAt,
                    User = new UserInfoDto
                    {
                        FullName = c.User!.FullName!,
                        UserName = c.User.UserName!,
                        Email = c.User.Email!,
                        PhoneNumber = c.User.PhoneNumber!
                    }
                });
            }
            return result;
        }

        public async Task<CustomerProfile?> GetCustomerProfileByIdAsync(string userId)
        {
            return await _db.CustomerProfiles
                            .Include(c => c.User)
                            .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> VerifyCustomerAsync(string userId)
        {
            var profile = await _db.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == userId);
            if (profile == null) return false;

            profile.IsVerified = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
