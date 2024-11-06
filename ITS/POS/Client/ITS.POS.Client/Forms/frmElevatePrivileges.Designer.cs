using ITS.POS.Client.UserControls;
namespace ITS.POS.Client.Forms
{
    partial class frmElevatePrivileges
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmElevatePrivileges));
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.teUserName = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.tePassword = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.teUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(30, 10, 30, 0);
            this.lblMessage.Size = new System.Drawing.Size(501, 155);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "MESSAGE";
            // 
            // teUserName
            // 
            this.teUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.teUserName.AutoHideTouchPad = true;
            this.teUserName.Location = new System.Drawing.Point(164, 78);
            this.teUserName.Name = "teUserName";
            this.teUserName.PoleDisplayName = "";
            this.teUserName.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.teUserName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.teUserName.Properties.Appearance.Options.UseBackColor = true;
            this.teUserName.Properties.Appearance.Options.UseFont = true;
            this.teUserName.Size = new System.Drawing.Size(328, 36);
            this.teUserName.TabIndex = 1;
            this.teUserName.Visible = false;
            this.teUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teUserName_KeyDown);
            // 
            // tePassword
            // 
            this.tePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tePassword.AutoHideTouchPad = true;
            this.tePassword.Location = new System.Drawing.Point(164, 114);
            this.tePassword.Name = "tePassword";
            this.tePassword.PoleDisplayName = "";
            this.tePassword.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.tePassword.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.tePassword.Properties.Appearance.Options.UseBackColor = true;
            this.tePassword.Properties.Appearance.Options.UseFont = true;
            this.tePassword.Properties.UseSystemPasswordChar = true;
            this.tePassword.Size = new System.Drawing.Size(328, 36);
            this.tePassword.TabIndex = 2;
            this.tePassword.Visible = false;
            this.tePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tePassword_KeyDown);
            // 
            // lblUserName
            // 
            this.lblUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserName.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblUserName.Location = new System.Drawing.Point(12, 79);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(145, 29);
            this.lblUserName.TabIndex = 3;
            this.lblUserName.Text = "USERNAME:";
            this.lblUserName.Visible = false;
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblPassword.Location = new System.Drawing.Point(12, 115);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(149, 29);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "PASSWORD:";
            this.lblPassword.Visible = false;
            // 
            // frmElevatePrivileges
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(501, 155);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.tePassword);
            this.Controls.Add(this.teUserName);
            this.Controls.Add(this.lblMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmElevatePrivileges";
            this.Text = "frmElevatePrivileges";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmElevatePrivileges_FormClosed);
            this.Load += new System.EventHandler(this.frmElevatePrivileges_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmElevatePrivileges_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.teUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblMessage;
        private ucTouchFriendlyInput teUserName;
        private ucTouchFriendlyInput tePassword;
        private DevExpress.XtraEditors.LabelControl lblUserName;
        private DevExpress.XtraEditors.LabelControl lblPassword;
    }
}