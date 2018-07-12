using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class PersonWorkRequest : IdRequest
    {
        public string Role { get; set; }
    }
}
