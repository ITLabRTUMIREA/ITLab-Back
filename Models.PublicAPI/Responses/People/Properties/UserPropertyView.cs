using System;
namespace Models.PublicAPI.Responses.People.Properties
{
    public class UserPropertyView
    {
        public string Value { get; set; }
        public string Status { get; set; }

        public UserPropertyTypeView UserPropertyType { get; set; }
    }
}
