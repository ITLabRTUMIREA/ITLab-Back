using System;

namespace Models.PublicAPI.Responses.Event
{
    public class CompactEventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventTypeView EventType { get; set; }
        public DateTime BeginTime { get; set; }
        public double TotalDurationInMinutes { get; set; }
        public int ShiftsCount { get; set; }
        public int CurrentParticipantsCount { get; set; }
        public int TargetParticipantsCount { get; set; }
    }
}