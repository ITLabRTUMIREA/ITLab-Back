using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.User.Properties.HardProperties
{
    public class VkVerifyRequest
    {
        public string Token { get; set; }
        public int  VkId { get; set; }
    }
}
