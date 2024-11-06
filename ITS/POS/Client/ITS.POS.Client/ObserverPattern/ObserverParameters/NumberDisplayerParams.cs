using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
	public class NumberDisplayerParams : ObserverParams
	{
        public decimal Number { get; set; }
		public string DisplayFormat { get; set; }

        public NumberDisplayerParams(decimal number, string format)
		{
			this.Number = number;
			this.DisplayFormat = format;
		}

        public override Type GetObserverType()
        {
            return typeof(IObserverNumberDisplayer);
        }
    }
}
