using System;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PersonWorkRequest : DeletableRequest
    {
        public Guid EventRoleId { get; set; }
    }
}
