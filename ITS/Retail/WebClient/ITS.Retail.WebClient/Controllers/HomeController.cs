using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Xpo.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.Common.ViewModel;


namespace ITS.Retail.WebClient.Controllers
{
    [RoleAuthorize]
    [LicensedAuthorize]
    public class HomeController : BaseController
    {
        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        public ActionResult Index()
        {
            if (IsMobileDevice())
            {
                return new RedirectResult("~/Login");
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("User",CurrentUser.Oid),
                                                             new BinaryOperator("IsVisible",true));
                XPCollection<WidgetManager> userWidgets = GetList<WidgetManager>(CurrentUser.Session,crop);
                ViewBag.SelectedMenuDockPanels = userWidgets.ToDictionary(widget => widget.DockPanel, widget => widget.DockZone);
            }
            return View();
        }

        public ActionResult ItemsStatistics()
        {
            ItemStatistics itemstats = ComputeStatistics();
            return PartialView("ItemsStatistics", itemstats);
        }

        public ActionResult OffersList()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            CriteriaOperator visibleOffers = null;
            if (!Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                try
                {
                    User currentUser = CurrentUser;

                    //Customer visible offers
                    //-----------------------------
                    List<CriteriaOperator> visibleOffersFilterList = new List<CriteriaOperator>();
                    if (UserHelper.IsCustomer(currentUser))
                    {
                        Customer currentCustomer = UserHelper.GetCustomer(currentUser);
                        if (currentCustomer.Owner != null)
                        {
                            visibleOffersFilterList.Add(new BinaryOperator("PriceCatalog.Owner.Oid", currentCustomer.Owner.Oid));
                        }
                    }

                    //Supplier Offers
                    //----------------------
                    if (UserHelper.IsCompanyUser(currentUser))
                    {
                        visibleOffersFilterList.Add(new BinaryOperator("PriceCatalog.Owner.Oid", UserHelper.GetCompany(currentUser).Oid));
                    }

                    if (visibleOffersFilterList.Count != 0)
                    {
                        visibleOffers = CriteriaOperator.Or(visibleOffersFilterList);
                    }

                }
                catch (NullReferenceException)
                {

                    Session["Error"] = Resources.CustomerHasNoPriceCatalog;
                }
            }

            CriteriaOperator validFilter = null;

            DateTime now = DateTime.Now;

