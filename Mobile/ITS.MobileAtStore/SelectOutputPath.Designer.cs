namespace ITS.MobileAtStore
{
    partial class SelectOutputPath
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectOutputPath));
            this.listboxOutputLocations = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new OpenNETCF.Windows.Forms.Button2();
            this.btnCancel = new OpenNETCF.Windows.Forms.Button2();
            this.SuspendLayout();
            // 
            // listboxOutputLocations
            // 
            this.listboxOutputLocations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listboxOutputLocations.DisplayMember = "OutputPathName";
            this.listboxOutputLocations.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.listboxOutputLocations.Location = new System.Drawing.Point(3, 30);
            this.listboxOutputLocations.Name = "listboxOutputLocations";
            this.listboxOutputLocations.Size = new System.Drawing.Size(232, 194);
            this.listboxOutputLocations.TabIndex = 18;
            this.listboxOutputLocations.ValueMember = "OutputPathName";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 24);
            this.label1.Text = "Επιλέξτε τοποθεσία εξαγωγής";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnOK.ImageIndex = -1;
            this.btnOK.ImageList = null;
            this.btnOK.Location = new System.Drawing.Point(3, 233);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(103, 32);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnCancel.ImageIndex = -1;
            this.btnCancel.ImageList = null;
            this.btnCancel.Location = new System.Drawing.Point(138, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 32);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "Άκυρο";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SelectOutputPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listboxOutputLocations);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectOutputPath";
            this.Text = "Τοποθεσίες εξαγωγής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SelectOutputPath_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listboxOutputLocations;
        private System.Windows.Forms.Label label1;
        private OpenNETCF.Windows.Forms.Button2 btnOK;
        private OpenNETCF.Windows.Forms.Button2 btnCancel;
    }
}