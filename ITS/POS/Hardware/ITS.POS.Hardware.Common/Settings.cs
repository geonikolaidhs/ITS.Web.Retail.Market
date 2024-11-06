using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Device Settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or Sets the device's COM Connection settings.
        /// </summary>
        public COMDeviceSettings COM { get; set; }

        /// <summary>
        /// Gets or Sets the device's OPOS Connection settings.
        /// </summary>
        public OPOSDeviceSettings OPOS { get; set; }

        /// <summary>
        /// Gets or Sets the device's LPT Connection settings.
        /// </summary>
        public LPTDeviceSettings LPT { get; set; }

        /// <summary>
        /// Gets or Sets the device's Ethernet Connection settings.
        /// </summary>
        public EthernetDeviceSettings Ethernet { get; set; }

        /// <summary>
        /// Gets or Sets the device's Indirect Connection settings.
        /// </summary>
        public IndirectDeviceSettings Indirect { get; set; }

        /// <summary>
        /// Gets or Sets the device's character set used.
        /// </summary>
        public int CharacterSet { get; set; }

        /// <summary>
        /// Gets or Sets the value used to interpret the end of a call to write and read internal functions. Default is "\n".
        /// </summary>
        public string NewLine { get; set; }

		/// <summary>
		/// Gets or Sets whether the device is primary (default).
		/// </summary>
        public bool IsPrimary { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of characters per line. Used for Print and Display Devices.
		/// </summary>
		public int LineChars { get; set; }
        public int CommandChars { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of lines. Used for Display Devices.
        /// </summary>
        public int NumberOfLines { get; set; }


        public bool ConvertCharset { get; set; }

        public int ConvertCharsetFrom { get; set; }

        public int ConvertCharsetTo { get; set; }


        public Settings()
        {
            COM = new COMDeviceSettings();
            OPOS = new OPOSDeviceSettings();
            LPT = new LPTDeviceSettings();
            Ethernet = new EthernetDeviceSettings();
            Indirect = new IndirectDeviceSettings();
            CharacterSet = 1253;
            NewLine = "\n";
            IsPrimary = false;
        }
    }
}
