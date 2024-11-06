using ITS.POS.Fiscal.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalGetSerialNumberResponse : FiscalResponse
    {
        public String SerialNumber { get; set; }

        public FiscalGetSerialNumberResponse()
        {
        }
        public FiscalGetSerialNumberResponse(String serialNumber, eFiscalResponseType result)
        {
            SerialNumber = serialNumber;
            Result = result;

        }
    }
}
