using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event.CreateEdit
{
    public class CreateEditEventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public EventTypeView EventType { get; set; }
        public List<CreateEditShiftView> Shifts { get; set; }
    }
}
