using System;
using System.ComponentModel.DataAnnotations;
namespace Models.PublicAPI.Requests.Account
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
