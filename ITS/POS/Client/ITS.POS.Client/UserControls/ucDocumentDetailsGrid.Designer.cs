namespace ITS.POS.Client.UserControls
{
    partial class ucDocumentDetailsGrid
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
            this.gridViewDocumentDetails = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdDocumentDetails = new DevExpress.XtraGrid.GridControl();
            this.gridViewDocumentTotals = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnPaymentMethod = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnOid = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).BeginInit();
            this.SuspendLayout();
            // 
            // gridViewDocumentDetails
            // 
            this.gridViewDocumentDetails.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.gridViewDocumentDetails.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.GreenYellow;
            this.gridViewDocumentDetails.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentDetails.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDocumentDetails.Appearance.FocusedRow.Options.UseFont = true;
            this.gridViewDocumentDetails.Appearance.FooterPanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentDetails.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Maroon;
            this.gridViewDocumentDetails.Appearance.FooterPanel.Options.UseFont = true;
            this.gridViewDocumentDetails.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gridViewDocumentDetails.Appearance.GroupFooter.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentDetails.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Navy;
            this.gridViewDocumentDetails.Appearance.GroupFooter.Options.UseFont = true;
            this.gridViewDocumentDetails.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gridViewDocumentDetails.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentDetails.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDocumentDetails.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn4,
            this.gridColumn6,
            this.gridColumn3,
            this.gridColumn5});
            this.gridViewDocumentDetails.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDocumentDetails.GridControl = this.grdDocumentDetails;
            this.gridViewDocumentDetails.IndicatorWidth = 40;
            this.gridViewDocumentDetails.Name = "gridViewDocumentDetails";
            this.gridViewDocumentDetails.OptionsBehavior.Editable = false;
            this.gridViewDocumentDetails.OptionsBehavior.ReadOnly = true;
            this.gridViewDocumentDetails.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewDocumentDetails.OptionsCustomization.AllowFilter = false;
            this.gridViewDocumentDetails.OptionsCustomization.AllowGroup = false;
            this.gridViewDocumentDetails.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridViewDocumentDetails.OptionsCustomization.AllowSort = false;
            this.gridViewDocumentDetails.OptionsDetail.EnableMasterViewMode = false;
            this.gridViewDocumentDetails.OptionsView.ShowFooter = true;
            this.gridViewDocumentDetails.OptionsView.ShowGroupPanel = false;
            this.gridViewDocumentDetails.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewDocumentDetails_CustomDrawRowIndicator);
            this.gridViewDocumentDetails.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView_FocusedRowChanged);
            this.gridViewDocumentDetails.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewDocumentDetails_CustomColumnDisplayText);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Κωδ.";
            this.gridColumn1.FieldName = "ItemCode";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 110;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Περιγραφή";
            this.gridColumn2.FieldName = "CustomDescription";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 287;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Ποσ.";
            this.gridColumn4.FieldName = "Qty";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            this.gridColumn4.Width = 72;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Εκπτ.";
            this.gridColumn6.ColumnEdit = this.repositoryItemTextEdit1;
            this.gridColumn6.FieldName = "TotalDiscountPercentageWithVat";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            this.gridColumn6.Width = 92;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.DisplayFormat.FormatString = "p";
            this.repositoryItemTextEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemTextEdit1.Mask.EditMask = "p";
            this.repositoryItemTextEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Αξία";
            this.gridColumn3.FieldName = "GrossTotal";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Oid";
            this.gridColumn5.FieldName = "Oid";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.ShowInCustomizationForm = false;
            // 
            // grdDocumentDetails
            // 
            this.grdDocumentDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDocumentDetails.Font = new System.Drawing.Font("Tahoma", 10F);
            this.grdDocumentDetails.Location = new System.Drawing.Point(0, 0);
            this.grdDocumentDetails.MainView = this.gridViewDocumentDetails;
            this.grdDocumentDetails.Name = "grdDocumentDetails";
            this.grdDocumentDetails.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1});
            this.grdDocumentDetails.Size = new System.Drawing.Size(544, 400);
            this.grdDocumentDetails.TabIndex = 3;
            this.grdDocumentDetails.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDocumentDetails,
            this.gridViewDocumentTotals});
            // 
            // gridViewDocumentTotals
            // 
            this.gridViewDocumentTotals.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.gridViewDocumentTotals.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.GreenYellow;
            this.gridViewDocumentTotals.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentTotals.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDocumentTotals.Appearance.FocusedRow.Options.UseFont = true;
            this.gridViewDocumentTotals.Appearance.FooterPanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentTotals.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Maroon;
            this.gridViewDocumentTotals.Appearance.FooterPanel.Options.UseFont = true;
            this.gridViewDocumentTotals.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gridViewDocumentTotals.Appearance.GroupFooter.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentTotals.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Navy;
            this.gridViewDocumentTotals.Appearance.GroupFooter.Options.UseFont = true;
            this.gridViewDocumentTotals.Appearance.GroupFooter.Options.UseForeColor = true;
            this.gridViewDocumentTotals.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridViewDocumentTotals.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDocumentTotals.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnPaymentMethod,
            this.gridColumnAmount,
            this.gridColumnOid});
            this.gridViewDocumentTotals.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDocumentTotals.GridControl = this.grdDocumentDetails;
            this.gridViewDocumentTotals.Name = "gridViewDocumentTotals";
            this.gridViewDocumentTotals.OptionsBehavior.Editable = false;
            this.gridViewDocumentTotals.OptionsBehavior.ReadOnly = true;
            this.gridViewDocumentTotals.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewDocumentTotals.OptionsCustomization.AllowFilter = false;
            this.gridViewDocumentTotals.OptionsCustomization.AllowGroup = false;
            this.gridViewDocumentTotals.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridViewDocumentTotals.OptionsCustomization.AllowSort = false;
            this.gridViewDocumentTotals.OptionsDetail.EnableMasterViewMode = false;
            this.gridViewDocumentTotals.OptionsView.ShowFooter = true;
            this.gridViewDocumentTotals.OptionsView.ShowGroupPanel = false;
            this.gridViewDocumentTotals.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewDocumentTotals_FocusedRowChanged);
            // 
            // gridColumnPaymentMethod
            // 
            this.gridColumnPaymentMethod.Caption = "Τρόπος Πληρωμής";
            this.gridColumnPaymentMethod.FieldName = "PaymentMethodDescription";
            this.gridColumnPaymentMethod.Name = "gridColumnPaymentMethod";
            this.gridColumnPaymentMethod.Visible = true;
            this.gridColumnPaymentMethod.VisibleIndex = 0;
            this.gridColumnPaymentMethod.Width = 346;
            // 
            // gridColumnAmount
            // 
            this.gridColumnAmount.Caption = "Ποσό";
            this.gridColumnAmount.FieldName = "Amount";
            this.gridColumnAmount.Name = "gridColumnAmount";
            this.gridColumnAmount.Visible = true;
            this.gridColumnAmount.VisibleIndex = 1;
            this.gridColumnAmount.Width = 180;
            // 
            // gridColumnOid
            // 
            this.gridColumnOid.Caption = "Oid";
            this.gridColumnOid.FieldName = "Oid";
            this.gridColumnOid.Name = "gridColumnOid";
            // 
            // ucDocumentDetailsGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grdDocumentDetails);
            this.Name = "ucDocumentDetailsGrid";
            this.Size = new System.Drawing.Size(544, 400);
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraGrid.Views.Grid.GridView gridViewDocumentDetails;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        public DevExpress.XtraGrid.GridControl grdDocumentDetails;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        public DevExpress.XtraGrid.Views.Grid.GridView gridViewDocumentTotals;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnPaymentMethod;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnAmount;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnOid;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;


    }
}
