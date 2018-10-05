using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Models.Events
{
    public class Shift
    {
        public Guid Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<Place> Places { get; set; }
            = new List<Place>();

        public string Description { get; set; }

        public Event Event { get; set; }
        public Guid EventId { get; set; }
    }
}
