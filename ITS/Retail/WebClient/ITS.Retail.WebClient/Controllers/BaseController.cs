using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using ITS.Retail.Common.Helpers;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using Newtonsoft.Json.Linq;
using StackExchange.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ITS.Retail.WebClient.Controllers
{

    [ActionLog(LogLevel = LogLevel.Advanced)]
    [HandleError]
    public class BaseController : Controller, IBaseController
    {
        protected DialogOptions DialogOptions { get; set; }
        protected CustomJSProperties CustomJSProperties { get; set; }
        protected List<Wizard> ActiveWizards
        {
            get
            {
                if (Session["ActiveWizards"] == null)
                {
                    Session["ActiveWizards"] = new List<Wizard>();
                }

                return Session["ActiveWizards"] as List<Wizard>;
            }
        }

        public static List<Wizard> StaticActiveWizards
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["ActiveWizards"] == null)
                {
                    System.Web.HttpContext.Current.Session["ActiveWizards"] = new List<Wizard>();
                }
                return System.Web.HttpContext.Current.Session["ActiveWizards"] as List<Wizard>;
            }
        }
        public BaseController()
        {
            fSession = XpoHelper.GetNewUnitOfWork();
            this.CustomJSProperties = new CustomJSProperties();
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (fSession != null && fSession.IsConnected)
            {
                fSession.Dispose();
            }
        }

        /// <summary>
        /// Selected owner. Admin can change.
        /// User selections can alter this property
        /// </summary>
        static public CompanyNew CurrentOwnerStatic
        {
            get
            {
                if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
                {
                    return StoreControllerAppiSettings.Owner;
                }
                if (System.Web.HttpContext.Current.Session["currentOwner"] == null || System.Web.HttpContext.Current.Session["currentOwner"] is CompanyNew == false)
                {
                    return null;
                }
                return (CompanyNew)System.Web.HttpContext.Current.Session["currentOwner"];
            }
            set
            {
                System.Web.HttpContext.Current.Session["currentOwner"] = value;
            }
        }

        private CompanyNew _CurrentOwner;
        public CompanyNew CurrentOwner
        {
            get
            {
                if (_CurrentOwner == null)
                {
                    _CurrentOwner = CurrentOwnerStatic;
                }
                return _CurrentOwner;
            }

        }



        static public List<StoreViewModel> StoresThatCurrentUserOwns
        {
            get
            {
                object stores = System.Web.HttpContext.Current.Session["StoresThatCurrentUserOwns"];
                if (stores == null || stores is List<StoreViewModel> == false)
                {
                    return null;
                }
                return (List<StoreViewModel>)stores;
            }
            set
            {
                System.Web.HttpContext.Current.Session["StoresThatCurrentUserOwns"] = value;
            }

        }
        private UnitOfWork fSession;
        public UnitOfWork XpoSession
        {
            get { return fSession; }
        }

        private User _currentUser = null;
        public User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    var user = Membership.GetUser();
                    if (user == null)
                    {
                        return null;
                    }
                    if (user.UserName == "superadmin")
                    {
                        _currentUser = new User(this.XpoSession)
                        {
                            UserName = "superadmin",
                            IsActive = true,
                            IsApproved = true,
                            TermsAccepted = true,
                            Role = new Role(this.XpoSession) { Description = "superadmin role" }
                        };
                    }
                    else
                    {
                        Guid cu = (Guid)user.ProviderUserKey;
                        User modelUser = XpoHelper.GetNewUnitOfWork().GetObjectByKey<User>(cu);
                        _currentUser = modelUser;
                    }
                }
                return _currentUser;
            }
        }

        protected StoreViewModel CurrentStore
        {
            get
            {
                return Session["currentStore"] as StoreViewModel;
            }
            set
            {
                Session["currentStore"] = value;
            }
        }


        static public User CurrentUserStatic
        {
            get
            {
                try
                {
                    var user = Membership.GetUser();
                    if (user == null)
                    {
                        return null;
                    }
                    if (user.UserName == "superadmin")
                    {
                        UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                        return new User(uow) { UserName = "superadmin", IsActive = true, IsApproved = true, TermsAccepted = true, Role = new Role(uow) { Description = "superadmin role" } };
                    }
                    Guid cu = (Guid)user.ProviderUserKey;
                    User modelUser = XpoHelper.GetNewUnitOfWork().GetObjectByKey<User>(cu);
                    return modelUser;
                }
                catch
                {
                    return null;
                }
            }

        }
        /// <summary>
        /// Application settings of the selected owner. If current owner is null or does not have specific settings, default settings are used
        /// </summary>
        public static OwnerApplicationSettings OwnerApplicationSettings
        {
            get
            {
                if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
                {
                    return StoreControllerAppiSettings.OwnerApplicationSettings;
                }
                if (System.Web.HttpContext.Current.Session["OwnerSettings"] == null || System.Web.HttpContext.Current.Session["OwnerSettings"] is OwnerApplicationSettings == false)
                {
                    return XpoHelper.GetNewUnitOfWork().FindObject<OwnerApplicationSettings>(new NullOperator("Owner"));
                }
                return (OwnerApplicationSettings)System.Web.HttpContext.Current.Session["OwnerSettings"];
            }
        }

        /// <summary>
        /// The owner of the user. Null for Admin.
        /// User selections cannot alter this property
        /// </summary>
        static public CompanyNew UserOwner
        {
            get
            {
                return (CompanyNew)System.Web.HttpContext.Current.Session["userOwner"];
            }
        }

        /// <summary>
        /// The actual owner that will be used during filtering/data manupulation
        /// </summary>
        static public CompanyNew EffectiveOwner
        {
            get
            {
                return UserOwner ?? CurrentOwnerStatic;
            }
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        private static string[] mobileDevices = new string[] { "windows ce", "windows phone", "windows mobile" };

        protected bool IsMobileDevice()
        {

            if (Request.UserAgent == null)
            {
                return false;
            }

            string userAgent = Request.UserAgent.ToLower();
            return mobileDevices.Any(x => userAgent.Contains(x));
        }


        protected Guid actionlogguid;

        public ActionResult InvalidFunction()
        {
            return View();
        }

        protected string applicationVersion;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        protected virtual void CreateMenu()
        {
            List<string> forbidden = Session["LayoutForbidden"] as List<String>;
            if (forbidden == null)
            {
                Session["LayoutForbidden"] = forbidden = new List<string>();
            }

            MenuInfo menuInfo = Session["Menu"] as MenuInfo;
            if (menuInfo != null)
            {
                if (DateTime.Now.Ticks - menuInfo.CreatedAt.Ticks <= TimeSpan.TicksPerHour)
                {
                    menuInfo.Menu.Url = Url;
                    menuInfo.Menu.RemoveHtmlClass("active");
                    return;
                }
            }

            if (CurrentUser == null)
            {
                return;
            }

            XPCollection<StoreDocumentSeriesType> orderDocumentTypes = null;
            if (CurrentStore != null)
            {
                orderDocumentTypes = DocumentHelper.GetOrderStoreDocumentSeriesTypes(this.XpoSession, this.XpoSession.GetObjectByKey<Store>(CurrentStore.Oid));
            }

            MenuNode megamenu = new MenuNode(forbidden, Url);
            MenuNode salesDocuments = new MenuNode(forbidden, Url)
            {
                HtmlID = "salesDocuments",
                Caption = Resources.SalesDocuments
            };

            List<string> errorMessages = new List<string>();
            if (Session["Error"] != null
                && !String.IsNullOrEmpty(Session["Error"].ToString())
                && !String.IsNullOrWhiteSpace(Session["Error"].ToString())
               )
            {
                errorMessages.Add(Session["Error"].ToString());
            }
            List<string> temperrorMessages = new List<string>();
            StoreViewModel storeViewModel = Session["currentStore"] as StoreViewModel;
            bool storeAndCompanyExist = storeViewModel != null && OwnerApplicationSettings != null;
            bool salesDocumentTypesExist = storeAndCompanyExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Sales, MvcApplication.ApplicationInstance, out temperrorMessages) : false;
            errorMessages.AddRange(temperrorMessages);
            bool orderDocumentTypesExist = salesDocumentTypesExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Sales, MvcApplication.ApplicationInstance, out temperrorMessages, isOrder: true) : false;
            errorMessages.AddRange(temperrorMessages);
            bool proformaDocumentTypesExist = salesDocumentTypesExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Sales, MvcApplication.ApplicationInstance, out temperrorMessages) : false;
            errorMessages.AddRange(temperrorMessages);
            bool purchaseDocumentTypesExist = storeAndCompanyExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Purchase, MvcApplication.ApplicationInstance, out temperrorMessages) : false;
            errorMessages.AddRange(temperrorMessages);
            bool storeDocumentTypesExist = storeAndCompanyExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Store, MvcApplication.ApplicationInstance, out temperrorMessages) : false;
            errorMessages.AddRange(temperrorMessages);
            bool financialDocumentTypesExist = storeAndCompanyExist ? DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, storeViewModel.Oid, eDivision.Financial, MvcApplication.ApplicationInstance, out temperrorMessages) : false;
            errorMessages.AddRange(temperrorMessages);

            ///New Sales Document
            if (UserHelper.IsCustomer(CurrentUser) == false)
            {
                salesDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.NewSalesDocument,
                    DirectLink = "javascript:;",
                    DataOid = true,
                    DataMode = eDivision.Sales,
                    HtmlID = "newSalesDocument",
                    HtmlClass = !salesDocumentTypesExist ? "order-shortcut disabled" : "order-shortcut"
                });
            }

            ////New Order Document
            if (orderDocumentTypes != null && orderDocumentTypes.Count > 0)
            {
                int orderTypeCount = 0;
                foreach (StoreDocumentSeriesType orderType in orderDocumentTypes)
                {
                    salesDocuments.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = orderType.Description.Replace("\n", "<br>"),
                        DirectLink = "javascript:;",
                        DataOid = true,
                        DataMode = eDivision.Sales,
                        DataDocType = orderType.Oid.ToString(),
                        HtmlID = "newSalesOrderNew" + orderTypeCount++,
                        HtmlClass = !orderDocumentTypesExist ? "disabled order-shortcut" : "order-shortcut"
                    });
                }
            }

            if (errorMessages.Count > 0)
            {
                Session["Error"] = errorMessages.Aggregate((err1, err2) => err1 + Environment.NewLine + err2);
            }

            ///Sales Documents List
            salesDocuments.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = Resources.SalesDocumentList,
                Action = "Index",
                Controller = "Document",
                LinkParameters = new { Mode = "Sales" },
                Javascript = "",
                HtmlID = "listSalesDocument",
                HtmlClass = ""
            });


            ///Proforma Documents
            if ((MvcApplication.ApplicationInstance == (eApplicationInstance.STORE_CONTROLER)
                || MvcApplication.ApplicationInstance == (eApplicationInstance.DUAL_MODE)) &&
                UserHelper.IsCustomer(CurrentUser) == false)
            {
                salesDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.ProformaInvoices,
                    Action = "Proforma",
                    Controller = "Document",
                    Javascript = "",
                    HtmlID = "proformaInvoices",
                    HtmlClass = !proformaDocumentTypesExist ? "disabled" : ""
                });

                if ((MvcApplication.ApplicationInstance == (eApplicationInstance.STORE_CONTROLER)
                        || MvcApplication.ApplicationInstance == (eApplicationInstance.DUAL_MODE)
                    )
                    && StoreHelper.StoreHasSpecialProformaTypeAndSeries(StoreControllerAppiSettings.CurrentStore)
                   )
                {
                    salesDocuments.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = Resources.SpecialProformaInvoices,
                        Action = "Proforma",
                        Controller = "Document",
                        LinkParameters = new { useSpecialProforma = true },
                        Javascript = "",
                        HtmlID = "specialProformaInvoices",
                        HtmlClass = !proformaDocumentTypesExist ? "disabled" : ""
                    });
                }
            }
            megamenu.AddChildren(salesDocuments);

            if (UserHelper.IsCustomer(CurrentUser) == false)
            {
                if (OwnerApplicationSettings != null && OwnerApplicationSettings.EnablePurchases)
                {
                    MenuNode purchaseDocuments = new MenuNode(forbidden, Url)
                    {
                        HtmlID = "purchaseDocuments",
                        Caption = Resources.PurchaseDocuments
                    };

                    ///New Purchase Document
                    purchaseDocuments.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = Resources.NewPurchaseDocument,
                        DirectLink = "javascript:;",
                        DataOid = true,
                        DataMode = eDivision.Purchase,
                        HtmlID = "newPurchaseDocument",
                        HtmlClass = !purchaseDocumentTypesExist ? "disabled order-shortcut" : "order-shortcut"
                    });

                    ///Purchase Documents List
                    purchaseDocuments.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = Resources.PurchaseDocumentList,
                        Action = "Index",
                        Controller = "Document",
                        LinkParameters = new { Mode = "Purchase" },
                        Javascript = "",
                        HtmlID = "listPurchaseDocument",
                        HtmlClass = ""
                    });
                    megamenu.AddChildren(purchaseDocuments);
                }

                MenuNode storeDocuments = new MenuNode(forbidden, Url)
                {
                    HtmlID = "storeDocuments",
                    Caption = Resources.StoreDocuments
                };

                ///New Store Document
                storeDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.NewStoreDocument,
                    DirectLink = "javascript:;",
                    DataOid = true,
                    DataMode = eDivision.Store,
                    HtmlID = "newStoreDocument",
                    HtmlClass = !storeDocumentTypesExist ? "disabled order-shortcut" : "order-shortcut"
                });

                ///Store Document List
                storeDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.StoreDocumentList,
                    DirectLink = "javascript:;",
                    DataOid = true,
                    DataMode = eDivision.Store,
                    HtmlID = "listStoreDocument",
                    HtmlClass = "",
                    Action = "Index",
                    Controller = "Document",
                    LinkParameters = new { Mode = eDivision.Store }
                });

                megamenu.AddChildren(storeDocuments);

                MenuNode financialDocuments = new MenuNode(forbidden, Url)
                {
                    HtmlID = "financialDocuments",
                    Caption = Resources.FinancialDocuments
                };

                ///New Financial Document
                financialDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.NewFinancialDocument,
                    DirectLink = "javascript:;",
                    DataOid = true,
                    DataMode = eDivision.Financial,
                    HtmlID = "newFinancialDocument",
                    HtmlClass = !financialDocumentTypesExist ? "disabled order-shortcut" : "order-shortcut"
                });

                ///Store Document List
                financialDocuments.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.FinancialDocumentList,
                    DirectLink = "javascript:;",
                    DataOid = true,
                    DataMode = eDivision.Financial,
                    HtmlID = "listFinancialDocument",
                    HtmlClass = "",
                    Action = "Index",
                    Controller = "Document",
                    LinkParameters = new { Mode = eDivision.Financial }
                });

                megamenu.AddChildren(financialDocuments);
            }

            MenuNode items = new MenuNode(forbidden, Url) { Caption = Resources.MenuItems, HtmlID = "menuItem" };
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemList, Action = "Index", Controller = "Item", Javascript = "", HtmlID = "menuItemList", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.AlternativeCodes, Action = "Index", Controller = "ItemBarcode", Javascript = "", HtmlID = "menuAlternativeCodes", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.ItemCategories, Action = "Index", Controller = "ItemCategory", Javascript = "", HtmlID = "menuItemCategories", HtmlClass = "" });
            //items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemPriceCatalogs, Action = "Index", Controller = "PriceCatalog", Javascript = "", HtmlID = "menuPriceCatalog", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemBuyer, Action = "Index", Controller = "Buyer", Javascript = "", HtmlID = "menuBuyer", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemSeasonality, Action = "Index", Controller = "Seasonality", Javascript = "", HtmlID = "menuSeasonality", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemOffer, Action = "Index", Controller = "Offer", Javascript = "", HtmlID = "menuOffer", HtmlClass = "" });
            items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.InformationSheets, Action = "Index", Controller = "InformationSheet", Javascript = "", HtmlID = "menuInformationSheets", HtmlClass = "" });
            if (UserHelper.IsAdmin(CurrentUser))
            {
                items.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.ItemStock, Action = "Index", Controller = "ItemStock", Javascript = "", HtmlID = "menuItemStock", HtmlClass = "" });
            }
            megamenu.AddChildren(items);

            MenuNode priceCatalogPolicyMenu = new MenuNode(forbidden, Url) { Caption = Resources.PriceCatalogPolicies, HtmlID = "menuPriceCatalogPolicy" };
            priceCatalogPolicyMenu.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.PriceCatalogPolicies, Action = "Index", Controller = "PriceCatalogPolicy", Javascript = "", HtmlID = "menuPriceCatalogPoliciesList", HtmlClass = "" });
            priceCatalogPolicyMenu.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuItemPriceCatalogs, Action = "Index", Controller = "PriceCatalog", Javascript = "", HtmlID = "menuPriceCatalog", HtmlClass = "" });
            priceCatalogPolicyMenu.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = Resources.PriceCheck,
                Action = "Index",
                Controller = "PriceCheck",
                Javascript = "",
                HtmlID = "menuPriceCheck",
                HtmlClass = ""
            });
            megamenu.AddChildren(priceCatalogPolicyMenu);

            MenuNode coupons = new MenuNode(forbidden, Url) { Caption = Resources.Coupons, HtmlID = "menuCoupon" };
            coupons.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.Coupons, Action = "Index", Controller = "Coupon", Javascript = "", HtmlID = "menuCouponList", HtmlClass = "" });
            coupons.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.CouponsMasks, Action = "Index", Controller = "CouponMask", Javascript = "", HtmlID = "menuCopounMasksList", HtmlClass = "" });
            coupons.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.CouponCategories, Action = "Index", Controller = "CouponCategory", Javascript = "", HtmlID = "menuCopounCategoryList", HtmlClass = "" });

            megamenu.AddChildren(coupons);

            MenuNode promotions = new MenuNode(forbidden, Url) { Caption = Resources.Campaigns, HtmlID = "menuCampaigns" };
            promotions.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.Promotions, Action = "Index", Controller = "Promotion", Javascript = "", HtmlID = "menuPromotions", HtmlClass = "" });
            promotions.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.Leaflets, Action = "Index", Controller = "Leaflet", Javascript = "", HtmlID = "menuLeaflets", HtmlClass = "" });
            megamenu.AddChildren(promotions);

            MenuNode traders = new MenuNode(forbidden, Url) { Caption = Resources.MenuTraders, HtmlID = "menuTraders" };
            traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuCustomers, Action = "Index", Controller = "Customer", Javascript = "", HtmlID = "menuCustomer", HtmlClass = "" });
            traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuSuplliers, Action = "Index", Controller = "Supplier", Javascript = "", HtmlID = "menuSupplier", HtmlClass = "" });
            traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.Company, Action = "Index", Controller = "Company", Javascript = "", HtmlID = "menuCompany", HtmlClass = "" });
            traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.CustomerCategories, Action = "Index", Controller = "CustomerCategory", Javascript = "", HtmlID = "menuCustomerCategories", HtmlClass = "" });
            if (MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER &&
              (UserHelper.IsSystemAdmin(CurrentUser) || UserHelper.IsCompanyAdmin(CurrentUser)))
            {
                traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.CreateCustomerPointsDocument, Action = "CreateTransactionPoints", Controller = "Customer", Javascript = "", HtmlID = "menuCreateTransactionPoints", HtmlClass = "" });
            }
            traders.AddChildren(new MenuNode(forbidden, Url) { Caption = Resources.MenuAdministrationUsers, Action = "Index", Controller = "User", Javascript = "", HtmlID = "menuAdministationUsers", HtmlClass = "" });
            megamenu.AddChildren(traders);


            MenuNode reports = new MenuNode(forbidden, Url) { Caption = Resources.Reports, HtmlID = "menuReports" };

            List<ReportCategory> reportCategories = ReportsHelper.GetVisibleReportCategories(CurrentUser, 7);

            string categoryRequest = Request["category"] ?? "";
            if (reportCategories.Count() > 0)
            {
                foreach (ReportCategory reportCategory in (reportCategories as List<ReportCategory>))
                {
                    reports.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = reportCategory.Description,
                        DirectLink = Url.Action("Categories", "Reports", new { category = reportCategory.Oid.ToString() }),
                        Javascript = "",
                        HtmlID = "menuReportCategory" + reportCategory.Oid.ToString(),
                        HtmlClass = ""
                    });

                }
            }

            reports.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = Resources.AllReports,
                Action = "Categories",
                Controller = "Reports",
                Javascript = "",
                HtmlID = "menuAllReports",
                HtmlClass = ""
            });

            if (ApplicationHelper.IsMasterInstance())
            {
                reports.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.ReportCategories,
                    Action = "Categories",
                    Controller = "CustomReport",
                    Javascript = "",
                    HtmlID = "menuCustomReportCategories",
                    HtmlClass = ""
                });
            }

            megamenu.AddChildren(reports);
            MenuNode storeFunctions = new MenuNode(forbidden, Url)
            {
                Caption = Resources.StoreFunctions,
                HtmlID = "menuStoreFunctions"
            };
            if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                storeFunctions.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.StoreDailyReports,
                    Action = "Index",
                    Controller = "StoreDailyReport",
                    Javascript = "",
                    HtmlID = "menuStoreDailyReport",
                    HtmlClass = ""
                });
            }
            megamenu.AddChildren(storeFunctions);

            MenuNode tools = new MenuNode(forbidden, Url)
            {
                Caption = Resources.Tools,
                HtmlID = "menuTools"
            };

            if (MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL)
            {
                if (CurrentUser != null && UserHelper.IsAdmin(CurrentUser))
                {
                    tools.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = Resources.LicenseManagement,
                        Action = "Index",
                        Controller = "LicenseManagement",
                        Javascript = "",
                        HtmlID = "menuLicenseManagement",
                        HtmlClass = ""
                    });
                }
            }
            else if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.Labels,
                    Action = "Index",
                    Controller = "Labels",
                    Javascript = "",
                    HtmlID = "menuLabels",
                    HtmlClass = ""
                });

                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.PosStatus,
                    Action = "POSStatus",
                    Controller = "POS",
                    Javascript = "",
                    HtmlID = "menuPosStatus",
                    HtmlClass = ""
                });

                if (UserHelper.IsSystemAdmin(CurrentUser))
                {
                    tools.AddChildren(new MenuNode(forbidden, Url)
                    {
                        Caption = Resources.DatabaseOperations,
                        Action = "Index",
                        Controller = "DbOperations",
                        Javascript = "",
                        HtmlID = "menuDbOperations",
                        HtmlClass = ""
                    });
                }

                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.SpreadSheets,
                    Action = "Index",
                    Controller = "SpreadSheet",
                    Javascript = "",
                    HtmlID = "menuSpreadSheets",
                    HtmlClass = ""
                });

                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.CashierRegisters,
                    Action = "Index",
                    Controller = "CashierRegister",
                    Javascript = "",
                    HtmlID = "menuCashierRegister",
                    HtmlClass = ""
                });
            }
            else
            {
                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.SpreadSheets,
                    Action = "Index",
                    Controller = "SpreadSheet",
                    Javascript = "",
                    HtmlID = "spreadSheets",
                    HtmlClass = ""
                });

                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.StoreControllerStatus,
                    Action = "Index",
                    Controller = "StoreControllerStatus",
                    Javascript = "",
                    HtmlID = "menuStoreControllerStatus",
                    HtmlClass = ""
                });
            }

            tools.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = Resources.ElectronicJournalFilePackages,
                HtmlID = "electronicJournalFilePackages",
                Action = "Index",
                Controller = "ElectronicJournalFilePackage",
                HtmlClass = ""
            });

            tools.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = Resources.SynchronizationInfo,
                Action = "Index",
                HtmlID = "synchronizationInfo",
                Controller = "SynchronizationInfo",
                HtmlClass = ""
            });

            tools.AddChildren(new MenuNode(forbidden, Url)
            {
                Caption = ResourcesLib.Resources.CreateSFADatabase,
                Action = "Index",
                HtmlID = "SFA",
                Controller = "SFA",
                HtmlClass = ""
            });
            //tools.AddChildren(new MenuNode(forbidden, Url)
            //{
            //    Caption = Resources.PriceCheck,
            //    Action = "Index",
            //    Controller = "PriceCheck",
            //    Javascript = "",
            //    HtmlID = "menuPriceCheck",
            //    HtmlClass = ""
            //});

            if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.MobPriceCheck,
                    Action = "Index",
                    HtmlID = "synchronizationInfo",
                    Controller = "ItemCheck",
                    HtmlClass = ""
                });
                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.MobInventory,
                    Action = "Index",
                    HtmlID = "synchronizationInfo",
                    Controller = "Inventory",
                    HtmlClass = ""
                });
                tools.AddChildren(new MenuNode(forbidden, Url)
                {
                    Caption = Resources.MobEslInventory,
                    Action = "Index",
                    HtmlID = "synchronizationInfo",
                    Controller = "EslInventory",
                    HtmlClass = ""
                });
            }
            megamenu.AddChildren(tools);
            menuInfo = new MenuInfo();
            menuInfo.Menu = megamenu;
            Session["Menu"] = menuInfo;
        }




        protected void ForceCookieExpire(string cookie)
        {

            if (Request.Cookies[cookie] != null)
            {
                HttpCookie myCookie = new HttpCookie(cookie);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }

            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-60));

        }

        protected void ExpireAllCookies()
        {
            foreach (string cn in Request.Cookies.AllKeys)
            {
                if (cn != "_culture" && cn != "ASP.NET_SessionId" && cn != "RetailVersion" && cn != "_theme")
                {
                    ForceCookieExpire(cn);
                }
            }
        }

        //[AjaxOrChildActionOnly, ActionLog(LogLevel = LogLevel.None)]
        //public JsonResult UserConnected()
        //{
        //    TimeSpan timespan = new TimeSpan(DateTime.Now.Ticks - MvcApplication.lastGarbageCollection.Ticks);

        //    if (timespan.Seconds > 3600 + new Random().Next() % 300)
        //    {
        //        try
        //        {
        //            MvcApplication.lastGarbageCollection = DateTime.Now;
        //            GC.Collect();
        //        }
        //        catch (Exception ex)
        //        {
        //            MvcApplication.Log.Error(ex, "GarbageCollection:UserConnected");
        //        }

        //    }
        //    string ip = Session.SessionID;

        //    if (MvcApplication.ConnectedtUsers.ContainsKey(ip))
        //    {
        //        MvcApplication.ConnectedtUsers[ip] = DateTime.Now;
        //    }
        //    else
        //    {
        //        MvcApplication.ConnectedtUsers.TryAdd(ip, DateTime.Now);
        //    }

        //    int connected = MvcApplication.ConnectedtUsers.Where(c => c.Value.AddSeconds(30d) > DateTime.Now).Count();

        //    foreach (string key in MvcApplication.ConnectedtUsers.Where(c => c.Value.AddSeconds(30d) < DateTime.Now).Select(c => c.Key))
        //    {
        //        DateTime tmp;
        //        MvcApplication.ConnectedtUsers.TryRemove(key, out tmp);
        //    }
        //    List<string> userNames;

        //    if (UserHelper.IsSystemAdmin(CurrentUser))
        //    {
        //        IEnumerable<Guid> mvcApplicationUsersOnlineSelect = MvcApplication.UsersOnline.Select(g => g.Key);
        //        CriteriaOperator crop = new InOperator("Oid", mvcApplicationUsersOnlineSelect);
        //        XPCollection<User> usersOnline = new XPCollection<User>(XpoSession, crop);
        //        userNames = usersOnline.Select(g => g.UserName).ToList();

        //        if (userNames.Count == 0)
        //        {
        //            userNames.Add("None");
        //        }
        //    }
        //    else
        //    {
        //        userNames = new List<string>();
        //        userNames.Add("");
        //    }

        //    string applicationInstance = "Retail";
        //    if (ApplicationHelper.IsStoreControllerInstance())//#if _RETAIL_STORECONTROLLER
        //    {
        //        applicationInstance = "Store Controller";
        //    }
        //    else if (ApplicationHelper.IsDualInstance())//#elif _RETAIL_DUAL
        //    {
        //        applicationInstance = "Dual Mode";
        //    }//#endif


        //    return Json(new { count = connected, applicationInstance = applicationInstance, userNames = userNames.Aggregate((f, s) => f + ", " + s) }, JsonRequestBehavior.AllowGet);
        //}


        protected bool performLog = true;

        [Security(ReturnsPartial = false, DontLogAction = true)]
        [AllowAnonymous]
        public FileContentResult ShowOwnerImage()
        {
            OwnerImage im = null;


            if (im == null && (OwnerApplicationSettings != null && OwnerApplicationSettings.OwnerImageOid != Guid.Empty))
            {
                im = OwnerApplicationSettings.Session.GetObjectByKey<OwnerImage>(OwnerApplicationSettings.OwnerImageOid);
            }

            if (im != null)
            {
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(im.Image, typeof(byte[]));
                string format = "";

                if (im.Image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    format = "jpeg";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Gif)))
                {
                    format = "gif";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Png)))
                {
                    format = "png";
                }

                return new FileContentResult(imageBytes, "image/" + format);
            }
            else
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/wrm_light.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }
        }

        protected virtual void InitiateViewBags(ActionExecutingContext filterContext)
        {
            using (MiniProfiler.Current.Step("InitiateViewBags()"))
            {
                ViewBag.ExtraCSSfiles = new List<string>();
                ViewBag.ExtraJSfiles = new List<string>();
#if DEBUG
                if (UserHelper.IsSystemAdmin(CurrentUser))
                {
                    ViewBag.DebugFooterInfo = XpoHelper.database + "@" + XpoHelper.sqlserver;
                }
#endif
                applicationVersion = Assembly.GetAssembly(this.GetType()).GetName().Version.ToString();
                ViewData["ApplicationVersionNumber"] = applicationVersion;

                if ((filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Base" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Notification")
                    && filterContext.IsChildAction == false)
                {
                    Session["ControllerName"] = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    Session["ActionName"] = (filterContext.ActionDescriptor as ReflectedActionDescriptor).MethodInfo.Name;
                }

                ViewBag.ControllerName = Session["ControllerName"];
                ViewBag.ActionName = Session["ActionName"];
                ViewBag.Owner = CurrentOwner;


                this.DialogOptions = new DialogOptions();
                ViewData["DialogOptions"] = this.DialogOptions;
                ViewBag.ShowSettings = false;
            }
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (MiniProfiler.Current.Step("BaseController.OnActionExecuting"))
            {
                if (CurrentUser != null && (Session["UserName"] == null || CurrentUser.UserName != Session["UserName"].ToString()))
                {
                    PrepareLoggedInUserVariables();
                }

                InitiateViewBags(filterContext);

                TrackUsers();
                CookieManager();
                if (this.ControllerContext.IsChildAction == false && this.Request.IsAjaxRequest() == false)
                {
                    CreateMenu();
                }
                this.CustomJSProperties.AddJSProperty("ITScontroller", Session["ControllerName"]);
                this.CustomJSProperties.AddJSProperty("action", Session["ActionName"]);
                this.CustomJSProperties.AddJSProperty("QUANTITY_MULTIPLIER", DocumentHelper.QUANTITY_MULTIPLIER);
                ViewData["CustomJSProperies"] = CustomJSProperties;
                SetFormsMessages(filterContext);

                ViewBag.CurrentUser = this.CurrentUser;
            }
            base.OnActionExecuting(filterContext);
        }

        private void CookieManager()
        {
            using (MiniProfiler.Current.Step("CookieManager()"))
            {
                HttpCookie versionCookie = Request.Cookies["RetailVersion"];
                if (versionCookie == null || versionCookie.Value != applicationVersion)
                {
                    ExpireAllCookies();
                }
                if (Request["PerformTest"] != null)
                {
                    HttpCookie testCookie = Request.Cookies["testCookie"];
                    if (testCookie == null)
                        testCookie = new HttpCookie("testCookie", "testCookie");
                    Response.Cookies.Add(testCookie);
                }
                if (versionCookie == null)
                {
                    versionCookie = new HttpCookie("RetailVersion", applicationVersion);
                }
                versionCookie.Value = applicationVersion;
                versionCookie.Expires = DateTime.Now.AddDays(999d);
                Response.Cookies.Add(versionCookie);

                if (Request.UserLanguages != null)
                {
                    string cultureName = null;
                    // Attempt to read the culture cookie from Request
                    HttpCookie cultureCookie = Request.Cookies["_culture"];
                    if (cultureCookie != null)
                    {
                        cultureName = cultureCookie.Value;
                    }
                    else
                    {
                        cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
                    }

                    // Validate culture name
                    cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                    // Modify current thread's cultures
                    System.Globalization.CultureInfo previousCultureInfo = (System.Globalization.CultureInfo)Session["cultureInfo"];
                    System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(cultureName);

                    Session["cultureInfo"] = cultureInfo;
                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
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

                    if (previousCultureInfo == null || previousCultureInfo.Name != cultureInfo.Name)
                    {
                        Session["Menu"] = null;
                        CreateMenu();
                    }
                    cultureCookie.Value = cultureName;
                    Response.Cookies.Add(cultureCookie);
                }
            }
        }

        private void TrackUsers()
        {
            if (CurrentUser != null)
            {
                //Keep track of logged in users
                Guid userOid = CurrentUser.Oid;
                if (MvcApplication.UsersOnline.ContainsKey(userOid))
                {
                    MvcApplication.UsersOnline[userOid] = DateTime.Now;
                }
                else
                {
                    MvcApplication.UsersOnline.TryAdd(userOid, DateTime.Now);
                }

                // Drop old sessions
                List<Guid> listToRemove = MvcApplication.UsersOnline.Where(c => c.Value.AddMinutes(30d) < DateTime.Now).Select(c => c.Key).ToList();

                foreach (Guid key in listToRemove)
                {
                    DateTime tmp;
                    MvcApplication.UsersOnline.TryRemove(key, out tmp);
                }
            }
        }



        protected void SetFormsMessages(ActionExecutingContext filterContext)
        {
            String controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator filter;
                if (CurrentOwner == null)
                {
                    filter = CriteriaOperator.And(
                                new ContainsOperator("ControllerMessages", new BinaryOperator("Description", controllerName)),
                                new NullOperator("Owner")
                            );
                }
                else
                {
                    filter = CriteriaOperator.And(
                            new ContainsOperator("ControllerMessages", new BinaryOperator("Description", controllerName)),
                            CriteriaOperator.Or(
                                new BinaryOperator("Owner.Oid", CurrentOwner.Oid),
                                new NullOperator("Owner")
                            )
                        );
                }
                XPCollection<FormMessage> messages = GetList<FormMessage>(uow, filter);

                if (messages.Count > 0)
                {
                    if (messages.Where(g => g.MessagePlace == eMessageType.Header).Count() > 0)
                    {
                        var validMessages = messages.Where(g => g.MessagePlace == eMessageType.Header && !String.IsNullOrWhiteSpace(g.ToString()));
                        if (validMessages.Count() > 0)
                        {
                            ViewBag.HeaderMessage = validMessages.Select(g => "<div class=\"form_message\">" + g.ToString() + "</div>").Aggregate((First, Second) => First + Second);
                        }
                    }
                    if (messages.Where(g => g.MessagePlace == eMessageType.Body).Count() > 0)
                    {
                        var validMessages = messages.Where(g => g.MessagePlace == eMessageType.Body && !String.IsNullOrWhiteSpace(g.ToString()));
                        if (validMessages.Count() > 0)
                        {
                            ViewBag.BodyMessage = validMessages.Select(g => "<div class=\"form_message\">" + g.ToString() + "</div>").Aggregate((First, Second) => First + Second);
                        }
                    }
                    if (messages.Where(g => g.MessagePlace == eMessageType.Footer).Count() > 0)
                    {
                        var validMessages = messages.Where(g => g.MessagePlace == eMessageType.Footer && !String.IsNullOrWhiteSpace(g.ToString()));
                        if (validMessages.Count() > 0)
                        {
                            ViewBag.FooterMessage = validMessages.Select(g => "<div class=\"form_message\">" + g.ToString() + "</div>").Aggregate((First, Second) => First + Second);
                        }
                    }
                    if (ViewBag.HeaderMessage != null && String.IsNullOrEmpty(ViewBag.HeaderMessage.ToString().Trim()))
                        ViewBag.HeaderMessage = null;
                    if (ViewBag.BodyMessage != null && String.IsNullOrEmpty(ViewBag.BodyMessage.ToString().Trim()))
                        ViewBag.BodyMessage = null;
                    if (ViewBag.FooterMessage != null && String.IsNullOrEmpty(ViewBag.FooterMessage.ToString().Trim()))
                        ViewBag.FooterMessage = null;

                }
            }
        }

        [AjaxOrChildActionOnly]
        public virtual ActionResult Dialog(List<string> arguments)
        {
            return PartialView();
        }

        [AjaxOrChildActionOnly]
        public virtual ActionResult DialogCallbackPanel()
        {
            this.DialogOptions.RenderDialog = true;
            string[] arguments = Request["DXCallbackArgument"].Split(':');
            this.DialogOptions.Arguments = arguments.Length > 1 ? arguments[1].Split(',').ToList() : null;

            return PartialView();
        }

        [AjaxOrChildActionOnly]
        public virtual ActionResult DialogCallbackPanelSecondary()
        {
            this.DialogOptions.RenderDialog = true;
            this.DialogOptions.Arguments = Request["DXCallbackArgument"].Split(':').Length > 1 ? Request["DXCallbackArgument"].Split(':').Skip(1).ToList() : null;

            return PartialView();
        }

        protected Wizard GetWizardByID(Guid id)
        {
            return this.ActiveWizards.FirstOrDefault(x => x.ID == id);
        }

        [AjaxOrChildActionOnly]
        public ActionResult WizardCallbackPanel()
        {
            WizardArguments args = new WizardArguments();
            args.RenderWizard = true;
            args.Arguments = Request["DXCallbackArgument"].Split(':').Length >= 2 ? Request["DXCallbackArgument"].Split(':').Skip(1).ToList() : null;
            ViewData["WizardArguments"] = args;
            return PartialView();
        }

        [AjaxOrChildActionOnly]
        public ActionResult WizardPopup(List<string> arguments)
        {
            Wizard newWizard = CreateWizard(arguments);
            ActiveWizards.Add(newWizard);
            ViewData["Wizard"] = newWizard;
            return PartialView();
        }

        [HttpPost]
        public ActionResult WizardStepForm(string ID, eWizardAction? StepCommand)
        {
            Guid id;
            if (Guid.TryParse(ID, out id))
            {
                Wizard wizard = GetWizardByID(id);
                if (wizard != null)
                {
                    ViewData["Wizard"] = wizard;
                    if (StepCommand != null)
                    {
                        WizardActionEventArgs args = new WizardActionEventArgs() { Wizard = wizard, WizardAction = StepCommand.Value };
                        WizardActionExecuting(args);

                        if (args.CancelAction == false)
                        {
                            switch (StepCommand.Value)
                            {
                                case eWizardAction.NEXT:
                                    wizard.MoveNext();
                                    break;
                                case eWizardAction.BACK:
                                    wizard.GoBack();
                                    break;
                                case eWizardAction.CANCEL:
                                    this.ActiveWizards.Remove(wizard);
                                    break;
                                case eWizardAction.FINISH:
                                    this.ActiveWizards.Remove(wizard);
                                    break;
                            }
                        }
                        ViewData["WizardStepArgs"] = args;
                    }

                    ViewData["WizardStep"] = wizard.CurrentStep;

                }

            }

            return PartialView();
        }

        protected virtual Wizard CreateWizard(List<string> arguments)
        {
            return new Wizard(null, null, arguments);
        }

        /// <summary>
        /// Runs before each wizard action is executed and can cancel it if desired
        /// </summary>
        /// <param name="wizard"></param>
        /// <returns></returns>
        protected virtual void WizardActionExecuting(WizardActionEventArgs args)
        {

        }

        public virtual ActionResult PopupViewCallbackPanel()
        {
            Guid ID = Guid.Empty;

            if (Guid.TryParse(Request["DXCallbackArgument"].Split(':').Last(), out ID))
            {
                ViewData["ID"] = ID;
            }

            return PartialView();
        }

        public virtual ActionResult LoadViewPopup()
        {
            Guid ID = Guid.Empty;

            if (Request["DXCallbackArgument"] != null && Guid.TryParse(Request["DXCallbackArgument"].Split(':').Last(), out ID))
            {
                ViewData["ID"] = ID;
            }
            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }

        public virtual ActionResult PopupEditCallbackPanel()
        {
            Guid ID = Guid.Empty;

            if (Guid.TryParse(Request["ID"], out ID))
            {
                ViewData["ID"] = ID;
            }

            return PartialView();
        }

        public virtual ActionResult PopupAddCallbackPanel()
        {
            Guid ID = Guid.Empty;

            if (Guid.TryParse(Request["ID"], out ID))
            {
                ViewData["ID"] = ID;
            }

            return PartialView();
        }


        public virtual ActionResult LoadEditPopup()
        {
            Guid ID = Guid.Empty;

            if (Guid.TryParse(Request["ID"], out ID))
            {
                ViewData["ID"] = ID;
            }
            return PartialView("LoadEditPopup");
        }

        public virtual ActionResult LoadAddPopup()
        {
            Guid ID = Guid.Empty;

            if (Guid.TryParse(Request["ID"], out ID))
            {
                ViewData["ID"] = ID;
            }
            ActionResult rt = PartialView("LoadAddPopup");
            return rt;
        }

        [AllowAnonymous]
        public ActionResult TermsOfUse()
        {
            return View();
        }

        [AjaxOrChildActionOnly]
        public ActionResult OwnersComboBoxPartial()
        {
            return PartialView();
        }

        public static object OwnersRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            CriteriaOperator visibleSuppliers = new AggregateOperand("Stores", Aggregate.Exists);
            CriteriaOperator criteria = String.IsNullOrEmpty(e.Filter) || String.IsNullOrWhiteSpace(e.Filter)
                                        ? visibleSuppliers
                                        : CriteriaOperator.And(visibleSuppliers,
                                                               CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), e.Filter),
                                                                                   new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), e.Filter)
                                                                                 //new BinaryOperator("CompanyName", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                 //new BinaryOperator("Trader.TaxCode", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                 )
                                                               );
            XPCollection<CompanyNew> collection = GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), criteria, "CompanyName");

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static IEnumerable GetObjectByValue(Type type, object value)
        {
            Guid gd;
            if (value == null || Guid.TryParse(value.ToString(), out gd) == false)
            {
                return null;
            }
            return new[] { XpoHelper.GetNewUnitOfWork().GetObjectByKey(type, gd) };
        }

        public static List<W> GetObjectByValue<W>(object value) where W : BaseObj
        {
            Guid gd;
            if (value == null || Guid.TryParse(value.ToString(), out gd) == false)
            {
                return null;
            }
            return new List<W>() { XpoHelper.GetNewUnitOfWork().GetObjectByKey<W>(gd) };
        }

        public static object ObjectRequestedByValue<W>(DevExpress.Web.ListEditItemRequestedByValueEventArgs e) where W : BaseObj
        {
            Guid id;
            if (e.Value != null && Guid.TryParse(e.Value.ToString(), out id))
            {
                return XpoHelper.GetNewUnitOfWork().GetObjectByKey<W>(e.Value);
            }
            return null;
        }

        public static object GetOwnerByValue(object value)
        {
            return GetObjectByValue<SupplierNew>(value);
        }

        protected void UpdateCurrentUserSettings()
        {
            User currentuser = CurrentUser;
            Session["StoreName"] = this.CurrentStore != null ? this.CurrentStore.Name : null;
            Session["TraderCompanyName"] = null;
            Session["UserOneStore"] = null;

            if (ApplicationHelper.IsMasterInstance() && Session["IsAdministrator"] != null && (bool)Session["IsAdministrator"])
            {
                XPCollection<CompanyNew> companies = GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork());
                Session["Companies"] = companies;
                if (companies.Count == 1)
                {
                    Session["currentOwner"] = companies.First();
                    Session["TraderCompanyName"] = companies.First().CompanyName;
                    Session["OwnerSettings"] = companies.First().OwnerApplicationSettings;
                }
            }

            StoresThatCurrentUserOwns = new List<StoreViewModel>();
            IEnumerable<Store> stores;
            if ((bool)Session["IsAdministrator"] && EffectiveOwner != null)
            {
                stores = GetList<Store>(currentuser.Session).ToList();
            }
            else if ((bool)Session["IsAdministrator"])
            {
                stores = new List<Store>();
            }
            else
            {
                stores = UserHelper.GetStoresThatUserOwns(currentuser).ToList();
            }
            foreach (Store store in stores)
            {
                StoreViewModel svm = new StoreViewModel();
                svm.LoadPersistent(store);
                StoresThatCurrentUserOwns.Add(svm);
            }
            List<Store> userbuystore = UserHelper.GetStoresWhereUserBuysFrom(currentuser).ToList();
            List<StoreViewModel> userbuystorevms = new List<StoreViewModel>(userbuystore.Count);
            foreach (Store store in userbuystore)
            {
                StoreViewModel svm = new StoreViewModel();
                svm.LoadPersistent(store);
                userbuystorevms.Add(svm);
            }
            Session["StoresThatCurrentUserBuysFrom"] = userbuystorevms;

            List<StoreViewModel> visible_stores = new List<StoreViewModel>();
            if (StoresThatCurrentUserOwns != null)
            {
                visible_stores = StoresThatCurrentUserOwns;

            }
            if (Session["StoresThatCurrentUserBuysFrom"] != null)
            {
                if (visible_stores.Count == 0)
                {
                    visible_stores = Session["StoresThatCurrentUserBuysFrom"] as List<StoreViewModel>;
                }
                else
                {
                    visible_stores.AddRange((List<StoreViewModel>)Session["StoresThatCurrentUserBuysFrom"]);
                }
            }

            this.CurrentStore = null;
            Session["StoreName"] = null;
            if (ApplicationHelper.IsMasterInstance())
            {
                Session["Application"] = "Retail";
                Session["UserOneStore"] = (visible_stores.Count == 1) && UserOwner != null;
                if ((bool)Session["UserOneStore"])
                {
                    this.CurrentStore = visible_stores.First();
                    Session["StoreName"] = this.CurrentStore.Name;
                }
            }
            else
            {
                if (ApplicationHelper.IsStoreControllerInstance())
                {
                    Session["Application"] = "StoreController";
                }
                else if (ApplicationHelper.IsDualInstance())
                {
                    Session["Application"] = "Dual Mode";
                }

                Session["UserOneStore"] = true;
                if ((bool)Session["UserOneStore"])
                {
                    if (StoreControllerAppiSettings.CurrentStore == null)
                    {
                        Session.Clear();
                        Session["Error"] = Resources.StoreNotFound + " ID: " + StoreControllerAppiSettings.CurrentStoreOid;
                        return;
                    }
                    Store store = StoreControllerAppiSettings.CurrentStore;
                    StoreViewModel svm = new StoreViewModel();
                    svm.LoadPersistent(store);
                    this.CurrentStore = svm;
                    Session["StoreName"] = svm.Name;
                    Session["TraderCompanyName"] = store.Owner.CompanyName;
                }
            }
            if (Session["TraderCompanyName"] == null && EffectiveOwner != null)
            {
                if (UserOwner != null)
                {
                    Session["userOwner"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>(UserOwner.Oid);
                }
                if (EffectiveOwner != null)
                {
                    Session["currentOwner"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                }
                Session["TraderCompanyName"] = EffectiveOwner.CompanyName;
            }
        }

        protected static XPCollection<W> GetList<W>(Session uow, CriteriaOperator filter = null, string sortingField = "Oid", PersistentCriteriaEvaluationBehavior behavior = PersistentCriteriaEvaluationBehavior.BeforeTransaction, SortingDirection direction = SortingDirection.Ascending)
        {
            SortProperty sortprop = new SortProperty(sortingField, direction);
            XPCollection<W> col = new XPCollection<W>(behavior, uow, ApplyOwnerCriteria(filter, typeof(W)));
            col.Sorting = new SortingCollection(sortprop);
            col.DeleteObjectOnRemove = true;
            return col;
        }

        protected static W FindObjectWithOwner<W>(Session uow, CriteriaOperator filter = null, PersistentCriteriaEvaluationBehavior behavior = PersistentCriteriaEvaluationBehavior.BeforeTransaction)
        {
            W obj = ((UnitOfWork)uow).FindObject<W>(behavior, ApplyOwnerCriteria(filter, typeof(W)));
            return obj;
        }

        public static CriteriaOperator ApplyOwnerCriteria(CriteriaOperator inputCriteria, Type type, CompanyNew owner = null)
        {
            if (owner == null)
            {
                owner = EffectiveOwner;
            }
            return RetailHelper.ApplyOwnerCriteria(inputCriteria, type, owner);
        }

        public ActionResult ReturnView(string ViewName, bool IsPartial)
        {
            return (IsPartial) ? PartialView(ViewName) : (ActionResult)View(ViewName);
        }

        [AjaxOrChildActionOnly]
        public virtual ActionResult GenericViewPopupCallbackPanel()
        {

            List<string> arguments = Request["DXCallbackArgument"].Split(':').Length >= 2 ? Request["DXCallbackArgument"].Split(':').Skip(1).ToList() : null;

            if (arguments != null && arguments.Count == 2)
            {
                ViewData["RenderView"] = true;
                ViewData["EntityOid"] = arguments[0];
                ViewData["EntityType"] = arguments[1];
            }
            return PartialView();
        }

        public static ItemRequestedByValueMethod RequestByValue(Type type)
        {
            return (DevExpress.Web.ListEditItemRequestedByValueEventArgs e) =>
                {
                    if (e.Value != null)
                    {
                        Guid value;
                        if (Guid.TryParse(e.Value.ToString(), out value))
                        {
                            return XpoHelper.GetNewUnitOfWork().GetObjectByKey(type, e.Value);
                        }
                    }
                    return null;
                };
        }

        [Obsolete("Use ITS.Retail.WebClient.Helpers.DynamicComboBoxDataSourceBuilder instead. To be removed")]
        public static ItemsRequestedByFilterConditionMethod RequestedByFilterCondition(Type type, IEnumerable<string> fields, SortProperty sort)
        {
            return (DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e) =>
            {
                if (e.Filter == "")
                {
                    return null;
                }
                string proccessed_filter = e.Filter.Replace("*", "%");
                if (!proccessed_filter.Contains("%"))
                {
                    proccessed_filter = String.Format("%{0}%", proccessed_filter);
                }
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Owner.Oid", EffectiveOwner.Oid),
                                        CriteriaOperator.Or(fields.Select(field => new BinaryOperator(field, proccessed_filter, BinaryOperatorType.Like))));
                XPCollection searched_item = new XPCollection(uow, type, crop, sort);
                searched_item.SkipReturnedObjects = e.BeginIndex;
                searched_item.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
                return searched_item;
            };
        }

        protected virtual GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = new GenericViewRuleset();
            return ruleset;
        }

        [AjaxOrChildActionOnly]
        public ActionResult GenericViewPartial(string entityOid, string entityType)
        {
            Type convertedEntityType = typeof(BasicObj).Assembly.GetType(typeof(BasicObj).FullName.Replace("BasicObj", entityType));
            GenericViewRuleset ruleset = GenerateGenericViewRuleset();
            GenericViewModelMaster model = GenericViewHelper.CreateGenericViewModel(entityOid, convertedEntityType, ruleset);
            return PartialView(model);
        }

        [AjaxOrChildActionOnly]
        public ActionResult GenericViewTabPartial(string DetailedPropertyName, string MasterObjKey, string MasterObjType)
        {
            Type convertedEntityType = typeof(BasicObj).Assembly.GetType(typeof(BasicObj).FullName.Replace("BasicObj", MasterObjType));
            GenericViewRuleset ruleset = GenerateGenericViewRuleset();
            GenericViewModelMaster master = GenericViewHelper.CreateGenericViewModel(MasterObjKey, convertedEntityType, ruleset);
            GenericViewModelDetailedProperty detailedProperty = master.DetailedProperties.FirstOrDefault(x => x.PropertyName == DetailedPropertyName);
            return PartialView(detailedProperty);
        }

        [AjaxOrChildActionOnly]
        public ActionResult GenericViewGridPartial(string DetailPropertyName, string MasterObjKey, string MasterObjType)
        {
            Type entityType = typeof(BasicObj).Assembly.GetType(typeof(BasicObj).FullName.Replace("BasicObj", MasterObjType));
            GenericViewRuleset ruleset = GenerateGenericViewRuleset();
            GenericViewModelMaster master = GenericViewHelper.CreateGenericViewModel(MasterObjKey, entityType, ruleset);
            ViewData["Detail"] = master.Details.Where(x => x.PropertyName == DetailPropertyName).FirstOrDefault();

            XPBaseCollection detailModel = master.Object.GetType().GetProperty(DetailPropertyName).GetValue(master.Object, null) as XPBaseCollection;
            Type collectionObjectType = detailModel.GetObjectClassInfo().ClassType;
            detailModel.Filter = ApplyOwnerCriteria(detailModel.Criteria, collectionObjectType);

            return PartialView(detailModel);
        }

        public ActionResult Menu(string user, bool showCheckBoxes)
        {
            ViewBag.ShowCheckBoxes = showCheckBoxes;
            return PartialView();
        }

