using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Synchronization;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Hardware.Common;

namespace ITS.POS.Client
{
    public partial class frmMenu : frmDialogBase
    {
        public frmMenu(IPosKernel kernel) : base(kernel)
        {

            InitializeComponent();

            this.btnExit.Text = POSClientResources.CLOSE.ToUpper();
            this.nbargrpActions.Caption = POSClientResources.ACTIONS;
            this.nbaritemShutDown.Caption = POSClientResources.SHUT_DOWN;
            this.nbaritmKeyboard.Caption = POSClientResources.KEYBOARD;
            this.nbaritmConfiguration.Caption = POSClientResources.CONFIGURATION;
            this.nbargrpFiscalPrinterCommands.Caption = POSClientResources.FISCAL_PRINTER_COMMANDS;
            this.nBarItemReadFiscalMemory.Caption = POSClientResources.READ_FISCAL_MEMORY;
            this.navBarCardLink.Caption = POSClientResources.CARDLINK_COMMUNICATION_CHECK;

            this.Text = POSClientResources.MAIN_MENU;//"Main Menu";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void nbaritemShutDown_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Program.ApplicationExit();
        }

        private void navBarControl1_Click(object sender, EventArgs e)
        {

        }

        private void nbaritmConfiguration_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

            //using ( frmConfig configForm = new frmConfig() )
            //{
            //    configForm.ShowDialog();
            //}

        }

        private void nbaritmKeyboard_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

            //Action keys = GlobalContext.GetAction(eActions.KEYMAPPINGS);
            //keys.Execute();


        }

        private void nBarItemFiscalPrinter_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            using (frmElevatePrivileges form = new frmElevatePrivileges(eKeyStatus.POSITION3, Kernel))
            {
                DialogResult result = form.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    using (frmFiscalPrinterCommands cmdFrm = new frmFiscalPrinterCommands(Kernel))
                    {
                        cmdFrm.ShowDialog();
                    }

                }
            }
        }

        private void nBarItemServiceForcedCancel_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.SERVICE_FORCED_CANCEL_DOCUMENT).Execute(new ActionServiceForcedCancelDocumentParams(true));
        }

        private void nBarItemForcedStartDay_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.SERVICE_FORCED_START_DAY_FISCAL_PRINTER).Execute();
        }

        private void nBarItemRestorePrinter_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.SERVICE_RESTORE_FISCAL_PRINTER).Execute();
        }

        private void nBarItemShowVATFactors_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.DISPLAY_VAT_FACTORS).Execute();
        }

        private void nBarItemEDPSCheckCommunication_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.EDPS_CHECK_COMMUNICATION).Execute();
        }

        private void nBarItemDbMaintenance_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.DATABASE_MAINTENANCE).Execute();
        }

        private void nBarItemDbMaintenanceLight_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.DATABASE_MAINTENANCE_LIGHT).Execute();
        }

        private void navBarCardLink_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.CARDLINK_CHECK_COMMUNICATION).Execute();
        }
    }
}