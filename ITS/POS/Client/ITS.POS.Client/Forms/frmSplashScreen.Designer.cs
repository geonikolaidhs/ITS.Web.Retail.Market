using ITS.Retail.Platform.Enumerations;
namespace ITS.POS.Client.Forms
{
    partial class frmSplashScreen
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
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.MessageDisplay = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucCloseDay = new ITS.POS.Client.UserControls.ucButton();
            this.ucStartShift = new ITS.POS.Client.UserControls.ucButton();
            this.ucStartDay = new ITS.POS.Client.UserControls.ucButton();
            this.ucCommunicationStatus1 = new ITS.POS.Client.UserControls.ucCommunicationStatus();
            this.ucLogo1 = new ITS.POS.Client.UserControls.ucLogo();
            this.cmdTextBox = new DevExpress.XtraEditors.TextEdit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucLogo1.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdTextBox.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(35, 313);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "@@PLEASE_SELECT_AN_ACTION";
            // 
            // MessageDisplay
            // 
            this.MessageDisplay.AutoSize = true;
            this.MessageDisplay.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MessageDisplay.Location = new System.Drawing.Point(34, 279);
            this.MessageDisplay.Name = "MessageDisplay";
            this.MessageDisplay.Size = new System.Drawing.Size(119, 19);
            this.MessageDisplay.TabIndex = 6;
            this.MessageDisplay.Text = "MessageDisplay";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ucCloseDay);
            this.panel1.Controls.Add(this.ucStartShift);
            this.panel1.Controls.Add(this.ucStartDay);
            this.panel1.Location = new System.Drawing.Point(38, 339);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(501, 45);
            this.panel1.TabIndex = 10;
            // 
            // ucCloseDay
            // 
            this.ucCloseDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ucCloseDay.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ucCloseDay.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucCloseDay.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(86)))), ((int)(((byte)(156)))));
            this.ucCloseDay.Appearance.Options.UseBackColor = true;
            this.ucCloseDay.Appearance.Options.UseFont = true;
            this.ucCloseDay.Appearance.Options.UseForeColor = true;
            // 
            // 
            // 
            this.ucCloseDay.Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ucCloseDay.Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ucCloseDay.Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCloseDay.Button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.ucCloseDay.Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ucCloseDay.Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.ucCloseDay.Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ucCloseDay.Button.Location = new System.Drawing.Point(0, 0);
            this.ucCloseDay.Button.Name = "Button";
            this.ucCloseDay.Button.Size = new System.Drawing.Size(138, 37);
            this.ucCloseDay.Button.TabIndex = 0;
            this.ucCloseDay.Button.Text = "@@ISSUE_Z";
            this.ucCloseDay.Button.UseVisualStyleBackColor = false;
            this.ucCloseDay.Button.Click += new System.EventHandler(this.ucCloseDay_Click);
            this.ucCloseDay.Location = new System.Drawing.Point(356, 3);
            this.ucCloseDay.LookAndFeel.SkinName = "Metropolis";
            this.ucCloseDay.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucCloseDay.Name = "ucCloseDay";
            this.ucCloseDay.Size = new System.Drawing.Size(138, 37);
            this.ucCloseDay.TabIndex = 65;
            // 
            // ucStartShift
            // 
            this.ucStartShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ucStartShift.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ucStartShift.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucStartShift.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(86)))), ((int)(((byte)(156)))));
            this.ucStartShift.Appearance.Options.UseBackColor = true;
            this.ucStartShift.Appearance.Options.UseFont = true;
            this.ucStartShift.Appearance.Options.UseForeColor = true;
            // 
            // 
            // 
            this.ucStartShift.Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ucStartShift.Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ucStartShift.Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucStartShift.Button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.ucStartShift.Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ucStartShift.Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.ucStartShift.Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ucStartShift.Button.Location = new System.Drawing.Point(0, 0);
            this.ucStartShift.Button.Name = "Button";
            this.ucStartShift.Button.Size = new System.Drawing.Size(138, 37);
            this.ucStartShift.Button.TabIndex = 0;
            this.ucStartShift.Button.Text = "@@START_SHIFT";
            this.ucStartShift.Button.UseVisualStyleBackColor = false;
            this.ucStartShift.Button.Click += new System.EventHandler(this.ucStartShift_Click);
            this.ucStartShift.Location = new System.Drawing.Point(184, 3);
            this.ucStartShift.LookAndFeel.SkinName = "Metropolis";
            this.ucStartShift.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucStartShift.Name = "ucStartShift";
            this.ucStartShift.Size = new System.Drawing.Size(138, 37);
            this.ucStartShift.TabIndex = 64;
            // 
            // ucStartDay
            // 
            this.ucStartDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ucStartDay.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ucStartDay.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucStartDay.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(86)))), ((int)(((byte)(156)))));
            this.ucStartDay.Appearance.Options.UseBackColor = true;
            this.ucStartDay.Appearance.Options.UseFont = true;
            this.ucStartDay.Appearance.Options.UseForeColor = true;
            // 
            // 
            // 
            this.ucStartDay.Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ucStartDay.Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ucStartDay.Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucStartDay.Button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.ucStartDay.Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ucStartDay.Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.ucStartDay.Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ucStartDay.Button.Location = new System.Drawing.Point(0, 0);
            this.ucStartDay.Button.Name = "Button";
            this.ucStartDay.Button.Size = new System.Drawing.Size(138, 37);
            this.ucStartDay.Button.TabIndex = 0;
            this.ucStartDay.Button.Text = "@@START_DAY";
            this.ucStartDay.Button.UseVisualStyleBackColor = false;
            this.ucStartDay.Button.Click += new System.EventHandler(this.ucStartDay_Click);
            this.ucStartDay.Location = new System.Drawing.Point(5, 3);
            this.ucStartDay.LookAndFeel.SkinName = "Metropolis";
            this.ucStartDay.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucStartDay.Name = "ucStartDay";
            this.ucStartDay.Size = new System.Drawing.Size(138, 37);
            this.ucStartDay.TabIndex = 63;
            // 
            // ucCommunicationStatus1
            // 
            // 
            // 
            // 
            this.ucCommunicationStatus1.DownloadingLabel.Name = "lblDownloading";
            this.ucCommunicationStatus1.DownloadingLabel.Size = new System.Drawing.Size(0, 18);
            // 
            // 
            // 
            this.ucCommunicationStatus1.DownloadingMessageLabel.Name = "lblDownloadingMessage";
            this.ucCommunicationStatus1.DownloadingMessageLabel.Size = new System.Drawing.Size(0, 18);
            // 
            // 
            // 
            this.ucCommunicationStatus1.DownloadingProgressBar.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.ucCommunicationStatus1.DownloadingProgressBar.Name = "pbDownloading";
            this.ucCommunicationStatus1.DownloadingProgressBar.Size = new System.Drawing.Size(50, 17);
            this.ucCommunicationStatus1.Location = new System.Drawing.Point(4, 404);
            this.ucCommunicationStatus1.Name = "ucCommunicationStatus1";
            this.ucCommunicationStatus1.Size = new System.Drawing.Size(585, 23);
            // 
            // 
            // 
            this.ucCommunicationStatus1.StatusIconLabel.ActiveLinkColor = System.Drawing.Color.Red;
            this.ucCommunicationStatus1.StatusIconLabel.Name = "lblStatusColor";
            this.ucCommunicationStatus1.StatusIconLabel.Size = new System.Drawing.Size(0, 18);
            this.ucCommunicationStatus1.TabIndex = 9;
            // 
            // 
            // 
            this.ucCommunicationStatus1.UploadingLabel.Name = "lblUploading";
            this.ucCommunicationStatus1.UploadingLabel.Size = new System.Drawing.Size(0, 18);
            // 
            // 
            // 
            this.ucCommunicationStatus1.UploadingMessageLabel.Name = "lblUploadingMessage";
            this.ucCommunicationStatus1.UploadingMessageLabel.Size = new System.Drawing.Size(0, 18);
            // 
            // 
            // 
            this.ucCommunicationStatus1.UploadingProgressBar.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.ucCommunicationStatus1.UploadingProgressBar.Name = "pbUploading";
            this.ucCommunicationStatus1.UploadingProgressBar.Size = new System.Drawing.Size(50, 17);
            // 
            // ucLogo1
            // 
            // 
            // 
            // 
            this.ucLogo1.ImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucLogo1.ImageBox.Image = global::ITS.POS.Client.Properties.Resources.itslogo;
            this.ucLogo1.ImageBox.Location = new System.Drawing.Point(0, 0);
            this.ucLogo1.ImageBox.Name = "Image";
            this.ucLogo1.ImageBox.Size = new System.Drawing.Size(527, 130);
            this.ucLogo1.ImageBox.TabIndex = 0;
            this.ucLogo1.ImageBox.TabStop = false;
            this.ucLogo1.Location = new System.Drawing.Point(29, 12);
            this.ucLogo1.Name = "ucLogo1";
            this.ucLogo1.Size = new System.Drawing.Size(527, 130);
            this.ucLogo1.TabIndex = 8;
            // 
            // cmdTextBox
            // 
            this.cmdTextBox.Location = new System.Drawing.Point(38, 255);
            this.cmdTextBox.Name = "cmdTextBox";
            this.cmdTextBox.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.cmdTextBox.Properties.Appearance.Options.UseBackColor = true;
            this.cmdTextBox.Size = new System.Drawing.Size(501, 20);
            this.cmdTextBox.TabIndex = 11;
            this.cmdTextBox.Visible = false;
            // 
            // frmSplashScreen
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Animation;
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(591, 429);
            this.ControlBox = false;
            this.Controls.Add(this.cmdTextBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ucCommunicationStatus1);
            this.Controls.Add(this.ucLogo1);
            this.Controls.Add(this.MessageDisplay);
            this.Controls.Add(this.label1);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSplashScreen";
            this.Text = "ITS.POS.Client";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucLogo1.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdTextBox.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label MessageDisplay;
        private UserControls.ucLogo ucLogo1;
        private UserControls.ucCommunicationStatus ucCommunicationStatus1;
        private System.Windows.Forms.Panel panel1;
        public DevExpress.XtraEditors.TextEdit cmdTextBox;
        private UserControls.ucButton ucStartDay;
        private UserControls.ucButton ucCloseDay;
        private UserControls.ucButton ucStartShift;
    }
}