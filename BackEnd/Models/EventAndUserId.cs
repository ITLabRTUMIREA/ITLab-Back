using System;
using Models.Events;
namespace BackEnd.Models
{
    public class EventAndUserId : Event
    {
        public Guid UserId { get; set; }
    }
}
