using System;
using System.ComponentModel.DataAnnotations;
using Models.PublicAPI.Responses.People.Properties;
namespace Models.PublicAPI.Requests.User.Properties.UserPropertyType
{
    public class UserPropertyTypeCreateRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultStatus { get; set; }
    }
}
