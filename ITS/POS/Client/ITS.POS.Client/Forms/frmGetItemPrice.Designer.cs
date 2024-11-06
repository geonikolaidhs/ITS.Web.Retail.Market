namespace ITS.POS.Client.Forms
{
    partial class frmGetItemPrice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGetItemPrice));
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.txtItemName = new DevExpress.XtraEditors.TextEdit();
            this.labelItem = new DevExpress.XtraEditors.LabelControl();
            this.txtItemPrice = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.labelInsertPrice = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemPrice.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnClose.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnClose.Location = new System.Drawing.Point(219, 141);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(114, 59);
            this.btnClose.TabIndex = 7;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Ακύρωση";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(12, 38);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtItemName.Properties.Appearance.Options.UseFont = true;
            this.txtItemName.Properties.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(321, 30);
            this.txtItemName.TabIndex = 6;
            // 
            // labelItem
            // 
            this.labelItem.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelItem.Location = new System.Drawing.Point(12, 12);
            this.labelItem.LookAndFeel.SkinName = "Metropolis";
            this.labelItem.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelItem.Name = "labelItem";
            this.labelItem.Size = new System.Drawing.Size(39, 19);
            this.labelItem.TabIndex = 5;
            this.labelItem.Text = "Είδος";
            // 
            // txtItemPrice
            // 
            this.txtItemPrice.AutoHideTouchPad = true;
            this.txtItemPrice.Location = new System.Drawing.Point(12, 101);
            this.txtItemPrice.Name = "txtItemPrice";
            this.txtItemPrice.PoleDisplayName = "";
            this.txtItemPrice.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtItemPrice.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtItemPrice.Properties.Appearance.Options.UseBackColor = true;
            this.txtItemPrice.Properties.Appearance.Options.UseFont = true;
            this.txtItemPrice.Properties.Mask.EditMask = "c";
            this.txtItemPrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtItemPrice.Properties.Enter += new System.EventHandler(this.textEdit1_Properties_Enter);
            this.txtItemPrice.Size = new System.Drawing.Size(321, 30);
            this.txtItemPrice.TabIndex = 1;
            this.txtItemPrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemPrice_KeyDown);
            // 
            // labelInsertPrice
            // 
            this.labelInsertPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelInsertPrice.Location = new System.Drawing.Point(12, 74);
            this.labelInsertPrice.LookAndFeel.SkinName = "Metropolis";
            this.labelInsertPrice.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelInsertPrice.Name = "labelInsertPrice";
            this.labelInsertPrice.Size = new System.Drawing.Size(96, 19);
            this.labelInsertPrice.TabIndex = 3;
            this.labelInsertPrice.Text = "Εισάγετε τιμή";
            // 
            // frmGetItemPrice
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(346, 212);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.labelItem);
            this.Controls.Add(this.txtItemPrice);
            this.Controls.Add(this.labelInsertPrice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmGetItemPrice";
            this.Text = "frmGetItemPrice";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGetItemPrice_FormClosed);
            this.Load += new System.EventHandler(this.frmGetItemPrice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemPrice.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelInsertPrice;
        private ITS.POS.Client.UserControls.ucTouchFriendlyInput txtItemPrice;
        private DevExpress.XtraEditors.LabelControl labelItem;
        private DevExpress.XtraEditors.TextEdit txtItemName;
        private DevExpress.XtraEditors.SimpleButton btnClose;

    }
}