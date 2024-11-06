using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    [Updater(Order = 1100, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalogPolicyPromotion", typeof(ResourcesLib.Resources))]

    public class PriceCatalogPolicyPromotion : BaseObj, IPriceCatalogPolicyPromotion
    {
        private Promotion _Promotion;
        private PriceCatalogPolicy _PriceCatalogPolicy;

        public PriceCatalogPolicyPromotion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPolicyPromotion(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("Promotion.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }


        [Indexed("PriceCatalogPolicy;GCRecord"), Association("Promotion-PriceCatalogPolicyPromotions")]
        public Promotion Promotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                SetPropertyValue("Promotion", ref _Promotion, value);
            }
        }

        [Indexed("Promotion;GCRecord")]
        [Association("PriceCatalogPolicy-PriceCatalogPolicyPromotions")]
        public PriceCatalogPolicy PriceCatalogPolicy
        {
            get
            {
                return _PriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
            }
        }

        public Guid PromotionOid
        {
            get
            {
                if (this.Promotion == null)
                {
                    return Guid.Empty;
                }
                return this.Promotion.Oid;
            }
        }
    }
}
