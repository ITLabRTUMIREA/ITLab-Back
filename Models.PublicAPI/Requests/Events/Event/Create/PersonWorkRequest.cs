using System;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PersonWorkRequest : DeletableRequest
    {
        public Guid RoleId { get; set; }
    }
}
