using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Master;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// After user confirmation, cancels the current document line
    /// </summary>
    public class ActionDeleteItem : Action
    {

        public ActionDeleteItem(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.DELETE_ITEM; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION2;
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        /// <summary>
        /// Deletes a document line from its header after user confirmation.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dontCheckPermissions"></param>
        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            DocumentDetail detail = (parameters as ActionDeleteItemParams).Detail;
            IDocumentService DocumentService = Kernel.GetModule<IDocumentService>();
            if (detail.IsLinkedLine)
            {
                return;
            }
            DocumentHeader header = detail.DocumentHeader;
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IFormManager FormManager = Kernel.GetModule<IFormManager>();
            ISessionManager SessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            DialogResult result = DialogResult.OK;
            if ((parameters as ActionDeleteItemParams).ShowDialog)
            {
                result = FormManager.ShowMessageBox(POSClientResources.DO_YOU_WANT_TO_CANCEL_LINE, MessageBoxButtons.OKCancel);//MessageBox.Show(POSClientResources.DO_YOU_WANT_TO_CANCEL, POSClientResources.CANCEL_RECEIPT, MessageBoxButtons.OKCancel);
            }

            if (result == DialogResult.OK)
            {
                DocumentService.CancelDocumentLine(detail);
                IEnumerable<DocumentDetail> headerDocumentDetailsWhere = header.DocumentDetails.Where(x => x.IsCanceled == false);
                AppContext.CurrentDocumentLine = headerDocumentDetailsWhere.Count() > 0 ? headerDocumentDetailsWhere.Last() : null;

                AppContext.CurrentDocument.Save();
                if (AppContext.CurrentDocumentLine != null)
                {
                    AppContext.CurrentDocumentLine.Save();
                }
                SessionManager.CommitTransactionsChanges();
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                actionManager.GetAction(eActions.MOVE_UP).Execute();
            }
        }


    }
}
