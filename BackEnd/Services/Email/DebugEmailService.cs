using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackEnd.Services.Email
{
    public class DebugEmailService : IEmailSender
    {
        private readonly ILogger<DebugEmailService> logger;

        public DebugEmailService(ILogger<DebugEmailService> logger)
        {
            this.logger = logger;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            logger.LogDebug($"sending email to {email} subject: {subject} message: {message}");
            return Task.CompletedTask;
        }

        public Task SendInvitationEmail(string email, string url, string accessToken)
        {
            logger.LogDebug($"sending invitation email to {email}  url: {url} token: {accessToken}");
            return Task.CompletedTask;
        }

        public Task SendResetPasswordEmail(string email, string url, string resetPassToken)
        {
            logger.LogDebug($"sending reset password email to {email} url: {url} token: {resetPassToken}");
            return Task.CompletedTask;
        }
    }
}
