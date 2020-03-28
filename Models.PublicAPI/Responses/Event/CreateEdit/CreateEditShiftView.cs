using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event.CreateEdit
{
    public class CreateEditShiftView
    {
        public Guid Id { get; set; }
        public int ClientId { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }

        public List<CreateEditPlaceView> Places { get; set; }
    }
}
