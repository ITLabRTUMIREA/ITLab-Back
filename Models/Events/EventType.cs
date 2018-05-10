using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Events
{
    public class EventType
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
