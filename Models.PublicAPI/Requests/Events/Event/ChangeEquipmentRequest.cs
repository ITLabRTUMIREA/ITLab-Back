using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class ChangeEquipmentRequest : IdRequest
    {
        public List<Guid> EquipmentIds {get; set;}
    }
}
