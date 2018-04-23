using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.EventType
{
    public class EventTypeEditRequest : IdRequest
    {
        public string Title { get; set; }
    }
}
