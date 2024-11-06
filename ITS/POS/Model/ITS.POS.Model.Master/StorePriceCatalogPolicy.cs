using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.POS.Model.Master
{
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

        private Guid _Store;
        private Guid _PriceCatalogPolicy;
        private bool _IsDefault;

        [Indexed("GCRecord", Unique = false)]
        public Guid Store
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
        
        public Guid PriceCatalogPolicy
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
    }
}
