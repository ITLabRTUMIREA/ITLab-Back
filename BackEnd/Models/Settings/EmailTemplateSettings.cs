using System;
namespace BackEnd.Models.Settings
{
    /// <summary>
    /// Settings for email templates
    /// </summary>
    public class EmailTemplateSettings
    {
        /// <summary>
        /// Template url for invitation email
        /// </summary>
        public string InvitationTemplateUrl { get; set; }
        /// <summary>
        /// Template url for reset password url
        /// </summary>
        public string ResetPasswordTemplateUrl { get; set; }
    }
}
