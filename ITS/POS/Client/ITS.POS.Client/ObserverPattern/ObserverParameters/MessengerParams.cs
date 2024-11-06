using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
	public class MessengerParams : ObserverParams
	{
		public string Message { get; set; }

		public MessengerParams(string message)
		{
			this.Message = message;
		}


        public override Type GetObserverType()
        {
            return typeof(IObserverSimpleMessenger);
        }
    }
}
