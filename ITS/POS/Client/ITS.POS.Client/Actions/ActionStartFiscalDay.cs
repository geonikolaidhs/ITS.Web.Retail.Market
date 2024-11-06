using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Synchronization;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.POS.Client.Receipt;
using System.Reflection;
using ITS.POS.Client.Forms;
using System.Windows.Forms;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Starts the Fiscal Day
    /// </summary>
    public class ActionStartFiscalDay : Action
    {

        public ActionStartFiscalDay(IPosKernel kernel)
            : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            bool canOpenDay = true;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            ISynchronizationManager synchronizationManager = Kernel.GetModule<ISynchronizationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IVatFactorService vatFactorService = this.Kernel.GetModule<IVatFactorService>();
            Printer printer = deviceManager.GetPrimaryDevice<Printer>();
            bool cutPaper = true;
            try
            {
                if (appContext.GetMachineStatus() == eMachineStatus.CLOSED)
                {
                    if (!deviceManager.HasDemoModeBeenSetupCorrectly(config.DemoMode, config.FiscalMethod, config.FiscalDevice))
                    {
                        throw new Exception(POSClientResources.DEMO_MODE_ERROR);
                    }

                    Model.Settings.POS currentPos = sessionManager.GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
                    if (currentPos == null)
                    {
                        throw new POSException(POSClientResources.INCORRECT_SETTINGS);
                    }

                    if (currentPos.IsActive == false)
                    {
                        throw new POSException(POSClientResources.CURRENT_POS_IS_NOT_ACTIVE_START_DAY_NOT_ALLOWED);
                    }


                    if (config.FiscalMethod == eFiscalMethod.ADHME)
                    {
                        FiscalPrinter primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                        if (primaryPrinter == null)
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }

                        bool? isVatAValid, isVatBValid, isVatCValid, isVatDValid, isVatEValid;
                        vatFactorService.CheckIfVatFactorsAreValid(primaryPrinter, out isVatAValid, out isVatBValid, out isVatCValid, out isVatDValid, out isVatEValid);
                        if (isVatAValid == false ||
                           isVatBValid == false ||
                           isVatCValid == false ||
                           isVatDValid == false ||
                           isVatEValid == false)
                        {
                            double fiscalVatRateA, fiscalVatRateB, fiscalVatRateC, fiscalVatRateD, fiscalVatRateE;
                            primaryPrinter.ReadVatRates(out fiscalVatRateA, out fiscalVatRateB, out fiscalVatRateC, out fiscalVatRateD, out fiscalVatRateE);
                            string message = (POSClientResources.FISCAL_VAT_FACTORS_ARE_INVALID + Environment.NewLine +
                                                    "(" + POSClientResources.FISCAL_PRINTER_VAT_FACTORS + ") " +
                                                    "A:" + fiscalVatRateA + " " +
                                                    "B:" + fiscalVatRateB + " " +
                                                    "C:" + fiscalVatRateC + " " +
                                                    "D:" + fiscalVatRateD + " " +
                                                    "E:" + fiscalVatRateE);
                            formManager.ShowCancelOnlyMessageBox(message);
                            throw new POSException(message);
                        }

                    }




                    frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.DAY_START_PLEASE_WAIT);


                    appContext.MainForm.Invoke((MethodInvoker)delegate ()
                    {
                        dialog.btnOK.Visible = false;
                        dialog.btnCancel.Visible = false;
                        dialog.btnRetry.Visible = false;
                        dialog.CanBeClosedByUser = false;
                        dialog.Show();
                        dialog.BringToFront();
                    });
                    Application.DoEvents();

                    if (synchronizationManager.ServiceIsAlive && config.DemoMode == false)
                    {
                        List<Guid> documentSeriesList = new List<Guid>() { config.DefaultDocumentSeriesOid, config.ProFormaInvoiceDocumentSeriesOid, config.WithdrawalDocumentSeriesOid, config.DepositDocumentSeriesOid };
                        int localNumber, remoteNumber;
                        foreach (Guid documentSeries in documentSeriesList)
                        {

                            try
                            {
                                synchronizationManager.CheckDocumentSequenceNumber(documentSeries, out localNumber, out remoteNumber);

                                if (localNumber != remoteNumber)
                                {
                                    string seriesDescription = documentSeries.ToString();
                                    DocumentSeries series = sessionManager.GetObjectByKey<DocumentSeries>(documentSeries);
                                    if (series != null && !String.IsNullOrWhiteSpace(series.Description))
                                    {
                                        seriesDescription = series.Description;
                                    }

                                    if (localNumber > remoteNumber)
                                    {
                                        string message = String.Format(POSClientResources.DOCUMENT_SEQUENCE_DOES_NOT_MATCH_WARNING, seriesDescription, localNumber, remoteNumber);
                                        formManager.ShowCancelOnlyMessageBox(message);
                                        Kernel.LogFile.Info(message);
                                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                                        break;
                                    }
                                    else if (localNumber < remoteNumber)
                                    {
                                        string message = String.Format(POSClientResources.DOCUMENT_SEQUENCE_DOES_NOT_MATCH_ERROR, seriesDescription, localNumber, remoteNumber);
                                        formManager.ShowCancelOnlyMessageBox(message);
                                        Kernel.LogFile.Info(message);
                                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                                        canOpenDay = false;
                                        break;
                                    }
                                }


                            }
                            catch (InvalidOperationException invalidOperationException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                            }
                        }
                        try
                        {
                            synchronizationManager.CheckZReportSequenceNumber(config.CurrentTerminalOid, out localNumber, out remoteNumber);

                            if (localNumber != remoteNumber)
                            {
                                if (localNumber > remoteNumber)
                                {
                                    string message = String.Format(POSClientResources.DOCUMENT_SEQUENCE_DOES_NOT_MATCH_WARNING, "Z", localNumber, remoteNumber);
                                    formManager.ShowCancelOnlyMessageBox(message);
                                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                                }
                                else if (localNumber < remoteNumber)
                                {
                                    string message = String.Format(POSClientResources.DOCUMENT_SEQUENCE_DOES_NOT_MATCH_ERROR, "Z", localNumber, remoteNumber);
                                    formManager.ShowCancelOnlyMessageBox(message);
                                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                                    canOpenDay = false;
                                }
                            }

                        }
                        catch (InvalidOperationException invalidOperationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                        }
                    }

                    try
                    {
                        if (canOpenDay)
                        {
                            if (config.FiscalMethod == eFiscalMethod.ADHME)
                            {
                                FiscalPrinter fprinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                                if (fprinter == null)
                                {
                                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                                }

                                DeviceResult result = fprinter.OpenFiscalDay(config.TerminalID);
                                if (result != DeviceResult.SUCCESS)
                                {
                                    return;
                                }
                                List<string> dayStartLines = receiptBuilder.CreateFiscalVersionLines(deviceManager.GetVisibleVersion(config.FiscalMethod), fprinter.Settings.LineChars, POSClientResources.DAY_START.ToUpperGR());
                                fprinter.PrintIllegal(dayStartLines.Select(x => new FiscalLine() { Value = x }).ToArray());
                            }
                            else if (printer == null)
                            {
                                throw new Exception(POSClientResources.ERROR_PRINT_RECEIPT + ". " + POSClientResources.PRINTER_NULL);
                                //"Error Printing Receipt. Printer is null.");
                            }
                            if (printer != null)
                            {
                                printer.BeginTransaction();
                                Receipt.Receipt receipt = new Receipt.Receipt();
                                receipt.Header = new List<string>();

                                //receipt.Body = new List<string>();
                                receipt.Body = new List<string>(receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, POSClientResources.START_DAY.ToUpperGR() + Environment.NewLine));
                                
                                receipt.Body.AddRange(receiptBuilder.CreateSimplePrinterLines(eAlignment.LEFT, printer, false,
                                                        POSClientResources.DATE + ":" + DateTime.Now.ToString() + Environment.NewLine + POSClientResources.POS + ":" + config.TerminalID + Environment.NewLine));
                                receipt.Footer = new List<string>(receiptBuilder.CreateSimplePrinterLines(eAlignment.CENTER, printer, false, POSClientResources.START_DAY.ToUpperGR() + Environment.NewLine));
                                List<string> allTheLines = new List<string>(receipt.GetReceiptLines());
                                if (cutPaper && printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                                {
                                    allTheLines.Add(ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                                }
                                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start Send Print");
                                DeviceResult printResult = printer.PrintLines(allTheLines.ToArray());
                                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End Send Print");
                                printer.EndTransaction();
                            }
                            DateTime fiscalDate;
                            appContext.CurrentDailyTotals = totalizersService.CreateDailyTotals(config.CurrentTerminalOid, config.CurrentStoreOid,
                                                                                                    appContext.CurrentDailyTotals == null ? Guid.Empty : appContext.CurrentDailyTotals.Oid,
                                                                                                    out fiscalDate);
                            appContext.FiscalDate = fiscalDate;
                            appContext.SetMachineStatus(eMachineStatus.DAYSTARTED);
                        }
                    }
                    catch (InvalidOperationException invalidOperationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                    }
                    finally
                    {
                        appContext.MainForm.Invoke((MethodInvoker)delegate ()
                        {
                            dialog.Hide();
                            dialog.Dispose();
                        });
                    }
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Kernel.LogFile.Info(invalidOperationException, "Action Start Shift Error InvalidOperationException");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(invalidOperationException.GetFullMessage()));
                formManager.ShowCancelOnlyMessageBox(invalidOperationException.GetFullMessage());
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED; }
        }


        public override eActions ActionCode
        {
            get { return eActions.START_FISCAL_DAY; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
