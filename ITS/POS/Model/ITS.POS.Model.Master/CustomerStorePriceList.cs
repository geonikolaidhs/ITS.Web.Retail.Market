using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class CustomerStorePriceList : BaseObj
    {
        public CustomerStorePriceList()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerStorePriceList(Session session)
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

        private Guid _Customer;
        [Indexed(Unique = false)]
        public Guid Customer
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

        private Guid _StorePriceList;
        public Guid StorePriceList
        {
            get
            {
                return _StorePriceList;
            }
            set
            {
                SetPropertyValue("StorePriceList", ref _StorePriceList, value);
            }
        }

        private bool _IsDefault;
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
