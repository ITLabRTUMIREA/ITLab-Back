using System;
namespace Models.PublicAPI.Responses.People.Properties
{
    public class UserPropertyView
    {
        public string Value { get; set; }
        public UserPropertyStatusView Status { get; set; }

        public UserPropertyTypeView PropertyType { get; set; }
    }
}
