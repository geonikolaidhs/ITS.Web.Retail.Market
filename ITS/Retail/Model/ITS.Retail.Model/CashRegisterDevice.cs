using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class CashRegisterDevice : TerminalDevice
    {
        private DeviceSettings _DeviceSettings;
        private PriceCatalog _PriceCatalog;
        private ItemCategory _ItemCategory;
        private BarcodeType _BarcodeType;
        
        public DeviceSettings DeviceSettings
        {
            get
            {
                return _DeviceSettings;
            }
            set
            {
                SetPropertyValue("DeviceSettings", ref _DeviceSettings, value);
            }
        }

        public BarcodeType BarcodeType
        {
            get
            {
                return _BarcodeType;
            }
            set
            {
                SetPropertyValue("BarcodeType", ref _BarcodeType, value);
            }
        }

        public ItemCategory ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("ItemCategory", ref _ItemCategory, value);
            }
        }

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
        
    }
}
