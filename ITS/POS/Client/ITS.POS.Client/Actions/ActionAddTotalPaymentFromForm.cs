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
    /// Opens up list of all payment methods in a SelectLookUpForm so that the user can select which payment method to apply. 
    /// Afterwards "ActionAddTotalPayment" is called
    /// </summary>
    public class ActionAddTotalPaymentFromForm : Action
    {
        public ActionAddTotalPaymentFromForm(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_TOTAL_PAYMENT_FROM_FORM; }
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
            ActionAddTotalPaymentFromFormParams castedParams = parameters as ActionAddTotalPaymentFromFormParams;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.CurrentDocument != null)
            {
                decimal paidAmount = appContext.CurrentDocument.DocumentPayments.Sum(x => x.Amount);
                decimal finalAmount = 0;
                if (castedParams.Amount == null)
                {
                    finalAmount = appContext.CurrentDocument.GrossTotal - paidAmount;
                }
                else
                {
                    finalAmount = castedParams.Amount.Value;
                }

                if (finalAmount <= 0)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.TOTAL_ALREADY_PAID));
                }
                else
                {
                    actionManager.GetAction(eActions.ADD_PAYMENT_FROM_FORM).Execute(new ActionAddPaymentFromFormParams(finalAmount));
                }
            }
        }
    }
}
