using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmFiscalPrinterCommands : frmDialogBase
    {
        public frmFiscalPrinterCommands(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            nbargrpActions.Caption = POSClientResources.READ_FISCAL_MEMORY;
            btnExit.Text = POSClientResources.CLOSE;

            itmAnalyticPerZ.Caption = POSClientResources.ANALYTIC + " - " + POSClientResources.BY_Z_NUMBER;
            itmAnalyticPerDate.Caption = POSClientResources.ANALYTIC + " - " + POSClientResources.BY_DATE;
            itmSignaturePerDate.Caption = POSClientResources.SIGNATURES + " - " + POSClientResources.BY_DATE;
            itmSignaturePerZ.Caption = POSClientResources.SIGNATURES + " - " + POSClientResources.BY_Z_NUMBER;
            itmSumPerZ.Caption = POSClientResources.SUMMARIZED + " - " + POSClientResources.BY_Z_NUMBER;
            itmSumPerDate.Caption = POSClientResources.SUMMARIZED + " - " + POSClientResources.BY_DATE;
            itmPerBlock.Caption = POSClientResources.PER_BLOCK;
        }

        private void Execute(bool useDateFilter, eReprintZReportsMode mode)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                actionManager.GetAction(eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS).Execute(new ActionFiscalPrinterReprintZReportsParams(useDateFilter, mode), true);
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "ReadFiscalMemoryButton:OnButtonClick,Exception catched");
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }

        private void itmAnalyticPerZ_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(false, eReprintZReportsMode.ANALYTIC);
        }

        private void itmSumPerZ_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(false, eReprintZReportsMode.SUMMARIZED);
        }

        private void itmSignaturePerZ_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(false, eReprintZReportsMode.SIGNATURES);
        }

        private void itmAnalyticPerDate_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(true, eReprintZReportsMode.ANALYTIC);
        }

        private void itmSumPerDate_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(true, eReprintZReportsMode.SUMMARIZED);
        }

        private void itmSignaturePerDate_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Execute(true, eReprintZReportsMode.SIGNATURES);
        }

        private void itmPerBlock_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            try
            {
                actionManager.GetAction(eActions.FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS).Execute(dontCheckPermissions: true);
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "ReadFiscalMemoryButton:OnButtonClick,Exception catched");
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }
    }
}
