using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using System.Reflection;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions;
using ITS.POS.Client.Kernel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DevExpress.XtraEditors;
using ITS.POS.Model.Transactions;
using ITS.POS.Hardware.Common;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// The main input control of the application. Inherited by frmMainBase to any main form
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucPOSInput : ucObserver, IObserverSimpleMessenger, IObserverKeyListener, IPoleDisplayTextInput, IScannerInput
    {

        public static string commandText = "CMD#";

        Type[] paramsTypes = new Type[] { typeof(KeyListenerParams), typeof(MessengerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public bool waitSpecialCommand { get; set; }

        private decimal _selectedQty;
        public decimal SelectedQty
        {
            get
            {
                return this._selectedQty;
            }
            set
            {
                this._selectedQty = value;
                if (this.FindForm() != null)
                {
                    IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                    actionManager.GetAction(eActions.PUBLISH_LINE_QUANTITY_INFO).Execute(new ActionPublishLineQuantityInfoParams(this._selectedQty, ""));
                }
            }
        }
        public bool FromScanner { get; set; }


        public ucPOSInput()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.OPEN_SCANNERS);
            this.ActionsToObserve.Add(eActions.PUBLISH_KEY_PRESS);
            waitSpecialCommand = false;
        }

        public void MoveCursorToEnd()
        {
            this.edtInput.Select(edtInput.Text.Length, 0);
        }

        public override string Text
        {
            get
            {
                if (this.edtInput != null)
                {
                    return this.edtInput.Text;
                }
                return null;
            }
            set
            {
                if (this.edtInput != null)
                {
                    this.edtInput.Text = value;
                }
            }
        }

        public MethodInfo callMethod;

        // Call this method when Keys.Back is has been given
        public string BackKey(string txt)
        {
            if (!waitSpecialCommand || (txt.StartsWith(commandText) && txt.Length > commandText.Length))
            {
                return txt.Length != 0 ? txt.Remove(txt.Length - 1, 1) : "";
            }
            else
                return commandText;
        }

        public string MultiplyKey(string txt) // Multiply = '*'
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT || AppContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    string qtyStr = txt;
                    decimal qty;
                    if (!decimal.TryParse(qtyStr, out qty) || qty <= 0)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_QUANTITY));
                    }
                    else
                    {
                        SelectedQty = qty;
                    }
                }
            }
            catch (Exception e)
            {
                Kernel.LogFile.Info(e, "ucPOSInput:MultiplyKey,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
            }
            return "";
        }
        public string EnterKey(string txt)
        {
            return "";
        }

        public string ReturnKey(string txt)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IScannedCodeHandler scannedCodeHandler = Kernel.GetModule<IScannedCodeHandler>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IFormManager formManager = this.Kernel.GetModule<IFormManager>();

            try
            {

                if (waitSpecialCommand)
                {
                    waitSpecialCommand = false;
                    this.edtInput.Properties.Appearance.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
                    actionManager.GetAction(eActions.CALL_OTHER_ACTION).Execute(new ActionCallOtherActionParams(txt.Substring(commandText.Length)));
                    return "";
                }
                decimal qty = SelectedQty;

                if (deviceManager.IsDrawerOpen)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PLEASE_CLOSE_DRAWER));
                    return "";
                }

                switch (appContext.GetMachineStatus())
                {
                    case eMachineStatus.SALE:
                        if (!String.IsNullOrWhiteSpace(txt))
                        {
                            actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(true), dontCheckPermissions: true); // Πρέπει να προηγηθεί πριν την αναζήτηση οποιουδήποτε στοιχείο, καθώς
                            goto case eMachineStatus.OPENDOCUMENT;
                        }
                        break;
                    case eMachineStatus.OPENDOCUMENT:
                        {
                            bool isReturn = false;
                            if (String.IsNullOrWhiteSpace(txt) && appContext.CurrentDocumentLine != null)  //on empty enter, re-add last line
                            {
                                isReturn = appContext.CurrentDocumentLine.IsReturn;
                                txt = appContext.CurrentDocumentLine.ItemCode;
                                qty = appContext.CurrentDocumentLine.Qty;
                            }
                            using (IGetItemPriceForm form = new frmGetItemPrice(Kernel))
                            {
                                scannedCodeHandler.HandleScannedCode(form, config.GetAppSettings(), config.POSSellsInactiveItems, txt, qty, false, isReturn, this.FromScanner, false);
                            }
                            this.FromScanner = false;
                        }
                        break;
                    case eMachineStatus.OPENDOCUMENT_PAYMENT:
                        decimal amount;
                        if (!decimal.TryParse(txt, out amount))
                        {
                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT));
                        }
                        else
                        {
                            PaymentMethod defaultPaymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(config.DefaultPaymentMethodOid);
                            actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(defaultPaymentMethod, amount));
                        }
                        break;
                    case eMachineStatus.CLOSED:
                        if (appContext.CurrentDailyTotals == null)
                        {
                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.YOU_SHOULD_OPEN_FISCAL_DAY));
                        }
                        else
                        {
                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.YOU_SHOULD_START_SHIFT));
                        }
                        break;
                }
                SelectedQty = 1;
                //actionManager.GetAction(eActions.PUBLISH_LINE_QUANTITY_INFO).Execute(new ActionPublishLineQuantityInfoParams(1, ""));
                return "";
            }
            catch (POSUserVisibleException puvxception)
            {
                UtilsHelper.HardwareBeep();
                Kernel.LogFile.Debug(puvxception, "ucPOSInput:ReturnKey,POSUserVisibleException catched");
                formManager.ShowCancelOnlyMessageBox(puvxception.Message);
                return "";
            }
            catch (PriceNotFoundException priceException)
            {
                UtilsHelper.HardwareBeep();
                Kernel.LogFile.Debug(priceException, "ucPOSInput:ReturnKey,PriceNotFoundException catched");
                formManager.ShowCancelOnlyMessageBox(priceException.Message);
                return "";
            }
            catch (Exception e)
            {
                Kernel.LogFile.Trace(e, "ucPOSInput:ReturnKey,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
                return "";
            }
        }

        public string EscapeKey(string txt)
        {
            if (waitSpecialCommand)
            {
                waitSpecialCommand = false;
                this.edtInput.Properties.Appearance.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            }

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IPromotionService promotionService = Kernel.GetModule<IPromotionService>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT) // go back to open document
            {
                if (appContext.CurrentDocument.DocumentPaymentsEdps.Where(x => x.DocumentPayment != Guid.Empty).Count() > 0)
                {
                    throw new POSUserVisibleException(POSClientResources.ACTION_FORBIDDEN_DUE_TO_EDPS);
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
                                        return "";
                                    }
                                    else
                                    {
                                        CardlinkPayment.DocumentPayment = Guid.Empty;
                                        DocumentPaymentCardlink cancelingCardlink = documentService.CreateDocumentPaymentCardlink(cardlinkresult, payment.Amount, CardlinkPayment.Session);
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
                                    return "";
                                }
                            }
                        }

                    }
                }


                appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT);

                try
                {
                    documentService.ClearAppliedLoyalty(appContext.CurrentDocument);
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to clear Loyalty");
                }

                try
                {
                    OwnerApplicationSettings appSettings = config.GetAppSettings();
                    promotionService.ClearDocumentPromotions(appContext.CurrentDocument);
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to clear Document Promotions");
                }

                try
                {
                    OwnerApplicationSettings appSettings = config.GetAppSettings();
                    documentService.ClearDocumentTotalDiscounts(appContext.CurrentDocument);
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to clear Document Discounts");
                }

                try
                {
                    documentService.ClearPayments(appContext.CurrentDocument);
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to clear Document Payments");
                }

                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(appContext.CurrentDocument));
            }
            return "";
        }

        ///// <summary>
        ///// Expected Params: param1 = POSKeyMapping keyMapping
        ///// </summary>
        ///// <param name="parameters"></param>
        //public void Update(params object[] parameters)
        //{

        //}

        private void edtInput_Leave(object sender, EventArgs e)
        {
            edtInput.Focus();
            MoveCursorToEnd();
        }


        public void Update(MessengerParams parameters)
        {
            if (!String.IsNullOrEmpty(parameters.Message))
            {
                this.edtInput.Text = parameters.Message;
                if (parameters.Message.EndsWith(Environment.NewLine))
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                        POSKeyMapping keyMap = new POSKeyMapping(sessionManager.MemorySettings) { KeyData = Keys.Enter, ActionCode = eActions.NONE, NotificationType = eNotificationsTypes.KEY };
                        this.Update(new KeyListenerParams(keyMap.ActionCode, keyMap.NotificationType, keyMap.RedirectTo, keyMap.KeyData, keyMap.ActionParameters));
                    });
                }
            }
        }


        public void Update(KeyListenerParams parameters)
        {
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            IActionExecutor actionExecutor = Kernel.GetModule<IActionExecutor>();

            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams("")); //Clears the Errors Area
            if (edtInput.Text.Contains("-9-9-9-") && parameters.KeyData != Keys.Enter && parameters.KeyData != Keys.Escape && parameters.KeyData != Keys.Back && parameters.KeyData != Keys.C && parameters.KeyData != Keys.Cancel && parameters.KeyData != Keys.ControlKey)
            {
                edtInput.EditValue = edtInput.Text + UtilsHelper.KeyboardDecode(parameters.KeyData);
                MoveCursorToEnd();
                return;
            }
            if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode != eActions.NONE)
            {
                // Open UserControl
                IAction action = actionManager.GetAction(parameters.ActionCode);
                if (action == null)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(""));
                }
                else
                {
                    actionExecutor.ExecuteAction(parameters.ActionCode, parameters.ActionParameters);
                }
                return;
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY && parameters.RedirectTo != Keys.None) //Redirect key detected, Call update with the new key
            {
                this.Update(new KeyListenerParams(eActions.NONE, eNotificationsTypes.KEY, Keys.None, parameters.RedirectTo, parameters.ActionParameters));
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY)// && keyMapping.ActionCode == eActions.KEYBOARD)
            {
                //if (!parameters.KeyData.GetType().Equals(typeof(Keys))) return;
                //if (!edtInput.Focused)
                //{
                //    //edtInput.Focus();
                //}
                callMethod = this.GetType().GetMethod(parameters.KeyData.ToString() + "Key");
                if (callMethod != null)
                {
                    edtInput.EditValue = callMethod.Invoke(this, new object[] { edtInput.Text }) as string;
                }
                else
                {
                    edtInput.EditValue = edtInput.Text + UtilsHelper.KeyboardDecode(parameters.KeyData);

                }
                MoveCursorToEnd();
                //edtInput
                return;
            }

        }

        public string poleDisplayName;
        public string PoleDisplayName
        {
            get
            {
                return poleDisplayName;
            }
            set
            {
                poleDisplayName = value;
            }
        }

        public void AttachTextChangedEvent(EventHandler handler)
        {
            this.edtInput.TextChanged += handler;
        }

        public void DetachTextChangedEvent(EventHandler handler)
        {
            this.edtInput.TextChanged -= handler;
        }


        public string GetText()
        {
            return this.edtInput.Text;
        }

        public void SetText(string text)
        {
            try
            {
                this.edtInput.Invoke((MethodInvoker)delegate ()
                {
                    this.edtInput.Text += text;
                });
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error setting Text, input:" + this.Name);
            }
        }


        public void SendEnter()
        {
            try
            {
                this.edtInput.Invoke((MethodInvoker)delegate ()
                {
                    SendKeys.SendWait("~");
                });
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error sending Enter, input:" + this.Name);
            }
        }

        private void edtInput_Enter(object sender, EventArgs e)
        {
            MoveCursorToEnd();
        }
    }
}
