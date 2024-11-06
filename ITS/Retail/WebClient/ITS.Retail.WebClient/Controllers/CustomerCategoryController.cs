using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common.Helpers;
using System.Collections.Generic;

namespace ITS.Retail.WebClient.Controllers
{
    public class CustomerCategoryController : BaseObjController<CustomerCategory>
    {
        UnitOfWork uow;

        public ActionResult AddItemsToCatecory(string ID)
        {
            //if (ToolbarOptions != null)
            //{
            //    ToolbarOptions.ForceVisible = false;
            //}
            if (!TableCanInsert || !TableCanUpdate)
            {
                return RedirectToAction("E404", "Error");
            }

            ViewData["CategoryID"] = ID;
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            XPCollection<Customer> customers = GetAllowedCustomers(ID);
            customers.Criteria = CriteriaOperator.And(customers.Criteria, filter);
            return PartialView("AddItemsToCategory", customers);
        }

        public JsonResult InsertCustomers(string CategoryID, string[] data)
        {
            if (data != null)
            {
                Guid CategoryOid;
                if (!Guid.TryParse(CategoryID, out CategoryOid))
                {
                    return Json(new { success = false });
                }

                CustomerCategory currentCategory = XpoSession.FindObject<CustomerCategory>(new BinaryOperator("Oid", CategoryOid));

                foreach (string itmoid in data)
                {

                    Guid ItemOid;
                    if (Guid.TryParse(itmoid, out ItemOid))
                    {

                        Customer customer = XpoSession.FindObject<Customer>(new BinaryOperator("Oid", ItemOid));
                        CustomerAnalyticTree tr = new CustomerAnalyticTree(XpoSession);
                        tr.Node = currentCategory;
                        tr.Object = customer;
                        tr.Root = currentCategory.GetRoot(currentCategory.Session);
                        tr.Save();
                    }
                }
                //XpoSession.CommitTransaction();
                XpoHelper.CommitTransaction(XpoSession);
            }
            return Json(new { success = true });
        }

