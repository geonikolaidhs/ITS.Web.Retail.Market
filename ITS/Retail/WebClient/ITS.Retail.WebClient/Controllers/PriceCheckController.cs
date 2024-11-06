using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.Linq.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    [RoleAuthorize]
    public class PriceCheckController : BaseController
    {
        public ActionResult Index()
        {
            if (EffectiveOwner == null)
            {
                Session["Error"] = ResourcesLib.Resources.SelectCompany;
                return new RedirectResult("~/Home/Index");
            }
            RetrieveInitialData();
            return View();
        }

        public ActionResult CheckPrice()
        {
            RetrieveInitialData();
            return PartialView(null);
        }

        private void RetrieveInitialData()
        {
            XPCollection<Store> companyStores = GetList<Store>(XpoSession);
            ViewBag.Stores = companyStores.Select(store => new { value = store.Oid.ToString(), label = store.Description });
            ViewBag.SelectedStore = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL
                                        ? companyStores.First().Oid.ToString()
                                        : (Session["currentStore"] as StoreViewModel).Oid.ToString();

            Store selectedStore = CurrentStore == null ? null : XpoSession.GetObjectByKey<Store>(CurrentStore.Oid);
            Customer DefaultCustomer = GetDefaultCustomer(selectedStore);
            ViewBag.DefaultCustomerLabel = DefaultCustomer == null ? string.Empty : DefaultCustomer.FullDescription;
            ViewBag.DefaultCustomerValue = DefaultCustomer == null ? Guid.Empty : DefaultCustomer.Oid;
        }

        private static Customer GetDefaultCustomer(Store selectedStore)
        {
            return MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL
                                            ? (selectedStore == null || selectedStore.StoreControllerSettings == null
                                                    ? null
                                                    : selectedStore.StoreControllerSettings.DefaultCustomer
                                              )
                                            : StoreControllerAppiSettings.DefaultCustomer;
        }

        public JsonResult SearchByType(string typeStr, string searchFields, string label, string searchTxt = "")
        {
            try
            {

                CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                Type type = typeof(BasicObj).Assembly.GetTypes().First(objtype => objtype.Name == typeStr);

                List<string> searchProperties = searchFields.Split(',').ToList();

                List<CriteriaOperator> criteriaList = new List<CriteriaOperator>();

                List<string> searchTextParts = searchTxt.Split(' ').ToList();

                searchProperties.ForEach(searchProperty =>
                {
                    List<CriteriaOperator> innerCriteriaList = new List<CriteriaOperator>();
                    searchTextParts.ForEach(textPart =>
                    {
                        innerCriteriaList.Add(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(searchProperty), new OperandValue(textPart)));
                    });
                    criteriaList.Add(CriteriaOperator.And(innerCriteriaList));                    
                });

                CriteriaOperator criteria = CriteriaOperator.Or(criteriaList);

                //Create XPQuery, 
                Type dynQueryType = typeof(XPQuery<>).MakeGenericType(type);
                IQueryable dynXPQuery = Activator.CreateInstance(dynQueryType, new object[] { XpoSession }) as IQueryable;
                CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
                IQueryable<BaseObj> rows = dynXPQuery.AppendWhere(conv, criteria).Take(100).Cast<BaseObj>();
                var result = rows.Select(obj => new { value = obj.Oid, label = obj.GetMemberValue(label) }).ToList();
                return Json(new { result = result });

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        public JsonResult GetPrice(Guid store, Guid customer, Guid itemBarcode)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Store selectedStore = uow.GetObjectByKey<Store>(store);
                Customer selectedCustomer = uow.GetObjectByKey<Customer>(customer);
                ItemBarcode selectedItemBarcode = uow.GetObjectByKey<ItemBarcode>(itemBarcode);

                Session["Error"] = string.Empty;

                if (selectedStore == null)
                {
                    Session["Error"] += Resources.SelectStore + Environment.NewLine;
                }
                if (selectedCustomer == null)
                {
                    Session["Error"] += Resources.PleaseSelectACustomer + Environment.NewLine;
                }
                if (selectedItemBarcode == null)
                {
                    Session["Error"] += Resources.PleaseSelectAnItem + Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(Session["Error"].ToString()))
                {
                    return Json(new
                    {
                        key = Guid.Empty.ToString(),
                        store = string.Empty,
                        storePriceCatalogPolicy = string.Empty,
                        customer = string.Empty,
                        customerPriceCatalogPolicy = string.Empty,
                        item = string.Empty,
                        price = string.Empty,
                        vatIncluded = string.Empty,
                        priceCatalog = string.Empty,
                        trace = new List<PriceSearchTraceStep>()
                    });
                }

                List<PriceSearchTraceStep> traces = new List<PriceSearchTraceStep>();
                PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(selectedStore, selectedItemBarcode.Barcode.Code, selectedCustomer, traces: traces);
                PriceCatalogDetail priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

                if (priceCatalogDetail == null)
                {
                    if (traces.Count <= 0)
                    {
                        Session["Error"] = Resources.ItemNotFound;
                    }
                    return Json(new
                    {
                        key = Guid.Empty.ToString(),
                        store = selectedStore.Description,
                        storePriceCatalogPolicy = selectedStore.DefaultPriceCatalogPolicy.Description,
                        customer = selectedCustomer.FullDescription,
                        customerPriceCatalogPolicy = selectedCustomer.PriceCatalogPolicy != null ? selectedCustomer.PriceCatalogPolicy.Description : string.Empty,
                        item = selectedItemBarcode.Item.Name,
                        price = Resources.PriceNotFound,
                        vatIncluded = string.Empty,
                        priceCatalog = string.Empty,
                        trace = traces.Select(trc=> new { Number=trc.Number, PriceCatalogDescription=trc.PriceCatalogDescription, SearchMethod = trc.SearchMethod.ToLocalizedString()}).ToList()
                    });
                }

                return Json(new
                {
                    key = Guid.NewGuid().ToString(),
                    store = selectedStore.Description,
                    storePriceCatalogPolicy = selectedStore.DefaultPriceCatalogPolicy.Description,
                    customer = selectedCustomer.FullDescription,
                    customerPriceCatalogPolicy = selectedCustomer.PriceCatalogPolicy != null ? selectedCustomer.PriceCatalogPolicy.Description : string.Empty,
                    item = selectedItemBarcode.Item.Name,
                    price = priceCatalogDetail.Value,
                    vatIncluded = priceCatalogDetail.VATIncluded ? Resources.Yes : Resources.No,
                    priceCatalog = priceCatalogDetail.PriceCatalog.Description,
                    trace = traces.Select(trc => new { Number = trc.Number, PriceCatalogDescription = trc.PriceCatalogDescription, SearchMethod = trc.SearchMethod.ToLocalizedString() }).ToList()
                });
            }
        }

        public JsonResult GetDefaultCustomer(Guid store)
        {
            string label = string.Empty;
            string value = Guid.Empty.ToString();

            Store selectedStore = XpoSession.GetObjectByKey<Store>(store);
            if ( selectedStore != null )
            {
                Customer customer = GetDefaultCustomer(selectedStore);
                if ( customer != null )
                {
                    label = customer.FullDescription;
                    value = customer.Oid.ToString();
                }
            }

            return Json(new { label = label , value = value });
        }
    }
}
