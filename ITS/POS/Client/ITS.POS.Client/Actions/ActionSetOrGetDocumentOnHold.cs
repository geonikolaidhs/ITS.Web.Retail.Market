using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Depending on current state, sets the current document on hold or loads a document that has been set on hold. 
    /// If more than one document is currently on hold, the user is prompted to select one.
    /// </summary>
    public class ActionSetOrGetDocumentOnHold : Action
    {
        public ActionSetOrGetDocumentOnHold(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.SALE;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.SET_OR_GET_DOCUMENT_ON_HOLD; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            if (appContext.GetMachineStatus() == eMachineStatus.SALE)
            {
                actionManager.GetAction(eActions.GET_DOCUMENT_FROM_HOLD).Execute(dontCheckPermissions: true);
            }
            else
            {
                actionManager.GetAction(eActions.SET_DOCUMENT_ON_HOLD).Execute(dontCheckPermissions: true);
            }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
