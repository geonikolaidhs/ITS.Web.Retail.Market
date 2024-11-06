using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.Attributes
{
    public class IsDefaultUniqueFieldsAttribute : System.Attribute
    {
        public string[] UniqueFields { get; set; }
    }
}
