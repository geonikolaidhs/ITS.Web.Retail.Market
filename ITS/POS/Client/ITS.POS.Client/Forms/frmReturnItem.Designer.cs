namespace ITS.POS.Client.Forms
{
    partial class frmReturnItem
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
            this.edtItemCode = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.ucLineQuantity = new ITS.POS.Client.UserControls.ucLineQuantity();
            ((System.ComponentModel.ISupportInitialize)(this.edtItemCode.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitle.Location = new System.Drawing.Point(9, 47);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(172, 19);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Κωδικός/Barcode Είδους";
            // 
            // edtItemCode
            // 
            this.edtItemCode.AutoHideTouchPad = false;
            this.edtItemCode.Location = new System.Drawing.Point(9, 72);
            this.edtItemCode.Name = "edtItemCode";
            this.edtItemCode.PoleDisplayName = "";
            this.edtItemCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtItemCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtItemCode.Properties.Appearance.Options.UseBackColor = true;
            this.edtItemCode.Properties.Appearance.Options.UseFont = true;
            this.edtItemCode.Size = new System.Drawing.Size(306, 26);
            this.edtItemCode.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(9, 110);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 80);
            this.btnOK.TabIndex = 5;
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
            this.btnCancel.Location = new System.Drawing.Point(279, 110);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(109, 80);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Ακύρωση";
            // 
            // ucLineQuantity
            // 
            this.ucLineQuantity.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.ucLineQuantity.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucLineQuantity.Appearance.Options.UseBackColor = true;
            this.ucLineQuantity.Appearance.Options.UseFont = true;
            this.ucLineQuantity.Location = new System.Drawing.Point(324, 63);
            this.ucLineQuantity.LookAndFeel.SkinName = "Metropolis";
            this.ucLineQuantity.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucLineQuantity.Name = "ucLineQuantity";
            this.ucLineQuantity.Size = new System.Drawing.Size(64, 41);
            this.ucLineQuantity.TabIndex = 7;
            // 
            // frmReturnItem
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(397, 202);
            this.Controls.Add(this.ucLineQuantity);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.edtItemCode);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmReturnItem";
            this.Text = "frmReturnItem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmReturnItem_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.edtItemCode.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitle;
        private UserControls.ucTouchFriendlyInput edtItemCode;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        public DevExpress.XtraEditors.SimpleButton btnCancel;
        private UserControls.ucLineQuantity ucLineQuantity;
    }
}