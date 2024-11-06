using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class DocumentDetailDisplayerParams: ObserverParams
    {
        public DocumentDetail DocumentDetail { get; set; }

        public DocumentDetailDisplayerParams(DocumentDetail documentDetail)
        {
            this.DocumentDetail = documentDetail;
        }

        public override Type GetObserverType()
        {
            return typeof(IObserverDocumentDetailDisplayer);
            
        }
    }
}
