namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class ProformaDocumentSecondaryFilterControl
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
            this.searchLookUpEditDocumentCustomerFilter = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTaxCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditCompanyName = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditTaxCode = new DevExpress.XtraEditors.DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentCustomerFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.searchLookUpEditDocumentCustomerFilter);
            this.layoutControl1.Controls.Add(this.textEditTaxCode);
            this.layoutControl1.Controls.Add(this.textEditCompanyName);
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
            this.layoutControlItem1.Control = this.searchLookUpEditDocumentCustomerFilter;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(259, 34);
            this.layoutControlItem1.Text = "@@Customer";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(67, 13);
            // 
            // searchLookUpEditDocumentCustomerFilter
            // 
            this.SetBoundFieldName(this.searchLookUpEditDocumentCustomerFilter, "Customer");
            this.SetBoundPropertyName(this.searchLookUpEditDocumentCustomerFilter, "EditValue");
            this.searchLookUpEditDocumentCustomerFilter.Location = new System.Drawing.Point(83, 12);
            this.searchLookUpEditDocumentCustomerFilter.Name = "searchLookUpEditDocumentCustomerFilter";
            this.searchLookUpEditDocumentCustomerFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.searchLookUpEditDocumentCustomerFilter.Properties.NullText = "";
            this.searchLookUpEditDocumentCustomerFilter.Properties.View = this.searchLookUpEdit1View;
            this.searchLookUpEditDocumentCustomerFilter.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.searchLookUpEditDocumentCustomerFilter_Properties_ButtonClick);
            this.searchLookUpEditDocumentCustomerFilter.Size = new System.Drawing.Size(184, 20);
            this.searchLookUpEditDocumentCustomerFilter.StyleController = this.layoutControl1;
            this.searchLookUpEditDocumentCustomerFilter.TabIndex = 5;
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDescription,
            this.gridColumnCode,
            this.gridColumnTaxCode});
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnDescription
            // 
            this.gridColumnDescription.Caption = "@@Description";
            this.gridColumnDescription.FieldName = "CompanyName";
            this.gridColumnDescription.Name = "gridColumnDescription";
            this.gridColumnDescription.OptionsColumn.AllowEdit = false;
            this.gridColumnDescription.Visible = true;
            this.gridColumnDescription.VisibleIndex = 0;
            // 
            // gridColumnCode
            // 
            this.gridColumnCode.Caption = "@@Code";
            this.gridColumnCode.FieldName = "Code";
            this.gridColumnCode.Name = "gridColumnCode";
            this.gridColumnCode.OptionsColumn.AllowEdit = false;
            this.gridColumnCode.Visible = true;
            this.gridColumnCode.VisibleIndex = 1;
            // 
            // gridColumnTaxCode
            // 
            this.gridColumnTaxCode.Caption = "@@TaxCode";
            this.gridColumnTaxCode.FieldName = "Trader.TaxCode";
            this.gridColumnTaxCode.Name = "gridColumnTaxCode";
            this.gridColumnTaxCode.OptionsColumn.AllowEdit = false;
            this.gridColumnTaxCode.Visible = true;
            this.gridColumnTaxCode.VisibleIndex = 2;
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
            // ProformaDocumentSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ProformaDocumentSecondaryFilterControl";
            this.Size = new System.Drawing.Size(769, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentCustomerFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEditDocumentCustomerFilter;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.DateEdit textEditTaxCode;
        private DevExpress.XtraEditors.DateEdit textEditCompanyName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTaxCode;
    }
}
