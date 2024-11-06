using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Adds the given payment method with the given amount to the current document.
    /// If the given amount is null, then the remaining amount to be paid is used instead
    /// </summary>
    public class ActionAddTotalPayment : Action
    {
        public ActionAddTotalPayment(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_TOTAL_PAYMENT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.CurrentDocument != null)
            {
                ActionAddTotalPaymentParams castedParameters = parameters as ActionAddTotalPaymentParams;
                decimal paidAmount = appContext.CurrentDocument.DocumentPayments.Sum(x => x.Amount);
                decimal finalAmount =0;
                if (castedParameters.Amount == null)
                {
                    finalAmount = appContext.CurrentDocument.GrossTotal - paidAmount;
                }
                else
                {
                    finalAmount = castedParameters.Amount.Value;
                }

                if (finalAmount <= 0 && castedParameters.Amount == null)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.TOTAL_ALREADY_PAID));
                }
                else
                {
                    actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(castedParameters.PaymentMethod, finalAmount));
                }
            }
        }
    }
}
