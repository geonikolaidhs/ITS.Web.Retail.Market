namespace ITS.POS.Client.Forms
{
    partial class frmWithdrawDeposit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWithdrawDeposit));
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.txtInput = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtTaxCode = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.listBoxReasons = new DevExpress.XtraEditors.ListBoxControl();
            this.btnPreviousReason = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextReason = new DevExpress.XtraEditors.SimpleButton();
            this.txtComment = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            ((System.ComponentModel.ISupportInitialize)(this.txtInput.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxReasons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(506, 371);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 70);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel.Location = new System.Drawing.Point(652, 371);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 70);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Ακύρωση";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl1.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(479, 252);
            this.labelControl1.LookAndFeel.SkinName = "Metropolis";
            this.labelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(184, 33);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "@@AMOUNT";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitle.Location = new System.Drawing.Point(6, 1);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(786, 54);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Title";
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.AutoHideTouchPad = true;
            this.txtInput.EditValue = "";
            this.txtInput.Location = new System.Drawing.Point(479, 291);
            this.txtInput.Name = "txtInput";
            this.txtInput.PoleDisplayName = "";
            this.txtInput.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtInput.Properties.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtInput.Properties.Appearance.Options.UseFont = true;
            this.txtInput.Properties.Appearance.Options.UseTextOptions = true;
            this.txtInput.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtInput.Properties.Mask.EditMask = "c";
            this.txtInput.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtInput.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtInput.Size = new System.Drawing.Size(313, 40);
            this.txtInput.TabIndex = 5;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // txtTaxCode
            // 
            this.txtTaxCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaxCode.AutoHideTouchPad = true;
            this.txtTaxCode.EditValue = "";
            this.txtTaxCode.EnterMoveNextControl = true;
            this.txtTaxCode.Location = new System.Drawing.Point(479, 101);
            this.txtTaxCode.Name = "txtTaxCode";
            this.txtTaxCode.PoleDisplayName = "";
            this.txtTaxCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtTaxCode.Properties.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtTaxCode.Properties.Appearance.Options.UseBackColor = true;
            this.txtTaxCode.Properties.Appearance.Options.UseFont = true;
            this.txtTaxCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtTaxCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtTaxCode.Properties.Mask.EditMask = "c";
            this.txtTaxCode.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtTaxCode.Size = new System.Drawing.Size(313, 40);
            this.txtTaxCode.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl2.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Location = new System.Drawing.Point(12, 61);
            this.labelControl2.LookAndFeel.SkinName = "Metropolis";
            this.labelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(184, 33);
            this.labelControl2.TabIndex = 9;
            this.labelControl2.Text = "@@REASON";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl3.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.Location = new System.Drawing.Point(479, 61);
            this.labelControl3.LookAndFeel.SkinName = "Metropolis";
            this.labelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(184, 33);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "@@TAX_CODE_OR_CODE";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl4.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelControl4.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Location = new System.Drawing.Point(479, 158);
            this.labelControl4.LookAndFeel.SkinName = "Metropolis";
            this.labelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(184, 33);
            this.labelControl4.TabIndex = 11;
            this.labelControl4.Text = "@@COMMENTS";
            // 
            // listBoxReasons
            // 
            this.listBoxReasons.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.listBoxReasons.Appearance.Options.UseFont = true;
            this.listBoxReasons.IncrementalSearch = true;
            this.listBoxReasons.Location = new System.Drawing.Point(12, 100);
            this.listBoxReasons.Name = "listBoxReasons";
            this.listBoxReasons.Size = new System.Drawing.Size(448, 267);
            this.listBoxReasons.TabIndex = 0;
            this.listBoxReasons.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxReasons_KeyDown);
            // 
            // btnPreviousReason
            // 
            this.btnPreviousReason.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnPreviousReason.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnPreviousReason.Appearance.Options.UseBackColor = true;
            this.btnPreviousReason.Appearance.Options.UseBorderColor = true;
            this.btnPreviousReason.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnPreviousReason.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousReason.Image")));
            this.btnPreviousReason.Location = new System.Drawing.Point(71, 373);
            this.btnPreviousReason.LookAndFeel.SkinName = "Metropolis";
            this.btnPreviousReason.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPreviousReason.Name = "btnPreviousReason";
            this.btnPreviousReason.Size = new System.Drawing.Size(159, 59);
            this.btnPreviousReason.TabIndex = 1;
            this.btnPreviousReason.Text = "@@PREVIOUS_REASON";
            this.btnPreviousReason.Click += new System.EventHandler(this.btnPreviousReason_Click);
            // 
            // btnNextReason
            // 
            this.btnNextReason.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnNextReason.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnNextReason.Appearance.Options.UseBackColor = true;
            this.btnNextReason.Appearance.Options.UseBorderColor = true;
            this.btnNextReason.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnNextReason.Image = ((System.Drawing.Image)(resources.GetObject("btnNextReason.Image")));
            this.btnNextReason.Location = new System.Drawing.Point(254, 373);
            this.btnNextReason.LookAndFeel.SkinName = "Metropolis";
            this.btnNextReason.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNextReason.Name = "btnNextReason";
            this.btnNextReason.Size = new System.Drawing.Size(163, 59);
            this.btnNextReason.TabIndex = 2;
            this.btnNextReason.Text = "@@NEXT_REASON";
            this.btnNextReason.Click += new System.EventHandler(this.btnNextReason_Click);
            // 
            // txtComment
            // 
            this.txtComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComment.AutoHideTouchPad = true;
            this.txtComment.EditValue = "";
            this.txtComment.EnterMoveNextControl = true;
            this.txtComment.Location = new System.Drawing.Point(479, 206);
            this.txtComment.Name = "txtComment";
            this.txtComment.PoleDisplayName = "";
            this.txtComment.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtComment.Properties.Appearance.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtComment.Properties.Appearance.Options.UseBackColor = true;
            this.txtComment.Properties.Appearance.Options.UseFont = true;
            this.txtComment.Properties.Appearance.Options.UseTextOptions = true;
            this.txtComment.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.txtComment.Properties.Mask.EditMask = "c";
            this.txtComment.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtComment.Size = new System.Drawing.Size(313, 40);
            this.txtComment.TabIndex = 4;
            // 
            // frmWithdrawDeposit
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(800, 455);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.btnPreviousReason);
            this.Controls.Add(this.btnNextReason);
            this.Controls.Add(this.listBoxReasons);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtTaxCode);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInput);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmWithdrawDeposit";
            this.Text = "frmWithdrawDeposit";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWithdrawDeposit_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.txtInput.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxReasons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.ucTouchFriendlyInput txtInput;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private UserControls.ucTouchFriendlyInput txtTaxCode;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.ListBoxControl listBoxReasons;
        public DevExpress.XtraEditors.SimpleButton btnPreviousReason;
        public DevExpress.XtraEditors.SimpleButton btnNextReason;
        private UserControls.ucTouchFriendlyInput txtComment;
    }
}