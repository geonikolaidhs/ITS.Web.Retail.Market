using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common.Exceptions
{
    public class POSFiscalPrinterException : POSHardwareException
    {
        public int ErrorCode { get; protected set; }
        public string ErrorDescription { get; protected set; }


        public POSFiscalPrinterException(string deviceName, int errorCode, string errorDescription)
            : base(deviceName,errorDescription)
        {
            this.ErrorCode = errorCode;
            this.ErrorDescription = errorDescription;
        }
    }
}
