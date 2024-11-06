using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ITS.Retail.Platform
{
    [Flags]
    public enum CompareOperatorType
    {
        EQUAL = 1,
        GREATER = 2,
        LESS = 4
    }

    public class CompareToAttribute : ValidationAttribute, IClientValidatable
    {
        public String OtherProperty { get; set; }
        public CompareOperatorType OperatorType { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.OtherProperty);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.OtherProperty));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null || value is IComparable == false)
            {
                return ValidationResult.Success;
            }

            if (propertyTestedValue == null || propertyTestedValue is IComparable == false)
            {
                return ValidationResult.Success;
            }

            // Compare values
            IComparable valueLeft = value as IComparable, valueRight = propertyTestedValue as IComparable;
            int result = valueLeft.CompareTo(valueRight);

            if ((result == 0 && OperatorType.HasFlag(CompareOperatorType.EQUAL)) || (result < 0 && OperatorType.HasFlag(CompareOperatorType.LESS)) || (result > 0 && OperatorType.HasFlag(CompareOperatorType.GREATER)))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareto"
            };
            rule.ValidationParameters["otherproperty"] = this.OtherProperty;
            rule.ValidationParameters["operatortype"] = this.OperatorType;
            yield return rule;
        }
    }

}
