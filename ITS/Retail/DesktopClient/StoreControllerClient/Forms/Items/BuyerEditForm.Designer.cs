namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class BuyerEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuyerEditForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelBuyer = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveBuyer = new DevExpress.XtraEditors.SimpleButton();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtReferenceCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item0 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcReferenceCode = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReferenceCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReferenceCode)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.txtCode);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.txtReferenceCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(378, 252);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btnCancelBuyer);
            this.panelControl1.Controls.Add(this.btnSaveBuyer);
            this.panelControl1.Location = new System.Drawing.Point(42, 42);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(294, 48);
            this.panelControl1.TabIndex = 9;
            // 
            // btnCancelBuyer
            // 
            this.btnCancelBuyer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelBuyer.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancelBuyer.Location = new System.Drawing.Point(188, 5);
            this.btnCancelBuyer.Name = "btnCancelBuyer";
            this.btnCancelBuyer.Size = new System.Drawing.Size(101, 38);
            this.btnCancelBuyer.TabIndex = 1;
            this.btnCancelBuyer.Text = "@@Cancel";
            this.btnCancelBuyer.Click += new System.EventHandler(this.btnCancelBuyer_Click);
            // 
            // btnSaveBuyer
            // 
            this.btnSaveBuyer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveBuyer.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.btnSaveBuyer.Location = new System.Drawing.Point(72, 5);
            this.btnSaveBuyer.Name = "btnSaveBuyer";
            this.btnSaveBuyer.Size = new System.Drawing.Size(110, 38);
            this.btnSaveBuyer.TabIndex = 2;
            this.btnSaveBuyer.Text = "@@Save";
            this.btnSaveBuyer.Click += new System.EventHandler(this.btnSaveBuyer_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(42, 110);
            this.txtCode.Name = "txtCode";
            this.txtCode.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtCode.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtCode.Size = new System.Drawing.Size(294, 20);
            this.txtCode.StyleController = this.layoutControl1;
            this.txtCode.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(42, 150);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtDescription.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtDescription.Size = new System.Drawing.Size(294, 20);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 6;
            // 
            // txtReferenceCode
            // 
            this.txtReferenceCode.Location = new System.Drawing.Point(42, 190);
            this.txtReferenceCode.Name = "txtReferenceCode";
            this.txtReferenceCode.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtReferenceCode.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtReferenceCode.Size = new System.Drawing.Size(294, 20);
            this.txtReferenceCode.StyleController = this.layoutControl1;
            this.txtReferenceCode.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.Root});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(378, 252);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // Root
            // 
            this.Root.CustomizationFormText = "Root";
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item0});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "item0";
            this.Root.Size = new System.Drawing.Size(358, 232);
            this.Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Text = "Root";
            this.Root.TextVisible = false;
            // 
            // item0
            // 
            this.item0.CustomizationFormText = "Root";
            this.item0.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.item0.GroupBordersVisible = false;
            this.item0.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item1});
            this.item0.Location = new System.Drawing.Point(0, 0);
            this.item0.Name = "item1";
            this.item0.Size = new System.Drawing.Size(338, 212);
            this.item0.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.item0.Text = "item0";
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
            this.lcDescription,
            this.lcReferenceCode});
            this.item1.Location = new System.Drawing.Point(0, 0);
            this.item1.Name = "item2";
            this.item1.Size = new System.Drawing.Size(318, 192);
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
            this.layoutControlItem2.Size = new System.Drawing.Size(298, 52);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lcCode
            // 
            this.lcCode.Control = this.txtCode;
            this.lcCode.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcCode, false);
            this.lcCode.Location = new System.Drawing.Point(0, 52);
            this.lcCode.Name = "lcCode";
            this.lcCode.Size = new System.Drawing.Size(298, 40);
            this.lcCode.Text = "@@Code";
            this.lcCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcCode.TextSize = new System.Drawing.Size(95, 13);
            // 
            // lcDescription
            // 
            this.lcDescription.Control = this.txtDescription;
            this.lcDescription.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcDescription, false);
            this.lcDescription.Location = new System.Drawing.Point(0, 92);
            this.lcDescription.Name = "lcDescription";
            this.lcDescription.Size = new System.Drawing.Size(298, 40);
            this.lcDescription.Text = "@@Description";
            this.lcDescription.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcDescription.TextSize = new System.Drawing.Size(95, 13);
            // 
            // lcReferenceCode
            // 
            this.lcReferenceCode.Control = this.txtReferenceCode;
            this.lcReferenceCode.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcReferenceCode, false);
            this.lcReferenceCode.Location = new System.Drawing.Point(0, 132);
            this.lcReferenceCode.Name = "lcReferenceCode";
            this.lcReferenceCode.Size = new System.Drawing.Size(298, 40);
            this.lcReferenceCode.Text = "@@ReferenceCode";
            this.lcReferenceCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcReferenceCode.TextSize = new System.Drawing.Size(95, 13);
            // 
            // BuyerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 252);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "BuyerEditForm";
            this.Text = "BuyerEditForm";
            this.Load += new System.EventHandler(this.BuyerEditForm_Load);
            this.Shown += new System.EventHandler(this.BuyerEditForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReferenceCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReferenceCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancelBuyer;
        private DevExpress.XtraEditors.SimpleButton btnSaveBuyer;
        private DevExpress.XtraEditors.TextEdit txtCode;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup item0;
        private DevExpress.XtraLayout.LayoutControlGroup item1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraLayout.LayoutControlItem lcDescription;
        private DevExpress.XtraEditors.TextEdit txtReferenceCode;
        private DevExpress.XtraLayout.LayoutControlItem lcReferenceCode;
    }
}