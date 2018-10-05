using System;
namespace Models.PublicAPI.Requests.User.Properties.UserProperty
{
    public class UserPropertyEditRequest : IdRequest
    {
        public string Value { get; set; }
    }
}
