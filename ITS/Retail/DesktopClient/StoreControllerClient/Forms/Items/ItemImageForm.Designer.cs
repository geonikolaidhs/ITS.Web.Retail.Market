namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class ItemImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemImageForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButtonOk = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.imageEdit = new DevExpress.XtraEditors.ImageEdit();
            this.btnDeleteImage = new DevExpress.XtraEditors.SimpleButton();
            this.btnUploadImage = new DevExpress.XtraEditors.SimpleButton();
            this.txtItemImageDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtItemImageInfo = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item0 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.item1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcItemImageInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcItemImageDescription = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemImageDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemImageInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemImageInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemImageDescription)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.txtItemImageDescription);
            this.layoutControl1.Controls.Add(this.txtItemImageInfo);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(379, 242);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.simpleButtonOk);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.imageEdit);
            this.panelControl1.Controls.Add(this.btnDeleteImage);
            this.panelControl1.Controls.Add(this.btnUploadImage);
            this.panelControl1.Location = new System.Drawing.Point(32, 32);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(315, 98);
            this.panelControl1.TabIndex = 9;
            // 
            // simpleButtonOk
            // 
            this.simpleButtonOk.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Ok_32;
            this.simpleButtonOk.Location = new System.Drawing.Point(3, 5);
            this.simpleButtonOk.Name = "simpleButtonOk";
            this.simpleButtonOk.Size = new System.Drawing.Size(89, 38);
            this.simpleButtonOk.TabIndex = 5;
            this.simpleButtonOk.Text = "@@OK";
            this.simpleButtonOk.Visible = false;
            this.simpleButtonOk.Click += new System.EventHandler(this.simpleButtonOk_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(3, 55);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(50, 13);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "@@Image";
            // 
            // imageEdit
            // 
            this.imageEdit.Location = new System.Drawing.Point(0, 74);
            this.imageEdit.Name = "imageEdit";
            this.imageEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.imageEdit.Properties.ShowMenu = false;
            this.imageEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.imageEdit.Size = new System.Drawing.Size(315, 20);
            this.imageEdit.TabIndex = 3;
            // 
            // btnDeleteImage
            // 
            this.btnDeleteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteImage.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Delete_Sign_32;
            this.btnDeleteImage.Location = new System.Drawing.Point(209, 5);
            this.btnDeleteImage.Name = "btnDeleteImage";
            this.btnDeleteImage.Size = new System.Drawing.Size(101, 38);
            this.btnDeleteImage.TabIndex = 1;
            this.btnDeleteImage.Text = "@@Delete";
            this.btnDeleteImage.Click += new System.EventHandler(this.btnDeleteImage_Click);
            // 
            // btnUploadImage
            // 
            this.btnUploadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUploadImage.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Upload_2_32;
            this.btnUploadImage.Location = new System.Drawing.Point(98, 5);
            this.btnUploadImage.Name = "btnUploadImage";
            this.btnUploadImage.Size = new System.Drawing.Size(105, 38);
            this.btnUploadImage.TabIndex = 2;
            this.btnUploadImage.Text = "@@Upload";
            this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
            // 
            // txtItemImageDescription
            // 
            this.txtItemImageDescription.EditValue = "";
            this.txtItemImageDescription.Location = new System.Drawing.Point(32, 150);
            this.txtItemImageDescription.Name = "txtItemImageDescription";
            this.txtItemImageDescription.Size = new System.Drawing.Size(315, 20);
            this.txtItemImageDescription.StyleController = this.layoutControl1;
            this.txtItemImageDescription.TabIndex = 2;
            // 
            // txtItemImageInfo
            // 
            this.txtItemImageInfo.EditValue = "";
            this.txtItemImageInfo.Location = new System.Drawing.Point(32, 190);
            this.txtItemImageInfo.Name = "txtItemImageInfo";
            this.txtItemImageInfo.Size = new System.Drawing.Size(315, 20);
            this.txtItemImageInfo.StyleController = this.layoutControl1;
            this.txtItemImageInfo.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item0});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(379, 242);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // item0
            // 
            this.item0.CustomizationFormText = "Root";
            this.item0.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.item0.GroupBordersVisible = false;
            this.item0.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.item1});
            this.item0.Location = new System.Drawing.Point(0, 0);
            this.item0.Name = "item0";
            this.item0.Size = new System.Drawing.Size(359, 222);
            this.item0.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.item0.TextVisible = false;
            // 
            // item1
            // 
            this.item1.CustomizationFormText = "Root";
            this.item1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.item1.GroupBordersVisible = false;
            this.item1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcItemImageInfo,
            this.layoutControlItem2,
            this.lcItemImageDescription});
            this.item1.Location = new System.Drawing.Point(0, 0);
            this.item1.Name = "item1";
            this.item1.Size = new System.Drawing.Size(339, 202);
            this.item1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.item1.Text = "Root";
            this.item1.TextVisible = false;
            // 
            // lcItemImageInfo
            // 
            this.lcItemImageInfo.Control = this.txtItemImageInfo;
            this.lcItemImageInfo.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcItemImageInfo, false);
            this.lcItemImageInfo.Location = new System.Drawing.Point(0, 142);
            this.lcItemImageInfo.Name = "lcItemImageInfo";
            this.lcItemImageInfo.Size = new System.Drawing.Size(319, 40);
            this.lcItemImageInfo.Text = "@@Info";
            this.lcItemImageInfo.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcItemImageInfo.TextSize = new System.Drawing.Size(73, 13);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panelControl1;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(319, 102);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lcItemImageDescription
            // 
            this.lcItemImageDescription.Control = this.txtItemImageDescription;
            this.lcItemImageDescription.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcItemImageDescription, false);
            this.lcItemImageDescription.Location = new System.Drawing.Point(0, 102);
            this.lcItemImageDescription.Name = "lcItemImageDescription";
            this.lcItemImageDescription.Size = new System.Drawing.Size(319, 40);
            this.lcItemImageDescription.Text = "@@Description";
            this.lcItemImageDescription.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcItemImageDescription.TextSize = new System.Drawing.Size(73, 13);
            // 
            // ItemImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 242);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "ItemImageForm";
            this.Text = "ItemImageForm";
            this.Load += new System.EventHandler(this.ItemImageForm_Load);
            this.Shown += new System.EventHandler(this.ItemImageForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemImageDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemImageInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.item1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemImageInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemImageDescription)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ImageEdit imageEdit;
        private DevExpress.XtraEditors.SimpleButton btnDeleteImage;
        private DevExpress.XtraEditors.SimpleButton btnUploadImage;
        private DevExpress.XtraEditors.TextEdit txtItemImageDescription;
        private DevExpress.XtraEditors.TextEdit txtItemImageInfo;
        private DevExpress.XtraLayout.LayoutControlGroup item0;
        private DevExpress.XtraLayout.LayoutControlGroup item1;
        private DevExpress.XtraLayout.LayoutControlItem lcItemImageInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lcItemImageDescription;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButtonOk;
    }
}