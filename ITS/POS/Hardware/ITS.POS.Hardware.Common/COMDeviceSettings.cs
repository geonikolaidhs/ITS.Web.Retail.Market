using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Settings regarding a COM connection.
    /// </summary>
    public class COMDeviceSettings
    {
        /// <summary>
        /// Gets or Sets the name of the port on the system. e.x. "COM1"
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Gets or Sets the Baud rate of the scanner. This setting should match the device's setting.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Gets or Sets the Parity bit of the scanner. This setting should match the device's setting.
        /// </summary>
        public Parity Parity { get; set; }

        /// <summary>
        /// Gets or Sets the Data bits of the scanner. This setting should match the device's setting.
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// Gets or Sets the Stop bits of the scanner. This setting should match the device's setting.
        /// </summary>
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Gets or Sets the handshaking protocol for serial port transmitionof data. This setting should match the device's setting.
        /// </summary>
        public Handshake Handshake { get; set; }

        /// <summary>
        /// Gets or Sets the number of millisecond before a time-out occurs when a write operation does not finish
        /// </summary>
        public int WriteTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the COM Settings for Scales.
        /// </summary>
        public COMScaleSettings ScaleSettings { get; set; }


        public COMDeviceSettings()
        {
            PortName = "";
            BaudRate = 9600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
            Handshake = Handshake.None;
            WriteTimeOut = -1;
            ScaleSettings = new COMScaleSettings();
        }
    }
}
