using System;

using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.Xml.Serialization;

namespace ITS.MobileAtStore.AuxilliaryClasses
{
    public class QueueQRPrinter
    {
        public string Address { get; set; }
        [XmlIgnore]
        public BluetoothAddress BluetoothAddress
        {
            get
            {
                BluetoothAddress bluetoothAddress = null;
                BluetoothAddress.TryParse(this.Address, out bluetoothAddress);
                return bluetoothAddress;
            }
            set
            {
                this.Address = value.ToString();
            }
        }
        public Guid Service
        {
            get
            {
                return BluetoothService.SerialPort;
            }
        }

        public int CodePage { get; set; }
        private Encoding _Encoding;
        [XmlIgnore]
        public Encoding Encoding
        {
            get
            {
                if (_Encoding == null || _Encoding.CodePage != this.CodePage)
                {
                    _Encoding = Encoding.GetEncoding(this.CodePage);
                }
                return _Encoding;
            }
            set
            {
                this.CodePage = value.CodePage;
                this._Encoding = value;
            }
        }

        /// <summary>
        /// Do not use this constructor.It is needed only for XMLSeserialisation
        /// </summary>
        public QueueQRPrinter()
        {
        }

        public QueueQRPrinter(BluetoothAddress bluetoothAddress, Encoding encoding)
        {
            this.BluetoothAddress = bluetoothAddress;
            this.Encoding = encoding;
        }
    }
}
