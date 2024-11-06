namespace ITS.Licensing.ClientLibrary
{
    partial class ActivationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivationForm));
            this.txtApplicationName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSerialNumber = new System.Windows.Forms.TextBox();
            this.lblActivation = new System.Windows.Forms.Label();
            this.txtActivationKey = new System.Windows.Forms.TextBox();
            this.lblMachineID = new System.Windows.Forms.Label();
            this.txtMachineID = new System.Windows.Forms.TextBox();
            this.btnOnlineActivation = new System.Windows.Forms.Button();
            this.btnOfflineActivation = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtApplicationName
            // 
            this.txtApplicationName.Location = new System.Drawing.Point(12, 25);
            this.txtApplicationName.Name = "txtApplicationName";
            this.txtApplicationName.ReadOnly = true;
            this.txtApplicationName.Size = new System.Drawing.Size(225, 20);
            this.txtApplicationName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ονομασία Προϊόντος";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(252, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Σειριακός Αριθμός";
            // 
            // txtSerialNumber
            // 
            this.txtSerialNumber.Location = new System.Drawing.Point(255, 25);
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.ReadOnly = true;
            this.txtSerialNumber.Size = new System.Drawing.Size(225, 20);
            this.txtSerialNumber.TabIndex = 3;
            // 
            // lblActivation
            // 
            this.lblActivation.AutoSize = true;
            this.lblActivation.Location = new System.Drawing.Point(252, 48);
            this.lblActivation.Name = "lblActivation";
            this.lblActivation.Size = new System.Drawing.Size(115, 13);
            this.lblActivation.TabIndex = 8;
            this.lblActivation.Text = "Κλειδί Ενεργοποίησης";
            this.lblActivation.Visible = false;
            // 
            // txtActivationKey
            // 
            this.txtActivationKey.Location = new System.Drawing.Point(255, 64);
            this.txtActivationKey.Name = "txtActivationKey";
            this.txtActivationKey.ReadOnly = true;
            this.txtActivationKey.Size = new System.Drawing.Size(225, 20);
            this.txtActivationKey.TabIndex = 7;
            this.txtActivationKey.Visible = false;
            // 
            // lblMachineID
            // 
            this.lblMachineID.AutoSize = true;
            this.lblMachineID.Location = new System.Drawing.Point(12, 48);
            this.lblMachineID.Name = "lblMachineID";
            this.lblMachineID.Size = new System.Drawing.Size(88, 13);
            this.lblMachineID.TabIndex = 6;
            this.lblMachineID.Text = "Κλειδί Συσκευής";
            this.lblMachineID.Visible = false;
            // 
            // txtMachineID
            // 
            this.txtMachineID.Location = new System.Drawing.Point(12, 64);
            this.txtMachineID.Name = "txtMachineID";
            this.txtMachineID.ReadOnly = true;
            this.txtMachineID.Size = new System.Drawing.Size(225, 20);
            this.txtMachineID.TabIndex = 5;
            this.txtMachineID.Visible = false;
            // 
            // btnOnlineActivation
            // 
            this.btnOnlineActivation.Location = new System.Drawing.Point(255, 126);
            this.btnOnlineActivation.Name = "btnOnlineActivation";
            this.btnOnlineActivation.Size = new System.Drawing.Size(225, 27);
            this.btnOnlineActivation.TabIndex = 9;
            this.btnOnlineActivation.Text = "Αυτόματη Ενεργοποίηση μέσω Internet";
            this.btnOnlineActivation.UseVisualStyleBackColor = true;
            this.btnOnlineActivation.Click += new System.EventHandler(this.btnOnlineActivation_Click);
            // 
            // btnOfflineActivation
            // 
            this.btnOfflineActivation.Location = new System.Drawing.Point(12, 126);
            this.btnOfflineActivation.Name = "btnOfflineActivation";
            this.btnOfflineActivation.Size = new System.Drawing.Size(225, 27);
            this.btnOfflineActivation.TabIndex = 10;
            this.btnOfflineActivation.Text = "Ενεργοποίηση μέσω τηλεφώνου";
            this.btnOfflineActivation.UseVisualStyleBackColor = true;
            this.btnOfflineActivation.Click += new System.EventHandler(this.btnOfflineActivation_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(255, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(225, 27);
            this.button1.TabIndex = 11;
            this.button1.Text = "Ενεργοποίηση";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Παρακαλώ επικοινωνήστε με την ITS ΑΕ";
            this.label1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 159);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(255, 100);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 271);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOfflineActivation);
            this.Controls.Add(this.btnOnlineActivation);
            this.Controls.Add(this.lblActivation);
            this.Controls.Add(this.txtActivationKey);
            this.Controls.Add(this.lblMachineID);
            this.Controls.Add(this.txtMachineID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSerialNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtApplicationName);
            this.Name = "ActivationForm";
            this.Text = "Ενεργοποίηση Προϊόντος";
            this.Load += new System.EventHandler(this.Activation_Load);
            this.Shown += new System.EventHandler(this.ActivationForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtApplicationName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSerialNumber;
        private System.Windows.Forms.Label lblActivation;
        private System.Windows.Forms.TextBox txtActivationKey;
        private System.Windows.Forms.Label lblMachineID;
        private System.Windows.Forms.TextBox txtMachineID;
        private System.Windows.Forms.Button btnOnlineActivation;
        private System.Windows.Forms.Button btnOfflineActivation;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}