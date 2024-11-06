using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraGrid.Columns;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class PriceCheckFilter : BaseFilterControl
    {
        public PriceCheckFilter()
        {
            InitializeComponent();
            Expandable = false;
            SecondaryFilterControl = new PriceCheckSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;

            CreateSearchFilter();
        }
        PriceCheckSearchFilter priceCatalogSearchFilter;

        protected virtual void CreateSearchFilter()
        {
            if (!DesignMode)
            {
                this.SearchFilter = priceCatalogSearchFilter =
                    new PriceCheckSearchFilter()
                    {
                        Store = Program.Settings.StoreControllerSettings.Store.Oid,
                        Customer = Program.Settings.StoreControllerSettings.DefaultCustomer.Oid
                    };
                this.SearchFilter.Set();
            }
        }
        

        protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {

            List<PriceSearchTraceStep> traces = new List<PriceSearchTraceStep>();
            Customer selectedCustomer = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<Customer>(this.priceCatalogSearchFilter.Customer);
            Store selectedStore = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<Store>(this.priceCatalogSearchFilter.Store);

            string errorMessage = string.Empty;
            if (selectedCustomer == null)
            {
                errorMessage += Resources.PleaseSelectACustomer + Environment.NewLine;
            }
            if (this.priceCatalogSearchFilter.ItemBarcode == null)
            {
                errorMessage += Resources.PleaseSelectAnItem + Environment.NewLine;
            }

            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(selectedStore,
                                                                                                                   this.priceCatalogSearchFilter.ItemBarcode,
                                                                                                                   selectedCustomer, 
                                                                                                                   traces: traces
                                                                                                                   );
            PriceCatalogDetail priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

            GridControl.MainView = this.GridView;
            if (priceCatalogDetail != null)
            {
                List<PriceCatalogDetail> priceCatalogDetailAsList = new List<PriceCatalogDetail>();
                priceCatalogDetailAsList.Add(priceCatalogDetail);
                IEnumerable<object> result = priceCatalogDetailAsList.Select(x => new
                {
                    Store = selectedStore.Name,
                    Code = priceCatalogDetail.Item.Code,
                    Name = priceCatalogDetail.Item.Name,
                    StorePriceCatalogPolicy = selectedStore.DefaultPriceCatalogPolicy.Description,
                    Customer = selectedCustomer.FullDescription,
                    CustomerPriceCatalogPolicy = selectedCustomer.PriceCatalogPolicy != null ? selectedCustomer.PriceCatalogPolicy.Description : string.Empty,
                    Price = x.Value,
                    VatIncluded = x.VATIncluded,
                    PriceCatalog = x.PriceCatalog.Description,
                    Traces = traces.
                        Select(trc => new
                        {
                            Number = trc.Number,
                            PriceCatalogDescription = trc.PriceCatalogDescription,
                            SearchMethod = trc.SearchMethod.ToLocalizedString()
                        }).ToList()
                }).ToList();

                GridControl.DataSource = result;
            }
            else
            {
                List<object> result = new List<object>();
                result.Add(new
                {
                    Store = string.Empty,
                    StorePriceCatalogPolicy = string.Empty,
                    Customer = string.Empty,
                    CustomerPriceCatalogPolicy = string.Empty,
                    Code = errorMessage,
                    Name = string.Empty,
                    Price = null as decimal?,
                    VatIncluded = null as bool?,
                    PriceCatalog = string.Empty,
                    Traces = traces.
                        Select(trc => new
                        {
                            Number = trc.Number,
                            PriceCatalogDescription = trc.PriceCatalogDescription,
                            SearchMethod = trc.SearchMethod.ToLocalizedString()
                        }).ToList()
                });
                GridControl.DataSource = result;
            }
            GridControl.Invalidate();
            GridControl.Update();

        }
    }
}
