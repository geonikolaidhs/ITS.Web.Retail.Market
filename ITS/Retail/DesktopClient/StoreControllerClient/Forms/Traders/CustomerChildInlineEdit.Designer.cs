namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class CustomerChildInlineEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerChildInlineEdit));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelCustomerChild = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveCustomerChild = new DevExpress.XtraEditors.SimpleButton();
            this.lueChildSex = new DevExpress.XtraEditors.LookUpEdit();
            this.dtChildBirthDate = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcChildSex = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcChildBirthDate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueChildSex.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtChildBirthDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtChildBirthDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChildSex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChildBirthDate)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.lueChildSex);
            this.layoutControl1.Controls.Add(this.dtChildBirthDate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(314, 150);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.btnCancelCustomerChild);
            this.panelControl1.Controls.Add(this.btnSaveCustomerChild);
            this.panelControl1.Location = new System.Drawing.Point(12, 12);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(290, 46);
            this.panelControl1.TabIndex = 10;
            // 
            // btnCancelCustomerChild
            // 
            this.btnCancelCustomerChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelCustomerChild.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancelCustomerChild.Location = new System.Drawing.Point(185, 3);
            this.btnCancelCustomerChild.Name = "btnCancelCustomerChild";
            this.btnCancelCustomerChild.Size = new System.Drawing.Size(101, 38);
            this.btnCancelCustomerChild.TabIndex = 1;
            this.btnCancelCustomerChild.Text = "@@Cancel";
            this.btnCancelCustomerChild.Click += new System.EventHandler(this.btnCancelCustomerChild_Click);
            // 
            // btnSaveCustomerChild
            // 
            this.btnSaveCustomerChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCustomerChild.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.btnSaveCustomerChild.Location = new System.Drawing.Point(69, 3);
            this.btnSaveCustomerChild.Name = "btnSaveCustomerChild";
            this.btnSaveCustomerChild.Size = new System.Drawing.Size(110, 38);
            this.btnSaveCustomerChild.TabIndex = 2;
            this.btnSaveCustomerChild.Text = "@@Save";
            this.btnSaveCustomerChild.Click += new System.EventHandler(this.btnSaveCustomerChild_Click);
            // 
            // lueChildSex
            // 
            this.lueChildSex.Location = new System.Drawing.Point(12, 78);
            this.lueChildSex.Name = "lueChildSex";
            this.lueChildSex.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueChildSex.Size = new System.Drawing.Size(290, 20);
            this.lueChildSex.StyleController = this.layoutControl1;
            this.lueChildSex.TabIndex = 5;
            // 
            // dtChildBirthDate
            // 
            this.dtChildBirthDate.EditValue = null;
            this.dtChildBirthDate.Location = new System.Drawing.Point(12, 118);
            this.dtChildBirthDate.Name = "dtChildBirthDate";
            this.dtChildBirthDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtChildBirthDate.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
            this.dtChildBirthDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtChildBirthDate.Properties.DisplayFormat.FormatString = "";
            this.dtChildBirthDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtChildBirthDate.Properties.EditFormat.FormatString = "";
            this.dtChildBirthDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtChildBirthDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtChildBirthDate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtChildBirthDate.Size = new System.Drawing.Size(290, 20);
            this.dtChildBirthDate.StyleController = this.layoutControl1;
            this.dtChildBirthDate.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcChildSex,
            this.lcChildBirthDate});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(314, 150);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.SetIsRequired(this.layoutControlItem1, false);
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(294, 50);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcChildSex
            // 
            this.lcChildSex.Control = this.lueChildSex;
            this.lcChildSex.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcChildSex, false);
            this.lcChildSex.Location = new System.Drawing.Point(0, 50);
            this.lcChildSex.Name = "lcChildSex";
            this.lcChildSex.Size = new System.Drawing.Size(294, 40);
            this.lcChildSex.Text = "@@Sex";
            this.lcChildSex.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcChildSex.TextSize = new System.Drawing.Size(65, 13);
            // 
            // lcChildBirthDate
            // 
            this.lcChildBirthDate.Control = this.dtChildBirthDate;
            this.lcChildBirthDate.CustomizationFormText = "@@Code";
            this.SetIsRequired(this.lcChildBirthDate, false);
            this.lcChildBirthDate.Location = new System.Drawing.Point(0, 90);
            this.lcChildBirthDate.Name = "lcChildBirthDate";
            this.lcChildBirthDate.Size = new System.Drawing.Size(294, 40);
            this.lcChildBirthDate.Text = "@@BirthDate";
            this.lcChildBirthDate.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcChildBirthDate.TextSize = new System.Drawing.Size(65, 13);
            // 
            // CustomerChildInlineEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 150);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "CustomerChildInlineEdit";
            this.Text = "@@Edit";
            this.Load += new System.EventHandler(this.CustomerChildInlineEdit_Load);
            this.Shown += new System.EventHandler(this.CustomerChildInlineEdit_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueChildSex.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtChildBirthDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtChildBirthDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChildSex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChildBirthDate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LookUpEdit lueChildSex;
        private DevExpress.XtraLayout.LayoutControlItem lcChildSex;
        private DevExpress.XtraLayout.LayoutControlItem lcChildBirthDate;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancelCustomerChild;
        private DevExpress.XtraEditors.SimpleButton btnSaveCustomerChild;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.DateEdit dtChildBirthDate;
    }
}