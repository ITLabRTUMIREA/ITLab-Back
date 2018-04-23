using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event
{
    public class EventPresent
    {
        public Guid Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Address { get; set; }

        public Guid EventTypeId { get; set; }
    }
}
