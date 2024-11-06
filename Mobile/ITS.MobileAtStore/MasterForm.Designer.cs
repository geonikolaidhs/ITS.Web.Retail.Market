namespace ITS.MobileAtStore
{
    partial class MasterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterForm));
            this.txtTrader = new System.Windows.Forms.TextBox();
            this.lblTrader = new System.Windows.Forms.Label();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.btnContinue = new OpenNETCF.Windows.Forms.Button2();
            this.lblTraderName = new System.Windows.Forms.Label();
            this.lblTraderAFM = new System.Windows.Forms.Label();
            this.txtTraderName = new System.Windows.Forms.TextBox();
            this.txtTraderAFM = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtTrader
            // 
            this.txtTrader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTrader.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtTrader.Location = new System.Drawing.Point(117, 4);
            this.txtTrader.Multiline = true;
            this.txtTrader.Name = "txtTrader";
            this.txtTrader.Size = new System.Drawing.Size(118, 24);
            this.txtTrader.TabIndex = 0;
            this.txtTrader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTrader.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrader_KeyPress);
            // 
            // lblTrader
            // 
            this.lblTrader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrader.BackColor = System.Drawing.Color.Gainsboro;
            this.lblTrader.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblTrader.ForeColor = System.Drawing.Color.Black;
            this.lblTrader.Location = new System.Drawing.Point(4, 4);
            this.lblTrader.Name = "lblTrader";
            this.lblTrader.Size = new System.Drawing.Size(107, 24);
            this.lblTrader.Text = "Προμηθευτής";
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
            // lblTraderName
            // 
            this.lblTraderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTraderName.BackColor = System.Drawing.Color.Gainsboro;
            this.lblTraderName.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblTraderName.ForeColor = System.Drawing.Color.Black;
            this.lblTraderName.Location = new System.Drawing.Point(4, 34);
            this.lblTraderName.Name = "lblTraderName";
            this.lblTraderName.Size = new System.Drawing.Size(107, 24);
            this.lblTraderName.Text = "Επωνυμία";
            // 
            // lblTraderAFM
            // 
            this.lblTraderAFM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTraderAFM.BackColor = System.Drawing.Color.Gainsboro;
            this.lblTraderAFM.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblTraderAFM.ForeColor = System.Drawing.Color.Black;
            this.lblTraderAFM.Location = new System.Drawing.Point(3, 64);
            this.lblTraderAFM.Name = "lblTraderAFM";
            this.lblTraderAFM.Size = new System.Drawing.Size(107, 24);
            this.lblTraderAFM.Text = "Α.Φ.Μ.";
            // 
            // txtTraderName
            // 
            this.txtTraderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTraderName.BackColor = System.Drawing.Color.LightBlue;
            this.txtTraderName.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.txtTraderName.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtTraderName.Location = new System.Drawing.Point(117, 34);
            this.txtTraderName.Multiline = true;
            this.txtTraderName.Name = "txtTraderName";
            this.txtTraderName.Size = new System.Drawing.Size(118, 24);
            this.txtTraderName.TabIndex = 22;
            this.txtTraderName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTraderName.GotFocus += new System.EventHandler(this.txtSupplierName_GotFocus);
            this.txtTraderName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplierName_KeyPress);
            // 
            // txtTraderAFM
            // 
            this.txtTraderAFM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTraderAFM.BackColor = System.Drawing.Color.LightBlue;
            this.txtTraderAFM.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.txtTraderAFM.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtTraderAFM.Location = new System.Drawing.Point(117, 64);
            this.txtTraderAFM.Multiline = true;
            this.txtTraderAFM.Name = "txtTraderAFM";
            this.txtTraderAFM.Size = new System.Drawing.Size(118, 24);
            this.txtTraderAFM.TabIndex = 23;
            this.txtTraderAFM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTraderAFM.GotFocus += new System.EventHandler(this.txtSupplierAFM_GotFocus);
            this.txtTraderAFM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplierAFM_KeyPress);
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.txtTraderAFM);
            this.Controls.Add(this.txtTraderName);
            this.Controls.Add(this.lblTraderAFM);
            this.Controls.Add(this.lblTraderName);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.txtTrader);
            this.Controls.Add(this.lblTrader);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MasterForm";
            this.Text = "Κύριες πληροφορίες";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MasterReceiptForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtTrader;
        private System.Windows.Forms.Label lblTrader;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private OpenNETCF.Windows.Forms.Button2 btnContinue;
        private System.Windows.Forms.Label lblTraderName;
        private System.Windows.Forms.Label lblTraderAFM;
        private System.Windows.Forms.TextBox txtTraderName;
        private System.Windows.Forms.TextBox txtTraderAFM;
    }
}