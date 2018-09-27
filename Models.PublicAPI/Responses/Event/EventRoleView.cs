using System;

namespace Models.PublicAPI.Responses.Event
{
    public class EventRoleView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}