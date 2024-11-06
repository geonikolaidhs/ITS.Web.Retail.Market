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
using DevExpress.Data.Linq;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class LabelItemChangeFilter : BaseFilterControl
    {
        public LabelItemChangeFilter()
        {
            InitializeComponent();
            SecondaryFilterControl = new LabelItemChangeSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;

            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
            CriteriaOperator standardCriteria = CriteriaOperator.And(ownerCriteria, activeCriteria);

            this.TreeListItemCategoryRepository.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(ItemCategory), standardCriteria);
            foreach(TreeListColumn col in this.TreeListItemCategoryRepository.Columns)
            {
                col.Caption = LocalizeString(col.Caption);
            }


            this.SearchFilter = new LabelItemChangeCriteria();
            this.SearchFilter.Set();
        }

        protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {
            GridControl.DataSource = null;
            GridControl.MainView = this.GridView;
            ItemCategory category = null;
            IEnumerable<PriceCatalogDetail> priceCatalogDetails = null;

            if (Program.Settings.StoreControllerSettings.Store.DefaultPriceCatalogPolicy == null)
            {
                XtraMessageBox.Show(string.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, Program.Settings.StoreControllerSettings.Store.Description), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                category = uow.GetObjectByKey<ItemCategory>(((LabelItemChangeCriteria)this.SearchFilter).ItemCategory);

                priceCatalogDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(Program.Settings.StoreControllerSettings.Store, criteria, dbtype: Program.Settings.UnderlyingeDatabaseType);
                //List<PriceCatalogPolicyDetail> policyDetails = Program.Settings.StoreControllerSettings.Store.DefaultPriceCatalogPolicy.PriceCatalogPolicyDetails.ToList();
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(Program.Settings.StoreControllerSettings.Store);
                priceCatalogDetails = priceCatalogDetails
                    .Select(x => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, x.Item))
                    .Where(priceCatalogPolicyResult => priceCatalogPolicyResult != null
                        && priceCatalogPolicyResult.PriceCatalogDetail != null
                        && priceCatalogPolicyResult.PriceCatalogDetail.Value > 0
                        )
                     .Select(priceCatalogPolicyResult => priceCatalogPolicyResult.PriceCatalogDetail);

                if (checkEditWithLeaflet.Checked)
                {
                    DateTime fromTime = ((LabelItemChangeCriteria)this.SearchFilter).FromDate ?? DateTime.Now;
                    DateTime toTime = ((LabelItemChangeCriteria)this.SearchFilter).ToDate ?? DateTime.Now.Date.AddDays(1);
                    CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid),
                                                                 new BinaryOperator("IsActive", true), CriteriaOperator.Or(
                                                                 new BetweenOperator(fromTime, new OperandProperty("StartTime"), new OperandProperty("EndTime")),
                                                                 new BetweenOperator(toTime, new OperandProperty("StartTime"), new OperandProperty("EndTime"))));
                    Leaflet leaflets = uow.FindObject<Leaflet>(crop);


                    if (priceCatalogDetails.Count() > 0 && leaflets != null)
                    {
                        foreach (PriceCatalogDetail pcd in priceCatalogDetails)
                        {
                            LeafletDetail leafletDetail = leaflets.LeafletDetails.FirstOrDefault(x => x.Item == pcd.Item);
                            //skip pricecatalogs and replace with leaflet value
                            if (leafletDetail != null)
                            { 
                                PriceCatalogDetailTimeValue effectiveTimeValueObject = pcd.TimeValues
                                                                                       .Where(x => x.IsActive && x.TimeValueValidFrom <= fromTime.Ticks && x.TimeValueValidUntil >= toTime.Ticks && x.TimeValue > 0)
                                                                                       .OrderBy(x => x.TimeValueRange).FirstOrDefault();
                                if (effectiveTimeValueObject != null)
                                {
                                    leafletDetail.Value = effectiveTimeValueObject.TimeValue;
                                }
                                leaflets.LeafletDetails.Remove(leafletDetail);
                            }
                        }
                    }

                    if (leaflets != null)
                    {
                        PriceCatalogDetail pcd = new PriceCatalogDetail(uow);
                        foreach (LeafletDetail detail in leaflets.LeafletDetails)
                        {
                            pcd = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, detail.Barcode.Code).PriceCatalogDetail;
                            priceCatalogDetails = priceCatalogDetails.Concat(new[] { pcd });
                        }

                    }
                }
            }
            GridControl.DataSource = GetLabesCategoryJoin(priceCatalogDetails, category).ToList();          
        }

        public override int Lines
        {
            get
            {
                return 2;
            }
        }
        
        private void TreeListItemCategory_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                (sender as LookUpEditBase).EditValue = null;
            }
        }

        private IEnumerable<object> GetLabesCategoryJoin(IEnumerable<PriceCatalogDetail> details, ItemCategory category)
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
                return details;
            } 
        }
    }
}
