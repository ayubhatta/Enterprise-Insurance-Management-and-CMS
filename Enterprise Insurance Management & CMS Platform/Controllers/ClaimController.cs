using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Enterprise_Insurance_Management___CMS_Platform.Services;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController(IClaimRepository _repo, IDocumentRepository _documentRepo, ICustomerPolicyRepository _customerPolicyRepo, JobTriggerService triggerService) : ControllerBase
    {
        [Authorize(Roles = "Admin, ClaimsOfficer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claims = await _repo.GetAllClaimsAsync();
            return Ok(claims.Select(c => new
            {
                c.Id,
                c.PolicyId,
                c.CustomerId,
                c.ClaimReason,
                c.Status,
                c.SubmittedAt,
                Files = c.Documents.Select(d => new { d.FileName, d.Url })
            }));
        }

        [Authorize(Roles = "Admin, ClaimsOfficer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var claim = await _repo.GetClaimByIdAsync(id);
            if (claim == null) return NotFound();

            return Ok(new
            {
                claim.Id,
                claim.PolicyId,
                claim.CustomerId,
                claim.ClaimReason,
                claim.Status,
                claim.SubmittedAt,
                Files = claim.Documents.Select(d => new { d.FileName, d.Url })
            });
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var claims = await _repo.GetClaimsByCustomerAsync(userId);

            var result = claims.Select(c => new
            {
                c.Id,
                c.PolicyId,
                c.CustomerId,
                c.ClaimReason,
                c.Status,
                c.SubmittedAt,
                Files = c.Documents.Select(d => new { d.FileName, d.Url })
            });

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my/{id}")]
        public async Task<IActionResult> GetMyClaimById(Guid id)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var claim = await _repo.GetClaimByIdAsync(id);
            if (claim == null) return NotFound();

            if (claim.CustomerId != userId)
                return Forbid("You can only access your own claims.");

            return Ok(new
            {
                claim.Id,
                claim.PolicyId,
                claim.CustomerId,
                claim.ClaimReason,
                claim.Status,
                claim.SubmittedAt,
                Files = claim.Documents.Select(d => new { d.FileName, d.Url })
            });
        }


        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> SubmitClaim([FromForm] ClaimDto dto, [FromForm] List<IFormFile>? files)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var validationResult = await ValidateClaimSubmission(userId, dto.PolicyId);
            if (validationResult != null) return validationResult;

            var createdClaim = await CreateClaim(userId, dto);

            var uploadedFiles = await UploadClaimFiles(userId, createdClaim.Id, files);

            triggerService.TriggerClaimSubmittedJob(createdClaim.Id, TimeSpan.Zero);

            return Ok(new
            {
                message = "Claim submitted successfully.",
                claim = new
                {
                    createdClaim.Id,
                    createdClaim.PolicyId,
                    createdClaim.CustomerId,
                    createdClaim.ClaimReason,
                    createdClaim.Status,
                    createdClaim.SubmittedAt
                },
                files = uploadedFiles.Select(f => new { f.FileName, f.Url })
            });
        }
        private async Task<IActionResult?> ValidateClaimSubmission(string userId, Guid policyId)
        {
            var customerPolicy = await _customerPolicyRepo.GetCustomerPolicyAsync(userId, policyId);
            if (customerPolicy == null || !customerPolicy.IsPaymentDone)
                return BadRequest(new { message = "You can only submit a claim for policies you have fully paid for." });

            var existingClaims = await _repo.GetClaimsByCustomerAsync(userId);
            var existingClaim = existingClaims.FirstOrDefault(c => c.PolicyId == policyId && c.Status != "Rejected");

            if (existingClaim != null)
            {
                if (existingClaim.Status == "Approved")
                    return BadRequest(new { message = "Claim already approved for this policy." });

                return BadRequest(new { message = "You already have a pending claim for this policy." });
            }

            return null;
        }
        private async Task<ClaimEntity> CreateClaim(string userId, ClaimDto dto)
        {
            var claim = new ClaimEntity
            {
                PolicyId = dto.PolicyId,
                CustomerId = userId,
                ClaimReason = dto.ClaimReason,
                Status = "Submitted",
                SubmittedAt = DateTime.UtcNow
            };

            return await _repo.SubmitClaimAsync(claim);
        }
        private async Task<List<DocumentEntity>> UploadClaimFiles(string userId, Guid claimId, List<IFormFile>? files)
        {
            var uploadedFiles = new List<DocumentEntity>();
            if (files == null || !files.Any()) return uploadedFiles;

            foreach (var file in files)
            {
                var fileType = FileHelper.GetFileType(file.FileName);
                var uploadPath = FileHelper.GetUploadPath("Claim", fileType);
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);

                uploadedFiles.Add(new DocumentEntity
                {
                    FileName = file.FileName,
                    Url = filePath,
                    UploadedById = userId,
                    LinkedToEntity = "Claim",
                    LinkedEntityId = claimId
                });
            }

            await _documentRepo.AddDocumentsAsync(uploadedFiles);
            return uploadedFiles;
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClaim(Guid id, [FromForm] ClaimDto dto)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var updatedClaim = new ClaimEntity { ClaimReason = dto.ClaimReason };
            var result = await _repo.UpdateClaimAsync(id, updatedClaim);

            if (result == null)
                return BadRequest("Claim cannot be updated after 24 hours or does not exist.");

            return Ok(result);
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(Guid id)
        {
            var deleted = await _repo.DeleteClaimAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "ClaimsOfficer, Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string newStatus)
        {
            try
            {
                var success = await _repo.UpdateClaimStatusAsync(id, newStatus);
                if (!success) return NotFound();

                triggerService.TriggerClaimStatusUpdatedJob(id, newStatus, TimeSpan.Zero);

                return Ok(new { message = $"Claim status updated to {newStatus}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}