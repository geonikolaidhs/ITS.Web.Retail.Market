using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Sets the current document "On Hold" and 
    /// </summary>
    public class ActionSetDocumentOnHold : Action
    {
        public ActionSetDocumentOnHold(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SET_DOCUMENT_ON_HOLD; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.CurrentDocument == null)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams("No Document Found"));
                return;
            }
            if (appContext.CurrentDocument.DocumentDetails.Count == 0)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.NO_ITEMS_ADDED));
                return;
            }
            if (appContext.DocumentsOnHold.Contains(appContext.CurrentDocument))
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams("Document Already on  List"));
                return;
            }
            appContext.CurrentDocument.HasBeenOnHold = true;
            appContext.CurrentDocument.DocumentOnHold = true;
            appContext.CurrentDocument.Save();
            appContext.CurrentDocument.Session.CommitTransaction();

            appContext.DocumentsOnHold.Add(appContext.CurrentDocument);
            appContext.CurrentDocument = null;
            appContext.CurrentDocumentLine = null;
            appContext.CurrentDocumentPayment = null;
            appContext.CurrentCustomer = null;
            appContext.SetMachineStatus(eMachineStatus.SALE, messageDelay: 3000);

            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(null));
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(null, true, false));

        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
