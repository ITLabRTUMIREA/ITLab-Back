using System;
namespace Models.PublicAPI.Requests.User
{
    public class InviteUserRequest
    {
        public string Email { get; set; }
        public string RedirectUrl { get; set; }
    }
}
