using System;
using Models.Events;

namespace Models.PublicAPI.Responses.Event
{
    public class CompactEventView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventType EventType { get; set; }
        public int Сompleteness { get; set; }
        public DateTime BeginTime { get; set; }
        public double TotalDurationInMinutes { get; set; }
        public int ShiftsCount { get; set; }
    }
}