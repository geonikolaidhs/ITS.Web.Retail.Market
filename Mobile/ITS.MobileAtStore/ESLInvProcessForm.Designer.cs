namespace ITS.MobileAtStore
{
    partial class ESLInvProcessForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ESLInvProcessForm));
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.lblDescr = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblQtyValue = new System.Windows.Forms.Label();
            this.lblInvNumber = new System.Windows.Forms.Label();
            this.txtInvNumber = new System.Windows.Forms.TextBox();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblProduct = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            this.btnReturn.Location = new System.Drawing.Point(115, 233);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(120, 32);
            this.btnReturn.TabIndex = 13;
            this.btnReturn.TabStop = false;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // lblDescr
            // 
            this.lblDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescr.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDescr.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblDescr.ForeColor = System.Drawing.Color.Black;
            this.lblDescr.Location = new System.Drawing.Point(3, 90);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(233, 24);
            this.lblDescr.Text = "Περιγραφή";
            this.lblDescr.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblQty
            // 
            this.lblQty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQty.BackColor = System.Drawing.Color.Gainsboro;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblQty.ForeColor = System.Drawing.Color.Black;
            this.lblQty.Location = new System.Drawing.Point(2, 61);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(98, 24);
            this.lblQty.Text = "Ποσότητα";
            // 
            // lblQtyValue
            // 
            this.lblQtyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQtyValue.BackColor = System.Drawing.Color.LightBlue;
            this.lblQtyValue.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblQtyValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblQtyValue.Location = new System.Drawing.Point(106, 61);
            this.lblQtyValue.Name = "lblQtyValue";
            this.lblQtyValue.Size = new System.Drawing.Size(129, 24);
            this.lblQtyValue.Text = "0";
            this.lblQtyValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblInvNumber
            // 
            this.lblInvNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInvNumber.BackColor = System.Drawing.Color.Gainsboro;
            this.lblInvNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblInvNumber.ForeColor = System.Drawing.Color.Black;
            this.lblInvNumber.Location = new System.Drawing.Point(2, 4);
            this.lblInvNumber.Name = "lblInvNumber";
            this.lblInvNumber.Size = new System.Drawing.Size(98, 24);
            this.lblInvNumber.Text = "Αρ. απογρ.";
            // 
            // txtInvNumber
            // 
            this.txtInvNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInvNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtInvNumber.Location = new System.Drawing.Point(106, 4);
            this.txtInvNumber.Multiline = true;
            this.txtInvNumber.Name = "txtInvNumber";
            this.txtInvNumber.Size = new System.Drawing.Size(129, 24);
            this.txtInvNumber.TabIndex = 0;
            this.txtInvNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInvNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInvNumber_KeyPress);
            // 
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(2, 117);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(233, 109);
            this.txtDescr.TabIndex = 45;
            this.txtDescr.TabStop = false;
            this.txtDescr.GotFocus += new System.EventHandler(this.txtDescr_GotFocus);
            // 
            // lblProduct
            // 
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(2, 33);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(98, 24);
            this.lblProduct.Text = "Κωδ. είδους";
            // 
            // txtProduct
            // 
            this.txtProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProduct.Location = new System.Drawing.Point(106, 33);
            this.txtProduct.Multiline = true;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(129, 24);
            this.txtProduct.TabIndex = 1;
            this.txtProduct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
            // 
            // ESLInvProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.lblDescr);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.lblQtyValue);
            this.Controls.Add(this.lblInvNumber);
            this.Controls.Add(this.txtInvNumber);
            this.Controls.Add(this.txtDescr);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.btnReturn);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ESLInvProcessForm";
            this.Text = "Απογραφή ESL";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ESLInvProcessForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblQtyValue;
        private System.Windows.Forms.Label lblInvNumber;
        private System.Windows.Forms.TextBox txtInvNumber;
        private OpenNETCF.Windows.Forms.TextBox2 txtDescr;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.TextBox txtProduct;
    }
}