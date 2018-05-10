using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Events.EventType
{
    public class EventTypeCreateRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
