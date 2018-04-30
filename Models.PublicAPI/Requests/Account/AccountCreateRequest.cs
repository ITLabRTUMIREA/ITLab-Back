using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Account
{
    public class AccountCreateRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [CorrectEnum(typeof(UserType))]
        public UserType UserType { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [RequiredIF(nameof(UserType), UserType.Student)]
        public string StudentID { get; set; }
    }
}
