namespace ITS.POS.Client.UserControls
{
    partial class ucCommunicationStatusSmall
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.imgUp = new System.Windows.Forms.PictureBox();
            this.imgDown = new System.Windows.Forms.PictureBox();
            this.imgOffline = new System.Windows.Forms.PictureBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgOffline)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.imgUp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.imgDown, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.imgOffline, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(137, 47);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // imgUp
            // 
            this.imgUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgUp.Image = global::ITS.POS.Client.Properties.Resources.arrow_up;
            this.imgUp.Location = new System.Drawing.Point(93, 3);
            this.imgUp.Name = "imgUp";
            this.imgUp.Size = new System.Drawing.Size(41, 41);
            this.imgUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgUp.TabIndex = 3;
            this.imgUp.TabStop = false;
            this.imgUp.Visible = false;
            // 
            // imgDown
            // 
            this.imgDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgDown.Image = global::ITS.POS.Client.Properties.Resources.arrow_down;
            this.imgDown.Location = new System.Drawing.Point(48, 3);
            this.imgDown.Name = "imgDown";
            this.imgDown.Size = new System.Drawing.Size(39, 41);
            this.imgDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgDown.TabIndex = 2;
            this.imgDown.TabStop = false;
            this.imgDown.Visible = false;
            // 
            // imgOffline
            // 
            this.imgOffline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgOffline.Image = global::ITS.POS.Client.Properties.Resources.offline_dot;
            this.imgOffline.Location = new System.Drawing.Point(3, 3);
            this.imgOffline.Name = "imgOffline";
            this.imgOffline.Size = new System.Drawing.Size(39, 41);
            this.imgOffline.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgOffline.TabIndex = 1;
            this.imgOffline.TabStop = false;
            // 
            // CommunicationStatusSmall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CommunicationStatusSmall";
            this.Size = new System.Drawing.Size(137, 47);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgOffline)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox imgUp;
        private System.Windows.Forms.PictureBox imgDown;
        private System.Windows.Forms.PictureBox imgOffline;
        private System.Windows.Forms.ToolTip tooltip;
    }
}
