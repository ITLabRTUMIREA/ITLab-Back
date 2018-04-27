using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EventEditRequest : IdRequest
    {
        [CompareWith(nameof(EndTime), Criterion.LessOrEqual)]
        public DateTime? BeginTime { get; set; }
        [CompareWith(nameof(BeginTime), Criterion.MoreOrEqual)]
        public DateTime? EndTime { get; set; }
        public string Address { get; set; }
        public Guid? EventTypeId { get; set; }
    }
}
