namespace ITS.POS.Client
{
    partial class frmMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lyctrlMenu = new DevExpress.XtraLayout.LayoutControl();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.nbargrpActions = new DevExpress.XtraNavBar.NavBarGroup();
            this.nbaritemShutDown = new DevExpress.XtraNavBar.NavBarItem();
            this.nbargrpFiscalPrinterCommands = new DevExpress.XtraNavBar.NavBarGroup();
            this.nBarItemReadFiscalMemory = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemServiceForcedCancel = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemForcedStartDay = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemShowVATFactors = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemEDPSCheckCommunication = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemDbMaintenance = new DevExpress.XtraNavBar.NavBarItem();
            this.nBarItemDbMaintenanceLight = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarCardLink = new DevExpress.XtraNavBar.NavBarItem();
            this.nbaritmKeyboard = new DevExpress.XtraNavBar.NavBarItem();
            this.nbaritmConfiguration = new DevExpress.XtraNavBar.NavBarItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarItem2 = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarItem3 = new DevExpress.XtraNavBar.NavBarItem();
            ((System.ComponentModel.ISupportInitialize)(this.lyctrlMenu)).BeginInit();
            this.lyctrlMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // lyctrlMenu
            // 
            this.lyctrlMenu.Controls.Add(this.btnExit);
            this.lyctrlMenu.Controls.Add(this.navBarControl1);
            this.lyctrlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lyctrlMenu.Location = new System.Drawing.Point(0, 0);
            this.lyctrlMenu.Name = "lyctrlMenu";
            this.lyctrlMenu.Root = this.layoutControlGroup1;
            this.lyctrlMenu.Size = new System.Drawing.Size(302, 426);
            this.lyctrlMenu.TabIndex = 0;
            this.lyctrlMenu.Text = "layoutControl1";
            // 
            // btnExit
            // 
            this.btnExit.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.btnExit.Appearance.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnExit.Appearance.Options.UseFont = true;
            this.btnExit.Appearance.Options.UseImage = true;
            this.btnExit.Appearance.Options.UseTextOptions = true;
            this.btnExit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExit.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnExit.Location = new System.Drawing.Point(12, 376);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(278, 38);
            this.btnExit.StyleController = this.lyctrlMenu;
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "C L O S E";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.nbargrpActions;
            this.navBarControl1.Appearance.GroupHeader.Font = new System.Drawing.Font("Tahoma", 14F);
            this.navBarControl1.Appearance.GroupHeader.Options.UseFont = true;
            this.navBarControl1.Appearance.GroupHeaderActive.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.navBarControl1.Appearance.GroupHeaderActive.Options.UseFont = true;
            this.navBarControl1.Appearance.GroupHeaderHotTracked.BorderColor = System.Drawing.SystemColors.HotTrack;
            this.navBarControl1.Appearance.GroupHeaderHotTracked.Font = new System.Drawing.Font("Tahoma", 14F);
            this.navBarControl1.Appearance.GroupHeaderHotTracked.Options.UseBorderColor = true;
            this.navBarControl1.Appearance.GroupHeaderHotTracked.Options.UseFont = true;
            this.navBarControl1.Appearance.GroupHeaderPressed.Font = new System.Drawing.Font("Tahoma", 14F);
            this.navBarControl1.Appearance.GroupHeaderPressed.Options.UseFont = true;
            this.navBarControl1.Appearance.Item.Font = new System.Drawing.Font("Tahoma", 11F);
            this.navBarControl1.Appearance.Item.Options.UseFont = true;
            this.navBarControl1.Appearance.ItemActive.Font = new System.Drawing.Font("Tahoma", 11F);
            this.navBarControl1.Appearance.ItemActive.Options.UseFont = true;
            this.navBarControl1.Appearance.ItemDisabled.Font = new System.Drawing.Font("Tahoma", 11F);
            this.navBarControl1.Appearance.ItemDisabled.Options.UseFont = true;
            this.navBarControl1.Appearance.ItemHotTracked.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.navBarControl1.Appearance.ItemHotTracked.Font = new System.Drawing.Font("Tahoma", 11F);
            this.navBarControl1.Appearance.ItemHotTracked.Options.UseBorderColor = true;
            this.navBarControl1.Appearance.ItemHotTracked.Options.UseFont = true;
            this.navBarControl1.Appearance.ItemPressed.Font = new System.Drawing.Font("Tahoma", 11F);
            this.navBarControl1.Appearance.ItemPressed.Options.UseFont = true;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbargrpActions,
            this.nbargrpFiscalPrinterCommands});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.nbaritmKeyboard,
            this.nbaritemShutDown,
            this.nbaritmConfiguration,
            this.nBarItemReadFiscalMemory,
            this.nBarItemServiceForcedCancel,
            this.nBarItemForcedStartDay,
            this.nBarItemShowVATFactors,
            this.nBarItemEDPSCheckCommunication,
            this.nBarItemDbMaintenance,
            this.nBarItemDbMaintenanceLight,
            this.navBarCardLink});
            this.navBarControl1.Location = new System.Drawing.Point(12, 12);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 278;
            this.navBarControl1.Size = new System.Drawing.Size(278, 360);
            this.navBarControl1.TabIndex = 4;
            this.navBarControl1.Text = "navbarctlMenu";
            this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.StandardSkinExplorerBarViewInfoRegistrator("Metropolis");
            this.navBarControl1.Click += new System.EventHandler(this.navBarControl1_Click);
            // 
            // nbargrpActions
            // 
            this.nbargrpActions.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.nbargrpActions.Appearance.Options.UseFont = true;
            this.nbargrpActions.Caption = "Actions";
            this.nbargrpActions.Expanded = true;
            this.nbargrpActions.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nbaritemShutDown)});
            this.nbargrpActions.Name = "nbargrpActions";
            // 
            // nbaritemShutDown
            // 
            this.nbaritemShutDown.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.nbaritemShutDown.Appearance.Options.UseFont = true;
            this.nbaritemShutDown.Caption = "Shut Down";
            this.nbaritemShutDown.Name = "nbaritemShutDown";
            this.nbaritemShutDown.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbaritemShutDown_LinkClicked);
            // 
            // nbargrpFiscalPrinterCommands
            // 
            this.nbargrpFiscalPrinterCommands.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.nbargrpFiscalPrinterCommands.Appearance.Options.UseFont = true;
            this.nbargrpFiscalPrinterCommands.Caption = "Fiscal Printer Commands";
            this.nbargrpFiscalPrinterCommands.Expanded = true;
            this.nbargrpFiscalPrinterCommands.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemReadFiscalMemory),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemServiceForcedCancel),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemForcedStartDay),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemShowVATFactors),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemEDPSCheckCommunication),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemDbMaintenance),
            new DevExpress.XtraNavBar.NavBarItemLink(this.nBarItemDbMaintenanceLight),
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarCardLink)});
            this.nbargrpFiscalPrinterCommands.Name = "nbargrpFiscalPrinterCommands";
            // 
            // nBarItemReadFiscalMemory
            // 
            this.nBarItemReadFiscalMemory.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.nBarItemReadFiscalMemory.AppearanceHotTracked.Options.UseForeColor = true;
            this.nBarItemReadFiscalMemory.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.nBarItemReadFiscalMemory.AppearancePressed.Options.UseForeColor = true;
            this.nBarItemReadFiscalMemory.Caption = "Read Fiscal Memory";
            this.nBarItemReadFiscalMemory.Name = "nBarItemReadFiscalMemory";
            this.nBarItemReadFiscalMemory.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemFiscalPrinter_LinkClicked);
            // 
            // nBarItemServiceForcedCancel
            // 
            this.nBarItemServiceForcedCancel.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.nBarItemServiceForcedCancel.AppearanceHotTracked.Options.UseForeColor = true;
            this.nBarItemServiceForcedCancel.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.nBarItemServiceForcedCancel.AppearancePressed.Options.UseForeColor = true;
            this.nBarItemServiceForcedCancel.Caption = "Forced Cancel Receipt";
            this.nBarItemServiceForcedCancel.Name = "nBarItemServiceForcedCancel";
            this.nBarItemServiceForcedCancel.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemServiceForcedCancel_LinkClicked);
            // 
            // nBarItemForcedStartDay
            // 
            this.nBarItemForcedStartDay.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.nBarItemForcedStartDay.AppearanceHotTracked.Options.UseForeColor = true;
            this.nBarItemForcedStartDay.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.nBarItemForcedStartDay.AppearancePressed.Options.UseForeColor = true;
            this.nBarItemForcedStartDay.Caption = "Forced Start Day";
            this.nBarItemForcedStartDay.Name = "nBarItemForcedStartDay";
            this.nBarItemForcedStartDay.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemForcedStartDay_LinkClicked);
            // 
            // nBarItemShowVATFactors
            // 
            this.nBarItemShowVATFactors.Caption = "Display VAT Factors";
            this.nBarItemShowVATFactors.Name = "nBarItemShowVATFactors";
            this.nBarItemShowVATFactors.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemShowVATFactors_LinkClicked);
            // 
            // nBarItemEDPSCheckCommunication
            // 
            this.nBarItemEDPSCheckCommunication.Caption = "EDPS Check Communication";
            this.nBarItemEDPSCheckCommunication.Name = "nBarItemEDPSCheckCommunication";
            this.nBarItemEDPSCheckCommunication.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemEDPSCheckCommunication_LinkClicked);
            // 
            // nBarItemDbMaintenance
            // 
            this.nBarItemDbMaintenance.Caption = "Database Maintenance";
            this.nBarItemDbMaintenance.Name = "nBarItemDbMaintenance";
            this.nBarItemDbMaintenance.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemDbMaintenance_LinkClicked);
            // 
            // nBarItemDbMaintenanceLight
            // 
            this.nBarItemDbMaintenanceLight.Caption = "Database Maintenance (Light)";
            this.nBarItemDbMaintenanceLight.Name = "nBarItemDbMaintenanceLight";
            this.nBarItemDbMaintenanceLight.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nBarItemDbMaintenanceLight_LinkClicked);
            // 
            // navBarCardLink
            // 
            this.navBarCardLink.Caption = "CardLink Check";
            this.navBarCardLink.Name = "navBarCardLink";
            this.navBarCardLink.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarCardLink_LinkClicked);
            // 
            // nbaritmKeyboard
            // 
            this.nbaritmKeyboard.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.nbaritmKeyboard.Appearance.Options.UseFont = true;
            this.nbaritmKeyboard.Caption = "Keyboard";
            this.nbaritmKeyboard.Name = "nbaritmKeyboard";
            this.nbaritmKeyboard.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbaritmKeyboard_LinkClicked);
            // 
            // nbaritmConfiguration
            // 
            this.nbaritmConfiguration.Caption = "Configuration";
            this.nbaritmConfiguration.Name = "nbaritmConfiguration";
            this.nbaritmConfiguration.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbaritmConfiguration_LinkClicked);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(302, 426);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.navBarControl1;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(282, 364);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnExit;
            this.layoutControlItem2.CustomizationFormText = "lyctrlitmExit";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 364);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(282, 42);
            this.layoutControlItem2.Text = "lyctrlitmExit";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // navBarItem1
            // 
            this.navBarItem1.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.navBarItem1.AppearanceHotTracked.Options.UseForeColor = true;
            this.navBarItem1.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.navBarItem1.AppearancePressed.Options.UseForeColor = true;
            this.navBarItem1.Caption = "Forced Start Day";
            this.navBarItem1.Name = "navBarItem1";
            // 
            // navBarItem2
            // 
            this.navBarItem2.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.navBarItem2.AppearanceHotTracked.Options.UseForeColor = true;
            this.navBarItem2.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.navBarItem2.AppearancePressed.Options.UseForeColor = true;
            this.navBarItem2.Caption = "Forced Start Day";
            this.navBarItem2.Name = "navBarItem2";
            // 
            // navBarItem3
            // 
            this.navBarItem3.AppearanceHotTracked.ForeColor = System.Drawing.SystemColors.Highlight;
            this.navBarItem3.AppearanceHotTracked.Options.UseForeColor = true;
            this.navBarItem3.AppearancePressed.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.navBarItem3.AppearancePressed.Options.UseForeColor = true;
            this.navBarItem3.Caption = "Forced Start Day";
            this.navBarItem3.Name = "navBarItem3";
            // 
            // frmMenu
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 426);
            this.ControlBox = false;
            this.Controls.Add(this.lyctrlMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmMenu";
            this.Text = "Main Menu";
            ((System.ComponentModel.ISupportInitialize)(this.lyctrlMenu)).EndInit();
            this.lyctrlMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl lyctrlMenu;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarItem nbaritmKeyboard;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraNavBar.NavBarGroup nbargrpActions;
        private DevExpress.XtraNavBar.NavBarItem nbaritemShutDown;
        private DevExpress.XtraNavBar.NavBarItem nbaritmConfiguration;
        private DevExpress.XtraNavBar.NavBarItem nBarItemReadFiscalMemory;
        private DevExpress.XtraNavBar.NavBarGroup nbargrpFiscalPrinterCommands;
        private DevExpress.XtraNavBar.NavBarItem nBarItemServiceForcedCancel;
        private DevExpress.XtraNavBar.NavBarItem nBarItemForcedStartDay;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private DevExpress.XtraNavBar.NavBarItem navBarItem2;
        private DevExpress.XtraNavBar.NavBarItem nBarItemShowVATFactors;
        private DevExpress.XtraNavBar.NavBarItem navBarItem3;
        private DevExpress.XtraNavBar.NavBarItem nBarItemEDPSCheckCommunication;
        private DevExpress.XtraNavBar.NavBarItem nBarItemDbMaintenance;
        private DevExpress.XtraNavBar.NavBarItem nBarItemDbMaintenanceLight;
        private DevExpress.XtraNavBar.NavBarItem navBarCardLink;
    }
}