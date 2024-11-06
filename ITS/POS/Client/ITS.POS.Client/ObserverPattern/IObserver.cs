using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Settings;
using System.Windows.Forms;

namespace ITS.POS.Client.ObserverPattern
{
    public interface IObserver
    {
        Type[] GetParamsTypes();
        void InitializeActionSubscriptions();
        void DropActionSubscriptions();
    }
}