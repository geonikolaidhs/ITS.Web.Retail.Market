using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Linq;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform;
using ITS.Retail.WebClient.ViewModel;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Attributes;
using System.Threading;
using System.Globalization;
using System.Reflection;
using ITS.Retail.Platform.Enumerations;
using System.Net;
using System.Text;

namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    [StoreControllerEditable]
    public class CustomerController : BaseObjController<Customer>
    {

        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return new Dictionary<PropertyInfo, string>()
                {
                    { typeof(Customer).GetProperty("VatLevel"), "VatLevel_VI" },
                    { typeof(Customer).GetProperty("PaymentMethod"), "PaymentMethod_VI" },
                    { typeof(Customer).GetProperty("RefundStore"), "RefundStore_VI" },
                    { typeof(Customer).GetProperty("PriceCatalogPolicy"), "PriceCatalogPolicy_VI" },
                    { typeof(StorePriceList).GetProperty("PriceList"), "PriceCatalogKey_VI" },
                    { typeof(StorePriceList).GetProperty("Store"), "StoreKey_VI" }
                };
            }
        }


        [HttpGet]
        public ActionResult CreateTransactionPoints()
        {
            DocumentSeries series = XpoSession.GetObjectByKey<DocumentSeries>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentSeriesOid);
            DocumentType docType = XpoSession.GetObjectByKey<DocumentType>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentTypeOid);
            DocumentStatus docStatus = XpoSession.GetObjectByKey<DocumentStatus>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentStatusOid);

            this.ToolbarOptions.ForceVisible = false;

            if (DocumentHelper.PointDocumentSettingsHaveBeenSet(EffectiveOwner.OwnerApplicationSettings) == false)
            {
                Session["Error"] = Resources.PleaseSetPointsDocumentTypeSeriesAndStatus;
            }
            return View(new CustomerPointTransaction() { Oid = Guid.NewGuid(), DateTime = DateTime.Now, User = CurrentUser.Oid });
        }

        [HttpPost]
        public ActionResult PointTransaction([ModelBinder(typeof(RetailModelBinder))]CustomerPointTransaction model)
        {
            this.ToolbarOptions.ForceVisible = false;
            if (DocumentHelper.PointDocumentSettingsHaveBeenSet(EffectiveOwner.OwnerApplicationSettings) == false)
            {
                Session["Error"] = Resources.PleaseSetPointsDocumentTypeSeriesAndStatus;
                model.Customer = null;
            }
            else if (ModelState.IsValid)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Customer customer = uow.GetObjectByKey<Customer>(model.Customer);
                    User user = uow.GetObjectByKey<User>(model.User);

                    DocumentSeries series = uow.GetObjectByKey<DocumentSeries>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentSeriesOid);
                    DocumentType docType = uow.GetObjectByKey<DocumentType>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentTypeOid);
                    DocumentStatus docStatus = uow.GetObjectByKey<DocumentStatus>(EffectiveOwner.OwnerApplicationSettings.PointsDocumentStatusOid);

                    Store store = uow.GetObjectByKey<Store>(this.CurrentStore.Oid);
                    DocumentHeader documentHeader = new DocumentHeader(uow)
                    {
                        Oid = model.Oid,
                        TotalPoints = model.Points,
                        Store = store,
                        Customer = customer,
                        CreatedBy = user,
                        DocumentType = docType,
                        DocumentSeries = series,
                        Status = docStatus,
                        Division = docType.Division.Section
                    };
                    documentHeader.Save();
                    XpoHelper.CommitChanges(uow);
                }
                Session["Notice"] = Resources.SuccesfullySaved;
                return new RedirectResult("~/Customer/CreateTransactionPoints");
            }
            return View("CreateTransactionPoints", model);
        }

        public ActionResult CustomerComboBox()
        {
            return PartialView();
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                TraderHelper.LoadViewPopup<Customer>(ViewData["ID"].ToString(), ViewData);
            }
            ActionResult rt = PartialView("LoadViewPopup");
            return rt;

        }

        public override ActionResult Grid()
        {
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ViewData["CallbackMode"] = "";
            
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") && UserCanEditRequest() == false)
            {
                Session["Error"] = Resources.YouCannotEditThisElement;
                return null;
            }
            if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                ViewData["CallbackMode"] = "DELETESELECTED";
                if (TableCanDelete)
                {
                    List<Guid> oids = new List<Guid>();
                    string allOids = Request["DXCallbackArgument"].Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
                    string[] unparsed = allOids.Split(',');
                    foreach (string unparsedOid in unparsed)
                    {
                        oids.Add(Guid.Parse(unparsedOid));
                    }
                    if (oids.Count > 0)
                    {
                        try
                        {
                            DeleteAll(XpoHelper.GetNewUnitOfWork(), oids);
                        }
                        catch (ConstraintViolationException)
                        {
                            Session["Error"] = Resources.CannotDeleteObject;
                        }
                        catch (Exception e)
                        {
                            Session["Error"] = e.Message;
                        }
                    }
                }
                else
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
                return PartialView("Grid", new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer))));
            }
            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {

                if (Session["CustomerFilter"] == null)
                {
                    Session["CustomerFilter"] = new BinaryOperator("Oid", Guid.Empty);
                }
                return PartialView("Grid", new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer))));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
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
                    // new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }

                if (taxn != "")
                {
                    taxnFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), taxn);
                    // new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
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

                criteria = CriteriaOperator.And(codeFilter,
                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                taxnFilter,
                                                cardidFilter,
                                                CreateCriteria(lcod, "Loyalty"),
                                                activeFilter, createdOnFilter, updatedOnFilter
                                                );

                if ((bool)Session["IsAdministrator"] == false)
                {
                    CompanyNew owner;
                    XPCollection<CompanyNew> sup = BOApplicationHelper.GetUserEntities<CompanyNew>(XpoHelper.GetNewUnitOfWork(), CurrentUser);
                    owner = sup[0];
                    CriteriaOperator permissions = new BinaryOperator("Owner.Oid", owner.Oid);
                    criteria = CriteriaOperator.And(criteria, permissions);
                }

                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }
            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            }
            Session["CustomerFilter"] = criteria;

            var tmp = new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["CustomerFilter"], typeof(Customer)));

            string CustomerID = Request.Params["CustomerID"];
            Guid CustomerGuid;

            if (!Guid.TryParse(CustomerID, out CustomerGuid))
            {
                CustomerGuid = Guid.Empty;
            }

            Session["CurrentElement"] = ViewData["CurrentElement"];
            return PartialView("Grid", tmp.AsQueryable());
        }

        public JsonResult jsonAddNewCustomerFromTrader()
        {
            string TraderId = Request["Oid"];
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + TraderId + "'", "");
            Trader trd = uow.FindObject<Trader>(filter);
            return Json(new { result = true });
        }

        public JsonResult jsonCheckForDuplicateCategory()
        {
            UnitOfWork uow = ((Customer)Session["Customer"]).Session as UnitOfWork;
            Guid SelectedNodeID = Request["SelectedNodeID"] != null || Request["SelectedNodeID"] != "null" ? Guid.Parse(Request["SelectedNodeID"]) : Guid.Empty;
            CustomerCategory SelectedNode = uow.FindObject<CustomerCategory>(new BinaryOperator("Oid", SelectedNodeID, BinaryOperatorType.Equal));

            Customer customer = (Customer)Session["Customer"];
            foreach (CustomerAnalyticTree cat in customer.CustomerAnalyticTrees)
            {
                if (SelectedNode.GetRoot(SelectedNode.Session) == cat.Root)
                {
                    return Json(new
                    {
                        hasDuplicate = true
                    });
                }
            }

            return Json(new
            {
                hasDuplicate = false
            });
        }


        public ActionResult UpdateAddressComboBox(string TraderID)
        {
            Trader trader = ((Customer)Session["Customer"]).Trader;
            FillLookupComboBoxes();
            ViewData["TraderID"] = TraderID;
            return PartialView("UpdateAddressComboBox", ((Customer)Session["Customer"]));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.FilterButton.Visible = true;
            ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.ShowHideMenu.Visible = false;
            this.ToolbarOptions.VariableValuesButton.Visible = this.GetType().GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null;
            this.ToolbarOptions.VariableValuesButton.OnClick = "VariableValuesDisplay.ShowVariableValuesPopUp";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "CustomerID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdCustomer");

            ViewData["Traders"] = new List<Trader>();
            ViewData["Addresses"] = new List<Address>();
            ViewData["displayCommands"] = false;
            Session["CustomerFilter"] = new BinaryOperator("Oid", Guid.Empty);

            return View(new List<Customer>().AsQueryable());
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult UpdateProfile()
        {
            this.ToolbarOptions.ForceVisible = false;
            Customer cust = UserHelper.GetCustomer(CurrentUser);
            ViewData["CustomerID"] = cust == null ? Guid.Empty : cust.Oid;
            ViewData["TraderID"] = cust == null ? Guid.Empty : cust.Trader.Oid;
            return View();
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            if (Session["IsNewCustomer"] != null && (bool)Session["IsNewCustomer"])
            {
                ViewBag.Title = Resources.NewCustomer;
            }
            else
            {
                ViewBag.Title = Resources.EditCustomerDetails;
            }

            return PartialView("LoadEditPopup");
        }

        public ActionResult LoadAssosiatedCustomerEditPopup()
        {
            Guid TraderID = Guid.Empty;

            if (Guid.TryParse(Request["TraderID"], out TraderID))
            {
                ViewData["TraderID"] = TraderID;
            }

            ViewBag.Title = Resources.EditCustomerDetails;

            ActionResult rt = PartialView("LoadAssosiatedCustomerEditPopup");
            return rt;
        }


        public ActionResult PopupAssosiatedEditCallbackPanel()
        {
            Guid TraderID = Guid.Empty;

            if (Guid.TryParse(Request["TraderID"], out TraderID))
            {
                ViewData["TraderID"] = TraderID;
            }

            return PartialView();
        }

        public ActionResult Edit(string Oid)
        {

            if (!this.TableCanInsert && !this.TableCanUpdate && Oid != null)
            {
                return new RedirectResult("~/Login");
            }
            ViewData["EditMode"] = true;
            ViewData["OwnEdit"] = false;
            Guid cguid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            Customer customer;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            if (cguid != Guid.Empty)
            {
                if (!this.TableCanUpdate)
                {
                    return new RedirectResult("~/Login");
                }
                customer = uow.GetObjectByKey<Customer>(cguid);
                if (customer != null)
                {
                    Session["IsNewCustomer"] = false;
                }
                else
                {
                    SupplierNew supplierFoundByCodeOrTaxCode = uow.GetObjectByKey<SupplierNew>(cguid);
                    if (supplierFoundByCodeOrTaxCode == null)
                    {
                        Session["Error"] = Resources.TraderNotFound;
                        return null;
                    }

                    customer = new Customer(uow);
                    customer.Trader = supplierFoundByCodeOrTaxCode.Trader;
                    customer.CompanyName = supplierFoundByCodeOrTaxCode.CompanyName;
                    customer.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                    Session["IsNewSupplier"] = true;
                }
            }
            else
            {
                if (!this.TableCanInsert)
                {
                    return new RedirectResult("~/Login");
                }
                customer = new Customer(uow);
                customer.Trader = new Trader(uow);
                customer.Trader.Customers.Add(customer);
                if (ApplicationHelper.IsMasterInstance() == false)
                {
                    customer.CreatedByDevice = StoreControllerAppiSettings.CurrentStoreOid.ToString();
                }
                Session["IsNewCustomer"] = true;
            }
            Session["NewTrader"] = true;
            Session["TraderType"] = "Customer";
            Session["TraderUow"] = uow;
            Session["Customer"] = customer;
            Session["Trader"] = customer.Trader;
            FillLookupComboBoxes();

            CustomerStorePriceList cpl = uow.FindObject<CustomerStorePriceList>(new BinaryOperator("Customer.Oid", customer.Oid));

            ViewData["CustomerStorePriceList"] = (cpl == null || cpl.StorePriceList == null) ? Guid.Empty : cpl.StorePriceList.Oid;
            ViewData["CustomerID"] = customer.Oid.ToString();
            ViewData["TraderID"] = customer.Trader.Oid.ToString();
            ViewData["Customer_ID"] = Oid;

            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            return PartialView("Edit", customer);
        }

        public JsonResult Save()
        {
            bool isOwnEdit = false;
            Guid cguid = Guid.Empty;
            bool correctCustomerGuid = Request["Customer_ID"] != null && Guid.TryParse(Request["Customer_ID"].ToString(), out cguid);
            if (correctCustomerGuid)
            {
                ViewData["OwnEdit"] = Request["isOwnEdit"];
                Boolean.TryParse(Request["isOwnEdit"], out isOwnEdit);
                if (isOwnEdit)
                {
                    XPCollection<Customer> cust = BOApplicationHelper.GetUserEntities<Customer>(XpoHelper.GetNewUnitOfWork(), CurrentUser);
                    if (cust.Count == 0 || cust[0].Oid != cguid)
                    {
                        Session["Error"] = Resources.WrongUserProfileModification;
                        return Json(new { error = Session["Error"] });
                    }
                }
                Customer ct = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Customer>(cguid);
                ViewData["Customer_ID"] = Request["Customer_ID"];
                if (ct == null)
                {
                    if (!this.TableCanInsert)
                    {
                        return Json(new { error = Resources.AnErrorOccurred });
                    }
                }
                else
                {
                    if (!this.TableCanUpdate)
                    {
                        return Json(new { error = Resources.AnErrorOccurred });
                    }
                }

                ct = (Customer)Session["Customer"];

                TryUpdateModel<Customer>(ct);
                TryUpdateModel<Trader>(ct.Trader);
                ct.Discount = ct.Discount / 100;

                if (!isOwnEdit)
                {
                    UpdateLookupObjects(ct);
                }

                Guid defAddress = Guid.Empty;
                if (Guid.TryParse(Request["DefaultAddress_VI"].ToString(), out defAddress) == true)
                {
                    ct.DefaultAddress = ct.Trader.Addresses.FirstOrDefault(g => g.Oid == defAddress);
                }

                Customer temp_ct = ct.Session.FindObject<Customer>(CriteriaOperator.And(new BinaryOperator("CardID", ct.CardID),
                                                                                        new BinaryOperator("Oid", ct.Oid, BinaryOperatorType.NotEqual)));

                if (temp_ct != null)
                {
                    Session["Error"] = Resources.CardIDExistsError;
                    ViewData["CustomerID"] = ct.Oid.ToString();
                    ViewData["TraderID"] = ct.Trader.Oid.ToString();
                    ViewData["Customer_ID"] = Request["Customer_ID"];
                    return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
                }
                else if (ct.Trader.Addresses.Count == 0)
                {
                    Session["Error"] = Resources.NoAddressInserted;
                    ViewData["CustomerID"] = ct.Oid.ToString();
                    ViewData["TraderID"] = ct.Trader.Oid.ToString();
                    ViewData["Customer_ID"] = Request["Customer_ID"];
                    return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
                }
                else
                {
                    AssignOwner(ct);
                    UpdateLookupObjects(ct);
                    string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                    try
                    {
                        if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
                        {
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
                            {
                                webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                                webService.Url = StoreControllerAppiSettings.MasterServiceURL;
                                string toSend = ct.FullJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, false), message;
                                if (webService.InsertOrUpdateRecord(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid, "Customer", toSend, out message) == false)
                                {
                                    Session["Error"] = "Remote Error" + message;
                                    return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
                                }
                                var v = ct.Session.GetObjectsToSave().Cast<object>().Where(G => G is BaseObj).ToList();
                                foreach (BaseObj o in v)
                                {
                                    if (o.IsDeleted)
                                    {
                                        webService.DeleteRecord(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid,
                                            o.GetType().Name,
                                            o.Oid,
                                            out message
                                            );
                                    }
                                }
                                ct.Save();
                                XpoHelper.CommitTransaction(ct.Session);
                            }
                        }
                        else
                        {
                            ct.Save();
                            XpoHelper.CommitTransaction(ct.Session);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewData["CustomerID"] = ct.Oid.ToString();
                        ViewData["TraderID"] = ct.Trader.Oid.ToString();
                        ViewData["Customer_ID"] = Request["Customer_ID"];
                        Session["Error"] = ex.GetFullMessage();
                        ViewBag.VatLevelComboBox = GetList<VatLevel>(XpoHelper.GetNewUnitOfWork());
                        return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                    }
                    Session["NewTrader"] = null;
                    Session["TraderType"] = null;
                    Session["TraderUow"] = null;
                    Session["Customer"] = null;
                    Session["Trader"] = null;
                }
            }

            SessionHelper.ReloadSessionCommonItems();

            if (isOwnEdit)
            {
                return Json(new { error = Resources.AnErrorOccurred });
            }
            return Json(new { });
        }


        [Security(OverrideSecurity = true)]
        public ActionResult GetTraderDescription(string CustomerID, bool isOwnEdit = false)
        {
            ViewData["isOwnEdit"] = isOwnEdit;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            TraderHelper.GetTraderDescription<Customer>(CustomerID, ViewData, uow);
            Customer customer = null;
            Guid CustomerGuid = Guid.Empty;
            ViewData["priceCatalogName"] = null;
            if (Guid.TryParse(CustomerID, out CustomerGuid))
            {
                customer = uow.GetObjectByKey<Customer>(CustomerGuid);
            }

            string phones = null;


            ViewData["StorePhones"] = String.IsNullOrWhiteSpace(phones) ? "" : phones.Substring(0, phones.Length - 3);

            return PartialView("GetTraderDescription", customer);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (Session["TraderUow"] != null)
            {
                (Session["TraderUow"] as UnitOfWork).Dispose();
            }
            Session["NewTrader"] = Session["TraderType"] = Session["TraderUow"] = Session["Customer"] = Session["Trader"] = null;
            return null;
        }

        public JsonResult UpdateTraderData(string sender, string Code, string TaxCode)
        {
            Customer customer = null;
            SupplierNew supplier = null;
            Trader trader = null;
            Trader currentObjectTrader = (Session["Trader"] as Trader);
            try
            {
                UnitOfWork uow = currentObjectTrader.Session as UnitOfWork;
                if (sender == "TaxCode" && !string.IsNullOrEmpty(TaxCode))
                {
                    trader = uow.FindObject<Trader>(new BinaryOperator("TaxCode", TaxCode));
                    if (trader != null)
                    {
                        customer = trader.Customers.FirstOrDefault();
                        if (customer == null)
                        {
                            supplier = trader.Suppliers.Where(supli => supli.Owner.Oid == EffectiveOwner.Oid).FirstOrDefault();
                        }
                    }
                    //else
                    //{
                    //    try
                    //    {
                    //        //Check with GSRT webservice
                    //        using (RgWsBasStoixN service = new RgWsBasStoixN())
                    //        {
                    //            RgWsBasStoixEpitRtUser basicInfo = new RgWsBasStoixEpitRtUser();
                    //            decimal sequence = 0;
                    //            GenWsErrorRtUser error = new GenWsErrorRtUser();
                    //            service.rgWsBasStoixEpit(TaxCode, ref basicInfo, ref sequence, ref error);
                    //        }
                    //    }
                    //    catch (Exception)
                    //    {
                    //        return Json(new { Error = "" });
                    //    }
                    //}
                }
                else if (sender == "Code" && !string.IsNullOrEmpty(Code))
                {
                    customer = FindObjectWithOwner<Customer>(uow, new BinaryOperator("Code", Code));
                    if (customer != null)
                    {
                        trader = customer.Trader;
                    }
                    else
                    {
                        supplier = FindObjectWithOwner<SupplierNew>(uow, new BinaryOperator("Code", Code));
                        if (supplier != null)
                        {
                            trader = supplier.Trader;
                        }
                    }
                }

                if (trader == null)
                {
                    return Json(new { Error = "", NoDuplicateFound = true });
                }
                else if (trader.Oid == currentObjectTrader.Oid)
                {
                    return Json(new { Error = "", NoDuplicateFound = true });
                }
                else
                {
                    string confirm_message = Resources.TraderFoundWithSameCode + Environment.NewLine + trader.Code + Environment.NewLine
                                             + String.Format(Resources.WouldYouLikeToOpenTheCurrentTrader, Resources.Trader);
                    return Json(new
                    {
                        TraderID = trader.Oid,
                        TraderFirstName = trader.FirstName,
                        TraderLastName = trader.LastName,
                        confirm_message = confirm_message,
                        triggered_by = sender,
                        supplier_id = supplier == null ? string.Empty : supplier.Oid.ToString(),
                        customer_id = customer == null ? string.Empty : customer.Oid.ToString(),
                        controller = "customer"
                    });
                }
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.GetFullMessage();
                return Json(new { Error = "", NoDuplicateFound = true });
            }
        }

        public ActionResult AssociateNewCustomerWithTrader(string TraderID)
        {
            if (!this.TableCanInsert)
            {
                return new RedirectResult("~/Login");
            }
            Customer ct;

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Guid TraderGuid;
            if (!Guid.TryParse(TraderID, out TraderGuid))
                return PartialView("Edit", Session["Customer"] as Customer);
            Trader t = uow.FindObject<Trader>(new BinaryOperator("Oid", TraderGuid));
            if (t == null)
                return PartialView("Edit", Session["Customer"] as Customer);
            ct = new Customer(uow);
            if (t.Companies.Count > 0)
            {
                ct.CompanyName = t.Companies.First().CompanyName;
                ct.Profession = t.Companies.First().Profession;
                ct.DefaultAddress = t.Companies.First().DefaultAddress;
            }
            t.Customers.Add(ct);
            Session["NewTrader"] = true;
            Session["TraderType"] = "Customer";
            Session["TraderUow"] = uow;
            Session["Customer"] = ct;
            Session["Trader"] = ct.Trader;
            ViewData["CustomerID"] = ct.Oid;
            ViewData["TraderID"] = t.Oid;
            ViewData["OwnEdit"] = false;


            ViewData["Customer_ID"] = ct.Oid;

            FillLookupComboBoxes();
            return PartialView("Edit", ct);
        }


        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.AddressComboBox = Session["Trader"] != null ? ((Trader)Session["Trader"]).Addresses : null;
            ViewBag.UserComboBox = new XPCollection<User>(XpoHelper.GetNewUnitOfWork());
            ViewBag.VatLevelComboBox = GetList<VatLevel>(XpoHelper.GetNewUnitOfWork());
            ViewBag.PriceCatalogPolicies = GetList<PriceCatalogPolicy>(XpoHelper.GetNewUnitOfWork());
            ViewBag.TaxOfficeComboBox = GetList<TaxOffice>(XpoHelper.GetNewUnitOfWork());
            ViewBag.PaymentMethodComboBox = GetList<PaymentMethod>(XpoHelper.GetNewUnitOfWork());
            ViewBag.RefundStoreComboBox = GetList<Store>(XpoHelper.GetNewUnitOfWork());
        }

        [HttpPost]
        public ActionResult GetStorePriceLists()
        {
            string StoreGUID = (Request["StoreGUID"] == null) ? "" : Request["StoreGUID"].ToString();
            Guid storeguid = Guid.Empty;
            Guid.TryParse(StoreGUID, out storeguid);
            Store xpoSessionFindObject = XpoSession.FindObject<Store>(new BinaryOperator("Oid", storeguid));
            ViewBag.StorePriceLists = xpoSessionFindObject == null ? null : xpoSessionFindObject.StorePriceLists;
            ViewData["OwnEdit"] = false;
            return PartialView();
        }

        [HttpPost]
        public ActionResult CustomerAnalyticTreeAddNewPartial([ModelBinder(typeof(RetailModelBinder))] CustomerAnalyticTree ct)
        {
            ViewData["EditMode"] = true;
            UnitOfWork uow = ((Customer)Session["Customer"]).Session as UnitOfWork;
            Customer customer = (Customer)Session["Customer"];
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SelectedNodeID = Request["SelectedNodeID"] != null || Request["SelectedNodeID"] != "null" ? Guid.Parse(Request["SelectedNodeID"]) : Guid.Empty;
                    CustomerAnalyticTree cat = new CustomerAnalyticTree(uow);
                    cat.GetData(ct);
                    cat.Node = cat.Session.FindObject<CustomerCategory>(new BinaryOperator("Oid", SelectedNodeID, BinaryOperatorType.Equal));
                    cat.Object = customer;
                    cat.Root = cat.Node.GetRoot(cat.Session);
                    customer.CustomerAnalyticTrees.Add(cat);
                    Session["Customer"] = customer;
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

            FillLookupComboBoxes();
            return PartialView("CustomerAnalyticTreeGrid", customer.CustomerAnalyticTrees);
        }

        [HttpPost]
        public ActionResult CustomerAnalyticTreeDeletePartial([ModelBinder(typeof(RetailModelBinder))] CustomerAnalyticTree ct)
        {
            ViewData["EditMode"] = true;
            UnitOfWork uow = ((Customer)Session["Customer"]).Session as UnitOfWork;
            Customer customer = (Customer)Session["Customer"];
            try
            {

                foreach (CustomerAnalyticTree cat in customer.CustomerAnalyticTrees)
                {
                    if (cat.Oid == ct.Oid)
                    {
                        customer.CustomerAnalyticTrees.Remove(cat);
                        cat.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("CustomerAnalyticTreeGrid", customer.CustomerAnalyticTrees);
        }

        public ActionResult CustomerAnalyticTreeGrid(string CustomerID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (editMode == true)  //edit mode
            {
                FillLookupComboBoxes();
                return PartialView("CustomerAnalyticTreeGrid", ((Customer)Session["Customer"]).CustomerAnalyticTrees);
            }
            else  //view mode
            {
                Guid CustomerGuid = (CustomerID == null || CustomerID == "null" || CustomerID == "-1") ? Guid.Empty : Guid.Parse(CustomerID);
                Customer customer = XpoHelper.GetNewUnitOfWork().FindObject<Customer>(new BinaryOperator("Oid", CustomerGuid, BinaryOperatorType.Equal));
                ViewData["CustomerID"] = CustomerID;
                return PartialView("CustomerAnalyticTreeGrid", customer.CustomerAnalyticTrees);
            }
        }

        [HttpPost]
        public ActionResult CustomerStorePriceListUpdate([ModelBinder(typeof(RetailModelBinder))] CustomerStorePriceList ct)
        {
            Customer customer = (Customer)Session["Customer"];

            Guid storeGuid, priceCatalogGuid;

            if (customer != null && Guid.TryParse(Request["StoreKey_VI"], out storeGuid) && Guid.TryParse(Request["PriceCatalogKey_VI"], out priceCatalogGuid))
            {
                CustomerStorePriceList customerStorePriceList = customer.CustomerStorePriceLists.FirstOrDefault(cspl => cspl.Oid == ct.Oid) ??
                                                        new CustomerStorePriceList(customer.Session);
                customerStorePriceList.GetData(ct, new List<string>() { "Session" });

                customerStorePriceList.StorePriceList = customer.Session.FindObject<StorePriceList>(CriteriaOperator.And(new BinaryOperator("Store.Oid", storeGuid), new BinaryOperator("PriceList.Oid", priceCatalogGuid)));

                if (customerStorePriceList.StorePriceList == null)
                {
                    new StorePriceList(customer.Session);
                    UpdateLookupObjects(customerStorePriceList.StorePriceList);
                }

                string modelKey;
                string modelError = CustomerHelper.CheckedCustomerStorePriceLists(customer, customerStorePriceList, out modelKey);
                if (String.IsNullOrWhiteSpace(modelError))
                {
                    customerStorePriceList.Customer = customer;
                }
                else
                {
                    ModelState.AddModelError(modelKey, modelError);
                    ViewBag.CurrentItem = customerStorePriceList;
                    ViewBag.Stores = GetList<Store>(customer.Session).OrderBy(store => store.Name);
                    ViewBag.PriceCatalogs = customerStorePriceList.StorePriceList.Store.StorePriceLists.Where(spl => spl.PriceList != null).Select(g => g.PriceList);
                }
            }

            ViewData["displayCommands"] = true;
            ViewData["CustomerID"] = customer.Oid;
            return PartialView("CustomerStorePriceListGrid", customer.CustomerStorePriceLists);
        }

        [HttpPost]
        public ActionResult CustomerStorePriceListDelete([ModelBinder(typeof(RetailModelBinder))] CustomerStorePriceList ct)
        {
            Customer customer = (Customer)Session["Customer"];

            CustomerStorePriceList customerStorePriceList = customer.CustomerStorePriceLists.FirstOrDefault(g => g.Oid == ct.Oid);
            if (customerStorePriceList != null)
            {
                customerStorePriceList.Delete();
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }
            ViewData["displayCommands"] = true;
            ViewData["CustomerID"] = customer.Oid;
            FillLookupComboBoxes();
            return PartialView("CustomerStorePriceListGrid", customer.CustomerStorePriceLists);
        }

        public ActionResult CustomerStorePriceListGrid(bool displayCommands, string customerID)
        {
            Customer customer;
            if (displayCommands)
            {
                customer = (Customer)Session["Customer"];
            }
            else
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                Guid customerGuid;
                if (!Guid.TryParse(customerID, out customerGuid))
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
                customer = uow.GetObjectByKey<Customer>(customerGuid);
            }

            ViewData["displayCommands"] = displayCommands;
            ViewData["CustomerID"] = customer.Oid;

            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                string editOid = Request["DXCallbackArgument"].Split('|').Last().Trim().Trim(';');
                Guid editGuid;
                if (Guid.TryParse(editOid, out editGuid))
                {
                    CustomerStorePriceList customerStorePriceList = customer.CustomerStorePriceLists.FirstOrDefault(g => g.Oid == editGuid);
                    if (customerStorePriceList != null)
                    {
                        ViewBag.Stores = EffectiveOwner.Stores;
                        ViewBag.PriceCatalogs = customerStorePriceList.StorePriceList.Store.StorePriceLists.Where(spl => spl.PriceList != null).Select(g => g.PriceList);
                    }
                }
            }
            else if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                ViewBag.Stores = GetList<Store>(customer.Session).OrderBy(store => store.Name);
            }
            FillLookupComboBoxes();
            return PartialView("CustomerStorePriceListGrid", customer.CustomerStorePriceLists);
        }


        public ActionResult UpdateStoreComboBox()
        {
            FillLookupComboBoxes();
            ViewBag.Stores = EffectiveOwner.Stores;
            ViewData["SelectedStore"] = null;
            return PartialView();
        }


        public ActionResult UpdatePriceCatalogComboBox()
        {
            FillLookupComboBoxes();

            Guid storeGuid;
            string storeGuidText;
            storeGuidText = Request["StoreID"].ToString();

            ViewBag.Stores = null;
            if (Guid.TryParse(storeGuidText, out storeGuid))
            {
                Store store = EffectiveOwner.Stores.FirstOrDefault(g => g.Oid == storeGuid);
                if (store != null)
                {
                    ViewBag.PriceCatalogs = store.StorePriceLists.Where(spl => spl.PriceList != null).Select(g => g.PriceList);
                }
            }
            return PartialView();
        }



        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return ExportToFile<Customer>(Session["CustomerGridSettings"] as GridViewSettings, (CriteriaOperator)Session["CustomerFilter"]);
        }

        public ActionResult CustomerTabView(bool displayCommands, string CustomerID, bool editMode)
        {
            ViewData["displayCommands"] = displayCommands;
            ViewBag.EditMode = editMode;
            Customer customer;
            Guid CustomerGuID;
            if (!Guid.TryParse(CustomerID, out CustomerGuID))
            {
                Session["Error"] = Resources.AnErrorOccurred;
                return PartialView(null);
            }

            if (!displayCommands)
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                customer = uow.GetObjectByKey<Customer>(CustomerGuID);
            }
            else
            {
                customer = (Customer)Session["Customer"];
            }
            ViewData["CustomerID"] = CustomerGuID;
            ViewData["TraderID"] = customer.Trader.Oid;
            ViewData["GDPREnabled"] = CurrentUser.Role.GDPREnabled;
            return PartialView(customer);
        }

        public static object TaxOfficeRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
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
            Customer ct = (System.Web.HttpContext.Current.Session["Customer"] as ITS.Retail.Model.Customer);
            if (ct != null)
            {
                UnitOfWork uow = ct.Session as UnitOfWork;
                CriteriaOperator crop = CriteriaOperator.Or(new BinaryOperator("Description", proccessed_filter, BinaryOperatorType.Like),
                                                            new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like));

                XPCollection<TaxOffice> searched_item_taxoffices = GetList<TaxOffice>(uow, crop, "Code");
                searched_item_taxoffices.SkipReturnedObjects = e.BeginIndex;
                searched_item_taxoffices.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

                return searched_item_taxoffices;
            }
            return null;
        }

        public ActionResult SelectTaxOffice()
        {
            return PartialView();
        }

        public ActionResult ChildGrid(bool displayCommands, string customerID)
        {
            Customer customer;
            if (displayCommands)
            {
                customer = (Customer)Session["Customer"];
            }
            else
            {
                Guid customerGuid;
                if (!Guid.TryParse(customerID, out customerGuid))
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
                customer = XpoSession.GetObjectByKey<Customer>(customerGuid);
            }

            ViewData["displayCommands"] = displayCommands;
            ViewData["CustomerID"] = customer.Oid;

            return PartialView(customer.CustomerChilds);
        }

        public ActionResult ChildGridAdd([ModelBinder(typeof(RetailModelBinder))] CustomerChild ct)
        {
            Customer customer = (Customer)Session["Customer"];
            ViewData["displayCommands"] = true;
            ViewData["CustomerID"] = customer.Oid;
            if (ModelState.IsValid == false)
            {
                return PartialView("ChildGrid", customer.CustomerChilds);
            }

            CustomerChild customerChild = new CustomerChild(customer.Session);
            customerChild.GetData(ct, new List<string>() { "Customer" });
            customerChild.Customer = customer;
            return PartialView("ChildGrid", customer.CustomerChilds);
        }

        public ActionResult ChildGridUpdate([ModelBinder(typeof(RetailModelBinder))] CustomerChild ct)
        {
            Customer customer = (Customer)Session["Customer"];
            ViewData["displayCommands"] = true;
            ViewData["CustomerID"] = customer.Oid;
            if (ModelState.IsValid == false)
            {
                return PartialView("ChildGrid", customer.CustomerChilds);
            }

            CustomerChild customerChild = customer.CustomerChilds.FirstOrDefault(x => x.Oid == ct.Oid);
            if (customerChild != null)
            {
                customerChild.GetData(ct, new List<string>() { "Customer" });
            }

            return PartialView("ChildGrid", customer.CustomerChilds);
        }

        public ActionResult ChildGridDelete([ModelBinder(typeof(RetailModelBinder))] CustomerChild ct)
        {
            Customer customer = (Customer)Session["Customer"];
            CustomerChild ct2 = customer.CustomerChilds.FirstOrDefault(x => x.Oid == ct.Oid);
            if (ct2 != null)
            {
                ct2.Delete();
            }
            ViewData["displayCommands"] = true;
            ViewData["CustomerID"] = customer.Oid;
            return PartialView("ChildGrid", customer.CustomerChilds);
        }

        public ActionResult TreeView()
        {
            return PartialView("../CustomerCategory/TreeView");
        }


        public ActionResult PreviewCustomerPoints()
        {
            this.ToolbarOptions.EditButton.Visible = false;
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.ToolbarOptions.NewButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.CustomButton.Name = "PointsCorrection";
            this.ToolbarOptions.CustomButton.Visible = true;
            this.ToolbarOptions.CustomButton.Title = Resources.PointDifference;
            this.ToolbarOptions.CustomButton.OnClick = "CorrectPoints";
            this.ToolbarOptions.CustomButton.CCSClass = "diff";
            IQueryable<Customer> customers = new XPQuery<Customer>(XpoSession).Where(x => x.CollectedPoints > 0 || x.DocumentHeaders.Sum(y => y.TotalPoints) > 0);
            List<CustomerViewModel> cvms = new List<CustomerViewModel>();
            cvms.Capacity = customers.Count();
            foreach (Customer customer in customers)
            {
                CustomerViewModel cvm = new CustomerViewModel();
                cvm.LoadPersistent(customer);
                IEnumerable<DocumentHeader> customerDocuments = customer.DocumentHeaders.Where(x => x.IsCanceled == false);
                cvm.ComputedPoints = customerDocuments.Sum(document => document.TotalPoints)
                                   - customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                cvm.TotalConsumedPoints = customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                cvm.TotalEarnedPoints = customerDocuments.Sum(document => document.TotalPoints);
                cvms.Add(cvm);
            }

            return View(cvms);
        }

        public ActionResult PreviewCustomerPointsGrid(string guids)
        {
            if (string.IsNullOrWhiteSpace(guids) == false)
            {
                IEnumerable<Guid> guidobjs = guids.Split(',').Select(x => Guid.Parse(x));
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<Customer> customercollection = GetList<Customer>(uow, new InOperator("Oid", guidobjs));
                    foreach (Customer customer in customercollection)
                    {
                        IEnumerable<DocumentHeader> customerDocuments = customer.DocumentHeaders.Where(x => x.IsCanceled == false);
                        customer.CollectedPoints = customerDocuments.Sum(document => document.TotalPoints)
                                           - customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                        customer.TotalConsumedPoints = customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                        customer.TotalEarnedPoints = customerDocuments.Sum(document => document.TotalPoints);
                        customer.Save();
                    }
                    XpoHelper.CommitChanges(uow);
                    XpoSession.ReloadChangedObjects();
                }
            }
            IEnumerable<Customer> customers = new XPQuery<Customer>(XpoSession).Where(x => x.CollectedPoints > 0 || x.DocumentHeaders.Sum(y => y.TotalPoints) > 0);
            List<CustomerViewModel> cvms = new List<CustomerViewModel>();
            cvms.Capacity = customers.Count();
            foreach (Customer customer in customers)
            {
                CustomerViewModel cvm = new CustomerViewModel();
                cvm.LoadPersistent(customer);
                IEnumerable<DocumentHeader> customerDocuments = customer.DocumentHeaders.Where(document => document.IsCanceled == false);
                cvm.ComputedPoints = customerDocuments.Sum(document => document.TotalPoints)
                                   - customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                customer.TotalConsumedPoints = customerDocuments.Sum(document => document.ConsumedPointsForDiscount);
                customer.TotalEarnedPoints = customerDocuments.Sum(document => document.TotalPoints);
                cvms.Add(cvm);
            }

            return PartialView(cvms);
        }

        public static object CustomerForPointsRequestedByFilter(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Filter))
            {
                return new List<CustomerViewModel>();
            }
            string filter = args.Filter.Replace("*", "%").Replace("%", "");
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPQuery<Customer> customers = new XPQuery<Customer>(uow);
                var filtered = customers.Where(x => x.Owner.Oid == EffectiveOwner.Oid && (x.Code.Contains(filter) || x.Trader.TaxCode.Contains(filter) || x.CompanyName.Contains(filter)))
                    .Skip(args.BeginIndex).Take(args.EndIndex - args.BeginIndex).Select(x => new CustomerViewModel(x));
                return filtered.ToList();
            }
        }

        public static object CustomerForPointsRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value is Guid)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Customer customer = uow.GetObjectByKey<Customer>((Guid)args.Value);
                    return customer == null ? null : new CustomerViewModel(customer);
                }
            }
            return null;
        }

        public JsonResult CheckAfm(String TaxCode, String CountryCode)
        {
            String name = String.Empty;
            String address = String.Empty;
            bool isValid = false;

            CustomerHelper.CheckTaxCodeOnViesApi(TaxCode, CountryCode, out name, out address, out isValid);

            return Json(new
            {
                ValidResponse = isValid,
                CompanyName = name,
                CompanyAddress = address

            });
        }

        public JsonResult GetDefaultVatLevel()
        {
            string Oid = string.Empty;
            Oid = CustomerHelper.GetDefaultVatLevel();

            return Json(new
            {
                key = Oid

            });
        }




    }
}
