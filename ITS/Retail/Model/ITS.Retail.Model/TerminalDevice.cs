using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model {

    public abstract class TerminalDevice : BaseObj {
        public TerminalDevice()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TerminalDevice(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }


        // Fields...

        private ConnectionType _ConnectionType;
        private string _Name;


		[Association("TerminalDevice-TerminalDeviceAssociations")]
		public XPCollection<TerminalDeviceAssociation> TerminalDeviceAssociations
        {
            get
            {
				return GetCollection<TerminalDeviceAssociation>("TerminalDeviceAssociations");
            }
        }

       [Association("TerminalDevice-StoreControllerTerminaDeviceAssociations")]
        public XPCollection<StoreControllerTerminalDeviceAssociation> StoreControllerTerminaDeviceAssociations
        {
            get
            {
                return GetCollection<StoreControllerTerminalDeviceAssociation>("StoreControllerTerminaDeviceAssociations");
            }
        }

/*
        [Association("Device-TerminalDevices")]
        public Device Device {
            get {
                return _Device;
            }
            set {
                SetPropertyValue("Device", ref _Device, value);
            }
        }
 */



        public ConnectionType ConnectionType {
            get {
                return _ConnectionType;
            }
            set {
                SetPropertyValue("ConnectionType", ref _ConnectionType, value);
            }
        }

		[Indexed("GCRecord",Unique = true)]
        [DescriptionField]
        public string Name {
            get {
                return _Name;
            }
            set {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    TerminalDevice td = item as TerminalDevice;
        //    ConnectionType = td.ConnectionType;
        //    Name = td.Name;

        //}

    }

}