using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Transactions;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// After user confirmation, deletes the current document payment.
    /// If the current document payment is an EDPS payment, it is also canceled at the device.
    /// </summary>
    public class ActionForceDeletePayment : Action
    {

        public ActionForceDeletePayment(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.FORCE_DELETE_PAYMENT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }


        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION4;
            }
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

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            DocumentPayment payment = (parameters as ActionForceDeletePaymentParams).Payment;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();

            if (payment != null)
            {
                DocumentHeader header = payment.DocumentHeader;
                if (formManager.ShowMessageBox(POSClientResources.DO_YOU_WANT_TO_CANCEL_LINE, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (payment.DocumentPaymentEdps != null)
                    {
                        DocumentPaymentEdps edpsPayment = payment.DocumentPaymentEdps;
                        EdpsPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is EdpsPaymentCreditDevice) as EdpsPaymentCreditDevice;

                        if (device != null)
                        {
                            using (frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PROCESS_PAYMENT_ON_DEVICE))
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
                                    EdpsDeviceResult result;
                                    if (edpsPayment.TRM[0] != 'p' && edpsPayment.TRM[0] != 'P')
                                    {
                                        result = device.CancelPayment(edpsPayment.ReceiptNumber, payment.Amount, 0, 1, 0, config.TerminalID.ToString());
                                    }
                                    else
                                    {
                                        result = device.ExecuteRefund(payment.Amount, 0, 0, 0, config.TerminalID.ToString());
                                    }
                                    if (result != null && result.ResponseCode == "00")
                                    {
                                        edpsPayment.DocumentPayment = Guid.Empty;
                                        DocumentPaymentEdps cancelingEdps = documentService.CreateDocumentPaymentEdps(result, payment.Amount, edpsPayment.Session);
                                        cancelingEdps.DocumentHeader = payment.DocumentHeader;
                                    }
                                    else
                                    {
                                        string errorMessage = POSClientResources.ERROR + " : " + POSClientResources.PAYMENT_METHOD;
                                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(errorMessage));
                                        return;
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
                        else
                        {
                            throw new POSUserVisibleException(POSClientResources.NO_PRIMARY_EDPS_FOUND);
                        }
                    }

                    if (payment.DocumentPaymentCardlink != null)
                    {
                        DocumentPaymentCardlink CardlinkPayment = payment.DocumentPaymentCardlink;
                        CardlinkPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is CardlinkPaymentCreditDevice) as CardlinkPaymentCreditDevice;
                        int NextDocNum = appContext.CurrentDocument.DocumentNumber == 0 ? documentService.GetNextDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid) : appContext.CurrentDocument.DocumentNumber;
                        if (device != null)
                        {
                            using (frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PROCESS_PAYMENT_ON_DEVICE))
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
                                    CardlinkDeviceResult result;
                                    result = CardlinkLink.ExecuteRefund(payment.Amount, NextDocNum, appContext.CurrentUser.POSUserName, config.TerminalID, 0, 0, device.Settings.Ethernet);
                                    if (result == null || result.RespCode != "00")
                                    {
                                        string errorMessage = result?.PosMessage ?? "Error : " + POSClientResources.PAYMENT_METHOD;
                                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(errorMessage));
                                        return;
                                    }
                                    else
                                    {
                                        CardlinkPayment.DocumentPayment = Guid.Empty;
                                        DocumentPaymentCardlink cancelingCardlink = documentService.CreateDocumentPaymentCardlink(result, payment.Amount, CardlinkPayment.Session);
                                        cancelingCardlink.DocumentHeader = payment.DocumentHeader;
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
                    payment.Delete();
                    appContext.CurrentDocumentPayment = header.DocumentPayments.LastOrDefault();
                    appContext.CurrentDocument.Save();
                    appContext.CurrentDocumentLine.Save();
                    sessionManager.CommitTransactionsChanges();
                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO).Execute(new ActionPublishDocumentPaymentInfoParams(appContext.CurrentDocumentPayment, header, true, false));
                    if (appContext.CurrentDocumentPayment != null)
                    {
                        actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(appContext.CurrentDocument));
                    }
                }
            }
        }
    }
}
