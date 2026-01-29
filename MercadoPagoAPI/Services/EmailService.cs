using Microsoft.Extensions.Options;
using Resend;

namespace MercadoPagoAPI.Services
{
    public class EmailSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ReplyTo { get; set; }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IResend _resend;
        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IResend resend)
        {
            _settings = settings.Value;
            _logger = logger;
            _resend = resend;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new EmailMessage();
                message.From = $"{_settings.FromName} <{_settings.FromEmail}>";
                message.To.Add(toEmail);
                message.ReplyTo = _settings.ReplyTo;
                message.Subject = subject;
                message.HtmlBody = body;

                await _resend.EmailSendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}
