using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.Attributes
{
    /// <summary>
    /// Take Note Suffix is declared using BindingListAttributeSuffix on
    /// view model's fields
    /// </summary>
    public class BindingListAttributeSuffix : Attribute
    {
        public string Suffix { get; set; }
    }
}
