using Enterprise_Insurance_Management___CMS_Platform.Helpers;

namespace Enterprise_Insurance_Management___CMS_Platform.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequestHelper mailRequest);
    }
}
