using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class PolicyRepository(AppDbContext _db, IDocumentRepository _docRepo) : IPolicyRepository
    {
        public async Task<IEnumerable<PolicyResponseDto>> GetAllAsync()
        {
            var policies = await _db.Policies.ToListAsync();

            var result = new List<PolicyResponseDto>();
            foreach (var p in policies)
            {
                var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);
                result.Add(new PolicyResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Category = p.Category,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    ExpiryDate = p.ExpiryDate,
                    IsActive = p.IsActive,
                    Version = p.Version,
                    Documents = docs.Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        FileName = d.FileName,
                        Url = d.Url,
                        UploadedAt = d.UploadedAt
                    }).ToList()
                });
            }

            return result;
        }

        public async Task<PolicyResponseDto?> GetByIdAsync(Guid id)
        {
            var p = await _db.Policies.FindAsync(id);
            if (p == null) return null;

            var docs = await _docRepo.GetDocumentsAsync("Policy", p.Id);

            return new PolicyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ExpiryDate = p.ExpiryDate,
                IsActive = p.IsActive,
                Version = p.Version,
                Documents = docs.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Url = d.Url,
                    UploadedAt = d.UploadedAt
                }).ToList()
            };
        }
        public async Task<Policy> CreateAsync(Policy policy)
        {
            _db.Policies.Add(policy);
            await _db.SaveChangesAsync();
            return policy;
        }

        public async Task<Policy?> UpdateAsync(Guid id, Policy updatedPolicy)
        {
            var policy = await _db.Policies.FindAsync(id);
            if (policy == null) return null;

            policy.Title = updatedPolicy.Title;
            policy.Category = updatedPolicy.Category;
            policy.Description = updatedPolicy.Description;

            await _db.SaveChangesAsync();
            return policy;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var policy = await _db.Policies.FindAsync(id);
            if (policy == null) return false;

            var documents = await _db.Documents
                .Where(d => d.LinkedToEntity == "Policy" && d.LinkedEntityId == id)
                .ToListAsync();

            foreach (var doc in documents)
            {
                if (System.IO.File.Exists(doc.Url))
                {
                    System.IO.File.Delete(doc.Url);
                }
            }

            _db.Documents.RemoveRange(documents);
            _db.Policies.Remove(policy);

            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<Policy?> GetPolicyEntityByIdAsync(Guid id)
        {
            return await _db.Policies.FindAsync(id);
        }

    }
}
