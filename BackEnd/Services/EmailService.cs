using BackEnd.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Services
{
    public class EmailService : IEmailSender
    {
        public IConfiguration Configuration { get; }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", 
                Configuration.GetSection("EmailSettings")["Email"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var admin = new SmtpClient())
            {
                await admin.ConnectAsync("smtp.yandex.ru", 465, true);
                await admin.AuthenticateAsync(Configuration.GetSection("EmailSettings")["Email"],
                    Configuration.GetSection("EmailSettings")["Password"]);
                await admin.SendAsync(emailMessage);
                await admin.DisconnectAsync(true);
            }
        }
        public async Task SendEmailConfirm(string email, string url)
        {
            await SendEmailAsync(email, "Подтверждение регистрации", $"<a href=\"{url}\">Подтверждение почты</a>");
        }
    }
}
