using System;
using System.Collections.Generic;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PlaceCreateRequest
    {
        public int TargetParticipantsCount { get; set; }
        public List<Guid> Equipment { get; set; } = new List<Guid>();
        public List<PersonWorkRequest> Invited { get; set; } = new List<PersonWorkRequest>();
    }
}