            validFilter = CriteriaOperator.And(new BinaryOperator("StartDate", now, BinaryOperatorType.LessOrEqual),
                                                         new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual),
                                                         new BinaryOperator("IsActive", true));

            CriteriaOperator criteria = CriteriaOperator.And(visibleOffers, validFilter);
            XPCollection<Offer> offers = GetList<Offer>(uow, criteria, "CreatedOn", direction: SortingDirection.Descending);
            return PartialView("OffersList", offers);
        }

        private class IntermediateObjectFrequency
        {
            public decimal Qty { get; set; }
            public decimal NetTotalAfterDiscount { get; set; }
            public CategoryNode Node { get; set; }

        }

        public JsonResult jsonSetCurrentStore()
        {
            string CompanyName = "", StoreName = "";
            if (Request["StoreOid"] != null && Request["StoreOid"] != "")
            {
                Guid store_guid;
                if (Guid.TryParse(Request["StoreOid"].ToString(), out store_guid))
                {
                    this.CurrentStore = new StoreViewModel();
                    Store sessionStore = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Store>(store_guid);
                    this.CurrentStore.LoadPersistent(sessionStore);
                    if (this.CurrentStore != null)
                    {
                        CompanyName = sessionStore.Owner.CompanyName;
                        StoreName = sessionStore.Name;

                        Session["TraderCompanyName"] = CompanyName;
                        Session["StoreName"] = StoreName;
                        Session["OwnerSettings"] = sessionStore.Owner.OwnerApplicationSettings;
                        Session["currentOwner"] = sessionStore.Owner;

                        Session["Menu"] = null;
                        CreateMenu();
                    }
                }
            }

            return Json(new { CompanyName = CompanyName, StoreName = StoreName });
        }

        private ItemStatistics ComputeStatistics()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            ItemStatistics statistics = new ItemStatistics();
            int selectedDateFilter = 1;
            int selectedTopItemsFilter = 1;
            string displayBy = "value";
            
            try
            {
            if (Request.Params["DXCallbackArgument"] != null)
            {
                selectedDateFilter = Int32.Parse(Request.Params["DXCallbackArgument"].Split(',')[1].Split(':')[1]);
                selectedTopItemsFilter = Int32.Parse(Request.Params["DXCallbackArgument"].Split(',')[2].Split(':')[1]);
                displayBy = Request.Params["DXCallbackArgument"].Split(',')[3].Split(':')[1];
            }
            }
            catch( Exception exception)
            {
                string exceptionMessage = exception.GetFullMessage();
            }

            ViewData["displayBy"] = displayBy;
            DateTime fl = DateTime.Now.AddDays((-1) * selectedDateFilter);
            IQueryable<DocumentDetail> queryDocumentDetails = new XPQuery<DocumentDetail>(uow).Where(documentDetail=>documentDetail.Item!=null);
            XPQuery<ItemAnalyticTree> queryItemAnalyticTree = new XPQuery<ItemAnalyticTree>(uow);

            CategoryNode mostCommonRoot = null;            
            try
            {
                mostCommonRoot = (
                                from iat in queryItemAnalyticTree
                                group iat by iat.Root into g
                                select new { Root = g.Key, Values = g.Count() }
                            ).OrderByDescending(g => g.Values).First().Root;
            }
            catch (Exception)
            {

            }
            User currentUser = CurrentUser;

            Customer customer = UserHelper.GetCustomer(currentUser);
            CompanyNew supplier = UserHelper.GetCompany(currentUser);

            bool userHelperIsCustomer = UserHelper.IsCustomer(currentUser);
            bool userHelperIsSupplier = UserHelper.IsCompanyUser(currentUser);

            Guid customerOid = customer == null ? Guid.Empty : customer.Oid;
            Guid supplierOid = supplier == null ? Guid.Empty : supplier.Oid;

            bool booleanParse = (Session["IsAdministrator"] == null) ? false : Boolean.Parse(Session["IsAdministrator"].ToString());
            if (displayBy == "value" && mostCommonRoot != null)
            {
                var details1 = (from det in queryDocumentDetails
                               where det.DocumentHeader.FinalizedDate > fl
                               && (det.IsCanceled == false) && (det.DocumentHeader.IsCanceled == false )
                               && det.DocumentHeader.DocumentNumber > 0
                               && (
                                   booleanParse == true
                                   || (userHelperIsCustomer && det.DocumentHeader.Customer.Oid == customerOid)
                                   || (userHelperIsSupplier && det.DocumentHeader.Store.Owner.Oid == supplierOid)
                               )
                               group det by det.Item into g
                               orderby Math.Round((decimal)g.Sum(x => x.NetTotal), (int)OwnerApplicationSettings.DisplayDigits) descending
                               select g
                               )
                               .Take(selectedTopItemsFilter).Select(g=> new ObjectFrequency<Item>()
                               {
                                   Item = g.Key,
                                   Frequency = g.Count(),
                                   Qty = g.Sum(x => x.Qty),
                                   Sum = Math.Round(g.Sum(x => x.NetTotal), (int)OwnerApplicationSettings.DisplayDigits),
                                   Owner = EffectiveOwner.Oid
                               });
                statistics.Items = details1.ToList();

                var details3 = from det in queryDocumentDetails
                               join iat in queryItemAnalyticTree on det.Item equals iat.Object
                               where det.DocumentHeader.FinalizedDate > fl && (det.IsCanceled == false ) &&
                               (det.DocumentHeader.IsCanceled == false ) && iat.Root == mostCommonRoot
                               && det.DocumentHeader.DocumentNumber > 0
                               && (
                                   booleanParse == true
                                   || (userHelperIsCustomer && det.DocumentHeader.Customer.Oid == customerOid)
                                   || (userHelperIsSupplier && det.DocumentHeader.Store.Owner.Oid == supplierOid)
                               )
                               select new IntermediateObjectFrequency() { Qty = det.Qty, NetTotalAfterDiscount = det.NetTotal, Node = iat.Node };

                var details2 = details3.GroupBy(g => g.Node).OrderByDescending(g => g.Sum(x => x.NetTotalAfterDiscount)).Take(selectedTopItemsFilter)
                    .Select(g => new ObjectFrequency<CategoryNode>()
                    {
                        Item = g.Key,
                        Frequency = g.Count(),
                        Qty = g.Sum(x => x.Qty),
                        Sum = Math.Round(g.Sum(x => x.NetTotalAfterDiscount),
                        (int)OwnerApplicationSettings.DisplayDigits),
                        Owner = EffectiveOwner.Oid
                    });

                statistics.ItemCategories = details2.ToList();
            }
            else
            {
                var details1 = (from det in queryDocumentDetails

                               where det.DocumentHeader.FinalizedDate > fl
                               && (det.IsCanceled == false) && (det.DocumentHeader.IsCanceled == false)
                               && det.DocumentHeader.DocumentNumber > 0
                               && (
                                   booleanParse == true
                                   || (userHelperIsCustomer && det.DocumentHeader.Customer.Oid == customerOid)
                                   || (userHelperIsSupplier && det.DocumentHeader.Store.Owner.Oid == supplierOid)
                               )
                               group det by det.Item into g select g).OrderByDescending(g => g.Sum(x => x.Qty)).Take(selectedTopItemsFilter).
                               Select(g=> new ObjectFrequency<Item>()
                               {
                                   Item = g.Key,
                                   Frequency = g.Count(),
                                   Qty = g.Sum(x => x.Qty),
                                   Sum = Math.Round(g.Sum(x => x.NetTotal), (int)OwnerApplicationSettings.DisplayDigits),
                                   Owner = EffectiveOwner.Oid
                               });
                statistics.Items = details1.ToList();
                
                var details3 = from det in queryDocumentDetails
                               join iat in queryItemAnalyticTree on det.Item equals iat.Object
                               where det.DocumentHeader.FinalizedDate > fl && (det.IsCanceled == false) &&
                               (det.DocumentHeader.IsCanceled == false) && iat.Root == mostCommonRoot
                               && det.DocumentHeader.DocumentNumber > 0
                               && (
                                   booleanParse == true
                                   || (userHelperIsCustomer && det.DocumentHeader.Customer.Oid == customerOid)
                                   || (userHelperIsSupplier && det.DocumentHeader.Store.Owner.Oid == supplierOid)
                               )
                               select new IntermediateObjectFrequency() { Qty = det.Qty, NetTotalAfterDiscount = det.NetTotal, Node = iat.Node };
                
                var details2 = details3.GroupBy(g => g.Node)
                    .Select(g => new ObjectFrequency<CategoryNode>()
                    {
                        Item = g.Key,
                        Frequency = g.Count(),
                        Qty = g.Sum(x => x.Qty),
                        Sum = Math.Round(g.Sum(x => x.NetTotalAfterDiscount), (int)OwnerApplicationSettings.DisplayDigits),
                        Owner = EffectiveOwner.Oid
                    });

                statistics.ItemCategories = details2.OrderByDescending(g => g.Qty).Take(selectedTopItemsFilter).ToList();
            }
            return statistics;
        }

        public ActionResult CustomersChart()
        {
            int yearFilter = DateTime.Now.Year;
            int topFilter = 10;
            bool getBestCustomers = true;
            if (Request.Params["DXCallbackArgument"] != null)
            {
                yearFilter = Int32.Parse(Request.Params["DXCallbackArgument"].Split(',')[1].Split(':')[1]);
                topFilter = Int32.Parse(Request.Params["DXCallbackArgument"].Split(',')[2].Split(':')[1]);
                getBestCustomers = (Request.Params["DXCallbackArgument"].Split(',')[3].Split(':')[1] == "best");
            }

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            User currentUser = CurrentUser;
            List<CustomerRevenue> list = new List<CustomerRevenue>();
            XPCollection<DocumentHeader> documentHeaders = GetList<DocumentHeader>(uow, new BinaryOperator("Oid", Guid.Empty));
            if (UserHelper.IsCompanyUser(currentUser))
            {
                CompanyNew supplier = UserHelper.GetCompany(currentUser);
                documentHeaders = GetList<DocumentHeader>(uow, CriteriaOperator.And(new BinaryOperator("Store.Owner.Oid", supplier.Oid), new BinaryOperator("IsCanceled", false)));
            }
            else if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                documentHeaders = GetList<DocumentHeader>(uow, CriteriaOperator.Or(new BinaryOperator("IsCanceled", false), new NullOperator("IsCanceled")));
            }

            var grouped = from detail in documentHeaders
                          group detail by new { detail.FinalizedDate.Year, detail.Customer };

            var unsortedResult = grouped.Where(x => x.Key.Year == yearFilter)
                          .Select(x => new CustomerRevenue() { Customer = x.Key.Customer, Year = x.Key.Year, Revenue = Math.Round(x.Sum(y => y.GrossTotal), (int)OwnerApplicationSettings.DisplayDigits) });
            if (getBestCustomers)
            {
                list = unsortedResult.OrderByDescending(x => x.Revenue).Take(topFilter).ToList();
            }
            else
            {
                list = unsortedResult.OrderBy(x => x.Revenue).Take(topFilter).ToList();
            }
            return PartialView("CustomersChart", list);
        }
        
        public ActionResult ItsPopup()
        {
            return PartialView("../Shared/ItsPopup");
        }

        public ActionResult Settings()
        {
            return View();
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();
            ActionResult returnType = PartialView("LoadViewPopup");
            if (Request["Type"] != null)
            {
                ViewData["ID"] = Request["ID"];
                if (Request["Type"].ToString() == "item")
                {
                    FillItem();

                    returnType = PartialView("../Item/LoadViewPopup");
                }
                else if (Request["Type"].ToString() == "customer")
                {                   
                    returnType = PartialView("../Customer/LoadViewPopup");
                }
                else if (Request["Type"].ToString() == "supplier")
                {                    
                    returnType = PartialView("../Supplier/LoadViewPopup");
                }
                else if (Request["Type"].ToString() == "company")
                {
                    returnType = PartialView("../Company/LoadViewPopup");
                }

            }

            return returnType;
        }

        public override ActionResult PopupViewCallbackPanel()
        {
            base.PopupViewCallbackPanel();

            if (Request["Type"] != "")
            {
                ViewData["Type"] = Request["Type"];
            }

            ActionResult returnResult = PartialView("PopupViewCallbackPanel");
            return returnResult;
        }

        public JsonResult SetCurrentCompany(string companyOid)
        {
            Guid companyGuid;
            CompanyNew currentOwner = null;

            if (Guid.TryParse(companyOid, out companyGuid))
            {
                currentOwner = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>(companyGuid); ;
                if (currentOwner != null)
                {
                    if (CurrentOwner == null || currentOwner.Oid != CurrentOwner.Oid)
                    {
                        CurrentOwnerStatic = currentOwner;
                        Session["OwnerSettings"] = currentOwner.OwnerApplicationSettings;
                        UpdateCurrentUserSettings();
                        Session["Menu"] = null;
                        CreateMenu();
                    }
                    return Json(new { success = true, CompanyName = EffectiveOwner.CompanyName, StoreName = "" });
                }
            }

            if (currentOwner == null)
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            return Json(new { success = false });
        }

        public ActionResult MenuDockPanels()
        {
            CreateMenu();
            if (Request["SelectedDockPanels"] != null)
            {
                JArray json = JsonConvert.DeserializeObject<JArray>(Request["SelectedDockPanels"]);
                Dictionary<string, int> selectedDockPanels = new Dictionary<string, int>();
                foreach (JObject jWidget in json)
                {
                    if (jWidget.Property("IsVisible").Value.Value<bool>())
                    {

                        string dockPanel = jWidget.Property("DockPanel").Value.ToString();
                        if (selectedDockPanels.ContainsKey(dockPanel) == false)
                        {
                            int dockZone = jWidget.Property("DockZone").Value.Value<int>();
                            selectedDockPanels.Add(dockPanel, dockZone);
                        }
                    }
                }

                ViewBag.SelectedMenuDockPanels = selectedDockPanels;

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    User CurrentUser = uow.GetObjectByKey<User>(base.CurrentUser.Oid);
                    foreach (WidgetManager widget in CurrentUser.WidgetManagers)
                    {
                        widget.IsVisible = selectedDockPanels.ContainsKey(widget.DockPanel);
                        if (widget.IsVisible)
                        {
                            widget.DockZone = selectedDockPanels[widget.DockPanel];
                        }
                    }
                    IEnumerable<string> existingManagers = CurrentUser.WidgetManagers.Select(g => g.DockPanel);
                    IEnumerable<string> notExisting = selectedDockPanels.Where(g => existingManagers.Contains(g.Key) == false).Select(x => x.Key);
                    foreach (string newManager in notExisting)
                    {
                        WidgetManager manager = new WidgetManager(uow);
                        manager.User = CurrentUser;
                        manager.IsVisible = true;
                        manager.DockPanel = newManager;
                        manager.DockZone = selectedDockPanels[newManager];
                    }
                    CurrentUser.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }

            return PartialView();
        }

        public ActionResult CloseWindow()
        {
            return View();
        }




        public ActionResult Search()
        {
          
            XPCollection<Item> itemXPC = null;
            XPCollection<Customer> customerXPC = null;
            XPCollection<SupplierNew> supplierXPC = null;
            XPCollection<CompanyNew> companyXPC = null;
            XPCollection<User> userXPC = null;
            bool? noResults = null;
            EntityAccessPermision entityAccessPermission;

            string searchPanelFormInput = Request["searchPanelFormInput"] == null ? String.Empty : Request["searchPanelFormInput"].ToString();

            if (Request["searchPanelFormInput"] != null)
            {
                entityAccessPermission = CurrentUser.Role.RoleEntityAccessPermisions.Select(roleEntAccPer => roleEntAccPer.EnityAccessPermision).FirstOrDefault(entAccPer => entAccPer.EntityType == "Item");
                if (entityAccessPermission == null || entityAccessPermission.Visible)
                {
                    itemXPC = GetList<Item>(XpoSession,
                        CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), searchPanelFormInput),
                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), searchPanelFormInput),
                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("DefaultBarcode"), searchPanelFormInput)
                                            //new BinaryOperator("Code", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                            //new BinaryOperator("Description", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                            //new BinaryOperator("DefaultBarcode", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like)
                                           )
                    );
                }

                CriteriaOperator traderCriteria = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), searchPanelFormInput),
                                                                      new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), searchPanelFormInput),
                                                                      new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), searchPanelFormInput)
                                                                     );

                entityAccessPermission = CurrentUser.Role.RoleEntityAccessPermisions.Select(roleEntAccPer => roleEntAccPer.EnityAccessPermision).FirstOrDefault(entAccPer => entAccPer.EntityType == "Customer");
                if (entityAccessPermission == null || entityAccessPermission.Visible || entityAccessPermission.CanUpdate || entityAccessPermission.CanDelete)
                {
                    customerXPC = GetList<Customer>(XpoSession, traderCriteria);
                                //CriteriaOperator.Or(new BinaryOperator("Code", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                //                    new BinaryOperator("Trader.TaxCode", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                //                    new BinaryOperator("CompanyName", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like)
                                //)
                                //);
                }

                entityAccessPermission = CurrentUser.Role.RoleEntityAccessPermisions.Select(roleEntAccPer => roleEntAccPer.EnityAccessPermision).FirstOrDefault(entAccPer => entAccPer.EntityType == "Supplier");
                if (entityAccessPermission == null || entityAccessPermission.Visible || entityAccessPermission.CanUpdate || entityAccessPermission.CanDelete)
                {
                    supplierXPC = GetList<SupplierNew>(XpoSession, traderCriteria);
                                        //    CriteriaOperator.Or(new BinaryOperator("Code", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                        //                        new BinaryOperator("Trader.TaxCode", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                        //                        new BinaryOperator("CompanyName", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like)
                                        //    )
                                        //);
                }

                entityAccessPermission = CurrentUser.Role.RoleEntityAccessPermisions.Select(roleEntAccPer => roleEntAccPer.EnityAccessPermision).FirstOrDefault(entAccPer => entAccPer.EntityType == "Company");
                if (entityAccessPermission == null || entityAccessPermission.Visible || entityAccessPermission.CanUpdate || entityAccessPermission.CanDelete)
                {
                    companyXPC = GetList<CompanyNew>(XpoSession, traderCriteria);
                                        //    CriteriaOperator.Or(new BinaryOperator("Code", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                        //                        new BinaryOperator("Trader.TaxCode", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                        //                        new BinaryOperator("CompanyName", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like)
                                        //    )
                                        //);
                }

                entityAccessPermission = CurrentUser.Role.RoleEntityAccessPermisions.Select(roleEntAccPer => roleEntAccPer.EnityAccessPermision).FirstOrDefault(entAccPer => entAccPer.EntityType == "User");
                if (entityAccessPermission == null || entityAccessPermission.Visible || entityAccessPermission.CanUpdate || entityAccessPermission.CanDelete)
                {
                    userXPC = new XPCollection<User>(XpoSession,
                                                     CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("UserName"), searchPanelFormInput),
                                                                        new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("FullName"), searchPanelFormInput)
                                                                        //new BinaryOperator("UserName", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like),
                                                                        //new BinaryOperator("FullName", string.Format("%{0}%", Request["searchPanelFormInput"]), BinaryOperatorType.Like)
                                                                        )
                                                    );
                }

                if ((itemXPC == null || itemXPC.Count == 0) &&
                    (customerXPC == null || customerXPC.Count == 0) && 
                    (supplierXPC == null || supplierXPC.Count == 0) && 
                    (companyXPC == null || companyXPC.Count == 0) && 
                    (userXPC == null || userXPC.Count == 0))
                {
                    noResults = true;
                }
            }

            ViewBag.NoResults = noResults;

            return PartialView("../Shared/SearchPanelResults", new ViewDataDictionary { 
                    {"Item", (itemXPC != null && itemXPC.Count != 0) ? itemXPC : null },
                    {"Customer",(customerXPC != null && customerXPC.Count != 0) ? customerXPC : null},
                    {"Supplier",(supplierXPC != null && supplierXPC.Count != 0) ? supplierXPC : null},
                    {"Company",(companyXPC != null && companyXPC.Count != 0) ? companyXPC : null},
                    {"User",(userXPC != null && userXPC.Count != 0) ? userXPC : null}
                }
            );
        }
    }
}
