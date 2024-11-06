using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    //[Updater(Order = 170,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class OPOSPrinterSettings : OPOSDeviceSettings
    {
        public OPOSPrinterSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public OPOSPrinterSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            PrinterStation = ITS.Retail.Model.PrinterStation.Receipt;
            LogoText = "";
            LogoLocation = PrinterLogoLocation.Top;
        }

        // Fields...
        private PrinterLogoLocation _LogoLocation;
        private string _LogoText;
        private PrinterStation _PrinterStation;

        /// <summary>
        /// Gets or sets the PrinterStation used for printing. Values: {Receipt, Journal, Slip, None }
        /// </summary>
		public PrinterStation PrinterStation
        {
            get
            {
                return  _PrinterStation;
            }
            set
            {
				SetPropertyValue("PrinterStation", ref _PrinterStation, value);
            }
        }


        public string LogoText
        {
            get
            {
                return _LogoText;
            }
            set
            {
                SetPropertyValue("LogoText", ref _LogoText, value);
            }
        }


        /// <summary>
        /// Gets or sets the printer logo location. Values: {Bottom, Top}
        /// </summary>
		public PrinterLogoLocation LogoLocation
        {
            get
            {
                return  _LogoLocation;
            }
            set
            {
				SetPropertyValue("LogoLocation", ref _LogoLocation, value);
            }
        }
    }
}
