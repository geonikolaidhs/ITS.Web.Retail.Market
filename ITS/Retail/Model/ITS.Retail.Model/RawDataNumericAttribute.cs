using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class RawDataNumericAttribute : Attribute
    {
        public NumericType Type { get; set; }

        public RawDataNumericAttribute(NumericType type)
        {
            Type = type;
        }

    }
}
