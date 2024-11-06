using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class ItemGridParams : ObserverParams
    {
        public ItemGridParams()
        {
            //Use this constructor to clear the grid
        }
        public eNavigation? Navigation { get; set; }
        public ItemGridParams(eNavigation navigation)
        {
            Navigation = navigation;
        }
        public override Type GetObserverType()
        {
            return typeof(IObserverItemGrid);
        }
    }
}