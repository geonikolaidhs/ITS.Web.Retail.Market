using System;
using System.Collections.Generic;
using System.Linq;
using ITS.POS.Model.Settings;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern.ObserverParameters;

namespace ITS.POS.Client.ObserverPattern
{
    public interface IObservable
    {
        ThreadSafeList<IObserver> Observers { get; set; }
        void Attach(IObserver os);
        void Dettach(IObserver os);
        //void Notify(POSKeyMapping keyMap);
        //void Notify(string message = "");
        //void Notify<T>(eActions actionCode, T obj);
        void Notify(ObserverParams parameters);
    }
}
