using System;
using System.Collections.Generic;
using System.Text;
using Models.Events;
using Models.People;

namespace Models.DataBaseLinks
{
    public class PlaceUserRole
    {
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