#if !DEBUG
        [OutputCache(Duration = 3600, VaryByParam = "HtmlEditor;Chart;Report;DockingPanels;PivotGrid")]
#endif
        public ActionResult DevExpressScripts(bool HtmlEditor, bool Chart, bool Report, bool DockingPanels, bool PivotGrid)
        {
            ViewBag.EnableDevexpressHtmlEditor = HtmlEditor;
            ViewBag.EnableDevexpressChart = Chart;
            ViewBag.EnableDevexpressReports = Report;
            ViewBag.EnableDevexpressDockingPanels = DockingPanels;
            ViewBag.EnableDevexpressPivotGrid = PivotGrid;
            return PartialView();
        }

#if !DEBUG
        [OutputCache(Duration = 3600, VaryByParam = "HtmlEditor;Chart;Report;PivotGrid;Culture")]
#endif
        public ActionResult StylesAndStaticScripts(bool HtmlEditor, bool Chart, bool Report, bool PivotGrid, string Culture)
        {

            ViewBag.EnableDevexpressHtmlEditor = HtmlEditor;
            ViewBag.EnableDevexpressChart = Chart;
            ViewBag.EnableDevexpressReports = Report;
            ViewBag.EnableDevexpressPivotGrid = PivotGrid;
            ViewData["CurrentCulture"] = Culture;
            return PartialView();
        }

        protected void PrepareLoggedInUserVariables()
        {
            using (MiniProfiler.Current.Step("PrepareLoggedInUserVariables"))
            {
                if (CurrentUser.UserName == "superadmin")
                {
                    List<string> forbiddenStringList = new List<string>();
                    Session["IsAdministrator"] = true;
                    StoresThatCurrentUserOwns = new List<StoreViewModel>();

                    foreach (Store store in GetList<Store>(XpoSession))
                    {
                        StoreViewModel svm = new StoreViewModel();
                        svm.LoadPersistent(store);
                        StoresThatCurrentUserOwns.Add(svm);
                    }

                    Session["LayoutForbidden"] = forbiddenStringList;

                    XPCollection<CustomReport> reports = new XPCollection<CustomReport>(XpoSession);
                    Session["ReportsToShowInMenu"] = reports.ToDictionary(g => g.Oid, g => g.Description);

                    XPCollection<CompanyNew> companies = new XPCollection<CompanyNew>(XpoSession);
                    Session["Companies"] = companies;
                    if (companies.Count == 1)
                    {
                        Session["currentOwner"] = companies.First();
                        Session["TraderCompanyName"] = companies.First().CompanyName;
                        Session["OwnerSettings"] = companies.First().OwnerApplicationSettings;
                    }

                }
                else
                {
                    Session["IsAdministrator"] = false;
                    Session["UserOneStore"] = false;
                    User currentuser = XpoSession.GetObjectByKey<User>(CurrentUser.Oid);
                    Session["UserName"] = currentuser.UserName;
                    UnitOfWork wow = XpoHelper.GetNewUnitOfWork();
                    {
                        if (currentuser.Role.RoleEntityAccessPermisions.Count > 0) //Restricted permissions
                        {
                            XPCollection<CustomReport> reports = new XPCollection<CustomReport>(wow, new ContainsOperator("ReportRoles", new BinaryOperator("Role.Oid", currentuser.Role.Oid)));
                            Session["ReportsToShowInMenu"] = reports.ToDictionary(g => g.Oid, g => g.Description);

                            XPCollection<Customer> getUserCustomers = BOApplicationHelper.GetUserEntities<Customer>(wow, currentuser);
                            Customer user_customer = (getUserCustomers.Count == 0) ? null : getUserCustomers.First();
                            XPCollection<CompanyNew> getUserSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(wow, currentuser);
                            CompanyNew user_company = (getUserSuppliers.Count == 0) ? null : getUserSuppliers.First();

                            if (user_customer != null && user_customer.CompanyName != null)
                            {
                                Session["currentCustomer"] = user_customer;
                                Session["TraderCompanyName"] = user_customer.CompanyName;
                                Session["OwnerSettings"] = user_customer.Owner.OwnerApplicationSettings;
                                Session["userOwner"] = Session["currentOwner"] = user_customer.Owner;
                            }

                            else if (user_company != null && user_company.CompanyName != null)
                            {
                                Session["currentUserSupplier"] = user_company;
                                Session["TraderCompanyName"] = user_company.CompanyName;
                                Session["OwnerSettings"] = user_company.OwnerApplicationSettings;
                                Session["userOwner"] = Session["currentOwner"] = user_company;
                            }

                            List<string> forbiddenStringList = new List<string>();
                            foreach (RoleEntityAccessPermision reap in currentuser.Role.RoleEntityAccessPermisions)
                            {
                                if (!reap.EnityAccessPermision.Visible)
                                {
                                    string toAdd = reap.EnityAccessPermision.EntityType.Substring(reap.EnityAccessPermision.EntityType.LastIndexOf('.') + 1);
                                    forbiddenStringList.Add(toAdd);
                                }
                            }
                            Session["LayoutForbidden"] = forbiddenStringList;
                        }
                        else
                        {
                            //Unrestricted access
                            List<string> forbiddenStringList = new List<string>();
                            Session["IsAdministrator"] = true;
                            StoresThatCurrentUserOwns = new List<StoreViewModel>();
                            foreach (Store store in new XPCollection<Store>(this.XpoSession))
                            {
                                StoreViewModel svm = new StoreViewModel();
                                svm.LoadPersistent(store);
                                StoresThatCurrentUserOwns.Add(svm);
                            }
                            Session["LayoutForbidden"] = forbiddenStringList;
                            XPCollection<CustomReport> reports = new XPCollection<CustomReport>(wow);
                            Session["ReportsToShowInMenu"] = reports.ToDictionary(g => g.Oid, g => g.Description);
                        }

                        UpdateCurrentUserSettings();

                        string message = "";
                        if (!UserHelper.UserCanLoginToCurrentStore(currentuser, MvcApplication.ApplicationInstance, StoreControllerAppiSettings.CurrentStoreOid, out message))
                        {
                            Session.Clear();
                            Session["Error"] = message;
                        }

                        if (ApplicationHelper.IsMasterInstance())
                        {
                            if (Session["IsAdministrator"] != null && (bool)Session["IsAdministrator"])
                            {
                                XPCollection<CompanyNew> companies = new XPCollection<CompanyNew>(XpoHelper.GetNewUnitOfWork());
                                Session["Companies"] = companies;
                                if (companies.Count == 1)
                                {
                                    Session["currentOwner"] = companies.First();
                                    Session["TraderCompanyName"] = companies.First().CompanyName;
                                    Session["OwnerSettings"] = companies.First().OwnerApplicationSettings;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected bool MyTryUpdateModel<TModel>(TModel model) where TModel : class
        {
            bool result = TryUpdateModel<TModel>(model);
            return result;
        }

        protected bool TryUpdateModel(object model)
        {
            MethodInfo method = this.GetType().GetMethod("MyTryUpdateModel", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = method.MakeGenericMethod(model.GetType());
            return (bool)genericMethod.Invoke(this, new object[] { model });
        }

        public static void UpdateUserGridSettings(Guid userOid, string gridName, string newGridLayout)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.GetObjectByKey<User>(userOid);
                if (user != null)
                {
                    GridSettings grd = user.GridSettings.FirstOrDefault(x => x.GridName == gridName);
                    if (grd == null)
                    {
                        grd = new GridSettings(uow) { GridName = gridName, User = user };
                    }
                    grd.GridLayout = newGridLayout;
                    XpoHelper.CommitChanges(uow);
                }
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            try
            {
                JObject jo;
                using (MiniProfiler.Current.Step("PrepareJsonResult"))
                {
                    jo = JObject.FromObject(data);
                    jo.Add("ApplicationError", Session["Error"] == null ? "" : Session["Error"].ToString());
                    jo.Add("ApplicationNotice", Session["Notice"] == null ? "" : Session["Notice"].ToString());

                    Session["Error"] = "";
                    Session["Notice"] = "";
                }
                using (MiniProfiler.Current.Step("WritingJsonResult"))
                {
                    return new ServiceStackJsonResult
                    {
                        Data = jo,
                        ContentType = contentType,
                        ContentEncoding = contentEncoding
                    };
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
                return base.Json(data, contentType, contentEncoding, behavior);
            }

        }

        public ActionResult CompanySelection()
        {
            return PartialView();
        }

        public ActionResult StoreSelection()
        {
            return PartialView();
        }

        public void FillItem()
        {
            Guid ItemGuid = Guid.Empty;

            Guid.TryParse(ViewData["ID"].ToString(), out ItemGuid);

            if (ItemGuid != Guid.Empty)
            {
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid));
                ViewData["Code"] = item.Code;
                ViewData["Name"] = item.Name;
                ViewData["DefaultBarcode"] = item.DefaultBarcode == null ? "" : item.DefaultBarcode.Code;
                ViewData["MotherCode"] = item.MotherCode == null ? "" : item.MotherCode.Code;
                ViewData["ItemSupplier"] = item.DefaultSupplier == null ? "" : item.DefaultSupplier.CompanyName;
                ViewData["VatCategory"] = item.VatCategory == null ? "" : item.VatCategory.Description;
                ViewData["Buyer"] = item.Buyer == null ? "" : item.Buyer.Description;
                ViewData["IsActive"] = item.IsActive;
                ViewData["IsCentralStored"] = item.IsCentralStored;
                ViewData["PackingQty"] = item.PackingQty;
                ViewData["OrderQty"] = item.OrderQty;
                ViewData["PackingMeasurementUnit"] = item.PackingMeasurementUnit == null ? "" : item.PackingMeasurementUnit.Description;
                ViewData["MaxOrderQty"] = item.MaxOrderQty;
                ViewData["InsertedOn"] = item.InsertedDate.ToString();
                ViewData["ExtraDescription"] = item.ExtraDescription;
                ViewData["Points"] = item.Points;
                ViewData["ref_unit"] = item.ReferenceUnit;
                ViewData["content_unit"] = item.ContentUnit;
                ViewData["MinOrderQty"] = item.MinOrderQty;
                ViewData["Remarks"] = item.Remarks;
                ViewData["UpdatedOnTicks"] = item.UpdatedOnTicks;
                ViewData["AcceptsCustomDescription"] = item.AcceptsCustomDescription;
                ViewData["AcceptsCustomPrice"] = item.AcceptsCustomPrice;
                ViewData["CustomPriceOptions"] = item.CustomPriceOptions.ToString();
                ViewData["Seasonality"] = item.Seasonality == null ? "" : item.Seasonality.ToString();
                ViewData["ExtraFilename"] = item.ExtraFilename == null ? "" : item.ExtraFilename.ToString();
                ViewBag.OwnerApplicationSettings = item.Owner.OwnerApplicationSettings;
            }
            else
            {
                ViewData["Code"] = "";
                ViewData["Name"] = "";
                ViewData["DefaultBarcode"] = "";
                ViewData["MotherCode"] = "";
                ViewData["ItemSupplier"] = "";
                ViewData["VatCategory"] = "";
                ViewData["Buyer"] = "";
                ViewData["IsActive"] = false;
                ViewData["IsCentralStored"] = false;
                ViewData["PackingQty"] = "";
                ViewData["OrderQty"] = "";
                ViewData["PackingMeasurementUnit"] = "";
                ViewData["MaxOrderQty"] = "";
                ViewData["InsertedOn"] = "";
                ViewData["ExtraDescription"] = "";
                ViewData["Points"] = "";
                ViewData["ref_unit"] = "";
                ViewData["content_unit"] = "";
                ViewData["MinOrderQty"] = "";
                ViewData["Remarks"] = "";
                ViewData["AcceptsCustomDescription"] = false;
                ViewData["AcceptsCustomPrice"] = false;
                ViewData["CustomPriceOptions"] = " ";
                ViewData["Seasonality"] = " ";
                ViewData["ExtraFilename"] = " ";
                ViewData["UpdatedOnTicks"] = 0;
            }

        }
    }
    public class ServiceStackJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(Data));
            }
        }
    }
}
