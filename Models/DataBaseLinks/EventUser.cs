using Models.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DataBaseLinks
{
    public class EventUser
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public static EventUser Create(Event ev, User eq)
            => new EventUser
            {
                EventId = ev.Id,
                Event = ev,
                UserId = eq.Id,
                User = eq
            };
    }
}
