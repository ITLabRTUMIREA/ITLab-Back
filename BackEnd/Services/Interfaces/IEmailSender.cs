using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendInvitationEmail(string email, string url, string accessToken);
        Task SendResetPasswordEmail(string email, string url, string resetPassToken);
    }
}
