using ITS.POS.Model.Master;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IPriceCreator : IKernelModule
    {
        string CreateOrUpdatePrice(PriceCatalogPolicy priceCatalogPolicy, Item item, decimal priceToSet, bool vatIncluded, PriceCatalogSearchMethod searchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE);
    }
}
