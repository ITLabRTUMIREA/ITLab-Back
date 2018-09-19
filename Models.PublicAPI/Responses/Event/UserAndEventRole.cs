using System;
using System.Collections.Generic;
using System.Text;
using Models.PublicAPI.Responses.People;

namespace Models.PublicAPI.Responses.Event
{
    public class UserAndEventRole
    {
        public UserView User { get; set; }
        public EventRoleView EventRole { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DoneTime { get; set; }
    }
}
