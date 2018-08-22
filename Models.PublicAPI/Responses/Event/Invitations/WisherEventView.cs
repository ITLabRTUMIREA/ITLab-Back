using System;
using Models.PublicAPI.Responses.People;
namespace Models.PublicAPI.Responses.Event.Invitations
{
    public class WisherEventView
    {
        public Guid EventId { get; set; }
        public EventTypeView EventType { get; set; }
        public UserAndRole Wish { get; set; }
        public int CurrentParticipantsCount { get; set; }
        public int TargetParticipantsCount { get; set; }
    }
}
