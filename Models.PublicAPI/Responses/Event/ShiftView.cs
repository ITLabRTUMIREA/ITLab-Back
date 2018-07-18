using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event
{
    public class ShiftView
    {
        public Guid Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<PlaceView> Places { get; set; }

    }
}
