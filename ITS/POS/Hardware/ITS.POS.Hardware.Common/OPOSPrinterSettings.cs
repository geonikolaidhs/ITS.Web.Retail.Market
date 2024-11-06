using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.PointOfService;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Settings regarding an OPOSPrinter.
    /// </summary>
    public class OPOSPrinterSettings
    {
        public PrinterStation _PrinterStation { get; protected set; }
        /// <summary>
        /// Gets or sets the PrinterStation used for printing. Values: {Receipt, Journal, Slip, None }
        /// </summary>
        public string PrinterStation 
        {
            get
            {
                return Enum.GetName(typeof(PrinterStation), _PrinterStation);
            }
            set
            {
                _PrinterStation = (PrinterStation)Enum.Parse(typeof(PrinterStation), value);
            }
        }

        /// <summary>
        /// Gets or sets the printer logo text.
        /// </summary>
        public string LogoText { get; set; }

        public PrinterLogoLocation _LogoLocation { get; protected set; }
        /// <summary>
        /// Gets or sets the printer logo location. Values: {Bottom, Top}
        /// </summary>
        public string LogoLocation 
        {
            get 
            {
                return Enum.GetName(typeof(PrinterLogoLocation), _LogoLocation);
            }
            set 
            {
                _LogoLocation = (PrinterLogoLocation)Enum.Parse(typeof(PrinterLogoLocation), value);
            }
        }

        public OPOSPrinterSettings()
        {
            _PrinterStation = ITS.POS.Hardware.Common.PrinterStation.Receipt;
            LogoText = "";
            _LogoLocation = PrinterLogoLocation.Top;
        }
    }
}
