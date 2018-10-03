using System;
namespace Models.PublicAPI.Requests.User.Properties.UserPropertyType
{
    public class UserPropertyTypeEditRequest : IdRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
