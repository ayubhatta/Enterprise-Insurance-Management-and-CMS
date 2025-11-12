using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.Repositories
{
    public class DocumentRepository(AppDbContext db) : IDocumentRepository
    {
        public async Task AddDocumentsAsync(List<DocumentEntity> documents)
        {
            db.Documents.AddRange(documents);
            await db.SaveChangesAsync();
        }
        public async Task<List<DocumentEntity>> GetDocumentsAsync(string linkedToEntity, Guid linkedEntityId)
        {
            return await db.Documents
                .Where(d => d.LinkedToEntity == linkedToEntity && d.LinkedEntityId == linkedEntityId)
                .ToListAsync();
        }
    }
}
