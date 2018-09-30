using BackEnd.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using BackEnd.Models.Settings;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace BackEnd.Services
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSenderSettings options;

        public EmailService(IOptions<EmailSenderSettings> options)
        {
            this.options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(options.Email),
                IsBodyHtml = true
            };
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = message;

            SmtpClient client = new SmtpClient
            {
                Host = options.SmtpHost,
                Port = options.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(options.Email, options.Password)
            };
            
            await client.SendMailAsync(mail);
        }
        public async Task SendInvitationEmail(string email, string url, string accessToken)
        {
            var template = (await GetInvitationTemplate())
                .Replace("%email%", email)
                .Replace("%url%", url)
                .Replace("%code%", accessToken);
            await SendEmailAsync(email, "Приглашение на регистрацию", template);
        }

        public async Task SendResetPasswordEmail(string email, string url, string resetPassToken)
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
