using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class CorrectEnum : ValidationAttribute
    {
        private readonly Type enumType;

        public CorrectEnum(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Type should be assignable from System.Enum");
            }
            this.enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            => Enum.IsDefined(enumType, value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}
