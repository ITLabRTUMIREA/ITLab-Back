using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using System.Text;
using Extensions.General;

namespace Models.Events
{
    public class Event : IdInterface
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime BeginTime { get; set; }

        public List<Shift> Shifts { get; set; }

        public Guid EventTypeId { get; set; }
        public EventType EventType { get; set; }
    }
}
