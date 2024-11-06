using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class EffectivePriceCatalogPolicy
    {
        public List<EffectivePriceCatalogPolicyDetail> PriceCatalogPolicyDetails { get; set; }

        public Guid Owner { get; set; }

        public EffectivePriceCatalogPolicy()
        {
            this.PriceCatalogPolicyDetails = new List<EffectivePriceCatalogPolicyDetail>();
        }

        public EffectivePriceCatalogPolicy(Store store, Customer customer = null) : this(store, customer == null ? null : customer.PriceCatalogPolicy)
        {
           
        }

        public EffectivePriceCatalogPolicy(Store store, PriceCatalogPolicy priceCatalogPolicy)
        {
            this.PriceCatalogPolicyDetails = new List<EffectivePriceCatalogPolicyDetail>();
            if (priceCatalogPolicy != null && priceCatalogPolicy.PriceCatalogPolicyDetails.Count > 0)
            {
                List<Guid> storePriceCatalogs = store.StorePriceLists.Select(priceLists => priceLists.PriceList.Oid).ToList();
                this.PriceCatalogPolicyDetails = priceCatalogPolicy.PriceCatalogPolicyDetails
                                                         .Where(policyDetail => storePriceCatalogs.Contains(policyDetail.PriceCatalog.Oid))
                                                         .Select(policyDetail => new EffectivePriceCatalogPolicyDetail(policyDetail))
                                                         .ToList();
            }
            int sortOffset = this.PriceCatalogPolicyDetails.Count == 0 ? 0 : this.PriceCatalogPolicyDetails.Max(policyDetail => policyDetail.Sort) + 1;

            List<EffectivePriceCatalogPolicyDetail> storePricePolicyDetails = store.DefaultPriceCatalogPolicy.PriceCatalogPolicyDetails
                                                                                   .Select(policyDetail => new EffectivePriceCatalogPolicyDetail(policyDetail, sortOffset)).ToList();

            this.PriceCatalogPolicyDetails.AddRange(storePricePolicyDetails);

            if (store != null)
            {
                this.Owner = store.Owner.Oid;
            }
        }

        public EffectivePriceCatalogPolicy(PriceCatalogPolicy priceCatalogPolicy)
        {
            this.PriceCatalogPolicyDetails = priceCatalogPolicy.PriceCatalogPolicyDetails
                                                               .Select(policyDetail => new EffectivePriceCatalogPolicyDetail(policyDetail)).ToList();
        }

        public bool HasPolicyDetails()
        {
            return this.PriceCatalogPolicyDetails.Count > 0;
        }

        public List<EffectivePriceCatalogPolicyDetail> OrderedPriceCatalogDetail
        {
            get
            {
                return this.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort).ToList();
            }
        }
    }
}
