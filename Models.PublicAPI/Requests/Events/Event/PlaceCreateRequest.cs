using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class PlaceCreateRequest
    {
        public List<Guid> Equipment { get; set; }
        public List<PersonWorkRequest> Workers { get; set; }
    }
}
