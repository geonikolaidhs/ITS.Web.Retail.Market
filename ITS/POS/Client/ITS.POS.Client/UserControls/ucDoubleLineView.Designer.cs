namespace ITS.POS.Client.UserControls
{
    partial class ucDoubleLineView
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
            this.lblPreviousLine = new DevExpress.XtraEditors.LabelControl();
            this.lblPreviousLineQty = new DevExpress.XtraEditors.LabelControl();
            this.txtPreviousLineDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblPreviousLineValue = new DevExpress.XtraEditors.LabelControl();
            this.lblPreviousLineX = new DevExpress.XtraEditors.LabelControl();
            this.lblCurrentLineX = new DevExpress.XtraEditors.LabelControl();
            this.lblCurrentLineValue = new DevExpress.XtraEditors.LabelControl();
            this.txtCurrentLineDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblCurrentLineQty = new DevExpress.XtraEditors.LabelControl();
            this.lblCurrentLine = new DevExpress.XtraEditors.LabelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.txtPreviousLineDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLineDescription.Properties)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPreviousLine
            // 
            this.lblPreviousLine.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblPreviousLine.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPreviousLine.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPreviousLine.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblPreviousLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviousLine.Location = new System.Drawing.Point(3, 3);
            this.lblPreviousLine.Name = "lblPreviousLine";
            this.lblPreviousLine.Size = new System.Drawing.Size(109, 25);
            this.lblPreviousLine.TabIndex = 1;
            this.lblPreviousLine.Text = "PREV.LINE";
            // 
            // lblPreviousLineQty
            // 
            this.lblPreviousLineQty.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblPreviousLineQty.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPreviousLineQty.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPreviousLineQty.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblPreviousLineQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviousLineQty.Location = new System.Drawing.Point(348, 3);
            this.lblPreviousLineQty.Name = "lblPreviousLineQty";
            this.lblPreviousLineQty.Size = new System.Drawing.Size(80, 0);
            this.lblPreviousLineQty.TabIndex = 2;
            // 
            // txtPreviousLineDescription
            // 
            this.txtPreviousLineDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreviousLineDescription.Location = new System.Drawing.Point(118, 3);
            this.txtPreviousLineDescription.Name = "txtPreviousLineDescription";
            this.txtPreviousLineDescription.Properties.AllowFocused = false;
            this.txtPreviousLineDescription.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtPreviousLineDescription.Properties.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.txtPreviousLineDescription.Properties.Appearance.Options.UseFont = true;
            this.txtPreviousLineDescription.Properties.Appearance.Options.UseForeColor = true;
            this.txtPreviousLineDescription.Properties.ReadOnly = true;
            this.txtPreviousLineDescription.Size = new System.Drawing.Size(224, 32);
            this.txtPreviousLineDescription.TabIndex = 3;
            // 
            // lblPreviousLineValue
            // 
            this.lblPreviousLineValue.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblPreviousLineValue.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPreviousLineValue.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPreviousLineValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblPreviousLineValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviousLineValue.Location = new System.Drawing.Point(451, 3);
            this.lblPreviousLineValue.Name = "lblPreviousLineValue";
            this.lblPreviousLineValue.Size = new System.Drawing.Size(121, 0);
            this.lblPreviousLineValue.TabIndex = 5;
            // 
            // lblPreviousLineX
            // 
            this.lblPreviousLineX.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblPreviousLineX.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.lblPreviousLineX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviousLineX.Location = new System.Drawing.Point(434, 3);
            this.lblPreviousLineX.Name = "lblPreviousLineX";
            this.lblPreviousLineX.Size = new System.Drawing.Size(12, 25);
            this.lblPreviousLineX.TabIndex = 7;
            this.lblPreviousLineX.Text = "X";
            this.lblPreviousLineX.Visible = false;
            // 
            // lblCurrentLineX
            // 
            this.lblCurrentLineX.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblCurrentLineX.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCurrentLineX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentLineX.Location = new System.Drawing.Point(434, 40);
            this.lblCurrentLineX.Name = "lblCurrentLineX";
            this.lblCurrentLineX.Size = new System.Drawing.Size(12, 25);
            this.lblCurrentLineX.TabIndex = 12;
            this.lblCurrentLineX.Text = "X";
            this.lblCurrentLineX.Visible = false;
            // 
            // lblCurrentLineValue
            // 
            this.lblCurrentLineValue.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblCurrentLineValue.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCurrentLineValue.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCurrentLineValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCurrentLineValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentLineValue.Location = new System.Drawing.Point(451, 40);
            this.lblCurrentLineValue.Name = "lblCurrentLineValue";
            this.lblCurrentLineValue.Size = new System.Drawing.Size(121, 0);
            this.lblCurrentLineValue.TabIndex = 11;
            // 
            // txtCurrentLineDescription
            // 
            this.txtCurrentLineDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentLineDescription.Location = new System.Drawing.Point(118, 40);
            this.txtCurrentLineDescription.Name = "txtCurrentLineDescription";
            this.txtCurrentLineDescription.Properties.AllowFocused = false;
            this.txtCurrentLineDescription.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtCurrentLineDescription.Properties.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtCurrentLineDescription.Properties.Appearance.Options.UseFont = true;
            this.txtCurrentLineDescription.Properties.Appearance.Options.UseForeColor = true;
            this.txtCurrentLineDescription.Properties.ReadOnly = true;
            this.txtCurrentLineDescription.Size = new System.Drawing.Size(224, 32);
            this.txtCurrentLineDescription.TabIndex = 10;
            // 
            // lblCurrentLineQty
            // 
            this.lblCurrentLineQty.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblCurrentLineQty.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCurrentLineQty.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCurrentLineQty.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCurrentLineQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentLineQty.Location = new System.Drawing.Point(348, 40);
            this.lblCurrentLineQty.Name = "lblCurrentLineQty";
            this.lblCurrentLineQty.Size = new System.Drawing.Size(80, 0);
            this.lblCurrentLineQty.TabIndex = 9;
            // 
            // lblCurrentLine
            // 
            this.lblCurrentLine.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblCurrentLine.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCurrentLine.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCurrentLine.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCurrentLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentLine.Location = new System.Drawing.Point(3, 40);
            this.lblCurrentLine.Name = "lblCurrentLine";
            this.lblCurrentLine.Size = new System.Drawing.Size(109, 25);
            this.lblCurrentLine.TabIndex = 8;
            this.lblCurrentLine.Text = "CURR.LINE";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.Controls.Add(this.lblPreviousLineX, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrentLine, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtCurrentLineDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPreviousLine, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrentLineValue, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrentLineQty, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPreviousLineDescription, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrentLineX, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPreviousLineValue, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPreviousLineQty, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(575, 74);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // ucDoubleLineView
            // 
            this.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucDoubleLineView";
            this.Size = new System.Drawing.Size(575, 74);
            this.Load += new System.EventHandler(this.ucDoubleLineView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtPreviousLineDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLineDescription.Properties)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblPreviousLine;
        private DevExpress.XtraEditors.LabelControl lblPreviousLineQty;
        private DevExpress.XtraEditors.TextEdit txtPreviousLineDescription;
        private DevExpress.XtraEditors.LabelControl lblPreviousLineValue;
        private DevExpress.XtraEditors.LabelControl lblPreviousLineX;
        private DevExpress.XtraEditors.LabelControl lblCurrentLineX;
        private DevExpress.XtraEditors.LabelControl lblCurrentLineValue;
        private DevExpress.XtraEditors.TextEdit txtCurrentLineDescription;
        private DevExpress.XtraEditors.LabelControl lblCurrentLineQty;
        private DevExpress.XtraEditors.LabelControl lblCurrentLine;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}
