using System;
using System.Collections.Generic;
namespace Models.People.UserProperties
{
    public class UserPropertyType
    {
        public Guid Id { get; set; }
        public string InternalName { get; set; }
        public string PublicName { get; set; }
        public string Description { get; set; }
        public UserPropertyStatus DefaultStatus { get; set; }

        public List<UserProperty> UserProperties { get; set; }
    }
}
