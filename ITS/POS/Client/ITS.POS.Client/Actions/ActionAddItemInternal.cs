using System;
using System.Linq;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Creates a document detail from the given parameters and adds it to the current document. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionAddItemInternal : Action
    {
        public ActionAddItemInternal(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM_INTERNAL; }
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
                return eKeyStatus.POSITION0;
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            DocumentDetail line = null;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IItemService itemService = Kernel.GetModule<IItemService>();
            IPriceCreator priceCreator = Kernel.GetModule<IPriceCreator>();

            try
            {
                ActionAddItemInternalParams castedParameters = (parameters as ActionAddItemInternalParams);
                DocumentHeader header = appContext.CurrentDocument;
                DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

                decimal quantity = castedParameters.Quantity;
                if (castedParameters.IsReturn && quantity > 0)
                {
                    quantity = -quantity;
                }
                if (docType.MaxDetailQty > 0 && (quantity > docType.MaxDetailQty || quantity < (-1) * docType.MaxDetailQty))
                {
                    throw new POSException(POSClientResources.INVALID_QUANTITY);
                }

                decimal customPrice = castedParameters.CustomPrice;
                if (docType.MaxDetailValue > 0 && customPrice > docType.MaxDetailValue)
                {
                    throw new POSException(POSClientResources.INVALID_AMOUNT);
                }
                if (docType.MaxDetailTotal>0 && Math.Abs(quantity * customPrice) > docType.MaxDetailTotal)
                {
                    throw new POSException(POSClientResources.INVALID_TOTAL);
                }

                PriceCatalogDetail pcd = castedParameters.PriceCatalogDetail;
                Item item = castedParameters.Item;
                bool hasCustomPrice = castedParameters.HasCustomPrice;
                Barcode userBarcode = castedParameters.UserBarcode;
                string customDescription = castedParameters.CustomDescription;
                bool isReturn = castedParameters.IsReturn;
                bool fromScanner = castedParameters.FromScanner;

                if (docType.MaxCountOfLines > 0 && header.DocumentDetails.Count >= docType.MaxCountOfLines)
                {
                    throw new POSException(POSClientResources.MAX_COUNT_OF_DOC_LINES_EXCEEDED);
                }

                if (itemService.CheckIfItemIsValidForDocumentType(item, docType) == false)
                {
                    throw new POSException(POSClientResources.DOCUMENT_TYPE + ":" + POSClientResources.ITEM_IS_INACTIVE);
                }


                if ((quantity % 1) != 0)  //if qty is NOT integer
                {
                    Guid muGuid = userBarcode == null ? sessionManager.GetObjectByKey<Barcode>(item.DefaultBarcode).MeasurementUnit(item.Owner) : userBarcode.MeasurementUnit(item.Owner);
                    MeasurementUnit mu = sessionManager.GetObjectByKey<MeasurementUnit>(muGuid);
                    if (mu != null && !mu.SupportDecimal)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(item.Code + " - " + POSClientResources.ITEM_DOES_NOT_SUPPORT_DECIMAL_QUANTITY));
                        return;
                    }
                }

                if (isReturn && item != null)
                {
                    customDescription = Resources.POSClientResources.RETURN_PRINTED + ": " + item.Name;
                }

                if (config.GetAppSettings().UseBarcodeRelationFactor && userBarcode != null)
                {
                    decimal rf = userBarcode.RelationFactor(item.Owner);
                    if (rf != 0)
                    {
                        quantity *= rf;
                    }
                }


                line = documentService.CreateDocumentLine(header, appContext.CurrentUser.Oid, item, pcd, userBarcode, quantity, customPrice, hasCustomPrice, isReturn, customDescription);
                line.FromScanner = fromScanner;
                documentService.ComputeDocumentHeader(ref header, false, line);

                if (config.GetAppSettings().POSCanSetPrices && (pcd == null || pcd.DatabaseValue == 0) && hasCustomPrice)
                {
                    Guid defaultPriceCatalogPolicyGuid = sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid).DefaultPriceCatalogPolicy;
                    PriceCatalogPolicy defaultPriceCatalogPolicy = sessionManager.GetObjectByKey<PriceCatalogPolicy>(defaultPriceCatalogPolicyGuid);

                    if (defaultPriceCatalogPolicy == null)
                    {
                        throw new POSException(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
                    }

                    string newPcdJson = priceCreator.CreateOrUpdatePrice(defaultPriceCatalogPolicy, item, customPrice, line.PriceCatalogValueVatIncluded);
                    line.POSGeneratedPriceCatalogDetailSerialized = newPcdJson;
                }

                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                appContext.CurrentDocumentLine = line;
                appContext.CurrentDocument.TotalQty = appContext.CurrentDocument.DocumentDetails.Where(x => x.IsCanceled != true).Sum(x => x.Qty);
                appContext.CurrentDocument.Save();
                appContext.CurrentDocumentLine.Save();
                sessionManager.GetSession<DocumentHeader>().CommitTransaction();

            }
            catch (Exception e)
            {
                Kernel.LogFile.Info(e, "ActionAddItemInternal:Execute,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
                formManager.ShowCancelOnlyMessageBox(e.GetFullMessage());
                if (line != null)
                {
                    line.Delete();
                }
            }

        }

    }
}
