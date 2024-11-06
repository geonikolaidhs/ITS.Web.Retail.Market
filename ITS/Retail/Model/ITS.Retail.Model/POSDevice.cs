using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model {
    //[Updater(Order = 310,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class POSDevice : TerminalDevice {
        public POSDevice()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSDevice(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...

        //for cashierRegisterDevices-->
        private PriceCatalog _PriceCatalog;
        private ItemCategory _ItemCategory;
        private BarcodeType _BarcodeType;
        private DocumentType _DocumentType;
        private DocumentSeries _DocumentSeries;
        private DocumentStatus _DocumentStatus;
        private PaymentMethod _PaymentMethod;
        private DateTime _LastSuccefullyItemUpdate;
        private int _MaxItemsAdd;
        
        //<--
        private eDeviceSpecificType _DeviceSpecificType;
        private DeviceSettings _DeviceSettings;
        public DateTime LastSuccefullyItemUpdate
        {
            get
            {
                return _LastSuccefullyItemUpdate;
            }
            set
            {
                SetPropertyValue("LastSuccefullyItemUpdate", ref _LastSuccefullyItemUpdate, value);
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
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }
        public DocumentSeries DocumentSeries
        {
            get
            {
                return _DocumentSeries;
            }
            set
            {
                SetPropertyValue("DocumentSeries", ref _DocumentSeries, value);
            }
        }
        public DocumentStatus DocumentStatus
        {
            get
            {
                return _DocumentStatus;
            }
            set
            {
                SetPropertyValue("DocumentStatus", ref _DocumentStatus, value);
            }
        }
        public PaymentMethod PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }
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


        public eDeviceSpecificType DeviceSpecificType
        {
            get
            {
                return _DeviceSpecificType;
            }
            set
            {
                SetPropertyValue("DeviceSpecificType", ref _DeviceSpecificType, value);
            }
        }


        public bool UsesSpecificDeviceLibrary
        {
            get
            {
                return this.DeviceSpecificType != eDeviceSpecificType.None; ;
            }
        }

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    POSDevice d = item as POSDevice;
        //    
        //    Name = d.Name;

        //}
        
        [Association("POSDevice-MapVatFactor"), Aggregated]
        public XPCollection<MapVatFactor> MapVatFactors
        {
            get
            {
                return GetCollection<MapVatFactor>("MapVatFactors");
            }
        }
        public int MaxItemsAdd
        {
            get
            {
                return _MaxItemsAdd;
            }
            set
            {
                SetPropertyValue("MaxItemsAdd", ref _MaxItemsAdd, value);
            }
        }
        
    }

}