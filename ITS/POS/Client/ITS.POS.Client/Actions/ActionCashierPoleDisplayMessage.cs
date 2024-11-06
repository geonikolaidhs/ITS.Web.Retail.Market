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
    /// Displays a message to the cashier's pole display. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionCashierPoleDisplayMessage : Action
    {

        public ActionCashierPoleDisplayMessage(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.CASHIER_POLE_DISPLAY_MESSAGE; }
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
            if ((parameters as ActionCashierPoleDisplayMessageParams).Lines.Count() > 0)
            {
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                PoleDisplay poleDisplay = deviceManager.GetCashierPoleDisplay();
                if (poleDisplay != null)
                {
                    try
                    {
                        if ((parameters as ActionCashierPoleDisplayMessageParams).Mode == eDisplayTextMode.NORMAL)
                        {
                            poleDisplay.Display((parameters as ActionCashierPoleDisplayMessageParams).Lines,true,true,false);
                        }
                        else if ((parameters as ActionCashierPoleDisplayMessageParams).Mode == eDisplayTextMode.BLINK)
                        {
                            poleDisplay.Blink((parameters as ActionCashierPoleDisplayMessageParams).Lines,true,true,false);
                        }
                    }
                    catch (Exception ex)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.LINE_DISPLAY_ERROR + ": " + ex.GetFullMessage()));
                    }
                }

                Notify(new MessengerParams((parameters as ActionCashierPoleDisplayMessageParams).Lines.Aggregate((f, s) => f + " " + s)));
            }
        }
    }
}
