using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class CustomEnumerationGridParams : ObserverParams
    {
        public Guid SelectedValue { get; set; }
         

        public override Type GetObserverType()
        {
            return typeof(IObserverCustomEnumerationGrid);
        }
    }
}
