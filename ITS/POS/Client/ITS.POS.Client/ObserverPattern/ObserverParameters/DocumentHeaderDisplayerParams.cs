using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class DocumentHeaderDisplayerParams: ObserverParams
    {
        public DocumentHeader DocumentHeader { get; set; }

        public DocumentHeaderDisplayerParams(DocumentHeader documentHeader)
        {
            this.DocumentHeader = documentHeader;
        }

        public override Type GetObserverType()
        {
            return typeof(IObserverDocumentHeaderDisplayer);
            
        }
    }
}
