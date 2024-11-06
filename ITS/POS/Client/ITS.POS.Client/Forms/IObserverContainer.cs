using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Forms
{
    public interface IObserverContainer
    {
        bool SubscriptionsAreInitialized { get; }

        void IntitializeSubscriptions();

        void DropSubscriptions();
    }
}
