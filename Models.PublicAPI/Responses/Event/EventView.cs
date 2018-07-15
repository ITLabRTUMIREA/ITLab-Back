using Models.PublicAPI.Responses.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event
{
    public class EventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public EventTypePresent EventType { get; set; }
        public List<ShiftView> Shifts { get; set; }
    }
}
