using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface IDocumentRepository
    {
        Task AddDocumentsAsync(List<DocumentEntity> documents);
        Task<List<DocumentEntity>> GetDocumentsAsync(string linkedToEntity, Guid linkedEntityId);
    }
}
