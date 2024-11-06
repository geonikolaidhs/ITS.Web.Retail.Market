using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Areas.B2C.ViewModel;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class BaseProductController : BaseController
    {
        private List<PriceCatalogDetail> GetPriceCatalogDetails(ProductSearchCriteria criteria)
        {
            ProductSearchCriteria cachedCriteria = Session["SearchCriteria"] as ProductSearchCriteria;
            //Predefined filter variables
            criteria.Owner = CurrentCompany.Oid;
            criteria.PriceCatalog = this.PriceCatalog.Oid;
            criteria.IsActive = true;

            CriteriaOperator filter;
            if (cachedCriteria != null && criteria.Page.HasValue == true)
            {
                cachedCriteria.Page = criteria.Page;
                criteria = cachedCriteria;
                ViewBag.TotalPages = criteria.TotalPages;
                ViewBag.TotalProducts = criteria.TotalResults;
                ViewBag.CurrentPage = criteria.Page;
                criteria.XpoSession = this.XpoSession;
                filter = criteria.BuildCriteria();
            }
            else
            {
                int prod = 0;
                criteria.XpoSession = this.XpoSession;
                filter = criteria.BuildCriteria();
                ViewBag.TotalPages = criteria.TotalPages = this.TotalProductPages(filter, out prod);
                ViewBag.TotalProducts = criteria.TotalResults = prod;
                ViewBag.CurrentPage = criteria.Page = 1;
            }
            Dictionary<string,string> searchResultTitle = new Dictionary<string,string>();
            if (String.IsNullOrWhiteSpace(criteria.SearchText)==false)
            {
                searchResultTitle.Add(Resources.SearchCriteria, criteria.SearchText);
            }
            if(criteria.CategoryID.HasValue)
            {
                searchResultTitle.Add(Resources.Category, criteria.CategoryDescription);
            }
            ViewBag.searchResultTitle = (searchResultTitle.Count == 0) ? Resources.RecentProducts : searchResultTitle.Select(x => string.Format("{0}: {1}", x.Key, x.Value)).Aggregate((x, y) => string.Format("{0}, {1}", x, y));

            Session["SearchCriteria"] = criteria;
            //    <value>Βλέπετε {0} έως {1} από {2} ({3} Σελίδες)</value>
            ViewBag.ShowingItemsOnPage = String.Format(Resources.ShowingItemsOnPage, (criteria.Page.Value-1) * TOP_RETURNED_ITEMS + 1, criteria.Page.Value * TOP_RETURNED_ITEMS , criteria.TotalResults, criteria.TotalPages);
            if (String.IsNullOrWhiteSpace(criteria.Barcode) && criteria.ForceXPCollection == false)
            {
                XPQuery<PriceCatalogDetail> priceCatalogDetailsXPQ = new XPQuery<PriceCatalogDetail>(XpoSession);
                CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
                IQueryable priceCatalogDetails = priceCatalogDetailsXPQ.Join(new XPQuery<Item>(XpoSession), x => x.Item.Oid, y => y.Oid, (x, y) => x)
                    .Join(new XPQuery<ItemBarcode>(XpoSession), x => x.Item.Oid, y => y.Item.Oid, (x, y) => x)
                    .AppendWhere(conv, filter).MakeOrderBy(conv, new ServerModeOrderDescriptor(new OperandProperty("Item.InsertedDate"), false))
                    .Skip((criteria.Page.Value-1) * TOP_RETURNED_ITEMS).Take(TOP_RETURNED_ITEMS);
                if (!String.IsNullOrWhiteSpace(ViewBag.Company.OwnerApplicationSettings.MetaDescription))
                {
                    ViewBag.MetaDescription = ViewBag.Company.OwnerApplicationSettings.MetaDescription;
                }
                return priceCatalogDetails.Cast<PriceCatalogDetail>().ToList();
            }
            return new XPCollection<PriceCatalogDetail>(XpoSession, filter).Skip((criteria.Page.Value - 1) * TOP_RETURNED_ITEMS).Take(TOP_RETURNED_ITEMS).ToList();
        }
        public virtual ActionResult Index([ModelBinder(typeof(RetailModelBinder))]ProductSearchCriteria criteria)
        {
            List<PriceCatalogDetail> priceCatalogDetails = GetPriceCatalogDetails(criteria);
            return View("../Products/Index", priceCatalogDetails);
        }

        public virtual ActionResult Search([ModelBinder(typeof(RetailModelBinder))]ProductSearchCriteria criteria)
        {
            List<PriceCatalogDetail> priceCatalogDetails = GetPriceCatalogDetails(criteria);
            ViewBag.IsPartial = true;
            return PartialView("../Products/Index", priceCatalogDetails);
        }
    }
}
