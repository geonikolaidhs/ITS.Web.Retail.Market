using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.DesktopClient.StoreControllerClient.Controls;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System;
using ITS.Retail.Common;
using ITS.Retail.Platform;
using System.Collections.Generic;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using System.Data;
using ITS.Retail.Platform.Kernel;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.Items
{
    public partial class PriceCatalogEditForm : XtraLocalizedForm
    {
        protected PriceCatalog _PriceCatalog;
        private bool priceCatalogIsSaved;

        protected PriceCatalog PriceCatalog { get { return _PriceCatalog; } }

        private bool PreviewItem { get; set; }

        //protected BindingList<IPersistableViewModel> PriceCatalogDetailsViewModel { get; set; }

        protected BindingList<PriceCatalogViewModel> PriceSubCatalogsViewModel { get; set; }

        protected PriceCatalogDetailFilter PriceCatalogDetailFilter { get; private set; }

        public PriceCatalogEditForm(PriceCatalog priceCatalog, bool previewMode)
        {
            this._PriceCatalog = priceCatalog;
            PreviewItem = previewMode;

            InitializeComponent();

            if (PreviewItem)
            {
                this.layoutControlItemSaveButton.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            PriceSubCatalogsViewModel = new BindingList<PriceCatalogViewModel>();
            foreach (PriceCatalog priceSubCatalog in priceCatalog.PriceCatalogs)
            {
                PriceCatalogViewModel priceCatalogViewModel = new PriceCatalogViewModel();
                priceCatalogViewModel.LoadPersistent(priceSubCatalog);
                PriceSubCatalogsViewModel.Add(priceCatalogViewModel);
            }

            this.PriceCatalogDetailFilter = new PriceCatalogDetailFilter()
            {
                GridView = this.gridViewPriceCatalogDetails,
                PersistentType = typeof(PriceCatalogDetail),
                DefaultSearchMethod = PersistentCriteriaEvaluationBehavior.InTransaction
            };

            (this.PriceCatalogDetailFilter.SearchFilter as PriceCatalogDetailSearchFilter).PriceCatalog = this.PriceCatalog.Oid;
            this.PriceCatalogDetailFilter.SearchFilter.Set();


            this.gridControlPriceCatalogDetails.DataSourceChanged += GridControlPriceCatalogDetails_DataSourceChanged;


            this.PriceCatalogDetailFilter.GridControl = this.gridControlPriceCatalogDetails;
            this.PriceCatalogDetailFilter.CreateUnitOfWork += PriceCatalogDetailFilter_CreateUnitOfWork;
            this.PriceCatalogDetailFilter.SearchComplete += SearchComplete;
            this.flyoutPanelControlPriceCatalogDetailsFilter.Controls.Add(this.PriceCatalogDetailFilter);
            this.PriceCatalogDetailFilter.Dock = DockStyle.Fill;

            gridControlPriceCatalogDetails.LevelTree.Nodes.Add("TimeValues", gridViewPriceCatalogDetailTimeValue);
            gridControlPriceCatalogDetails.ShowOnlyPredefinedDetails = true;
            this.priceCatalogIsSaved = false;
            LocaliseApplication();
            if(PreviewItem == false && Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE)
            {
                repositoryItemSearchLookUpEditItem.DataSource = new XPServerCollectionSource(this.PriceCatalog.Session, typeof(ItemBarcode)
                    
                   // select * from itembarcode where Barcode not in (select Barcode from pricecatalogdetail where Pricecatalog = this.pricecatlog )
                   /*new NotOperator(
                       new InOperator("Barcode.Oid", this.PriceCatalog.PriceCatalogDetails.Select(x => x.Barcode.Oid))
                   )*/);
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Barcode.Code", Caption = Resources.Barcode, Visible = true });
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Item.Code", Caption = Resources.Code, Visible = true });
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Item.Name", Caption = Resources.Name, Visible = true });
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Oid", Caption = "Oid", Visible = false });

                repositoryItemSearchLookUpEditItem.ValueMember = "Oid";
                
                repositoryItemSearchLookUpEditItem.DisplayMember = "Barcode.Code";
                grdColBarcode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
        }

        XPCollection oldDataSource = null;
        private void GridControlPriceCatalogDetails_DataSourceChanged(object sender, EventArgs e)
        {
            if(oldDataSource !=null)
            {
                oldDataSource.CollectionChanged -= DataSource_CollectionChanged;
            }
            oldDataSource = gridControlPriceCatalogDetails.DataSource as XPCollection;
            if(oldDataSource !=null)
            {
                oldDataSource.CollectionChanged += DataSource_CollectionChanged;
            }
        }

        private void DataSource_CollectionChanged(object sender, XPCollectionChangedEventArgs e)
        {
            if(e.CollectionChangedType == XPCollectionChangedType.BeforeAdd)
            {
                if(e.ChangedObject is PriceCatalogDetail)
                {
                    (e.ChangedObject as PriceCatalogDetail).PriceCatalog = this.PriceCatalog;
                }
            }
        }

        private void LocaliseApplication()
        {
            this.flyoutPanelPriceCatalogDetailsFilter.OptionsButtonPanel.Buttons.ForEach(x =>
            {
                if ((x is DevExpress.XtraEditors.ButtonPanel.BaseButton))
                {
                    (x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption = LocalizeString((x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption);
                }
            });
        }
        private void SearchComplete(object sender, EventArgs e)
        {
            this.flyoutPanelPriceCatalogDetailsFilter.HidePopup();
            
        }

        private void PriceCatalogDetailFilter_CreateUnitOfWork(BaseFilterControl sender, SearchEventArgs e)
        {
            e.UnitOfWork = this.PriceCatalog.Session as UnitOfWork;
        }

        private void PriceCatalogEditForm_Load(object sender, System.EventArgs e)
        {
            InitializeLookupEdits();
            InitializeBindings();

            if (PreviewItem || Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                foreach (BaseEdit editControl in this.EnumerateComponents().Where(x => x is BaseEdit).Cast<BaseEdit>())
                {
                    editControl.ReadOnly = true;
                    if (editControl is ButtonEdit)
                    {
                        EditorButtonCollection editbuttons = (editControl as ButtonEdit).Properties.Buttons;
                        if (editbuttons.Count > 0)
                        {
                            foreach (EditorButton button in editbuttons)
                            {
                                button.Enabled = button.Visible = false;
                            }
                        }
                    }

                    if (editControl is DateEdit)
                    {
                        (editControl as DateEdit).Properties.AllowDropDownWhenReadOnly = DefaultBoolean.False;
                    }
                }

                if (PreviewItem)
                {
                    foreach (GridView gridView in this.EnumerateComponents().Where(x => x is GridView).Cast<GridView>())
                    {
                        gridView.OptionsBehavior.Editable = false;
                        gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                        foreach (GridColumn gridColumn in gridView.Columns)
                        {
                            if (gridColumn.ColumnEdit != null)
                            {
                                if (gridColumn.ColumnEdit.EditorTypeName == "ButtonEdit")
                                {
                                    gridColumn.Visible = false;
                                }
                                else
                                {
                                    gridColumn.ColumnEdit.ReadOnly = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    grdColItem.OptionsColumn.AllowEdit = false;
                    grdColBarcode.OptionsColumn.AllowEdit = false;
                }
            }
            else
            {
                this.PriceCatalogDetailFilter.Search();
            }
        }

        private void InitializeLookupEdits()
        {
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
            CriteriaOperator standardCriteria = CriteriaOperator.And(ownerCriteria, activeCriteria);

            this.lueParentCatalog.Properties.DataSource = new XPServerCollectionSource(this.PriceCatalog.Session, typeof(PriceCatalog),
                                                            CriteriaOperator.And(standardCriteria,
                                                                                new BinaryOperator("Oid", this.PriceCatalog.Oid, BinaryOperatorType.NotEqual)));
            this.lueParentCatalog.Properties.ValueMember = "Oid";
            this.lueParentCatalog.Properties.DisplayMember = "Description";
            this.lueParentCatalog.Properties.Columns.Clear();
            this.lueParentCatalog.Properties.Columns.Add(new LookUpColumnInfo("Code"));
            this.lueParentCatalog.Properties.Columns.Add(new LookUpColumnInfo("Description"));
        }

        private void InitializeBindings()
        {
            this.txtCode.DataBindings.Add("EditValue", this.PriceCatalog, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDescription.DataBindings.Add("EditValue", this.PriceCatalog, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueParentCatalog.DataBindings.Add("EditValue", this.PriceCatalog, "ParentCatalog!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dateEditStartDate.DataBindings.Add("EditValue", this.PriceCatalog, "StartDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dateEditEndDate.DataBindings.Add("EditValue", this.PriceCatalog, "EndDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkBoxSupportLoyalty.DataBindings.Add("EditValue", this.PriceCatalog, "SupportLoyalty", true, DataSourceUpdateMode.OnPropertyChanged);
            this.checkEditIgnoreZeroPrices.DataBindings.Add("EditValue", this.PriceCatalog, "IgnoreZeroPrices", true, DataSourceUpdateMode.OnPropertyChanged);

            this.gridControlSubCatalogs.DataSource = this.PriceSubCatalogsViewModel;
            this.gridControlPriceCatalogStores.DataSource = this.PriceCatalog.StorePriceLists.Select(storePriceList => storePriceList.Store);
        }

        private void btnShowFilters_Clicked(object sender, System.EventArgs e)
        {

            if (flyoutPanelPriceCatalogDetailsFilter.IsPopupOpen)
            {
                this.flyoutPanelPriceCatalogDetailsFilter.HidePopup();
            }
            else //if (panelFilter.Controls.Cast<Control>().FirstOrDefault() is BaseFilterControl)
            {
                this.flyoutPanelPriceCatalogDetailsFilter.ShowPopup();
                this.flyoutPanelPriceCatalogDetailsFilter.Focus();
            }
        }

        private void simpleButtonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private bool CancelUserChanges()
        {
            if (this.PreviewItem || XtraMessageBox.Show(Resources.YouHaveUnsavedChangesAreYouSureYouWantToContinue, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.PriceCatalog.Session.RollbackTransaction();
                return true;
            }
            return false;
        }

        private bool SaveItem()
        {
            try
            {
                if (Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE)
                {
                    foreach (var subPriceCatalogViewModel in this.PriceSubCatalogsViewModel)
                    {
                        if (subPriceCatalogViewModel.IsDeleted)
                        {
                            PriceCatalog subCatalog = this.PriceCatalog.PriceCatalogs.FirstOrDefault(priceCatalog => priceCatalog.Oid == subPriceCatalogViewModel.Oid);
                            if (subCatalog != null)
                            {
                                this.PriceCatalog.PriceCatalogs.Remove(subCatalog);
                            }
                        }
                        else if (subPriceCatalogViewModel.IsNew)
                        {
                            PriceCatalog newSubCatalog = this.PriceCatalog.Session.GetObjectByKey<PriceCatalog>(subPriceCatalogViewModel.Oid);
                            if (newSubCatalog != null)
                            {
                                this.PriceCatalog.PriceCatalogs.Add(newSubCatalog);
                            }
                        }
                    }
                    //#region Validate Time Values
                    //PlatformPriceCatalogDetailService platformPriceCatalogDetailService = new PlatformPriceCatalogDetailService();
                    //bool timeValuesResultValid = true;
                    //string timeValuesMessage = "";
                    //foreach (PriceCatalogDetail currentPriceCatalogDetail in this.PriceCatalog.PriceCatalogDetails)
                    //{
                    //    ValidationPriceCatalogDetailTimeValuesResult validationResult = platformPriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(currentPriceCatalogDetail.TimeValues);
                    //    if (validationResult != null)
                    //    {
                    //        timeValuesResultValid = false;
                    //        timeValuesMessage = (validationResult.PartialOverlap ? Resources.PARTIALLY_OVERLAPPING_TIME_VALUES : Resources.Error)
                    //                                                + Environment.NewLine + Resources.FromDate + ": " + validationResult.From.ToString()
                    //                                                + Environment.NewLine + Resources.ToDate + ": " + validationResult.To.ToString();
                    //    }
                    //}
                    //if (!timeValuesResultValid)
                    //{
                    //    XtraMessageBox.Show(timeValuesMessage, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    return false;
                    //}
                    //#endregion
                    this.PriceCatalog.Save();
                    XpoHelper.CommitChanges(this.PriceCatalog.Session as UnitOfWork);
                }
                else if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    using (ITSStoreControllerDesktopServiceClient storeControllerDesktopServiceClient = Program.Settings.ITSStoreControllerDesktopService)
                    {
                        ApplicationStatus applicationStatus = storeControllerDesktopServiceClient.GetApplicationStatus();
                        if (applicationStatus != ApplicationStatus.ONLINE)
                        {
                            XtraMessageBox.Show(Resources.ApplicationMustBeConnectedToHeadQuartersToEditPrices, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }

                    using (ITSStoreControllerDesktopPOSUpdateService.POSUpdateService pOSUpdateService = new ITSStoreControllerDesktopPOSUpdateService.POSUpdateService())
                    {
                        pOSUpdateService.Url = Program.Settings.MasterURLService;

                        string errorMessage = string.Empty;

                        foreach (PriceCatalogDetail storeControllerPriceCatalogDetail in this.PriceCatalog.PriceCatalogDetails.Where(priceDetail => priceDetail.HasChangedOrHasTimeValueChanges))
                        {
                            string jsonItem = storeControllerPriceCatalogDetail.JsonWithDetails(PlatformConstants.JSON_SERIALIZER_SETTINGS, false);
                            if (pOSUpdateService.InsertOrUpdateRecord(Program.Settings.StoreControllerSettings.Oid, "PriceCatalogDetail", jsonItem, out errorMessage)
                                == false
                               )
                            {
                                XtraMessageBox.Show(errorMessage, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                        }
                        #region manage deletion of item details
                        List<object> itemDetails = this.PriceCatalog.Session.GetObjectsToSave().Cast<object>().Where(priceCatalogDetailObject => priceCatalogDetailObject is BaseObj).ToList();
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
                        this.PriceCatalog.Save();
                        XpoHelper.CommitChanges(this.PriceCatalog.Session as UnitOfWork);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Logger.Error(ex, "Save Price Catalog Exception");
                return false;
            }
        }

        private void simpleButtonSave_Click(object sender, System.EventArgs e)
        {
            if (SaveItem())
            {
                this.priceCatalogIsSaved = true;
                this.Close();
            }
        }

        private void flyoutPanelPriceCatalogDetailsFilter_ButtonClick(object sender, FlyoutPanelButtonClickEventArgs e)
        {
            switch (e.Button.Tag.ToString())
            {
                case "Search":
                    this.PriceCatalogDetailFilter.Search();
                    break;
                case "Clear":
                    this.PriceCatalogDetailFilter.SearchFilter.Reset();
                    break;
            }
        }

        private void repositoryItemButtonEditDeletePriceCatalogDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            PriceCatalogDetail priceCatalogDetail = this.gridViewPriceCatalogDetails.GetFocusedRow() as PriceCatalogDetail;
            if (priceCatalogDetail != null)
            {
                priceCatalogDetail.Delete();
            }
            this.gridViewPriceCatalogDetails.RefreshData();
        }

        private void repositoryItemButtonEditDeletePriceCatalogDetailTimeValue_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            PriceCatalogDetailTimeValue priceCatalogDetailTimeValue = (this.gridControlPriceCatalogDetails.FocusedView as GridView).GetFocusedRow() as PriceCatalogDetailTimeValue;
            if (priceCatalogDetailTimeValue != null)
            {
                priceCatalogDetailTimeValue.Delete();
                this.gridViewPriceCatalogDetailTimeValue.RefreshData();
            }
        }

        private void PriceCatalogEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.priceCatalogIsSaved)
            {
                bool cancelUserActions = CancelUserChanges();
                if (!cancelUserActions)
                {
                    e.Cancel = true;
                }
            }
        }

        private void gridViewPriceCatalogDetails_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                Store priceCatalogIsEditableAtStore = (this.gridViewPriceCatalogDetails.GetFocusedRow() as PriceCatalogDetail).PriceCatalog.IsEditableAtStore;
                if (priceCatalogIsEditableAtStore == null || priceCatalogIsEditableAtStore.Oid != Program.Settings.StoreControllerSettings.Store.Oid)
                {
                    e.Cancel = true;
                }
            }
        }

        private void gridViewPriceCatalogDetails_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if(e.Column == grdColBarcode && e.Value is Guid)
            {
                ItemBarcode itemBarcode = this.PriceCatalog.Session.GetObjectByKey<ItemBarcode>((Guid)e.Value);
                e.DisplayText = string.Format("{0} - {1} - {2}", itemBarcode.Barcode.Code, itemBarcode.Item.Code, itemBarcode.Item.Name);
            }
        }
    }
}
