using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.INFRASTRUCTURE.Configurations;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        // Nhận EmailSettings qua DI
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var email = new MimeMessage();

            // Cấu hình người gửi
            email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            // Cấu hình nội dung (HTML)
            var builder = new BodyBuilder
            {
                HtmlBody = content,
                TextBody = content
            };
            email.Body = builder.ToMessageBody();

            // Gửi email qua SMTP
            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);

                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}