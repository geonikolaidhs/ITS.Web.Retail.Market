using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// A generic cancel action that can be used to cancel a line, a discount, a payment or the whole document
    /// </summary>
    public class ActionGenericCancel : Action
    {
        public ActionGenericCancel(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.GENERIC_CANCEL; }
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

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionGenericCancelParams castedParams = parameters as ActionGenericCancelParams;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            switch (castedParams.CancelMode)
            {
                case eActionGenericCancelMode.CANCEL_LINE:
                    if (appContext.CurrentDocumentLine != null)
                    {
                        actionManager.GetAction(eActions.DELETE_ITEM).Execute(new ActionDeleteItemParams(appContext.CurrentDocumentLine,true));
                    }
                    break;
                case eActionGenericCancelMode.CANCEL_DOCUMENT:
                    actionManager.GetAction(eActions.CANCEL_DOCUMENT).Execute(new ActionCancelDocumentParams(true));
                    break;
                case eActionGenericCancelMode.CANCEL_DISCOUNT:
                    actionManager.GetAction(eActions.CANCEL_DISCOUNT).Execute();
                    break;
                case eActionGenericCancelMode.CANCEL_PAYMENT:
                    if (appContext.CurrentDocumentPayment != null)
                    {
                        actionManager.GetAction(eActions.DELETE_PAYMENT).Execute(new ActionDeletePaymentParams(appContext.CurrentDocumentPayment));
                    }
                    break;
            }
        }
    }

}
