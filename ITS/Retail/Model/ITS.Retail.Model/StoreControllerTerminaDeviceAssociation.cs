using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class StoreControllerTerminalDeviceAssociation : BaseObj, IRequiredOwner
    {
        public StoreControllerTerminalDeviceAssociation()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreControllerTerminalDeviceAssociation(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsPrimary = false;
        }


        private int _Priority;
        private CompanyNew _Owner;
        private TerminalDevice _TerminalDevice;
        private bool _IsPrimary;
        private string _ABCDirectory;
        private StoreControllerSettings _StoreControllerSettings;

        [Association("TerminalDevice-StoreControllerTerminaDeviceAssociations")]
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


        [Association("StoreControllerSettings-StoreControllerTerminaDeviceAssociations")]
        public StoreControllerSettings StoreControllerSettings
        {
            get
            {
                return _StoreControllerSettings;
            }
            set
            {
                SetPropertyValue("StoreControllerSettings", ref _StoreControllerSettings, value);
            }
        }


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

        public string ABCDirectory
        {
            get
            {
                return _ABCDirectory;
            }
            set
            {
                SetPropertyValue("ABCDirectory", ref _ABCDirectory, value);
            }
        }


        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
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

        public string Description
        {
            get
            {
                return (this.TerminalDevice != null ? this.TerminalDevice.Name : "");
            }
        }

        [Aggregated, Association("FiscalDevice-DocumentSeries")]
        public XPCollection<FiscalDeviceDocumentSeries> DocumentSeries
        {
            get
            {
                return GetCollection<FiscalDeviceDocumentSeries>("DocumentSeries");
            }
        }
    }
}
