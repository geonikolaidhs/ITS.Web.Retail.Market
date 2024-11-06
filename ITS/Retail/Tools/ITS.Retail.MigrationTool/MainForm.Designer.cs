namespace ITS.Retail.MigrationTool
{
    partial class MainForm
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
            this.txtEmailFrom = new System.Windows.Forms.TextBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.btnResetConfiguration = new System.Windows.Forms.Button();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.btnSaveConfiguration = new System.Windows.Forms.Button();
            this.btnUpgradeDB = new System.Windows.Forms.Button();
            this.chkEnableSSL = new System.Windows.Forms.CheckBox();
            this.btnInitialiseDB = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbDBType = new System.Windows.Forms.ComboBox();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDBUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grbSmtpSettings = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSMTPHostPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSMTPHostUsername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSMTPHost = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.richtxtBoxLog = new System.Windows.Forms.RichTextBox();
            this.btnCheckVersions = new System.Windows.Forms.Button();
            this.storeControllerRadioButton = new System.Windows.Forms.RadioButton();
            this.retailRadioButton = new System.Windows.Forms.RadioButton();
            this.grbStoreControllerSettings = new System.Windows.Forms.GroupBox();
            this.txtStoreControllerPassword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtStoreControllerUserName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtStoreControllerID = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtMasterURL = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dualModeRadioButton = new System.Windows.Forms.RadioButton();
            this.grbDualModeSettings = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtStoreCode = new System.Windows.Forms.TextBox();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.comboBoxLanguages = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDeployAnalysis = new System.Windows.Forms.Button();
            this.btnTestOlapConnection = new System.Windows.Forms.Button();
            this.txtOlapServer = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtOlapPassword = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtOlapUsername = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtLicenseServer = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtUpdaterBatchSize = new System.Windows.Forms.TextBox();
            this.txtIISCache = new System.Windows.Forms.TextBox();
            this.chkMiniProfiler = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.grbSmtpSettings.SuspendLayout();
            this.grbStoreControllerSettings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grbDualModeSettings.SuspendLayout();
            this.groupBoxLanguage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEmailFrom
            // 
            this.txtEmailFrom.Location = new System.Drawing.Point(100, 159);
            this.txtEmailFrom.Name = "txtEmailFrom";
            this.txtEmailFrom.Size = new System.Drawing.Size(100, 20);
            this.txtEmailFrom.TabIndex = 19;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnClearLog.Location = new System.Drawing.Point(614, 556);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(141, 23);
            this.btnClearLog.TabIndex = 34;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 159);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "From";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnExit.Location = new System.Drawing.Point(761, 556);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(141, 23);
            this.btnExit.TabIndex = 33;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 303);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 13);
            this.label11.TabIndex = 32;
            this.label11.Text = "Log";
            // 
            // btnResetConfiguration
            // 
            this.btnResetConfiguration.Location = new System.Drawing.Point(574, 277);
            this.btnResetConfiguration.Name = "btnResetConfiguration";
            this.btnResetConfiguration.Size = new System.Drawing.Size(117, 23);
            this.btnResetConfiguration.TabIndex = 31;
            this.btnResetConfiguration.Text = "Reset Configuration";
            this.btnResetConfiguration.UseVisualStyleBackColor = true;
            this.btnResetConfiguration.Click += new System.EventHandler(this.btnResetConfiguration_Click);
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(100, 133);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(100, 20);
            this.txtDomain.TabIndex = 17;
            // 
            // btnSaveConfiguration
            // 
            this.btnSaveConfiguration.Location = new System.Drawing.Point(455, 277);
            this.btnSaveConfiguration.Name = "btnSaveConfiguration";
            this.btnSaveConfiguration.Size = new System.Drawing.Size(113, 23);
            this.btnSaveConfiguration.TabIndex = 30;
            this.btnSaveConfiguration.Text = "Save Configuration";
            this.btnSaveConfiguration.UseVisualStyleBackColor = true;
            this.btnSaveConfiguration.Click += new System.EventHandler(this.btnSaveConfiguration_Click);
            // 
            // btnUpgradeDB
            // 
            this.btnUpgradeDB.Location = new System.Drawing.Point(128, 277);
            this.btnUpgradeDB.Name = "btnUpgradeDB";
            this.btnUpgradeDB.Size = new System.Drawing.Size(110, 23);
            this.btnUpgradeDB.TabIndex = 29;
            this.btnUpgradeDB.Text = "Upgrade Database";
            this.btnUpgradeDB.UseVisualStyleBackColor = true;
            this.btnUpgradeDB.Click += new System.EventHandler(this.btnUpgradeDB_Click);
            // 
            // chkEnableSSL
            // 
            this.chkEnableSSL.AutoSize = true;
            this.chkEnableSSL.Location = new System.Drawing.Point(105, 189);
            this.chkEnableSSL.Name = "chkEnableSSL";
            this.chkEnableSSL.Size = new System.Drawing.Size(103, 17);
            this.chkEnableSSL.TabIndex = 20;
            this.chkEnableSSL.Text = "SSL Connection";
            this.chkEnableSSL.UseVisualStyleBackColor = true;
            // 
            // btnInitialiseDB
            // 
            this.btnInitialiseDB.Location = new System.Drawing.Point(18, 277);
            this.btnInitialiseDB.Name = "btnInitialiseDB";
            this.btnInitialiseDB.Size = new System.Drawing.Size(105, 23);
            this.btnInitialiseDB.TabIndex = 28;
            this.btnInitialiseDB.Text = "Initialise Database";
            this.btnInitialiseDB.UseVisualStyleBackColor = true;
            this.btnInitialiseDB.Click += new System.EventHandler(this.btnInitialiseDB_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestConnection);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.cmbDBType);
            this.groupBox1.Controls.Add(this.txtDBPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtDBUsername);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtDatabase);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(18, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 221);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Settings";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(100, 176);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnTestConnection.TabIndex = 18;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 139);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "DB Type";
            // 
            // cmbDBType
            // 
            this.cmbDBType.FormattingEnabled = true;
            this.cmbDBType.Items.AddRange(new object[] {
            "SQLServer",
            "MySQL",
            "Oracle",
            "postgres",
            "sqlite"});
            this.cmbDBType.Location = new System.Drawing.Point(100, 131);
            this.cmbDBType.Name = "cmbDBType";
            this.cmbDBType.Size = new System.Drawing.Size(100, 21);
            this.cmbDBType.TabIndex = 16;
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Location = new System.Drawing.Point(100, 107);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.Size = new System.Drawing.Size(100, 20);
            this.txtDBPassword.TabIndex = 15;
            this.txtDBPassword.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Password";
            // 
            // txtDBUsername
            // 
            this.txtDBUsername.Location = new System.Drawing.Point(100, 81);
            this.txtDBUsername.Name = "txtDBUsername";
            this.txtDBUsername.Size = new System.Drawing.Size(100, 20);
            this.txtDBUsername.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Username";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(100, 55);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(100, 20);
            this.txtDatabase.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Database";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(100, 29);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(100, 20);
            this.txtServer.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Server";
            // 
            // grbSmtpSettings
            // 
            this.grbSmtpSettings.Controls.Add(this.chkEnableSSL);
            this.grbSmtpSettings.Controls.Add(this.txtEmailFrom);
            this.grbSmtpSettings.Controls.Add(this.label9);
            this.grbSmtpSettings.Controls.Add(this.txtDomain);
            this.grbSmtpSettings.Controls.Add(this.label10);
            this.grbSmtpSettings.Controls.Add(this.txtSMTPHostPassword);
            this.grbSmtpSettings.Controls.Add(this.label5);
            this.grbSmtpSettings.Controls.Add(this.txtSMTPHostUsername);
            this.grbSmtpSettings.Controls.Add(this.label6);
            this.grbSmtpSettings.Controls.Add(this.txtPort);
            this.grbSmtpSettings.Controls.Add(this.label7);
            this.grbSmtpSettings.Controls.Add(this.txtSMTPHost);
            this.grbSmtpSettings.Controls.Add(this.label8);
            this.grbSmtpSettings.Location = new System.Drawing.Point(244, 50);
            this.grbSmtpSettings.Name = "grbSmtpSettings";
            this.grbSmtpSettings.Size = new System.Drawing.Size(211, 221);
            this.grbSmtpSettings.TabIndex = 27;
            this.grbSmtpSettings.TabStop = false;
            this.grbSmtpSettings.Text = "SMTP Settings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Domain";
            // 
            // txtSMTPHostPassword
            // 
            this.txtSMTPHostPassword.Location = new System.Drawing.Point(100, 107);
            this.txtSMTPHostPassword.Name = "txtSMTPHostPassword";
            this.txtSMTPHostPassword.Size = new System.Drawing.Size(100, 20);
            this.txtSMTPHostPassword.TabIndex = 15;
            this.txtSMTPHostPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Password";
            // 
            // txtSMTPHostUsername
            // 
            this.txtSMTPHostUsername.Location = new System.Drawing.Point(100, 81);
            this.txtSMTPHostUsername.Name = "txtSMTPHostUsername";
            this.txtSMTPHostUsername.Size = new System.Drawing.Size(100, 20);
            this.txtSMTPHostUsername.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Username";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(100, 55);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Port";
            // 
            // txtSMTPHost
            // 
            this.txtSMTPHost.Location = new System.Drawing.Point(100, 29);
            this.txtSMTPHost.Name = "txtSMTPHost";
            this.txtSMTPHost.Size = new System.Drawing.Size(100, 20);
            this.txtSMTPHost.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "SMTP Host";
            // 
            // richtxtBoxLog
            // 
            this.richtxtBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richtxtBoxLog.Location = new System.Drawing.Point(18, 319);
            this.richtxtBoxLog.Name = "richtxtBoxLog";
            this.richtxtBoxLog.Size = new System.Drawing.Size(877, 231);
            this.richtxtBoxLog.TabIndex = 35;
            this.richtxtBoxLog.Text = "";
            // 
            // btnCheckVersions
            // 
            this.btnCheckVersions.Location = new System.Drawing.Point(241, 277);
            this.btnCheckVersions.Name = "btnCheckVersions";
            this.btnCheckVersions.Size = new System.Drawing.Size(110, 23);
            this.btnCheckVersions.TabIndex = 36;
            this.btnCheckVersions.Text = "Check Versions";
            this.btnCheckVersions.UseVisualStyleBackColor = true;
            this.btnCheckVersions.Click += new System.EventHandler(this.btnCheckVersions_Click);
            // 
            // storeControllerRadioButton
            // 
            this.storeControllerRadioButton.AutoSize = true;
            this.storeControllerRadioButton.Location = new System.Drawing.Point(109, 18);
            this.storeControllerRadioButton.Name = "storeControllerRadioButton";
            this.storeControllerRadioButton.Size = new System.Drawing.Size(97, 17);
            this.storeControllerRadioButton.TabIndex = 37;
            this.storeControllerRadioButton.Text = "Store Controller";
            this.storeControllerRadioButton.UseVisualStyleBackColor = true;
            this.storeControllerRadioButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // retailRadioButton
            // 
            this.retailRadioButton.AutoSize = true;
            this.retailRadioButton.Checked = true;
            this.retailRadioButton.Location = new System.Drawing.Point(6, 18);
            this.retailRadioButton.Name = "retailRadioButton";
            this.retailRadioButton.Size = new System.Drawing.Size(87, 17);
            this.retailRadioButton.TabIndex = 38;
            this.retailRadioButton.TabStop = true;
            this.retailRadioButton.Text = "Retail Master";
            this.retailRadioButton.UseVisualStyleBackColor = true;
            this.retailRadioButton.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // grbStoreControllerSettings
            // 
            this.grbStoreControllerSettings.Controls.Add(this.txtStoreControllerPassword);
            this.grbStoreControllerSettings.Controls.Add(this.label13);
            this.grbStoreControllerSettings.Controls.Add(this.txtStoreControllerUserName);
            this.grbStoreControllerSettings.Controls.Add(this.label14);
            this.grbStoreControllerSettings.Controls.Add(this.txtStoreControllerID);
            this.grbStoreControllerSettings.Controls.Add(this.label16);
            this.grbStoreControllerSettings.Controls.Add(this.txtMasterURL);
            this.grbStoreControllerSettings.Controls.Add(this.label18);
            this.grbStoreControllerSettings.Enabled = false;
            this.grbStoreControllerSettings.Location = new System.Drawing.Point(461, 50);
            this.grbStoreControllerSettings.Name = "grbStoreControllerSettings";
            this.grbStoreControllerSettings.Size = new System.Drawing.Size(232, 146);
            this.grbStoreControllerSettings.TabIndex = 27;
            this.grbStoreControllerSettings.TabStop = false;
            this.grbStoreControllerSettings.Text = "Store Controller Settings";
            // 
            // txtStoreControllerPassword
            // 
            this.txtStoreControllerPassword.Location = new System.Drawing.Point(117, 109);
            this.txtStoreControllerPassword.Name = "txtStoreControllerPassword";
            this.txtStoreControllerPassword.Size = new System.Drawing.Size(100, 20);
            this.txtStoreControllerPassword.TabIndex = 18;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 112);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = "Password";
            // 
            // txtStoreControllerUserName
            // 
            this.txtStoreControllerUserName.Location = new System.Drawing.Point(117, 82);
            this.txtStoreControllerUserName.Name = "txtStoreControllerUserName";
            this.txtStoreControllerUserName.Size = new System.Drawing.Size(100, 20);
            this.txtStoreControllerUserName.TabIndex = 16;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(20, 84);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Username";
            // 
            // txtStoreControllerID
            // 
            this.txtStoreControllerID.Location = new System.Drawing.Point(117, 56);
            this.txtStoreControllerID.Name = "txtStoreControllerID";
            this.txtStoreControllerID.Size = new System.Drawing.Size(100, 20);
            this.txtStoreControllerID.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 55);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(93, 13);
            this.label16.TabIndex = 12;
            this.label16.Text = "Store Controller ID";
            // 
            // txtMasterURL
            // 
            this.txtMasterURL.Location = new System.Drawing.Point(117, 29);
            this.txtMasterURL.Name = "txtMasterURL";
            this.txtMasterURL.Size = new System.Drawing.Size(100, 20);
            this.txtMasterURL.TabIndex = 9;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(18, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(64, 13);
            this.label18.TabIndex = 8;
            this.label18.Text = "Master URL";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dualModeRadioButton);
            this.groupBox2.Controls.Add(this.storeControllerRadioButton);
            this.groupBox2.Controls.Add(this.retailRadioButton);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(19, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 41);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Installation Type";
            // 
            // dualModeRadioButton
            // 
            this.dualModeRadioButton.AutoSize = true;
            this.dualModeRadioButton.Location = new System.Drawing.Point(216, 18);
            this.dualModeRadioButton.Name = "dualModeRadioButton";
            this.dualModeRadioButton.Size = new System.Drawing.Size(77, 17);
            this.dualModeRadioButton.TabIndex = 37;
            this.dualModeRadioButton.Text = "Dual Mode";
            this.dualModeRadioButton.UseVisualStyleBackColor = true;
            this.dualModeRadioButton.CheckedChanged += new System.EventHandler(this.dualModeRadioButton_CheckedChanged);
            // 
            // grbDualModeSettings
            // 
            this.grbDualModeSettings.Controls.Add(this.label15);
            this.grbDualModeSettings.Controls.Add(this.txtStoreCode);
            this.grbDualModeSettings.Enabled = false;
            this.grbDualModeSettings.Location = new System.Drawing.Point(461, 203);
            this.grbDualModeSettings.Name = "grbDualModeSettings";
            this.grbDualModeSettings.Size = new System.Drawing.Size(232, 68);
            this.grbDualModeSettings.TabIndex = 40;
            this.grbDualModeSettings.TabStop = false;
            this.grbDualModeSettings.Text = "Dual Mode Settings";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(17, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Store Code";
            // 
            // txtStoreCode
            // 
            this.txtStoreCode.Location = new System.Drawing.Point(116, 23);
            this.txtStoreCode.Name = "txtStoreCode";
            this.txtStoreCode.Size = new System.Drawing.Size(100, 20);
            this.txtStoreCode.TabIndex = 10;
            // 
            // groupBoxLanguage
            // 
            this.groupBoxLanguage.Controls.Add(this.comboBoxLanguages);
            this.groupBoxLanguage.Location = new System.Drawing.Point(461, 3);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.Size = new System.Drawing.Size(441, 41);
            this.groupBoxLanguage.TabIndex = 42;
            this.groupBoxLanguage.TabStop = false;
            this.groupBoxLanguage.Text = "Initialization Language";
            // 
            // comboBoxLanguages
            // 
            this.comboBoxLanguages.FormattingEnabled = true;
            this.comboBoxLanguages.Location = new System.Drawing.Point(6, 14);
            this.comboBoxLanguages.Name = "comboBoxLanguages";
            this.comboBoxLanguages.Size = new System.Drawing.Size(424, 21);
            this.comboBoxLanguages.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDeployAnalysis);
            this.groupBox3.Controls.Add(this.btnTestOlapConnection);
            this.groupBox3.Controls.Add(this.txtOlapServer);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.txtOlapPassword);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Controls.Add(this.txtOlapUsername);
            this.groupBox3.Location = new System.Drawing.Point(699, 50);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 146);
            this.groupBox3.TabIndex = 43;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "OLAP Configuration";
            // 
            // btnDeployAnalysis
            // 
            this.btnDeployAnalysis.Location = new System.Drawing.Point(112, 97);
            this.btnDeployAnalysis.Name = "btnDeployAnalysis";
            this.btnDeployAnalysis.Size = new System.Drawing.Size(80, 36);
            this.btnDeployAnalysis.TabIndex = 18;
            this.btnDeployAnalysis.Text = "Deploy Analysis";
            this.btnDeployAnalysis.UseVisualStyleBackColor = true;
            this.btnDeployAnalysis.Click += new System.EventHandler(this.btnDeployAnalysis_Click);
            // 
            // btnTestOlapConnection
            // 
            this.btnTestOlapConnection.Location = new System.Drawing.Point(6, 97);
            this.btnTestOlapConnection.Name = "btnTestOlapConnection";
            this.btnTestOlapConnection.Size = new System.Drawing.Size(80, 36);
            this.btnTestOlapConnection.TabIndex = 18;
            this.btnTestOlapConnection.Text = "Test Connection";
            this.btnTestOlapConnection.UseVisualStyleBackColor = true;
            this.btnTestOlapConnection.Click += new System.EventHandler(this.btnTestOlapConnection_Click);
            // 
            // txtOlapServer
            // 
            this.txtOlapServer.Location = new System.Drawing.Point(92, 22);
            this.txtOlapServer.Name = "txtOlapServer";
            this.txtOlapServer.Size = new System.Drawing.Size(100, 20);
            this.txtOlapServer.TabIndex = 9;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(38, 13);
            this.label22.TabIndex = 8;
            this.label22.Text = "Server";
            // 
            // txtOlapPassword
            // 
            this.txtOlapPassword.Location = new System.Drawing.Point(92, 71);
            this.txtOlapPassword.Name = "txtOlapPassword";
            this.txtOlapPassword.Size = new System.Drawing.Size(100, 20);
            this.txtOlapPassword.TabIndex = 15;
            this.txtOlapPassword.UseSystemPasswordChar = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 71);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(53, 13);
            this.label24.TabIndex = 14;
            this.label24.Text = "Password";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(10, 45);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(55, 13);
            this.label25.TabIndex = 12;
            this.label25.Text = "Username";
            // 
            // txtOlapUsername
            // 
            this.txtOlapUsername.Location = new System.Drawing.Point(92, 45);
            this.txtOlapUsername.Name = "txtOlapUsername";
            this.txtOlapUsername.Size = new System.Drawing.Size(100, 20);
            this.txtOlapUsername.TabIndex = 13;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.txtLicenseServer);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.txtUpdaterBatchSize);
            this.groupBox4.Controls.Add(this.txtIISCache);
            this.groupBox4.Controls.Add(this.chkMiniProfiler);
            this.groupBox4.Location = new System.Drawing.Point(699, 202);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 111);
            this.groupBox4.TabIndex = 44;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other Settings";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(4, 89);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(78, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "License Server";
            // 
            // txtLicenseServer
            // 
            this.txtLicenseServer.Location = new System.Drawing.Point(88, 86);
            this.txtLicenseServer.Name = "txtLicenseServer";
            this.txtLicenseServer.Size = new System.Drawing.Size(102, 20);
            this.txtLicenseServer.TabIndex = 3;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(4, 61);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(99, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Updater Batch Size";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 35);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(86, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "Cache Size (MB)";
            // 
            // txtUpdaterBatchSize
            // 
            this.txtUpdaterBatchSize.Location = new System.Drawing.Point(109, 61);
            this.txtUpdaterBatchSize.Name = "txtUpdaterBatchSize";
            this.txtUpdaterBatchSize.Size = new System.Drawing.Size(81, 20);
            this.txtUpdaterBatchSize.TabIndex = 1;
            // 
            // txtIISCache
            // 
            this.txtIISCache.Location = new System.Drawing.Point(109, 35);
            this.txtIISCache.Name = "txtIISCache";
            this.txtIISCache.Size = new System.Drawing.Size(81, 20);
            this.txtIISCache.TabIndex = 1;
            // 
            // chkMiniProfiler
            // 
            this.chkMiniProfiler.AutoSize = true;
            this.chkMiniProfiler.Location = new System.Drawing.Point(109, 12);
            this.chkMiniProfiler.Name = "chkMiniProfiler";
            this.chkMiniProfiler.Size = new System.Drawing.Size(77, 17);
            this.chkMiniProfiler.TabIndex = 0;
            this.chkMiniProfiler.Text = "MiniProfiler";
            this.chkMiniProfiler.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 583);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxLanguage);
            this.Controls.Add(this.grbDualModeSettings);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCheckVersions);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnResetConfiguration);
            this.Controls.Add(this.btnSaveConfiguration);
            this.Controls.Add(this.btnUpgradeDB);
            this.Controls.Add(this.btnInitialiseDB);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbStoreControllerSettings);
            this.Controls.Add(this.grbSmtpSettings);
            this.Controls.Add(this.richtxtBoxLog);
            this.Name = "MainForm";
            this.Text = "Retail Migration Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbSmtpSettings.ResumeLayout(false);
            this.grbSmtpSettings.PerformLayout();
            this.grbStoreControllerSettings.ResumeLayout(false);
            this.grbStoreControllerSettings.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grbDualModeSettings.ResumeLayout(false);
            this.grbDualModeSettings.PerformLayout();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEmailFrom;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnResetConfiguration;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Button btnSaveConfiguration;
        private System.Windows.Forms.Button btnUpgradeDB;
        private System.Windows.Forms.CheckBox chkEnableSSL;
        private System.Windows.Forms.Button btnInitialiseDB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbDBType;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDBUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grbSmtpSettings;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSMTPHostPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSMTPHostUsername;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSMTPHost;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox richtxtBoxLog;
        private System.Windows.Forms.Button btnCheckVersions;
		private System.Windows.Forms.RadioButton storeControllerRadioButton;
		private System.Windows.Forms.RadioButton retailRadioButton;
		private System.Windows.Forms.GroupBox grbStoreControllerSettings;
		private System.Windows.Forms.TextBox txtStoreControllerID;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtMasterURL;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox txtStoreControllerPassword;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtStoreControllerUserName;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton dualModeRadioButton;
        private System.Windows.Forms.GroupBox grbDualModeSettings;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtStoreCode;
        private System.Windows.Forms.GroupBox groupBoxLanguage;
        private System.Windows.Forms.ComboBox comboBoxLanguages;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDeployAnalysis;
        private System.Windows.Forms.Button btnTestOlapConnection;
        private System.Windows.Forms.TextBox txtOlapServer;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtOlapPassword;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtOlapUsername;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtIISCache;
        private System.Windows.Forms.CheckBox chkMiniProfiler;
        private System.Windows.Forms.TextBox txtUpdaterBatchSize;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtLicenseServer;
    }
}

