using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalSetOnErrorResponse : FiscalResponse
    {
        //public String ErrorMessage { get; set; }

        public FiscalSetOnErrorResponse()
        {
        }

        public FiscalSetOnErrorResponse(eFiscalResponseType result,string errorMessage)
        {
            this.Result = result;
            this.ErrorMessage = errorMessage;
        }
    }
}
