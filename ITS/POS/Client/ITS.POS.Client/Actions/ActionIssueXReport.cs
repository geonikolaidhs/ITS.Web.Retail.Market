using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using System.IO;
using ITS.POS.Client.Forms;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using System.Windows.Forms;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Handles the closing of the current shift and issues an X Report
    /// </summary>
    public class ActionIssueXReport : Action
    {
        public ActionIssueXReport(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ISSUE_X; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION2;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ActionIssueXReportParams castedParams = parameters as ActionIssueXReportParams;
            OwnerApplicationSettings appSettings = config.GetAppSettings();

            if (appContext.DocumentsOnHold.Count != 0)
            {
                throw new POSException(String.Format(POSClientResources.YOU_CANNOT_ISSUE_X_DOCUMENTS_ON_HOLD, appContext.DocumentsOnHold.Count.ToString()));
            }
            Device primaryPrinter = null;

            if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<Printer>();
            }
            else if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
            }

            if (primaryPrinter == null)
            {
                throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
            }

            decimal cashAmountCountedByUser = 0;
            if (config.AsksForFinalAmount)
            {
                actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
                if (config.UseCashCounter == true)
                {
                    bool smallScreenSize = true;
                    if (Screen.PrimaryScreen.Bounds.Width > 800) smallScreenSize = false;
                    using (frmCashCount frm = new frmCashCount(this.Kernel, smallScreenSize))
                    {
                        try
                        {
                            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                cashAmountCountedByUser = frm.FinalAmount();
                                frm.ApplyCountedAmmounts(appContext.CurrentUserDailyTotals);
                            }
                            else
                            {
                                return;
                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }

                    }
                }
                else
                {

                    using (frmFinalAmount frm = new frmFinalAmount(this.Kernel))
                    {
                        if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            cashAmountCountedByUser = frm.FinalAmount;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            appContext.CurrentUserDailyTotals.UserCashFinalAmount = cashAmountCountedByUser;

            Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
            Model.Settings.POS currentTerminal = sessionManager.GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
            Report report = totalizersService.CreateXReport(appSettings, appContext.CurrentDailyTotals, appContext.CurrentUserDailyTotals, currentTerminal.Name, config.DefaultDocumentTypeOid,
                        config.ProFormaInvoiceDocumentTypeOid, defaultCustomer.VatLevel, config.DepositDocumentTypeOid, config.WithdrawalDocumentTypeOid, config.AsksForFinalAmount);

            if (castedParams.SkipNonFiscalReportPrint == false)
            {
                List<string> allTheLines = new List<string>();
                Receipt.Receipt receipt = new Receipt.Receipt();
                receipt.Header = receiptBuilder.CreateReceiptHeader(config.ΧReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                receipt.Body = receiptBuilder.CreateReceiptBody(config.ΧReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                receipt.Footer = receiptBuilder.CreateReceiptFooter(config.ΧReportSchema, report, primaryPrinter.Settings.LineChars, primaryPrinter.ConType);
                allTheLines = receipt.GetReceiptLines();

                if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                {
                    issueX_EAFDSS(primaryPrinter as Printer, allTheLines, report);
                }
                else if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    issueX_ADHME(primaryPrinter as FiscalPrinter, allTheLines, report);
                }
                else
                {
                    throw new POSException("Unknown Fiscal Method");
                }
            }

            Guid userDailyTotalsGuid = appContext.CurrentUserDailyTotals.Oid;
            Guid userClosingShift = appContext.CurrentUser.Oid;
            appContext.CurrentUser = null;
            appContext.CurrentUserDailyTotals.PrintedDate = report.DateIssued;
            appContext.CurrentUserDailyTotals.IsOpen = false;
            appContext.CurrentUserDailyTotals.CashDifference = appContext.CurrentUserDailyTotals.UserCashFinalAmount - totalizersService.GetTotalCashInPos(appContext.CurrentDailyTotals);


            //if ( appContext.CurrentUserDailyTotals.CashDifference != 0 )
            //{
            totalizersService.SaveUserDailyTotalCashDifference(appContext, appContext.CurrentUserDailyTotals.CashDifference, appContext.CurrentUserDailyTotals.UserCashFinalAmount, userClosingShift, config.CurrentTerminalOid, config.CurrentStoreOid);
            //}
            appContext.CurrentUserDailyTotals.Save();
            appContext.CurrentUserDailyTotals.Session.CommitTransaction();
            appContext.CurrentUserDailyTotals = null;

            appContext.SetMachineStatus(eMachineStatus.DAYSTARTED);
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
            try
            {
                actionManager.GetAction(eActions.EDPS_BATCH_CLOSE).Execute(new ActionEdpsBatchCloseParams() { UserDailyTotals = userDailyTotalsGuid });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                appContext.MainForm.Invoke((MethodInvoker)delegate ()
                {
                    formManager.ShowMessageBox(POSClientResources.PACKET_CLOSE_FAILURE + Environment.NewLine + errorMessage);
                });
            }
            formManager.ShowForm<frmSplashScreen>(appContext.MainForm, true);
        }

        protected void issueX_ADHME(FiscalPrinter printer, List<string> lines, Report report)
        {
            if (printer == null)
            {
                throw new Exception(POSClientResources.ERROR_PRINT_X + POSClientResources.PRINTER_NULL);
            }

            if (printer.ConType != ConnectionType.NONE)
            {
                FiscalLine[] fiscalLines = lines.Select(x => new FiscalLine() { Type = ePrintType.NORMAL, Value = x }).ToArray();
                DeviceResult result = printer.PrintIllegal(fiscalLines);
                if (result != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_X + ": " + result.ToLocalizedString();
                    throw new POSException(message);
                }

                result = printer.IssueXReport();
                if (result != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_X + ": " + result.ToLocalizedString();
                    throw new POSException(message);
                }


            }
        }

        protected void issueX_EAFDSS(Printer printer, List<string> lines, Report report)
        {
            bool cutPaper = true;
            if (printer == null)
            {
                throw new POSException(POSClientResources.ERROR_PRINT_X + POSClientResources.PRINTER_NULL);
            }

            if (printer.ConType != ConnectionType.NONE)
            {
                DeviceResult beginTranResult = printer.BeginTransaction();
                if (beginTranResult != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_X + ": " + beginTranResult.ToLocalizedString();
                    printer.EndTransaction();
                    throw new POSException(message);
                }

                //testing if printer is alive, maybe deprecated because of begin transaction
                DeviceResult testResult = printer.Print("\n");
                if (testResult != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_X + ": " + testResult.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), testResult);
                    printer.EndTransaction();
                    throw new Exception(message);
                }

                if (cutPaper && printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                {
                    lines.Add(ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                }
                DeviceResult result = printer.PrintLines(lines.ToArray());

                printer.EndTransaction();

                if (result != DeviceResult.SUCCESS)
                {
                    string message = POSClientResources.ERROR_PRINT_X + ": " + result.ToLocalizedString();//Enum.GetName(typeof(DeviceResult), result);
                    //Notify(message);
                    throw new Exception(message);
                }
            }
        }



    }
}
