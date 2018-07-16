using System;
using System.Collections.Generic;
using System.Text;
using Extensions.General;

namespace Models.Events
{
    public class EventType : IdInterface
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Event> Events { get; set; }
    }
}
