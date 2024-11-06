using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Requests
{
    public class FiscalGetSerialNumberRequest : FiscalRequest
    {
        public FiscalGetSerialNumberRequest()
        {
            Command = eFiscalRequestType.GET_ID_SN;
        }
    }
}
