using Microsoft.AspNetCore.Identity;
using Models.Events;
using Models.People;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DataBaseLinks
{
    public class EventUserRole
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }


        public static EventUserRole Create(Event ev, User eq, Role role)
            => new EventUserRole
            {
                EventId = ev.Id,
                Event = ev,
                UserId = eq.Id,
                User = eq,
                RoleId = role.Id,
                Role = role
            };
    }
}
