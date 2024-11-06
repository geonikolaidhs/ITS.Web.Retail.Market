namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtHQUrl = new DevExpress.XtraEditors.TextEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.txtStoreControllerURL = new DevExpress.XtraEditors.TextEdit();
            this.lueDefaultPrinter = new DevExpress.XtraEditors.LookUpEdit();
            this.lueCulture = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnFlag = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcCulture = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDefaultPrinter = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStoreControllerURL = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.loHQUrl = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHQUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStoreControllerURL.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDefaultPrinter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCulture.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCulture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDefaultPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStoreControllerURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loHQUrl)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtHQUrl);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnOk);
            this.layoutControl1.Controls.Add(this.txtStoreControllerURL);
            this.layoutControl1.Controls.Add(this.lueDefaultPrinter);
            this.layoutControl1.Controls.Add(this.lueCulture);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(731, 365, 478, 514);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(384, 172);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtHQUrl
            // 
            this.txtHQUrl.Location = new System.Drawing.Point(107, 84);
            this.txtHQUrl.Name = "txtHQUrl";
            this.txtHQUrl.Size = new System.Drawing.Size(265, 20);
            this.txtHQUrl.StyleController = this.layoutControl1;
            this.txtHQUrl.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancel.Location = new System.Drawing.Point(269, 118);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 38);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "@@Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Ok_32;
            this.btnOk.Location = new System.Drawing.Point(12, 118);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(103, 38);
            this.btnOk.StyleController = this.layoutControl1;
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "@@OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtStoreControllerURL
            // 
            this.txtStoreControllerURL.Location = new System.Drawing.Point(107, 60);
            this.txtStoreControllerURL.Name = "txtStoreControllerURL";
            this.txtStoreControllerURL.Size = new System.Drawing.Size(265, 20);
            this.txtStoreControllerURL.StyleController = this.layoutControl1;
            this.txtStoreControllerURL.TabIndex = 6;
            // 
            // lueDefaultPrinter
            // 
            this.lueDefaultPrinter.Location = new System.Drawing.Point(107, 12);
            this.lueDefaultPrinter.Name = "lueDefaultPrinter";
            this.lueDefaultPrinter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.lueDefaultPrinter.Properties.NullText = "";
            this.lueDefaultPrinter.Size = new System.Drawing.Size(265, 20);
            this.lueDefaultPrinter.StyleController = this.layoutControl1;
            this.lueDefaultPrinter.TabIndex = 4;
            // 
            // lueCulture
            // 
            this.lueCulture.Location = new System.Drawing.Point(107, 36);
            this.lueCulture.Name = "lueCulture";
            this.lueCulture.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCulture.Properties.NullText = "";
            this.lueCulture.Properties.View = this.gridLookUpEdit1View;
            this.lueCulture.Size = new System.Drawing.Size(265, 20);
            this.lueCulture.StyleController = this.layoutControl1;
            this.lueCulture.TabIndex = 5;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnFlag,
            this.gridColumnDescription});
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsBehavior.AutoPopulateColumns = false;
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnFlag
            // 
            this.gridColumnFlag.Caption = " ";
            this.gridColumnFlag.FieldName = "Flag";
            this.gridColumnFlag.Name = "gridColumnFlag";
            this.gridColumnFlag.Visible = true;
            this.gridColumnFlag.VisibleIndex = 0;
            // 
            // gridColumnDescription
            // 
            this.gridColumnDescription.Caption = "  ";
            this.gridColumnDescription.FieldName = "Description";
            this.gridColumnDescription.Name = "gridColumnDescription";
            this.gridColumnDescription.Visible = true;
            this.gridColumnDescription.VisibleIndex = 1;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcCulture,
            this.lcDefaultPrinter,
            this.lcStoreControllerURL,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.loHQUrl});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(384, 172);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lcCulture
            // 
            this.lcCulture.Control = this.lueCulture;
            this.SetIsRequired(this.lcCulture, false);
            this.lcCulture.Location = new System.Drawing.Point(0, 24);
            this.lcCulture.Name = "lcCulture";
            this.lcCulture.Size = new System.Drawing.Size(364, 24);
            this.lcCulture.Text = "@@Language";
            this.lcCulture.TextSize = new System.Drawing.Size(92, 13);
            // 
            // lcDefaultPrinter
            // 
            this.lcDefaultPrinter.Control = this.lueDefaultPrinter;
            this.SetIsRequired(this.lcDefaultPrinter, false);
            this.lcDefaultPrinter.Location = new System.Drawing.Point(0, 0);
            this.lcDefaultPrinter.Name = "lcDefaultPrinter";
            this.lcDefaultPrinter.Size = new System.Drawing.Size(364, 24);
            this.lcDefaultPrinter.Text = "@@DefaultPrinter";
            this.lcDefaultPrinter.TextSize = new System.Drawing.Size(92, 13);
            // 
            // lcStoreControllerURL
            // 
            this.lcStoreControllerURL.Control = this.txtStoreControllerURL;
            this.lcStoreControllerURL.CustomizationFormText = "Master Url";
            this.SetIsRequired(this.lcStoreControllerURL, false);
            this.lcStoreControllerURL.Location = new System.Drawing.Point(0, 48);
            this.lcStoreControllerURL.Name = "lcStoreControllerURL";
            this.lcStoreControllerURL.Size = new System.Drawing.Size(364, 24);
            this.lcStoreControllerURL.Text = "Store Controller Url";
            this.lcStoreControllerURL.TextSize = new System.Drawing.Size(92, 13);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnOk;
            this.SetIsRequired(this.layoutControlItem1, false);
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(107, 56);
            this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 0);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnCancel;
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(257, 96);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(107, 56);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 0);
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem1, false);
            this.emptySpaceItem1.Location = new System.Drawing.Point(107, 96);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(150, 56);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // loHQUrl
            // 
            this.loHQUrl.Control = this.txtHQUrl;
            this.SetIsRequired(this.loHQUrl, false);
            this.loHQUrl.Location = new System.Drawing.Point(0, 72);
            this.loHQUrl.Name = "loHQUrl";
            this.loHQUrl.Size = new System.Drawing.Size(364, 24);
            this.loHQUrl.Text = "HQ Url";
            this.loHQUrl.TextSize = new System.Drawing.Size(92, 13);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 172);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(418, 236);
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "SettingsForm";
            this.Text = "@@Settings";
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtHQUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStoreControllerURL.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDefaultPrinter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCulture.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCulture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDefaultPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStoreControllerURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loHQUrl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LookUpEdit lueDefaultPrinter;
        private DevExpress.XtraLayout.LayoutControlItem lcDefaultPrinter;
        private DevExpress.XtraLayout.LayoutControlItem lcCulture;
        private DevExpress.XtraEditors.TextEdit txtStoreControllerURL;
        private DevExpress.XtraLayout.LayoutControlItem lcStoreControllerURL;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.GridLookUpEdit lueCulture;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFlag;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDescription;
        private DevExpress.XtraEditors.TextEdit txtHQUrl;
        private DevExpress.XtraLayout.LayoutControlItem loHQUrl;
    }
}