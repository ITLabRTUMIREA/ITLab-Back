using System;
using System.Collections.Generic;
using System.Text;
using Models.Events;
using Models.Events.Roles;
using Models.People;

namespace Models.DataBaseLinks
{
    public class PlaceUserEventRole
    {
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid EventRoleId { get; set; }
        public EventRole EventRole { get; set; }

        public UserStatus UserStatus { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DoneTime { get; set; }
    }
}
