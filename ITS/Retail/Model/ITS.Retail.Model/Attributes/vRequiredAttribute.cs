//-----------------------------------------------------------------------
// <copyright file="vRequiredAttribute.cs" company="ITS">
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
    public class vRequiredAttribute : ValidationAttribute
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
        public vRequiredAttribute() : base("{0} is Required")
        {
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessageString, name);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ( value != null )
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return base.IsValid(value, validationContext);
        }
    }
}
