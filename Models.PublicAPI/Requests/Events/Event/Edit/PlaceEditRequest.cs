using System.Collections.Generic;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Requests.Events.Event.Create;

namespace Models.PublicAPI.Requests.Events.Event.Edit
{
    public class PlaceEditRequest : DeletableRequest
    {
        public int TargetParticipantsCount { get; set; }
        public List<DeletableRequest> Equipment { get; set; }
        public List<PersonWorkRequest> Workers { get; set; }
    }
}