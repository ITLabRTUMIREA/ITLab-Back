using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.People
{
    public class UserPresent
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
}
