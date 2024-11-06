namespace ITS.POS.Client.Forms
{
    partial class frmPauseSlider
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
            this.ImageSlider = new ITS.POS.Client.UserControls.ucImageSlider();
            this.tePassword = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblUser = new DevExpress.XtraEditors.LabelControl();
            this.ImageSlider.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageSlider
            // 
            this.ImageSlider.AnimationInterval = 4000;
            this.ImageSlider.Controls.Add(this.tePassword);
            this.ImageSlider.Controls.Add(this.lblUser);
            this.ImageSlider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImageSlider.Location = new System.Drawing.Point(0, 0);
            this.ImageSlider.MediaFolder = null;
            this.ImageSlider.Name = "ImageSlider";
            this.ImageSlider.Size = new System.Drawing.Size(594, 292);
            this.ImageSlider.SliderHeight = 0;
            this.ImageSlider.SliderWidth = 0;
            this.ImageSlider.TabIndex = 0;
            this.ImageSlider.Click += new System.EventHandler(this.ImageSlider_Click);
            // 
            // tePassword
            // 
            this.tePassword.AutoHideTouchPad = false;
            this.tePassword.Location = new System.Drawing.Point(85, 12);
            this.tePassword.Name = "tePassword";
            this.tePassword.PoleDisplayName = "";
            this.tePassword.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.tePassword.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.tePassword.Properties.Appearance.Options.UseBackColor = true;
            this.tePassword.Properties.Appearance.Options.UseFont = true;
            this.tePassword.Properties.UseSystemPasswordChar = true;
            this.tePassword.Size = new System.Drawing.Size(328, 36);
            this.tePassword.TabIndex = 13;
            this.tePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tePassword_KeyDown);
            // 
            // lblUser
            // 
            this.lblUser.Appearance.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Bold);
            this.lblUser.Appearance.ForeColor = System.Drawing.Color.DimGray;
            this.lblUser.Location = new System.Drawing.Point(13, 12);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(64, 28);
            this.lblUser.TabIndex = 12;
            this.lblUser.Text = "USER";
            // 
            // frmPauseSlider
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 292);
            this.Controls.Add(this.ImageSlider);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmPauseSlider";
            this.Text = "frmPause";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPauseSlider_FormClosing);
            this.Click += new System.EventHandler(this.frmPauseSlider_Click);
            this.ImageSlider.ResumeLayout(false);
            this.ImageSlider.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.ucImageSlider ImageSlider;
        private DevExpress.XtraEditors.LabelControl lblUser;
        private UserControls.ucTouchFriendlyInput tePassword;
    }
}