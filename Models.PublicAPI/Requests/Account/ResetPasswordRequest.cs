using System;
using System.ComponentModel.DataAnnotations;
namespace Models.PublicAPI.Requests.Account
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
