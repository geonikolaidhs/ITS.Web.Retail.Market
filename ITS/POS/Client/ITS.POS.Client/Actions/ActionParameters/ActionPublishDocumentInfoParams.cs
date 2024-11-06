using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishDocumentInfoParams : ActionParams
    {
        public DocumentHeader Header { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_INFO; }
        }

        public ActionPublishDocumentInfoParams(DocumentHeader header)
        {
            Header = header;
        }
    }
}
