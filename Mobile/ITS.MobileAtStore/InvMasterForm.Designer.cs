namespace ITS.MobileAtStore
{
    partial class InvMasterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvMasterForm));
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.btnInv = new OpenNETCF.Windows.Forms.Button2();
            this.btnControl = new OpenNETCF.Windows.Forms.Button2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
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
            // btnInv
            // 
            this.btnInv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnInv.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInv.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnInv.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnInv.Image = ((System.Drawing.Image)(resources.GetObject("btnInv.Image")));
            this.btnInv.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnInv.ImageIndex = -1;
            this.btnInv.ImageList = null;
            this.btnInv.Location = new System.Drawing.Point(0, 12);
            this.btnInv.Name = "btnInv";
            this.btnInv.Size = new System.Drawing.Size(238, 32);
            this.btnInv.TabIndex = 17;
            this.btnInv.TabStop = false;
            this.btnInv.Text = "(1) Απογραφή";
            this.btnInv.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnInv.Click += new System.EventHandler(this.btnInv_Click);
            // 
            // btnControl
            // 
            this.btnControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnControl.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnControl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnControl.Image = ((System.Drawing.Image)(resources.GetObject("btnControl.Image")));
            this.btnControl.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnControl.ImageIndex = -1;
            this.btnControl.ImageList = null;
            this.btnControl.Location = new System.Drawing.Point(0, 68);
            this.btnControl.Name = "btnControl";
            this.btnControl.Size = new System.Drawing.Size(238, 32);
            this.btnControl.TabIndex = 18;
            this.btnControl.TabStop = false;
            this.btnControl.Text = "(2) Έλεγχος";
            this.btnControl.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnControl.Click += new System.EventHandler(this.btnControl_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(238, 12);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gainsboro;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(238, 24);
            // 
            // InvMasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.btnControl);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnInv);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnReturn);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvMasterForm";
            this.Text = "Κέντρο απογραφής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.InvMasterForm_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.InvMasterForm_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private OpenNETCF.Windows.Forms.Button2 btnInv;
        private OpenNETCF.Windows.Forms.Button2 btnControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}