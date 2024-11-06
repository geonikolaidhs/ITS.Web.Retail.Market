namespace ITS.MobileAtStore
{
    partial class AdvancedPriceCheckingSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedPriceCheckingSettingsForm));
            this.listItem1 = new OpenNETCF.Windows.Forms.ListItem();
            this.lblOperationMode = new System.Windows.Forms.Label();
            this.button21 = new OpenNETCF.Windows.Forms.Button2();
            this.cmbCompCode = new OpenNETCF.Windows.Forms.ComboBox2(this.components);
            this.cmdPriceCatalogPolicy = new OpenNETCF.Windows.Forms.ComboBox2(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listItem1
            // 
            this.listItem1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.listItem1.ForeColor = System.Drawing.Color.Black;
            this.listItem1.ImageIndex = -1;
            this.listItem1.Text = "1";
            // 
            // lblOperationMode
            // 
            this.lblOperationMode.BackColor = System.Drawing.Color.Gainsboro;
            this.lblOperationMode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblOperationMode.ForeColor = System.Drawing.Color.Black;
            this.lblOperationMode.Location = new System.Drawing.Point(4, 3);
            this.lblOperationMode.Name = "lblOperationMode";
            this.lblOperationMode.Size = new System.Drawing.Size(231, 24);
            this.lblOperationMode.Text = "Αποθηκευτικός χώρος";
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
            // cmbCompCode
            // 
            this.cmbCompCode.DisplayMember = "Description";
            this.cmbCompCode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.cmbCompCode.Location = new System.Drawing.Point(4, 30);
            this.cmbCompCode.Name = "cmbCompCode";
            this.cmbCompCode.Size = new System.Drawing.Size(231, 24);
            this.cmbCompCode.TabIndex = 0;
            this.cmbCompCode.ValueMember = "CompCode";
            // 
            // cmdPriceCatalogPolicy
            // 
            this.cmdPriceCatalogPolicy.DisplayMember = "Description";
            this.cmdPriceCatalogPolicy.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.cmdPriceCatalogPolicy.Location = new System.Drawing.Point(4, 84);
            this.cmdPriceCatalogPolicy.Name = "cmdPriceCatalogPolicy";
            this.cmdPriceCatalogPolicy.Size = new System.Drawing.Size(231, 24);
            this.cmdPriceCatalogPolicy.TabIndex = 6;
            this.cmdPriceCatalogPolicy.ValueMember = "ID";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(4, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 24);
            this.label1.Text = "Τιμολογιακή Πολιτική";
            // 
            // AdvancedPriceCheckingSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(238, 269);
            this.Controls.Add(this.cmdPriceCatalogPolicy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button21);
            this.Controls.Add(this.cmbCompCode);
            this.Controls.Add(this.lblOperationMode);
            this.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedPriceCheckingSettingsForm";
            this.Text = "Ρυθμίσεις ελέγχου τιμής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AdvancedPriceCheckingSettingsForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.ListItem listItem1;
        private System.Windows.Forms.Label lblOperationMode;
        private OpenNETCF.Windows.Forms.Button2 button21;
        private OpenNETCF.Windows.Forms.ComboBox2 cmbCompCode;
        private OpenNETCF.Windows.Forms.ComboBox2 cmdPriceCatalogPolicy;
        private System.Windows.Forms.Label label1;
    }
}