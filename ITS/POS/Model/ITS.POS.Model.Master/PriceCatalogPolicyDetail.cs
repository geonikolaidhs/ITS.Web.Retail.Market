using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.POS.Model.Master
{
    public class PriceCatalogPolicyDetail : BaseObj, IPriceCatalogPolicyDetail
    {
        private Guid _PriceCatalog;
        private int _Sort;
        private PriceCatalogSearchMethod _PriceCatalogSearchMethod;
        private bool _IsDefault;
        private Guid _PriceCatalogPolicy;
        private bool _TakesEffect;

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

        [Indexed("GCRecord", Unique =false)]
        public Guid PriceCatalog
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

        public PriceCatalog GetPriceCatalog
        {
            get
            {
                return Session.GetObjectByKey<PriceCatalog>(PriceCatalog);
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

        [Indexed("GCRecord", Unique = false)]
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
    }
}
