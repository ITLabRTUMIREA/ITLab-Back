using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class CompareWithAttribute : ValidationAttribute
    {
        private readonly Criterion criterion;
        private readonly Func<ValidationContext, IComparable> GetComparableValue;


        private static Dictionary<Criterion, Func<int, bool>> comparingFuncs = new Dictionary<Criterion, Func<int, bool>>
        {
            {Criterion.Less, a => a < 0 },
            {Criterion.LessOrEqual, a => a <= 0 },
            {Criterion.Equal, a => a == 0 },
            {Criterion.MoreOrEqual, a => a >= 0 },
            {Criterion.More, a => a > 0 },
        };

        public CompareWithAttribute(object comparisonConstant, Criterion criterion) : this(vc => comparisonConstant as IComparable, criterion)
        { }

        public CompareWithAttribute(string comparisonProperty, Criterion criterion) : this(vc => GetFieldByName(vc, comparisonProperty), criterion)
        {}

        private CompareWithAttribute(Func<ValidationContext, IComparable> getComparableValue, Criterion criterion)
        {
            if (!comparingFuncs.TryGetValue(criterion, out var compareFunc))
                throw new ArgumentException("Property with this name not found");
            GetComparableValue = getComparableValue;
            this.criterion = criterion;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (IComparable)value;
            var comparisonValue = GetComparableValue(validationContext);
            ErrorMessage = $"value must be {criterion.ToString()} than {comparisonValue}";

            if (currentValue == null || comparisonValue == null) return ValidationResult.Success;
            var result = currentValue.CompareTo(comparisonValue);
            return comparingFuncs[criterion](result) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        private static IComparable GetFieldByName(ValidationContext validationContext, string comparisonProperty)
        {
            var property = validationContext.ObjectType.GetProperty(comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            return (IComparable)property.GetValue(validationContext.ObjectInstance);
        }
    }
}
