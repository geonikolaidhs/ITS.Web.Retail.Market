using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishDocumentPaymentInfoParams : ActionParams
    {

        public DocumentPayment Payment { get; set; }
        public DocumentHeader DocumentHeader {get; set;}
        public bool RefreshGrids { get; set; }
        public bool RefreshPoleDisplays { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_PAYMENT_INFO; }
        }

        public ActionPublishDocumentPaymentInfoParams(DocumentPayment payment,DocumentHeader header,bool refreshGrids,bool refreshPoleDisplays)
        {
            this.Payment = payment;
            this.RefreshGrids = refreshGrids;
            this.DocumentHeader = header;
            this.RefreshPoleDisplays = refreshPoleDisplays;
        }
    }
}
