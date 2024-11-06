using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System.Diagnostics;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Shutdowns the machine 
    /// </summary>
    public class ActionShutDown : Action
    {

        public ActionShutDown(IPosKernel kernel) : base(kernel)
        {

        }

        /// <summary>
        /// Closes the Application.
        /// </summary>
        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -f -s -t 0";
            Process.Start(proc);
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SHUTDOWN; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
