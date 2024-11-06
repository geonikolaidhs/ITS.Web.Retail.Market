using ITS.POS.Fiscal.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalGetOnlineResponse : FiscalResponse
    {

        public FiscalGetOnlineResponse()
        {
        }

        public bool ReloadSettings { get; set; }

        public FiscalGetOnlineResponse(eFiscalResponseType result, bool reloadSettings)
        {

            Result = result;
            this.ReloadSettings = reloadSettings;
        }
    }
}
