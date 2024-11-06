using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.IO.Ports;

namespace ITS.Retail.Model
{

    public class COMDeviceSettings : DeviceSettings
    {
        public COMDeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public COMDeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            PortName = "";
            BaudRate = 9600;
            Parity = System.IO.Ports.Parity.None;
            DataBits = 8;
            StopBits = System.IO.Ports.StopBits.One;
            Handshake = System.IO.Ports.Handshake.None;
            WriteTimeOut = -1;
        }

        // Fields...
        private int _WriteTimeOut;
        private Handshake _Handshake;
        private StopBits _StopBits;
        private int _DataBits;
        private Parity _Parity;
        private int _BaudRate;
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


        public int BaudRate
        {
            get
            {
                return _BaudRate;
            }
            set
            {
                SetPropertyValue("BaudRate", ref _BaudRate, value);
            }
        }


        public Parity Parity
        {
            get
            {
                return _Parity;
            }
            set
            {
                SetPropertyValue("Parity", ref _Parity, value);
            }
        }


        public int DataBits
        {
            get
            {
                return _DataBits;
            }
            set
            {
                SetPropertyValue("DataBits", ref _DataBits, value);
            }
        }


        public StopBits StopBits
        {
            get
            {
                return _StopBits;
            }
            set
            {
                SetPropertyValue("StopBits", ref _StopBits, value);
            }
        }


        public Handshake Handshake
        {
            get
            {
                return _Handshake;
            }
            set
            {
                SetPropertyValue("Handshake", ref _Handshake, value);
            }
        }

        public int WriteTimeOut
        {
            get
            {
                return _WriteTimeOut;
            }
            set
            {
                SetPropertyValue("WriteTimeOut", ref _WriteTimeOut, value);
            }
        }
    }
}
