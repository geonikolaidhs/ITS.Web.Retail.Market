namespace ITS.Retail.WrmDbTransfer
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTestSrcConnection = new DevExpress.XtraEditors.SimpleButton();
            this.btnUpdateSrcSettings = new DevExpress.XtraEditors.SimpleButton();
            this.txtSrcDbType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtSrcPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtSrcUsername = new DevExpress.XtraEditors.TextEdit();
            this.txtsrcDatabase = new DevExpress.XtraEditors.TextEdit();
            this.txtSrcServer = new DevExpress.XtraEditors.TextEdit();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSrcDbType = new System.Windows.Forms.Label();
            this.lblSrcServer = new System.Windows.Forms.Label();
            this.lblSrcHeader = new DevExpress.XtraEditors.LabelControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioVersion = new DevExpress.XtraEditors.RadioGroup();
            this.btnTransferData = new DevExpress.XtraEditors.SimpleButton();
            this.btnTruncateTargetTables = new DevExpress.XtraEditors.SimpleButton();
            this.btnCreateTargetSchema = new DevExpress.XtraEditors.SimpleButton();
            this.btnTestTargetConnection = new DevExpress.XtraEditors.SimpleButton();
            this.btnUpdateTargetConnection = new DevExpress.XtraEditors.SimpleButton();
            this.txtTargetDbType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtTargetPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtTargetUsername = new DevExpress.XtraEditors.TextEdit();
            this.txtTargetDatabase = new DevExpress.XtraEditors.TextEdit();
            this.txtTargetServer = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTargetHeader = new DevExpress.XtraEditors.LabelControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcDbType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtsrcDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcServer.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioVersion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetDbType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetServer.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Appearance.BackColor = System.Drawing.SystemColors.GrayText;
            this.xtraTabControl1.Appearance.Options.UseBackColor = true;
            this.xtraTabControl1.AppearancePage.Header.BackColor = System.Drawing.Color.SandyBrown;
            this.xtraTabControl1.AppearancePage.Header.Options.UseBackColor = true;
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.xtraTabControl1.MaxTabPageWidth = 150;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
            this.xtraTabControl1.Size = new System.Drawing.Size(1252, 875);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Appearance.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.xtraTabPage1.Appearance.Header.Options.UseBackColor = true;
            this.xtraTabPage1.Appearance.HeaderActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.xtraTabPage1.Appearance.HeaderActive.Options.UseBackColor = true;
            this.xtraTabPage1.Controls.Add(this.tableLayoutPanel1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(1246, 847);
            this.xtraTabPage1.Text = "Database Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1246, 847);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnTestSrcConnection);
            this.panel1.Controls.Add(this.btnUpdateSrcSettings);
            this.panel1.Controls.Add(this.txtSrcDbType);
            this.panel1.Controls.Add(this.txtSrcPassword);
            this.panel1.Controls.Add(this.txtSrcUsername);
            this.panel1.Controls.Add(this.txtsrcDatabase);
            this.panel1.Controls.Add(this.txtSrcServer);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblSrcDbType);
            this.panel1.Controls.Add(this.lblSrcServer);
            this.panel1.Controls.Add(this.lblSrcHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(617, 841);
            this.panel1.TabIndex = 0;
            // 
            // btnTestSrcConnection
            // 
            this.btnTestSrcConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnTestSrcConnection.Image")));
            this.btnTestSrcConnection.Location = new System.Drawing.Point(22, 418);
            this.btnTestSrcConnection.Name = "btnTestSrcConnection";
            this.btnTestSrcConnection.Size = new System.Drawing.Size(137, 34);
            this.btnTestSrcConnection.TabIndex = 12;
            this.btnTestSrcConnection.Text = "Test Connection";
            this.btnTestSrcConnection.Click += new System.EventHandler(this.btnTestSrcConnection_Click);
            // 
            // btnUpdateSrcSettings
            // 
            this.btnUpdateSrcSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateSrcSettings.Image")));
            this.btnUpdateSrcSettings.Location = new System.Drawing.Point(22, 357);
            this.btnUpdateSrcSettings.Name = "btnUpdateSrcSettings";
            this.btnUpdateSrcSettings.Size = new System.Drawing.Size(137, 34);
            this.btnUpdateSrcSettings.TabIndex = 11;
            this.btnUpdateSrcSettings.Text = "Update Settings";
            this.btnUpdateSrcSettings.Click += new System.EventHandler(this.btnUpdateSrcSettings_Click);
            // 
            // txtSrcDbType
            // 
            this.txtSrcDbType.Location = new System.Drawing.Point(216, 101);
            this.txtSrcDbType.Name = "txtSrcDbType";
            this.txtSrcDbType.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtSrcDbType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
            this.txtSrcDbType.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtSrcDbType.Properties.Appearance.Options.UseBackColor = true;
            this.txtSrcDbType.Properties.Appearance.Options.UseFont = true;
            this.txtSrcDbType.Properties.Appearance.Options.UseForeColor = true;
            this.txtSrcDbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtSrcDbType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.txtSrcDbType.Size = new System.Drawing.Size(255, 20);
            this.txtSrcDbType.TabIndex = 10;
            // 
            // txtSrcPassword
            // 
            this.txtSrcPassword.EditValue = "";
            this.txtSrcPassword.Location = new System.Drawing.Point(216, 278);
            this.txtSrcPassword.Name = "txtSrcPassword";
            this.txtSrcPassword.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtSrcPassword.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrcPassword.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtSrcPassword.Properties.Appearance.Options.UseBackColor = true;
            this.txtSrcPassword.Properties.Appearance.Options.UseFont = true;
            this.txtSrcPassword.Properties.Appearance.Options.UseForeColor = true;
            this.txtSrcPassword.Size = new System.Drawing.Size(255, 22);
            this.txtSrcPassword.TabIndex = 9;
            // 
            // txtSrcUsername
            // 
            this.txtSrcUsername.EditValue = "";
            this.txtSrcUsername.Location = new System.Drawing.Point(216, 235);
            this.txtSrcUsername.Name = "txtSrcUsername";
            this.txtSrcUsername.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtSrcUsername.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrcUsername.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtSrcUsername.Properties.Appearance.Options.UseBackColor = true;
            this.txtSrcUsername.Properties.Appearance.Options.UseFont = true;
            this.txtSrcUsername.Properties.Appearance.Options.UseForeColor = true;
            this.txtSrcUsername.Size = new System.Drawing.Size(255, 22);
            this.txtSrcUsername.TabIndex = 8;
            // 
            // txtsrcDatabase
            // 
            this.txtsrcDatabase.EditValue = "";
            this.txtsrcDatabase.Location = new System.Drawing.Point(216, 192);
            this.txtsrcDatabase.Name = "txtsrcDatabase";
            this.txtsrcDatabase.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtsrcDatabase.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsrcDatabase.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtsrcDatabase.Properties.Appearance.Options.UseBackColor = true;
            this.txtsrcDatabase.Properties.Appearance.Options.UseFont = true;
            this.txtsrcDatabase.Properties.Appearance.Options.UseForeColor = true;
            this.txtsrcDatabase.Size = new System.Drawing.Size(255, 22);
            this.txtsrcDatabase.TabIndex = 7;
            // 
            // txtSrcServer
            // 
            this.txtSrcServer.EditValue = "";
            this.txtSrcServer.Location = new System.Drawing.Point(216, 149);
            this.txtSrcServer.Name = "txtSrcServer";
            this.txtSrcServer.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtSrcServer.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrcServer.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtSrcServer.Properties.Appearance.Options.UseBackColor = true;
            this.txtSrcServer.Properties.Appearance.Options.UseFont = true;
            this.txtSrcServer.Properties.Appearance.Options.UseForeColor = true;
            this.txtSrcServer.Size = new System.Drawing.Size(255, 22);
            this.txtSrcServer.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Location = new System.Drawing.Point(19, 280);
            this.label4.MinimumSize = new System.Drawing.Size(100, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(19, 237);
            this.label3.MinimumSize = new System.Drawing.Size(100, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(19, 194);
            this.label2.MinimumSize = new System.Drawing.Size(100, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database";
            // 
            // lblSrcDbType
            // 
            this.lblSrcDbType.AutoSize = true;
            this.lblSrcDbType.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblSrcDbType.ForeColor = System.Drawing.Color.LightGray;
            this.lblSrcDbType.Location = new System.Drawing.Point(19, 101);
            this.lblSrcDbType.MinimumSize = new System.Drawing.Size(100, 20);
            this.lblSrcDbType.Name = "lblSrcDbType";
            this.lblSrcDbType.Size = new System.Drawing.Size(100, 20);
            this.lblSrcDbType.TabIndex = 2;
            this.lblSrcDbType.Text = "DbType";
            // 
            // lblSrcServer
            // 
            this.lblSrcServer.AutoSize = true;
            this.lblSrcServer.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblSrcServer.ForeColor = System.Drawing.Color.LightGray;
            this.lblSrcServer.Location = new System.Drawing.Point(19, 149);
            this.lblSrcServer.MinimumSize = new System.Drawing.Size(100, 20);
            this.lblSrcServer.Name = "lblSrcServer";
            this.lblSrcServer.Size = new System.Drawing.Size(100, 20);
            this.lblSrcServer.TabIndex = 1;
            this.lblSrcServer.Text = "Server";
            // 
            // lblSrcHeader
            // 
            this.lblSrcHeader.Appearance.Font = new System.Drawing.Font("Courier New", 18.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Pixel);
            this.lblSrcHeader.Appearance.ForeColor = System.Drawing.Color.LightGray;
            this.lblSrcHeader.Location = new System.Drawing.Point(216, 38);
            this.lblSrcHeader.MinimumSize = new System.Drawing.Size(150, 25);
            this.lblSrcHeader.Name = "lblSrcHeader";
            this.lblSrcHeader.Size = new System.Drawing.Size(165, 25);
            this.lblSrcHeader.TabIndex = 0;
            this.lblSrcHeader.Text = "Source Database";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioVersion);
            this.panel2.Controls.Add(this.btnTransferData);
            this.panel2.Controls.Add(this.btnTruncateTargetTables);
            this.panel2.Controls.Add(this.btnCreateTargetSchema);
            this.panel2.Controls.Add(this.btnTestTargetConnection);
            this.panel2.Controls.Add(this.btnUpdateTargetConnection);
            this.panel2.Controls.Add(this.txtTargetDbType);
            this.panel2.Controls.Add(this.txtTargetPassword);
            this.panel2.Controls.Add(this.txtTargetUsername);
            this.panel2.Controls.Add(this.txtTargetDatabase);
            this.panel2.Controls.Add(this.txtTargetServer);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.lblTargetHeader);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(626, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(617, 841);
            this.panel2.TabIndex = 1;
            // 
            // radioVersion
            // 
            this.radioVersion.Location = new System.Drawing.Point(49, 519);
            this.radioVersion.Name = "radioVersion";
            this.radioVersion.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.radioVersion.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
            this.radioVersion.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.radioVersion.Properties.Appearance.Options.UseBackColor = true;
            this.radioVersion.Properties.Appearance.Options.UseFont = true;
            this.radioVersion.Properties.Appearance.Options.UseForeColor = true;
            this.radioVersion.Properties.Columns = 1;
            this.radioVersion.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.radioVersion.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "  Start Transfer From Current Version"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "  Start Transfer From Version 0")});
            this.radioVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radioVersion.Size = new System.Drawing.Size(430, 96);
            this.radioVersion.TabIndex = 21;
            // 
            // btnTransferData
            // 
            this.btnTransferData.Image = ((System.Drawing.Image)(resources.GetObject("btnTransferData.Image")));
            this.btnTransferData.Location = new System.Drawing.Point(331, 429);
            this.btnTransferData.Name = "btnTransferData";
            this.btnTransferData.Size = new System.Drawing.Size(137, 34);
            this.btnTransferData.TabIndex = 20;
            this.btnTransferData.Text = "Start Transfer Data";
            this.btnTransferData.Click += new System.EventHandler(this.btnTransferData_Click);
            // 
            // btnTruncateTargetTables
            // 
            this.btnTruncateTargetTables.Location = new System.Drawing.Point(0, 0);
            this.btnTruncateTargetTables.Name = "btnTruncateTargetTables";
            this.btnTruncateTargetTables.Size = new System.Drawing.Size(75, 23);
            this.btnTruncateTargetTables.TabIndex = 22;
            // 
            // btnCreateTargetSchema
            // 
            this.btnCreateTargetSchema.Image = ((System.Drawing.Image)(resources.GetObject("btnCreateTargetSchema.Image")));
            this.btnCreateTargetSchema.Location = new System.Drawing.Point(331, 357);
            this.btnCreateTargetSchema.Name = "btnCreateTargetSchema";
            this.btnCreateTargetSchema.Size = new System.Drawing.Size(137, 34);
            this.btnCreateTargetSchema.TabIndex = 18;
            this.btnCreateTargetSchema.Text = "Create Schema";
            this.btnCreateTargetSchema.Click += new System.EventHandler(this.btnCreateTargetSchema_Click);
            // 
            // btnTestTargetConnection
            // 
            this.btnTestTargetConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnTestTargetConnection.Image")));
            this.btnTestTargetConnection.Location = new System.Drawing.Point(49, 418);
            this.btnTestTargetConnection.Name = "btnTestTargetConnection";
            this.btnTestTargetConnection.Size = new System.Drawing.Size(137, 34);
            this.btnTestTargetConnection.TabIndex = 17;
            this.btnTestTargetConnection.Text = "Test Connection";
            this.btnTestTargetConnection.Click += new System.EventHandler(this.btnTestTargetConnection_Click);
            // 
            // btnUpdateTargetConnection
            // 
            this.btnUpdateTargetConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateTargetConnection.Image")));
            this.btnUpdateTargetConnection.Location = new System.Drawing.Point(49, 357);
            this.btnUpdateTargetConnection.Name = "btnUpdateTargetConnection";
            this.btnUpdateTargetConnection.Size = new System.Drawing.Size(137, 34);
            this.btnUpdateTargetConnection.TabIndex = 16;
            this.btnUpdateTargetConnection.Text = "Update Settings";
            this.btnUpdateTargetConnection.Click += new System.EventHandler(this.btnUpdateTargetConnection_Click);
            // 
            // txtTargetDbType
            // 
            this.txtTargetDbType.Location = new System.Drawing.Point(213, 98);
            this.txtTargetDbType.Name = "txtTargetDbType";
            this.txtTargetDbType.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtTargetDbType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
            this.txtTargetDbType.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtTargetDbType.Properties.Appearance.Options.UseBackColor = true;
            this.txtTargetDbType.Properties.Appearance.Options.UseFont = true;
            this.txtTargetDbType.Properties.Appearance.Options.UseForeColor = true;
            this.txtTargetDbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtTargetDbType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.txtTargetDbType.Size = new System.Drawing.Size(255, 20);
            this.txtTargetDbType.TabIndex = 15;
            // 
            // txtTargetPassword
            // 
            this.txtTargetPassword.EditValue = "";
            this.txtTargetPassword.Location = new System.Drawing.Point(213, 278);
            this.txtTargetPassword.Name = "txtTargetPassword";
            this.txtTargetPassword.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtTargetPassword.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetPassword.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtTargetPassword.Properties.Appearance.Options.UseBackColor = true;
            this.txtTargetPassword.Properties.Appearance.Options.UseFont = true;
            this.txtTargetPassword.Properties.Appearance.Options.UseForeColor = true;
            this.txtTargetPassword.Size = new System.Drawing.Size(255, 22);
            this.txtTargetPassword.TabIndex = 14;
            // 
            // txtTargetUsername
            // 
            this.txtTargetUsername.EditValue = "";
            this.txtTargetUsername.Location = new System.Drawing.Point(213, 235);
            this.txtTargetUsername.Name = "txtTargetUsername";
            this.txtTargetUsername.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtTargetUsername.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetUsername.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtTargetUsername.Properties.Appearance.Options.UseBackColor = true;
            this.txtTargetUsername.Properties.Appearance.Options.UseFont = true;
            this.txtTargetUsername.Properties.Appearance.Options.UseForeColor = true;
            this.txtTargetUsername.Size = new System.Drawing.Size(255, 22);
            this.txtTargetUsername.TabIndex = 13;
            // 
            // txtTargetDatabase
            // 
            this.txtTargetDatabase.EditValue = "";
            this.txtTargetDatabase.Location = new System.Drawing.Point(213, 192);
            this.txtTargetDatabase.Name = "txtTargetDatabase";
            this.txtTargetDatabase.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtTargetDatabase.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetDatabase.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtTargetDatabase.Properties.Appearance.Options.UseBackColor = true;
            this.txtTargetDatabase.Properties.Appearance.Options.UseFont = true;
            this.txtTargetDatabase.Properties.Appearance.Options.UseForeColor = true;
            this.txtTargetDatabase.Size = new System.Drawing.Size(255, 22);
            this.txtTargetDatabase.TabIndex = 12;
            // 
            // txtTargetServer
            // 
            this.txtTargetServer.EditValue = "";
            this.txtTargetServer.Location = new System.Drawing.Point(213, 149);
            this.txtTargetServer.Name = "txtTargetServer";
            this.txtTargetServer.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.txtTargetServer.Properties.Appearance.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetServer.Properties.Appearance.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtTargetServer.Properties.Appearance.Options.UseBackColor = true;
            this.txtTargetServer.Properties.Appearance.Options.UseFont = true;
            this.txtTargetServer.Properties.Appearance.Options.UseForeColor = true;
            this.txtTargetServer.Size = new System.Drawing.Size(255, 22);
            this.txtTargetServer.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(46, 280);
            this.label1.MinimumSize = new System.Drawing.Size(100, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(46, 237);
            this.label5.MinimumSize = new System.Drawing.Size(100, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Username";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label6.ForeColor = System.Drawing.Color.LightGray;
            this.label6.Location = new System.Drawing.Point(46, 194);
            this.label6.MinimumSize = new System.Drawing.Size(100, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Database";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label7.ForeColor = System.Drawing.Color.LightGray;
            this.label7.Location = new System.Drawing.Point(46, 101);
            this.label7.MinimumSize = new System.Drawing.Size(100, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "DbType";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Courier New", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label8.ForeColor = System.Drawing.Color.LightGray;
            this.label8.Location = new System.Drawing.Point(46, 149);
            this.label8.MinimumSize = new System.Drawing.Size(100, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Server";
            // 
            // lblTargetHeader
            // 
            this.lblTargetHeader.Appearance.Font = new System.Drawing.Font("Courier New", 18.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Pixel);
            this.lblTargetHeader.Appearance.ForeColor = System.Drawing.Color.LightGray;
            this.lblTargetHeader.Location = new System.Drawing.Point(213, 38);
            this.lblTargetHeader.MinimumSize = new System.Drawing.Size(150, 25);
            this.lblTargetHeader.Name = "lblTargetHeader";
            this.lblTargetHeader.Size = new System.Drawing.Size(176, 25);
            this.lblTargetHeader.TabIndex = 1;
            this.lblTargetHeader.Text = "Target  Database";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.AllowTouchScroll = true;
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(1246, 847);
            this.xtraTabPage2.Text = "xtraTabPage2";
            // 
            // Main
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 875);
            this.Controls.Add(this.xtraTabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinMaskColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LookAndFeel.SkinMaskColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LookAndFeel.SkinName = "Office 2016 Dark";
            this.Name = "Main";
            this.Text = "Wrm Database Transfer";
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcDbType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtsrcDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSrcServer.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioVersion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetDbType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetServer.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl lblSrcHeader;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.LabelControl lblTargetHeader;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSrcDbType;
        private System.Windows.Forms.Label lblSrcServer;
        private DevExpress.XtraEditors.TextEdit txtSrcServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private DevExpress.XtraEditors.TextEdit txtSrcPassword;
        private DevExpress.XtraEditors.TextEdit txtSrcUsername;
        private DevExpress.XtraEditors.TextEdit txtsrcDatabase;
        private DevExpress.XtraEditors.TextEdit txtTargetPassword;
        private DevExpress.XtraEditors.TextEdit txtTargetUsername;
        private DevExpress.XtraEditors.TextEdit txtTargetDatabase;
        private DevExpress.XtraEditors.TextEdit txtTargetServer;
        private DevExpress.XtraEditors.LookUpEdit txtSrcDbType;
        private DevExpress.XtraEditors.LookUpEdit txtTargetDbType;
        private DevExpress.XtraEditors.SimpleButton btnTestSrcConnection;
        private DevExpress.XtraEditors.SimpleButton btnUpdateSrcSettings;
        private DevExpress.XtraEditors.SimpleButton btnTestTargetConnection;
        private DevExpress.XtraEditors.SimpleButton btnUpdateTargetConnection;
        private DevExpress.XtraEditors.SimpleButton btnCreateTargetSchema;
        private DevExpress.XtraEditors.SimpleButton btnTruncateTargetTables;
        private DevExpress.XtraEditors.SimpleButton btnTransferData;
        private DevExpress.XtraEditors.RadioGroup radioVersion;
    }
}

