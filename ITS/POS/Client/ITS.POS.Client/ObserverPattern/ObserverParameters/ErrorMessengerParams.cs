using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class ErrorMessengerParams : ObserverParams
    {
        public string Message { get; set; }

        public ErrorMessengerParams(string message)
        {
            this.Message = message;
        }

        public override Type GetObserverType()
        {
            return typeof(IObserverErrorMessenger);
        }
    }
}
