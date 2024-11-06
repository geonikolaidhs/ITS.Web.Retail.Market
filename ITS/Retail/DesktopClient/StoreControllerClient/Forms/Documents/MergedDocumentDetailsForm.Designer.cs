namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class MergedDocumentDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergedDocumentDetailsForm));
            this.simpleButtonSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlOptions = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroupOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemSaveButton = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.gridControlMergeDocumentDetails = new DevExpress.XtraGrid.GridControl();
            this.gridViewMarkUpValues = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnBarcodeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnVatFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIsLinkedLine = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnRemarks = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControlMarkUp = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlMarkUpGrid = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroupMarkUpGrid = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemMarkUpGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlOptions)).BeginInit();
            this.layoutControlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMergeDocumentDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMarkUpValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlMarkUp)).BeginInit();
            this.panelControlMarkUp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlMarkUpGrid)).BeginInit();
            this.layoutControlMarkUpGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMarkUpGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemMarkUpGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButtonSave
            // 
            this.simpleButtonSave.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.XLS_32;
            this.simpleButtonSave.Location = new System.Drawing.Point(577, 12);
            this.simpleButtonSave.Name = "simpleButtonSave";
            this.simpleButtonSave.Size = new System.Drawing.Size(229, 45);
            this.simpleButtonSave.StyleController = this.layoutControlOptions;
            this.simpleButtonSave.TabIndex = 4;
            this.simpleButtonSave.Text = "@@ExportΤoXLS";
            this.simpleButtonSave.Click += new System.EventHandler(this.simpleButtonSave_Click);
            // 
            // layoutControlOptions
            // 
            this.layoutControlOptions.Controls.Add(this.simpleButtonSave);
            this.layoutControlOptions.Controls.Add(this.simpleButtonCancel);
            this.layoutControlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControlOptions.Location = new System.Drawing.Point(2, 2);
            this.layoutControlOptions.Name = "layoutControlOptions";
            this.layoutControlOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(426, 155, 675, 496);
            this.layoutControlOptions.Root = this.layoutControlGroupOptions;
            this.layoutControlOptions.Size = new System.Drawing.Size(1078, 69);
            this.layoutControlOptions.TabIndex = 8;
            this.layoutControlOptions.Text = "layoutControl1";
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.simpleButtonCancel.Location = new System.Drawing.Point(810, 12);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(256, 45);
            this.simpleButtonCancel.StyleController = this.layoutControlOptions;
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "@@Cancel";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // layoutControlGroupOptions
            // 
            this.layoutControlGroupOptions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupOptions.GroupBordersVisible = false;
            this.layoutControlGroupOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemSaveButton,
            this.layoutControlItemCancelButton,
            this.emptySpaceItem1});
            this.layoutControlGroupOptions.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupOptions.Name = "Root";
            this.layoutControlGroupOptions.Size = new System.Drawing.Size(1078, 69);
            this.layoutControlGroupOptions.TextVisible = false;
            // 
            // layoutControlItemSaveButton
            // 
            this.layoutControlItemSaveButton.Control = this.simpleButtonSave;
            this.SetIsRequired(this.layoutControlItemSaveButton, false);
            this.layoutControlItemSaveButton.Location = new System.Drawing.Point(565, 0);
            this.layoutControlItemSaveButton.MinSize = new System.Drawing.Size(86, 34);
            this.layoutControlItemSaveButton.Name = "layoutControlItemSaveButton";
            this.layoutControlItemSaveButton.Size = new System.Drawing.Size(233, 49);
            this.layoutControlItemSaveButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItemSaveButton.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemSaveButton.TextVisible = false;
            // 
            // layoutControlItemCancelButton
            // 
            this.layoutControlItemCancelButton.Control = this.simpleButtonCancel;
            this.SetIsRequired(this.layoutControlItemCancelButton, false);
            this.layoutControlItemCancelButton.Location = new System.Drawing.Point(798, 0);
            this.layoutControlItemCancelButton.MinSize = new System.Drawing.Size(94, 34);
            this.layoutControlItemCancelButton.Name = "layoutControlItemCancelButton";
            this.layoutControlItemCancelButton.Size = new System.Drawing.Size(260, 49);
            this.layoutControlItemCancelButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItemCancelButton.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemCancelButton.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem1, false);
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(565, 49);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // gridControlMergeDocumentDetails
            // 
            this.gridControlMergeDocumentDetails.Location = new System.Drawing.Point(12, 12);
            this.gridControlMergeDocumentDetails.MainView = this.gridViewMarkUpValues;
            this.gridControlMergeDocumentDetails.Name = "gridControlMergeDocumentDetails";
            this.gridControlMergeDocumentDetails.Size = new System.Drawing.Size(1054, 554);
            this.gridControlMergeDocumentDetails.TabIndex = 0;
            this.gridControlMergeDocumentDetails.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMarkUpValues});
            // 
            // gridViewMarkUpValues
            // 
            this.gridViewMarkUpValues.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDescription,
            this.gridColumnItemCode,
            this.gridColumnBarcodeCode,
            this.gridColumnQty,
            this.gridColumnVatFactor,
            this.gridColumnIsLinkedLine,
            this.gridColumnRemarks});
            this.gridViewMarkUpValues.GridControl = this.gridControlMergeDocumentDetails;
            this.gridViewMarkUpValues.Name = "gridViewMarkUpValues";
            this.gridViewMarkUpValues.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMarkUpValues_CellValueChanged);
            // 
            // gridColumnDescription
            // 
            this.gridColumnDescription.Caption = "@@Description";
            this.gridColumnDescription.FieldName = "Description";
            this.gridColumnDescription.Name = "gridColumnDescription";
            this.gridColumnDescription.Visible = true;
            this.gridColumnDescription.VisibleIndex = 0;
            // 
            // gridColumnItemCode
            // 
            this.gridColumnItemCode.Caption = "@@ItemCode";
            this.gridColumnItemCode.FieldName = "ItemCode";
            this.gridColumnItemCode.Name = "gridColumnItemCode";
            this.gridColumnItemCode.OptionsColumn.AllowEdit = false;
            this.gridColumnItemCode.Visible = true;
            this.gridColumnItemCode.VisibleIndex = 1;
            // 
            // gridColumnBarcodeCode
            // 
            this.gridColumnBarcodeCode.Caption = "@@BarcodeCode";
            this.gridColumnBarcodeCode.FieldName = "BarcodeCode";
            this.gridColumnBarcodeCode.Name = "gridColumnBarcodeCode";
            this.gridColumnBarcodeCode.OptionsColumn.AllowEdit = false;
            this.gridColumnBarcodeCode.Visible = true;
            this.gridColumnBarcodeCode.VisibleIndex = 2;
            // 
            // gridColumnQty
            // 
            this.gridColumnQty.Caption = "@@SummedQty";
            this.gridColumnQty.DisplayFormat.FormatString = "N";
            this.gridColumnQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnQty.FieldName = "Qty";
            this.gridColumnQty.Name = "gridColumnQty";
            this.gridColumnQty.OptionsColumn.AllowEdit = false;
            this.gridColumnQty.Visible = true;
            this.gridColumnQty.VisibleIndex = 3;
            // 
            // gridColumnVatFactor
            // 
            this.gridColumnVatFactor.Caption = "@@VatFactor";
            this.gridColumnVatFactor.FieldName = "VatFactor";
            this.gridColumnVatFactor.Name = "gridColumnVatFactor";
            this.gridColumnVatFactor.OptionsColumn.AllowEdit = false;
            this.gridColumnVatFactor.Visible = true;
            this.gridColumnVatFactor.VisibleIndex = 4;
            // 
            // gridColumnIsLinkedLine
            // 
            this.gridColumnIsLinkedLine.Caption = "Linked Line";
            this.gridColumnIsLinkedLine.FieldName = "IsLinkedLine";
            this.gridColumnIsLinkedLine.Name = "gridColumnIsLinkedLine";
            this.gridColumnIsLinkedLine.OptionsColumn.AllowEdit = false;
            this.gridColumnIsLinkedLine.Visible = true;
            this.gridColumnIsLinkedLine.VisibleIndex = 5;
            // 
            // gridColumnRemarks
            // 
            this.gridColumnRemarks.Caption = "@@Remarks";
            this.gridColumnRemarks.FieldName = "Remarks";
            this.gridColumnRemarks.Name = "gridColumnRemarks";
            this.gridColumnRemarks.OptionsColumn.AllowEdit = false;
            this.gridColumnRemarks.Visible = true;
            this.gridColumnRemarks.VisibleIndex = 6;
            // 
            // panelControlMarkUp
            // 
            this.panelControlMarkUp.AutoSize = true;
            this.panelControlMarkUp.Controls.Add(this.layoutControlMarkUpGrid);
            this.panelControlMarkUp.Controls.Add(this.layoutControlOptions);
            this.panelControlMarkUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlMarkUp.Location = new System.Drawing.Point(0, 0);
            this.panelControlMarkUp.Name = "panelControlMarkUp";
            this.panelControlMarkUp.Size = new System.Drawing.Size(1082, 651);
            this.panelControlMarkUp.TabIndex = 3;
            // 
            // layoutControlMarkUpGrid
            // 
            this.layoutControlMarkUpGrid.Controls.Add(this.gridControlMergeDocumentDetails);
            this.layoutControlMarkUpGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlMarkUpGrid.Location = new System.Drawing.Point(2, 71);
            this.layoutControlMarkUpGrid.Name = "layoutControlMarkUpGrid";
            this.layoutControlMarkUpGrid.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(602, 405, 635, 350);
            this.layoutControlMarkUpGrid.Root = this.layoutControlGroupMarkUpGrid;
            this.layoutControlMarkUpGrid.Size = new System.Drawing.Size(1078, 578);
            this.layoutControlMarkUpGrid.TabIndex = 9;
            this.layoutControlMarkUpGrid.Text = "layoutControl2";
            // 
            // layoutControlGroupMarkUpGrid
            // 
            this.layoutControlGroupMarkUpGrid.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupMarkUpGrid.GroupBordersVisible = false;
            this.layoutControlGroupMarkUpGrid.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemMarkUpGrid});
            this.layoutControlGroupMarkUpGrid.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupMarkUpGrid.Name = "layoutControlGroupMarkUpGrid";
            this.layoutControlGroupMarkUpGrid.Size = new System.Drawing.Size(1078, 578);
            this.layoutControlGroupMarkUpGrid.TextVisible = false;
            // 
            // layoutControlItemMarkUpGrid
            // 
            this.layoutControlItemMarkUpGrid.Control = this.gridControlMergeDocumentDetails;
            this.SetIsRequired(this.layoutControlItemMarkUpGrid, false);
            this.layoutControlItemMarkUpGrid.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemMarkUpGrid.Name = "layoutControlItemMarkUpGrid";
            this.layoutControlItemMarkUpGrid.Size = new System.Drawing.Size(1058, 558);
            this.layoutControlItemMarkUpGrid.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemMarkUpGrid.TextVisible = false;
            // 
            // saveFileDialog2
            // 
            this.saveFileDialog2.CheckFileExists = true;
            this.saveFileDialog2.CreatePrompt = true;
            this.saveFileDialog2.DefaultExt = "xlsx";
            this.saveFileDialog2.Filter = "(*.xlsx*)|*.xlsx*";
            // 
            // MergedDocumentDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 651);
            this.Controls.Add(this.panelControlMarkUp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MergedDocumentDetailsForm";
            this.Text = "@@MergeDocuments";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlOptions)).EndInit();
            this.layoutControlOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMergeDocumentDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMarkUpValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlMarkUp)).EndInit();
            this.panelControlMarkUp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlMarkUpGrid)).EndInit();
            this.layoutControlMarkUpGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMarkUpGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemMarkUpGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.SimpleButton simpleButtonSave;
        private DevExpress.XtraGrid.GridControl gridControlMergeDocumentDetails;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMarkUpValues;
        private DevExpress.XtraEditors.PanelControl panelControlMarkUp;
        private DevExpress.XtraLayout.LayoutControl layoutControlOptions;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupOptions;
        private DevExpress.XtraLayout.LayoutControl layoutControlMarkUpGrid;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupMarkUpGrid;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemMarkUpGrid;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBarcodeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnQty;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnVatFactor;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsLinkedLine;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnRemarks;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSaveButton;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCancelButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
    }
}