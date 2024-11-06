using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.ObserverPattern.ObserverParameters;

namespace ITS.POS.Client.ObserverPattern
{
    interface IObserverDocumentHeaderDisplayer : IObserver
    {
        void Update(DocumentHeaderDisplayerParams parameters);
    }
}
