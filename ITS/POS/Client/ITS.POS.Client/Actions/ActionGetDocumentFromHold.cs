using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Loads a document that has been set on hold. If more than one document is currently on hold, the user is prompted to select one.
    /// </summary>
    public class ActionGetDocumentFromHold: Action
    {

        public ActionGetDocumentFromHold(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.GET_DOCUMENT_FROM_HOLD; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            DocumentHeader selectedDocument;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if(appContext.DocumentsOnHold.Count ==0)
            {
                throw new POSException(POSClientResources.NOT_FOUND);
            }
            else if (appContext.DocumentsOnHold.Count == 1)
            {
                selectedDocument = appContext.DocumentsOnHold.First();
            }
            else
            {
                using (frmDocumentsOnHold frm = new frmDocumentsOnHold(Kernel))
                {
                    DialogResult result = frm.ShowDialog();
                    if(result==DialogResult.OK)
                    {
                        selectedDocument = frm.SelectedObject;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if(selectedDocument == null)
            {
                return;
            }

            DocumentHeader docInList = appContext.DocumentsOnHold.FirstOrDefault(x => x.Oid == selectedDocument.Oid);
            if(docInList == null)
            {
                throw new POSException(POSClientResources.NO_DOCUMENTS_ON_HOLD);
            }

            UnitOfWork uow = sessionManager.GetSession<DocumentHeader>();
            DocumentHeader docHead = uow.GetObjectByKey<DocumentHeader>(selectedDocument.Oid);
            if(docHead == null)
            {
                throw new POSException(POSClientResources.NO_DOCUMENTS_ON_HOLD);
            }
            docHead.DocumentOnHold = false;
            docHead.Save();
            uow.CommitChanges();
            actionManager.GetAction(eActions.LOAD_EXISTING_DOCUMENT).Execute();
            appContext.DocumentsOnHold.Remove(docInList);

        }

    }
}
