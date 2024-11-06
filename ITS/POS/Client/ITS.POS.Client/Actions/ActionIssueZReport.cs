using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
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
using ITS.Retail.WebClient.AuxillaryClasses;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Handles the closing of the fiscal day and issues a Z report and
    /// </summary>
    public class ActionIssueZReport : Action
    {
        public ActionIssueZReport(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ISSUE_Z; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }


        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.DAYSTARTED | eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        private void AssignZReportNumber(ISessionManager sessionManager, IConfigurationManager config, DailyTotals currentDailyTotals, int fiscalPrinterZReportNumber = -1)
        {
            ZReportSequence seq = sessionManager.FindObject<ZReportSequence>(new BinaryOperator("POS", config.CurrentTerminalOid));
            if (seq == null)
            {
                seq = new ZReportSequence(sessionManager.GetSession<ZReportSequence>());
                seq.POS = config.CurrentTerminalOid;
                seq.CreatedByDevice = config.CurrentTerminalOid.ToString();
                seq.ZReportNumber = 0;
            }

            if (fiscalPrinterZReportNumber > 0) //Got it from fiscal printer
            {
                seq.ZReportNumber = fiscalPrinterZReportNumber;
            }
            else
            {
                seq.ZReportNumber++;
            }
            currentDailyTotals.ZReportNumber = seq.ZReportNumber;
            currentDailyTotals.Save();
            seq.Save();
            sessionManager.GetSession<ZReportSequence>().CommitChanges();
        }

        frmMessageBox dialogZ;

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ITS.POS.Hardware.RBS.Fiscal.RBSElioFiscalPrinter.ShowMessageHandlePrinterActions += RBSElioFiscalPrinter_ShowMessageHandlePrinterActions;
            ITS.POS.Hardware.RBS.Fiscal.RBSElioFiscalPrinter.ShowMessageIssueingZReport += RBSElioFiscalPrinter_ShowMessageIssueingZReport;
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            OwnerApplicationSettings appSettings = config.GetAppSettings();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IPlatformRoundingHandler platformRoundingHandler = Kernel.GetModule<IPlatformRoundingHandler>();
            ActionIssueZReportParams castedParams = parameters as ActionIssueZReportParams;

            dialogZ = formManager.CreateMessageBox(POSClientResources.ISSUING_Z_PLEASE_WAIT);
            try
            {
                //Issue X if needed
                if (appContext.CurrentUserDailyTotals != null &&
                   (appContext.GetMachineStatus() == eMachineStatus.SALE || appContext.GetMachineStatus() == eMachineStatus.PAUSE))
                {
                    actionManager.GetAction(eActions.ISSUE_X).Execute(new ActionIssueXReportParams(castedParams.SkipNonFiscalReportPrint), true);
                    Application.DoEvents();
                }

                if (appContext.CurrentUserDailyTotals != null &&
                   (appContext.GetMachineStatus() == eMachineStatus.SALE || appContext.GetMachineStatus() == eMachineStatus.PAUSE))
                {
                    //Issue X has been aborted by user
                    Application.DoEvents();
                    return;
                }

                appContext.MainForm.Invoke((MethodInvoker)delegate ()
                {
                    dialogZ.btnOK.Visible = false;
                    dialogZ.btnCancel.Visible = false;
                    dialogZ.btnRetry.Visible = false;
                    dialogZ.CanBeClosedByUser = false;
                    dialogZ.Show();
                    dialogZ.BringToFront();
                });
                Application.DoEvents();

                Device primaryPrinter = null;
                if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                {
                    primaryPrinter = deviceManager.GetPrimaryDevice<Printer>();
                }
                else if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                }
                else
                {
                    throw new POSException("Unknown Fiscal Method");
                }

                if (primaryPrinter == null)
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }

                if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                {
                    if (appContext.CurrentDailyTotals != null)
                    {
                        AssignZReportNumber(sessionManager, config, appContext.CurrentDailyTotals);
                    }
                }
                else if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    try
                    {
                        FiscalPrinter fiscalPrinter = (primaryPrinter as FiscalPrinter);
                        fiscalPrinter.ReadDeviceStatus();
                        if (fiscalPrinter.FiscalStatus.DayOpen && appContext.CurrentDailyTotals != null)
                        {
                            double vatAmountA, vatAmountB, vatAmountC, vatAmountD, vatAmountE, netAmountA, netAmountB, netAmountC, netAmountD, netAmountE;
                            fiscalPrinter.GetDayAmounts(out vatAmountA, out vatAmountB, out vatAmountC, out vatAmountD, out vatAmountE,
                                                        out netAmountA, out netAmountB, out netAmountC, out netAmountD, out netAmountE);
                            decimal roundedVatAmountA = platformRoundingHandler.RoundDisplayValue((decimal)vatAmountA);
                            decimal roundedVatAmountB = platformRoundingHandler.RoundDisplayValue((decimal)vatAmountB);
                            decimal roundedVatAmountC = platformRoundingHandler.RoundDisplayValue((decimal)vatAmountC);
                            decimal roundedVatAmountD = platformRoundingHandler.RoundDisplayValue((decimal)vatAmountD);
                            decimal roundedVatAmountE = platformRoundingHandler.RoundDisplayValue((decimal)vatAmountE);
                            totalizersService.SetVatTotalsFromFiscalPrinter(appContext.CurrentDailyTotals, roundedVatAmountA, roundedVatAmountB,
                                                                            roundedVatAmountC, roundedVatAmountD, roundedVatAmountE);
                        }
                    }
                    catch (NotSupportedException ex)
                    {
                        Kernel.LogFile.Warn(ex, "Fiscal printer does not support \"GetDayAmounts\"");
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error(ex, "Failure getting DayAmounts from Fiscal Printer");
                    }
                }

                List<string> allTheLines = new List<string>();
                DateTime printedDate = DateTime.Now;
                if (appContext.CurrentDailyTotals != null)
                {
                    //Fix differences in Totalizors and Documents
                    //------------------------------------------
                    XPCollection<VatFactor> allFactors = new XPCollection<VatFactor>(sessionManager.GetSession<VatFactor>());
                    foreach (VatFactor factor in allFactors)
                    {
                        totalizersService.FixDocumentVatDeviations(documentService, config, appContext.CurrentDailyTotals, factor, appSettings);
                    }
                    //------------------------------------------


                    Model.Settings.POS currentTerminal = sessionManager.GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
                    Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);

                    Report report = totalizersService.CreateZReport(appSettings, appContext.CurrentDailyTotals, currentTerminal.Name, config.DefaultDocumentTypeOid,
                        config.ProFormaInvoiceDocumentTypeOid, defaultCustomer.VatLevel, config.DepositDocumentTypeOid, config.WithdrawalDocumentTypeOid, config.AsksForFinalAmount);

                    Receipt.Receipt receipt = new Receipt.Receipt();
                    receipt.Header = receiptBuilder.CreateReceiptHeader(config.ZReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                    receipt.Body = receiptBuilder.CreateReceiptBody(config.ZReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                    receipt.Footer = receiptBuilder.CreateReceiptFooter(config.ZReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                    allTheLines = receipt.GetReceiptLines();
                    printedDate = report.DateIssued;
                }

                if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                {
                    List<Device> allFiscalDevices = deviceManager.GetEAFDSSDevicesByPriority(config.FiscalDevice);
                    List<Device> fiscalDevices = allFiscalDevices.Where(x => x.Settings.IsPrimary).ToList();
                    //fix auto issue z non primary single eafdsss
                    if (fiscalDevices.Count == 0 && allFiscalDevices.Count == 1)
                    {
                        fiscalDevices = allFiscalDevices;
                    }

                    bool issueZ = fiscalDevices.Count > 0 && config.AutoIssueZEAFDSS && castedParams.SkipIssuingZToTheDevice == false;
                    bool printNonFiscalReport = castedParams.SkipNonFiscalReportPrint == false;
                    string uploadZerrorMessage = string.Empty;
                    issueZ_EAFDSS(appContext, formManager, config.ABCDirectory, Kernel.LogFile, primaryPrinter as Printer, fiscalDevices, allTheLines, issueZ, printNonFiscalReport, ref uploadZerrorMessage);
                }
                else if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    bool issueZToTheDevice = castedParams.SkipIssuingZToTheDevice == false;
                    bool printNonFiscalReport = castedParams.SkipNonFiscalReportPrint == false;
                    issueZ_ADHME(sessionManager, appContext, config, primaryPrinter as FiscalPrinter, allTheLines, issueZToTheDevice, printNonFiscalReport);
                }

                if (appContext.CurrentDailyTotals != null)
                {
                    appContext.CurrentUser = null;
                    appContext.CurrentDailyTotals.PrintedDate = printedDate;
                    appContext.CurrentDailyTotals.FiscalDateOpen = false;
                    appContext.CurrentDailyTotals.Save();
                    appContext.CurrentDailyTotals.Session.CommitTransaction();
                    appContext.CurrentDailyTotals = null;
                }

                appContext.SetMachineStatus(eMachineStatus.CLOSED);
                try
                {
                    actionManager.GetAction(eActions.RESET_EAFDSS_DEVICES_ORDER).Execute();
                }
                catch (Exception exception)
                {
                    string errorMessage = exception.GetFullMessage();
                    appContext.MainForm.Invoke((MethodInvoker)delegate ()
                    {
                        formManager.ShowMessageBox(errorMessage);
                    });
                }
                ISynchronizationManager synchronizer = Kernel.GetModule<ISynchronizationManager>();
                if (synchronizer.CurrentUploadingStatus != eUploadingStatus.CHECKING
                    ||
                    formManager.ShowMessageBox(POSClientResources.POS_OFFLINE_DO_YOU_WANT_TO_TRY_TO_CLOSE_OFFLINE, MessageBoxButtons.OKCancel) == DialogResult.OK
                    )
                {
                    if (synchronizer.PostTransactionsAndPauseThread(1))
                    {
                        sessionManager.ReconnectToNewFile(TransactionConnectionHelper.DataLayer, TransactionConnectionHelper.ObjectsToDisposeOnDisconnect);
                    }
                    else
                    {
                        if (formManager.ShowMessageBox(POSClientResources.SYNC_FAILED + Environment.NewLine + POSClientResources.DO_YOU_WANT_TO_RETRY, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            if (synchronizer.PostTransactionsAndPauseThread(2))
                            {
                                sessionManager.ReconnectToNewFile(TransactionConnectionHelper.DataLayer, TransactionConnectionHelper.ObjectsToDisposeOnDisconnect);
                            }
                            else
                            {
                                formManager.ShowMessageBox(POSClientResources.SYNC_FAILED);
                            }
                        }
                    }

                    synchronizer.ResumePostTransactionThread();
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "Issue Z");
                dialogZ.Invoke((MethodInvoker)delegate ()
                {
                    dialogZ.Hide();
                    dialogZ.Dispose();
                    throw ex;
                });
            }

            appContext.MainForm.Invoke((MethodInvoker)delegate ()
            {
                dialogZ.Hide();
                dialogZ.Dispose();
            });
            try
            {
                ITS.POS.Hardware.RBS.Fiscal.RBSElioFiscalPrinter.ShowMessageHandlePrinterActions -= RBSElioFiscalPrinter_ShowMessageHandlePrinterActions;
                ITS.POS.Hardware.RBS.Fiscal.RBSElioFiscalPrinter.ShowMessageIssueingZReport -= RBSElioFiscalPrinter_ShowMessageIssueingZReport;
            }
            catch { }
        }

        private void RBSElioFiscalPrinter_ShowMessageIssueingZReport(object sender, EventArgs e)
        {
            try
            {
                dialogZ.Invoke((MethodInvoker)delegate ()
                {
                    dialogZ.rtbMessageArea.Text = POSClientResources.ISSUING_Z_PLEASE_WAIT;
                    dialogZ.Refresh();
                    dialogZ.Show();
                });
            }
            catch { }
        }

        private void RBSElioFiscalPrinter_ShowMessageHandlePrinterActions(object sender, EventArgs e)
        {
            try
            {
                dialogZ.Invoke((MethodInvoker)delegate ()
                {
                    dialogZ.rtbMessageArea.Text = POSClientResources.PLEASE_HANDLE_PRINTER_ACTIONS;
                    dialogZ.Refresh();
                    dialogZ.Show();
                });
            }
            catch { }
        }

        protected void issueZ_EAFDSS(IAppContext appContext, IFormManager formManager, string ABCDirectory, Logger logFile, Printer printer,
                                     List<Device> fiscalDevices, List<string> lines, bool issueZEAFDSS, bool printNonFiscalReport, ref string uploadZerrorMessage)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            bool cutPaper = true;

            if (printer == null)
            {
                throw new Exception(POSClientResources.ERROR_PRINT_Z + POSClientResources.PRINTER_NULL);
            }
            if (issueZEAFDSS)
            {
                bool eafdssIssued = false, cancelPressed = false;
                do
                {
                    try
                    {
                        actionManager.GetAction(eActions.ISSUE_Z_EAFDSS).Execute();
                        eafdssIssued = true;
                    }
                    catch (POSException posException)
                    {
                        cancelPressed = true;
                        if (formManager.ShowMessageBox(posException.GetFullMessage()) == DialogResult.OK)
                        {

                        }
                    }
                    catch (POSUploadZException uploadException)
                    {
                        eafdssIssued = true;
                        if (formManager.ShowMessageBox(uploadException.GetFullMessage()) == DialogResult.OK)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        if (formManager.ShowMessageBox(POSClientResources.ISSUE_Z_EAFDSS_ERROR_RETRY, MessageBoxButtons.RetryCancel) == DialogResult.Cancel)
                        {
                            cancelPressed = true;
                            throw new POSUserVisibleException(POSClientResources.FAILURE_ISSUING_EAFDSS_Z);
                        }
                    }
                } while (eafdssIssued == false && cancelPressed == false);
            }
            if (printer.ConType != ConnectionType.NONE && printNonFiscalReport)
            {

                DeviceResult beginTranResult = printer.BeginTransaction();
                if (beginTranResult != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_Z + ": " + beginTranResult.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), beginTranResult);
                    printer.EndTransaction();
                    throw new Exception(message);
                }

                //testing if printer is alive, maybe deprecated because of begin transaction
                DeviceResult testResult = printer.Print("\n");
                if (testResult != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_Z + ": " + testResult.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), testResult);
                    printer.EndTransaction();
                    throw new Exception(message);
                }
                //AssignZReportNumber(GlobalContext.CurrentDailyTotals);

                if (cutPaper && printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                {
                    lines.Add(ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                }
                DeviceResult result = printer.PrintLines(lines.ToArray());

                if (result != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_Z + ": " + result.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), result);                            
                    throw new Exception(message);
                }
                printer.EndTransaction();
            }

            if (appContext.CurrentDailyTotals != null)
            {
                appContext.CurrentDailyTotals.FiscalDeviceSerialNumber = "POS " + appContext.CurrentDailyTotals.POSID;
            }


        }

        protected static bool customZReportIsPrinted = false; //quick but dirty ....

        protected void issueZ_ADHME(ISessionManager sessionManager, IAppContext appContext, IConfigurationManager config, FiscalPrinter printer, List<string> lines,
                                    bool issueZToTheDevice, bool printNonFiscalReport)
        {
            if (printer == null)
            {
                throw new Exception(POSClientResources.ERROR_PRINT_Z + POSClientResources.PRINTER_NULL);
            }

            if (printer.ConType != ConnectionType.NONE && issueZToTheDevice)
            {

                int zReportNumber = 0;
                string pathToEJFiles;
                DeviceResult result;

                if (customZReportIsPrinted == false && printNonFiscalReport)
                {
                    try
                    {
                        FiscalLine[] fiscalLines = lines.Select(x => new FiscalLine() { Type = ePrintType.NORMAL, Value = x }).ToArray();
                        result = printer.PrintIllegal(fiscalLines);
                        if (result != DeviceResult.SUCCESS)
                        {
                            string message = POSClientResources.ERROR_PRINT_Z + ": " + result.ToLocalizedString();
                            throw new POSException(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        //a problem occured trying to print an illegal receipt, probably the fiscal printer is in a status that cannot print an illegal receipt
                        //Let it try to print Z
                        string errorMessage = ex.GetFullMessage();
                    }
                }

                customZReportIsPrinted = true;

                result = printer.IssueZReport(config.ABCDirectory, out zReportNumber, out pathToEJFiles);

                if (result != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_Z + ": " + result.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), result);                            
                    throw new POSException(message);
                }

                if (appContext.CurrentDailyTotals != null)
                {
                    AssignZReportNumber(sessionManager, config, appContext.CurrentDailyTotals, zReportNumber);
                    // Get Fiscal Device SerialNumber
                    appContext.CurrentDailyTotals.FiscalDeviceSerialNumber = String.IsNullOrWhiteSpace(printer.FiscalStatus.SerialNumber) ? "-" : printer.FiscalStatus.SerialNumber;
                }

                //Create EJFilePackage
                //------------------
                if (string.IsNullOrWhiteSpace(pathToEJFiles) == false)
                {
                    string zipfilePath = UtilsHelper.CreateEJZipFile(pathToEJFiles);

                    ElectronicJournalFilePackage newPackage = new ElectronicJournalFilePackage(sessionManager.GetSession<ElectronicJournalFilePackage>());
                    newPackage.CreatedByDevice = config.CurrentTerminalOid.ToString();
                    newPackage.POS = config.CurrentTerminalOid;
                    newPackage.Store = config.CurrentStoreOid;
                    newPackage.ZReportNumber = zReportNumber;
                    newPackage.Date = DateTime.Now;

                    using (FileStream file = new FileStream(zipfilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        newPackage.PackageData = data;
                    }

                    newPackage.Save();
                }
                sessionManager.CommitTransactionsChanges();

                customZReportIsPrinted = false;
            }
        }


    }
}
