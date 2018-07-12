using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models.PublicAPI.Responses.Event;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EventCreateRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Guid EventTypeId { get; set; }
        public string Description { get; set; }

        public List<ShiftView> Shifts { get; set; }

    }
}
