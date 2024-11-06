using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Hardware.Common.Exceptions;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Cancels the current document
    /// </summary>
    public class ActionCancelDocument : Action
    {

        public ActionCancelDocument(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.CANCEL_DOCUMENT; }
        }

        public override bool RequiresParameters  //works both ways
        {
            get { return false; }
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
                return eKeyStatus.POSITION2;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();

            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT || appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                DialogResult result = DialogResult.OK;
                if (parameters == null || (parameters is ActionCancelDocumentParams && (parameters as ActionCancelDocumentParams).ShowDialog))
                {
                    result = formManager.ShowMessageBox(POSClientResources.DO_YOU_WANT_TO_CANCEL, MessageBoxButtons.OKCancel);//MessageBox.Show(POSClientResources.DO_YOU_WANT_TO_CANCEL, POSClientResources.CANCEL_RECEIPT, MessageBoxButtons.OKCancel);
                }
                if (result == DialogResult.OK)
                {
                    if (appContext.CurrentDocument.DocumentPaymentsEdps.Where(x => x.DocumentPayment != Guid.Empty).Count() > 0)
                    {
                        if (appContext.CurrentDocument.DocumentPaymentsEdps.Where(x => x.DocumentPayment != Guid.Empty).Count() > 1)
                        {
                            throw new POSException(POSClientResources.CANCEL_EDPS_PAYMENTS_FIRST);
                        }
                        EdpsPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is EdpsPaymentCreditDevice) as EdpsPaymentCreditDevice;
                        if (device != null)
                        {
                            foreach (DocumentPayment payment in appContext.CurrentDocument.DocumentPayments.Where(x => x.DocumentPaymentEdps != null))
                            {
                                DocumentPaymentEdps edpsPayment = payment.DocumentPaymentEdps;
                                EdpsDeviceResult edresult;
                                if (edpsPayment.TRM[0] != 'p' && edpsPayment.TRM[0] != 'P')
                                {
                                    edresult = device.CancelPayment(edpsPayment.ReceiptNumber, payment.Amount, 0, 1, 0, config.TerminalID.ToString());
                                }
                                else
                                {
                                    edresult = device.ExecuteRefund(payment.Amount, 0, 0, 0, config.TerminalID.ToString());
                                }
                                if (edresult != null && edresult.ResponseCode == "00")
                                {
                                    edpsPayment.DocumentPayment = Guid.Empty;
                                    DocumentPaymentEdps cancelingEdps = documentService.CreateDocumentPaymentEdps(edresult, payment.Amount, edpsPayment.Session);
                                    cancelingEdps.DocumentHeader = payment.DocumentHeader;

                                }
                                else
                                {
                                    string errorMessage = POSClientResources.ERROR + ":" + POSClientResources.PAYMENT_METHOD;
                                    if (edresult != null && !string.IsNullOrWhiteSpace(edresult.ErrorCode))
                                    {
                                        errorMessage += ":" + edresult.ErrorCode;
                                    }
                                    throw new POSUserVisibleException(errorMessage);
                                }
                            }
                        }
                    }

                    if (appContext.CurrentDocument.DocumentPaymentsCardlink.Where(x => x.DocumentPayment != Guid.Empty).Count() > 0)
                    {

                        CardlinkPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is CardlinkPaymentCreditDevice) as CardlinkPaymentCreditDevice;
                        if (device != null)
                        {
                            using (frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PROCESS_PAYMENT_ON_DEVICE))
                            {
                                foreach (DocumentPayment payment in appContext.CurrentDocument.DocumentPayments.Where(x => x.DocumentPaymentCardlink != null))
                                {
                                    try
                                    {
                                        dialog.btnOK.Visible = false;
                                        dialog.btnCancel.Visible = false;
                                        dialog.btnRetry.Visible = false;
                                        dialog.CanBeClosedByUser = false;
                                        dialog.Show();
                                        dialog.BringToFront();
                                        Application.DoEvents();
                                        DocumentPaymentCardlink CardlinkPayment = payment.DocumentPaymentCardlink;
                                        CardlinkDeviceResult cardlinkresult = null;
                                        cardlinkresult = CardlinkLink.ExecuteRefund(payment.Amount, CardlinkPayment.ReceiptNumber, appContext.CurrentUser.POSUserName, config.TerminalID, 0, 0, device.Settings.Ethernet);
                                        if (cardlinkresult == null || cardlinkresult.RespCode != "00")
                                        {
                                            string errorMessage = POSClientResources.ERROR + " : " + cardlinkresult.PosMessage == null || cardlinkresult.PosMessage == string.Empty ? POSClientResources.PAYMENT_METHOD : cardlinkresult.PosMessage;
                                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(errorMessage));
                                            return;
                                        }
                                        else
                                        {
                                            CardlinkPayment.DocumentPayment = Guid.Empty;
                                            DocumentPaymentCardlink cancelingCardlink = documentService.CreateDocumentPaymentCardlink(cardlinkresult, payment.Amount, CardlinkPayment.Session);
                                            cancelingCardlink.DocumentHeader = payment.DocumentHeader;
                                            CardlinkPayment.Save();
                                            CardlinkPayment.Session.CommitTransaction();
                                            cancelingCardlink.Save();
                                            cancelingCardlink.Session.CommitTransaction();

                                        }
                                        dialog.Hide();
                                        dialog.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        Kernel.LogFile.Error(ex.Message);
                                        string errorMessage = ex.Message + "Error : " + POSClientResources.PAYMENT_METHOD;
                                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(errorMessage));
                                        return;
                                    }
                                }
                            }

                        }
                    }
                }
                bool IsTotalVatCalculationPerReceipt = false;
                if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    FiscalPrinter primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                    if (primaryPrinter == null)
                    {
                        throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                    }
                    try
                    {
                        IsTotalVatCalculationPerReceipt = primaryPrinter.IsTotalVatCalculationPerReceipt;
                        actionManager.GetAction(eActions.FISCAL_PRINTER_PRINT_RECEIPT).Execute(new ActionFiscalPrinterPrintReceiptParams(primaryPrinter, true, true, false));
                    }
                    catch (POSFiscalPrinterException ex)
                    {
                        string message = String.Format(POSClientResources.FISCAL_PRINTER_ERROR, ex.ErrorCode, ex.ErrorDescription);
                        throw new POSException(message);
                    }
                    catch (Exception ex)
                    {
                        string message = String.Format(POSClientResources.FISCAL_PRINTER_ERROR, "", ex.GetFullMessage());
                        throw new POSException(message);
                    }
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

                appContext.CurrentDocument.TransactionCoupons.ToList().ForEach(transactionCoupon => transactionCoupon.IsCanceled = true);
                documentService.CancelDocument(appContext.CurrentDocument, appContext.FiscalDate, appContext.CurrentDocument.DocumentNumber, config.FiscalMethod == eFiscalMethod.ADHME);

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
                    totalizersService.UpdateTotalizers(config, appContext.CurrentDocument, appContext.CurrentUser, appContext.CurrentDailyTotals,
                    appContext.CurrentUserDailyTotals, config.CurrentStoreOid, config.CurrentTerminalOid, increaseVatAndSales, IsTotalVatCalculationPerReceipt, eTotalizorAction.INCREASE);
                }

                appContext.CurrentDocument = null;
                appContext.CurrentDocumentLine = null;
                appContext.CurrentDocumentPayment = null;
                appContext.CurrentCustomer = null;
                appContext.SetMachineStatus(eMachineStatus.SALE);
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(null, true, false)); //clears the price textboxes
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(null)); //clears the price textboxes

            }
        }
    }

}
