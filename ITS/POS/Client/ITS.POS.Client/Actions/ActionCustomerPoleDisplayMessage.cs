using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays a message to the customer's pole display. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionCustomerPoleDisplayMessage : Action
    {
        public ActionCustomerPoleDisplayMessage(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.CUSTOMER_POLE_DISPLAY_MESSAGE; }
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

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            if ((parameters as ActionCustomerPoleDisplayMessageParams).Lines.Count() > 0)
            {
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                PoleDisplay poleDisplay = deviceManager.GetCustomerPoleDisplay();
                if (poleDisplay != null)
                {
                    try
                    {
                        if ((parameters as ActionCustomerPoleDisplayMessageParams).Mode == eDisplayTextMode.NORMAL)
                        {
                            poleDisplay.Display((parameters as ActionCustomerPoleDisplayMessageParams).Lines,true,true,true);
                        }
                        else if ((parameters as ActionCustomerPoleDisplayMessageParams).Mode == eDisplayTextMode.BLINK)
                        {
                            poleDisplay.Blink((parameters as ActionCustomerPoleDisplayMessageParams).Lines,true,true,true);
                        }
                    }
                    catch (Exception ex)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.LINE_DISPLAY_ERROR + ": " + ex.GetFullMessage()));
                    }
                }
                Notify(new MessengerParams((parameters as ActionCustomerPoleDisplayMessageParams).Lines.Aggregate((f, s) => f + " " + s)));
            }
        }
    }
}
