using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Requests
{
    public class FiscalSetOnErrorRequest : FiscalRequest
    {
        public bool SetFiscalOnError { get; set; }
        public FiscalSetOnErrorRequest()
        {
            Command = eFiscalRequestType.SET_FISCAL_ON_ERROR;
        }

        public FiscalSetOnErrorRequest(bool setOnError)
        {
            Command = eFiscalRequestType.SET_FISCAL_ON_ERROR;
            SetFiscalOnError = setOnError;
        }
    }
}
