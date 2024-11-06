using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.UserControls;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Resources;
using System.Threading;
using ITS.POS.Client.ObserverPattern;

using ITS.POS.Client.Receipt;
using ITS.POS.Client.Exceptions;
using ITS.POS.Model.Master;
using DevExpress.Xpo;

namespace ITS.POS.Client.Forms
{
    public partial class frmLogin : frmInputFormBase, IObserverContainer
    {
        private eMachineStatus previousMachineStatus;
        public frmLogin(IPosKernel kernel)
            : base(kernel)
        {
            InitializeComponent();
            labelControl1.Text = POSClientResources.USERNAME.ToUpperGR();
            labelControl2.Text = POSClientResources.PASSWORD.ToUpperGR();
            lblStartingAmount.Text = POSClientResources.STARTING_AMOUNT.ToUpperGR();

            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            if (config.AsksForStartingAmount == false)
            {
                lblStartingAmount.Visible = false;
                teStartingAmount.Visible = false;
                this.Height = this.Height - teStartingAmount.Height;
            }
            //teStartingAmount.Text = TotalizorsHelper.GetTotalCashInPos(GlobalContext.CurrentDailyTotals).ToString("c");

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            previousMachineStatus = appContext.GetMachineStatus();
            appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT);
        }

        private void teUsername_KeyDown(object sender, KeyEventArgs e)
        {
            //&& e.KeyCode != Keys.Back 
            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Back && e.KeyCode != Keys.Delete && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
            {
                //only numbers are allowed
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                tePassword.Focus();
            }
        }

        private void tePassword_KeyDown(object sender, KeyEventArgs e)
        {
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            try
            {
                if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Back && e.KeyCode != Keys.Delete && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                {
                    //only numbers are allowed
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (config.AsksForStartingAmount)
                    {
                        teStartingAmount.Focus();
                    }
                    else
                    {
                        StartShift();
                    }
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmLogin:tePassword_KeyDown,Exception catched");
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                teUsername.Focus();
            }
        }

        private void teStartingAmount_KeyDown(object sender, KeyEventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Back && e.KeyCode != Keys.Delete && e.KeyCode != Keys.Oemcomma && e.KeyCode != Keys.OemPeriod
                    && e.KeyCode != Keys.Decimal && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                {
                    //only numbers are allowed
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    StartShift();
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmLogin:teStartingAmount_KeyDown,Exception catched");
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                teUsername.Focus();
            }
        }

        private void StartShift()
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            User user = sessionManager.FindObject<User>(CriteriaOperator.And(new BinaryOperator("POSUserName", teUsername.EditValue.ToString()),
                                                                            new BinaryOperator("POSPassword", tePassword.EditValue.ToString())));

            DocumentType depositDocType = sessionManager.GetObjectByKey<DocumentType>(config.DepositDocumentTypeOid);

            bool invalidUser = user == null || teUsername.EditValue == null || String.IsNullOrWhiteSpace(teUsername.EditValue.ToString());
            string errorMessage = POSClientResources.USER_NOT_FOUND;
            if (!invalidUser)
            {
                XPCollection<UserTypeAccess> accesses = new XPCollection<UserTypeAccess>(sessionManager.GetSession<UserTypeAccess>(),
                    CriteriaOperator.And(
                        new BinaryOperator("User", user.Oid),
                        new BinaryOperator("EntityOid", config.CurrentStoreOid),
                        new BinaryOperator("EntityType", "ITS.Retail.Model.Store")
                    ));
                if (accesses.Count == 0)
                {
                    invalidUser = true;
                    errorMessage = POSClientResources.USER_HAS_NO_STORE_ACCESS;
                }
            }
            if (invalidUser)
            {
                formManager.ShowMessageBox(errorMessage);
                teUsername.Text = "";
                tePassword.Text = "";
                if (teStartingAmount.Visible)
                {
                    teStartingAmount.Text = "0,00 €";
                }
                teUsername.Focus();
                return;
            }
            else
            {
                decimal value = 0;
                if (config.AsksForStartingAmount)
                {
                    value = decimal.Parse(teStartingAmount.Text.Replace(config.CurrencySymbol, "").Trim());
                    decimal limit = depositDocType.MaxDetailValue > 0 ? depositDocType.MaxDetailValue : int.MaxValue;
                    if (value >= limit)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.AMOUNT_EXCEED_LIMIT);
                        return;
                    }
                }

