using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes the given document detail's info to the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishDocumentLineInfo : Action
    {

        public ActionPublishDocumentLineInfo(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionPublishDocumentLineInfoParams castedParameters = (parameters as ActionPublishDocumentLineInfoParams);
            if (castedParameters.Detail != null)
            {
                Notify(new NumberDisplayerParams(castedParameters.Detail.GrossTotal, ""));
                Notify(new DocumentDetailDisplayerParams(castedParameters.Detail));
                if (castedParameters.RefreshGrids)
                {
                    Notify(new GridParams(castedParameters.Detail));
                }

                if (castedParameters.RefreshPoleDisplays)
                {
                    IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                    IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                    PoleDisplay customerDisplay = deviceManager.GetCustomerPoleDisplay();
                    string[] customerLines = deviceManager.GetDocumentDetailPoleDisplayLines(castedParameters.Detail, false);
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));
                    string[] cashierLines = deviceManager.GetDocumentDetailPoleDisplayLines(castedParameters.Detail, true);
                    actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
                }
            }
            else
            {
                Notify(new NumberDisplayerParams(0, ""));
            }
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
            get { return eActions.PUBLISH_DOCUMENT_LINE_INFO; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
