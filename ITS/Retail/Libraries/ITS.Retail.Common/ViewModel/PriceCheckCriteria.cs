using System;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCheckSearchFilter: BaseSearchFilter
    {
        private Guid _Store;
        private Guid? _Customer;
        private string _ItemBarcode;

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
        public Guid? Customer
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

        public string ItemBarcode
        {
            get
            {
                return _ItemBarcode;
            }
            set
            {
                SetPropertyValue("ItemBarcode", ref _ItemBarcode, value);
            }
        }
    }
}
