using DevExpress.XtraLayout;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class ItemEditForm
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditForm));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject17 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject18 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject19 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject20 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject21 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject22 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject23 = new DevExpress.Utils.SerializableAppearanceObject();
            this.grdViewPriceCatalogDetailTimeValues = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColDateFrom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dtTimeValueFrom = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.grdColDateTo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dtTimeValueUntil = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.grdColTimeValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
            this.gridColumnIsActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDeletePriceCatalogDetailTimeValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridPriceCatalogs = new DevExpress.XtraGrid.GridControl();
            this.grdViewPriceCatalogDetails = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColPriceCatalog = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbPriceCatalog = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColDatabaseValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtDatabaseValue = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColBarcode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbBarcode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColPriceCatalogCreatedOn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPriceCatalogUpdatedOn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColDiscount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtDiscount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColMarkUp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtMarkup = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColVatIncluded = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkVatIncluded = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColumnPriceCatalogDetailIsActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkPcdIsActive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColDeletePriceCatalogDetail = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_delete_PriceCatalog = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.txtTimeValue = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.btn_Del_SubItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlHeader = new DevExpress.XtraLayout.LayoutControl();
            this.DoesNotAllowDiscount = new DevExpress.XtraEditors.CheckEdit();
            this.IsTax = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tabItemInfo = new DevExpress.XtraTab.XtraTabControl();
            this.xtpBarcodes = new DevExpress.XtraTab.XtraTabPage();
            this.gridItemBarcodes = new DevExpress.XtraGrid.GridControl();
            this.grdViewItemBarcodes = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColBarcodeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColBarcodeMeasurementUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemBarcodeMeasurementUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColCreatedOn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColUpdatedOn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPLUCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPLUPrefix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColBarcodeRelationFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.spinEditItemBarcodeRelationFactor = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColBarcodeType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemBarcodeBarcodeType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColDeleteItemBarcode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_delete_ItemBarcode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.xtpPriceCatalogs = new DevExpress.XtraTab.XtraTabPage();
            this.xtpItemCategories = new DevExpress.XtraTab.XtraTabPage();
            this.gridItemCategories = new DevExpress.XtraGrid.GridControl();
            this.grdViewItemCategories = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColCategoryNode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TreeListItemCategoryNode = new DevExpress.XtraEditors.Repository.RepositoryItemTreeListLookUpEdit();
            this.TreeListItemCategoryRepository = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.grdColCategoryRoot = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColCategoryPath = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColDelItemCategory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_delete_category = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtpChildItems = new DevExpress.XtraTab.XtraTabPage();
            this.gridChildItems = new DevExpress.XtraGrid.GridControl();
            this.grdViewChildItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColChildItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbChildItem = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.cmbChildItemGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColChildItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbChildItemName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColDeleteChildItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_Del_ChildItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtpLinkedItems = new DevExpress.XtraTab.XtraTabPage();
            this.gridLinkedItems = new DevExpress.XtraGrid.GridControl();
            this.grdViewLinkedItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColLinkedItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbLinkedItem = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnLinkedItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLinkedItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColLinkedItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbLinkedItemName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColLinkedItemQtyFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtLinkedItemsQuantityFactor = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColDeleteLinkedItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_Del_LinkedItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtpSubItems = new DevExpress.XtraTab.XtraTabPage();
            this.gridSubItems = new DevExpress.XtraGrid.GridControl();
            this.grdViewSubItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSubItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemLinkedTo = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridViewCmbItemLinkedTo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnItemLinkedToCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnItemLinkedToName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColSubItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemLinkedToName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColSubItemQtyFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtItemsLinkedToQuantityFactor = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.grdColDeleteSubItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_delete_SubItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtpStores = new DevExpress.XtraTab.XtraTabPage();
            this.gridItemStores = new DevExpress.XtraGrid.GridControl();
            this.grdViewItemStores = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColItemStoreCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemStoreStore = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColItemStoreName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemStoreStoreName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColDeleteItemStore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_Del_ItemStore = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtpItemStocks = new DevExpress.XtraTab.XtraTabPage();
            this.gridItemStocks = new DevExpress.XtraGrid.GridControl();
            this.grdViewItemStocks = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColItemStockStore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemStockStore = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColItemStockQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtColItemStockQty = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumnDesirableStock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbItemStockBarcode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.btn_Del_ItemStock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.xtraTabPageItemExtraInfo = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.txtIngredients = new DevExpress.XtraEditors.MemoEdit();
            this.dtExpiresAt = new DevExpress.XtraEditors.DateEdit();
            this.dtPackedAt = new DevExpress.XtraEditors.DateEdit();
            this.txtLot = new DevExpress.XtraEditors.TextEdit();
            this.txtOrigin = new DevExpress.XtraEditors.TextEdit();
            this.textEditItemExtraInfoDescription = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemItemExtraInfoDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemItemExtraInfoLot = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemItemExtraInfoOrigin = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPackedAt = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExpiresAt = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIngredients = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.imageEditItemImage = new DevExpress.XtraEditors.ImageEdit();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();
            this.panelControlHeader = new DevExpress.XtraEditors.PanelControl();
            this.btnCancelItem = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveItem = new DevExpress.XtraEditors.SimpleButton();
            this.lueSeasonality = new DevExpress.XtraEditors.LookUpEdit();
            this.lueAcceptsCustomPrice = new DevExpress.XtraEditors.LookUpEdit();
            this.lueVatCategory = new DevExpress.XtraEditors.LookUpEdit();
            this.txtExtraDescription = new DevExpress.XtraEditors.TextEdit();
            this.luePackingMeasurementUnit = new DevExpress.XtraEditors.LookUpEdit();
            this.chkAcceptsCustomDescription = new DevExpress.XtraEditors.CheckEdit();
            this.txtPoints = new DevExpress.XtraEditors.SpinEdit();
            this.txtPackingQty = new DevExpress.XtraEditors.SpinEdit();
            this.dtInsertedOn = new DevExpress.XtraEditors.DateEdit();
            this.lueDefaultBarcode = new DevExpress.XtraEditors.LookUpEdit();
            this.txtMaxOrderQty = new DevExpress.XtraEditors.SpinEdit();
            this.lueBuyer = new DevExpress.XtraEditors.LookUpEdit();
            this.txtReferenceUnit = new DevExpress.XtraEditors.SpinEdit();
            this.txtContentUnit = new DevExpress.XtraEditors.SpinEdit();
            this.chkIsCentralStored = new DevExpress.XtraEditors.CheckEdit();
            this.memoRemarks = new DevExpress.XtraEditors.MemoEdit();
            this.txtMinOrderQty = new DevExpress.XtraEditors.SpinEdit();
            this.lueMotherCode = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lueItemSupplier = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtExtraFile = new DevExpress.XtraEditors.ButtonEdit();
            this.layoutControlGroupAllItems = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemHeader = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemImage = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcMotherCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcVatCategory = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDefaultBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPoints = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcInsertedOn = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPackingMeasurementUnit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPackingQty = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcItemSupplier = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcMinOrderQty = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcMaxOrderQty = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcBreakOrderToCentral = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExtraDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSeasonality = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcBuyer = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcReferenceUnit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcContentUnit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcAcceptsCustomPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSex = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcRemarks = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIsActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExtraFile = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnDelChildItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPriceCatalogDetailTimeValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueFrom.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueUntil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueUntil.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEditPriceCatalogDetailTimeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPriceCatalogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPriceCatalogDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPriceCatalog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMarkup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVatIncluded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPcdIsActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_PriceCatalog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_SubItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlHeader)).BeginInit();
            this.layoutControlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DoesNotAllowDiscount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IsTax.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabItemInfo)).BeginInit();
            this.tabItemInfo.SuspendLayout();
            this.xtpBarcodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItemBarcodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemBarcodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemBarcodeMeasurementUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditItemBarcodeRelationFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemBarcodeBarcodeType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_ItemBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            this.xtpPriceCatalogs.SuspendLayout();
            this.xtpItemCategories.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItemCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryNode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryRepository)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_category)).BeginInit();
            this.xtpChildItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridChildItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewChildItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItemGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ChildItem)).BeginInit();
            this.xtpLinkedItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLinkedItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewLinkedItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLinkedItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLinkedItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLinkedItemsQuantityFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_LinkedItem)).BeginInit();
            this.xtpSubItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSubItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSubItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemLinkedTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmbItemLinkedTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemLinkedToName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemsLinkedToQuantityFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_SubItem)).BeginInit();
            this.xtpStores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItemStores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemStores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStoreStore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStoreStoreName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ItemStore)).BeginInit();
            this.xtpItemStocks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItemStocks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemStocks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStockStore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColItemStockQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStockBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ItemStock)).BeginInit();
            this.xtraTabPageItemExtraInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIngredients.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiresAt.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiresAt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPackedAt.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPackedAt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLot.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrigin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditItemExtraInfoDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoLot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoOrigin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackedAt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExpiresAt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIngredients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageEditItemImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlHeader)).BeginInit();
            this.panelControlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeasonality.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueAcceptsCustomPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueVatCategory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExtraDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePackingMeasurementUnit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAcceptsCustomDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPoints.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackingQty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInsertedOn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInsertedOn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDefaultBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxOrderQty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBuyer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReferenceUnit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContentUnit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsCentralStored.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinOrderQty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMotherCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueItemSupplier.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExtraFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupAllItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMotherCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcVatCategory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDefaultBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcInsertedOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackingMeasurementUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackingQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemSupplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMinOrderQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMaxOrderQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBreakOrderToCentral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExtraDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSeasonality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReferenceUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcContentUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAcceptsCustomPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExtraFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelChildItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // grdViewPriceCatalogDetailTimeValues
            // 
            this.grdViewPriceCatalogDetailTimeValues.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColDateFrom,
            this.grdColDateTo,
            this.grdColTimeValue,
            this.gridColumnIsActive,
            this.gridColumnDeletePriceCatalogDetailTimeValue});
            this.grdViewPriceCatalogDetailTimeValues.GridControl = this.gridPriceCatalogs;
            this.grdViewPriceCatalogDetailTimeValues.Name = "grdViewPriceCatalogDetailTimeValues";
            this.grdViewPriceCatalogDetailTimeValues.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.grdViewPriceCatalogDetailTimeValues.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.grdViewPriceCatalogDetailTimeValues.OptionsBehavior.AutoPopulateColumns = false;
            this.grdViewPriceCatalogDetailTimeValues.OptionsDetail.EnableMasterViewMode = false;
            this.grdViewPriceCatalogDetailTimeValues.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewPriceCatalogDetailTimeValues.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewPriceCatalogDetailTimeValues_CustomDrawCell);
            this.grdViewPriceCatalogDetailTimeValues.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.grdViewPriceCatalogDetailTimeValues_ShowingEditor);
            // 
            // grdColDateFrom
            // 
            this.grdColDateFrom.Caption = "@@TimeValueValidFromDate";
            this.grdColDateFrom.ColumnEdit = this.dtTimeValueFrom;
            this.grdColDateFrom.FieldName = "TimeValueValidFromDate";
            this.grdColDateFrom.Name = "grdColDateFrom";
            this.grdColDateFrom.Visible = true;
            this.grdColDateFrom.VisibleIndex = 0;
            // 
            // dtTimeValueFrom
            // 
            this.dtTimeValueFrom.AutoHeight = false;
            this.dtTimeValueFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeValueFrom.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.dtTimeValueFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeValueFrom.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeValueFrom.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeValueFrom.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeValueFrom.Name = "dtTimeValueFrom";
            // 
            // grdColDateTo
            // 
            this.grdColDateTo.Caption = "@@TimeValueValidUntilDate";
            this.grdColDateTo.ColumnEdit = this.dtTimeValueUntil;
            this.grdColDateTo.FieldName = "TimeValueValidUntilDate";
            this.grdColDateTo.Name = "grdColDateTo";
            this.grdColDateTo.Visible = true;
            this.grdColDateTo.VisibleIndex = 1;
            // 
            // dtTimeValueUntil
            // 
            this.dtTimeValueUntil.AutoHeight = false;
            this.dtTimeValueUntil.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeValueUntil.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.dtTimeValueUntil.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeValueUntil.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeValueUntil.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeValueUntil.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeValueUntil.Name = "dtTimeValueUntil";
            // 
            // grdColTimeValue
            // 
            this.grdColTimeValue.Caption = "@@TimeValue";
            this.grdColTimeValue.ColumnEdit = this.repositoryItemCalcEditPriceCatalogDetailTimeValue;
            this.grdColTimeValue.FieldName = "TimeValue";
            this.grdColTimeValue.Name = "grdColTimeValue";
            this.grdColTimeValue.Visible = true;
            this.grdColTimeValue.VisibleIndex = 2;
            // 
            // repositoryItemCalcEditPriceCatalogDetailTimeValue
            // 
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue.AutoHeight = false;
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue.Name = "repositoryItemCalcEditPriceCatalogDetailTimeValue";
            // 
            // gridColumnIsActive
            // 
            this.gridColumnIsActive.Caption = "@@ActiveFemale";
            this.gridColumnIsActive.FieldName = "IsActive";
            this.gridColumnIsActive.Name = "gridColumnIsActive";
            this.gridColumnIsActive.Visible = true;
            this.gridColumnIsActive.VisibleIndex = 3;
            // 
            // gridColumnDeletePriceCatalogDetailTimeValue
            // 
            this.gridColumnDeletePriceCatalogDetailTimeValue.ColumnEdit = this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue;
            this.gridColumnDeletePriceCatalogDetailTimeValue.Name = "gridColumnDeletePriceCatalogDetailTimeValue";
            this.gridColumnDeletePriceCatalogDetailTimeValue.Visible = true;
            this.gridColumnDeletePriceCatalogDetailTimeValue.VisibleIndex = 4;
            // 
            // repositoryItemButtonEditDeletePriceCatalogDetailTimeValue
            // 
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue.AutoHeight = false;
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Delete_Sign_24, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue.Name = "repositoryItemButtonEditDeletePriceCatalogDetailTimeValue";
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue_ButtonClick);
            // 
            // gridPriceCatalogs
            // 
            this.gridPriceCatalogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPriceCatalogs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            gridLevelNode1.LevelTemplate = this.grdViewPriceCatalogDetailTimeValues;
            gridLevelNode1.RelationName = "TimeValues";
            this.gridPriceCatalogs.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gridPriceCatalogs.Location = new System.Drawing.Point(0, 0);
            this.gridPriceCatalogs.MainView = this.grdViewPriceCatalogDetails;
            this.gridPriceCatalogs.Name = "gridPriceCatalogs";
            this.gridPriceCatalogs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_delete_PriceCatalog,
            this.cmbPriceCatalog,
            this.txtDatabaseValue,
            this.cmbBarcode,
            this.txtDiscount,
            this.txtTimeValue,
            this.dtTimeValueFrom,
            this.dtTimeValueUntil,
            this.txtMarkup,
            this.chkVatIncluded,
            this.chkPcdIsActive,
            this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue,
            this.repositoryItemCalcEditPriceCatalogDetailTimeValue});
            this.gridPriceCatalogs.Size = new System.Drawing.Size(1134, 367);
            this.gridPriceCatalogs.TabIndex = 0;
            this.gridPriceCatalogs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewPriceCatalogDetails,
            this.grdViewPriceCatalogDetailTimeValues});
            // 
            // grdViewPriceCatalogDetails
            // 
            this.grdViewPriceCatalogDetails.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColPriceCatalog,
            this.grdColDatabaseValue,
            this.grdColBarcode,
            this.grdColPriceCatalogCreatedOn,
            this.grdColPriceCatalogUpdatedOn,
            this.grdColDiscount,
            this.grdColMarkUp,
            this.grdColVatIncluded,
            this.grdColumnPriceCatalogDetailIsActive,
            this.grdColDeletePriceCatalogDetail});
            this.grdViewPriceCatalogDetails.GridControl = this.gridPriceCatalogs;
            this.grdViewPriceCatalogDetails.Name = "grdViewPriceCatalogDetails";
            this.grdViewPriceCatalogDetails.OptionsBehavior.AutoPopulateColumns = false;
            this.grdViewPriceCatalogDetails.OptionsDetail.AllowExpandEmptyDetails = true;
            this.grdViewPriceCatalogDetails.OptionsDetail.AllowZoomDetail = false;
            this.grdViewPriceCatalogDetails.OptionsDetail.ShowDetailTabs = false;
            this.grdViewPriceCatalogDetails.OptionsDetail.SmartDetailExpand = false;
            this.grdViewPriceCatalogDetails.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewPriceCatalogDetails.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewPriceCatalogs_CustomDrawCell);
            this.grdViewPriceCatalogDetails.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.grdViewPriceCatalogs_ShowingEditor);
            this.grdViewPriceCatalogDetails.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewPriceCatalogs_ValidateRow);
            // 
            // grdColPriceCatalog
            // 
            this.grdColPriceCatalog.Caption = "@@PriceCatalog";
            this.grdColPriceCatalog.ColumnEdit = this.cmbPriceCatalog;
            this.grdColPriceCatalog.FieldName = "PriceCatalog";
            this.grdColPriceCatalog.Name = "grdColPriceCatalog";
            this.grdColPriceCatalog.Visible = true;
            this.grdColPriceCatalog.VisibleIndex = 0;
            this.grdColPriceCatalog.Width = 173;
            // 
            // cmbPriceCatalog
            // 
            this.cmbPriceCatalog.AutoHeight = false;
            this.cmbPriceCatalog.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbPriceCatalog.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description")});
            this.cmbPriceCatalog.DisplayMember = "Description";
            this.cmbPriceCatalog.Name = "cmbPriceCatalog";
            this.cmbPriceCatalog.NullText = "";
            this.cmbPriceCatalog.ValueMember = "Oid";
            // 
            // grdColDatabaseValue
            // 
            this.grdColDatabaseValue.Caption = "@@Value";
            this.grdColDatabaseValue.ColumnEdit = this.txtDatabaseValue;
            this.grdColDatabaseValue.DisplayFormat.FormatString = "f3";
            this.grdColDatabaseValue.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.grdColDatabaseValue.FieldName = "DatabaseValue";
            this.grdColDatabaseValue.Name = "grdColDatabaseValue";
            this.grdColDatabaseValue.Visible = true;
            this.grdColDatabaseValue.VisibleIndex = 1;
            this.grdColDatabaseValue.Width = 65;
            // 
            // txtDatabaseValue
            // 
            this.txtDatabaseValue.AutoHeight = false;
            this.txtDatabaseValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDatabaseValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.txtDatabaseValue.Name = "txtDatabaseValue";
            // 
            // grdColBarcode
            // 
            this.grdColBarcode.Caption = "@@Barcode";
            this.grdColBarcode.ColumnEdit = this.cmbBarcode;
            this.grdColBarcode.FieldName = "Barcode";
            this.grdColBarcode.Name = "grdColBarcode";
            this.grdColBarcode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
            this.grdColBarcode.Visible = true;
            this.grdColBarcode.VisibleIndex = 2;
            this.grdColBarcode.Width = 65;
            // 
            // cmbBarcode
            // 
            this.cmbBarcode.AutoHeight = false;
            this.cmbBarcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbBarcode.DisplayMember = "Code";
            this.cmbBarcode.Name = "cmbBarcode";
            this.cmbBarcode.NullText = "";
            this.cmbBarcode.ValueMember = "Oid";
            // 
            // grdColPriceCatalogCreatedOn
            // 
            this.grdColPriceCatalogCreatedOn.Caption = "@@Created";
            this.grdColPriceCatalogCreatedOn.FieldName = "CreatedOn";
            this.grdColPriceCatalogCreatedOn.Name = "grdColPriceCatalogCreatedOn";
            this.grdColPriceCatalogCreatedOn.OptionsColumn.AllowEdit = false;
            this.grdColPriceCatalogCreatedOn.Visible = true;
            this.grdColPriceCatalogCreatedOn.VisibleIndex = 3;
            // 
            // grdColPriceCatalogUpdatedOn
            // 
            this.grdColPriceCatalogUpdatedOn.Caption = "@@UpdatedOn";
            this.grdColPriceCatalogUpdatedOn.FieldName = "UpdatedOn";
            this.grdColPriceCatalogUpdatedOn.Name = "grdColPriceCatalogUpdatedOn";
            this.grdColPriceCatalogUpdatedOn.OptionsColumn.AllowEdit = false;
            this.grdColPriceCatalogUpdatedOn.Visible = true;
            this.grdColPriceCatalogUpdatedOn.VisibleIndex = 4;
            // 
            // grdColDiscount
            // 
            this.grdColDiscount.Caption = "@@Discount";
            this.grdColDiscount.ColumnEdit = this.txtDiscount;
            this.grdColDiscount.DisplayFormat.FormatString = "p";
            this.grdColDiscount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.grdColDiscount.FieldName = "Discount";
            this.grdColDiscount.Name = "grdColDiscount";
            this.grdColDiscount.Visible = true;
            this.grdColDiscount.VisibleIndex = 5;
            this.grdColDiscount.Width = 84;
            // 
            // txtDiscount
            // 
            this.txtDiscount.AutoHeight = false;
            this.txtDiscount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDiscount.DisplayFormat.FormatString = "p";
            this.txtDiscount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDiscount.EditFormat.FormatString = "p";
            this.txtDiscount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDiscount.Mask.EditMask = "p";
            this.txtDiscount.Name = "txtDiscount";
            // 
            // grdColMarkUp
            // 
            this.grdColMarkUp.Caption = "@@MarkUp";
            this.grdColMarkUp.ColumnEdit = this.txtMarkup;
            this.grdColMarkUp.FieldName = "MarkUp";
            this.grdColMarkUp.Name = "grdColMarkUp";
            this.grdColMarkUp.Visible = true;
            this.grdColMarkUp.VisibleIndex = 6;
            this.grdColMarkUp.Width = 81;
            // 
            // txtMarkup
            // 
            this.txtMarkup.AutoHeight = false;
            this.txtMarkup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtMarkup.DisplayFormat.FormatString = "p";
            this.txtMarkup.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtMarkup.EditFormat.FormatString = "p";
            this.txtMarkup.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtMarkup.Mask.EditMask = "p";
            this.txtMarkup.Name = "txtMarkup";
            // 
            // grdColVatIncluded
            // 
            this.grdColVatIncluded.Caption = "@@VatIncluded";
            this.grdColVatIncluded.ColumnEdit = this.chkVatIncluded;
            this.grdColVatIncluded.FieldName = "VATIncluded";
            this.grdColVatIncluded.Name = "grdColVatIncluded";
            this.grdColVatIncluded.Visible = true;
            this.grdColVatIncluded.VisibleIndex = 7;
            this.grdColVatIncluded.Width = 81;
            // 
            // chkVatIncluded
            // 
            this.chkVatIncluded.AutoHeight = false;
            this.chkVatIncluded.Name = "chkVatIncluded";
            // 
            // grdColumnPriceCatalogDetailIsActive
            // 
            this.grdColumnPriceCatalogDetailIsActive.Caption = "@@IsActive";
            this.grdColumnPriceCatalogDetailIsActive.ColumnEdit = this.chkPcdIsActive;
            this.grdColumnPriceCatalogDetailIsActive.FieldName = "IsActive";
            this.grdColumnPriceCatalogDetailIsActive.Name = "grdColumnPriceCatalogDetailIsActive";
            this.grdColumnPriceCatalogDetailIsActive.Visible = true;
            this.grdColumnPriceCatalogDetailIsActive.VisibleIndex = 8;
            this.grdColumnPriceCatalogDetailIsActive.Width = 79;
            // 
            // chkPcdIsActive
            // 
            this.chkPcdIsActive.AutoHeight = false;
            this.chkPcdIsActive.Name = "chkPcdIsActive";
            // 
            // grdColDeletePriceCatalogDetail
            // 
            this.grdColDeletePriceCatalogDetail.ColumnEdit = this.btn_delete_PriceCatalog;
            this.grdColDeletePriceCatalogDetail.Name = "grdColDeletePriceCatalogDetail";
            this.grdColDeletePriceCatalogDetail.Visible = true;
            this.grdColDeletePriceCatalogDetail.VisibleIndex = 9;
            this.grdColDeletePriceCatalogDetail.Width = 59;
            // 
            // btn_delete_PriceCatalog
            // 
            this.btn_delete_PriceCatalog.AutoHeight = false;
            this.btn_delete_PriceCatalog.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_delete_PriceCatalog.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, true)});
            this.btn_delete_PriceCatalog.Name = "btn_delete_PriceCatalog";
            this.btn_delete_PriceCatalog.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_delete_PriceCatalog.UseReadOnlyAppearance = false;
            this.btn_delete_PriceCatalog.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_delete_PriceCatalog_ButtonClick);
            // 
            // txtTimeValue
            // 
            this.txtTimeValue.AutoHeight = false;
            this.txtTimeValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtTimeValue.Name = "txtTimeValue";
            // 
            // btn_Del_SubItem
            // 
            this.btn_Del_SubItem.AutoHeight = false;
            this.btn_Del_SubItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_Del_SubItem.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true)});
            this.btn_Del_SubItem.Name = "btn_Del_SubItem";
            this.btn_Del_SubItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // layoutControlHeader
            // 
            this.layoutControlHeader.Controls.Add(this.DoesNotAllowDiscount);
            this.layoutControlHeader.Controls.Add(this.IsTax);
            this.layoutControlHeader.Controls.Add(this.layoutControl1);
            this.layoutControlHeader.Controls.Add(this.imageEditItemImage);
            this.layoutControlHeader.Controls.Add(this.txtCode);
            this.layoutControlHeader.Controls.Add(this.txtName);
            this.layoutControlHeader.Controls.Add(this.chkIsActive);
            this.layoutControlHeader.Controls.Add(this.panelControlHeader);
            this.layoutControlHeader.Controls.Add(this.lueSeasonality);
            this.layoutControlHeader.Controls.Add(this.lueAcceptsCustomPrice);
            this.layoutControlHeader.Controls.Add(this.lueVatCategory);
            this.layoutControlHeader.Controls.Add(this.txtExtraDescription);
            this.layoutControlHeader.Controls.Add(this.luePackingMeasurementUnit);
            this.layoutControlHeader.Controls.Add(this.chkAcceptsCustomDescription);
            this.layoutControlHeader.Controls.Add(this.txtPoints);
            this.layoutControlHeader.Controls.Add(this.txtPackingQty);
            this.layoutControlHeader.Controls.Add(this.dtInsertedOn);
            this.layoutControlHeader.Controls.Add(this.lueDefaultBarcode);
            this.layoutControlHeader.Controls.Add(this.txtMaxOrderQty);
            this.layoutControlHeader.Controls.Add(this.lueBuyer);
            this.layoutControlHeader.Controls.Add(this.txtReferenceUnit);
            this.layoutControlHeader.Controls.Add(this.txtContentUnit);
            this.layoutControlHeader.Controls.Add(this.chkIsCentralStored);
            this.layoutControlHeader.Controls.Add(this.memoRemarks);
            this.layoutControlHeader.Controls.Add(this.txtMinOrderQty);
            this.layoutControlHeader.Controls.Add(this.lueMotherCode);
            this.layoutControlHeader.Controls.Add(this.lueItemSupplier);
            this.layoutControlHeader.Controls.Add(this.txtExtraFile);
            this.layoutControlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlHeader.Location = new System.Drawing.Point(0, 0);
            this.layoutControlHeader.Name = "layoutControlHeader";
            this.layoutControlHeader.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(476, 0, 861, 655);
            this.layoutControlHeader.Root = this.layoutControlGroupAllItems;
            this.layoutControlHeader.Size = new System.Drawing.Size(1204, 859);
            this.layoutControlHeader.TabIndex = 0;
            this.layoutControlHeader.Text = "layoutControl1";
            // 
            // DoesNotAllowDiscount
            // 
            this.DoesNotAllowDiscount.Location = new System.Drawing.Point(22, 375);
            this.DoesNotAllowDiscount.Name = "DoesNotAllowDiscount";
            this.DoesNotAllowDiscount.Properties.Caption = "@@DoesNotAllowDiscount";
            this.DoesNotAllowDiscount.Size = new System.Drawing.Size(287, 19);
            this.DoesNotAllowDiscount.StyleController = this.layoutControlHeader;
            this.DoesNotAllowDiscount.TabIndex = 15;
            // 
            // IsTax
            // 
            this.IsTax.Location = new System.Drawing.Point(313, 375);
            this.IsTax.Name = "IsTax";
            this.IsTax.Properties.Caption = "@@IsTax";
            this.IsTax.Size = new System.Drawing.Size(287, 19);
            this.IsTax.StyleController = this.layoutControlHeader;
            this.IsTax.TabIndex = 14;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tabItemInfo);
            this.layoutControl1.Location = new System.Drawing.Point(22, 398);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(956, 306, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup2;
            this.layoutControl1.Size = new System.Drawing.Size(1160, 439);
            this.layoutControl1.TabIndex = 13;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tabItemInfo
            // 
            this.tabItemInfo.Location = new System.Drawing.Point(12, 12);
            this.tabItemInfo.Name = "tabItemInfo";
            this.tabItemInfo.SelectedTabPage = this.xtpBarcodes;
            this.tabItemInfo.Size = new System.Drawing.Size(1136, 415);
            this.tabItemInfo.TabIndex = 3;
            this.tabItemInfo.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtpBarcodes,
            this.xtpPriceCatalogs,
            this.xtpItemCategories,
            this.xtpChildItems,
            this.xtpLinkedItems,
            this.xtpSubItems,
            this.xtpStores,
            this.xtpItemStocks,
            this.xtraTabPageItemExtraInfo});
            // 
            // xtpBarcodes
            // 
            this.xtpBarcodes.Controls.Add(this.gridItemBarcodes);
            this.xtpBarcodes.Name = "xtpBarcodes";
            this.xtpBarcodes.Size = new System.Drawing.Size(1134, 390);
            this.xtpBarcodes.Text = "@@Barcodes";
            // 
            // gridItemBarcodes
            // 
            this.gridItemBarcodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItemBarcodes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridItemBarcodes.Location = new System.Drawing.Point(0, 0);
            this.gridItemBarcodes.MainView = this.grdViewItemBarcodes;
            this.gridItemBarcodes.Name = "gridItemBarcodes";
            this.gridItemBarcodes.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_delete_ItemBarcode,
            this.cmbItemBarcodeMeasurementUnit,
            this.spinEditItemBarcodeRelationFactor,
            this.repositoryItemComboBox1,
            this.cmbItemBarcodeBarcodeType});
            this.gridItemBarcodes.Size = new System.Drawing.Size(1134, 390);
            this.gridItemBarcodes.TabIndex = 0;
            this.gridItemBarcodes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewItemBarcodes});
            this.gridItemBarcodes.Click += new System.EventHandler(this.gridItemBarcodes_Click);
            // 
            // grdViewItemBarcodes
            // 
            this.grdViewItemBarcodes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColBarcodeCode,
            this.grdColBarcodeMeasurementUnit,
            this.grdColCreatedOn,
            this.grdColUpdatedOn,
            this.grdColPLUCode,
            this.grdColPLUPrefix,
            this.grdColBarcodeRelationFactor,
            this.grdColBarcodeType,
            this.grdColDeleteItemBarcode});
            this.grdViewItemBarcodes.GridControl = this.gridItemBarcodes;
            this.grdViewItemBarcodes.Name = "grdViewItemBarcodes";
            this.grdViewItemBarcodes.OptionsMenu.EnableColumnMenu = false;
            this.grdViewItemBarcodes.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemBarcodes.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemBarcodes.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewItemBarcodes.OptionsView.ShowDetailButtons = false;
            this.grdViewItemBarcodes.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewItemBarcodes.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewItemBarcodes_CustomDrawCell);
            this.grdViewItemBarcodes.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewItemBarcodes_ValidateRow);
            // 
            // grdColBarcodeCode
            // 
            this.grdColBarcodeCode.Caption = "@@Code";
            this.grdColBarcodeCode.FieldName = "Barcode.Code";
            this.grdColBarcodeCode.Name = "grdColBarcodeCode";
            this.grdColBarcodeCode.Visible = true;
            this.grdColBarcodeCode.VisibleIndex = 0;
            this.grdColBarcodeCode.Width = 252;
            // 
            // grdColBarcodeMeasurementUnit
            // 
            this.grdColBarcodeMeasurementUnit.Caption = "@@MeasurementUnit";
            this.grdColBarcodeMeasurementUnit.ColumnEdit = this.cmbItemBarcodeMeasurementUnit;
            this.grdColBarcodeMeasurementUnit.DisplayFormat.FormatString = "d";
            this.grdColBarcodeMeasurementUnit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.grdColBarcodeMeasurementUnit.FieldName = "MeasurementUnit";
            this.grdColBarcodeMeasurementUnit.Name = "grdColBarcodeMeasurementUnit";
            this.grdColBarcodeMeasurementUnit.Visible = true;
            this.grdColBarcodeMeasurementUnit.VisibleIndex = 1;
            this.grdColBarcodeMeasurementUnit.Width = 94;
            // 
            // cmbItemBarcodeMeasurementUnit
            // 
            this.cmbItemBarcodeMeasurementUnit.AutoHeight = false;
            this.cmbItemBarcodeMeasurementUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemBarcodeMeasurementUnit.DisplayMember = "Description";
            this.cmbItemBarcodeMeasurementUnit.Name = "cmbItemBarcodeMeasurementUnit";
            this.cmbItemBarcodeMeasurementUnit.NullText = "";
            this.cmbItemBarcodeMeasurementUnit.ValueMember = "Oid";
            // 
            // grdColCreatedOn
            // 
            this.grdColCreatedOn.Caption = "@@Created";
            this.grdColCreatedOn.FieldName = "CreatedOn";
            this.grdColCreatedOn.Name = "grdColCreatedOn";
            this.grdColCreatedOn.Visible = true;
            this.grdColCreatedOn.VisibleIndex = 2;
            // 
            // grdColUpdatedOn
            // 
            this.grdColUpdatedOn.Caption = "@@UpdatedOn";
            this.grdColUpdatedOn.FieldName = "UpdatedOn";
            this.grdColUpdatedOn.Name = "grdColUpdatedOn";
            this.grdColUpdatedOn.Visible = true;
            this.grdColUpdatedOn.VisibleIndex = 3;
            // 
            // grdColPLUCode
            // 
            this.grdColPLUCode.Caption = "PLU Code";
            this.grdColPLUCode.FieldName = "PluCode";
            this.grdColPLUCode.Name = "grdColPLUCode";
            this.grdColPLUCode.Visible = true;
            this.grdColPLUCode.VisibleIndex = 4;
            this.grdColPLUCode.Width = 94;
            // 
            // grdColPLUPrefix
            // 
            this.grdColPLUPrefix.Caption = "PLU Prefix";
            this.grdColPLUPrefix.FieldName = "PluPrefix";
            this.grdColPLUPrefix.Name = "grdColPLUPrefix";
            this.grdColPLUPrefix.Visible = true;
            this.grdColPLUPrefix.VisibleIndex = 5;
            this.grdColPLUPrefix.Width = 94;
            // 
            // grdColBarcodeRelationFactor
            // 
            this.grdColBarcodeRelationFactor.Caption = "@@RelationFactor";
            this.grdColBarcodeRelationFactor.ColumnEdit = this.spinEditItemBarcodeRelationFactor;
            this.grdColBarcodeRelationFactor.FieldName = "RelationFactor";
            this.grdColBarcodeRelationFactor.Name = "grdColBarcodeRelationFactor";
            this.grdColBarcodeRelationFactor.Visible = true;
            this.grdColBarcodeRelationFactor.VisibleIndex = 6;
            this.grdColBarcodeRelationFactor.Width = 94;
            // 
            // spinEditItemBarcodeRelationFactor
            // 
            this.spinEditItemBarcodeRelationFactor.AutoHeight = false;
            this.spinEditItemBarcodeRelationFactor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinEditItemBarcodeRelationFactor.Name = "spinEditItemBarcodeRelationFactor";
            // 
            // grdColBarcodeType
            // 
            this.grdColBarcodeType.Caption = "@@Type";
            this.grdColBarcodeType.ColumnEdit = this.cmbItemBarcodeBarcodeType;
            this.grdColBarcodeType.FieldName = "Type";
            this.grdColBarcodeType.Name = "grdColBarcodeType";
            this.grdColBarcodeType.Visible = true;
            this.grdColBarcodeType.VisibleIndex = 7;
            this.grdColBarcodeType.Width = 94;
            // 
            // cmbItemBarcodeBarcodeType
            // 
            this.cmbItemBarcodeBarcodeType.AutoHeight = false;
            this.cmbItemBarcodeBarcodeType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemBarcodeBarcodeType.DisplayMember = "Description";
            this.cmbItemBarcodeBarcodeType.Name = "cmbItemBarcodeBarcodeType";
            this.cmbItemBarcodeBarcodeType.NullText = "";
            this.cmbItemBarcodeBarcodeType.ValueMember = "Oid";
            // 
            // grdColDeleteItemBarcode
            // 
            this.grdColDeleteItemBarcode.ColumnEdit = this.btn_delete_ItemBarcode;
            this.grdColDeleteItemBarcode.Name = "grdColDeleteItemBarcode";
            this.grdColDeleteItemBarcode.Visible = true;
            this.grdColDeleteItemBarcode.VisibleIndex = 8;
            this.grdColDeleteItemBarcode.Width = 53;
            // 
            // btn_delete_ItemBarcode
            // 
            this.btn_delete_ItemBarcode.AutoHeight = false;
            this.btn_delete_ItemBarcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_delete_ItemBarcode.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject4, "", null, null, true)});
            this.btn_delete_ItemBarcode.Name = "btn_delete_ItemBarcode";
            this.btn_delete_ItemBarcode.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_delete_ItemBarcode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_delete_ItemBarcode_ButtonClick);
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // xtpPriceCatalogs
            // 
            this.xtpPriceCatalogs.Controls.Add(this.gridPriceCatalogs);
            this.xtpPriceCatalogs.Name = "xtpPriceCatalogs";
            this.xtpPriceCatalogs.Size = new System.Drawing.Size(1134, 367);
            this.xtpPriceCatalogs.Text = "@@Prices";
            // 
            // xtpItemCategories
            // 
            this.xtpItemCategories.Controls.Add(this.gridItemCategories);
            this.xtpItemCategories.Name = "xtpItemCategories";
            this.xtpItemCategories.Size = new System.Drawing.Size(1134, 367);
            this.xtpItemCategories.Text = "@@Categories";
            // 
            // gridItemCategories
            // 
            this.gridItemCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItemCategories.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridItemCategories.Location = new System.Drawing.Point(0, 0);
            this.gridItemCategories.MainView = this.grdViewItemCategories;
            this.gridItemCategories.Name = "gridItemCategories";
            this.gridItemCategories.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_delete_category,
            this.TreeListItemCategoryNode});
            this.gridItemCategories.Size = new System.Drawing.Size(1134, 367);
            this.gridItemCategories.TabIndex = 0;
            this.gridItemCategories.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewItemCategories});
            // 
            // grdViewItemCategories
            // 
            this.grdViewItemCategories.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColCategoryNode,
            this.grdColCategoryRoot,
            this.gridColCategoryPath,
            this.gridColDelItemCategory});
            this.grdViewItemCategories.GridControl = this.gridItemCategories;
            this.grdViewItemCategories.Name = "grdViewItemCategories";
            this.grdViewItemCategories.OptionsMenu.EnableColumnMenu = false;
            this.grdViewItemCategories.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemCategories.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemCategories.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewItemCategories.OptionsView.ShowDetailButtons = false;
            this.grdViewItemCategories.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewItemCategories.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewItemCategories_CustomDrawCell);
            this.grdViewItemCategories.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewItemCategories_ValidateRow);
            // 
            // gridColCategoryNode
            // 
            this.gridColCategoryNode.Caption = "@@CategoryNode";
            this.gridColCategoryNode.ColumnEdit = this.TreeListItemCategoryNode;
            this.gridColCategoryNode.FieldName = "Node!Key";
            this.gridColCategoryNode.Name = "gridColCategoryNode";
            this.gridColCategoryNode.Visible = true;
            this.gridColCategoryNode.VisibleIndex = 0;
            // 
            // TreeListItemCategoryNode
            // 
            this.TreeListItemCategoryNode.AutoHeight = false;
            this.TreeListItemCategoryNode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.TreeListItemCategoryNode.DisplayMember = "Description";
            this.TreeListItemCategoryNode.Name = "TreeListItemCategoryNode";
            this.TreeListItemCategoryNode.NullText = "";
            this.TreeListItemCategoryNode.TreeList = this.TreeListItemCategoryRepository;
            this.TreeListItemCategoryNode.ValueMember = "Oid";
            // 
            // TreeListItemCategoryRepository
            // 
            this.TreeListItemCategoryRepository.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2});
            this.TreeListItemCategoryRepository.KeyFieldName = "Oid";
            this.TreeListItemCategoryRepository.Location = new System.Drawing.Point(0, 0);
            this.TreeListItemCategoryRepository.Name = "TreeListItemCategoryRepository";
            this.TreeListItemCategoryRepository.OptionsBehavior.EnableFiltering = true;
            this.TreeListItemCategoryRepository.OptionsView.ShowIndentAsRowStyle = true;
            this.TreeListItemCategoryRepository.ParentFieldName = "ParentOid";
            this.TreeListItemCategoryRepository.Size = new System.Drawing.Size(400, 200);
            this.TreeListItemCategoryRepository.TabIndex = 0;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "@@Code";
            this.treeListColumn1.FieldName = "Code";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "@@Description";
            this.treeListColumn2.FieldName = "Description";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            // 
            // grdColCategoryRoot
            // 
            this.grdColCategoryRoot.Caption = "@@CategoryRoot";
            this.grdColCategoryRoot.FieldName = "Root.Description";
            this.grdColCategoryRoot.Name = "grdColCategoryRoot";
            this.grdColCategoryRoot.OptionsColumn.AllowEdit = false;
            this.grdColCategoryRoot.Visible = true;
            this.grdColCategoryRoot.VisibleIndex = 1;
            // 
            // gridColCategoryPath
            // 
            this.gridColCategoryPath.Caption = "@@CategoryPath";
            this.gridColCategoryPath.FieldName = "CategoryPath";
            this.gridColCategoryPath.Name = "gridColCategoryPath";
            this.gridColCategoryPath.OptionsColumn.AllowEdit = false;
            this.gridColCategoryPath.Visible = true;
            this.gridColCategoryPath.VisibleIndex = 2;
            // 
            // gridColDelItemCategory
            // 
            this.gridColDelItemCategory.ColumnEdit = this.btn_delete_category;
            this.gridColDelItemCategory.Name = "gridColDelItemCategory";
            this.gridColDelItemCategory.Visible = true;
            this.gridColDelItemCategory.VisibleIndex = 3;
            // 
            // btn_delete_category
            // 
            this.btn_delete_category.AutoHeight = false;
            this.btn_delete_category.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_delete_category.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, "", null, null, true)});
            this.btn_delete_category.Name = "btn_delete_category";
            this.btn_delete_category.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_delete_category.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_delete_category_ButtonClick);
            // 
            // xtpChildItems
            // 
            this.xtpChildItems.Controls.Add(this.gridChildItems);
            this.xtpChildItems.Name = "xtpChildItems";
            this.xtpChildItems.Size = new System.Drawing.Size(1134, 367);
            this.xtpChildItems.Text = "@@ChildItems";
            // 
            // gridChildItems
            // 
            this.gridChildItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridChildItems.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridChildItems.Location = new System.Drawing.Point(0, 0);
            this.gridChildItems.MainView = this.grdViewChildItems;
            this.gridChildItems.Name = "gridChildItems";
            this.gridChildItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cmbChildItem,
            this.btn_Del_ChildItem,
            this.cmbChildItemName});
            this.gridChildItems.Size = new System.Drawing.Size(1134, 367);
            this.gridChildItems.TabIndex = 0;
            this.gridChildItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewChildItems});
            // 
            // grdViewChildItems
            // 
            this.grdViewChildItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColChildItemCode,
            this.grdColChildItemName,
            this.grdColDeleteChildItem});
            this.grdViewChildItems.GridControl = this.gridChildItems;
            this.grdViewChildItems.Name = "grdViewChildItems";
            this.grdViewChildItems.OptionsMenu.EnableColumnMenu = false;
            this.grdViewChildItems.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewChildItems.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewChildItems.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewChildItems.OptionsView.ShowDetailButtons = false;
            this.grdViewChildItems.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewChildItems.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewChildItems_CustomDrawCell);
            // 
            // grdColChildItemCode
            // 
            this.grdColChildItemCode.Caption = "@@Item";
            this.grdColChildItemCode.ColumnEdit = this.cmbChildItem;
            this.grdColChildItemCode.FieldName = "Item";
            this.grdColChildItemCode.Name = "grdColChildItemCode";
            this.grdColChildItemCode.Visible = true;
            this.grdColChildItemCode.VisibleIndex = 0;
            this.grdColChildItemCode.Width = 150;
            // 
            // cmbChildItem
            // 
            this.cmbChildItem.AutoHeight = false;
            this.cmbChildItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbChildItem.DisplayMember = "Code";
            this.cmbChildItem.Name = "cmbChildItem";
            this.cmbChildItem.ValueMember = "Oid";
            this.cmbChildItem.View = this.cmbChildItemGridView;
            // 
            // cmbChildItemGridView
            // 
            this.cmbChildItemGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cmbChildItemGridView.Name = "cmbChildItemGridView";
            this.cmbChildItemGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cmbChildItemGridView.OptionsView.ShowGroupPanel = false;
            // 
            // grdColChildItemName
            // 
            this.grdColChildItemName.Caption = "@@Name";
            this.grdColChildItemName.ColumnEdit = this.cmbChildItemName;
            this.grdColChildItemName.FieldName = "Item";
            this.grdColChildItemName.Name = "grdColChildItemName";
            this.grdColChildItemName.OptionsColumn.AllowEdit = false;
            this.grdColChildItemName.Visible = true;
            this.grdColChildItemName.VisibleIndex = 1;
            // 
            // cmbChildItemName
            // 
            this.cmbChildItemName.AutoHeight = false;
            this.cmbChildItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbChildItemName.DisplayMember = "Name";
            this.cmbChildItemName.Name = "cmbChildItemName";
            this.cmbChildItemName.NullText = "";
            this.cmbChildItemName.ValueMember = "Oid";
            // 
            // grdColDeleteChildItem
            // 
            this.grdColDeleteChildItem.ColumnEdit = this.btn_Del_ChildItem;
            this.grdColDeleteChildItem.Name = "grdColDeleteChildItem";
            this.grdColDeleteChildItem.Visible = true;
            this.grdColDeleteChildItem.VisibleIndex = 2;
            // 
            // btn_Del_ChildItem
            // 
            this.btn_Del_ChildItem.AutoHeight = false;
            this.btn_Del_ChildItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_Del_ChildItem.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject6, "", null, null, true)});
            this.btn_Del_ChildItem.Name = "btn_Del_ChildItem";
            this.btn_Del_ChildItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_Del_ChildItem.UseReadOnlyAppearance = false;
            this.btn_Del_ChildItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnDelChildItem_Click);
            // 
            // xtpLinkedItems
            // 
            this.xtpLinkedItems.Controls.Add(this.gridLinkedItems);
            this.xtpLinkedItems.Name = "xtpLinkedItems";
            this.xtpLinkedItems.Size = new System.Drawing.Size(1134, 367);
            this.xtpLinkedItems.Text = "@@LinkedItems";
            // 
            // gridLinkedItems
            // 
            this.gridLinkedItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLinkedItems.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridLinkedItems.Location = new System.Drawing.Point(0, 0);
            this.gridLinkedItems.MainView = this.grdViewLinkedItems;
            this.gridLinkedItems.Name = "gridLinkedItems";
            this.gridLinkedItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_Del_LinkedItem,
            this.cmbLinkedItem,
            this.txtLinkedItemsQuantityFactor,
            this.cmbLinkedItemName});
            this.gridLinkedItems.Size = new System.Drawing.Size(1134, 367);
            this.gridLinkedItems.TabIndex = 0;
            this.gridLinkedItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewLinkedItems});
            // 
            // grdViewLinkedItems
            // 
            this.grdViewLinkedItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColLinkedItemCode,
            this.grdColLinkedItemName,
            this.grdColLinkedItemQtyFactor,
            this.grdColDeleteLinkedItem});
            this.grdViewLinkedItems.GridControl = this.gridLinkedItems;
            this.grdViewLinkedItems.Name = "grdViewLinkedItems";
            this.grdViewLinkedItems.OptionsMenu.EnableColumnMenu = false;
            this.grdViewLinkedItems.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewLinkedItems.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewLinkedItems.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewLinkedItems.OptionsView.ShowDetailButtons = false;
            this.grdViewLinkedItems.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewLinkedItems.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewLinkedItems_CustomDrawCell);
            // 
            // grdColLinkedItemCode
            // 
            this.grdColLinkedItemCode.Caption = "@@Code";
            this.grdColLinkedItemCode.ColumnEdit = this.cmbLinkedItem;
            this.grdColLinkedItemCode.FieldName = "SubItem";
            this.grdColLinkedItemCode.Name = "grdColLinkedItemCode";
            this.grdColLinkedItemCode.Visible = true;
            this.grdColLinkedItemCode.VisibleIndex = 0;
            this.grdColLinkedItemCode.Width = 150;
            // 
            // cmbLinkedItem
            // 
            this.cmbLinkedItem.AutoHeight = false;
            this.cmbLinkedItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbLinkedItem.DisplayMember = "Code";
            this.cmbLinkedItem.Name = "cmbLinkedItem";
            this.cmbLinkedItem.NullText = "";
            this.cmbLinkedItem.ValueMember = "Oid";
            this.cmbLinkedItem.View = this.repositoryItemSearchLookUpEdit1View;
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnLinkedItemCode,
            this.gridColumnLinkedItemName});
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnLinkedItemCode
            // 
            this.gridColumnLinkedItemCode.Caption = "@@Code";
            this.gridColumnLinkedItemCode.FieldName = "Code";
            this.gridColumnLinkedItemCode.Name = "gridColumnLinkedItemCode";
            this.gridColumnLinkedItemCode.Visible = true;
            this.gridColumnLinkedItemCode.VisibleIndex = 1;
            // 
            // gridColumnLinkedItemName
            // 
            this.gridColumnLinkedItemName.Caption = "@@Name";
            this.gridColumnLinkedItemName.FieldName = "Name";
            this.gridColumnLinkedItemName.Name = "gridColumnLinkedItemName";
            this.gridColumnLinkedItemName.Visible = true;
            this.gridColumnLinkedItemName.VisibleIndex = 0;
            // 
            // grdColLinkedItemName
            // 
            this.grdColLinkedItemName.Caption = "@@Name";
            this.grdColLinkedItemName.ColumnEdit = this.cmbLinkedItemName;
            this.grdColLinkedItemName.FieldName = "SubItem";
            this.grdColLinkedItemName.Name = "grdColLinkedItemName";
            this.grdColLinkedItemName.OptionsColumn.AllowEdit = false;
            this.grdColLinkedItemName.Visible = true;
            this.grdColLinkedItemName.VisibleIndex = 1;
            this.grdColLinkedItemName.Width = 57;
            // 
            // cmbLinkedItemName
            // 
            this.cmbLinkedItemName.AutoHeight = false;
            this.cmbLinkedItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject7, "", null, null, true)});
            this.cmbLinkedItemName.DisplayMember = "Name";
            this.cmbLinkedItemName.Name = "cmbLinkedItemName";
            this.cmbLinkedItemName.NullText = "";
            this.cmbLinkedItemName.ValueMember = "Oid";
            // 
            // grdColLinkedItemQtyFactor
            // 
            this.grdColLinkedItemQtyFactor.Caption = "@@QuantityFactor";
            this.grdColLinkedItemQtyFactor.ColumnEdit = this.txtLinkedItemsQuantityFactor;
            this.grdColLinkedItemQtyFactor.FieldName = "QtyFactor";
            this.grdColLinkedItemQtyFactor.Name = "grdColLinkedItemQtyFactor";
            this.grdColLinkedItemQtyFactor.Visible = true;
            this.grdColLinkedItemQtyFactor.VisibleIndex = 2;
            this.grdColLinkedItemQtyFactor.Width = 57;
            // 
            // txtLinkedItemsQuantityFactor
            // 
            this.txtLinkedItemsQuantityFactor.AutoHeight = false;
            this.txtLinkedItemsQuantityFactor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtLinkedItemsQuantityFactor.Name = "txtLinkedItemsQuantityFactor";
            // 
            // grdColDeleteLinkedItem
            // 
            this.grdColDeleteLinkedItem.ColumnEdit = this.btn_Del_LinkedItem;
            this.grdColDeleteLinkedItem.Name = "grdColDeleteLinkedItem";
            this.grdColDeleteLinkedItem.Visible = true;
            this.grdColDeleteLinkedItem.VisibleIndex = 3;
            // 
            // btn_Del_LinkedItem
            // 
            this.btn_Del_LinkedItem.AutoHeight = false;
            this.btn_Del_LinkedItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_Del_LinkedItem.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject8, "", null, null, true)});
            this.btn_Del_LinkedItem.Name = "btn_Del_LinkedItem";
            this.btn_Del_LinkedItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_Del_LinkedItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_Del_LinkedItem_ButtonClick);
            // 
            // xtpSubItems
            // 
            this.xtpSubItems.Controls.Add(this.gridSubItems);
            this.xtpSubItems.Name = "xtpSubItems";
            this.xtpSubItems.Size = new System.Drawing.Size(1134, 367);
            this.xtpSubItems.Text = "@@ItemsLinkedTo";
            // 
            // gridSubItems
            // 
            this.gridSubItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSubItems.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridSubItems.Location = new System.Drawing.Point(0, 0);
            this.gridSubItems.MainView = this.grdViewSubItems;
            this.gridSubItems.Name = "gridSubItems";
            this.gridSubItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_delete_SubItem,
            this.cmbItemLinkedTo,
            this.txtItemsLinkedToQuantityFactor,
            this.cmbItemLinkedToName});
            this.gridSubItems.Size = new System.Drawing.Size(1134, 367);
            this.gridSubItems.TabIndex = 0;
            this.gridSubItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewSubItems});
            // 
            // grdViewSubItems
            // 
            this.grdViewSubItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSubItemCode,
            this.grdColSubItemName,
            this.grdColSubItemQtyFactor,
            this.grdColDeleteSubItem});
            this.grdViewSubItems.GridControl = this.gridSubItems;
            this.grdViewSubItems.Name = "grdViewSubItems";
            this.grdViewSubItems.OptionsMenu.EnableColumnMenu = false;
            this.grdViewSubItems.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewSubItems.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewSubItems.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewSubItems.OptionsView.ShowDetailButtons = false;
            this.grdViewSubItems.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewSubItems.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewSubItems_CustomDrawCell);
            // 
            // grdColSubItemCode
            // 
            this.grdColSubItemCode.Caption = "@@Code";
            this.grdColSubItemCode.ColumnEdit = this.cmbItemLinkedTo;
            this.grdColSubItemCode.FieldName = "Item";
            this.grdColSubItemCode.Name = "grdColSubItemCode";
            this.grdColSubItemCode.Visible = true;
            this.grdColSubItemCode.VisibleIndex = 0;
            this.grdColSubItemCode.Width = 150;
            // 
            // cmbItemLinkedTo
            // 
            this.cmbItemLinkedTo.AutoHeight = false;
            this.cmbItemLinkedTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemLinkedTo.DisplayMember = "Code";
            this.cmbItemLinkedTo.Name = "cmbItemLinkedTo";
            this.cmbItemLinkedTo.NullText = "";
            this.cmbItemLinkedTo.ShowFooter = false;
            this.cmbItemLinkedTo.ValueMember = "Oid";
            this.cmbItemLinkedTo.View = this.gridViewCmbItemLinkedTo;
            // 
            // gridViewCmbItemLinkedTo
            // 
            this.gridViewCmbItemLinkedTo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnItemLinkedToCode,
            this.gridColumnItemLinkedToName});
            this.gridViewCmbItemLinkedTo.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewCmbItemLinkedTo.Name = "gridViewCmbItemLinkedTo";
            this.gridViewCmbItemLinkedTo.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewCmbItemLinkedTo.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnItemLinkedToCode
            // 
            this.gridColumnItemLinkedToCode.Caption = "@@Code";
            this.gridColumnItemLinkedToCode.FieldName = "Code";
            this.gridColumnItemLinkedToCode.Name = "gridColumnItemLinkedToCode";
            this.gridColumnItemLinkedToCode.Visible = true;
            this.gridColumnItemLinkedToCode.VisibleIndex = 0;
            // 
            // gridColumnItemLinkedToName
            // 
            this.gridColumnItemLinkedToName.Caption = "@@Name";
            this.gridColumnItemLinkedToName.FieldName = "Name";
            this.gridColumnItemLinkedToName.Name = "gridColumnItemLinkedToName";
            this.gridColumnItemLinkedToName.Visible = true;
            this.gridColumnItemLinkedToName.VisibleIndex = 1;
            // 
            // grdColSubItemName
            // 
            this.grdColSubItemName.Caption = "@@Name";
            this.grdColSubItemName.ColumnEdit = this.cmbItemLinkedToName;
            this.grdColSubItemName.FieldName = "Item";
            this.grdColSubItemName.Name = "grdColSubItemName";
            this.grdColSubItemName.OptionsColumn.AllowEdit = false;
            this.grdColSubItemName.Visible = true;
            this.grdColSubItemName.VisibleIndex = 1;
            this.grdColSubItemName.Width = 57;
            // 
            // cmbItemLinkedToName
            // 
            this.cmbItemLinkedToName.AutoHeight = false;
            this.cmbItemLinkedToName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemLinkedToName.DisplayMember = "Name";
            this.cmbItemLinkedToName.Name = "cmbItemLinkedToName";
            this.cmbItemLinkedToName.NullText = "";
            this.cmbItemLinkedToName.ReadOnly = true;
            this.cmbItemLinkedToName.ValueMember = "Oid";
            // 
            // grdColSubItemQtyFactor
            // 
            this.grdColSubItemQtyFactor.Caption = "@@QuantityFactor";
            this.grdColSubItemQtyFactor.ColumnEdit = this.txtItemsLinkedToQuantityFactor;
            this.grdColSubItemQtyFactor.FieldName = "QtyFactor";
            this.grdColSubItemQtyFactor.Name = "grdColSubItemQtyFactor";
            this.grdColSubItemQtyFactor.Visible = true;
            this.grdColSubItemQtyFactor.VisibleIndex = 2;
            this.grdColSubItemQtyFactor.Width = 57;
            // 
            // txtItemsLinkedToQuantityFactor
            // 
            this.txtItemsLinkedToQuantityFactor.AutoHeight = false;
            this.txtItemsLinkedToQuantityFactor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtItemsLinkedToQuantityFactor.Name = "txtItemsLinkedToQuantityFactor";
            // 
            // grdColDeleteSubItem
            // 
            this.grdColDeleteSubItem.ColumnEdit = this.btn_delete_SubItem;
            this.grdColDeleteSubItem.Name = "grdColDeleteSubItem";
            this.grdColDeleteSubItem.Visible = true;
            this.grdColDeleteSubItem.VisibleIndex = 3;
            // 
            // btn_delete_SubItem
            // 
            this.btn_delete_SubItem.AutoHeight = false;
            this.btn_delete_SubItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_delete_SubItem.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, "", null, null, true)});
            this.btn_delete_SubItem.Name = "btn_delete_SubItem";
            this.btn_delete_SubItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_delete_SubItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_delete_SubItem_ButtonClick);
            // 
            // xtpStores
            // 
            this.xtpStores.Controls.Add(this.gridItemStores);
            this.xtpStores.Name = "xtpStores";
            this.xtpStores.Size = new System.Drawing.Size(1134, 367);
            this.xtpStores.Text = "@@Storages";
            // 
            // gridItemStores
            // 
            this.gridItemStores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItemStores.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridItemStores.Location = new System.Drawing.Point(0, 0);
            this.gridItemStores.MainView = this.grdViewItemStores;
            this.gridItemStores.Name = "gridItemStores";
            this.gridItemStores.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_Del_ItemStore,
            this.cmbItemStoreStore,
            this.cmbItemStoreStoreName});
            this.gridItemStores.Size = new System.Drawing.Size(1134, 367);
            this.gridItemStores.TabIndex = 0;
            this.gridItemStores.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewItemStores});
            // 
            // grdViewItemStores
            // 
            this.grdViewItemStores.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColItemStoreCode,
            this.grdColItemStoreName,
            this.grdColDeleteItemStore});
            this.grdViewItemStores.GridControl = this.gridItemStores;
            this.grdViewItemStores.Name = "grdViewItemStores";
            this.grdViewItemStores.OptionsMenu.EnableColumnMenu = false;
            this.grdViewItemStores.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStores.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStores.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewItemStores.OptionsView.ShowDetailButtons = false;
            this.grdViewItemStores.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewItemStores.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.grdViewItemStores_CustomDrawCell);
            this.grdViewItemStores.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewItemStores_ValidateRow);
            // 
            // grdColItemStoreCode
            // 
            this.grdColItemStoreCode.Caption = "@@Code";
            this.grdColItemStoreCode.ColumnEdit = this.cmbItemStoreStore;
            this.grdColItemStoreCode.FieldName = "Store";
            this.grdColItemStoreCode.Name = "grdColItemStoreCode";
            this.grdColItemStoreCode.Visible = true;
            this.grdColItemStoreCode.VisibleIndex = 0;
            this.grdColItemStoreCode.Width = 150;
            // 
            // cmbItemStoreStore
            // 
            this.cmbItemStoreStore.AutoHeight = false;
            this.cmbItemStoreStore.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemStoreStore.DisplayMember = "Description";
            this.cmbItemStoreStore.Name = "cmbItemStoreStore";
            this.cmbItemStoreStore.NullText = "";
            this.cmbItemStoreStore.ValueMember = "Oid";
            // 
            // grdColItemStoreName
            // 
            this.grdColItemStoreName.Caption = "@@Name";
            this.grdColItemStoreName.ColumnEdit = this.cmbItemStoreStoreName;
            this.grdColItemStoreName.FieldName = "Store";
            this.grdColItemStoreName.Name = "grdColItemStoreName";
            this.grdColItemStoreName.OptionsColumn.AllowEdit = false;
            this.grdColItemStoreName.Visible = true;
            this.grdColItemStoreName.VisibleIndex = 1;
            this.grdColItemStoreName.Width = 57;
            // 
            // cmbItemStoreStoreName
            // 
            this.cmbItemStoreStoreName.AutoHeight = false;
            this.cmbItemStoreStoreName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemStoreStoreName.DisplayMember = "Name";
            this.cmbItemStoreStoreName.Name = "cmbItemStoreStoreName";
            this.cmbItemStoreStoreName.NullText = "";
            this.cmbItemStoreStoreName.ReadOnly = true;
            this.cmbItemStoreStoreName.ValueMember = "Oid";
            // 
            // grdColDeleteItemStore
            // 
            this.grdColDeleteItemStore.ColumnEdit = this.btn_Del_ItemStore;
            this.grdColDeleteItemStore.Name = "grdColDeleteItemStore";
            this.grdColDeleteItemStore.Visible = true;
            this.grdColDeleteItemStore.VisibleIndex = 2;
            // 
            // btn_Del_ItemStore
            // 
            this.btn_Del_ItemStore.AutoHeight = false;
            this.btn_Del_ItemStore.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_Del_ItemStore.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject10, "", null, null, true)});
            this.btn_Del_ItemStore.Name = "btn_Del_ItemStore";
            this.btn_Del_ItemStore.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btn_Del_ItemStore.Click += new System.EventHandler(this.btn_Del_ItemStore_Click);
            // 
            // xtpItemStocks
            // 
            this.xtpItemStocks.Controls.Add(this.gridItemStocks);
            this.xtpItemStocks.Name = "xtpItemStocks";
            this.xtpItemStocks.Size = new System.Drawing.Size(1134, 367);
            this.xtpItemStocks.Text = "@@ItemStock";
            // 
            // gridItemStocks
            // 
            this.gridItemStocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItemStocks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gridItemStocks.Location = new System.Drawing.Point(0, 0);
            this.gridItemStocks.MainView = this.grdViewItemStocks;
            this.gridItemStocks.Name = "gridItemStocks";
            this.gridItemStocks.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cmbItemStockBarcode,
            this.cmbItemStockStore,
            this.txtColItemStockQty,
            this.btn_Del_ItemStock});
            this.gridItemStocks.Size = new System.Drawing.Size(1134, 367);
            this.gridItemStocks.TabIndex = 0;
            this.gridItemStocks.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewItemStocks});
            // 
            // grdViewItemStocks
            // 
            this.grdViewItemStocks.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColItemStockStore,
            this.grdColItemStockQty,
            this.gridColumnDesirableStock});
            this.grdViewItemStocks.GridControl = this.gridItemStocks;
            this.grdViewItemStocks.Name = "grdViewItemStocks";
            this.grdViewItemStocks.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStocks.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStocks.OptionsMenu.EnableColumnMenu = false;
            this.grdViewItemStocks.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStocks.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewItemStocks.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.grdViewItemStocks.OptionsView.ShowDetailButtons = false;
            this.grdViewItemStocks.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.grdViewItemStocks.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewItemStock_ValidateRow);
            // 
            // grdColItemStockStore
            // 
            this.grdColItemStockStore.Caption = "@@Store";
            this.grdColItemStockStore.ColumnEdit = this.cmbItemStockStore;
            this.grdColItemStockStore.FieldName = "Store";
            this.grdColItemStockStore.Name = "grdColItemStockStore";
            this.grdColItemStockStore.OptionsColumn.AllowEdit = false;
            this.grdColItemStockStore.Visible = true;
            this.grdColItemStockStore.VisibleIndex = 0;
            this.grdColItemStockStore.Width = 150;
            // 
            // cmbItemStockStore
            // 
            this.cmbItemStockStore.AutoHeight = false;
            this.cmbItemStockStore.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemStockStore.DisplayMember = "Description";
            this.cmbItemStockStore.Name = "cmbItemStockStore";
            this.cmbItemStockStore.ValueMember = "Oid";
            // 
            // grdColItemStockQty
            // 
            this.grdColItemStockQty.Caption = "@@Stock";
            this.grdColItemStockQty.ColumnEdit = this.txtColItemStockQty;
            this.grdColItemStockQty.DisplayFormat.FormatString = "d";
            this.grdColItemStockQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.grdColItemStockQty.FieldName = "Stock";
            this.grdColItemStockQty.Name = "grdColItemStockQty";
            this.grdColItemStockQty.OptionsColumn.AllowEdit = false;
            this.grdColItemStockQty.Visible = true;
            this.grdColItemStockQty.VisibleIndex = 1;
            // 
            // txtColItemStockQty
            // 
            this.txtColItemStockQty.AutoHeight = false;
            this.txtColItemStockQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtColItemStockQty.Name = "txtColItemStockQty";
            // 
            // gridColumnDesirableStock
            // 
            this.gridColumnDesirableStock.Caption = "@@DesirableStock";
            this.gridColumnDesirableStock.FieldName = "DesirableStock";
            this.gridColumnDesirableStock.Name = "gridColumnDesirableStock";
            this.gridColumnDesirableStock.Visible = true;
            this.gridColumnDesirableStock.VisibleIndex = 2;
            // 
            // cmbItemStockBarcode
            // 
            this.cmbItemStockBarcode.AutoHeight = false;
            this.cmbItemStockBarcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbItemStockBarcode.DisplayMember = "Code";
            this.cmbItemStockBarcode.Name = "cmbItemStockBarcode";
            this.cmbItemStockBarcode.NullText = "";
            this.cmbItemStockBarcode.ValueMember = "Oid";
            // 
            // btn_Del_ItemStock
            // 
            this.btn_Del_ItemStock.AutoHeight = false;
            this.btn_Del_ItemStock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btn_Del_ItemStock.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject11, "", null, null, true)});
            this.btn_Del_ItemStock.Name = "btn_Del_ItemStock";
            this.btn_Del_ItemStock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // xtraTabPageItemExtraInfo
            // 
            this.xtraTabPageItemExtraInfo.Controls.Add(this.layoutControl2);
            this.xtraTabPageItemExtraInfo.Name = "xtraTabPageItemExtraInfo";
            this.xtraTabPageItemExtraInfo.Size = new System.Drawing.Size(1134, 367);
            this.xtraTabPageItemExtraInfo.Text = "@@ItemExtraInfo";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.txtIngredients);
            this.layoutControl2.Controls.Add(this.dtExpiresAt);
            this.layoutControl2.Controls.Add(this.dtPackedAt);
            this.layoutControl2.Controls.Add(this.txtLot);
            this.layoutControl2.Controls.Add(this.txtOrigin);
            this.layoutControl2.Controls.Add(this.textEditItemExtraInfoDescription);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(992, 485, 518, 424);
            this.layoutControl2.Root = this.layoutControlGroup3;
            this.layoutControl2.Size = new System.Drawing.Size(1134, 367);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // txtIngredients
            // 
            this.txtIngredients.Location = new System.Drawing.Point(90, 132);
            this.txtIngredients.Name = "txtIngredients";
            this.txtIngredients.Size = new System.Drawing.Size(1032, 223);
            this.txtIngredients.StyleController = this.layoutControl2;
            this.txtIngredients.TabIndex = 7;
            // 
            // dtExpiresAt
            // 
            this.dtExpiresAt.EditValue = null;
            this.dtExpiresAt.Location = new System.Drawing.Point(90, 108);
            this.dtExpiresAt.Name = "dtExpiresAt";
            this.dtExpiresAt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiresAt.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiresAt.Size = new System.Drawing.Size(475, 20);
            this.dtExpiresAt.StyleController = this.layoutControl2;
            this.dtExpiresAt.TabIndex = 6;
            // 
            // dtPackedAt
            // 
            this.dtPackedAt.EditValue = null;
            this.dtPackedAt.Location = new System.Drawing.Point(90, 84);
            this.dtPackedAt.Name = "dtPackedAt";
            this.dtPackedAt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPackedAt.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPackedAt.Size = new System.Drawing.Size(475, 20);
            this.dtPackedAt.StyleController = this.layoutControl2;
            this.dtPackedAt.TabIndex = 5;
            // 
            // txtLot
            // 
            this.txtLot.Location = new System.Drawing.Point(90, 60);
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new System.Drawing.Size(1032, 20);
            this.txtLot.StyleController = this.layoutControl2;
            this.txtLot.TabIndex = 4;
            // 
            // txtOrigin
            // 
            this.txtOrigin.Location = new System.Drawing.Point(90, 36);
            this.txtOrigin.Name = "txtOrigin";
            this.txtOrigin.Size = new System.Drawing.Size(1032, 20);
            this.txtOrigin.StyleController = this.layoutControl2;
            this.txtOrigin.TabIndex = 4;
            // 
            // textEditItemExtraInfoDescription
            // 
            this.textEditItemExtraInfoDescription.Location = new System.Drawing.Point(90, 12);
            this.textEditItemExtraInfoDescription.Name = "textEditItemExtraInfoDescription";
            this.textEditItemExtraInfoDescription.Size = new System.Drawing.Size(1032, 20);
            this.textEditItemExtraInfoDescription.StyleController = this.layoutControl2;
            this.textEditItemExtraInfoDescription.TabIndex = 4;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemItemExtraInfoDescription,
            this.layoutControlItemItemExtraInfoLot,
            this.layoutControlItemItemExtraInfoOrigin,
            this.lcPackedAt,
            this.lcExpiresAt,
            this.lcIngredients,
            this.emptySpaceItem2,
            this.emptySpaceItem3});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "Root";
            this.layoutControlGroup3.Size = new System.Drawing.Size(1134, 367);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItemItemExtraInfoDescription
            // 
            this.layoutControlItemItemExtraInfoDescription.Control = this.textEditItemExtraInfoDescription;
            this.layoutControlItemItemExtraInfoDescription.CustomizationFormText = "@Description";
            this.SetIsRequired(this.layoutControlItemItemExtraInfoDescription, false);
            this.layoutControlItemItemExtraInfoDescription.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemItemExtraInfoDescription.Name = "layoutControlItemItemExtraInfoDescription";
            this.layoutControlItemItemExtraInfoDescription.Size = new System.Drawing.Size(1114, 24);
            this.layoutControlItemItemExtraInfoDescription.Text = "@@Description";
            this.layoutControlItemItemExtraInfoDescription.TextSize = new System.Drawing.Size(75, 13);
            // 
            // layoutControlItemItemExtraInfoLot
            // 
            this.layoutControlItemItemExtraInfoLot.Control = this.txtLot;
            this.layoutControlItemItemExtraInfoLot.CustomizationFormText = "@Lot";
            this.SetIsRequired(this.layoutControlItemItemExtraInfoLot, false);
            this.layoutControlItemItemExtraInfoLot.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItemItemExtraInfoLot.Name = "layoutControlItemItemExtraInfoLot";
            this.layoutControlItemItemExtraInfoLot.Size = new System.Drawing.Size(1114, 24);
            this.layoutControlItemItemExtraInfoLot.Text = "@@Lot";
            this.layoutControlItemItemExtraInfoLot.TextSize = new System.Drawing.Size(75, 13);
            // 
            // layoutControlItemItemExtraInfoOrigin
            // 
            this.layoutControlItemItemExtraInfoOrigin.Control = this.txtOrigin;
            this.layoutControlItemItemExtraInfoOrigin.CustomizationFormText = "@Origin";
            this.SetIsRequired(this.layoutControlItemItemExtraInfoOrigin, false);
            this.layoutControlItemItemExtraInfoOrigin.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItemItemExtraInfoOrigin.Name = "layoutControlItemItemExtraInfoOrigin";
            this.layoutControlItemItemExtraInfoOrigin.Size = new System.Drawing.Size(1114, 24);
            this.layoutControlItemItemExtraInfoOrigin.Text = "@@Origin";
            this.layoutControlItemItemExtraInfoOrigin.TextSize = new System.Drawing.Size(75, 13);
            // 
            // lcPackedAt
            // 
            this.lcPackedAt.Control = this.dtPackedAt;
            this.SetIsRequired(this.lcPackedAt, false);
            this.lcPackedAt.Location = new System.Drawing.Point(0, 72);
            this.lcPackedAt.Name = "lcPackedAt";
            this.lcPackedAt.Size = new System.Drawing.Size(557, 24);
            this.lcPackedAt.Text = "@@PackedAt";
            this.lcPackedAt.TextSize = new System.Drawing.Size(75, 13);
            // 
            // lcExpiresAt
            // 
            this.lcExpiresAt.Control = this.dtExpiresAt;
            this.SetIsRequired(this.lcExpiresAt, false);
            this.lcExpiresAt.Location = new System.Drawing.Point(0, 96);
            this.lcExpiresAt.Name = "lcExpiresAt";
            this.lcExpiresAt.Size = new System.Drawing.Size(557, 24);
            this.lcExpiresAt.Text = "@@ExpiresAt";
            this.lcExpiresAt.TextSize = new System.Drawing.Size(75, 13);
            // 
            // lcIngredients
            // 
            this.lcIngredients.Control = this.txtIngredients;
            this.SetIsRequired(this.lcIngredients, false);
            this.lcIngredients.Location = new System.Drawing.Point(0, 120);
            this.lcIngredients.Name = "lcIngredients";
            this.lcIngredients.Size = new System.Drawing.Size(1114, 227);
            this.lcIngredients.Text = "@@Ingredients";
            this.lcIngredients.TextSize = new System.Drawing.Size(75, 13);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem2, false);
            this.emptySpaceItem2.Location = new System.Drawing.Point(557, 72);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(557, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem3, false);
            this.emptySpaceItem3.Location = new System.Drawing.Point(557, 96);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(557, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "Root";
            this.layoutControlGroup2.Size = new System.Drawing.Size(1160, 439);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.tabItemInfo;
            this.SetIsRequired(this.layoutControlItem3, false);
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1140, 419);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // imageEditItemImage
            // 
            this.imageEditItemImage.Location = new System.Drawing.Point(22, 86);
            this.imageEditItemImage.Name = "imageEditItemImage";
            this.imageEditItemImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.TopLeft, ((System.Drawing.Image)(resources.GetObject("imageEditItemImage.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject12, "", "Upload", null, true)});
            this.imageEditItemImage.Properties.ShowMenu = false;
            this.imageEditItemImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.imageEditItemImage.Size = new System.Drawing.Size(287, 30);
            this.imageEditItemImage.StyleController = this.layoutControlHeader;
            this.imageEditItemImage.TabIndex = 12;
            this.imageEditItemImage.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.imageEditItemImage_ButtonClick);
            // 
            // txtCode
            // 
            this.txtCode.EditValue = "";
            this.txtCode.Location = new System.Drawing.Point(313, 86);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(432, 20);
            this.txtCode.StyleController = this.layoutControlHeader;
            this.txtCode.TabIndex = 4;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(313, 134);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(432, 20);
            this.txtName.StyleController = this.layoutControlHeader;
            this.txtName.TabIndex = 4;
            // 
            // chkIsActive
            // 
            this.chkIsActive.EditValue = true;
            this.chkIsActive.Location = new System.Drawing.Point(22, 128);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.chkIsActive.Properties.Caption = "@@IsActive";
            this.chkIsActive.Size = new System.Drawing.Size(287, 19);
            this.chkIsActive.StyleController = this.layoutControlHeader;
            this.chkIsActive.TabIndex = 4;
            // 
            // panelControlHeader
            // 
            this.panelControlHeader.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControlHeader.Appearance.Options.UseBackColor = true;
            this.panelControlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlHeader.Controls.Add(this.btnCancelItem);
            this.panelControlHeader.Controls.Add(this.btnSaveItem);
            this.panelControlHeader.Location = new System.Drawing.Point(22, 22);
            this.panelControlHeader.Name = "panelControlHeader";
            this.panelControlHeader.Size = new System.Drawing.Size(1160, 44);
            this.panelControlHeader.TabIndex = 7;
            // 
            // btnCancelItem
            // 
            this.btnCancelItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelItem.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelItem.Image")));
            this.btnCancelItem.Location = new System.Drawing.Point(1047, 3);
            this.btnCancelItem.Name = "btnCancelItem";
            this.btnCancelItem.Size = new System.Drawing.Size(110, 38);
            this.btnCancelItem.TabIndex = 0;
            this.btnCancelItem.Text = "@@Cancel";
            this.btnCancelItem.Click += new System.EventHandler(this.btnCancelItem_Click);
            // 
            // btnSaveItem
            // 
            this.btnSaveItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveItem.Image")));
            this.btnSaveItem.Location = new System.Drawing.Point(931, 3);
            this.btnSaveItem.Name = "btnSaveItem";
            this.btnSaveItem.Size = new System.Drawing.Size(110, 38);
            this.btnSaveItem.TabIndex = 0;
            this.btnSaveItem.Text = "@@Save";
            this.btnSaveItem.Click += new System.EventHandler(this.btnSaveItem_Click);
            // 
            // lueSeasonality
            // 
            this.lueSeasonality.Location = new System.Drawing.Point(604, 259);
            this.lueSeasonality.Name = "lueSeasonality";
            this.lueSeasonality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueSeasonality.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, "", null, null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueSeasonality.Properties.Buttons1"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject14, "", null, null, true)});
            this.lueSeasonality.Properties.NullText = "";
            this.lueSeasonality.Size = new System.Drawing.Size(287, 22);
            this.lueSeasonality.StyleController = this.layoutControlHeader;
            this.lueSeasonality.TabIndex = 4;
            this.lueSeasonality.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueSeasonality_ButtonClick);
            // 
            // lueAcceptsCustomPrice
            // 
            this.lueAcceptsCustomPrice.Location = new System.Drawing.Point(604, 301);
            this.lueAcceptsCustomPrice.Name = "lueAcceptsCustomPrice";
            this.lueAcceptsCustomPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueAcceptsCustomPrice.Properties.NullText = "";
            this.lueAcceptsCustomPrice.Size = new System.Drawing.Size(287, 20);
            this.lueAcceptsCustomPrice.StyleController = this.layoutControlHeader;
            this.lueAcceptsCustomPrice.TabIndex = 4;
            // 
            // lueVatCategory
            // 
            this.lueVatCategory.Location = new System.Drawing.Point(22, 175);
            this.lueVatCategory.Name = "lueVatCategory";
            this.lueVatCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueVatCategory.Properties.NullText = "";
            this.lueVatCategory.Size = new System.Drawing.Size(287, 20);
            this.lueVatCategory.StyleController = this.layoutControlHeader;
            this.lueVatCategory.TabIndex = 4;
            // 
            // txtExtraDescription
            // 
            this.txtExtraDescription.Location = new System.Drawing.Point(313, 259);
            this.txtExtraDescription.Name = "txtExtraDescription";
            this.txtExtraDescription.Size = new System.Drawing.Size(287, 20);
            this.txtExtraDescription.StyleController = this.layoutControlHeader;
            this.txtExtraDescription.TabIndex = 4;
            // 
            // luePackingMeasurementUnit
            // 
            this.luePackingMeasurementUnit.Location = new System.Drawing.Point(22, 217);
            this.luePackingMeasurementUnit.Name = "luePackingMeasurementUnit";
            this.luePackingMeasurementUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("luePackingMeasurementUnit.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject15, "", null, null, true)});
            this.luePackingMeasurementUnit.Properties.NullText = "";
            this.luePackingMeasurementUnit.Size = new System.Drawing.Size(287, 22);
            this.luePackingMeasurementUnit.StyleController = this.layoutControlHeader;
            this.luePackingMeasurementUnit.TabIndex = 4;
            this.luePackingMeasurementUnit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.luePackingMeasurementUnit_ButtonClick);
            // 
            // chkAcceptsCustomDescription
            // 
            this.chkAcceptsCustomDescription.EditValue = null;
            this.chkAcceptsCustomDescription.Location = new System.Drawing.Point(22, 251);
            this.chkAcceptsCustomDescription.Name = "chkAcceptsCustomDescription";
            this.chkAcceptsCustomDescription.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.chkAcceptsCustomDescription.Properties.Caption = "@@AcceptsCustomDescription";
            this.chkAcceptsCustomDescription.Size = new System.Drawing.Size(287, 19);
            this.chkAcceptsCustomDescription.StyleController = this.layoutControlHeader;
            this.chkAcceptsCustomDescription.TabIndex = 5;
            // 
            // txtPoints
            // 
            this.txtPoints.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtPoints.Location = new System.Drawing.Point(604, 175);
            this.txtPoints.Name = "txtPoints";
            this.txtPoints.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPoints.Properties.DisplayFormat.FormatString = "N00";
            this.txtPoints.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtPoints.Properties.IsFloatValue = false;
            this.txtPoints.Properties.Mask.EditMask = "N00";
            this.txtPoints.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtPoints.Properties.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtPoints.Size = new System.Drawing.Size(287, 20);
            this.txtPoints.StyleController = this.layoutControlHeader;
            this.txtPoints.TabIndex = 4;
            // 
            // txtPackingQty
            // 
            this.txtPackingQty.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtPackingQty.Location = new System.Drawing.Point(313, 217);
            this.txtPackingQty.Name = "txtPackingQty";
            this.txtPackingQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPackingQty.Properties.DisplayFormat.FormatString = "N00";
            this.txtPackingQty.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtPackingQty.Properties.IsFloatValue = false;
            this.txtPackingQty.Properties.Mask.EditMask = "N00";
            this.txtPackingQty.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtPackingQty.Properties.MaxValue = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.txtPackingQty.Size = new System.Drawing.Size(287, 20);
            this.txtPackingQty.StyleController = this.layoutControlHeader;
            this.txtPackingQty.TabIndex = 4;
            // 
            // dtInsertedOn
            // 
            this.dtInsertedOn.EditValue = null;
            this.dtInsertedOn.Location = new System.Drawing.Point(313, 175);
            this.dtInsertedOn.Name = "dtInsertedOn";
            this.dtInsertedOn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInsertedOn.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInsertedOn.Properties.DisplayFormat.FormatString = "";
            this.dtInsertedOn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtInsertedOn.Properties.EditFormat.FormatString = "";
            this.dtInsertedOn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtInsertedOn.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtInsertedOn.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtInsertedOn.Size = new System.Drawing.Size(287, 20);
            this.dtInsertedOn.StyleController = this.layoutControlHeader;
            this.dtInsertedOn.TabIndex = 4;
            // 
            // lueDefaultBarcode
            // 
            this.lueDefaultBarcode.Location = new System.Drawing.Point(749, 133);
            this.lueDefaultBarcode.Name = "lueDefaultBarcode";
            this.lueDefaultBarcode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueDefaultBarcode.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject16, "", null, null, true)});
            this.lueDefaultBarcode.Properties.NullText = "";
            this.lueDefaultBarcode.Size = new System.Drawing.Size(433, 22);
            this.lueDefaultBarcode.StyleController = this.layoutControlHeader;
            this.lueDefaultBarcode.TabIndex = 4;
            this.lueDefaultBarcode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueDefaultBarcode_ButtonClick);
            // 
            // txtMaxOrderQty
            // 
            this.txtMaxOrderQty.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtMaxOrderQty.Location = new System.Drawing.Point(895, 217);
            this.txtMaxOrderQty.Name = "txtMaxOrderQty";
            this.txtMaxOrderQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtMaxOrderQty.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtMaxOrderQty.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.txtMaxOrderQty.Properties.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtMaxOrderQty.Size = new System.Drawing.Size(287, 20);
            this.txtMaxOrderQty.StyleController = this.layoutControlHeader;
            this.txtMaxOrderQty.TabIndex = 4;
            // 
            // lueBuyer
            // 
            this.lueBuyer.Location = new System.Drawing.Point(895, 259);
            this.lueBuyer.Name = "lueBuyer";
            this.lueBuyer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueBuyer.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject17, "", null, null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueBuyer.Properties.Buttons1"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject18, "", null, null, true)});
            this.lueBuyer.Properties.NullText = "";
            this.lueBuyer.Size = new System.Drawing.Size(287, 22);
            this.lueBuyer.StyleController = this.layoutControlHeader;
            this.lueBuyer.TabIndex = 4;
            this.lueBuyer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueBuyer_ButtonClick);
            // 
            // txtReferenceUnit
            // 
            this.txtReferenceUnit.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtReferenceUnit.Location = new System.Drawing.Point(22, 301);
            this.txtReferenceUnit.Name = "txtReferenceUnit";
            this.txtReferenceUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtReferenceUnit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtReferenceUnit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.txtReferenceUnit.Properties.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtReferenceUnit.Size = new System.Drawing.Size(287, 20);
            this.txtReferenceUnit.StyleController = this.layoutControlHeader;
            this.txtReferenceUnit.TabIndex = 4;
            // 
            // txtContentUnit
            // 
            this.txtContentUnit.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtContentUnit.Location = new System.Drawing.Point(313, 301);
            this.txtContentUnit.Name = "txtContentUnit";
            this.txtContentUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtContentUnit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtContentUnit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.txtContentUnit.Properties.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtContentUnit.Size = new System.Drawing.Size(287, 20);
            this.txtContentUnit.StyleController = this.layoutControlHeader;
            this.txtContentUnit.TabIndex = 4;
            // 
            // chkIsCentralStored
            // 
            this.chkIsCentralStored.EditValue = null;
            this.chkIsCentralStored.Location = new System.Drawing.Point(895, 293);
            this.chkIsCentralStored.Name = "chkIsCentralStored";
            this.chkIsCentralStored.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.chkIsCentralStored.Properties.Caption = "@@IsCentralStored";
            this.chkIsCentralStored.Size = new System.Drawing.Size(287, 19);
            this.chkIsCentralStored.StyleController = this.layoutControlHeader;
            this.chkIsCentralStored.TabIndex = 4;
            // 
            // memoRemarks
            // 
            this.memoRemarks.Location = new System.Drawing.Point(604, 341);
            this.memoRemarks.Name = "memoRemarks";
            this.memoRemarks.Size = new System.Drawing.Size(578, 53);
            this.memoRemarks.StyleController = this.layoutControlHeader;
            this.memoRemarks.TabIndex = 8;
            // 
            // txtMinOrderQty
            // 
            this.txtMinOrderQty.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtMinOrderQty.Location = new System.Drawing.Point(604, 217);
            this.txtMinOrderQty.Name = "txtMinOrderQty";
            this.txtMinOrderQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtMinOrderQty.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.txtMinOrderQty.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.txtMinOrderQty.Properties.MaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtMinOrderQty.Size = new System.Drawing.Size(287, 20);
            this.txtMinOrderQty.StyleController = this.layoutControlHeader;
            this.txtMinOrderQty.TabIndex = 4;
            // 
            // lueMotherCode
            // 
            this.lueMotherCode.Location = new System.Drawing.Point(749, 85);
            this.lueMotherCode.Name = "lueMotherCode";
            this.lueMotherCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueMotherCode.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject19, "", null, null, true)});
            this.lueMotherCode.Properties.NullText = "";
            this.lueMotherCode.Properties.View = this.searchLookUpEdit1View;
            this.lueMotherCode.Size = new System.Drawing.Size(433, 22);
            this.lueMotherCode.StyleController = this.layoutControlHeader;
            this.lueMotherCode.TabIndex = 4;
            this.lueMotherCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueMotherCode_ButtonClick);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // lueItemSupplier
            // 
            this.lueItemSupplier.Location = new System.Drawing.Point(895, 175);
            this.lueItemSupplier.Name = "lueItemSupplier";
            this.lueItemSupplier.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("lueItemSupplier.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject20, "", null, null, true)});
            this.lueItemSupplier.Properties.NullText = "";
            this.lueItemSupplier.Properties.View = this.gridView1;
            this.lueItemSupplier.Size = new System.Drawing.Size(287, 22);
            this.lueItemSupplier.StyleController = this.layoutControlHeader;
            this.lueItemSupplier.TabIndex = 4;
            this.lueItemSupplier.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueItemSupplier_ButtonClick);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // txtExtraFile
            // 
            this.txtExtraFile.EditValue = "";
            this.txtExtraFile.Location = new System.Drawing.Point(22, 341);
            this.txtExtraFile.Name = "txtExtraFile";
            this.txtExtraFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "@@Upload", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("txtExtraFile.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject21, "", "Upload", null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "@@Delete", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("txtExtraFile.Properties.Buttons1"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject22, "", "Delete", null, true)});
            this.txtExtraFile.Properties.Mask.EditMask = "N00";
            this.txtExtraFile.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtExtraFile.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txtExtraFile.Size = new System.Drawing.Size(578, 30);
            this.txtExtraFile.StyleController = this.layoutControlHeader;
            this.txtExtraFile.TabIndex = 4;
            this.txtExtraFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtExtraFile_ButtonClick);
            // 
            // layoutControlGroupAllItems
            // 
            this.layoutControlGroupAllItems.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupAllItems.GroupBordersVisible = false;
            this.layoutControlGroupAllItems.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.Root});
            this.layoutControlGroupAllItems.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupAllItems.Name = "Root";
            this.layoutControlGroupAllItems.Size = new System.Drawing.Size(1204, 859);
            this.layoutControlGroupAllItems.TextVisible = false;
            // 
            // Root
            // 
            this.Root.CustomizationFormText = "Root";
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcCode,
            this.layoutControlItemHeader,
            this.layoutControlItemImage,
            this.lcMotherCode,
            this.lcVatCategory,
            this.lcDefaultBarcode,
            this.lcName,
            this.lcPoints,
            this.lcInsertedOn,
            this.lcPackingMeasurementUnit,
            this.lcPackingQty,
            this.lcItemSupplier,
            this.lcMinOrderQty,
            this.lcMaxOrderQty,
            this.lcBreakOrderToCentral,
            this.lcExtraDescription,
            this.lcSeasonality,
            this.lcBuyer,
            this.lcReferenceUnit,
            this.lcContentUnit,
            this.lcAcceptsCustomPrice,
            this.lcSex,
            this.lcRemarks,
            this.lcIsActive,
            this.lcExtraFile,
            this.layoutControlItem2,
            this.layoutControlItem5,
            this.layoutControlItem4});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "item0";
            this.Root.Size = new System.Drawing.Size(1184, 839);
            this.Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.TextVisible = false;
            // 
            // lcCode
            // 
            this.lcCode.Control = this.txtCode;
            this.SetIsRequired(this.lcCode, true);
            this.lcCode.Location = new System.Drawing.Point(291, 48);
            this.lcCode.Name = "lcCode";
            this.lcCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.lcCode.Size = new System.Drawing.Size(436, 48);
            this.lcCode.Text = "@@Code";
            this.lcCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcCode.TextSize = new System.Drawing.Size(140, 13);
            // 
            // layoutControlItemHeader
            // 
            this.layoutControlItemHeader.Control = this.panelControlHeader;
            this.SetIsRequired(this.layoutControlItemHeader, false);
            this.layoutControlItemHeader.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemHeader.MinSize = new System.Drawing.Size(5, 5);
            this.layoutControlItemHeader.Name = "layoutControlItemHeader";
            this.layoutControlItemHeader.Size = new System.Drawing.Size(1164, 48);
            this.layoutControlItemHeader.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItemHeader.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemHeader.TextVisible = false;
            // 
            // layoutControlItemImage
            // 
            this.layoutControlItemImage.Control = this.imageEditItemImage;
            this.layoutControlItemImage.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.SetIsRequired(this.layoutControlItemImage, false);
            this.layoutControlItemImage.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItemImage.Name = "layoutControlItemImage";
            this.layoutControlItemImage.Size = new System.Drawing.Size(291, 50);
            this.layoutControlItemImage.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.SupportHorzAlignment;
            this.layoutControlItemImage.Text = "@@Image";
            this.layoutControlItemImage.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemImage.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcMotherCode
            // 
            this.lcMotherCode.Control = this.lueMotherCode;
            this.SetIsRequired(this.lcMotherCode, false);
            this.lcMotherCode.Location = new System.Drawing.Point(727, 48);
            this.lcMotherCode.Name = "lcMotherCode";
            this.lcMotherCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 1, 10);
            this.lcMotherCode.Size = new System.Drawing.Size(437, 49);
            this.lcMotherCode.Text = "@@MotherCode";
            this.lcMotherCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcMotherCode.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcVatCategory
            // 
            this.lcVatCategory.Control = this.lueVatCategory;
            this.SetIsRequired(this.lcVatCategory, true);
            this.lcVatCategory.Location = new System.Drawing.Point(0, 137);
            this.lcVatCategory.Name = "lcVatCategory";
            this.lcVatCategory.Size = new System.Drawing.Size(291, 42);
            this.lcVatCategory.Text = "@@VatCategory";
            this.lcVatCategory.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcVatCategory.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcDefaultBarcode
            // 
            this.lcDefaultBarcode.Control = this.lueDefaultBarcode;
            this.SetIsRequired(this.lcDefaultBarcode, true);
            this.lcDefaultBarcode.Location = new System.Drawing.Point(727, 97);
            this.lcDefaultBarcode.Name = "lcDefaultBarcode";
            this.lcDefaultBarcode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
            this.lcDefaultBarcode.Size = new System.Drawing.Size(437, 40);
            this.lcDefaultBarcode.Text = "@@DefaultBarcode";
            this.lcDefaultBarcode.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcDefaultBarcode.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcName
            // 
            this.lcName.Control = this.txtName;
            this.SetIsRequired(this.lcName, true);
            this.lcName.Location = new System.Drawing.Point(291, 96);
            this.lcName.Name = "lcName";
            this.lcName.Size = new System.Drawing.Size(436, 41);
            this.lcName.Text = "@@Name";
            this.lcName.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcName.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcPoints
            // 
            this.lcPoints.Control = this.txtPoints;
            this.SetIsRequired(this.lcPoints, false);
            this.lcPoints.Location = new System.Drawing.Point(582, 137);
            this.lcPoints.Name = "lcPoints";
            this.lcPoints.Size = new System.Drawing.Size(291, 42);
            this.lcPoints.Text = "@@Points";
            this.lcPoints.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPoints.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcInsertedOn
            // 
            this.lcInsertedOn.Control = this.dtInsertedOn;
            this.SetIsRequired(this.lcInsertedOn, false);
            this.lcInsertedOn.Location = new System.Drawing.Point(291, 137);
            this.lcInsertedOn.Name = "lcInsertedOn";
            this.lcInsertedOn.Size = new System.Drawing.Size(291, 42);
            this.lcInsertedOn.Text = "@@InsertedDate";
            this.lcInsertedOn.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcInsertedOn.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcPackingMeasurementUnit
            // 
            this.lcPackingMeasurementUnit.Control = this.luePackingMeasurementUnit;
            this.SetIsRequired(this.lcPackingMeasurementUnit, false);
            this.lcPackingMeasurementUnit.Location = new System.Drawing.Point(0, 179);
            this.lcPackingMeasurementUnit.Name = "lcPackingMeasurementUnit";
            this.lcPackingMeasurementUnit.Size = new System.Drawing.Size(291, 42);
            this.lcPackingMeasurementUnit.Text = "@@PackingMeasurementUnit";
            this.lcPackingMeasurementUnit.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPackingMeasurementUnit.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcPackingQty
            // 
            this.lcPackingQty.Control = this.txtPackingQty;
            this.SetIsRequired(this.lcPackingQty, false);
            this.lcPackingQty.Location = new System.Drawing.Point(291, 179);
            this.lcPackingQty.Name = "lcPackingQty";
            this.lcPackingQty.Size = new System.Drawing.Size(291, 42);
            this.lcPackingQty.Text = "@@PackingQty";
            this.lcPackingQty.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcPackingQty.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcItemSupplier
            // 
            this.lcItemSupplier.Control = this.lueItemSupplier;
            this.SetIsRequired(this.lcItemSupplier, false);
            this.lcItemSupplier.Location = new System.Drawing.Point(873, 137);
            this.lcItemSupplier.Name = "lcItemSupplier";
            this.lcItemSupplier.Size = new System.Drawing.Size(291, 42);
            this.lcItemSupplier.Text = "@@ItemSupplier";
            this.lcItemSupplier.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcItemSupplier.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcMinOrderQty
            // 
            this.lcMinOrderQty.Control = this.txtMinOrderQty;
            this.SetIsRequired(this.lcMinOrderQty, false);
            this.lcMinOrderQty.Location = new System.Drawing.Point(582, 179);
            this.lcMinOrderQty.Name = "lcMinOrderQty";
            this.lcMinOrderQty.Size = new System.Drawing.Size(291, 42);
            this.lcMinOrderQty.Text = "@@MinOrderQty";
            this.lcMinOrderQty.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcMinOrderQty.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcMaxOrderQty
            // 
            this.lcMaxOrderQty.Control = this.txtMaxOrderQty;
            this.SetIsRequired(this.lcMaxOrderQty, false);
            this.lcMaxOrderQty.Location = new System.Drawing.Point(873, 179);
            this.lcMaxOrderQty.Name = "lcMaxOrderQty";
            this.lcMaxOrderQty.Size = new System.Drawing.Size(291, 42);
            this.lcMaxOrderQty.Text = "@@MaxOrderQty";
            this.lcMaxOrderQty.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcMaxOrderQty.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcBreakOrderToCentral
            // 
            this.lcBreakOrderToCentral.Control = this.chkAcceptsCustomDescription;
            this.lcBreakOrderToCentral.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lcBreakOrderToCentral.ImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.SetIsRequired(this.lcBreakOrderToCentral, false);
            this.lcBreakOrderToCentral.Location = new System.Drawing.Point(0, 221);
            this.lcBreakOrderToCentral.Name = "lcBreakOrderToCentral";
            this.lcBreakOrderToCentral.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
            this.lcBreakOrderToCentral.Size = new System.Drawing.Size(291, 42);
            this.lcBreakOrderToCentral.TextSize = new System.Drawing.Size(0, 0);
            this.lcBreakOrderToCentral.TextVisible = false;
            // 
            // lcExtraDescription
            // 
            this.lcExtraDescription.Control = this.txtExtraDescription;
            this.SetIsRequired(this.lcExtraDescription, false);
            this.lcExtraDescription.Location = new System.Drawing.Point(291, 221);
            this.lcExtraDescription.Name = "lcExtraDescription";
            this.lcExtraDescription.Size = new System.Drawing.Size(291, 42);
            this.lcExtraDescription.Text = "@@Description";
            this.lcExtraDescription.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcExtraDescription.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcSeasonality
            // 
            this.lcSeasonality.Control = this.lueSeasonality;
            this.SetIsRequired(this.lcSeasonality, false);
            this.lcSeasonality.Location = new System.Drawing.Point(582, 221);
            this.lcSeasonality.Name = "lcSeasonality";
            this.lcSeasonality.Size = new System.Drawing.Size(291, 42);
            this.lcSeasonality.Text = "@@Seasonality";
            this.lcSeasonality.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcSeasonality.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcBuyer
            // 
            this.lcBuyer.Control = this.lueBuyer;
            this.SetIsRequired(this.lcBuyer, false);
            this.lcBuyer.Location = new System.Drawing.Point(873, 221);
            this.lcBuyer.Name = "lcBuyer";
            this.lcBuyer.Size = new System.Drawing.Size(291, 42);
            this.lcBuyer.Text = "@@Buyer";
            this.lcBuyer.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcBuyer.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcReferenceUnit
            // 
            this.lcReferenceUnit.Control = this.txtReferenceUnit;
            this.SetIsRequired(this.lcReferenceUnit, false);
            this.lcReferenceUnit.Location = new System.Drawing.Point(0, 263);
            this.lcReferenceUnit.Name = "lcReferenceUnit";
            this.lcReferenceUnit.Size = new System.Drawing.Size(291, 40);
            this.lcReferenceUnit.Text = "@@ReferenceUnit";
            this.lcReferenceUnit.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcReferenceUnit.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcContentUnit
            // 
            this.lcContentUnit.Control = this.txtContentUnit;
            this.SetIsRequired(this.lcContentUnit, false);
            this.lcContentUnit.Location = new System.Drawing.Point(291, 263);
            this.lcContentUnit.Name = "lcContentUnit";
            this.lcContentUnit.Size = new System.Drawing.Size(291, 40);
            this.lcContentUnit.Text = "@@ContentUnit";
            this.lcContentUnit.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcContentUnit.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcAcceptsCustomPrice
            // 
            this.lcAcceptsCustomPrice.Control = this.lueAcceptsCustomPrice;
            this.SetIsRequired(this.lcAcceptsCustomPrice, false);
            this.lcAcceptsCustomPrice.Location = new System.Drawing.Point(582, 263);
            this.lcAcceptsCustomPrice.Name = "lcAcceptsCustomPrice";
            this.lcAcceptsCustomPrice.Size = new System.Drawing.Size(291, 40);
            this.lcAcceptsCustomPrice.Text = "@@AcceptsCustomPrice";
            this.lcAcceptsCustomPrice.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcAcceptsCustomPrice.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcSex
            // 
            this.lcSex.Control = this.chkIsCentralStored;
            this.SetIsRequired(this.lcSex, false);
            this.lcSex.Location = new System.Drawing.Point(873, 263);
            this.lcSex.Name = "lcSex";
            this.lcSex.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
            this.lcSex.Size = new System.Drawing.Size(291, 40);
            this.lcSex.Text = "@@Sex";
            this.lcSex.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcSex.TextSize = new System.Drawing.Size(0, 0);
            this.lcSex.TextVisible = false;
            // 
            // lcRemarks
            // 
            this.lcRemarks.Control = this.memoRemarks;
            this.lcRemarks.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
            this.SetIsRequired(this.lcRemarks, false);
            this.lcRemarks.Location = new System.Drawing.Point(582, 303);
            this.lcRemarks.Name = "lcRemarks";
            this.lcRemarks.Size = new System.Drawing.Size(582, 73);
            this.lcRemarks.Text = "@@Remarks";
            this.lcRemarks.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcRemarks.TextSize = new System.Drawing.Size(140, 13);
            // 
            // lcIsActive
            // 
            this.lcIsActive.Control = this.chkIsActive;
            this.SetIsRequired(this.lcIsActive, false);
            this.lcIsActive.Location = new System.Drawing.Point(0, 98);
            this.lcIsActive.Name = "lcIsActive";
            this.lcIsActive.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
            this.lcIsActive.Size = new System.Drawing.Size(291, 39);
            this.lcIsActive.Text = "@@IsActive";
            this.lcIsActive.TextSize = new System.Drawing.Size(0, 0);
            this.lcIsActive.TextVisible = false;
            // 
            // lcExtraFile
            // 
            this.lcExtraFile.Control = this.txtExtraFile;
            this.SetIsRequired(this.lcExtraFile, false);
            this.lcExtraFile.Location = new System.Drawing.Point(0, 303);
            this.lcExtraFile.Name = "lcExtraFile";
            this.lcExtraFile.Size = new System.Drawing.Size(582, 50);
            this.lcExtraFile.Text = "@@File";
            this.lcExtraFile.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcExtraFile.TextSize = new System.Drawing.Size(140, 13);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl1;
            this.SetIsRequired(this.layoutControlItem2, false);
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 376);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1164, 443);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.DoesNotAllowDiscount;
            this.SetIsRequired(this.layoutControlItem5, false);
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 353);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(291, 23);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.IsTax;
            this.SetIsRequired(this.layoutControlItem4, false);
            this.layoutControlItem4.Location = new System.Drawing.Point(291, 353);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(291, 23);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // btnDelChildItem
            // 
            this.btnDelChildItem.AutoHeight = false;
            this.btnDelChildItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnDelChildItem.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject23, "", null, null, true)});
            this.btnDelChildItem.Name = "btnDelChildItem";
            this.btnDelChildItem.Click += new System.EventHandler(this.btnDelChildItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(962, 437);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 79);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(182, 94);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.SetIsRequired(this.emptySpaceItem1, false);
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 795);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(968, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(962, 437);
            this.flowLayoutPanel4.TabIndex = 5;
            // 
            // ItemEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 859);
            this.Controls.Add(this.layoutControlHeader);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "ItemEditForm";
            this.Text = "@@EditItem";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemEditForm_FormClosing);
            this.Load += new System.EventHandler(this.ItemEditForm_Load);
            this.Shown += new System.EventHandler(this.ItemEditForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPriceCatalogDetailTimeValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueFrom.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueUntil.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeValueUntil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEditPriceCatalogDetailTimeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditDeletePriceCatalogDetailTimeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPriceCatalogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPriceCatalogDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPriceCatalog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMarkup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVatIncluded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPcdIsActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_PriceCatalog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_SubItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlHeader)).EndInit();
            this.layoutControlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DoesNotAllowDiscount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IsTax.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabItemInfo)).EndInit();
            this.tabItemInfo.ResumeLayout(false);
            this.xtpBarcodes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItemBarcodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemBarcodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemBarcodeMeasurementUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditItemBarcodeRelationFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemBarcodeBarcodeType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_ItemBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            this.xtpPriceCatalogs.ResumeLayout(false);
            this.xtpItemCategories.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItemCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryNode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryRepository)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_category)).EndInit();
            this.xtpChildItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridChildItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewChildItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItemGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbChildItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ChildItem)).EndInit();
            this.xtpLinkedItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLinkedItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewLinkedItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLinkedItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbLinkedItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLinkedItemsQuantityFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_LinkedItem)).EndInit();
            this.xtpSubItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSubItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSubItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemLinkedTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmbItemLinkedTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemLinkedToName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemsLinkedToQuantityFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete_SubItem)).EndInit();
            this.xtpStores.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItemStores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemStores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStoreStore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStoreStoreName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ItemStore)).EndInit();
            this.xtpItemStocks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItemStocks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewItemStocks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStockStore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColItemStockQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItemStockBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Del_ItemStock)).EndInit();
            this.xtraTabPageItemExtraInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtIngredients.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiresAt.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiresAt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPackedAt.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPackedAt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLot.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrigin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditItemExtraInfoDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoLot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemExtraInfoOrigin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackedAt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExpiresAt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIngredients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageEditItemImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlHeader)).EndInit();
            this.panelControlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueSeasonality.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueAcceptsCustomPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueVatCategory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExtraDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePackingMeasurementUnit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAcceptsCustomDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPoints.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackingQty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInsertedOn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInsertedOn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDefaultBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxOrderQty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBuyer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReferenceUnit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContentUnit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsCentralStored.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinOrderQty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMotherCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueItemSupplier.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExtraFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupAllItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMotherCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcVatCategory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDefaultBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcInsertedOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackingMeasurementUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPackingQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcItemSupplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMinOrderQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMaxOrderQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBreakOrderToCentral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExtraDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSeasonality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReferenceUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcContentUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAcceptsCustomPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExtraFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelChildItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlHeader;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupAllItems;
        private DevExpress.XtraEditors.TextEdit txtCode;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.CheckEdit chkIsActive;
        private DevExpress.XtraTab.XtraTabControl tabItemInfo;
        private DevExpress.XtraTab.XtraTabPage xtpSubItems;
        private DevExpress.XtraTab.XtraTabPage xtpBarcodes;
        private DevExpress.XtraTab.XtraTabPage xtpPriceCatalogs;
        private DevExpress.XtraTab.XtraTabPage xtpItemCategories;
        private DevExpress.XtraEditors.PanelControl panelControlHeader;
        private DevExpress.XtraEditors.SimpleButton btnCancelItem;
        private DevExpress.XtraEditors.SimpleButton btnSaveItem;
        private DevExpress.XtraEditors.LookUpEdit lueSeasonality;
        private DevExpress.XtraEditors.LookUpEdit lueAcceptsCustomPrice;
        private DevExpress.XtraEditors.LookUpEdit lueVatCategory;
        private DevExpress.XtraEditors.TextEdit txtExtraDescription;
        private DevExpress.XtraEditors.LookUpEdit luePackingMeasurementUnit;
        private DevExpress.XtraEditors.CheckEdit chkAcceptsCustomDescription;
        private DevExpress.XtraEditors.SpinEdit txtPoints;
        private DevExpress.XtraEditors.SpinEdit txtPackingQty;
        private DevExpress.XtraEditors.DateEdit dtInsertedOn;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraLayout.LayoutControlItem lcName;
        private DevExpress.XtraLayout.LayoutControlItem lcIsActive;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemHeader;
        private DevExpress.XtraLayout.LayoutControlItem lcMaxOrderQty;
        private DevExpress.XtraLayout.LayoutControlItem lcMotherCode;
        private DevExpress.XtraLayout.LayoutControlItem lcItemSupplier;
        private DevExpress.XtraLayout.LayoutControlItem lcDefaultBarcode;
        private DevExpress.XtraLayout.LayoutControlItem lcSeasonality;
        private DevExpress.XtraLayout.LayoutControlItem lcAcceptsCustomPrice;
        private DevExpress.XtraLayout.LayoutControlItem lcVatCategory;
        private DevExpress.XtraLayout.LayoutControlItem lcExtraDescription;
        private DevExpress.XtraLayout.LayoutControlItem lcBuyer;
        private DevExpress.XtraLayout.LayoutControlItem lcPackingMeasurementUnit;
        private DevExpress.XtraLayout.LayoutControlItem lcBreakOrderToCentral;
        private DevExpress.XtraLayout.LayoutControlItem lcReferenceUnit;
        private DevExpress.XtraLayout.LayoutControlItem lcContentUnit;
        private DevExpress.XtraLayout.LayoutControlItem lcPoints;
        private DevExpress.XtraLayout.LayoutControlItem lcPackingQty;
        private DevExpress.XtraLayout.LayoutControlItem lcInsertedOn;
        private DevExpress.XtraLayout.LayoutControlItem lcSex;
        private DevExpress.XtraEditors.LookUpEdit lueDefaultBarcode;
        private DevExpress.XtraEditors.SpinEdit txtMaxOrderQty;
        private DevExpress.XtraEditors.LookUpEdit lueBuyer;
        private DevExpress.XtraEditors.SpinEdit txtReferenceUnit;
        private DevExpress.XtraEditors.SpinEdit txtContentUnit;
        private DevExpress.XtraEditors.CheckEdit chkIsCentralStored;
        private DevExpress.XtraEditors.MemoEdit memoRemarks;
        private DevExpress.XtraLayout.LayoutControlItem lcRemarks;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevExpress.XtraTab.XtraTabPage xtpLinkedItems;
        private DevExpress.XtraTab.XtraTabPage xtpStores;
        private DevExpress.XtraGrid.GridControl gridItemBarcodes;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewItemBarcodes;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBarcodeCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBarcodeMeasurementUnit;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPLUCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPLUPrefix;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBarcodeRelationFactor;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBarcodeType;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeleteItemBarcode;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_delete_ItemBarcode;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraGrid.GridControl gridPriceCatalogs;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewPriceCatalogDetails;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPriceCatalog;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDatabaseValue;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBarcode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDiscount;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeletePriceCatalogDetail;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_delete_PriceCatalog;
        private DevExpress.XtraGrid.Columns.GridColumn grdColMarkUp;
        private DevExpress.XtraGrid.Columns.GridColumn grdColVatIncluded;
        private DevExpress.XtraGrid.Columns.GridColumn grdColumnPriceCatalogDetailIsActive;
        private DevExpress.XtraGrid.GridControl gridItemCategories;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewItemCategories;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCategoryRoot;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCategoryPath;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCategoryNode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColDelItemCategory;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_delete_category;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_Del_SubItem;
        private DevExpress.XtraGrid.GridControl gridLinkedItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewLinkedItems;
        private DevExpress.XtraGrid.Columns.GridColumn grdColLinkedItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColLinkedItemName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColLinkedItemQtyFactor;
        private DevExpress.XtraGrid.GridControl gridSubItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewSubItems;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSubItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSubItemName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSubItemQtyFactor;
        private DevExpress.XtraGrid.GridControl gridItemStores;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewItemStores;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemStoreCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemStoreName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeleteLinkedItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_Del_LinkedItem;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeleteSubItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_delete_SubItem;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeleteItemStore;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_Del_ItemStore;
        private DevExpress.XtraLayout.LayoutControlItem lcExtraFile;
        private DevExpress.XtraEditors.ImageEdit imageEditItemImage;
        private DevExpress.XtraEditors.SpinEdit txtMinOrderQty;
        private DevExpress.XtraLayout.LayoutControlItem lcMinOrderQty;
        private DevExpress.XtraEditors.SearchLookUpEdit lueMotherCode;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.SearchLookUpEdit lueItemSupplier;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemImage;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.ButtonEdit txtExtraFile;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbPriceCatalog;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtDatabaseValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbBarcode;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtDiscount;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtTimeValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit dtTimeValueFrom;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit dtTimeValueUntil;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtMarkup;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkVatIncluded;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkPcdIsActive;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cmbLinkedItem;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtLinkedItemsQuantityFactor;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLinkedItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLinkedItemName;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cmbItemLinkedTo;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCmbItemLinkedTo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnItemLinkedToCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnItemLinkedToName;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtItemsLinkedToQuantityFactor;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemStoreStore;
        private DevExpress.XtraEditors.Repository.RepositoryItemTreeListLookUpEdit TreeListItemCategoryNode;
        private DevExpress.XtraTreeList.TreeList TreeListItemCategoryRepository;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemBarcodeMeasurementUnit;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit spinEditItemBarcodeRelationFactor;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemBarcodeBarcodeType;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        protected DevExpress.XtraGrid.Columns.GridColumn grdColCreatedOn;
        protected DevExpress.XtraGrid.Columns.GridColumn grdColUpdatedOn;
        protected DevExpress.XtraGrid.Columns.GridColumn grdColPriceCatalogCreatedOn;
        protected DevExpress.XtraGrid.Columns.GridColumn grdColPriceCatalogUpdatedOn;
        private LayoutControl layoutControl1;
        private LayoutControlGroup layoutControlGroup2;
        private LayoutControlItem layoutControlItem3;
        private LayoutControlItem layoutControlItem2;
        private DevExpress.XtraTab.XtraTabPage xtpChildItems;
        private DevExpress.XtraGrid.GridControl gridChildItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewChildItems;
        private DevExpress.XtraTab.XtraTabPage xtpItemStocks;
        private DevExpress.XtraGrid.GridControl gridItemStocks;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewItemStocks;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_Del_ItemStock;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemStockStore;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemStockBarcode;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit txtColItemStockQty;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemStockStore;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemStockQty;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_Del_ChildItem;
        private DevExpress.XtraGrid.Views.Grid.GridView cmbChildItemGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cmbChildItem;
        private DevExpress.XtraGrid.Columns.GridColumn grdColChildItemCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColChildItemName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDeleteChildItem;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageItemExtraInfo;
        private LayoutControl layoutControl2;
        private DevExpress.XtraEditors.TextEdit textEditItemExtraInfoDescription; 
        private LayoutControlGroup layoutControlGroup3;
        private LayoutControlItem layoutControlItemItemExtraInfoDescription; 
        private LayoutControlItem layoutControlItemItemExtraInfoLot;
        private LayoutControlItem layoutControlItemItemExtraInfoOrigin;
        //private DevExpress.Xpo.UnitOfWork unitOfWork1;
        private DevExpress.XtraEditors.MemoEdit txtIngredients;
        private DevExpress.XtraEditors.DateEdit dtExpiresAt;
        private DevExpress.XtraEditors.DateEdit dtPackedAt;
        private LayoutControlItem lcPackedAt;
        private LayoutControlItem lcExpiresAt;
        private LayoutControlItem lcIngredients;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnDelChildItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbLinkedItemName;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemLinkedToName;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbItemStoreStoreName;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cmbChildItemName;
        private EmptySpaceItem emptySpaceItem2;
        private EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewPriceCatalogDetailTimeValues;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDateFrom;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDateTo;
        private DevExpress.XtraGrid.Columns.GridColumn grdColTimeValue;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDeletePriceCatalogDetailTimeValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditDeletePriceCatalogDetailTimeValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit repositoryItemCalcEditPriceCatalogDetailTimeValue;
        private DevExpress.XtraEditors.TextEdit txtLot;
        private DevExpress.XtraEditors.TextEdit txtOrigin;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDesirableStock;
        protected DevExpress.XtraGrid.Columns.GridColumn gridColumnIsActive;
        private DevExpress.XtraEditors.CheckEdit DoesNotAllowDiscount;
        private DevExpress.XtraEditors.CheckEdit IsTax;
        private LayoutControlItem layoutControlItem4;
        private LayoutControlItem layoutControlItem5;
    }
}