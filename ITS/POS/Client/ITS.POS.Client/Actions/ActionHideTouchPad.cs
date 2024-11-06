using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Hides the Touchpad. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionHideTouchPad : Action
    {

        public ActionHideTouchPad(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.HIDE_TOUCH_PAD; }
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

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            Application.DoEvents();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (appContext.TouchPadPopup !=null && appContext.TouchPadPopup.Visible == true)
            {
                appContext.TouchPadPopup.Hide();
            }
        }
    }
}
