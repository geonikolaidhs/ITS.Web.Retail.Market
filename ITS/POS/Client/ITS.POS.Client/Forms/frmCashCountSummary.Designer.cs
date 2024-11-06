namespace ITS.POS.Client.Forms
{
    partial class frmCashCountSummary
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnOk2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel2 = new DevExpress.XtraEditors.SimpleButton();
            this.grdMain = new DevExpress.XtraGrid.GridControl();
            this.grvMain = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.rpsDelButton = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridViewDocumentTotals = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnPaymentMethod = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnOid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblSummary = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpsDelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "Column3";
            this.dataGridViewImageColumn1.Image = global::ITS.POS.Client.Properties.Resources.minus_32;
            this.dataGridViewImageColumn1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            // 
            // btnOk2
            // 
            this.btnOk2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk2.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOk2.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOk2.Appearance.Options.UseBackColor = true;
            this.btnOk2.Appearance.Options.UseBorderColor = true;
            this.btnOk2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk2.Image = global::ITS.POS.Client.Properties.Resources.correct_64;
            this.btnOk2.Location = new System.Drawing.Point(295, 518);
            this.btnOk2.LookAndFeel.SkinName = "Metropolis";
            this.btnOk2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOk2.Name = "btnOk2";
            this.btnOk2.Size = new System.Drawing.Size(200, 60);
            this.btnOk2.TabIndex = 93;
            this.btnOk2.Text = "Ακύρωση";
            // 
            // btnCancel2
            // 
            this.btnCancel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel2.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel2.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel2.Appearance.Options.UseBackColor = true;
            this.btnCancel2.Appearance.Options.UseBorderColor = true;
            this.btnCancel2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel2.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel2.Location = new System.Drawing.Point(12, 518);
            this.btnCancel2.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(194, 60);
            this.btnCancel2.TabIndex = 86;
            this.btnCancel2.Text = "Ακύρωση";
            // 
            // grdMain
            // 
            this.grdMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMain.Font = new System.Drawing.Font("Tahoma", 10F);
            this.grdMain.Location = new System.Drawing.Point(12, 91);
            this.grdMain.MainView = this.grvMain;
            this.grdMain.Name = "grdMain";
            this.grdMain.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.rpsDelButton});
            this.grdMain.Size = new System.Drawing.Size(483, 421);
            this.grdMain.TabIndex = 114;
            this.grdMain.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvMain,
            this.gridViewDocumentTotals});
            // 
            // grvMain
            // 
            this.grvMain.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.FocusedRow.Options.UseFont = true;
            this.grvMain.Appearance.FooterPanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Maroon;
            this.grvMain.Appearance.FooterPanel.Options.UseFont = true;
            this.grvMain.Appearance.FooterPanel.Options.UseForeColor = true;
            this.grvMain.Appearance.GroupFooter.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.GroupFooter.ForeColor = System.Drawing.Color.Navy;
            this.grvMain.Appearance.GroupFooter.Options.UseFont = true;
            this.grvMain.Appearance.GroupFooter.Options.UseForeColor = true;
            this.grvMain.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.HeaderPanel.Options.UseFont = true;
            this.grvMain.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.Row.Options.UseFont = true;
            this.grvMain.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn1});
            this.grvMain.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.grvMain.GridControl = this.grdMain;
            this.grvMain.IndicatorWidth = 40;
            this.grvMain.Name = "grvMain";
            this.grvMain.OptionsBehavior.ReadOnly = true;
            this.grvMain.OptionsCustomization.AllowColumnMoving = false;
            this.grvMain.OptionsCustomization.AllowFilter = false;
            this.grvMain.OptionsCustomization.AllowGroup = false;
            this.grvMain.OptionsCustomization.AllowQuickHideColumns = false;
            this.grvMain.OptionsCustomization.AllowSort = false;
            this.grvMain.OptionsDetail.EnableMasterViewMode = false;
            this.grvMain.OptionsView.ShowGroupPanel = false;
            this.grvMain.RowHeight = 40;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Τύπος";
            this.gridColumn3.FieldName = "Description";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 266;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.Caption = "Αξία";
            this.gridColumn1.DisplayFormat.FormatString = "n2";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn1.FieldName = "Amount";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 175;
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
            // rpsDelButton
            // 
            this.rpsDelButton.AutoHeight = false;
            this.rpsDelButton.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, global::ITS.POS.Client.Properties.Resources.trash_32, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject6, "", null, null, true)});
            this.rpsDelButton.Name = "rpsDelButton";
            this.rpsDelButton.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
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
            this.gridViewDocumentTotals.GridControl = this.grdMain;
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
            // lblSummary
            // 
            this.lblSummary.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblSummary.Location = new System.Drawing.Point(12, 9);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(483, 79);
            this.lblSummary.TabIndex = 115;
            this.lblSummary.Text = "-----Έχετε συμπληρώσει τα παρακάτω ποσά,  θέλετε να αποθηκευτούν;-------";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmCashCountSummary
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 600);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.grdMain);
            this.Controls.Add(this.btnCancel2);
            this.Controls.Add(this.btnOk2);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmCashCountSummary";
            this.Text = "frmCancelNotIncludedItems";
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpsDelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).EndInit();
            this.ResumeLayout(false);

        }


        #endregion
        public System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        public DevExpress.XtraEditors.SimpleButton btnOk2;
        public DevExpress.XtraEditors.SimpleButton btnCancel2;
        public DevExpress.XtraGrid.GridControl grdMain;
        public DevExpress.XtraGrid.Views.Grid.GridView grvMain;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rpsDelButton;
        public DevExpress.XtraGrid.Views.Grid.GridView gridViewDocumentTotals;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnPaymentMethod;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnAmount;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnOid;
        private System.Windows.Forms.Label lblSummary;
    }
}