using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;

namespace ITS.POS.Model.Settings
{
    public class POS : LookupField
    {
        public POS()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POS(Session session)
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

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

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
        private string _Remarks;
        [Size(2048)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private Guid _TerminalType;
        public Guid TerminalType
        {
            get
            {
                return _TerminalType;
            }
            set
            {
                SetPropertyValue("TerminalType", ref _TerminalType, value);
            }
        }

        private Guid _Store;
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
        private Guid _POSKeysLayout;
        public Guid POSKeysLayout
        {
            get
            {
                return _POSKeysLayout;
            }
            set
            {
                SetPropertyValue("POSKeysLayout", ref _POSKeysLayout, value);
            }
        }

        private Guid _DefaultCustomer;
        public Guid DefaultCustomer
        {
            get
            {
                return _DefaultCustomer;
            }
            set
            {
                SetPropertyValue("DefaultCustomer", ref _DefaultCustomer, value);
            }
        }

        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
            }
        }

        private bool _IsStandaloneFiscalOnError;
        public bool IsStandaloneFiscalOnError
        {
            get
            {
                return _IsStandaloneFiscalOnError;
            }
            set
            {
                SetPropertyValue("IsStandaloneFiscalOnError", ref _IsStandaloneFiscalOnError, value);
            }
        }


        private string _StandaloneFiscalOnErrorMessage;
        public string StandaloneFiscalOnErrorMessage
        {
            get
            {
                return _StandaloneFiscalOnErrorMessage;
            }
            set
            {
                SetPropertyValue("StandaloneFiscalOnErrorMessage", ref _StandaloneFiscalOnErrorMessage, value);
            }
        }

        private bool _UseSliderPauseForm;
        public bool UseSliderPauseForm
        {
            get
            {
                return _UseSliderPauseForm;
            }
            set
            {
                SetPropertyValue("UseSliderPauseForm", ref _UseSliderPauseForm, value);
            }
        }

        private bool _UseCashCounter;
        public bool UseCashCounter
        {
            get
            {
                return _UseCashCounter;
            }
            set
            {
                _UseCashCounter = value;
            }
        }

        private bool _OnIssueXClosePacketOnCreditDevice;
        public bool OnIssueXClosePacketOnCreditDevice
        {
            get
            {
                return _OnIssueXClosePacketOnCreditDevice;
            }
            set
            {
                SetPropertyValue("OnIssueXClosePacketOnCreditDevice", ref _OnIssueXClosePacketOnCreditDevice, value);
            }
        }
    }
}
