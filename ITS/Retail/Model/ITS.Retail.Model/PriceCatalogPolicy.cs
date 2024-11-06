using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Model
{
    [Updater(Order = 46, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalogPolicy", typeof(ResourcesLib.Resources))]
    public class PriceCatalogPolicy : Lookup2Fields, IRequiredOwner, IPriceCatalogPolicy
    {
        public PriceCatalogPolicy()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPolicy(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("PriceCatalogPolicy.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Aggregated, Association("PriceCatalogPolicy-PriceCatalogPolicyDetails")]
        public XPCollection<PriceCatalogPolicyDetail> PriceCatalogPolicyDetails
        {
            get
            {
                return GetCollection<PriceCatalogPolicyDetail>("PriceCatalogPolicyDetails");
            }
        }

        [Aggregated, Association("PriceCatalogPolicy-StorePriceCatalogPolicies")]
        public XPCollection<StorePriceCatalogPolicy> StorePriceCatalogPolicies
        {
            get
            {
                return GetCollection<StorePriceCatalogPolicy>("StorePriceCatalogPolicies");
            }
        }

        [Association("PriceCatalogPolicy-PriceCatalogPolicyPromotions"), Aggregated]
        public XPCollection<PriceCatalogPolicyPromotion> PriceCatalogPolicyPromotions
        {
            get
            {
                return GetCollection<PriceCatalogPolicyPromotion>("PriceCatalogPolicyPromotions");
            }
        }

        [Association("PriceCatalogPolicy-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

        IEnumerable<IPriceCatalogPolicyPromotion> IPriceCatalogPolicy.PriceCatalogPolicyPromotions
        {
            get
            {
                return this.PriceCatalogPolicyPromotions;
            }
        }

        IEnumerable<IPriceCatalogPolicyDetail> IPriceCatalogPolicy.PriceCatalogPolicyDetails
        {
            get
            {
                return this.PriceCatalogPolicyDetails;
            }
        }

        IEnumerable<IDocumentHeader> IPriceCatalogPolicy.DocumentHeaders
        {
            get
            {
                return this.DocumentHeaders;
            }
        }

        IEnumerable<IStorePriceCatalogPolicy> IPriceCatalogPolicy.StorePriceCatalogPolicies
        {
            get
            {
                return this.StorePriceCatalogPolicies;
            }
        }
    }
}
