using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackEnd.Services.Email
{
    public class EmailService : MailKitEmailService
    {
        public EmailService(IOptions<EmailSenderSettings> options) : base(options)
        {
        }

        public override async Task SendInvitationEmail(string email, string url, string accessToken)
        {
            var template = (await GetInvitationTemplate())
                .Replace("%email%", email)
                .Replace("%url%", url)
                .Replace("%code%", accessToken);
            await SendEmailAsync(email, "Приглашение на регистрацию", template);
        }

        public override async Task SendResetPasswordEmail(string email, string url, string resetPassToken)
        {
            var template = (await GetResetPasswordTemplate())
                .Replace("%email%", email)
                .Replace("%url%", url)
                .Replace("%code%", resetPassToken);
            await SendEmailAsync(email, "Восстановление пароля", template);
        }

        private Task<string> GetInvitationTemplate()
        {
            return new HttpClient().GetStringAsync(options.InvitationTemplateUrl);
        }
        private Task<string> GetResetPasswordTemplate()
        {
            return new HttpClient().GetStringAsync(options.ResetPasswordTemplateUrl);
        }
    }
}
