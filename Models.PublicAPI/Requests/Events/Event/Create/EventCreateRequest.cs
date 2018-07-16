using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Events.Event.Create
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
        
        [MinCount(1)]
        public List<ShiftCreateRequest> Shifts { get; set; }

    }
}
