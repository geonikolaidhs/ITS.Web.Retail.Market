using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IStorePriceCatalogPolicy : IPersistentObject
    {
        Guid Store { get; set; }
        Guid PriceCatalogPolicy { get; set; }
        bool IsDefault { get; set; }
    }
}
