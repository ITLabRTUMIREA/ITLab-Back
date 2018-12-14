using System;
using System.Collections.Generic;
using System.Text;
using Models.PublicAPI.Responses.People.Properties;

namespace Models.PublicAPI.Responses.People
{
    public class UserView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<UserPropertyView> Properties { get; set; }

    }
}
