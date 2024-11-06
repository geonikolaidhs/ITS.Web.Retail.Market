using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes the given document header's info to the observers attached to this action. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishDocumentInfo : Action
    {

        public ActionPublishDocumentInfo(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_INFO; }
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

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            DocumentHeader header = (parameters as ActionPublishDocumentInfoParams).Header;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            if (header != null)
            {
                Notify(new NumberDisplayerParams(header.GrossTotal, ""));
                if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
                {
                    Notify(new GridParams(header, eDisplayMode.DETAILS));
                }
                else if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                {
                    Notify(new GridParams(header, eDisplayMode.PAYMENTS));
                }
                decimal totalQty = header.DocumentDetails.Where(x=>x.IsCanceled == false).Sum(x=>x.Qty);
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_QUANTITY).Execute(new ActionPublishDocumentQuantityInfoParams(totalQty,""),true);
            }
            else
            {
                Notify(new NumberDisplayerParams(0, ""));
                Notify(new GridParams());
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_QUANTITY).Execute(new ActionPublishDocumentQuantityInfoParams(0, ""),true);
            }
        }

        /// <summary>
        /// Gets a document header and passes it's info to the observers attached to this action. 
        /// </summary>
        /// <param name="header"></param>
        public void Execute(DocumentHeader header)
        {
            Execute(new ActionPublishDocumentInfoParams(header));
        }

    }
}
