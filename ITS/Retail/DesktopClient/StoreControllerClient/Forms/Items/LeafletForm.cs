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
    public partial class LeafletForm : XtraLocalizedForm
    {
        protected Leaflet _Leaflet;
        private bool LeafletIsSaved;

        protected Leaflet Leaflet { get { return _Leaflet; } }

        private bool PreviewItem { get; set; }

        //protected BindingList<IPersistableViewModel> LeafletDetailsViewModel { get; set; }

        protected BindingList<LeafletStoreViewModel> LeafletStoresViewModel { get; set; }

        protected LeafletDetailFilter LeafletDetailFilter { get; private set; }

        public LeafletForm(Leaflet Leaflet, bool previewMode)
        {
            this._Leaflet = Leaflet;
            PreviewItem = previewMode;

            InitializeComponent();

            LeafletStoresViewModel = new BindingList<LeafletStoreViewModel>();
            foreach (LeafletStore leafletstore in Leaflet.Stores)
            {
                LeafletStoreViewModel LeafletViewModel = new LeafletStoreViewModel();
                LeafletViewModel.LoadPersistent(leafletstore);
                LeafletStoresViewModel.Add(LeafletViewModel);
            }

            this.LeafletDetailFilter = new LeafletDetailFilter()
            {
                GridView = this.gridViewLeafletDetails,
                PersistentType = typeof(LeafletDetail),
                DefaultSearchMethod = PersistentCriteriaEvaluationBehavior.InTransaction
            };

            (this.LeafletDetailFilter.SearchFilter as LeafletDetailSearchFilter).Leaflet = this.Leaflet.Oid;
            this.LeafletDetailFilter.SearchFilter.Set();

            this.gridControlLeafletDetails.DataSourceChanged += GridControlLeafletDetails_DataSourceChanged;
            
            this.LeafletDetailFilter.GridControl = this.gridControlLeafletDetails;
            this.LeafletDetailFilter.CreateUnitOfWork += LeafletDetailFilter_CreateUnitOfWork;
            this.LeafletDetailFilter.SearchComplete += SearchComplete;
            this.flyoutPanelControlLeafletDetailsFilter.Controls.Add(this.LeafletDetailFilter);
            this.LeafletDetailFilter.Dock = DockStyle.Fill;

            gridControlLeafletDetails.LevelTree.Nodes.Add("TimeValues", gridViewLeafletDetailTimeValue);
            gridControlLeafletDetails.ShowOnlyPredefinedDetails = true;
            this.LeafletIsSaved = false;
            LocaliseApplication();
            if(PreviewItem == false && Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE)
            {
                repositoryItemSearchLookUpEditItem.DataSource = new XPServerCollectionSource(this.Leaflet.Session, typeof(ItemBarcode)
                    
                   // select * from itembarcode where Barcode not in (select Barcode from Leafletdetail where Leaflet = this.pricecatlog )
                   /*new NotOperator(
                       new InOperator("Barcode.Oid", this.Leaflet.LeafletDetails.Select(x => x.Barcode.Oid))
                   )*/);
               
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Item.Code", Caption = Resources.Code, Visible = true });
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Item.Name", Caption = Resources.Name, Visible = true });
                repositoryItemSearchLookUpEditItem.View.Columns.Add(new GridColumn() { FieldName = "Oid", Caption = "Oid", Visible = false });

                repositoryItemSearchLookUpEditItem.ValueMember = "Oid";
               
            }
        }

        XPCollection oldDataSource = null;
        private void GridControlLeafletDetails_DataSourceChanged(object sender, EventArgs e)
        {
            if(oldDataSource !=null)
            {
                oldDataSource.CollectionChanged -= DataSource_CollectionChanged;
            }
            oldDataSource = gridControlLeafletDetails.DataSource as XPCollection;
            if(oldDataSource !=null)
            {
                oldDataSource.CollectionChanged += DataSource_CollectionChanged;
            }
        }

        private void DataSource_CollectionChanged(object sender, XPCollectionChangedEventArgs e)
        {
            if(e.CollectionChangedType == XPCollectionChangedType.BeforeAdd)
            {
                if(e.ChangedObject is LeafletDetail)
                {
                    (e.ChangedObject as LeafletDetail).Leaflet = this.Leaflet;
                }
            }
        }

        private void LocaliseApplication()
        {
            this.flyoutPanelLeafletDetailsFilter.OptionsButtonPanel.Buttons.ForEach(x =>
            {
                if ((x is DevExpress.XtraEditors.ButtonPanel.BaseButton))
                {
                    (x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption = LocalizeString((x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption);
                }
            });
        }
        private void SearchComplete(object sender, EventArgs e)
        {
            this.flyoutPanelLeafletDetailsFilter.HidePopup();
            
        }

        private void LeafletDetailFilter_CreateUnitOfWork(BaseFilterControl sender, SearchEventArgs e)
        {
            e.UnitOfWork = this.Leaflet.Session as UnitOfWork;
        }

        private void LeafletEditForm_Load(object sender, System.EventArgs e)
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
                }
            }

            this.LeafletDetailFilter.Search();
            
        }

        private void InitializeLookupEdits()
        {
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
            CriteriaOperator standardCriteria = CriteriaOperator.And(ownerCriteria, activeCriteria);

            LookUpColumnInfo lookUpColCode = new LookUpColumnInfo("Code", Resources.Code);
            LookUpColumnInfo lookUpColDescription = new LookUpColumnInfo("Description", Resources.Name);

            cmbLeafletStoreStore.Columns.Add(lookUpColCode);
            cmbLeafletStoreStore.Columns.Add(lookUpColDescription);

            XPCollection<Store> storesCollection = new XPCollection<Store>(Program.Settings.ReadOnlyUnitOfWork, standardCriteria);

            cmbLeafletStoreStoreName.DataSource = cmbLeafletStoreStore.DataSource = storesCollection;

        }

        private void InitializeBindings()
        {
            this.txtCode.DataBindings.Add("EditValue", this.Leaflet, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDescription.DataBindings.Add("EditValue", this.Leaflet, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dateEditStartDate.DataBindings.Add("EditValue", this.Leaflet, "StartDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dateEditEndDate.DataBindings.Add("EditValue", this.Leaflet, "EndDate", true, DataSourceUpdateMode.OnPropertyChanged);

            this.gridControlLeafletStores.DataSource = this.LeafletStoresViewModel;
            this.imageEditLeafletImage.Image = this.Leaflet.Image;
        }

        private void btnShowFilters_Clicked(object sender, System.EventArgs e)
        {

            if (flyoutPanelLeafletDetailsFilter.IsPopupOpen)
            {
                this.flyoutPanelLeafletDetailsFilter.HidePopup();
            }
            else //if (panelFilter.Controls.Cast<Control>().FirstOrDefault() is BaseFilterControl)
            {
                this.flyoutPanelLeafletDetailsFilter.ShowPopup();
                this.flyoutPanelLeafletDetailsFilter.Focus();
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
                this.Leaflet.Session.RollbackTransaction();
                return true;
            }
            return false;
        }

        private void flyoutPanelLeafletDetailsFilter_ButtonClick(object sender, FlyoutPanelButtonClickEventArgs e)
        {
            switch (e.Button.Tag.ToString())
            {
                case "Search":
                    this.LeafletDetailFilter.Search();
                    break;
                case "Clear":
                    this.LeafletDetailFilter.SearchFilter.Reset();
                    break;
            }
        }

      
        private void LeafletEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.LeafletIsSaved)
            {
                bool cancelUserActions = CancelUserChanges();
                if (!cancelUserActions)
                {
                    e.Cancel = true;
                }
            }
        }

        //private void gridViewLeafletDetails_ShowingEditor(object sender, CancelEventArgs e)
        //{
        //    if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
        //    {
        //        Store LeafletIsEditableAtStore = (this.gridViewLeafletDetails.GetFocusedRow() as LeafletDetail).Leaflet.IsEditableAtStore;
        //        if (LeafletIsEditableAtStore == null || LeafletIsEditableAtStore.Oid != Program.Settings.StoreControllerSettings.Store.Oid)
        //        {
        //            e.Cancel = true;
        //        }
        //    }
        //}

        
    }
}
