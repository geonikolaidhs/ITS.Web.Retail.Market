using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Common.Helpers;
using System.Collections.Generic;

namespace ITS.Retail.WebClient.Controllers
{
    public class ItemCategoryController : BaseObjController<ItemCategory>
    {
        //
        // GET: /ItemAnalyticTreeView/
        UnitOfWork uow;


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

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ForceVisible = false;
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");

            return View();
        }

        public ActionResult PopUpCallBackPanel()
        {
            string parent = Request["parent"] == null || Request["parent"] == "null" || Request["parent"] == "-1" ? "" : Request["parent"];
            string mode = Request["mode"] == null || Request["mode"] == "null" ? "" : Request["mode"];
            string oid = Request["oid"] == null || Request["oid"] == "null" ? "-1" : Request["oid"];
            string descr = Request["descr"] == null || Request["descr"] == "null" ? "" : Request["descr"];
            string code = Request["code"] == null || Request["code"] == "null" ? "" : Request["code"];
            string points = Request["points"] == null || Request["points"] == "null" ? "0" : Request["points"];
            GenerateUnitOfWork();
            ItemCategory itemcategory;
            CriteriaOperator filter = (oid == "-1" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'") : CriteriaOperator.Parse("Oid='" + Guid.Parse(oid) + "'", ""));
            itemcategory = uow.FindObject<ItemCategory>(filter);
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
                if (!UserCanPerformAction(itemcategory))
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
                    itemcategory = new ItemCategory(uow);
                    AssignOwner(itemcategory);
                }

                itemcategory.Description = descr;
                itemcategory.Code = code;
                itemcategory.Points = Convert.ToInt32(points);
                if (parent == "")
                {
                    itemcategory.Parent = null;
                }
                else
                {
                    itemcategory.Parent = uow.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + Guid.Parse(parent) + "'", ""));
                }
                try
                {
                    itemcategory.Save();
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
                    XPCollection<ItemAnalyticTree> itemTrees = GetList<ItemAnalyticTree>(uow, new BinaryOperator("Node.Oid", itemcategory.Oid));
                    uow.Delete(itemTrees);
                    itemcategory.Delete();
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

            ViewData["Nodes"] = GetList<ItemCategory>(XpoHelper.GetNewUnitOfWork());
            if (itemcategory != null)
            {
                ViewBag.Unexpandable = !itemcategory.HasChild && itemcategory.Parent == null;
                if (itemcategory.Parent != null)
                {
                    ViewBag.ParentUnexpandable = !(itemcategory.Parent.ChildCategories.Count() > 1);
                }
            }
            return PartialView("PopUpCallBackPanel", itemcategory);
        }

        public ActionResult EditTreeViewPopup()
        {
            return PartialView("EditTreeViewPopup");
        }

        public ActionResult TreeViewPartial()
        {
            return PartialView();
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

            ViewBag.Title = Resources.AddItemsToCategory + ": " + text;

            ActionResult rt = PartialView("LoadAddPopup");
            return rt;
        }


        public ActionResult ItemsOfNode()
        {
            GenerateUnitOfWork();
            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            if (categoryid != "")
            {
                Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;

                ItemCategory cat = XpoHelper.GetNewSession().FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                ViewBag.CatDescription = cat != null ? cat.Description : "";
                if (cat != null)
                {
                    ItemCategory root = (ItemCategory)cat.GetRoot(XpoHelper.GetNewSession());

                    CriteriaOperator filter = (categoryid == "" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "") : CriteriaOperator.Parse("Node='" + Guid.Parse(categoryid) + "'" + " and Root='" + root.Oid + "'"));

                    ViewData["categoryid"] = categoryid;
                    User currentUser = CurrentUser;
                    XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
                    XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
                    if (Boolean.Parse(Session["IsAdministrator"].ToString()))
                    {
                        ViewData["ItemAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>().OrderBy(x => x.ShowOrder);
                    }
                    else
                    {
                        if (userSuppliers.Count != 0)
                        {
                            CompanyNew owner = userSuppliers.First();
                            XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Owner.Oid", owner.Oid)));
                            ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalogs(cat, supplierCatalogs).OrderBy(x => x.ShowOrder);

                        }
                        else if (userCustomers.Count != 0)
                        {
                            Customer cust = userCustomers.First();
                            Store store = cust.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);
                            ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalog(cat, PriceCatalogHelper.GetPriceCatalogPolicy(store, cust)).OrderBy(x => x.ShowOrder);
                        }
                    }
                    ViewData["rootid"] = root.Oid;
                }


            }
            return PartialView("ItemsOfNode");
        }

        public ActionResult AllowedItems(string CategoryID)
        {
            Session["AddItemsFilter"] = Session["AddItemsFilter"] ?? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            if (Request["DXCallbackArgument"].Contains("CLEARFILTERS"))
            {
                Session["AddItemsFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            }
            else
            {
                string Fcode = Request["Fcode"] == null || Request["Fcode"] == "null" ? "" : Request["Fcode"];
                string Fname = Request["Fname"] == null || Request["Fname"] == "null" ? "" : Request["Fname"];
                string FBarcode = Request["Fbarcode"] == null || Request["Fbarcode"] == "null" ? "" : Request["Fbarcode"];
                string Factive = Request["Factive"] == null || Request["Factive"] == "null" ? "" : Request["Factive"];
                string FitemSupplier = Request["FitemSupplier"] == null || Request["FitemSupplier"] == "null" ? "" : Request["FitemSupplier"];
                string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];
                string Fbuyer = Request["Fbuyer"] == null || Request["Fbuyer"] == "null" ? "" : Request["Fbuyer"];
                string Fseasonality = Request["Fseasonality"] == null || Request["Fseasonality"] == "null" ? "" : Request["Fseasonality"];
                string Fmothercode = Request["Fmothercode"] == null || Request["Fmothercode"] == "null" ? "" : Request["Fmothercode"];

                if (OwnerApplicationSettings != null && OwnerApplicationSettings.PadBarcodes)
                {
                    FBarcode = FBarcode != "" && !FBarcode.Contains("*") ? FBarcode.PadLeft(OwnerApplicationSettings.BarcodeLength, OwnerApplicationSettings.BarcodePaddingCharacter[0]) : FBarcode;
                }

                if (OwnerApplicationSettings != null && OwnerApplicationSettings.PadItemCodes)
                {
                    Fcode = Fcode != "" && !Fcode.Contains("*") ? Fcode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fcode;
                    Fmothercode = Fmothercode != "" && !Fmothercode.Contains("*") ? Fmothercode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fmothercode;
                }


                CriteriaOperator codeFilter = null;
                if (Fcode != null && Fcode.Trim() != "")
                {
                    if (Fcode.Replace('%', '*').Contains("*"))
                    {
                        codeFilter = new BinaryOperator("Code", Fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                    }
                    else
                    {
                        codeFilter = new BinaryOperator("Code", Fcode, BinaryOperatorType.Equal);
                    }
                }

                CriteriaOperator nameFilter = null;
                if (Fname != null && Fname.Trim() != "")
                {
                    nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), Fname);
                    // new BinaryOperator("Name", "%" + Fname + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator barfilter = null;
                if (FBarcode != null && FBarcode.Trim() != "")
                {
                    if (FBarcode.Replace('%', '*').Contains("*"))
                    {
                        barfilter = new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Code", FBarcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like));
                    }
                    else
                    {
                        barfilter = new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Code", FBarcode, BinaryOperatorType.Equal));
                    }
                }

                CriteriaOperator activefilter = null;
                if (Factive == "0" || Factive == "1")
                {
                    activefilter = CriteriaOperator.Parse("IsActive=" + Factive, "");
                }

                CriteriaOperator itemSupplierFilter = null;
                if (FitemSupplier != null && FitemSupplier.Trim() != "")
                {
                    itemSupplierFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("DefaultSupplier.CompanyName"), FitemSupplier);
                    // new BinaryOperator("DefaultSupplier.CompanyName", "%" + FitemSupplier + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator createdOnFilter = null;
                if (FcreatedOn != "")
                {
                    createdOnFilter = CriteriaOperator.Or(new BinaryOperator("InsertedDate", DateTime.Parse(FcreatedOn), BinaryOperatorType.GreaterOrEqual));
                }

                CriteriaOperator updatedOnFilter = null;
                if (FupdatedOn != "")
                {
                    updatedOnFilter = CriteriaOperator.Or(new BinaryOperator("UpdatedOn", DateTime.Parse(FupdatedOn), BinaryOperatorType.GreaterOrEqual));
                }

                CriteriaOperator buyerFilter = null;
                if (Fbuyer != null && Fbuyer.Trim() != "")
                {
                    buyerFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Buyer.Description"), Fbuyer);
                    // new BinaryOperator("Buyer.Description", "%" + Fbuyer + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator seasonalityFilter = null;
                if (Fseasonality != null && Fseasonality.Trim() != "")
                {
                    seasonalityFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Seasonality.Description"), Fseasonality);
                    // new BinaryOperator("Seasonality.Description", "%" + Fseasonality + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator mothercodeFilter = null;
                if (Fmothercode != null && Fmothercode.Trim() != "")
                {
                    if (Fmothercode.Replace('%', '*').Contains("*"))
                    {
                        mothercodeFilter = new BinaryOperator("MotherCode.Code", Fmothercode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                    }
                    else
                    {
                        mothercodeFilter = new BinaryOperator("MotherCode.Code", Fmothercode, BinaryOperatorType.Equal);
                    }
                }
                Session["AddItemsFilter"] = CriteriaOperator.And(codeFilter, nameFilter, barfilter, activefilter, itemSupplierFilter, createdOnFilter, updatedOnFilter, buyerFilter, seasonalityFilter, mothercodeFilter);

            }
            ViewData["CategoryID"] = CategoryID;
            XPCollection<Item> items = new XPCollection<Item>(this.XpoSession, CriteriaOperator.And(getAllowedItems(CategoryID), (CriteriaOperator)Session["AddItemsFilter"]));
            //items.Criteria = CriteriaOperator.And(items.Criteria, (CriteriaOperator)Session["AddItemsFilter"]);
            return PartialView("AllowedItems", items);
        }

        public ActionResult AddItemsToCatecory(string ID)
        {
            if (!TableCanInsert || !TableCanUpdate)
            {
                return RedirectToAction("E404", "Error");
            }
            ViewData["CategoryID"] = ID;
            //CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            //XPCollection<Item> items = getAllowedItems(ID);
            //items.Criteria = CriteriaOperator.And(items.Criteria, filter);
            return PartialView("AddItemsToCatecory", new List<Item>());
        }



        public JsonResult AddItemsToCategory(string CategoryID, string[] data)
        {
            if (data != null)
            {
                Guid CategoryOid;
                if (!Guid.TryParse(CategoryID, out CategoryOid))
                {
                    return Json(new { success = false });
                }

                ItemCategory currentCategory = XpoSession.FindObject<ItemCategory>(new BinaryOperator("Oid", CategoryOid));

                foreach (string itmoid in data)
                {

                    Guid ItemOid;
                    if (Guid.TryParse(itmoid, out ItemOid))
                    {

                        Item item = XpoSession.FindObject<Item>(new BinaryOperator("Oid", ItemOid));
                        ItemAnalyticTree tr = new ItemAnalyticTree(XpoSession);
                        tr.Node = currentCategory;
                        tr.Object = item;
                        tr.Root = currentCategory.GetRoot(currentCategory.Session);
                        tr.Save();
                    }
                }
                XpoHelper.CommitTransaction(XpoSession);
            }
            return Json(new { success = true });
        }

        private CriteriaOperator getAllowedItems(string CategoryID)
        {
            //XPCollection<Item> allowedItems;
            CriteriaOperator notAllowedItemsCriteria;
            Guid CategoryOid;
            ViewData["CategoryID"] = CategoryID;
            if (!Guid.TryParse(CategoryID, out CategoryOid))
            {
                ViewData["CategoryName"] = "Λανθασμένη κατηγορία";
                return new BinaryOperator("Oid", Guid.Empty);
            }
            else
            {
                ItemCategory currentCategory, root;
                currentCategory = XpoSession.FindObject<ItemCategory>(new BinaryOperator("Oid", CategoryOid));
                ViewData["CategoryName"] = currentCategory.Description;
                root = currentCategory.GetRoot(XpoSession) as ItemCategory;
                notAllowedItemsCriteria = root.GetAllNodeTreeFilter();
                return new NotOperator(notAllowedItemsCriteria);
            }
        }

        [HttpPost]
        public ActionResult InlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] ItemAnalyticTree ct)
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
                Session["Error"] = Resources.AnErrorOccurred;

            //FillLookupComboBoxes();

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
            ItemCategory cat = XpoHelper.GetNewSession().FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            ItemCategory root = (ItemCategory)cat.GetRoot(XpoHelper.GetNewSession());
            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["ItemAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>().OrderBy(x => x.ShowOrder);
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalogs(cat, supplierCatalogs).OrderBy(x => x.ShowOrder);

                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First();
                    if (UserHelper.IsCustomer(currentUser) && this.CurrentStore == null) //no store selected
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("ItemsOfNode");
                    }
                    Store store = cust.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalog(cat, PriceCatalogHelper.GetPriceCatalogPolicy(store, cust)).OrderBy(x => x.ShowOrder);
                }
            }
            return PartialView("ItemsOfNode");
        }
        [HttpPost]
        public ActionResult InlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] ItemAnalyticTree ct)
        {
            if (!TableCanUpdate)
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
                Session["Error"] = Resources.AnErrorOccurred;

            //FillLookupComboBoxes();

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            ItemCategory cat = XpoHelper.GetNewSession().FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + categoryid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["ItemAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>().OrderBy(x => x.ShowOrder);
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalogs(cat, supplierCatalogs).OrderBy(x => x.ShowOrder);

                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First();
                    if (this.CurrentStore == null) //no store selected
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("ItemsOfNode");
                    }
                    Store store = cust.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalog(cat, PriceCatalogHelper.GetPriceCatalogPolicy(store, cust)).OrderBy(x => x.ShowOrder);//cust.GetPriceCatalog(store));
                }
            }
            return PartialView("ItemsOfNode");
        }
        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] ItemAnalyticTree ct)
        {
            if (!TableCanDelete)
                return null;

            GenerateUnitOfWork();
            try
            {
                DeleteT(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            //FillLookupComboBoxes();

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
            ItemCategory cat = XpoHelper.GetNewSession().FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
            ViewBag.CatDescription = cat != null ? cat.Description : "";
            ItemCategory root = (ItemCategory)cat.GetRoot(XpoHelper.GetNewSession());

            User currentUser = CurrentUser;
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
            if (Boolean.Parse(Session["IsAdministrator"].ToString()))
            {
                ViewData["ItemAnalyticTree"] = cat.GetAllNodeTreeItems<ItemAnalyticTree>().OrderBy(x => x.ShowOrder);
            }
            else
            {
                if (userSuppliers.Count != 0)
                {
                    CompanyNew supl = userSuppliers.First();
                    XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Supplier", supl.Oid)));
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalogs(cat, supplierCatalogs).OrderBy(x => x.ShowOrder);

                }
                else if (userCustomers.Count != 0)
                {
                    Customer cust = userCustomers.First<Customer>();
                    if (this.CurrentStore == null) //no store selected
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return PartialView("ItemsOfNode");
                    }
                    Store store = cust.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);
                    ViewData["ItemAnalyticTree"] = TreeHelper.GetAllNodeTreeItemsOfCatalog(cat, PriceCatalogHelper.GetPriceCatalogPolicy(store, cust)).OrderBy(x => x.ShowOrder); //cust.GetPriceCatalog(store));
                }
            }
            return PartialView("ItemsOfNode");
        }

        protected void UpdateLookupObjectsT(ItemAnalyticTree a)
        {
            ItemCategory node = GetObjectByArgument<ItemCategory>(a.Session, "NodeComboBoxID");
            Item item = GetObjectByArgument<Item>(a.Session, "ObjectComboBoxID");
            if (node != null && item != null)
            {
                base.UpdateLookupObjects(a);
                a.Node = node;
                a.Root = GetObjectByArgument<ItemCategory>(a.Session, "RootComboBoxID");
                a.Object = item;
            }
        }

        public ActionResult TreeView()
        {
            return PartialView("TreeView");
        }

        public ActionResult TreeViewCallback()
        {
            return PartialView("TreeViewCallback");
        }

        public ActionResult TreeViewPopup()
        {
            return PartialView();
        }

        public ActionResult TreeViewPopupPartial()
        {
            return PartialView();
        }

        public JsonResult jsonChangeCategory()
        {
            if (!TableCanUpdate) return null;

            try
            {
                Guid ItemAnalyticTreeID = (Request["AnalyticTreeID"] == null || Request["AnalyticTreeID"] == "null" || Request["AnalyticTreeID"] == "-1") ? Guid.Empty : Guid.Parse(Request["AnalyticTreeID"]);
                Guid CategoryID = (Request["CategoryID"] == null || Request["CategoryID"] == "null" || Request["CategoryID"] == "-1") ? Guid.Empty : Guid.Parse(Request["CategoryID"]);
                ItemAnalyticTree iat = XpoSession.FindObject<ItemAnalyticTree>(new BinaryOperator("Oid", ItemAnalyticTreeID, BinaryOperatorType.Equal));
                if (iat != null)
                {
                    iat.Node = XpoSession.FindObject<CategoryNode>(new BinaryOperator("Oid", CategoryID, BinaryOperatorType.Equal));
                    iat.Root = iat.Node.GetRoot(XpoSession);

                    iat.Save();

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
    }
}
