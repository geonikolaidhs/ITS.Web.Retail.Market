using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class EthernetDeviceSettings : DeviceSettings
    {
        public EthernetDeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public EthernetDeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...
        private string _IPAddress;

        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                SetPropertyValue("IPAddress", ref _IPAddress, value);
            }
        }

        private int _Port;

        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                SetPropertyValue("Port", ref _Port, value);
            }
        }
        
    }
}
