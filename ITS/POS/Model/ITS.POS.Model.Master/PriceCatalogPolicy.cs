using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;
using System.Collections.Generic;
using System;

namespace ITS.POS.Model.Master
{
    public class PriceCatalogPolicy : Lookup2Fields, IPriceCatalogPolicy
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

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public XPCollection<PriceCatalogPolicyDetail> PriceCatalogPolicyDetails
        {
            get
            {
                return new XPCollection<PriceCatalogPolicyDetail>(this.Session, new BinaryOperator("PriceCatalogPolicy", Oid));
            }
        }

        public XPCollection<PriceCatalogPolicyPromotion> PriceCatalogPolicyPromotions
        {
            get
            {
                return new XPCollection<PriceCatalogPolicyPromotion>(this.Session, new BinaryOperator("PriceCatalogPolicy", Oid));
            }
        }

        public XPCollection<StorePriceCatalogPolicy> StorePriceCatalogPolicies
        {
            get
            {
                return new XPCollection<StorePriceCatalogPolicy>(this.Session, new BinaryOperator("PriceCatalogPolicy", Oid));
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

        public IEnumerable<IDocumentHeader> DocumentHeaders
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
