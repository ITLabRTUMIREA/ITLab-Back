using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EqiupmentEditRequest : IdRequest
    {
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Address { get; set; }
        public Guid? EventTypeId { get; set; }
    }
}
