using System;
using System.Collections.Generic;
using Models.PublicAPI.Responses.Event;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class ShiftCreateRequest
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<PlaceCreateRequest> Places { get; set; }
    }
}