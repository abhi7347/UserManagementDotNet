using DomainLayer.DTOs;
using IServiceLayer.EmailSetting;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;


        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public string Email
        {
            get { return _config["EmailSettings:Email"]; }
        }
        private string Password => _config["EmailSettings:Password"];
        private string Host => _config["EmailSettings:Host"];
        private string DisplayName => _config["EmailSettings:DisplayName"];
        private int Port => int.Parse(_config["EmailSettings:Port"]);

        public async Task SendEmailAsync(string toEmail, string subject, string resetLink, string htmlBody, string password = null)
        {
            
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SharedLayer", "Templates", htmlBody);
            var templateContent = await File.ReadAllTextAsync(templatePath);
            var resetLinkWithEmail = $"{resetLink}?email={toEmail}";
            var htmlMessage = templateContent.Replace("{ResetLink}", resetLinkWithEmail).Replace("{pass}", password);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(DisplayName, Email));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(Email, Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}

