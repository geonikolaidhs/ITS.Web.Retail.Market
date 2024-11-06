using System.Collections.Generic;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPriceCatalogPolicy : IPersistentObject
    {
        IEnumerable<IPriceCatalogPolicyPromotion> PriceCatalogPolicyPromotions { get; }

        IEnumerable<IPriceCatalogPolicyDetail> PriceCatalogPolicyDetails { get; }

        IEnumerable<IDocumentHeader> DocumentHeaders { get; }

        IEnumerable<IStorePriceCatalogPolicy> StorePriceCatalogPolicies { get; }
    }
}
