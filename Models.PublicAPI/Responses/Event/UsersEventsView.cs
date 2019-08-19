using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event
{
    public class UsersEventsView
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public EventTypeView EventType { get; set; }
        public DateTime BeginTime { get; set; }
        public EventRoleView Role { get; set; }
    }
}
