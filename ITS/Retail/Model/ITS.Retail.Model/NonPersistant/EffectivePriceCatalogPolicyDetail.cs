using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class EffectivePriceCatalogPolicyDetail
    {
        public Guid PriceCatalogOid { get; set; }
        public int Sort { get; set; }
        public PriceCatalogSearchMethod PriceCatalogSearchMethod { get; set; }
        public bool IsDefault { get; set; }

        public EffectivePriceCatalogPolicyDetail(PriceCatalogPolicyDetail priceCatalogPolicyDetail)
        {            
            this.PriceCatalogOid = priceCatalogPolicyDetail.PriceCatalog.Oid;
            this.Sort = priceCatalogPolicyDetail.Sort;
            this.PriceCatalogSearchMethod = priceCatalogPolicyDetail.PriceCatalogSearchMethod;
            this.IsDefault = priceCatalogPolicyDetail.IsDefault;
        }

        public EffectivePriceCatalogPolicyDetail(PriceCatalogPolicyDetail priceCatalogPolicyDetail, int sortOffset) : this(priceCatalogPolicyDetail)
        {
            this.Sort += sortOffset;
        }
    }
}
