using System;
namespace Models.PublicAPI.Requests.Account
{
    public class RequestResetPasswordRequest
    {
        public string Email { get; set; }
        public string RedirectUrl { get; set; }
    }
}
