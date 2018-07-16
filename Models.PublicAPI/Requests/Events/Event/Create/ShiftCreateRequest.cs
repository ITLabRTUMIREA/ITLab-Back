using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Events.Event.Create
{
    public class ShiftCreateRequest
    {
        [CompareWith(nameof(EndTime), Criterion.LessOrEqual)]
        public DateTime BeginTime { get; set; }
        [CompareWith(nameof(BeginTime), Criterion.MoreOrEqual)]
        public DateTime EndTime { get; set; }
        public List<PlaceCreateRequest> Places { get; set; }
    }
}