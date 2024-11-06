using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.POS.Client.Helpers;
namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes the given document payment's info to the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishDocumentPaymentInfo : Action
    {

        public ActionPublishDocumentPaymentInfo(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionPublishDocumentPaymentInfoParams castedParams = parameters as ActionPublishDocumentPaymentInfoParams;
            DocumentPayment payment = (parameters as ActionPublishDocumentPaymentInfoParams).Payment;
            DocumentHeader header = (parameters as ActionPublishDocumentPaymentInfoParams).DocumentHeader;
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            if (payment != null)
            {
                Notify(new NumberDisplayerParams(payment.Amount, ""));
                if (castedParams.RefreshGrids)
                {
                    Notify(new GridParams(payment));
                }
                if (castedParams.RefreshPoleDisplays)
                {
                    PoleDisplay customerDisplay = deviceManager.GetCustomerPoleDisplay();
                    string[] customerLines = deviceManager.GetPaymentPoleDisplayLines(header, payment, customerDisplay, false);
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));

                    PoleDisplay cashierDisplay = deviceManager.GetCashierPoleDisplay();
                    string[] cashierLines = deviceManager.GetPaymentPoleDisplayLines(header, payment, cashierDisplay, true);
                    actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
                }
            }
            else
            {
                if (castedParams.RefreshPoleDisplays)
                {
                    string firstLine = POSClientResources.REMAINING_AMOUNT.ToUpperGR() + ": " + String.Format("{0:C}", header.GrossTotal);
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(new string[] { firstLine }));
                    actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(new string[] { firstLine }));
                }
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
            get { return eActions.PUBLISH_DOCUMENT_PAYMENT_INFO; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
