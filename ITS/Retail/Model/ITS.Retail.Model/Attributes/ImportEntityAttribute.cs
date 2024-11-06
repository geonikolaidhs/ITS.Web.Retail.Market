using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Model.Attributes
{
    /// <summary>
    /// Marks a property of the DecodedRawData with the entity that uses it
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ImportEntityAttribute : Attribute
    {
        public Type EntityType { get; protected set; }
        public bool IsMandatory { get; set; }

        public ImportEntityAttribute(Type entityType)
        {
            this.EntityType = entityType;
            this.IsMandatory = false;
        }
    }
}