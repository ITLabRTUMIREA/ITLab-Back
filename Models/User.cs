using Microsoft.AspNetCore.Identity;
using System;

namespace Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
    }
}
