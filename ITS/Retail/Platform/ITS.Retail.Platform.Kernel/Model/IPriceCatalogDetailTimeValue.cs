using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPriceCatalogDetailTimeValue
    {
        DateTime TimeValueValidFromDate { get; }
        DateTime TimeValueValidUntilDate { get; }

        decimal TimeValue { get; }

        bool IsActive { get; }
    }
}
