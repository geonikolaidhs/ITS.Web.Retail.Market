namespace ITS.Retail.ScanTool
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.xpcScannedDocumentHeaders = new DevExpress.Xpo.XPCollection(this.components);
            this.grpEdit = new System.Windows.Forms.GroupBox();
            this.picture = new DevExpress.XtraEditors.PictureEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSavePrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnScan = new DevExpress.XtraEditors.SimpleButton();
            this.txtDate = new DevExpress.XtraEditors.DateEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtAmount = new DevExpress.XtraEditors.TextEdit();
            this.txt = new DevExpress.XtraEditors.TextEdit();
            this.txtVat = new DevExpress.XtraEditors.TextEdit();
            this.grpDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnSettings = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteUploaded = new DevExpress.XtraEditors.SimpleButton();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.btnUpload = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.DetailView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSupplierTaxCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.colDocumentNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDocumentIssueDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDocumentAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colScannedImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemImageEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageEdit();
            this.colUploaded = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.colValid = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.xpcScannedDocumentHeaders)).BeginInit();
            this.grpEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVat.Properties)).BeginInit();
            this.grpDocuments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // xpcScannedDocumentHeaders
            // 
            this.xpcScannedDocumentHeaders.ObjectType = typeof(ITS.Retail.ScanTool.Model.ScannedDocumentHeader);
            // 
            // grpEdit
            // 
            this.grpEdit.Controls.Add(this.picture);
            this.grpEdit.Controls.Add(this.btnCancel);
            this.grpEdit.Controls.Add(this.btnSavePrint);
            this.grpEdit.Controls.Add(this.btnScan);
            this.grpEdit.Controls.Add(this.txtDate);
            this.grpEdit.Controls.Add(this.labelControl4);
            this.grpEdit.Controls.Add(this.labelControl3);
            this.grpEdit.Controls.Add(this.labelControl2);
            this.grpEdit.Controls.Add(this.labelControl1);
            this.grpEdit.Controls.Add(this.txtAmount);
            this.grpEdit.Controls.Add(this.txt);
            this.grpEdit.Controls.Add(this.txtVat);
            this.grpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEdit.Enabled = false;
            this.grpEdit.Location = new System.Drawing.Point(0, 0);
            this.grpEdit.Name = "grpEdit";
            this.grpEdit.Size = new System.Drawing.Size(780, 610);
            this.grpEdit.TabIndex = 48;
            this.grpEdit.TabStop = false;
            this.grpEdit.Text = "Προσθήκη/ Επεξεργασία Παραστατικού";
            // 
            // picture
            // 
            this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picture.Location = new System.Drawing.Point(310, 19);
            this.picture.Name = "picture";
            this.picture.Properties.PictureInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            this.picture.Properties.PictureStoreMode = DevExpress.XtraEditors.Controls.PictureStoreMode.Image;
            this.picture.Properties.ReadOnly = true;
            this.picture.Properties.ShowScrollBars = true;
            this.picture.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            this.picture.Size = new System.Drawing.Size(464, 591);
            this.picture.TabIndex = 58;
            // 
            // btnCancel
            // 
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(158, 170);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(146, 39);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Α&κύρωση (Esc)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSavePrint
            // 
            this.btnSavePrint.Image = ((System.Drawing.Image)(resources.GetObject("btnSavePrint.Image")));
            this.btnSavePrint.Location = new System.Drawing.Point(7, 170);
            this.btnSavePrint.Name = "btnSavePrint";
            this.btnSavePrint.Size = new System.Drawing.Size(146, 39);
            this.btnSavePrint.TabIndex = 6;
            this.btnSavePrint.Text = "&Αποθήκευση (F9)";
            this.btnSavePrint.Click += new System.EventHandler(this.btnSavePrint_Click);
            // 
            // btnScan
            // 
            this.btnScan.Image = ((System.Drawing.Image)(resources.GetObject("btnScan.Image")));
            this.btnScan.Location = new System.Drawing.Point(7, 125);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(297, 39);
            this.btnScan.TabIndex = 5;
            this.btnScan.Text = "Σάρωση (F8)";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // txtDate
            // 
            this.txtDate.EditValue = null;
            this.txtDate.EnterMoveNextControl = true;
            this.txtDate.Location = new System.Drawing.Point(158, 71);
            this.txtDate.Name = "txtDate";
            this.txtDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDate.Properties.CalendarTimeProperties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.F4);
            this.txtDate.Properties.CalendarTimeProperties.PopupBorderStyle = DevExpress.XtraEditors.Controls.PopupBorderStyles.Default;
            this.txtDate.Size = new System.Drawing.Size(146, 20);
            this.txtDate.TabIndex = 3;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl4.Location = new System.Drawing.Point(6, 97);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(41, 18);
            this.labelControl4.TabIndex = 50;
            this.labelControl4.Text = "Ποσό";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl3.Location = new System.Drawing.Point(6, 71);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(87, 18);
            this.labelControl3.TabIndex = 51;
            this.labelControl3.Text = "Ημερομηνία";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl2.Location = new System.Drawing.Point(6, 45);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(133, 18);
            this.labelControl2.TabIndex = 52;
            this.labelControl2.Text = "Αρ. Παραστατικού";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelControl1.Location = new System.Drawing.Point(6, 19);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(95, 18);
            this.labelControl1.TabIndex = 53;
            this.labelControl1.Text = "ΑΦΜ Εκδότη";
            // 
            // txtAmount
            // 
            this.txtAmount.EditValue = 0D;
            this.txtAmount.EnterMoveNextControl = true;
            this.txtAmount.Location = new System.Drawing.Point(158, 97);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Properties.Appearance.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtAmount.Properties.Appearance.Options.UseFont = true;
            this.txtAmount.Properties.Appearance.Options.UseTextOptions = true;
            this.txtAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtAmount.Properties.DisplayFormat.FormatString = "c2";
            this.txtAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtAmount.Properties.EditFormat.FormatString = "c2";
            this.txtAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtAmount.Size = new System.Drawing.Size(146, 22);
            this.txtAmount.TabIndex = 4;
            // 
            // txt
            // 
            this.txt.EnterMoveNextControl = true;
            this.txt.Location = new System.Drawing.Point(158, 43);
            this.txt.Name = "txt";
            this.txt.Properties.Appearance.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txt.Properties.Appearance.Options.UseFont = true;
            this.txt.Size = new System.Drawing.Size(146, 22);
            this.txt.TabIndex = 2;
            // 
            // txtVat
            // 
            this.txtVat.EnterMoveNextControl = true;
            this.txtVat.Location = new System.Drawing.Point(158, 15);
            this.txtVat.Name = "txtVat";
            this.txtVat.Properties.Appearance.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtVat.Properties.Appearance.Options.UseFont = true;
            this.txtVat.Size = new System.Drawing.Size(146, 22);
            this.txtVat.TabIndex = 1;
            // 
            // grpDocuments
            // 
            this.grpDocuments.Controls.Add(this.btnScanAll);
            this.grpDocuments.Controls.Add(this.btnSettings);
            this.grpDocuments.Controls.Add(this.btnDelete);
            this.grpDocuments.Controls.Add(this.btnNew);
            this.grpDocuments.Controls.Add(this.btnDeleteUploaded);
            this.grpDocuments.Controls.Add(this.btnExit);
            this.grpDocuments.Controls.Add(this.btnUpload);
            this.grpDocuments.Controls.Add(this.gridControl1);
            this.grpDocuments.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpDocuments.Location = new System.Drawing.Point(0, 303);
            this.grpDocuments.MinimumSize = new System.Drawing.Size(0, 220);
            this.grpDocuments.Name = "grpDocuments";
            this.grpDocuments.Size = new System.Drawing.Size(780, 307);
            this.grpDocuments.TabIndex = 49;
            this.grpDocuments.TabStop = false;
            this.grpDocuments.Text = "Σαρωμένα Παραστατικά";
            // 
            // btnScanAll
            // 
            this.btnScanAll.Image = ((System.Drawing.Image)(resources.GetObject("btnScanAll.Image")));
            this.btnScanAll.Location = new System.Drawing.Point(310, 19);
            this.btnScanAll.Name = "btnScanAll";
            this.btnScanAll.Size = new System.Drawing.Size(169, 39);
            this.btnScanAll.TabIndex = 50;
            this.btnScanAll.Text = "Μαζική Σάρωση (F7)";
            this.btnScanAll.Click += new System.EventHandler(this.btnScanAll_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.Location = new System.Drawing.Point(632, 19);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(146, 39);
            this.btnSettings.TabIndex = 48;
            this.btnSettings.Text = "&Ρυθμίσεις";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(157, 19);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(145, 39);
            this.btnDelete.TabIndex = 49;
            this.btnDelete.Text = "&Διαγραφή (Del)";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.Location = new System.Drawing.Point(6, 19);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(145, 39);
            this.btnNew.TabIndex = 49;
            this.btnNew.Text = "&Νέα (F5)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDeleteUploaded
            // 
            this.btnDeleteUploaded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteUploaded.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteUploaded.Image")));
            this.btnDeleteUploaded.Location = new System.Drawing.Point(6, 259);
            this.btnDeleteUploaded.Name = "btnDeleteUploaded";
            this.btnDeleteUploaded.Size = new System.Drawing.Size(158, 39);
            this.btnDeleteUploaded.TabIndex = 45;
            this.btnDeleteUploaded.Text = "Διαγραφή επιτυχώντων";
            this.btnDeleteUploaded.Click += new System.EventHandler(this.btnDeleteUploaded_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.Location = new System.Drawing.Point(615, 259);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(165, 39);
            this.btnExit.TabIndex = 46;
            this.btnExit.Text = "Έξοδος";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click_1);
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.Location = new System.Drawing.Point(170, 259);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(133, 39);
            this.btnUpload.TabIndex = 47;
            this.btnUpload.Text = "Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.DataMember = null;
            this.gridControl1.DataSource = this.xpcScannedDocumentHeaders;
            this.gridControl1.Location = new System.Drawing.Point(5, 64);
            this.gridControl1.MainView = this.DetailView;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemImageEdit1,
            this.repositoryItemTextEdit1});
            this.gridControl1.Size = new System.Drawing.Size(773, 189);
            this.gridControl1.TabIndex = 44;
            this.gridControl1.UseEmbeddedNavigator = true;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.DetailView});
            // 
            // DetailView
            // 
            this.DetailView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSupplierTaxCode,
            this.colDocumentNumber,
            this.colDocumentIssueDate,
            this.colDocumentAmount,
            this.colScannedImage,
            this.colValid,
            this.colUploaded});
            this.DetailView.GridControl = this.gridControl1;
            this.DetailView.Name = "DetailView";
            this.DetailView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.DetailView.OptionsBehavior.Editable = false;
            this.DetailView.OptionsNavigation.AutoFocusNewRow = true;
            this.DetailView.OptionsNavigation.EnterMoveNextColumn = true;
            this.DetailView.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.DetailView_RowClick);
            // 
            // colSupplierTaxCode
            // 
            this.colSupplierTaxCode.Caption = "ΑΦΜ Εκδότη";
            this.colSupplierTaxCode.ColumnEdit = this.repositoryItemTextEdit1;
            this.colSupplierTaxCode.FieldName = "SupplierTaxCode";
            this.colSupplierTaxCode.Name = "colSupplierTaxCode";
            this.colSupplierTaxCode.Visible = true;
            this.colSupplierTaxCode.VisibleIndex = 0;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // colDocumentNumber
            // 
            this.colDocumentNumber.Caption = "Αριθμός Παραστατικού";
            this.colDocumentNumber.FieldName = "DocumentNumber";
            this.colDocumentNumber.Name = "colDocumentNumber";
            this.colDocumentNumber.Visible = true;
            this.colDocumentNumber.VisibleIndex = 1;
            // 
            // colDocumentIssueDate
            // 
            this.colDocumentIssueDate.Caption = "Ημερομηνία Έκδοσης";
            this.colDocumentIssueDate.FieldName = "DocumentIssueDate";
            this.colDocumentIssueDate.Name = "colDocumentIssueDate";
            this.colDocumentIssueDate.Visible = true;
            this.colDocumentIssueDate.VisibleIndex = 2;
            // 
            // colDocumentAmount
            // 
            this.colDocumentAmount.Caption = "Ποσό";
            this.colDocumentAmount.FieldName = "DocumentAmount";
            this.colDocumentAmount.Name = "colDocumentAmount";
            this.colDocumentAmount.Visible = true;
            this.colDocumentAmount.VisibleIndex = 3;
            // 
            // colScannedImage
            // 
            this.colScannedImage.Caption = "Εικόνα";
            this.colScannedImage.ColumnEdit = this.repositoryItemImageEdit1;
            this.colScannedImage.FieldName = "ScannedImage";
            this.colScannedImage.Name = "colScannedImage";
            this.colScannedImage.Visible = true;
            this.colScannedImage.VisibleIndex = 4;
            // 
            // repositoryItemImageEdit1
            // 
            this.repositoryItemImageEdit1.AutoHeight = false;
            this.repositoryItemImageEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageEdit1.Name = "repositoryItemImageEdit1";
            // 
            // colUploaded
            // 
            this.colUploaded.Caption = "Uploaded";
            this.colUploaded.FieldName = "Uploaded";
            this.colUploaded.Name = "colUploaded";
            this.colUploaded.OptionsColumn.AllowEdit = false;
            this.colUploaded.OptionsColumn.AllowFocus = false;
            this.colUploaded.Visible = true;
            this.colUploaded.VisibleIndex = 6;
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Appearance.BackColor = System.Drawing.Color.SkyBlue;
            this.splitterControl1.Appearance.Options.UseBackColor = true;
            this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl1.Location = new System.Drawing.Point(0, 298);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(780, 5);
            this.splitterControl1.TabIndex = 50;
            this.splitterControl1.TabStop = false;
            this.splitterControl1.Move += new System.EventHandler(this.splitterControl1_Move);
            // 
            // colValid
            // 
            this.colValid.Caption = "Εγκυρη Εγγραφή";
            this.colValid.FieldName = "IsValid";
            this.colValid.Name = "colValid";
            this.colValid.Visible = true;
            this.colValid.VisibleIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 610);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.grpDocuments);
            this.Controls.Add(this.grpEdit);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Σάρωση Παραστατικών";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.xpcScannedDocumentHeaders)).EndInit();
            this.grpEdit.ResumeLayout(false);
            this.grpEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVat.Properties)).EndInit();
            this.grpDocuments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Xpo.XPCollection xpcScannedDocumentHeaders;
        private System.Windows.Forms.GroupBox grpEdit;
        private DevExpress.XtraEditors.PictureEdit picture;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSavePrint;
        private DevExpress.XtraEditors.SimpleButton btnScan;
        private DevExpress.XtraEditors.DateEdit txtDate;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtAmount;
        private DevExpress.XtraEditors.TextEdit txt;
        private DevExpress.XtraEditors.TextEdit txtVat;
        private System.Windows.Forms.GroupBox grpDocuments;
        private DevExpress.XtraEditors.SimpleButton btnSettings;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnDeleteUploaded;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.SimpleButton btnUpload;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView DetailView;
        private DevExpress.XtraGrid.Columns.GridColumn colSupplierTaxCode;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colDocumentNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colDocumentIssueDate;
        private DevExpress.XtraGrid.Columns.GridColumn colDocumentAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colScannedImage;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageEdit repositoryItemImageEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colUploaded;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraEditors.SimpleButton btnScanAll;
        private DevExpress.XtraGrid.Columns.GridColumn colValid;
    }
}

