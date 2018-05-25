using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class RequiredIFAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly IComparable criterion;


        public RequiredIFAttribute(string comparisonProperty, object criterion)
        {
            _comparisonProperty = comparisonProperty;
            this.criterion = criterion as IComparable 
                ?? throw new ArgumentException($"{nameof(criterion)} must be IComparable");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = $"{validationContext.MemberName} require when {_comparisonProperty} is {criterion}";

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException($"Property with {_comparisonProperty} name was not found");

            var comparisonValue = (IComparable)property.GetValue(validationContext.ObjectInstance);
            if (comparisonValue?.CompareTo(criterion) != 0)
                return ValidationResult.Success;

            return value != null ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
