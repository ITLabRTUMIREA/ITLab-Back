using Microsoft.AspNetCore.Identity;
using Models.DataBaseLinks;
using System;
using System.Collections.Generic;

namespace Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
        public List<EventUser> EventUsers { get; set; }
    }
}
