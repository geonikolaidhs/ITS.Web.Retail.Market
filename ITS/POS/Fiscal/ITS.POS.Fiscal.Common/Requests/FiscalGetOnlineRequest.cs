using ITS.POS.Fiscal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Requests
{
    public class FiscalGetOnlineRequest : FiscalRequest
    {
        public FiscalGetOnlineRequest()
        {
            Command = eFiscalRequestType.CHECK_ONLINE_STATUS;
        }
    }
}
