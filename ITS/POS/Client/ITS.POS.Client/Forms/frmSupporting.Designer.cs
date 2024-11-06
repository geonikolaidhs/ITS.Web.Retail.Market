namespace ITS.POS.Client.Forms
{
    partial class frmSupporting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSupporting));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.ucDoubleLineView1 = new ITS.POS.Client.UserControls.ucDoubleLineView();
            this.ucTotalPriceView1 = new ITS.POS.Client.UserControls.ucTotalPriceView();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Size = new System.Drawing.Size(504, 250);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://www.its.net.gr/", System.UriKind.Absolute);
            // 
            // ucDoubleLineView1
            // 
            this.ucDoubleLineView1.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.Appearance.Options.UseFont = true;
            // 
            // 
            // 
            this.ucDoubleLineView1.CurrentLineLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.CurrentLineLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.CurrentLineLabel.Location = new System.Drawing.Point(3, 53);
            this.ucDoubleLineView1.CurrentLineLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.CurrentLineLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.CurrentLineLabel.Name = "lblCurrentLine";
            this.ucDoubleLineView1.CurrentLineLabel.Size = new System.Drawing.Size(103, 25);
            this.ucDoubleLineView1.CurrentLineLabel.TabIndex = 8;
            this.ucDoubleLineView1.CurrentLineLabel.Text = "CURR.LINE";
            // 
            // 
            // 
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.Location = new System.Drawing.Point(153, 53);
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.Name = "lblCurrentLineX";
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.Size = new System.Drawing.Size(0, 25);
            this.ucDoubleLineView1.CurrentLineMuliplierLabel.TabIndex = 12;
            // 
            // 
            // 
            this.ucDoubleLineView1.CurrentLineQtyLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.CurrentLineQtyLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.CurrentLineQtyLabel.Location = new System.Drawing.Point(123, 53);
            this.ucDoubleLineView1.CurrentLineQtyLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.CurrentLineQtyLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.CurrentLineQtyLabel.Name = "lblCurrentLineQty";
            this.ucDoubleLineView1.CurrentLineQtyLabel.Size = new System.Drawing.Size(40, 25);
            this.ucDoubleLineView1.CurrentLineQtyLabel.TabIndex = 9;
            this.ucDoubleLineView1.CurrentLineQtyLabel.Text = "QTY";
            // 
            // 
            // 
            this.ucDoubleLineView1.CurrentLineValueLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.CurrentLineValueLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.CurrentLineValueLabel.Location = new System.Drawing.Point(159, 53);
            this.ucDoubleLineView1.CurrentLineValueLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.CurrentLineValueLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.CurrentLineValueLabel.Name = "lblCurrentLineValue";
            this.ucDoubleLineView1.CurrentLineValueLabel.Size = new System.Drawing.Size(62, 25);
            this.ucDoubleLineView1.CurrentLineValueLabel.TabIndex = 11;
            this.ucDoubleLineView1.CurrentLineValueLabel.Text = "VALUE";
            this.ucDoubleLineView1.LabelPercentage = 20F;
            this.ucDoubleLineView1.Location = new System.Drawing.Point(0, 255);
            this.ucDoubleLineView1.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.Name = "ucDoubleLineView1";
            // 
            // 
            // 
            this.ucDoubleLineView1.PreviousLineLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.PreviousLineLabel.Location = new System.Drawing.Point(3, 3);
            this.ucDoubleLineView1.PreviousLineLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.PreviousLineLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.PreviousLineLabel.Name = "lblPreviousLine";
            this.ucDoubleLineView1.PreviousLineLabel.Size = new System.Drawing.Size(51, 13);
            this.ucDoubleLineView1.PreviousLineLabel.TabIndex = 1;
            this.ucDoubleLineView1.PreviousLineLabel.Text = "PREV.LINE";
            // 
            // 
            // 
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.Location = new System.Drawing.Point(153, 3);
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.Name = "lblPreviousLineX";
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.Size = new System.Drawing.Size(0, 13);
            this.ucDoubleLineView1.PreviousLineMuliplierLabel.TabIndex = 7;
            // 
            // 
            // 
            this.ucDoubleLineView1.PreviousLineQtyLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.PreviousLineQtyLabel.Location = new System.Drawing.Point(123, 3);
            this.ucDoubleLineView1.PreviousLineQtyLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.PreviousLineQtyLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.PreviousLineQtyLabel.Name = "lblPreviousLineQty";
            this.ucDoubleLineView1.PreviousLineQtyLabel.Size = new System.Drawing.Size(0, 13);
            this.ucDoubleLineView1.PreviousLineQtyLabel.TabIndex = 2;
            // 
            // 
            // 
            this.ucDoubleLineView1.PreviousLineValueLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucDoubleLineView1.PreviousLineValueLabel.Location = new System.Drawing.Point(159, 3);
            this.ucDoubleLineView1.PreviousLineValueLabel.LookAndFeel.SkinName = "Metropolis";
            this.ucDoubleLineView1.PreviousLineValueLabel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucDoubleLineView1.PreviousLineValueLabel.Name = "lblPreviousLineValue";
            this.ucDoubleLineView1.PreviousLineValueLabel.Size = new System.Drawing.Size(0, 13);
            this.ucDoubleLineView1.PreviousLineValueLabel.TabIndex = 5;
            this.ucDoubleLineView1.Size = new System.Drawing.Size(498, 77);
            this.ucDoubleLineView1.TabIndex = 1;
            // 
            // 
            // 
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.ucDoubleLineView1.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.PreviousLineMuliplierLabel, 3, 0);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.CurrentLineLabel, 0, 1);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.PreviousLineLabel, 0, 0);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.CurrentLineValueLabel, 4, 1);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.CurrentLineQtyLabel, 2, 1);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.CurrentLineMuliplierLabel, 3, 1);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.PreviousLineValueLabel, 4, 0);
            this.ucDoubleLineView1.TableLayoutPanel.Controls.Add(this.ucDoubleLineView1.PreviousLineQtyLabel, 2, 0);
            this.ucDoubleLineView1.TableLayoutPanel.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucDoubleLineView1.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ucDoubleLineView1.TableLayoutPanel.Name = "tableLayoutPanel1";
            this.ucDoubleLineView1.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucDoubleLineView1.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucDoubleLineView1.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucDoubleLineView1.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucDoubleLineView1.TableLayoutPanel.TabIndex = 13;
            this.ucDoubleLineView1.TabStop = false;
            // 
            // ucTotalPriceView1
            // 
            this.ucTotalPriceView1.AlwaysShow = false;
            this.ucTotalPriceView1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(117)))), ((int)(((byte)(254)))));
            this.ucTotalPriceView1.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucTotalPriceView1.Appearance.ForeColor = System.Drawing.Color.Azure;
            this.ucTotalPriceView1.Appearance.Options.UseBackColor = true;
            this.ucTotalPriceView1.Appearance.Options.UseFont = true;
            this.ucTotalPriceView1.Appearance.Options.UseForeColor = true;
            this.ucTotalPriceView1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ucTotalPriceView1.LabelPercentage = 50F;
            this.ucTotalPriceView1.Location = new System.Drawing.Point(12, 356);
            this.ucTotalPriceView1.LookAndFeel.SkinName = "Metropolis";
            this.ucTotalPriceView1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucTotalPriceView1.Name = "ucTotalPriceView1";
            this.ucTotalPriceView1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.ucTotalPriceView1.Size = new System.Drawing.Size(486, 36);
            this.ucTotalPriceView1.TabIndex = 2;
            // 
            // 
            // 
            this.ucTotalPriceView1.TitleLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucTotalPriceView1.TitleLabel.Location = new System.Drawing.Point(4, 4);
            this.ucTotalPriceView1.TitleLabel.Name = "lblTitle";
            this.ucTotalPriceView1.TitleLabel.Size = new System.Drawing.Size(66, 25);
            this.ucTotalPriceView1.TitleLabel.TabIndex = 0;
            this.ucTotalPriceView1.TitleLabel.Text = "Σύνολο";
            // 
            // 
            // 
            this.ucTotalPriceView1.ValueLabel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.ucTotalPriceView1.ValueLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ucTotalPriceView1.ValueLabel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.ucTotalPriceView1.ValueLabel.Location = new System.Drawing.Point(246, 4);
            this.ucTotalPriceView1.ValueLabel.Name = "lblValue";
            this.ucTotalPriceView1.ValueLabel.Size = new System.Drawing.Size(0, 25);
            this.ucTotalPriceView1.ValueLabel.TabIndex = 5;
            this.ucTotalPriceView1.Visible = false;
            // 
            // frmSupporting
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 404);
            this.Controls.Add(this.ucTotalPriceView1);
            this.Controls.Add(this.ucDoubleLineView1);
            this.Controls.Add(this.webBrowser1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSupporting";
            this.Text = "frmSupporting";
            this.Load += new System.EventHandler(this.frmSupporting_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private UserControls.ucDoubleLineView ucDoubleLineView1;
        private UserControls.ucTotalPriceView ucTotalPriceView1;
    }
}