                appContext.CurrentUser = user;

                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                    if (printer != null)
                    {
                        string reason;
                        bool canPrintIllegal = printer.CheckIfCanPrintIllegal(out reason);

                        if (canPrintIllegal == false)
                        {
                            throw new POSException(reason);
                        }

                        printer.SetCashierID(user.POSUserName);
                        string shiftStarted = POSClientResources.SHIFT_STARTED.ToUpperGR();
                        string userConnected = POSClientResources.CASHIER_CONNECTED.ToUpperGR() + ": " + user.POSUserName;
                        List<string> shiftStartLines = receiptBuilder.CreateFiscalVersionLines(deviceManager.GetVisibleVersion(config.FiscalMethod), printer.Settings.LineChars, shiftStarted, userConnected);
                        printer.PrintIllegal(shiftStartLines.Select(x => new FiscalLine() { Value = x }).ToArray());
                    }
                    else
                    {
                        throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                    }
                }

                appContext.CurrentUserDailyTotals = totalizersService.CreateUserDailtyTotals(appContext.CurrentUser.Oid, appContext.CurrentDailyTotals.Oid,
                   appContext.CurrentUserDailyTotals == null ? Guid.Empty : appContext.CurrentUserDailyTotals.Oid, config.CurrentTerminalOid, config.CurrentStoreOid);

                DocumentHeader movementHeader = null;
                try
                {
                    movementHeader = CreateStartingAmountDepositOrWithdraw(value);
                    if (movementHeader != null)
                    {
                        movementHeader.Save();
                        sessionManager.CommitTransactionsChanges();
                        totalizersService.UpdateTotalizers(config, movementHeader, appContext.CurrentUser, appContext.CurrentDailyTotals, appContext.CurrentUserDailyTotals,
                            config.CurrentStoreOid, config.CurrentTerminalOid, false, false, eTotalizorAction.INCREASE);
                    }

                    appContext.SetMachineStatus(eMachineStatus.SALE);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                    formManager.ShowForm<frmMainBase>(this, true);
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(""));
                }
                catch (Exception ex)
                {
                    appContext.CurrentUser = null;
                    if (movementHeader != null)
                    {
                        movementHeader.Delete();
                    }

                    if (appContext.CurrentUserDailyTotals != null)
                    {
                        appContext.CurrentUserDailyTotals.Delete();
                        appContext.CurrentUserDailyTotals = null;
                    }

                    sessionManager.CommitTransactionsChanges();
                    formManager.ShowCancelOnlyMessageBox(ex.Message);
                    Kernel.LogFile.Error(ex, "Error creating starting amount deposit or withdraw");
                }
                return;
            }
        }

        private DocumentHeader CreateStartingAmountDepositOrWithdraw(decimal startingValue)
        {
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            Guid docType = Guid.Empty;
            Guid docSeries = Guid.Empty;
            SpecialItem item = null;
            eOpenDrawerMode mode = eOpenDrawerMode.NONE;
            string store = sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid).Name;
            //PriceCatalog pc = customerService.GetPriceCatalog(config.DefaultCustomerOid, config.CurrentStoreOid);
            PriceCatalogPolicy priceCatalogPolicy = customerService.GetPriceCatalogPolicy(config.DefaultCustomerOid, config.CurrentStoreOid);
            bool isDayStartingAmount = false;
            bool isShiftStartingAmount = false;
            string customDescription = "";
            decimal movementAmount = 0;

            if (priceCatalogPolicy == null)
            {
                throw new Exception(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
            }

            if (appContext.CurrentDailyTotals.UserDailyTotalss.Count == 1)
            {
                ////Starting Amount of the day
                docType = config.DepositDocumentTypeOid;
                docSeries = config.DepositDocumentSeriesOid;

                item = sessionManager.GetObjectByKey<SpecialItem>(config.DepositItemOid);
                mode = eOpenDrawerMode.DEPOSIT;
                isDayStartingAmount = true;
                movementAmount = startingValue;
            }
            else
            {
                movementAmount = startingValue;// totalizersService.GetTotalCashInPos(appContext.CurrentDailyTotals) - startingValue;
                                               //if (movementAmount >= 0)
                                               //{

                //    docType = config.WithdrawalDocumentTypeOid;
                //    docSeries = config.WithdrawalDocumentSeriesOid;
                //    item = sessionManager.GetObjectByKey<SpecialItem>(config.WithdrawalItemOid);
                //    mode = eOpenDrawerMode.WITHDRAW;
                //}
                //else
                //{
                docType = config.DepositDocumentTypeOid;
                docSeries = config.DepositDocumentSeriesOid;
                item = sessionManager.GetObjectByKey<SpecialItem>(config.DepositItemOid);
                mode = eOpenDrawerMode.DEPOSIT;
                //movementAmount = movementAmount * (-1);
                //}

                isShiftStartingAmount = true;
            }

            if (item == null || docType == Guid.Empty || docSeries == Guid.Empty)
            {
                string message = mode + " " + POSClientResources.ERROR + ": " + POSClientResources.INVALID_SETTINGS;// " error: Invalid settings.";
                throw new POSException(message);
            }

            DocumentType type = sessionManager.GetObjectByKey<DocumentType>(docType);
            if (type != null)
            {
                if ((mode == eOpenDrawerMode.WITHDRAW && type.ValueFactor >= 0) ||
                    (mode == eOpenDrawerMode.DEPOSIT && type.ValueFactor <= 0))
                {
                    string message = mode + " " + POSClientResources.ERROR + ": " + POSClientResources.INVALID_SETTINGS + Environment.NewLine
                        + (mode == eOpenDrawerMode.WITHDRAW ? POSClientResources.VALUE_FACTOR_MUST_BE_NEGATIVE : POSClientResources.VALUE_FACTOR_MUST_BE_POSITIVE);

                    throw new POSException(message);
                }
            }

            string headerLine = "*" + POSClientResources.STARTING_AMOUNT.ToUpperGR() + "*";

            DocumentHeader header = null;
            if (movementAmount > 0)
            {
                header = documentService.CreateWithdrawOrDeposit(appContext.CurrentUser, appContext.CurrentUserDailyTotals, docType, docSeries, movementAmount, item, priceCatalogPolicy.Oid, customDescription, isShiftStartingAmount, isDayStartingAmount);
            }
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                FiscalPrinter fiscalPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();

                if (fiscalPrinter != null)
                {
                    try
                    {
                        if (config.AsksForStartingAmount)
                        {
                            List<string> lines = receiptBuilder.CreateWithdrawOrDepositLines(fiscalPrinter, appContext.CurrentUser, headerLine, startingValue, store, receiptBuilder, config);
                            fiscalPrinter.PrintIllegal(lines.Select(line => new FiscalLine() { Type = ePrintType.NORMAL, Value = line }).ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.PRINTER_FAILURE + " " + ex.Message);
                        Kernel.LogFile.Error(ex, "Error durring fiscal printer " + mode);
                    }
                }
                else
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }
            }
            else if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                Printer printer = deviceManager.GetPrimaryDevice<Printer>();
                if (printer != null)
                {
                    try
                    {
                        if (config.AsksForStartingAmount)
                        {
                            List<string> lines = receiptBuilder.CreateWithdrawOrDepositLines(printer, appContext.CurrentUser, headerLine, startingValue, store, receiptBuilder, config);
                            DeviceResult printerResult = printer.PrintLines(lines.ToArray());
                            if (printerResult != DeviceResult.SUCCESS)
                            {
                                string message = POSClientResources.PRINTER_FAILURE + ": " + printerResult.ToLocalizedString();
                                printer.EndTransaction();
                                throw new POSException(message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.PRINTER_FAILURE + " " + ex.Message);
                        Kernel.LogFile.Error(ex, "Error durring printer " + mode);
                        throw;
                    }
                }
                else
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }
            }

            return header;
        }


        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IEnumerable<KeyLock> indirectKeylocks = deviceManager.Devices.Where(x => typeof(KeyLock).IsAssignableFrom(x.GetType()) && x.ConType == ConnectionType.INDIRECT).Cast<KeyLock>();

            bool keyHandled = false;
            foreach (KeyLock keylock in indirectKeylocks)
            {
                keyHandled = keylock.IndirectConnectionHandleKey(e);
                if (keyHandled)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }

            if (e.Handled)
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private bool _subscriptionsAreInitialized;
        public bool SubscriptionsAreInitialized
        {
            get { return _subscriptionsAreInitialized; }
        }

        public void IntitializeSubscriptions()
        {

            if (!SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (typeof(IObserver).IsAssignableFrom(control.GetType()))
                    {
                        ((IObserver)control).InitializeActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = true;
            }
        }

        public void DropSubscriptions()
        {
            if (SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (control is ucObserver)
                    {
                        (control as ucObserver).DropActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = false;
            }
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            tePassword.HideTouchPad();
            teUsername.HideTouchPad();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
            {
                appContext.SetMachineStatus(previousMachineStatus);
            }
        }


    }
}