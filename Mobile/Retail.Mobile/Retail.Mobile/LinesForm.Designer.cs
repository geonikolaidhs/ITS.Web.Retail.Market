namespace Retail.Mobile
{
    partial class LinesForm
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
            this.btnclose = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.txt_item_details = new System.Windows.Forms.TextBox();
            this.btnUpdateQty = new System.Windows.Forms.Button();
            this.txtEditQty = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDeleteLine = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnclose
            // 
            this.btnclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnclose.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnclose.Location = new System.Drawing.Point(563, 399);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(72, 53);
            this.btnclose.TabIndex = 0;
            this.btnclose.Text = "Επιστροφή";
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // dataGrid1
            // 
            this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular);
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(637, 358);
            this.dataGrid1.TabIndex = 1;
            this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
            this.dataGrid1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGrid1_KeyPress);
            // 
            // txt_item_details
            // 
            this.txt_item_details.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_item_details.BackColor = System.Drawing.Color.LightCyan;
            this.txt_item_details.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txt_item_details.Location = new System.Drawing.Point(3, 364);
            this.txt_item_details.Multiline = true;
            this.txt_item_details.Name = "txt_item_details";
            this.txt_item_details.ReadOnly = true;
            this.txt_item_details.Size = new System.Drawing.Size(632, 32);
            this.txt_item_details.TabIndex = 3;
            // 
            // btnUpdateQty
            // 
            this.btnUpdateQty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateQty.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnUpdateQty.Location = new System.Drawing.Point(81, 399);
            this.btnUpdateQty.Name = "btnUpdateQty";
            this.btnUpdateQty.Size = new System.Drawing.Size(476, 53);
            this.btnUpdateQty.TabIndex = 4;
            this.btnUpdateQty.Text = "Ποσότητα";
            this.btnUpdateQty.Click += new System.EventHandler(this.btnUpdateQty_Click);
            // 
            // txtEditQty
            // 
            this.txtEditQty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEditQty.Location = new System.Drawing.Point(614, 373);
            this.txtEditQty.Name = "txtEditQty";
            this.txtEditQty.Size = new System.Drawing.Size(21, 23);
            this.txtEditQty.TabIndex = 5;
            this.txtEditQty.Visible = false;
            this.txtEditQty.TextChanged += new System.EventHandler(this.txtEditQty_TextChanged);
            this.txtEditQty.GotFocus += new System.EventHandler(this.txtEditQty_GotFocus);
            this.txtEditQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEditQty_KeyPress);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel1.Controls.Add(this.btnDeleteLine);
            this.panel1.Controls.Add(this.txtEditQty);
            this.panel1.Controls.Add(this.btnUpdateQty);
            this.panel1.Controls.Add(this.txt_item_details);
            this.panel1.Controls.Add(this.dataGrid1);
            this.panel1.Controls.Add(this.btnclose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 455);
            // 
            // btnDeleteLine
            // 
            this.btnDeleteLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteLine.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteLine.Location = new System.Drawing.Point(3, 399);
            this.btnDeleteLine.Name = "btnDeleteLine";
            this.btnDeleteLine.Size = new System.Drawing.Size(72, 53);
            this.btnDeleteLine.TabIndex = 6;
            this.btnDeleteLine.Text = "Διαγραφή";
            this.btnDeleteLine.Click += new System.EventHandler(this.btnDeleteLine_Click);
            // 
            // LinesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(638, 455);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinesForm";
            this.Text = "Γραμμές";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.LinesForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.TextBox txt_item_details;
        private System.Windows.Forms.Button btnUpdateQty;
        private System.Windows.Forms.TextBox txtEditQty;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDeleteLine;

    }
}