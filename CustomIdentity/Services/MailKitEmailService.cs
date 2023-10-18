using Microsoft.Extensions.Options;
using CustomIdentity.Services.Interfaces;
using CustomIdentity.Models.View;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CustomIdentity.Services
{
    public class MailKitEmailService : IMailKitEmailSender
    {
        private readonly MailSettingsVM _mailSettings;

        public MailKitEmailService(IOptions<MailSettingsVM> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public bool IsValid(string email)
        {
            try
            {
                // well known .Net style
                // var netMailAddress = new System.Net.Mail.MailAddress(mail);

                // Mailkit 3.4 style
                var kitMailAddress = new MailboxAddress(null, email);
                return true;
            }
            catch
            { 
                return false; 
            }
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlBody}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }
    }
}
