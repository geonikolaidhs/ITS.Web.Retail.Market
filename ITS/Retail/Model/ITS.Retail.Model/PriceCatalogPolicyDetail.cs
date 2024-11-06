using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    [Updater(Order = 52, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalogPolicyDetail", typeof(ResourcesLib.Resources))]
    public class PriceCatalogPolicyDetail : BaseObj, IPriceCatalogPolicyDetail
    {
        private PriceCatalog _PriceCatalog;
        private int _Sort;
        private PriceCatalogSearchMethod _PriceCatalogSearchMethod;
        private bool _IsDefault;
        private PriceCatalogPolicy _PriceCatalogPolicy;

        public PriceCatalogPolicyDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPolicyDetail(Session session)
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

        [Association("PriceCatalog-PriceCatalogPolicyDetails")]
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

        public int Sort
        {
            get
            {
                return _Sort;
            }
            set
            {
                SetPropertyValue("Sort", ref _Sort, value);
            }
        }

        public PriceCatalogSearchMethod PriceCatalogSearchMethod
        {
            get
            {
                return _PriceCatalogSearchMethod;
            }
            set
            {
                SetPropertyValue("PriceCatalogSearchMethod", ref _PriceCatalogSearchMethod, value);
            }
        }

        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }

        [Association("PriceCatalogPolicy-PriceCatalogPolicyDetails")]
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception(typeof(PriceCatalogPolicyDetail).Name+".GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("PriceCatalogPolicy.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }
    }
}
