namespace ITS.MobileAtStore
{
    partial class BluetoothPrinterSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BluetoothPrinterSettingsForm));
            this.labelBluetoothDevice = new System.Windows.Forms.Label();
            this.labelEncoding = new System.Windows.Forms.Label();
            this.buttonΟΚ = new OpenNETCF.Windows.Forms.Button2();
            this.comboBoxBluetoothDevice = new OpenNETCF.Windows.Forms.ComboBox2(this.components);
            this.comboBoxEncoding = new OpenNETCF.Windows.Forms.ComboBox2(this.components);
            this.SuspendLayout();
            // 
            // labelBluetoothDevice
            // 
            this.labelBluetoothDevice.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.labelBluetoothDevice.Location = new System.Drawing.Point(4, 3);
            this.labelBluetoothDevice.Name = "labelBluetoothDevice";
            this.labelBluetoothDevice.Size = new System.Drawing.Size(231, 24);
            this.labelBluetoothDevice.Text = "Διαθέσιμες Bluetooth Συσκευές";
            // 
            // labelEncoding
            // 
            this.labelEncoding.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.labelEncoding.Location = new System.Drawing.Point(4, 122);
            this.labelEncoding.Name = "labelEncoding";
            this.labelEncoding.Size = new System.Drawing.Size(231, 24);
            this.labelEncoding.Text = "Κωδικοποίηση";
            // 
            // buttonΟΚ
            // 
            this.buttonΟΚ.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonΟΚ.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.buttonΟΚ.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonΟΚ.Image = ((System.Drawing.Image)(resources.GetObject("buttonΟΚ.Image")));
            this.buttonΟΚ.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.buttonΟΚ.ImageIndex = -1;
            this.buttonΟΚ.ImageList = null;
            this.buttonΟΚ.Location = new System.Drawing.Point(4, 234);
            this.buttonΟΚ.Name = "buttonΟΚ";
            this.buttonΟΚ.Size = new System.Drawing.Size(231, 32);
            this.buttonΟΚ.TabIndex = 6;
            this.buttonΟΚ.Text = "OK";
            this.buttonΟΚ.Click += new System.EventHandler(this.buttonΟΚ_Click);
            // 
            // comboBoxBluetoothDevice
            // 
            this.comboBoxBluetoothDevice.DisplayMember = "Name";
            this.comboBoxBluetoothDevice.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.comboBoxBluetoothDevice.Items.Add("Batch");
            this.comboBoxBluetoothDevice.Items.Add("Online");
            this.comboBoxBluetoothDevice.Location = new System.Drawing.Point(3, 30);
            this.comboBoxBluetoothDevice.Name = "comboBoxBluetoothDevice";
            this.comboBoxBluetoothDevice.Size = new System.Drawing.Size(231, 24);
            this.comboBoxBluetoothDevice.TabIndex = 7;
            this.comboBoxBluetoothDevice.ValueMember = "Address";
            // 
            // comboBoxEncoding
            // 
            this.comboBoxEncoding.DisplayMember = "WebName";
            this.comboBoxEncoding.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.comboBoxEncoding.Items.Add("Batch");
            this.comboBoxEncoding.Items.Add("Online");
            this.comboBoxEncoding.Location = new System.Drawing.Point(3, 149);
            this.comboBoxEncoding.Name = "comboBoxEncoding";
            this.comboBoxEncoding.Size = new System.Drawing.Size(231, 24);
            this.comboBoxEncoding.TabIndex = 8;
            this.comboBoxEncoding.ValueMember = "CodePage";
            // 
            // BluetoothPrinterSettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(238, 269);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxEncoding);
            this.Controls.Add(this.comboBoxBluetoothDevice);
            this.Controls.Add(this.buttonΟΚ);
            this.Controls.Add(this.labelEncoding);
            this.Controls.Add(this.labelBluetoothDevice);
            this.Name = "BluetoothPrinterSettingsForm";
            this.Text = "Bluetooth Εκτυπωτής";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelBluetoothDevice;
        private System.Windows.Forms.Label labelEncoding;
        private OpenNETCF.Windows.Forms.Button2 buttonΟΚ;
        private OpenNETCF.Windows.Forms.ComboBox2 comboBoxBluetoothDevice;
        private OpenNETCF.Windows.Forms.ComboBox2 comboBoxEncoding;
    }
}