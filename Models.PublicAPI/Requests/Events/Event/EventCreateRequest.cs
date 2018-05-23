using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class EventCreateRequest
    {
        [Required]
        [CompareWith(nameof(EndTime), Criterion.LessOrEqual)]
        public DateTime BeginTime { get; set; }
        [Required]
        [CompareWith(nameof(BeginTime), Criterion.MoreOrEqual)]
        public DateTime EndTime { get; set; }
        [Required]
        public string Address { get; set; }
        public List<PersonWorkRequest> Participants { get; set; }
        public List<Guid> Equipment { get; set; }
        [Required]
        public Guid EventTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int NeededParticipantsCount { get; set; }

    }
}
