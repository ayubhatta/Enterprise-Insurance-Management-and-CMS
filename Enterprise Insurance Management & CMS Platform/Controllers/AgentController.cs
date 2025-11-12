using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Insurance_Management___CMS_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Agent")]
public class AgentController(IAgentRepository _repo, JobTriggerService triggerService) : ControllerBase
{
    [HttpGet("unverified-customers")]
    public async Task<IActionResult> GetUnverifiedCustomers()
    {
        var customers = await _repo.GetUnverifiedCustomersAsync();
        return Ok(customers);
    }

    [HttpPatch("verify-customer/{id}")]
    public async Task<IActionResult> VerifyCustomer(string id)
    {
        var verified = await _repo.VerifyCustomerAsync(id);
        if (!verified) return NotFound(new { message = "Customer not found" });

        triggerService.TriggerCustomerVerifiedJob(id, TimeSpan.Zero);

        return Ok(new { message = "Customer verified successfully" });
    }

    [HttpGet("customer/{id}")]
    public async Task<IActionResult> GetCustomerProfile(string id)
    {
        var profile = await _repo.GetCustomerProfileByIdAsync(id);
        if (profile == null) return NotFound(new { message = "Customer not found" });

        return Ok(profile);
    }
}
