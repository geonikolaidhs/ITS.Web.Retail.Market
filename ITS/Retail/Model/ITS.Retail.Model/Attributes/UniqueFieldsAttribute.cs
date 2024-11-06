using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class UniqueFieldsAttribute : Attribute
    {
        public String[] UniqueFields;
    }
}
