namespace ITS.MobileAtStore
{
    partial class DecompositionDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecompositionDetailForm));
            this.btnDelete = new OpenNETCF.Windows.Forms.Button2();
            this.txtProduct = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblProduct = new System.Windows.Forms.Label();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.lblMainItem = new System.Windows.Forms.Label();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.btnLeft = new OpenNETCF.Windows.Forms.Button2();
            this.btnRight = new OpenNETCF.Windows.Forms.Button2();
            this.lblPosition = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageIndex = -1;
            this.btnDelete.ImageList = null;
            this.btnDelete.Location = new System.Drawing.Point(84, 126);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(71, 38);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtProduct
            // 
            this.txtProduct.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtProduct.Location = new System.Drawing.Point(0, 101);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(235, 23);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
            // 
            // lblProduct
            // 
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(1, 82);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(169, 18);
            this.lblProduct.Text = "Κωδ. Είδους";
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
            this.btnReturn.Location = new System.Drawing.Point(3, 3);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(232, 32);
            this.btnReturn.TabIndex = 5;
            this.btnReturn.Text = "Επιστροφή";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // lblMainItem
            // 
            this.lblMainItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMainItem.BackColor = System.Drawing.Color.Gainsboro;
            this.lblMainItem.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblMainItem.ForeColor = System.Drawing.Color.Black;
            this.lblMainItem.Location = new System.Drawing.Point(0, 40);
            this.lblMainItem.Name = "lblMainItem";
            this.lblMainItem.Size = new System.Drawing.Size(235, 39);
            this.lblMainItem.Text = "Περιγραφή είδους προς ανάλωση με ποσότητα";
            // 
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(2, 165);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(235, 89);
            this.txtDescr.TabIndex = 65;
            this.txtDescr.GotFocus += new System.EventHandler(this.txtDescr_GotFocus);
            // 
            // btnLeft
            // 
            this.btnLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
            this.btnLeft.ImageIndex = -1;
            this.btnLeft.ImageList = null;
            this.btnLeft.Location = new System.Drawing.Point(3, 126);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(71, 38);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnRight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
            this.btnRight.ImageIndex = -1;
            this.btnRight.ImageList = null;
            this.btnRight.Location = new System.Drawing.Point(165, 126);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(71, 38);
            this.btnRight.TabIndex = 2;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // lblPosition
            // 
            this.lblPosition.Location = new System.Drawing.Point(3, 249);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(231, 20);
            this.lblPosition.Text = "label1";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DecompositionDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 273);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.txtDescr);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.lblMainItem);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnDelete);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DecompositionDetailForm";
            this.Text = "Ανάλωση Είδους";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.Button2 btnDelete;
        private OpenNETCF.Windows.Forms.TextBox2 txtProduct;
        private System.Windows.Forms.Label lblProduct;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private System.Windows.Forms.Label lblMainItem;
        private OpenNETCF.Windows.Forms.TextBox2 txtDescr;
        private OpenNETCF.Windows.Forms.Button2 btnLeft;
        private OpenNETCF.Windows.Forms.Button2 btnRight;
        private System.Windows.Forms.Label lblPosition;


    }
}