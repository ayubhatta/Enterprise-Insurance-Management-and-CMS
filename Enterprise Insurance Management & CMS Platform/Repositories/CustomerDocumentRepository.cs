using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class CustomerDocumentRepository(AppDbContext _db) : ICustomerDocumentRepository
    {
        public async Task AddDocumentsAsync(IEnumerable<DocumentEntity> documents)
        {
            _db.Documents.AddRange(documents);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<DocumentEntity>> GetDocumentsByCustomerAsync(string customerId)
        {
            return await _db.Documents
                .Where(d => d.LinkedToEntity == "CustomerProfile" && d.LinkedEntityId == Guid.Parse(customerId))
                .ToListAsync();
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId, string customerId)
        {
            var doc = await _db.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId
                                          && d.LinkedToEntity == "CustomerProfile"
                                          && d.LinkedEntityId == Guid.Parse(customerId));

            if (doc == null) return false;

            if (File.Exists(doc.Url))
                File.Delete(doc.Url);

            _db.Documents.Remove(doc);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
