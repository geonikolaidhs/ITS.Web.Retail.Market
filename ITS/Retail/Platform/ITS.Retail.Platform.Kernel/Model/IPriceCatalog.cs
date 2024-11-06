using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPriceCatalog : IPersistentObject
    {
        IEnumerable<IPriceCatalogPromotion> PriceCatalogPromotions { get; }

        bool SupportLoyalty { get; set; }

        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        Guid? ParentCatalogOid { get; set; }

        bool IsRoot { get; set; }

        IEnumerable<IPriceCatalog> SubPriceCatalogs { get; }
    }
}