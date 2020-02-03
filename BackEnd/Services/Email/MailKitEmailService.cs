using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Email
{
    public abstract class MailKitEmailService : IEmailSender
    {
        protected readonly EmailSenderSettings options;
        private readonly ILogger<MailKitEmailService> logger;

        public MailKitEmailService(
            IOptions<EmailSenderSettings> options,
            ILogger<MailKitEmailService> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart("html")
                {
                    Text = message
                }
            };
            mailMessage.From.Add(new MailboxAddress(options.Name, options.Email));
            mailMessage.To.Add(new MailboxAddress("Some", email));


            using (var client = new SmtpClient())
            {
                logger.LogDebug($"Connecting to smtp {options.SmtpHost}:{options.SmtpPort}");
                //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(options.SmtpHost, options.SmtpPort, true);
                logger.LogDebug($"Connected");

                await client.AuthenticateAsync(options.Email, options.Password);
                logger.LogDebug($"Authenticated");

                await client.SendAsync(mailMessage);
                logger.LogDebug($"Sended");
                await client.DisconnectAsync(true);
            }
        }

        public abstract Task SendInvitationEmail(string email, string url, string accessToken);
        public abstract Task SendResetPasswordEmail(string email, string url, string resetPassToken);
    }
}
