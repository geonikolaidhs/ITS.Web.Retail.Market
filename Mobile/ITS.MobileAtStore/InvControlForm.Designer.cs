namespace ITS.MobileAtStore
{
    partial class InvControlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvControlForm));
            this.InvLine = new System.Windows.Forms.DataGridTableStyle();
            this.ProdCode = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Qty = new System.Windows.Forms.DataGridTextBoxColumn();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblProduct = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.SuspendLayout();
            // 
            // InvLine
            // 
            this.InvLine.GridColumnStyles.Add(this.ProdCode);
            this.InvLine.GridColumnStyles.Add(this.Qty);
            this.InvLine.MappingName = "InvLine";
            // 
            // ProdCode
            // 
            this.ProdCode.Format = "";
            this.ProdCode.FormatInfo = null;
            this.ProdCode.HeaderText = "Κωδ. Είδους";
            this.ProdCode.MappingName = "ProdCode";
            this.ProdCode.NullText = "";
            this.ProdCode.Width = 237;
            // 
            // Qty
            // 
            this.Qty.Format = "";
            this.Qty.FormatInfo = null;
            this.Qty.HeaderText = "Ποσότ.";
            this.Qty.MappingName = "Qty";
            this.Qty.NullText = "";
            this.Qty.Width = 70;
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
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(3, 33);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(232, 45);
            this.txtDescr.TabIndex = 49;
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
            this.lblProduct.Location = new System.Drawing.Point(4, 3);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(98, 24);
            this.lblProduct.Text = "Κωδ. είδους";
            // 
            // txtProduct
            // 
            this.txtProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProduct.Location = new System.Drawing.Point(107, 3);
            this.txtProduct.Multiline = true;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(128, 24);
            this.txtProduct.TabIndex = 54;
            this.txtProduct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress_);
            // 
            // dataGrid1
            // 
            this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.dataGrid1.Location = new System.Drawing.Point(4, 84);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.RowHeadersVisible = false;
            this.dataGrid1.Size = new System.Drawing.Size(231, 143);
            this.dataGrid1.TabIndex = 55;
            this.dataGrid1.TableStyles.Add(this.InvLine);
            this.dataGrid1.TabStop = false;
            // 
            // InvControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.txtDescr);
            this.Controls.Add(this.btnReturn);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvControlForm";
            this.Text = "Έλεγχος απογραφής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.InvControlForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private OpenNETCF.Windows.Forms.TextBox2 txtDescr;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.DataGridTextBoxColumn ProdCode;
        private System.Windows.Forms.DataGridTextBoxColumn Qty;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.DataGridTableStyle InvLine;
    }
}