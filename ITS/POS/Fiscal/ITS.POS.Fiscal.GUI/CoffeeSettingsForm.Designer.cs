using DevExpress.XtraEditors;

namespace ITS.POS.Fiscal.GUI
{
    partial class CoffeeSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoffeeSettingsForm));
            this.txtListeningPort = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnConnect = new DevExpress.XtraEditors.SimpleButton();
            this.btnGetVersion = new DevExpress.XtraEditors.SimpleButton();
            this.btnGetIDSN = new DevExpress.XtraEditors.SimpleButton();
            this.btnIssueZ = new DevExpress.XtraEditors.SimpleButton();
            this.btnStartService = new DevExpress.XtraEditors.SimpleButton();
            this.picServiceStatus = new System.Windows.Forms.PictureBox();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.tabSettings = new DevExpress.XtraTab.XtraTabPage();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtRbsRegistrationNumber = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchFolder = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtAbcFolderPath = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtIPAddress = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtComPort = new DevExpress.XtraEditors.TextEdit();
            this.tabGGPSSettings = new DevExpress.XtraTab.XtraTabPage();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.txtSmtpPass = new DevExpress.XtraEditors.TextEdit();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.txtSmtpUser = new DevExpress.XtraEditors.TextEdit();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.txtSmtpServer = new DevExpress.XtraEditors.TextEdit();
            this.txtMailList = new DevExpress.XtraEditors.MemoEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.checkMailFail = new System.Windows.Forms.CheckBox();
            this.checkSendSuccessMail = new System.Windows.Forms.CheckBox();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtGGPSUrl = new DevExpress.XtraEditors.TextEdit();
            this.labelSeriaNumber = new DevExpress.XtraEditors.LabelControl();
            this.txtEafdssSN = new DevExpress.XtraEditors.TextEdit();
            this.txtAesKey = new DevExpress.XtraEditors.MemoEdit();
            this.labelAesKey = new DevExpress.XtraEditors.LabelControl();
            this.radCOM = new System.Windows.Forms.RadioButton();
            this.radEthernet = new System.Windows.Forms.RadioButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnInstallService = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveSettings = new DevExpress.XtraEditors.SimpleButton();
            this.picAlgoboxStatus = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblServiceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblServiceConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFiscalStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAlgoSN = new ITS.POS.Fiscal.GUI.BindableToolStripStatusLabel();
            this.taskBarIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.issueZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.chkBoxInError = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSendOnlineCheck = new DevExpress.XtraEditors.SimpleButton();
            this.grbDebug = new System.Windows.Forms.GroupBox();
            this.simpleButtonViewLog = new DevExpress.XtraEditors.SimpleButton();
            this.checkBoxKeepLog = new System.Windows.Forms.CheckBox();
            this.checkBoχSendFiles = new System.Windows.Forms.CheckBox();
            this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
            this.txtSmtpPort = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtListeningPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picServiceStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRbsRegistrationNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbcFolderPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIPAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComPort.Properties)).BeginInit();
            this.tabGGPSSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpPass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMailList.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGGPSUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEafdssSN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAesKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAlgoboxStatus)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mnuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grbDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpPort.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtListeningPort
            // 
            this.txtListeningPort.EditValue = "100";
            this.txtListeningPort.Location = new System.Drawing.Point(130, 28);
            this.txtListeningPort.Name = "txtListeningPort";
            this.txtListeningPort.Size = new System.Drawing.Size(106, 20);
            this.txtListeningPort.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(11, 31);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(107, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Service Listening Port:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 72);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(150, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect to Service";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnGetVersion
            // 
            this.btnGetVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetVersion.Location = new System.Drawing.Point(6, 45);
            this.btnGetVersion.Name = "btnGetVersion";
            this.btnGetVersion.Size = new System.Drawing.Size(191, 23);
            this.btnGetVersion.TabIndex = 3;
            this.btnGetVersion.Text = "Get Version";
            this.btnGetVersion.Click += new System.EventHandler(this.btnGetVersion_Click);
            // 
            // btnGetIDSN
            // 
            this.btnGetIDSN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetIDSN.Location = new System.Drawing.Point(6, 17);
            this.btnGetIDSN.Name = "btnGetIDSN";
            this.btnGetIDSN.Size = new System.Drawing.Size(191, 23);
            this.btnGetIDSN.TabIndex = 3;
            this.btnGetIDSN.Text = "Get ID/Serial Number";
            this.btnGetIDSN.Click += new System.EventHandler(this.btnGetIDSN_Click);
            // 
            // btnIssueZ
            // 
            this.btnIssueZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIssueZ.Location = new System.Drawing.Point(6, 73);
            this.btnIssueZ.Name = "btnIssueZ";
            this.btnIssueZ.Size = new System.Drawing.Size(191, 23);
            this.btnIssueZ.TabIndex = 3;
            this.btnIssueZ.Text = "Issue Z";
            this.btnIssueZ.Click += new System.EventHandler(this.btnIssueZ_Click);
            // 
            // btnStartService
            // 
            this.btnStartService.Location = new System.Drawing.Point(6, 46);
            this.btnStartService.Name = "btnStartService";
            this.btnStartService.Size = new System.Drawing.Size(150, 23);
            this.btnStartService.TabIndex = 3;
            this.btnStartService.Text = "Start Service";
            this.btnStartService.Click += new System.EventHandler(this.btnStartService_Click);
            // 
            // picServiceStatus
            // 
            this.picServiceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picServiceStatus.Image = global::ITS.POS.Fiscal.GUI.Properties.Resources.offline_dot;
            this.picServiceStatus.Location = new System.Drawing.Point(3, 17);
            this.picServiceStatus.Name = "picServiceStatus";
            this.picServiceStatus.Size = new System.Drawing.Size(55, 36);
            this.picServiceStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picServiceStatus.TabIndex = 4;
            this.picServiceStatus.TabStop = false;
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.xtraTabControl1.Appearance.Options.UseBackColor = true;
            this.xtraTabControl1.Location = new System.Drawing.Point(19, 92);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.tabSettings;
            this.xtraTabControl1.Size = new System.Drawing.Size(480, 342);
            this.xtraTabControl1.TabIndex = 5;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabSettings,
            this.tabGGPSSettings});
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.labelControl7);
            this.tabSettings.Controls.Add(this.txtRbsRegistrationNumber);
            this.tabSettings.Controls.Add(this.btnSearchFolder);
            this.tabSettings.Controls.Add(this.labelControl6);
            this.tabSettings.Controls.Add(this.txtAbcFolderPath);
            this.tabSettings.Controls.Add(this.labelControl2);
            this.tabSettings.Controls.Add(this.txtIPAddress);
            this.tabSettings.Controls.Add(this.labelControl4);
            this.tabSettings.Controls.Add(this.txtComPort);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.PageVisible = false;
            this.tabSettings.Size = new System.Drawing.Size(474, 314);
            this.tabSettings.Text = "Ρυθμίσεις Σύνδεσης";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(23, 129);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(120, 13);
            this.labelControl7.TabIndex = 10;
            this.labelControl7.Text = "RBS Registration Number";
            // 
            // txtRbsRegistrationNumber
            // 
            this.txtRbsRegistrationNumber.Location = new System.Drawing.Point(187, 126);
            this.txtRbsRegistrationNumber.Name = "txtRbsRegistrationNumber";
            this.txtRbsRegistrationNumber.Size = new System.Drawing.Size(218, 20);
            this.txtRbsRegistrationNumber.TabIndex = 9;
            // 
            // btnSearchFolder
            // 
            this.btnSearchFolder.Location = new System.Drawing.Point(422, 87);
            this.btnSearchFolder.Name = "btnSearchFolder";
            this.btnSearchFolder.Size = new System.Drawing.Size(28, 20);
            this.btnSearchFolder.TabIndex = 8;
            this.btnSearchFolder.Text = "...";
            this.btnSearchFolder.Click += new System.EventHandler(this.btnSearchFolder_Click_1);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(23, 90);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(134, 13);
            this.labelControl6.TabIndex = 7;
            this.labelControl6.Text = "Φάκελος αποθ. υπογραφών";
            // 
            // txtAbcFolderPath
            // 
            this.txtAbcFolderPath.Location = new System.Drawing.Point(187, 84);
            this.txtAbcFolderPath.Name = "txtAbcFolderPath";
            this.txtAbcFolderPath.Size = new System.Drawing.Size(218, 20);
            this.txtAbcFolderPath.TabIndex = 6;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(23, 53);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(53, 13);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "ΕΑΦΔΣΣ IP";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(187, 46);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(218, 20);
            this.txtIPAddress.TabIndex = 4;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(23, 17);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(46, 13);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "COM Port";
            // 
            // txtComPort
            // 
            this.txtComPort.EditValue = "com1";
            this.txtComPort.Location = new System.Drawing.Point(187, 14);
            this.txtComPort.Name = "txtComPort";
            this.txtComPort.Size = new System.Drawing.Size(218, 20);
            this.txtComPort.TabIndex = 2;
            // 
            // tabGGPSSettings
            // 
            this.tabGGPSSettings.Controls.Add(this.txtSmtpPort);
            this.tabGGPSSettings.Controls.Add(this.labelControl13);
            this.tabGGPSSettings.Controls.Add(this.labelControl12);
            this.tabGGPSSettings.Controls.Add(this.txtSmtpPass);
            this.tabGGPSSettings.Controls.Add(this.labelControl11);
            this.tabGGPSSettings.Controls.Add(this.txtSmtpUser);
            this.tabGGPSSettings.Controls.Add(this.labelControl10);
            this.tabGGPSSettings.Controls.Add(this.txtSmtpServer);
            this.tabGGPSSettings.Controls.Add(this.txtMailList);
            this.tabGGPSSettings.Controls.Add(this.labelControl9);
            this.tabGGPSSettings.Controls.Add(this.checkMailFail);
            this.tabGGPSSettings.Controls.Add(this.checkSendSuccessMail);
            this.tabGGPSSettings.Controls.Add(this.labelControl8);
            this.tabGGPSSettings.Controls.Add(this.txtGGPSUrl);
            this.tabGGPSSettings.Controls.Add(this.labelSeriaNumber);
            this.tabGGPSSettings.Controls.Add(this.txtEafdssSN);
            this.tabGGPSSettings.Controls.Add(this.txtAesKey);
            this.tabGGPSSettings.Controls.Add(this.labelAesKey);
            this.tabGGPSSettings.Name = "tabGGPSSettings";
            this.tabGGPSSettings.Size = new System.Drawing.Size(474, 314);
            this.tabGGPSSettings.Text = "Αποστολή Αρχείων ΑΑΔΕ";
            // 
            // labelControl12
            // 
            this.labelControl12.Location = new System.Drawing.Point(233, 281);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Size = new System.Drawing.Size(85, 13);
            this.labelControl12.TabIndex = 31;
            this.labelControl12.Text = "SMTP Password : ";
            // 
            // txtSmtpPass
            // 
            this.txtSmtpPass.EditValue = "";
            this.txtSmtpPass.Location = new System.Drawing.Point(332, 278);
            this.txtSmtpPass.Name = "txtSmtpPass";
            this.txtSmtpPass.Size = new System.Drawing.Size(112, 20);
            this.txtSmtpPass.TabIndex = 30;
            // 
            // labelControl11
            // 
            this.labelControl11.Location = new System.Drawing.Point(15, 281);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Size = new System.Drawing.Size(61, 13);
            this.labelControl11.TabIndex = 29;
            this.labelControl11.Text = "SMTP User : ";
            // 
            // txtSmtpUser
            // 
            this.txtSmtpUser.EditValue = "";
            this.txtSmtpUser.Location = new System.Drawing.Point(104, 274);
            this.txtSmtpUser.Name = "txtSmtpUser";
            this.txtSmtpUser.Size = new System.Drawing.Size(112, 20);
            this.txtSmtpUser.TabIndex = 28;
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(15, 250);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(71, 13);
            this.labelControl10.TabIndex = 27;
            this.labelControl10.Text = "SMTP Server : ";
            // 
            // txtSmtpServer
            // 
            this.txtSmtpServer.EditValue = "";
            this.txtSmtpServer.Location = new System.Drawing.Point(104, 243);
            this.txtSmtpServer.Name = "txtSmtpServer";
            this.txtSmtpServer.Size = new System.Drawing.Size(200, 20);
            this.txtSmtpServer.TabIndex = 26;
            // 
            // txtMailList
            // 
            this.txtMailList.Location = new System.Drawing.Point(102, 187);
            this.txtMailList.Name = "txtMailList";
            this.txtMailList.Size = new System.Drawing.Size(342, 40);
            this.txtMailList.TabIndex = 25;
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(11, 189);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(63, 13);
            this.labelControl9.TabIndex = 24;
            this.labelControl9.Text = "Λίστα Email : ";
            // 
            // checkMailFail
            // 
            this.checkMailFail.AutoSize = true;
            this.checkMailFail.Location = new System.Drawing.Point(105, 155);
            this.checkMailFail.Name = "checkMailFail";
            this.checkMailFail.Size = new System.Drawing.Size(120, 17);
            this.checkMailFail.TabIndex = 23;
            this.checkMailFail.Text = "Αποστολή Fail Email";
            this.checkMailFail.UseVisualStyleBackColor = true;
            // 
            // checkSendSuccessMail
            // 
            this.checkSendSuccessMail.AutoSize = true;
            this.checkSendSuccessMail.Location = new System.Drawing.Point(302, 155);
            this.checkSendSuccessMail.Name = "checkSendSuccessMail";
            this.checkSendSuccessMail.Size = new System.Drawing.Size(142, 17);
            this.checkSendSuccessMail.TabIndex = 22;
            this.checkSendSuccessMail.Text = "Αποστολή Success Email";
            this.checkSendSuccessMail.UseVisualStyleBackColor = true;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(15, 56);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(59, 13);
            this.labelControl8.TabIndex = 12;
            this.labelControl8.Text = "ΑΑΔΕ URL : ";
            // 
            // txtGGPSUrl
            // 
            this.txtGGPSUrl.EditValue = "";
            this.txtGGPSUrl.Location = new System.Drawing.Point(104, 49);
            this.txtGGPSUrl.Name = "txtGGPSUrl";
            this.txtGGPSUrl.Size = new System.Drawing.Size(340, 20);
            this.txtGGPSUrl.TabIndex = 11;
            // 
            // labelSeriaNumber
            // 
            this.labelSeriaNumber.Location = new System.Drawing.Point(15, 17);
            this.labelSeriaNumber.Name = "labelSeriaNumber";
            this.labelSeriaNumber.Size = new System.Drawing.Size(70, 13);
            this.labelSeriaNumber.TabIndex = 10;
            this.labelSeriaNumber.Text = "ΕΑΦΔΣΣ S/N : ";
            // 
            // txtEafdssSN
            // 
            this.txtEafdssSN.EditValue = "";
            this.txtEafdssSN.Location = new System.Drawing.Point(104, 14);
            this.txtEafdssSN.Name = "txtEafdssSN";
            this.txtEafdssSN.Size = new System.Drawing.Size(340, 20);
            this.txtEafdssSN.TabIndex = 9;
            // 
            // txtAesKey
            // 
            this.txtAesKey.Location = new System.Drawing.Point(104, 93);
            this.txtAesKey.Name = "txtAesKey";
            this.txtAesKey.Size = new System.Drawing.Size(340, 40);
            this.txtAesKey.TabIndex = 8;
            // 
            // labelAesKey
            // 
            this.labelAesKey.Location = new System.Drawing.Point(15, 108);
            this.labelAesKey.Name = "labelAesKey";
            this.labelAesKey.Size = new System.Drawing.Size(50, 13);
            this.labelAesKey.TabIndex = 7;
            this.labelAesKey.Text = "AES Key : ";
            // 
            // radCOM
            // 
            this.radCOM.AutoSize = true;
            this.radCOM.Location = new System.Drawing.Point(129, 62);
            this.radCOM.Name = "radCOM";
            this.radCOM.Size = new System.Drawing.Size(48, 17);
            this.radCOM.TabIndex = 6;
            this.radCOM.Text = "COM";
            this.radCOM.UseVisualStyleBackColor = true;
            this.radCOM.CheckedChanged += new System.EventHandler(this.radEthernet_CheckedChanged);
            // 
            // radEthernet
            // 
            this.radEthernet.AutoSize = true;
            this.radEthernet.Checked = true;
            this.radEthernet.Location = new System.Drawing.Point(183, 60);
            this.radEthernet.Name = "radEthernet";
            this.radEthernet.Size = new System.Drawing.Size(67, 17);
            this.radEthernet.TabIndex = 7;
            this.radEthernet.TabStop = true;
            this.radEthernet.Text = "Ethernet";
            this.radEthernet.UseVisualStyleBackColor = true;
            this.radEthernet.CheckedChanged += new System.EventHandler(this.radEthernet_CheckedChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(19, 64);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(86, 13);
            this.labelControl3.TabIndex = 1;
            this.labelControl3.Text = "ΕΑΦΔΣΣ Settings:";
            // 
            // btnInstallService
            // 
            this.btnInstallService.Location = new System.Drawing.Point(6, 20);
            this.btnInstallService.Name = "btnInstallService";
            this.btnInstallService.Size = new System.Drawing.Size(150, 23);
            this.btnInstallService.TabIndex = 3;
            this.btnInstallService.Text = "Install Service";
            this.btnInstallService.Click += new System.EventHandler(this.btnInstallService_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(546, 487);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(142, 50);
            this.btnSaveSettings.TabIndex = 3;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // picAlgoboxStatus
            // 
            this.picAlgoboxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAlgoboxStatus.Image = global::ITS.POS.Fiscal.GUI.Properties.Resources.offline_dot;
            this.picAlgoboxStatus.Location = new System.Drawing.Point(3, 17);
            this.picAlgoboxStatus.Name = "picAlgoboxStatus";
            this.picAlgoboxStatus.Size = new System.Drawing.Size(55, 36);
            this.picAlgoboxStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picAlgoboxStatus.TabIndex = 8;
            this.picAlgoboxStatus.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picServiceStatus);
            this.groupBox1.Location = new System.Drawing.Point(546, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(61, 56);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.picAlgoboxStatus);
            this.groupBox2.Location = new System.Drawing.Point(635, 115);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(61, 56);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ΕΑΦΔΣΣ";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(756, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Controls.Add(this.btnInstallService);
            this.groupBox3.Controls.Add(this.btnStartService);
            this.groupBox3.Location = new System.Drawing.Point(21, 449);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(166, 100);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Service Management";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnGetIDSN);
            this.groupBox4.Controls.Add(this.btnGetVersion);
            this.groupBox4.Controls.Add(this.btnIssueZ);
            this.groupBox4.Location = new System.Drawing.Point(320, 455);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(209, 100);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Commands";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblServiceStatus,
            this.toolStripStatusLabel3,
            this.lblServiceConnected,
            this.toolStripStatusLabel5,
            this.lblFiscalStatus,
            this.lblAlgoSN});
            this.statusStrip1.Location = new System.Drawing.Point(0, 583);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(756, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel1.Text = "Service Status";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblServiceStatus
            // 
            this.lblServiceStatus.AutoSize = false;
            this.lblServiceStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(70, 17);
            this.lblServiceStatus.Text = "Running";
            this.lblServiceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(70, 17);
            this.toolStripStatusLabel3.Text = "Service";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblServiceConnected
            // 
            this.lblServiceConnected.AutoSize = false;
            this.lblServiceConnected.Name = "lblServiceConnected";
            this.lblServiceConnected.Size = new System.Drawing.Size(80, 17);
            this.lblServiceConnected.Text = "Connected";
            this.lblServiceConnected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.AutoSize = false;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabel5.Text = "ΕΑΦΔΣΣ Status";
            this.toolStripStatusLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFiscalStatus
            // 
            this.lblFiscalStatus.AutoSize = false;
            this.lblFiscalStatus.Image = global::ITS.POS.Fiscal.GUI.Properties.Resources.online_dot;
            this.lblFiscalStatus.Name = "lblFiscalStatus";
            this.lblFiscalStatus.Size = new System.Drawing.Size(30, 17);
            this.lblFiscalStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlgoSN
            // 
            this.lblAlgoSN.AutoSize = false;
            this.lblAlgoSN.Name = "lblAlgoSN";
            this.lblAlgoSN.Size = new System.Drawing.Size(100, 17);
            this.lblAlgoSN.Text = "asdfasfsadf";
            // 
            // taskBarIcon
            // 
            this.taskBarIcon.ContextMenuStrip = this.mnuStrip;
            this.taskBarIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("taskBarIcon.Icon")));
            this.taskBarIcon.Text = "notifyIcon1";
            this.taskBarIcon.Visible = true;
            this.taskBarIcon.Click += new System.EventHandler(this.taskBarIcon_Click);
            this.taskBarIcon.DoubleClick += new System.EventHandler(this.taskBarIcon_DoubleClick);
            // 
            // mnuStrip
            // 
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.issueZToolStripMenuItem,
            this.mnuExitToolStripMenuItem});
            this.mnuStrip.Name = "mnuOpen";
            this.mnuStrip.Size = new System.Drawing.Size(181, 70);
            this.mnuStrip.Text = "Show Window";
            // 
            // mnuOpen
            // 
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Size = new System.Drawing.Size(180, 22);
            this.mnuOpen.Text = "toolStripMenuItem1";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // issueZToolStripMenuItem
            // 
            this.issueZToolStripMenuItem.Name = "issueZToolStripMenuItem";
            this.issueZToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.issueZToolStripMenuItem.Text = "Έκδοση Z";
            this.issueZToolStripMenuItem.Click += new System.EventHandler(this.issueZToolStripMenuItem_Click);
            // 
            // mnuExitToolStripMenuItem
            // 
            this.mnuExitToolStripMenuItem.Name = "mnuExitToolStripMenuItem";
            this.mnuExitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mnuExitToolStripMenuItem.Text = "Έξοδος";
            this.mnuExitToolStripMenuItem.Click += new System.EventHandler(this.mnuExitToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(673, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 70);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(292, 64);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(71, 13);
            this.labelControl5.TabIndex = 1;
            this.labelControl5.Text = "ΕΑΦΔΣΣ Type:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Algobox",
            "Rbs 101",
            "AlgoboxV2"});
            this.comboBox1.Location = new System.Drawing.Point(379, 61);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(137, 21);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // chkBoxInError
            // 
            this.chkBoxInError.AutoSize = true;
            this.chkBoxInError.Location = new System.Drawing.Point(292, 31);
            this.chkBoxInError.Name = "chkBoxInError";
            this.chkBoxInError.Size = new System.Drawing.Size(81, 17);
            this.chkBoxInError.TabIndex = 17;
            this.chkBoxInError.Text = "Σε Βλαβη !!";
            this.chkBoxInError.UseVisualStyleBackColor = true;
            // 
            // btnSendOnlineCheck
            // 
            this.btnSendOnlineCheck.Location = new System.Drawing.Point(6, 20);
            this.btnSendOnlineCheck.Name = "btnSendOnlineCheck";
            this.btnSendOnlineCheck.Size = new System.Drawing.Size(101, 23);
            this.btnSendOnlineCheck.TabIndex = 18;
            this.btnSendOnlineCheck.Text = "Send Online Check";
            this.btnSendOnlineCheck.Click += new System.EventHandler(this.btnSendOnlineCheck_Click);
            // 
            // grbDebug
            // 
            this.grbDebug.Controls.Add(this.simpleButtonViewLog);
            this.grbDebug.Controls.Add(this.btnSendOnlineCheck);
            this.grbDebug.Location = new System.Drawing.Point(193, 455);
            this.grbDebug.Name = "grbDebug";
            this.grbDebug.Size = new System.Drawing.Size(113, 99);
            this.grbDebug.TabIndex = 19;
            this.grbDebug.TabStop = false;
            this.grbDebug.Text = "Debug";
            this.grbDebug.Visible = false;
            // 
            // simpleButtonViewLog
            // 
            this.simpleButtonViewLog.Location = new System.Drawing.Point(6, 49);
            this.simpleButtonViewLog.Name = "simpleButtonViewLog";
            this.simpleButtonViewLog.Size = new System.Drawing.Size(101, 23);
            this.simpleButtonViewLog.TabIndex = 19;
            this.simpleButtonViewLog.Text = "View Log";
            this.simpleButtonViewLog.Click += new System.EventHandler(this.simpleButtonViewLog_Click);
            // 
            // checkBoxKeepLog
            // 
            this.checkBoxKeepLog.AutoSize = true;
            this.checkBoxKeepLog.Location = new System.Drawing.Point(379, 31);
            this.checkBoxKeepLog.Name = "checkBoxKeepLog";
            this.checkBoxKeepLog.Size = new System.Drawing.Size(101, 17);
            this.checkBoxKeepLog.TabIndex = 20;
            this.checkBoxKeepLog.Text = "Καταγραφή Log";
            this.checkBoxKeepLog.UseVisualStyleBackColor = true;
            // 
            // checkBoχSendFiles
            // 
            this.checkBoχSendFiles.AutoSize = true;
            this.checkBoχSendFiles.Location = new System.Drawing.Point(504, 31);
            this.checkBoχSendFiles.Name = "checkBoχSendFiles";
            this.checkBoχSendFiles.Size = new System.Drawing.Size(147, 17);
            this.checkBoχSendFiles.TabIndex = 21;
            this.checkBoχSendFiles.Text = "Αποστολή Αρχείων ΑΑΔΕ";
            this.checkBoχSendFiles.UseVisualStyleBackColor = true;
            this.checkBoχSendFiles.CheckedChanged += new System.EventHandler(this.checkBoχSendFiles_CheckedChanged);
            // 
            // labelControl13
            // 
            this.labelControl13.Location = new System.Drawing.Point(320, 246);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(59, 13);
            this.labelControl13.TabIndex = 32;
            this.labelControl13.Text = "SMTP Port : ";
            // 
            // txtSmtpPort
            // 
            this.txtSmtpPort.EditValue = "";
            this.txtSmtpPort.Location = new System.Drawing.Point(385, 243);
            this.txtSmtpPort.Name = "txtSmtpPort";
            this.txtSmtpPort.Size = new System.Drawing.Size(58, 20);
            this.txtSmtpPort.TabIndex = 33;
            // 
            // CoffeeSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 605);
            this.Controls.Add(this.checkBoχSendFiles);
            this.Controls.Add(this.checkBoxKeepLog);
            this.Controls.Add(this.grbDebug);
            this.Controls.Add(this.chkBoxInError);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radEthernet);
            this.Controls.Add(this.radCOM);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtListeningPort);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoffeeSettingsForm";
            this.Text = "FICOS Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoffeeSettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.CoffeeSettingsForm_Load);
            this.Shown += new System.EventHandler(this.CoffeeSettingsForm_Shown);
            this.ClientSizeChanged += new System.EventHandler(this.CoffeeSettingsForm_ClientSizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.txtListeningPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picServiceStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRbsRegistrationNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbcFolderPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIPAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComPort.Properties)).EndInit();
            this.tabGGPSSettings.ResumeLayout(false);
            this.tabGGPSSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpPass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMailList.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGGPSUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEafdssSN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAesKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAlgoboxStatus)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grbDebug.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtpPort.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtListeningPort;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnConnect;
        private DevExpress.XtraEditors.SimpleButton btnGetVersion;
        private DevExpress.XtraEditors.SimpleButton btnGetIDSN;
        private DevExpress.XtraEditors.SimpleButton btnIssueZ;
        private DevExpress.XtraEditors.SimpleButton btnStartService;
        private System.Windows.Forms.PictureBox picServiceStatus;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage tabSettings;
        private DevExpress.XtraTab.XtraTabPage tabGGPSSettings;
        private System.Windows.Forms.RadioButton radCOM;
        private System.Windows.Forms.RadioButton radEthernet;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtComPort;
        private DevExpress.XtraEditors.SimpleButton btnInstallService;
        private DevExpress.XtraEditors.SimpleButton btnSaveSettings;
        private System.Windows.Forms.PictureBox picAlgoboxStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblServiceStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lblServiceConnected;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel lblFiscalStatus;
        private System.Windows.Forms.NotifyIcon taskBarIcon;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private BindableToolStripStatusLabel lblAlgoSN;
        private System.Windows.Forms.ContextMenuStrip mnuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox chkBoxInError;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem issueZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuExitToolStripMenuItem;
        private DevExpress.XtraEditors.SimpleButton btnSendOnlineCheck;
        private System.Windows.Forms.GroupBox grbDebug;
        private System.Windows.Forms.CheckBox checkBoxKeepLog;
        private DevExpress.XtraEditors.SimpleButton simpleButtonViewLog;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit txtRbsRegistrationNumber;
        private DevExpress.XtraEditors.SimpleButton btnSearchFolder;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtAbcFolderPath;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtIPAddress;
        private DevExpress.XtraEditors.LabelControl labelAesKey;
        private MemoEdit txtAesKey;
        private LabelControl labelControl8;
        private TextEdit txtGGPSUrl;
        private LabelControl labelSeriaNumber;
        private TextEdit txtEafdssSN;
        private System.Windows.Forms.CheckBox checkBoχSendFiles;
        private System.Windows.Forms.CheckBox checkMailFail;
        private System.Windows.Forms.CheckBox checkSendSuccessMail;
        private MemoEdit txtMailList;
        private LabelControl labelControl9;
        private LabelControl labelControl12;
        private TextEdit txtSmtpPass;
        private LabelControl labelControl11;
        private TextEdit txtSmtpUser;
        private LabelControl labelControl10;
        private TextEdit txtSmtpServer;
        private TextEdit txtSmtpPort;
        private LabelControl labelControl13;
    }
}

