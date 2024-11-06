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
    /// Forces the cancelation of a document, on both the application and the fiscal printer, ignoring all device errors.
    /// </summary>
    public class ActionServiceForcedCancelDocument : Action
    {
        public ActionServiceForcedCancelDocument(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SERVICE_FORCED_CANCEL_DOCUMENT; }
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

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();

            DialogResult result = DialogResult.OK;
            ActionServiceForcedCancelDocumentParams castedParameters = parameters as ActionServiceForcedCancelDocumentParams;
            if (castedParameters.WarnUser)
            {
                string message = POSClientResources.WARNING.ToUpperGR() + "!!!" + Environment.NewLine + POSClientResources.DO_NOT_PROCEED_IF_YOU_DONT_KNOW;
                result = formManager.ShowMessageBox(message, MessageBoxButtons.OKCancel);
            }

            if (result == DialogResult.OK)
            {
                ///if application is in receipt, cancel the receipt silently without printing it to the adhme
                if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT || appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                {
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



                    bool isFiscalPrinterHandled = false;
                    bool IsTotalVatCalculationPerReceipt = false;
                    DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
                    if (config.FiscalMethod == eFiscalMethod.ADHME)
                    {
                        FiscalPrinter primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                        if (primaryPrinter == null)
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }
                        try
                        {
                            actionManager.GetAction(eActions.FISCAL_PRINTER_PRINT_RECEIPT).Execute(new ActionFiscalPrinterPrintReceiptParams(primaryPrinter, true, true, true));
                        }
                        catch (POSFiscalPrinterException ex)
                        {
                            string message = String.Format(POSClientResources.FISCAL_PRINTER_ERROR, ex.ErrorCode, ex.ErrorDescription);
                            Kernel.LogFile.Error(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            string message = String.Format(POSClientResources.FISCAL_PRINTER_ERROR, "", ex.GetFullMessage());
                            Kernel.LogFile.Error(ex.Message);
                        }
                        if (config.DefaultDocumentTypeOid == documentType.Oid && primaryPrinter.FiscalStatus.TransactionOpen)
                        {
                            isFiscalPrinterHandled = true;
                        }
                        IsTotalVatCalculationPerReceipt = primaryPrinter.IsTotalVatCalculationPerReceipt;
                    }




                    appContext.CurrentDocument.TransactionCoupons.ToList().ForEach(transactionCoupon => transactionCoupon.IsCanceled = true);
                    documentService.CancelDocument(appContext.CurrentDocument, appContext.FiscalDate, appContext.CurrentDocument.DocumentNumber, isFiscalPrinterHandled);

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
                                                                    increaseVatAndSales, IsTotalVatCalculationPerReceipt, eTotalizorAction.INCREASE);
                    }
                    try
                    {
                        appContext.CurrentDocument.Save();
                        sessionManager.CommitTransactionsChanges();
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Error(ex);
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
}