        public ActionResult AllowedCustomers(string CategoryID)
        {
            Session["AddCustomersFilter"] = Session["AddCustomersFilter"] ?? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            if (Request["DXCallbackArgument"].Contains("CLEARFILTERS"))
            {
                Session["AddCustomersFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            }
            else
            {
                string code = (Request["customer_code"] == null || Request["customer_code"] == "null") ? "" : Request["customer_code"];
                string cardid = (Request["card_id"] == null || Request["card_id"] == "null") ? "" : Request["card_id"];
                string name = (Request["customer_name"] == null || Request["customer_name"] == "null") ? "" : Request["customer_name"];
                string taxn = (Request["customer_tax_number"] == null || Request["customer_tax_number"] == "null") ? "" : Request["customer_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];
                string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];

                CriteriaOperator codeFilter, nameFilter = null, lastNameFilter = null, cardidFilter = null, taxnFilter = null, activeFilter, companyNameFilter = null;
                if (name != "")
                {
                    nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), name);
                    // new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), name);
                    // new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), name);
                    // new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like););
                }
                if (taxn != "")
                {
                    taxnFilter = //new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), taxn);
                    new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }

                if (cardid != "")
                {
                    cardidFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CardID"), cardid);
                    // new BinaryOperator("CardID", "%" + cardid + "%", BinaryOperatorType.Like);
                }

                if (code.Replace('%', '*').Contains("*"))
                {
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%'), BinaryOperatorType.Like);

                }
                else
                {
                    codeFilter = CreateCriteria(code, "Code");
                }

                activeFilter = null;
                bool isActiveFilter;
                if (Boolean.TryParse(Request["is_active"], out isActiveFilter))
                {
                    activeFilter = new BinaryOperator("IsActive", isActiveFilter);
                }

                CriteriaOperator createdOnFilter = null;
                if (FcreatedOn != "")
                {
                    createdOnFilter = new BinaryOperator("CreatedOnTicks", DateTime.Parse(FcreatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                CriteriaOperator updatedOnFilter = null;
                if (FupdatedOn != "")
                {
                    updatedOnFilter = new BinaryOperator("UpdatedOnTicks", DateTime.Parse(FupdatedOn).Ticks, BinaryOperatorType.GreaterOrEqual);
                }

                Session["AddCustomersFilter"] = CriteriaOperator.And(codeFilter,
                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                taxnFilter,
                                                cardidFilter,
                                                CreateCriteria(lcod, "Loyalty"),
                                                activeFilter, createdOnFilter, updatedOnFilter
                                                );
            }
            ViewData["CategoryID"] = CategoryID;
            CriteriaOperator allowedCustomersCriteria = GetAllowedCustomersCriteria(CategoryID);
            //XPCollection<Customer> customers = GetAllowedCustomers(CategoryID);
            //customers.Criteria = CriteriaOperator.And(customers.Criteria, (CriteriaOperator)Session["AddCustomersFilter"]);
            allowedCustomersCriteria = CriteriaOperator.And(allowedCustomersCriteria, (CriteriaOperator)Session["AddCustomersFilter"]);
            XPCollection<Customer> customers = GetList<Customer>(XpoSession, allowedCustomersCriteria);
            return PartialView("AllowedCustomers", customers);
        }


        public override ActionResult PopupAddCallbackPanel()
        {
            base.PopupAddCallbackPanel();

            return PartialView();
        }

        public override ActionResult LoadAddPopup()
        {
            base.LoadAddPopup();

            string text = null;

            if (!String.IsNullOrEmpty(Request["Text"]))
            {
                text = Request["Text"];
            }

            ViewBag.Title = Resources.AddCustomersToCategory + ": " + text;

            ActionResult rt = PartialView("LoadAddPopup");
            return rt;
        }

        public ActionResult CustomersOfNode()
        {
            GenerateUnitOfWork();
            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            if (categoryid != "")
            {
                Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;

                CustomerCategory cat = XpoHelper.GetNewSession().FindObject<CustomerCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                ViewBag.CatDescription = cat != null ? cat.Description : "";
                if (cat != null)
                {
                    CustomerCategory root = (CustomerCategory)cat.GetRoot(XpoHelper.GetNewSession());

                    CriteriaOperator filter = (categoryid == "" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "") : CriteriaOperator.Parse("Node='" + Guid.Parse(categoryid) + "'" + " and Root='" + root.Oid + "'"));

                    ViewData["categoryid"] = categoryid;
                    User currentUser = CurrentUser;

                    XPCollection<CompanyNew> userCompanies = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
                    XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);

                    if (Boolean.Parse(Session["IsAdministrator"].ToString()))
                    {
                        ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<CustomerAnalyticTree>();
                    }
                    else
                    {
                        if (userCompanies.Count != 0)
                        {
                            CompanyNew owner = userCompanies.First();
                            ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<CustomerAnalyticTree>();
                        }
                        else if (userCustomers.Count != 0)
                        {
                            Customer cust = userCustomers.First();
                            ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<CustomerAnalyticTree>();
                        }
                    }
                    ViewData["rootid"] = root.Oid;
                }


            }
            return PartialView("CustomersOfNode");
        }



        public ActionResult EditTreeViewPopup()
        {
            return PartialView("EditTreeViewPopup");
        }



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

        private XPCollection<Customer> GetAllowedCustomers(string CategoryID)
        {
            CriteriaOperator allowedCustomersCriteria = GetAllowedCustomersCriteria(CategoryID);
            XPCollection<Customer> allowedCustomers = GetList<Customer>(XpoSession, allowedCustomersCriteria);
            return allowedCustomers;
        }

        private CriteriaOperator GetAllowedCustomersCriteria(string CategoryID)
        {
            CriteriaOperator notAllowedCustomersCriteria = null;
            Guid CategoryOid;
            ViewData["CategoryID"] = CategoryID;
            if (!Guid.TryParse(CategoryID, out CategoryOid))
            {
                ViewData["CategoryName"] = "Λανθασμένη κατηγορία";
                notAllowedCustomersCriteria = new BinaryOperator("Oid", Guid.Empty);
            }
            else
            {
                CustomerCategory currentCategory, root;
                currentCategory = XpoSession.FindObject<CustomerCategory>(new BinaryOperator("Oid", CategoryOid));
                ViewData["CategoryName"] = currentCategory.Description;
                root = currentCategory.GetRoot(XpoSession) as CustomerCategory;
                notAllowedCustomersCriteria = root.GetAllNodeTreeFilter();
                notAllowedCustomersCriteria = new NotOperator(notAllowedCustomersCriteria);
            }
            return notAllowedCustomersCriteria;
        }

        //
        // GET: /CustomerCategory/
        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ForceVisible = false;
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");

            return View();
        }

