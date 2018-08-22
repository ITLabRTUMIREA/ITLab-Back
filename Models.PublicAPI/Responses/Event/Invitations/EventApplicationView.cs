using System;
using Models.PublicAPI.Responses.People;
namespace Models.PublicAPI.Responses.Event.Invitations
{
    public class EventApplicationView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventTypeView EventType { get; set; }
        public DateTime BeginTime { get; set; }
        public Guid PlaceId { get; set; }
        public double ShiftDurationInMinutes { get; set; }
        public RoleView Role { get; set; }
    }
}
