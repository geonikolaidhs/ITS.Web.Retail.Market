namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class PurchaseDocumentSecondaryFilterControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.searchLookUpEditDocumentSupplierFilter = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSupplierDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSupplierCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSupplierTaxCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditTaxCode = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditCompanyName = new DevExpress.XtraEditors.DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textEditTaxCode);
            this.layoutControl1.Controls.Add(this.textEditCompanyName);
            this.layoutControl1.Controls.Add(this.searchLookUpEditDocumentSupplierFilter);
            this.layoutControl1.Size = new System.Drawing.Size(639, 54);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(639, 54);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.searchLookUpEditDocumentSupplierFilter;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(259, 34);
            this.layoutControlItem1.Text = "@@Supplier";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(67, 13);
            // 
            // searchLookUpEditDocumentSupplierFilter
            // 
            this.SetBoundFieldName(this.searchLookUpEditDocumentSupplierFilter, "Supplier");
            this.SetBoundPropertyName(this.searchLookUpEditDocumentSupplierFilter, "EditValue");
            this.searchLookUpEditDocumentSupplierFilter.Location = new System.Drawing.Point(83, 12);
            this.searchLookUpEditDocumentSupplierFilter.Name = "searchLookUpEditDocumentSupplierFilter";
            this.searchLookUpEditDocumentSupplierFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.searchLookUpEditDocumentSupplierFilter.Properties.NullText = "";
            this.searchLookUpEditDocumentSupplierFilter.Properties.View = this.searchLookUpEdit1View;
            this.searchLookUpEditDocumentSupplierFilter.Size = new System.Drawing.Size(184, 20);
            this.searchLookUpEditDocumentSupplierFilter.StyleController = this.layoutControl1;
            this.searchLookUpEditDocumentSupplierFilter.TabIndex = 5;
            this.searchLookUpEditDocumentSupplierFilter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.searchLookUpEditDocumentSupplierFilter_ButtonClick);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSupplierDescription,
            this.gridColumnSupplierCode,
            this.gridColumnSupplierTaxCode});
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnSupplierDescription
            // 
            this.gridColumnSupplierDescription.Caption = "@@Description";
            this.gridColumnSupplierDescription.FieldName = "CompanyName";
            this.gridColumnSupplierDescription.Name = "gridColumnSupplierDescription";
            this.gridColumnSupplierDescription.OptionsColumn.AllowEdit = false;
            this.gridColumnSupplierDescription.Visible = true;
            this.gridColumnSupplierDescription.VisibleIndex = 0;
            // 
            // gridColumnSupplierCode
            // 
            this.gridColumnSupplierCode.Caption = "@@Code";
            this.gridColumnSupplierCode.FieldName = "Code";
            this.gridColumnSupplierCode.Name = "gridColumnSupplierCode";
            this.gridColumnSupplierCode.OptionsColumn.AllowEdit = false;
            this.gridColumnSupplierCode.Visible = true;
            this.gridColumnSupplierCode.VisibleIndex = 1;
            // 
            // gridColumnSupplierTaxCode
            // 
            this.gridColumnSupplierTaxCode.Caption = "@@TaxCode";
            this.gridColumnSupplierTaxCode.FieldName = "Trader.TaxCode";
            this.gridColumnSupplierTaxCode.Name = "gridColumnSupplierTaxCode";
            this.gridColumnSupplierTaxCode.OptionsColumn.AllowEdit = false;
            this.gridColumnSupplierTaxCode.Visible = true;
            this.gridColumnSupplierTaxCode.VisibleIndex = 2;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textEditTaxCode;
            this.layoutControlItem3.Location = new System.Drawing.Point(439, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(180, 24);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(180, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(180, 34);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "@@ToDate";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(67, 13);
            // 
            // textEditTaxCode
            // 
            this.SetBoundFieldName(this.textEditTaxCode, "ToDate");
            this.SetBoundPropertyName(this.textEditTaxCode, "EditValue");
            this.textEditTaxCode.EditValue = null;
            this.textEditTaxCode.Location = new System.Drawing.Point(522, 12);
            this.textEditTaxCode.Name = "textEditTaxCode";
            this.textEditTaxCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditTaxCode.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditTaxCode.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.textEditTaxCode.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditTaxCode.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.textEditTaxCode.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditTaxCode.Properties.Mask.EditMask = "";
            this.textEditTaxCode.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditTaxCode.Size = new System.Drawing.Size(105, 20);
            this.textEditTaxCode.StyleController = this.layoutControl1;
            this.textEditTaxCode.TabIndex = 7;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEditCompanyName;
            this.layoutControlItem2.Location = new System.Drawing.Point(259, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(180, 24);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(180, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(180, 34);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "@@FromDate";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(67, 13);
            // 
            // textEditCompanyName
            // 
            this.SetBoundFieldName(this.textEditCompanyName, "FromDate");
            this.SetBoundPropertyName(this.textEditCompanyName, "EditValue");
            this.textEditCompanyName.EditValue = null;
            this.textEditCompanyName.Location = new System.Drawing.Point(342, 12);
            this.textEditCompanyName.Name = "textEditCompanyName";
            this.textEditCompanyName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditCompanyName.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditCompanyName.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.textEditCompanyName.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditCompanyName.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.textEditCompanyName.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditCompanyName.Properties.Mask.EditMask = "";
            this.textEditCompanyName.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditCompanyName.Size = new System.Drawing.Size(105, 20);
            this.textEditCompanyName.StyleController = this.layoutControl1;
            this.textEditCompanyName.TabIndex = 6;
            // 
            // PurchaseDocumentSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PurchaseDocumentSecondaryFilterControl";
            this.Size = new System.Drawing.Size(769, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.DateEdit textEditTaxCode;
        private DevExpress.XtraEditors.DateEdit textEditCompanyName;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEditDocumentSupplierFilter;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSupplierDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSupplierCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSupplierTaxCode;
    }
}
