using ITS.POS.Fiscal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Requests
{
    public abstract class FiscalRequest : FiscalMessage
    {
        public eFiscalRequestType Command
        {
            get;
            set;
        }
    }
}
