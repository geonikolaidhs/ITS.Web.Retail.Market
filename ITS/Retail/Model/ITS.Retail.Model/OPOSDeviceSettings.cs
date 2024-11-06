using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    /// <summary>
    /// Settings regarding an OPOS connection.
    /// </summary>
    //[Updater(Order = 160,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class OPOSDeviceSettings : DeviceSettings
    {
        public OPOSDeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public OPOSDeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            LogicalDeviceName = "";
        }

        // Fields...
        private string _LogicalDeviceName;

        /// <summary>
        /// Gets or sets the Logical Device Name (ldn) of the device.
        /// </summary>
        public string LogicalDeviceName
        {
            get
            {
                return _LogicalDeviceName;
            }
            set
            {
                SetPropertyValue("LogicalDeviceName", ref _LogicalDeviceName, value);
            }
        }
    }
}
