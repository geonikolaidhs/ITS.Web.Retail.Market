namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class SelectDocumentReportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDocumentReportForm));
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lookUpEditDocumentReports = new DevExpress.XtraEditors.LookUpEdit();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonOK = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroupMain = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemReports = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItemBottom = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItemTop = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditDocumentReports.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemReports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTop)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlItem2
            // 
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 93);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1, 149);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(50, 20);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lookUpEditDocumentReports);
            this.layoutControl1.Controls.Add(this.simpleButtonCancel);
            this.layoutControl1.Controls.Add(this.simpleButtonOK);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(514, 63, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroupMain;
            this.layoutControl1.Size = new System.Drawing.Size(284, 262);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lookUpEditDocumentReports
            // 
            this.lookUpEditDocumentReports.Location = new System.Drawing.Point(12, 133);
            this.lookUpEditDocumentReports.Name = "lookUpEditDocumentReports";
            this.lookUpEditDocumentReports.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lookUpEditDocumentReports.Properties.NullText = "";
            this.lookUpEditDocumentReports.Size = new System.Drawing.Size(260, 20);
            this.lookUpEditDocumentReports.StyleController = this.layoutControl1;
            this.lookUpEditDocumentReports.TabIndex = 6;
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.simpleButtonCancel.Location = new System.Drawing.Point(143, 157);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(129, 38);
            this.simpleButtonCancel.StyleController = this.layoutControl1;
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "Cancel";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // simpleButtonOK
            // 
            this.simpleButtonOK.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Ok_32;
            this.simpleButtonOK.Location = new System.Drawing.Point(12, 157);
            this.simpleButtonOK.Name = "simpleButtonOK";
            this.simpleButtonOK.Size = new System.Drawing.Size(127, 38);
            this.simpleButtonOK.StyleController = this.layoutControl1;
            this.simpleButtonOK.TabIndex = 4;
            this.simpleButtonOK.Text = "OK";
            this.simpleButtonOK.Click += new System.EventHandler(this.simpleButtonOK_Click);
            // 
            // layoutControlGroupMain
            // 
            this.layoutControlGroupMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupMain.GroupBordersVisible = false;
            this.layoutControlGroupMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItemReports,
            this.emptySpaceItemBottom,
            this.emptySpaceItemTop});
            this.layoutControlGroupMain.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupMain.Name = "Root";
            this.layoutControlGroupMain.Size = new System.Drawing.Size(284, 262);
            this.layoutControlGroupMain.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.simpleButtonOK;
            this.SetIsRequired(this.layoutControlItem1, false);
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 145);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(131, 42);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButtonCancel;
            this.SetIsRequired(this.layoutControlItem3, false);
            this.layoutControlItem3.Location = new System.Drawing.Point(131, 145);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(133, 42);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItemReports
            // 
            this.layoutControlItemReports.Control = this.lookUpEditDocumentReports;
            this.SetIsRequired(this.layoutControlItemReports, false);
            this.layoutControlItemReports.Location = new System.Drawing.Point(0, 105);
            this.layoutControlItemReports.Name = "layoutControlItemReports";
            this.layoutControlItemReports.Size = new System.Drawing.Size(264, 40);
            this.layoutControlItemReports.Text = "Reports";
            this.layoutControlItemReports.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemReports.TextSize = new System.Drawing.Size(38, 13);
            // 
            // emptySpaceItemBottom
            // 
            this.emptySpaceItemBottom.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItemBottom, false);
            this.emptySpaceItemBottom.Location = new System.Drawing.Point(0, 187);
            this.emptySpaceItemBottom.Name = "emptySpaceItemBottom";
            this.emptySpaceItemBottom.Size = new System.Drawing.Size(264, 55);
            this.emptySpaceItemBottom.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItemTop
            // 
            this.emptySpaceItemTop.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItemTop, false);
            this.emptySpaceItemTop.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItemTop.Name = "emptySpaceItemTop";
            this.emptySpaceItemTop.Size = new System.Drawing.Size(264, 105);
            this.emptySpaceItemTop.TextSize = new System.Drawing.Size(0, 0);
            // 
            // SelectDocumentReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(318, 326);
            this.MinimumSize = new System.Drawing.Size(318, 326);
            this.Name = "SelectDocumentReportForm";
            this.Text = "SelectDocumentReportForm";
            this.Shown += new System.EventHandler(this.SelectDocumentReportForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditDocumentReports.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemReports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupMain;
        private DevExpress.XtraEditors.LookUpEdit lookUpEditDocumentReports;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.SimpleButton simpleButtonOK;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemReports;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemBottom;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemTop;




    }
}