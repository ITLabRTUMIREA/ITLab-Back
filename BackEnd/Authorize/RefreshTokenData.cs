using System;

namespace BackEnd.Authorize
{

    public class RefreshTokenData
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string UserAgent { get; set; }
    }

}

