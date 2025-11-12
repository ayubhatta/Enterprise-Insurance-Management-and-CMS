using Enterprise_Insurance_Management___CMS_Platform.Entities;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerPolicyController(ICustomerPolicyRepository _repo, JobTriggerService triggerService) : ControllerBase
    {
        [Authorize(Roles = "Customer")]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePolicy(Guid policyId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var policy = await _repo.GetPolicyByIdAsync(policyId);
            if (policy == null || !policy.IsActive)
                return BadRequest(new { message = "This policy is not available for purchase." });

            var existing = await _repo.GetCustomerPolicyAsync(userId, policyId);
            if (existing != null)
                return BadRequest(new { message = "You already purchased this policy." });

            var purchase = new CustomerPolicy
            {
                CustomerId = userId,
                PolicyId = policyId,
                IsPaymentDone = false,
                PurchasedAt = DateTime.UtcNow
            };

            var success = await _repo.PurchasePolicyAsync(purchase);
            if (!success)
                return BadRequest(new { message = $"Failed to purchase policy: {policyId}." });

            triggerService.TriggerPolicyPurchasedJob(purchase.Id, TimeSpan.Zero);

            var result = new
            {
                message = "Policy purchased successfully. Awaiting payment.",
                purchase = new
                {
                    purchase.Id,
                    purchase.CustomerId,
                    purchase.PolicyId,
                    purchase.PurchasedAt,
                    purchase.IsPaymentDone
                }
            };
            return Ok(result);
        }


        [Authorize(Roles = "Customer")]
        [HttpPatch("{id}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var record = await _repo.GetPurchaseByIdAsync(id);

            if (record == null) return NotFound(new { message = "Purchase not found." });
            if (record.CustomerId != userId) return Forbid("You can only confirm payment for your own purchases.");

            var success = await _repo.UpdatePaymentStatusAsync(id, true);
            if (!success) return BadRequest(new { message = "Failed to update payment status." });

            triggerService.TriggerPolicyPaymentConfirmedJob(record.Id, TimeSpan.Zero); 

            return Ok(new { message = "Payment confirmed successfully." });
        }



        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPolicies()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized(new { message = "The user can only access their own policies."});

            var policies = await _repo.GetPoliciesByCustomerAsync(userId); 
            return Ok(new { message = "The users policies are listed below.", data = policies});
        }
    }
}