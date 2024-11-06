namespace ITS.POS.Client.Forms
{
    partial class frmInstallments
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
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.edtInstallmetns = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.edtInstallmetns.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(12, 67);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 57);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel.Location = new System.Drawing.Point(175, 67);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(109, 57);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Ακύρωση";
            // 
            // edtInstallmetns
            // 
            this.edtInstallmetns.AutoHideTouchPad = false;
            this.edtInstallmetns.Location = new System.Drawing.Point(12, 35);
            this.edtInstallmetns.Name = "edtInstallmetns";
            this.edtInstallmetns.PoleDisplayName = "";
            this.edtInstallmetns.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtInstallmetns.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtInstallmetns.Properties.Appearance.Options.UseBackColor = true;
            this.edtInstallmetns.Properties.Appearance.Options.UseFont = true;
            this.edtInstallmetns.Properties.Appearance.Options.UseTextOptions = true;
            this.edtInstallmetns.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtInstallmetns.Properties.DisplayFormat.FormatString = "0";
            this.edtInstallmetns.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.edtInstallmetns.Properties.EditFormat.FormatString = "0";
            this.edtInstallmetns.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.edtInstallmetns.Size = new System.Drawing.Size(272, 26);
            this.edtInstallmetns.TabIndex = 0;
            this.edtInstallmetns.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtInstallmetns_KeyDown);
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(47, 19);
            this.lblTitle.TabIndex = 9;
            this.lblTitle.Text = "Δόσεις";
            // 
            // frmInstallments
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 139);
            this.Controls.Add(this.edtInstallmetns);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmInstallments";
            this.Text = "frmInstallments";
            ((System.ComponentModel.ISupportInitialize)(this.edtInstallmetns.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.SimpleButton btnOK;
        public DevExpress.XtraEditors.SimpleButton btnCancel;
        private UserControls.ucTouchFriendlyInput edtInstallmetns;
        private DevExpress.XtraEditors.LabelControl lblTitle;
    }
}