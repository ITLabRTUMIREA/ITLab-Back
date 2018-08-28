using System;
using Models.PublicAPI.Responses.People;
namespace Models.PublicAPI.Responses.Event.Invitations
{
    public class WisherEventView : UserAndRole
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventTypeView EventType { get; set; }
        public DateTime BeginTime { get; set; }
        public Guid PlaceId { get; set; }
        public int CurrentParticipantsCount { get; set; }
        public int TargetParticipantsCount { get; set; }
    }
}
