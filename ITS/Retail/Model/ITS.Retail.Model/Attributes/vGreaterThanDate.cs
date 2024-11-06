//-----------------------------------------------------------------------
// <copyright file="vGreaterThanDate.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    class vGreaterThanDate :ValidationAttribute
    {

        private string _OtherPropertyName;
        public string OtherPropertyName
        {
            get { return _OtherPropertyName; }
            set
            {
                _OtherPropertyName = value;
            }
        }
        

        public vGreaterThanDate(string otherPropertyName) : base("{0} must be greater than {1}")
        {
            OtherPropertyName = otherPropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);
            var otherDate = (DateTime)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var thisDate = (DateTime)value;

            if (thisDate <= otherDate)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return base.IsValid(value, validationContext);
        }
    }
}
