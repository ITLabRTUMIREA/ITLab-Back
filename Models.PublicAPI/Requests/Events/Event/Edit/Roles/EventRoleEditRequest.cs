using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests.Events.Event.Edit.Roles
{
    public class EventRoleEditRequest : IdRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
