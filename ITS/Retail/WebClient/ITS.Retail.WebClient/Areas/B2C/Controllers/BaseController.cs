using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using System.Reflection;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Web.Mvc;
using ITS.Retail.WebClient.AuxillaryClasses;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Extensions;
using System.IO;
using ITS.Retail.WebClient.Areas.B2C.ViewModel;
using ITS.Retail.WebClient.ViewModel;
using Newtonsoft.Json;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    [ActionLog(LogLevel = ITS.Retail.Platform.Enumerations.LogLevel.Advanced)]
    public class BaseController : Controller, ITS.Retail.WebClient.Controllers.IBaseController
    {
        public const int TOP_RETURNED_ITEMS = 10;

        private UnitOfWork fSession;
        public UnitOfWork XpoSession
        {
            get { return fSession; }
        }

        public BaseController()
        {
            fSession = XpoHelper.GetNewUnitOfWork();
        }


        protected string Danger
        {
            get
            {
                return Session["Danger"] == null ? string.Empty : Session["Danger"].ToString();
            }
            set
            {
                Session["Danger"] = value;
            }
        }

        protected string Success
        {
            get
            {
                return Session["Success"] == null ? string.Empty : Session["Success"].ToString();
            }
            set
            {
                Session["Success"] = value;
            }
        }

        protected string Warning
        {
            get
            {
                return Session["Warning"] == null ? string.Empty : Session["Warning"].ToString();
            }
            set
            {
                Session["Warning"] = value;
            }
        }

        protected UnitOfWork uow;

        protected void GenerateUnitOfWork()
        {
            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        protected void DisposeUnitOfWork()
        {
            if (uow != null)
            {
                uow.Dispose();
                uow = null;
            }
            Session["uow"] = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (fSession != null)
            {
                fSession.Dispose();
                fSession = null;
            }
        }

        protected int TotalProductPages(CriteriaOperator filter, out int numberOfProducts)
        {
            numberOfProducts = (int)XpoSession.Evaluate(typeof(PriceCatalogDetail), CriteriaOperator.Parse("Count"), filter);
            int numberOfPages = (int)Math.Ceiling(numberOfProducts * 1.0 / TOP_RETURNED_ITEMS);
            return numberOfPages;
        }


        private CompanyNew _CurrentCompany;
        /// <summary>
        /// Use ONLY when there is Request. There are also things that need consideration. See Case http://www.its.net.gr/fb/default.asp?5473
        /// </summary>
        public CompanyNew CurrentCompany
        {
            get
            {
                if (_CurrentCompany == null)
                {
                    _CurrentCompany = B2CHelper.GetB2CCompany(Request["SERVER_NAME"], XpoSession);
                    Session["CurrentCompanyOid"] = _CurrentCompany.Oid;
                }
                return _CurrentCompany;
            }
        }

        
        public static CompanyNew CurrentOwnerStatic
        {
            get
            {
                var v = System.Web.HttpContext.Current.Session["CurrentCompanyOid"];
                if (v != null && v is Guid)
                {
                    return XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>((Guid)v);
                }
                return null;
            }
        }

        protected CustomJSProperties CustomJSProperties { get; set; }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        protected bool performLog = true;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Example Add theme
            //DevExpressHelper.Theme = "MetropolisBlue";
            //DevExpressHelper.Theme = "Moderno";
            DevExpressHelper.Theme = "SimpleB2C";

            base.OnActionExecuting(filterContext);

            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Base" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Notification")
            {
                Session["ControllerName"] = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                Session["ActionName"] = filterContext.ActionDescriptor.ActionName;
            }

            this.CustomJSProperties = new CustomJSProperties();
            this.CustomJSProperties.AddJSProperty("ITScontroller", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            this.CustomJSProperties.AddJSProperty("action", filterContext.ActionDescriptor.ActionName);
            this.CustomJSProperties.AddJSProperty("home", Url.Content("~"));

            ViewData["CustomJSProperies"] = CustomJSProperties;

            //ViewBag.CurrentOwner = CurrentCompany;
            ViewBag.IsUserLoggedIn = CurrentUser != null;
            ViewBag.ControllerName = Session["ControllerName"];
            ViewBag.ActionName = Session["ActionName"];
            CookieManager();

            if (CurrentCompany != null)
            {
                ViewBag.Company = CurrentCompany;

                if (!String.IsNullOrWhiteSpace(ViewBag.Company.OwnerApplicationSettings.LocationGoogleID))
                {
                    string location = ViewBag.Company.OwnerApplicationSettings.LocationGoogleID.Trim();

                    string[] locationArray = location.Split(',');

                    this.CustomJSProperties.AddJSProperty("companyLatitute", locationArray[0]);

                    this.CustomJSProperties.AddJSProperty("companyLongtitude", locationArray[1]);
                }
                else
                {

                    this.CustomJSProperties.AddJSProperty("companyLatitute", null);

                    this.CustomJSProperties.AddJSProperty("companyLongtitude", null);
                }

                System.Text.StringBuilder mapContent = new System.Text.StringBuilder();

                if (!String.IsNullOrWhiteSpace(ViewBag.Company.CompanyName))
                {
                    mapContent.Append("<p><b>").Append(ViewBag.Company.CompanyName).Append("</b></p>");

                    this.CustomJSProperties.AddJSProperty("companyName", ViewBag.Company.CompanyName);
                }
                else
                {
                    this.CustomJSProperties.AddJSProperty("companyName", "");
                }

                if (ViewBag.Company.DefaultAddress != null)
                {

                    mapContent.Append("<p>");


                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.DefaultAddress.Street))
                    {
                        mapContent.Append(ViewBag.Company.DefaultAddress.Street);
                    }

                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.DefaultAddress.PostCode))
                    {

                        mapContent.Append(", ").Append(ViewBag.Company.DefaultAddress.PostCode);
                    }


                    mapContent.Append("</p>");

                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.DefaultAddress.City))
                    {

                        mapContent.Append("<p>").Append(ViewBag.Company.DefaultAddress.City).Append("</p>");

                    }

                }

                if (ViewBag.Company.OwnerApplicationSettings != null)
                {

                    mapContent.Append("<p>");


                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.OwnerApplicationSettings.Phone))
                    {
                        mapContent.Append("T: ").Append(ViewBag.Company.OwnerApplicationSettings.Phone);
                    }

                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.OwnerApplicationSettings.FAX))
                    {
                        mapContent.Append(", F: ").Append(ViewBag.Company.OwnerApplicationSettings.FAX);
                    }


                    if (!String.IsNullOrWhiteSpace(ViewBag.Company.OwnerApplicationSettings.eMail))
                    {
                        mapContent.Append(", E: ").Append(ViewBag.Company.OwnerApplicationSettings.eMail);
                    }

                    mapContent.Append("</p>");
                }

                this.CustomJSProperties.AddJSProperty("mapContent", mapContent);
                if (Request.IsAjaxRequest() == false)
                {
                    XPCollection<ItemCategory> rootItemCategories = new XPCollection<ItemCategory>(XpoSession, new NullOperator("Parent"));
                    List<CategoryView> rootCategories = new List<CategoryView>();
                    foreach (ItemCategory cat in rootItemCategories)
                    {
                        CategoryView view = new CategoryView();
                        view.LoadPersistent(cat);
                        rootCategories.Add(view);
                    }
                    CategoryView root = new CategoryView() { title = "Κατηγορίες", items = rootCategories.OrderBy(x => x.Description).ToList(), icon = "fa fa-tags" };
                    this.CustomJSProperties.AddJSProperty("root_categories", JsonConvert.SerializeObject(root, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));                    
                    this.CustomJSProperties.AddJSProperty("ShoppingCartItemsCount", ShoppingCart.DocumentDetails.Select(documentDetail => documentDetail.Item).Distinct().Count());
                }
            }
            else
            {
                ViewBag.Company = null;
            }


            /** Example Should be Dynamic  **/
            ViewData["FontCssName"] = string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.Fonts) ? "Open_Sans" : CurrentCompany.OwnerApplicationSettings.Fonts;


            ItemCategory ic = XpoSession.FindObject<ItemCategory>(null);
            ViewBag.CategoriesExist = ic != null;

        }

        public JsonResult jsonGetItemCategories(Guid? CategoryOid)
        {
            if (CategoryOid.HasValue)
            {
                ItemCategory category = XpoSession.GetObjectByKey<ItemCategory>(CategoryOid.Value);
                XPCollection<ItemCategory> rootItemCategories = new XPCollection<ItemCategory>(XpoSession, new BinaryOperator("ParentOid", CategoryOid.Value));
                List<CategoryView> rootCategories = new List<CategoryView>();
                foreach (ItemCategory cat in rootItemCategories)
                {
                    CategoryView view = new CategoryView();
                    view.LoadPersistent(cat);
                    rootCategories.Add(view);
                }
                CategoryView root = new CategoryView()
                {
                    title = category.Description,
                    name = category.Description,
                    Oid = category.Oid,
                    link = "#" + category.Oid,
                    items = rootCategories.OrderByDescending(x => x.Description).ToList()
                };
                return Json(new { result = JsonConvert.SerializeObject(root, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }) });
            }
            return null;
        }

        public ActionResult ShoppingCartView([ModelBinder(typeof(RetailModelBinder))] ShoppingCartAction parameters)
        {
            switch (parameters.Action)
            {
                case eShoppingCartAction.ADD:
                    {
                        string erromsg;
                        if(DocumentHelper.MaxCountOfLinesExceeded(ShoppingCart,out erromsg))
                        {
                            Danger = erromsg;
                        }
                        else
                        {
                            decimal quantity = parameters.Qty.Value;
                            PriceCatalogDetail priceCatalogDetail = AddToCart(parameters.PriceCatalogDetailGuid.Value, quantity);
                            if (quantity > 0)
                            {
                                Success = String.Format(Resources.ItemSuccessfullyAddedB2C, priceCatalogDetail.Item.Description);
                            }
                        }
                    }
                    break;
                case eShoppingCartAction.UPDATE:
                    {
                        DocumentDetail detail = ShoppingCart.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Oid == parameters.DocumentDetailOid.Value);
                        DocumentHeader header = ShoppingCart;
                        decimal oldQty = detail.Qty;
                        decimal newQuantity = parameters.Qty.Value;                        
                        if (newQuantity > 0)
                        {
                            DocumentHelper.ComputeDocumentLine(ref header, detail.Item, detail.Barcode, newQuantity, false, -1, false, "", null, oldDocumentLine: detail);
                            DocumentHelper.RecalculateDocumentCosts(ref header, false);
                            Success = String.Format(Resources.ItemQuantityChangedSuccessfullyB2C, detail.Item.Description, oldQty, newQuantity);
                        }
                        else
                        {
                            ShoppingCart.DocumentDetails.Remove(detail);
                            DocumentHelper.RecalculateDocumentCosts(ref header, false);
                            Success = String.Format(Resources.ItemSuccessfullyDeletedΒ2C, detail.Item.Description);
                        }
                    }
                    break;
                case eShoppingCartAction.DELETE:
                    {
                        DocumentDetail detail = ShoppingCart.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Oid == parameters.DocumentDetailOid.Value);
                        ShoppingCart.DocumentDetails.Remove(detail);
                        DocumentHeader documentHeader = (DocumentHeader)ShoppingCart;
                        DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                        Success = String.Format(Resources.ItemSuccessfullyDeletedΒ2C, detail.Item.Description);
                    }
                    break;
                case eShoppingCartAction.ADDFROMWISHLIST:
                    {
                        string erromsg;
                        if (DocumentHelper.MaxCountOfLinesExceeded(ShoppingCart, out erromsg))
                        {
                            Danger = erromsg;
                        }
                        else
                        {
                            DocumentDetail detail = WishList.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Oid == parameters.DocumentDetailOid.Value);
                            PriceCatalogDetail priceCatalogDetail;
                            if (detail != null)
                            {
                                priceCatalogDetail = ItemHelper.GetItemPrice(PriceCatalog, detail.Item);
                                if (priceCatalogDetail != null)
                                {
                                    WishList.DocumentDetails.Remove(detail);
                                    if (SaveWishlistToDatabase())
                                    {
                                        PriceCatalogDetail priceCatalogDetailToCart = AddToCart(priceCatalogDetail.Oid, 1);
                                        Success = String.Format(Resources.ItemSuccessfullyAddedB2C, detail.Item.Description);
                                    }
                                }
                                else
                                {
                                    Danger = Resources.AnErrorOccurred;
                                }
                            }
                            else
                            {
                                Danger = Resources.AnErrorOccurred;
                            }
                        }
                       
                    }
                    break;
            }
            return PartialView("../Shared/ShoppingCartSideBar", ShoppingCart);
        }

        protected static PriceCatalogDetail AddToCart(Guid priceCatalogDetailOid, decimal qty)
        {            
            PriceCatalogDetail priceCatalogDetail = ShoppingCart.Session.GetObjectByKey<PriceCatalogDetail>(priceCatalogDetailOid);
            if ( qty > 0)
            {
                DocumentDetail detail = ShoppingCart.DocumentDetails.FirstOrDefault(documentDetail => documentDetail.Item.Oid == priceCatalogDetail.Item.Oid);
                DocumentHeader documentHeader = (DocumentHeader)ShoppingCart;
                if (detail == null)
                {
                    detail = DocumentHelper.ComputeDocumentLine(ref documentHeader, priceCatalogDetail.Item, priceCatalogDetail.Barcode, qty, false, -1, false, "", null);
                    DocumentHelper.AddItem(ref documentHeader, detail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
                else
                {
                    detail = DocumentHelper.ComputeDocumentLine(ref documentHeader, priceCatalogDetail.Item, priceCatalogDetail.Barcode, detail.Qty + qty, false, -1, false, "", null, oldDocumentLine: detail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
                ShoppingCart.GrossTotal = ShoppingCart.DocumentDetails.Sum(g => g.GrossTotal);
            }
            return priceCatalogDetail;
        }

        public static DocumentHeader ShoppingCart
        {
            get
            {
                DocumentHeader _ShoppingCart = (DocumentHeader)System.Web.HttpContext.Current.Session["ShoppingCart"];                
                if (_ShoppingCart == null)
                {
                    _ShoppingCart = new DocumentHeader(XpoHelper.GetNewUnitOfWork());
                    _ShoppingCart.Store = _ShoppingCart.Session.GetObjectByKey<Store>(CurrentOwnerStatic.OwnerApplicationSettings.B2CStore.Oid);
                    _ShoppingCart.DocumentType = _ShoppingCart.Session.GetObjectByKey<DocumentType>(CurrentOwnerStatic.OwnerApplicationSettings.B2CDocumentType.Oid);

                    _ShoppingCart.DocumentSeries = _ShoppingCart.Session.GetObjectByKey<DocumentSeries>(CurrentOwnerStatic.OwnerApplicationSettings.B2CDocumentSeries.Oid);
                    _ShoppingCart.Customer = _ShoppingCart.Session.GetObjectByKey<Customer>(CurrentOwnerStatic.OwnerApplicationSettings.B2CDefaultCustomer.Oid);
                    if (_ShoppingCart.Customer == null)
                    {
                        System.Web.HttpContext.Current.Session["Danger"] = Resources.PleaseLogin;
                    }
                    _ShoppingCart.PriceCatalog = _ShoppingCart.Session.GetObjectByKey<PriceCatalog>(CurrentOwnerStatic.OwnerApplicationSettings.B2CPriceCatalog.Oid);
                    _ShoppingCart.Source = Platform.Enumerations.DocumentSource.B2C;
                    _ShoppingCart.FinalizedDate = DateTime.Now;
                    _ShoppingCart.Division = eDivision.Sales;
                    CriteriaOperator statusCriteria = ApplyOwnerCriteriaStatic(new BinaryOperator("IsDefault",true),typeof(DocumentStatus),_ShoppingCart.Owner);
                    _ShoppingCart.Status = _ShoppingCart.Session.FindObject<DocumentStatus>(statusCriteria);
                    if(_ShoppingCart.Status==null)
                    {
                        statusCriteria = ApplyOwnerCriteriaStatic(null,typeof(DocumentStatus),_ShoppingCart.Owner);
                        _ShoppingCart.Status = _ShoppingCart.Session.FindObject<DocumentStatus>(statusCriteria);
                    }
                }
                System.Web.HttpContext.Current.Session["ShoppingCart"] = _ShoppingCart;
                return _ShoppingCart;
            }
            set
            {
                System.Web.HttpContext.Current.Session["ShoppingCart"] = value;
            }

        }


        /// <summary>
        /// Returns User's WishList. A new WishList object is created if neccessary. 
        /// Be Carefull!!! Session["WishList"] is also filled and is returned in a different UnitOfWork. 
        /// In this way when user adds/removes items from the WishList this occurs on this different UnitOfWork so as to be easyly and safely saved.
        /// See also SaveWishlistToDatabase() Method.
        /// </summary>
        public static DocumentHeader WishList
        {
            get
            {
                if(CurrentUser==null)
                {
                    return null;
                }
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader _WishList =  (DocumentHeader)System.Web.HttpContext.Current.Session["WishList"]!=null
                                                ? (DocumentHeader)System.Web.HttpContext.Current.Session["WishList"]
                                                : ( CurrentUser.WishList == null ? null : uow.GetObjectByKey<DocumentHeader>(CurrentUser.WishList.Oid));
                    if (_WishList == null)
                    {
                        _WishList = new DocumentHeader(uow);
                        _WishList.Store = _WishList.Session.GetObjectByKey<Store>(CurrentOwnerStatic.OwnerApplicationSettings.B2CStore.Oid);
                        _WishList.DocumentType = _WishList.Session.GetObjectByKey<DocumentType>(CurrentOwnerStatic.OwnerApplicationSettings.B2CDocumentType.Oid);
                        _WishList.Source = DocumentSource.B2C;
                        _WishList.Customer = _WishList.Session.GetObjectByKey<Customer>(CurrentCustomer.Oid);
                        if (_WishList.Customer == null)
                        {
                            System.Web.HttpContext.Current.Session["Danger"] = Resources.PleaseLogin;
                        }
                        _WishList.PriceCatalog = _WishList.Session.GetObjectByKey<PriceCatalog>(CurrentOwnerStatic.OwnerApplicationSettings.B2CPriceCatalog.Oid);
                        _WishList.Source = Platform.Enumerations.DocumentSource.B2C;
                        _WishList.FinalizedDate = DateTime.Now;
                        _WishList.Division = eDivision.Sales;
                        _WishList.Save();
                        User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
                        if (user.WishList != null)
                        {
                            user.WishList.Delete();
                        }
                        user.WishList = _WishList;
                        user.Save();
                        uow.CommitTransaction();
                        CurrentUser.Reload();
                    }
                    
                    if (System.Web.HttpContext.Current.Session["WishList"]==null)
                    {
                        System.Web.HttpContext.Current.Session["WishList"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(CurrentUser.WishList.Oid);
                    }
                    return (DocumentHeader)System.Web.HttpContext.Current.Session["WishList"];
                }
            }
        }

        private void CookieManager()
        {
            if (Request.UserLanguages != null)
            {
                string cultureName = null;
                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                // Modify current thread's cultures
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol = "€";
                Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyPositivePattern = 3;
                Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyNegativePattern = 8;
                Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencySymbol = "€";
                Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyPositivePattern = 3;
                Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyNegativePattern = 8;
                ViewData["CurrentCulture"] = cultureName;
                if (cultureCookie == null)
                {
                    cultureCookie = new HttpCookie("_culture");
                }
                cultureCookie.Value = cultureName;
                Response.Cookies.Add(cultureCookie);
            }
        }

        public bool SaveWishlistToDatabase()
        {
            try
            {
                WishList.Save();
                WishList.Session.CommitTransaction();
                WishList.Session.Dispose();
                //((DocumentHeader)Session["WishList"]).Session = null;
                Session["WishList"] = null;
                CurrentUser.Reload();//This is nessecary!!!
                return true;
            }
            catch (Exception exception)
            {
                Danger = exception.GetFullMessage();
                return false;
            }
        }

        public static User CurrentUser
        {
            get
            {
                return (User)System.Web.HttpContext.Current.Session["B2CUser"];
            }
            set
            {
                System.Web.HttpContext.Current.Session["B2CUser"] = value;
            }
        }



        public static Customer CurrentCustomer
        {
            get
            {
                if (IsUserLoggedIn == false)
                {
                    return null;
                }

                Customer customer = UserHelper.GetCustomer(CurrentUser);
                return customer;
            }
        }

        private PriceCatalog _PriceCatalog;
        public PriceCatalog PriceCatalog
        {
            get
            {
                if(this._PriceCatalog == null)
                {
                    if (CurrentCompany == null || CurrentCompany.OwnerApplicationSettings == null || CurrentCompany.OwnerApplicationSettings.B2CPriceCatalog == null)
                    {
                        this._PriceCatalog=XpoSession.FindObject<PriceCatalog>(new NullOperator("ParentCatalog"));
                    }
                    else
                    {
                        this._PriceCatalog = CurrentCompany.OwnerApplicationSettings.B2CPriceCatalog;
                    }                    
                }               
                return this._PriceCatalog;
            }
        }

        public ActionResult ItemCategoriesTreeView()
        {
            return PartialView();
        }

        public static CriteriaOperator ApplyOwnerCriteriaStatic(CriteriaOperator inputCriteria, Type type, CompanyNew owner = null)
        {
            if (owner == null)
            {
                owner = CurrentOwnerStatic;
            }

            if (typeof(IOwner).IsAssignableFrom(type) && owner != null)
            {
                return CriteriaOperator.And(
                            inputCriteria,
                            CriteriaOperator.Or(
                                new BinaryOperator("Owner.Oid", owner.Oid),
                                new NullOperator("Owner")
                            )
                        );
            }
            return inputCriteria;
        }

        public CriteriaOperator ApplyOwnerCriteria(CriteriaOperator inputCriteria, Type type, CompanyNew owner = null)
        {
            if (owner == null)
            {
                owner = CurrentCompany;
            }

            if (typeof(IOwner).IsAssignableFrom(type) && owner != null)
            {
                return CriteriaOperator.And(
                            inputCriteria,
                            CriteriaOperator.Or(
                                new BinaryOperator("Owner.Oid", owner.Oid),
                                new NullOperator("Owner")
                            )
                        );
            }
            return inputCriteria;
        }


        public static void VirtualModeCreateChildren(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                IEnumerable<ItemCategory> itcs = null;
                if (e.NodeName == null)
                {
                    //root
                    itcs = new XPCollection<ItemCategory>(uow, ApplyOwnerCriteriaStatic(new NullOperator("ParentOid"), typeof(ItemCategory))).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                }
                else
                {
                    //child
                    string nodeOidString = e.NodeName;
                    Guid nodeOid;
                    if (Guid.TryParse(nodeOidString, out nodeOid))
                    {
                        itcs = new XPCollection<ItemCategory>(uow, ApplyOwnerCriteriaStatic(new BinaryOperator("ParentOid", nodeOid, BinaryOperatorType.Equal), typeof(ItemCategory))).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                    }
                    else
                    {
                        //todo
                    }

                }
                foreach (ItemCategory ic in itcs)
                {
                    TreeViewVirtualNode tvvn = new TreeViewVirtualNode(ic.Oid.ToString(), ic.Code + " - " + ic.Description);
                    tvvn.IsLeaf = ic.ChildCategories.Count == 0;
                    children.Add(tvvn);
                }
            }
            e.Children = children;
        }

        public static bool IsUserLoggedIn
        {
            get
            {
                return CurrentUser != null;
            }
        }

        public ActionResult ReturnsView(string ViewName, bool? IsPartial)
        {
            return IsPartial.HasValue && IsPartial.Value ? PartialView(ViewName) : (ActionResult)View(ViewName);
        }

        public string RenderViewToString(Model.NonPersistant.EmailTemplate model)
        {
            ViewData.Model = model;
            using (StringWriter streamWriter = new StringWriter())
            {
                ViewEngineResult viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, model.ViewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewEngineResult.View, ViewData, TempData, streamWriter);
                viewEngineResult.View.Render(viewContext, streamWriter);
                viewEngineResult.ViewEngine.ReleaseView(ControllerContext, viewEngineResult.View);
                return streamWriter.GetStringBuilder().ToString();
            }
        }
               
        public FileContentResult DownloadExtraFile()
        {
            if (!String.IsNullOrEmpty(Request["Oid"]))
            {
                Item item;
                Guid itemGuid;
                if (Guid.TryParse(Request["Oid"], out itemGuid))
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        item = uow.GetObjectByKey<Item>(itemGuid);
                        return File(item.ExtraFile, item.ExtraMimeType, "Item_" + item.Code + "_InformationSheet." + item.ExtraFilename.Split('.').Last());
                    }
                }

            }
            return null;
        }
    }
}
