namespace Retail.Mobile
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.groupBox1 = new OpenNETCF.Windows.Forms.GroupBox();
            this.animateCtl1 = new OpenNETCF.Windows.Forms.AnimateCtl();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.groupBox1.Controls.Add(this.animateCtl1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 40);
            this.groupBox1.TabIndex = 3;
            // 
            // animateCtl1
            // 
            this.animateCtl1.BackColor = System.Drawing.SystemColors.Control;
            this.animateCtl1.DrawDirection = OpenNETCF.Windows.Forms.DrawDirection.Horizontal;
            this.animateCtl1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.animateCtl1.FrameHeight = 27;
            this.animateCtl1.FrameWidth = 27;
            this.animateCtl1.Height = 27;
            this.animateCtl1.Image = ((System.Drawing.Image)(resources.GetObject("animateCtl1.Image")));
            this.animateCtl1.Location = new System.Drawing.Point(3, 3);
            this.animateCtl1.Name = "animateCtl1";
            this.animateCtl1.Size = new System.Drawing.Size(27, 27);
            this.animateCtl1.TabIndex = 5;
            this.animateCtl1.Width = 27;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(48, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 35);
            this.label1.Text = "Επικοινωνία με τον Server. Παρακαλώ περιμένετε ....";
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(222, 41);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoadingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenNETCF.Windows.Forms.GroupBox groupBox1;
        private OpenNETCF.Windows.Forms.AnimateCtl animateCtl1;
        private System.Windows.Forms.Label label1;


    }
}