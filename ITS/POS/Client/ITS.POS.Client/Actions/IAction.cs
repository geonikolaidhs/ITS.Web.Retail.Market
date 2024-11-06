using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    public interface IAction : IObservable
    {
        void Execute(ActionParams parameters = null, bool dontCheckPermissions = false, bool validateMachineStatus = true);
        eActions ActionCode { get; }
        bool IsInternal { get; }
    }
}
