namespace ITS.POS.Client.UserControls
{
    partial class ucCashNonMoneyInput
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.rpsDelButton = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.edtSummary = new DevExpress.XtraEditors.TextEdit();
            this.edtInput = new DevExpress.XtraEditors.TextEdit();
            this.grdMain = new DevExpress.XtraGrid.GridControl();
            this.grvMain = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridViewDocumentTotals = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnPaymentMethod = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnOid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ucPad = new ITS.POS.Client.UserControls.ucCashKeyPad();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.rpsDelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtSummary.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtInput.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).BeginInit();
            this.SuspendLayout();
            // 
            // rpsDelButton
            // 
            this.rpsDelButton.AutoHeight = false;
            this.rpsDelButton.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, global::ITS.POS.Client.Properties.Resources.trash_32, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.rpsDelButton.Name = "rpsDelButton";
            this.rpsDelButton.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.rpsDelButton.Click += new System.EventHandler(this.rpsDelButton_Click_1);
            // 
            // edtSummary
            // 
            this.edtSummary.EditValue = "";
            this.edtSummary.Enabled = false;
            this.edtSummary.Location = new System.Drawing.Point(3, 3);
            this.edtSummary.Name = "edtSummary";
            this.edtSummary.Properties.AllowFocused = false;
            this.edtSummary.Properties.AllowMouseWheel = false;
            this.edtSummary.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.edtSummary.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.edtSummary.Properties.Appearance.Options.UseBackColor = true;
            this.edtSummary.Properties.Appearance.Options.UseBorderColor = true;
            this.edtSummary.Properties.Appearance.Options.UseFont = true;
            this.edtSummary.Properties.Appearance.Options.UseTextOptions = true;
            this.edtSummary.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtSummary.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtSummary.Properties.AppearanceFocused.Options.UseTextOptions = true;
            this.edtSummary.Properties.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtSummary.Properties.AutoHeight = false;
            this.edtSummary.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
            this.edtSummary.Properties.Mask.ShowPlaceHolders = false;
            this.edtSummary.Properties.ReadOnly = true;
            this.edtSummary.Size = new System.Drawing.Size(387, 40);
            this.edtSummary.TabIndex = 115;
            // 
            // edtInput
            // 
            this.edtInput.EditValue = "";
            this.edtInput.Location = new System.Drawing.Point(395, 3);
            this.edtInput.Name = "edtInput";
            this.edtInput.Properties.AllowMouseWheel = false;
            this.edtInput.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.edtInput.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.edtInput.Properties.Appearance.Options.UseBackColor = true;
            this.edtInput.Properties.Appearance.Options.UseBorderColor = true;
            this.edtInput.Properties.Appearance.Options.UseFont = true;
            this.edtInput.Properties.Appearance.Options.UseTextOptions = true;
            this.edtInput.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtInput.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtInput.Properties.AppearanceFocused.Options.UseTextOptions = true;
            this.edtInput.Properties.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtInput.Properties.AutoHeight = false;
            this.edtInput.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
            this.edtInput.Properties.Mask.BeepOnError = true;
            this.edtInput.Properties.Mask.EditMask = "[0-9]+[.,]?[0-9]?[0-9]?";
            this.edtInput.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.edtInput.Properties.Mask.ShowPlaceHolders = false;
            this.edtInput.Size = new System.Drawing.Size(399, 40);
            this.edtInput.TabIndex = 114;
            this.edtInput.TextChanged += new System.EventHandler(this.edtInput_TextChanged);
            this.edtInput.Leave += new System.EventHandler(this.edtInput_Leave);
            this.edtInput.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.edtInput_PreviewKeyDown);
            // 
            // grdMain
            // 
            this.grdMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grdMain.Font = new System.Drawing.Font("Tahoma", 10F);
            this.grdMain.Location = new System.Drawing.Point(3, 49);
            this.grdMain.MainView = this.grvMain;
            this.grdMain.Name = "grdMain";
            this.grdMain.Size = new System.Drawing.Size(387, 539);
            this.grdMain.TabIndex = 113;
            this.grdMain.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvMain,
            this.gridViewDocumentTotals});
            // 
            // grvMain
            // 
            this.grvMain.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.grvMain.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.GreenYellow;
            this.grvMain.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.grvMain.Appearance.FocusedRow.Options.UseBackColor = true;
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
            this.gridColumn2,
            this.gridColumn1});
            this.grvMain.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.grvMain.GridControl = this.grdMain;
            this.grvMain.IndicatorWidth = 40;
            this.grvMain.Name = "grvMain";
            this.grvMain.OptionsCustomization.AllowColumnMoving = false;
            this.grvMain.OptionsCustomization.AllowFilter = false;
            this.grvMain.OptionsCustomization.AllowGroup = false;
            this.grvMain.OptionsCustomization.AllowQuickHideColumns = false;
            this.grvMain.OptionsCustomization.AllowSort = false;
            this.grvMain.OptionsView.RowAutoHeight = true;
            this.grvMain.OptionsView.ShowGroupPanel = false;
            this.grvMain.RowHeight = 40;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = " ";
            this.gridColumn2.ColumnEdit = this.rpsDelButton;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 50;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Αξία";
            this.gridColumn1.DisplayFormat.FormatString = "n2";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn1.FieldName = "Amount";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 204;
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
            // ucPad
            // 
            this.ucPad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ucPad.Appearance.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucPad.Appearance.Options.UseFont = true;
            this.ucPad.AutoSize = true;
            this.ucPad.Location = new System.Drawing.Point(396, 352);
            this.ucPad.Name = "ucPad";
            this.ucPad.ShowEnter = true;
            this.ucPad.Size = new System.Drawing.Size(392, 243);
            this.ucPad.TabIndex = 117;
            this.ucPad.KeyNotify += new System.EventHandler<ITS.POS.Client.UserControls.KeyNotifyEventArgs>(this.ucPad_KeyNotify);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOk.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOk.Appearance.Options.UseBackColor = true;
            this.btnOk.Appearance.Options.UseBorderColor = true;
            this.btnOk.Image = global::ITS.POS.Client.Properties.Resources.correct_64;
            this.btnOk.Location = new System.Drawing.Point(3, 526);
            this.btnOk.LookAndFeel.SkinName = "Metropolis";
            this.btnOk.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(387, 62);
            this.btnOk.TabIndex = 116;
            this.btnOk.Text = "Αποδοχή";
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Location = new System.Drawing.Point(397, 50);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(391, 296);
            this.lblDescription.TabIndex = 118;
            // 
            // ucCashNonMoneyInput
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.edtSummary);
            this.Controls.Add(this.edtInput);
            this.Controls.Add(this.grdMain);
            this.Controls.Add(this.ucPad);
            this.Name = "ucCashNonMoneyInput";
            this.Size = new System.Drawing.Size(800, 600);
            this.VisibleChanged += new System.EventHandler(this.ucCashNonMoneyInput_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.rpsDelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtSummary.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtInput.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocumentTotals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.TextEdit edtSummary;
        public DevExpress.XtraEditors.TextEdit edtInput;
        public DevExpress.XtraGrid.GridControl grdMain;
        public DevExpress.XtraGrid.Views.Grid.GridView grvMain;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraGrid.Views.Grid.GridView gridViewDocumentTotals;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnPaymentMethod;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnAmount;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumnOid;
        private ucCashKeyPad ucPad;
        public DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit rpsDelButton;
        public DevExpress.XtraEditors.LabelControl lblDescription;
    }
}
