namespace ITS.POS.Client.Forms
{
    partial class frmDocumentsOnHold
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDocumentsOnHold));
            this.grdDocumentsOnHold = new DevExpress.XtraGrid.GridControl();
            this.grdViewDocumentsOnHold = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentsOnHold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewDocumentsOnHold)).BeginInit();
            this.SuspendLayout();
            // 
            // grdDocumentsOnHold
            // 
            this.grdDocumentsOnHold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDocumentsOnHold.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdDocumentsOnHold.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.grdDocumentsOnHold.Location = new System.Drawing.Point(13, 13);
            this.grdDocumentsOnHold.LookAndFeel.SkinName = "Metropolis";
            this.grdDocumentsOnHold.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grdDocumentsOnHold.MainView = this.grdViewDocumentsOnHold;
            this.grdDocumentsOnHold.Name = "grdDocumentsOnHold";
            this.grdDocumentsOnHold.Size = new System.Drawing.Size(471, 223);
            this.grdDocumentsOnHold.TabIndex = 0;
            this.grdDocumentsOnHold.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewDocumentsOnHold});
            // 
            // grdViewDocumentsOnHold
            // 
            this.grdViewDocumentsOnHold.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.grdViewDocumentsOnHold.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdViewDocumentsOnHold.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 12F);
            this.grdViewDocumentsOnHold.Appearance.Row.Options.UseFont = true;
            this.grdViewDocumentsOnHold.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.grdViewDocumentsOnHold.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.grdViewDocumentsOnHold.GridControl = this.grdDocumentsOnHold;
            this.grdViewDocumentsOnHold.Name = "grdViewDocumentsOnHold";
            this.grdViewDocumentsOnHold.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewDocumentsOnHold.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewDocumentsOnHold.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewDocumentsOnHold.OptionsBehavior.Editable = false;
            this.grdViewDocumentsOnHold.OptionsBehavior.ReadOnly = true;
            this.grdViewDocumentsOnHold.OptionsCustomization.AllowColumnMoving = false;
            this.grdViewDocumentsOnHold.OptionsCustomization.AllowFilter = false;
            this.grdViewDocumentsOnHold.OptionsCustomization.AllowGroup = false;
            this.grdViewDocumentsOnHold.OptionsCustomization.AllowQuickHideColumns = false;
            this.grdViewDocumentsOnHold.OptionsCustomization.AllowSort = false;
            this.grdViewDocumentsOnHold.OptionsDetail.EnableMasterViewMode = false;
            this.grdViewDocumentsOnHold.OptionsFilter.AllowFilterEditor = false;
            this.grdViewDocumentsOnHold.OptionsView.EnableAppearanceEvenRow = true;
            this.grdViewDocumentsOnHold.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Ημερομηνία";
            this.gridColumn1.DisplayFormat.FormatString = "g";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn1.FieldName = "UpdatedOn";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn1.OptionsColumn.AllowMove = false;
            this.gridColumn1.OptionsColumn.AllowSize = false;
            this.gridColumn1.OptionsFilter.AllowAutoFilter = false;
            this.gridColumn1.OptionsFilter.AllowFilter = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 276;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Ποσό";
            this.gridColumn2.DisplayFormat.FormatString = "c2";
            this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn2.FieldName = "GrossTotalBeforeDocumentDiscount";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 287;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnClose.Location = new System.Drawing.Point(383, 242);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 59);
            this.btnClose.TabIndex = 6;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(13, 241);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 59);
            this.btnOK.TabIndex = 7;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmDocumentsOnHold
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(497, 312);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grdDocumentsOnHold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmDocumentsOnHold";
            this.Text = "frmDocumentsOnHold";
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentsOnHold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewDocumentsOnHold)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdDocumentsOnHold;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewDocumentsOnHold;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnOK;
    }
}