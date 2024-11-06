using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes an error message to all the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionShowError : Action
    {
        public ActionShowError(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            string errorMessage = (parameters as ActionShowErrorParams).ErrorMessage;
            Notify(new ErrorMessengerParams(errorMessage));
            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                Kernel.LogFile.Trace("Error shown to user:" + errorMessage);
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.SHOW_ERROR; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
