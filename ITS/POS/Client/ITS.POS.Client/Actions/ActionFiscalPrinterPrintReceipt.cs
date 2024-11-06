using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// For EAFDSS only. Prints the current document to the Fiscal Printer
    /// </summary>
    public class ActionFiscalPrinterPrintReceipt : Action
    {

        public ActionFiscalPrinterPrintReceipt(IPosKernel kernel)
            : base(kernel)
        {

        }
        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT; }
        }
        public override eActions ActionCode
        {
            get { return eActions.FISCAL_PRINTER_PRINT_RECEIPT; }
        }
        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();


            if (!deviceManager.HasDemoModeBeenSetupCorrectly(config.DemoMode, config.FiscalMethod, config.FiscalDevice))
            {
                throw new Exception(POSClientResources.DEMO_MODE_ERROR);
            }

            ActionFiscalPrinterPrintReceiptParams castedParams = parameters as ActionFiscalPrinterPrintReceiptParams;
            FiscalPrinter printer = castedParams.Printer;
            bool printAsCanceled = castedParams.PrintAsCanceled;
            bool skipPrint = castedParams.SkipPrint;

            if (printer == null)
            {
                throw new Exception(POSClientResources.ERROR_PRINT_RECEIPT + ". " + POSClientResources.PRINTER_NULL);
            }

            printer.ReadDeviceStatus();

            double change = 0;
            double vatAGrossTotal = 0;
            double vatBGrossTotal = 0;
            double vatCGrossTotal = 0;
            double vatDGrossTotal = 0;
            double vatEGrossTotal = 0;
            int receiptNumber = 0;
            double grossTotal = 0;
            bool fiscalPrint = false;
            printer.GetTransactionInfo(out vatAGrossTotal, out vatBGrossTotal, out vatCGrossTotal, out vatDGrossTotal, out vatEGrossTotal, out receiptNumber, out grossTotal);

            ///Called From Action CancelDocument or Action ForceCancelDocument
            if (printAsCanceled)
            {
                /// From Action ForceCancelDocument dont print canceled receipt if printer is not in transaction
                if (skipPrint)
                {
                    try
                    {
                        if (!printer.FiscalStatus.TransactionInPayment)
                        {
                            printer.CancelLegalReceipt();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error(ex, "Error trying to cancel existing receipt");
                    }
                }

                try
                {
                    /// Check if there is a stacked receipt in the printer from previous actions and cancel it
                    bool shouldReturn = false;
                    if (grossTotal > 0 || printer.FiscalStatus.TransactionOpen)
                    {
                        if (receiptNumber > 0)
                        {
                            DocumentHeader headerTocancel = GetExistingDocumentFromDatabase(config, sessionManager, receiptNumber, grossTotal, appContext.CurrentDailyTotals.Oid);
                            if (headerTocancel != null)
                            {
                                shouldReturn = headerTocancel.Oid == appContext.CurrentDocument.Oid;
                                bool isFiscalPrinterHandled = headerTocancel.DocumentType == config.DefaultDocumentTypeOid;
                                documentService.CancelDocument(headerTocancel, appContext.FiscalDate, receiptNumber, isFiscalPrinterHandled);
                            }
                        }
                        printer.CancelLegalReceipt();
                        printer.ReadDeviceStatus();
                    }
                    if (shouldReturn)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to cancel existing receipt");
                }

                ///Already existing code cancels the receipt in case of negative totals
                IEnumerable<DocumentVatInfo> vatGrossTotals = documentService.GetDocumentVatAnalysis(appContext.CurrentDocument);
                var negativeVatCategoryTotals = vatGrossTotals.Where(x => x.GrossTotal < 0);

                if (negativeVatCategoryTotals.Count() > 0 || appContext.CurrentDocument.GrossTotal < 0 || appContext.CurrentDocument.DocumentDetails.Count == 0)
                {
                    CancelReceiptIfTransactionOpen(printer, appContext.CurrentDocument, documentService, appContext.FiscalDate);
                    return;
                }
            }

            DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
            if (documentType.TakesDigitalSignature)
            {
                if (appContext.CurrentDocument.DocumentPayments.Count > 0)
                {
                    decimal maxPaymentAmount = appContext.CurrentDocument.DocumentPayments.Select(x => x.Amount).Max();
                    if (maxPaymentAmount > (decimal)printer.MaximumPaymentAmount)
                    {
                        throw new POSException(Resources.POSClientResources.INVALID_AMOUNT + ": " + maxPaymentAmount);
                    }
                }
                try
                {
                    /// Check if there is a stacked receipt in the printer from previous actions and cancel it
                    if (grossTotal > 0 || printer.FiscalStatus.TransactionOpen)
                    {
                        if (receiptNumber > 0)
                        {
                            DocumentHeader headerTocancel = GetExistingDocumentFromDatabase(config, sessionManager, receiptNumber, grossTotal, appContext.CurrentDailyTotals.Oid);
                            if (headerTocancel != null)
                            {
                                bool isFiscalPrinterHandled = sessionManager.GetObjectByKey<DocumentType>(headerTocancel.DocumentType)?.TakesDigitalSignature ?? false;
                                documentService.CancelDocument(headerTocancel, appContext.FiscalDate, receiptNumber, isFiscalPrinterHandled);
                            }
                        }
                        printer.CancelLegalReceipt();
                        printer.ReadDeviceStatus();
                    }
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "ActionFiscalPrinterPrintReceipt, Error trying to cancel existing receipt");
                }

                if (!printer.FiscalStatus.TransactionOpen)
                {
                    printer.OpenLegalReceipt();
                }

                ///Print lines
                foreach (DocumentDetail detail in appContext.CurrentDocument.DocumentDetails.Where(x => x.IsReturn == false && x.IsCanceled == false
                                                                                          && x.IsTax == false && x.DoesNotAllowDiscount == false))

                {

                    string DetailPrintDescription = GetPrintDescription(detail, config);
                    if (detail.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.NONE)
                    {
                        throw new Exception(POSClientResources.MINISTY_CATEGORY_CODE_NOT_DEFINED_FOR_VAT_CATEGORY + " '" + detail.ItemVatCategoryDescription + "'");
                    }

                    MeasurementUnit mu = sessionManager.GetObjectByKey<MeasurementUnit>(detail.MeasurementUnit);
                    bool supportsDecimal = mu == null ? false : mu.SupportDecimal;
                    double vatFactor = (double)Math.Round(detail.VatFactor, 3, MidpointRounding.AwayFromZero);
                    if (detail.IsCanceled)
                    {
                        printer.SellItemAndCancelIt(DetailPrintDescription, "", "",
                                                                                (double)Math.Round(detail.Qty, 3, MidpointRounding.AwayFromZero),
                                                                                (double)Math.Round(detail.FinalUnitPrice, 2, MidpointRounding.AwayFromZero),
                                                                                (double)Math.Round(detail.GrossTotalBeforeDiscount, 2, MidpointRounding.AwayFromZero),
                                                                                detail.ItemVatCategoryMinistryCode, vatFactor, supportsDecimal);
                    }
                    else
                    {
                        printer.SellItem(DetailPrintDescription, "", "",
                                                                                (double)Math.Round(detail.Qty, 3, MidpointRounding.AwayFromZero),
                                                                                (double)Math.Round(detail.FinalUnitPrice, 2, MidpointRounding.AwayFromZero),
                                                                                (double)Math.Round(detail.GrossTotalBeforeDiscount, 2, MidpointRounding.AwayFromZero),
                                                                                detail.ItemVatCategoryMinistryCode, vatFactor, supportsDecimal);

                        decimal discountWithoutHeader = Math.Round(detail.TotalNonDocumentDiscount, 2, MidpointRounding.AwayFromZero);
                        if (discountWithoutHeader > 0)
                        {
                            printer.AddLineDiscount(detail.ItemVatCategoryMinistryCode, "ΕΚΠΤΩΣΗ", "", (double)discountWithoutHeader);
                        }
                    }
                    if (!fiscalPrint && printer.FiscalStatus.TransactionOpen)
                    {
                        SetDocumentToFiscalPrinterHanledState(printer, appContext.CurrentDocument, sessionManager, out fiscalPrint, out receiptNumber);
                    }
                }
                foreach (DocumentDetail detail in appContext.CurrentDocument.DocumentDetails.Where(x => x.IsReturn == true))  //returns
                {
                    double vatFactor = (double)Math.Round(detail.VatFactor, 3, MidpointRounding.AwayFromZero);
                    string DetailPrintDescription = GetPrintDescription(detail, config);

                    if (detail.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.NONE)
                    {
                        throw new Exception(POSClientResources.MINISTY_CATEGORY_CODE_NOT_DEFINED_FOR_VAT_CATEGORY + " '" + detail.ItemVatCategoryDescription + "'");
                    }

                    if (detail.IsCanceled)
                    {
                        printer.ReturnItemAndCancelIt(DetailPrintDescription, "", "", (double)Math.Round(detail.Qty, 3, MidpointRounding.AwayFromZero) * (-1), (double)Math.Round(detail.FinalUnitPrice, 2, MidpointRounding.AwayFromZero), (double)Math.Round(detail.GrossTotalBeforeDiscount, 2, MidpointRounding.AwayFromZero), detail.ItemVatCategoryMinistryCode, vatFactor);
                    }
                    else
                    {
                        decimal discountWithoutHeader = Math.Abs(Math.Round(detail.TotalNonDocumentDiscount, 2, MidpointRounding.AwayFromZero));
                        double returnFinalUnitPrice = (double)(Math.Round(detail.FinalUnitPrice, 2, MidpointRounding.AwayFromZero) - discountWithoutHeader);
                        printer.ReturnItem(DetailPrintDescription, "", "", (double)Math.Round(detail.Qty, 3, MidpointRounding.AwayFromZero) * (-1), returnFinalUnitPrice, (double)Math.Round(detail.GrossTotalBeforeDiscount, 2, MidpointRounding.AwayFromZero) * (-1), detail.ItemVatCategoryMinistryCode, vatFactor);
                    }
                }
                if (!fiscalPrint && printer.FiscalStatus.TransactionOpen)
                {
                    SetDocumentToFiscalPrinterHanledState(printer, appContext.CurrentDocument, sessionManager, out fiscalPrint, out receiptNumber);
                }

                if (appContext.CurrentDocument.AllDocumentHeaderDiscounts > 0)
                {
                    printer.ReceiptSubtotal("");
                    decimal vatADiscount, vatBDiscount, vatCDiscount, vatDDiscount, vatEDiscount, totalDiscount;
                    documentService.GetDocumentDiscountPerVatCategory(appContext.CurrentDocument, out vatADiscount, out vatBDiscount, out vatCDiscount, out vatDDiscount, out vatEDiscount);
                    totalDiscount = vatADiscount + vatBDiscount + vatCDiscount + vatDDiscount + vatEDiscount;
                    printer.AddSubtotalDiscount("ΕΚΠΤΩΣΗ ΣΥΝΟΛΟΥ", "",
                                                                    (double)Math.Round(vatADiscount, 2, MidpointRounding.AwayFromZero),
                                                                    (double)Math.Round(vatBDiscount, 2, MidpointRounding.AwayFromZero),
                                                                    (double)Math.Round(vatCDiscount, 2, MidpointRounding.AwayFromZero),
                                                                    (double)Math.Round(vatDDiscount, 2, MidpointRounding.AwayFromZero),
                                                                    (double)Math.Round(vatEDiscount, 2, MidpointRounding.AwayFromZero),
                                                                    (double)Math.Round(totalDiscount, 2, MidpointRounding.AwayFromZero));
                }

                if (appContext.CurrentDocument.DocumentDetails.Where(x => x.IsTax == true || x.DoesNotAllowDiscount == true).Count() > 0)
                {
                    //printer.ReceiptSubtotal("---------Είδη Χωρίς Έκπτωση---------");
                    foreach (DocumentDetail detail in appContext.CurrentDocument.DocumentDetails.Where(x => (x.IsCanceled == false && x.IsReturn == false) &&
                                                                                           (x.DoesNotAllowDiscount == true || x.IsTax == true)))
                    {
                        double vatFactor = (double)Math.Round(detail.VatFactor, 3, MidpointRounding.AwayFromZero);
                        string DetailPrintDescription = GetPrintDescription(detail, config);
                        MeasurementUnit mu = sessionManager.GetObjectByKey<MeasurementUnit>(detail.MeasurementUnit);
                        bool supportsDecimal = mu == null ? false : mu.SupportDecimal;

                        printer.SellItem(DetailPrintDescription, "", "",
                                                                        (double)Math.Round(detail.Qty, 3, MidpointRounding.AwayFromZero),
                                                                        (double)Math.Round(detail.FinalUnitPrice, 2, MidpointRounding.AwayFromZero),
                                                                        (double)Math.Round(detail.GrossTotalBeforeDiscount, 2, MidpointRounding.AwayFromZero),
                                                                        detail.ItemVatCategoryMinistryCode, vatFactor, supportsDecimal);
                    }
                    if (!fiscalPrint && printer.FiscalStatus.TransactionOpen)
                    {
                        SetDocumentToFiscalPrinterHanledState(printer, appContext.CurrentDocument, sessionManager, out fiscalPrint, out receiptNumber);
                    }
                }

                DeviceResult info = printer.GetTransactionInfo(out vatAGrossTotal, out vatBGrossTotal, out vatCGrossTotal, out vatDGrossTotal, out vatEGrossTotal, out receiptNumber, out grossTotal);
                if (info != DeviceResult.SUCCESS)
                {
                    throw new POSUserVisibleException(POSClientResources.ERROR_DURING_RECEIPT + " " + POSClientResources.FISCAL_PRINTER_ERROR);
                }
                try
                {
                    DocumentHeader header = appContext.CurrentDocument;
                    documentService.FixFiscalPrinterDeviationsFromDiscounts((decimal)vatAGrossTotal, eMinistryVatCategoryCode.A, header);
                    documentService.FixFiscalPrinterDeviationsFromDiscounts((decimal)vatBGrossTotal, eMinistryVatCategoryCode.B, header);
                    documentService.FixFiscalPrinterDeviationsFromDiscounts((decimal)vatCGrossTotal, eMinistryVatCategoryCode.C, header);
                    documentService.FixFiscalPrinterDeviationsFromDiscounts((decimal)vatDGrossTotal, eMinistryVatCategoryCode.D, header);
                    documentService.FixFiscalPrinterDeviationsFromDiscounts((decimal)vatEGrossTotal, eMinistryVatCategoryCode.E, header);
                    documentService.RecalculateDocumentCosts(header, false);
                }
                catch (Exception ex)
                {
                    string message = String.Format("Error trying to fix fiscal printer deviations. DocumentHeader {0} ", appContext.CurrentDocument.Oid);
                    Kernel.LogFile.Error(ex, message);
                }

                if (!printAsCanceled)
                {
                    decimal discountDeviation = appContext.CurrentDocument.DocumentDetails.SelectMany(x => x.DocumentDetailDiscounts).Sum(x => x.DiscountDeviation);
                    if (Math.Abs(discountDeviation) > 0)
                    {
                        Kernel.LogFile.Info("ActionFiscalPrinterPrintReceipt, Total Discounts Deviation : " + discountDeviation + " Document No : " + appContext.CurrentDocument.DocumentNumber);
                        FixPaymentDifferenceFromDiscountDeviation(appContext.CurrentDocument);
                        decimal payTotal = appContext.CurrentDocument.DocumentPayments.Sum(x => x.Amount);
                        decimal paymentDifference = appContext.CurrentDocument.GrossTotal - payTotal;
                        if (paymentDifference > 0)
                        {
                            Kernel.LogFile.Info("ActionFiscalPrinterPrintReceipt, Payment Difference : " + paymentDifference + " Document No : " + appContext.CurrentDocument.DocumentNumber);
                            CancelFiscalReceipt(appContext.CurrentDocument, printer, formManager, config, documentService, deviceManager, actionManager, totalizersService, appContext, sessionManager);
                            return;
                        }
                    }
                }


                if ((decimal)grossTotal != appContext.CurrentDocument.GrossTotal && printAsCanceled == false)
                {
                    throw new POSException(String.Format(POSClientResources.GROSS_TOTAL_MISSMATCH, grossTotal, appContext.CurrentDocument.GrossTotal));
                }

                if (printAsCanceled == false)
                {
                    printer.ReceiptPaymentMode(grossTotal);
                }

                if (printAsCanceled)
                {
                    CancelReceiptIfTransactionOpen(printer, appContext.CurrentDocument, documentService, appContext.FiscalDate);
                }
                else
                {
                    //Print Payments
                    bool opendrawer = false;
                    if (appContext.CurrentDocument.DocumentPayments.Count > 0)
                    {
                        Dictionary<PaymentMethod, double> paymentMethodsAmounts = new Dictionary<PaymentMethod, double>();

                        foreach (DocumentPayment documentPayment in appContext.CurrentDocument.DocumentPayments.Where(x => x.Amount >= 0))
                        {
                            if (documentPayment.Amount > 0)
                            {
                                PaymentMethod paymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(documentPayment.PaymentMethod);

                                if (paymentMethod == null)
                                {
                                    throw new POSException("Payment Method Not found. Oid:" + documentPayment.PaymentMethod);
                                }

                                if (paymentMethod.PaymentMethodType == ePaymentMethodType.UNDEFINED)
                                {
                                    throw new POSException("Payment Method Type is Undefined. Payment Method Code:" + documentPayment.PaymentMethodCode + ", Description:" +
                                    documentPayment.PaymentMethodDescription + " ,Oid:" + documentPayment.PaymentMethod);
                                }

                                if (paymentMethodsAmounts.ContainsKey(paymentMethod))
                                {
                                    paymentMethodsAmounts[paymentMethod] = paymentMethodsAmounts[paymentMethod] + (double)documentPayment.Amount;
                                }
                                else
                                {
                                    paymentMethodsAmounts.Add(paymentMethod, (double)documentPayment.Amount);
                                }
                            }
                        }

                        foreach (KeyValuePair<PaymentMethod, double> pair in paymentMethodsAmounts)
                        {
                            if (pair.Key.PaymentMethodType == ePaymentMethodType.UNDEFINED)
                            {
                                continue;
                            }
                            printer.AddPayment(pair.Value, pair.Key.PaymentMethodType, out change, pair.Key.Description.ToUpperGR(), " ");
                            if (pair.Key.OpensDrawer)
                            {
                                opendrawer = true;
                            }
                        }
                    }
                    else
                    {
                        PaymentMethod defaultPaymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(config.DefaultPaymentMethodOid);
                        printer.AddPayment(0, defaultPaymentMethod.PaymentMethodType, out change, defaultPaymentMethod.Description.ToUpperGR(), " ");
                        if (defaultPaymentMethod.OpensDrawer)
                        {
                            opendrawer = true;
                        }
                    }

                    DeviceResult result = printer.CloseLegalReceipt(opendrawer);
                    appContext.CurrentDocument.Signature = result.ToString();
                    appContext.CurrentDocument.FinalizedDate = DateTime.Now;
                    if (result != DeviceResult.SUCCESS)
                    {
                        throw new POSUserVisibleException(POSClientResources.ERROR_DURING_RECEIPT + " " + POSClientResources.FISCAL_PRINTER_ERROR);
                    }
                    documentService.AssignDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid, receiptNumber);
                    appContext.CurrentDocument.IsFiscalPrinterHandled = true;
                    fiscalPrint = true;
                }
            }
            else //NON-Fiscal Document Type Print
            {
                try
                {

                    double proformaVatAGrossTotal = 0;
                    double proformaVatBGrossTotal = 0;
                    double proformaVatCGrossTotal = 0;
                    double proformaVatDGrossTotal = 0;
                    double proformaVatEGrossTotal = 0;
                    double proformaGrossTotal = 0;
                    int proformaFiscalNo = 0;
                    printer.GetTransactionInfo(out proformaVatAGrossTotal, out proformaVatBGrossTotal, out proformaVatCGrossTotal, out proformaVatDGrossTotal, out proformaVatEGrossTotal, out proformaFiscalNo, out proformaGrossTotal);

                    /// Check if there is a stacked receipt in the printer from previous actions and cancel it
                    if (printer.FiscalStatus.TransactionOpen || proformaGrossTotal > 0)
                    {
                        if (proformaFiscalNo > 0)
                        {
                            DocumentHeader headerTocancel = GetExistingDocumentFromDatabase(config, sessionManager, proformaFiscalNo, proformaGrossTotal, appContext.CurrentDailyTotals.Oid);
                            if (headerTocancel != null)
                            {
                                bool isFiscalPrinterHandled = sessionManager.GetObjectByKey<DocumentType>(headerTocancel.DocumentType)?.TakesDigitalSignature ?? false;
                                documentService.CancelDocument(headerTocancel, appContext.FiscalDate, receiptNumber, isFiscalPrinterHandled);
                            }
                        }
                        printer.CancelLegalReceipt();
                        printer.ReadDeviceStatus();
                    }
                    printer.CancelLegalReceipt();
                }
                catch (Exception ex)
                {
                    string message = String.Format("Error trying to cancel existing receipt");
                    Kernel.LogFile.Error(ex, message);
                }

                if (printAsCanceled == false)
                {
                    List<string> allTheLines = new List<string>();
                    Receipt.Receipt receipt = new Receipt.Receipt();
                    Receipt.ReceiptSchema receiptSchema = config.GetReceiptSchema(appContext.CurrentDocument);
                    receipt.Header = receiptBuilder.CreateReceiptHeader(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);
                    receipt.Body = receiptBuilder.CreateReceiptBody(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);
                    receipt.Footer = receiptBuilder.CreateReceiptFooter(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);
                    allTheLines = receipt.GetReceiptLines();

                    FiscalLine[] fiscalLines = allTheLines.Select(x => new FiscalLine() { Type = ePrintType.NORMAL, Value = x }).ToArray();
                    (printer as FiscalPrinter).PrintIllegal(fiscalLines);
                }
            }

            if (printAsCanceled == false)
            {
                if (appContext.CurrentDocument.Change != 0)
                {
                    DocumentPayment newPayment = new DocumentPayment(sessionManager.GetSession<DocumentPayment>());
                    newPayment.DocumentHeader = appContext.CurrentDocument;
                    newPayment.DocumentHeaderOid = appContext.CurrentDocument.Oid;
                    newPayment.Amount = appContext.CurrentDocument.Change * (-1);
                    newPayment.PaymentMethod = config.DefaultPaymentMethodOid;
                    PaymentMethod defaultPaymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(config.DefaultPaymentMethodOid);
                    newPayment.PaymentMethodDescription = defaultPaymentMethod.Description;
                    newPayment.PaymentMethodType = defaultPaymentMethod.PaymentMethodType;
                    newPayment.IncreasesDrawerAmount = defaultPaymentMethod.IncreasesDrawerAmount;
                    sessionManager.FillDenormalizedFields(newPayment);
                }

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

                if (appContext.CurrentDocument.IsAddedToTotals == false && documentType.JoinInTotalizers == true)
                {
                    totalizersService.UpdateTotalizers(config, appContext.CurrentDocument, appContext.CurrentUser, appContext.CurrentDailyTotals,
                        appContext.CurrentUserDailyTotals, config.CurrentStoreOid, config.CurrentTerminalOid,
                        increaseVatAndSales, printer.IsTotalVatCalculationPerReceipt, eTotalizorAction.INCREASE, fiscalPrint,
                        (decimal)vatAGrossTotal, (decimal)vatBGrossTotal, (decimal)vatCGrossTotal, (decimal)vatDGrossTotal, (decimal)vatEGrossTotal, (decimal)grossTotal);
                }
            }
        }


        private void CancelFiscalReceipt(DocumentHeader header, FiscalPrinter printer, IFormManager formManager, IConfigurationManager config,
                                             IDocumentService documentService, IDeviceManager deviceManager, IActionManager actionManager,
                                                                            ITotalizersService totalizersService, IAppContext appContext, ISessionManager sessionManager)
        {
            var res = formManager.ShowMessageBox(POSClientResources.WRONG_PAYMENT_VALUE + " " + POSClientResources.CANCEL_RECEIPT, System.Windows.Forms.MessageBoxButtons.OK);
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    header.TransactionCoupons.ToList().ForEach(transactionCoupon => transactionCoupon.IsCanceled = true);
                    CancelReceiptIfTransactionOpen(printer, appContext.CurrentDocument, documentService, appContext.FiscalDate);
                    DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);

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
                    if (appContext.CurrentDocument.IsAddedToTotals == false && documentType.JoinInTotalizers == true)
                    {
                        totalizersService.UpdateTotalizers(config, header, appContext.CurrentUser, appContext.CurrentDailyTotals, appContext.CurrentUserDailyTotals,
                                                                                                config.CurrentStoreOid, config.CurrentTerminalOid, increaseVatAndSales,
                                                                                                printer.IsTotalVatCalculationPerReceipt, eTotalizorAction.INCREASE);
                    }

                    appContext.CurrentDocument = null;
                    appContext.CurrentDocumentLine = null;
                    appContext.CurrentDocumentPayment = null;
                    appContext.CurrentCustomer = null;
                    appContext.SetMachineStatus(eMachineStatus.SALE);
                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(null, true, false)); //clears the price textboxes
                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(null)); //clears the price textboxes

                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "CancelFiscalReceipt");
                    throw new POSException(POSClientResources.ERROR_DURING_RECEIPT);
                }
            }
        }

        private void CancelReceiptIfTransactionOpen(FiscalPrinter printer, DocumentHeader header, IDocumentService documentService, DateTime fiscalDate)
        {
            if (printer.FiscalStatus.TransactionOpen)
            {
                double cancelVatAGrossTotal = 0;
                double cancelVatBGrossTotal = 0;
                double cancelVatCGrossTotal = 0;
                double cancelVatDGrossTotal = 0;
                double cancelVatEGrossTotal = 0;
                double cancelTotal = 0;
                int fiscalNo = 0;
                printer.GetTransactionInfo(out cancelVatAGrossTotal, out cancelVatBGrossTotal, out cancelVatCGrossTotal, out cancelVatDGrossTotal, out cancelVatEGrossTotal, out fiscalNo, out cancelTotal);
                documentService.CancelDocument(header, fiscalDate, fiscalNo, true);
            }
            printer.CancelLegalReceipt();
        }



        private int GetCurrentFiscalNumber(FiscalPrinter printer)
        {
            int fiscalNo = -1;
            try
            {
                double cancelVatAGrossTotal = 0;
                double cancelVatBGrossTotal = 0;
                double cancelVatCGrossTotal = 0;
                double cancelVatDGrossTotal = 0;
                double cancelVatEGrossTotal = 0;
                double cancelTotal = 0;
                printer.GetTransactionInfo(out cancelVatAGrossTotal, out cancelVatBGrossTotal, out cancelVatCGrossTotal, out cancelVatDGrossTotal, out cancelVatEGrossTotal, out fiscalNo, out cancelTotal);
            }
            catch { }
            return fiscalNo;
        }






        private void FixPaymentDifferenceFromDiscountDeviation(DocumentHeader header)
        {
            try
            {
                decimal discountDeviation = header.DocumentDetails.SelectMany(x => x.DocumentDetailDiscounts).Sum(x => x.DiscountDeviation);
                decimal payTotal = header.DocumentPayments.Sum(x => x.Amount);
                decimal paymentDifference = header.GrossTotal - payTotal;
                if (paymentDifference > 0 && paymentDifference <= Math.Abs(discountDeviation))
                {
                    var payment = header.DocumentPayments.Where(x => x.IncreasesDrawerAmount && x.CanExceedTotal).FirstOrDefault()
                                                                                                ?? header.DocumentPayments.Where(x => x.Amount > 0).FirstOrDefault();
                    if (payment != null)
                    {
                        payment.Amount += paymentDifference;
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "FixPaymentDifferenceFromDiscountDeviation");
            }
        }


        private DocumentHeader GetExistingDocumentFromDatabase(IConfigurationManager config, ISessionManager sessionManager, int docNumber, double grossTotal, Guid currentDailyTotalsOid)
        {
            decimal gtotal = Math.Round((decimal)grossTotal, 2, MidpointRounding.AwayFromZero);
            CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("DocumentNumber", docNumber, BinaryOperatorType.Equal),
                                                                            new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                                                                            new BinaryOperator("GrossTotal", gtotal, BinaryOperatorType.Equal),
                                                                            new BinaryOperator("UserDailyTotals.DailyTotals.Oid", currentDailyTotalsOid));
            DocumentHeader headerTocancel = sessionManager.FindObject<DocumentHeader>(crit);
            if (headerTocancel == null)
            {
                crit = CriteriaOperator.And(new BinaryOperator("DocumentType", config.ProFormaInvoiceDocumentTypeOid),
                                                    new BinaryOperator("GrossTotal", grossTotal, BinaryOperatorType.Equal),
                                                            new BinaryOperator("UserDailyTotals.DailyTotals.Oid", currentDailyTotalsOid));
                headerTocancel = sessionManager.FindObject<DocumentHeader>(crit);
            }

            return headerTocancel;
        }


        private void SetDocumentToFiscalPrinterHanledState(FiscalPrinter printer, DocumentHeader header, ISessionManager sessionManager, out bool fiscalPrint, out int fiscalNumber)
        {
            fiscalPrint = false;
            fiscalNumber = 0;
            try
            {
                int fiscalNo = GetCurrentFiscalNumber(printer);
                if (fiscalNo != -1)
                {
                    header.DocumentNumber = header.FiscalPrinterNumber = fiscalNumber = fiscalNo;
                    header.IsFiscalPrinterHandled = fiscalPrint = true;
                    header.Save();
                    sessionManager.CommitTransactionsChanges();
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex);
            }
        }

        private string GetPrintDescription(DocumentDetail detail, IConfigurationManager config)
        {
            string DetailPrintDescription = detail.ItemName;
            switch (config.DocumentDetailPrintDescription)
            {
                case eDocumentDetailPrintDescription.CustomDescription:
                    DetailPrintDescription = detail.CustomDescription;
                    break;
                case eDocumentDetailPrintDescription.ItemExtraInfoName:
                    DetailPrintDescription = detail.ItemExtraInfoDescription;
                    break;
                default:
                    DetailPrintDescription = detail.ItemName;
                    break;
            }
            return DetailPrintDescription;
        }
    }
}
