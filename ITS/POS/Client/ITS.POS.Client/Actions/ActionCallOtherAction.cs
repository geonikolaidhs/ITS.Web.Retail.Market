using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Executes any external action by action code. 
    /// Custom action codes can be used to remap the actions
    /// </summary>
    public class ActionCallOtherAction : Action
    {
        public ActionCallOtherAction(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.CALL_OTHER_ACTION; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
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
            string actionCode =(parameters as ActionCallOtherActionParams).NewActionCode;
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            IActionExecutor actionExecutor = Kernel.GetModule<IActionExecutor>();

            CustomActionCode customActionCode = actionManager.CustomActionCodes.FirstOrDefault(cac => cac.Code == actionCode);
            if(customActionCode != null)
            {
                actionExecutor.ExecuteAction(customActionCode.Action, "");
            }
            else
            {
                int code;
                if (int.TryParse(actionCode, out code) && actionManager.IsExternalAction((eActions)code))
                {
                    actionExecutor.ExecuteAction((eActions)code, "");
                }
                else
                {
                    throw new POSException(POSClientResources.INVALID_ACTION);
                }
            }

            
        }
    }
}
