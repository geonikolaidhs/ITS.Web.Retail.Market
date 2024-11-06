using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class ValueModelBinder<T> : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
            {
                return null;
            }
            
            return value
                .AttemptedValue
                .Split(',')
                .Select(x=> (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(x))
                .ToArray();
        }
    }
}