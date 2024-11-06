using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Model.Master;
using ITS.Retail.Platform.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Adds the given document discount to the current document
    /// </summary>
    public class ActionAddDocumentDiscount : Action
    {
        public ActionAddDocumentDiscount(IPosKernel kernel)
            : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
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
            get { return eActions.ADD_DOCUMENT_DISCOUNT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionAddDocumentDiscountParams castedParameters = (parameters as ActionAddDocumentDiscountParams);

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IPlatformDocumentDiscountService platformDocumentDiscountservice = Kernel.GetModule<IPlatformDocumentDiscountService>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            DocumentHeader header = appContext.CurrentDocument;
            DiscountType discountType = castedParameters.DiscountType;

            if (header.DocumentDetails.Where(x => x.IsTax == false && x.DoesNotAllowDiscount == false).Count() < 1)
            {
                throw new POSException(POSClientResources.INVALID_DISCOUNT);
            }


            if (header.DocumentPayments.Count() > 0)
            {
                throw new POSException(POSClientResources.INVALID_DISCOUNT);
            }

            decimal taxesValue = 0;
            taxesValue = documentService.GetTotalNonDiscountableValue(appContext.CurrentDocument);

            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                decimal percentage = castedParameters.ValueOrPercentage;
                if (percentage > 1 || percentage < 0)
                {
                    throw new POSException(POSClientResources.INVALID_DISCOUNT);
                }
            }
            else
            {
                decimal value = castedParameters.ValueOrPercentage;
                DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
                decimal amountToCheck = docType.IsForWholesale ? header.NetTotal - taxesValue : header.GrossTotal - taxesValue;
                if (value > amountToCheck || value < 0)
                {
                    throw new POSException(POSClientResources.INVALID_DISCOUNT);
                }
            }

            if (header != null)
            {
                documentService.ApplyCustomDocumentHeaderDiscount(ref header, castedParameters.ValueOrPercentage, discountType, castedParameters.CouponViewModel);
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                appContext.CurrentDocument.Save();
                sessionManager.CommitTransactionsChanges();
            }
        }
    }
}