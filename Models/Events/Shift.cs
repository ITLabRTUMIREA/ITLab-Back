using System;
using System.Collections.Generic;
using System.Text;
using Extensions.General;
using Newtonsoft.Json.Serialization;

namespace Models.Events
{
    public class Shift : IdInterface
    {
        public Guid Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<Place> Places { get; set; }

        public Event Event { get; set; }
        public Guid EventId { get; set; }
    }
}
