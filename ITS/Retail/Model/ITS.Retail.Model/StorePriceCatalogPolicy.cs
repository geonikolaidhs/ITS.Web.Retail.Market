using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    [Updater(Order = 60,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("StorePriceCatalogPolicy", typeof(ResourcesLib.Resources))]
    public class StorePriceCatalogPolicy : BaseObj, IStorePriceCatalogPolicy
    {
        public StorePriceCatalogPolicy()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StorePriceCatalogPolicy(Session session)
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

        private Store _Store;
        private PriceCatalogPolicy _PriceCatalogPolicy;
        private bool _IsDefault;

        [Association("Store-StorePriceCatalogPolicies"), Indexed("GCRecord", Unique = false)]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        [Association("PriceCatalogPolicy-StorePriceCatalogPolicies")]
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

        Guid IStorePriceCatalogPolicy.Store
        {
            get
            {
                if ( this.Store == null )
                {
                    return Guid.Empty;
                }
                return this.Store.Oid;
            }

            set
            {
                this.Store = this.Session.GetObjectByKey<Store>(value);
            }
        }

        Guid IStorePriceCatalogPolicy.PriceCatalogPolicy
        {
            get
            {
                if (this.PriceCatalogPolicy == null)
                {
                    return Guid.Empty;
                }
                return this.PriceCatalogPolicy.Oid;
            }

            set
            {
                this.PriceCatalogPolicy = this.Session.GetObjectByKey<PriceCatalogPolicy>(value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception("StorePriceCatalogPolicy.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = new BinaryOperator("Store.Oid", store.Oid);
                    break;
            }
            return crop;
        }
    }
}
