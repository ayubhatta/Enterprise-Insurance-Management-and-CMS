using Enterprise_Insurance_Management___CMS_Platform.DTOs;
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
    
    public class PolicyController(IPolicyRepository _repo, IDocumentRepository _documentRepo, JobTriggerService triggerService) : ControllerBase
    {

        [Authorize(Roles = "Admin, Editor, Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var policies = await _repo.GetAllAsync();
            if (policies == null || !policies.Any()) return NotFound(new { message = "No policies found."});
            return Ok(new { message = $"Policies found successfully.", data = policies });
        }

        [Authorize(Roles = "Admin, Editor, Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var policy = await _repo.GetByIdAsync(id);
            if (policy == null) return NotFound(new { message = $"No policy found for Id: {id}."});
            return Ok(new { message = $"Policy with Id: {id} found successfully.", data = policy });
        }

        [Authorize(Roles = "Editor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PolicyDto dto, [FromForm] List<IFormFile>? files)
        {
            var policy = new Policy
            {
                Title = dto.Title,
                Category = dto.Category,
                Description = dto.Description,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsActive = false,
                Version = dto.Version
            };

            var createdPolicy = await _repo.CreateAsync(policy);

            var uploadedFiles = new List<DocumentEntity>();
            if (files != null && files.Count != 0)
            {
                foreach (var file in files)
                {
                    var fileType = FileHelper.GetFileType(file.FileName);
                    var uploadPath = FileHelper.GetUploadPath("Policy", fileType);

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(new DocumentEntity
                    {
                        FileName = file.FileName,
                        Url = filePath, 
                        UploadedById = User.FindFirstValue("uid")!,
                        LinkedToEntity = "Policy",
                        LinkedEntityId = createdPolicy.Id
                    });
                }

                await _documentRepo.AddDocumentsAsync(uploadedFiles);
            }

            triggerService.TriggerPolicyCreatedJob(createdPolicy.Id, TimeSpan.Zero);

            var result = new
            {
                message = "Policy created successfully.",
                policy = new PolicyPostResponseDto
                {
                    Id = createdPolicy.Id,
                    Title = createdPolicy.Title,
                    Category = createdPolicy.Category,
                    Description = createdPolicy.Description,
                    CreatedAt = createdPolicy.CreatedAt,
                    ExpiryDate = createdPolicy.ExpiryDate,
                    IsActive = createdPolicy.IsActive,
                    Version = createdPolicy.Version
                },
                files = uploadedFiles.Select(f => new { f.FileName, f.Url })
            };

            return CreatedAtAction(nameof(GetById), new { id = createdPolicy.Id }, result);
        }


        [Authorize(Roles = "Admin, Editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] PolicyDto dto, [FromForm] List<IFormFile>? files)
        {
            var policy = await _repo.GetPolicyEntityByIdAsync(id);
            if (policy == null) return NotFound();

            policy.Title = dto.Title;
            policy.Category = dto.Category;
            policy.Description = dto.Description;

            await _repo.UpdateAsync(id, policy);

            var uploadedFiles = new List<DocumentEntity>();
            if (files != null && files.Count != 0)
            {
                foreach (var file in files)
                {
                    var fileType = FileHelper.GetFileType(file.FileName);
                    var uploadPath = FileHelper.GetUploadPath("Policy", fileType);

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(new DocumentEntity
                    {
                        FileName = file.FileName,
                        Url = filePath,
                        UploadedById = User.FindFirstValue("uid")!,
                        LinkedToEntity = "Policy",
                        LinkedEntityId = policy.Id
                    });
                }

                await _documentRepo.AddDocumentsAsync(uploadedFiles);
            }

            var result = new
            {
                message = "Policy updated successfully.",
                policy = new PolicyPostResponseDto
                {
                    Id = policy.Id,
                    Title = policy.Title,
                    Category = policy.Category,
                    Description = policy.Description,
                    CreatedAt = policy.CreatedAt,
                    ExpiryDate = policy.ExpiryDate,
                    IsActive = policy.IsActive,
                    Version = policy.Version
                },
                files = uploadedFiles.Select(f => new { f.FileName, f.Url })
            };

            return Ok(result);
        }


        [Authorize(Roles = "Admin, Editor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Policy with Id: {id} not found."});
            return Ok(new { message = $"Policy with Id: {id} deleted successfully."});
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> VerifyPolicy(Guid id)
        {
            var policy = await _repo.GetPolicyEntityByIdAsync(id);
            if (policy == null) return NotFound(new {message = $"No policy found for Id: {id}" });

            policy.IsActive = true;
            await _repo.UpdateAsync(id, policy);

            var result = new PolicyResponseDto
            {
                Id = policy.Id,
                Title = policy.Title,
                Category = policy.Category,
                Description = policy.Description,
                CreatedAt = policy.CreatedAt,
                ExpiryDate = policy.ExpiryDate,
                IsActive = policy.IsActive,
                Version = policy.Version
            };

            return Ok(new { message = $"Policy verified for Id: {id}, and now active.", policy = result });
        }
    }
}
