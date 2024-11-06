using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel.Importable
{
    public class MasterDetailImportableAttribute : Attribute
    {
        public string MasterProperty { get; set; }

        public string DetailProperty { get; set; }


    }

    public class KeyImportableAttribute : Attribute
    {
        public KeyImportableAttribute(string persistentLookupProperty)
        {
            this.PersistentLookupProperty = persistentLookupProperty;
        }

        public string PersistentLookupProperty { get; protected set; }
    }

    public class LookupImportableAttribute : Attribute
    {
        public LookupImportableAttribute(Type persistentType, string persistentProperty, string localProperty)
        {
            this.PersistentType = persistentType;
            this.PersistentProperty = persistentProperty;
            this.LocalProperty = localProperty;
        }
        public Type PersistentType { get; private set; }
        public string PersistentProperty { get; private set; }
        public string LocalProperty { get; private set; }

    }
}
