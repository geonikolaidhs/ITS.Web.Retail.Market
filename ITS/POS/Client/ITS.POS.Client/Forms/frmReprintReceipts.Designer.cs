namespace ITS.POS.Client.Forms
{
    partial class frmReprintReceipts
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
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.edtToReceipt = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblToFilter = new DevExpress.XtraEditors.LabelControl();
            this.edtFromReceipt = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.lblFromFilter = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.edtToReceipt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromReceipt.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(34, 19);
            this.lblTitle.TabIndex = 28;
            this.lblTitle.Text = "title";
            // 
            // edtToReceipt
            // 
            this.edtToReceipt.AutoHideTouchPad = true;
            this.edtToReceipt.EditValue = "2";
            this.edtToReceipt.Location = new System.Drawing.Point(12, 125);
            this.edtToReceipt.Name = "edtToReceipt";
            this.edtToReceipt.PoleDisplayName = "";
            this.edtToReceipt.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtToReceipt.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtToReceipt.Properties.Appearance.Options.UseBackColor = true;
            this.edtToReceipt.Properties.Appearance.Options.UseFont = true;
            this.edtToReceipt.Properties.Mask.EditMask = "d";
            this.edtToReceipt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.edtToReceipt.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.edtToReceipt.Size = new System.Drawing.Size(266, 26);
            this.edtToReceipt.TabIndex = 25;
            this.edtToReceipt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtToReceipt_KeyDown);
            // 
            // lblToFilter
            // 
            this.lblToFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblToFilter.Location = new System.Drawing.Point(12, 100);
            this.lblToFilter.LookAndFeel.SkinName = "Metropolis";
            this.lblToFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblToFilter.Name = "lblToFilter";
            this.lblToFilter.Size = new System.Drawing.Size(54, 19);
            this.lblToFilter.TabIndex = 24;
            this.lblToFilter.Text = "ToFilter";
            // 
            // edtFromReceipt
            // 
            this.edtFromReceipt.AutoHideTouchPad = true;
            this.edtFromReceipt.EditValue = "1";
            this.edtFromReceipt.Location = new System.Drawing.Point(12, 68);
            this.edtFromReceipt.Name = "edtFromReceipt";
            this.edtFromReceipt.PoleDisplayName = "";
            this.edtFromReceipt.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtFromReceipt.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtFromReceipt.Properties.Appearance.Options.UseBackColor = true;
            this.edtFromReceipt.Properties.Appearance.Options.UseFont = true;
            this.edtFromReceipt.Properties.Mask.EditMask = "d";
            this.edtFromReceipt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.edtFromReceipt.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.edtFromReceipt.Size = new System.Drawing.Size(266, 26);
            this.edtFromReceipt.TabIndex = 23;
            this.edtFromReceipt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtFromReceipt_KeyDown);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(12, 158);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 59);
            this.btnOK.TabIndex = 22;
            this.btnOK.Text = "OK";
            // 
            // lblFromFilter
            // 
            this.lblFromFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblFromFilter.Location = new System.Drawing.Point(12, 43);
            this.lblFromFilter.LookAndFeel.SkinName = "Metropolis";
            this.lblFromFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblFromFilter.Name = "lblFromFilter";
            this.lblFromFilter.Size = new System.Drawing.Size(72, 19);
            this.lblFromFilter.TabIndex = 21;
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
            this.btnClose.Location = new System.Drawing.Point(177, 158);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 59);
            this.btnClose.TabIndex = 20;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            // 
            // frmReprintReceipts
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(284, 230);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.edtToReceipt);
            this.Controls.Add(this.lblToFilter);
            this.Controls.Add(this.edtFromReceipt);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblFromFilter);
            this.Controls.Add(this.btnClose);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmReprintReceipts";
            this.Text = "frmReprintReceipts";
            this.Shown += new System.EventHandler(this.frmReprintReceipts_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.edtToReceipt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFromReceipt.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitle;
        private UserControls.ucTouchFriendlyInput edtToReceipt;
        private DevExpress.XtraEditors.LabelControl lblToFilter;
        private UserControls.ucTouchFriendlyInput edtFromReceipt;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.LabelControl lblFromFilter;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}