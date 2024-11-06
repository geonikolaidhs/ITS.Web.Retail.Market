using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ITS.Retail.Common.ViewModel
{
    public class UpdateViewModelAttribute: Attribute
    {
        public Type ModelType { get; protected set; }
        
        public PropertyInfo ModelProperty { get; protected set; }

        public string ViewModelPropertyName { get; protected set; }

        public UpdateViewModelAttribute(Type modelType, string persistedPropertyName, string lookupPropertyName)
        {
            this.ModelType = modelType;
            ModelProperty = modelType.GetProperty(persistedPropertyName);
            ViewModelPropertyName = lookupPropertyName;
        }
    }
}