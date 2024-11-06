namespace ITS.POS.Client.Forms
{
    partial class frmReprintZ
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
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.lblFromFilter = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.edtFromZ = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblToFilter = new DevExpress.XtraEditors.LabelControl();
            this.edtToZ = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.edtFromDate = new DevExpress.XtraEditors.DateEdit();
            this.edtToDate = new DevExpress.XtraEditors.DateEdit();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromZ.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToZ.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToDate.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(12, 154);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 59);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            // 
            // lblFromFilter
            // 
            this.lblFromFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblFromFilter.Location = new System.Drawing.Point(12, 40);
            this.lblFromFilter.LookAndFeel.SkinName = "Metropolis";
            this.lblFromFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblFromFilter.Name = "lblFromFilter";
            this.lblFromFilter.Size = new System.Drawing.Size(72, 19);
            this.lblFromFilter.TabIndex = 12;
            this.lblFromFilter.Text = "FromFilter";
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnClose.Location = new System.Drawing.Point(177, 154);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 59);
            this.btnClose.TabIndex = 10;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            // 
            // edtFromZ
            // 
            this.edtFromZ.AutoHideTouchPad = true;
            this.edtFromZ.EditValue = "1";
            this.edtFromZ.Location = new System.Drawing.Point(12, 65);
            this.edtFromZ.Name = "edtFromZ";
            this.edtFromZ.PoleDisplayName = "";
            this.edtFromZ.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtFromZ.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtFromZ.Properties.Appearance.Options.UseBackColor = true;
            this.edtFromZ.Properties.Appearance.Options.UseFont = true;
            this.edtFromZ.Properties.Mask.EditMask = "d";
            this.edtFromZ.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.edtFromZ.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.edtFromZ.Size = new System.Drawing.Size(266, 26);
            this.edtFromZ.TabIndex = 14;
            this.edtFromZ.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtFromZ_KeyDown);
            // 
            // lblToFilter
            // 
            this.lblToFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblToFilter.Location = new System.Drawing.Point(12, 97);
            this.lblToFilter.LookAndFeel.SkinName = "Metropolis";
            this.lblToFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblToFilter.Name = "lblToFilter";
            this.lblToFilter.Size = new System.Drawing.Size(54, 19);
            this.lblToFilter.TabIndex = 15;
            this.lblToFilter.Text = "ToFilter";
            // 
            // edtToZ
            // 
            this.edtToZ.AutoHideTouchPad = true;
            this.edtToZ.EditValue = "2";
            this.edtToZ.Location = new System.Drawing.Point(12, 122);
            this.edtToZ.Name = "edtToZ";
            this.edtToZ.PoleDisplayName = "";
            this.edtToZ.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtToZ.Properties.Appearance.Options.UseFont = true;
            this.edtToZ.Properties.Mask.EditMask = "d";
            this.edtToZ.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.edtToZ.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.edtToZ.Size = new System.Drawing.Size(266, 26);
            this.edtToZ.TabIndex = 16;
            this.edtToZ.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtToZ_KeyDown);
            // 
            // edtFromDate
            // 
            this.edtFromDate.EditValue = null;
            this.edtFromDate.Location = new System.Drawing.Point(12, 67);
            this.edtFromDate.Name = "edtFromDate";
            this.edtFromDate.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtFromDate.Properties.Appearance.Options.UseBackColor = true;
            this.edtFromDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.edtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.edtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.edtFromDate.Size = new System.Drawing.Size(266, 20);
            this.edtFromDate.TabIndex = 17;
            this.edtFromDate.Visible = false;
            this.edtFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtFromDate_KeyDown);
            // 
            // edtToDate
            // 
            this.edtToDate.EditValue = null;
            this.edtToDate.Location = new System.Drawing.Point(13, 125);
            this.edtToDate.Name = "edtToDate";
            this.edtToDate.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtToDate.Properties.Appearance.Options.UseBackColor = true;
            this.edtToDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.edtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.edtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.edtToDate.Size = new System.Drawing.Size(265, 20);
            this.edtToDate.TabIndex = 18;
            this.edtToDate.Visible = false;
            this.edtToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtToDate_KeyDown);
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 6);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(34, 19);
            this.lblTitle.TabIndex = 19;
            this.lblTitle.Text = "title";
            // 
            // frmReprintZ
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(290, 219);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.edtToDate);
            this.Controls.Add(this.edtFromDate);
            this.Controls.Add(this.edtToZ);
            this.Controls.Add(this.lblToFilter);
            this.Controls.Add(this.edtFromZ);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblFromFilter);
            this.Controls.Add(this.btnClose);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmReprintZ";
            this.Text = "frmReprintZ";
            this.Load += new System.EventHandler(this.frmReprintZ_Load);
            this.Shown += new System.EventHandler(this.frmReprintZ_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.edtFromZ.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToZ.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtToDate.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.LabelControl lblFromFilter;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private UserControls.ucTouchFriendlyInput edtFromZ;
        private DevExpress.XtraEditors.LabelControl lblToFilter;
        private UserControls.ucTouchFriendlyInput edtToZ;
        private DevExpress.XtraEditors.DateEdit edtFromDate;
        private DevExpress.XtraEditors.DateEdit edtToDate;
        private DevExpress.XtraEditors.LabelControl lblTitle;
    }
}