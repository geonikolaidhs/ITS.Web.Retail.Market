namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class ImportForm
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
            if (disposing && this.WritableUnitOfWork != null)
            {
                this.WritableUnitOfWork.Dispose();
                this.WritableUnitOfWork = null;
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnPerformImport = new DevExpress.XtraEditors.SimpleButton();
            this.lueSuppliderImportFilesSet = new DevExpress.XtraEditors.LookUpEdit();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lueFile = new DevExpress.XtraEditors.ButtonEdit();
            this.lueEncoding = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemPreview = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemImport = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcFile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcEncoding = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSupplierImportFilesSet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPreview = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueSuppliderImportFilesSet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueEncoding.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEncoding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSupplierImportFilesSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnOk);
            this.layoutControl1.Controls.Add(this.btnPerformImport);
            this.layoutControl1.Controls.Add(this.lueSuppliderImportFilesSet);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.lueFile);
            this.layoutControl1.Controls.Add(this.lueEncoding);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(530, 427, 925, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1008, 730);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.btnCancel.Location = new System.Drawing.Point(672, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(324, 38);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "@@Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Binoculars_32;
            this.btnOk.Location = new System.Drawing.Point(342, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(326, 38);
            this.btnOk.StyleController = this.layoutControl1;
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "@@Preview";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnPerformImport
            // 
            this.btnPerformImport.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Import_32;
            this.btnPerformImport.Location = new System.Drawing.Point(12, 12);
            this.btnPerformImport.Name = "btnPerformImport";
            this.btnPerformImport.Size = new System.Drawing.Size(326, 38);
            this.btnPerformImport.StyleController = this.layoutControl1;
            this.btnPerformImport.TabIndex = 11;
            this.btnPerformImport.Text = "@@Import";
            this.btnPerformImport.Click += new System.EventHandler(this.btnPerformImport_Click);
            // 
            // lueSuppliderImportFilesSet
            // 
            this.lueSuppliderImportFilesSet.Location = new System.Drawing.Point(12, 70);
            this.lueSuppliderImportFilesSet.Name = "lueSuppliderImportFilesSet";
            this.lueSuppliderImportFilesSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueSuppliderImportFilesSet.Size = new System.Drawing.Size(326, 20);
            this.lueSuppliderImportFilesSet.StyleController = this.layoutControl1;
            this.lueSuppliderImportFilesSet.TabIndex = 5;
            // 
            // gridControl1
            // 
            gridLevelNode1.RelationName = "Level1";
            gridLevelNode2.RelationName = "Level2";
            this.gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1,
            gridLevelNode2});
            this.gridControl1.Location = new System.Drawing.Point(12, 110);
            this.gridControl1.LookAndFeel.SkinName = "iMaginary";
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(984, 608);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.Editable = false;
            // 
            // lueFile
            // 
            this.lueFile.Location = new System.Drawing.Point(342, 70);
            this.lueFile.Name = "lueFile";
            this.lueFile.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.lueFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.lueFile.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.lueFile.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.lueFile.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.lueFile.Size = new System.Drawing.Size(326, 20);
            this.lueFile.StyleController = this.layoutControl1;
            this.lueFile.TabIndex = 7;
            this.lueFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueFile_ButtonClick);
            // 
            // lueEncoding
            // 
            this.lueEncoding.Location = new System.Drawing.Point(672, 70);
            this.lueEncoding.Name = "lueEncoding";
            this.lueEncoding.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueEncoding.Properties.NullText = "";
            this.lueEncoding.Properties.PopupSizeable = false;
            this.lueEncoding.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueEncoding.Size = new System.Drawing.Size(324, 20);
            this.lueEncoding.StyleController = this.layoutControl1;
            this.lueEncoding.TabIndex = 10;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemPreview,
            this.layoutControlItemImport,
            this.layoutControlItemCancel,
            this.lcFile,
            this.lcEncoding,
            this.lcSupplierImportFilesSet,
            this.lcPreview});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1008, 730);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItemPreview
            // 
            this.layoutControlItemPreview.Control = this.btnOk;
            this.SetIsRequired(this.layoutControlItemPreview, false);
            this.layoutControlItemPreview.Location = new System.Drawing.Point(330, 0);
            this.layoutControlItemPreview.Name = "layoutControlItemPreview";
            this.layoutControlItemPreview.Size = new System.Drawing.Size(330, 42);
            this.layoutControlItemPreview.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemPreview.TextVisible = false;
            // 
            // layoutControlItemImport
            // 
            this.layoutControlItemImport.Control = this.btnPerformImport;
            this.SetIsRequired(this.layoutControlItemImport, false);
            this.layoutControlItemImport.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemImport.Name = "layoutControlItemImport";
            this.layoutControlItemImport.Size = new System.Drawing.Size(330, 42);
            this.layoutControlItemImport.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemImport.TextVisible = false;
            // 
            // layoutControlItemCancel
            // 
            this.layoutControlItemCancel.Control = this.btnCancel;
            this.SetIsRequired(this.layoutControlItemCancel, false);
            this.layoutControlItemCancel.Location = new System.Drawing.Point(660, 0);
            this.layoutControlItemCancel.Name = "layoutControlItemCancel";
            this.layoutControlItemCancel.Size = new System.Drawing.Size(328, 42);
            this.layoutControlItemCancel.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemCancel.TextVisible = false;
            // 
            // lcFile
            // 
            this.lcFile.Control = this.lueFile;
            this.SetIsRequired(this.lcFile, false);
            this.lcFile.Location = new System.Drawing.Point(330, 42);
            this.lcFile.Name = "lcFile";
            this.lcFile.Size = new System.Drawing.Size(330, 40);
            this.lcFile.Text = "@@File";
            this.lcFile.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcFile.TextSize = new System.Drawing.Size(127, 13);
            // 
            // lcEncoding
            // 
            this.lcEncoding.Control = this.lueEncoding;
            this.SetIsRequired(this.lcEncoding, false);
            this.lcEncoding.Location = new System.Drawing.Point(660, 42);
            this.lcEncoding.Name = "lcEncoding";
            this.lcEncoding.Size = new System.Drawing.Size(328, 40);
            this.lcEncoding.Text = "@@Locale";
            this.lcEncoding.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcEncoding.TextSize = new System.Drawing.Size(127, 13);
            // 
            // lcSupplierImportFilesSet
            // 
            this.lcSupplierImportFilesSet.Control = this.lueSuppliderImportFilesSet;
            this.SetIsRequired(this.lcSupplierImportFilesSet, false);
            this.lcSupplierImportFilesSet.Location = new System.Drawing.Point(0, 42);
            this.lcSupplierImportFilesSet.Name = "lcSupplierImportFilesSet";
            this.lcSupplierImportFilesSet.Size = new System.Drawing.Size(330, 40);
            this.lcSupplierImportFilesSet.Text = "@@SupplierImportFilesSet";
            this.lcSupplierImportFilesSet.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcSupplierImportFilesSet.TextSize = new System.Drawing.Size(127, 13);
            // 
            // lcPreview
            // 
            this.lcPreview.Control = this.gridControl1;
            this.SetIsRequired(this.lcPreview, false);
            this.lcPreview.Location = new System.Drawing.Point(0, 82);
            this.lcPreview.Name = "lcPreview";
            this.lcPreview.Size = new System.Drawing.Size(988, 628);
            this.lcPreview.Text = "@@Preview";
            this.lcPreview.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPreview.TextSize = new System.Drawing.Size(127, 13);
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "ImportForm";
            this.Text = "@@ImportForm";
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.Shown += new System.EventHandler(this.ImportForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueSuppliderImportFilesSet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueEncoding.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEncoding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSupplierImportFilesSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.LookUpEdit lueSuppliderImportFilesSet;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lcSupplierImportFilesSet;
        private DevExpress.XtraLayout.LayoutControlItem lcFile;
        private DevExpress.XtraLayout.LayoutControlItem lcPreview;
        private DevExpress.XtraEditors.ButtonEdit lueFile;
        private DevExpress.XtraEditors.LookUpEdit lueEncoding;
        private DevExpress.XtraLayout.LayoutControlItem lcEncoding;
        private DevExpress.XtraEditors.SimpleButton btnPerformImport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemPreview;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemImport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCancel;
    }
}