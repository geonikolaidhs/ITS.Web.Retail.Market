using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Micrelec.Fiscal
{
    public class MicrelecFiscalPrinterStatus : FiscalPrinterStatus
    {
        bool _DeviceBusy;
        public bool DeviceBusy
        {
            get
            {
                return _DeviceBusy;
            }
            set
            {
                _DeviceBusy = value;
            }
        }
        public bool FatalError { get; set; }
        public bool BatteryWarning { get; set; }
        public bool PrinterOffline { get; set; }
        public bool PrinterTimeout { get; set; }
        public bool CutterError { get; set; }
        
        public bool CashOutOpen { get; set; }
        public bool CashInOpen { get; set; }
        public bool DrawerOpen { get; set; }
        
        
    }
}
