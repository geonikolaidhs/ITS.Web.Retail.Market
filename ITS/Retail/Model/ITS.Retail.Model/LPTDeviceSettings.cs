using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    //[Updater(Order = 140,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class LPTDeviceSettings : DeviceSettings
    {
        public LPTDeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LPTDeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...
		private string _PortName;

		public string PortName
		{
			get
			{
				return _PortName;
			}
			set
			{
				SetPropertyValue("PortName", ref _PortName, value);
			}
		}
        
    }
}
