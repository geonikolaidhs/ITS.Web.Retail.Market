using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IAnalyticTree : IPersistentObject
    {

        Guid NodeOid { get; }
        Guid ObjectOid { get; }
    }
}
