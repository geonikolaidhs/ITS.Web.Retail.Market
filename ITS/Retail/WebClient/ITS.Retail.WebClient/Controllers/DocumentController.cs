using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraReports.UI;
using ITS.Retail.Common;
using ITS.Retail.Common.Helpers;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.PrintServer.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Attributes;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Reports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;


namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    [StoreControllerEditable]
    public class DocumentController : BaseObjController<DocumentHeader>
    {
        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();
            if (ViewData["ID"] != null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid docOid;
                    if (Guid.TryParse(ViewData["ID"].ToString(), out docOid))
                    {
                        DocumentHeader docHead = uow.GetObjectByKey<DocumentHeader>(docOid);
                        ViewData["DocumentHeader"] = docHead;
                    }
                }
            }
            return PartialView("LoadViewPopup");
        }

        public JsonResult jsonCheckDocumentLinesCount()
        {
            string message;
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            DocumentHelper.MaxCountOfLinesExceeded(document, out message);
            return Json(new { result = message });
        }

        public JsonResult JsonUpdateCustomerDocumentDiscount(decimal documentDiscount, decimal customerDiscount)
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            document.DefaultDocumentDiscount = documentDiscount;
            document.DefaultCustomerDiscount = customerDiscount;
            if (document.DocumentDetails.Count > 0)
            {
                DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
            }
            return Json(new { });
        }

        public JsonResult JsonUpdateDefaultDocumentDiscount(decimal documentDiscount, decimal customerDiscount)
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            document.DefaultDocumentDiscount = documentDiscount;
            document.DefaultCustomerDiscount = customerDiscount;
            if (document.DocumentDetails.Count > 0)
            {
                DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
            }
            return Json(new { });
        }

        public ActionResult OrderItemsPopUp()
        {
            return PartialView("OrderItemsPopUp");
        }

        public ActionResult MarkUpPopUp()
        {
            return PartialView("MarkUpPopUp");
        }

        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            ViewData["Recover"] = Request["Recover"];

            ViewData["Division"] = Request["Division"];

            ViewData["LoadFromSession"] = Request["LoadFromSession"];

            ViewData["HasReturnedFromOrderItemsForm"] = Request["HasReturnedFromOrderItemsForm"];

            ViewData["RestoreTemporary"] = Request["RestoreTemporary"];

            ViewData["DocType"] = Request["DocType"];

            bool displayGeneric = false;
            bool.TryParse(Request["DisplayGeneric"], out displayGeneric);
            if (displayGeneric)
            {
                return PartialView("../Shared/PopupGenericEditCallbackPanel");
            }
            return PartialView();
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditDocument;

            if (ViewData["ID"] != null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Guid docOid;
                    if (Guid.TryParse(ViewData["ID"].ToString(), out docOid))
                    {
                        if (docOid == Guid.Empty)
                        {
                            ViewBag.Title = Resources.NewDocument;
                        }
                        DocumentHeader docHead = uow.GetObjectByKey<DocumentHeader>(docOid);
                        ViewData["DocumentHeader"] = docHead;
                    }
                }
            }
            if (Request["Recover"] != null)
            {
                ViewData["Recover"] = Request["Recover"];
            }

            if (Request["Division"] != null)
            {
                ViewData["Division"] = Request["Division"];
            }

            if (Request["LoadFromSession"] != null)
            {
                ViewData["LoadFromSession"] = Request["LoadFromSession"];
            }

            if (Request["HasReturnedFromOrderItemsForm"] != null)
            {
                ViewData["HasReturnedFromOrderItemsForm"] = Request["HasReturnedFromOrderItemsForm"];
            }

            if (Request["RestoreTemporary"] != null)
            {
                ViewData["RestoreTemporary"] = Request["RestoreTemporary"];
            }

            if (Request["DocType"] != null)
            {
                ViewData["DocType"] = Request["DocType"];
            }

            return PartialView("LoadEditPopup");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index(eDivision Mode)
        {
            bool userIsCustomer = UserHelper.IsCustomer(CurrentUser);
            if (CurrentOwner == null)
            {
                Session["Error"] = Resources.SelectCompany;
                return new RedirectResult("~/Home/Index");
            }
            else if (Mode != eDivision.Sales && userIsCustomer)
            {
                return new RedirectResult("~/Document?Mode=" + eDivision.Sales.ToString());
            }

            User user = CurrentUser;

            if (userIsCustomer && Session["StoresThatCurrentUserBuysFrom"] != null)
            {
                if (Session["currentCustomer"] == null)
                {
                    Session["currentCustomer"] = BOApplicationHelper.GetUserEntities<Customer>(user.Session, user).First<Customer>();
                }
            }

            List<string> errorMessages = new List<string>();
            StoreViewModel currentStoreViewModel = (Session["currentStore"] as StoreViewModel);

            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRows";

            this.ToolbarOptions.NewButton.Visible = currentStoreViewModel == null
                              ? false
                              : DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, currentStoreViewModel.Oid, Mode, MvcApplication.ApplicationInstance, out errorMessages);

            this.ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            this.ToolbarOptions.PrintButton.Visible = true;
            this.ToolbarOptions.PrintButton.OnClick = "PrintSelectedDocuments";
            this.ToolbarOptions.CopyButton.Visible = true;
            this.ToolbarOptions.CopyButton.OnClick = "CopySelectedDocument";
            this.ToolbarOptions.UndoButton.OnClick = "CancelDocument";
            this.ToolbarOptions.VariableValuesButton.Visible = this.GetType().GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null;
            this.ToolbarOptions.VariableValuesButton.OnClick = "VariableValuesDisplay.ShowVariableValuesPopUp";
            this.ToolbarOptions.MergeDocumentsButton.Title = Resources.MergeDocuments;
            this.ToolbarOptions.MergeDocumentsButton.Name = Resources.MergeDocuments;
            this.ToolbarOptions.MergeDocumentsButton.Visible = true;
            this.ToolbarOptions.MergeDocumentsButton.CCSClass = "button";
            this.ToolbarOptions.MergeDocumentsButton.OnClick = "MergeSelectedDocuments";



            if (UserHelper.IsCustomer(CurrentUser))
            {
                this.ToolbarOptions.NewButton.OnClick = "NumberOfOrderDocumentTypesDefined";
            }
            else
            {
                this.ToolbarOptions.NewButton.OnClick = "AddNewItem";
            }
            this.ToolbarOptions.UndoButton.Visible = !(CurrentUser.Role.Type == eRoleType.Customer);

            this.ToolbarOptions.TransformButton.Visible = this.ToolbarOptions.UndoButton.Visible;
            this.ToolbarOptions.TransformButton.OnClick = "TransformSelectedDocument";

            this.CustomJSProperties.AddJSProperty("gridName", "grdDocument");


            this.CustomJSProperties.AddJSProperty("DocumentsDivision", Mode);

            FillLookupComboBoxesForDocFilter(Mode);

            XPCollection<TemporaryObject> temps = GetTemporaryObjects(Mode);
            IEnumerable<Guid> oids = temps.Select(document => document.EntityOid);
            CriteriaOperator criteriaOperator = CriteriaOperator.And(new BinaryOperator("Division", Mode), new InOperator("Oid", oids));
            List<Guid> docs = GetList<DocumentHeader>(XpoSession, new InOperator("Oid", oids)).Select(document => document.Oid).ToList();

            ViewBag.TemporaryDocuments = temps;

            ViewBag.Division = Mode;
            ViewBag.POSs = GetList<Model.POS>(XpoSession);

            string proforma = Request["Proforma"] ?? (ViewData["Proforma"] as String ?? "");
            bool isProforma = !String.IsNullOrWhiteSpace(proforma) && proforma == "Proforma";

            SetViewTitle(Mode, isProforma);

            return View("Index", new List<DocumentHeader>());
        }

        public ActionResult TemporaryFilter()
        {
            eDivision currentDivision = eDivision.Sales;
            try
            {
                currentDivision = (eDivision)Enum.Parse(typeof(eDivision), Request["ModeForTemporaryForm"].ToString());
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.GetFullMessage();
            }

            XPCollection<TemporaryObject> temps = GetTemporaryObjects(currentDivision);
            ViewBag.TemporaryDocuments = temps;
            return PartialView();
        }

        private XPCollection<TemporaryObject> GetTemporaryObjects(eDivision division)
        {
            return GetList<TemporaryObject>(XpoSession,
            CriteriaOperator.And(
                new BinaryOperator("EntityType", typeof(DocumentHeader).FullName),
                UserHelper.IsSystemAdmin(this.CurrentUser) ? null : new BinaryOperator("CreatedBy.Oid", this.CurrentUser.Oid),
                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("SerializedData"), "\"Division\": \"" + (int)division + "\"")
            ));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Proforma(bool? useSpecialProforma)
        {
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.ToolbarOptions.NewButton.OnClick = "AddNewItem";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRows";

            List<string> errorMessages = new List<string>();

            StoreViewModel currentStoreViewModel = (Session["currentStore"] as StoreViewModel);
            this.ToolbarOptions.NewButton.Visible = currentStoreViewModel == null ? false : DocumentHelper.AvailableDocumentTypesPerStore(CurrentUser, EffectiveOwner, currentStoreViewModel.Oid, eDivision.Sales, MvcApplication.ApplicationInstance, out errorMessages, isProforma: true);

            this.ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            this.ToolbarOptions.PrintButton.Visible = true;
            this.ToolbarOptions.PrintButton.OnClick = "PrintSelectedDocuments";
            this.ToolbarOptions.TransformButton.Visible = true;
            this.ToolbarOptions.TransformButton.OnClick = "TransformSelectedDocument";
            this.ToolbarOptions.CopyButton.Visible = true;
            this.ToolbarOptions.CopyButton.OnClick = "CopySelectedDocument";
            this.ToolbarOptions.UndoButton.Visible = true;
            this.ToolbarOptions.UndoButton.OnClick = "CancelDocument";

            this.CustomJSProperties.AddJSProperty("gridName", "grdDocument");
            this.CustomJSProperties.AddJSProperty("DocumentsDivision", "Sales");

            CriteriaOperator proformaFilter = null;
            IEnumerable<Guid> proformaDocumentTypeGuids;
            if (useSpecialProforma.HasValue && useSpecialProforma.Value)
            {
                if ((MvcApplication.ApplicationInstance == (eApplicationInstance.STORE_CONTROLER)
                     || MvcApplication.ApplicationInstance == (eApplicationInstance.DUAL_MODE)
                  )
                  && !StoreHelper.StoreHasSpecialProformaTypeAndSeries(StoreControllerAppiSettings.CurrentStore))
                {
                    return new RedirectResult("~/Document/Proforma?useSpecialProforma=False");
                }
                ViewData["Proforma"] = "SpecialProforma";
                proformaDocumentTypeGuids = XpoSession.Query<Model.POS>()
                                                .Where(pos => pos.SpecialProformaDocumentType != null)
                                                .Select(pos => pos.SpecialProformaDocumentType.Oid)
                                                .Distinct();
                ViewBag.Title = Resources.SpecialProformaInvoices;
            }
            else
            {
                ViewData["Proforma"] = "Proforma";
                proformaDocumentTypeGuids = XpoSession.Query<Model.POS>().Select(pos => pos.ProFormaInvoiceDocumentType.Oid).Distinct();
                ViewBag.Title = Resources.ProformaInvoices;
            }

            if (proformaDocumentTypeGuids == null && proformaDocumentTypeGuids.Count() <= 0)
            {
                proformaDocumentTypeGuids = new List<Guid>() { Guid.Empty };
            }

            proformaFilter = new InOperator("DocumentType.Oid", proformaDocumentTypeGuids);
            List<DocumentHeader> docs = new List<DocumentHeader>();
            try
            {
                CriteriaOperator totalFilter = CriteriaOperator.And(
                 new BinaryOperator("Division", eDivision.Sales),
                 new BinaryOperator("IsCanceled", false),
                 new BinaryOperator("FinalizedDate", DateTime.Now.Date, BinaryOperatorType.GreaterOrEqual),
                 new BinaryOperator("FinalizedDate", DateTime.Now.Date.AddDays(1).AddMilliseconds(-1), BinaryOperatorType.LessOrEqual),
                 new BinaryOperator("TransformationStatus", eTransformationStatus.NOT_TRANSFORMED),
                 proformaFilter
                 );

                Session["SearchDocumentSearchFilter"] = totalFilter;
                ViewBag.Division = eDivision.Sales;

                docs = GetList<DocumentHeader>(XpoSession, totalFilter).ToList();
            }
            catch (NotSupportedException ex)
            {
            }
            return View(docs);
        }

        private void FillLookupComboBoxesForDocFilter(eDivision selectedDivision)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Store store = CurrentStore != null ? uow.GetObjectByKey<Store>(CurrentStore.Oid) : null;
            IQueryable<DocumentType> getListAsQueryable = store != null ?
                                                            StoreHelper.StoreDocumentTypes(store,
                                                                                            selectedDivision,
                                                                                            null,
                                                                                            false,
                                                                                            false,
                                                                                            true).AsQueryable() :
                                                            GetList<DocumentType>(uow, CriteriaOperator.And(new BinaryOperator("Division.Section", selectedDivision),
                                                                                new ContainsOperator("StoreDocumentSeriesTypes",
                                                                                StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(store, null, false, false, true))
                                                                                )).AsQueryable();
            ViewBag.DocumentTypesFilterComboBox = getListAsQueryable;
            ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(uow);
            ViewBag.Devices = GetList<Model.POS>(uow);
            ViewBag.DocumentSeriesComboBox = ViewBag.DocumentTypesFilterComboBox == null ?
                                            null : getListAsQueryable.SelectMany(docType => docType.DocumentSeries).Distinct().Where(documentSeries => documentSeries != null);
            ViewBag.SfaListComboBox = GetList<Model.SFA>(uow, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal))).ToList() ?? new List<SFA>();

        }


        private CriteriaOperator GetProformaCriteria(UnitOfWork uow)
        {
            if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE)
            {

                IEnumerable<Guid> proformaDocumentTypeGuids = (GetList<Model.POS>(uow)).Select(pos => pos.ProFormaInvoiceDocumentType.Oid).Distinct();

                if (proformaDocumentTypeGuids != null && proformaDocumentTypeGuids.Count() > 0)
                {
                    CriteriaOperator proformaCriteria = new InOperator("DocumentType", proformaDocumentTypeGuids);
                    return proformaCriteria;
                }
            }
            return null;
        }

        public override ActionResult Grid()
        {
            CriteriaOperator criteria = (CriteriaOperator)Session["SearchDocumentSearchFilter"];
            if (Request["btnSearch"] != null)
            {
                //Action -> Search
                DocumentSearchFilter filter = new DocumentSearchFilter();
                TryUpdateModel<DocumentSearchFilter>(filter);

                filter.User = CurrentUser.Oid;
                if (CurrentStore != null)
                {
                    filter.Store = CurrentStore.Oid;
                }
                else if (Session["currentOwner"] != null)
                {
                    filter.Owner = ((CompanyNew)Session["currentOwner"]).Oid;
                }


                if (filter.Proforma == "Proforma")
                {
                    filter.ProformaTypes = DocumentHelper.GetProformaTypes(XpoSession).Select(type => type.Oid).ToList();
                }

                if (filter.Proforma == "SpecialProforma")
                {
                    filter.ProformaTypes = XpoSession.Query<Model.POS>()
                                                        .Where(pos => pos.SpecialProformaDocumentType != null)
                                                        .Select(pos => pos.SpecialProformaDocumentType.Oid)
                                                        .ToList();
                }
                criteria = filter.BuildCriteria();

                //Extra checks according to user role type
                if (UserHelper.IsCustomer(CurrentUser))
                {
                    Customer currentCustomer = UserHelper.GetCustomer(CurrentUser, this.XpoSession);
                    criteria = CriteriaOperator.And(criteria, CriteriaOperator.Or(new BinaryOperator("CreatedBy.Oid", CurrentUser.Oid), new BinaryOperator("Customer.Oid", currentCustomer.Oid)));
                }
                else if (UserHelper.IsCompanyUser(CurrentUser))
                {
                    if ((filter.Store == null || filter.Store == Guid.Empty))
                    {
                        CriteriaOperator storeCriteria = null;
                        foreach (Store store in UserHelper.GetStoresThatUserOwns(CurrentUser))
                        {
                            storeCriteria = CriteriaOperator.Or(storeCriteria, new BinaryOperator("Store.Oid", store.Oid));
                        }
                        criteria = CriteriaOperator.And(criteria, storeCriteria);
                    }
                    if ((filter.CreatedBy == null || filter.CreatedBy == Guid.Empty))
                    {
                        CriteriaOperator createdByCriteria = CriteriaOperator.Or(new NullOperator("CreatedBy"),
                                                                                 new NotOperator(new ContainsOperator("CreatedBy.UserTypeAccesses", new BinaryOperator("EntityType", typeof(Customer).FullName))),
                                                                                 CriteriaOperator.And(new BinaryOperator("DocumentNumber", 0, BinaryOperatorType.Greater),
                                                                                                      new ContainsOperator("CreatedBy.UserTypeAccesses",
                                                                                                                            new BinaryOperator("EntityType", typeof(Customer).FullName))));
                        criteria = CriteriaOperator.And(criteria, createdByCriteria);
                    }
                }
            }
            else if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                ViewData["CallbackMode"] = "DELETESELECTED";
                if (TableCanDelete)
                {
                    List<Guid> oids = new List<Guid>();
                    string allOids = Request["DXCallbackArgument"].Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
                    string[] guidStrings = allOids.Split(',');
                    List<string> reasons = new List<string>();
                    foreach (string guidStr in guidStrings)
                    {
                        Guid docGuid = Guid.Parse(guidStr);
                        string reason = "";
                        if (DocumentHelper.DocumentCanBeDeleted(docGuid, CurrentUser, out reason))
                        {
                            oids.Add(docGuid);
                        }
                        else
                        {
                            reasons.Add(reason);
                        }
                    }
                    if (oids.Count > 0)
                    {
                        try
                        {
                            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                            {
                                DeleteAll(uow, oids);
                            }
                        }
                        catch (ConstraintViolationException e)
                        {
                            string errorMessage = e.GetFullMessage();
                            Session["Error"] = Resources.CannotDeleteObject;
                        }
                        catch (Exception e)
                        {
                            Session["Error"] = e.Message;
                        }
                    }
                    if (reasons.Count > 0)
                    {
                        Session["Error"] = Resources.CannotDeleteDocument + ": " + Environment.NewLine + String.Join(Environment.NewLine, reasons.ToArray());
                    }
                }
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            eDivision selectedDivision;
            if (Enum.TryParse(Request["Mode"], out selectedDivision) == false)
            {
                selectedDivision = eDivision.Sales;
            }

            SortingCollection sortBy = new SortingCollection();
            sortBy.Add(new SortProperty("CreatedOn", SortingDirection.Descending));
            sortBy.Add(new SortProperty("POS", SortingDirection.Descending));

            if (criteria == null)
            {
                criteria = RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Division", selectedDivision), typeof(DocumentHeader), EffectiveOwner);
            }
            Session["SearchDocumentSearchFilter"] = criteria;

            ViewBag.Division = selectedDivision;

            string proforma = Request["Proforma"] ?? (ViewData["Proforma"] as String ?? "");
            bool isProforma = !String.IsNullOrWhiteSpace(proforma) && proforma == "Proforma";

            SetViewTitle(selectedDivision, isProforma);
            return PartialView("Grid", new XPCollection<DocumentHeader>(XpoSession, criteria, sortBy.ToArray()));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UserStores()
        {
            return View();
        }

        public ActionResult SelectStoreForAddOrder()
        {
            return PartialView();
        }

        public ActionResult SelectStore()
        {
            return PartialView();
        }

        public JsonResult jsonGetNumberOfCustomReports()
        {
            Guid documentID = Guid.Parse(Request["DOid"]);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                String SelectedCustomReportOid = "";
                DocumentHeader docHeader = uow.GetObjectByKey<DocumentHeader>(documentID);
                StoreDocumentSeriesType storeDocSerType = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", docHeader.DocumentType),
                                                                                                                       new BinaryOperator("DocumentSeries", docHeader.DocumentSeries)));
                if (storeDocSerType != null)
                {
                    if (storeDocSerType.DefaultCustomReport != null &&
                        storeDocSerType.DefaultCustomReport.ReportRoles.Select(reportrole => reportrole.Role.Oid).Contains(CurrentUser.Role.Oid) &&
                        (storeDocSerType.UserType == UserType.ALL || storeDocSerType.UserType == UserHelper.GetUserType(CurrentUser)))
                    {
                        SelectedCustomReportOid = storeDocSerType.DefaultCustomReport.Oid.ToString();
                        return Json(new { numberOfReports = 1 });
                    }
                }
                int numberOfValidReports = ReportsHelper.GetValidDocumentTypeCustomReports(CurrentUser, docHeader.DocumentType, docHeader.Session as UnitOfWork).Distinct().Count();
                return Json(new { numberOfReports = numberOfValidReports });
            }
        }

        public JsonResult jsonSelectStoreForOrder()
        {
            string url = "";
            if (Request["selected_store"] != null)
            {
                User currentUser = CurrentUser;
                Store store = XpoHelper.GetNewUnitOfWork().FindObject<Store>(new BinaryOperator("Oid", Guid.Parse(Request["selected_store"])));
                StoreViewModel svm = new StoreViewModel();
                svm.LoadPersistent(store);
                this.CurrentStore = svm;
                string division = "";

                if (UserHelper.IsCustomer(currentUser))
                {
                    division = "Purchase";
                }
                else if (UserHelper.IsSystemAdmin(currentUser) || UserHelper.IsCompanyUser(currentUser))
                {
                    division = "Sales";
                }

                if (division != "")
                {
                    url = Url.Content("Document/Store?Store=" + store.Oid + "&Division=" + division);
                }
            }
            return Json(new { url = url });
        }

        protected override void FillLookupComboBoxes()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            ViewBag.DocumentTypesComboBox = GetList<DocumentType>(uow, new ContainsOperator("StoreDocumentSeriesType", new BinaryOperator("DocumentSeries.eModule", eModule.POS, BinaryOperatorType.NotEqual)));
            ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(uow);
            ViewBag.Devices = GetList<Model.POS>(uow);
            ViewBag.SfaListComboBox = GetList<Model.SFA>(uow, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal))).ToList() ?? new List<SFA>();
        }

        [HttpPost]
        public ActionResult DeleteDocument([ModelBinder(typeof(RetailModelBinder))] DocumentHeader ct)
        {
            try
            {
                Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("Grid", GetList<DocumentHeader>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["DocumentFilter"]).AsEnumerable());
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Print(Guid? DOid, bool directPrint = false)
        {
            try
            {
                Guid documentGuid;
                if (DOid.HasValue == false)
                {
                    return View("../Home/CloseWindow");
                }
                documentGuid = DOid.Value;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    String SelectedCustomReportOid;
                    DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                    if (documentHeader == null)
                    {
                        return View("../Home/CloseWindow");
                    }

                    string errorMessage = SignDocumentIfNecessary(documentHeader);
                    if (!String.IsNullOrWhiteSpace(errorMessage))
                    {
                        Session["Error"] = errorMessage;
                        return new RedirectResult("~/Home/Index");
                    }

                    StoreDocumentSeriesType storeDocSerType = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", documentHeader.DocumentType),
                                                                                                                           new BinaryOperator("DocumentSeries", documentHeader.DocumentSeries)));
                    if (storeDocSerType != null)
                    {
                        if (storeDocSerType.DefaultCustomReport != null &&
                            storeDocSerType.DefaultCustomReport != null &&
                            storeDocSerType.DefaultCustomReport.ReportRoles.Select(reportrole => reportrole.Role.Oid).Contains(CurrentUser.Role.Oid) &&
                            (storeDocSerType.UserType == UserType.ALL || storeDocSerType.UserType == UserHelper.GetUserType(CurrentUser)))
                        {
                            return DocumentCustomReport(storeDocSerType.DefaultCustomReport.Oid, documentHeader.Oid, directPrint, storeDocSerType.Duplicates, true);
                        }
                    }
                    IEnumerable<DocumentTypeCustomReport> validCustomReports = ReportsHelper.GetValidDocumentTypeCustomReports(CurrentUser, documentHeader.DocumentType, documentHeader.Session as UnitOfWork).Distinct();
                    if (validCustomReports.Count() == 1)
                    {
                        StoreDocumentSeriesType storeDocumentSeriesType = documentHeader.DocumentType
                                                                          .StoreDocumentSeriesTypes
                                                                          .FirstOrDefault(storeDocSeriesType => storeDocSeriesType.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid);

                        POSDevice remotePrinterService = storeDocumentSeriesType.PrintServiceSettings == null ? null : storeDocumentSeriesType.PrintServiceSettings.RemotePrinterService;
                        if (remotePrinterService != null && remotePrinterService.IsActive)
                        {
                            PrintServerPrintDocumentResponse response = PrinterServiceHelper.PrintDocument(remotePrinterService, CurrentUser.Oid, documentHeader.Oid, documentHeader.POSID, storeDocumentSeriesType.PrintServiceSettings.PrinterNickName);
                            if (response == null)
                            {
                                Session["Error"] = Resources.CouldNotEstablishConnection + " Remote Print Service :" + remotePrinterService.Name;
                                return new RedirectResult("~/Home/Index");
                            }
                            switch (response.Result)
                            {
                                case ePrintServerResponseType.FAILURE:
                                    Session["Error"] = response.Explanation + Environment.NewLine + response.ErrorMessage;
                                    return new RedirectResult("~/Home/Index");
                                case ePrintServerResponseType.SUCCESS:
                                    Session["Notice"] = Resources.SuccefullyCompleted;
                                    return new RedirectResult("~/Home/Index");
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            SelectedCustomReportOid = validCustomReports.First().Oid.ToString();
                            return DocumentCustomReport(validCustomReports.First().Oid, documentHeader.Oid, directPrint);
                        }
                    }
                }
                return DocumentReport(documentGuid, directPrint);
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.GetFullMessage();
                MvcApplication.WRMLogModule.Log(exception);
                return new RedirectResult("~/Home/Index");
            }
        }

        private string SignDocumentIfNecessary(DocumentHeader documentHeader)
        {
            if (documentHeader.DocumentType.TakesDigitalSignature
                && documentHeader.DocumentNumber > 0
                && String.IsNullOrWhiteSpace(documentHeader.Signature)
               )
            {
                try
                {
                    StoreControllerSettings settings = documentHeader.Session.GetObjectByKey<StoreControllerSettings>(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
                    List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                        Where(x =>
                                x.DocumentSeries.Any(y => y.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid)
                             && x.TerminalDevice is POSDevice
                             && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                        ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                    string signature = DocumentHelper.SignDocument(documentHeader, this.CurrentUser, documentHeader.Owner, String.Empty/*MvcApplication.OLAPConnectionString*/, posDevices);
                    if (string.IsNullOrWhiteSpace(signature))
                    {
                        return Resources.CannotRetreiveSignature;
                    }
                    documentHeader.Signature = signature;
                    documentHeader.Save();
                    XpoHelper.CommitTransaction(documentHeader.Session);
                }
                catch (Exception exception)
                {
                    return exception.GetFullMessage();
                }
            }
            return String.Empty;
        }

        [AllowAnonymous]
        public ActionResult AnonymousPrint(Guid? DOid, Guid userId, bool directPrint = false)
        {

            Guid documentGuid;
            if (DOid.HasValue == false)
            {
                return View("../Home/CloseWindow");
            }
            documentGuid = DOid.Value;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User currentUser = uow.GetObjectByKey<User>(userId);
                String SelectedCustomReportOid;
                DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                if (documentHeader == null)
                {
                    return View("../Home/CloseWindow");
                }
                string errorMessage = SignDocumentIfNecessary(documentHeader);
                if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    Session["Error"] = errorMessage;
                    return View("../Home/CloseWindow");
                }

                StoreDocumentSeriesType storeDocSerType = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", documentHeader.DocumentType),
                                                                                                                       new BinaryOperator("DocumentSeries", documentHeader.DocumentSeries)));
                if (storeDocSerType != null)
                {
                    if (storeDocSerType.DefaultCustomReport != null &&
                        storeDocSerType.DefaultCustomReport != null &&
                        storeDocSerType.DefaultCustomReport.ReportRoles.Select(reportrole => reportrole.Role.Oid).Contains(currentUser.Role.Oid) &&
                        (storeDocSerType.UserType == UserType.ALL || storeDocSerType.UserType == UserHelper.GetUserType(currentUser)))
                    {
                        return DocumentCustomReport(storeDocSerType.DefaultCustomReport.Oid, documentHeader.Oid, directPrint, storeDocSerType.Duplicates, true);
                    }
                }
                IEnumerable<DocumentTypeCustomReport> validCustomReports = ReportsHelper.GetValidDocumentTypeCustomReports(currentUser, documentHeader.DocumentType, uow).Distinct();
                if (validCustomReports.Count() == 1)
                {
                    SelectedCustomReportOid = validCustomReports.First().Oid.ToString();
                    return DocumentCustomReport(validCustomReports.First().Oid, documentHeader.Oid, directPrint);
                }
            }
            return DocumentReport(documentGuid, directPrint);
        }

        // Oid
        private ActionResult DocumentCustomReport(Guid dtCustomReportOid, Guid objectOid, bool directPrint, int duplicates = 1, bool isDefault = false)
        {
            this.ToolbarOptions.ForceVisible = false;
            XtraReportBaseExtension report;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                if (isDefault)
                {
                    CustomReport CustomReport = uow.GetObjectByKey<CustomReport>(dtCustomReportOid);
                    report = PrepareReport(CustomReport.Oid, objectOid, (duplicates > 0 ? duplicates : 1));
                }
                else
                {
                    DocumentTypeCustomReport documentTypeCustomReport = uow.GetObjectByKey<DocumentTypeCustomReport>(dtCustomReportOid);
                    report = PrepareReport(documentTypeCustomReport.Report.Oid, objectOid, (documentTypeCustomReport.Duplicates > 0 ? documentTypeCustomReport.Duplicates : 1));
                }

                if (directPrint == true)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        report.ExportToPdf(stream, new DevExpress.XtraPrinting.PdfExportOptions() { ShowPrintDialogOnOpen = true, Compressed = true, ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Medium });
                        return new FileContentResult(stream.GetBuffer(), "application/pdf");
                    }
                }
                return View("../Reports/WebDocumentViewer", report);
            }

        }

        private XtraReportBaseExtension PrepareReport(Guid reportID, Guid objOid, int duplicates)
        {
            string title;
            string description;

            XtraReportBaseExtension report = ReportsHelper.GetXtraReport(reportID, EffectiveOwner, CurrentUser,
                MvcApplication.OLAPConnectionString, out title, out description);
            if (report is SingleObjectXtraReport)
            {
                (report as SingleObjectXtraReport).ObjectOid = objOid;
            }
            if (report != null)
            {
                ReportsHelper.DuplicateReport(report, duplicates, reportID, EffectiveOwner, CurrentUser);
            }

            return report;
        }

        private ActionResult DocumentReport(Guid documentOid, bool directPrint)
        {
            try
            {
                User currentUser = CurrentUser;
                XtraReport report = null;
                if (UserHelper.IsAdmin(currentUser))
                {
                    report = new SupplierDocumentReport(this.CurrentUser);
                }
                else
                {
                    report = new CustomerDocumentReport();
                }

                report.DataSource = GetList<DocumentHeader>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("Oid", documentOid));
                if (directPrint == true)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        report.ExportToPdf(stream, new DevExpress.XtraPrinting.PdfExportOptions() { ShowPrintDialogOnOpen = true, Compressed = true });
                        return new FileContentResult(stream.GetBuffer(), "application/pdf");
                    }
                }
                return View("../Reports/WebDocumentViewer", report);
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.GetFullMessage();
                MvcApplication.WRMLogModule.Log(exception);
                return View("../Home/CloseWindow");
            }
        }

        protected CriteriaOperator GetPriceCatalogCriteria(DocumentHeader documentHeader)
        {
            CriteriaOperator crop = new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Oid", documentHeader.Store.Oid));
            return crop;
        }

        public ActionResult PriceCatalogGrid()
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            XPQuery<PriceCatalog> priceCatalogs = new XPQuery<PriceCatalog>(documentHeader.Session);
            CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
            IQueryable query = priceCatalogs.AppendWhere(converter, GetPriceCatalogCriteria(documentHeader));
            return PartialView(query);
        }

        public ActionResult PriceCatalogPolicyGrid()
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            List<PriceCatalogPolicy> priceCatalogPolicies = documentHeader.Store.StorePriceCatalogPolicies.Select(stprcatpol => stprcatpol.PriceCatalogPolicy).ToList();
            priceCatalogPolicies.Add(documentHeader.Customer.PriceCatalogPolicy);
            return PartialView(priceCatalogPolicies.Distinct());
        }

        public override ActionResult Dialog(List<string> arguments)
        {
            DialogOptions.Width = 1200;
            DialogOptions.Height = 500;
            if (arguments.Contains("mergeDocuments"))
            {
                //MergeDocuments
                List<MergedDocumentDetail> details = new List<MergedDocumentDetail>();
                try
                {
                    List<Guid> documentOids = new List<Guid>();
                    if (arguments.Count() > 2)
                    {
                        int width;
                        int height;
                        if (int.TryParse(arguments[1], out width) && int.TryParse(arguments[2], out height))
                        {
                            DialogOptions.Width = width;
                            DialogOptions.Height = height;
                        }
                    }
                    if (arguments.Count() > 3)
                    {
                        for (int i = 3; i <= (arguments.Count() - 1); i++)
                        {
                            Guid docOid;
                            if (Guid.TryParse(arguments[i], out docOid))
                            {
                                documentOids.Add(docOid);
                            }
                        }
                        details = DocumentHelper.MergeDetails(documentOids, null);
                        Session["MergeDoccumentDetails"] = details;
                        DialogOptions.HeaderText = Resources.MergeDocuments;
                        DialogOptions.BodyPartialView = "../Document/MergedDocumentGridPopup";
                        DialogOptions.OKButton.OnClick = "PrintMergedDocument";
                        DialogOptions.OKButton.Text = Resources.ExportΤoXLS;
                        DialogOptions.CancelButton.OnClick = DialogOptions.CloseUpEvent = "CloseDialog";
                        ViewBag.EnableOriginalJSEvents = false;
                        ViewBag.HidePopup = true;
                        Session["HideOrderLink"] = true;
                        return PartialView(details);
                    }
                }
                catch (Exception ex)
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                    return PartialView(details);
                }
            }
            else if (arguments.Contains("storeCustomers"))
            {//DisplayCustomers
                DialogOptions.HeaderText = Resources.Customers;
                DialogOptions.BodyPartialView = "../Customer/Grid";
                DialogOptions.OKButton.OnClick = "SetCustomerOnDocumentHiddenValues";
                DialogOptions.CancelButton.OnClick = DialogOptions.CloseUpEvent = "CloseDialog";
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                ViewBag.EnableOriginalJSEvents = false;
                ViewBag.HidePopup = true;
                CriteriaOperator crop = CriteriaOperator.And(DocumentHelper.DocumentTypeSupportedCustomersCriteria(documentHeader), ApplyOwnerCriteria(null, typeof(Customer), documentHeader.Owner));
                CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                XPQuery<Customer> newXPQuery = new XPQuery<Customer>(documentHeader.Session);
                Session["HideOrderLink"] = true;
                IQueryable xpQuery = newXPQuery.AppendWhere(converter, crop);
                Session["CustomerFilter"] = crop;
                return PartialView(xpQuery);
            }
            else if (arguments.Contains("storeSuppliers"))
            {//DisplaySuppliers
                DialogOptions.HeaderText = Resources.Suppliers;
                DialogOptions.BodyPartialView = "../Supplier/Grid";
                DialogOptions.OKButton.OnClick = "SetSupplierOnDocumentHiddenValues";
                DialogOptions.CancelButton.OnClick = DialogOptions.CloseUpEvent = "CloseDialog";
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                ViewBag.EnableOriginalJSEvents = false;
                ViewBag.HidePopup = true;
                Session["SupplierFilter"] = CriteriaOperator.And(new BinaryOperator("IsActive", true), ApplyOwnerCriteria(null, typeof(SupplierNew), documentHeader.Owner));
                CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                CriteriaOperator crop = (CriteriaOperator)Session["SupplierFilter"];
                XPQuery<SupplierNew> newXPQuery = new XPQuery<SupplierNew>(documentHeader.Session);
                IQueryable xpQuery = newXPQuery.AppendWhere(converter, crop);
                return PartialView(xpQuery);
            }
            else if (arguments.Contains("PriceCatalogPolicyCb"))
            {//Display Price Catalogs Policies
                DialogOptions.HeaderText = Resources.PriceCatalogPolicies;
                DialogOptions.BodyPartialView = "PriceCatalogPolicyGrid";
                DialogOptions.OKButton.OnClick = "SetPriceCatalogPolicyOnDocumentHiddenValues";
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                List<PriceCatalogPolicy> priceCatalogPolicies = documentHeader.Store.StorePriceCatalogPolicies.Select(stprcatpol => stprcatpol.PriceCatalogPolicy).ToList();
                PriceCatalogHelper.IncludeCustomerPolicyToPoliciesList(documentHeader, priceCatalogPolicies);
                ViewBag.HidePopup = true;
                return PartialView(priceCatalogPolicies.Distinct());
            }
            else if (arguments.Contains("DisplayCustomerAddresses"))
            {//SelectCustomerAddressDialog
                DialogOptions.HeaderText = Resources.SelectDelivery;
                DialogOptions.BodyPartialView = "SelectCustomerAddressDialog";
                DialogOptions.OKButton.OnClick = "function(s,e){SetAddressDialog(s,e,'Delivery');}";
                DialogOptions.OnShownEvent = "function(s,e){InitAddressDialog(s,e,'Delivery');}";
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                return PartialView(DocumentHelper.GetAddresses(arguments[1], documentHeader.Session, documentHeader.Division));
            }
            else if (arguments.Contains("DisplayTriangularAddresses"))
            {//SelectTriangularAddressDialog
                DialogOptions.HeaderText = Resources.SelectDelivery;
                DialogOptions.BodyPartialView = "SelectCustomerAddressDialog";
                DialogOptions.OKButton.OnClick = "function(s,e){SetAddressDialog(s,e,'Triangular');}";
                DialogOptions.OnShownEvent = "function(s,e){InitAddressDialog(s,e,'Triangular');}";
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                return PartialView(DocumentHelper.GetAddresses(arguments[1], documentHeader.Session, documentHeader.Division));
            }
            else if (Request["action"] != null && Request["action"] == "SELECT_ORDER")
            {//Display Order Document Types
                DialogOptions.HeaderText = Resources.SelectDocumentType;
                DialogOptions.BodyPartialView = "SelectOrderDocumentType";
                DialogOptions.OKButton.OnClick = "ContinueToOrder";
                DialogOptions.Width = 450;
                DialogOptions.Height = 100;
                PrepareOrderDocumentTypes();
            }
            else
            {//SelectReportDialog
                Guid docId;
                if (Guid.TryParse(arguments.FirstOrDefault(), out docId))
                {
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

                    DocumentHeader documentHeader = uow.FindObject<DocumentHeader>(new BinaryOperator("Oid", docId));
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.SelectReport;

                    this.DialogOptions.OKButton.OnClick = "DialogOKButton_OnClick";
                    this.DialogOptions.CancelButton.OnClick = "DialogCloseEvent";
                    this.DialogOptions.CloseUpEvent = "DialogCloseEvent";

                    this.DialogOptions.Width = 300;
                    this.DialogOptions.Height = 200;

                    //-- The name of the partial to render in the Dialog
                    this.DialogOptions.BodyPartialView = "SelectReportDialog";
                    IEnumerable<DocumentTypeCustomReport> validCustomReports = ReportsHelper.GetValidDocumentTypeCustomReports(CurrentUser, documentHeader.DocumentType, uow);
                    ViewData["ValidReports"] = validCustomReports.Distinct();

                }
            }
            return PartialView();
        }

        public JsonResult jsonIsDocumentEditable(string documentOid)
        {
            try
            {
                if (this.CurrentStore == null)
                {
                    Session["Error"] = Resources.PleaseSelectAStore;
                    return Json(new { returnValue = false });
                }

                Guid documentGuid = Guid.Parse(documentOid);
                bool returnValue = false, warnForCrashed = false;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader docHead = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                    if (docHead != null)
                    {
                        Store cStore = docHead.Store;
                        if (cStore.DocumentSeries.Count == 0)
                        {
                            Session["Error"] = Resources.StoreHasNoSeries;
                            return Json(new { returnValue = false, warnForCrashed = false });
                        }
                        else if (!StoreHelper.StoreHasSeriesForDocumentType(cStore, docHead.DocumentType, DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance)) && !UserHelper.IsSystemAdmin(CurrentUser))
                        {
                            Session["Error"] = Resources.StoreHasNoSeriesTypes;
                            return Json(new { returnValue = false, warnForCrashed = false });
                        }
                    }
                    returnValue = DocumentHelper.CanEdit(docHead, uow.GetObjectByKey<User>(CurrentUser.Oid));
                    if (returnValue == false)
                    {
                        Session["Error"] = Resources.YouCannotEditThisDocument;
                    }
                    else
                    {
                        warnForCrashed = docHead.TempObjExists;
                    }
                }
                return Json(new { returnValue = returnValue, warnForCrashed = warnForCrashed });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                Session["Error"] = Resources.YouCannotEditThisDocument;
                return Json(new { returnValue = false });
            }
        }

        public JsonResult jsonIsDocumentCopyable(string documentOid)
        {
            try
            {
                if (this.CurrentStore == null)
                {
                    Session["Error"] = Resources.PleaseSelectAStore;
                    return Json(new { returnValue = false });
                }

                Guid documentGuid = Guid.Parse(documentOid);
                bool returnValue = false;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader docHead = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                    returnValue = DocumentHelper.CanCopy(docHead, uow.GetObjectByKey<User>(CurrentUser.Oid));
                    if (returnValue == false)
                    {
                        Session["Error"] = Resources.YouCannotCopyThisDocument;
                    }
                }
                return Json(new { returnValue = returnValue });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                Session["Error"] = Resources.YouCannotCopyThisDocument;
                return Json(new { returnValue = false });
            }
        }

        public JsonResult jsonSelectedDocumentsCanBeTransformed(string documents, bool? isFast)
        {
            try
            {
                string returnValue = "";
                List<string> documentGuidsStrs = documents.Split(',').ToList();

                if (CurrentStore == null)
                {
                    Session["Error"] = returnValue = Resources.PleaseSelectAStore;
                    return Json(new { returnValue = returnValue });
                }

                if (documentGuidsStrs.Count <= 0)
                {
                    throw new Exception();
                }

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    List<DocumentHeader> documentHeaders = new List<DocumentHeader>();
                    foreach (string docGuid in documentGuidsStrs)
                    {
                        DocumentHeader docHeader = uow.GetObjectByKey<DocumentHeader>(Guid.Parse(docGuid));
                        if (docHeader == null)
                        {
                            throw new Exception();
                        }
                        documentHeaders.Add(docHeader);
                    }
                    returnValue = DocumentHelper.DocumentsCanBeTransformed(documentHeaders, isFast.HasValue && isFast.Value, DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance));
                }
                if (returnValue != "")
                {
                    Session["Error"] = returnValue;
                }
                return Json(new { returnValue = returnValue });
            }
            catch (Exception ex)
            {
                Session["Error"] = Resources.TransformationCannotTakePlace;
                throw ex;
            }
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return base.ExportToFile<DocumentHeader>(Session["DocumentHeaderGridSettings"] as GridViewSettings, (CriteriaOperator)Session["DocumentFilter"]);
        }

        public ActionResult Users()
        {
            return PartialView();
        }

        public ActionResult DocumentCustomerFilter()
        {
            return PartialView();
        }

        public ActionResult DocumentSupplierFilter()
        {
            return PartialView();
        }

        public static object UsersRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
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
            IEnumerable<User> searched_users = new XPCollection<User>(XpoHelper.GetNewUnitOfWork(), CriteriaOperator.And(CriteriaOperator.Or(
                new BinaryOperator("UserName", proccessed_filter, BinaryOperatorType.Like),
                new BinaryOperator("FullName", proccessed_filter, BinaryOperatorType.Like),
                new BinaryOperator("TaxCode", proccessed_filter, BinaryOperatorType.Equal)
                ),
                new BinaryOperator("IsActive", true)
                ));
            if (searched_users.Count() == 0)
            {
                return null;
            }
            return searched_users;
        }

        public static object UserRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                User obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<User>(e.Value);
                return obj;
            }
            return null;
        }

        #region Document Form

        public JsonResult Copy(string FromDC)
        {
            Guid documentGuid = Guid.Empty;
            if (Guid.TryParse(FromDC, out documentGuid))
            {
                DocumentHeader documentHeader = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(documentGuid);
                if (documentHeader == null)
                {
                    Session["Error"] = Resources.InvalidDocument;

                    return Json(new { result = "" });
                }

                Session["currentDocumentToLoad"] = DocumentHelper.CopyDocument(documentGuid, XpoHelper.GetNewUnitOfWork());

                return Json(new { LoadFromSession = true });

            }
            return Json(new { result = "" });
        }

        public ActionResult MergedDocumentGridPopup()
        {

            List<MergedDocumentDetail> details = new List<MergedDocumentDetail>();

            try
            {
                string ItemCode = Request["Code"];
                string Barcode = Request["Barcode"];
                string Description = Request["Description"];
                string Qty = Request["Qty"];
                string VatFactor = Request["VatFactor"];
                string Reamarks = Request["Reamarks"];
                bool isLinkedLine = Request["IsLinkedLine"]?.ToString() == "C" ? true : false;

                CriteriaOperator crit = null;
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    crit = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("ItemCode"), ItemCode));
                }
                if (!string.IsNullOrEmpty(Barcode))
                {
                    crit = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("BarcodeCode"), Barcode));
                }
                if (!string.IsNullOrEmpty(Description))
                {
                    crit = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CustomDescription"), Description));
                }
                if (!string.IsNullOrEmpty(Reamarks))
                {
                    crit = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Reamarks"), Reamarks));
                }
                if (!string.IsNullOrEmpty(VatFactor))
                {
                    crit = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("VatFactor"), VatFactor));
                }
                if (isLinkedLine)
                {
                    crit = CriteriaOperator.And(new BinaryOperator("IsLinkedLine", true, BinaryOperatorType.Equal));
                }

                string param = Request["Param"];
                if (!string.IsNullOrEmpty(param))
                {
                    List<Guid> docOids = new List<Guid>();
                    string[] arrayString = param.Split(':');
                    if (arrayString != null && arrayString.Count() > 0)
                    {
                        for (int i = 0; i < (arrayString.Count() - 1); i++)
                        {
                            Guid docOid;
                            if (Guid.TryParse(arrayString[i], out docOid))
                            {
                                docOids.Add(docOid);
                            }
                        }
                        if (docOids.Count > 0)
                        {
                            details = DocumentHelper.MergeDetails(docOids, crit);
                            if (!string.IsNullOrEmpty(Qty) && details.Count > 0)
                            {
                                decimal quantity;
                                if (decimal.TryParse(Qty, out quantity))
                                {
                                    details = details.Where(x => x.Qty == quantity).ToList();
                                }
                            }
                        }
                        Session["MergeDoccumentDetails"] = details;
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            return PartialView(details);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportMergeDetails()
        {

            List<MergedDocumentDetail> details = new List<MergedDocumentDetail>();
            if (Session["MergeDoccumentDetails"] != null)
            {
                try
                {
                    details = Session["MergeDoccumentDetails"] as List<MergedDocumentDetail>;
                }
                catch (Exception ex)
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
            }

            var settings = Session["MergeDoccumentGridSettings"] as GridViewSettings;
            return ExportDetails(settings, details);


        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportDetails(GridViewSettings gridViewSettings, List<MergedDocumentDetail> details)
        {
            return base.ExportDetailsToFile(gridViewSettings, details);
        }


        public ActionResult Edit(string Oid, eDivision Mode = eDivision.Sales, bool LoadFromSession = false, string DocType = "", bool HasReturnedFromOrderItemsForm = false,
            bool InViewMode = false, string CustomerOid = "00000000-0000-0000-0000-000000000000", string SupplierOid = "00000000-0000-0000-0000-000000000000",
            bool? Recover = null, Guid? RestoreTemporary = null)
        {
            CleanDocument();

            if (!LoadFromSession)
            {
                Session["currentDocumentToLoad"] = null;
            }
            DocumentHeader document = null;
            IEnumerable<DocumentSeries> docSeries = null;
            ViewBag.InViewMode = InViewMode;
            bool isOrder = false;

            if (InViewMode)
            {
                Session["currentDocument"] = null;
                ViewData["currentDocument"] = null;

                Guid documentGuid;
                if (!Guid.TryParse(Oid, out documentGuid))
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                    List<string> originalURL = RetailHelper.GoBack();
                    return RedirectToAction(originalURL.Last(), originalURL.First());
                }

                document = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(documentGuid);
                if (document == null)
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }

                if (document.TempObjExists)
                {
                    TemporaryObject tmpobj = document.GetTemporaryObject();
                    if (Recover.HasValue == false)
                    {
                        Session["Error"] = Resources.DocumentHasCrushed;
                    }
                    else if (Recover.Value == true)
                    {
                        string error;
                        document.FromJson(tmpobj.SerializedData, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                    }
                }

                ViewBag.IsNewDocument = false;
                ViewData["DocumentNumberIsEditable"] = false;
                ViewBag.DocumentDetailIsNew = false;
                ViewData["currentDocument"] = document;
                ViewBag.DocumentTypesComboBox = new List<DocumentType>() { document.DocumentType };
                ViewBag.DocumentStatusComboBox = new List<DocumentStatus>() { document.Status };
                ViewBag.TransferPurposes = null;
                ViewBag.DocumentSeriesCombobox = new List<DocumentSeries>() { document.DocumentSeries };
                ViewData["currentDocumentHeaderOid"] = document.Oid;
                ViewBag.DocumentDetailFormMode = "Hidden";
                ViewBag.SfaListComboBox = GetList<Model.SFA>(XpoHelper.GetNewUnitOfWork(), CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal))).ToList() ?? new List<SFA>();
                ViewBag.DocumentViewForm = eDocumentTypeView.Advanced;
                if (document.TransferPurpose != null)
                {
                    ViewBag.TransferPurposes = new List<TransferPurpose>() { document.TransferPurpose };
                }
            }
            else
            {///EDIT MODE

                if (HasReturnedFromOrderItemsForm == false)
                {
                    CleanDocument();
                    Session["currentDocument"] = null;

                    if (this.CurrentStore == null)
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                        return new RedirectResult("~/Document/CloseOrderForm");
                    }
                    Guid documentGuid;
                    if (!Guid.TryParse(Oid, out documentGuid) && !LoadFromSession && RestoreTemporary == Guid.Empty)
                    {
                        Session["Error"] = Resources.AnErrorOccurred;
                        List<string> originalURL = RetailHelper.GoBack();
                        return RedirectToAction(originalURL.Last(), originalURL.First());
                    }

                    ViewBag.IsNewDocument = documentGuid == Guid.Empty;

                    if (LoadFromSession)
                    {
                        if (Session["currentDocumentToLoad"] == null && (Session["currentDocumentToLoad"] as DocumentHeader) != null)
                        {
                            Session["Error"] = Resources.InvalidDocument;
                        }
                        Session["currentDocument"] = Session["currentDocumentToLoad"];
                        Session["currentDocumentToLoad"] = null;
                        document = Session["currentDocument"] as DocumentHeader;
                        ViewBag.IsNewDocument = true;
                    }
                    else if (RestoreTemporary != Guid.Empty)
                    {
                        UnitOfWork uw = XpoHelper.GetNewUnitOfWork();
                        TemporaryObject obj = uw.GetObjectByKey<TemporaryObject>(RestoreTemporary.Value);
                        ViewBag.IsNewDocument = false;
                        document = uw.GetObjectByKey<DocumentHeader>(obj.EntityOid) ?? new DocumentHeader(uw);
                        string error;
                        document.FromJson(obj.SerializedData, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                        Session["currentDocument"] = document;

                    }
                    else if (ViewBag.IsNewDocument)
                    {
                        document = new DocumentHeader(XpoHelper.GetNewUnitOfWork());
                        document.Division = Mode;
                        document.Store = document.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);

                        if (UserHelper.IsCustomer(CurrentUser))
                        {
                            document.Division = eDivision.Sales;
                            document.ExecutionDate = DateTime.Now;

                            document.Customer = UserHelper.GetCustomer(CurrentUser, (UnitOfWork)document.Session);
                            if (document.Customer == null)
                            {
                                Session["Error"] = Resources.PleaseSelectACustomer;
                                return new RedirectResult("~/Document/CloseOrderForm");
                            }
                            document.BillingAddress = document.Customer.DefaultAddress;
                            document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);

                            List<DocumentType> orderDocumentTypes = StoreHelper.StoreDocumentTypes(document.Store, eDivision.Sales, onlyForOrder: true);
                            if (orderDocumentTypes.Count <= 0)
                            {
                                Session["Error"] = Resources.PleaseSelectDefaultDocumentType;
                                return new RedirectResult("~/Document/CloseOrderForm");
                            }
                            document.DocumentType = orderDocumentTypes.Count == 1
                                                  ? orderDocumentTypes.First()
                                                  : orderDocumentTypes.Where(orderDocType => orderDocType.IsDefault).FirstOrDefault();
                            if (document.DocumentType == null)
                            {
                                orderDocumentTypes.FirstOrDefault();
                            }
                            if (document.DocumentType == null)
                            {
                                Session["Error"] = Resources.PleaseSelectDefaultDocumentType;
                                return new RedirectResult("~/Document/CloseOrderForm");
                            }

                            IEnumerable<DocumentSeries> orderDocumentSeries = StoreHelper.StoreSeriesForDocumentType(document.Store, document.DocumentType, onlyForOrder: true);
                            if (orderDocumentSeries.Count() <= 0 || orderDocumentSeries.FirstOrDefault() == null)
                            {
                                Session["Error"] = Resources.PleaseSelectSeriesDefaultDocumentTypeInCurrentStore;
                                return new RedirectResult("~/Document/CloseOrderForm");
                            }
                            document.DocumentSeries = document.Session.GetObjectByKey<DocumentSeries>(orderDocumentSeries.FirstOrDefault().Oid);

                        }

                        document.RefferenceDate = document.FinalizedDate = DateTime.Now;
                        Guid storeDocumentSeriesTypeGuid = Guid.Empty;
                        Guid.TryParse(DocType, out storeDocumentSeriesTypeGuid);

                        if (Guid.TryParse(DocType, out storeDocumentSeriesTypeGuid))
                        {
                            StoreDocumentSeriesType storeDocumentSeriesType = document.Session.GetObjectByKey<StoreDocumentSeriesType>(storeDocumentSeriesTypeGuid);
                            if (storeDocumentSeriesType != null)
                            {
                                document.DocumentType = storeDocumentSeriesType.DocumentType;
                                document.DocumentSeries = storeDocumentSeriesType.DocumentSeries;
                                isOrder = storeDocumentSeriesType.StoreDocumentType == eStoreDocumentType.ORDER && Mode == eDivision.Sales;
                            }

                            if (document.DocumentType == null)
                            {
                                Session["Error"] = Resources.NoOrderDocumentTypeDefined;
                            }
                        }

                        if (document.Division == eDivision.Sales)
                        {
                            ViewBag.IsOrder = isOrder;
                        }

                        if (document.DocumentType != null)
                        {
                            if (document.DocumentSeries != null)
                            {
                                //Initialize Customer
                                if (CustomerOid != Guid.Empty.ToString())
                                {

                                    Guid CustomerGuid;
                                    if (Guid.TryParse(CustomerOid, out CustomerGuid))
                                    {
                                        document.Customer = document.Session.GetObjectByKey<Customer>(CustomerGuid);
                                        if (document.Customer == null)
                                        {
                                            Session["Error"] = Resources.PleaseSelectACustomer;
                                            return new RedirectResult("~/Document/CloseOrderForm");
                                        }
                                        document.BillingAddress = document.Customer.DefaultAddress;
                                        document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                                    }
                                    else
                                    {
                                        Session["Error"] = Resources.PleaseSelectACustomer;
                                        return new RedirectResult("~/Document/CloseOrderForm");
                                    }
                                }
                                else
                                {
                                    StoreDocumentSeriesType storedoctype = document.Session.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And
                                                                                                                               (new BinaryOperator("DocumentType", document.DocumentType),
                                                                                                                               (new BinaryOperator("DocumentSeries", document.DocumentSeries))));
                                    document.Customer = storedoctype.DefaultCustomer ?? null;
                                    document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                                }

                                //Initialize Supplier
                                if (SupplierOid != Guid.Empty.ToString())
                                {

                                    Guid SupplierGuid;
                                    if (Guid.TryParse(SupplierOid, out SupplierGuid))
                                    {
                                        document.Supplier = document.Session.GetObjectByKey<SupplierNew>(SupplierGuid);
                                        if (document.Supplier == null)
                                        {
                                            Session["Error"] = Resources.PleaseSelectASupplier;
                                        }
                                        document.BillingAddress = document.Supplier.DefaultAddress;
                                    }
                                    else
                                    {
                                        Session["Error"] = Resources.PleaseSelectASupplier;
                                    }
                                }
                                else
                                {
                                    StoreDocumentSeriesType storedoctype = document.Session.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And
                                                                                                                               (new BinaryOperator("DocumentType", document.DocumentType),
                                                                                                                               (new BinaryOperator("DocumentSeries", document.DocumentSeries))));
                                    document.Supplier = storedoctype.DefaultSupplier ?? null;
                                }
                            }
                            //Initialize Document Status
                            DocumentHelper.GetDefaultDocStatus(ref document, CurrentOwner);
                            //Initialize Address Profession
                            document.AddressProfession = document.BillingAddress != null ? document.BillingAddress.Profession : "";
                        }
                    }
                    else
                    {
                        document = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(documentGuid);
                        isOrder = document.Division == eDivision.Sales
                               && document.DocumentType.StoreDocumentSeriesTypes.FirstOrDefault(sdst => sdst.DocumentSeries == document.DocumentSeries).StoreDocumentType == eStoreDocumentType.ORDER
                                                                                                                                                     && Mode == eDivision.Sales;

                        if (document == null)
                        {
                            Session["Error"] = Resources.AnErrorOccurred;
                        }
                        else if (document.TempObjExists)
                        {
                            TemporaryObject tmpobj = document.GetTemporaryObject();
                            if (Recover.HasValue == false)
                            {
                                Session["Error"] = Resources.DocumentHasCrushed;
                            }
                            else if (Recover.Value == true)
                            {
                                string error;
                                document.FromJson(tmpobj.SerializedData, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                            }
                        }
                    }
                }
                else
                {
                    document = Session["currentDocument"] as DocumentHeader;
                }

                ViewBag.Mode = document.Division;
                ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, document.DocumentType);

                Session["currentDocument"] = document;

                CriteriaOperator storeDocTypeSerCriteria =
                    UserHelper.IsSystemAdmin(CurrentUser) ?
                        StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(document.Store, null, document.IsCancelingAnotherDocument) :
                        StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(document.Store, DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance),
                        document.IsCancelingAnotherDocument);

                IEnumerable<StoreDocumentSeriesType> storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>(document.Session, storeDocTypeSerCriteria);
                docSeries = storeDocumentSeriesTypes.Where(sdst => sdst.DocumentType == document.DocumentType).Select(sdst => sdst.DocumentSeries);
                if (docSeries.Count() == 1)
                {
                    document.DocumentSeries = docSeries.First();
                }
                IEnumerable<DocumentType> documentTypes = storeDocumentSeriesTypes.Select(sdst => sdst.DocumentType).Where(docType => docType != null && docType.Division.Section == document.Division).Distinct();
                if (documentTypes.Count() <= 0)
                {
                    Session["Error"] = Resources.StoreHasNoSeriesTypes;
                }
                ViewData["DocumentNumberIsEditable"] = !DocumentPermissionHelper.DocumentHasAutomaticNumbering(document);
                ViewData["currentDocumentHeaderOid"] = document.Oid;
                ViewBag.DocumentDetailIsNew = true;
                eModule module = DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance);
                if (ViewBag.IsNewDocument == false && ViewBag.InViewMode == false)
                {
                    ViewBag.DocumentTypesComboBox = new List<DocumentType>() { document.DocumentType };
                    ViewBag.DocumentSeriesCombobox = new List<DocumentSeries>() { document.DocumentSeries };
                }
                else
                {
                    ViewBag.DocumentTypesComboBox = StoreHelper.StoreDocumentTypes(document.Store, document.Division, module);
                    ViewBag.DocumentSeriesCombobox = docSeries;

                }
                ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(document.Session);
                ViewBag.TransferPurposes = GetList<TransferPurpose>(document.Session);
                ViewBag.DocumentDetailFormMode = "Hidden";
                ViewBag.SfaListComboBox = GetList<Model.SFA>(document.Session, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal))).ToList() ?? new List<SFA>();
                if (!InViewMode && ((eDocumentTypeView)ViewBag.DocumentViewForm).Equals(eDocumentTypeView.Simple))
                {
                    //Fill hidden data
                    if (UserHelper.IsCustomer(CurrentUser))
                    {
                        document.Customer = UserHelper.GetCustomer(CurrentUser, (UnitOfWork)document.Session);
                        document.BillingAddress = document.Customer.DefaultAddress;
                        document.AddressProfession = document.BillingAddress != null ? document.BillingAddress.Profession : "";
                        document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                    }
                    else
                    {
                        Session["Error"] = Resources.PleaseReviseDocumentTypeSettingsDocumentViewForm;
                    }
                }
            }
            //Initialize Delivery Address
            if (document.BillingAddress != null)
            {
                if (document.DeliveryAddress == null)
                {
                    document.DeliveryAddress = document.BillingAddress.Description;
                }
            }
            ViewBag.Title = document.Session.IsNewObject(document) ? Resources.NewDocument : Resources.EditDocument;
            SetDocumentTraderViewBags(document);
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, document.DocumentType);
            return PartialView(document);
        }

        public ActionResult DocumentDetails()
        {
            return PartialView();
        }

        [Security(ReturnsPartial = false)]
        public JsonResult jsonDocumentStatusChanged()
        {
            DocumentHeader document = (DocumentHeader)Session["currentDocument"];
            if (Request["DocumentStatus"] != null)
            {
                Guid docStatusGuid;
                if (Guid.TryParse(Request["DocumentStatus"].ToString(), out docStatusGuid))
                {
                    DocumentStatus docStatus = document.Session.GetObjectByKey<DocumentStatus>(docStatusGuid);
                    if (docStatus == null)
                    {
                        throw new Exception();
                    }
                    document.Status = docStatus;
                }
            }
            bool autoDocNumber = DocumentPermissionHelper.DocumentHasAutomaticNumbering(document);
            bool disableDocNumberInput = false;

            if (autoDocNumber ||
                (autoDocNumber == false && document.Status.TakeSequence && document.DocumentNumber > 0)
                )
            {
                disableDocNumberInput = true;
            }

            return Json(new { AutoDocNumber = disableDocNumberInput });
        }


        [Security(ReturnsPartial = false)]
        public JsonResult jsonTabletValueChanged()
        {
            DocumentHeader document = (DocumentHeader)Session["currentDocument"];
            if (document != null && Request["Tablet"] != null)
            {
                Guid oid;
                if (Guid.TryParse(Request["Tablet"].ToString(), out oid))
                {
                    document.Tablet = document.Session.GetObjectByKey<SFA>(oid);
                }
            }
            return Json(new { selected = document?.Tablet?.Oid ?? Guid.Empty });
        }

        public JsonResult ProcessDocumentDetailStore()
        {

            Hashtable result = new Hashtable();
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = (Session["currentDocumentDetail"] as DocumentDetail);

            string triggered_by = JSHelper.GetInputValue("triggered_by");
            string spinlineqty = JSHelper.GetInputValue("spinlineqty");
            string documentDetailFormMode = JSHelper.GetInputValue("DocumentDetailFormMode") ?? "Edit";
            string custom_price = JSHelper.GetInputValue("custom_price");
            string CustomMeasurementUnit = JSHelper.GetInputValue("CustomMeasurementUnit");
            string Remarks = JSHelper.GetInputValue("Remarks");
            string userDiscount = JSHelper.GetInputValue("userDiscount");
            string isPercentage = JSHelper.GetInputValue("isPercentage");
            string fpaFactorString = JSHelper.GetInputValue("fpa_factor");
            string netTotalAfterDiscount = JSHelper.GetInputValue("net_total_after_discount");
            string totalVatAmount = JSHelper.GetInputValue("total_vat_amount");
            string customDescription = JSHelper.GetInputValue("item_info_name");

            result["DetailIsForSave"] = false;

            switch (triggered_by)
            {
                case "btnSaveItem":
                case "btnSaveCloseItem":
                case "btnRecalculateItem":
                case "btnUpdateItem":
                case "btnUpdateCloseItem":
                case "userDiscountPercentage":
                case "userDiscountMoneyValue":
                case "spinlineqty":
                case "custom_price":
                case "fpa_factor":
                case "total_vat_amount":
                    //UPDATE_QUANTITY
                    if (documentDetail != null)
                    {
                        result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");

                        decimal quantity, customPriceValue, totalVat;
                        Guid fpaFactor = Guid.Empty;

                        if (decimal.TryParse(spinlineqty, out quantity) && decimal.TryParse(custom_price, out customPriceValue) && decimal.TryParse(totalVatAmount, out totalVat))
                        {
                            VatFactor vatFactor = null;
                            quantity /= DocumentHelper.QUANTITY_MULTIPLIER;
                            customPriceValue /= DocumentHelper.QUANTITY_MULTIPLIER;
                            totalVat /= DocumentHelper.QUANTITY_MULTIPLIER;
                            documentDetail.CustomUnitPrice = customPriceValue;
                            if (triggered_by != "total_vat_amount")
                            {
                                totalVat = -1;
                            }
                            string errorMessage = DocumentHelper.RecalculateTemporaryDocumentDetail(documentHeader, documentDetail, quantity, vatFactor, CustomMeasurementUnit, customDescription, Remarks, isPercentage, userDiscount);
                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                result["DetailIsForSave"] = (triggered_by == "btnSaveItem" || triggered_by == "btnSaveCloseItem") ? true : false;
                            }
                            else
                            {
                                Session["Error"] = errorMessage;
                            }
                        }
                    }
                    break;
                case "CustomMeasurementUnit":
                    Guid mmuOid;
                    if (Guid.TryParse(CustomMeasurementUnit, out mmuOid) && documentDetail != null)
                    {
                        documentDetail.MeasurementUnit = documentDetail.Session.GetObjectByKey<MeasurementUnit>(mmuOid);
                        documentDetail.Remarks = Remarks;
                    }
                    break;
                case "Remarks":
                    if (documentDetail != null)
                    {
                        documentDetail.Remarks = Remarks;
                    }
                    result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");
                    break;
                default:
                    Session["Error"] = Resources.AnErrorOccurred;
                    return Json(new { });
            }

            string customMeasurementUnit = documentDetail == null ? "" : documentDetail.CustomMeasurementUnit;
            if (documentHeader.UsesPackingQuantities && documentDetail != null)
            {
                customMeasurementUnit = documentDetail.MeasurementUnitsQuantities;
                if (string.IsNullOrEmpty(customMeasurementUnit))
                {
                    customMeasurementUnit = documentDetail.CustomMeasurementUnit;
                }
            }

            result["CustomMeasurementUnit"] = customMeasurementUnit;
            VatFactor defaultVatFactor = GetList<VatFactor>(documentHeader.Session).FirstOrDefault();

            result["item_info_code"] = documentDetail != null && documentDetail.Item != null ? documentDetail.Item.Code : "";
            result["item_info_barcode"] = documentDetail != null && documentDetail.Barcode != null ? documentDetail.Barcode.Code : "";
            result["item_info_name"] = documentDetail != null && documentDetail.Item != null ? documentDetail.Item.Description : "";
            result["spinlineqty"] = documentDetail != null ? documentDetail.PackingQuantity : 0;
            result["fpa_factor"] = documentDetail != null ? documentDetail.VatFactorGuid : (defaultVatFactor != null ? defaultVatFactor.Oid : Guid.Empty);


            result["custom_price"] = documentDetail != null ? documentDetail.CustomUnitPrice : 0;
            result["total_cost"] = documentDetail != null ? (documentHeader.DocumentType.IsForWholesale ? documentDetail.NetTotalBeforeDiscount.ToString() : documentDetail.GrossTotalBeforeDiscount.ToString()) : "";
            result["userDiscount"] = documentDetail != null ? documentDetail.CustomDiscountsAmount : 0;
            result["isPercentage"] = isPercentage;
            result["userDiscountPercentage"] = documentDetail != null ? documentDetail.CustomDiscountsPercentageWholeSale * 100 : 0;
            result["userDiscountMoneyValue"] = (documentDetail != null && documentHeader != null && documentHeader.DocumentType != null) ? documentDetail.CustomDiscountsAmount : 0;
            result["total_discount_percentage"] = documentDetail != null ? documentDetail.TotalDiscountPercentage * 100 : 0;
            result["total_discount"] = documentDetail != null ? documentDetail.TotalDiscount : 0;
            result["net_total"] = documentDetail != null ? documentDetail.NetTotal : 0;
            result["total_vat_amount"] = documentDetail != null ? documentDetail.TotalVatAmount : 0;
            result["final_sum"] = result["gross_total"] = documentDetail != null ? documentDetail.GrossTotal : 0;
            result["final_sum"] += " β‚¬";
            result["Remarks"] = documentDetail != null ? documentDetail.Remarks : "";

            List<string> input_order = new List<string>{
                //"item_info_code",
                //"item_info_barcode",
                //"item_info_name",
                "spinlineqty",
                //"CustomMeasurementUnit",
                //"fpa_factor",
                //"custom_price",
                //"total_cost",
                //"price_catalog_discount_percentage",
                //"price_catalog_discount_value",
                //"userDiscount","isPercentage",
                "userDiscountPercentage",
                "userDiscountMoneyValue",
                //"total_discount_percentage",
                //"total_discount",
                //"net_total_after_discount",
                //"total_vat_amount",
                //"gross_total",
                //"Remarks",
                //"triggered_by",//who triggered the call
                //"DocumentDetailFormMode"//current form mode
            };

            //if (!documentHeader.DocumentType.UsesPrices)
            //{
            //    input_order.Remove("custom_price");
            //}

            string focus_on = "";
            if (Session["Error"] == null || string.IsNullOrEmpty(Session["Error"].ToString()) == false)
            {
                focus_on = triggered_by;
            }
            else
            {
                int trigger_index = input_order.IndexOf(triggered_by);
                if (trigger_index != -1)
                {

                    int newIndex = (input_order.IndexOf(triggered_by) + 1);
                    if (newIndex < input_order.Count)
                    {
                        focus_on = input_order[newIndex];
                    }
                    else
                    {
                        focus_on = "btnSaveItem";
                    }
                }
                if (documentDetail == null)
                {
                    focus_on = "barcode_search";
                }
                bool retain_focus = false;
                if (Boolean.TryParse(JSHelper.GetInputValue("retain_focus"), out retain_focus) && retain_focus)
                {
                    focus_on = triggered_by;
                }
            }

            result["focus_on"] = focus_on;
            return Json(new { result = result });
        }

        public JsonResult ProcessDocumentDetailPurchase()
        {

            Hashtable result = new Hashtable();
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = (Session["currentDocumentDetail"] as DocumentDetail);

            string triggered_by = JSHelper.GetInputValue("triggered_by");
            string spinlineqty = JSHelper.GetInputValue("spinlineqty");
            string documentDetailFormMode = JSHelper.GetInputValue("DocumentDetailFormMode") ?? "Edit"; ;
            string custom_price = JSHelper.GetInputValue("custom_price");
            string CustomMeasurementUnit = JSHelper.GetInputValue("CustomMeasurementUnit");
            string Remarks = JSHelper.GetInputValue("Remarks");
            string userDiscount = JSHelper.GetInputValue("userDiscount");
            string isPercentage = JSHelper.GetInputValue("isPercentage");
            string fpaFactorString = JSHelper.GetInputValue("fpa_factor");
            string netTotalAfterDiscount = JSHelper.GetInputValue("net_total_after_discount");
            string totalVatAmount = JSHelper.GetInputValue("total_vat_amount");
            string customDescription = JSHelper.GetInputValue("item_info_name");

            result["DetailIsForSave"] = false;

            switch (triggered_by)
            {
                case "btnSaveItem":
                case "btnSaveCloseItem":
                case "btnRecalculateItem":
                case "btnUpdateItem":
                case "btnUpdateCloseItem":
                case "userDiscountPercentage":
                case "userDiscountMoneyValue":
                case "spinlineqty":
                case "custom_price":
                case "fpa_factor":
                case "total_vat_amount":
                    //case "net_total_after_discount":
                    //UPDATE_QUANTITY
                    if (documentDetail != null)
                    {
                        result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");

                        decimal quantity, customPriceValue, totalVat;
                        Guid fpaFactor = Guid.Empty;

                        if (decimal.TryParse(spinlineqty, out quantity)
                            && decimal.TryParse(custom_price, out customPriceValue)
                            && Guid.TryParse(fpaFactorString, out fpaFactor)
                            && decimal.TryParse(totalVatAmount, out totalVat))
                        {
                            VatFactor vatFactor = documentHeader.Session.GetObjectByKey<VatFactor>(fpaFactor);
                            if (vatFactor != null)
                            {
                                quantity /= DocumentHelper.QUANTITY_MULTIPLIER;
                                customPriceValue /= DocumentHelper.QUANTITY_MULTIPLIER;
                                totalVat /= DocumentHelper.QUANTITY_MULTIPLIER;
                                documentDetail.CustomUnitPrice = customPriceValue;
                                if (triggered_by != "total_vat_amount")
                                {
                                    totalVat = -1;
                                }
                                string errorMessage = DocumentHelper.RecalculateTemporaryDocumentDetail(documentHeader, documentDetail, quantity, vatFactor, CustomMeasurementUnit, customDescription, Remarks, isPercentage, userDiscount);
                                if (string.IsNullOrEmpty(errorMessage))
                                {
                                    result["DetailIsForSave"] = (triggered_by == "btnSaveItem" || triggered_by == "btnSaveCloseItem") ? true : false;
                                }
                                else
                                {
                                    Session["Error"] = errorMessage;
                                }
                            }
                        }
                    }
                    break;
                case "CustomMeasurementUnit":
                    Guid mmuOid;
                    if (Guid.TryParse(CustomMeasurementUnit, out mmuOid) && documentDetail != null)
                    {
                        documentDetail.MeasurementUnit = documentDetail.Session.GetObjectByKey<MeasurementUnit>(mmuOid);
                        documentDetail.Remarks = Remarks;
                    }
                    break;
                case "Remarks":
                    if (documentDetail != null)
                    {
                        documentDetail.Remarks = Remarks;
                    }
                    result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");
                    break;
                default:
                    Session["Error"] = Resources.AnErrorOccurred;
                    return Json(new { });
            }

            string customMeasurementUnit = documentDetail == null ? "" : documentDetail.CustomMeasurementUnit;
            if (documentHeader.UsesPackingQuantities && documentDetail != null)
            {
                customMeasurementUnit = documentDetail.MeasurementUnitsQuantities;
                if (string.IsNullOrEmpty(customMeasurementUnit))
                {
                    customMeasurementUnit = documentDetail.CustomMeasurementUnit;
                }
            }

            result["CustomMeasurementUnit"] = customMeasurementUnit;
            VatFactor defaultVatFactor = GetList<VatFactor>(documentHeader.Session).FirstOrDefault();

            result["item_info_code"] = documentDetail != null && documentDetail.Item != null ? documentDetail.Item.Code : "";
            result["item_info_barcode"] = documentDetail != null && documentDetail.Barcode != null ? documentDetail.Barcode.Code : "";
            result["item_info_name"] = documentDetail != null && documentDetail.Item != null ? documentDetail.Item.Description : "";
            result["spinlineqty"] = documentDetail != null ? documentDetail.PackingQuantity : 0;
            result["fpa_factor"] = documentDetail != null
                                        ? documentDetail.VatFactorGuid
                                        : (defaultVatFactor != null ? defaultVatFactor.Oid : Guid.Empty);
            result["custom_price"] = documentDetail != null ? documentDetail.CustomUnitPrice : 0;
            result["total_cost"] = documentDetail != null ? (documentHeader.DocumentType.IsForWholesale ? documentDetail.NetTotalBeforeDiscount.ToString() : documentDetail.GrossTotalBeforeDiscount.ToString()) : "";
            result["userDiscount"] = documentDetail != null ? documentDetail.CustomDiscountsAmount : 0;
            result["isPercentage"] = isPercentage;
            result["userDiscountPercentage"] = documentDetail != null ? documentDetail.CustomDiscountsPercentageWholeSale * 100 : 0;
            result["userDiscountMoneyValue"] = (documentDetail != null && documentHeader != null && documentHeader.DocumentType != null) ? documentDetail.CustomDiscountsAmount : 0;
            result["total_discount_percentage"] = documentDetail != null ? documentDetail.TotalDiscountPercentage * 100 : 0;
            result["total_discount"] = documentDetail != null ? documentDetail.TotalDiscount : 0;
            result["net_total"] = documentDetail != null ? documentDetail.NetTotal : 0;
            result["total_vat_amount"] = documentDetail != null ? documentDetail.TotalVatAmount : 0;
            result["final_sum"] = result["gross_total"] = documentDetail != null ? documentDetail.GrossTotal : 0;
            result["final_sum"] += " &euro;";
            result["Remarks"] = documentDetail != null ? documentDetail.Remarks : "";

            List<string> input_order = new List<string>{
                //"item_info_code",
                //"item_info_barcode",
                //"item_info_name",
                "spinlineqty",
                //"CustomMeasurementUnit",
                "fpa_factor",
                "custom_price",
                //"total_cost",
                //"price_catalog_discount_percentage",
                //"price_catalog_discount_value",
                //"userDiscount","isPercentage",
                "userDiscountPercentage",
                "userDiscountMoneyValue",
                //"total_discount_percentage",
                //"total_discount",
                //"net_total_after_discount",
                //"total_vat_amount",
                //"gross_total",
                //"Remarks",
                //"triggered_by",//who triggered the call
                //"DocumentDetailFormMode"//current form mode
            };

            if (!documentHeader.DocumentType.UsesPrices)
            {
                input_order.Remove("custom_price");
            }

            string focus_on = "";
            if (Session["Error"] == null || string.IsNullOrEmpty(Session["Error"].ToString()) == false)
            {
                focus_on = triggered_by;
            }
            else
            {
                int trigger_index = input_order.IndexOf(triggered_by);
                if (trigger_index != -1)
                {

                    int newIndex = (input_order.IndexOf(triggered_by) + 1);
                    if (newIndex < input_order.Count)
                    {
                        focus_on = input_order[newIndex];
                    }
                    else
                    {
                        focus_on = "btnSaveItem";
                    }
                }
                if (documentDetail == null)
                {
                    focus_on = "barcode_search";
                }
                Boolean retain_focus = false;
                if (Boolean.TryParse(JSHelper.GetInputValue("retain_focus"), out retain_focus) && retain_focus)
                {
                    focus_on = triggered_by;
                }
            }

            result["focus_on"] = focus_on;
            return Json(new { result = result });
        }

        public JsonResult ProcessDocumentDetail()
        {
            Hashtable result = new Hashtable();
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = (Session["currentDocumentDetail"] as DocumentDetail);
            PriceCatalogDetail priceCatalogDetail = Session["currentPriceCatalogDetail"] as PriceCatalogDetail;

            bool newitem = false;
            Boolean.TryParse(JSHelper.GetInputValue("check_box_state"), out newitem);
            string userDiscount = JSHelper.GetInputValue("userDiscount");
            string triggered_by = JSHelper.GetInputValue("triggered_by");
            string spinlineqty = JSHelper.GetInputValue("spinlineqty");
            bool isSingleEditForm = false;
            if (spinlineqty == "NaN")
            {
                if (documentDetail == null)
                {
                    return Json(new { error = Session["Error"] });
                }
                isSingleEditForm = true;
                decimal dval = documentDetail.Qty;
                Decimal.TryParse(JSHelper.GetInputValue("qty_spin_edit"), out dval);
                spinlineqty = dval.ToString();
            }
            string documentDetailFormMode = JSHelper.GetInputValue("DocumentDetailFormMode") ?? "Edit";
            string jSHelperGetInputValue = JSHelper.GetInputValue("custom_price");
            string custom_price = jSHelperGetInputValue != null && jSHelperGetInputValue != "NaN" ? jSHelperGetInputValue : (documentDetail.CustomUnitPrice * DocumentHelper.QUANTITY_MULTIPLIER).ToString();
            string CustomMeasurementUnit = JSHelper.GetInputValue("CustomMeasurementUnit");
            string Remarks = JSHelper.GetInputValue("Remarks");
            string isPercentage = JSHelper.GetInputValue("isPercentage");
            string customDescription = JSHelper.GetInputValue("item_info_name");

            bool isWholeSale = documentHeader != null && documentHeader.DocumentType != null ? documentHeader.DocumentType.IsForWholesale : true;

            result["DetailIsForSave"] = false;

            switch (triggered_by)
            {
                case "item_info_name":
                    documentDetail.CustomDescription = JSHelper.GetInputValue("item_info_name");
                    break;
                case "btnSaveItem":
                case "btnSaveCloseItem":
                case "btnRecalculateItem":
                case "btnUpdateItem":
                case "btnUpdateCloseItem":
                case "userDiscountPercentage":
                case "userDiscountMoneyValue":
                case "spinlineqty":
                    //UPDATE_QUANTITY
                    if (documentDetail != null)
                    {
                        result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");

                        decimal quantity;
                        decimal customPriceValue;
                        if (decimal.TryParse(spinlineqty, out quantity) && decimal.TryParse(custom_price, out customPriceValue))
                        {
                            quantity /= DocumentHelper.QUANTITY_MULTIPLIER;
                            customPriceValue /= DocumentHelper.QUANTITY_MULTIPLIER;
                            documentDetail.CustomUnitPrice = customPriceValue;
                            VatFactor vatFactor = documentDetail.Session.GetObjectByKey<VatFactor>(documentDetail.VatFactorGuid);
                            string errorMessage = DocumentHelper.RecalculateTemporaryDocumentDetail(documentHeader, documentDetail, quantity, vatFactor, CustomMeasurementUnit, customDescription, Remarks, isPercentage, userDiscount, priceCatalogDetail);
                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                result["DetailIsForSave"] = (triggered_by == "btnSaveItem" || triggered_by == "btnSaveCloseItem") ? true : false;
                                if (isSingleEditForm)
                                {
                                    DocumentHelper.UpdateLinkedItems(ref documentHeader, documentDetail);
                                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                                    SaveObjectTemp(documentHeader, CurrentUser);
                                }
                            }
                            else
                            {
                                Session["Error"] = errorMessage;
                            }
                        }
                    }
                    break;
                case "custom_price":
                    result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");

                    if (documentDetail != null)
                    {
                        Session["DocumentDetail2Edit"] = null;
                        decimal customUnitPrice;
                        if (decimal.TryParse(custom_price, out customUnitPrice) == false)
                        {
                            Session["Error"] = Resources.InvalidValue;
                            return Json(new { error = Session["Error"] });
                        }
                        customUnitPrice /= DocumentHelper.QUANTITY_MULTIPLIER;

                        if (customUnitPrice == 0 && documentHeader.DocumentType != null && documentHeader.DocumentType.AllowItemZeroPrices == false)
                        {
                            Session["Error"] = Resources.InvalidValue;
                            return Json(new { error = Session["Error"] });
                        }

                        documentDetail.CustomUnitPrice = customUnitPrice;
                        documentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader, documentDetail.Item, documentDetail.Barcode, documentDetail.Qty, false, customUnitPrice, true,
                                                                              documentDetail.CustomDescription, documentDetail.DocumentDetailDiscounts, true, CustomMeasurementUnit);

                        documentDetail.Remarks = Remarks;
                        Session["currentDocumentDetail"] = documentDetail;
                    }
                    break;
                case "Remarks":
                    if (documentDetail != null)
                    {
                        documentDetail.Remarks = Remarks;
                    }
                    result["DocumentDetailFormMode"] = documentDetailFormMode.Replace("Start", "");
                    break;
                default:
                    Session["Error"] = Resources.AnErrorOccurred;
                    return Json(new { error = Session["Error"] });
            }



            string itemCode = documentDetail != null && documentDetail.Item != null ? documentDetail.Item.Code : "";
            result["item_info_code"] = OwnerApplicationSettings.PadItemCodes && OwnerApplicationSettings.TrimBarcodeOnDisplay ? itemCode.TrimStart(OwnerApplicationSettings.ItemCodePaddingCharacter.ToCharArray()) : itemCode;
            result["item_info_barcode"] = documentDetail != null && documentDetail.Barcode != null ? documentDetail.Barcode.Code : "";
            result["item_info_name"] = documentDetail != null ? documentDetail.CustomDescription : "";
            result["spinlineqty"] = documentDetail != null ? documentDetail.PackingQuantity : 0;

            string customMeasurementUnit = documentDetail == null ? "" : documentDetail.CustomMeasurementUnit;
            if (documentHeader.UsesPackingQuantities && documentDetail != null)
            {
                customMeasurementUnit = documentDetail.MeasurementUnitsQuantities;
                if (string.IsNullOrEmpty(customMeasurementUnit))
                {
                    customMeasurementUnit = documentDetail.CustomMeasurementUnit;
                }
            }

            result["CustomMeasurementUnit"] = customMeasurementUnit;

            result["MeasurementUnitLabel"] = documentHeader.UsesPackingQuantities ? Resources.MeasurementUnits : Resources.MeasurementUnit;
            result["fpa_factor"] = documentDetail != null ? documentDetail.VatFactor : 0;
            result["custom_price"] = documentDetail != null ? documentDetail.CustomUnitPrice : 0;

            result["total_cost"] = documentDetail != null ? (documentHeader.DocumentType.IsForWholesale ? documentDetail.NetTotalBeforeDiscount : documentDetail.GrossTotalBeforeDiscount) : 0;
            result["price_catalog_discount_percentage"] = priceCatalogDetail != null ? 100 * priceCatalogDetail.Discount : 0;


            if (documentHeader != null && documentDetail != null && priceCatalogDetail != null)
            {
                decimal priceCatalogDiscountValue = 0;
                decimal customPrice = documentDetail.CustomUnitPrice;
                priceCatalogDiscountValue = documentDetail.Qty * customPrice * priceCatalogDetail.Discount;
                result["price_catalog_discount_value"] = priceCatalogDiscountValue;
            }
            else
            {
                result["price_catalog_discount_value"] = 0;
            }

            result["userDiscountPercentage"] = (documentDetail != null && documentHeader != null && documentHeader.DocumentType != null)
                                                ? 100 * (documentHeader.DocumentType.IsForWholesale
                                                        ? documentDetail.CustomDiscountsPercentageWholeSale
                                                        : documentDetail.CustomDiscountsPercentageRetail
                                                    )
                                               : 0;
            result["userDiscountMoneyValue"] = (documentDetail != null && documentHeader != null && documentHeader.DocumentType != null) ? documentDetail.CustomDiscountsAmount : 0;
            result["isPercentage"] = isPercentage;
            result["userDiscount"] = documentDetail != null ? (isPercentage == "true" ? result["userDiscountPercentage"] : result["userDiscountMoneyValue"]) : 0;
            result["total_discount_percentage"] = documentDetail != null ? Math.Round(100 * (documentHeader.DocumentType.IsForWholesale ? documentDetail.TotalDiscountPercentage : documentDetail.TotalDiscountPercentageWithVat), (int)documentHeader.Owner.OwnerApplicationSettings.DisplayDigits, MidpointRounding.AwayFromZero) : 0;
            result["total_discount"] = documentDetail != null ? (documentHeader.DocumentType.IsForWholesale ? documentDetail.TotalDiscount : documentDetail.TotalDiscountIncludingVAT) : 0;
            result["net_total"] = documentDetail != null ? documentDetail.NetTotal : 0;
            result["total_vat_amount"] = documentDetail != null ? documentDetail.TotalVatAmount : 0;
            result["final_sum"] = result["gross_total"] = documentDetail != null ? documentDetail.GrossTotal : 0;
            result["final_sum"] += " &euro;";
            result["Remarks"] = documentDetail != null ? documentDetail.Remarks : "";
            result["points"] = documentDetail != null ? documentDetail.Points : 0;

            List<string> input_order = new List<string>{
                //"item_info_code",
                //"item_info_barcode",
                //"item_info_name",
                "spinlineqty",
                //"CustomMeasurementUnit",
                //"fpa_factor",
                //"custom_price",
                //"total_cost",
                //"price_catalog_discount_percentage",
                //"price_catalog_discount_value",
                //"userDiscount","isPercentage",
                "userDiscountPercentage",
                "userDiscountMoneyValue",
                //"total_discount_percentage",
                //"total_discount",
                //"net_total",
                //"total_vat_amount",
                //"gross_total",
                //"Remarks",
                //"triggered_by",//who triggered the call
                //"DocumentDetailFormMode"//current form mode
            };
            string focus_on = "";
            if (Session["Error"] != null && !string.IsNullOrEmpty(Session["Error"].ToString()))
            {
                focus_on = triggered_by;
            }
            else
            {
                int trigger_index = input_order.IndexOf(triggered_by);
                if (documentDetail == null)
                {
                    focus_on = "barcode_search";
                }
                else if (trigger_index != -1)//triggered by input found in List input order
                {
                    int newIndex = (input_order.IndexOf(triggered_by) + 1);
                    if (triggered_by == "userDiscountMoneyValue" && newitem)
                    {
                        focus_on = "btnSaveItem";
                    }
                    else if (newIndex < input_order.Count)
                    {
                        focus_on = input_order[newIndex];
                    }
                    else
                    {
                        focus_on = "btnSaveItem";
                    }
                }
                else if (trigger_index == -1)//triggered by input not found in List input order
                {
                    if (triggered_by == "btnSaveItem" || triggered_by == "btnSaveCloseItem")
                    {
                        focus_on = triggered_by;
                    }
                    else
                    {
                        focus_on = input_order.First();
                    }
                }

                bool isOrder = documentHeader.DocumentType == null ? false : documentHeader.DocumentType.IsDefault;
                if (isOrder && trigger_index == 0 && UserHelper.IsCustomer(CurrentUser))
                {
                    focus_on = "Remarks";
                }
                bool retain_focus = false;
                if (Boolean.TryParse(JSHelper.GetInputValue("retain_focus"), out retain_focus) && retain_focus)
                {
                    focus_on = triggered_by;
                }
            }
            result["focus_on"] = focus_on;
            return Json(new { result = result });
        }

        public ActionResult SelectCustomer()
        {
            return PartialView("SelectCustomer");
        }

        public ActionResult SelectSupplier()
        {
            return PartialView("SelectSupplier");
        }

        public static object SupplierRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                return XpoHelper.GetNewUnitOfWork().GetObjectByKey<SupplierNew>(e.Value);
            }
            return null;
        }

        public static object TraderRequestedByFilterCondition<W>(ListEditItemsRequestedByFilterConditionEventArgs e) where W : BaseObj
        {
            if (e.Filter == "") { return null; }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            DocumentHeader docHeader = System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader;
            CriteriaOperator crop = DocumentHelper.CustomerCriteria(proccessed_filter, docHeader, EffectiveOwner);
            SortProperty sortproperty = new SortProperty("CompanyName", SortingDirection.Ascending);

            crop = ApplyOwnerCriteria(crop, typeof(W), EffectiveOwner);

            UnitOfWork uow = docHeader == null ? XpoHelper.GetNewUnitOfWork() : (UnitOfWork)(docHeader.Session);
            XPCollection<W> searched_items = new XPCollection<W>(uow, crop, sortproperty);

            searched_items.SkipReturnedObjects = e.BeginIndex;
            searched_items.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return searched_items;
        }

        public static object PriceCatalogRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (e.Filter == "") { return null; }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            UnitOfWork uow = (System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader).Session as UnitOfWork;
            Store store = (System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader).Store;



            CriteriaOperator crop = CriteriaOperator.And(new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Oid", store.Oid)),
                                                         CriteriaOperator.Or(new BinaryOperator("Description", proccessed_filter, BinaryOperatorType.Like),
                                                                             new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like)));


            XPCollection<PriceCatalog> searched_pricecatalogs = GetList<PriceCatalog>(uow, crop);
            searched_pricecatalogs.SkipReturnedObjects = e.BeginIndex;
            searched_pricecatalogs.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return searched_pricecatalogs;
        }

        public static object PriceCatalogPolicyRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
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

            UnitOfWork uow = (System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader).Session as UnitOfWork;
            Store store = (System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader).Store;



            CriteriaOperator crop = CriteriaOperator.And(new ContainsOperator("StorePriceCatalogPolicies", new BinaryOperator("Store.Oid", store.Oid)),
                                                         CriteriaOperator.Or(new BinaryOperator("Description", proccessed_filter, BinaryOperatorType.Like),
                                                                             new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like)));


            XPCollection<PriceCatalogPolicy> priceCatalogPolicies = GetList<PriceCatalogPolicy>(uow, crop);
            priceCatalogPolicies.SkipReturnedObjects = e.BeginIndex;
            priceCatalogPolicies.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return priceCatalogPolicies;
        }

        public ActionResult DocumentSummary()
        {
            return PartialView();
        }

        public ActionResult DocumentSummaryPartial()
        {
            ViewBag.InViewMode = false;
            return PartialView();
        }

        public JsonResult jsonCancelOrder()
        {
            try
            {
                DocumentHeader document = Session["currentDocument"] as DocumentHeader;
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                using (uow)
                {
                    TemporaryObject tempObject = document.GetTemporaryObject(uow);
                    if (tempObject != null)
                    {
                        tempObject.Delete();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                ClearDocumentSession();
                return Json(new { });
            }
            catch (Exception e)
            {
                string exceptionMessage = e.GetFullMessage();
                return Json(new { error = Resources.AnErrorOccurred });
            }
        }

        public JsonResult jsonClearCurrentOrder()
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            if (document != null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    TemporaryObject tempObject = document.GetTemporaryObject(uow);
                    if (tempObject != null)
                    {
                        tempObject.Delete();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
            }
            ClearDocumentSession();
            return Json(new { });
        }

        public ActionResult SearchByDescriptionPurchase()
        {
            return PartialView();
        }

        public ActionResult SearchByDescriptionStore()
        {
            return PartialView();
        }

        public static object ItemAllRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Filter))
            {
                return null;
            }
            DocumentHeader documentHeader = System.Web.HttpContext.Current.Session["currentDocument"] as DocumentHeader;
            string[] filter_fields = e.Filter.Replace("*", "%").Split(' ');
            CriteriaOperator filterName = null;
            foreach (string filt in filter_fields.Where(str => string.IsNullOrWhiteSpace(str) == false))
            {
                string temporaryFilter = (filt.Contains('%')) ? filt : String.Format("%{0}%", filt);
                CriteriaOperator documentDivisionFilter = null;

                switch (documentHeader.Division)
                {
                    case eDivision.Purchase:
                    case eDivision.Store:
                    case eDivision.Sales:
                        documentDivisionFilter = CriteriaOperator.Or(new BinaryOperator("Name", temporaryFilter, BinaryOperatorType.Like),
                            new BinaryOperator("Code", temporaryFilter, BinaryOperatorType.Like));
                        break;
                    default:
                        break;
                }

                filterName = CriteriaOperator.And(filterName, documentDivisionFilter);
            }
            switch (documentHeader.Division)
            {
                case eDivision.Sales:
                    PriceCatalogPolicy currentPriceCatalogPolicy = documentHeader.PriceCatalogPolicy;
                    IEnumerable<PriceCatalogDetail> priceCatalogDetails = new List<PriceCatalogDetail>();
                    if (e.Filter == "" || currentPriceCatalogPolicy == null)
                    {
                        return null;
                    }
                    else
                    {
                        CriteriaOperator itemFilterFinal = CriteriaOperator.And(
                                                                                filterName,
                                                                                DocumentHelper.DocumentTypeSupportedItemsCriteria(documentHeader));
                        EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(documentHeader.Store);
                        priceCatalogDetails = documentHeader.Session.Query<Item>().AppendWhere(new CriteriaToExpressionConverter(), itemFilterFinal)
                                                            .Cast<Item>().ToList()
                                                            .Select(item => PriceCatalogHelper
                                                                                    .GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                effectivePriceCatalogPolicy,
                                                                                                item, null,
                                                                                                PriceCatalogSearchMethod.PRICECATALOG_TREE,
                                                                                                null
                                                                                                )
                                                            )
                                                            .Where(priceCatalogPolicyPriceResult => priceCatalogPolicyPriceResult != null
                                                                                                  && priceCatalogPolicyPriceResult.PriceCatalogDetail != null
                                                                                                  && priceCatalogPolicyPriceResult.PriceCatalogDetail.Value > 0)
                                                            .Select(priceCatalogPolicyPriceResult => priceCatalogPolicyPriceResult.PriceCatalogDetail);
                    }
                    return priceCatalogDetails;
                //break;
                case eDivision.Purchase:
                case eDivision.Store:
                    if (e.Filter == "")
                    {
                        return null;
                    }
                    //Checks for items in available Items Categories
                    XPCollection<Item> items = GetList<Item>(documentHeader.Session, CriteriaOperator.And(filterName, DocumentHelper.DocumentTypeSupportedItemsCriteria(documentHeader)));
                    int skipItems = e.BeginIndex;
                    int len = e.EndIndex - e.BeginIndex + 1;
                    List<Item> returnItems;
                    if (skipItems == -1 && len == -1)
                    {
                        returnItems = items.OrderByDescending(g => g.InsertedDate).ToList();
                    }
                    else if (skipItems != -1 && len == -1)
                    {
                        returnItems = items.OrderByDescending(g => g.InsertedDate).Skip(skipItems).ToList();
                    }
                    else
                    {
                        returnItems = items.OrderByDescending(g => g.InsertedDate).Skip(skipItems).Take(len).ToList();
                    }
                    return returnItems;
                default:
                    throw new Exception();
            }
        }

        public JsonResult jsonSearchByBarcode()
        {
            DocumentHeader documentHeader = (Session["currentDocument"] as DocumentHeader);
            //fix case 5564
            if (documentHeader == null)
            {
                Session["Error"] = Resources.ConnectionTimeOut;
                return Json(new { });
            }

            if (documentHeader.Customer == null && documentHeader.Division == eDivision.Sales)
            {
                Session["Error"] = Resources.PleaseSelectACustomer;
                return Json(new { });
            }
            else if (documentHeader.Supplier == null && documentHeader.Division == eDivision.Purchase)
            {
                Session["Error"] = Resources.PleaseSelectASupplier;
                return Json(new { });
            }
            string search = Request["user_search"];
            if (documentHeader.Owner.OwnerApplicationSettings.PadBarcodes)
            {
                search = search.PadLeft(documentHeader.Owner.OwnerApplicationSettings.BarcodeLength, documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
            }

            CriteriaOperator ownerCriteria = null;
            switch (documentHeader.Division)
            {
                case eDivision.Other:
                    break;
                case eDivision.Purchase:
                    string userSearch = Request["user_search"];
                    CriteriaOperator purchaseBarcodeCriteria = ApplyOwnerCriteria(new BinaryOperator("Code", search, BinaryOperatorType.Equal), typeof(Barcode), documentHeader.Owner);
                    Barcode purchaseBarcode = documentHeader.Session.FindObject<Barcode>(purchaseBarcodeCriteria);
                    ItemBarcode itemBarcode = null;
                    if (purchaseBarcode == null)
                    {
                        if (documentHeader.Owner.OwnerApplicationSettings.PadItemCodes)
                        {
                            search = Request["user_search"];
                            search = search.PadLeft(documentHeader.Owner.OwnerApplicationSettings.ItemCodeLength, documentHeader.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                        }

                        ownerCriteria = ApplyOwnerCriteria(new BinaryOperator("Code", search, BinaryOperatorType.Equal), typeof(Barcode), documentHeader.Owner);
                        purchaseBarcode = documentHeader.Session.FindObject<Barcode>(ownerCriteria);
                    }

                    if (purchaseBarcode != null)
                    {
                        CriteriaOperator purchaseItemCriteria = ApplyOwnerCriteria(new BinaryOperator("Barcode.Oid", purchaseBarcode.Oid), typeof(ItemBarcode), documentHeader.Owner);
                        itemBarcode = documentHeader.Session.FindObject<ItemBarcode>(purchaseItemCriteria);
                    }
                    WeightedBarcodeInfo purchaseWeightedBarcodeInfo = null;
                    if (purchaseBarcode == null || itemBarcode == null)
                    {
                        purchaseWeightedBarcodeInfo = ItemHelper.GetWeightedBarcodeInfo(userSearch, documentHeader);
                        if (purchaseWeightedBarcodeInfo != null)
                        {
                            if (purchaseWeightedBarcodeInfo.Barcode.HasValue)
                            {
                                purchaseBarcode = documentHeader.Session.GetObjectByKey<Barcode>(purchaseWeightedBarcodeInfo.Barcode.Value);
                            }

                            if (purchaseWeightedBarcodeInfo.ItemBarcode.HasValue)
                            {
                                itemBarcode = documentHeader.Session.GetObjectByKey<ItemBarcode>(purchaseWeightedBarcodeInfo.ItemBarcode.Value);
                            }
                        }
                    }

                    if (purchaseBarcode == null || itemBarcode == null)
                    {
                        Session["Error"] = Resources.ItemNotFound;
                        return Json(new { });
                    }
                    return SearchItemPurchase(itemBarcode.Item, purchaseBarcode, purchaseWeightedBarcodeInfo);
                case eDivision.Sales:
                    Session["currentPriceCatalogDetail"] = Session["currentDocumentDetail"] = Session["barcode_search"] = null;
                    PriceCatalogPolicy currentPriceCatalogPolicy = ((DocumentHeader)Session["currentDocument"]).PriceCatalogPolicy;
                    if (currentPriceCatalogPolicy == null)
                    {
                        Session["Error"] = Resources.SelectPriceCatalog;
                        return Json(new { });
                    }


                    ownerCriteria = new BinaryOperator("Code", search);
                    ownerCriteria = ApplyOwnerCriteria(ownerCriteria, typeof(Barcode), documentHeader.Owner);
                    Session["barcode_search"] = documentHeader.Session.FindObject<Barcode>(ownerCriteria);

                    PriceCatalogDetail pricecatalogdetail;
                    using (MiniProfiler.Current.Step("jsonSearchByBarcode.GetItemByBarcode"))
                    {
                        PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                documentHeader.EffectivePriceCatalogPolicy,
                                                                                                search);
                        pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    }

                    if (pricecatalogdetail == null)
                    {
                        if (documentHeader.Owner.OwnerApplicationSettings.PadItemCodes)
                        {
                            search = Request["user_search"];
                            search = search.PadLeft(documentHeader.Owner.OwnerApplicationSettings.ItemCodeLength, documentHeader.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                        }
                        using (MiniProfiler.Current.Step("jsonSearchByBarcode.GetItemByBarcode #2"))
                        {
                            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                    documentHeader.EffectivePriceCatalogPolicy,
                                                                                                    search);
                            pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                        }
                    }

                    ownerCriteria = new BinaryOperator("Code", search, BinaryOperatorType.Equal);
                    ownerCriteria = ApplyOwnerCriteria(ownerCriteria, typeof(Barcode), documentHeader.Owner);
                    Barcode salesBarcode = documentHeader.Session.FindObject<Barcode>(ownerCriteria);
                    WeightedBarcodeInfo salesWeightedBarcodeInfo = null;
                    if (pricecatalogdetail == null && Request["user_search"] != null)
                    {
                        search = Request["user_search"];
                        salesWeightedBarcodeInfo = ItemHelper.GetWeightedBarcodeInfo(search, documentHeader);
                        if (salesWeightedBarcodeInfo != null)
                        {
                            if (salesWeightedBarcodeInfo.PriceCatalogDetail.HasValue)
                            {
                                pricecatalogdetail = documentHeader.Session.GetObjectByKey<PriceCatalogDetail>(salesWeightedBarcodeInfo.PriceCatalogDetail.Value);
                            }
                            if (salesWeightedBarcodeInfo.Barcode.HasValue)
                            {
                                salesBarcode = documentHeader.Session.GetObjectByKey<Barcode>(salesWeightedBarcodeInfo.Barcode.Value);
                            }
                        }
                    }
                    if (salesBarcode == null || pricecatalogdetail == null)
                    {
                        Session["Error"] = Resources.ItemNotFound;
                        return Json(new { });
                    }
                    return SearchItem(pricecatalogdetail, salesBarcode, salesWeightedBarcodeInfo);
                case eDivision.Store:
                    string userCodeSearch = Request["user_search"];
                    CriteriaOperator storeBarcodeCriteria = ApplyOwnerCriteria(new BinaryOperator("Code", search, BinaryOperatorType.Equal), typeof(Barcode), documentHeader.Owner);

                    ItemBarcode storeItemBarcode = null;
                    Barcode storeBarcode = documentHeader.Session.FindObject<Barcode>(storeBarcodeCriteria);

                    if (storeBarcode == null)
                    {
                        if (documentHeader.Owner.OwnerApplicationSettings.PadItemCodes)
                        {
                            search = userCodeSearch;
                            search = search.PadLeft(documentHeader.Owner.OwnerApplicationSettings.ItemCodeLength, documentHeader.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                        }

                        ownerCriteria = ApplyOwnerCriteria(new BinaryOperator("Code", search, BinaryOperatorType.Equal), typeof(Barcode), documentHeader.Owner);
                        storeBarcode = documentHeader.Session.FindObject<Barcode>(ownerCriteria);
                    }

                    if (storeBarcode != null)
                    {
                        CriteriaOperator storeItemBarcodeCriteria = ApplyOwnerCriteria(new BinaryOperator("Barcode.Oid", storeBarcode.Oid), typeof(ItemBarcode), documentHeader.Owner);
                        storeItemBarcode = documentHeader.Session.FindObject<ItemBarcode>(storeItemBarcodeCriteria);
                    }

                    WeightedBarcodeInfo storeWeightedBarcodeInfo = null;
                    if (storeBarcode == null || storeItemBarcode == null)
                    {
                        storeWeightedBarcodeInfo = ItemHelper.GetWeightedBarcodeInfo(userCodeSearch, documentHeader);
                        if (storeWeightedBarcodeInfo != null)
                        {
                            if (storeWeightedBarcodeInfo.Barcode.HasValue)
                            {
                                storeBarcode = documentHeader.Session.GetObjectByKey<Barcode>(storeWeightedBarcodeInfo.Barcode.Value);
                            }
                            if (storeWeightedBarcodeInfo.ItemBarcode.HasValue)
                            {
                                storeItemBarcode = documentHeader.Session.GetObjectByKey<ItemBarcode>(storeWeightedBarcodeInfo.ItemBarcode.Value);
                            }
                        }
                    }

                    if (storeBarcode == null || storeItemBarcode == null)
                    {
                        Session["Error"] = Resources.ItemNotFound;
                        return Json(new { });
                    }
                    return SearchItemStore(storeItemBarcode.Item, storeBarcode, storeWeightedBarcodeInfo);
            }
            throw new Exception();
        }

        private JsonResult SearchItemStore(Item item, Barcode barcode, WeightedBarcodeInfo weightedBarcodeInfo)
        {

            DocumentHeader documentHeader = (DocumentHeader)Session["currentDocument"];

            if (item == null)
            {
                Session["Error"] = Resources.ItemNotFound;
                return Json(new { });
            }
            decimal found_document_detail_qty = .0m;
            found_document_detail_qty = documentHeader.DocumentDetails.Where(detail => detail.Item.Oid == item.Oid).Sum(x => x.Qty);

            if (found_document_detail_qty > .0m)
            {
                string errorMessage = String.Empty;
                ApplyCompositionDecompositionConstraints(item, documentHeader, out errorMessage);
                if (String.IsNullOrWhiteSpace(errorMessage) == false)
                {
                    Session["Error"] = errorMessage;
                    return Json(new { });
                }
                Session["Notice"] = Resources.ItemAlreadyOrderedWithQuantity + " " + found_document_detail_qty;
            }
            if (barcode == null)
            {
                barcode = item.DefaultBarcode;
            }

            CalculateTemporaryDocumentDetailStore(item, barcode, weightedBarcodeInfo);

            return Json(new
            {
                existing_item_qty = found_document_detail_qty,
                item = item.Name,
                item_info_code = item.Code,
                item_info_barcode = barcode.Code,
                item_info_name = item.Name,
            });
        }

        private void ApplyCompositionDecompositionConstraints(Item item, DocumentHeader documentHeader, out string errorMessage)
        {
            errorMessage = String.Empty;
            if (documentHeader.DocumentType.ManualLinkedLineInsertion
                            && documentHeader.DocumentDetails.Where(detail => detail.Item.Oid == item.Oid && detail.IsLinkedLine == false).Count() > 0
                               )
            {
                errorMessage = Resources.MainItemAndLinkedItemCannotMatch;
            }
        }

        private JsonResult SearchItemPurchase(Item item, Barcode bc, WeightedBarcodeInfo weightedBarcodeInfo)
        {

            DocumentHeader documentHeader = (DocumentHeader)Session["currentDocument"];

            if (item == null)
            {
                Session["Error"] = Resources.ItemNotFound;
                return Json(new { });
            }

            if (!DocumentHelper.DocumentTypeSupportsItem(documentHeader, item))
            {
                Session["currentPriceCatalogDetail"] = null;
                Session["Error"] = Resources.ItemNotSupported;
                return Json(new { item = "", prices = "" });
            }

            decimal found_document_detail_qty = .0m;

            found_document_detail_qty = documentHeader.DocumentDetails.Where(g => g.Item.Oid == item.Oid).Sum(x => x.Qty);

            if (found_document_detail_qty > .0m)
            {
                string errorMessage = String.Empty;
                ApplyCompositionDecompositionConstraints(item, documentHeader, out errorMessage);
                if (String.IsNullOrWhiteSpace(errorMessage) == false)
                {
                    Session["Error"] = errorMessage;
                    return Json(new { });
                }
                Session["Notice"] = Resources.ItemAlreadyOrderedWithQuantity + " " + found_document_detail_qty;
            }
            if (bc == null)
            {
                bc = item.DefaultBarcode;
            }

            CalculateTemporaryDocumentDetailPurchase(item, bc, weightedBarcodeInfo);

            return Json(new
            {
                existing_item_qty = found_document_detail_qty,
                item = item.Name,
                item_info_code = item.Code,
                item_info_barcode = bc.Code,
                item_info_name = item.Name,
            });
        }

        public JsonResult jsonSelectByDescription()
        {
            DocumentHeader documentHeader = (Session["currentDocument"] as DocumentHeader);

            if (documentHeader.Customer == null && documentHeader.Division == eDivision.Sales)
            {
                Session["Error"] = Resources.PleaseSelectACustomer;
                return Json(new { });
            }
            else if (documentHeader.Supplier == null && documentHeader.Division == eDivision.Purchase)
            {
                Session["Error"] = Resources.PleaseSelectASupplier;
                return Json(new { });
            }
            Session["NoticeFromDocumentDetails"] =
            Session["ErrorFromDocumentDetails"] =
            Session["currentPriceCatalogDetail"] = Session["currentDocumentDetail"] = Session["barcode_search"] = null;
            string search = Request["user_search"];
            if (String.IsNullOrEmpty(search))
            {
                Session["currentPriceCatalogDetail"] = null;
                return Json(new { item = "" });
            }
            switch (documentHeader.Division)
            {
                //case eDivision.Other:
                //    break;
                case eDivision.Purchase:
                    Guid searchOid;
                    if (Guid.TryParse(search, out searchOid))
                    {
                        Item item = documentHeader.Session.GetObjectByKey<Item>(searchOid);
                        return SearchItemPurchase(item, null, null);
                    }
                    Session["Error"] = Resources.ItemNotFound;
                    return Json(new { item = "" });
                case eDivision.Sales:
                    try
                    {
                        Guid selectedPriceCatalogDetailOid;
                        if (Guid.TryParse(search, out selectedPriceCatalogDetailOid))
                        {
                            PriceCatalogDetail selectedPriceCatalogDetail = documentHeader.Session.GetObjectByKey<PriceCatalogDetail>(selectedPriceCatalogDetailOid);
                            if (selectedPriceCatalogDetail == null)
                            {
                                Session["Error"] = Resources.ItemNotFound;
                                return Json(new { item = "" });
                            }
                            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                                       documentHeader.EffectivePriceCatalogPolicy,
                                                                                                                       selectedPriceCatalogDetail.Item,
                                                                                                                       selectedPriceCatalogDetail.Barcode);
                            PriceCatalogDetail pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

                            if (pricecatalogdetail != null)
                            {
                                Session["barcode_search"] = pricecatalogdetail.Barcode;
                            }
                            return SearchItem(pricecatalogdetail);
                        }
                        else
                        {
                            Session["Error"] = Resources.ItemNotFound;
                            return Json(new { item = "" });
                        }
                    }
                    catch (Exception e)
                    {
                        Session["Error"] = e.Message;
                        return Json(new { item = "" });
                    }
                case eDivision.Store:
                    Guid itemOid;
                    if (Guid.TryParse(search, out itemOid))
                    {
                        Item item = documentHeader.Session.GetObjectByKey<Item>(itemOid);
                        return SearchItemStore(item, null, null);
                    }
                    Session["Error"] = Resources.ItemNotFound;
                    return Json(new { item = "" });
                default:
                    throw new Exception();
            }
        }

        protected JsonResult SearchItem(PriceCatalogDetail pricecatalogdetail, Barcode item_info_barcode = null, WeightedBarcodeInfo weightedBarcodeInfo = null)
        {
            DocumentHeader documentHeader = (DocumentHeader)Session["currentDocument"];
            using (MiniProfiler.Current.Step("SearchItem"))
            {
                if (pricecatalogdetail == null)
                {
                    Session["currentPriceCatalogDetail"] = null;
                    Session["Error"] = Resources.ItemNotFound;
                    return Json(new { item = "", prices = "" });
                }

                if (!DocumentHelper.DocumentTypeSupportsItem(documentHeader, pricecatalogdetail.Item))
                {
                    Session["currentPriceCatalogDetail"] = null;
                    Session["Error"] = Resources.ItemNotSupported;
                    return Json(new { item = "", prices = "" });
                }


                Session["currentPriceCatalogDetail"] = pricecatalogdetail;

                IEnumerable<DocumentDetail> sameDetails = documentHeader.DocumentDetails.Where(docDetail => docDetail.Item.Oid == pricecatalogdetail.Item.Oid);
                decimal found_document_detail_qty = sameDetails.Count() == 0 ? .0m : sameDetails.Sum(x => x.Qty);

                if (found_document_detail_qty > .0m)
                {
                    string errorMessage = String.Empty;
                    ApplyCompositionDecompositionConstraints(pricecatalogdetail.Item, documentHeader, out errorMessage);
                    if (String.IsNullOrWhiteSpace(errorMessage) == false)
                    {
                        Session["Error"] = errorMessage;
                        return Json(new { });
                    }
                    Session["Notice"] = Resources.ItemAlreadyOrderedWithQuantity + " " + found_document_detail_qty;
                }

                if (item_info_barcode == null)
                {
                    Session["barcode_search"] = pricecatalogdetail.Barcode;
                }

                try
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = new PriceCatalogPolicyPriceResult()
                    {
                        PriceCatalogDetail = Session["currentPriceCatalogDetail"] as PriceCatalogDetail,
                        SearchBarcode = Session["barcode_search"] as Barcode
                    };
                    CalculateTemporaryDocumentDetail(priceCatalogPolicyPriceResult, weightedBarcodeInfo);
                }
                catch (Exception exc)
                {
                    Session["Error"] = exc.Message;
                    return Json(new
                    {
                        existing_item_qty = "",
                        item = "",
                        item_info_code = "",
                        item_info_barcode = "",
                        item_info_name = ""
                    });
                }

                return Json(new
                {
                    existing_item_qty = found_document_detail_qty,
                    item = pricecatalogdetail.Item.Name,
                    item_info_code = pricecatalogdetail.Item.Code,
                    item_info_barcode = item_info_barcode == null ? pricecatalogdetail.Barcode.Code : item_info_barcode.Code,
                    item_info_name = pricecatalogdetail.Item.Name,
                    qty = (Session["currentDocumentDetail"] as DocumentDetail).PackingQuantity
                });
            }
        }

        private void CalculateTemporaryDocumentDetailStore(Item item, Barcode barcode, WeightedBarcodeInfo weightedBarcodeInfo)
        {
            if (item != null && barcode != null)
            {
                decimal quantity = 1;
                decimal itemValue = 0;
                DocumentHeader currentDocumentHeader = Session["currentDocument"] as DocumentHeader;

                if (weightedBarcodeInfo != null)
                {
                    quantity = weightedBarcodeInfo.Quantity;
                    itemValue = weightedBarcodeInfo.Value;
                }

                Session["currentDocumentDetail"] = DocumentHelper.ComputeDocumentLine(
                                ref currentDocumentHeader,
                                item,
                                barcode,
                                quantity,
                                false,
                                itemValue,
                                true,
                                "",
                                null
                                );
                (Session["currentDocumentDetail"] as DocumentDetail).Remarks = JSHelper.GetInputValue("Remarks");
                MeasurementUnit barcodeMeasurementUnit = (Session["currentDocumentDetail"] as DocumentDetail).Barcode.MeasurementUnit(currentDocumentHeader.Owner);
                (Session["currentDocumentDetail"] as DocumentDetail).CustomMeasurementUnit = barcodeMeasurementUnit == null ? "" : barcodeMeasurementUnit.Description;
                (Session["currentDocumentDetail"] as DocumentDetail).CustomUnitPrice = (Session["currentDocumentDetail"] as DocumentDetail).UnitPrice;
            }
        }

        private void CalculateTemporaryDocumentDetailPurchase(Item item, Barcode barcode, WeightedBarcodeInfo weightedBarcodeInfo)
        {
            if (item != null && barcode != null)
            {
                decimal quantity = 1;
                DocumentHeader currentDocumentHeader = Session["currentDocument"] as DocumentHeader;
                decimal customUnitPrice = currentDocumentHeader.DocumentType.PriceSuggestionType == ePriceSuggestionType.NONE ? 0 :
                ItemHelper.GetSupplierLastPrice(item, (currentDocumentHeader.DocumentType.PriceSuggestionType == ePriceSuggestionType.LAST_SUPPLIER_PRICE) ? currentDocumentHeader.Supplier : null);

                if (weightedBarcodeInfo != null)
                {
                    quantity = weightedBarcodeInfo.Quantity;
                    customUnitPrice = weightedBarcodeInfo.Value;
                }

                Session["currentDocumentDetail"] = DocumentHelper.ComputeDocumentLine(
                                ref currentDocumentHeader,
                                item,
                                barcode,
                                quantity,
                                false,
                                customUnitPrice,
                                true,
                                "",
                                null
                                );
                (Session["currentDocumentDetail"] as DocumentDetail).Remarks = JSHelper.GetInputValue("Remarks");
                MeasurementUnit barcodeMeasurementUnit = (Session["currentDocumentDetail"] as DocumentDetail).Barcode.MeasurementUnit(currentDocumentHeader.Owner);
                (Session["currentDocumentDetail"] as DocumentDetail).CustomMeasurementUnit = barcodeMeasurementUnit == null ? "" : barcodeMeasurementUnit.Description;
                (Session["currentDocumentDetail"] as DocumentDetail).CustomUnitPrice = (Session["currentDocumentDetail"] as DocumentDetail).UnitPrice;

            }
        }

        private void CalculateTemporaryDocumentDetail(PriceCatalogPolicyPriceResult priceCatalogPricePolicyDetail, WeightedBarcodeInfo weightedBarcodeInfo)
        {
            if (priceCatalogPricePolicyDetail != null && priceCatalogPricePolicyDetail.PriceCatalogDetail != null)
            {
                decimal quantity = 1;
                decimal itemValue = -1;
                bool hasCustomPrice = weightedBarcodeInfo != null && weightedBarcodeInfo.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE;
                DocumentHeader currentDocumentHeader = Session["currentDocument"] as DocumentHeader;
                if (weightedBarcodeInfo != null)
                {
                    quantity = weightedBarcodeInfo.Quantity;
                    if (hasCustomPrice)
                    {
                        itemValue = weightedBarcodeInfo.Value;
                    }
                }

                Barcode barcode = priceCatalogPricePolicyDetail.SearchBarcode ?? priceCatalogPricePolicyDetail.PriceCatalogDetail.Barcode;
                Session["currentDocumentDetail"] = DocumentHelper.ComputeDocumentLine(
                                     ref currentDocumentHeader,
                                     priceCatalogPricePolicyDetail.PriceCatalogDetail.Item,
                                     barcode,
                                     quantity,
                                     false,
                                     itemValue,
                                     hasCustomPrice,
                                     "",
                                     null
                                     );

                (Session["currentDocumentDetail"] as DocumentDetail).Remarks = JSHelper.GetInputValue("Remarks");
                (Session["currentDocumentDetail"] as DocumentDetail).CustomMeasurementUnit = (Session["currentDocumentDetail"] as DocumentDetail).Barcode == null || (Session["currentDocumentDetail"] as DocumentDetail).Barcode.MeasurementUnit(currentDocumentHeader.Owner) == null ? "" : (Session["currentDocumentDetail"] as DocumentDetail).Barcode.MeasurementUnit(currentDocumentHeader.Owner).Description;
            }
        }



        private void CleanDocument()
        {
            Session["HideOrderLink"] = null;
            Session["DocumentDetail2Edit"] = null;
            Session["currentDocumentDetail"] = null;
            Session["currentPriceCatalogDetail"] = null;
            Session["CustomerFilter"] = null;
            Session["OldDocumentDetail"] = null;
            Session["mainLine"] = null;
        }

        private void ClearDocumentSession()
        {
            if (Session["currentDocument"] != null)
            {
                ((DocumentHeader)Session["currentDocument"]).Session.Dispose();
                Session["HideOrderLink"] = null;
                Session["currentCustomerGuid"] = null;
                Session["DocumentDetail2Edit"] = null;
                Session["itemSupplierGuid"] = null;
                Session["barcode_search"] = null;
                Session["currentPriceCatalogDetail"] = null;
                Session["currentDocumentDetail"] = null;
                Session["currentDocument"] = null;
                Session["TreeItems"] = null;
                Session["selected_items_qty"] = null;
                Session["currentOffer"] = null;
                Session["newItems"] = null;
                Session["ItemOfferDetails"] = null;
                Session["selected_offers_qty"] = null;
                Session["CustomerFilter"] = null;

                Session["mainLine"] = null;
                Session["OldDocumentDetail"] = null;//TODO Perhaps delete???
            }
        }

        public JsonResult AddItem()
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = Session["currentDocumentDetail"] as DocumentDetail;
            bool newitem = false;
            if (documentDetail != null)
            {
                string errormsg = "";
                if (DocumentHelper.MaxCountOfLinesExceeded(document, out errormsg))
                {
                    return Json(new { error = errormsg });
                }
                else
                {
                    try
                    {
                        DocumentHelper.AddItem(ref document, documentDetail);
                        DocumentHelper.RecalculateDocumentCosts(ref document, false);
                        SaveObjectTemp(document, CurrentUser);
                        Session["Notice"] = Resources.SuccesfullyAdded;
                    }
                    catch (Exception exception)
                    {
                        return Json(new { error = exception.GetFullMessage() });
                    }
                }
            }
            if (Boolean.TryParse(Request["check_box_state"], out newitem))
            {
                if (newitem)
                {
                    CleanDocument();
                }
                else
                {
                    Session["DocumentDetail2Edit"] = Session["currentDocumentDetail"];
                }
            }
            return Json(new { });
        }

        public JsonResult DeleteItem(string documentDetailOid)
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = Session["currentDocumentDetail"] as DocumentDetail;

            if (documentDetail == null)
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }
            DocumentHelper.DeleteItem(ref document, documentDetail);
            Session["Notice"] = Resources.SuccesfullyDeleted;
            DocumentHelper.RecalculateDocumentCosts(ref document, false);

            CleanDocument();

            return Json(new { });
        }

        public JsonResult EditItem(string DocumentDetailOid)
        {
            Guid DocumentDetailGuid;
            Session["DocumentDetail2Edit"] = null;

            if (Guid.TryParse(DocumentDetailOid, out DocumentDetailGuid))
            {
                DocumentHeader document = (DocumentHeader)Session["currentDocument"];
                if (document == null)
                {//View Mode
                    return Json(new { });
                }

                Session["DocumentDetail2Edit"] = Session["currentDocumentDetail"] = document.DocumentDetails.Where(document_detail => document_detail.Oid == DocumentDetailGuid).FirstOrDefault();

                switch (document.Division)
                {
                    case eDivision.Sales:

                        Session["currentPriceCatalogDetail"] = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(document.Session as UnitOfWork,
                                                                                                                  document.EffectivePriceCatalogPolicy,
                                                                                                                  (Session["currentDocumentDetail"] as DocumentDetail).Item,
                                                                                                                  (Session["currentDocumentDetail"] as DocumentDetail).Barcode
                                                                                                                  );
                        break;

                }
                ViewBag.DocumentDetailIsNew = false;
            }

            return Json(new { });
        }

        public ActionResult UpdateHeader()
        {
            DocumentHeader document = (DocumentHeader)Session["currentDocument"];
            ViewBag.InViewMode = false;
            if (document != null)
            {
                ViewBag.SfaListComboBox = GetList<Model.SFA>(document.Session, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal))).ToList() ?? new List<SFA>();
            }

            IEnumerable<DocumentSeries> docSeries = null;
            try
            {
                Session session = document.Session;

                if (Request["field_name"] == "secondaryStores")
                {
                    Guid secondaryStoreGuid = Guid.Empty;
                    if (Guid.TryParse(Request["field_value"], out secondaryStoreGuid))
                    {
                        document.SecondaryStore = document.Session.GetObjectByKey<Store>(secondaryStoreGuid);
                    }
                }
                else if (Request["field_name"] == "HasBeenChecked")
                {
                    document.HasBeenChecked = Boolean.Parse(Request["field_value"]);
                }
                else if (Request["field_name"] == "ExecutionDate")
                {
                    document.ExecutionDate = DocumentHelper.ConvertToDateTime(Request["field_value"]);
                }
                else if (Request["field_name"] == "FinalizedDate")
                {
                    document.FinalizedDate = DocumentHelper.ConvertToDateTime(Request["field_value"]);
                }
                else if (Request["field_name"] == "HasBeenExecuted")
                {
                    document.HasBeenExecuted = Boolean.Parse(Request["field_value"]);
                }
                else if (Request["field_name"] == "InvoicingDate")
                {
                    document.InvoicingDate = DocumentHelper.ConvertToDateTime(Request["field_value"]);
                }
                else if (Request["field_name"] == "AddressProfession")
                {
                    document.AddressProfession = Request["field_value"];
                }
                else if (Request["field_name"] == "Tablet")
                {
                    Guid objectOid = String.IsNullOrEmpty(Request["field_value"]) ? Guid.Empty : Guid.Parse(Request["field_value"]);
                    document.Tablet = session.GetObjectByKey<SFA>(objectOid);
                }
                else
                {
                    Guid objectOid = String.IsNullOrEmpty(Request["field_value"]) ? Guid.Empty : Guid.Parse(Request["field_value"]);
                    if (Request["field_name"] == "Status")
                    {
                        document.Status = session.GetObjectByKey<DocumentStatus>(objectOid);
                    }
                    else
                    {
                        if (Request["field_name"] == "PriceCatalogPolicyCb")
                        {
                            Session["notIncludedDetailsOids"] = null;
                            PriceCatalogPolicy newPriceCatalogPolicy = session.GetObjectByKey<PriceCatalogPolicy>(objectOid);
                            EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(document.Store, newPriceCatalogPolicy);
                            List<DocumentDetail> notIncludedDetails = DocumentHelper.PriceCatalogNotIncludedItems(document, effectivePriceCatalogPolicy);
                            if (notIncludedDetails.Count == 0)
                            {
                                document.PriceCatalogPolicy = newPriceCatalogPolicy;
                            }
                            else
                            {
                                Session["notIncludedDetailsOids"] = notIncludedDetails.Select(detail => detail.Oid.ToString()).ToList();
                                throw new Exception(Resources.SelectedPriceCatalogPolicyNotContainsAllItems);
                            }
                        }
                        else
                        {
                            if (Request["field_name"] == "BillingAddress")
                            {
                                document.BillingAddress = session.GetObjectByKey<Address>(objectOid);
                            }
                            else
                            {
                                if (Request["field_name"] == "storeCustomers")
                                {
                                    Customer newCustomer = session.GetObjectByKey<Customer>(objectOid);
                                    EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(document.Store, newCustomer);
                                    List<DocumentDetail> notIncludedDetails = DocumentHelper.PriceCatalogNotIncludedItems(document, effectivePriceCatalogPolicy);
                                    if (notIncludedDetails.Count == 0)
                                    {

                                        document.Customer = newCustomer;
                                        document.DefaultCustomerDiscount = (decimal)newCustomer.Discount * 100;
                                        JsonUpdateCustomerDocumentDiscount(document.DefaultDocumentDiscount, document.DefaultCustomerDiscount);
                                    }
                                    else
                                    {
                                        Session["notIncludedDetailsOids"] = notIncludedDetails.Select(detail => detail.Oid.ToString()).ToList();
                                        throw new Exception(Resources.SelectedCustomerPriceCatalogPolicyNotContainsAllItems);
                                    }
                                }
                                else if (Request["field_name"] == "storeSuppliers")
                                {
                                    document.Supplier = session.GetObjectByKey<SupplierNew>(objectOid);
                                }
                                else
                                {
                                    if (Request["field_name"] == "DocumentType")
                                    {
                                        document.DocumentType = session.GetObjectByKey<DocumentType>(objectOid);
                                        document.DocumentSeries = null;
                                        document.Status = document.DocumentType.DefaultDocumentStatus;
                                        eModule module = DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance);
                                        docSeries = StoreHelper.StoreSeriesForDocumentType(document.Store, document.DocumentType, module: module).Where(ds => ds.IsCancelingSeries == false);
                                        document.DocumentSeries = docSeries.Count() == 1 && (document.DocumentSeries == null || !docSeries.Contains(document.DocumentSeries)) ? docSeries.First() : null;
                                        document.DefaultDocumentDiscount = document.StoreDocumentSeriesType.DefaultDiscountPercentage * 100;
                                    }
                                    else if (Request["field_name"] == "DocumentSeries")
                                    {
                                        document.DocumentSeries = session.GetObjectByKey<DocumentSeries>(objectOid);
                                        document.DefaultDocumentDiscount = document.StoreDocumentSeriesType.DefaultDiscountPercentage * 100;
                                    }
                                    StoreDocumentSeriesType storedoctype = session.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And
                                                                                                                                (new BinaryOperator("DocumentType", document.DocumentType),
                                                                                                                                (new BinaryOperator("DocumentSeries", document.DocumentSeries))));
                                    session.Delete(document.DocumentDetails);
                                    document.TotalDiscountAmount = 0;
                                    document.NetTotalBeforeDiscount = 0;
                                    document.NetTotal = 0;
                                    document.TotalPoints = 0;
                                    document.TotalQty = 0;
                                    document.TotalVatAmount = 0;
                                    document.GrossTotal = 0;
                                    document.TotalVatAmountBeforeDiscount = 0;
                                    document.Customer = document.Division == eDivision.Sales ? (storedoctype != null ? storedoctype.DefaultCustomer : null) : null;
                                    document.Supplier = document.Division == eDivision.Purchase ? (storedoctype != null ? storedoctype.DefaultSupplier : null) : null;
                                }

                                if (document.Division == eDivision.Sales)
                                {
                                    document.BillingAddress = document.Customer != null ? document.Customer.DefaultAddress : null;
                                    document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                                }
                                else if (document.Division == eDivision.Purchase)
                                {
                                    document.BillingAddress = document.Supplier != null ? document.Supplier.DefaultAddress : null;
                                }
                            }

                            if (document.BillingAddress != null)
                            {
                                document.AddressProfession = document.BillingAddress.Profession;
                                document.DeliveryAddress = document.BillingAddress.Description;
                            }
                            else
                            {
                                document.DeliveryAddress = document.AddressProfession = "";
                            }
                        }

                        //Checks if details should be recomputed
                        if (document.DocumentDetails != null && document.DocumentDetails.Count > 0)
                        {
                            List<string> RecomputeDetails = new List<string>() { "PriceCatalogPolicyCb", "storeCustomers" };
                            bool recomputePrices = RecomputeDetails.Contains(Request["field_name"]) && document.TransformationLevel == eTransformationLevel.DEFAULT;
                            DocumentHelper.RecalculateDocumentCosts(ref document, recomputePrices);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
            }

            CriteriaOperator storeDocTypeSerCriteria = UserHelper.IsSystemAdmin(CurrentUser) ?
                                                       StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(document.Store, null, document.IsCancelingAnotherDocument) :
                                                       StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(document.Store,
                                                                                            DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance),
                                                                                            document.IsCancelingAnotherDocument);

            IEnumerable<StoreDocumentSeriesType> storeDocumentSeriesTypes = GetList<StoreDocumentSeriesType>(document.Session, storeDocTypeSerCriteria);


            docSeries = storeDocumentSeriesTypes.Where(sdst => sdst.DocumentType == document.DocumentType).Select(sdst => sdst.DocumentSeries);
            if (docSeries.Count() == 1)
            {
                document.DocumentSeries = docSeries.First();
            }

            ViewBag.Title = document.Session.IsNewObject(document) ? Resources.NewDocument : Resources.EditDocument;
            ViewBag.DocumentSeriesComboBox = docSeries;
            ViewBag.DocumentTypesComboBox = StoreHelper.StoreDocumentTypes(document.Store, document.Division, DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance));
            ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(document.Session);
            SetDocumentTraderViewBags(document);
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, document.DocumentType);
            return PartialView("Advanced", document);
        }

        public JsonResult UpdateItem()
        {
            DocumentHeader document = Session["currentDocument"] as DocumentHeader;
            DocumentDetail documentDetail = Session["currentDocumentDetail"] as DocumentDetail;
            bool newitem = false;
            if (documentDetail != null)
            {
                DocumentHelper.UpdateLinkedItems(ref document, documentDetail);
                DocumentHelper.RecalculateDocumentCosts(ref document, false);
                SaveObjectTemp(document, CurrentUser);
                Session["Notice"] = Resources.SavedSuccesfully;
            }
            if (Boolean.TryParse(Request["check_box_state"], out newitem))
            {
                if (newitem)
                {
                    CleanDocument();
                }
                else
                {
                    Session["DocumentDetail2Edit"] = Session["currentDocumentDetail"];
                }
            }
            return Json(new { oid = documentDetail.Oid });
        }

        public ActionResult CustomerInfoPanel()
        {
            ViewBag.TransferPurposes = GetList<TransferPurpose>(XpoSession);
            return PartialView();
        }

        public ActionResult CustomerInfo()
        {
            return PartialView();
        }

        public ActionResult SupplierInfoPanel()
        {
            ViewBag.TransferPurposes = GetList<TransferPurpose>(XpoSession);
            return PartialView();
        }

        public ActionResult DeliveryDetails()
        {
            return PartialView();
        }

        public ActionResult SearchPriceCatalogCombobox()
        {
            return PartialView();
        }

        public ActionResult VATAnalysis()
        {
            using (MiniProfiler.Current.Step("VATAnalysis_Step1"))
            {
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;

                if (Request["DXCallbackArgument"].ToUpper().Contains("RECALCULATE"))
                {
                    using (MiniProfiler.Current.Step("VATAnalysis_Step1_1"))
                    {
                        JObject jVatInputs = JsonConvert.DeserializeObject<JObject>(Request["vatInputs"]);
                        JObject jNetTotalInputs = JsonConvert.DeserializeObject<JObject>(Request["netTotalInputs"]);
                        IEnumerable<Guid> vatFactorGuids = documentHeader.DocumentDetails.Select(documentDetail => documentDetail.VatFactorGuid).Distinct();
                        foreach (Guid vatFactorGuid in vatFactorGuids)
                        {
                            decimal netTotalUserValue, vatFactorUserValue;
                            JProperty jVatFactorProperty = jVatInputs.Property(vatFactorGuid.ToString());
                            JProperty jNetTotalProperty = jNetTotalInputs.Property(vatFactorGuid.ToString());
                            if (jVatFactorProperty != null && decimal.TryParse(jVatFactorProperty.Value.ToString(), out vatFactorUserValue)
                                && jNetTotalProperty != null && decimal.TryParse(jNetTotalProperty.Value.ToString(), out netTotalUserValue))
                            {
                                DocumentHelper.FixDocumentVatDeviations(ref documentHeader, documentHeader.Session.GetObjectByKey<VatFactor>(vatFactorGuid), netTotalUserValue, vatFactorUserValue);
                            }
                        }
                    }
                }
                using (MiniProfiler.Current.Step("VATAnalysis_GetDocumentVatAnalysis"))
                {
                    ViewBag.VatAnalysis = DocumentHelper.GetDocumentVatAnalysis(documentHeader);
                }
                ViewBag.CurrentOwner = CurrentOwner;
                return PartialView();
            }
        }

        public JsonResult SetDeliveryAddress(string address, string profession)
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            documentHeader.DeliveryAddress = address;
            documentHeader.DeliveryProfession = profession;
            return Json(new { address = documentHeader.DeliveryAddress, profession = documentHeader.DeliveryProfession });
        }

        public JsonResult SetTriangularAddress(string address, string profession)
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            documentHeader.TriangularAddress = address;
            documentHeader.TriangularProfession = profession;
            return Json(new { address = documentHeader.TriangularAddress, profession = documentHeader.TriangularProfession });
        }

        public ActionResult BillingAddressCombobox()
        {
            return PartialView();
        }

        public static object GetPriceCatalogByValue(object value)
        {
            return GetObjectByValue<PriceCatalog>(value);
        }

        public JsonResult BillingAddressValueChanged(string address)
        {
            Guid billingAddressGuid;

            if (Guid.TryParse(address, out billingAddressGuid))
            {
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                documentHeader.BillingAddress = documentHeader.Session.GetObjectByKey<Address>(billingAddressGuid);
                bool recalculateCosts = true;
                if (documentHeader.ReferencedDocuments.Count > 0 && documentHeader.TransformationLevel != eTransformationLevel.DEFAULT)
                {
                    recalculateCosts = false;

                }
                DocumentHelper.RecalculateDocumentCosts(ref documentHeader, recalculateCosts);
            }
            else
            {
                Session["Error"] = Resources.InvalidAddress;
            }

            return Json(new { });
        }

        #region Payments
        public ActionResult DocumentPaymentMethodForm()
        {
            DocumentPayment documentPayment = Session["currentDocumentPayment"] as DocumentPayment;
            Dictionary<PropertyInfo, string> formFields = new Dictionary<PropertyInfo, string>();

            foreach (PropertyInfo requiredPaymentMethodProperty in PaymentFieldHelper.RequiredDocumentPaymentFields())
            {
                formFields.Add(requiredPaymentMethodProperty, requiredPaymentMethodProperty.Name);
                if (requiredPaymentMethodProperty.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                {
                    ViewData[requiredPaymentMethodProperty.PropertyType.Name] = new XPCollection(((DocumentHeader)Session["currentDocument"]).Session, requiredPaymentMethodProperty.PropertyType);
                }
            }

            if (documentPayment != null && documentPayment.PaymentMethod != null)
            {
                IEnumerable<PropertyInfo> documentPaymentProperties = typeof(DocumentPayment).GetProperties();
                foreach (PaymentMethodField optionalPaymentMethodField in documentPayment.PaymentMethod.PaymentMethodFields)
                {
                    formFields.Add(documentPaymentProperties.Where(propertyInfo => propertyInfo.Name.Equals(optionalPaymentMethodField.FieldName)).First(), optionalPaymentMethodField.Label);
                    if (optionalPaymentMethodField.CustomEnumeration != null)
                    {
                        ViewData[optionalPaymentMethodField.FieldName] = optionalPaymentMethodField.CustomEnumeration.CustomEnumerationValues.OrderBy(value => value.Ordering);
                    }
                    else
                    {
                        ViewData[optionalPaymentMethodField.FieldName] = documentPayment.GetMemberValue(optionalPaymentMethodField.FieldName);
                    }
                }
            }

            return PartialView(formFields);
        }


        [Security(ReturnsPartial = false)]
        public ActionResult Payments(Guid? currentDocumentHeaderOid)
        {
            string sessionDocument = "currentDocument";
            DocumentHeader documentHeader = Session[sessionDocument] as DocumentHeader;
            UnitOfWork uow = documentHeader == null ? XpoHelper.GetNewUnitOfWork() : (documentHeader.Session as UnitOfWork);

            if (documentHeader == null && currentDocumentHeaderOid != null && currentDocumentHeaderOid != Guid.Empty)
            {
                documentHeader = uow.GetObjectByKey<DocumentHeader>(currentDocumentHeaderOid);
            }

            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("ADDNEW"))
            {
                Session["currentDocumentPayment"] = new DocumentPayment(uow);
                Session["currentDocumentPaymentIsNew"] = true;
            }
            else if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                int guidLength = Guid.Empty.ToString().Length;
                String docPaymentOid = Request["DXCallbackArgument"].ToString().Substring(Request["DXCallbackArgument"].ToString().Length - (guidLength + 1), guidLength);
                Guid docPaymentGuid;
                if (Guid.TryParse(docPaymentOid, out docPaymentGuid))
                {
                    Session["currentDocumentPayment"] = documentHeader.DocumentPayments.Where(docPaiment => docPaiment.Oid == docPaymentGuid).FirstOrDefault();
                    Session["currentDocumentPaymentIsNew"] = false;
                }
                else
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                    return null;
                }
            }
            else
            {
                Session["currentDocumentPayment"] = null;
                Session["currentDocumentPaymentIsNew"] = null;
            }

            XPCollection<DocumentPayment> documentPayments = null;
            documentPayments = documentHeader.DocumentPayments;
            if (Session[sessionDocument] == null)
            {
                ViewData["currentDocument"] = documentHeader;
            }
            return PartialView("Payments", documentPayments);
        }

        public JsonResult jsonPaymentMethodChanged(string paymentMethodOid)
        {
            Guid paymentMethodGuid;
            if (Guid.TryParse(paymentMethodOid, out paymentMethodGuid))
            {
                (Session["currentDocumentPayment"] as DocumentPayment).PaymentMethod = (Session["currentDocument"] as DocumentHeader).Session.GetObjectByKey<PaymentMethod>(paymentMethodGuid);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult DocumentPaymentComboBox()
        {
            ViewBag.PaymentMethods = GetList<PaymentMethod>(((DocumentHeader)Session["currentDocument"]).Session);
            return PartialView();
        }




        [HttpPost]
        public ActionResult UpdatePayment([ModelBinder(typeof(RetailModelBinder))] DocumentPayment ct)
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            try
            {
                if (ct.Amount <= 0)
                {
                    ModelState.AddModelError("Amount", Resources.InvalidValue);
                }
                if (documentHeader.DocumentType.MaxPaymentAmount > 0 && ct.Amount > documentHeader.DocumentType.MaxPaymentAmount)
                {
                    ModelState.AddModelError("Amount", Resources.MaxPaymentAmount + ": " + documentHeader.DocumentType.MaxPaymentAmount.ToString());
                }

                if (ModelState.IsValid)
                {
                    DocumentPayment documentPayment = documentHeader.DocumentPayments.FirstOrDefault(docPayment => docPayment.Oid == ct.Oid) ?? new DocumentPayment(documentHeader.Session);

                    documentPayment.GetData(ct, new List<string>() { "Session" });
                    Guid paymentMethodGuid = Guid.Empty;

                    if (Guid.TryParse(Request["PaymentMethod!Key_VI"], out paymentMethodGuid))
                    {
                        documentPayment.PaymentMethod = documentHeader.Session.GetObjectByKey<PaymentMethod>(paymentMethodGuid);
                    }

                    Guid customEnumerationValueGuid;
                    if (Guid.TryParse(Request["CustomEnumerationValue1!Key_VI"], out customEnumerationValueGuid))
                    {
                        documentPayment.CustomEnumerationValue1 = documentPayment.Session.GetObjectByKey<CustomEnumerationValue>(customEnumerationValueGuid);
                    }
                    if (Guid.TryParse(Request["CustomEnumerationValue2!Key_VI"], out customEnumerationValueGuid))
                    {
                        documentPayment.CustomEnumerationValue2 = documentPayment.Session.GetObjectByKey<CustomEnumerationValue>(customEnumerationValueGuid);
                    }
                    if (Guid.TryParse(Request["CustomEnumerationValue3!Key_VI"], out customEnumerationValueGuid))
                    {
                        documentPayment.CustomEnumerationValue3 = documentPayment.Session.GetObjectByKey<CustomEnumerationValue>(customEnumerationValueGuid);
                    }
                    if (Guid.TryParse(Request["CustomEnumerationValue4!Key_VI"], out customEnumerationValueGuid))
                    {
                        documentPayment.CustomEnumerationValue4 = documentPayment.Session.GetObjectByKey<CustomEnumerationValue>(customEnumerationValueGuid);
                    }
                    if (Guid.TryParse(Request["CustomEnumerationValue5!Key_VI"], out customEnumerationValueGuid))
                    {
                        documentPayment.CustomEnumerationValue5 = documentPayment.Session.GetObjectByKey<CustomEnumerationValue>(customEnumerationValueGuid);
                    }
                    documentPayment.DocumentHeader = documentHeader;
                    documentPayment.Save();

                    if (documentHeader.Division == eDivision.Financial)
                    {
                        DocumentHelper.SetFinancialDocumentDetail(documentHeader);
                    }
                }
                else
                {
                    Session["Error"] = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                    ViewBag.CurrentItem = ct;
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.GetFullMessage();
                ViewBag.CurrentItem = ct;
            }
            return PartialView("Payments", documentHeader.DocumentPayments);
        }

        [HttpPost]
        public ActionResult DeletePayment([ModelBinder(typeof(RetailModelBinder))] DocumentPayment ct)
        {
            DocumentHeader documentHeader = (Session["currentDocument"] as DocumentHeader);
            if (documentHeader.DocumentPayments != null)
            {
                try
                {
                    DocumentPayment documentPayment = documentHeader.DocumentPayments.Where(docPayment => docPayment.Oid == ct.Oid).First();
                    documentHeader.DocumentPayments.Remove(documentPayment);
                }
                catch
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
            }

            if (documentHeader.Division == eDivision.Financial)
            {
                DocumentHelper.SetFinancialDocumentDetail(documentHeader);
            }

            XPCollection<DocumentPayment> documentPayments = documentHeader == null ? null : documentHeader.DocumentPayments;
            return PartialView("Payments", documentPayments);
        }
        #endregion

        #endregion //DocumentForm

        public JsonResult TransformDocument()
        {
            if (this.ToolbarOptions != null)
            {
                this.ToolbarOptions.ForceVisible = false;
            }
            ClearDocumentSession();

            try
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                Guid docTypeGuid;
                Guid docSeriesGuid;
                DocumentType transformToDocumentType = null;
                DocumentSeries transformToDocumentSeries = null;
                string ids = Request["DC"] != null ? Request["DC"] : Request["DCs"];
                string[] oids = ids.ToString().Split(',');
                List<Guid> documentGuids = GetDocumentGuids(oids);
                XPCollection<DocumentHeader> documents = GetList<DocumentHeader>(uow, new InOperator("Oid", documentGuids));
                List<DocumentDetail> linkedlines = DocumentHelper.GetLinkedLines(documents);
                List<DocumentDetailAssociation> transformationDetails = (List<DocumentDetailAssociation>)Session["transformationDetails"];

                if (Request["TransformationRuleDerrivedType"] == "undefined" && Request["DocumentSeries"] == "undefined")
                {
                    //Proforma Transformation
                    PrepareTransformationDocumentDetails(oids);
                    transformationDetails = (List<DocumentDetailAssociation>)Session["transformationDetails"];
                    List<TransformationRule> transfromationRules = (List<TransformationRule>)ViewData["allowedTransformationRulesTypes"];
                    TransformationRule activeRule = transfromationRules.FirstOrDefault(x => x.IsDefault) ?? transfromationRules.FirstOrDefault();


                    if (activeRule == null)
                    {
                        return Json(new { result = "" });
                    }
                    transformToDocumentType = uow.GetObjectByKey<DocumentType>(activeRule.DerrivedType.Oid);

                    eModule module = DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance);

                    Store store = uow.GetObjectByKey<Store>(transformationDetails.First().documentDetail.DocumentHeader.Store.Oid);
                    IEnumerable<DocumentSeries> series = StoreHelper.StoreSeriesForDocumentType(store, transformToDocumentType, module);
                    if (series.Count() != 1)
                    {
                        return Json(new { result = "" });
                    }
                    transformToDocumentSeries = uow.GetObjectByKey<DocumentSeries>(series.First().Oid);
                }
                else if (Guid.TryParse(Request["TransformationRuleDerrivedType"], out docTypeGuid) && Guid.TryParse(Request["DocumentSeries"], out docSeriesGuid))
                {
                    transformToDocumentType = uow.GetObjectByKey<DocumentType>(docTypeGuid);
                    transformToDocumentSeries = uow.GetObjectByKey<DocumentSeries>(docSeriesGuid);
                    if (transformToDocumentType == null || transformToDocumentSeries == null)
                    {
                        return Json(new { result = "" });
                    }
                }
                else
                {
                    return Json(new { result = "" });
                }


                DocumentHeader transformedDocumentHeader = DocumentHelper.CreateDerivativeDocument(transformationDetails, CurrentUser, transformToDocumentType, transformToDocumentSeries, linkedlines);
                Session["transformationDetails"] = null;

                if (transformedDocumentHeader != null)
                {
                    Session["IsNewDocument"] = true;
                    Session["currentDocumentToLoad"] = transformedDocumentHeader;
                    if (transformedDocumentHeader.Customer == null)
                    {
                        Session["Error"] = Resources.PleaseSelectACustomer;
                    }
                    return Json(new { LoadFromSession = true });
                }
                Session["Error"] = Resources.DocumentAlreadyTransformed;
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
            }

            return Json(new { result = "" });
        }

        public JsonResult jsonCancelDocument(string DocumentGuid)
        {
            Guid documentGuid;
            string reason = string.Empty;
            if (!Guid.TryParse(DocumentGuid, out documentGuid))
            {
                reason = Resources.AnErrorOccurred;
            }
            else
            {
                if (DocumentHelper.DocumentCanBeCanceled(documentGuid, CurrentUser, out reason))
                {
                    Guid currentUserOid = CurrentUser.Oid;
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        DocumentHeader currentDocumentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                        Guid cancelingDocument = DocumentHelper.CancelDocument(documentGuid, currentUserOid, 0);
                        return Json(new { success = Resources.DocumentIsCancelled, CancelingDocument = cancelingDocument });
                    }
                }
            }

            return Json(new { error = reason });
        }

        public ActionResult ReferencedDocuments()
        {
            return PartialView();
        }

        public ActionResult DerivedDocuments()
        {
            return PartialView();
        }

        public ActionResult OrderTabsCallbackPanel(Guid DocumentHeaderOid)
        {
            ViewBag.InViewMode = Session["currentDocument"] == null;
            DocumentHeader documentHeader = null;
            if (ViewBag.InViewMode)
            {
                documentHeader = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(DocumentHeaderOid);
                ViewData["currentDocument"] = documentHeader;
                ViewBag.TransferPurposes = GetList<TransferPurpose>(documentHeader.Session as UnitOfWork);
            }
            else
            {
                documentHeader = Session["currentDocument"] as DocumentHeader;
                ViewBag.TransferPurposes = GetList<TransferPurpose>(documentHeader.Session as UnitOfWork);
            }

            SetDocumentTraderViewBags(documentHeader);
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, documentHeader.DocumentType);
            return PartialView();
        }


        public ActionResult MarkUp()
        {
            Session["DOids"] = Request["DOids"];
            if (Request["DOids"] != null)
            {
                CollectValueChanges(false, EffectiveOwner.OwnerApplicationSettings.MarkupDefaultValueDifference);
            }

            if (Session["Error"] != null)
            {
                //return View("../Home/;CloseWindow");
            }

            return PartialView();

        }

        public ActionResult MarkUpGrid()
        {
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("STARTEDIT") >= 0)
            {
                return PartialView();
            }

            bool AllValues = false;
            Boolean.TryParse(Request["AllValues"], out AllValues);
            decimal sensitivity = .0m;
            decimal.TryParse(Request["sensitivity"], out sensitivity);
            CollectValueChanges(AllValues, sensitivity / DocumentHelper.QUANTITY_MULTIPLIER);

            return PartialView();
        }

        private void CollectValueChanges(bool includeUnchangedValues = false, decimal sensitivity = .0m)
        {
            Session["valueChanges"] = null;
            string DOids = Session["DOids"] == null ? "" : Session["DOids"].ToString();
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            if (String.IsNullOrEmpty(DOids))
            {
                Session["Error"] = Resources.InvalidDocument;
                return;
            }

            Session["Error"] = null;

            List<Reevaluate> valueChanges = new List<Reevaluate>();

            DOids.Split(',').ToList().ForEach(delegate (string documentOid)
            {
                if (Session["Error"] != null)
                {
                    return;
                }

                Guid documentGuid;
                DocumentHeader documentHeader = null;
                if (Guid.TryParse(documentOid, out documentGuid))
                {
                    documentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                }

                if (documentHeader == null)
                {
                    Session["Error"] = Resources.InvalidDocument;
                }

                List<Reevaluate> currentValueChanges = PriceCatalogHelper.GetPriceChanges(documentHeader, sensitivity, includeUnchangedValues);
                if (currentValueChanges.Count > 0)
                {
                    valueChanges.AddRange(currentValueChanges);
                }
            });

            Session["valueChanges"] = valueChanges;
            ViewBag.Sensitivity = sensitivity;
        }

        public JsonResult SetUnitPrice(string inputID, string unitPriceValue)
        {
            Guid reevaluateObjectGuid = GetReevaluateObjectGuid(inputID);
            if (reevaluateObjectGuid == Guid.Empty)
            {
                return Json(new { success = false });
            }


            if ((Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault() == null)
            {
                return Json(new { success = false });
            }

            decimal unitPrice = (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault().UnitPrice;
            if (decimal.TryParse(unitPriceValue, out unitPrice))
            {
                unitPrice /= DocumentHelper.QUANTITY_MULTIPLIER * DocumentHelper.QUANTITY_MULTIPLIER;
            }
            (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == (reevaluateObjectGuid)).FirstOrDefault().UnitPrice = unitPrice;
            Reevaluate reevaluate = (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == (reevaluateObjectGuid)).FirstOrDefault();
            int index = (Session["valueChanges"] as List<Reevaluate>).IndexOf(reevaluate) + 1;
            string focus_on = "btnUpdateMarkUp";

            if (index < (Session["valueChanges"] as List<Reevaluate>).Count())
            {
                string nextrLineOid = ((Session["valueChanges"] as List<Reevaluate>)[index]).Oid.ToString();
                focus_on = "markup" + nextrLineOid;
            }

            return Json(new { success = true, markup = reevaluate.MarkUp, unit_price = reevaluate.UnitPrice, selected = reevaluate.Selected, focus_on = focus_on });
        }

        public JsonResult SetMarkUp(string inputID, string markUpValue)
        {
            Guid reevaluateObjectGuid = GetReevaluateObjectGuid(inputID);
            if (reevaluateObjectGuid == Guid.Empty)
            {
                return Json(new { success = false });
            }

            if ((Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault() == null)
            {
                return Json(new { success = false });
            }

            decimal markUp = (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault().MarkUp;
            if (decimal.TryParse(markUpValue, out markUp))
            {
                markUp /= DocumentHelper.QUANTITY_MULTIPLIER * 100;
            }
            (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault().MarkUp = markUp;
            Reevaluate reevaluate = (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault();
            return Json(new { success = true, markup = reevaluate.MarkUp, unit_price = reevaluate.UnitPrice, selected = reevaluate.Selected });
        }

        public JsonResult SetSelected(string inputID, string selected)
        {
            Guid reevaluateObjectGuid = GetReevaluateObjectGuid(inputID);
            bool hasBeenSelected = false;
            if (reevaluateObjectGuid == Guid.Empty || !Boolean.TryParse(selected, out hasBeenSelected)
                || (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault() == null
                )
            {
                return Json(new { success = false });
            }

            (Session["valueChanges"] as List<Reevaluate>).Where(revaluate => revaluate.Oid == reevaluateObjectGuid).FirstOrDefault().Selected = hasBeenSelected;

            return Json(new { success = true });
        }

        private Guid GetReevaluateObjectGuid(string inputID)
        {
            Guid ReevaluateObjectGuid = Guid.Empty;

            Guid.TryParse(inputID.Replace("markup", "").Replace("unit_price", "").Replace("selected", ""), out ReevaluateObjectGuid);

            return ReevaluateObjectGuid;
        }

        public JsonResult SaveMarkUps()
        {
            List<Reevaluate> reavaluations = (Session["valueChanges"] as List<Reevaluate>);
            try
            {
                bool saveMarkUps = false;
                Boolean.TryParse(Request["saveMarkUps"], out saveMarkUps);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHelper.SaveMarkUps(reavaluations, uow, saveMarkUps);
                }
                Session["Notice"] = Resources.SavedSuccesfully;
                return Json(new { success = true });
            }
            catch
            {
                Session["Notice"] = Resources.AnErrorOccurred;
                return Json(new { success = false });
            }
        }

        public ActionResult EditGrid(bool? InViewMode, Guid DocumentOid)
        {
            ViewData["InViewMode"] = InViewMode;
            ViewBag.nextOid = String.IsNullOrEmpty(Request["nextOid"]) ? null : Request["nextOid"];
            ViewBag.previousOid = String.IsNullOrEmpty(Request["previousOid"]) ? null : Request["previousOid"];
            bool checkstate = false;
            Boolean.TryParse(Request["check_box_state"], out checkstate);
            ViewBag.checkbox = checkstate;

            DocumentHeader document = null;
            if (InViewMode.HasValue && InViewMode.Value == true)
            {
                document = XpoSession.GetObjectByKey<DocumentHeader>(DocumentOid);
                ViewData["currentDocument"] = document;
                return PartialView(document.DocumentDetails);
            }

            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["OldDocumentDetail"] = null;
                    string errormsg;
                    document = (DocumentHeader)(Session["currentDocument"]);
                    if (DocumentHelper.MaxCountOfLinesExceeded(document, out errormsg))
                    {
                        Session["Error"] = errormsg;
                        return null;
                    }
                    else
                    {
                        ViewBag.CurrentItem = new DocumentDetail(((DocumentHeader)(Session["currentDocument"])).Session);
                        ViewBag.DocumentDetailIsNew = true;
                        ViewBag.InViewMode = false;
                        ViewBag.DocumentDetailFormMode = "StartAdd";
                    }
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    string oid = Request["DXCallbackArgument"].Split('|').Last().Split(';').First();
                    Guid guid = Guid.Empty;
                    if (Guid.TryParse(oid, out guid))
                    {
                        document = (DocumentHeader)(Session["currentDocument"]);
                        DocumentDetail documentDetail = document.DocumentDetails.Where(docDetail => docDetail.Oid == guid).FirstOrDefault();
                        Session["DocumentDetail2Edit"] = Session["currentDocumentDetail"] = documentDetail;
                        Session["OldDocumentDetail"] = documentDetail.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                        if (document.Division == eDivision.Sales)
                        {
                            Session["currentPriceCatalogDetail"] = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(document.Session as UnitOfWork,
                                                                                                                      document.EffectivePriceCatalogPolicy,
                                                                                                                      documentDetail.Item,
                                                                                                                           documentDetail.Barcode);
                        }
                    }
                    ViewBag.DocumentDetailFormMode = "Edit";
                    ViewBag.DocumentDetailIsNew = false;
                    ViewBag.InViewMode = false;
                }
                else if (Request["DXCallbackArgument"].Contains("RECALCULATE"))
                {
                    ViewData["InViewMode"] = false;
                    DocumentHeader docHeader = (DocumentHeader)(Session["currentDocument"]);
                    DocumentHelper.RecalculateDocumentCosts(ref docHeader, true);
                }
                else if (!Request["DXCallbackArgument"].Contains("PAGERONCLICK") &&
                         !Request["DXCallbackArgument"].Contains("COLUMNMOVE") &&
                         !Request["DXCallbackArgument"].Contains("SORT") &&
                         !Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER") &&
                         !Request["DXCallbackArgument"].Contains("APPLYFILTER"))
                {
                    Session["OldDocumentDetail"] = null;
                    User user = CurrentUser;
                    document = (DocumentHeader)Session["currentDocument"];

                    if (document != null)
                    {
                        Store currentStore = document.Store;

                        if (document.Customer != null
                            && document.Customer.BreakOrderToCentral
                            && UserHelper.IsCompanyUser(user)
                            && user.IsCentralStore == true
                            && currentStore.IsCentralStore)
                        {
                            document.DocumentDetails.Filter = new BinaryOperator("CentralStore.Oid", currentStore.Oid);
                        }
                    }
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT") || Request["DXCallbackArgument"].Contains("UPDATEEDIT"))
                {
                    CleanDocument();
                }
                else
                {
                    Session["OldDocumentDetail"] = null;
                    document = Session["currentDocument"] as DocumentHeader;
                }
            }


            ViewData["currentDocument"] = Session["currentDocument"];
            ViewBag.VatFactors = GetList<VatFactor>(document.Session as UnitOfWork);
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            ViewBag.NotIncludedDetails = Session["notIncludedDetailsOids"];
            return PartialView(document.DocumentDetails);
        }

        protected void InitializeMultipleOrders()
        {
            Session["selected_items_qty"] = DocumentHelper.SelectedItemsTotalQuantity(((DocumentHeader)Session["currentDocument"]));
            Session["TreeItems"] = Session["currentOffer"] = Session["ItemOfferDetails"] = Session["selected_offers_qty"] = null;
        }

        protected void InitializeByOfferOrders()
        {
            Session["selected_offers_qty"] = DocumentHelper.SelectedItemsTotalQuantity(((DocumentHeader)Session["currentDocument"]));
            Session["ItemOfferDetails"] = Session["selected_items_qty"] = null;
        }

        protected void InitializeNewItems()
        {
            Session["selected_fresh_items_qty"] = DocumentHelper.SelectedItemsTotalQuantity(((DocumentHeader)Session["currentDocument"]));
            Session["freshItemsPriceCatalogDetails"] = Session["newItems"] = null;
        }

        public ActionResult DeleteDocumentDetail()
        {
            DocumentHeader currentDocumentHeader = ((DocumentHeader)Session["currentDocument"]);
            foreach (DocumentDetail DocDetail in currentDocumentHeader.DocumentDetails)
            {
                if (DocDetail.Oid.ToString().Equals(Request["Oid"].ToString()))
                {
                    DocumentHelper.DeleteItem(ref currentDocumentHeader, DocDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false, false);
                    break;
                }
            }
            DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
            SaveObjectTemp(currentDocumentHeader, CurrentUser);
            ViewData["currentDocument"] = Session["currentDocument"];
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView("EditGrid", currentDocumentHeader.DocumentDetails);
        }

        private void UpdateDocumentHeaderValues(Dictionary<string, string> requestedValues)
        {
            String docType, docSeries, docNumber, tablet, docFinDate, docStatus, docChecked, docVehicleNumber, docExecuted, docInvoiceDate, docRemarks, docDelAddr, docTransferMethod, docPlaceOfLoading, docTransferPurposeStr, docTriangularCustomerStr, docTriangularSupplierStr, docTriangularStoreStr;//, docChargedToUserStr;
            TransferPurpose docTransferPurpose = null;

            requestedValues.TryGetValue("docType", out docType);
            requestedValues.TryGetValue("docSeries", out docSeries);
            requestedValues.TryGetValue("docNumber", out docNumber);
            requestedValues.TryGetValue("docFinDate", out docFinDate);
            requestedValues.TryGetValue("docStatus", out docStatus);
            requestedValues.TryGetValue("docChecked", out docChecked);
            requestedValues.TryGetValue("docExecuted", out docExecuted);
            requestedValues.TryGetValue("docInvoiceDate", out docInvoiceDate);
            requestedValues.TryGetValue("docRemarks", out docRemarks);
            requestedValues.TryGetValue("docDelAddr", out docDelAddr);
            requestedValues.TryGetValue("docTransferMethod", out docTransferMethod);
            requestedValues.TryGetValue("docPlaceOfLoading", out docPlaceOfLoading);
            requestedValues.TryGetValue("docVehicleNumber", out docVehicleNumber);
            requestedValues.TryGetValue("docTransferPurpose", out docTransferPurposeStr);
            requestedValues.TryGetValue("docTriangularCustomer", out docTriangularCustomerStr);
            requestedValues.TryGetValue("docTriangularSupplier", out docTriangularSupplierStr);
            requestedValues.TryGetValue("docTriangularStore", out docTriangularStoreStr);
            requestedValues.TryGetValue("Tablet", out tablet);
            DocumentHeader document = (DocumentHeader)Session["currentDocument"];

            if (document == null)
            {
                return;
            }

            Guid transferPurposeGuid;
            if (Guid.TryParse(docTransferPurposeStr, out transferPurposeGuid))
            {
                docTransferPurpose = document.Session.GetObjectByKey<TransferPurpose>(transferPurposeGuid);
            }

            if (docType != null && docType != "")
            {
                Guid docTypeGuid;
                if (Guid.TryParse(docSeries, out docTypeGuid))
                {
                    DocumentType docTypeObj = document.Session.GetObjectByKey<DocumentType>(docTypeGuid);
                    if (docTypeObj != null)
                    {
                        document.DocumentType = docTypeObj;
                    }
                }
            }

            if (docSeries != null && docSeries != "")
            {
                Guid docSeriesGuid;
                if (Guid.TryParse(docSeries, out docSeriesGuid))
                {
                    DocumentSeries docSeriesObj = document.Session.GetObjectByKey<DocumentSeries>(docSeriesGuid);
                    if (docSeriesObj != null)
                    {
                        document.DocumentSeries = docSeriesObj;
                    }
                }
            }

            if (docNumber != null && docNumber != "")
            {
                int number;
                if (int.TryParse(docNumber, out number))
                {
                    document.DocumentNumber = number;// TO CHECK
                }
            }

            if (docFinDate != null && docFinDate != "")
            {
                DateTime finDate;
                if (DateTime.TryParse(docFinDate, out finDate))
                {
                    document.FinalizedDate = finDate;
                }
            }

            if (docStatus != null && docStatus != "")
            {
                Guid docStatusGuid;
                if (Guid.TryParse(docStatus, out docStatusGuid))
                {
                    DocumentStatus docStatusObj = document.Session.GetObjectByKey<DocumentStatus>(docStatusGuid);
                    if (docStatusObj != null)
                    {
                        document.Status = docStatusObj;
                    }
                }
            }

            if (tablet != null && tablet != "")
            {
                Guid tabletGuid;
                if (Guid.TryParse(tablet, out tabletGuid))
                {
                    SFA tabletObj = document.Session.GetObjectByKey<SFA>(tabletGuid);
                    if (tabletObj != null)
                    {
                        document.Tablet = tabletObj;
                    }
                }
            }

            if (docChecked != null)
            {
                document.HasBeenChecked = (docChecked == "true");//TO CHECK
            }

            if (docExecuted != null)
            {
                document.HasBeenExecuted = (docExecuted == "true");//TO CHECK
            }

            if (document.HasBeenExecuted && docInvoiceDate != null && docInvoiceDate != "")
            {
                DateTime invoiceDate;
                if (DateTime.TryParse(docInvoiceDate, out invoiceDate))
                {
                    document.InvoicingDate = invoiceDate;
                }
            }
            else
            {
                document.InvoicingDate = DateTime.MinValue;
            }

            if (docRemarks != null)
            {
                document.Remarks = docRemarks;
            }

            if (docDelAddr != null)
            {
                document.DeliveryAddress = docDelAddr;
            }

            if (docTransferMethod != null && docTransferMethod != "null")
            {
                document.TransferMethod = docTransferMethod;
            }

            if (docPlaceOfLoading != null && docPlaceOfLoading != "null")
            {
                document.PlaceOfLoading = docPlaceOfLoading;
            }

            if (docVehicleNumber != null && docVehicleNumber != "null")
            {
                document.VehicleNumber = docVehicleNumber;
            }

            if (docTransferPurpose != null)
            {
                document.TransferPurpose = docTransferPurpose;
            }

            Guid triangularCustomerGuid = Guid.Empty;
            if (Guid.TryParse(docTriangularCustomerStr, out triangularCustomerGuid))
            {
                Customer triangularCustomer = document.Session.GetObjectByKey<Customer>(triangularCustomerGuid);
                if (triangularCustomer != null)
                {
                    document.TriangularCustomer = triangularCustomer;
                }
            }

            Guid triangularSupplierGuid = Guid.Empty;
            if (Guid.TryParse(docTriangularSupplierStr, out triangularSupplierGuid))
            {
                SupplierNew triangularSupplier = document.Session.GetObjectByKey<SupplierNew>(triangularSupplierGuid);
                if (triangularSupplier != null)
                {
                    document.TriangularSupplier = triangularSupplier;
                }
            }

            Guid triangularStoreGuid = Guid.Empty;
            if (Guid.TryParse(docTriangularStoreStr, out triangularStoreGuid))
            {
                Store triangularStore = document.Session.GetObjectByKey<Store>(triangularStoreGuid);
                if (triangularStore != null)
                {
                    document.TriangularStore = triangularStore;
                }
            }
        }

        public ActionResult DocumentDetailAdvancedPurchase()
        {
            DocumentHeader document = ((DocumentHeader)Session["currentDocument"]);
            CriteriaOperator crop = new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING, BinaryOperatorType.NotEqual);
            ViewBag.MeasurementUnits = GetList<MeasurementUnit>(document.Session);
            ViewBag.VatFactors = GetList<VatFactor>(document.Session);
            return PartialView(DocumentDetailFormsCommon());
        }

        public ActionResult DocumentDetailAdvancedSales()
        {
            DocumentHeader document = ((DocumentHeader)Session["currentDocument"]);
            CriteriaOperator crop = new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING, BinaryOperatorType.NotEqual);
            ViewBag.MeasurementUnits = GetList<MeasurementUnit>(document.Session);
            return PartialView(DocumentDetailFormsCommon());
        }

        public ActionResult DocumentDetailAdvancedStore()
        {
            DocumentHeader document = ((DocumentHeader)Session["currentDocument"]);
            CriteriaOperator crop = new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING, BinaryOperatorType.NotEqual);
            ViewBag.MeasurementUnits = GetList<MeasurementUnit>(document.Session);
            ViewBag.VatFactors = GetList<VatFactor>(document.Session as UnitOfWork);
            return PartialView(DocumentDetailFormsCommon());
        }

        private DocumentDetail DocumentDetailFormsCommon()
        {
            ViewBag.DocumentDetailIsNew = true;
            ViewBag.DocumentDetailFormMode = String.IsNullOrEmpty(Request["DocumentDetailFormMode"]) ? "Hidden" : Request["DocumentDetailFormMode"].ToString().Replace("Start", "");
            ViewBag.nextOid = String.IsNullOrEmpty(Request["nextOid"]) ? null : Request["nextOid"];
            ViewBag.previousOid = String.IsNullOrEmpty(Request["previousOid"]) ? null : Request["previousOid"];
            DocumentHeader documentHeader = (DocumentHeader)Session["currentDocument"];
            DocumentDetail documentDetail = (DocumentDetail)(Session["currentDocumentDetail"]);

            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, documentHeader.DocumentType);
            ViewBag.InViewMode = documentHeader == null;
            ViewBag.IsOrder = documentHeader == null || documentHeader.DocumentType == null ? false : DocumentHelper.CurrentUserDocumentView(CurrentUser, documentHeader.DocumentType).Equals(eDocumentTypeView.Simple);
            if (ViewBag.InViewMode)
            {//View Mode
                Guid documentDetailOid;
                if (Guid.TryParse(Request["DXCallbackArgument"].ToString().Split(':').Last().Replace("'", ""), out documentDetailOid))
                {
                    //Preview only item in view mode
                    documentDetail = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentDetail>(documentDetailOid);
                    if (documentDetail != null)
                    {
                        documentHeader = documentDetail.DocumentHeader;
                    }
                    if (documentHeader != null)
                    {
                        ViewData["currentDocument"] = documentHeader;
                        ViewData["currentDocumentDetail"] = documentDetail;
                        ViewBag.IsOrder = documentHeader.DocumentType == null ? false : DocumentHelper.CurrentUserDocumentView(CurrentUser, documentHeader.DocumentType).Equals(eDocumentTypeView.Simple);

                        switch (documentHeader.Division)
                        {
                            case eDivision.Sales:

                                ViewData["currentpriceCatalogDetail"] = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                                           documentHeader.EffectivePriceCatalogPolicy,
                                                                                                                           documentDetail.Item,
                                                                                                                           documentDetail.Barcode);
                                break;
                        }
                    }

                    CleanDocument();
                    ViewBag.DocumentDetailFormMode = "StartEdit";
                    ViewBag.DocumentDetailIsNew = false;

                }
            }
            bool checkstate = false;
            Boolean.TryParse(Request["check_box_state"], out checkstate);
            ViewBag.checkbox = checkstate;
            if (Request["updateMode"] == "SAVE" && !checkstate)
            {
                ViewBag.DocumentDetailFormMode = "Edit";
                ViewBag.DocumentDetailIsNew = false;
                ViewBag.InViewMode = false;
            }
            else if ((Request["updateMode"] == "UPDATE" && checkstate))
            {
                CleanDocument();
                ViewBag.DocumentDetailFormMode = "StartAdd";
                ViewBag.DocumentDetailIsNew = true;
                ViewBag.InViewMode = false;
                ViewBag.nextOid = null;
                ViewBag.previousOid = null;
            }

            if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].ToUpper().Contains("CLEAN") && documentDetail != null && Session["OldDocumentDetail"] != null)
            {
                string error;
                bool conversionSucceeded = documentDetail.FromJson(Session["OldDocumentDetail"] as string, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                Session["OldDocumentDetail"] = null;
            }
            return documentDetail;
        }

        public ActionResult SearchByBarcode()
        {
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView();
        }

        public ActionResult SearchByDescription()
        {
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView();
        }

        public JsonResult jsonInvoicingDateValueChanged()
        {
            try
            {
                DocumentHeader documentHeader = ((DocumentHeader)Session["currentDocument"]);
                documentHeader.InvoicingDate = DocumentHelper.ConvertToDateTime(Request["InvoicingDate"]);
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }

            return Json(new
            {
            });
        }

        public JsonResult jsonIsSaved()
        {
            bool IsSaved = false;
            try
            {
                DocumentDetail documentdetail = (DocumentDetail)Session["currentDocumentDetail"];
                DocumentDetail oldDocumentDetail = (DocumentDetail)Session["DocumentDetail2Edit"];
                if (documentdetail.Equals(oldDocumentDetail) || documentdetail.Oid == oldDocumentDetail.Oid)
                {
                    IsSaved = true;
                }
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }

            return Json(new { saved = IsSaved });
        }

        public static object ItemRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                Item obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Item>(e.Value);
                return obj;
            }
            return null;
        }

        public static object SupplierItemAllRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
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
            IEnumerable<SupplierNew> searched_item_supliers = GetList<SupplierNew>(XpoHelper.GetNewUnitOfWork(), CriteriaOperator.And(CriteriaOperator.Or(
                new BinaryOperator("CompanyName", proccessed_filter, BinaryOperatorType.Like),
                new BinaryOperator("Trader.TaxCode", proccessed_filter, BinaryOperatorType.Like)
                ),
                new BinaryOperator("IsActive", true)
                ));
            if (searched_item_supliers.Count() == 0)
            {
                return null;
            }
            return searched_item_supliers;
        }

        public static object SupplierItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                CompanyNew obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>(e.Value);
                return obj;
            }
            return null;
        }

        [AjaxOrChildActionOnly]
        public JsonResult jsonUpdateTabs()
        {
            Session["selected_items_qty"] = null;
            Session["selected_offers_qty"] = null;
            Session["selected_fresh_items_qty"] = null;

            return Json(new { });
        }

        [AjaxOrChildActionOnly]
        public ActionResult TreeView()
        {
            return PartialView();
        }

        private void UpdateList(List<SelectedItemsQty> firstList, List<SelectedItemsQty> listToUpdate)
        {
            foreach (SelectedItemsQty item in firstList)
            {
                foreach (SelectedItemsQty item2 in listToUpdate)
                {
                    if (item.item.Oid == item2.item.Oid)
                    {
                        item2.order_qty = item.order_qty;
                    }
                }
            }
        }

        [AjaxOrChildActionOnly]
        public ActionResult ItemsOfNode()
        {
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("SHOWHIDEFILTERS") >= 0)
            {
                if (Session["showItemCategoryFilter"] == null)
                {
                    Session["showItemCategoryFilter"] = false;
                }
                Session["showItemCategoryFilter"] = !(bool)Session["showItemCategoryFilter"];
                return PartialView();
            }

            bool itemSupplierChanged = (string)Session["itemSupplierId"] != Request["itemSupplierId"];
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("STARTEDIT") >= 0)
            {
                string category_id = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
                Guid category_guid = category_id != "" ? Guid.Parse(category_id) : Guid.Empty;
                ItemCategory cat = ((DocumentHeader)Session["currentDocument"]).Session.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                ViewData["categoryDescription"] = cat.Description;
                return PartialView();
            }
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("STARTUP") >= 0)
            {
                InitializeMultipleOrders();
                ViewData["itemsOfNode"] = false;
            }
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDERITEMS") >= 0)
            {
                string errorMessage = "";
                if (DocumentHelper.MaxCountOfLinesExceeded((DocumentHeader)Session["currentDocument"], out errorMessage))
                {
                    return Json(new { error = errorMessage });
                }
                else
                {
                    errorMessage = OrderMultipleItems();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Session["Error"] = errorMessage;
                        return null;
                    }
                }
            }

            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            if (categoryid != "")
            {
                Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
                ItemCategory cat = ((DocumentHeader)Session["currentDocument"]).Session.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                ViewData["categoryid"] = categoryid;
                IEnumerable<PriceCatalogDetail> details;
                ViewData["categoryDescription"] = cat.Description;
                CriteriaOperator firstFilter;
                Guid itemSupplierGuid = Guid.Empty;
                if (Request["itemSupplierId"] != null && Guid.TryParse(Request["itemSupplierId"].ToString(), out itemSupplierGuid))
                {
                    firstFilter = CriteriaOperator.And(cat.GetAllNodeTreeFilter("Item.ItemAnalyticTrees"), new BinaryOperator("Item.DefaultSupplier.Oid", itemSupplierGuid), new BinaryOperator("Item.IsActive", true));
                    Session["itemSupplierId"] = Request["itemSupplierId"];
                }
                else
                {
                    Session["itemSupplierId"] = null;
                    firstFilter = cat.GetAllNodeTreeFilter("Item.ItemAnalyticTrees");
                }

                DocumentHeader documentHeader = (DocumentHeader)Session["currentDocument"];

                if (Session["ItemsOfNodeSelectedCategoryID"] == null || (Guid)Session["ItemsOfNodeSelectedCategoryID"] != category_guid || itemSupplierChanged)
                {
                    details = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(documentHeader.Store, CriteriaOperator.And(firstFilter, DocumentHelper.DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(documentHeader)));
                    //PriceCatalogHelper.GetTreePriceCatalogDetails(documentHeader.PriceCatalog, CriteriaOperator.And(firstFilter, DocumentHelper.DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(documentHeader)));
                    Session["ItemsOfNodeSelectedCategoryID"] = category_guid;
                    Session["ItemsOfNodeSelectedPricesOfCategory"] = details == null ? new List<PriceCatalogDetail>() : details.ToList();
                }
                else
                {
                    details = Session["ItemsOfNodeSelectedPricesOfCategory"] as List<PriceCatalogDetail>;
                }

                bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;


                List<SelectedItemsQty> selected_items_qty =
                    Session["selected_items_qty"] != null && (Session["selected_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(documentHeader);

                var qtys = GetSelectedItemsQtys(documentHeader, details, selected_items_qty, SupportsPacking);
                var qtysToList = qtys.ToList();
                Session["TreeItems"] = qtysToList;
                Session["selected_items_qty"] = selected_items_qty;
                ViewData["itemsOfNode"] = selected_items_qty.Where(g => g.qty > 0).Count() > 0;

            }
            return PartialView();
        }

        private IEnumerable<object> GetSelectedItemsQtys(DocumentHeader documentHeader, IEnumerable<PriceCatalogDetail> details, List<SelectedItemsQty> selected_items_qty, bool SupportsPacking)
        {
            return from obj in details
                   join itm in selected_items_qty on obj.Item.Oid equals itm.item.Oid into gj
                   from subset in gj.DefaultIfEmpty()
                   select new
                   {
                       item_oid = obj.Item.Oid,
                       item_description = obj.Item.Name,
                       item_code = obj.Item.Code,
                       Value = ItemHelper.GetItemPriceWithoutTax(obj),
                       Qty = (subset == null ? .0m : subset.qty),
                       pack_measurement_unit = GetPackingMeasurementUnit(documentHeader, obj, SupportsPacking),
                       packing_qty = GetPackingQuantiy(documentHeader, subset, obj, SupportsPacking),
                       measurement_unit = GetMeasurementUnit(documentHeader, obj),
                       order_qty = (subset == null ? .0m : subset.order_qty),
                       item_checked = (subset == null ? .0m : subset.qty) > 0,
                       item_support_decimal = obj.Barcode.MeasurementUnit(documentHeader.Owner) == null ? false : obj.Barcode.MeasurementUnit(documentHeader.Owner).SupportDecimal
                   };
        }

        private MeasurementUnit GetPackingMeasurementUnit(DocumentHeader documentHeader, PriceCatalogDetail priceCatalogDetail, bool SupportsPacking)
        {
            ItemBarcode itemBarcode = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, priceCatalogDetail.Item, documentHeader.Owner);
            if (SupportsPacking)
            {
                if (
                    priceCatalogDetail.Item.PackingQty <= 0 || priceCatalogDetail.Item.PackingMeasurementUnit == null || priceCatalogDetail.Item.PackingMeasurementUnit == itemBarcode.MeasurementUnit)
                {
                    if (itemBarcode == null)
                    {
                        return null;
                    }
                    return itemBarcode.MeasurementUnit;
                }
                else
                {
                    return priceCatalogDetail.Item.PackingMeasurementUnit;
                }
            }
            else
            {
                if (itemBarcode == null)
                {
                    return null;
                }
                return itemBarcode.MeasurementUnit;
            }
        }

        private decimal GetPackingQuantiy(DocumentHeader documentHeader, SelectedItemsQty subset, PriceCatalogDetail priceCatalogDetail, bool SupportsPacking)
        {
            if (subset == null)
            {
                return 0;
            }
            else
            {
                if (SupportsPacking)
                {
                    ItemBarcode itemBarcode = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, priceCatalogDetail.Item, documentHeader.Owner);
                    if (priceCatalogDetail.Item.PackingQty <= 0 || priceCatalogDetail.Item.PackingMeasurementUnit == null || priceCatalogDetail.Item.PackingMeasurementUnit == itemBarcode.MeasurementUnit)
                    {
                        return (subset == null ? .0m : subset.qty);
                    }
                    else
                    {
                        return (decimal)priceCatalogDetail.Item.PackingQty * (subset == null ? .0m : subset.qty);
                    }
                }
                else if (subset == null)
                {
                    return .0m;
                }
                else
                {
                    return subset.qty;
                }
            }
        }

        private MeasurementUnit GetMeasurementUnit(DocumentHeader documentHeader, PriceCatalogDetail priceCatalogDetail)
        {
            ItemBarcode itemBarcode = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, priceCatalogDetail.Item, documentHeader.Owner);
            return itemBarcode == null ? null : itemBarcode.MeasurementUnit;
        }

        [AjaxOrChildActionOnly]
        public ActionResult UpdateQtyOfSelectedItemOfNode()
        {
            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            Guid item_oid = Request["item_oid"] == null || Request["item_oid"] == "null" ? Guid.Empty : Guid.Parse(Request["item_oid"]);
            String qty_string = Request["qty_spin_edit"] == null || Request["qty_spin_edit"] == "null" ? ".0" : Request["qty_spin_edit"];
            decimal qty;

            bool itemSupplierChanged = Session["itemSupplierId"] == Request["itemSupplierId"];
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;


            if (!decimal.TryParse(qty_string, out qty))//IE hack
            {
                Session["Error"] = Resources.InvalidItemQty;
                return null;
            }
            qty /= DocumentHelper.QUANTITY_MULTIPLIER;

            if (documentHeader.DocumentType.MaxDetailQty > 0 && qty >= documentHeader.DocumentType.MaxDetailQty)
            {
                Session["Error"] = Resources.InvalidItemQty;
                return null;
            }

            List<SelectedItemsQty> selected_items_qty = Session["selected_items_qty"] != null && (Session["selected_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(Session["currentDocument"] as DocumentHeader);
            bool add_to_list = true;
            if (selected_items_qty != null && item_oid != Guid.Empty)
            {
                SelectedItemsQty f = selected_items_qty.Find(x => x.item.Oid == item_oid);
                if (f != null)
                {
                    f.qty = qty;
                    add_to_list = false;
                }
            }

            if (add_to_list && qty > 0)
            {
                SelectedItemsQty temp_item = new SelectedItemsQty() { item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", item_oid, BinaryOperatorType.Equal)), qty = qty };
                selected_items_qty.Add(temp_item);
            }

            if (categoryid != "")
            {
                Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;
                ItemCategory cat = ((DocumentHeader)Session["currentDocument"]).Session.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                ViewData["categoryid"] = categoryid;
                ViewData["categoryDescription"] = cat.Description;

                List<PriceCatalogDetail> details;
                CriteriaOperator firstFilter;
                Guid itemSupplierGuid = Guid.Empty;
                if (Request["itemSupplierId"] != null && Guid.TryParse(Request["itemSupplierId"].ToString(), out itemSupplierGuid))
                {
                    firstFilter = CriteriaOperator.And(cat.GetAllNodeTreeFilter("Item.ItemAnalyticTrees"), new BinaryOperator("Item.DefaultSupplier.Oid", itemSupplierGuid));
                    Session["itemSupplierId"] = Request["itemSupplierId"];
                }
                else
                {
                    firstFilter = cat.GetAllNodeTreeFilter("Item.ItemAnalyticTrees");
                }
                if (Session["ItemsOfNodeSelectedCategoryID"] == null || (Guid)Session["ItemsOfNodeSelectedCategoryID"] != category_guid || itemSupplierChanged)
                {
                    //details = PriceCatalogHelper.GetTreePriceCatalogDetails(((DocumentHeader)Session["currentDocument"]).PriceCatalog, firstFilter);
                    details = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(documentHeader.Store, firstFilter).ToList();

                    //Checks for items in available Items Categories
                    if (((DocumentHeader)Session["currentDocument"]).DocumentType.DocumentTypeItemCategoryMode != eDocumentTypeItemCategory.NONE)
                    {
                        foreach (PriceCatalogDetail detail in details.ToList())
                        {
                            if (!DocumentHelper.DocumentTypeSupportsItem((DocumentHeader)Session["currentDocument"], detail.Item))
                            {
                                details.Remove(detail);
                            }
                        }
                    }

                    Session["ItemsOfNodeSelectedCategoryID"] = category_guid;
                    Session["ItemsOfNodeSelectedPricesOfCategory"] = details == null ? new List<PriceCatalogDetail>() : details.ToList();
                    Session["itemSupplierId"] = Request["itemSupplierId"];

                }
                else
                {
                    details = Session["ItemsOfNodeSelectedPricesOfCategory"] as List<PriceCatalogDetail>;
                }

                var qtys = GetSelectedItemsQtys(documentHeader, details, selected_items_qty, SupportsPacking);
                var qtysToList = qtys.ToList();
                Session["TreeItems"] = qtysToList;
                Session["selected_items_qty"] = selected_items_qty;
                ViewData["itemsOfNode"] = selected_items_qty.Where(g => g.qty > 0).Count() > 0;
            }
            return PartialView("ItemsOfNode");
        }

        protected string OrderMultipleItems()
        {
            Int32 newItemsInOrder = 0;
            List<SelectedItemsQty> selected_items_qty = Session["selected_items_qty"] != null && (Session["selected_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(Session["currentDocument"] as DocumentHeader);
            DocumentHeader currentDocumentHeader = ((DocumentHeader)Session["currentDocument"]);

            IEnumerable<SelectedItemsQty> new_added_items = selected_items_qty.Where(item => item.qty > 0).ToList();

            if (DocumentHelper.CheckIfMaximumLinesLimitWillExceed(currentDocumentHeader, new_added_items.Count()))
            {
                return Resources.MaxCountOfDocLinesExceeded;
            }

            foreach (SelectedItemsQty new_order in new_added_items)
            {
                try
                {
                    Item the_item = currentDocumentHeader.Session.FindObject<Item>(new BinaryOperator("Oid", new_order.item.Oid, BinaryOperatorType.Equal));
                    PriceCatalogDetail priceCatalogDetail = new_order.PriceCatalogDetail(currentDocumentHeader.Store, currentDocumentHeader.Customer);
                    Barcode the_barcode = currentDocumentHeader.Session.FindObject<Barcode>(new BinaryOperator("Oid", priceCatalogDetail.Barcode.Oid, BinaryOperatorType.Equal));
                    DocumentDetail tempDocumentDetail = DocumentHelper.ComputeDocumentLine(ref currentDocumentHeader, the_item, the_barcode, new_order.qty, false, -1, false, "", null);
                    tempDocumentDetail.IsActive = true;
                    DocumentHelper.SetDocumentHeaderValuesToZero(ref currentDocumentHeader);
                    DocumentHelper.AddItem(ref currentDocumentHeader, tempDocumentDetail);

                    if (currentDocumentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING && !(the_item.PackingQty <= 0 || the_item.PackingMeasurementUnit == null || the_item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(currentDocumentHeader.Session as UnitOfWork, the_item, currentDocumentHeader.Owner).MeasurementUnit))
                    {
                        new_order.order_qty += new_order.qty * (decimal)the_item.PackingQty;
                    }
                    else
                    {
                        new_order.order_qty += new_order.qty;
                    }
                    new_order.qty = .0m;
                    newItemsInOrder++;

                    Session["selected_items_qty"] = DocumentHelper.SelectedItemsTotalQuantity(currentDocumentHeader);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
                if (Session["selected_offers_qty"] != null)
                {
                    UpdateList(selected_items_qty, Session["selected_offers_qty"] as List<SelectedItemsQty>);
                }
                if (Session["selected_fresh_items_qty"] != null)
                {
                    UpdateList(selected_items_qty, Session["selected_fresh_items_qty"] as List<SelectedItemsQty>);
                }
            }
            Session["Notice"] = String.Format("{0} {1}", newItemsInOrder, Resources.SuccesfullyAddedItems);
            ViewData["itemsOfNode"] = (newItemsInOrder > 0);
            DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
            SaveObjectTemp(currentDocumentHeader, CurrentUser);
            return string.Empty;
        }

        [AjaxOrChildActionOnly]
        public ActionResult OrderByOffer()
        {

            List<CriteriaOperator> priceCatalogFilters;
            if (UserHelper.IsCustomer(CurrentUser))
            {
                if ((Session["currentDocument"] as DocumentHeader).Store == null) //no store selected
                {
                    Session["Error"] = Resources.PleaseSelectAStore;
                    ViewData["ItemOffers"] = GetList<Offer>((Session["currentDocument"] as DocumentHeader).Session, new BinaryOperator("Oid", Guid.Empty));
                    return PartialView("OrderByOffer", ViewData["ItemOffers"]);
                }

                priceCatalogFilters = UserHelper.GetUserCustomerPriceCatalogsFilter(CurrentUser, (Session["currentDocument"] as DocumentHeader).Store, "PriceCatalog.Oid");
            }
            else
            {
                priceCatalogFilters = UserHelper.GetUserCustomerPriceCatalogsFilter((Session["currentDocument"] as DocumentHeader).Customer, (Session["currentDocument"] as DocumentHeader).Store, "PriceCatalog.Oid");
            }
            CriteriaOperator offersCrop = CriteriaOperator.And(
                new ContainsOperator("OfferDetails", new BinaryOperator("Item.IsActive", true))
                , CriteriaOperator.Or(priceCatalogFilters), new BinaryOperator("EndDate", DateTime.Now, BinaryOperatorType.GreaterOrEqual));
            ViewData["ItemOffers"] = GetList<Offer>((Session["currentDocument"] as DocumentHeader).Session, offersCrop);
            return PartialView("OrderByOffer", ViewData["ItemOffers"]);
        }

        [AjaxOrChildActionOnly]
        public ActionResult OfferDetails()
        {
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("SHOWHIDEFILTERS") >= 0)
            {
                if (Session["showOffersFilter"] == null)
                {
                    Session["showOffersFilter"] = false;
                }
                Session["showOffersFilter"] = !(bool)Session["showOffersFilter"];
                return PartialView();
            }

            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("STARTEDIT") >= 0)
            {
                return PartialView("OfferDetails");
            }
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDEROFFERS") >= 0)
            {
                string errorMessage = OrderMultipleOffers();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Session["Error"] = errorMessage;
                    return null;
                }

                ViewData["offerDetails"] = false;
            }

            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("CLEAROFFERS") >= 0 && Session["selected_offers_qty"] != null)
            {
                List<SelectedItemsQty> selected_offers_qty = Session["selected_offers_qty"] as List<SelectedItemsQty>;

                foreach (SelectedItemsQty itm in selected_offers_qty)
                {
                    itm.qty = 0;
                }

                Session["selected_offers_qty"] = selected_offers_qty;
            }

            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("CLEAROFFERS") >= 0 || Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("GETOFFERDETAILS") >= 0 || Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDEROFFERS") >= 0)
            {
                Guid offerGuid;
                String toParse = Request["DXCallbackArgument"].ToString().Substring(Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("GETOFFERDETAILS:") + "GETOFFERDETAILS:".Length);
                toParse = toParse.Substring(0, toParse.Length - 1);
                if (Guid.TryParse(toParse, out offerGuid) ||
                        (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDEROFFERS") >= 0 && Session["currentOffer"] != null)
                    )
                {
                    Offer offer;
                    if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDEROFFERS") >= 0 && Session["currentOffer"] != null)
                    {
                        offer = Session["currentOffer"] as Offer;
                    }
                    else
                    {
                        offer = ((DocumentHeader)Session["currentDocument"]).Session.FindObject<Offer>(new BinaryOperator("Oid", offerGuid));
                        Session["currentOffer"] = offer;
                    }
                    Guid itemSupplierGuid;
                    if (Request["itemSupplierId"] != null && Guid.TryParse(Request["itemSupplierId"].ToString(), out itemSupplierGuid))
                    {
                        offer.OfferDetails.Filter = CriteriaOperator.And(offer.OfferDetails.Filter
                                                            , new BinaryOperator("Item.DefaultSupplier.Oid", itemSupplierGuid));
                    }
                    else
                    {
                        offer.OfferDetails.Filter = null;
                    }
                    DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;

                    offer.OfferDetails.Filter = CriteriaOperator.And(offer.OfferDetails.Filter, DocumentHelper.DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(documentHeader));


                    bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;
                    List<SelectedItemsQty> selected_offers_qty = Session["selected_offers_qty"] != null && (Session["selected_offers_qty"] as List<SelectedItemsQty>).Count > 0
                        ? Session["selected_offers_qty"] as List<SelectedItemsQty>
                        : DocumentHelper.SelectedItemsTotalQuantity(documentHeader);

                    var qtys = from obj in offer.OfferDetails
                               join ofr in selected_offers_qty on obj.Item.Oid equals ofr.item.Oid into gj
                               from subset in gj.DefaultIfEmpty()
                               select new
                               {
                                   item_oid = obj.Item.Oid,
                                   item_description = obj.Item.Name,
                                   item_code = obj.Item.Code,
                                   Value = ItemHelper.GetItemPriceWithoutTax(documentHeader.Store, obj.Item, documentHeader.Customer),
                                   Qty = (subset == null ? .0m : subset.qty),
                                   pack_measurement_unit = (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit : obj.Item.PackingMeasurementUnit) : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit),
                                   packing_qty = (subset == null ? 0 : (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? (subset == null ? .0m : subset.qty) : (decimal)obj.Item.PackingQty * (subset == null ? .0m : subset.qty)) : (subset == null ? .0m : subset.qty))),
                                   measurement_unit = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner) == null ? null : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit,
                                   order_qty = (subset == null ? .0m : subset.order_qty),
                                   item_checked = (subset == null ? .0m : subset.qty) > 0,
                                   item_support_decimal = (obj.Item.DefaultBarcode != null && obj.Item.DefaultBarcode.MeasurementUnit(documentHeader.Owner) != null) ? obj.Item.DefaultBarcode.MeasurementUnit(documentHeader.Owner).SupportDecimal : false
                               };
                    var qtysToList = qtys.Where(g => g.Value >= 0).ToList();
                    Session["ItemOfferDetails"] = qtysToList;
                    Session["selected_offers_qty"] = selected_offers_qty;
                    ViewData["offerDetails"] = selected_offers_qty.Where(g => g.qty > 0).Count() > 0;

                    if (Session["selected_items_qty"] != null)
                    {
                        UpdateList(selected_offers_qty, Session["selected_items_qty"] as List<SelectedItemsQty>);
                    }
                    if (Session["selected_fresh_items_qty"] != null)
                    {
                        UpdateList(selected_offers_qty, Session["selected_fresh_items_qty"] as List<SelectedItemsQty>);
                    }
                }
            }
            return PartialView("OfferDetails");
        }

        protected void GetOfferDetails()
        {
            Guid selected_offer = Request["selected_offer"] == null || Request["selected_offer"] == "null" ? Guid.Empty : Guid.Parse(Request["selected_offer"]);
            if (selected_offer != Guid.Empty)
            {
                Offer offer = ((DocumentHeader)Session["currentDocument"]).Session.FindObject<Offer>(new BinaryOperator("Oid", selected_offer, BinaryOperatorType.Equal));
                Session["currentOffer"] = offer;
            }
            else// initialize offer details
            {
                Session["ItemOfferDetails"] = Session["selected_offers_qty"] = null;
            }
        }

        [AjaxOrChildActionOnly]
        public ActionResult UpdateOfferDetails()
        {
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("CLEARLINE") >= 0)
                GetOfferDetails();
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("STARTUP") >= 0)
            {
                Session["ItemOfferDetails"] = null;
                ViewData["offerDetails"] = false;
                return PartialView("OfferDetails");

            }
            Guid offer_detail_item_guid = Request["item_oid"] == null || Request["item_oid"] == "null" ? Guid.Empty : Guid.Parse(Request["item_oid"]);
            if (offer_detail_item_guid != Guid.Empty && Session["currentOffer"] != null)
            {
                String qty_string = Request["qty_spin_edit"] == null || Request["qty_spin_edit"] == "null" ? ".0" : Request["qty_spin_edit"];
                decimal qty;
                if (!decimal.TryParse(qty_string, out qty))//IE hack
                {
                    Session["Error"] = Resources.InvalidItemQty;
                    return null;
                }
                DocumentHeader currentDocumentHeader = Session["currentDocument"] as DocumentHeader;
                qty /= DocumentHelper.QUANTITY_MULTIPLIER;

                Offer offer = ((Offer)Session["currentOffer"]);
                DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
                bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;
                List<SelectedItemsQty> selected_offers_qty = Session["selected_offers_qty"] != null && (Session["selected_offers_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_offers_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(documentHeader);

                if (selected_offers_qty != null)
                {
                    bool add_to_list = true;
                    foreach (var f in selected_offers_qty.Where(x => x.item.Oid == offer_detail_item_guid))
                    {
                        f.qty = qty;
                        add_to_list = false;
                    }
                    if (add_to_list)
                    {
                        SelectedItemsQty temp_item = new SelectedItemsQty() { item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", offer_detail_item_guid, BinaryOperatorType.Equal)), qty = qty };
                        selected_offers_qty.Add(temp_item);
                    }
                }

                var qtys = from obj in offer.OfferDetails
                           join ofr in selected_offers_qty on obj.Item.Oid equals ofr.item.Oid into gj
                           from subset in gj.DefaultIfEmpty()
                           select new
                           {
                               item_oid = obj.Item.Oid,
                               item_description = obj.Item.Name,
                               item_code = obj.Item.Code,
                               Value = ItemHelper.GetItemPriceWithoutTax(documentHeader.Store, obj.Item, documentHeader.Customer),
                               Qty = (subset == null ? .0m : subset.qty),
                               pack_measurement_unit = (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit : obj.Item.PackingMeasurementUnit) : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit),
                               packing_qty = (subset == null ? 0 : (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? (subset == null ? .0m : subset.qty) : (decimal)obj.Item.PackingQty * (subset == null ? .0m : subset.qty)) : (subset == null ? .0m : subset.qty))),
                               measurement_unit = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner) == null ? null : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit,
                               order_qty = (subset == null ? .0m : subset.order_qty),
                               item_checked = (subset == null ? .0m : subset.qty) > 0,
                               item_support_decimal = (obj.Item.DefaultBarcode != null && obj.Item.DefaultBarcode.MeasurementUnit(documentHeader.Owner) != null) ? obj.Item.DefaultBarcode.MeasurementUnit(documentHeader.Owner).SupportDecimal : false
                           };
                var qtysToList = qtys.Where(g => g.Value >= 0).ToList();
                Session["ItemOfferDetails"] = qtysToList;
                Session["selected_offers_qty"] = selected_offers_qty;
                ViewData["offerDetails"] = selected_offers_qty.Where(g => g.qty > 0).Count() > 0;
            }
            return PartialView("OfferDetails");
        }

        protected string OrderMultipleOffers()
        {
            int newItemsInOrder = 0;
            List<SelectedItemsQty> selected_offers_qty = Session["selected_offers_qty"] != null && (Session["selected_offers_qty"] as List<SelectedItemsQty>).Count > 0
                ? Session["selected_offers_qty"] as List<SelectedItemsQty>
                : DocumentHelper.SelectedItemsTotalQuantity(Session["currentDocument"] as DocumentHeader);
            DocumentHeader currentDocumentHeader = ((DocumentHeader)Session["currentDocument"]);

            IEnumerable<SelectedItemsQty> new_added_items = selected_offers_qty.Where(item => item.qty > 0).ToList();

            if (DocumentHelper.CheckIfMaximumLinesLimitWillExceed(currentDocumentHeader, new_added_items.Count()))
            {
                return Resources.MaxCountOfDocLinesExceeded;
            }

            foreach (SelectedItemsQty new_order in new_added_items)
            {
                try
                {
                    Item the_item = currentDocumentHeader.Session.FindObject<Item>(new BinaryOperator("Oid", new_order.item.Oid, BinaryOperatorType.Equal));
                    PriceCatalogDetail priceCatalogDetail = new_order.PriceCatalogDetail(currentDocumentHeader.Store, currentDocumentHeader.Customer);
                    Barcode the_barcode = currentDocumentHeader.Session.FindObject<Barcode>(new BinaryOperator("Oid", priceCatalogDetail.Barcode.Oid, BinaryOperatorType.Equal));
                    DocumentDetail tempDocumentDetail = DocumentHelper.ComputeDocumentLine(ref currentDocumentHeader, the_item, the_barcode, new_order.qty, false, -1, false, "", null);
                    tempDocumentDetail.IsActive = true;
                    DocumentHelper.SetDocumentHeaderValuesToZero(ref currentDocumentHeader);
                    DocumentHelper.AddItem(ref currentDocumentHeader, tempDocumentDetail);

                    if (currentDocumentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING && !(the_item.PackingQty <= 0 || the_item.PackingMeasurementUnit == null || the_item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(currentDocumentHeader.Session as UnitOfWork, the_item, currentDocumentHeader.Owner).MeasurementUnit))
                    {
                        new_order.order_qty += new_order.qty * (decimal)the_item.PackingQty;
                    }
                    else
                    {
                        new_order.order_qty += new_order.qty;
                    }
                    new_order.qty = 0;
                    newItemsInOrder++;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            Session["Notice"] = String.Format("{0} {1}", newItemsInOrder, Resources.SuccesfullyAddedItems);
            DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
            SaveObjectTemp(currentDocumentHeader, CurrentUser);
            return string.Empty;
        }

        [AjaxOrChildActionOnly]
        public JsonResult jsonCheckDocumentUserActions()
        {
            Session["Error"] = null;

            DocumentHeader documentHeader = ((DocumentHeader)Session["currentDocument"]);

            int displayTab = -1;
            bool document_ok = false;
            bool displayMarkUpForm = false;
            int numberOfReports = 0;



            if (documentHeader == null)
            {
                Session["Error"] = Resources.ConnectionTimeOut;
                return Json(new
                {
                    document_ok = document_ok,
                    doid = Guid.Empty,
                    numberOfReports = numberOfReports,
                    displayMarkUpForm = displayMarkUpForm,
                    displayTab = displayTab
                });
            }

            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;
            int num_of_details = documentHeader.DocumentDetails.Count;
            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
            if ((Session["Error"] == null
                && documentHeader.Division != eDivision.Financial
                && num_of_details < 1
                 )
                 || (documentHeader.DocumentType.ManualLinkedLineInsertion
                      && (num_of_details < 1
                           || documentHeader.DocumentDetails.Where(docDetail => docDetail.LinkedLine == Guid.Empty && docDetail.LinkedLines.Count <= 0).Count() >= 1
                         )
                    )
               )
            {
                Session["Error"] = Resources.PleaseAddItems;
                displayTab = 0;
            }

            if (documentHeader.DefaultDocumentDiscount < 0 || documentHeader.DefaultDocumentDiscount > 100)
            {
                Session["Error"] = "Invalid Document Discount";
            }

            if (Session["Error"] == null
              && documentHeader.Division == eDivision.Financial
              && (documentHeader.DocumentPayments == null
                   || documentHeader.DocumentPayments.Count <= 0
                )
              )
            {
                Session["Error"] = Resources.PleaseAddPayments;
                displayTab = 1;
            }

            if (Session["notIncludedDetailsOids"] != null)
            {
                foreach (DocumentDetail detail in documentHeader.DocumentDetails)
                {
                    if (((List<string>)Session["notIncludedDetailsOids"]).Contains(detail.Oid.ToString()))
                    {
                        Session["Error"] = Resources.DocumentContainsNotIncludedItems;
                    }
                }
            }

            switch (documentHeader.Division)
            {
                case eDivision.Financial:
                    break;
                case eDivision.Other:
                    throw new NotSupportedException(String.Format("Unknown Division {0}", documentHeader.Division));
                case eDivision.Purchase:
                    if (Session["Error"] == null && documentHeader.Supplier == null)
                    {
                        Session["Error"] = Resources.PleaseSelectASupplier;
                    }
                    break;
                case eDivision.Sales:
                    if (Session["Error"] == null && documentHeader.Customer == null)
                    {
                        Session["Error"] = Resources.PleaseSelectACustomer;
                    }
                    else if (Session["Error"] == null && !DocumentHelper.DocTypeSupportsCustomer(documentHeader, documentHeader.Customer))
                    {
                        Session["Error"] = Resources.DocTypeNoSupportCustCat;
                    }
                    else if (Session["Error"] == null && String.IsNullOrWhiteSpace(documentHeader.DeliveryAddress))
                    {
                        Session["Error"] = Resources.PleaseFillInDeliveryAddress + ".";
                        displayTab = 1;
                    }
                    break;
                case eDivision.Store:
                    if (Session["Error"] == null && documentHeader.SecondaryStore == null)
                    {
                        Session["Error"] = Resources.PleaseSelectAStore;
                    }
                    break;
                default:
                    throw new NotSupportedException(String.Format("Unknown Division {0}", documentHeader.Division));
            }

            if (Session["Error"] == null && (documentHeader.DocumentType == null || documentHeader.DocumentSeries == null || documentHeader.Status == null))
            {
                Session["Error"] = Resources.PleaseFillInDocumentHeaderData;
            }
            if (Session["Error"] == null && documentHeader.DocumentSeries.HasAutomaticNumbering == false && documentHeader.Status.TakeSequence && documentHeader.DocumentNumber <= 0)
            {
                Session["Error"] = Resources.InvalidDocumentNumber;
            }
            if (Session["Error"] == null && documentHeader.HasBeenExecuted && (documentHeader.InvoicingDate == null || documentHeader.InvoicingDate == DateTime.MinValue))
            {
                Session["Error"] = Resources.PleaseFillInInvoiceDate;
            }

            string errorMessage = DocumentHelper.CreateDefaultDocumentPayments(documentHeader);
            if (Session["Error"] == null && string.IsNullOrEmpty(errorMessage) == false)
            {
                Session["Error"] = errorMessage;
                displayTab = 2;
            }

            if (Session["Error"] == null)
            {
                try
                {
                    StoreDocumentSeriesType storeDocSerType = documentHeader.Session.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", documentHeader.DocumentType),
                                                                                                                                              new BinaryOperator("DocumentSeries", documentHeader.DocumentSeries)));
                    if (storeDocSerType != null &&
                        storeDocSerType.DefaultCustomReport != null &&
                        storeDocSerType.DefaultCustomReport.ReportRoles.Select(reportrole => reportrole.Role).Contains(CurrentUser.Role) &&
                        (storeDocSerType.UserType == UserType.ALL || storeDocSerType.UserType == UserHelper.GetUserType(CurrentUser)))
                    {
                        numberOfReports = 1;
                    }
                    else
                    {
                        numberOfReports = ReportsHelper.GetValidDocumentTypeCustomReports(CurrentUser, documentHeader.DocumentType, XpoHelper.GetNewUnitOfWork()).Select(x => x.Report).Distinct().Count();
                    }

                    SaveDocument(documentHeader);
                    document_ok = true;
                }
                catch (SignatureFailureException signatureFailureException)
                {
                    Session["Error"] = Resources.CannotRetreiveSignature + Environment.NewLine + signatureFailureException.Message;
                }
                catch (Exception e)
                {
                    string message = e.Message;
                    if (e.InnerException != null && String.IsNullOrEmpty(e.InnerException.Message) == false)
                    {
                        message += e.InnerException.Message;
                    }
                    Session["Error"] = message;
                }
            }




            displayMarkUpForm = document_ok && documentHeader.DisplayMarkUpForm;

            return Json(new
            {
                document_ok = document_ok,
                doid = documentHeader.Oid.ToString(),
                numberOfReports = numberOfReports,
                displayMarkUpForm = displayMarkUpForm,
                displayTab = displayTab
            });
        }

        private void SaveDocument(DocumentHeader currentDocumentHeader)
        {
            if (currentDocumentHeader == null || currentDocumentHeader.DocumentDetails.Count == 0)
            {
                Session["Error"] = Resources.PleaseAddItems;
                return;
            }

            Guid DocumentType = Guid.Empty;
            if (String.IsNullOrEmpty(Request["DocumentType"]) || Request["DocumentType"] == null || Request["DocumentType"] == "null")
            {
                Guid.TryParse(Request["DocumentType"].ToString(), out DocumentType);
            }
            Guid DocumentSeries = Guid.Empty;
            if (String.IsNullOrEmpty(Request["DocumentSeries"]) || Request["DocumentSeries"] == null || Request["DocumentSeries"] == "null")
            {
                Guid.TryParse(Request["DocumentSeries"].ToString(), out DocumentSeries);
            }

            Guid Status = Guid.Empty;
            if (String.IsNullOrEmpty(Request["Status"]) || Request["Status"] == null || Request["Status"] == "null")
            {
                Guid.TryParse(Request["Status"].ToString(), out Status);
            };

            Guid Tablet = Guid.Empty;
            if (!String.IsNullOrEmpty(Request["Tablet"]) && Request["Tablet"] != null && Request["Tablet"] != "null")
            {
                Guid.TryParse(Request["Tablet"].ToString(), out Tablet);
            };

            string FinalizedDate_str = Request["FinalizedDate"] == null || Request["FinalizedDate"] == "null" ? DateTime.MinValue.ToString() : Request["FinalizedDate"];
            string RefferenceDate_str = Request["RefferenceDate"] == null || Request["RefferenceDate"] == "null" ? DateTime.MinValue.ToString() : Request["RefferenceDate"];
            string InvoicingDate_str = Request["InvoicingDate"] == null || Request["InvoicingDate"] == "null" ? null : Request["InvoicingDate"];
            Guid BillingAddressGuid = String.IsNullOrEmpty(Request["BillingAddress"]) || Request["BillingAddress"] == null || Request["BillingAddress"] == "null" ? Guid.Empty : Guid.Parse(Request["BillingAddress"]);
            String ExecutionDate_str = Request["ExecutionDate"] == null || Request["ExecutionDate"] == "null" ? "" : Request["ExecutionDate"];

            int DocumentNumber;
            if (!Int32.TryParse(Request["DocumentNumber"], out DocumentNumber))
            {
                DocumentNumber = -1;
            }

            #region Document Date Times
            DateTime FinalizedDate = DocumentHelper.ConvertToDateTime(FinalizedDate_str);
            DateTime RefferenceDate;
            DateTime.TryParse(RefferenceDate_str, out RefferenceDate);
            DateTime InvoicingDate = DateTime.MinValue;
            DateTime? ExecutionDate = string.IsNullOrEmpty(ExecutionDate_str) ? null : (DateTime?)DocumentHelper.ConvertToDateTime(ExecutionDate_str);


            if (!String.IsNullOrWhiteSpace(InvoicingDate_str))
            {
                InvoicingDate = DocumentHelper.ConvertToDateTime(InvoicingDate_str);
            }
            if (DocumentHelper.CurrentUserDocumentView(CurrentUser, currentDocumentHeader.DocumentType).Equals(eDocumentTypeView.Simple))
            {
                //Data DO NOT exist in the form, so they are not posted but they have been set at the beggining
                FinalizedDate = currentDocumentHeader.FinalizedDate;
                RefferenceDate = currentDocumentHeader.RefferenceDate;
                InvoicingDate = currentDocumentHeader.InvoicingDate;
            }
            #endregion

            //Payment Methods Check

            if (currentDocumentHeader.TransformationLevel != eTransformationLevel.FREEZE_EDIT
                && currentDocumentHeader.TransformationLevel != eTransformationLevel.FREEZE_VALUES
                )
            {
                if (currentDocumentHeader.DocumentType.MergedSameDocumentLines)
                {
                    DocumentHelper.MergeDocumentLines(ref currentDocumentHeader);
                }
            }

            currentDocumentHeader.Remarks = Request["Remarks"] == null ? "" : Request["Remarks"].ToString();

            currentDocumentHeader.TransferMethod = Request["TransferMethod"] == null || Request["TransferMethod"] == "null" ? "" : Request["TransferMethod"].ToString();
            currentDocumentHeader.PlaceOfLoading = Request["PlaceOfLoading"] == null || Request["PlaceOfLoading"] == "null" ? "" : Request["PlaceOfLoading"].ToString();
            currentDocumentHeader.VehicleNumber = Request["VehicleNumber"] == null || Request["VehicleNumber"] == "null" ? "" : Request["VehicleNumber"].ToString();
            Guid transferPurposeGuid = Guid.Empty;
            Guid.TryParse(Request["TransferPurpose"], out transferPurposeGuid);
            currentDocumentHeader.TransferPurpose = currentDocumentHeader.Session.GetObjectByKey<TransferPurpose>(transferPurposeGuid);

            if (Request["HasBeenChecked"] != null)
            {
                currentDocumentHeader.HasBeenChecked = Boolean.Parse(Request["HasBeenChecked"]);
            }

            if (Request["HasBeenExecuted"] != null)
            {
                currentDocumentHeader.HasBeenExecuted = Boolean.Parse(Request["HasBeenExecuted"]);
            }

            if (Request["DeliveryAddress"] != null)
            {
                currentDocumentHeader.DeliveryAddress = Request["DeliveryAddress"];
            }

            if (currentDocumentHeader.DocumentType == null)
            {

                if (DocumentType == Guid.Empty
                    && currentDocumentHeader.TransformationLevel != eTransformationLevel.FREEZE_EDIT
                    && currentDocumentHeader.TransformationLevel != eTransformationLevel.FREEZE_VALUES
                    )
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
                else if (DocumentType != Guid.Empty)
                {
                    currentDocumentHeader.DocumentType =
                        currentDocumentHeader.Session.FindObject<DocumentType>(new BinaryOperator("Oid", DocumentType, BinaryOperatorType.Equal));
                }
            }

            if (currentDocumentHeader.DocumentSeries == null
                || (currentDocumentHeader.DocumentSeries != null
                        && currentDocumentHeader.DocumentSeries.Oid != DocumentSeries
                        && DocumentSeries != Guid.Empty
                    )
                )
            {
                currentDocumentHeader.DocumentSeries =
                  currentDocumentHeader.Session.FindObject<DocumentSeries>(new BinaryOperator("Oid", DocumentSeries, BinaryOperatorType.Equal));

            }


            Guid triangularCustomerGuid = Guid.Empty;
            if (Guid.TryParse(Request["TriangularCustomer"], out triangularCustomerGuid))
            {
                Customer triangularCustomer = currentDocumentHeader.Session.GetObjectByKey<Customer>(triangularCustomerGuid);
                if (triangularCustomer != null)
                {
                    currentDocumentHeader.TriangularCustomer = triangularCustomer;
                }
            }
            else
            {
                currentDocumentHeader.TriangularCustomer = null;
            }

            Guid triangularSupplierGuid = Guid.Empty;
            if (Guid.TryParse(Request["TriangularSupplier"], out triangularSupplierGuid))
            {
                SupplierNew triangularSupplier = currentDocumentHeader.Session.GetObjectByKey<SupplierNew>(triangularSupplierGuid);
                if (triangularSupplier != null)
                {
                    currentDocumentHeader.TriangularSupplier = triangularSupplier;
                }
            }
            else
            {
                currentDocumentHeader.TriangularSupplier = null;
            }

            Guid triangularStoreGuid = Guid.Empty;
            if (Guid.TryParse(Request["TriangularStore"], out triangularStoreGuid))
            {
                Store triangularStore = currentDocumentHeader.Session.GetObjectByKey<Store>(triangularStoreGuid);
                if (triangularStore != null)
                {
                    currentDocumentHeader.TriangularStore = triangularStore;
                }
            }
            else
            {
                currentDocumentHeader.TriangularStore = null;
            }

            if (Status != null && Status != Guid.Empty)
            {
                currentDocumentHeader.Status = currentDocumentHeader.Session.FindObject<DocumentStatus>(new BinaryOperator("Oid", Status, BinaryOperatorType.Equal));
                if (currentDocumentHeader.Status.TakeSequence == false && DocumentNumber >= 0)
                {
                    currentDocumentHeader.DocumentNumber = DocumentNumber;
                }
            }
            if (Tablet != null && Tablet != Guid.Empty)
            {
                currentDocumentHeader.Tablet = currentDocumentHeader.Session.FindObject<SFA>(new BinaryOperator("Oid", Tablet, BinaryOperatorType.Equal));
            }

            if (FinalizedDate != DateTime.MinValue)
            {
                currentDocumentHeader.FinalizedDate = FinalizedDate;
            }
            if (RefferenceDate != DateTime.MinValue)
            {
                currentDocumentHeader.RefferenceDate = RefferenceDate;
            }
            if (InvoicingDate != DateTime.MinValue)
            {
                currentDocumentHeader.InvoicingDate = InvoicingDate;
            }

            currentDocumentHeader.ExecutionDate = ExecutionDate;

            currentDocumentHeader.BillingAddress = currentDocumentHeader.Session.GetObjectByKey<Address>(BillingAddressGuid);

            User currentUser = CurrentUser == null ? null : currentDocumentHeader.Session.GetObjectByKey<User>(CurrentUser.Oid);
            if (currentUser != null)
            {
                if (currentDocumentHeader.IsNewRecord)
                {
                    currentDocumentHeader.CreatedBy = currentUser;
                }
                currentDocumentHeader.UpdatedBy = currentUser;
            }

            currentDocumentHeader.Save();

            UnitOfWork uow = currentDocumentHeader.Session as UnitOfWork;
            ICollection todelete = uow.GetObjectsInTransaction(uow.GetClassInfo<DocumentDetail>(), new NullOperator("DocumentHeader"), false);
            uow.Delete(todelete);

            todelete = uow.GetObjectsInTransaction(uow.GetClassInfo<DocumentDetailDiscount>(), new NullOperator("DocumentDetail"), false);
            uow.Delete(todelete);


            XpoHelper.CommitTransaction(uow);
            using (UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
            {
                TemporaryObject tempObject = uow2.FindObject<TemporaryObject>(CriteriaOperator.And(
                                                                            new BinaryOperator("EntityOid", currentDocumentHeader.Oid),
                                                                            new BinaryOperator("CreatedBy.Oid", this.CurrentUser.Oid),
                                                                            new BinaryOperator("EntityType", typeof(DocumentHeader).FullName)
                                                                            ));
                if (tempObject != null)
                {
                    tempObject.Delete();
                    uow2.CommitChanges();
                }
            }


            //Digital Signature
            if (currentDocumentHeader.DocumentType.TakesDigitalSignature
                && currentDocumentHeader.DocumentNumber > 0
                && currentDocumentHeader.Status.TakeSequence
                && String.IsNullOrEmpty(currentDocumentHeader.Signature)
                //TEMPORARY Restriction

                )
            {
                List<POSDevice> posDevices = null;

                if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
                {
                    StoreControllerSettings settings = currentDocumentHeader.Session.GetObjectByKey<StoreControllerSettings>(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
                    posDevices = settings.StoreControllerTerminalDeviceAssociations.
                        Where(x =>
                                x.DocumentSeries.Any(y => y.DocumentSeries.Oid == currentDocumentHeader.DocumentSeries.Oid)
                             && x.TerminalDevice is POSDevice
                             && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                        ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                }
                else
                {
                    XPCollection<StoreControllerTerminalDeviceAssociation> associations = GetList<StoreControllerTerminalDeviceAssociation>(currentDocumentHeader.Session, new NullOperator("StoreControllerSettings"));
                    posDevices = associations.
                        Where(x =>
                                x.DocumentSeries.Any(y => y.DocumentSeries.Oid == currentDocumentHeader.DocumentSeries.Oid)
                             && x.TerminalDevice is POSDevice
                             && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                        ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                }


                string signature = DocumentHelper.SignDocument(currentDocumentHeader, this.CurrentUser, EffectiveOwner,
                    MvcApplication.OLAPConnectionString,
                    posDevices
                    );
                currentDocumentHeader.Signature = signature;
                currentDocumentHeader.Save();
                XpoHelper.CommitChanges(uow);
            }
            // End of Digital Signature

            ClearDocumentSession();

        }


        private void UpdateReferencedDocumentDetails(DocumentHeader documentHeader, bool isNewDocument)
        {
            if (isNewDocument || documentHeader.ReferencedDocuments.Count() == 0)
            {
                return;
            }

        }

        public ActionResult CloseOrderForm()
        {
            if (this.ToolbarOptions != null)
            {
                this.ToolbarOptions.ForceVisible = false;
            }
            return View();
        }

        public ActionResult TransformationPopup()
        {
            Session["transformationDetails"] = null;
            string[] oids = null;
            try
            {
                oids = Request["documents"].Split(',');
            }
            catch
            {

            }
            if (oids == null || oids.Length == 0)
            {
                Session["Error"] = "User not logged in or request invalid";
                return PartialView();
            }
            PrepareTransformationDocumentDetails(oids);
            return PartialView();
        }

        private void PrepareTransformationDocumentDetails(string[] oids)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            List<TransformationRule> allowedTransformationRulesTypes = new List<TransformationRule>();
            List<Guid> documentGuids = GetDocumentGuids(oids);
            XPCollection<DocumentHeader> documents = GetList<DocumentHeader>(uow, new InOperator("Oid", documentGuids));
            ViewData["DCs"] = documents.ToList();
            List<DocumentType> documentTypes = documents.Select(docHead => docHead.DocumentType).ToList();
            string message = String.Empty;
            eModule module = DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance);
            allowedTransformationRulesTypes = DocumentHelper.AllowedTransformationRules(documentTypes,
                                                                                        documents.First().Store,
                                                                                        module,
                                                                                         documents.First().Division,
                                                                                        out message);
            TransformationRule defaultRule = allowedTransformationRulesTypes == null ? null : (allowedTransformationRulesTypes.FirstOrDefault(transRule => transRule.IsDefault) ?? allowedTransformationRulesTypes.FirstOrDefault());
            DocumentType currentDocType = defaultRule == null ? null : defaultRule.DerrivedType;
            IEnumerable<DocumentSeries> allowedDocumentSeries = allowedTransformationRulesTypes == null ? null : StoreHelper.StoreSeriesForDocumentType(documents.First().Store, currentDocType, module);
            Session["transformationDetails"] = DocumentHelper.CreateDocumentDetailAssociations(documents);
            ViewData["allowedTransformationRulesTypes"] = allowedTransformationRulesTypes;
            ViewData["allowedDocumentSeries"] = allowedDocumentSeries;
            ViewData["defaultRule"] = defaultRule;
        }

        [AjaxOrChildActionOnly]
        public ActionResult TransformationDetailsGrid()
        {
            return PartialView();
        }

        public JsonResult jsonUpdateTransformationSelectedDocumentDetails()
        {
            if (Request["isSelected"] != null && Request["key"] != null)
            {
                Guid key = Guid.Empty;
                if (Guid.TryParse(Request["key"], out key))
                {
                    bool isSelected = Boolean.Parse(Request["isSelected"].ToString());
                    if (isSelected)
                    {
                        (Session["transformationDetails"] as List<DocumentDetailAssociation>).Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(key)).First<DocumentDetailAssociation>().RetrieveAllQuantity();
                    }
                    else
                    {
                        (Session["transformationDetails"] as List<DocumentDetailAssociation>).Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(key)).First<DocumentDetailAssociation>().RetrieveNothing();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            return Json(new { });
        }

        public JsonResult jsonClearTransformationData()
        {
            Session["transformationDetails"] = null;
            return Json(new { });
        }

        public ActionResult UpdateTransformationDetailsQty()
        {
            if (Request["DXCallbackArgument"].Contains("UPDATEEDIT") && Request["qty_spin_edit"] != null && Request["document_detail_association_guid"] != null)
            {
                string documentDetailAssociationGuidStr = Request["document_detail_association_guid"].ToString();
                Guid documentDetailAssociationGuid;
                decimal retrievedQuantity;
                if (Guid.TryParse(documentDetailAssociationGuidStr, out documentDetailAssociationGuid) && decimal.TryParse(Request["qty_spin_edit"], out retrievedQuantity))
                {
                    retrievedQuantity /= DocumentHelper.QUANTITY_MULTIPLIER;
                    if ((Session["transformationDetails"] as List<DocumentDetailAssociation>).
                        Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(documentDetailAssociationGuid)).Count() == 1
                        )
                    {
                        (Session["transformationDetails"] as List<DocumentDetailAssociation>).Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(documentDetailAssociationGuid)).
                            First().RetrievedQuantity = retrievedQuantity;
                        (Session["transformationDetails"] as List<DocumentDetailAssociation>).
                            Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(documentDetailAssociationGuid)).First<DocumentDetailAssociation>().RetrievedQuantity = retrievedQuantity;

                        (Session["transformationDetails"] as List<DocumentDetailAssociation>).
                            Where(docDetAssoc => docDetAssoc.UniqueKey.Equals(documentDetailAssociationGuid)).First<DocumentDetailAssociation>().IsSelected = retrievedQuantity > 0;
                    }
                }
                else
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
            }
            return PartialView("TransformationDetailsGrid");
        }

        private List<Guid> GetDocumentGuids(string[] guidsStrings)
        {
            List<Guid> dh_guids = new List<Guid>();
            foreach (string dh in guidsStrings)
            {
                Guid dh_guid;
                if (Guid.TryParse(dh, out dh_guid))
                {
                    dh_guids.Add(dh_guid);
                }
            }
            return dh_guids;
        }

        public ActionResult SelectItemSuppliers(string tab)
        {
            if (tab.Contains("_categories"))
            {
                ViewData["Tab"] = "_categories";
            }
            if (tab.Contains("_offers"))
            {
                ViewData["Tab"] = "_offers";
            }
            return PartialView();
        }

        public ActionResult DocumentInfoPanel()
        {
            return PartialView();
        }

        public ActionResult OrderTabsCustomer()
        {
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView();
        }

        public ActionResult NewItems()
        {
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;

            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("SHOWHIDEFILTERS") >= 0)
            {
                if (Session["showFreshItemsFilter"] == null)
                {
                    Session["showFreshItemsFilter"] = false;
                }
                Session["showFreshItemsFilter"] = !(bool)Session["showFreshItemsFilter"];
                return PartialView();
            }

            //CLEARFRESHITEMS
            if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("CLEARFRESHITEMS") >= 0 && Session["selected_offers_qty"] != null)
            {
                List<SelectedItemsQty> selected_fresh_items_qty = Session["selected_fresh_items_qty"] as List<SelectedItemsQty>;

                foreach (SelectedItemsQty itm in selected_fresh_items_qty)
                    itm.qty = 0;

                Session["selected_fresh_items_qty"] = selected_fresh_items_qty;
            }
            else if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ORDERITEMS") >= 0)
            {
                string errorMessage = OrderFreshItems();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Session["Error"] = errorMessage;
                    return null;
                }
            }
            else if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("SEARCH") >= 0)
            {
                if (Request["rdBLNewItems"] != null)
                {
                    CriteriaOperator crop = null;

                    NewItemOptions.TimeOptions userSelection = NewItemOptions.CustomTryParse(Request["rdBLNewItems"]);
                    switch (userSelection)
                    {
                        case NewItemOptions.TimeOptions.Today:
                            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                            crop = new BinaryOperator("Item.InsertedDate", now, BinaryOperatorType.GreaterOrEqual);
                            break;
                        case NewItemOptions.TimeOptions.LastWeek:
                            DateTime lastWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                            lastWeek = lastWeek.AddDays(-7);
                            crop = new BinaryOperator("Item.InsertedDate", lastWeek, BinaryOperatorType.GreaterOrEqual);
                            break;
                        case NewItemOptions.TimeOptions.LastMonth:
                            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                            crop = new BinaryOperator("Item.InsertedDate", lastMonth, BinaryOperatorType.GreaterOrEqual);
                            break;
                        case NewItemOptions.TimeOptions.TimePeriod:
                            DateTime today;
                            if (DateTime.TryParse(Request["inserted_after"], out today))
                            {
                                DateTime after = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                                crop = new BinaryOperator("Item.InsertedDate", after, BinaryOperatorType.GreaterOrEqual);
                            }
                            else
                            {
                                Session["Error"] = Resources.PleaseSelectDates;
                                return PartialView("NewItems", Session["newItems"]);
                            }
                            break;
                        default:
                            break;
                    }

                    //List<PriceCatalogDetail> freshItems = PriceCatalogHelper.GetTreePriceCatalogDetails(documentHeader.PriceCatalog, CriteriaOperator.And(crop,
                    //                                        DocumentHelper.DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(documentHeader))).Where(x => x.Item.ItemBarcodes.Count > 0).ToList();
                    List<PriceCatalogDetail> freshItems = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(documentHeader.Store, CriteriaOperator.And(crop,
                                                            DocumentHelper.DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(documentHeader))).Where(x => x.Item.ItemBarcodes.Count > 0).ToList();

                    Session["freshItemsPriceCatalogDetails"] = freshItems;
                }
            }

            bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;

            List<SelectedItemsQty> selected_items_qty = Session["selected_fresh_items_qty"] != null && (Session["selected_fresh_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_fresh_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(documentHeader);

            if (Session["freshItemsPriceCatalogDetails"] != null)
            {
                var qtys = from obj in Session["freshItemsPriceCatalogDetails"] as List<PriceCatalogDetail>
                           join itm in selected_items_qty on obj.Item.Oid equals itm.item.Oid into gj
                           from subset in gj.DefaultIfEmpty()
                           select new
                           {
                               item_oid = obj.Item.Oid,
                               item_description = obj.Item.Name,
                               item_code = obj.Item.Code,
                               item_inserted = obj.Item.InsertedDate,
                               Value = ItemHelper.GetItemPriceWithoutTax(obj),
                               Qty = (subset == null ? .0m : subset.qty),
                               pack_measurement_unit = (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit : obj.Item.PackingMeasurementUnit) : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit),
                               packing_qty = (subset == null ? 0 : (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? (subset == null ? .0m : subset.qty) : (decimal)obj.Item.PackingQty * (subset == null ? .0m : subset.qty)) : (subset == null ? .0m : subset.qty))),
                               measurement_unit = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner) == null ? null : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit,
                               order_qty = (subset == null ? .0m : subset.order_qty),
                               item_checked = (subset == null ? .0m : subset.qty) > 0,
                               item_support_decimal = obj.Barcode.MeasurementUnit(documentHeader.Owner) == null ? false : obj.Barcode.MeasurementUnit(documentHeader.Owner).SupportDecimal
                           };
                var qtysToList = qtys.ToList();
                Session["newItems"] = qtysToList;
                Session["selected_fresh_items_qty"] = selected_items_qty;
                ViewData["freshItems"] = selected_items_qty.Where(g => g.qty > 0).Count() > 0;
            }
            return PartialView("NewItems");
        }

        public ActionResult NewItemsFilters()
        {
            return PartialView();
        }

        public ActionResult UpdateNewItemsQty()
        {
            Guid item_oid = Request["item_oid"] == null || Request["item_oid"] == "null" || String.IsNullOrEmpty(Request["item_oid"]) ? Guid.Empty : Guid.Parse(Request["item_oid"]);
            string qty_string = Request["qty_spin_edit"] == null || Request["qty_spin_edit"] == "null" || String.IsNullOrEmpty(Request["qty_spin_edit"]) ? ".0" : Request["qty_spin_edit"];
            decimal qty;
            DocumentHeader documentHeader = Session["currentDocument"] as DocumentHeader;
            bool SupportsPacking = documentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;


            if (!decimal.TryParse(qty_string, out qty))//IE hack
            {
                Session["Error"] = Resources.InvalidItemQty;
                return null;
            }
            qty /= DocumentHelper.QUANTITY_MULTIPLIER;

            List<SelectedItemsQty> selected_fresh_items_qty = Session["selected_fresh_items_qty"] != null && (Session["selected_fresh_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_fresh_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(Session["currentDocument"] as DocumentHeader);

            bool add_to_list = true;
            if (Session["selected_fresh_items_qty"] != null && item_oid != Guid.Empty)
            {
                var f = selected_fresh_items_qty.Find(x => x.item.Oid == item_oid);
                if (f != null)
                {
                    f.qty = qty;
                    add_to_list = false;
                }
            }

            if (add_to_list && qty > 0)
            {
                SelectedItemsQty temp_item = new SelectedItemsQty() { item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", item_oid, BinaryOperatorType.Equal)), qty = qty };
                selected_fresh_items_qty.Add(temp_item);
            }
            List<PriceCatalogDetail> freshItems = Session["freshItemsPriceCatalogDetails"] as List<PriceCatalogDetail>;

            var qtys = from obj in freshItems
                       join itm in selected_fresh_items_qty on obj.Item.Oid equals itm.item.Oid into gj
                       from subset in gj.DefaultIfEmpty()
                       select new
                       {
                           item_oid = obj.Item.Oid,
                           item_description = obj.Item.Name,
                           item_code = obj.Item.Code,
                           item_inserted = obj.Item.InsertedDate,
                           Value = ItemHelper.GetItemPriceWithoutTax(obj),
                           Qty = (subset == null ? .0m : subset.qty),
                           pack_measurement_unit = (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit : obj.Item.PackingMeasurementUnit) : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit),
                           packing_qty = (subset == null ? 0 : (SupportsPacking ? (obj.Item.PackingQty <= 0 || obj.Item.PackingMeasurementUnit == null || obj.Item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit ? (subset == null ? .0m : subset.qty) : (decimal)obj.Item.PackingQty * (subset == null ? .0m : subset.qty)) : (subset == null ? .0m : subset.qty))),
                           measurement_unit = BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner) == null ? null : BOItemHelper.GetTaxCodeBarcode(documentHeader.Session as UnitOfWork, obj.Item, documentHeader.Owner).MeasurementUnit,
                           order_qty = (subset == null ? .0m : subset.order_qty),
                           item_checked = (subset == null ? .0m : subset.qty) > 0,
                           item_support_decimal = obj.Barcode.MeasurementUnit(documentHeader.Owner) == null ? false : obj.Barcode.MeasurementUnit(documentHeader.Owner).SupportDecimal
                       };
            var qtysToList = qtys.ToList();
            Session["newItems"] = qtysToList;
            Session["selected_fresh_items_qty"] = selected_fresh_items_qty;
            ViewData["freshItems"] = selected_fresh_items_qty.Where(g => g.qty > 0).Count() > 0;
            return PartialView("NewItems");
        }

        protected string OrderFreshItems()
        {
            int newItemsInOrder = 0;

            List<SelectedItemsQty> selected_fresh_items_qty = Session["selected_fresh_items_qty"] != null && (Session["selected_fresh_items_qty"] as List<SelectedItemsQty>).Count > 0
                    ? Session["selected_fresh_items_qty"] as List<SelectedItemsQty>
                    : DocumentHelper.SelectedItemsTotalQuantity(Session["currentDocument"] as DocumentHeader);
            DocumentHeader currentDocumentHeader = ((DocumentHeader)Session["currentDocument"]);

            IEnumerable<SelectedItemsQty> new_added_items = selected_fresh_items_qty.Where(item => item.qty > 0).ToList();

            if (DocumentHelper.CheckIfMaximumLinesLimitWillExceed(currentDocumentHeader, new_added_items.Count()))
            {
                return Resources.MaxCountOfDocLinesExceeded;
            }

            foreach (SelectedItemsQty new_order in new_added_items)
            {
                try
                {
                    Item the_item = currentDocumentHeader.Session.FindObject<Item>(new BinaryOperator("Oid", new_order.item.Oid, BinaryOperatorType.Equal));
                    PriceCatalogDetail priceCatalogDetail = new_order.PriceCatalogDetail(currentDocumentHeader.Store, currentDocumentHeader.Customer);
                    Barcode the_barcode = currentDocumentHeader.Session.FindObject<Barcode>(new BinaryOperator("Oid", priceCatalogDetail.Barcode.Oid, BinaryOperatorType.Equal));
                    DocumentDetail tempDocumentDetail = DocumentHelper.ComputeDocumentLine(ref currentDocumentHeader, the_item, the_barcode, new_order.qty, false, -1, false, "", null);
                    tempDocumentDetail.IsActive = true;
                    DocumentHelper.SetDocumentHeaderValuesToZero(ref currentDocumentHeader);
                    DocumentHelper.AddItem(ref currentDocumentHeader, tempDocumentDetail);

                    if (currentDocumentHeader.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING && !(the_item.PackingQty <= 0 || the_item.PackingMeasurementUnit == null || the_item.PackingMeasurementUnit == BOItemHelper.GetTaxCodeBarcode(currentDocumentHeader.Session as UnitOfWork, the_item, currentDocumentHeader.Owner).MeasurementUnit))
                    {
                        new_order.order_qty += new_order.qty * (decimal)the_item.PackingQty;
                    }
                    else
                    {
                        new_order.order_qty += new_order.qty;
                    }
                    new_order.qty = 0;
                    newItemsInOrder++;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }

                if (Session["selected_items_qty"] != null)
                {
                    UpdateList(selected_fresh_items_qty, Session["selected_items_qty"] as List<SelectedItemsQty>);
                }
                if (Session["selected_offers_qty"] != null)
                {
                    UpdateList(selected_fresh_items_qty, Session["selected_offers_qty"] as List<SelectedItemsQty>);
                }
            }
            Session["Notice"] = String.Format("{0} {1}", newItemsInOrder, Resources.SuccesfullyAddedItems);
            DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
            SaveObjectTemp(currentDocumentHeader, CurrentUser);
            return string.Empty;
        }

        [Security(ReturnsPartial = false, DontLogAction = true)]
        public FileContentResult ShowImage()
        {
            if (Session["currentDocumentDetail"] == null)
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/img/no_image.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }
            else
            {
                DocumentDetail tempDocumentDetail = Session["currentDocumentDetail"] as DocumentDetail;
                Image im = tempDocumentDetail.Item.ImageSmall;
                if (im != null)
                {
                    ImageConverter converter = new ImageConverter();

                    byte[] imageBytes = (byte[])converter.ConvertTo(im, typeof(byte[]));
                    string format = "";

                    if (im.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        format = "jpeg";
                    }
                    else if ((im.RawFormat.Equals(ImageFormat.Gif)))
                    {
                        format = "gif";
                    }
                    else if ((im.RawFormat.Equals(ImageFormat.Png)))
                    {
                        format = "png";
                    }

                    return new FileContentResult(imageBytes, "image/" + format);
                }
                else
                {
                    Image defaultImage = Image.FromFile(Server.MapPath("~/Content/img/no_image.png"));
                    ImageConverter converter = new ImageConverter();

                    byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                    return new FileContentResult(imageBytes, "image/gif");
                }
            }
        }

        public ActionResult SearchCustomer()
        {
            return PartialView("SelectCustomer");
        }

        public JsonResult jsonGetCurrentStore()
        {
            return Json(new { IsLoggedIn = (this.CurrentStore != null), CanOrderFromSelectedStore = CanOrderFromSelectedStore() });
        }

        private bool CanOrderFromSelectedStore()
        {
            if (this.CurrentStore == null)
            {
                Session["Error"] = Resources.PleaseSelectAStore;
            }
            else
            {
                Store store = XpoSession.GetObjectByKey<Store>(this.CurrentStore.Oid);
                if (store.DocumentSeries.Count == 0)
                {
                    Session["Error"] = Resources.StoreHasNoSeries;
                }
                else if (StoreHelper.StoreMissesDocumentType(store, DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance)))
                {
                    Session["Error"] = Resources.StoreHasNoSeriesTypes;
                }

                bool defaultOrderHasBeenAsked = false;
                bool.TryParse(Request["defaultOrderHasBeenAsked"], out defaultOrderHasBeenAsked);
                if ((Session["Error"] == null || String.IsNullOrEmpty(Session["Error"].ToString()))
                    && defaultOrderHasBeenAsked
                    )
                {
                    CriteriaOperator ownerCriteria = new BinaryOperator("IsDefault", true);
                    ownerCriteria = ApplyOwnerCriteria(ownerCriteria, typeof(DocumentType), EffectiveOwner);
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    DocumentType documentType = uow.FindObject<DocumentType>(ownerCriteria);
                    if (documentType == null)
                    {
                        ownerCriteria = null;
                        ownerCriteria = ApplyOwnerCriteria(ownerCriteria, typeof(DocumentType), EffectiveOwner);
                        documentType = uow.FindObject<DocumentType>(ownerCriteria);
                    }

                    if (documentType == null)
                    {
                        Session["Error"] = Resources.PleaseSelectDefaultDocumentType;
                    }
                    else
                    {
                        CriteriaOperator criteria = CriteriaOperator.And(new ContainsOperator("StoreDocumentSeriesTypes", new BinaryOperator("DocumentType.Oid", documentType.Oid)),
                            new BinaryOperator("Store.Oid", store.Oid));
                        if (uow.FindObject<DocumentSeries>(criteria) == null)
                        {
                            Session["Error"] = Resources.PleaseSelectSeriesDefaultDocumentTypeInCurrentStore;
                        }

                        if ((Session["Error"] == null || String.IsNullOrEmpty(Session["Error"].ToString()))
                            && UserHelper.IsCustomer(CurrentUser) == false
                            && DocumentHelper.CurrentUserDocumentView(CurrentUser, documentType).Equals(eDocumentTypeView.Simple)
                           )
                        {
                            Session["Error"] = Resources.PleaseReviseDocumentTypeSettingsDocumentViewForm;
                        }
                    }
                }
            }

            return (Session["Error"] == null || Session["Error"].Equals(""));
        }

        public JsonResult jsonRecalculateDocumentLine(string documentDetail)
        {
            DocumentHeader currentDocumentHeader = (Session["currentDocument"] as DocumentHeader);
            Guid docDetailGuid = Guid.Empty;
            if (Guid.TryParse(documentDetail, out docDetailGuid))
            {
                DocumentDetail oldDocumentDetail = (Session["currentDocument"] as DocumentHeader).DocumentDetails.Where(docline => docline.Oid == docDetailGuid).First();
                if (oldDocumentDetail != null)
                {
                    DocumentDetail newDocumentDetail = DocumentHelper.ComputeDocumentLine(
                                                                                                    ref currentDocumentHeader,
                                                                                                    oldDocumentDetail.Item,
                                                                                                    oldDocumentDetail.Barcode,
                                                                                                    oldDocumentDetail.Qty,
                                  oldDocumentDetail.LinkedLine != Guid.Empty,
                                                                                                    -1,
                                                                                                    false,
                                                                                                    oldDocumentDetail.CustomDescription,
                                                                                                    oldDocumentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource != eDiscountSource.PRICE_CATALOG)
                                                                                                );


                    DocumentHelper.UpdateLinkedItems(ref currentDocumentHeader, newDocumentDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
                    SaveObjectTemp(currentDocumentHeader, CurrentUser);
                }
            }
            return Json(new { });
        }

        public ActionResult TransformationDocumentTypes()
        {
            return PartialView();
        }

        public ActionResult TransformationDocumentSeries()
        {
            DocumentSeries selectedDocumentSeries = null;
            IEnumerable<DocumentSeries> docSeries = new List<DocumentSeries>();

            if (String.IsNullOrEmpty(Request["DocumentType"]) == false)
            {
                Guid docTypeGuid;
                if (Guid.TryParse(Request["DocumentType"], out docTypeGuid))
                {
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    DocumentType documentType = uow.GetObjectByKey<DocumentType>(docTypeGuid);
                    eModule module = DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance);
                    Store store = uow.GetObjectByKey<Store>(this.CurrentStore.Oid);
                    docSeries = StoreHelper.StoreSeriesForDocumentType(store, documentType, module);
                    if (docSeries.Count() > 0 && (selectedDocumentSeries == null || selectedDocumentSeries != null && !docSeries.Contains(selectedDocumentSeries)))
                    {
                        selectedDocumentSeries = docSeries.First();
                    }
                }
            }

            ViewBag.SelectedDocumentSeries = selectedDocumentSeries;
            return PartialView(docSeries);
        }

        public ActionResult Carousel()
        {
            return PartialView();
        }

        [AjaxOrChildActionOnly]
        public ActionResult ItemImage()
        {
            return PartialView();
        }

        [AjaxOrChildActionOnly]
        public ActionResult ItemInfoPanel()
        {
            Session["Error"] = Session["ErrorFromDocumentDetails"];
            Session["Notice"] = Session["NoticeFromDocumentDetails"];
            Session["NoticeFromDocumentDetails"] = null;
            Session["ErrorFromDocumentDetails"] = null;
            try
            {
                if (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("ADDITEM") >= 0)
                {
                    if (Session["barcode_search"] == null && Session["currentDocumentDetail"] == null)
                    {
                        Session["Error"] = Resources.AnErrorOccurred;
                    }
                    else
                    {//Add Item
                        DocumentHeader currentDocumentHeader = Session["currentDocument"] as DocumentHeader;
                        string errormsg;
                        if (DocumentHelper.MaxCountOfLinesExceeded(currentDocumentHeader, out errormsg))
                        {
                            Session["Error"] = errormsg;
                        }
                        else
                        {
                            DocumentDetail docdet = (Session["currentDocumentDetail"] as DocumentDetail);

                            string custom_description = docdet.CustomDescription;
                            if (String.IsNullOrEmpty(Request["item_info_name"]) == false)
                            {
                                custom_description = Request["item_info_name"];
                            }
                            else if (String.IsNullOrEmpty(custom_description))
                            {
                                custom_description = docdet.Item.Name;
                            }

                            (Session["currentDocumentDetail"] as DocumentDetail).CustomDescription = custom_description;

                            decimal unit_price = docdet.CustomUnitPrice;
                            if ((Session["currentDocumentDetail"] as DocumentDetail).Item.AcceptsCustomPrice)
                            {
                                if (Decimal.TryParse(Request["custom_price"], out unit_price) == false)
                                {
                                    Session["Error"] = Resources.PleaseSetPrice;
                                    return PartialView();
                                }
                                else
                                {
                                    unit_price /= DocumentHelper.QUANTITY_MULTIPLIER;
                                }
                            }

                            if ((unit_price == 0 && currentDocumentHeader.DocumentType.AllowItemZeroPrices == false)
                            || (currentDocumentHeader.DocumentType.MaxDetailValue > 0 && unit_price > currentDocumentHeader.DocumentType.MaxDetailValue)
                               )
                            {
                                Session["Error"] = Resources.InvalidValue;
                                return PartialView();
                            }

                            DocumentDetail document_detail = DocumentHelper.ComputeDocumentLine(
                                        ref currentDocumentHeader,
                                        docdet.Item,
                                        docdet.Barcode,
                                        Decimal.Parse(Request["spinlineqty"].ToString()) / DocumentHelper.QUANTITY_MULTIPLIER,
                                        false, unit_price, true,
                                        custom_description,
                                        docdet.DocumentDetailDiscounts,
                                        false,
                                        "",
                                        docdet);

                            DocumentHelper.SetDocumentHeaderValuesToZero(ref currentDocumentHeader);
                            DocumentHelper.AddItem(ref currentDocumentHeader, document_detail);
                            DocumentHelper.RecalculateDocumentCosts(ref currentDocumentHeader, false);
                            SaveObjectTemp(currentDocumentHeader, CurrentUser);

                            Session["currentpriceCatalogDetail"] = Session["currentDocumentDetail"] = Session["barcode_search"] = null;
                            if (Session["Error"] == null)
                            {
                                Session["Notice"] = Resources.SuccesfullyAdded;
                            }
                        }
                    }
                    return PartialView();
                }
                else if (Request["DXCallbackArgument"].ToString().ToUpper().Contains("CLEAN"))
                {
                }
                else if (Request["DXCallbackArgument"].ToString().ToUpper().Contains("CLEAN") == false
                    && (Session["barcode_search"] == null && ((DocumentDetail)Session["currentDocumentDetail"]) == null))
                {
                    Session["NoticeFromDocumentDetails"] = Session["Notice"] = Resources.PleaseSelectAnItem;
                }
                else
                {
                    string qty_str = Request["spinlineqty"] == null || Request["spinlineqty"] == "null" || Request["spinlineqty"] == "" ? "0.0" : Request["spinlineqty"];
                    decimal qty;
                    if (Decimal.TryParse(qty_str, out qty))
                    {
                        DocumentHeader documentHeader = ((DocumentHeader)Session["currentDocument"]);
                        qty /= DocumentHelper.QUANTITY_MULTIPLIER;
                        Guid barcodeGuid = Session["barcode_search"] != null ? (Session["barcode_search"] as Barcode).Oid : ((DocumentDetail)Session["currentDocumentDetail"]).Barcode.Oid;

                        Barcode barcode = documentHeader.Session.GetObjectByKey<Barcode>(barcodeGuid);

                        if (qty <= 0)
                        {
                            Session["ErrorFromDocumentDetails"] = Session["Error"] = Resources.InvalidItemQty;
                        }
                        else
                        {
                            //if (qty > documentHeader.DocumentType.MaxDetailQty)
                            //{
                            //    Session["ErrorFromDocumentDetails"] = Session["Error"] = Resources.InvalidItemQty + " " + qty.ToString();
                            //    qty = 1.0m;
                            //}

                            DocumentHeader currentDocumentHeader = (DocumentHeader)Session["currentDocument"];
                            Item item = barcode != null ? ItemHelper.GetItemOfSupplier(barcode.Session, barcode, currentDocumentHeader.Store.Owner) : null;
                            if (Session["currentpriceCatalogDetail"] == null)
                            {
                                Session["Notice"] = Resources.PleaseSelectAnItem;
                            }
                            else
                            {
                                bool actionIsSearch = (Request["DXCallbackArgument"].ToString().ToUpper().IndexOf("SEARCH") >= 0);
                                bool userEnteredCustomPrice = false;
                                string custom_description = (Session["currentpriceCatalogDetail"] as PriceCatalogDetail).Item.Name;
                                if (String.IsNullOrEmpty(Request["item_info_name"]) == false
                                    && (Session["currentpriceCatalogDetail"] as PriceCatalogDetail).Item.AcceptsCustomDescription
                                    )
                                {
                                    custom_description = Request["item_info_name"];
                                }
                                decimal unit_price = (Session["currentpriceCatalogDetail"] as PriceCatalogDetail).GetUnitPrice();
                                if ((Session["currentpriceCatalogDetail"] as PriceCatalogDetail).Item.AcceptsCustomPrice &&
                                    String.IsNullOrEmpty(Request["custom_price"]) == false && !actionIsSearch)
                                {
                                    if (Decimal.TryParse(Request["custom_price"], out unit_price))
                                    {
                                        userEnteredCustomPrice = true;
                                        unit_price /= DocumentHelper.QUANTITY_MULTIPLIER;
                                    }
                                }

                                if (actionIsSearch)
                                {
                                    custom_description = "";
                                    unit_price = -1;
                                }


                                DocumentDetail document_detail = DocumentHelper.ComputeDocumentLine(
                                                                ref currentDocumentHeader, item, barcode, qty,
                                                                false, unit_price, userEnteredCustomPrice, custom_description,
                                                                null);

                                DocumentHelper.SetDocumentHeaderValuesToZero(ref currentDocumentHeader);
                                document_detail.IsActive = true;
                                Session["currentDocumentDetail"] = document_detail;
                            }
                        }
                    }
                    else
                    {
                        Session["ErrorFromDocumentDetails"] = Session["Error"] = Resources.InvalidItemQty;
                    }
                }
            }
            catch (Exception e)
            {
                Session["ErrorFromDocumentDetails"] = Session["Error"] = System.ExtensionMethods.GetFullMessage(e);
            }
            return PartialView();
        }

        public ActionResult ItemTagCloud(DateTime min, DateTime max)
        {
            Customer customer = ((DocumentHeader)Session["currentDocument"]).Customer;
            Store store = ((DocumentHeader)Session["currentDocument"]).Store;
            XPQuery<DocumentDetail> documentDetails = new XPQuery<DocumentDetail>(XpoHelper.GetNewUnitOfWork());
            XPQuery<DocumentHeader> documentHeaders = new XPQuery<DocumentHeader>(XpoHelper.GetNewUnitOfWork());
            XPQuery<Customer> customers = new XPQuery<Customer>(XpoHelper.GetNewUnitOfWork());
            XPQuery<Item> items = new XPQuery<Item>(XpoHelper.GetNewUnitOfWork());
            XPQuery<Store> stores = new XPQuery<Store>(XpoHelper.GetNewUnitOfWork());

            var firstQuery = (from docdetail in documentDetails
                              join dochead in documentHeaders on docdetail.DocumentHeader.Oid equals dochead.Oid
                              join cust in customers on dochead.Customer.Oid equals cust.Oid
                              join item in items on docdetail.Item.Oid equals item.Oid
                              join str in stores on dochead.Store.Oid equals str.Oid

                              where dochead.FinalizedDate >= min && dochead.FinalizedDate <= max
                              && cust.Oid == customer.Oid && item.IsActive == true
                              && str.Owner.Oid == store.Owner.Oid && item.IsActive == true
                              group docdetail by docdetail.Item into g
                              orderby g.Count() descending
                              select g).Take(100).Select(g => new { item = g.Key, frequency = g.Count() }).ToList();

            IEnumerable<ObjectFrequency<Item>> filteredDocumentDetailsFromDB = firstQuery.Select(g => new ObjectFrequency<Item>(g.item, g.item.Owner.Oid) { Frequency = g.frequency });
            IEnumerable<ObjectFrequency<Item>> en1 = filteredDocumentDetailsFromDB.Where(g => PriceCatalogHelper.GetUnitPrice(store, customer, g.Item.Code) >= 0).Take(30);

            return PartialView("ItemTagCloud", en1.OrderByDescending(g => g.Item.InsertedDate));
        }

        public ActionResult ItemTagCloudLastTwoMonths()
        {
            DateTime now, twoMonthsBack;
            now = DateTime.Now;
            twoMonthsBack = DateTime.Now.AddMonths(-2);
            ViewBag.CallbackRouteValues = new { Controller = "Document", Action = "ItemTagCloudLastTwoMonths" };
            ViewBag.Name = "ItemTagCloudLastTwoMonths";
            ViewBag.TagCloudHeader = Resources.RecentProducts;
            return ItemTagCloud(twoMonthsBack, now);
        }

        public ActionResult ItemTagCloudLastYearTwoMonths()
        {
            DateTime now, twoMonthsBack;
            now = DateTime.Now.AddYears(-1);
            twoMonthsBack = DateTime.Now.AddYears(-1).AddMonths(-2);
            ViewBag.CallbackRouteValues = new { Controller = "Document", Action = "ItemTagCloudLastYearTwoMonths" };
            ViewBag.Name = "ItemTagCloudLastYearTwoMonths";
            ViewBag.TagCloudHeader = Resources.SuggestedProducts;
            return ItemTagCloud(twoMonthsBack, now);
        }

        public ActionResult OrderItems()
        {
            if (Session["currentDocument"] == null)
            {
                return null;
            }

            Session["DocumentDetail2Edit"] = null;
            Session["currentDocumentDetail"] = null;
            Session["currentPriceCatalogDetail"] = null;



            bool addDocumentDetail = false;
            Boolean.TryParse(Request["addDocumentDetail"], out addDocumentDetail);
            ViewBag.EditOneItem = (Session["DocumentDetail2Edit"] != null);

            if (ViewBag.EditOneItem == false)
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(uow);
                ViewBag.DocumentPayment = GetList<PaymentMethod>(uow);
            }

            Dictionary<string, string> requestedValues = new Dictionary<string, string>();
            requestedValues["docType"] = Request["DocumentType"] ?? Request["docType"];
            requestedValues["docSeries"] = Request["DocumentSeries"] ?? Request["docSeries"];
            requestedValues["docNumber"] = Request["DocNumber"] ?? Request["docNumber"];
            requestedValues["docFinDate"] = Request["FinalizedDate"] ?? Request["docFinDate"];
            requestedValues["docStatus"] = Request["Status"] ?? Request["docStatus"];
            requestedValues["docChecked"] = Request["HasBeenChecked"] ?? Request["docChecked"];
            requestedValues["docExecuted"] = Request["HasBeenExecuted"] ?? Request["docExecuted"];
            requestedValues["docInvoiceDate"] = Request["docInvoiceDate"];
            requestedValues["docDelAddr"] = Request["DeliveryAddress"] ?? Request["AllCustomerAddresses"] ?? Request["docDelAddr"];
            requestedValues["docRemarks"] = Request["documentcomments"] ?? Request["docRemarks"];
            requestedValues["docTransferMethod"] = Request["TransferMethod"] ?? Request["docTransferMethod"];
            requestedValues["docPlaceOfLoading"] = Request["PlaceOfLoading"] ?? Request["docPlaceOfLoading"];
            requestedValues["docVehicleNumber"] = Request["VehicleNumber"] ?? Request["docVehicleNumber"];
            requestedValues["docTransferPurpose"] = Request["TransferPurpose"] ?? Request["docTransferPurpose"];
            requestedValues["docExecutionDate"] = Request["docExecutionDate"];
            requestedValues["docTriangularCustomer"] = Request["docTriangularCustomer"];
            requestedValues["docTriangularSupplier"] = Request["docTriangularSupplier"];
            requestedValues["docTriangularStore"] = Request["docTriangularStore"];
            requestedValues["Tablet"] = Request["docTablet"] ?? Request["Tablet"];
            UpdateDocumentHeaderValues(requestedValues);

            InitializeMultipleOrders();
            InitializeByOfferOrders();
            InitializeNewItems();

            ViewBag.sliderImages = new Dictionary<Guid, string>();

            List<CriteriaOperator> priceCatalogFilters = UserHelper.GetUserCustomerPriceCatalogsFilter(
                (Session["currentDocument"] as DocumentHeader).Customer,
                (Session["currentDocument"] as DocumentHeader).Store,
                "PriceCatalog.Oid");


            CriteriaOperator offersCrop = CriteriaOperator.And(
            new ContainsOperator("OfferDetails", new BinaryOperator("Item.IsActive", true))
            , CriteriaOperator.Or(priceCatalogFilters),
            new BinaryOperator("EndDate", DateTime.Now, BinaryOperatorType.GreaterOrEqual));
            XPCollection<Offer> activeOffers = GetList<Offer>(XpoSession, offersCrop);
            ViewBag.sliderImages = null;

            try
            {
                ViewBag.sliderImages = activeOffers.SelectMany(x => x.OfferDetails).GroupBy(x => x.Item)
                    .Where(group => group.Key.ImageMedium != null && group.Key.ImageMedium.Size.Height > 0)
                    .ToDictionary(group => group.Key.Oid, group => group.Key.Code);
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);

            return PartialView();
        }

        public ActionResult TriangularCallbackPanel()
        {
            return PartialView();
        }

        public ActionResult TriangularCustomerPartial()
        {
            return PartialView();
        }

        public ActionResult TriangularSupplierPartial()
        {
            return PartialView();
        }

        public ActionResult TriangularAddressPartial()
        {
            return PartialView();
        }

        public static object StoreRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),
                                                            CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)), "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public JsonResult AddRemainingPaymentAmount()
        {
            DocumentHeader document = (DocumentHeader)Session["currentDocument"];

            if (document == null)
            {
                Session["Error"] = Resources.ConnectionTimeOut;
                return Json(new { success = false });
            }

            decimal remainingPayment = document.RemainingPayment;
            DocumentPayment currentDocumentPayment = Session["currentDocumentPayment"] as DocumentPayment;

            if (currentDocumentPayment != null && Session["currentDocumentPaymentIsNew"] != null && (bool)Session["currentDocumentPaymentIsNew"] == false)
            {
                remainingPayment += currentDocumentPayment.Amount;
            }

            if (document.Division == eDivision.Financial)
            {
                DocumentHelper.SetFinancialDocumentDetail(document);
            }
            return Json(new { success = true, remainingPaymentAmount = remainingPayment });
        }

        public ActionResult SelectDocumentStore()
        {
            return PartialView();
        }

        public JsonResult NumberOfOrderDocumentTypesDefined()
        {
            StoreViewModel storeViewModel = Session["currentStore"] as StoreViewModel;
            if (storeViewModel == null)
            {
                return Json(new { error = Resources.PleaseSelectAStore });
            }
            Store store = XpoSession.GetObjectByKey<Store>(storeViewModel.Oid);
            XPCollection<StoreDocumentSeriesType> orderStoreDocumentSeriesTypes = DocumentHelper.GetOrderStoreDocumentSeriesTypes(XpoSession, store);
            int numberOfOrderDocumentTypesDefined = orderStoreDocumentSeriesTypes.Count;
            string storeDocumentSeriesType = string.Empty;
            if (numberOfOrderDocumentTypesDefined == 1)
            {
                storeDocumentSeriesType = orderStoreDocumentSeriesTypes.First().DocumentType.Oid.ToString();
            }
            return Json(new { numberOfOrderDocumentTypesDefined = numberOfOrderDocumentTypesDefined, storeDocumentSeriesType = storeDocumentSeriesType });
        }

        public ActionResult SelectOrderDocumentType()
        {
            PrepareOrderDocumentTypes();
            return PartialView();
        }

        private void PrepareOrderDocumentTypes()
        {
            StoreViewModel currentStore = Session["currentStore"] as StoreViewModel;
            if (currentStore == null)
            {
                ViewBag.StoreDocumentSeriesTypesComboBox = null;
            }
            else
            {
                ViewBag.StoreDocumentSeriesTypesComboBox = DocumentHelper.GetOrderStoreDocumentSeriesTypes(this.XpoSession, this.XpoSession.GetObjectByKey<Store>(currentStore.Oid));
            }
        }

        public ActionResult CustomerPoints()
        {
            this.ToolbarOptions.EditButton.Visible = false;
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.ToolbarOptions.NewButton.Visible = false;
            this.ToolbarOptions.OptionsButton.Visible = true;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.PrintButton.Visible = true;
            this.ToolbarOptions.PrintButton.OnClick = "PrintSelectedDocuments";

            this.CustomJSProperties.AddJSProperty("gridName", "grdDocument");
            this.CustomJSProperties.AddJSProperty("DocumentsDivision", EffectiveOwner.OwnerApplicationSettings.PointsDocumentType.Division.Section);

            ViewBag.Title = Resources.PreviewCustomerPoints;
            return View();
        }

        public ActionResult TransactionCoupons()
        {
            return PartialView();
        }

        public JsonResult RemoveUnavailableItem()
        {
            try
            {
                ((List<string>)Session["notIncludedDetailsOids"]).Remove(Request["itemOid"]);
                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }
        }

        public ActionResult UpdatedByInfoPanel()
        {
            ViewBag.InViewMode = false;
            return PartialView();
        }

        public JsonResult jsonUpdateTriangularAddress(string senderName, Guid? senderOid)
        {
            try
            {
                DocumentHeader document = (DocumentHeader)Session["currentDocument"];
                string triangularAddress = "";
                if (senderOid.HasValue)
                {
                    switch (senderName)
                    {
                        case "TriangularCustomer":
                            Customer triangularCustomer = document.Session.GetObjectByKey<Customer>(senderOid);
                            document.TriangularCustomer = triangularCustomer;
                            if (triangularCustomer != null)
                            {
                                triangularAddress = triangularCustomer.DefaultAddress != null ? triangularCustomer.DefaultAddress.Description : "";
                            }
                            break;
                        case "TriangularSupplier":
                            SupplierNew triangularSupplier = document.Session.GetObjectByKey<SupplierNew>(senderOid);
                            document.TriangularSupplier = triangularSupplier;
                            if (triangularSupplier != null)
                            {
                                triangularAddress = triangularSupplier.DefaultAddress != null ? triangularSupplier.DefaultAddress.Description : "";
                            }
                            break;
                        case "TriangularStore":
                            Store triangularStore = document.Session.GetObjectByKey<Store>(senderOid);
                            document.TriangularStore = triangularStore;
                            if (triangularStore != null)
                            {
                                triangularAddress = triangularStore.Address != null ? triangularStore.Address.Description : "";
                            }
                            break;
                        default:
                            break;
                    }
                }
                return Json(new { triangularAddress = triangularAddress });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }
        }

        private void SetViewTitle(eDivision division, bool isProforma)
        {
            string title = string.Empty;

            if (isProforma)
            {
                title = Resources.ProformaInvoices;
            }
            else if (Request.Path.Contains("Document/CustomerPoints"))
            {
                title = Resources.PreviewCustomerPoints;
            }
            else
            {
                switch (division)
                {
                    case eDivision.Financial:
                        title = Resources.FinancialDocumentList;
                        break;
                    case eDivision.Other:
                        title = Resources.OtherDocumentList;
                        break;
                    case eDivision.Purchase:
                        title = Resources.PurchaseDocumentList;
                        break;
                    case eDivision.Sales:
                        title = Resources.SalesDocumentList;
                        break;
                    case eDivision.Store:
                        title = Resources.StoreDocumentList;
                        break;
                    default:
                        break;
                }
            }
            ViewBag.Title = title;
        }

        private void SetDocumentTraderViewBags(DocumentHeader documentHeader)
        {
            switch (documentHeader.Division)
            {
                case eDivision.Financial:

                    if (documentHeader.DocumentType == null)
                    {
                        SetDocumentTraderNoneViewBags();
                        return;
                    }
                    switch (documentHeader.DocumentType.TraderType)
                    {
                        case eDocumentTraderType.CUSTOMER:
                            SetDocumentTraderCustomerViewBags(documentHeader);
                            return;
                        case eDocumentTraderType.SUPPLIER:
                            SetDocumentTraderSupplierViewBags(documentHeader);
                            return;
                        case eDocumentTraderType.STORE:
                            SetDocumentTraderStoreViewBags(documentHeader);
                            return;
                    }
                    return;
                case eDivision.Other:
                    SetDocumentTraderNoneViewBags();
                    return;
                case eDivision.Purchase:
                    SetDocumentTraderSupplierViewBags(documentHeader);
                    return;
                case eDivision.Sales:
                    SetDocumentTraderCustomerViewBags(documentHeader);
                    return;
                case eDivision.Store:
                    SetDocumentTraderStoreViewBags(documentHeader);
                    return;
                default:
                    //should be unreachable code
                    break;
            }
        }

        private void SetDocumentTraderNoneViewBags()
        {
            ViewBag.model = null;
            ViewBag.name = "undefined";
            ViewBag.caption = String.Empty;
            ViewBag.TraderEditForm = String.Empty;
            ViewBag.TraderTabFormName = String.Empty;
        }

        private void SetDocumentTraderStoreViewBags(DocumentHeader documentHeader)
        {
            ViewBag.model = documentHeader.SecondaryStore;
            ViewBag.name = "storeSuppliers";
            ViewBag.caption = Resources.Store;
            ViewBag.TraderEditForm = "SelectDocumentStore";
            ViewBag.TraderTabFormName = String.Empty;//This is because Secondary Store does not have a separate Tab.
        }

        private void SetDocumentTraderCustomerViewBags(DocumentHeader documentHeader)
        {
            ViewBag.model = documentHeader.Customer;
            ViewBag.name = "storeCustomers";
            ViewBag.caption = Resources.Customer;
            if (documentHeader.Customer != null && documentHeader.Customer.Trader != null)
            {
                ViewBag.addresses = documentHeader.Customer.Trader.Addresses;
            }
            ViewBag.TraderEditForm = "SelectCustomer";
            ViewBag.TraderTabFormName = "CustomerInfoPanel";
        }

        private void SetDocumentTraderSupplierViewBags(DocumentHeader documentHeader)
        {
            ViewBag.model = documentHeader.Supplier;
            ViewBag.name = "storeSuppliers";
            ViewBag.caption = Resources.Supplier;
            if (documentHeader.Supplier != null && documentHeader.Supplier.Trader != null)
            {
                ViewBag.addresses = documentHeader.Supplier.Trader.Addresses;
            }
            ViewBag.TraderEditForm = "SelectSupplier";
            ViewBag.TraderTabFormName = "SupplierInfoPanel";
        }

        public ActionResult CompositionDecomposition()
        {
            return PartialView();
        }

        public ActionResult CompositionDecompositionMainLinesGrid()
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);

            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid detailGuid = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    Session["currentDocumentDetail"] = documentHeader.DocumentDetails.Where(detail => detail.Oid == detailGuid).FirstOrDefault();
                }
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["currentDocumentDetail"] = null;
                }
            }
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView(documentHeader.MainLines);
        }

        public ActionResult AddMainDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            DocumentDetail sessionDocumentDetail = Session["currentDocumentDetail"] as DocumentDetail;
            if (sessionDocumentDetail == null)
            {
                ModelState.AddModelError("Item", new Exception(Resources.PleaseSelectAnItem));
                Session["Error"] = Resources.PleaseSelectAnItem;
                return PartialView("CompositionDecompositionMainLinesGrid", documentHeader.MainLines);
            }
            if (documentDetail.Qty <= 0)
            {
                ModelState.AddModelError("Qty", new Exception(Resources.InvalidItemQty));
                Session["Error"] = Resources.InvalidItemQty;
                return PartialView("CompositionDecompositionMainLinesGrid", documentHeader.MainLines);
            }

            try
            {
                if (ModelState.IsValid && sessionDocumentDetail != null)
                {
                    DocumentDetail currentDocumentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader,
                                                       sessionDocumentDetail.Item,
                                                       sessionDocumentDetail.Barcode,
                                                       documentDetail.Qty,
                                                       false,
                                                       0,
                                                       true,
                                                       sessionDocumentDetail.Item.Name,
                                                       null
                                                       );
                    DocumentHelper.AddItem(ref documentHeader, currentDocumentDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
                Session["currentDocumentDetail"] = null;
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }

            return PartialView("CompositionDecompositionMainLinesGrid", documentHeader.MainLines);
        }

        public ActionResult UpdateMainDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            DocumentDetail sessionDocumentDetail = Session["currentDocumentDetail"] as DocumentDetail;
            try
            {
                if (ModelState.IsValid)
                {
                    DocumentDetail currentDocumentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader,
                                                       sessionDocumentDetail.Item,
                                                       sessionDocumentDetail.Barcode,
                                                       documentDetail.Qty,
                                                       false,
                                                       0,
                                                       true,
                                                       sessionDocumentDetail.Item.Name,
                                                       null,
                                                       oldDocumentLine: sessionDocumentDetail
                                                       );
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
                Session["currentDocumentDetail"] = null;
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            return PartialView("CompositionDecompositionMainLinesGrid", documentHeader.MainLines);
        }

        public ActionResult DeleteMainDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            try
            {
                if (ModelState.IsValid)
                {
                    DocumentDetail documentDetailToDelete = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == documentDetail.Oid);
                    DocumentHelper.DeleteItem(ref documentHeader, documentDetailToDelete);
                }
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            return PartialView("CompositionDecompositionMainLinesGrid", documentHeader.MainLines);
        }

        public ActionResult SearchByDescriptionCompositionDecomposition()
        {
            ViewBag.DocumentViewForm = DocumentHelper.CurrentUserDocumentView(CurrentUser, ((DocumentHeader)(Session["currentDocument"])).DocumentType);
            return PartialView();
        }

        public ActionResult CompositionDecompositionLinkedLinesGrid()
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            ViewBag.Title = Resources.LineDetails + " " + Resources.PleaseSelectAnItem;

            Guid mainLineGuid = Guid.Empty;
            if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                mainLineGuid = (Guid)Session["mainLine"];
            }
            else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                mainLineGuid = (Guid)Session["mainLine"];
            }
            else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
            {
                Session["currentDocumentDetail"] = null;
                mainLineGuid = (Guid)Session["mainLine"];
                DocumentDetail selectedMainDocumentDetail = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == mainLineGuid);
                SetLinkedLinesGridTitle(selectedMainDocumentDetail);
                return PartialView(selectedMainDocumentDetail.LinkedLines);
            }
            else
            {
                Session["mainLine"] = null;
                Guid.TryParse(Request["mainLine"], out mainLineGuid);
            }

            if (mainLineGuid != Guid.Empty)
            {

                if (documentHeader == null)
                {//Session["currentDocument"] == null when in View Model
                    documentHeader = XpoSession.GetObjectByKey<DocumentDetail>(mainLineGuid).DocumentHeader;
                    ViewData["currentDocument"] = documentHeader;
                }
                DocumentDetail documentDetail = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == mainLineGuid);
                if (documentDetail != null)
                {
                    Session["mainLine"] = documentDetail.Oid;
                    SetLinkedLinesGridTitle(documentDetail);
                    return PartialView(documentDetail.LinkedLines);
                }
            }
            return PartialView(null);
        }

        public ActionResult AddLinkedDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            DocumentDetail mainLine = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == (Guid)Session["mainLine"]);
            DocumentDetail sessionDocumentDetail = Session["currentDocumentDetail"] as DocumentDetail;
            if (sessionDocumentDetail == null)
            {
                ModelState.AddModelError("Item", new Exception(Resources.PleaseSelectAnItem));
                Session["Error"] = Resources.PleaseSelectAnItem;
                return PartialView("CompositionDecompositionLinkedLinesGrid", mainLine.LinkedLines);
            }
            if (documentDetail.Qty <= 0)
            {
                ModelState.AddModelError("Qty", new Exception(Resources.InvalidItemQty));
                Session["Error"] = Resources.InvalidItemQty;
                return PartialView("CompositionDecompositionLinkedLinesGrid", mainLine.LinkedLines);
            }
            try
            {
                Session["currentDocumentDetail"] = null;
                if (sessionDocumentDetail != null && mainLine.Item.Oid == sessionDocumentDetail.Item.Oid)
                {
                    Session["Error"] = Resources.MainItemAndLinkedItemCannotMatch;
                }
                else if (ModelState.IsValid && sessionDocumentDetail != null)
                {
                    DocumentDetail currentDocumentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader,
                                                       sessionDocumentDetail.Item,
                                                       sessionDocumentDetail.Barcode,
                                                       documentDetail.Qty,
                                                       true,
                                                       0,
                                                       true,
                                                       sessionDocumentDetail.Item.Name,
                                                       null
                                                       );
                    currentDocumentDetail.LinkedLine = mainLine.Oid;
                    DocumentHelper.AddItem(ref documentHeader, currentDocumentDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
                SetLinkedLinesGridTitle(mainLine);
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            return PartialView("CompositionDecompositionLinkedLinesGrid", mainLine.LinkedLines);
        }

        public ActionResult UpdateLinkedDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            DocumentDetail mainLine = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == (Guid)Session["mainLine"]);
            DocumentDetail documentDetailToEdit = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == documentDetail.Oid);
            try
            {
                if (ModelState.IsValid && documentDetailToEdit != null)
                {
                    DocumentDetail finalDocumentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader,
                                                       documentDetailToEdit.Item,
                                                       documentDetailToEdit.Barcode,
                                                       documentDetail.Qty,
                                                       true,
                                                       0,
                                                       true,
                                                       documentDetailToEdit.Item.Name,
                                                       null,
                                                       oldDocumentLine: documentDetailToEdit
                                                       );
                    finalDocumentDetail.LinkedLine = mainLine.Oid;
                    DocumentHelper.ReplaceItem(ref documentHeader, documentDetailToEdit, finalDocumentDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            SetLinkedLinesGridTitle(mainLine);
            return PartialView("CompositionDecompositionLinkedLinesGrid", mainLine.LinkedLines);
        }

        public ActionResult DeleteLinkedDocumentDetail([ModelBinder(typeof(RetailModelBinder))] DocumentDetail documentDetail)
        {
            DocumentHeader documentHeader = (DocumentHeader)(Session["currentDocument"]);
            DocumentDetail currentDocumentDetail = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == documentDetail.Oid);
            try
            {
                if (ModelState.IsValid && currentDocumentDetail != null)
                {
                    DocumentHelper.DeleteItem(ref documentHeader, currentDocumentDetail);
                    DocumentHelper.RecalculateDocumentCosts(ref documentHeader, false);
                }
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.Message;
            }
            DocumentDetail mainLine = documentHeader.DocumentDetails.FirstOrDefault(detail => detail.Oid == (Guid)Session["mainLine"]);
            SetLinkedLinesGridTitle(mainLine);
            return PartialView("CompositionDecompositionLinkedLinesGrid", mainLine.LinkedLines);
        }

        private void SetLinkedLinesGridTitle(DocumentDetail documentDetail)
        {
            ViewBag.Title = Resources.LineDetails + ":" + documentDetail.Item.Name + " (" + documentDetail.Item.Code + " , " + documentDetail.Barcode.Code + ")";
        }
    }
}
