using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraTreeList.Columns;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using DevExpress.XtraTreeList;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Model.SupportingClasses;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class CashierItemFilter : BaseFilterControl
    {
        private UnitOfWork unitOfWork;
        private POSDevice _DeviceSettings;
        public CashierItemFilter(POSDevice deviceSettings)
        {
            InitializeComponent();
            if ( deviceSettings == null )
            {
                return;
            }
            _DeviceSettings = deviceSettings;
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
            BinaryOperator itemCategoryCriteria = new BinaryOperator("Oid", deviceSettings.ItemCategory.Oid);
            CriteriaOperator standardCriteria = CriteriaOperator.And(ownerCriteria, activeCriteria, itemCategoryCriteria);

            //TopReturnedObjects 
            this.TreeListItemCategoryRepository.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(ItemCategory), standardCriteria);

            foreach (TreeListColumn col in this.TreeListItemCategoryRepository.Columns)
            {
                col.Caption = LocalizeString(col.Caption);
                
            }
            if (TreeListItemCategoryRepository.FocusedNode == null & TreeListItemCategoryRepository.Nodes.Count > 0)
            {
                TreeListItemCategoryRepository.FocusedNode = TreeListItemCategoryRepository.Nodes[0];
            }

            this.CreateUnitOfWork += CashierItemFilterCreateUnitOFWork;
            this.SearchFilter = new CashierSearchCriteria(deviceSettings);
            this.SearchFilter.Set();
            
        }

        private void CashierItemFilterCreateUnitOFWork(BaseFilterControl sender, SearchEventArgs e)
        {
            if (unitOfWork == null)
            {
                unitOfWork = XpoHelper.GetNewUnitOfWork();
            }
            e.UnitOfWork = unitOfWork;
        }

        protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {
            GridControl.DataSource = null;
            GridControl.MainView = this.GridView;
            ItemCategory category = null;
            IEnumerable<PriceCatalogDetail> priceCatalogDetails = null;

            if (Program.Settings.StoreControllerSettings.Store.DefaultPriceCatalogPolicy == null)
            {
                XtraMessageBox.Show(string.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore,Program.Settings.StoreControllerSettings.Store.Description), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                category = uow.GetObjectByKey<ItemCategory>(((CashierSearchCriteria)this.SearchFilter).ItemCategory);

                priceCatalogDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(Program.Settings.StoreControllerSettings.Store, criteria, dbtype:Program.Settings.UnderlyingeDatabaseType);
                
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(Program.Settings.StoreControllerSettings.Store);
                priceCatalogDetails = priceCatalogDetails
                    .Select(x => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, x.Item))
                    .Where(priceCatalogPolicyResult => priceCatalogPolicyResult != null
                        && priceCatalogPolicyResult.PriceCatalogDetail !=null
                        && priceCatalogPolicyResult.PriceCatalogDetail.Value > 0
                        )
                     .Select(priceCatalogPolicyResult=> priceCatalogPolicyResult.PriceCatalogDetail);
            }
            int limitLines = _DeviceSettings.MaxItemsAdd;
            IEnumerable<PriceCatalogDetail> priceCatalogDetailsList = GetJoinedFilteredList(priceCatalogDetails, category).ToList().Take(limitLines);

            List<MapVatFactor> deviceMapVatFactors = _DeviceSettings.MapVatFactors.ToList();
            List<ItemCashRegister> dictItemsToAdd = new List<ItemCashRegister>();
            //List<ItemCashRegister> itemCashRegister = new List<ItemCashRegister>();
            Customer DefaultCustomer = Program.Settings.StoreControllerSettings.DefaultCustomer;
            int loop = 0;
            foreach (var currentItem in priceCatalogDetailsList)
            {
                loop++;
                if ( currentItem.RetailValue < 0 )
                {
                    continue;
                }

                ItemCashRegister cashRegister = new ItemCashRegister()
                {
                    Item = currentItem.Item,
                    RetailPriceValue = currentItem.RetailValue,
                    CashRegisterBarcode = ItemHelper.GetBarcodeCodeForCashRegister(currentItem.Item, _DeviceSettings, Program.Settings.StoreControllerSettings.Owner),
                    //CashRegisterQTY = 0, //TODO if we want to use item stock
                    eSItemtatus = eCashRegisterItemStatus.WAITING
                };                

                VatFactor vatFactor = cashRegister.GetVatFactor(currentItem.Item.VatCategory.Oid, DefaultCustomer.VatLevel.Oid);
                MapVatFactor mapVATFactor = deviceMapVatFactors.FirstOrDefault(mapVatFactor => mapVatFactor.VatFactor.Oid == vatFactor.Oid);
                if( mapVATFactor == null)
                {
                    continue;//TODO display error message for this item
                }
                int intVatDeviceLevel = 0;
                Int32.TryParse(mapVATFactor.DeviceVatLevel, out intVatDeviceLevel);
                cashRegister.CashRegisterVatLevel = intVatDeviceLevel;
                cashRegister.CashRegisterPoints = cashRegister.GetPointsOfItem(currentItem.Item, Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings);
                //itemCashRegister.Add(cashRegister);
                dictItemsToAdd.Add(cashRegister);
            }
            GridControl.DataSource = dictItemsToAdd;
        }

        public override int Lines
        {
            get
            {
                return 2;
            }
        }

        private void checkEditWithValueFilter_CheckedChanged(object sender, EventArgs e)
        {
            this.dtTimeValueFromTime.Enabled = this.dtTimeValueToDate.Enabled = 
                this.dtTimeValueToTime.Enabled = this.dateEditTimeValueFromDate.Enabled =
                checkEditWithValueFilter.Checked;
        }

        private void TreeListItemCategory_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            
            if (e.Button.Index == 1)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private IEnumerable<PriceCatalogDetail> GetJoinedFilteredList(IEnumerable<PriceCatalogDetail> details, ItemCategory category)
        {
            if(category != null)
            {
                return from pcd in details
                        join iat in category.GetAllNodeTreeItems<ItemAnalyticTree>() on pcd.Item.Oid equals iat.Object.Oid
                       where iat != null
                        select pcd;
            }
            else
            {
                return details                    ;
            } 
        }
    }
}
