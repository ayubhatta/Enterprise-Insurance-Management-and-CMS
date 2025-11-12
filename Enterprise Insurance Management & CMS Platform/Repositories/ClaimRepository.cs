using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class ClaimRepository(AppDbContext _db) : IClaimRepository
    {
        private readonly Dictionary<string, string[]> AllowedTransitions = new()
        {
            { "Submitted", new[] { "UnderReview" } },
            { "UnderReview", new[] { "Approved", "Rejected" } },
            { "Approved", Array.Empty<string>() },
            { "Rejected", Array.Empty<string>() }
        };
        public async Task<ClaimEntity> SubmitClaimAsync(ClaimEntity claim)
        {
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();
            return claim;
        }

        public async Task<ClaimEntity?> GetClaimByIdAsync(Guid id)
        {
            var claim = await _db.Claims
                .Include(c => c.Policy)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claim != null)
            {
                claim.Documents = await _db.Documents
                    .Where(d => d.LinkedEntityId == claim.Id && d.LinkedToEntity == "Claim")
                    .ToListAsync();
            }

            return claim;
        }

        public async Task<IEnumerable<ClaimEntity>> GetClaimsByCustomerAsync(string customerId)
        {
            var claims = await _db.Claims
                .Include(c => c.Policy)
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (claims.Any())
            {
                var claimIds = claims.Select(c => c.Id).ToList();

                var allDocs = await _db.Documents
                    .Where(d => claimIds.Contains(d.LinkedEntityId!.Value) && d.LinkedToEntity == "Claim")
                    .ToListAsync();

                foreach (var claim in claims)
                {
                    claim.Documents = allDocs.Where(d => d.LinkedEntityId == claim.Id).ToList();
                }
            }

            return claims;
        }

        public async Task<IEnumerable<ClaimEntity>> GetAllClaimsAsync()
        {
            var claims = await _db.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .ToListAsync();

            if (claims.Any())
            {
                var claimIds = claims.Select(c => c.Id).ToList();

                var allDocs = await _db.Documents
                    .Where(d => claimIds.Contains(d.LinkedEntityId!.Value) && d.LinkedToEntity == "Claim")
                    .ToListAsync();

                foreach (var claim in claims)
                {
                    claim.Documents = allDocs.Where(d => d.LinkedEntityId == claim.Id).ToList();
                }
            }

            return claims;
        }

        public async Task<bool> UpdateClaimStatusAsync(Guid id, string newStatus)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return false;

            // Enforce valid transitions
            var allowed = AllowedTransitions.ContainsKey(claim.Status)
                          ? AllowedTransitions[claim.Status]
                          : Array.Empty<string>();

            if (!allowed.Contains(newStatus))
                throw new InvalidOperationException($"Invalid status transition from {claim.Status} to {newStatus}");

            claim.Status = newStatus;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ClaimEntity?> UpdateClaimAsync(Guid id, ClaimEntity updatedClaim)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return null;

            // Check if claim is editable within 24 hours
            if ((DateTime.UtcNow - claim.SubmittedAt).TotalHours > 24)
                return null;

            claim.ClaimReason = updatedClaim.ClaimReason;
            await _db.SaveChangesAsync();
            return claim;
        }
        public async Task<bool> DeleteClaimAsync(Guid id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return false;

            // Only allow deletion if status is "Submitted"
            if (claim.Status != "Submitted")
                throw new InvalidOperationException("Only claims with status 'Submitted' can be deleted.");

            // Delete related documents
            var docs = await _db.Documents
                .Where(d => d.LinkedToEntity == "Claim" && d.LinkedEntityId == id)
                .ToListAsync();

            foreach (var doc in docs)
            {
                if (File.Exists(doc.Url))
                    File.Delete(doc.Url);
                _db.Documents.Remove(doc);
            }

            _db.Claims.Remove(claim);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}