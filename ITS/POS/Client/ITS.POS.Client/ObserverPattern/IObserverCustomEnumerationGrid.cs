using ITS.POS.Client.ObserverPattern.ObserverParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern
{
    interface IObserverCustomEnumerationGrid : IObserver
    {
        void Update(CustomEnumerationGridParams parameters);
    }
}
