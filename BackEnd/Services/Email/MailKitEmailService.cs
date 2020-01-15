using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using MailKit.Net.Smtp;
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

        public MailKitEmailService(IOptions<EmailSenderSettings> options)
        {
            this.options = options.Value;
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
             //   client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(options.SmtpHost, options.SmtpPort, true);

                client.Authenticate(options.Email, options.Password);

                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public abstract Task SendInvitationEmail(string email, string url, string accessToken);
        public abstract Task SendResetPasswordEmail(string email, string url, string resetPassToken);
    }
}
