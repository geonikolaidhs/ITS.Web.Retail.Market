using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface ILookup2Fields : ILookupField
    {
        string Code { get; set; }
    }
}
