using Enterprise_Insurance_Management___CMS_Platform.Data;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Insurance_Management___CMS_Platform.BackgroundServices
{
    public class PolicyExpiryBackgroundService(IServiceProvider _serviceProvider, ILogger<PolicyExpiryBackgroundService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PolicyExpiryBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.UtcNow;
                    var expiredPolicies = await db.Policies
                        .Where(p => p.IsActive && p.ExpiryDate <= now)
                        .ToListAsync(stoppingToken);

                    if (expiredPolicies.Count > 0)
                    {
                        foreach (var policy in expiredPolicies)
                        {
                            policy.IsActive = false;
                            _logger.LogInformation("Policy {Title} expired at {Time}.", policy.Title, now);

                            var users = await db.Users.ToListAsync(stoppingToken);
                            foreach (var user in users)
                            {
                                var mail = new MailRequestHelper
                                {
                                    To = user.Email,
                                    Subject = $"Policy Expired: {policy.Title}",
                                    Body = $@"
                                        <p>Dear {user.UserName},</p>
                                        <p>The policy <strong>{policy.Title}</strong> has expired and cannot be purchased anymore until admin reviews or updates it.</p>
                                        <p>Regards,<br/>Insurance Management System</p>"
                                };
                                await emailService.SendEmailAsync(mail);
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("{Count} policies marked expired and alerts sent.", expiredPolicies.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking policy expirations.");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("PolicyExpiryBackgroundService stopped.");
        }
    }
}
