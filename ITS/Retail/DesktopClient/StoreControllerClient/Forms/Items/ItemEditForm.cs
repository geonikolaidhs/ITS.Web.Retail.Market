using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using ITS.Retail.Common.ViewModel;
using DevExpress.XtraGrid.Views.Base;
using ITS.Retail.Common;
using DevExpress.XtraTreeList.Columns;
using ITS.Retail.Platform.Kernel;
using System.Threading;
using System.Globalization;
using ITS.Retail.Platform;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;
using DevExpress.Xpo.DB.Exceptions;
using System.Collections.ObjectModel;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class ItemEditForm : XtraLocalizedForm
    {
        protected bool onInitialization;
        protected Item _Item;
        protected Item Item { get { return _Item; } }
        private bool PreviewItem { get; set; }
        protected bool ItemIsSaved { get; set; }

        protected BindingList<ChildItemViewModel> ChildItemsViewModel { get; set; }
        protected BindingList<ItemBarcodeViewModel> ItemBarcodesViewModel { get; set; }
        protected BindingList<PriceCatalogDetailViewModel> PriceCatalogDetailsViewModel { get; set; }
        protected BindingList<LinkedItemViewModel> LinkedItemsViewModel { get; set; }
        protected BindingList<LinkedItemViewModel> SubItemsViewModel { get; set; }
        protected BindingList<ItemStoreViewModel> ItemStoresViewModel { get; set; }
        protected BindingList<ItemStockViewModel> ItemStocksViewModel { get; set; }

        protected PlatformPriceCatalogDetailService PriceCatalogDetailService { get; set; }

        public ItemEditForm(Item item, bool previewMode)
        {
            this._Item = item;
            PreviewItem = previewMode;

            InitializeComponent();

            if (PreviewItem)
            {
                string title = Resources.Item;
                if (item != null)
                {
                    title += " " + item.Name;
                }
                this.Text = title;
            }

            ItemBarcodesViewModel = new BindingList<ItemBarcodeViewModel>();
            foreach (ItemBarcode itemBarcode in this.Item.ItemBarcodes)
            {
                ItemBarcodeViewModel itemBarcodeViewModel = new ItemBarcodeViewModel();
                itemBarcodeViewModel.LoadPersistent(itemBarcode);
                ItemBarcodesViewModel.Add(itemBarcodeViewModel);
            }

            ChildItemsViewModel = new BindingList<ChildItemViewModel>();
            this.Item.ChildItems.Select(x => x.Oid).ToList().ForEach(x => ChildItemsViewModel.Add(new ChildItemViewModel() { Item = x }));

            PriceCatalogDetailsViewModel = new BindingList<PriceCatalogDetailViewModel>();

            foreach (PriceCatalogDetail detail in this.Item.PriceCatalogs)
            {
                PriceCatalogDetailViewModel pcdvm = new PriceCatalogDetailViewModel();
                pcdvm.LoadPersistent(detail);
                PriceCatalogDetailsViewModel.Add(pcdvm);
            }

            LinkedItemsViewModel = new BindingList<LinkedItemViewModel>();
            foreach (LinkedItem linkeditem in this.Item.LinkedItems)
            {
                LinkedItemViewModel model = new LinkedItemViewModel();
                model.LoadPersistent(linkeditem);
                LinkedItemsViewModel.Add(model);
            }

            SubItemsViewModel = new BindingList<LinkedItemViewModel>();
            foreach (LinkedItem subitem in this.Item.SubItems)
            {
                LinkedItemViewModel model = new LinkedItemViewModel();
                model.LoadPersistent(subitem);
                SubItemsViewModel.Add(model);
            }

            ItemStoresViewModel = new BindingList<ItemStoreViewModel>();
            foreach (ItemStore itemstore in this.Item.Stores)
            {
                ItemStoreViewModel model = new ItemStoreViewModel();
                model.LoadPersistent(itemstore);
                ItemStoresViewModel.Add(model);
            }

            ItemStocksViewModel = new BindingList<ItemStockViewModel>();
            foreach (ItemStock itemstock in this.Item.ItemStocks)
            {
                ItemStockViewModel model = new ItemStockViewModel();
                model.LoadPersistent(itemstock);
                ItemStocksViewModel.Add(model);
            }

            if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                grdColDeletePriceCatalogDetail.Visible = false;
                btn_delete_ItemBarcode.Buttons[0].Enabled = false;
            }
        }

        private void InitializeBindings()
        {
            this.lueMotherCode.DataBindings.Add("EditValue", this.Item, "MotherCode!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDefaultBarcode.DataBindings.Add("EditValue", this.Item, "DefaultBarcode!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueVatCategory.DataBindings.Add("EditValue", this.Item, "VatCategory!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.luePackingMeasurementUnit.DataBindings.Add("EditValue", this.Item, "PackingMeasurementUnit!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueSeasonality.DataBindings.Add("EditValue", this.Item, "Seasonality!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueItemSupplier.DataBindings.Add("EditValue", this.Item, "DefaultSupplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueBuyer.DataBindings.Add("EditValue", this.Item, "Buyer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueAcceptsCustomPrice.DataBindings.Add("EditValue", this.Item, "CustomPriceOptions", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtCode.DataBindings.Add("EditValue", this.Item, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtName.DataBindings.Add("EditValue", this.Item, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPackingQty.DataBindings.Add("EditValue", this.Item, "PackingQty", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPoints.DataBindings.Add("EditValue", this.Item, "Points", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtReferenceUnit.DataBindings.Add("EditValue", this.Item, "ReferenceUnit", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtContentUnit.DataBindings.Add("EditValue", this.Item, "ContentUnit", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtMinOrderQty.DataBindings.Add("EditValue", this.Item, "MinOrderQty", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtMaxOrderQty.DataBindings.Add("EditValue", this.Item, "MaxOrderQty", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtExtraDescription.DataBindings.Add("EditValue", this.Item, "ExtraDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtExtraFile.DataBindings.Add("EditValue", this.Item, "ExtraFileName", true, DataSourceUpdateMode.OnPropertyChanged);

            this.memoRemarks.DataBindings.Add("EditValue", this.Item, "Remarks", true, DataSourceUpdateMode.OnPropertyChanged);

            this.chkAcceptsCustomDescription.DataBindings.Add("EditValue", this.Item, "AcceptsCustomDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            this.chkIsCentralStored.DataBindings.Add("EditValue", this.Item, "IsCentralStored", true, DataSourceUpdateMode.OnPropertyChanged);
            this.chkIsActive.DataBindings.Add("EditValue", this.Item, "IsActive", true, DataSourceUpdateMode.OnPropertyChanged);

            this.IsTax.DataBindings.Add("EditValue", this.Item, "IsTax", true, DataSourceUpdateMode.OnPropertyChanged);
            this.DoesNotAllowDiscount.DataBindings.Add("EditValue", this.Item, "DoesNotAllowDiscount", true, DataSourceUpdateMode.OnPropertyChanged);

            this.dtInsertedOn.DataBindings.Add("EditValue", this.Item, "InsertedDate", true, DataSourceUpdateMode.OnPropertyChanged);

            this.imageEditItemImage.Image = this.Item.ImageSmall;

            this.gridItemBarcodes.DataSource = this.ItemBarcodesViewModel;
            this.gridPriceCatalogs.DataSource = this.PriceCatalogDetailsViewModel;

            this.gridItemCategories.DataSource = this.Item.ItemAnalyticTrees;
            this.gridChildItems.DataSource = this.ChildItemsViewModel;
            this.gridLinkedItems.DataSource = this.LinkedItemsViewModel;
            this.gridSubItems.DataSource = this.SubItemsViewModel;
            this.gridItemStores.DataSource = this.ItemStoresViewModel;
            this.gridItemStocks.DataSource = this.ItemStocksViewModel;

            if (this.Item.ItemExtraInfos == null)
            {
                this.Item.ItemExtraInfos.FirstOrDefault(x => x.Store.Oid == Program.Settings.StoreControllerSettings.Store.Oid).Item = this.Item;

            }
            this.textEditItemExtraInfoDescription.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtPackedAt.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "PackedAt", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtExpiresAt.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "ExpiresAt", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtIngredients.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "Ingredients", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtLot.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "Lot", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtOrigin.DataBindings.Add("EditValue", this.Item.ItemExtraInfos, "Origin", true, DataSourceUpdateMode.OnPropertyChanged);

        }

        private void InitializeLookupEdits()
        {
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
            CriteriaOperator standardCriteria = CriteriaOperator.And(ownerCriteria, activeCriteria);

            lueMotherCode.Properties.DataSource = new XPServerCollectionSource(this.Item.Session, typeof(Item),
                                                            CriteriaOperator.And(standardCriteria,
                                                                                new BinaryOperator("Oid", this.Item.Oid, BinaryOperatorType.NotEqual)));
            this.lueMotherCode.Properties.ValueMember = "Oid";
            this.lueMotherCode.Properties.DisplayMember = "Name";
            this.lueMotherCode.Properties.View.Columns.AddField("Code").Visible = true;
            this.lueMotherCode.Properties.View.Columns.AddField("Name").Visible = true;

            lueVatCategory.Properties.DataSource = new XPCollection<VatCategory>(this.Item.Session, standardCriteria);
            lueVatCategory.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueVatCategory.Properties.ValueMember = "Oid";

            lueDefaultBarcode.Properties.DataSource = this.ItemBarcodesViewModel.Select(itembarcode => itembarcode.Barcode).ToList();
            lueDefaultBarcode.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Barcode));
            lueDefaultBarcode.Properties.ValueMember = "Oid";
            lueDefaultBarcode.Properties.DisplayMember = "Code";

            luePackingMeasurementUnit.Properties.DataSource = new XPCollection<MeasurementUnit>(this.Item.Session, standardCriteria);
            luePackingMeasurementUnit.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            luePackingMeasurementUnit.Properties.ValueMember = "Oid";

            lueSeasonality.Properties.DataSource = new XPCollection<Seasonality>(this.Item.Session, standardCriteria);
            lueSeasonality.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueSeasonality.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueSeasonality.Properties.ValueMember = "Oid";
            lueSeasonality.Properties.DisplayFormat.FormatString = "{0} {1}";

            lueItemSupplier.Properties.DataSource = new XPServerCollectionSource(this.Item.Session, typeof(SupplierNew), standardCriteria);
            this.lueItemSupplier.Properties.ValueMember = "Oid";
            this.lueItemSupplier.Properties.DisplayMember = "CompanyName";
            this.lueItemSupplier.Properties.View.Columns.AddField("Code").Visible = true;
            this.lueItemSupplier.Properties.View.Columns.AddField("CompanyName").Visible = true;

            lueBuyer.Properties.DataSource = new XPCollection<Buyer>(this.Item.Session, standardCriteria);
            lueBuyer.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueBuyer.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueBuyer.Properties.ValueMember = "Oid";
            lueBuyer.Properties.DisplayFormat.FormatString = "{0} {1}";

            lueAcceptsCustomPrice.Properties.DataSource = Enum<eItemCustomPriceOptions>.GetLocalizedDictionary();
            lueAcceptsCustomPrice.Properties.Columns.Add(new LookUpColumnInfo("Value", Resources.Description));
            lueAcceptsCustomPrice.Properties.ValueMember = "Key";
            lueAcceptsCustomPrice.Properties.DisplayMember = "Value";

            cmbChildItem.View.Columns.Clear();
            cmbChildItem.View.Columns.AddVisible("Code", Resources.Code);
            cmbChildItem.View.Columns.AddVisible("Name", Resources.Name);

            LookUpColumnInfo lookUpColCode = new LookUpColumnInfo("Code", Resources.Code);
            LookUpColumnInfo lookUpColDescription = new LookUpColumnInfo("Description", Resources.Description);

            cmbBarcode.Columns.Add(lookUpColCode);
            cmbPriceCatalog.Columns.Add(lookUpColCode);
            cmbPriceCatalog.Columns.Add(lookUpColDescription);
            cmbItemStoreStore.Columns.Add(lookUpColCode);
            cmbItemStoreStore.Columns.Add(lookUpColDescription);
            cmbItemStockStore.Columns.Add(lookUpColCode);
            cmbItemStockStore.Columns.Add(lookUpColDescription);
            cmbItemBarcodeBarcodeType.Columns.Add(lookUpColCode);
            cmbItemBarcodeBarcodeType.Columns.Add(lookUpColDescription);
            cmbItemBarcodeMeasurementUnit.Columns.Add(lookUpColCode);
            cmbItemBarcodeMeasurementUnit.Columns.Add(lookUpColDescription);
            cmbItemStockBarcode.Columns.Add(lookUpColCode);

            XPServerCollectionSource itemsCollection = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(Item), standardCriteria);
            XPCollection<Store> storesCollection = new XPCollection<Store>(Program.Settings.ReadOnlyUnitOfWork, standardCriteria);

            cmbBarcode.DataSource = cmbItemStockBarcode.DataSource = this.ItemBarcodesViewModel.Select(x => x.Barcode).ToList();
            cmbItemBarcodeBarcodeType.DataSource = new XPCollection<BarcodeType>(Program.Settings.ReadOnlyUnitOfWork, standardCriteria);
            cmbItemBarcodeMeasurementUnit.DataSource = new XPCollection<MeasurementUnit>(Program.Settings.ReadOnlyUnitOfWork, standardCriteria);
            cmbPriceCatalog.DataSource = new XPCollection<PriceCatalog>(Program.Settings.ReadOnlyUnitOfWork, standardCriteria);
            cmbChildItem.DataSource = cmbChildItemName.DataSource = itemsCollection;
            cmbLinkedItem.DataSource = cmbLinkedItemName.DataSource = itemsCollection;
            cmbItemLinkedTo.DataSource = cmbItemLinkedToName.DataSource = itemsCollection;
            cmbItemStoreStore.DataSource = cmbItemStoreStoreName.DataSource = storesCollection;
            cmbItemStockStore.DataSource = storesCollection;
            TreeListItemCategoryNode.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(ItemCategory), standardCriteria);
            foreach (TreeListColumn col in this.TreeListItemCategoryRepository.Columns)
            {
                col.Caption = LocalizeString(col.Caption);
            }
        }

        private void ItemEditForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                onInitialization = false;
                return;
            }

            InitializeLookupEdits();
            InitializeBindings();

            if (PreviewItem || Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                if (PreviewItem)
                {
                    this.btnCancelItem.Text = Resources.Close;
                    IEnumerable<SimpleButton> buttons = this.EnumerateComponents().Where(x => x is SimpleButton).Cast<SimpleButton>();
                    foreach (SimpleButton btn in buttons)
                    {
                        if (btn != this.btnCancelItem)
                        {
                            btn.Visible = btn.Enabled = false;
                        }
                        else
                        {
                            btn.Text = Resources.Close;
                        }
                    }
                }
                IEnumerable<BaseEdit> editControls = this.EnumerateComponents().Where(x => x is BaseEdit).Cast<BaseEdit>();
                foreach (BaseEdit editControl in editControls)
                {
                    editControl.ReadOnly = true;
                    if (editControl is ButtonEdit)
                    {
                        EditorButtonCollection editbuttons = (editControl as ButtonEdit).Properties.Buttons;
                        if (editbuttons.Count > 0)
                        {
                            foreach (EditorButton btn in editbuttons)
                            {
                                btn.Enabled = btn.Visible = false;
                            }
                        }
                    }

                    if (editControl is DateEdit)
                    {
                        (editControl as DateEdit).Properties.AllowDropDownWhenReadOnly = DefaultBoolean.False;
                    }
                }

                IEnumerable<GridView> gridViews = this.EnumerateComponents().Where(x => x is GridView).Cast<GridView>();
                foreach (GridView gridView in gridViews)
                {
                    gridView.OptionsBehavior.Editable = false;
                    gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                    foreach (GridColumn col in gridView.Columns)
                    {
                        if (col.ColumnEdit != null)
                        {
                            if (col.ColumnEdit.EditorTypeName == "ButtonEdit")
                            {
                                col.Visible = false;
                            }
                            else
                            {
                                col.ColumnEdit.ReadOnly = true;
                            }
                        }
                    }
                }
            }

            if (PreviewItem == false
                && Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE
                && this.Item.ItemExtraInfos.Count == 0
                )
            {
                ItemExtraInfo itenmExtraInfo = new ItemExtraInfo(this.Item.Session)
                {
                    Item = this.Item,
                    Store = this.Item.Session.GetObjectByKey<Store>(Program.Settings.StoreControllerSettings.Store.Oid)
                };
            }

            this.tabItemInfo.SelectedTabPageIndex = 0;
            onInitialization = false;

            ReenableStoreControllerEditableComponents();
        }

        //Delete Actions

        private void btn_delete_ItemBarcode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ItemBarcodeViewModel model = this.grdViewItemBarcodes.GetFocusedRow() as ItemBarcodeViewModel;
            if (model != null)
            {
                this.ItemBarcodesViewModel.Remove(model);
            }
            this.grdViewItemBarcodes.RefreshData();
        }

        private void btn_delete_PriceCatalog_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (Program.Settings.MasterAppInstance != eApplicationInstance.STORE_CONTROLER)
            {
                PriceCatalogDetailViewModel model = this.grdViewPriceCatalogDetails.GetFocusedRow() as PriceCatalogDetailViewModel;
                if (model != null)
                {
                    this.PriceCatalogDetailsViewModel.Remove(model);
                }
                this.grdViewPriceCatalogDetails.RefreshData();
            }
        }

        private void btn_delete_category_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ItemAnalyticTree iat = this.grdViewItemCategories.GetFocusedRow() as ItemAnalyticTree;
            if (iat != null)
            {
                iat.Delete();
            }
            this.grdViewItemCategories.RefreshData();
        }

        private void btnDelChildItem_Click(object sender, EventArgs e)
        {
            ChildItemViewModel model = this.grdViewChildItems.GetFocusedRow() as ChildItemViewModel;
            if (model != null)
            {
                this.ChildItemsViewModel.Remove(model);
            }
            this.grdViewChildItems.RefreshData();
        }

        private void btn_Del_LinkedItem_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            LinkedItemViewModel model = this.grdViewLinkedItems.GetFocusedRow() as LinkedItemViewModel;
            if (model != null)
            {
                this.LinkedItemsViewModel.Remove(model);
            }
            this.grdViewLinkedItems.RefreshData();
        }

        private void btn_delete_SubItem_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            LinkedItemViewModel model = this.grdViewSubItems.GetFocusedRow() as LinkedItemViewModel;
            if (model != null)
            {
                this.SubItemsViewModel.Remove(model);
            }
            this.grdViewSubItems.RefreshData();
        }

        private void btn_Del_ItemStore_Click(object sender, EventArgs e)
        {
            ItemStoreViewModel model = this.grdViewItemStores.GetFocusedRow() as ItemStoreViewModel;
            if (model != null)
            {
                this.ItemStoresViewModel.Remove(model);
            }
            this.grdViewItemStores.RefreshData();
        }

        //Edit Actions
        private bool ObjectCanBeSaved(out string errormessage, out int selectedTabIndex)
        {
            errormessage = string.Empty;
            selectedTabIndex = 0;
            if (String.IsNullOrWhiteSpace(this.Item.Code))
            {
                errormessage = Resources.CodeIsEmpty;
                return false;
            }
            if (String.IsNullOrWhiteSpace(this.Item.Name))
            {
                errormessage = string.Format(Resources.RequiredFieldError, Resources.Name);
                return false;
            }

            if (this.Item.VatCategory == null)
            {
                errormessage = Resources.VatCategoryIsEmpty;
                return false;
            }
            if (lueDefaultBarcode.EditValue == null || lueDefaultBarcode.EditValue == DBNull.Value)
            {
                errormessage = string.Format(Resources.RequiredFieldError, Resources.DefaultBarcode);
                return false;
            }
            CriteriaOperator sameCodeFilter = CriteriaOperator.And(new BinaryOperator("Code", this.Item.Code), new BinaryOperator("Oid", this.Item.Oid, BinaryOperatorType.NotEqual));
            Item sameItemCode = this.Item.Session.FindObject<Item>(sameCodeFilter);
            if (sameItemCode != null)
            {
                errormessage = Resources.CodeAlreadyExists;
                return false;
            }

            if (this.PriceCatalogDetailService == null)
            {
                this.PriceCatalogDetailService = new PlatformPriceCatalogDetailService();
            }

            if (this.Item.DoesNotAllowDiscount == true || this.Item.IsTax == true)
            {
                int numberOfDiscounts = PriceCatalogDetailsViewModel.Where(priceDetail => priceDetail.Discount > 0).Count();
                if (numberOfDiscounts > 0)
                {
                    errormessage = Resources.NotAllowDiscounts;
                    return false;
                }
            }


            foreach (PriceCatalogDetailViewModel pcdtv in PriceCatalogDetailsViewModel)
            {
                selectedTabIndex = 1;
                ValidationPriceCatalogDetailTimeValuesResult result = this.PriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(pcdtv.TimeValues);
                if (result != null)
                {
                    errormessage = (result.PartialOverlap ? Resources.PARTIALLY_OVERLAPPING_TIME_VALUES : Resources.Error)
                                                            + Environment.NewLine + Resources.FromDate + ": " + result.From.ToString()
                                                            + Environment.NewLine + Resources.ToDate + ": " + result.To.ToString();
                    return false;
                }
            }
            return true;
        }


        private void btnCancelItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ItemEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ItemIsSaved == false)
            {
                e.Cancel = !this.CancelItem();
            }
        }

        private bool CancelItem()
        {
            if (this.PreviewItem || XtraMessageBox.Show(Resources.Cancel, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (this.PreviewItem == false)
                {
                    this.Item.Session.RollbackTransaction();
                }
                return true;
            }
            return false;
        }

        protected void SaveItem()
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            try
            {
                #region check if there is a connection
                if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                    using (ITSStoreControllerDesktopServiceClient storeControllerDesktopServiceClient = Program.Settings.ITSStoreControllerDesktopService)
                    {
                        ApplicationStatus applicationStatus = storeControllerDesktopServiceClient.GetApplicationStatus();
                        if (applicationStatus != ApplicationStatus.ONLINE)
                        {
                            XtraMessageBox.Show(Resources.ApplicationMustBeConnectedToHeadQuartersToEditPrices, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }
                #endregion

                #region Manage item info master and detail elements from view models no matter the application instance
                //ItemBarcodes
                //Remove
                List<Guid> newIbcsOids = this.ItemBarcodesViewModel.Select(x => x.Oid).ToList();
                this.Item.ItemBarcodes.Where(x => !newIbcsOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                //Add or update
                foreach (ItemBarcodeViewModel ibcvm in this.ItemBarcodesViewModel)
                {
                    ItemBarcode ibc = this.Item.ItemBarcodes.FirstOrDefault(x => x.Oid == ibcvm.Oid);
                    if (ibc == null)
                    {
                        ibc = new ItemBarcode(this.Item.Session) { Owner = this.Item.Owner };
                    }
                    ibcvm.Persist(ibc);
                    this.Item.ItemBarcodes.Add(ibc);
                }
                if (this.Item.DefaultBarcode == null)
                {
                    this.Item.DefaultBarcode = this.Item.ItemBarcodes.FirstOrDefault(x => x.Barcode.Oid == (Guid)this.lueDefaultBarcode.EditValue).Barcode;
                }

                //PriceCatalogDetails
                //Remove
                List<Guid> newPcdOids = this.PriceCatalogDetailsViewModel.Select(x => x.Oid).ToList();
                this.Item.PriceCatalogs.Where(x => !newPcdOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                //Add or Update
                foreach (PriceCatalogDetailViewModel pcdvm in this.PriceCatalogDetailsViewModel)
                {
                    PriceCatalogDetail pcd = this.Item.PriceCatalogs.FirstOrDefault(x => x.Oid == pcdvm.Oid);
                    if (pcd == null)
                    {
                        pcd = new PriceCatalogDetail(this.Item.Session);
                    }
                    pcdvm.Persist(pcd);
                    this.Item.PriceCatalogs.Add(pcd);
                }

                ////Child Items
                List<Guid> newChildItemOids = this.ChildItemsViewModel.Select(x => x.Oid).ToList();
                this.Item.ChildItems.Where(x => !newChildItemOids.Contains(x.Oid)).ToList().ForEach(x => x.MotherCodeOid = null);
                this.ChildItemsViewModel.ToList().ForEach(childItemViewModel =>
                {
                    Item item = this.Item.ChildItems.FirstOrDefault(x => x.Oid == childItemViewModel.Item);
                    if (item == null)
                    {
                        item = this.Item.Session.GetObjectByKey<Item>(childItemViewModel.Item);
                        item.MotherCodeOid = this.Item.Oid;
                    }
                });

                ////Linked Items
                List<Guid> newLinkedItemOids = this.LinkedItemsViewModel.Select(x => x.Oid).ToList();
                this.Item.LinkedItems.Where(x => !newLinkedItemOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                foreach (LinkedItemViewModel linkedItemViewModel in this.LinkedItemsViewModel)
                {
                    LinkedItem linkedItem = this.Item.LinkedItems.FirstOrDefault(x => x.Oid == linkedItemViewModel.Oid);
                    if (linkedItem == null)
                    {
                        linkedItem = new LinkedItem(this.Item.Session);
                    }
                    linkedItemViewModel.Persist(linkedItem);
                    this.Item.LinkedItems.Add(linkedItem);
                }

                ////Sub Items
                List<Guid> newSubItemOids = this.SubItemsViewModel.Select(x => x.Oid).ToList();
                this.Item.SubItems.Where(x => !newSubItemOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                foreach (LinkedItemViewModel subItemViewModel in this.SubItemsViewModel)
                {
                    LinkedItem subItem = this.Item.SubItems.FirstOrDefault(x => x.Oid == subItemViewModel.Oid);
                    if (subItem == null)
                    {
                        subItem = new LinkedItem(this.Item.Session);
                    }
                    subItemViewModel.Persist(subItem);
                    this.Item.SubItems.Add(subItem);
                }

                ////Items Stores
                List<Guid> newItemStoreOids = this.ItemStoresViewModel.Select(x => x.Oid).ToList();
                this.Item.Stores.Where(x => !newItemStoreOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                foreach (ItemStoreViewModel itemStoreViewModel in this.ItemStoresViewModel)
                {
                    ItemStore itemStore = this.Item.Stores.FirstOrDefault(x => x.Oid == itemStoreViewModel.Oid);
                    if (itemStore == null)
                    {
                        itemStore = new ItemStore(this.Item.Session);
                    }
                    itemStoreViewModel.Persist(itemStore);
                    this.Item.Stores.Add(itemStore);
                }

                ////Items Stocks
                List<Guid> newItemStocksOids = this.ItemStocksViewModel.Select(x => x.Oid).ToList();
                this.Item.ItemStocks.Where(x => !newItemStocksOids.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                foreach (ItemStockViewModel itemStockViewModel in this.ItemStocksViewModel)
                {
                    ItemStock itemStock = this.Item.ItemStocks.FirstOrDefault(x => x.Oid == itemStockViewModel.Oid);
                    if (itemStock == null)
                    {
                        itemStock = new ItemStock(this.Item.Session);
                    }
                    itemStockViewModel.Persist(itemStock);
                    this.Item.ItemStocks.Add(itemStock);
                }
                #endregion

                if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    using (ITSStoreControllerDesktopPOSUpdateService.POSUpdateService pOSUpdateService = new ITSStoreControllerDesktopPOSUpdateService.POSUpdateService())
                    {
                        pOSUpdateService.Url = Program.Settings.MasterURLService;

                        string errorMessage = string.Empty;

                        foreach (PriceCatalogDetail storeControllerPriceCatalogDetail in this.Item.PriceCatalogs.Where(priceDetail => priceDetail.HasChangedOrHasTimeValueChanges))
                        {
                            string jsonItem = storeControllerPriceCatalogDetail.JsonWithDetails(PlatformConstants.JSON_SERIALIZER_SETTINGS, false);
                            if (pOSUpdateService.InsertOrUpdateRecord(Program.Settings.StoreControllerSettings.Oid, "PriceCatalogDetail", jsonItem, out errorMessage)
                                == false
                               )
                            {
                                XtraMessageBox.Show(errorMessage, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        #region manage deletion of item details
                        List<object> itemDetails = this.Item.Session.GetObjectsToSave().Cast<object>().Where(itemDetailObject => itemDetailObject is BaseObj).ToList();
                        foreach (BaseObj currentBaseObject in itemDetails)
                        {
                            if (currentBaseObject.IsDeleted)
                            {
                                pOSUpdateService.DeleteRecord(Program.Settings.StoreControllerSettings.Oid,
                                    currentBaseObject.GetType().Name,
                                    currentBaseObject.Oid,
                                    out errorMessage
                                    );
                            }
                        }
                        #endregion
                        //this.Item.Save();// For safety reasons
                        XpoHelper.CommitChanges((UnitOfWork)Item.Session);
                        this.ItemIsSaved = true;
                        this.Close();
                    }
                }
                else// Is Dual
                {
                    this.Item.Save();
                    XpoHelper.CommitChanges((UnitOfWork)Item.Session);
                    this.ItemIsSaved = true;
                    this.Close();
                }
            }
            catch (LockingException ex2)
            {
                string exceptionMessage = ex2.GetFullMessage();
                XtraMessageBox.Show(
                    Resources.ObjectHasBeenUpdatedFromHQ + Environment.NewLine +
                    Resources.PossibleOwnUpdate + Environment.NewLine +
                    Resources.DoYouWantToReload
                    , Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ItemIsSaved = true;
                this.DialogResult = DialogResult.Retry;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            }
        }

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            string message;
            int selecteTabIndex = 0;
            if (ObjectCanBeSaved(out message, out selecteTabIndex))
            {
                if (ValidateItemCodeBarcode())
                {
                    this.SaveItem();
                }
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.tabItemInfo.SelectedTabPageIndex = selecteTabIndex;
            }
        }

        private bool ValidateItemCodeBarcode()
        {
            string paddedCode = this.Item.Code;
            if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadBarcodes)
            {
                int length = Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodeLength;
                char chr = Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0];
                paddedCode = paddedCode.PadLeft(length, chr);
            }
            if (this.ItemBarcodesViewModel.Any(x => x.Barcode.Code == paddedCode))
            {
                return true;
            }
            if (DialogResult.Yes == XtraMessageBox.Show(Resources.CreateBarcodeFromItemCode, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                grdViewItemBarcodes.FocusedColumn = grdColBarcodeCode;
                grdViewItemBarcodes.AddNewRow();
                grdViewItemBarcodes.ShowEditor();
                grdViewItemBarcodes.ActiveEditor.EditValue = paddedCode;
            }
            return false;
        }

        private void lueMotherCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private void lueDefaultBarcode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private void lueItemSupplier_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private void luePackingMeasurementUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private void lueSeasonality_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                (sender as LookUpEdit).EditValue = null;
            }
            else if (e.Button.Index == 2)
            {
                Guid newSeasonalityGuid = Guid.Empty;
                User currentUser = Program.Settings.CurrentUser;
                ITS.Retail.WebClient.Helpers.ePermition eap = UserHelper.GetUserEntityPermition(currentUser, "Seasonality");
                if (Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE && eap.HasFlag(ITS.Retail.WebClient.Helpers.ePermition.Insert))
                {
                    DialogResult dialogResult = DialogResult.None;
                    using (SeasonalityEditForm form = new SeasonalityEditForm())
                    {
                        dialogResult = form.ShowDialog();
                    }
                    if (dialogResult == DialogResult.OK)
                    {
                        lueSeasonality.Properties.DataSource = new XPCollection<Seasonality>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Item.Session,
                                                                                             new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
                        lueSeasonality.Refresh();
                    }
                }
                else
                {
                    XtraMessageBox.Show(Resources.YouDontHavePermissions, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
        }

        private void lueBuyer_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                (sender as LookUpEdit).EditValue = null;
            }
            else if (e.Button.Index == 2)
            {
                Guid newSeasonalityGuid = Guid.Empty;
                User currentUser = Program.Settings.CurrentUser;
                ITS.Retail.WebClient.Helpers.ePermition eap = UserHelper.GetUserEntityPermition(currentUser, "Buyer");
                if (Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE && eap.HasFlag(ITS.Retail.WebClient.Helpers.ePermition.Insert))
                {
                    DialogResult dialogResult = DialogResult.None;
                    using (BuyerEditForm form = new BuyerEditForm())
                    {
                        dialogResult = form.ShowDialog();
                    }
                    if (dialogResult == DialogResult.OK)
                    {
                        this.lueBuyer.Properties.DataSource = new XPCollection<Buyer>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Item.Session,
                                                                                      new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
                        lueBuyer.Refresh();
                    }
                }
                else
                {
                    XtraMessageBox.Show(Resources.YouDontHavePermissions, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
        }

        private void imageEditItemImage_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Tag != null && e.Button.Tag.ToString() == "Upload")
            {
                DialogResult dialogResult = DialogResult.None;
                using (ItemImageForm form = new ItemImageForm(this.Item))
                {
                    dialogResult = form.ShowDialog();
                }
                this.imageEditItemImage.Image = this.Item.ImageMedium;
            }
        }

        private void txtExtraFile_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Tag != null && e.Button.Tag.ToString() == "Upload")
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Allowed Files (.jpg, .jpeg, .jpe, .gif, .png, .doc, .docx, .pdf, .html)|*.jpg;*.jpeg;*.jpe;*.gif;*.png;*.doc;*.docx; *.pdf;*.html";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Item.ExtraFilename = Path.GetFileName(dialog.FileName); // get name of file
                    this.Item.ExtraFile = File.ReadAllBytes(dialog.FileName);
                    this.Item.ExtraMimeType = Path.GetExtension(dialog.FileName);
                }
            }
            else if (e.Button.Tag != null && e.Button.Tag.ToString() == "Delete")
            {
                this.Item.ExtraFile = null;
                this.Item.ExtraFilename = this.Item.ExtraMimeType = null;
            }
        }


        private void grdViewItemCategories_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            ItemAnalyticTree iat = e.Row as ItemAnalyticTree;
            if (iat.Node == null)
            {
                e.Valid = false;
                e.ErrorText = Resources.SelectCategory;
                return;
            }
            iat.Root = iat.Node.GetRoot(iat.Session);
            if (iat.Root != null && this.Item.ItemAnalyticTrees.Any(x => x.Oid != iat.Oid && x.Root != null && x.Root.Oid == iat.Root.Oid))
            {
                e.Valid = false;
                e.ErrorText = Resources.DuplicateItemCategory + "(" + Resources.CategoryRoot + ")";
                return;
            }
        }

        private void grdViewPriceCatalogs_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            PriceCatalogDetailViewModel pcd = e.Row as PriceCatalogDetailViewModel;
            if (pcd.PriceCatalog == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.NoPriceCatalogDefinded;
                return;
            }
            if (pcd.Barcode == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.Barcode;
                return;
            }
            if (this.PriceCatalogDetailsViewModel.Any(x => x.Oid != pcd.Oid && x.PriceCatalog == pcd.PriceCatalog && x.Barcode == pcd.Barcode))
            {
                e.Valid = false;
                e.ErrorText = Resources.PriceCatalogAlreadyExists;
                return;
            }

            if (pcd.MarkUp < 0)
            {
                e.Valid = false;
                e.ErrorText = Resources.MarkUp;
                return;
            }

            if (pcd.DatabaseValue < 0)
            {
                e.Valid = false;
                e.ErrorText = Resources.Value;
                return;
            }



            if (pcd.Discount < 0 || pcd.Discount > 1)
            {
                e.Valid = false;
                e.ErrorText = Resources.Discount;
                return;
            }


        }

        private void grdViewItemStores_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            ItemStoreViewModel iStore = e.Row as ItemStoreViewModel;
            if (iStore.Store == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.StoreIsEmpty;
                return;
            }

            if (this.Item.Stores.Any(x => x.Store.Oid == iStore.Store && x.Oid != iStore.Oid))
            {
                e.Valid = false;
                e.ErrorText = Resources.InvalidStoreCode;
                return;
            }


        }

        private void grdViewItemStock_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                e.Valid = false;
                e.ErrorText = Resources.YouCannotEditThisElement;
                return;
            }

            ItemStockViewModel iStock = e.Row as ItemStockViewModel;
            if (iStock.Store == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.StoreIsEmpty;
                return;
            }

            if (this.Item.ItemStocks.Any(x => x.Store.Oid == iStock.Store && x.Oid != iStock.Oid))
            {
                e.Valid = false;
                e.ErrorText = Resources.StoreAlreadyExists;
                return;
            }
        }

        private void grdViewItemBarcodes_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            ItemBarcodeViewModel ibcvm = e.Row as ItemBarcodeViewModel;

            if (string.IsNullOrWhiteSpace(ibcvm.Barcode.Code))
            {
                e.Valid = false;
                e.ErrorText = Resources.Barcode;
                return;
            }

            if (ibcvm.Type == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.BarcodeType;
                return;
            }

            if (ibcvm.MeasurementUnit == Guid.Empty)
            {
                e.Valid = false;
                e.ErrorText = Resources.MeasurementUnit;
                return;
            }
            if (ibcvm.RelationFactor < 0)
            {
                e.Valid = false;
                e.ErrorText = Resources.RelationFactor;
                return;
            }

            if (ItemBarcodesViewModel.Any(x => x.Oid != ibcvm.Oid && x.Barcode.Code == ibcvm.Barcode.Code))
            {
                e.Valid = false;
                e.ErrorText = Resources.BarcodeAlreadyAttached;
                return;
            }

            Barcode bc = this.Item.Session.FindObject<Barcode>(new BinaryOperator("Code", ibcvm.Barcode.Code));
            if (bc != null && bc.Oid != ibcvm.Barcode.Oid)
            {
                ItemBarcode ibc = this.Item.Session.FindObject<ItemBarcode>(
                    CriteriaOperator.And(new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid),
                                         new BinaryOperator("Barcode.Oid", bc.Oid)));
                if (ibc == null)
                {
                    ibcvm.Barcode.Oid = bc.Oid;
                }
                else
                {
                    e.Valid = false;
                    e.ErrorText = Resources.BarcodeAlreadyAttached + " (" + ibc.Item.Code + ")";
                    return;
                }
            }
            lueDefaultBarcode.Properties.DataSource = this.ItemBarcodesViewModel.Select(itembarcode => itembarcode.Barcode).ToList();
            cmbBarcode.DataSource = this.ItemBarcodesViewModel.Select(itembarcode => itembarcode.Barcode).ToList();
            lueDefaultBarcode.Refresh();
        }

        private void GridCustomDrawCellDeleteButton<T>(RowCellCustomDrawEventArgs e, GridColumn column, GridView view)
        {
            if (e.Column == column && view.GetRow(e.RowHandle) is T == false)
            {
                e.Handled = true;
            }
        }

        private void grdViewItemBarcodes_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<ItemBarcodeViewModel>(e, grdColDeleteItemBarcode, grdViewItemBarcodes);
        }

        private void grdViewPriceCatalogs_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<PriceCatalogDetailViewModel>(e, grdColDeletePriceCatalogDetail, grdViewPriceCatalogDetails);
        }

        private void grdViewItemCategories_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<ItemAnalyticTree>(e, gridColDelItemCategory, grdViewItemCategories);
        }

        private void grdViewChildItems_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<ChildItemViewModel>(e, grdColDeleteChildItem, grdViewChildItems);
        }

        private void grdViewLinkedItems_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<LinkedItemViewModel>(e, grdColDeleteLinkedItem, grdViewLinkedItems);
        }

        private void grdViewSubItems_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<LinkedItemViewModel>(e, grdColDeleteSubItem, grdViewSubItems);
        }

        private void grdViewItemStores_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<ItemStoreViewModel>(e, grdColDeleteItemStore, grdViewItemStores);
        }

        private void ItemEditForm_Shown(object sender, EventArgs e)
        {
            this.txtCode.Focus();
        }

        private void ReenableStoreControllerEditableComponents()
        {
            if (!this.PreviewItem)
            {
                this.textEditItemExtraInfoDescription.ReadOnly = false;
                this.txtIngredients.ReadOnly = false;
                this.txtLot.ReadOnly = false;
                this.txtOrigin.ReadOnly = false;

                this.dtPackedAt.ReadOnly = false;
                this.dtPackedAt.Properties.Buttons[0].Visible = true;
                this.dtPackedAt.Properties.Buttons[0].Enabled = true;

                this.dtExpiresAt.ReadOnly = false;
                this.dtExpiresAt.Properties.Buttons[0].Visible = true;
                this.dtExpiresAt.Properties.Buttons[0].Enabled = true;

                this.gridPriceCatalogs.Enabled = true;
                this.grdViewPriceCatalogDetailTimeValues.OptionsBehavior.Editable = true;
                this.grdViewPriceCatalogDetailTimeValues.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;

                this.grdViewPriceCatalogDetails.OptionsBehavior.Editable = true;
                this.gridColumnDeletePriceCatalogDetailTimeValue.Visible = true;
                //this.gridColumnDeletePriceCatalogDetailTimeValue.VisibleIndex = 3;
                this.grdColDatabaseValue.ColumnEdit.ReadOnly = false;
                this.grdColDiscount.ColumnEdit.ReadOnly = false;
                this.grdColVatIncluded.ColumnEdit.ReadOnly = false;
                this.grdColumnPriceCatalogDetailIsActive.ColumnEdit.ReadOnly = false;

                this.grdColTimeValue.OptionsColumn.AllowEdit = true;
                this.grdColTimeValue.ColumnEdit.ReadOnly = false;
                this.grdColDateFrom.OptionsColumn.AllowEdit = true;
                this.grdColDateTo.OptionsColumn.AllowEdit = true;

                this.grdColDateFrom.ColumnEdit.ReadOnly = false;
                this.grdColDateTo.ColumnEdit.ReadOnly = false;


            }
        }

        private void grdViewPriceCatalogDetailTimeValues_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridCustomDrawCellDeleteButton<PriceCatalogDetailViewModel>(e, gridColumnDeletePriceCatalogDetailTimeValue, grdViewPriceCatalogDetailTimeValues);
        }

        private void repositoryItemButtonEditDeletePriceCatalogDetailTimeValue_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            //GridView detailGridView = this.grdViewPriceCatalogDetails.GetDetailView(0, 0) as GridView;
            //PriceCatalogDetailTimeValueViewModel priceCatalogDetailTimeValueViewModel = detailGridView.GetFocusedRow() as PriceCatalogDetailTimeValueViewModel;
            PriceCatalogDetailTimeValueViewModel priceCatalogDetailTimeValueViewModel = (this.gridPriceCatalogs.FocusedView as GridView).GetFocusedRow() as PriceCatalogDetailTimeValueViewModel;
            if (priceCatalogDetailTimeValueViewModel != null)
            {
                PriceCatalogDetailTimeValue priceCatalogDetailTimeValue = this.Item.Session.GetObjectByKey<PriceCatalogDetailTimeValue>(priceCatalogDetailTimeValueViewModel.Oid);
                this.PriceCatalogDetailsViewModel.Where(priceDetailViewModel => priceDetailViewModel.Oid == priceCatalogDetailTimeValue.PriceCatalogDetail.Oid)
                                                 .First().TimeValues.Remove(priceCatalogDetailTimeValueViewModel);
                priceCatalogDetailTimeValue.Delete();
                this.grdViewPriceCatalogDetailTimeValues.RefreshData();
            }
        }

        private void grdViewPriceCatalogs_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (Program.Settings.MasterAppInstance != eApplicationInstance.STORE_CONTROLER)
            {
                return;
            }
            PriceCatalogDetailViewModel priceCatalogDetailViewModel = (sender as GridView).GetFocusedRow() as PriceCatalogDetailViewModel;
            if (priceCatalogDetailViewModel == null)
            {
                e.Cancel = true;
                return;
            }

            PriceCatalog priceCatalog = this.Item.Session.GetObjectByKey<PriceCatalog>(priceCatalogDetailViewModel.PriceCatalog);
            if (
                 !(Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER
                     && priceCatalog != null
                     && priceCatalog.IsEditableAtStore != null
                     && priceCatalog.IsEditableAtStore.Oid == Program.Settings.StoreControllerSettings.Store.Oid
                    )
             )
            {
                e.Cancel = true;
            }
        }

        private void grdViewPriceCatalogDetailTimeValues_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView detailView = sender as GridView;
            GridView parentView = detailView.ParentView as GridView;
            PriceCatalogDetailViewModel priceCatalogDetailViewModel = parentView.GetRow(detailView.SourceRowHandle) as PriceCatalogDetailViewModel;

            PriceCatalog priceCatalog = this.Item.Session.GetObjectByKey<PriceCatalog>(priceCatalogDetailViewModel.PriceCatalog);

            if (Program.Settings.MasterAppInstance != eApplicationInstance.STORE_CONTROLER
                || (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER
                     && priceCatalog != null
                     && priceCatalog.IsEditableAtStore != null
                     && priceCatalog.IsEditableAtStore.Oid == Program.Settings.StoreControllerSettings.Store.Oid
                    )
             )
            {

            }
            else
            {
                e.Cancel = true;
            }

        }

        private void gridItemBarcodes_Click(object sender, EventArgs e)
        {

        }
    }
}
