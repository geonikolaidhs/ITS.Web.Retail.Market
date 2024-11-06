using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmCashCountSummary : frmInputFormBase
    {
        public frmCashCountSummary(IPosKernel kernel, BindingList<CashCountFinalObject> Amounts) : base(kernel)
        {
            InitializeComponent();
            grdMain.DataSource = Amounts;
            lblSummary.Text = POSClientResources.CASH_COUNT_SUMMARY_TEXT;
            this.btnOk2.Text = POSClientResources.OK;
            this.btnCancel2.Text = POSClientResources.CANCEL;
            this.KeyDown -= this.frmDialogBaseKeyDownHandler;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCashCount_KeyDownz);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }
        private void frmCashCount_KeyDownz(object sender, KeyEventArgs e)
        {
            try
            {
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                ISynchronizationManager sync = Kernel.GetModule<ISynchronizationManager>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                try
                {
                    if (sync.UpdateKeyMappings)
                    {
                        sync.UpdateKeyMappings = false;
                        UtilsHelper.InitKeyMaps(config.CurrentTerminalOid, sessionManager);
                    }
                    Keys keyData = e.KeyData;
                    if (!keyData.ToString().Contains("Shift")) keyData = keyData | Keys.Shift;
                    if (!keyData.ToString().Contains("Control")) keyData = keyData | Keys.Control;
                    if (!keyData.ToString().Contains("Alt")) keyData = keyData | Keys.Alt;
                    if (UtilsHelper.hKeyMap.ContainsKey(keyData.ToString()))
                    {
                        POSKeyMapping x;
                        UtilsHelper.hKeyMap.TryGetValue(keyData.ToString(), out x);
                        switch (x.ActionCode)
                        {
                            case eActions.CASH_COUNT_SUBMIT:
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                break;
                            case eActions.CASH_COUNT_CANCEL:
                                this.DialogResult = DialogResult.Cancel;
                                this.Close();
                                break;
                            default:
                                break;
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
                catch { }
            }
            catch { }
        }
    }
}
