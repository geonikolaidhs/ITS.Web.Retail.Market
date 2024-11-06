using System;
using System.Collections.Generic;
using System.ComponentModel;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.Helpers;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.XtraEditors;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class DocumentFilter : BaseFilterControl
    {
        public DocumentSearchFilter DocumentSearchFilter
        {
            get
            {
                return SearchFilter as DocumentSearchFilter;
            }
        }

        private DocumentFilter()
        {
            InitializeComponent();
            this.SearchFilter = new DocumentSearchFilter();
        }

        private bool isProforma;


        public DocumentFilter(eDivision division, bool proforma, List<Guid> proformaTypes = null)
        {
            InitializeComponent();
            isProforma = proforma;
            if (proforma)
            {
                this.SearchFilter = new DocumentSearchFilter()
                {
                    LoggedInUser = Program.Settings.CurrentUser.Oid,
                    Store = Program.Settings.StoreControllerSettings.Store.Oid,
                    Division = eDivision.Sales,
                    TransformationStatus = eTransformationStatus.NOT_TRANSFORMED,
                    Proforma = "Proforma",
                    ProformaTypes = proformaTypes
                };
            }
            else
            {
                this.SearchFilter = new DocumentSearchFilter()
                {
                    LoggedInUser = Program.Settings.CurrentUser.Oid,
                    Store = Program.Settings.StoreControllerSettings.Store.Oid,
                    Division = division
                };
            }
            SetLookups();
            this.SearchFilter.Set();
            this.SearchFilter.PropertyChanged += SearchFilter_PropertyChanged;
        }

        private void SearchFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DocumentType")
            {
                if(this.DocumentSearchFilter.DocumentType.HasValue)
                {
                    CriteriaOperator documentSeriesCriteria = CriteriaOperator.And(
                        new BinaryOperator("Store", Program.Settings.StoreControllerSettings.Store.Oid),
                                                                new BinaryOperator("IsCancelingSeries", false),
                                                                new ContainsOperator("StoreDocumentSeriesTypes",
                                                                    CriteriaOperator.And(
                                                                   new BinaryOperator("DocumentType.Oid", this.DocumentSearchFilter.DocumentType.Value),
                                                                    new BinaryOperator("DocumentType.Division.Section", this.DocumentSearchFilter.Division)
                                                                    ))
                                                              );
                    lueDocumentSeries.Properties.DataSource = new XPCollection<DocumentSeries>(Program.Settings.ReadOnlyUnitOfWork, documentSeriesCriteria);
                    lueDocumentSeries.Properties.ValueMember = "Oid";
                    lueDocumentSeries.Properties.Columns.Clear();
                    lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
                    lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
                }
            }
        }

        private void SetLookups()
        {
            eModule officeManagerModule = eModule.STORECONTROLLER;
            switch (Program.Settings.MasterAppInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    officeManagerModule = eModule.DUAL;
                    break;
                case eApplicationInstance.RETAIL:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
                default:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
            }

            lueDocumentType.Properties.DataSource = StoreHelper.StoreDocumentTypes(Program.Settings.StoreControllerSettings.Store, DocumentSearchFilter.Division, officeManagerModule,false,false,true);
            lueDocumentType.Properties.Columns.Clear();
            lueDocumentType.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueDocumentType.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDocumentType.Properties.ValueMember = "Oid";

            lueTransformationStatus.Properties.DataSource = Platform.Enumerations.ExtensionMethods.ToDictionary<eTransformationStatus>();
            lueTransformationStatus.Properties.ValueMember = "Key";
            lueTransformationStatus.Properties.DisplayMember = "Value";
            lueTransformationStatus.Properties.Columns.Clear();
            lueTransformationStatus.Properties.Columns.Add(new LookUpColumnInfo("Value", Resources.HasBeenTransformed));

            CriteriaOperator statusCriteria = new BinaryOperator("Owner", Program.Settings.StoreControllerSettings.Owner.Oid);
            lueDocumentStatus.Properties.DataSource = new XPCollection<DocumentStatus>(Program.Settings.ReadOnlyUnitOfWork, statusCriteria);
            lueDocumentStatus.Properties.ValueMember = "Oid";
            lueDocumentStatus.Properties.Columns.Clear();
            lueDocumentStatus.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueDocumentStatus.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));

            lueCreatedBy.Properties.DataSource = new XPCollection<User>(Program.Settings.ReadOnlyUnitOfWork);
            lueCreatedBy.Properties.ValueMember = "Oid";
            lueCreatedBy.Properties.Columns.Clear();
            lueCreatedBy.Properties.Columns.Add(new LookUpColumnInfo("UserName", Resources.UserName));
            lueCreatedBy.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));


            lueCreatedByDevice.Properties.DataSource = new XPCollection<Model.POS>(Program.Settings.ReadOnlyUnitOfWork);
            lueCreatedByDevice.Properties.ValueMember = "Oid";
            lueCreatedByDevice.Properties.DisplayMember = "Name";

            searchLookUpEditDocumentCustomerFilter.EditValue = null;
            searchLookUpEditDocumentSupplierFilter.EditValue = null;

            searchLookUpEditDocumentCustomerFilter.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(Customer),
                new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
            searchLookUpEditDocumentCustomerFilter.Properties.ValueMember = "Oid";            

            searchLookUpEditDocumentSupplierFilter.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(SupplierNew),
                new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
            searchLookUpEditDocumentSupplierFilter.Properties.ValueMember = "Oid";
        }

        public override int Lines
        {
            get
            {
                return 4;
            }
        }

        private void lueTransformationStatus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void txtToDocumentNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }



        private void txtFromDocumentNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueDocumentStatus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueCreatedBy_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueCreatedByDevice_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueDocumentType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueDocumentSeries_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void txtFromDocumentNumber_EditValueChanged(object sender, EventArgs e)
        {
            if (txtFromDocumentNumber.EditValue is string && string.IsNullOrWhiteSpace((string)txtFromDocumentNumber.EditValue))
            {
                txtFromDocumentNumber.EditValue = null;
            }
        }

        private void txtToDocumentNumber_EditValueChanged(object sender, EventArgs e)
        {
            if (txtToDocumentNumber.EditValue is string && string.IsNullOrWhiteSpace((string)txtToDocumentNumber.EditValue))
            {
                txtToDocumentNumber.EditValue = null;
            }
        }
    }
}
