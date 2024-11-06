using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class BlinkingErrorMessengerParams : ObserverParams
    {
        public bool Blink { get; set; }

        public BlinkingErrorMessengerParams(bool Blink)
        {
            this.Blink = Blink;
        }

        public override Type GetObserverType()
        {
            return typeof(IObserverBlinkingErrorMessenger);
        }
    }
}
