using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerDocumentRepository _documentRepo, IDocumentService _documentService) : ControllerBase
    {

        #region Customer APIs

        [Authorize(Roles = "Customer")]
        [HttpPost("upload-documents")]
        public async Task<IActionResult> UploadCustomerDocuments(IFormFileCollection files)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized(new { message = "Please login to use this feature." });

            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files provided" });

            var documents = await _documentService.SaveDocumentsAsync(files, userId, "CustomerProfile", Guid.Parse(userId));
            return Ok(new { message = "Documents uploaded successfully", count = documents.Count });
        }


        [Authorize(Roles = "Customer")]
        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized(new { message = "You need to login for this feature."});

            var documents = await _documentRepo.GetDocumentsByCustomerAsync(userId);
            return Ok(documents.Select(d => new { d.Id, d.FileName, d.Url, d.UploadedAt }));
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete("my-documents/{id}")]
        public async Task<IActionResult> DeleteMyDocument(Guid id)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized(new {message = "You need to login for this feature."});

            var deleted = await _documentRepo.DeleteDocumentAsync(id, userId);
            if (!deleted) return NotFound(new {message = $"Document with Id: {id} not found."});

            return Ok(new { message = $"Document with Id: {id} deleted successfully."});
        }

        #endregion

        #region Admin / Staff APIs

        [Authorize(Roles = "Admin, Editor, ClaimsOfficer, Agent")]
        [HttpGet("{customerId}/documents")]
        public async Task<IActionResult> GetDocumentsByCustomer(string customerId)
        {
            if (string.IsNullOrEmpty(customerId)) return BadRequest(new { mesasge = "There are no documents for this user."});

            var documents = await _documentRepo.GetDocumentsByCustomerAsync(customerId);
            return Ok(documents.Select(d => new { d.Id, d.FileName, d.Url, d.UploadedAt, d.UploadedById }));
        }

        [Authorize(Roles = "Admin, Editor, ClaimsOfficer")]
        [HttpGet("{customerId}/documents/{documentId}")]
        public async Task<IActionResult> GetCustomerDocumentById(string customerId, Guid documentId)
        {
            if (string.IsNullOrEmpty(customerId)) return BadRequest(new { message = "There are no documents for this user."});

            var documents = await _documentRepo.GetDocumentsByCustomerAsync(customerId);
            var document = documents.FirstOrDefault(d => d.Id == documentId);

            if (document == null) return NotFound(new { message = "No documents found."});

            return Ok(new { document.Id, document.FileName, document.Url, document.UploadedAt, document.UploadedById });
        }

        #endregion
    }
}
