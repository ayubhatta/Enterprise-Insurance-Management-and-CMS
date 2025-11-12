using Microsoft.EntityFrameworkCore;
using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Enterprise_Insurance_Management___CMS_Platform.Services;

namespace Enterprise_Insurance_Management___CMS_Platform.BackgroundServices
{
    public class JobService(AppDbContext _context, ILogger<JobService> _logger, IEmailService _emailService)
    {
        public async Task NewUserRegisteredJob(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.Email)) return;

                // Send Welcome Email
                var customerMail = new MailRequestHelper
                {
                    To = user.Email,
                    Subject = "Welcome to Enterprise Insurance Management Platform",
                    Body = $"<p>Dear {user.UserName},</p><p>Welcome to our Insurance platform! We’re glad to have you onboard.</p><p>Our agent will verify your profile soon.</p><p>Regards,<br/>Enterprise Insurance Team</p>"
                };
                await _emailService.SendEmailAsync(customerMail);

                // Notify Agent for Verification
                var agent = await _context.Users
                    .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, r })
                    .Where(x => x.r.Name == "Agent")
                    .Select(x => x.u)
                    .FirstOrDefaultAsync();

                if (agent != null)
                {
                    var agentMail = new MailRequestHelper
                    {
                        To = agent.Email,
                        Subject = "New Customer Verification Required",
                        Body = $"<p>Dear Agent,</p><p>A new customer <strong>{user.UserName}</strong> with Id: <strong>{user.Id}, has registered and requires verification.</p><p>Please review their profile and documents.</p>"
                    };
                    await _emailService.SendEmailAsync(agentMail);
                }

                _logger.LogInformation($"Welcome and agent verification emails sent for user {user.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in NewUserRegisteredJob for {userId}: {ex.Message}");
            }
        }

        public async Task CustomerVerifiedJob(string userId)
        {
            try
            {
                var customer = await _context.Users.FindAsync(userId);
                if (customer == null || string.IsNullOrEmpty(customer.Email)) return;

                var mail = new MailRequestHelper
                {
                    To = customer.Email,
                    Subject = "Your Account Has Been Verified",
                    Body = $"<p>Dear {customer.UserName},</p><p>Your account has been successfully verified.</p><p>You are now eligible to buy policies and file claims.</p><p>Regards,<br/>Enterprise Insurance Team</p>"
                };

                await _emailService.SendEmailAsync(mail);
                _logger.LogInformation($"Customer verified email sent to {customer.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CustomerVerifiedJob for {userId}: {ex.Message}");
            }
        }

        public async Task PolicyCreatedJob(Guid policyId)
        {
            try
            {
                var policy = await _context.Policies.FindAsync(policyId);
                if (policy == null) return;

                var admin = await _context.Users
                    .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, r })
                    .Where(x => x.r.Name == "Admin")
                    .Select(x => x.u)
                    .FirstOrDefaultAsync();

                if (admin != null)
                {
                    var mail = new MailRequestHelper
                    {
                        To = admin.Email,
                        Subject = "New Policy Created - Requires Review",
                        Body = $"<p>Dear Admin,</p><p>A new policy <strong>{policy.Title}</strong> has been created.</p><p>Please review and approve it.</p>"
                    };

                    await _emailService.SendEmailAsync(mail);
                    _logger.LogInformation($"Policy review email sent to {admin.Email}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PolicyCreatedJob for PolicyId {policyId}: {ex.Message}");
            }
        }

        public async Task PolicyPurchasedJob(Guid customerPolicyId)
        {
            try
            {
                var purchase = await _context.CustomerPolicies
                    .Include(cp => cp.Policy)
                    .Include(cp => cp.Customer)
                    .FirstOrDefaultAsync(cp => cp.Id == customerPolicyId);

                if (purchase == null || purchase.Customer == null) return;

                var mail = new MailRequestHelper
                {
                    To = purchase.Customer.Email,
                    Subject = "Policy Purchase Confirmation",
                    Body = $"<p>Dear {purchase.Customer.UserName},</p><p>You have successfully purchased the policy. Awaiting payment. <strong>{purchase.Policy!.Title}</strong>.</p><p>Thank you for trusting us!</p>"
                };

                await _emailService.SendEmailAsync(mail);
                _logger.LogInformation($"Policy purchased email sent to {purchase.Customer.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PolicyPurchasedJob for PurchaseId {customerPolicyId}: {ex.Message}");
            }
        }

        public async Task PolicyPaymentConfirmedJob(Guid customerPolicyId)
        {
            try
            {
                var purchase = await _context.CustomerPolicies
                    .Include(cp => cp.Policy)
                    .Include(cp => cp.Customer)
                    .FirstOrDefaultAsync(cp => cp.Id == customerPolicyId);

                if (purchase == null || purchase.Customer == null) return;

                var mail = new MailRequestHelper 
                { 
                    To = purchase.Customer.Email, 
                    Subject = "Payment Confirmation", 
                    Body = $"<p>Dear {purchase.Customer.UserName}," +
                    $"</p><p>Your payment for the policy <strong>{purchase.Policy!.Title}" +
                    $"</strong> has been received successfully." +
                    $"</p><p>Regards,<br/>Enterprise Insurance Team</p>" };

                await _emailService.SendEmailAsync(mail);
                _logger.LogInformation($"Payment confirmation email sent to {purchase.Customer.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PolicyPaymentConfirmedJob for {customerPolicyId}: {ex.Message}");
            }
        }
        public async Task ClaimSubmittedJob(Guid claimId)
        {
            try
            {
                var claim = await _context.Claims
                    .Include(c => c.Customer)
                    .Include(c => c.Policy)
                    .FirstOrDefaultAsync(c => c.Id == claimId);

                if (claim == null) return;

                var claimsOfficer = await _context.Users
                    .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, r })
                    .Where(x => x.r.Name == "ClaimsOfficer")
                    .Select(x => x.u)
                    .FirstOrDefaultAsync();

                if (claimsOfficer != null)
                {
                    var mail = new MailRequestHelper
                    {
                        To = claimsOfficer.Email,
                        Subject = "New Claim Submitted",
                        Body = $"<p>Dear Claims Officer,</p><p>A new claim has been submitted by <strong>{claim.Customer!.UserName}</strong> for the policy <strong>{claim.Policy!.Title}</strong>.</p><p>Please review the claim details.</p>"
                    };

                    await _emailService.SendEmailAsync(mail);
                    _logger.LogInformation($"Claim submitted email sent to {claimsOfficer.Email}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ClaimSubmittedJob for ClaimId {claimId}: {ex.Message}");
            }
        }

        public async Task ClaimStatusUpdatedJob(Guid claimId, string newStatus)
        {
            try
            {
                var claim = await _context.Claims
                    .Include(c => c.Customer)
                    .Include(c => c.Policy)
                    .FirstOrDefaultAsync(c => c.Id == claimId);

                if (claim == null || claim.Customer == null) return;

                string subject, body;
                switch (newStatus)
                {
                    case "UnderReview":
                        subject = "Your Claim is Under Review";
                        body = $"<p>Dear {claim.Customer.UserName},</p><p>Your claim for <strong>{claim.Policy!.Title}</strong> is now under review.</p>";
                        break;

                    case "Approved":
                        subject = "Your Claim Has Been Approved";
                        body = $"<p>Dear {claim.Customer.UserName},</p><p>Good news! Your claim for <strong>{claim.Policy!.Title}</strong> has been approved.</p>";
                        break;

                    case "Rejected":
                        subject = "Your Claim Was Rejected";
                        body = $"<p>Dear {claim.Customer.UserName},</p><p>Unfortunately, your claim for <strong>{claim.Policy!.Title}</strong> was rejected. Please review your documents and try again.</p>";
                        break;

                    default:
                        return;
                }

                var mail = new MailRequestHelper
                {
                    To = claim.Customer.Email,
                    Subject = subject,
                    Body = body + "<p>Regards,<br/>Enterprise Insurance Team</p>"
                };

                await _emailService.SendEmailAsync(mail);
                _logger.LogInformation($"Claim status update email ({newStatus}) sent to {claim.Customer.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ClaimStatusUpdatedJob for ClaimId {claimId}: {ex.Message}");
            }
        }

        public async Task PolicyExpiredAlertJob()
        {
            try
            {
                var expiredPolicies = await _context.Policies
                    .Where(p => p.ExpiryDate <= DateTime.UtcNow && p.IsActive)
                    .ToListAsync();

                if (expiredPolicies.Count == 0) return;

                var users = await _context.Users.ToListAsync();
                foreach (var user in users)
                {
                    var mail = new MailRequestHelper
                    {
                        To = user.Email,
                        Subject = "System Alert: Policy Expired",
                        Body = "<p>Dear User,</p><p>One or more policies have expired in the system.</p><p>Please review and renew as necessary.</p>"
                    };
                    await _emailService.SendEmailAsync(mail);
                }

                _logger.LogInformation($"Policy expiration alert sent to all users.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PolicyExpiredAlertJob: {ex.Message}");
            }
        }
    }
}
