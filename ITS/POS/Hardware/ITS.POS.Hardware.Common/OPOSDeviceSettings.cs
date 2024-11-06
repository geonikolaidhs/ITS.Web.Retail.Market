using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.PointOfService;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Settings regarding an OPOS connection.
    /// </summary>
    public class OPOSDeviceSettings
    {
        /// <summary>
        /// Gets or sets the Logical Device Name (ldn) of the device.
        /// </summary>
        public string LogicalDeviceName { get; set; }

        /// <summary>
        /// Gets or sets the OPOS Settings for Printers.
        /// </summary>
        public OPOSPrinterSettings PrinterSettings { get; set; }

        public OPOSDeviceSettings()
        {
            LogicalDeviceName = "";
            PrinterSettings = new OPOSPrinterSettings();
        }
    }
}
