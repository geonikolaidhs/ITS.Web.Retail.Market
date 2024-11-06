using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.Attributes
{
    /// <summary>
    /// Marks a field of a BaseObject that represents that object's description. If found it will be used on that objects ToString() method.
    /// </summary>
    public class DescriptionFieldAttribute : Attribute
    {
    }
}
