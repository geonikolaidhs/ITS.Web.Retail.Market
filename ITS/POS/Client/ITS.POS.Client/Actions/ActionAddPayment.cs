using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Helpers;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Xpo;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.UserControls;
using System.Runtime.InteropServices;
using System.Threading;

namespace ITS.POS.Client.Actions
{


    /// <summary>
    /// Adds the given payment method with the given amount to the current document
    /// </summary>
    public class ActionAddPayment : Action
    {
        public ActionAddPayment(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_PAYMENT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        /// <summary>
        /// Expected Parameters: param1 = DocumentHeader header, param2 = double Amount, param3 = string PaymentMethod.Description
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dontCheckPermissions"></param>
        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            IDocumentService documentService = this.Kernel.GetModule<IDocumentService>();

            DocumentHeader header = appContext.CurrentDocument;
            DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
            if (header == null)
            {
                return;
            }

            decimal amount = (parameters as ActionAddPaymentParams).Amount;
            if (amount > docType.MaxPaymentAmount && docType.MaxPaymentAmount > 0)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.AMOUNT_EXCEED_LIMIT));
                return;
            }

            if (amount <= 0)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ZERO_AMOUNT_IS_NOT_ALLOWED));
                return;
            }

            ActionAddPaymentParams actionAddPaymentParams = (parameters as ActionAddPaymentParams);
            PaymentMethod paymentMethod = actionAddPaymentParams.PaymentMethod;
            if (paymentMethod == null)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAYMENT_METHOD_NOT_FOUND));
                return;
            }

            if (!paymentMethod.CanExceedTotal && !paymentMethod.IsNegative)
            {
                decimal paidAmount = header.DocumentPayments.Sum(x => x.Amount);

                if ((paidAmount + amount) > header.GrossTotal)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAID_AMOUNT_CANNOT_EXCEED_TOTAL));
                    return;
                }
            }

            decimal amountCannotExceedTotal = header.DocumentPayments.Where(x => x.CanExceedTotal == false).Sum(x => x.Amount);
            if (header.GrossTotal <= amountCannotExceedTotal)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAID_AMOUNT_CANNOT_EXCEED_TOTAL));
                return;
            }

            if (paymentMethod.NeedsRatification)
            {
                header.HasPaymentWithRatification = true;
            }


            XPCollection<PaymentMethodField> paymentMethodFields = new XPCollection<PaymentMethodField>(sessionManager.GetSession<PaymentMethodField>(), new BinaryOperator("PaymentMethod", paymentMethod.Oid));

            Dictionary<Guid, object> dictionary = new Dictionary<Guid, object>();

            EdpsDeviceResult edpsresult = null;

            CardlinkDeviceResult cardlinkresult = null;
            CardlinkPaymentCreditDevice cardlinkdevice = deviceManager.Devices.FirstOrDefault(x => x is CardlinkPaymentCreditDevice) as CardlinkPaymentCreditDevice;

            if ((parameters as ActionAddPaymentParams).CouponViewModel != null && paymentMethodFields.Select(x => x.FieldName).Contains((parameters as ActionAddPaymentParams).CouponViewModel.PropertyName))
            {
                dictionary.Add(paymentMethodFields.FirstOrDefault(x => x.FieldName == (parameters as ActionAddPaymentParams).CouponViewModel.PropertyName).Oid, (parameters as ActionAddPaymentParams).CouponViewModel.DecodedString);
            }
            IEnumerable<PaymentMethodField> remainingFields = paymentMethodFields.Where(x => dictionary.ContainsKey(x.Oid) == false);


            if (remainingFields.Count() > 0)
            {

                using (Form form = formManager.CreateCustomFieldsInputForm(remainingFields))
                {
                    form.ShowDialog();
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(new string[] { "" }));

                    if (form.DialogResult == DialogResult.OK)
                    {
                        foreach (Control control in form.Controls)
                        {
                            if (paymentMethodFields.Where(x => x.Oid.ToString() == control.Name).Count() > 0)
                            {
                                if (control is LookUpEdit)
                                {
                                    dictionary.Add(Guid.Parse(control.Name), (control as LookUpEdit).EditValue);
                                }
                                else if (control is ucCustomEnumerationGrid)
                                {
                                    dictionary.Add(Guid.Parse(control.Name), (control as ucCustomEnumerationGrid).SelectedValue);
                                }
                                else
                                {
                                    dictionary.Add(Guid.Parse(control.Name), control.Text);
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (paymentMethod.UseEDPS)
            {
                EdpsPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is EdpsPaymentCreditDevice) as EdpsPaymentCreditDevice;
                if (device != null)
                {
                    int installments = 1;
                    if (paymentMethod.UsesInstallments)
                    {
                        using (frmInstallments frmInstall = new frmInstallments(this.Kernel))
                        {
                            if (frmInstall.ShowDialog() != DialogResult.OK)
                            {
                                throw new POSException(POSClientResources.ACTION_CANCELED);
                            }
                            installments = (int)frmInstall.Installments;
                        }
                    }

                    using (frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PROCESS_PAYMENT_ON_DEVICE))
                    {
                        dialog.btnOK.Visible = false;
                        dialog.btnCancel.Visible = false;
                        dialog.btnRetry.Visible = false;
                        dialog.CanBeClosedByUser = false;
                        dialog.Show();
                        dialog.BringToFront();
                        Application.DoEvents();
                        if (paymentMethod.IsNegative == false)
                        {
                            if (paymentMethod.ForceEdpsOffline)
                            {
                                using (frmInsertEDPSAuthID frmAuthID = new frmInsertEDPSAuthID(this.Kernel))
                                {
                                    if (frmAuthID.ShowDialog() != DialogResult.OK)
                                    {
                                        throw new POSException(POSClientResources.ACTION_CANCELED);
                                    }
                                    edpsresult = device.ExecuteOfflinePayment(amount, 0m, installments, 0, config.TerminalID.ToString(), frmAuthID.AuthorizationKey);
                                }
                            }
                            else
                            {
                                edpsresult = device.ExecutePayment(amount, 0m, installments, 0, config.TerminalID.ToString());
                            }
                        }
                        else
                        {
                            edpsresult = device.ExecuteRefund(amount, 0m, 1, 0, config.TerminalID.ToString());
                        }
                        dialog.Hide();
                        dialog.Close();
                    }
                    if (edpsresult == null || edpsresult.ErrorCode != EdpsLink.RC_OK)
                    {
                        string errorMessage = POSClientResources.ERROR + ":" + POSClientResources.PAYMENT_METHOD;
                        if (edpsresult != null && !string.IsNullOrWhiteSpace(edpsresult.ErrorCode))
                        {
                            errorMessage += ":" + edpsresult.ErrorCode;

                        }
                        throw new POSUserVisibleException(errorMessage);
                    }
                }
                else
                {
                    throw new POSUserVisibleException(POSClientResources.NO_PRIMARY_EDPS_FOUND);
                }
            }

            if (paymentMethod.UseCardlink)
            {
                try
                {
                    if (cardlinkdevice != null)
                    {
                        int installments = 1;
                        if (paymentMethod.UsesInstallments)
                        {
                            using (frmInstallments frmInstall = new frmInstallments(this.Kernel))
                            {
                                if (frmInstall.ShowDialog() != DialogResult.OK)
                                {
                                    throw new POSException(POSClientResources.ACTION_CANCELED);
                                }
                                installments = (int)frmInstall.Installments;
                            }
                        }
                        int NextDocNum = header.DocumentNumber == 0 ? documentService.GetNextDocumentNumber(header, config.CurrentTerminalOid, appContext.CurrentUser.Oid) : header.DocumentNumber;
                        using (frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PROCESS_PAYMENT_ON_DEVICE))
                        {
                            dialog.btnOK.Visible = false;
                            dialog.btnCancel.Visible = false;
                            dialog.btnRetry.Visible = false;
                            dialog.CanBeClosedByUser = false;
                            dialog.Show();
                            dialog.BringToFront();
                            Application.DoEvents();
                            if (paymentMethod.IsNegative == false)
                            {
                                cardlinkresult = CardlinkLink.ExecutePayment(amount, NextDocNum, appContext.CurrentUser.POSUserName, config.TerminalID, installments, 0, cardlinkdevice.Settings.Ethernet);
                            }
                            else
                            {
                                cardlinkresult = CardlinkLink.ExecuteRefund(amount, NextDocNum, appContext.CurrentUser.POSUserName, config.TerminalID, 0, 0, cardlinkdevice.Settings.Ethernet);
                            }
                            dialog.Hide();
                            dialog.Close();
                        }
                        if (cardlinkresult == null || cardlinkresult.RespCode != "00")
                        {
                            string errorMessage = cardlinkresult?.PosMessage ?? "Error : " + POSClientResources.PAYMENT_METHOD;
                            Kernel.LogFile.Error(errorMessage);
                            throw new POSUserVisibleException(errorMessage);
                        }
                    }
                    else
                    {
                        Kernel.LogFile.Error(POSClientResources.NO_PRIMARY_CARDLINK_FOUND);
                        throw new POSUserVisibleException(POSClientResources.NO_PRIMARY_CARDLINK_FOUND);
                    }
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, ex.Message.ToString());
                    throw new POSUserVisibleException(ex.Message);

                }
            }

            DocumentPayment newPayment = new DocumentPayment(sessionManager.GetSession<DocumentPayment>());
            newPayment.Amount = paymentMethod.IsNegative && (edpsresult != null) ? -amount : amount;
            newPayment.PaymentMethod = paymentMethod.Oid;
            newPayment.PaymentMethodDescription = paymentMethod.Description;
            newPayment.PaymentMethodType = paymentMethod.PaymentMethodType;
            newPayment.IncreasesDrawerAmount = paymentMethod.IncreasesDrawerAmount;
            newPayment.CanExceedTotal = paymentMethod.CanExceedTotal;

            newPayment.DocumentHeader = header; //must go last


            foreach (KeyValuePair<Guid, object> pair in dictionary)
            {
                PaymentMethodField matchedField = paymentMethodFields.Where(x => x.Oid == pair.Key).FirstOrDefault();
                object convertedValue = Convert.ChangeType(pair.Value, typeof(DocumentPayment).GetProperty(matchedField.FieldName).PropertyType);
                newPayment.GetType().GetProperty(matchedField.FieldName).SetValue(newPayment, convertedValue, null);
            }

            if (edpsresult != null)
            {
                DocumentPaymentEdps edpsPayment = documentService.CreateDocumentPaymentEdps(edpsresult, amount, newPayment.Session);
                edpsPayment.DocumentPayment = newPayment.Oid;
                edpsPayment.DocumentHeader = header;
            }

            if (cardlinkresult != null)
            {
                DocumentPaymentCardlink cardlinkPayment = documentService.CreateDocumentPaymentCardlink(cardlinkresult, amount, newPayment.Session);
                cardlinkPayment.DocumentPayment = newPayment.Oid;
                cardlinkPayment.DocumentHeader = header;
            }

            if (actionAddPaymentParams.CouponViewModel != null)
            {
                documentService.CreateTransactionCoupon(header, actionAddPaymentParams.CouponViewModel, documentPayment: newPayment);
            }

            sessionManager.FillDenormalizedFields(newPayment);
            appContext.CurrentDocumentPayment = newPayment;
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO).Execute(new ActionPublishDocumentPaymentInfoParams(newPayment, header, true, false));
            appContext.CurrentDocument.Save();
            sessionManager.CommitTransactionsChanges();
        }
    }
}
