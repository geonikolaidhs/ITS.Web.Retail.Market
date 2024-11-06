using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface ILookupField : IPersistentObject
    {
        string Description { get; set; }
		 bool IsDefault { get; set; }
    }
}
