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
        public string PlaceDescription { get; set; }
        public int PlaceNumber { get; set; }
        public string ShiftDescription { get; set; }
        public double ShiftDurationInMinutes { get; set; }
        public EventRoleView EventRole { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DoneTime { get; set; }
    }
}
