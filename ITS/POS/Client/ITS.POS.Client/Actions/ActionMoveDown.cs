using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.UserControls;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Notifies all the line displaying components to move to the next line
    /// </summary>
	public class ActionMoveDown : Action
	{

        public ActionMoveDown(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.MOVE_DOWN; }
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
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (appContext.MainForm.ContainsFocus == true)
            {
                Notify(new GridParams(eNavigation.MOVEDOWN));
            }
            else
            {
                Notify(new ItemGridParams(eNavigation.MOVEDOWN));
            }
        }
	}
}
