using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class IndirectDeviceSettings : DeviceSettings
    {
        public IndirectDeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public IndirectDeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ParentDeviceName = "";
            OpenCommandString = "";
        }

        // Fields...
        private byte [] _OpenCommandStringByteArray;
        private string _ParentDeviceName;
        public string ParentDeviceName
        {
            get
            {
                return _ParentDeviceName;
            }
            set
            {
                SetPropertyValue("ParentDeviceName", ref _ParentDeviceName, value);
            }
        }


        public byte[] OpenCommandStringByteArray
        {
            get
            {
                return _OpenCommandStringByteArray;
            }
            set
            {
                SetPropertyValue("OpenCommandStringByteArray", ref _OpenCommandStringByteArray, value);
            }
        }

        private string _OpenCommandString;
        [NonPersistent]
        public string OpenCommandString
        {
            get
            {
                if (String.IsNullOrEmpty(_OpenCommandString) && OpenCommandStringByteArray!=null)
                {
                    _OpenCommandString = Encoding.UTF8.GetString(OpenCommandStringByteArray);
                }
                return _OpenCommandString;
            }
            set
            {
                OpenCommandStringByteArray = Encoding.UTF8.GetBytes(value);
                _OpenCommandString = value;
            }
        }

        private string _KeyPosition0CommandString;
        public string KeyPosition0CommandString
        {
            get
            {
                return _KeyPosition0CommandString;
            }
            set
            {
                SetPropertyValue("KeyPosition0CommandString", ref _KeyPosition0CommandString, value);
            }
        }

        private string _KeyPosition1CommandString;
        public string KeyPosition1CommandString
        {
            get
            {
                return _KeyPosition1CommandString;
            }
            set
            {
                SetPropertyValue("KeyPosition1CommandString", ref _KeyPosition1CommandString, value);
            }
        }

        private string _KeyPosition2CommandString;
        public string KeyPosition2CommandString
        {
            get
            {
                return _KeyPosition2CommandString;
            }
            set
            {
                SetPropertyValue("KeyPosition2CommandString", ref _KeyPosition2CommandString, value);
            }
        }

        private string _KeyPosition3CommandString;
        public string KeyPosition3CommandString
        {
            get
            {
                return _KeyPosition3CommandString;
            }
            set
            {
                SetPropertyValue("KeyPosition3CommandString", ref _KeyPosition3CommandString, value);
            }
        }

        private string _KeyPosition4CommandString;
        public string KeyPosition4CommandString
        {
            get
            {
                return _KeyPosition4CommandString;
            }
            set
            {
                SetPropertyValue("KeyPosition4CommandString", ref _KeyPosition4CommandString, value);
            }
        }
    }
}
