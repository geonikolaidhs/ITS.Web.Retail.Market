namespace ITS.MobileAtStore
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lblWebServiceURL = new System.Windows.Forms.Label();
            this.listItem1 = new OpenNETCF.Windows.Forms.ListItem();
            this.txtWebServiceURL = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblOperationMode = new System.Windows.Forms.Label();
            this.button21 = new OpenNETCF.Windows.Forms.Button2();
            this.cmbOperationMode = new OpenNETCF.Windows.Forms.ComboBox2(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtTerminalID = new OpenNETCF.Windows.Forms.TextBox2();
            this.buttonPairBluetoothPrinter = new OpenNETCF.Windows.Forms.Button2();
            this.btnScanNPair = new OpenNETCF.Windows.Forms.Button2();
            this.SuspendLayout();
            // 
            // lblWebServiceURL
            // 
            this.lblWebServiceURL.BackColor = System.Drawing.Color.Gainsboro;
            this.lblWebServiceURL.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblWebServiceURL.ForeColor = System.Drawing.Color.Black;
            this.lblWebServiceURL.Location = new System.Drawing.Point(3, 57);
            this.lblWebServiceURL.Name = "lblWebServiceURL";
            this.lblWebServiceURL.Size = new System.Drawing.Size(231, 24);
            this.lblWebServiceURL.Text = "Server IP";
            // 
            // listItem1
            // 
            this.listItem1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.listItem1.ForeColor = System.Drawing.Color.Black;
            this.listItem1.ImageIndex = -1;
            this.listItem1.Text = "1";
            // 
            // txtWebServiceURL
            // 
            this.txtWebServiceURL.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtWebServiceURL.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtWebServiceURL.Location = new System.Drawing.Point(3, 84);
            this.txtWebServiceURL.MaxLength = 100;
            this.txtWebServiceURL.Name = "txtWebServiceURL";
            this.txtWebServiceURL.Size = new System.Drawing.Size(231, 24);
            this.txtWebServiceURL.TabIndex = 2;
            // 
            // lblOperationMode
            // 
            this.lblOperationMode.BackColor = System.Drawing.Color.Gainsboro;
            this.lblOperationMode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblOperationMode.ForeColor = System.Drawing.Color.Black;
            this.lblOperationMode.Location = new System.Drawing.Point(4, 3);
            this.lblOperationMode.Name = "lblOperationMode";
            this.lblOperationMode.Size = new System.Drawing.Size(231, 24);
            this.lblOperationMode.Text = "Operation Mode";
            // 
            // button21
            // 
            this.button21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.button21.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.button21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button21.Image = ((System.Drawing.Image)(resources.GetObject("button21.Image")));
            this.button21.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.button21.ImageIndex = -1;
            this.button21.ImageList = null;
            this.button21.Location = new System.Drawing.Point(4, 234);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(231, 32);
            this.button21.TabIndex = 3;
            this.button21.Text = "OK";
            this.button21.Click += new System.EventHandler(this.button21_Click);
            // 
            // cmbOperationMode
            // 
            this.cmbOperationMode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.cmbOperationMode.Items.Add("Batch");
            this.cmbOperationMode.Items.Add("Online");
            this.cmbOperationMode.Location = new System.Drawing.Point(4, 30);
            this.cmbOperationMode.Name = "cmbOperationMode";
            this.cmbOperationMode.Size = new System.Drawing.Size(231, 24);
            this.cmbOperationMode.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 24);
            this.label1.Text = "Terminal ID";
            // 
            // txtTerminalID
            // 
            this.txtTerminalID.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtTerminalID.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtTerminalID.Location = new System.Drawing.Point(3, 135);
            this.txtTerminalID.MaxLength = 100;
            this.txtTerminalID.Name = "txtTerminalID";
            this.txtTerminalID.Size = new System.Drawing.Size(231, 24);
            this.txtTerminalID.TabIndex = 2;
            // 
            // buttonPairBluetoothPrinter
            // 
            this.buttonPairBluetoothPrinter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonPairBluetoothPrinter.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.buttonPairBluetoothPrinter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPairBluetoothPrinter.Image = ((System.Drawing.Image)(resources.GetObject("buttonPairBluetoothPrinter.Image")));
            this.buttonPairBluetoothPrinter.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.buttonPairBluetoothPrinter.ImageIndex = -1;
            this.buttonPairBluetoothPrinter.ImageList = null;
            this.buttonPairBluetoothPrinter.Location = new System.Drawing.Point(4, 163);
            this.buttonPairBluetoothPrinter.Name = "buttonPairBluetoothPrinter";
            this.buttonPairBluetoothPrinter.Size = new System.Drawing.Size(231, 32);
            this.buttonPairBluetoothPrinter.TabIndex = 7;
            this.buttonPairBluetoothPrinter.Text = "Αναζ. Bluetooth Εκτυπ.";
            this.buttonPairBluetoothPrinter.Click += new System.EventHandler(this.buttonPairBluetoothPrinter_Click);
            // 
            // btnScanNPair
            // 
            this.btnScanNPair.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnScanNPair.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnScanNPair.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnScanNPair.Image = ((System.Drawing.Image)(resources.GetObject("btnScanNPair.Image")));
            this.btnScanNPair.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnScanNPair.ImageIndex = -1;
            this.btnScanNPair.ImageList = null;
            this.btnScanNPair.Location = new System.Drawing.Point(4, 196);
            this.btnScanNPair.Name = "btnScanNPair";
            this.btnScanNPair.Size = new System.Drawing.Size(231, 32);
            this.btnScanNPair.TabIndex = 7;
            this.btnScanNPair.Text = "Scan & Pair Bluetooth";
            this.btnScanNPair.Click += new System.EventHandler(this.btnScanNPair_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(238, 269);
            this.Controls.Add(this.btnScanNPair);
            this.Controls.Add(this.buttonPairBluetoothPrinter);
            this.Controls.Add(this.button21);
            this.Controls.Add(this.txtTerminalID);
            this.Controls.Add(this.txtWebServiceURL);
            this.Controls.Add(this.cmbOperationMode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblOperationMode);
            this.Controls.Add(this.lblWebServiceURL);
            this.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Ρυθμίσεις";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SettingsForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWebServiceURL;
        private OpenNETCF.Windows.Forms.ListItem listItem1;
        private OpenNETCF.Windows.Forms.TextBox2 txtWebServiceURL;
        private System.Windows.Forms.Label lblOperationMode;
        private OpenNETCF.Windows.Forms.Button2 button21;
        private OpenNETCF.Windows.Forms.ComboBox2 cmbOperationMode;
        private System.Windows.Forms.Label label1;
        private OpenNETCF.Windows.Forms.TextBox2 txtTerminalID;
        private OpenNETCF.Windows.Forms.Button2 buttonPairBluetoothPrinter;
        private OpenNETCF.Windows.Forms.Button2 btnScanNPair;
    }
}