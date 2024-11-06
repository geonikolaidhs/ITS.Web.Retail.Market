using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Emulates the keyboard's multiply symbol (*)
    /// </summary>
	public class ActionMultiplyQuantity : Action
	{
        public ActionMultiplyQuantity(IPosKernel kernel) : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.MULTIPLY_QTY; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
		    SendKeys.SendWait("{MULTIPLY}");
		}
	}
}
