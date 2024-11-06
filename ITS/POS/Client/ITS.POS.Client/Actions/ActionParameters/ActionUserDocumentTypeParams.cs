using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionUserDocumentTypeParams : ActionParams
    {
        public string DocumentTypeCode { get; set; }

        public override eActions ActionCode
        {
            get
            {
                return eActions.USE_DOCUMENT_TYPE;
            }
        }

        public ActionUserDocumentTypeParams(string documentTypeCode)
        {
            this.DocumentTypeCode = documentTypeCode;
        }
    }
}
