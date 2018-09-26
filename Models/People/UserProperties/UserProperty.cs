using System;
namespace Models.People.UserProperties
{
    public class UserProperty
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public bool IsConfirmed { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid UserPropertyTypeId { get; set; }
        public UserPropertyType UserPropertyType { get; set; }
    }
}
