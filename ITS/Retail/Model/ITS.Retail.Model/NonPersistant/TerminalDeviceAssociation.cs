using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
	//[Updater(Order = 760,
	//    Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)]
	public class TerminalDeviceAssociation: BaseObj {
        public TerminalDeviceAssociation()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

		public TerminalDeviceAssociation(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
			IsPrimary = false;
        }


		private int _Priority;
        private Terminal _Terminal;
		[Association("Terminal-TerminalDeviceAssociations")]
		public Terminal Terminal
		{
			get
			{
				return _Terminal;
			}
			set
			{
				SetPropertyValue("Terminal", ref _Terminal, value);
			}
		}

		private TerminalDevice _TerminalDevice;
		[Association("TerminalDevice-TerminalDeviceAssociations")]
		public TerminalDevice TerminalDevice
		{
			get
			{
				return _TerminalDevice;
			}
			set
			{
				SetPropertyValue("TerminalDevice", ref _TerminalDevice, value);
			}
		}

		private bool _IsPrimary;
		/// <summary>
		/// Gets or Sets whether the device is primary (default).
		/// </summary>
		public bool IsPrimary
		{
			get
			{
				return _IsPrimary;
			}
			set
			{
				SetPropertyValue("IsPrimary", ref _IsPrimary, value);
			}
		}


        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
        }

	}
}
