using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishDocumentLineInfoParams : ActionParams
    {
        public DocumentDetail Detail { get; set; }
        public bool RefreshGrids { get; set; }
        public bool RefreshPoleDisplays { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_LINE_INFO; }
        }

        public ActionPublishDocumentLineInfoParams(DocumentDetail detail,bool refreshGrids,bool refreshPoleDisplays)
        {
            this.Detail = detail;
            this.RefreshGrids = refreshGrids;
            this.RefreshPoleDisplays = refreshPoleDisplays;
        }
    }
}
