using ITS.POS.Fiscal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Requests
{
    public class FiscalIssueZRequest : FiscalRequest
    {
        public FiscalIssueZRequest()
        {
            Command = eFiscalRequestType.ISSUE_Z;
        }
    }
}
