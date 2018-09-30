using System;
namespace BackEnd.Models.Settings
{
    public class EmailSenderSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string InvitationTemplateUrl { get; set; }
        public string ResetPasswordTemplateUrl { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
