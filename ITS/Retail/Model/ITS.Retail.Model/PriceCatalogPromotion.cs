using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    [Updater(Order = 1090,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]

    public class PriceCatalogPromotion : BaseObj, IPriceCatalogPromotion
    {
        private Promotion _Promotion;
        private PriceCatalog _PriceCatalog;
        
        public PriceCatalogPromotion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPromotion(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("Promotion.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }


        [Indexed("PriceCatalog;GCRecord"), Association("Promotion-PriceCatalogPromotions")]
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
        [Association("PriceCatalog-PriceCatalogPromotions")]
        public PriceCatalog PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
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
