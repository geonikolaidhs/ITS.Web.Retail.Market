using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DenormalizedFieldAttribute : Attribute
    {
        public String DenormalizedField { get; protected set; }

        public Type LocalEntityType { get; protected set; }

        public String RemoteEntityTypeName { get; protected set; }        
        public String RemotePropertyName { get; protected set; }

        public DenormalizedFieldAttribute(String denormalizedField, Type localEntityType, string remoteEntityTypeName, string remotePropertyName)
        {
            DenormalizedField = denormalizedField;
            LocalEntityType = localEntityType;
            RemoteEntityTypeName = remoteEntityTypeName;
            RemotePropertyName = remotePropertyName;
        }
    }
}
