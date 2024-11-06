using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.ResourcesLib;
using DevExpress.Web.Mvc;
using ITS.Retail.WebClient.Areas.B2C.ViewModel;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class ProductsController : BaseProductController
    {
        public static CriteriaOperator Filter
        {
            get
            {
                return System.Web.HttpContext.Current.Session["Filter"] as CriteriaOperator;
            }
            set
            {
                System.Web.HttpContext.Current.Session["Filter"] = value;
            }
        }

        public static string SearchResultTitle
        {
            get
            {
                return System.Web.HttpContext.Current.Session["SearchResultTitle"] == null
                                                            ? string.Empty
                                                            : System.Web.HttpContext.Current.Session["SearchResultTitle"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["SearchResultTitle"] = value;
            }
        }

        public static Dictionary<string, string> SearchCriteria
        {
            get
            {
                return System.Web.HttpContext.Current.Session["SearchCriteria"] == null
                                                        ? new Dictionary<string, string>()
                                                        : System.Web.HttpContext.Current.Session["SearchCriteria"] as Dictionary<string, string>;
            }
            set
            {
                System.Web.HttpContext.Current.Session["SearchCriteria"] = value;
            }
        }

        public override ActionResult Index([ModelBinder(typeof(RetailModelBinder))]ProductSearchCriteria criteria)
        {
            ViewBag.searchResultTitle = ResourcesLib.Resources.LatestProducts;
            ViewBag.Title = ResourcesLib.Resources.LatestProducts;
            return base.Index(criteria);
        }


        public ActionResult ItemsOfMotherCode(string ItemID)
        {
            Guid ItemGuid = ItemID == null || ItemID == "null" || ItemID == "" || ItemID == "-1" ? Guid.Empty : Guid.Parse(ItemID);
            Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));

            return PartialView("ItemsOfMotherCode", item.ChildItems);
        }

        public ActionResult CategoriesMenuCallbackPanel(Guid? CategoryID)
        {

            ItemCategory itemCategory = XpoSession.GetObjectByKey<ItemCategory>(CategoryID.HasValue ? CategoryID.Value : Guid.Empty);
            CriteriaOperator crop = new NullOperator("Parent");
            XPCollection<ItemCategory> categories = new XPCollection<ItemCategory>(XpoSession, crop);
            string htmlTree = ItemCategoryHelper.GenerateTreeHtml(categories, Url.Content("~/B2C/Products?CategoryID="));

            ViewBag.CategoriesMenu = htmlTree;
            return PartialView();
        }

        public JsonResult CategoriesTree(Guid? CategoryID)
        {
            if (CategoryID.HasValue == false)
            {
                CategoryID = Guid.Empty;
            }
            ItemCategory itemCategory = XpoSession.GetObjectByKey<ItemCategory>(CategoryID.Value);
            CriteriaOperator crop = new NullOperator("Parent");
            if (itemCategory != null)
            {
                crop = new BinaryOperator("Parent", itemCategory.Oid);
            }
            XPCollection<ItemCategory> categories = new XPCollection<ItemCategory>(XpoSession, crop);
            JObject jsonTree = ItemCategoryHelper.GenerateTreeJson(categories);
            JArray jArrayTree = new JArray() { jsonTree };
            string js = JsonConvert.SerializeObject(jArrayTree);
            return Json(js);
        }

        public ActionResult GetItem(string PriceCatalogOid)
        {

            Guid priceCatalogGuid = Guid.Empty;
            Guid.TryParse(PriceCatalogOid, out priceCatalogGuid);
            XPCollection<PriceCatalogDetail> priceCatalogDetail = new XPCollection<PriceCatalogDetail>(XpoHelper.GetNewUnitOfWork(),
                ApplyOwnerCriteria(new BinaryOperator("Oid", priceCatalogGuid), typeof(PriceCatalogDetail)));
            if (priceCatalogDetail == null)
            {
                XPCollection<Item> item = new XPCollection<Item>(XpoHelper.GetNewUnitOfWork(),
                    ApplyOwnerCriteria(new BinaryOperator("Oid", priceCatalogGuid), typeof(Item)));
                return View("Index", item);
            }
            else
            {
                return View("Index", priceCatalogDetail);
            }
        }

        public JsonResult JsonAddToWishList()
        {
            if(!IsUserLoggedIn)
            {
                Warning = Resources.PleaseLogin;
                return Json(new { success = false });
            }

            string priceCatalogDetailOidString = Request["PriceCatalogDetailOid"];
            
            PriceCatalogDetail priceCatalogDetail = WishList.Session.GetObjectByKey<PriceCatalogDetail>(Guid.Parse(priceCatalogDetailOidString));
            DocumentDetail detail = WishList.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Item.Oid == priceCatalogDetail.Item.Oid);
            if (detail == null)
            {
                detail = new DocumentDetail(WishList.Session) { /*DocumentHeader = WishList,*/ Item = priceCatalogDetail.Item, Barcode = priceCatalogDetail.Barcode, FinalUnitPrice = priceCatalogDetail.RetailValue };
                WishList.DocumentDetails.Add(detail);

                if (SaveWishlistToDatabase())
                {
                    Success = String.Format(Resources.ItemSuccessfullyAddedtoWishList, priceCatalogDetail.Item.Description);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                Warning = String.Format(Resources.ItemAlreadyAddedToWishList, priceCatalogDetail.Item.Description);
                return Json(new { success = true });
            }
        }

        public JsonResult JsonDeleteFromWishList()
        {
            string documentDetailOidString = Request["DocumentDetailOid"];
            Guid documentDetailGuid = Guid.Empty;
            if (Guid.TryParse(documentDetailOidString, out documentDetailGuid))
            {
                DocumentDetail detail = WishList.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Oid == documentDetailGuid);
                WishList.DocumentDetails.Remove(detail);
                if (SaveWishlistToDatabase())
                {
                    Success = String.Format(Resources.ItemSuccessfullyDeletedFromWishList, detail.Item.Description);
                    return Json(new { success = true });
                }
                
            }
            return Json(new { success = false });

        }

        public ActionResult List()
        {
            ViewBag.Title = Resources.CartOverview;
            ViewBag.MetaDescription = Resources.CartOverview;
            return View();
        }

        public ActionResult ListPartial()
        {
            return PartialView("ListPartial");

        }

    }
}