        [HttpPost]
        public ActionResult InlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] CustomerAnalyticTree ct)
        {
            if (!TableCanInsert)
                return null;

            GenerateUnitOfWork();

            if (ModelState.IsValid)
            {
                try
                {
                    UpdateLookupObjectsT(ct);
                    SaveT(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
            CustomerCategory cat = XpoHelper.GetNewSession().FindObject<CustomerCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            CustomerCategory root = (CustomerCategory)cat.GetRoot(XpoHelper.GetNewSession());
            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);

            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>();
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First();
                    if (UserHelper.IsCustomer(currentUser) && this.CurrentStore == null)
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("CustomersOfNode");
                    }
                }
            }
            return PartialView("CustomersOfNode");
        }

        [HttpPost]
        public ActionResult InlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] CustomerAnalyticTree ct)
        {
            if (!TableCanUpdate)
            {
                return null;
            }


            GenerateUnitOfWork();
            if (ModelState.IsValid)
            {
                try
                {
                    UpdateLookupObjectsT(ct);
                    SaveT(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }


            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            CustomerCategory cat = XpoHelper.GetNewSession().FindObject<CustomerCategory>(CriteriaOperator.Parse("Oid='" + categoryid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);

            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>();
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First();
                    if (this.CurrentStore == null)
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("CustomersOfNode");
                    }
                }
            }
            return PartialView("CustomersOfNode");
        }

        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] CustomerAnalyticTree ct)
        {
            if (!TableCanDelete)
            {
                return null;
            }

            GenerateUnitOfWork();
            try
            {
                DeleteT(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
            CustomerCategory cat = XpoHelper.GetNewSession().FindObject<CustomerCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            CustomerCategory root = (CustomerCategory)cat.GetRoot(XpoHelper.GetNewSession());
            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["CustomerAnalyticTree"] = cat.GetAllNodeTreeItems<CustomerAnalyticTree>();
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First();
                    if (this.CurrentStore == null) //no store selected
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("ItemsOfNode");
                    }
                }
            }
            return PartialView("CustomersOfNode");
        }

        public JsonResult jsonChangeCategory()
        {
            if (!TableCanUpdate)
            {
                return null;
            }

            try
            {
                Guid customerAnalyticTreeID = (Request["AnalyticTreeID"] == null || Request["AnalyticTreeID"] == "null" || Request["AnalyticTreeID"] == "-1") ? Guid.Empty : Guid.Parse(Request["AnalyticTreeID"]);
                Guid CategoryID = (Request["CategoryID"] == null || Request["CategoryID"] == "null" || Request["CategoryID"] == "-1") ? Guid.Empty : Guid.Parse(Request["CategoryID"]);
                CustomerAnalyticTree cat = XpoSession.FindObject<CustomerAnalyticTree>(new BinaryOperator("Oid", customerAnalyticTreeID, BinaryOperatorType.Equal));
                if (cat != null)
                {
                    cat.Node = XpoSession.FindObject<CategoryNode>(new BinaryOperator("Oid", CategoryID, BinaryOperatorType.Equal));
                    cat.Root = cat.Node.GetRoot(XpoSession);

                    cat.Save();

                    XpoHelper.CommitChanges(XpoSession);

                    Session["Notice"] = Resources.SavedSuccesfully;
                    return Json(new { success = true });
                }
                else
                {
                    Session["Notice"] = Resources.NotSaved;
                    return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                Session["Error"] = Resources.AnErrorOccurred + ": " + e.Message + Environment.NewLine + e.StackTrace;
                return Json(new { success = false });
            }
        }

        public ActionResult PopUpCallBackPanel()
        {
            string parent = Request["parent"] == null || Request["parent"] == "null" || Request["parent"] == "-1" ? "" : Request["parent"];
            string mode = Request["mode"] == null || Request["mode"] == "null" ? "" : Request["mode"];
            string oid = Request["oid"] == null || Request["oid"] == "null" ? "-1" : Request["oid"];
            string descr = Request["descr"] == null || Request["descr"] == "null" ? "" : Request["descr"];
            string code = Request["code"] == null || Request["code"] == "null" ? "" : Request["code"];
            bool isLoyalty = false;
            if (Request["IsLoyalty"] == null)
            {
                isLoyalty = false;
            }
            else
            {
                isLoyalty = Request["IsLoyalty"].Equals("C") ? true : false;
            }

            GenerateUnitOfWork();
            CustomerCategory customercategory;
            CriteriaOperator filter = (oid == "-1" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'") : CriteriaOperator.Parse("Oid='" + Guid.Parse(oid) + "'", ""));
            customercategory = uow.FindObject<CustomerCategory>(filter);
            if (mode == "0")//insert
            {
                if (parent == "")
                {
                    ViewData["parent"] = null;
                }
                else
                {
                    ViewData["parent"] = parent;
                }
                ViewData["isnew"] = true;
            }
            else if (mode == "1")//edit
            {
                if (!UserCanPerformAction(customercategory))
                {
                    return null;
                }

                if (parent == "")
                {
                    ViewData["parent"] = null;
                }
                else
                {
                    ViewData["parent"] = parent;
                }
                ViewData["isnew"] = false;
            }
            else if (mode == "2")//save
            {
                if (oid != "-1")
                {
                    if (!TableCanUpdate)
                    {
                        return null;
                    }
                }
                else
                {
                    if (!TableCanInsert)
                    {
                        return null;
                    }
                    customercategory = new CustomerCategory(uow);
                    AssignOwner(customercategory);
                }

                customercategory.Description = descr;
                customercategory.Code = code;


                if (parent == "")
                {
                    customercategory.Parent = null;
                    customercategory.IsLoyalty = isLoyalty;
                    List<CategoryNode> nodes = customercategory.GetAllNodes().ToList();
                    foreach (CategoryNode node in nodes)
                    {
                        CustomerCategory cc = uow.GetObjectByKey<CustomerCategory>(node.Oid);
                        cc.IsLoyalty = isLoyalty;
                    }
                }
                else
                {
                    CategoryNode root = customercategory.GetRoot(uow);
                    CustomerCategory cc = uow.GetObjectByKey<CustomerCategory>(root.Oid);
                    bool isloyalty = cc?.IsLoyalty ?? false;
                    customercategory.IsLoyalty = isloyalty == true ? true : false;
                    customercategory.Parent = uow.FindObject<CustomerCategory>(CriteriaOperator.Parse("Oid='" + Guid.Parse(parent) + "'", ""));
                }

                try
                {
                    customercategory.Save();
                    XpoHelper.CommitChanges(uow);
                    ViewData["Success"] = true;
                }
                catch (Exception e)
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    ViewData["Success"] = false;
                    if (e.Message.Contains("Cannot insert duplicate key row"))
                    {
                        Session["Error"] = Resources.CodeAlreadyExists;
                    }
                    else
                    {
                        Session["Error"] = e.Message;
                    }
                }
            }
            else if (mode == "-1")//delete
            {
                if (!TableCanDelete)
                {
                    return null;
                }

                try
                {
                    XPCollection<CustomerAnalyticTree> customerTrees = GetList<CustomerAnalyticTree>(uow, new BinaryOperator("Node.Oid", customercategory.Oid));
                    uow.Delete(customerTrees);
                    customercategory.Delete();
                    XpoHelper.CommitChanges(uow);
                    ViewData["Success"] = true;
                }
                catch (Exception e)
                {
                    string exceptionMessage = e.GetFullMessage();
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    ViewData["Success"] = false;
                    Session["Error"] = Resources.CannotDeleteItemCategory;
                }
            }
            ViewData["Nodes"] = GetList<CustomerCategory>(XpoHelper.GetNewUnitOfWork());
            if (customercategory != null)
            {
                ViewBag.Unexpandable = !customercategory.HasChild && customercategory.Parent == null;
                if (customercategory.Parent != null)
                {
                    ViewBag.ParentUnexpandable = !(customercategory.Parent.ChildCategories.Count() > 1);
                }
            }

            ViewData["IsRoot"] = false;
            if (customercategory != null)
            {
                if (customercategory.Parent == null)
                {
                    ViewData["IsRoot"] = true;
                    if (customercategory.IsLoyalty != true)
                    {
                        customercategory.IsLoyalty = false;
                    }
                }
                else
                {
                    ViewData["IsRoot"] = false;
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        CategoryNode root = customercategory.GetRoot(uow);
                        CustomerCategory cc = uow.GetObjectByKey<CustomerCategory>(root.Oid);
                        bool isloyalty = cc?.IsLoyalty ?? false;
                        customercategory.IsLoyalty = isloyalty == true ? true : false;

                    }
                }
            }

            return PartialView("PopUpCallBackPanel", customercategory);
        }

        protected void UpdateLookupObjectsT(CustomerAnalyticTree a)
        {
            base.UpdateLookupObjects(a);
            a.Node = GetObjectByArgument<CustomerCategory>(a.Session, "NodeComboBoxID");
            a.Root = GetObjectByArgument<CustomerCategory>(a.Session, "RootComboBoxID");
            a.Object = GetObjectByArgument<Customer>(a.Session, "ObjectComboBoxID");
        }

        public ActionResult TreeView()
        {
            return PartialView("TreeView");
        }

        public ActionResult TreeViewCallback()
        {
            return PartialView("TreeViewCallback");
        }

        public ActionResult TreeViewPartial()
        {
            return PartialView();
        }

        public ActionResult TreeViewPopup()
        {
            return PartialView();
        }

        public ActionResult TreeViewPopupPartial()
        {
            return PartialView();
        }
    }
}
