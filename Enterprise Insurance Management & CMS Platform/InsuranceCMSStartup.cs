using Microsoft.Extensions.DependencyInjection;
using Enterprise_Insurance_Management___CMS_Platform.Services;
using Enterprise_Insurance_Management___CMS_Platform.Interfaces;
using Enterprise_Insurance_Management___CMS_Platform.Repositories;
using Enterprise_Insurance_Management___CMS_Platform.BackgroundServices;
public static class InsuranceCMSStartup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<ICustomerPolicyRepository, CustomerPolicyRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ICustomerDocumentRepository, CustomerDocumentRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<JobTriggerService>();
        services.AddScoped<JobService>();
        services.AddHostedService<PolicyExpiryBackgroundService>();
        services.AddScoped<IDocumentService, DocumentService>();
    }
}