namespace ITS.MobileAtStore
{
    partial class MasterReceiptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterReceiptForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.txtDocNumber = new System.Windows.Forms.TextBox();
            this.lblDocNumber = new System.Windows.Forms.Label();
            this.txtSupplier = new System.Windows.Forms.TextBox();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.btnContinue = new OpenNETCF.Windows.Forms.Button2();
            this.lblSupplierName = new System.Windows.Forms.Label();
            this.lblSupplierAFM = new System.Windows.Forms.Label();
            this.txtSupplierName = new System.Windows.Forms.TextBox();
            this.txtSupplierAFM = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtDocNumber
            // 
            this.txtDocNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDocNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtDocNumber.Location = new System.Drawing.Point(117, 33);
            this.txtDocNumber.Multiline = true;
            this.txtDocNumber.Name = "txtDocNumber";
            this.txtDocNumber.Size = new System.Drawing.Size(118, 24);
            this.txtDocNumber.TabIndex = 2;
            this.txtDocNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDocNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDocNumber_KeyPress);
            // 
            // lblDocNumber
            // 
            this.lblDocNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDocNumber.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDocNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblDocNumber.ForeColor = System.Drawing.Color.Black;
            this.lblDocNumber.Location = new System.Drawing.Point(4, 33);
            this.lblDocNumber.Name = "lblDocNumber";
            this.lblDocNumber.Size = new System.Drawing.Size(107, 24);
            this.lblDocNumber.Text = "Αρ. Παραλαβής";
            // 
            // txtSupplier
            // 
            this.txtSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSupplier.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtSupplier.Location = new System.Drawing.Point(117, 4);
            this.txtSupplier.Multiline = true;
            this.txtSupplier.Name = "txtSupplier";
            this.txtSupplier.Size = new System.Drawing.Size(118, 24);
            this.txtSupplier.TabIndex = 0;
            this.txtSupplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSupplier.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplier_KeyPress);
            // 
            // lblSupplier
            // 
            this.lblSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSupplier.BackColor = System.Drawing.Color.Gainsboro;
            this.lblSupplier.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblSupplier.ForeColor = System.Drawing.Color.Black;
            this.lblSupplier.Location = new System.Drawing.Point(4, 4);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(107, 24);
            this.lblSupplier.Text = "Προμηθευτής";
            // 
            // btnReturn
            // 
            this.btnReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReturn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReturn.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnReturn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReturn.Image = ((System.Drawing.Image)(resources.GetObject("btnReturn.Image")));
            this.btnReturn.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnReturn.ImageIndex = -1;
            this.btnReturn.ImageList = null;
            this.btnReturn.Location = new System.Drawing.Point(127, 233);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(104, 32);
            this.btnReturn.TabIndex = 13;
            this.btnReturn.TabStop = false;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnContinue.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnContinue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnContinue.Image = ((System.Drawing.Image)(resources.GetObject("btnContinue.Image")));
            this.btnContinue.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnContinue.ImageIndex = -1;
            this.btnContinue.ImageList = null;
            this.btnContinue.Location = new System.Drawing.Point(4, 233);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(112, 32);
            this.btnContinue.TabIndex = 14;
            this.btnContinue.TabStop = false;
            this.btnContinue.Text = "Συνέχεια";
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // lblSupplierName
            // 
            this.lblSupplierName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSupplierName.BackColor = System.Drawing.Color.Gainsboro;
            this.lblSupplierName.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblSupplierName.ForeColor = System.Drawing.Color.Black;
            this.lblSupplierName.Location = new System.Drawing.Point(4, 63);
            this.lblSupplierName.Name = "lblSupplierName";
            this.lblSupplierName.Size = new System.Drawing.Size(107, 24);
            this.lblSupplierName.Text = "Επων. Προμηθ.";
            // 
            // lblSupplierAFM
            // 
            this.lblSupplierAFM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSupplierAFM.BackColor = System.Drawing.Color.Gainsboro;
            this.lblSupplierAFM.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblSupplierAFM.ForeColor = System.Drawing.Color.Black;
            this.lblSupplierAFM.Location = new System.Drawing.Point(4, 93);
            this.lblSupplierAFM.Name = "lblSupplierAFM";
            this.lblSupplierAFM.Size = new System.Drawing.Size(107, 24);
            this.lblSupplierAFM.Text = "ΑΦΜ Προμηθ.";
            // 
            // txtSupplierName
            // 
            this.txtSupplierName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSupplierName.BackColor = System.Drawing.Color.LightBlue;
            this.txtSupplierName.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.txtSupplierName.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtSupplierName.Location = new System.Drawing.Point(117, 63);
            this.txtSupplierName.Multiline = true;
            this.txtSupplierName.Name = "txtSupplierName";
            this.txtSupplierName.Size = new System.Drawing.Size(118, 24);
            this.txtSupplierName.TabIndex = 22;
            this.txtSupplierName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSupplierName.GotFocus += new System.EventHandler(this.txtSupplierName_GotFocus);
            this.txtSupplierName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplierName_KeyPress);
            // 
            // txtSupplierAFM
            // 
            this.txtSupplierAFM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSupplierAFM.BackColor = System.Drawing.Color.LightBlue;
            this.txtSupplierAFM.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.txtSupplierAFM.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtSupplierAFM.Location = new System.Drawing.Point(117, 93);
            this.txtSupplierAFM.Multiline = true;
            this.txtSupplierAFM.Name = "txtSupplierAFM";
            this.txtSupplierAFM.Size = new System.Drawing.Size(118, 24);
            this.txtSupplierAFM.TabIndex = 23;
            this.txtSupplierAFM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSupplierAFM.GotFocus += new System.EventHandler(this.txtSupplierAFM_GotFocus);
            this.txtSupplierAFM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplierAFM_KeyPress);
            // 
            // MasterReceiptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.txtSupplierAFM);
            this.Controls.Add(this.txtSupplierName);
            this.Controls.Add(this.lblSupplierAFM);
            this.Controls.Add(this.lblSupplierName);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.txtDocNumber);
            this.Controls.Add(this.lblDocNumber);
            this.Controls.Add(this.txtSupplier);
            this.Controls.Add(this.lblSupplier);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MasterReceiptForm";
            this.Text = "Κύριες πληροφορίες παραλαβής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MasterReceiptForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtDocNumber;
        private System.Windows.Forms.Label lblDocNumber;
        private System.Windows.Forms.TextBox txtSupplier;
        private System.Windows.Forms.Label lblSupplier;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private OpenNETCF.Windows.Forms.Button2 btnContinue;
        private System.Windows.Forms.Label lblSupplierName;
        private System.Windows.Forms.Label lblSupplierAFM;
        private System.Windows.Forms.TextBox txtSupplierName;
        private System.Windows.Forms.TextBox txtSupplierAFM;
    }
}