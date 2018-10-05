using System;
using Models.PublicAPI.Requests.Events.EventType;

namespace Models.PublicAPI.Responses.Event
{
    public class CompactEventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventTypeView EventType { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Address { get; set; }
        public int ShiftsCount { get; set; }
        public int CurrentParticipantsCount { get; set; }
        public int TargetParticipantsCount { get; set; }
        public bool Participating { get; set; }
    }
}