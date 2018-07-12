using System;
using System.Collections.Generic;
using System.Text;
using Models.PublicAPI.Responses.People;

namespace Models.PublicAPI.Responses.Event
{
    public class UserAndRole
    {
        public UserView User { get; set; }
        public string Role { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
