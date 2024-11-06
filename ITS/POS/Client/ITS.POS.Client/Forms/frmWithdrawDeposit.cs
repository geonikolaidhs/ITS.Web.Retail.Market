using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.POS.Client.Exceptions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Client.UserControls;

namespace ITS.POS.Client.Forms
{
    public partial class frmWithdrawDeposit : frmInputFormBase
    {
        private eOpenDrawerMode mode;
        public bool FormClosedWithError { get; set; }

        public frmWithdrawDeposit(eOpenDrawerMode drawerMode, IPosKernel kernel, string customTitle = null)
            : base(kernel)
        {
            this.mode = drawerMode;
            InitializeComponent();
            //Localization
            this.btnOK.Text = POSClientResources.OK;
            this.btnCancel.Text = POSClientResources.CANCEL;
            this.labelControl1.Text = POSClientResources.AMOUNT;

            Guid docTypeGuid = Guid.Empty;

            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            switch (mode)
            {
                case eOpenDrawerMode.DEPOSIT:
                    this.lblTitle.Text = POSClientResources.DEPOSIT;
                    docTypeGuid = config.DepositDocumentTypeOid;
                    break;
                case eOpenDrawerMode.WITHDRAW:
                    this.lblTitle.Text = POSClientResources.WITHDRAW;
                    docTypeGuid = config.WithdrawalDocumentTypeOid;
                    break;
            }

            DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(docTypeGuid);
            listBoxReasons.DataSource = new XPCollection<Reason>(sessionManager.GetSession<Reason>(), new BinaryOperator("Category", docType.ReasonCategory));
            listBoxReasons.ValueMember = "Oid";
            listBoxReasons.DisplayMember = "CustomDescription";

            if (customTitle != null)
            {
                this.lblTitle.Text = customTitle;
            }

            this.txtInput.Focus();
        }

        new public void Close()
        {
            base.Close();
            this.txtInput.HideTouchPad();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtInput.Text = "0";
            this.Close();
        }



        private bool processingOkButtonClick;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (processingOkButtonClick == false)
            {


                processingOkButtonClick = true;
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
                ICustomerService customerService = Kernel.GetModule<ICustomerService>();
                IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();
                IDocumentService documentService = Kernel.GetModule<IDocumentService>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

                try
                {
                    Guid docTypeOid = Guid.Empty;
                    Guid docSeriesOid = Guid.Empty;
                    SpecialItem item = null;
                    string customDescription = "";
                    string store = sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid).Name;
                    if (listBoxReasons.SelectedItem == null)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.REASON_MISSING);
                        this.FormClosedWithError = true;
                        processingOkButtonClick = false;
                        return;
                    }

                    switch (this.mode)
                    {
                        case eOpenDrawerMode.WITHDRAW:
                            docTypeOid = config.WithdrawalDocumentTypeOid;
                            docSeriesOid = config.WithdrawalDocumentSeriesOid;
                            item = sessionManager.GetObjectByKey<SpecialItem>(config.WithdrawalItemOid);
                            break;
                        case eOpenDrawerMode.DEPOSIT:
                            docTypeOid = config.DepositDocumentTypeOid;
                            docSeriesOid = config.DepositDocumentSeriesOid;
                            item = sessionManager.GetObjectByKey<SpecialItem>(config.DepositItemOid);
                            break;
                    }

                    if (item == null || docTypeOid == Guid.Empty || docSeriesOid == Guid.Empty)
                    {
                        string message = this.mode + " " + POSClientResources.ERROR + ": " + POSClientResources.INVALID_SETTINGS;// " error: Invalid settings.";
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                        this.FormClosedWithError = true;
                        processingOkButtonClick = false;
                        this.Close();
                        return;
                    }

                    DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(docTypeOid);
                    if (docType != null)
                    {
                        if ((this.mode == eOpenDrawerMode.WITHDRAW && docType.ValueFactor >= 0) ||
                            (this.mode == eOpenDrawerMode.DEPOSIT && docType.ValueFactor <= 0))
                        {
                            string message = this.mode + " " + POSClientResources.ERROR + ": " + POSClientResources.INVALID_SETTINGS + Environment.NewLine
                                + (this.mode == eOpenDrawerMode.WITHDRAW ? POSClientResources.VALUE_FACTOR_MUST_BE_NEGATIVE : POSClientResources.VALUE_FACTOR_MUST_BE_POSITIVE);

                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                            this.FormClosedWithError = true;
                            processingOkButtonClick = false;
                            this.Close();
                            return;
                        }
                    }

                    decimal value = decimal.Parse(txtInput.Text.Replace(config.CurrencySymbol, "").Trim());
                    decimal limit = docType.MaxDetailValue > 0 ? docType.MaxDetailValue : int.MaxValue;

