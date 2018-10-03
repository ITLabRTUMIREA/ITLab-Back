using System;
using Models.People.UserProperties;
namespace BackEnd.Services.UserProperties
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UserPropertyInitializerAttribute : Attribute
    {
        public UserPropertyNames UserPropertyName { get; }
        public UserPropertyInitializerAttribute(UserPropertyNames userPropertyName)
        {
            UserPropertyName = userPropertyName;
        }
    }
}
