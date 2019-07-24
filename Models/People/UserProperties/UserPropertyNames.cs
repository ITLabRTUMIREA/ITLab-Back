using System;
using System.ComponentModel;

namespace Models.People.UserProperties
{
    public enum UserPropertyNames
    {
        [Description("Учебная группа")]
        StudyGroup,
        [Description("Id стираницы Вконтакте")]
        VKID,
        [Description("Логин в Skype")]
        SkypeLogin
    }
}
