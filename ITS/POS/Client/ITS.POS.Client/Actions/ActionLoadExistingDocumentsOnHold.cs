using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;


namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Used at application startup. Loads the list of the documents that are set "on hold". For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionLoadExistingDocumentsOnHold: Action
    {

        public ActionLoadExistingDocumentsOnHold(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }


        public override eActions ActionCode
        {
            get { return eActions.LOAD_EXISTING_DOCUMENTS_ON_HOLD; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();

            UnitOfWork uow = sessionManager.GetSession<DocumentHeader>();
            XPCollection<DocumentHeader> headers = new XPCollection<DocumentHeader>(uow, CriteriaOperator.And(
                    new BinaryOperator("IsOpen", true),
                    new BinaryOperator("DocumentOnHold", true)
                    ));
            appContext.DocumentsOnHold.Clear();
            appContext.DocumentsOnHold.AddRange(headers);
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
