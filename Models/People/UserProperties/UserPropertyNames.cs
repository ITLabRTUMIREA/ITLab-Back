using System;
using System.ComponentModel;

namespace Models.People.UserProperties
{
    public enum UserPropertyNames
    {
        [Description("Учебная группа")]
        StudyGroup,
        [Description("Страничка ВК")]
        VKID,
        [Description("Skype")]
        SkypeLogin,
        [Description("Отчеcтво")]
        Patronymic
    }
}
