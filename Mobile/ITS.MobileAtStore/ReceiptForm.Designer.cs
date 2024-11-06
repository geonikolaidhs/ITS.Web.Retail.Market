namespace ITS.MobileAtStore
{
    partial class ReceiptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReceiptForm));
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.txtRecords = new System.Windows.Forms.TextBox();
            this.lblRecords = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.lblExtraInfo = new System.Windows.Forms.Label();
            this.btnExport = new OpenNETCF.Windows.Forms.Button2();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblReqQtyText = new System.Windows.Forms.Label();
            this.lblReqQtyValue = new System.Windows.Forms.Label();
            this.lblScannedQtyValue = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRestInvQty = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnReturn
            // 
            this.btnReturn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReturn.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnReturn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReturn.Image = ((System.Drawing.Image)(resources.GetObject("btnReturn.Image")));
            this.btnReturn.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnReturn.ImageIndex = -1;
            this.btnReturn.ImageList = null;
            this.btnReturn.Location = new System.Drawing.Point(195, 233);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(120, 32);
            this.btnReturn.TabIndex = 2;
            this.btnReturn.TabStop = false;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // txtRecords
            // 
            this.txtRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecords.BackColor = System.Drawing.Color.LightBlue;
            this.txtRecords.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtRecords.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtRecords.Location = new System.Drawing.Point(3, 30);
            this.txtRecords.Multiline = true;
            this.txtRecords.Name = "txtRecords";
            this.txtRecords.Size = new System.Drawing.Size(224, 24);
            this.txtRecords.TabIndex = 5;
            this.txtRecords.TabStop = false;
            this.txtRecords.Text = "0";
            this.txtRecords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecords.GotFocus += new System.EventHandler(this.txtRecords_GotFocus);
            this.txtRecords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRecords_KeyPress);
            // 
            // lblRecords
            // 
            this.lblRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecords.BackColor = System.Drawing.Color.Gainsboro;
            this.lblRecords.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblRecords.ForeColor = System.Drawing.Color.Black;
            this.lblRecords.Location = new System.Drawing.Point(3, 3);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(224, 24);
            this.lblRecords.Text = "Εγγραφές";
            // 
            // lblProduct
            // 
            this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(3, 64);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(224, 24);
            this.lblProduct.Text = "Κωδ. && Περιγραφή Είδους";
            // 
            // txtProduct
            // 
            this.txtProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProduct.Location = new System.Drawing.Point(3, 91);
            this.txtProduct.Multiline = true;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(224, 24);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
            // 
            // lblExtraInfo
            // 
            this.lblExtraInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExtraInfo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblExtraInfo.Location = new System.Drawing.Point(4, 207);
            this.lblExtraInfo.Name = "lblExtraInfo";
            this.lblExtraInfo.Size = new System.Drawing.Size(311, 20);
            // 
            // btnExport
            // 
            this.btnExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnExport.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnExport.ImageIndex = -1;
            this.btnExport.ImageList = null;
            this.btnExport.Location = new System.Drawing.Point(3, 233);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(186, 32);
            this.btnExport.TabIndex = 20;
            this.btnExport.Text = "Εξαγωγή";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(3, 118);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(224, 86);
            this.txtDescr.TabIndex = 25;
            this.txtDescr.GotFocus += new System.EventHandler(this.txtDescr_GotFocus);
            this.txtDescr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDescr_KeyPress);
            // 
            // lblReqQtyText
            // 
            this.lblReqQtyText.BackColor = System.Drawing.Color.Gainsboro;
            this.lblReqQtyText.ForeColor = System.Drawing.Color.Black;
            this.lblReqQtyText.Location = new System.Drawing.Point(235, 3);
            this.lblReqQtyText.Name = "lblReqQtyText";
            this.lblReqQtyText.Size = new System.Drawing.Size(80, 20);
            this.lblReqQtyText.Text = "Παραγγελίας";
            this.lblReqQtyText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblReqQtyValue
            // 
            this.lblReqQtyValue.BackColor = System.Drawing.Color.LightBlue;
            this.lblReqQtyValue.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblReqQtyValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblReqQtyValue.Location = new System.Drawing.Point(235, 23);
            this.lblReqQtyValue.Name = "lblReqQtyValue";
            this.lblReqQtyValue.Size = new System.Drawing.Size(80, 24);
            this.lblReqQtyValue.Text = "0";
            this.lblReqQtyValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblScannedQtyValue
            // 
            this.lblScannedQtyValue.BackColor = System.Drawing.Color.LightBlue;
            this.lblScannedQtyValue.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblScannedQtyValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblScannedQtyValue.Location = new System.Drawing.Point(235, 103);
            this.lblScannedQtyValue.Name = "lblScannedQtyValue";
            this.lblScannedQtyValue.Size = new System.Drawing.Size(80, 24);
            this.lblScannedQtyValue.Text = "0";
            this.lblScannedQtyValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(235, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 24);
            this.label1.Text = "Παραλαβής";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblRestInvQty
            // 
            this.lblRestInvQty.BackColor = System.Drawing.Color.LightBlue;
            this.lblRestInvQty.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblRestInvQty.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblRestInvQty.Location = new System.Drawing.Point(235, 180);
            this.lblRestInvQty.Name = "lblRestInvQty";
            this.lblRestInvQty.Size = new System.Drawing.Size(80, 24);
            this.lblRestInvQty.Text = "0";
            this.lblRestInvQty.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Gainsboro;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(235, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.Text = "Τιμολογίου";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ReceiptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(318, 268);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblRestInvQty);
            this.Controls.Add(this.lblScannedQtyValue);
            this.Controls.Add(this.lblReqQtyValue);
            this.Controls.Add(this.lblReqQtyText);
            this.Controls.Add(this.txtDescr);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblExtraInfo);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.txtRecords);
            this.Controls.Add(this.lblRecords);
            this.Controls.Add(this.btnReturn);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReceiptForm";
            this.Text = "Εισαγωγή προϊόντων παραλαβής ";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ReceiptForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private System.Windows.Forms.TextBox txtRecords;
        private System.Windows.Forms.Label lblRecords;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.Label lblExtraInfo;
        private OpenNETCF.Windows.Forms.Button2 btnExport;
        private OpenNETCF.Windows.Forms.TextBox2 txtDescr;
        private System.Windows.Forms.Label lblReqQtyText;
        private System.Windows.Forms.Label lblReqQtyValue;
        private System.Windows.Forms.Label lblScannedQtyValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRestInvQty;
        private System.Windows.Forms.Label label3;
    }
}