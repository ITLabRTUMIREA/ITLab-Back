using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event.Edit
{
    public class EventEditRequest : IdRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Guid? EventTypeId { get; set; }
        
        public List<ShiftEditRequest> Shifts { get; set; }
    }
}
