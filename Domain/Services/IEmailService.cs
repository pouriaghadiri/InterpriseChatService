using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendEmailAsync(string to, string subject, string body, List<string> cc, List<string> bcc, bool isHtml = true);
        Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string resetLink);
        Task<bool> SendEmailVerificationEmailAsync(string to, string verificationToken, string verificationLink);
    }
}

