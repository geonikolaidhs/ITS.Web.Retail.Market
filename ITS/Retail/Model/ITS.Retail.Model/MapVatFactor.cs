using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class MapVatFactor : BasicObj
    {
        public MapVatFactor()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MapVatFactor(Session session)
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

        // Fields...
        private string _DeviceVatLevel;
        private VatFactor _VatFactor;
        private POSDevice _CashRegisterDevice;
        private Item _Item;
        [Indexed(Unique = false), Association("POSDevice-MapVatFactor")]
        public POSDevice CashRegisterDevice
        {
            get
            {
                return _CashRegisterDevice;
            }
            set
            {
                SetPropertyValue("CashRegisterDevice", ref _CashRegisterDevice, value);
            }
        }


        public string DeviceVatLevel
        {
            get
            {
                return _DeviceVatLevel;
            }
            set
            {
                SetPropertyValue("DeviceVatLevel", ref _DeviceVatLevel, value);
            }
        }

        public VatFactor VatFactor
        {
            get
            {
                return _VatFactor;
            }
            set
            {
                SetPropertyValue("VatFactor", ref _VatFactor, value);
            }
        }

        public Item Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }
    }
}
