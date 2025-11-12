using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.Interfaces
{
    public interface ICustomerDocumentRepository
    {
        Task AddDocumentsAsync(IEnumerable<DocumentEntity> documents);
        Task<IEnumerable<DocumentEntity>> GetDocumentsByCustomerAsync(string customerId);
        Task<bool> DeleteDocumentAsync(Guid documentId, string customerId);
    }
}
