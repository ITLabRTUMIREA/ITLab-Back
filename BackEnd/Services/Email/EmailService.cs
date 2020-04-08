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
    public class EmailService : IEmailSender
    {
        private readonly ILogger<EmailService> logger;
        private readonly EmailTemplateSettings options;
        private readonly RTUITLab.EmailService.Client.IEmailSender rtuItLabEmailSender;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<EmailTemplateSettings> options,
            RTUITLab.EmailService.Client.IEmailSender rtuItLabEmailSender
            )
        {
            this.logger = logger;
            this.options = options.Value;
            this.rtuItLabEmailSender = rtuItLabEmailSender;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return rtuItLabEmailSender.SendEmailAsync(email, subject, message);
        }

        public async Task SendInvitationEmail(string email, string url, string accessToken)
        {
            logger.LogInformation($"Sending invitation email to {email}");
            var template = (await GetInvitationTemplate())
                .Replace("%email%", email)
                .Replace("%url%", url)
                .Replace("%code%", accessToken);
            await SendEmailAsync(email, "Приглашение на регистрацию", template);
        }

        public async Task SendResetPasswordEmail(string email, string url, string resetPassToken)
        {
            logger.LogInformation($"Sending reset password email to {email}");
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
