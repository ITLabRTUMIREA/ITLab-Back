using System;
using System.Collections.Generic;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PlaceCreateRequest
    {
        public int TargetParticipantsCount { get; set; }
        public List<Guid> Equipment { get; set; }
        public List<PersonWorkRequest> Workers { get; set; }
    }
}
