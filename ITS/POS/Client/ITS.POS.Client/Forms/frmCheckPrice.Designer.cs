using System.Resources;
using ITS.POS.Resources;
namespace ITS.POS.Client
{
    partial class frmCheckPrice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ResourceManager LocRM = new ResourceManager("POSClientResources", typeof(POSClientResources).Assembly);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCheckPrice));
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.edtVatCategory = new DevExpress.XtraEditors.TextEdit();
            this.lblVatCategory = new DevExpress.XtraEditors.LabelControl();
            this.edtCode = new DevExpress.XtraEditors.TextEdit();
            this.lblCode = new DevExpress.XtraEditors.LabelControl();
            this.edtPrice = new DevExpress.XtraEditors.TextEdit();
            this.lblPrice = new DevExpress.XtraEditors.LabelControl();
            this.medtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cbIsActive = new System.Windows.Forms.CheckBox();
            this.edtItemCodeSearch = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            ((System.ComponentModel.ISupportInitialize)(this.edtVatCategory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.medtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtItemCodeSearch.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnClose.Location = new System.Drawing.Point(158, 253);
            this.btnClose.LookAndFeel.SkinName = "Metropolis";
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 59);
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // edtVatCategory
            // 
            this.edtVatCategory.EditValue = "";
            this.edtVatCategory.Location = new System.Drawing.Point(211, 163);
            this.edtVatCategory.Name = "edtVatCategory";
            this.edtVatCategory.Properties.AllowFocused = false;
            this.edtVatCategory.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.edtVatCategory.Properties.Appearance.Options.UseFont = true;
            this.edtVatCategory.Properties.Appearance.Options.UseTextOptions = true;
            this.edtVatCategory.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtVatCategory.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtVatCategory.Properties.DisplayFormat.FormatString = "c2";
            this.edtVatCategory.Properties.EditFormat.FormatString = "c2";
            this.edtVatCategory.Properties.ReadOnly = true;
            this.edtVatCategory.Properties.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.edtPrice_Properties_ParseEditValue);
            this.edtVatCategory.Size = new System.Drawing.Size(194, 30);
            this.edtVatCategory.TabIndex = 10;
            this.edtVatCategory.TabStop = false;
            // 
            // lblVatCategory
            // 
            this.lblVatCategory.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblVatCategory.Location = new System.Drawing.Point(211, 139);
            this.lblVatCategory.LookAndFeel.SkinName = "Metropolis";
            this.lblVatCategory.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblVatCategory.Name = "lblVatCategory";
            this.lblVatCategory.Size = new System.Drawing.Size(101, 18);
            this.lblVatCategory.TabIndex = 9;
            this.lblVatCategory.Text = "Κατηγορία ΦΠΑ";
            // 
            // edtCode
            // 
            this.edtCode.EditValue = "";
            this.edtCode.Location = new System.Drawing.Point(12, 163);
            this.edtCode.Name = "edtCode";
            this.edtCode.Properties.AllowFocused = false;
            this.edtCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.edtCode.Properties.Appearance.Options.UseFont = true;
            this.edtCode.Properties.Appearance.Options.UseTextOptions = true;
            this.edtCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtCode.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtCode.Properties.DisplayFormat.FormatString = "c2";
            this.edtCode.Properties.EditFormat.FormatString = "c2";
            this.edtCode.Properties.ReadOnly = true;
            this.edtCode.Properties.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.edtPrice_Properties_ParseEditValue);
            this.edtCode.Size = new System.Drawing.Size(194, 30);
            this.edtCode.TabIndex = 8;
            this.edtCode.TabStop = false;
            // 
            // lblCode
            // 
            this.lblCode.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblCode.Location = new System.Drawing.Point(12, 139);
            this.lblCode.LookAndFeel.SkinName = "Metropolis";
            this.lblCode.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(53, 18);
            this.lblCode.TabIndex = 7;
            this.lblCode.Text = "Κωδικός";
            // 
            // edtPrice
            // 
            this.edtPrice.EditValue = "";
            this.edtPrice.Location = new System.Drawing.Point(265, 217);
            this.edtPrice.Name = "edtPrice";
            this.edtPrice.Properties.AllowFocused = false;
            this.edtPrice.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.edtPrice.Properties.Appearance.Options.UseFont = true;
            this.edtPrice.Properties.Appearance.Options.UseTextOptions = true;
            this.edtPrice.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtPrice.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtPrice.Properties.DisplayFormat.FormatString = "c2";
            this.edtPrice.Properties.EditFormat.FormatString = "c2";
            this.edtPrice.Properties.ReadOnly = true;
            this.edtPrice.Properties.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.edtPrice_Properties_ParseEditValue);
            this.edtPrice.Size = new System.Drawing.Size(140, 30);
            this.edtPrice.TabIndex = 6;
            this.edtPrice.TabStop = false;
            // 
            // lblPrice
            // 
            this.lblPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(211, 220);
            this.lblPrice.LookAndFeel.SkinName = "Metropolis";
            this.lblPrice.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(48, 23);
            this.lblPrice.TabIndex = 5;
            this.lblPrice.Text = "Τιμή ";
            // 
            // medtDescription
            // 
            this.medtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.medtDescription.EditValue = "";
            this.medtDescription.Location = new System.Drawing.Point(12, 89);
            this.medtDescription.Name = "medtDescription";
            this.medtDescription.Properties.AllowFocused = false;
            this.medtDescription.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.medtDescription.Properties.Appearance.Options.UseFont = true;
            this.medtDescription.Properties.ReadOnly = true;
            this.medtDescription.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.medtDescription.Size = new System.Drawing.Size(393, 44);
            this.medtDescription.TabIndex = 4;
            this.medtDescription.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Italic);
            this.lblDescription.Location = new System.Drawing.Point(12, 65);
            this.lblDescription.LookAndFeel.SkinName = "Metropolis";
            this.lblDescription.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(72, 18);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Description";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl1.Location = new System.Drawing.Point(12, 14);
            this.labelControl1.LookAndFeel.SkinName = "Metropolis";
            this.labelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(173, 19);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Κωδικός/Barcode Είδους";
            // 
            // cbIsActive
            // 
            this.cbIsActive.AutoCheck = false;
            this.cbIsActive.AutoSize = true;
            this.cbIsActive.BackColor = System.Drawing.Color.Transparent;
            this.cbIsActive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cbIsActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbIsActive.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cbIsActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(40)))), ((int)(((byte)(0)))));
            this.cbIsActive.Location = new System.Drawing.Point(12, 220);
            this.cbIsActive.Name = "cbIsActive";
            this.cbIsActive.Size = new System.Drawing.Size(85, 26);
            this.cbIsActive.TabIndex = 11;
            this.cbIsActive.TabStop = false;
            this.cbIsActive.Text = "Ενεργό";
            this.cbIsActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbIsActive.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.cbIsActive.UseVisualStyleBackColor = false;
            // 
            // edtItemCodeSearch
            // 
            this.edtItemCodeSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtItemCodeSearch.AutoHideTouchPad = false;
            this.edtItemCodeSearch.Location = new System.Drawing.Point(12, 33);
            this.edtItemCodeSearch.Name = "edtItemCodeSearch";
            this.edtItemCodeSearch.PoleDisplayName = "";
            this.edtItemCodeSearch.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtItemCodeSearch.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtItemCodeSearch.Properties.Appearance.Options.UseBackColor = true;
            this.edtItemCodeSearch.Properties.Appearance.Options.UseFont = true;
            this.edtItemCodeSearch.Size = new System.Drawing.Size(393, 26);
            this.edtItemCodeSearch.TabIndex = 1;
            this.edtItemCodeSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtItemCode_KeyDown);
            this.edtItemCodeSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edtItemCode_KeyPress);
            this.edtItemCodeSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtItemCode_KeyUp);
            this.edtItemCodeSearch.Leave += new System.EventHandler(this.edtItemCode_Leave);
            // 
            // frmCheckPrice
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Tile;
            this.BackgroundImageStore = global::ITS.POS.Client.Properties.Resources.BackgroundImage;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(417, 319);
            this.Controls.Add(this.cbIsActive);
            this.Controls.Add(this.edtVatCategory);
            this.Controls.Add(this.lblVatCategory);
            this.Controls.Add(this.edtCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.edtPrice);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.medtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.edtItemCodeSearch);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmCheckPrice";
            this.Text = "frmCheckPrice";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCheckPrice_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCheckPrice_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.edtVatCategory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.medtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtItemCodeSearch.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnClose;
        private ITS.POS.Client.UserControls.ucTouchFriendlyInput edtItemCodeSearch;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.MemoEdit medtDescription;
        private DevExpress.XtraEditors.LabelControl lblPrice;
        private DevExpress.XtraEditors.TextEdit edtPrice;
        private DevExpress.XtraEditors.TextEdit edtCode;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraEditors.TextEdit edtVatCategory;
        private DevExpress.XtraEditors.LabelControl lblVatCategory;
        private System.Windows.Forms.CheckBox cbIsActive;
    }
}