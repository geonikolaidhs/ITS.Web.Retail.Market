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
    /// Publishes a key press to all the observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishKeyPress : Action
    {

        public ActionPublishKeyPress(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            POSKeyMapping keyPress = (parameters as ActionPublishKeyPressParams).KeyPress;
            Notify(new KeyListenerParams(keyPress.ActionCode, keyPress.NotificationType, keyPress.RedirectTo, keyPress.KeyData,keyPress.ActionParameters));
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
            get { return eActions.PUBLISH_KEY_PRESS; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
