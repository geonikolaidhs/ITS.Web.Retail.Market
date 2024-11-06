using System;
using DevExpress.Xpo;
using System.Linq;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Hardware;
using System.Data;
using System.Collections.Generic;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Common.Helpers;
using ITS.POS.Resources;
using System.Media;
using ITS.POS.Client.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Tries to add the given scanned code to the document. If a document is not already open, "ActionStartNewDocument" is called
    /// </summary>
    public class ActionAddItem : Action
    {

        public ActionAddItem(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM; }
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

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IScannedCodeHandler scannedCodeHandler = Kernel.GetModule<IScannedCodeHandler>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.GetMachineStatus() == eMachineStatus.SALE)
            {
                actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(true), dontCheckPermissions: true);
            }

            string errorMessage = string.Empty;
            if (MaxCountOfLinesExceeded(out errorMessage))
            {
                throw new POSException(errorMessage);
            }

            ActionAddItemParams castedParams = parameters as ActionAddItemParams;
            string itemCode = castedParams.ItemCode;
            using (IGetItemPriceForm form = new frmGetItemPrice(Kernel)) 
            {
                scannedCodeHandler.HandleScannedCode(form, config.GetAppSettings(), config.POSSellsInactiveItems, itemCode, castedParams.Quantity, castedParams.CheckForWeightedItem, false, false, castedParams.ReadWeight);
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.SALE; }
        }

        /// <summary>
        /// Checks if document's maximum count of lines is exceeded
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="errorMessage">The error message to be returned</param>
        /// <returns>True if count is exceeded, false otherwise</returns>
        public bool MaxCountOfLinesExceeded(out string errorMessage)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();

            if (appContext.CurrentDocument == null || appContext.CurrentDocument.DocumentType == Guid.Empty)
            {
                throw new POSException(Resources.POSClientResources.INVALID_ACTION);
            }

            Guid documentTypeGuid = appContext.CurrentDocument.DocumentType;
            DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(documentTypeGuid);
            if (documentType == null)
            {
                throw new POSException(Resources.POSClientResources.INVALID_ACTION);
            }

            errorMessage = "";
            if (documentType.MaxCountOfLines > 0 && appContext.CurrentDocument.DocumentDetails.Count >= documentType.MaxCountOfLines)
            {
                errorMessage = Resources.POSClientResources.MAX_COUNT_OF_LINES;
                return true;
            }
            return false;
        }
    }
}
