using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class PersistableModelReflectionDescription
    {
        public Type Type { get; protected set; }
        public List<PropertyInfo> Properties { get; protected set; }

        public Dictionary<PropertyInfo, PersistableViewModelAttribute> PersistableViewModelAttributes { get; protected set; }
        public PersistableModelReflectionDescription(Type type )
        {
            if(typeof(IPersistableViewModel).IsAssignableFrom(type) == false)
            {
                throw new Exception("PersistableModelReflectionDescription requires a type that implementsIPersistableViewModel");
            }
            Properties = type.GetProperties().ToList();
            PersistableViewModelAttributes = Properties.ToDictionary(x => x, pInfo => pInfo.GetCustomAttributes(typeof(PersistableViewModelAttribute), true).Cast<PersistableViewModelAttribute>().FirstOrDefault());
        }
    }
}
