using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class PersonWorkRequest : IdRequest
    {
        [CompareWith(nameof(EndWork), Criterion.LessOrEqual)]
        public DateTime? BeginWork { get; set; }
        [CompareWith(nameof(BeginWork), Criterion.MoreOrEqual)]
        public DateTime? EndWork { get; set; }
    }
}
