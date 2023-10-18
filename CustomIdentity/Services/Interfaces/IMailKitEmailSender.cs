using Microsoft.AspNetCore.Identity.UI.Services;

namespace CustomIdentity.Services.Interfaces
{
    public interface IMailKitEmailSender : IEmailSender
    {
        public Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlBody);

        public bool IsValid(string email);
    }
}
