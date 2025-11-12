using System.Net;
using System.Net.Mail;
using Enterprise_Insurance_Management___CMS_Platform.Helpers;
using Microsoft.Extensions.Options;

namespace Enterprise_Insurance_Management___CMS_Platform.Services
{
    public class EmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings emailSettings = options.Value;

        public async Task SendEmailAsync(MailRequestHelper mailRequest)
        {
            if (!emailSettings.UseSSL)
            {
                throw new InvalidOperationException("Email cannot be sent because SSL is not enabled.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings.Email!, emailSettings.DisplayName),
                Subject = mailRequest.Subject,
                Body = mailRequest.Body,
                IsBodyHtml = true 
            };

            mailMessage.To.Add(mailRequest.To!);

            using var smtpClient = new SmtpClient
            {
                Host = emailSettings.Host!,
                Port = emailSettings.Port,
                EnableSsl = emailSettings.UseSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.Email, emailSettings.Password)
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email: " + ex.Message);
            }
        }
    }
}
