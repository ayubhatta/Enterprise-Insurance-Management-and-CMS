using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;

namespace Enterprise_Insurance_Management___CMS_Platform.Services
{
    public class DocumentService(IDocumentRepository documentRepo) : IDocumentService
    {
        public async Task<List<DocumentEntity>> SaveDocumentsAsync(
            IFormFileCollection files,
            string uploadedById,
            string linkedToEntity,
            Guid linkedEntityId
        )
        {
            var documents = new List<DocumentEntity>();

            foreach (var file in files)
            {
                var fileType = FileHelper.GetFileType(file.FileName);
                var uploadPath = FileHelper.GetUploadPath(linkedToEntity, fileType);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = File.Create(filePath);
                await file.CopyToAsync(stream);

                documents.Add(new DocumentEntity
                {
                    FileName = file.FileName,
                    Url = filePath,
                    UploadedById = uploadedById,
                    LinkedToEntity = linkedToEntity,
                    LinkedEntityId = linkedEntityId
                });
            }

            await documentRepo.AddDocumentsAsync(documents);
            return documents;
        }
    }
}