namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class AddressPhoneInlineEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressPhoneInlineEdit));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelPhone = new DevExpress.XtraEditors.SimpleButton();
            this.btnSavePhone = new DevExpress.XtraEditors.SimpleButton();
            this.txtPhoneNumber = new DevExpress.XtraEditors.TextEdit();
            this.luePhoneType = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPhoneNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPhoneType = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhoneNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePhoneType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPhoneNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPhoneType)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.txtPhoneNumber);
            this.layoutControl1.Controls.Add(this.luePhoneType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(314, 169);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btnCancelPhone);
            this.panelControl1.Controls.Add(this.btnSavePhone);
            this.panelControl1.Location = new System.Drawing.Point(22, 22);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(270, 45);
            this.panelControl1.TabIndex = 10;
            // 
            // btnCancelPhone
            // 
            this.btnCancelPhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelPhone.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancelPhone.Location = new System.Drawing.Point(164, 3);
            this.btnCancelPhone.Name = "btnCancelPhone";
            this.btnCancelPhone.Size = new System.Drawing.Size(101, 38);
            this.btnCancelPhone.TabIndex = 1;
            this.btnCancelPhone.Text = "@@Cancel";
            this.btnCancelPhone.Click += new System.EventHandler(this.btnCancelPhone_Click);
            // 
            // btnSavePhone
            // 
            this.btnSavePhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePhone.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.btnSavePhone.Location = new System.Drawing.Point(44, 3);
            this.btnSavePhone.Name = "btnSavePhone";
            this.btnSavePhone.Size = new System.Drawing.Size(110, 38);
            this.btnSavePhone.TabIndex = 2;
            this.btnSavePhone.Text = "@@Save";
            this.btnSavePhone.Click += new System.EventHandler(this.btnSavePhone_Click);
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Location = new System.Drawing.Point(22, 87);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(270, 20);
            this.txtPhoneNumber.StyleController = this.layoutControl1;
            this.txtPhoneNumber.TabIndex = 5;
            // 
            // luePhoneType
            // 
            this.luePhoneType.Location = new System.Drawing.Point(22, 127);
            this.luePhoneType.Name = "luePhoneType";
            this.luePhoneType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.luePhoneType.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.luePhoneType.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.luePhoneType.Size = new System.Drawing.Size(270, 20);
            this.luePhoneType.StyleController = this.layoutControl1;
            this.luePhoneType.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.Root});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(314, 169);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // Root
            // 
            this.Root.CustomizationFormText = "Root";
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcPhoneNumber,
            this.lcPhoneType});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(294, 149);
            this.Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.SetIsRequired(this.layoutControlItem1, false);
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(274, 49);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcPhoneNumber
            // 
            this.lcPhoneNumber.Control = this.txtPhoneNumber;
            this.lcPhoneNumber.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcPhoneNumber, false);
            this.lcPhoneNumber.Location = new System.Drawing.Point(0, 49);
            this.lcPhoneNumber.Name = "lcPhoneNumber";
            this.lcPhoneNumber.Size = new System.Drawing.Size(274, 40);
            this.lcPhoneNumber.Text = "@@Number";
            this.lcPhoneNumber.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPhoneNumber.TextSize = new System.Drawing.Size(74, 13);
            // 
            // lcPhoneType
            // 
            this.lcPhoneType.Control = this.luePhoneType;
            this.lcPhoneType.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcPhoneType, false);
            this.lcPhoneType.Location = new System.Drawing.Point(0, 89);
            this.lcPhoneType.Name = "lcPhoneType";
            this.lcPhoneType.Size = new System.Drawing.Size(274, 40);
            this.lcPhoneType.Text = "@@PhoneType";
            this.lcPhoneType.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPhoneType.TextSize = new System.Drawing.Size(74, 13);
            // 
            // AddressPhoneInlineEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 169);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "AddressPhoneInlineEdit";
            this.Text = "@@Edit";
            this.Load += new System.EventHandler(this.AddressPhoneInlineEdit_Load);
            this.Shown += new System.EventHandler(this.AddressPhoneInlineEdit_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPhoneNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePhoneType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPhoneNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPhoneType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancelPhone;
        private DevExpress.XtraEditors.SimpleButton btnSavePhone;
        private DevExpress.XtraEditors.TextEdit txtPhoneNumber;
        private DevExpress.XtraEditors.LookUpEdit luePhoneType;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lcPhoneType;
        private DevExpress.XtraLayout.LayoutControlItem lcPhoneNumber;
    }
}