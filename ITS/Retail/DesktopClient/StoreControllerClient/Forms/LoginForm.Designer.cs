namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.simpleButtonLogin = new DevExpress.XtraEditors.SimpleButton();
            this.textEditPassword = new DevExpress.XtraEditors.TextEdit();
            this.textEditUserName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.labelUserName = new DevExpress.XtraLayout.LayoutControlItem();
            this.labelPassword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcLogo = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.pictureBoxLogo);
            this.layoutControl1.Controls.Add(this.simpleButtonLogin);
            this.layoutControl1.Controls.Add(this.textEditPassword);
            this.layoutControl1.Controls.Add(this.textEditUserName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.LookAndFeel.SkinName = "Metropolis";
            this.layoutControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(775, 135, 497, 562);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(284, 198);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.wrm;
            this.pictureBoxLogo.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(260, 92);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 7;
            this.pictureBoxLogo.TabStop = false;
            // 
            // simpleButtonLogin
            // 
            this.simpleButtonLogin.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Key_24;
            this.simpleButtonLogin.Location = new System.Drawing.Point(12, 156);
            this.simpleButtonLogin.Name = "simpleButtonLogin";
            this.simpleButtonLogin.Size = new System.Drawing.Size(260, 30);
            this.simpleButtonLogin.StyleController = this.layoutControl1;
            this.simpleButtonLogin.TabIndex = 6;
            this.simpleButtonLogin.Text =  "@@Login";
            this.simpleButtonLogin.Click += new System.EventHandler(this.simpleButtonLogin_Click);
            // 
            // textEditPassword
            // 
            this.textEditPassword.Location = new System.Drawing.Point(84, 132);
            this.textEditPassword.Name = "textEditPassword";
            this.textEditPassword.Properties.UseSystemPasswordChar = true;
            this.textEditPassword.Size = new System.Drawing.Size(188, 20);
            this.textEditPassword.StyleController = this.layoutControl1;
            this.textEditPassword.TabIndex = 5;
            this.textEditPassword.Enter += new System.EventHandler(this.textEditPassword_Enter);
            this.textEditPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditPassword_KeyPress);
            // 
            // textEditUserName
            // 
            this.textEditUserName.Location = new System.Drawing.Point(84, 108);
            this.textEditUserName.Name = "textEditUserName";
            this.textEditUserName.Size = new System.Drawing.Size(188, 20);
            this.textEditUserName.StyleController = this.layoutControl1;
            this.textEditUserName.TabIndex = 4;
            this.textEditUserName.Enter += new System.EventHandler(this.textEditUserName_Enter);
            this.textEditUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditUserName_KeyPress);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.labelUserName,
            this.labelPassword,
            this.layoutControlItem3,
            this.lcLogo});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(284, 198);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // labelUserName
            // 
            this.labelUserName.Control = this.textEditUserName;
            this.SetIsRequired(this.labelUserName, false);
            this.labelUserName.Location = new System.Drawing.Point(0, 96);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(264, 24);
            this.labelUserName.Text = "@@UserName";
            this.labelUserName.TextSize = new System.Drawing.Size(69, 13);
            // 
            // labelPassword
            // 
            this.labelPassword.Control = this.textEditPassword;
            this.SetIsRequired(this.labelPassword, false);
            this.labelPassword.Location = new System.Drawing.Point(0, 120);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(264, 24);
            this.labelPassword.Text = "@@Password";
            this.labelPassword.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButtonLogin;
            this.SetIsRequired(this.layoutControlItem3, false);
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 144);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(264, 34);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lcLogo
            // 
            this.lcLogo.Control = this.pictureBoxLogo;
            this.SetIsRequired(this.lcLogo, false);
            this.lcLogo.Location = new System.Drawing.Point(0, 0);
            this.lcLogo.Name = "lcLogo";
            this.lcLogo.Size = new System.Drawing.Size(264, 96);
            this.lcLogo.TextSize = new System.Drawing.Size(0, 0);
            this.lcLogo.TextVisible = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 198);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(318, 262);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(318, 236);
            this.Name = "LoginForm";
            this.Text = "@@Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit textEditPassword;
        private DevExpress.XtraEditors.TextEdit textEditUserName;
        private DevExpress.XtraLayout.LayoutControlItem labelUserName;
        private DevExpress.XtraLayout.LayoutControlItem labelPassword;
        private DevExpress.XtraEditors.SimpleButton simpleButtonLogin;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private DevExpress.XtraLayout.LayoutControlItem lcLogo;

    }
}