                    if (value <= 0)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.ZERO_AMOUNT_IS_NOT_ALLOWED);
                        this.FormClosedWithError = true;
                        processingOkButtonClick = false;
                        return;
                    }
                    else if (value >= limit)
                    {
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.AMOUNT_EXCEED_LIMIT);
                        this.FormClosedWithError = true;
                        processingOkButtonClick = false;
                        return;
                    }

                    if (this.mode == eOpenDrawerMode.WITHDRAW)
                    {
                        decimal totalCash = totalizersService.GetTotalCashInPos(appContext.CurrentDailyTotals);
                        if (totalCash < value)
                        {
                            formManager.ShowCancelOnlyMessageBox(POSClientResources.NOT_ENOUGH_CASH);
                            this.FormClosedWithError = true;
                            processingOkButtonClick = false;
                            return;
                        }
                    }

                    //PriceCatalog pc = customerService.GetPriceCatalog(config.DefaultCustomerOid, config.CurrentStoreOid);
                    PriceCatalogPolicy priceCatalogPolicy = customerService.GetPriceCatalogPolicy(config.DefaultCustomerOid, config.CurrentStoreOid);
                    if (priceCatalogPolicy == null)
                    {
                        throw new Exception(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
                    }

                    string headerLine = "";
                    switch (mode)
                    {
                        case eOpenDrawerMode.WITHDRAW:
                            headerLine = POSClientResources.WITHDRAW.ToUpperGR();
                            break;
                        case eOpenDrawerMode.DEPOSIT:
                            headerLine = POSClientResources.DEPOSIT.ToUpperGR();
                            break;
                    }

                    if (config.FiscalMethod == eFiscalMethod.ADHME)
                    {
                        FiscalPrinter fiscalPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();

                        if (fiscalPrinter != null)
                        {
                            List<string> allTheLines = new List<string>();
                            allTheLines.AddRange(receiptBuilder.CreateWithdrawOrDepositLines(fiscalPrinter, appContext.CurrentUser, headerLine, value, store, receiptBuilder, config, (Reason)listBoxReasons.SelectedItem, txtComment.Text, txtTaxCode.Text));
                            fiscalPrinter.PrintIllegal(allTheLines.Select(line => new FiscalLine() { Type = ePrintType.NORMAL, Value = line }).ToArray());
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
                            List<string> allTheLines = new List<string>();
                            allTheLines.AddRange(receiptBuilder.CreateWithdrawOrDepositLines(printer, appContext.CurrentUser, headerLine, value, store, receiptBuilder, config, (Reason)listBoxReasons.SelectedItem, txtComment.Text, txtTaxCode.Text));
                            DeviceResult printerResult = printer.PrintLines(allTheLines.ToArray());
                            if (printerResult != DeviceResult.SUCCESS)
                            {
                                string message = POSClientResources.PRINTER_FAILURE + ": " + printerResult.ToLocalizedString();
                                printer.EndTransaction();
                                throw new POSException(message);
                            }
                        }
                        else
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }
                    }

                    DocumentHeader header = documentService.CreateWithdrawOrDeposit(appContext.CurrentUser, appContext.CurrentUserDailyTotals, docTypeOid, docSeriesOid, value, item, priceCatalogPolicy.Oid, customDescription, false, false, (Guid)listBoxReasons.SelectedValue, txtComment.Text, txtTaxCode.Text);
                    header.Save();
                    sessionManager.CommitTransactionsChanges();
                    totalizersService.UpdateTotalizers(config, header, appContext.CurrentUser, appContext.CurrentDailyTotals, appContext.CurrentUserDailyTotals,
                                                        config.CurrentStoreOid, config.CurrentTerminalOid, false, false, eTotalizorAction.INCREASE);
                    Close();
                }
                catch (Exception exception)
                {
                    Kernel.LogFile.Error(exception, "frmWithdrawDeposit:btnOkClick:Exception");
                    //actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                    formManager.ShowCancelOnlyMessageBox(exception.GetFullMessage());
                    this.FormClosedWithError = true;
                }
                finally
                {
                    processingOkButtonClick = false;
                }
            }
        }

        private void frmWithdrawDeposit_FormClosed(object sender, FormClosedEventArgs e)
        {
            txtInput.HideTouchPad();
        }

        private void btnNextReason_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxReasons.SelectedIndex;
            if (selectedIndex < (listBoxReasons.DataSource as IEnumerable<Reason>).Count() - 1)
            {
                listBoxReasons.SelectedIndex = ++selectedIndex;
            }
        }

        private void btnPreviousReason_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxReasons.SelectedIndex;
            if ((listBoxReasons.DataSource as IEnumerable<Reason>).Count() > 0 && selectedIndex > 0)
            {
                listBoxReasons.SelectedIndex = --selectedIndex;
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnOK_Click(sender, e);
            }
        }

        private void listBoxReasons_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.txtTaxCode.Focus();
            }
        }
    }
}
