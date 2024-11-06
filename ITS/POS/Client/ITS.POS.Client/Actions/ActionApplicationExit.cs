using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Closes the application
    /// </summary>
    public class ActionApplicationExit : Action
    {
        public ActionApplicationExit(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE | eMachineStatus.UNKNOWN; }
        }

        public override eActions ActionCode
        {
            get { return eActions.APPLICATION_EXIT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            Program.ApplicationExit();
        }
    }
}
