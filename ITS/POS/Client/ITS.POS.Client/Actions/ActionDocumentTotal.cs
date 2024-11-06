using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Moves the current open transaction to the next logical step by performing all required intermediate logic. 
    /// If the current state is the OPENDOCUMENT state, the application moves to the OPENDOCUMENT_PAYMENT state
    /// If the current state is the OPENDOCUMENT_PAYMENT state, the application tries to finallize the transaction and return to the SALES state
    /// </summary>
    public class ActionDocumentTotal : Action
    {
        public ActionDocumentTotal(IPosKernel kernel)
            : base(kernel)
        {

        }


        public override eActions ActionCode
        {
            get { return eActions.DOCUMENT_TOTAL; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }


        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IPromotionService promotionService = Kernel.GetModule<IPromotionService>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();


            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
            {
                MoveToPayments(deviceManager, appContext, documentService, config, promotionService, customerService, formManager, actionManager);
            }
            else if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                Application.DoEvents();
                if (appContext.CurrentDocument == null || appContext.CurrentDocument.GrossTotal < 0)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.NEGATIVE_TOTAL_NOT_PERMITED));
                }
                else
                {
                    TryToCloseDocument(deviceManager, appContext, documentService, config, formManager, sessionManager, totalizersService, actionManager);
                }
            }
        }

        private void TryToCloseDocument(IDeviceManager deviceManager, IAppContext appContext, IDocumentService documentService, IConfigurationManager config, IFormManager formManager, ISessionManager sessionManager, ITotalizersService totalizersService, IActionManager actionManager)
        {

            decimal amountPaid = appContext.CurrentDocument.DocumentPayments.Sum(x => x.Amount);

            IEnumerable<DocumentVatInfo> vatGrossTotals = documentService.GetDocumentVatAnalysis(appContext.CurrentDocument);

            DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);

            if (documentType.UsesPaymentMethods && appContext.CurrentDocument.GrossTotal > amountPaid)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.TOTAL_NOT_FULLY_PAID));
            }
            else
            {
                IEnumerable<DocumentVatInfo> negativeVatCategoryTotals = vatGrossTotals.Where(x => x.GrossTotal < 0);

                if (negativeVatCategoryTotals.Count() > 0 && config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    DocumentVatInfo firstNegative = negativeVatCategoryTotals.First();
                    string message = String.Format(POSClientResources.NEGATIVE_TOTAL_PER_VAT_CATEGORY_NOT_PERMITED, firstNegative.VatCategoryDescription, firstNegative.GrossTotal);
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                }
                else
                {
                    if (documentService.CheckIfShouldGiveChange(appContext.CurrentDocument))
                    {
                        actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(new string[]
                            {
                                POSClientResources.TOTAL.ToUpperGR() + ": " + String.Format("{0:C}", appContext.CurrentDocument.GrossTotal),
                                POSClientResources.CHANGE.ToUpperGR() + ":  " + String.Format("{0:C}", appContext.CurrentDocument.Change)
                            }));
                        actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(new string[]
                            {
                                POSClientResources.TOTAL.ToUpperGR() + ": " + String.Format("{0:C}", appContext.CurrentDocument.GrossTotal),
                                POSClientResources.CHANGE.ToUpperGR() + ":  " + String.Format("{0:C}", appContext.CurrentDocument.Change)
                            }));
                    }
                    else
                    {
                        actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(new string[]
                            {
                                POSClientResources.TOTAL.ToUpperGR() + ": " + String.Format("{0:C}", appContext.CurrentDocument.GrossTotal)
                            }));
                        actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(new string[]
                            {
                                POSClientResources.TOTAL.ToUpperGR() + ": " + String.Format("{0:C}", appContext.CurrentDocument.GrossTotal)
                            }));
                    }
                    DialogResult result = DialogResult.OK;
                    do
                    {
                        try
                        {
                            if (config.FiscalMethod == eFiscalMethod.ADHME && documentType.Oid == config.DefaultDocumentTypeOid)
                            {
                                FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                                int fiscalPrinterReceipts = -1;
                                CriteriaOperator filter =
                                           CriteriaOperator.And(
                                                    new BinaryOperator("IsOpen", false),
                                                    new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                                                    new BinaryOperator("IsFiscalPrinterHandled", true),
                                                    new BinaryOperator("UserDailyTotals.DailyTotals.Oid", appContext.CurrentDailyTotals.Oid));

                                int applicationReceipts = new XPCollection<DocumentHeader>(sessionManager.GetSession<DocumentHeader>(), filter).Count;

                                if (printer == null)
                                {
                                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                                }

                                printer.GetCurrentDayReceiptsCount(out fiscalPrinterReceipts);

                                if (fiscalPrinterReceipts >= 0)
                                {
                                    int difference = printer.ReceiptsDifference(fiscalPrinterReceipts, applicationReceipts);
                                    //Must close open document, fiscal printer already printed it
                                    //set IsFiscalPrinterHandled true to keep up with printer sequence
                                    if (difference == 1 && !printer.FiscalStatus.TransactionOpen)
                                    {
                                        Kernel.LogFile.Error(String.Format(POSClientResources.FISCAL_PRINTER_RECEIPTS_COUNT_MISSMATCH + " Difference 1 ", applicationReceipts, fiscalPrinterReceipts));
                                        DocumentHeader existing = sessionManager.FindObject<DocumentHeader>(CriteriaOperator.And(
                                                                                                        new BinaryOperator("IsOpen", false),
                                                                                                        new BinaryOperator("DocumentNumber", fiscalPrinterReceipts),
                                                                                                        new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                                                                                                        new BinaryOperator("UserDailyTotals.DailyTotals.Oid", appContext.CurrentDailyTotals.Oid)));
                                        if (existing == null && appContext.CurrentDocument.IsFiscalPrinterHandled)
                                        {
                                            appContext.CurrentDocument.FiscalPrinterNumber = fiscalPrinterReceipts;
                                            actionManager.GetAction(eActions.CLOSE_DOCUMENT).Execute(new ActionCloseDocumentParams(true, true), true);
                                            break;
                                        }

                                    }
                                    else if (difference > 1 || difference < 0)
                                    {
                                        try
                                        {
                                            Kernel.LogFile.Error(String.Format(POSClientResources.FISCAL_PRINTER_RECEIPTS_COUNT_MISSMATCH, applicationReceipts, fiscalPrinterReceipts));
                                            int fiscalPrinterNextNumber = fiscalPrinterReceipts + 1;

                                            DocumentHeader existingThatMustBeCanceled = sessionManager.FindObject<DocumentHeader>(
                                                CriteriaOperator.And(filter, new BinaryOperator("DocumentNumber", fiscalPrinterNextNumber), new BinaryOperator("IsOpen", false)));

                                            if (existingThatMustBeCanceled != null)
                                            {
                                                bool increaseVatAndSales = false;
                                                if (documentType != null && documentType.IncreaseVatAndSales == true
                                                && documentType.Oid != config.ProFormaInvoiceDocumentTypeOid
                                                && documentType.Oid != config.WithdrawalDocumentTypeOid
                                                && documentType.Oid != config.DepositDocumentTypeOid
                                                && documentType.Oid != config.SpecialProformaDocumentTypeOid

                                                 )
                                                {
                                                    increaseVatAndSales = true;
                                                }
                                                totalizersService.UpdateTotalizers(config, existingThatMustBeCanceled, appContext.CurrentUser, appContext.CurrentDailyTotals,
                                                                                           appContext.CurrentUserDailyTotals, config.CurrentStoreOid, config.CurrentTerminalOid,
                                                                                           increaseVatAndSales, printer.IsTotalVatCalculationPerReceipt, eTotalizorAction.DECREASE);

                                                existingThatMustBeCanceled.IsCanceled = true;

                                                totalizersService.UpdateTotalizers(config, existingThatMustBeCanceled, appContext.CurrentUser, appContext.CurrentDailyTotals,
                                                                                            appContext.CurrentUserDailyTotals, config.CurrentStoreOid, config.CurrentTerminalOid,
                                                                                            increaseVatAndSales, printer.IsTotalVatCalculationPerReceipt, eTotalizorAction.INCREASE);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Kernel.LogFile.Error(e, "ActionDocumentTotal:FixingPrinterReceiptsCountMissmatch,Exception catched");
                                            result = formManager.ShowMessageBox(String.Format(POSClientResources.ERROR_DURING_RECEIPT, e.GetFullMessage()), MessageBoxButtons.RetryCancel);
                                        }

                                    }
                                }
                            }


                            actionManager.GetAction(eActions.CLOSE_DOCUMENT).Execute(new ActionCloseDocumentParams(false, false), true);
                            result = DialogResult.OK;
                        }
                        catch (Exception e)
                        {
                            Kernel.LogFile.Error(e, "ActionDocumentTotal:Execute,Exception catched");
                            result = formManager.ShowMessageBox(String.Format(POSClientResources.ERROR_DURING_RECEIPT, e.GetFullMessage()), MessageBoxButtons.RetryCancel);
                        }
                    } while (result == DialogResult.Retry);
                }
            }
        }

        private void MoveToPayments(IDeviceManager deviceManager, IAppContext appContext, IDocumentService documentService, IConfigurationManager config, IPromotionService promotionService, ICustomerService customerService, IFormManager formManager, IActionManager actionManager)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IPlatformDocumentDiscountService platformDocumentDiscountservice = Kernel.GetModule<IPlatformDocumentDiscountService>();

            string errormessage;
            if (documentService.CheckIfDocumentIsValidForMovingToPayment(appContext.CurrentDocument, out errormessage) == false)
            {
                throw new POSUserVisibleException(errormessage);
            }
            else
            {
                appContext.CurrentDocumentPayment = null;
                ////Promotions
                Device primaryPrinter = config.FiscalMethod == eFiscalMethod.EAFDSS ? deviceManager.GetPrimaryDevice<Printer>() as Device : deviceManager.GetPrimaryDevice<FiscalPrinter>() as Device;
                //PriceCatalog priceCatalog = customerService.GetPriceCatalog(appContext.CurrentDocument.Customer, config.CurrentStoreOid);
                PriceCatalogPolicy priceCatalogPolicy = sessionManager.FindObject<PriceCatalogPolicy>(new BinaryOperator("Oid", appContext.CurrentDocument.PriceCatalogPolicy));

                if (priceCatalogPolicy == null)
                {
                    customerService.GetPriceCatalogPolicy(appContext.CurrentDocument.Customer, config.CurrentStoreOid);
                }
                try
                {
                    decimal grossTotalBeforeCustomDocumentDiscount = platformDocumentDiscountservice.GetDocumentHeaderGrossTotalBeforeDiscountBySource(appContext.CurrentDocument, eDiscountSource.DOCUMENT);
                    promotionService.ExecutePromotions(appContext.CurrentDocument, priceCatalogPolicy, config.GetAppSettings(), DateTime.Now, config.DemoMode);
                    promotionService.ExecutePromotionResults(appContext.CurrentDocument, ePromotionResultExecutionPlan.BEFORE_DOCUMENT_CLOSED, primaryPrinter, formManager, Kernel.LogFile);

                }
                catch (Exception ex)
                {
                    promotionService.ClearDocumentPromotions(appContext.CurrentDocument);
                    if (ex is DocumentNegativeTotalException)
                    {
                        throw;
                    }
                    Kernel.LogFile.Error(ex, "ActionDocumentTotal:Executing Promotions,Exception catched");
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.PROMOTIONS_EXECUTION_ERROR + ": " + ex.GetFullMessage());
                }

                try
                {

                    DocumentHeader header = appContext.CurrentDocument;
                    documentService.ApplyDocumentTotalDiscounts(header);
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "ActionDocumentTotal:Executing Customer And Default Document Discount,Exception catched");
                }

                //Loyalty
                ISynchronizationManager SynchronizationManager = Kernel.GetModule<ISynchronizationManager>();
                if (documentService.CheckLoyaltyRefund(appContext.CurrentDocument) && SynchronizationManager.ServiceIsAlive)
                {
                    if (formManager.ShowMessageBox(POSClientResources.APPLY_POINTS_DISCOUNT, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        documentService.ApplyLoyalty(appContext.CurrentDocument, actionManager);
                    }
                }

                //Document Total Discount
                if (appContext.CurrentDocument.DocumentDiscountAmount != 0)
                {
                    DiscountType documentDiscountType = sessionManager.GetObjectByKey<DiscountType>(appContext.CurrentDocument.DocumentDiscountType);
                    CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("eDiscountType", eDiscountType.PERCENTAGE), new BinaryOperator("IsHeaderDiscount", true));
                    documentDiscountType = sessionManager.FindObject<DiscountType>(crit);

                    decimal discount = documentDiscountType.eDiscountType == eDiscountType.PERCENTAGE
                                                                            ? appContext.CurrentDocument.DocumentDiscountPercentage
                                                                            : appContext.CurrentDocument.DocumentDiscountAmount;

                    DocumentHeader documentHeader = appContext.CurrentDocument;
                    documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, discount, documentDiscountType);
                }



                documentService.FixDocumentDiscountDeviations(appContext.CurrentDocument);

                appContext.CurrentDocument.Save();
                appContext.CurrentDocument.Session.CommitTransaction();

                DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
                if (documentType.UsesPaymentMethods)
                {
                    appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT_PAYMENT);
                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(appContext.CurrentDocument));
                }
                else
                {
                    DialogResult dialogResult =
                            formManager.ShowMessageBox(
                                                string.Format(POSClientResources.DOCUMENT_TYPE_DOES_NOT_USE_PAYMENT_METHODS_DO_YOU_WANT_TO_CLOSE_DOCUMENT, documentType.Description),
                                                MessageBoxButtons.OKCancel);
                    switch (dialogResult)
                    {
                        case DialogResult.OK:
                            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
                            TryToCloseDocument(deviceManager, appContext, documentService, config, formManager, sessionManager, totalizersService, actionManager);
                            return;
                        case DialogResult.Cancel:
                            //Revert Loyalty
                            try
                            {
                                documentService.ClearAppliedLoyalty(appContext.CurrentDocument);
                            }
                            catch (Exception ex)
                            {
                                Kernel.LogFile.Error(ex, "Error trying to clear Loyalty");
                            }

                            //Revert promotions
                            try
                            {
                                OwnerApplicationSettings appSettings = config.GetAppSettings();
                                promotionService.ClearDocumentPromotions(appContext.CurrentDocument);
                            }
                            catch (Exception ex)
                            {
                                Kernel.LogFile.Error(ex, "Error trying to clear Document Promotions");
                            }

                            //Revert Document Total Discount
                            try
                            {
                                OwnerApplicationSettings appSettings = config.GetAppSettings();
                                documentService.ClearDocumentTotalDiscounts(appContext.CurrentDocument);
                            }
                            catch (Exception ex)
                            {
                                Kernel.LogFile.Error(ex, "Error trying to clear Document Discounts");
                            }

                            //save document
                            appContext.CurrentDocument.Save();
                            appContext.CurrentDocument.Session.CommitTransaction();

                            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(appContext.CurrentDocument));
                            break;
                        default:
                            throw new NotSupportedException("ActionDocumentTotals.ExecuteCore()");
                    }
                }
            }
        }


    }
}
