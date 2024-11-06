using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ItemStockController : BaseController
    {
        [Security(ReturnsPartial = false)]
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

        public ActionResult SearchItemStock()
        {
            RetrieveInitialData();
            return PartialView();
        }

        private void RetrieveInitialData()
        {
            XPCollection<Store> companyStores = GetList<Store>(XpoSession);
            ViewBag.Stores = companyStores.Select(store => new { value = store.Oid.ToString(), label = store.Description });
            ViewBag.SelectedStore = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL
                                        ? companyStores.First().Oid.ToString()
                                        : (Session["currentStore"] as StoreViewModel).Oid.ToString();

            Store selectedStore = CurrentStore == null ? null : XpoSession.GetObjectByKey<Store>(CurrentStore.Oid);
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

        public JsonResult RecalculateItemStock(Guid store, DateTime fromDate, Guid itemBarcode)
        {
            try
            {
                if ( MvcApplication.ApplicationInstance != eApplicationInstance.DUAL_MODE
                    && MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL
                   )
                {
                    return Json(new { error = ResourcesLib.Resources.PermissionDenied });
                }

                string errorMessage = ItemStockHelper.RecalculateItemStock(store, fromDate, itemBarcode);
                if (String.IsNullOrEmpty(errorMessage) == false)
                {
                    return Json(new { error = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
            return Json(new { });
        }
    }
}
