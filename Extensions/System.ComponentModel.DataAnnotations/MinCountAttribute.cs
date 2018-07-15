using System.Collections;
using System.Linq;

namespace System.ComponentModel.DataAnnotations
{
    public class MinCountAttribute : ValidationAttribute
    {
        private readonly int minLength;

        public MinCountAttribute(int minLength)
        {
            if (minLength < 0)
                throw new ArgumentOutOfRangeException();
            this.minLength = minLength;
            ErrorMessage = "Length of";
        }


        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            ErrorMessage = $"Length of {context.DisplayName} must be >= {minLength}";
            return value is ICollection collection &&
                collection.Count >= minLength ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}