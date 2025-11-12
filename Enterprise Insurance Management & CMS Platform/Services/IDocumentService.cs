using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Services
{
    public interface IDocumentService
    {
        Task<List<DocumentEntity>> SaveDocumentsAsync(IFormFileCollection files, string uploadedById, string linkedToEntity, Guid linkedEntityId);
    }
}