using System;
using System.Collections.Generic;
using System.Text;
using Models.PublicAPI.Responses.People;

namespace Models.PublicAPI.Responses.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserView User { get; set; }
    }
}
