using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class PlaceCreateRequest
    {
        [CompareWith(0, Criterion.MoreOrEqual)]
        public int TargetParticipantsCount { get; set; }
        public List<Guid> Equipment { get; set; } = new List<Guid>();
        public List<PersonWorkRequest> Invited { get; set; } = new List<PersonWorkRequest>();
        public string Description { get; set; }
    }
}
