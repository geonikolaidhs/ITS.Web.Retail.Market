namespace ITS.POS.Tools.FiscalPrinterConfigurator
{
    partial class frmMain
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
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.cmbPrinter = new System.Windows.Forms.ComboBox();
            this.lblPrinter = new System.Windows.Forms.Label();
            this.lblParity = new System.Windows.Forms.Label();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabHeader = new System.Windows.Forms.TabPage();
            this.cmbPrintType8 = new System.Windows.Forms.ComboBox();
            this.txtLine8 = new System.Windows.Forms.TextBox();
            this.lblLine8 = new System.Windows.Forms.Label();
            this.cmbPrintType7 = new System.Windows.Forms.ComboBox();
            this.txtLine7 = new System.Windows.Forms.TextBox();
            this.lblLine7 = new System.Windows.Forms.Label();
            this.cmbPrintType6 = new System.Windows.Forms.ComboBox();
            this.cmbPrintType5 = new System.Windows.Forms.ComboBox();
            this.cmbPrintType4 = new System.Windows.Forms.ComboBox();
            this.cmbPrintType3 = new System.Windows.Forms.ComboBox();
            this.cmbPrintType2 = new System.Windows.Forms.ComboBox();
            this.cmbPrintType1 = new System.Windows.Forms.ComboBox();
            this.btnSetHeader = new System.Windows.Forms.Button();
            this.txtLine6 = new System.Windows.Forms.TextBox();
            this.lblLine6 = new System.Windows.Forms.Label();
            this.txtLine5 = new System.Windows.Forms.TextBox();
            this.lblLine5 = new System.Windows.Forms.Label();
            this.txtLine4 = new System.Windows.Forms.TextBox();
            this.lblLine4 = new System.Windows.Forms.Label();
            this.txtLine3 = new System.Windows.Forms.TextBox();
            this.lblLine3 = new System.Windows.Forms.Label();
            this.txtLine2 = new System.Windows.Forms.TextBox();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.txtLine1 = new System.Windows.Forms.TextBox();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSetVatRates = new System.Windows.Forms.Button();
            this.btnBtnReloadVatRates = new System.Windows.Forms.Button();
            this.numericVatE = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericVatD = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericVatC = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericVatB = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericVatA = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGetVatRates = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnStartDay = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.RichTextBox();
            this.btnShowStatus = new System.Windows.Forms.Button();
            this.btnIssueZ = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.numBaudRate = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCharLines = new System.Windows.Forms.TextBox();
            this.lblCharLines = new System.Windows.Forms.Label();
            this.CharsCommand = new System.Windows.Forms.Label();
            this.txtCommandChars = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabHeader.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatA)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBaudRate)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(48, 47);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(88, 21);
            this.cmbPort.TabIndex = 0;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(13, 50);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 1;
            this.lblPort.Text = "Port:";
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Location = new System.Drawing.Point(142, 51);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(61, 13);
            this.lblBaudRate.TabIndex = 3;
            this.lblBaudRate.Text = "Baud Rate:";
            // 
            // cmbPrinter
            // 
            this.cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrinter.FormattingEnabled = true;
            this.cmbPrinter.Location = new System.Drawing.Point(48, 11);
            this.cmbPrinter.Name = "cmbPrinter";
            this.cmbPrinter.Size = new System.Drawing.Size(313, 21);
            this.cmbPrinter.TabIndex = 4;
            // 
            // lblPrinter
            // 
            this.lblPrinter.AutoSize = true;
            this.lblPrinter.Location = new System.Drawing.Point(1, 14);
            this.lblPrinter.Name = "lblPrinter";
            this.lblPrinter.Size = new System.Drawing.Size(40, 13);
            this.lblPrinter.TabIndex = 5;
            this.lblPrinter.Text = "Printer:";
            // 
            // lblParity
            // 
            this.lblParity.AutoSize = true;
            this.lblParity.Location = new System.Drawing.Point(334, 52);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(36, 13);
            this.lblParity.TabIndex = 6;
            this.lblParity.Text = "Parity:";
            // 
            // cmbParity
            // 
            this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Location = new System.Drawing.Point(376, 49);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(104, 21);
            this.cmbParity.TabIndex = 7;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(517, 6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(143, 40);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(45, 410);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(134, 23);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Not Connected";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabHeader);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(4, 86);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(656, 321);
            this.tabControl1.TabIndex = 12;
            // 
            // tabHeader
            // 
            this.tabHeader.Controls.Add(this.cmbPrintType8);
            this.tabHeader.Controls.Add(this.txtLine8);
            this.tabHeader.Controls.Add(this.lblLine8);
            this.tabHeader.Controls.Add(this.cmbPrintType7);
            this.tabHeader.Controls.Add(this.txtLine7);
            this.tabHeader.Controls.Add(this.lblLine7);
            this.tabHeader.Controls.Add(this.cmbPrintType6);
            this.tabHeader.Controls.Add(this.cmbPrintType5);
            this.tabHeader.Controls.Add(this.cmbPrintType4);
            this.tabHeader.Controls.Add(this.cmbPrintType3);
            this.tabHeader.Controls.Add(this.cmbPrintType2);
            this.tabHeader.Controls.Add(this.cmbPrintType1);
            this.tabHeader.Controls.Add(this.btnSetHeader);
            this.tabHeader.Controls.Add(this.txtLine6);
            this.tabHeader.Controls.Add(this.lblLine6);
            this.tabHeader.Controls.Add(this.txtLine5);
            this.tabHeader.Controls.Add(this.lblLine5);
            this.tabHeader.Controls.Add(this.txtLine4);
            this.tabHeader.Controls.Add(this.lblLine4);
            this.tabHeader.Controls.Add(this.txtLine3);
            this.tabHeader.Controls.Add(this.lblLine3);
            this.tabHeader.Controls.Add(this.txtLine2);
            this.tabHeader.Controls.Add(this.lblLine2);
            this.tabHeader.Controls.Add(this.txtLine1);
            this.tabHeader.Controls.Add(this.lblLine1);
            this.tabHeader.Location = new System.Drawing.Point(4, 22);
            this.tabHeader.Name = "tabHeader";
            this.tabHeader.Padding = new System.Windows.Forms.Padding(3);
            this.tabHeader.Size = new System.Drawing.Size(648, 295);
            this.tabHeader.TabIndex = 0;
            this.tabHeader.Text = "Set Header";
            this.tabHeader.UseVisualStyleBackColor = true;
            // 
            // cmbPrintType8
            // 
            this.cmbPrintType8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType8.FormattingEnabled = true;
            this.cmbPrintType8.Location = new System.Drawing.Point(537, 190);
            this.cmbPrintType8.Name = "cmbPrintType8";
            this.cmbPrintType8.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType8.TabIndex = 56;
            // 
            // txtLine8
            // 
            this.txtLine8.Location = new System.Drawing.Point(51, 191);
            this.txtLine8.Name = "txtLine8";
            this.txtLine8.Size = new System.Drawing.Size(480, 20);
            this.txtLine8.TabIndex = 55;
            // 
            // lblLine8
            // 
            this.lblLine8.AutoSize = true;
            this.lblLine8.Location = new System.Drawing.Point(6, 194);
            this.lblLine8.Name = "lblLine8";
            this.lblLine8.Size = new System.Drawing.Size(39, 13);
            this.lblLine8.TabIndex = 54;
            this.lblLine8.Text = "Line 8:";
            // 
            // cmbPrintType7
            // 
            this.cmbPrintType7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType7.FormattingEnabled = true;
            this.cmbPrintType7.Location = new System.Drawing.Point(537, 164);
            this.cmbPrintType7.Name = "cmbPrintType7";
            this.cmbPrintType7.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType7.TabIndex = 53;
            // 
            // txtLine7
            // 
            this.txtLine7.Location = new System.Drawing.Point(51, 165);
            this.txtLine7.Name = "txtLine7";
            this.txtLine7.Size = new System.Drawing.Size(480, 20);
            this.txtLine7.TabIndex = 52;
            // 
            // lblLine7
            // 
            this.lblLine7.AutoSize = true;
            this.lblLine7.Location = new System.Drawing.Point(6, 168);
            this.lblLine7.Name = "lblLine7";
            this.lblLine7.Size = new System.Drawing.Size(39, 13);
            this.lblLine7.TabIndex = 51;
            this.lblLine7.Text = "Line 7:";
            // 
            // cmbPrintType6
            // 
            this.cmbPrintType6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType6.FormattingEnabled = true;
            this.cmbPrintType6.Location = new System.Drawing.Point(537, 138);
            this.cmbPrintType6.Name = "cmbPrintType6";
            this.cmbPrintType6.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType6.TabIndex = 50;
            // 
            // cmbPrintType5
            // 
            this.cmbPrintType5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType5.FormattingEnabled = true;
            this.cmbPrintType5.Location = new System.Drawing.Point(537, 113);
            this.cmbPrintType5.Name = "cmbPrintType5";
            this.cmbPrintType5.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType5.TabIndex = 49;
            // 
            // cmbPrintType4
            // 
            this.cmbPrintType4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType4.FormattingEnabled = true;
            this.cmbPrintType4.Location = new System.Drawing.Point(537, 87);
            this.cmbPrintType4.Name = "cmbPrintType4";
            this.cmbPrintType4.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType4.TabIndex = 48;
            // 
            // cmbPrintType3
            // 
            this.cmbPrintType3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType3.FormattingEnabled = true;
            this.cmbPrintType3.Location = new System.Drawing.Point(537, 61);
            this.cmbPrintType3.Name = "cmbPrintType3";
            this.cmbPrintType3.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType3.TabIndex = 47;
            // 
            // cmbPrintType2
            // 
            this.cmbPrintType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType2.FormattingEnabled = true;
            this.cmbPrintType2.Location = new System.Drawing.Point(537, 34);
            this.cmbPrintType2.Name = "cmbPrintType2";
            this.cmbPrintType2.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType2.TabIndex = 46;
            // 
            // cmbPrintType1
            // 
            this.cmbPrintType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintType1.FormattingEnabled = true;
            this.cmbPrintType1.Location = new System.Drawing.Point(537, 9);
            this.cmbPrintType1.Name = "cmbPrintType1";
            this.cmbPrintType1.Size = new System.Drawing.Size(104, 21);
            this.cmbPrintType1.TabIndex = 45;
            this.cmbPrintType1.SelectedIndexChanged += new System.EventHandler(this.cmbPrintType1_SelectedIndexChanged);
            // 
            // btnSetHeader
            // 
            this.btnSetHeader.Location = new System.Drawing.Point(498, 249);
            this.btnSetHeader.Name = "btnSetHeader";
            this.btnSetHeader.Size = new System.Drawing.Size(143, 40);
            this.btnSetHeader.TabIndex = 44;
            this.btnSetHeader.Text = "Set Header";
            this.btnSetHeader.UseVisualStyleBackColor = true;
            this.btnSetHeader.Click += new System.EventHandler(this.btnSetHeader_Click);
            // 
            // txtLine6
            // 
            this.txtLine6.Location = new System.Drawing.Point(51, 139);
            this.txtLine6.Name = "txtLine6";
            this.txtLine6.Size = new System.Drawing.Size(480, 20);
            this.txtLine6.TabIndex = 39;
            // 
            // lblLine6
            // 
            this.lblLine6.AutoSize = true;
            this.lblLine6.Location = new System.Drawing.Point(6, 142);
            this.lblLine6.Name = "lblLine6";
            this.lblLine6.Size = new System.Drawing.Size(39, 13);
            this.lblLine6.TabIndex = 38;
            this.lblLine6.Text = "Line 6:";
            // 
            // txtLine5
            // 
            this.txtLine5.Location = new System.Drawing.Point(51, 113);
            this.txtLine5.Name = "txtLine5";
            this.txtLine5.Size = new System.Drawing.Size(480, 20);
            this.txtLine5.TabIndex = 33;
            // 
            // lblLine5
            // 
            this.lblLine5.AutoSize = true;
            this.lblLine5.Location = new System.Drawing.Point(6, 116);
            this.lblLine5.Name = "lblLine5";
            this.lblLine5.Size = new System.Drawing.Size(39, 13);
            this.lblLine5.TabIndex = 32;
            this.lblLine5.Text = "Line 5:";
            // 
            // txtLine4
            // 
            this.txtLine4.Location = new System.Drawing.Point(51, 87);
            this.txtLine4.Name = "txtLine4";
            this.txtLine4.Size = new System.Drawing.Size(480, 20);
            this.txtLine4.TabIndex = 27;
            // 
            // lblLine4
            // 
            this.lblLine4.AutoSize = true;
            this.lblLine4.Location = new System.Drawing.Point(6, 90);
            this.lblLine4.Name = "lblLine4";
            this.lblLine4.Size = new System.Drawing.Size(39, 13);
            this.lblLine4.TabIndex = 26;
            this.lblLine4.Text = "Line 4:";
            // 
            // txtLine3
            // 
            this.txtLine3.Location = new System.Drawing.Point(51, 61);
            this.txtLine3.Name = "txtLine3";
            this.txtLine3.Size = new System.Drawing.Size(480, 20);
            this.txtLine3.TabIndex = 21;
            // 
            // lblLine3
            // 
            this.lblLine3.AutoSize = true;
            this.lblLine3.Location = new System.Drawing.Point(6, 64);
            this.lblLine3.Name = "lblLine3";
            this.lblLine3.Size = new System.Drawing.Size(39, 13);
            this.lblLine3.TabIndex = 20;
            this.lblLine3.Text = "Line 3:";
            // 
            // txtLine2
            // 
            this.txtLine2.Location = new System.Drawing.Point(51, 35);
            this.txtLine2.Name = "txtLine2";
            this.txtLine2.Size = new System.Drawing.Size(480, 20);
            this.txtLine2.TabIndex = 15;
            // 
            // lblLine2
            // 
            this.lblLine2.AutoSize = true;
            this.lblLine2.Location = new System.Drawing.Point(6, 38);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(39, 13);
            this.lblLine2.TabIndex = 14;
            this.lblLine2.Text = "Line 2:";
            // 
            // txtLine1
            // 
            this.txtLine1.Location = new System.Drawing.Point(51, 9);
            this.txtLine1.Name = "txtLine1";
            this.txtLine1.Size = new System.Drawing.Size(480, 20);
            this.txtLine1.TabIndex = 9;
            // 
            // lblLine1
            // 
            this.lblLine1.AutoSize = true;
            this.lblLine1.Location = new System.Drawing.Point(6, 12);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(39, 13);
            this.lblLine1.TabIndex = 7;
            this.lblLine1.Text = "Line 1:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnSetVatRates);
            this.tabPage2.Controls.Add(this.btnBtnReloadVatRates);
            this.tabPage2.Controls.Add(this.numericVatE);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.numericVatD);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.numericVatC);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.numericVatB);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.numericVatA);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(648, 295);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Set Vat Rates";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSetVatRates
            // 
            this.btnSetVatRates.Location = new System.Drawing.Point(385, 252);
            this.btnSetVatRates.Name = "btnSetVatRates";
            this.btnSetVatRates.Size = new System.Drawing.Size(131, 37);
            this.btnSetVatRates.TabIndex = 25;
            this.btnSetVatRates.Text = "Set Vat Rates";
            this.btnSetVatRates.UseVisualStyleBackColor = true;
            this.btnSetVatRates.Click += new System.EventHandler(this.btnSetVatRates_Click);
            // 
            // btnBtnReloadVatRates
            // 
            this.btnBtnReloadVatRates.Location = new System.Drawing.Point(75, 5);
            this.btnBtnReloadVatRates.Name = "btnBtnReloadVatRates";
            this.btnBtnReloadVatRates.Size = new System.Drawing.Size(131, 37);
            this.btnBtnReloadVatRates.TabIndex = 24;
            this.btnBtnReloadVatRates.Text = "Reload Vat Rates";
            this.btnBtnReloadVatRates.UseVisualStyleBackColor = true;
            this.btnBtnReloadVatRates.Click += new System.EventHandler(this.btnBtnReloadVatRates_Click);
            // 
            // numericVatE
            // 
            this.numericVatE.DecimalPlaces = 4;
            this.numericVatE.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericVatE.Location = new System.Drawing.Point(75, 152);
            this.numericVatE.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericVatE.Name = "numericVatE";
            this.numericVatE.Size = new System.Drawing.Size(131, 20);
            this.numericVatE.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Vat E";
            // 
            // numericVatD
            // 
            this.numericVatD.DecimalPlaces = 4;
            this.numericVatD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericVatD.Location = new System.Drawing.Point(75, 126);
            this.numericVatD.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericVatD.Name = "numericVatD";
            this.numericVatD.Size = new System.Drawing.Size(131, 20);
            this.numericVatD.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Vat D";
            // 
            // numericVatC
            // 
            this.numericVatC.DecimalPlaces = 4;
            this.numericVatC.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericVatC.InterceptArrowKeys = false;
            this.numericVatC.Location = new System.Drawing.Point(75, 100);
            this.numericVatC.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericVatC.Name = "numericVatC";
            this.numericVatC.Size = new System.Drawing.Size(131, 20);
            this.numericVatC.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Vat C";
            // 
            // numericVatB
            // 
            this.numericVatB.DecimalPlaces = 4;
            this.numericVatB.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericVatB.InterceptArrowKeys = false;
            this.numericVatB.Location = new System.Drawing.Point(75, 74);
            this.numericVatB.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericVatB.Name = "numericVatB";
            this.numericVatB.Size = new System.Drawing.Size(131, 20);
            this.numericVatB.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Vat B";
            // 
            // numericVatA
            // 
            this.numericVatA.DecimalPlaces = 4;
            this.numericVatA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericVatA.InterceptArrowKeys = false;
            this.numericVatA.Location = new System.Drawing.Point(75, 48);
            this.numericVatA.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericVatA.Name = "numericVatA";
            this.numericVatA.Size = new System.Drawing.Size(131, 20);
            this.numericVatA.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Vat A";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnGetVatRates);
            this.tabPage1.Controls.Add(this.btnClearLog);
            this.tabPage1.Controls.Add(this.btnStartDay);
            this.tabPage1.Controls.Add(this.tbLog);
            this.tabPage1.Controls.Add(this.btnShowStatus);
            this.tabPage1.Controls.Add(this.btnIssueZ);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(648, 295);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Service";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnGetVatRates
            // 
            this.btnGetVatRates.Location = new System.Drawing.Point(80, 6);
            this.btnGetVatRates.Name = "btnGetVatRates";
            this.btnGetVatRates.Size = new System.Drawing.Size(66, 40);
            this.btnGetVatRates.TabIndex = 15;
            this.btnGetVatRates.Text = "Get Vat Rates";
            this.btnGetVatRates.UseVisualStyleBackColor = true;
            this.btnGetVatRates.Click += new System.EventHandler(this.btnGetVatRates_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(152, 6);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(60, 40);
            this.btnClearLog.TabIndex = 14;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnStartDay
            // 
            this.btnStartDay.Location = new System.Drawing.Point(400, 6);
            this.btnStartDay.Name = "btnStartDay";
            this.btnStartDay.Size = new System.Drawing.Size(60, 40);
            this.btnStartDay.TabIndex = 13;
            this.btnStartDay.Text = "Start Day";
            this.btnStartDay.UseVisualStyleBackColor = true;
            this.btnStartDay.Click += new System.EventHandler(this.btnStartDay_Click);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(8, 52);
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(508, 237);
            this.tbLog.TabIndex = 12;
            this.tbLog.Text = "";
            // 
            // btnShowStatus
            // 
            this.btnShowStatus.Location = new System.Drawing.Point(8, 6);
            this.btnShowStatus.Name = "btnShowStatus";
            this.btnShowStatus.Size = new System.Drawing.Size(66, 40);
            this.btnShowStatus.TabIndex = 11;
            this.btnShowStatus.Text = "Get Status Flags";
            this.btnShowStatus.UseVisualStyleBackColor = true;
            this.btnShowStatus.Click += new System.EventHandler(this.btnShowStatus_Click);
            // 
            // btnIssueZ
            // 
            this.btnIssueZ.Location = new System.Drawing.Point(466, 6);
            this.btnIssueZ.Name = "btnIssueZ";
            this.btnIssueZ.Size = new System.Drawing.Size(54, 40);
            this.btnIssueZ.TabIndex = 10;
            this.btnIssueZ.Text = "Issue Z";
            this.btnIssueZ.UseVisualStyleBackColor = true;
            this.btnIssueZ.Click += new System.EventHandler(this.btnIssueZ_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(673, 437);
            this.shapeContainer1.TabIndex = 13;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape1
            // 
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 4;
            this.lineShape1.X2 = 528;
            this.lineShape1.Y1 = 77;
            this.lineShape1.Y2 = 77;
            // 
            // numBaudRate
            // 
            this.numBaudRate.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numBaudRate.Location = new System.Drawing.Point(209, 49);
            this.numBaudRate.Maximum = new decimal(new int[] {
            230400,
            0,
            0,
            0});
            this.numBaudRate.Name = "numBaudRate";
            this.numBaudRate.Size = new System.Drawing.Size(120, 20);
            this.numBaudRate.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 415);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Status:";
            // 
            // txtCharLines
            // 
            this.txtCharLines.Location = new System.Drawing.Point(601, 50);
            this.txtCharLines.Name = "txtCharLines";
            this.txtCharLines.Size = new System.Drawing.Size(59, 20);
            this.txtCharLines.TabIndex = 16;
            // 
            // lblCharLines
            // 
            this.lblCharLines.AutoSize = true;
            this.lblCharLines.Location = new System.Drawing.Point(499, 54);
            this.lblCharLines.Name = "lblCharLines";
            this.lblCharLines.Size = new System.Drawing.Size(78, 13);
            this.lblCharLines.TabIndex = 17;
            this.lblCharLines.Text = "Chars per Line:";
            this.lblCharLines.Click += new System.EventHandler(this.label7_Click);
            // 
            // CharsCommand
            // 
            this.CharsCommand.AutoSize = true;
            this.CharsCommand.Location = new System.Drawing.Point(499, 80);
            this.CharsCommand.Name = "CharsCommand";
            this.CharsCommand.Size = new System.Drawing.Size(98, 13);
            this.CharsCommand.TabIndex = 19;
            this.CharsCommand.Text = "Chars in Command:";
            // 
            // txtCommandChars
            // 
            this.txtCommandChars.Location = new System.Drawing.Point(601, 76);
            this.txtCommandChars.Name = "txtCommandChars";
            this.txtCommandChars.Size = new System.Drawing.Size(59, 20);
            this.txtCommandChars.TabIndex = 18;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 437);
            this.Controls.Add(this.CharsCommand);
            this.Controls.Add(this.txtCommandChars);
            this.Controls.Add(this.lblCharLines);
            this.Controls.Add(this.txtCharLines);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.numBaudRate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbParity);
            this.Controls.Add(this.lblParity);
            this.Controls.Add(this.lblPrinter);
            this.Controls.Add(this.cmbPrinter);
            this.Controls.Add(this.lblBaudRate);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.cmbPort);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ITS Fiscal Printer Configurator";
            this.tabControl1.ResumeLayout(false);
            this.tabHeader.ResumeLayout(false);
            this.tabHeader.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVatA)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numBaudRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cmbPrinter;
        private System.Windows.Forms.Label lblPrinter;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabHeader;
        private System.Windows.Forms.TextBox txtLine6;
        private System.Windows.Forms.Label lblLine6;
        private System.Windows.Forms.TextBox txtLine5;
        private System.Windows.Forms.Label lblLine5;
        private System.Windows.Forms.TextBox txtLine4;
        private System.Windows.Forms.Label lblLine4;
        private System.Windows.Forms.TextBox txtLine3;
        private System.Windows.Forms.Label lblLine3;
        private System.Windows.Forms.TextBox txtLine2;
        private System.Windows.Forms.Label lblLine2;
        private System.Windows.Forms.TextBox txtLine1;
        private System.Windows.Forms.Label lblLine1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.Button btnSetHeader;
        private System.Windows.Forms.NumericUpDown numBaudRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbPrintType1;
        private System.Windows.Forms.ComboBox cmbPrintType6;
        private System.Windows.Forms.ComboBox cmbPrintType5;
        private System.Windows.Forms.ComboBox cmbPrintType4;
        private System.Windows.Forms.ComboBox cmbPrintType3;
        private System.Windows.Forms.ComboBox cmbPrintType2;
        private System.Windows.Forms.ComboBox cmbPrintType8;
        private System.Windows.Forms.TextBox txtLine8;
        private System.Windows.Forms.Label lblLine8;
        private System.Windows.Forms.ComboBox cmbPrintType7;
        private System.Windows.Forms.TextBox txtLine7;
        private System.Windows.Forms.Label lblLine7;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnShowStatus;
        private System.Windows.Forms.Button btnIssueZ;
        public System.Windows.Forms.RichTextBox tbLog;
        private System.Windows.Forms.Button btnStartDay;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnSetVatRates;
        private System.Windows.Forms.Button btnBtnReloadVatRates;
        private System.Windows.Forms.NumericUpDown numericVatE;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericVatD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericVatC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericVatB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericVatA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetVatRates;
        private System.Windows.Forms.TextBox txtCharLines;
        private System.Windows.Forms.Label lblCharLines;
        private System.Windows.Forms.Label CharsCommand;
        private System.Windows.Forms.TextBox txtCommandChars;
    }
}

