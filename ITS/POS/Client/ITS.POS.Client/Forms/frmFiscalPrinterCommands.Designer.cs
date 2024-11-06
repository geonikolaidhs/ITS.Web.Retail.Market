namespace ITS.POS.Client.Forms
{
    partial class frmFiscalPrinterCommands
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFiscalPrinterCommands));
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.nbaritmKeyboard = new DevExpress.XtraNavBar.NavBarItem();
            this.nbaritmConfiguration = new DevExpress.XtraNavBar.NavBarItem();
            this.nbargrpActions = new DevExpress.XtraNavBar.NavBarGroup();
            this.itmAnalyticPerZ = new DevExpress.XtraNavBar.NavBarItem();
            this.itmSumPerZ = new DevExpress.XtraNavBar.NavBarItem();
            this.itmSignaturePerZ = new DevExpress.XtraNavBar.NavBarItem();
            this.itmAnalyticPerDate = new DevExpress.XtraNavBar.NavBarItem();
            this.itmSumPerDate = new DevExpress.XtraNavBar.NavBarItem();
            this.itmSignaturePerDate = new DevExpress.XtraNavBar.NavBarItem();
            this.itmPerBlock = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnExit.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.btnExit.Appearance.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnExit.Appearance.Options.UseBackColor = true;
            this.btnExit.Appearance.Options.UseFont = true;
            this.btnExit.Appearance.Options.UseImage = true;
            this.btnExit.Appearance.Options.UseTextOptions = true;
            this.btnExit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnExit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExit.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnExit.Location = new System.Drawing.Point(12, 418);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(264, 38);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "C L O S E";
            // 
            // nbaritmKeyboard
            // 
            this.nbaritmKeyboard.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.nbaritmKeyboard.Appearance.Options.UseFont = true;
            this.nbaritmKeyboard.Caption = "Keyboard";
            this.nbaritmKeyboard.Name = "nbaritmKeyboard";
            // 
            // nbaritmConfiguration
            // 
            this.nbaritmConfiguration.Caption = "Configuration";
            this.nbaritmConfiguration.Name = "nbaritmConfiguration";
            // 
            // nbargrpActions
            // 
            this.nbargrpActions.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.nbargrpActions.Appearance.Options.UseFont = true;
            this.nbargrpActions.Caption = "Read Fiscal Memory";
            this.nbargrpActions.Expanded = true;
            this.nbargrpActions.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmAnalyticPerZ),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmSumPerZ),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmSignaturePerZ),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmAnalyticPerDate),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmSumPerDate),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmSignaturePerDate),
            new DevExpress.XtraNavBar.NavBarItemLink(this.itmPerBlock)});
            this.nbargrpActions.Name = "nbargrpActions";
            // 
            // itmAnalyticPerZ
            // 
            this.itmAnalyticPerZ.Caption = "Analytic - Per Z";
            this.itmAnalyticPerZ.Name = "itmAnalyticPerZ";
            this.itmAnalyticPerZ.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmAnalyticPerZ_LinkClicked);
            // 
            // itmSumPerZ
            // 
            this.itmSumPerZ.Caption = "Summarized - Per Z";
            this.itmSumPerZ.Name = "itmSumPerZ";
            this.itmSumPerZ.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmSumPerZ_LinkClicked);
            // 
            // itmSignaturePerZ
            // 
            this.itmSignaturePerZ.Caption = "Signatures - Per Z";
            this.itmSignaturePerZ.Name = "itmSignaturePerZ";
            this.itmSignaturePerZ.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmSignaturePerZ_LinkClicked);
            // 
            // itmAnalyticPerDate
            // 
            this.itmAnalyticPerDate.Caption = "Analytic - Per Date";
            this.itmAnalyticPerDate.Name = "itmAnalyticPerDate";
            this.itmAnalyticPerDate.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmAnalyticPerDate_LinkClicked);
            // 
            // itmSumPerDate
            // 
            this.itmSumPerDate.Caption = "Summarized - Per Date";
            this.itmSumPerDate.Name = "itmSumPerDate";
            this.itmSumPerDate.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmSumPerDate_LinkClicked);
            // 
            // itmSignaturePerDate
            // 
            this.itmSignaturePerDate.Caption = "Signatures - Per Date";
            this.itmSignaturePerDate.Name = "itmSignaturePerDate";
            this.itmSignaturePerDate.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmSignaturePerDate_LinkClicked);
            // 
            // itmPerBlock
            // 
            this.itmPerBlock.Caption = "Per Block";
            this.itmPerBlock.Name = "itmPerBlock";
            this.itmPerBlock.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.itmPerBlock_LinkClicked);
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
            this.nbargrpActions});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.nbaritmKeyboard,
            this.nbaritmConfiguration,
            this.itmAnalyticPerZ,
            this.itmSumPerZ,
            this.itmSignaturePerZ,
            this.itmAnalyticPerDate,
            this.itmSumPerDate,
            this.itmSignaturePerDate,
            this.itmPerBlock});
            this.navBarControl1.Location = new System.Drawing.Point(12, 12);
            this.navBarControl1.LookAndFeel.SkinName = "Metropolis";
            this.navBarControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 264;
            this.navBarControl1.Size = new System.Drawing.Size(264, 400);
            this.navBarControl1.TabIndex = 15;
            this.navBarControl1.Text = "navbarctlMenu";
            this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.StandardSkinExplorerBarViewInfoRegistrator("Metropolis");
            // 
            // frmFiscalPrinterCommands
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(287, 468);
            this.Controls.Add(this.navBarControl1);
            this.Controls.Add(this.btnExit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmFiscalPrinterCommands";
            this.Text = "frmFiscalPrinterCommands";
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraNavBar.NavBarItem nbaritmKeyboard;
        private DevExpress.XtraNavBar.NavBarItem nbaritmConfiguration;
        private DevExpress.XtraNavBar.NavBarGroup nbargrpActions;
        private DevExpress.XtraNavBar.NavBarItem itmAnalyticPerZ;
        private DevExpress.XtraNavBar.NavBarItem itmSumPerZ;
        private DevExpress.XtraNavBar.NavBarItem itmSignaturePerZ;
        private DevExpress.XtraNavBar.NavBarItem itmAnalyticPerDate;
        private DevExpress.XtraNavBar.NavBarItem itmSumPerDate;
        private DevExpress.XtraNavBar.NavBarItem itmSignaturePerDate;
        private DevExpress.XtraNavBar.NavBarItem itmPerBlock;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
    }
}