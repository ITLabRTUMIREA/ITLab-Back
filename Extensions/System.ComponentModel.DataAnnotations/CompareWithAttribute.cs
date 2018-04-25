using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class CompareWithAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly Criterion criterion;

        private static Dictionary<Criterion, Func<int, bool>> comparingFuncs = new Dictionary<Criterion, Func<int, bool>>
        {
            {Criterion.Less, a => a < 0 },
            {Criterion.LessOrEqual, a => a <= 0 },
            {Criterion.Equal, a => a == 0 },
            {Criterion.MoreOrEqual, a => a >= 0 },
            {Criterion.More, a => a > 0 },
        };


        public CompareWithAttribute(string comparisonProperty, Criterion criterion)
        {
            _comparisonProperty = comparisonProperty;
            this.criterion = criterion;
            if (!comparingFuncs.TryGetValue(criterion, out var compareFunc))
                throw new ArgumentException("Property with this name not found");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = $"value must be {criterion.ToString()} than {_comparisonProperty}";
            var currentValue = (IComparable)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (IComparable)property.GetValue(validationContext.ObjectInstance);
            var result = currentValue.CompareTo(comparisonValue);
            return comparingFuncs[criterion](result) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
