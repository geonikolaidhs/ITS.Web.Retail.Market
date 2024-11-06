namespace ITS.POS.Client.Forms
{
    partial class frmGroupButton
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGroupButton));
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCommands = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lblPages = new DevExpress.XtraEditors.LabelControl();
            this.btnNext = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrevious = new DevExpress.XtraEditors.SimpleButton();
            this.btnUp = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCommands)).BeginInit();
            this.pnlCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 461F));
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 39);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Size = new System.Drawing.Size(461, 256);
            this.tableLayout.TabIndex = 0;
            // 
            // pnlCommands
            // 
            this.pnlCommands.Controls.Add(this.btnCancel);
            this.pnlCommands.Controls.Add(this.lblPages);
            this.pnlCommands.Controls.Add(this.btnNext);
            this.pnlCommands.Controls.Add(this.btnPrevious);
            this.pnlCommands.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCommands.Location = new System.Drawing.Point(0, 295);
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(461, 72);
            this.pnlCommands.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCancel.Location = new System.Drawing.Point(102, 15);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(257, 55);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "@@CLOSE";
            // 
            // lblPages
            // 
            this.lblPages.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblPages.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPages.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPages.Location = new System.Drawing.Point(102, 2);
            this.lblPages.Name = "lblPages";
            this.lblPages.Size = new System.Drawing.Size(257, 13);
            this.lblPages.TabIndex = 4;
            this.lblPages.Text = "labelControl1";
            // 
            // btnNext
            // 
            this.btnNext.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnNext.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnNext.Appearance.Options.UseBackColor = true;
            this.btnNext.Appearance.Options.UseBorderColor = true;
            this.btnNext.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnNext.Location = new System.Drawing.Point(359, 2);
            this.btnNext.LookAndFeel.SkinName = "Metropolis";
            this.btnNext.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(100, 68);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "@@NEXT";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnPrevious.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnPrevious.Appearance.Options.UseBackColor = true;
            this.btnPrevious.Appearance.Options.UseBorderColor = true;
            this.btnPrevious.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnPrevious.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPrevious.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevious.Image")));
            this.btnPrevious.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnPrevious.Location = new System.Drawing.Point(2, 2);
            this.btnPrevious.LookAndFeel.SkinName = "Metropolis";
            this.btnPrevious.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(100, 68);
            this.btnPrevious.TabIndex = 3;
            this.btnPrevious.Text = "@@PREVIOUS";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnUp
            // 
            this.btnUp.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnUp.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnUp.Appearance.Options.UseBackColor = true;
            this.btnUp.Appearance.Options.UseBorderColor = true;
            this.btnUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnUp.Location = new System.Drawing.Point(0, 0);
            this.btnUp.LookAndFeel.SkinName = "Metropolis";
            this.btnUp.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(461, 39);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "@@UP";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // frmGroupButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(461, 367);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayout);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.pnlCommands);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmGroupButton";
            this.Text = "frmGroupButton";
            ((System.ComponentModel.ISupportInitialize)(this.pnlCommands)).EndInit();
            this.pnlCommands.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TableLayoutPanel tableLayout;
        public DevExpress.XtraEditors.PanelControl pnlCommands;
        public DevExpress.XtraEditors.SimpleButton btnNext;
        public DevExpress.XtraEditors.SimpleButton btnCancel;
        public DevExpress.XtraEditors.SimpleButton btnPrevious;
        public DevExpress.XtraEditors.SimpleButton btnUp;
        private DevExpress.XtraEditors.LabelControl lblPages;
    }
}