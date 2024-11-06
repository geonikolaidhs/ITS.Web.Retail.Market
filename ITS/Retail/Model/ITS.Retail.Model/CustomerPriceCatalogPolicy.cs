using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 220, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]

    public class CustomerPriceCatalogPolicy : BaseObj
    {
        private Customer _Customer;
        private PriceCatalogPolicy _PriceCatalogPolicy;

        public CustomerPriceCatalogPolicy()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerPriceCatalogPolicy(Session session)
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

        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        //[Association("PriceCatalogPolicy-CustomerPriceCatalogPolicies")]
        //public PriceCatalogPolicy PriceCatalogPolicy
        //{
        //    get
        //    {
        //        return _PriceCatalogPolicy;
        //    }
        //    set
        //    {
        //        SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
        //    }
        //}
    }
}
