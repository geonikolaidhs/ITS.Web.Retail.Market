using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Reports;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Finallizes the current document by handling all the logic that must be executed when the document is done.
    /// For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionCloseDocument : Action
    {
        public ActionCloseDocument(IPosKernel kernel)
            : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.CLOSE_DOCUMENT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ActionCloseDocumentParams castedParams = (ActionCloseDocumentParams)parameters;
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            IPromotionService promotionService = Kernel.GetModule<IPromotionService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();

            if (totalizersService.CheckIfMustIssueZ(appContext.CurrentDailyTotals) && castedParams.SkipZCheck == false)
            {
                string errorMessage = String.Format(POSClientResources.YOU_MUST_ISSUE_Z, appContext.CurrentDailyTotals.FiscalDate);
                formManager.ShowCancelOnlyMessageBox(errorMessage);
                throw new POSException(errorMessage);
            }

            if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                Device device = deviceManager.GetEAFDSSDevice(config.FiscalDevice);
                if (device != null && device.ConType == ConnectionType.EMULATED && appContext.CurrentDocument != null)
                {
                    appContext.CurrentDocument.InEmulationMode = true;
                    appContext.CurrentUserDailyTotals.InEmulationMode = true;
                    appContext.CurrentDailyTotals.InEmulationMode = true;
                }
            }

            Device primaryPrinter = null;
            bool printToWindowsPrinter = false;
            bool printToOposPrinter = false;

            POSDocumentReportSettings pdrs = documentService.GetReportForDocumentType(appContext.CurrentDocument.DocumentType, config);
            if (pdrs != null)
            {
                if (pdrs.CustomReportOid != null && pdrs.CustomReportOid != Guid.Empty)
                {
                    printToWindowsPrinter = true;
                    printToOposPrinter = false;
                }
                else if (pdrs.XMLPrintFormat != null)
                {
                    printToWindowsPrinter = false;
                    printToOposPrinter = true;
                }
            }

            if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<Printer>();
            }

            else if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
            }

            bool openDrawer = documentService.CheckIfShouldOpenDrawer(appContext.CurrentDocument);
            try
            {
                if (openDrawer && castedParams.SkipPrint == false)
                {
                    actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
                }
            }
            catch (Exception e) //silent try catch, document has printed but drawer failed. It should not retry
            {
                Kernel.LogFile.Info(e, "ActionPrintReceipt:OpenDrawer");
            }

            if (appContext.CurrentDocument != null)
            {
                appContext.CurrentDocument.FiscalDate = appContext.FiscalDate;
                appContext.CurrentDocument.FinalizedDate = DateTime.Now;
                appContext.CurrentDocument.Session.CommitTransaction();
                DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
                List<string> receiptLines = new List<string>();
                Printer printer = primaryPrinter as Printer;

                if (castedParams.SkipPrint == false)
                {
                    #region
                    /// <summary>
                    /// Signs and Print the document to StoreController through Webservice
                    /// </summary>
                    if (documentType.IsPrintedOnStoreController)
                    {
                        if (string.IsNullOrWhiteSpace(appContext.CurrentDocument.Signature))
                        {
                            appContext.CurrentDocument.FinalizedDate = DateTime.Now;
                        }
                        using (WebService.POSUpdateService webService = new WebService.POSUpdateService())
                        {
                            webService.Url = config.StoreControllerWebServiceURL;
                            string errorMessage = "";
                            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                            Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;
                            string documentToSend = appContext.CurrentDocument.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

                            string remoteDocument = webService.DirectPostNewDocument(documentToSend, config.CurrentTerminalOid, WebService.eIdentifier.POS, appContext.CurrentUser.Oid, out errorMessage);
                            if (null == remoteDocument)
                            {
                                formManager.ShowCancelOnlyMessageBox("Document Post failed." + Environment.NewLine + errorMessage);
                            }
                            else if (String.IsNullOrWhiteSpace(errorMessage) == false)
                            {
                                Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;
                                appContext.CurrentDocument.FromJson(remoteDocument, PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                                SaveSignedDocument(appContext);
                                formManager.ShowCancelOnlyMessageBox(String.Format("{0}: {1}", POSClientResources.DOCUMENT_SUCCESFULLY_SAVED_BUT_NOT_PRINTED, errorMessage));
                            }
                            else
                            {
                                Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;
                                appContext.CurrentDocument.FromJson(remoteDocument, PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                                SaveSignedDocument(appContext);
                                formManager.ShowMessageBox(POSClientResources.DOCUMENT_SUCCESFULLY_SAVED_AND_PRINTED);
                            }
                        }
                    }
                    #endregion

                    #region
                    /// <summary>
                    /// Signs and Print the document to directly to windows printer
                    /// v2.3.7.3 patch(10)
                    /// </summary>
                    else if (printToWindowsPrinter == true)
                    {
                        if (appContext.CurrentDocument.IsOpen == true)
                        {
                            appContext.CurrentDocument.DocumentNumber = 0;
                            appContext.CurrentDocument.DocumentNumber = documentService.GetNextDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid);
                            if (appContext.CurrentDocument.DocumentNumber == -1)
                            {
                                throw new POSException(POSClientResources.DOCUMENT_SEQUENCE_NOT_FOUND_FOR_SERIES);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(appContext.CurrentDocument.Signature))
                        {
                            appContext.CurrentDocument.FinalizedDate = DateTime.Now;
                        }

                        Receipt.Receipt receipt = new Receipt.Receipt();
                        Receipt.ReceiptSchema receiptSchema = config.GetReceiptSchema(appContext.CurrentDocument);


                        if (printer.ConType != ConnectionType.NONE)
                        {
                            Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: Start GetReceiptLines");
                            receiptLines = CreateReceiptLines(receiptSchema, receipt, receiptBuilder, appContext, printer);
                            Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End GetReceiptLines");

                            if (documentType.TakesDigitalSignature && string.IsNullOrWhiteSpace(appContext.CurrentDocument.Signature))
                            {
                                appContext.CurrentDocument.Signature = SignDocumentFromEAFDDSSDevice(config, sessionManager, appContext, documentService, receiptLines, actionManager);
                            }

                            if (appContext.CurrentDocument.InEmulationMode)
                            {
                                receiptLines.Add(Environment.NewLine + Environment.NewLine + receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, appContext.CurrentDocument.Signature).FirstOrDefault()
                                               + Environment.NewLine);
                            }
                            else
                            {
                                receiptLines.Add(Environment.NewLine + Environment.NewLine + appContext.CurrentDocument.Signature + Environment.NewLine);
                            }
                        }

                    }
                    #endregion

                    #region
                    /// <summary>
                    /// Signs the document from EAFDSS device and print to POS printer
                    /// v2.3.7.3 patch(12)
                    /// </summary>
                    else if (config.FiscalMethod == eFiscalMethod.EAFDSS && printToWindowsPrinter == false && documentType.IsPrintedOnStoreController == false)
                    {

                        if (printToOposPrinter == true)
                        {
                            primaryPrinter = deviceManager.GetDeviceByName<Printer>(pdrs.Printer);
                        }

                        if (primaryPrinter == null)
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }

                        CheckPrinter(primaryPrinter as Printer);

                        if (appContext.CurrentDocument.IsOpen == true)
                        {
                            appContext.CurrentDocument.DocumentNumber = 0;
                            appContext.CurrentDocument.DocumentNumber = documentService.GetNextDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid);
                            if (appContext.CurrentDocument.DocumentNumber == -1)
                            {
                                throw new POSException(POSClientResources.DOCUMENT_SEQUENCE_NOT_FOUND_FOR_SERIES);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(appContext.CurrentDocument.Signature))
                        {
                            appContext.CurrentDocument.FinalizedDate = DateTime.Now;
                        }

                        Receipt.Receipt receipt = new Receipt.Receipt();
                        Receipt.ReceiptSchema receiptSchema = config.GetReceiptSchema(appContext.CurrentDocument);

                        if (printer.ConType != ConnectionType.NONE)
                        {
                            Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: Start GetReceiptLines");
                            receiptLines = CreateReceiptLines(receiptSchema, receipt, receiptBuilder, appContext, printer);
                            Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End GetReceiptLines");

                            if (documentType.TakesDigitalSignature && string.IsNullOrWhiteSpace(appContext.CurrentDocument.Signature))
                            {
                                appContext.CurrentDocument.Signature = SignDocumentFromEAFDDSSDevice(config, sessionManager, appContext, documentService, receiptLines, actionManager);
                            }

                            if (appContext.CurrentDocument.InEmulationMode)
                            {
                                receiptLines.Add(Environment.NewLine + Environment.NewLine + receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, appContext.CurrentDocument.Signature).FirstOrDefault()
                                               + Environment.NewLine);
                            }
                            else
                            {
                                receiptLines.Add(Environment.NewLine + Environment.NewLine + appContext.CurrentDocument.Signature + Environment.NewLine);
                            }
                        }
                    }
                    #endregion

                    #region
                    /// <summary>
                    /// Signs and Print the document to ADHME device
                    /// </summary>
                    else if (config.FiscalMethod == eFiscalMethod.ADHME)
                    {
                        if (primaryPrinter == null)
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }
                        actionManager.GetAction(eActions.FISCAL_PRINTER_PRINT_RECEIPT).Execute(new ActionFiscalPrinterPrintReceiptParams(primaryPrinter as FiscalPrinter, true, false, castedParams.SkipPrint), true);
                    }
                    #endregion
                    else
                    {
                        throw new POSException("Unknown Fiscal Method");
                    }
                    ///appcontext may be cleared on fiscal printer error
                    if (appContext.CurrentDocument == null)
                    {
                        return;
                    }
                    ExecuteRatificationOnPayments(appContext.CurrentDocument, sessionManager, deviceManager, formManager, config.FiscalMethod, config.TerminalID, actionManager);
                }



                #region

                /// <summary>
                /// Document marked as closed
                /// Update and Save Totalizers
                /// Assign and save Sequences
                /// Upload Transactions Coupons on Store Controller
                /// Save Document
                /// Save Customer Points
                /// </summary>
                if (appContext.CurrentDocument.IsOpen)
                {
                    try
                    {
                        appContext.CurrentDocument.IsOpen = false;

                        CalculateTotals(appContext, documentType, documentService, config, sessionManager, totalizersService);

                        UploadTransactionCoupons(appContext, documentType, sessionManager, config.StoreControllerWebServiceURL);

                        SaveSignedDocument(appContext);

                        SaveCustomerPoints(appContext, sessionManager, config);
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error(DateTime.Now + " Fail to save document with error : " + ex.GetFullMessage());
                        throw;
                    }
                }
                #endregion

                #region
                /// <summary>
                /// Prints the Legal Receipt Signed from EAFDSS device
                /// </summary>
                if (config.FiscalMethod == eFiscalMethod.EAFDSS && printToWindowsPrinter == false && documentType.IsPrintedOnStoreController == false && castedParams.SkipPrint == false)
                {
                    DialogResult dialogResult = DialogResult.None;
                    bool feedAndPaperCut = false;
                    do
                    {
                        DeviceResult printResult = DeviceResult.SUCCESS;

                        if (printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                        {
                            if (feedAndPaperCut == false)
                            {
                                receiptLines.Add(Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                                feedAndPaperCut = true;
                            }
                        }
                        try
                        {
                            Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start Send Print");
                            printResult = printer.PrintLines(receiptLines.ToArray());
                            Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End Send Print");
                        }
                        catch (Exception ex)
                        {
                            printer.EndTransaction();
                            Kernel.LogFile.Error("ActionCloseDocument: Fail to print" + ex.GetFullMessage());
                        }

                        if (printResult != DeviceResult.SUCCESS)
                        {
                            printer.EndTransaction();
                            string dialogMessage = string.Empty;
                            string dialogMessageType = string.Empty;
                            if (documentType.TakesDigitalSignature == true)
                            {
                                dialogMessageType = string.Format(POSClientResources.DOCUMENT_SAVED_PRINT_FAIL, appContext.CurrentDocument.DocumentNumber.ToString());
                            }
                            else
                            {
                                dialogMessageType = string.Format(POSClientResources.PROFORMA_SAVED_PRINT_FAIL, appContext.CurrentDocument.DocumentNumber.ToString());
                            }
                            dialogMessage = dialogMessageType
                                 + Environment.NewLine +
                                   POSClientResources.PRESS_ENTER_FOR_REPRINT
                                 + Environment.NewLine +
                                   POSClientResources.PRESS_C_TO_CONTINUE
                                 + Environment.NewLine +
                                   POSClientResources.REPRINT_FROM_RECEPTION;

                            dialogResult = formManager.ShowFailToPrintMessageBox(dialogMessage);
                        }
                        else
                        {
                            break;
                        }
                    } while (dialogResult == DialogResult.OK);

                    Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start EndTransaction");
                    printer.EndTransaction();
                    Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End EndTransaction");
                }
                #endregion

                #region
                /// <summary>
                /// Prints the Illegal Part of Receipt both for EAFDSS and ADHME
                /// </summary>
                if (printToWindowsPrinter == false && documentType.IsPrintedOnStoreController == false && castedParams.SkipPrint == false)
                {
                    PrintLoyalty(appContext, config, primaryPrinter, sessionManager, receiptBuilder);

                    try
                    {
                        promotionService.ExecutePromotionResults(appContext.CurrentDocument, ePromotionResultExecutionPlan.AFTER_DOCUMENT_CLOSED, primaryPrinter, formManager, Kernel.LogFile);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = POSClientResources.ERROR_EXECUTING_PROMOTIONS_RESULT + Environment.NewLine + ex.GetFullMessage();
                        formManager.ShowCancelOnlyMessageBox(errorMessage);
                    }
                }
                #endregion

                /// <summary>
                /// Prints the the document to windows printer
                /// </summary>
                if (config.FiscalMethod == eFiscalMethod.EAFDSS && printToWindowsPrinter == true && documentType.IsPrintedOnStoreController == false && castedParams.SkipPrint == false)
                {
                    DialogResult dialogResult = DialogResult.None;
                    do
                    {
                        DeviceResult printResult = DeviceResult.SUCCESS;
                        try
                        {
                            Kernel.LogFile.Debug(DateTime.Now + " Print Document To Windows Printer: Start Send Print");
                            POSDocumentReportSettings posDocumentReportSettings = config.DocumentReports.Where(documentReport => documentReport.DocumentTypeOid == appContext.CurrentDocument.DocumentType).FirstOrDefault();
                            Guid customReportGuid = posDocumentReportSettings.CustomReportOid;
                            CustomReport customReport = sessionManager.GetObjectByKey<CustomReport>(customReportGuid);
                            Store store = sessionManager.GetObjectByKey<Store>(appContext.CurrentDocument.Store);
                            DocumentHeaderPrinter documentHeaderPrinter = new DocumentHeaderPrinter();
                            ReportPrintResult printToWindowsPrinterResult = documentHeaderPrinter.PrintDocumentHeader(customReport.ReportFile, formManager, sessionManager, appContext.CurrentDocument, posDocumentReportSettings.Printer, store.Owner, appContext.CurrentUser.Oid, config);
                            if (printToWindowsPrinterResult.PrintResult == PrintResult.FAILURE && !String.IsNullOrEmpty(printToWindowsPrinterResult.ErrorMessage))
                            {
                                throw new POSException(printToWindowsPrinterResult.ErrorMessage);
                            }
                            Kernel.LogFile.Debug(DateTime.Now + " Print Document To Windows Printer: End Send Print");
                        }
                        catch (Exception ex)
                        {
                            printResult = DeviceResult.FAILURE;
                            Kernel.LogFile.Error("ActionCloseDocument: Fail to print" + ex.GetFullMessage());
                        }

                        if (printResult != DeviceResult.SUCCESS)
                        {
                            string dialogMessage = string.Empty;
                            string dialogMessageType = string.Empty;
                            if (documentType.TakesDigitalSignature == true)
                            {
                                dialogMessageType = string.Format(POSClientResources.DOCUMENT_SAVED_PRINT_FAIL, appContext.CurrentDocument.DocumentNumber.ToString());
                            }
                            else
                            {
                                dialogMessageType = string.Format(POSClientResources.PROFORMA_SAVED_PRINT_FAIL, appContext.CurrentDocument.DocumentNumber.ToString());
                            }
                            dialogMessage = dialogMessageType
                                 + Environment.NewLine +
                                   POSClientResources.PRESS_ENTER_FOR_REPRINT
                                 + Environment.NewLine +
                                   POSClientResources.PRESS_C_TO_CONTINUE
                                 + Environment.NewLine +
                                   POSClientResources.REPRINT_FROM_RECEPTION;

                            dialogResult = formManager.ShowFailToPrintMessageBox(dialogMessage);
                        }
                        else
                        {
                            break;
                        }
                    } while (dialogResult == DialogResult.OK);

                    Kernel.LogFile.Debug(DateTime.Now + " Print Document To Windows Printer: End EndTransaction");
                }


                appContext.CurrentDocument = null;
                appContext.CurrentDocumentLine = null;
                appContext.CurrentDocumentPayment = null;
                appContext.CurrentCustomer = null;
            }

            appContext.SetMachineStatus(eMachineStatus.SALE, messageDelay: 3000);
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(null));
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(null, true, false));

            CheckWithDraw(config, totalizersService, appContext, actionManager, deviceManager, formManager);
        }

        /// <summary>
        /// Checks the availiability of printer.
        /// </summary>
        private void CheckPrinter(Printer printer)
        {
            DeviceResult beginTranResult = printer.BeginTransaction();

            if (beginTranResult != DeviceResult.SUCCESS)
            {
                string message = Resources.POSClientResources.PRINTER_FAILURE + " : " + beginTranResult.ToLocalizedString();
                printer.EndTransaction();
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Returns the legal lines from document signed from EAFDDSS device.
        /// </summary>
        private List<string> CreateReceiptLines(Receipt.ReceiptSchema receiptSchema, Receipt.Receipt receipt, IReceiptBuilder receiptBuilder, IAppContext appContext, Printer printer)
        {
            List<string> allTheLines = new List<string>();

            receipt.Header = receiptBuilder.CreateReceiptHeader(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);
            if (appContext.CurrentDocument.InEmulationMode && receipt.Header.Count > 0)
            {
                ////Inject illegal warning
                receipt.Header.InsertRange(0, receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                        POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                        POSClientResources.PUNISHABLE_BY_LAW_MESSAGE,
                                                        POSClientResources.IS_NOT_VALID_SALES_DOCUMENT_MESSAGE));
            }

            receipt.Body = receiptBuilder.CreateReceiptBody(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);

            if (appContext.CurrentDocument.InEmulationMode)
            {
                ////Inject illegal warning
                List<string> injectionMessage = receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                                POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                                POSClientResources.PUNISHABLE_BY_LAW_MESSAGE);
                if (receipt.Body.Count > 4)
                {
                    List<string> copyOfBody = receipt.Body.ToList();
                    int injectionCounter = 0;

                    int injectionOffset = injectionMessage.Count;
                    for (int i = 4; i < receipt.Body.Count; i += 5)
                    {
                        copyOfBody.InsertRange(i + (injectionCounter * injectionOffset), injectionMessage);
                        injectionCounter++;
                    }
                    receipt.Body = copyOfBody;
                }
                else
                {
                    receipt.Body.InsertRange(receipt.Body.Count, injectionMessage);
                }
            }

            receipt.Footer = receiptBuilder.CreateReceiptFooter(receiptSchema, appContext.CurrentDocument, printer.Settings.LineChars, printer.ConType);
            if (appContext.CurrentDocument.InEmulationMode)
            {
                ////Inject illegal warning
                receipt.Footer.AddRange(receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false,
                                                        POSClientResources.ILLEGAL_RECEIPT_TRAINING_MODE_MESSAGE,
                                                        POSClientResources.PUNISHABLE_BY_LAW_MESSAGE,
                                                        POSClientResources.IS_NOT_VALID_SALES_DOCUMENT_MESSAGE));
            }
            allTheLines = receipt.GetReceiptLines();
            Kernel.LogFile.Debug(DateTime.Now + "  Print Receipt: End GetReceiptLines");
            return allTheLines;
        }

        /// <summary>
        /// Returns the signature from EAFDDSS device, Empty in case of fail.
        /// </summary>
        private string SignDocumentFromEAFDDSSDevice(IConfigurationManager config, ISessionManager sessionManager, IAppContext appContext, IDocumentService documentService, List<string> allTheLines, IActionManager actionManager)
        {
            string signature = string.Empty;
            if (eFiscalMethod.EAFDSS == config.FiscalMethod)
            {
                Model.Settings.POS currentTerminal = sessionManager.GetSession<Model.Settings.POS>().GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
                if (currentTerminal == null)
                {
                    throw new POSException(Resources.POSClientResources.INVALID_TERMINAL_OID);
                }

                if (currentTerminal.IsStandaloneFiscalOnError)
                {
                    if (String.IsNullOrWhiteSpace(currentTerminal.StandaloneFiscalOnErrorMessage))
                    {
                        throw new POSException(Resources.POSClientResources.STANDALONE_FISCAL_ON_ERROR_MESSAGE_IS_EMPTY);
                    }
                    signature = currentTerminal.StandaloneFiscalOnErrorMessage;
                }
                else
                {
                    string tempFile = Path.GetTempFileName();
                    using (StreamWriter wr = new StreamWriter(tempFile, false, Encoding.GetEncoding(1253)))
                    {
                        foreach (string line in allTheLines)
                        {
                            wr.WriteLine(line);
                        }
                    }

                    Kernel.LogFile.Debug(String.Format("Start get signature of receipt {0} - {1}", appContext.CurrentDocument.DocumentSeriesCode, appContext.CurrentDocument.DocumentNumber));
                    IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();

                    List<Device> fiscalDevices = deviceManager.GetEAFDSSDevicesByPriority(config.FiscalDevice);
                    if (fiscalDevices == null || fiscalDevices.Count == 0)
                    {
                        throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
                    }
                    List<string> exceptionsThrown = new List<string>();
                    foreach (Device fiscalDevice in fiscalDevices)
                    {
                        Kernel.LogFile.Trace("Trying to sign using " + fiscalDevice.DeviceName);
                        try
                        {
                            switch (config.FiscalDevice)
                            {
                                case eFiscalDevice.DATASIGN:
                                    DataSignESD defaultDataSign = fiscalDevice as DataSignESD;
                                    DataSignESD.DataSignResult datasignResult = defaultDataSign.SignDocument(config.ABCDirectory, tempFile, ref signature);
                                    if (datasignResult != DataSignESD.DataSignResult.ERR_SUCCESS)
                                    {
                                        string message = "Error Signing Receipt: " + datasignResult.ToString();
                                        throw new Exception(message);
                                    }
                                    break;

                                case eFiscalDevice.ALGOBOX_NET:
                                    AlgoboxNetESD defaultAlgobox = fiscalDevice as AlgoboxNetESD;
                                    AlgoboxNetESD.AlgoboxNetResult algoboxResult = defaultAlgobox.SignDocument(tempFile, documentService.CreateFiscalInfoLine(appContext.CurrentDocument, Kernel.LogFile), ref signature);
                                    if (algoboxResult != AlgoboxNetESD.AlgoboxNetResult.SUCCESS)
                                    {
                                        string message = "Error Signing Receipt: " + signature.ToString();
                                        throw new Exception(message);
                                    }
                                    break;

                                case eFiscalDevice.DISIGN:
                                    DiSign defaultDisign = fiscalDevice as DiSign;
                                    int result;
                                    int maxNumberOfRetries = 3;
                                    int currentTry = 0;
                                    do
                                    {
                                        currentTry++;
                                        result = defaultDisign.SignDocument(allTheLines.Aggregate((f, s) => f + Environment.NewLine + s), documentService.CreateFiscalInfoLine(appContext.CurrentDocument, Kernel.LogFile), ref signature);
                                    } while (result != 0 && currentTry < maxNumberOfRetries);

                                    if (result == -4)//eFiscalResponseType.FISCAL_MUST_ISSUE_Z
                                    {
                                        actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(false));
                                        throw new POSException(Resources.POSClientResources.YOU_MUST_ISSUE_Z);
                                    }
                                    if (result == -2)//eFiscalResponseType.FISCAL_IS_ON_ERROR
                                    {
                                        actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(true));
                                    }
                                    else if (result != 0)
                                    {
                                        string message = "Error Signing Receipt. Error Code: " + result + " Message: " + signature.ToString();
                                        throw new Exception(message);
                                    }
                                    else
                                    {
                                        actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(false));
                                    }
                                    break;

                                default:
                                    throw new POSException(POSClientResources.INVALID_FISCAL_DEVICE);
                            }
                            break;
                        }
                        catch (POSException posException)
                        {
                            Kernel.LogFile.Error(posException, "Unable to sign using " + fiscalDevice.DeviceName);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            fiscalDevice.FailureCount++;
                            Kernel.LogFile.Info(ex, "Unable to sign using " + fiscalDevice.DeviceName);
                            exceptionsThrown.Add(fiscalDevice.DeviceName + " : " + ex.GetFullMessage());
                        }
                    }
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                    if (exceptionsThrown.Count >= fiscalDevices.Count)
                    {
                        string fiscalDevicesErrors = string.Empty;
                        exceptionsThrown.ForEach(exception =>
                        {
                            fiscalDevicesErrors += Environment.NewLine + exception + Environment.NewLine;
                        });
                        throw new POSException(string.Format("Cannot sign the receipt after trying with all fiscal devices. {0}", fiscalDevicesErrors));
                    }
                    Kernel.LogFile.Debug(String.Format("End get signature of receipt {0} - {1}", appContext.CurrentDocument.DocumentSeriesCode, appContext.CurrentDocument.DocumentNumber));
                }
            }
            return signature;
        }

        /// <summary>
        /// Ratificates the payments on ADHME Printer.
        /// </summary>
        private void ExecuteRatificationOnPayments(DocumentHeader header, ISessionManager sessionManager, IDeviceManager deviceManager, IFormManager formManager, eFiscalMethod fiscalMethod, int terminalId, IActionManager actionManager)
        {
            if (header.HasPaymentWithRatification)
            {
                foreach (DocumentPayment payment in header.DocumentPayments.Where(x => x.Amount >= 0))
                {
                    PaymentMethod method = sessionManager.GetObjectByKey<PaymentMethod>(payment.PaymentMethod);
                    if (method.NeedsRatification)
                    {
                        if (fiscalMethod == eFiscalMethod.ADHME)
                        {
                            FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                            if (printer.HasSlipStation == false)
                            {
                                formManager.ShowCancelOnlyMessageBox(POSClientResources.PRINTER_DOES_NOT_SUPPORT_SLIP_STATION);
                                break;
                            }

                            List<string> lines = new List<string>();
                            lines.Add("***" + POSClientResources.RATIFIED.ToUpperGR() + "***");
                            lines.Add(POSClientResources.POS.ToUpperGR() + " " + terminalId + " " + DateTime.Now);
                            lines.Add(POSClientResources.SIGNATURE.ToUpperGR() + "..............");

                            actionManager.GetAction(eActions.SLIP_PRINT).Execute(new ActionSlipPrintParams(printer, lines.ToArray()));
                        }
                        else
                        {
                            //TODO for non fiscals
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Persists to Local DB the customer's earned points.
        /// </summary>
        private void SaveCustomerPoints(IAppContext appContext, ISessionManager sessionManager, IConfigurationManager config)
        {
            if (appContext.CurrentDocument.Customer != config.DefaultCustomerOid)
            {
                Customer customer = sessionManager.GetObjectByKey<Customer>(appContext.CurrentDocument.Customer);
                int newPoints = (int)appContext.CurrentDocument.TotalPoints;
                int previousTotalPoints = (int)customer.CollectedPoints;
                int subtractedPoints = (int)appContext.CurrentDocument.ConsumedPointsForDiscount;

                ////case 5261
                customer.CollectedPoints -= appContext.CurrentDocument.ConsumedPointsForDiscount;
                customer.CollectedPoints += appContext.CurrentDocument.TotalPoints;
                customer.Save();
                customer.Session.CommitTransaction();
            }
        }

        /// <summary>
        /// Try to updated document's coupons on StoreController.
        /// </summary>
        private void UploadTransactionCoupons(IAppContext appContext, DocumentType documentType, ISessionManager sessionManager, string storeControllerUrl)
        {
            try
            {
                if (appContext.CurrentDocument.TransactionCoupons.Count > 0)
                {
                    using (WebService.POSUpdateService webService = new WebService.POSUpdateService())
                    {
                        webService.Url = storeControllerUrl;
                        string errorMessage = string.Empty;
                        List<WebService.TransactionCouponViewModel> transactionCoupons = new List<WebService.TransactionCouponViewModel>();

                        foreach (TransactionCoupon transactionCoupon in appContext.CurrentDocument.TransactionCoupons.Where(transCoupon => transCoupon.IsCanceled == false))
                        {
                            transactionCoupons.Add(new WebService.TransactionCouponViewModel() { Oid = transactionCoupon.Oid, Coupon = transactionCoupon.Coupon });
                        }

                        appContext.CurrentDocument.CouponsHaveBeenUpdatedOnStoreController = documentType.ReserveCoupons ? webService.UpdateCoupons(transactionCoupons.ToArray(), out errorMessage) : false;
                        if (appContext.CurrentDocument.CouponsHaveBeenUpdatedOnStoreController == false)
                        {
                            Kernel.LogFile.Error(errorMessage);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //NOP If coupons have not been updated they should be when the document is uploaded
                string errorMessage = exception.GetFullMessage();
            }
        }

        /// <summary>
        /// Check cashier cash limits and forcing a withdraw if needed.
        /// </summary>
        private void CheckWithDraw(IConfigurationManager config, ITotalizersService totalizersService, IAppContext appContext, IActionManager actionManager, IDeviceManager deviceManager, IFormManager formManager)
        {
            try
            {
                eForcedWithdrawMode forcedWithdrawMode = config.ForcedWithdrawMode;
                decimal forceWithdrawCashLimit = config.ForcedWithdrawCashAmountLimit;
                if (forcedWithdrawMode != eForcedWithdrawMode.NO && config.ForcedWithdrawCashAmountLimit > 0)
                {
                    bool done = (totalizersService.GetTotalCashInPos(appContext.CurrentDailyTotals) <= forceWithdrawCashLimit);
                    if (!done)
                    {
                        actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
                    }

                    while (!done)
                    {
                        string title = String.Format(POSClientResources.FORCED_WITHDRAW_TITLE, forceWithdrawCashLimit);
                        using (frmWithdrawDeposit form = new frmWithdrawDeposit(eOpenDrawerMode.WITHDRAW, Kernel, title))
                        {
                            DialogResult result = form.ShowDialog();
                            bool cashIsBelowLimit = totalizersService.GetTotalCashInPos(appContext.CurrentDailyTotals) <= forceWithdrawCashLimit;
                            done = (cashIsBelowLimit || (forcedWithdrawMode == eForcedWithdrawMode.SKIPPABLE && result != DialogResult.OK)) && form.FormClosedWithError == false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = POSClientResources.ERROR_DURING_FORCED_WITHDRAW;
                formManager.ShowCancelOnlyMessageBox(errorMessage + Environment.NewLine + ex.GetFullMessage());
                Kernel.LogFile.Error(ex, errorMessage);
            }
        }

        private void CalculateTotals(IAppContext appContext, DocumentType documentType, IDocumentService documentService, IConfigurationManager config, ISessionManager sessionManager, ITotalizersService totalizersService)
        {
            if (appContext.CurrentDocument.IsAddedToTotals == false)
            {
                if (documentType.IsPrintedOnStoreController == false)
                {
                    appContext.CurrentDocument.DocumentNumber = 0;
                    documentService.AssignDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid);
                }

                if (appContext.CurrentDocument.Change != 0)
                {
                    DocumentPayment changePayment = new DocumentPayment(sessionManager.GetSession<DocumentPayment>());
                    PaymentMethod defaultPaymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(config.DefaultPaymentMethodOid);
                    changePayment.DocumentHeader = appContext.CurrentDocument;
                    changePayment.DocumentHeaderOid = appContext.CurrentDocument.Oid;
                    changePayment.Amount = appContext.CurrentDocument.Change * (-1);
                    changePayment.PaymentMethod = config.DefaultPaymentMethodOid;
                    changePayment.PaymentMethodDescription = defaultPaymentMethod.Description;
                    changePayment.PaymentMethodType = defaultPaymentMethod.PaymentMethodType;
                    changePayment.IncreasesDrawerAmount = defaultPaymentMethod.IncreasesDrawerAmount;
                    sessionManager.FillDenormalizedFields(changePayment);
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
                        increaseVatAndSales, false, eTotalizorAction.INCREASE);
                }
            }
        }

        /// <summary>
        /// Persists to Local DB the document after it is signed.
        /// </summary>
        private void SaveSignedDocument(IAppContext appContext)
        {
            appContext.CurrentDocument.Save();
            appContext.CurrentDocument.Session.CommitTransaction();
        }

        /// <summary>
        /// Prints customer loyalty.
        /// </summary>
        private void PrintLoyalty(IAppContext appContext, IConfigurationManager config, Device primaryPrinter, ISessionManager sessionManager, IReceiptBuilder receiptBuilder)
        {
            try
            {
                if (appContext.CurrentDocument.Customer != config.DefaultCustomerOid)
                {
                    Customer customer = sessionManager.GetObjectByKey<Customer>(appContext.CurrentDocument.Customer);
                    int newPoints = (int)appContext.CurrentDocument.TotalPoints;
                    int previousTotalPoints = (int)customer.CollectedPoints - newPoints;
                    int subtractedPoints = (int)appContext.CurrentDocument.ConsumedPointsForDiscount;
                    int lineChars = primaryPrinter.Settings.LineChars;
                    List<string> prefixLines = new List<string>();
                    string customerLine = POSClientResources.CUSTOMER.ToUpperGR() + " : " + customer.CompanyName.ToUpperGR();
                    prefixLines.Add(new string(customerLine.Take(lineChars).ToArray()));
                    prefixLines.Add(POSClientResources.POINT_ANALYSIS.ToUpperGR());
                    ISynchronizationManager SynchronizationManager = Kernel.GetModule<ISynchronizationManager>();
                    bool showTotals = SynchronizationManager.ServiceIsAlive;
                    List<string> lines = receiptBuilder.CreatePointsPrintingLines(showTotals, newPoints, previousTotalPoints, subtractedPoints, lineChars, prefixLines.ToArray());

                    if (lines.Count > 0)
                    {
                        if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                        {
                            if ((primaryPrinter as Printer).ConType == ConnectionType.OPOS && (primaryPrinter as Printer).SupportsCutter)
                            {
                                lines.Add(Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                            }
                            (primaryPrinter as Printer).PrintLines(lines.ToArray());
                        }
                        else if (config.FiscalMethod == eFiscalMethod.ADHME)
                        {
                            (primaryPrinter as FiscalPrinter).PrintIllegal(lines.Select(x => new FiscalLine() { Value = x }).ToArray());
                        }
                    }
                    if (config.PrintDiscountAnalysis)
                    {
                        List<string> discountLines = receiptBuilder.CreateDiscountPrintingLines(appContext.CurrentDocument, lineChars);
                        if (discountLines.Count > 0)
                        {
                            if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                            {
                                if ((primaryPrinter as Printer).ConType == ConnectionType.OPOS && (primaryPrinter as Printer).SupportsCutter)
                                {
                                    discountLines.Add(ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                                }
                                (primaryPrinter as Printer).PrintLines(discountLines.ToArray());
                            }
                            else if (config.FiscalMethod == eFiscalMethod.ADHME)
                            {
                                (primaryPrinter as FiscalPrinter).PrintIllegal(discountLines.Select(x => new FiscalLine() { Value = x }).ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception e) //silent try catch, document has printed but printing loyalty failed.
            {
                Kernel.LogFile.Info(e.GetFullMessage(), "  ActionPrintReceipt:PrintPoints");
            }
        }
    }
}
