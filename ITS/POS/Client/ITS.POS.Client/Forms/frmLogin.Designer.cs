namespace ITS.POS.Client.Forms
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tePassword = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.teUsername = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblStartingAmount = new DevExpress.XtraEditors.LabelControl();
            this.teStartingAmount = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teStartingAmount.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl1.Location = new System.Drawing.Point(112, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(145, 29);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "USERNAME:";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl2.Location = new System.Drawing.Point(109, 57);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(149, 29);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "PASSWORD:";
            // 
            // tePassword
            // 
            this.tePassword.AutoHideTouchPad = false;
            this.tePassword.EditValue = "";
            this.tePassword.Location = new System.Drawing.Point(262, 53);
            this.tePassword.Name = "tePassword";
            this.tePassword.PoleDisplayName = "";
            this.tePassword.Properties.AccessibleName = "";
            this.tePassword.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.tePassword.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.tePassword.Properties.Appearance.Options.UseBackColor = true;
            this.tePassword.Properties.Appearance.Options.UseFont = true;
            this.tePassword.Properties.Appearance.Options.UseTextOptions = true;
            this.tePassword.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.tePassword.Properties.PasswordChar = '%';
            this.tePassword.Properties.UseSystemPasswordChar = true;
            this.tePassword.Size = new System.Drawing.Size(213, 40);
            this.tePassword.TabIndex = 3;
            this.tePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tePassword_KeyDown);
            // 
            // teUsername
            // 
            this.teUsername.AutoHideTouchPad = false;
            this.teUsername.EditValue = "";
            this.teUsername.Location = new System.Drawing.Point(262, 13);
            this.teUsername.Name = "teUsername";
            this.teUsername.PoleDisplayName = "";
            this.teUsername.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.teUsername.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.teUsername.Properties.Appearance.Options.UseBackColor = true;
            this.teUsername.Properties.Appearance.Options.UseFont = true;
            this.teUsername.Properties.Appearance.Options.UseTextOptions = true;
            this.teUsername.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.teUsername.Size = new System.Drawing.Size(213, 40);
            this.teUsername.TabIndex = 2;
            this.teUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teUsername_KeyDown);
            // 
            // lblStartingAmount
            // 
            this.lblStartingAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblStartingAmount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblStartingAmount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStartingAmount.Location = new System.Drawing.Point(2, 97);
            this.lblStartingAmount.Name = "lblStartingAmount";
            this.lblStartingAmount.Size = new System.Drawing.Size(254, 29);
            this.lblStartingAmount.TabIndex = 4;
            this.lblStartingAmount.Text = "STARTING AMOUNT:";
            // 
            // teStartingAmount
            // 
            this.teStartingAmount.AutoHideTouchPad = true;
            this.teStartingAmount.EditValue = "";
            this.teStartingAmount.Location = new System.Drawing.Point(262, 93);
            this.teStartingAmount.Name = "teStartingAmount";
            this.teStartingAmount.PoleDisplayName = "";
            this.teStartingAmount.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.teStartingAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.teStartingAmount.Properties.Appearance.Options.UseBackColor = true;
            this.teStartingAmount.Properties.Appearance.Options.UseFont = true;
            this.teStartingAmount.Properties.Appearance.Options.UseTextOptions = true;
            this.teStartingAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.teStartingAmount.Properties.Mask.EditMask = "c";
            this.teStartingAmount.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teStartingAmount.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teStartingAmount.Size = new System.Drawing.Size(213, 42);
            this.teStartingAmount.TabIndex = 5;
            this.teStartingAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teStartingAmount_KeyDown);
            // 
            // frmLogin
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(487, 151);
            this.Controls.Add(this.teStartingAmount);
            this.Controls.Add(this.lblStartingAmount);
            this.Controls.Add(this.tePassword);
            this.Controls.Add(this.teUsername);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmLogin";
            this.Text = "Έναρξη Βάρδιας";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLogin_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmLogin_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teStartingAmount.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private UserControls.ucTouchFriendlyInput teUsername;
        private UserControls.ucTouchFriendlyInput tePassword;
        private DevExpress.XtraEditors.LabelControl lblStartingAmount;
        private UserControls.ucTouchFriendlyInput teStartingAmount;

    }
}