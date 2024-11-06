using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AvailabeTraderTypeAttribute: Attribute
    {
        public eDocumentTraderType AvailableTraderTypes { get; private set; }
        public AvailabeTraderTypeAttribute(eDocumentTraderType availableTraderTypes)
        {
            this.AvailableTraderTypes = availableTraderTypes;
        }
    }
}
