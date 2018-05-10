using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class NotIntersectAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;


        public NotIntersectAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = $"{validationContext.MemberName} must not intersect with {_comparisonProperty}";
            var currentValue = value as IEnumerable?? 
                throw new ArgumentException("property must be IEnumebrable");

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as IEnumerable ??
                    throw new ArgumentException("property must be IEnumebrable");

            if (!IsIntersect(currentValue, comparisonValue))
                return ValidationResult.Success;

            return value != null ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
        private static bool IsIntersect(IEnumerable first, IEnumerable second)
        {
            var set = new HashSet<object>();
            foreach (var i in first)
                set.Add(i);
            foreach (var i in second)
                if (set.Remove(i))
                    return true;
            return false;
        }
    }
}
