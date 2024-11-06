namespace ITS.MobileAtStore
{
    partial class AddOrReplace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddOrReplace));
            this.label1 = new System.Windows.Forms.Label();
            this.btnReplace = new OpenNETCF.Windows.Forms.Button2();
            this.btnAdd = new OpenNETCF.Windows.Forms.Button2();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(0, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 24);
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnReplace
            // 
            this.btnReplace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReplace.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReplace.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnReplace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReplace.Image = ((System.Drawing.Image)(resources.GetObject("btnReplace.Image")));
            this.btnReplace.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnReplace.ImageIndex = -1;
            this.btnReplace.ImageList = null;
            this.btnReplace.Location = new System.Drawing.Point(0, 0);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(238, 32);
            this.btnReplace.TabIndex = 0;
            this.btnReplace.TabStop = false;
            this.btnReplace.Text = "(1) Αντικατάσταση";
            this.btnReplace.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnAdd.ImageIndex = -1;
            this.btnAdd.ImageList = null;
            this.btnAdd.Location = new System.Drawing.Point(0, 56);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(238, 32);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.TabStop = false;
            this.btnAdd.Text = "(2) Πρόσθεση";
            this.btnAdd.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Gainsboro;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(0, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 180);
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AddOrReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReplace);
            this.Location = new System.Drawing.Point(0, 68);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddOrReplace";
            this.Text = "Αντικατάσταση ή πρόσθεση;";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AddOrReplace_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddOrReplace_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private OpenNETCF.Windows.Forms.Button2 btnReplace;
        private OpenNETCF.Windows.Forms.Button2 btnAdd;
        private System.Windows.Forms.Label label2;
    }
}