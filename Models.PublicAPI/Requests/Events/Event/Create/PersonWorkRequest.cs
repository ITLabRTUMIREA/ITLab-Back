using System;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PersonWorkRequest : IdRequest
    {
        public Guid RoleId { get; set; }
    }
}
