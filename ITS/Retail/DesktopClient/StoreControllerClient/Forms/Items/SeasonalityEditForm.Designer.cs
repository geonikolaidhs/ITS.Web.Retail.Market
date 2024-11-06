namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class SeasonalityEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeasonalityEditForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelSeasonality = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveSeasonality = new DevExpress.XtraEditors.SimpleButton();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item0 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDescription = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.txtCode);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(377, 191);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btnCancelSeasonality);
            this.panelControl1.Controls.Add(this.btnSaveSeasonality);
            this.panelControl1.Location = new System.Drawing.Point(32, 32);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(313, 47);
            this.panelControl1.TabIndex = 9;
            // 
            // btnCancelSeasonality
            // 
            this.btnCancelSeasonality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelSeasonality.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancelSeasonality.Location = new System.Drawing.Point(207, 5);
            this.btnCancelSeasonality.Name = "btnCancelSeasonality";
            this.btnCancelSeasonality.Size = new System.Drawing.Size(101, 38);
            this.btnCancelSeasonality.TabIndex = 1;
            this.btnCancelSeasonality.Text = "@@Cancel";
            this.btnCancelSeasonality.Click += new System.EventHandler(this.btnCancelSeasonality_Click);
            // 
            // btnSaveSeasonality
            // 
            this.btnSaveSeasonality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSeasonality.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.btnSaveSeasonality.Location = new System.Drawing.Point(91, 5);
            this.btnSaveSeasonality.Name = "btnSaveSeasonality";
            this.btnSaveSeasonality.Size = new System.Drawing.Size(110, 38);
            this.btnSaveSeasonality.TabIndex = 2;
            this.btnSaveSeasonality.Text = "@@Save";
            this.btnSaveSeasonality.Click += new System.EventHandler(this.btnSaveSeasonality_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(32, 99);
            this.txtCode.Name = "txtCode";
            this.txtCode.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtCode.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtCode.Size = new System.Drawing.Size(313, 20);
            this.txtCode.StyleController = this.layoutControl1;
            this.txtCode.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(32, 139);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtDescription.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtDescription.Size = new System.Drawing.Size(313, 20);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item0});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(377, 191);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // item0
            // 
            this.item0.CustomizationFormText = "Root";
            this.item0.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.item0.GroupBordersVisible = false;
            this.item0.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item1});
            this.item0.Location = new System.Drawing.Point(0, 0);
            this.item0.Name = "item0";
            this.item0.Size = new System.Drawing.Size(357, 171);
            this.item0.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.item0.TextVisible = false;
            // 
            // item1
            // 
            this.item1.CustomizationFormText = "Root";
            this.item1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.item1.GroupBordersVisible = false;
            this.item1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.lcCode,
            this.lcDescription});
            this.item1.Location = new System.Drawing.Point(0, 0);
            this.item1.Name = "item1";
            this.item1.Size = new System.Drawing.Size(337, 151);
            this.item1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.item1.Text = "Root";
            this.item1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panelControl1;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(317, 51);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lcCode
            // 
            this.lcCode.Control = this.txtCode;
            this.lcCode.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcCode, false);
            this.lcCode.Location = new System.Drawing.Point(0, 51);
            this.lcCode.Name = "lcCode";
            this.lcCode.Size = new System.Drawing.Size(317, 40);
            this.lcCode.Text = "@@Code";
            this.lcCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcCode.TextSize = new System.Drawing.Size(73, 13);
            // 
            // lcDescription
            // 
            this.lcDescription.Control = this.txtDescription;
            this.lcDescription.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcDescription, false);
            this.lcDescription.Location = new System.Drawing.Point(0, 91);
            this.lcDescription.Name = "lcDescription";
            this.lcDescription.Size = new System.Drawing.Size(317, 40);
            this.lcDescription.Text = "@@Description";
            this.lcDescription.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcDescription.TextSize = new System.Drawing.Size(73, 13);
            // 
            // SeasonalityEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 191);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "SeasonalityEditForm";
            this.Text = "SeasonalityEditForm";
            this.Load += new System.EventHandler(this.SeasonalityEditForm_Load);
            this.Shown += new System.EventHandler(this.SeasonalityEditForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancelSeasonality;
        private DevExpress.XtraEditors.SimpleButton btnSaveSeasonality;
        private DevExpress.XtraLayout.LayoutControlGroup item0;
        private DevExpress.XtraLayout.LayoutControlGroup item1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraEditors.TextEdit txtCode;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem lcDescription;
    }
}