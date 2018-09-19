using System;
using System.Collections.Generic;
using System.Text;
using Models.DataBaseLinks;

namespace Models.Events.Roles
{
    public class EventRole
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<PlaceUserEventRole> PlaceUserEventRoles { get; set; }
    }
}
