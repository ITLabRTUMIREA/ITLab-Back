using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Events.Event.Edit
{
    public class ShiftEditRequest : DeletableRequest
    {
        [CompareWith(nameof(EndTime), Criterion.LessOrEqual)]
        public DateTime? BeginTime { get; set; }
        [CompareWith(nameof(BeginTime), Criterion.MoreOrEqual)]
        public DateTime? EndTime { get; set; }
        
        public List<PlaceEditRequest> Places { get; set; } = new List<PlaceEditRequest>();
        public string Description { get; set; }
    }
}