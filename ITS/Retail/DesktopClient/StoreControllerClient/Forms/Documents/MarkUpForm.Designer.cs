namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class MarkUpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkUpForm));
            this.simpleButtonSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlOptions = new DevExpress.XtraLayout.LayoutControl();
            this.spinEditMarkUpDif = new DevExpress.XtraEditors.SpinEdit();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.toggleSwitchAllProducts = new DevExpress.XtraEditors.ToggleSwitch();
            this.toggleSwitchSaveMarkUp = new DevExpress.XtraEditors.ToggleSwitch();
            this.layoutControlGroupOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemSaveButton = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemAllProducts = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemMarkupDif = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemSaveMarkUp = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.gridControlMarkUpValues = new DevExpress.XtraGrid.GridControl();
            this.gridViewMarkUpValues = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSelected = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPriceCatalog = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnVatIncluded = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnOldUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnNewUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnMarkUp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnUserUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControlMarkUp = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlMarkUpGrid = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroupMarkUpGrid = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemMarkUpGrid = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlOptions)).BeginInit();
            this.layoutControlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditMarkUpDif.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchAllProducts.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchSaveMarkUp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAllProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemMarkupDif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveMarkUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMarkUpValues)).BeginInit();
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
            this.simpleButtonSave.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Save_32;
            this.simpleButtonSave.Location = new System.Drawing.Point(663, 12);
            this.simpleButtonSave.Name = "simpleButtonSave";
            this.simpleButtonSave.Size = new System.Drawing.Size(188, 45);
            this.simpleButtonSave.StyleController = this.layoutControlOptions;
            this.simpleButtonSave.TabIndex = 4;
            this.simpleButtonSave.Text = "@@Save";
            this.simpleButtonSave.Click += new System.EventHandler(this.simpleButtonSave_Click);
            // 
            // layoutControlOptions
            // 
            this.layoutControlOptions.Controls.Add(this.spinEditMarkUpDif);
            this.layoutControlOptions.Controls.Add(this.simpleButtonSave);
            this.layoutControlOptions.Controls.Add(this.simpleButtonCancel);
            this.layoutControlOptions.Controls.Add(this.toggleSwitchAllProducts);
            this.layoutControlOptions.Controls.Add(this.toggleSwitchSaveMarkUp);
            this.layoutControlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControlOptions.Location = new System.Drawing.Point(2, 2);
            this.layoutControlOptions.Name = "layoutControlOptions";
            this.layoutControlOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(426, 155, 675, 496);
            this.layoutControlOptions.Root = this.layoutControlGroupOptions;
            this.layoutControlOptions.Size = new System.Drawing.Size(1078, 69);
            this.layoutControlOptions.TabIndex = 8;
            this.layoutControlOptions.Text = "layoutControl1";
            // 
            // spinEditMarkUpDif
            // 
            this.spinEditMarkUpDif.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEditMarkUpDif.Location = new System.Drawing.Point(12, 28);
            this.spinEditMarkUpDif.Name = "spinEditMarkUpDif";
            this.spinEditMarkUpDif.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinEditMarkUpDif.Size = new System.Drawing.Size(161, 20);
            this.spinEditMarkUpDif.StyleController = this.layoutControlOptions;
            this.spinEditMarkUpDif.TabIndex = 0;
            this.spinEditMarkUpDif.EditValueChanged += new System.EventHandler(this.spinEditMarkUpDif_EditValueChanged);
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.simpleButtonCancel.Location = new System.Drawing.Point(855, 12);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(211, 45);
            this.simpleButtonCancel.StyleController = this.layoutControlOptions;
            this.simpleButtonCancel.TabIndex = 5;
            this.simpleButtonCancel.Text = "@@Cancel";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // toggleSwitchAllProducts
            // 
            this.toggleSwitchAllProducts.AutoSizeInLayoutControl = true;
            this.toggleSwitchAllProducts.Location = new System.Drawing.Point(177, 28);
            this.toggleSwitchAllProducts.Name = "toggleSwitchAllProducts";
            this.toggleSwitchAllProducts.Properties.OffText = "@@Off";
            this.toggleSwitchAllProducts.Properties.OnText = "@@On";
            this.toggleSwitchAllProducts.Size = new System.Drawing.Size(110, 24);
            this.toggleSwitchAllProducts.StyleController = this.layoutControlOptions;
            this.toggleSwitchAllProducts.TabIndex = 2;
            this.toggleSwitchAllProducts.EditValueChanged += new System.EventHandler(this.toggleSwitchAllProducts_EditValueChanged);
            // 
            // toggleSwitchSaveMarkUp
            // 
            this.toggleSwitchSaveMarkUp.Location = new System.Drawing.Point(291, 28);
            this.toggleSwitchSaveMarkUp.Name = "toggleSwitchSaveMarkUp";
            this.toggleSwitchSaveMarkUp.Properties.OffText = "@@Off";
            this.toggleSwitchSaveMarkUp.Properties.OnText = "@@On";
            this.toggleSwitchSaveMarkUp.Size = new System.Drawing.Size(112, 24);
            this.toggleSwitchSaveMarkUp.StyleController = this.layoutControlOptions;
            this.toggleSwitchSaveMarkUp.TabIndex = 3;
            // 
            // layoutControlGroupOptions
            // 
            this.layoutControlGroupOptions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupOptions.GroupBordersVisible = false;
            this.layoutControlGroupOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemSaveButton,
            this.layoutControlItemCancelButton,
            this.layoutControlItemAllProducts,
            this.layoutControlItemMarkupDif,
            this.layoutControlItemSaveMarkUp,
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
            this.layoutControlItemSaveButton.Location = new System.Drawing.Point(651, 0);
            this.layoutControlItemSaveButton.MinSize = new System.Drawing.Size(86, 34);
            this.layoutControlItemSaveButton.Name = "layoutControlItemSaveButton";
            this.layoutControlItemSaveButton.Size = new System.Drawing.Size(192, 49);
            this.layoutControlItemSaveButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItemSaveButton.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemSaveButton.TextVisible = false;
            // 
            // layoutControlItemCancelButton
            // 
            this.layoutControlItemCancelButton.Control = this.simpleButtonCancel;
            this.SetIsRequired(this.layoutControlItemCancelButton, false);
            this.layoutControlItemCancelButton.Location = new System.Drawing.Point(843, 0);
            this.layoutControlItemCancelButton.MinSize = new System.Drawing.Size(94, 34);
            this.layoutControlItemCancelButton.Name = "layoutControlItemCancelButton";
            this.layoutControlItemCancelButton.Size = new System.Drawing.Size(215, 49);
            this.layoutControlItemCancelButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItemCancelButton.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemCancelButton.TextVisible = false;
            // 
            // layoutControlItemAllProducts
            // 
            this.layoutControlItemAllProducts.Control = this.toggleSwitchAllProducts;
            this.SetIsRequired(this.layoutControlItemAllProducts, false);
            this.layoutControlItemAllProducts.Location = new System.Drawing.Point(165, 0);
            this.layoutControlItemAllProducts.Name = "layoutControlItemAllProducts";
            this.layoutControlItemAllProducts.Size = new System.Drawing.Size(114, 49);
            this.layoutControlItemAllProducts.Text = "@@AllProducts";
            this.layoutControlItemAllProducts.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemAllProducts.TextSize = new System.Drawing.Size(96, 13);
            // 
            // layoutControlItemMarkupDif
            // 
            this.layoutControlItemMarkupDif.Control = this.spinEditMarkUpDif;
            this.SetIsRequired(this.layoutControlItemMarkupDif, false);
            this.layoutControlItemMarkupDif.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemMarkupDif.Name = "layoutControlItemMarkupDif";
            this.layoutControlItemMarkupDif.Size = new System.Drawing.Size(165, 49);
            this.layoutControlItemMarkupDif.Text = "@@ValueDifference";
            this.layoutControlItemMarkupDif.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemMarkupDif.TextSize = new System.Drawing.Size(96, 13);
            // 
            // layoutControlItemSaveMarkUp
            // 
            this.layoutControlItemSaveMarkUp.Control = this.toggleSwitchSaveMarkUp;
            this.SetIsRequired(this.layoutControlItemSaveMarkUp, false);
            this.layoutControlItemSaveMarkUp.Location = new System.Drawing.Point(279, 0);
            this.layoutControlItemSaveMarkUp.Name = "layoutControlItemSaveMarkUp";
            this.layoutControlItemSaveMarkUp.Size = new System.Drawing.Size(116, 49);
            this.layoutControlItemSaveMarkUp.Text = "@@SaveMarkups";
            this.layoutControlItemSaveMarkUp.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemSaveMarkUp.TextSize = new System.Drawing.Size(96, 13);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem1, false);
            this.emptySpaceItem1.Location = new System.Drawing.Point(395, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(256, 49);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // gridControlMarkUpValues
            // 
            this.gridControlMarkUpValues.Location = new System.Drawing.Point(12, 12);
            this.gridControlMarkUpValues.MainView = this.gridViewMarkUpValues;
            this.gridControlMarkUpValues.Name = "gridControlMarkUpValues";
            this.gridControlMarkUpValues.Size = new System.Drawing.Size(1054, 554);
            this.gridControlMarkUpValues.TabIndex = 0;
            this.gridControlMarkUpValues.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMarkUpValues});
            // 
            // gridViewMarkUpValues
            // 
            this.gridViewMarkUpValues.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSelected,
            this.gridColumnItemCode,
            this.gridColumnItem,
            this.gridColumnPriceCatalog,
            this.gridColumnUnitPrice,
            this.gridColumnVatIncluded,
            this.gridColumnOldUnitPrice,
            this.gridColumnNewUnitPrice,
            this.gridColumnMarkUp,
            this.gridColumnUserUnitPrice});
            this.gridViewMarkUpValues.GridControl = this.gridControlMarkUpValues;
            this.gridViewMarkUpValues.Name = "gridViewMarkUpValues";
            this.gridViewMarkUpValues.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMarkUpValues_CellValueChanged);
            // 
            // gridColumnSelected
            // 
            this.gridColumnSelected.Caption = "@@Select";
            this.gridColumnSelected.FieldName = "Selected";
            this.gridColumnSelected.Name = "gridColumnSelected";
            this.gridColumnSelected.Visible = true;
            this.gridColumnSelected.VisibleIndex = 0;
            // 
            // gridColumnItemCode
            // 
            this.gridColumnItemCode.Caption = "@@Code";
            this.gridColumnItemCode.FieldName = "priceCatalogDetail.Item.Code";
            this.gridColumnItemCode.Name = "gridColumnItemCode";
            this.gridColumnItemCode.OptionsColumn.AllowEdit = false;
            this.gridColumnItemCode.Visible = true;
            this.gridColumnItemCode.VisibleIndex = 1;
            // 
            // gridColumnItem
            // 
            this.gridColumnItem.Caption = "@@Item";
            this.gridColumnItem.FieldName = "newDocumentDetail.Item.Name";
            this.gridColumnItem.Name = "gridColumnItem";
            this.gridColumnItem.OptionsColumn.AllowEdit = false;
            this.gridColumnItem.Visible = true;
            this.gridColumnItem.VisibleIndex = 2;
            // 
            // gridColumnPriceCatalog
            // 
            this.gridColumnPriceCatalog.Caption = "@@PriceCatalog";
            this.gridColumnPriceCatalog.FieldName = "priceCatalogDetail.PriceCatalog.Description";
            this.gridColumnPriceCatalog.Name = "gridColumnPriceCatalog";
            this.gridColumnPriceCatalog.OptionsColumn.AllowEdit = false;
            this.gridColumnPriceCatalog.Visible = true;
            this.gridColumnPriceCatalog.VisibleIndex = 3;
            // 
            // gridColumnUnitPrice
            // 
            this.gridColumnUnitPrice.Caption = "@@UnitPrice";
            this.gridColumnUnitPrice.DisplayFormat.FormatString = "f";
            this.gridColumnUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnUnitPrice.FieldName = "priceCatalogDetail.Value";
            this.gridColumnUnitPrice.Name = "gridColumnUnitPrice";
            this.gridColumnUnitPrice.OptionsColumn.AllowEdit = false;
            this.gridColumnUnitPrice.Visible = true;
            this.gridColumnUnitPrice.VisibleIndex = 4;
            // 
            // gridColumnVatIncluded
            // 
            this.gridColumnVatIncluded.Caption = "@@VatIncluded";
            this.gridColumnVatIncluded.FieldName = "priceCatalogDetail.VATIncluded";
            this.gridColumnVatIncluded.Name = "gridColumnVatIncluded";
            this.gridColumnVatIncluded.OptionsColumn.AllowEdit = false;
            this.gridColumnVatIncluded.Visible = true;
            this.gridColumnVatIncluded.VisibleIndex = 5;
            // 
            // gridColumnOldUnitPrice
            // 
            this.gridColumnOldUnitPrice.Caption = "@@OldUnitPrice";
            this.gridColumnOldUnitPrice.DisplayFormat.FormatString = "f";
            this.gridColumnOldUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnOldUnitPrice.FieldName = "lastValueStr";
            this.gridColumnOldUnitPrice.Name = "gridColumnOldUnitPrice";
            this.gridColumnOldUnitPrice.OptionsColumn.AllowEdit = false;
            this.gridColumnOldUnitPrice.Visible = true;
            this.gridColumnOldUnitPrice.VisibleIndex = 6;
            // 
            // gridColumnNewUnitPrice
            // 
            this.gridColumnNewUnitPrice.Caption = "@@NewUnitPrice";
            this.gridColumnNewUnitPrice.DisplayFormat.FormatString = "f";
            this.gridColumnNewUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnNewUnitPrice.FieldName = "newValue";
            this.gridColumnNewUnitPrice.Name = "gridColumnNewUnitPrice";
            this.gridColumnNewUnitPrice.OptionsColumn.AllowEdit = false;
            this.gridColumnNewUnitPrice.Visible = true;
            this.gridColumnNewUnitPrice.VisibleIndex = 7;
            // 
            // gridColumnMarkUp
            // 
            this.gridColumnMarkUp.Caption = "@@MarkUp";
            this.gridColumnMarkUp.DisplayFormat.FormatString = "p";
            this.gridColumnMarkUp.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumnMarkUp.FieldName = "MarkUp";
            this.gridColumnMarkUp.Name = "gridColumnMarkUp";
            this.gridColumnMarkUp.Visible = true;
            this.gridColumnMarkUp.VisibleIndex = 8;
            // 
            // gridColumnUserUnitPrice
            // 
            this.gridColumnUserUnitPrice.Caption = "@@UnitPrice";
            this.gridColumnUserUnitPrice.DisplayFormat.FormatString = "d2";
            this.gridColumnUserUnitPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnUserUnitPrice.FieldName = "UnitPrice";
            this.gridColumnUserUnitPrice.Name = "gridColumnUserUnitPrice";
            this.gridColumnUserUnitPrice.Visible = true;
            this.gridColumnUserUnitPrice.VisibleIndex = 9;
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
            this.layoutControlMarkUpGrid.Controls.Add(this.gridControlMarkUpValues);
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
            this.layoutControlItemMarkUpGrid.Control = this.gridControlMarkUpValues;
            this.SetIsRequired(this.layoutControlItemMarkUpGrid, false);
            this.layoutControlItemMarkUpGrid.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemMarkUpGrid.Name = "layoutControlItemMarkUpGrid";
            this.layoutControlItemMarkUpGrid.Size = new System.Drawing.Size(1058, 558);
            this.layoutControlItemMarkUpGrid.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemMarkUpGrid.TextVisible = false;
            // 
            // MarkUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 651);
            this.Controls.Add(this.panelControlMarkUp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MarkUpForm";
            this.Text = "@@MarkUp";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.MarkUpForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlOptions)).EndInit();
            this.layoutControlOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinEditMarkUpDif.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchAllProducts.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchSaveMarkUp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCancelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAllProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemMarkupDif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSaveMarkUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMarkUpValues)).EndInit();
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
        private DevExpress.XtraEditors.ToggleSwitch toggleSwitchAllProducts;
        private DevExpress.XtraEditors.ToggleSwitch toggleSwitchSaveMarkUp;
        private DevExpress.XtraGrid.GridControl gridControlMarkUpValues;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMarkUpValues;
        private DevExpress.XtraEditors.PanelControl panelControlMarkUp;
        private DevExpress.XtraEditors.SpinEdit spinEditMarkUpDif;
        private DevExpress.XtraLayout.LayoutControl layoutControlOptions;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupOptions;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemAllProducts;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemMarkupDif;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSaveMarkUp;
        private DevExpress.XtraLayout.LayoutControl layoutControlMarkUpGrid;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupMarkUpGrid;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemMarkUpGrid;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSelected;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnItem;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPriceCatalog;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnVatIncluded;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnOldUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnNewUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnMarkUp;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnUserUnitPrice;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSaveButton;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCancelButton;


    }
}