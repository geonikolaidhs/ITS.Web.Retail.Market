using ITS.POS.Client.Kernel;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern.ObserverParameters;

namespace ITS.POS.Client.Actions
{
    public class ActionShowBlinkingError : Action
    {
        public ActionShowBlinkingError(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get
            {
                return eActions.SHOW_BLINKING_ERROR;
            }
        }

        public override bool RequiresParameters
        {
            get
            {
                return true;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.UNKNOWN 
                     | eMachineStatus.SALE
                     | eMachineStatus.PAUSE
                     | eMachineStatus.OPENDOCUMENT_PAYMENT
                     | eMachineStatus.OPENDOCUMENT
                     | eMachineStatus.DAYSTARTED
                     | eMachineStatus.CLOSED;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            bool blink = (parameters as ActionShowBlinkingErrorParams).Blink;
            Notify(new BlinkingErrorMessengerParams(blink));
            if (blink)
            {
                Kernel.LogFile.Trace("Blinking Error shown to user.");
            }
        }
    }
}
