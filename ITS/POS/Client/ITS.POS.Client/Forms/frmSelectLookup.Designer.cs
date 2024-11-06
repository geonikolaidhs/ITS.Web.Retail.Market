namespace ITS.POS.Client.Forms
{
    partial class frmSelectLookup
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
            DevExpress.XtraGrid.Views.Grid.GridView grvLookupList;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectLookup));
            this.grcLookupList = new DevExpress.XtraGrid.GridControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            grvLookupList = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(grvLookupList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcLookupList)).BeginInit();
            this.SuspendLayout();
            // 
            // grvLookupList
            // 
            grvLookupList.Appearance.EvenRow.Font = new System.Drawing.Font("Tahoma", 12F);
            grvLookupList.Appearance.EvenRow.Options.UseFont = true;
            grvLookupList.Appearance.FocusedCell.BackColor = System.Drawing.SystemColors.ActiveCaption;
            grvLookupList.Appearance.FocusedCell.Options.UseBackColor = true;
            grvLookupList.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.ActiveBorder;
            grvLookupList.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 12F);
            grvLookupList.Appearance.FocusedRow.Options.UseBackColor = true;
            grvLookupList.Appearance.OddRow.Font = new System.Drawing.Font("Tahoma", 12F);
            grvLookupList.Appearance.OddRow.Options.UseFont = true;
            grvLookupList.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            grvLookupList.Appearance.Row.Options.UseFont = true;
            grvLookupList.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            grvLookupList.GridControl = this.grcLookupList;
            grvLookupList.GroupFormat = "";
            grvLookupList.Name = "grvLookupList";
            grvLookupList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            grvLookupList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            grvLookupList.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            grvLookupList.OptionsBehavior.AutoPopulateColumns = false;
            grvLookupList.OptionsBehavior.AutoSelectAllInEditor = false;
            grvLookupList.OptionsBehavior.AutoUpdateTotalSummary = false;
            grvLookupList.OptionsBehavior.Editable = false;
            grvLookupList.OptionsBehavior.ReadOnly = true;
            grvLookupList.OptionsCustomization.AllowColumnMoving = false;
            grvLookupList.OptionsCustomization.AllowColumnResizing = false;
            grvLookupList.OptionsCustomization.AllowFilter = false;
            grvLookupList.OptionsCustomization.AllowGroup = false;
            grvLookupList.OptionsCustomization.AllowQuickHideColumns = false;
            grvLookupList.OptionsCustomization.AllowSort = false;
            grvLookupList.OptionsDetail.EnableMasterViewMode = false;
            grvLookupList.OptionsFilter.AllowFilterEditor = false;
            grvLookupList.OptionsMenu.EnableColumnMenu = false;
            grvLookupList.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvLookupList.OptionsView.ShowColumnHeaders = false;
            grvLookupList.OptionsView.ShowDetailButtons = false;
            grvLookupList.OptionsView.ShowGroupPanel = false;
            grvLookupList.OptionsView.ShowIndicator = false;
            grvLookupList.RowHeight = 40;
            // 
            // grcLookupList
            // 
            this.grcLookupList.Font = new System.Drawing.Font("Tahoma", 12F);
            this.grcLookupList.Location = new System.Drawing.Point(13, 40);
            this.grcLookupList.MainView = grvLookupList;
            this.grcLookupList.Name = "grcLookupList";
            this.grcLookupList.Size = new System.Drawing.Size(301, 246);
            this.grcLookupList.TabIndex = 0;
            this.grcLookupList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            grvLookupList});
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnClose.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnClose.Location = new System.Drawing.Point(213, 290);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 59);
            this.btnClose.TabIndex = 2;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitle.Location = new System.Drawing.Point(13, 13);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(48, 19);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "lblTitle";
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnOK.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(12, 290);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 59);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmSelectLookup
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Tile;
            this.BackgroundImageStore = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImageStore")));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(326, 361);
            this.Controls.Add(this.grcLookupList);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnClose);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSelectLookup";
            this.Text = "frmSelectPaymentMethod";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSelectLookup_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(grvLookupList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcLookupList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraGrid.GridControl grcLookupList;
    }
}