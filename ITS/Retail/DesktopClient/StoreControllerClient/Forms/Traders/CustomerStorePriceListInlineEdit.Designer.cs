namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class CustomerStorePriceListInlineEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerStorePriceListInlineEdit));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lueOwner = new DevExpress.XtraEditors.LookUpEdit();
            this.luePriceCatalog = new DevExpress.XtraEditors.LookUpEdit();
            this.lueStore = new DevExpress.XtraEditors.LookUpEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelCustomerStorePriceList = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveCustomerStorePriceList = new DevExpress.XtraEditors.SimpleButton();
            this.chkIsDefault = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcOwner = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPriceCatalog = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCity = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStore = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueOwner.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePriceCatalog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueStore.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsDefault.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcOwner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPriceCatalog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStore)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lueOwner);
            this.layoutControl1.Controls.Add(this.luePriceCatalog);
            this.layoutControl1.Controls.Add(this.lueStore);
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.chkIsDefault);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(434, 192);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lueOwner
            // 
            this.lueOwner.Location = new System.Drawing.Point(22, 87);
            this.lueOwner.Name = "lueOwner";
            this.lueOwner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueOwner.Size = new System.Drawing.Size(193, 20);
            this.lueOwner.StyleController = this.layoutControl1;
            this.lueOwner.TabIndex = 5;
            this.lueOwner.EditValueChanged += new System.EventHandler(this.lueOwner_EditValueChanged);
            // 
            // luePriceCatalog
            // 
            this.luePriceCatalog.Location = new System.Drawing.Point(22, 127);
            this.luePriceCatalog.Name = "luePriceCatalog";
            this.luePriceCatalog.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.luePriceCatalog.Size = new System.Drawing.Size(390, 20);
            this.luePriceCatalog.StyleController = this.layoutControl1;
            this.luePriceCatalog.TabIndex = 7;
            this.luePriceCatalog.EditValueChanged += new System.EventHandler(this.luePriceCatalog_EditValueChanged);
            // 
            // lueStore
            // 
            this.lueStore.Location = new System.Drawing.Point(219, 87);
            this.lueStore.Name = "lueStore";
            this.lueStore.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueStore.Size = new System.Drawing.Size(193, 20);
            this.lueStore.StyleController = this.layoutControl1;
            this.lueStore.TabIndex = 6;
            this.lueStore.EditValueChanged += new System.EventHandler(this.lueStore_EditValueChanged);
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btnCancelCustomerStorePriceList);
            this.panelControl1.Controls.Add(this.btnSaveCustomerStorePriceList);
            this.panelControl1.Location = new System.Drawing.Point(22, 22);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(390, 45);
            this.panelControl1.TabIndex = 9;
            // 
            // btnCancelCustomerStorePriceList
            // 
            this.btnCancelCustomerStorePriceList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelCustomerStorePriceList.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancelCustomerStorePriceList.Location = new System.Drawing.Point(286, 3);
            this.btnCancelCustomerStorePriceList.Name = "btnCancelCustomerStorePriceList";
            this.btnCancelCustomerStorePriceList.Size = new System.Drawing.Size(101, 38);
            this.btnCancelCustomerStorePriceList.TabIndex = 1;
            this.btnCancelCustomerStorePriceList.Text = "@@Cancel";
            this.btnCancelCustomerStorePriceList.Click += new System.EventHandler(this.btnCancelCustomerStorePriceList_Click);
            // 
            // btnSaveCustomerStorePriceList
            // 
            this.btnSaveCustomerStorePriceList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCustomerStorePriceList.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.btnSaveCustomerStorePriceList.Location = new System.Drawing.Point(170, 3);
            this.btnSaveCustomerStorePriceList.Name = "btnSaveCustomerStorePriceList";
            this.btnSaveCustomerStorePriceList.Size = new System.Drawing.Size(110, 38);
            this.btnSaveCustomerStorePriceList.TabIndex = 2;
            this.btnSaveCustomerStorePriceList.Text = "@@Save";
            this.btnSaveCustomerStorePriceList.Click += new System.EventHandler(this.btnSaveCustomerStorePriceList_Click);
            // 
            // chkIsDefault
            // 
            this.chkIsDefault.EditValue = null;
            this.chkIsDefault.Location = new System.Drawing.Point(22, 151);
            this.chkIsDefault.Name = "chkIsDefault";
            this.chkIsDefault.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.chkIsDefault.Properties.Caption = "@@IsDefault";
            this.chkIsDefault.Size = new System.Drawing.Size(390, 19);
            this.chkIsDefault.StyleController = this.layoutControl1;
            this.chkIsDefault.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.Root});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(434, 192);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // Root
            // 
            this.Root.CustomizationFormText = "Root";
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcOwner,
            this.lcPriceCatalog,
            this.layoutControlItem2,
            this.lcCity,
            this.lcStore});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "item0";
            this.Root.Size = new System.Drawing.Size(414, 172);
            this.Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Text = "Root";
            this.Root.TextVisible = false;
            // 
            // lcOwner
            // 
            this.lcOwner.Control = this.lueOwner;
            this.lcOwner.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcOwner, false);
            this.lcOwner.Location = new System.Drawing.Point(0, 49);
            this.lcOwner.Name = "lcOwner";
            this.lcOwner.Size = new System.Drawing.Size(197, 40);
            this.lcOwner.Text = "@@Owner";
            this.lcOwner.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcOwner.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lcPriceCatalog
            // 
            this.lcPriceCatalog.Control = this.luePriceCatalog;
            this.lcPriceCatalog.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcPriceCatalog, false);
            this.lcPriceCatalog.Location = new System.Drawing.Point(0, 89);
            this.lcPriceCatalog.Name = "lcPriceCatalog";
            this.lcPriceCatalog.Size = new System.Drawing.Size(394, 40);
            this.lcPriceCatalog.Text = "@@PriceCatalog";
            this.lcPriceCatalog.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPriceCatalog.TextSize = new System.Drawing.Size(80, 13);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panelControl1;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(394, 49);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lcCity
            // 
            this.lcCity.Control = this.chkIsDefault;
            this.lcCity.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcCity, false);
            this.lcCity.Location = new System.Drawing.Point(0, 129);
            this.lcCity.Name = "lcCity";
            this.lcCity.Size = new System.Drawing.Size(394, 23);
            this.lcCity.Text = "@@City";
            this.lcCity.TextSize = new System.Drawing.Size(0, 0);
            this.lcCity.TextVisible = false;
            // 
            // lcStore
            // 
            this.lcStore.Control = this.lueStore;
            this.lcStore.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcStore, false);
            this.lcStore.Location = new System.Drawing.Point(197, 49);
            this.lcStore.Name = "lcStore";
            this.lcStore.Size = new System.Drawing.Size(197, 40);
            this.lcStore.Text = "@@Store";
            this.lcStore.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcStore.TextSize = new System.Drawing.Size(80, 13);
            // 
            // CustomerStorePriceListInlineEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 192);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "CustomerStorePriceListInlineEdit";
            this.Text = "@@Edit";
            this.Load += new System.EventHandler(this.CustomerStorePriceListInlineEdit_Load);
            this.Shown += new System.EventHandler(this.CustomerStorePriceListInlineEdit_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueOwner.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePriceCatalog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueStore.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIsDefault.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcOwner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPriceCatalog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStore)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LookUpEdit lueOwner;
        private DevExpress.XtraEditors.LookUpEdit luePriceCatalog;
        private DevExpress.XtraEditors.LookUpEdit lueStore;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancelCustomerStorePriceList;
        private DevExpress.XtraEditors.SimpleButton btnSaveCustomerStorePriceList;
        private DevExpress.XtraEditors.CheckEdit chkIsDefault;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcOwner;
        private DevExpress.XtraLayout.LayoutControlItem lcPriceCatalog;
        private DevExpress.XtraLayout.LayoutControlItem lcStore;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lcCity;
    }
}