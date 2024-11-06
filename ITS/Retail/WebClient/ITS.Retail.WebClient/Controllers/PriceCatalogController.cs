using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Kernel;
using System.Threading;
using ITS.Retail.Platform.Enumerations;
using System.Globalization;
using ITS.Retail.Platform;
using ITS.Retail.Common.ViewModel;
using DevExpress.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class PriceCatalogController : BaseObjController<PriceCatalog>
    {

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

        public ActionResult TreeView()
        {
            ViewData["VirtualPriceCatalogsTreeMethod"] = TreeHelper.GetPriceCatalogTreeViewVirtualMethod(EffectiveOwner);
            return PartialView();
        }

        public ActionResult TreeViewCallbackPanel()
        {
            ViewData["VirtualPriceCatalogsTreeMethod"] = TreeHelper.GetPriceCatalogTreeViewVirtualMethod(EffectiveOwner);
            return PartialView();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ForceVisible = false;
            GenerateUnitOfWork();

            User currentUser = CurrentUser;

            ViewData["IsAdministrator"] = UserHelper.IsSystemAdmin(currentUser);
            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);
            Customer userCustomer = UserHelper.GetCustomer(currentUser);
            ViewData["IsSupplier"] = userSupplier != null;
            ViewData["IsCustomer"] = userCustomer != null;

            if (Boolean.Parse(Session["IsAdministrator"].ToString()) || Boolean.Parse(ViewData["IsSupplier"].ToString())) // admin or supplier (TreeView)
            {
                ViewData["VirtualPriceCatalogsTreeMethod"] = TreeHelper.GetPriceCatalogTreeViewVirtualMethod(EffectiveOwner);
            }
            return View("Index");

        }
        public ActionResult PriceCatalogInfoCallbackPanel(string TabPage)
        {
            ViewData["TabPage"] = TabPage;
            GenerateUnitOfWork();
            Guid priceCatalogGuid;
            if (Guid.TryParse(Request["PriceCatalogID"], out priceCatalogGuid))
            {
                PriceCatalog priceCatalog = uow.GetObjectByKey<PriceCatalog>(priceCatalogGuid);
                if (priceCatalog != null)
                {
                    ViewData["Code"] = priceCatalog.Code;
                    ViewData["Description"] = priceCatalog.Description;
                    ViewData["StartDate"] = priceCatalog.StartDate;
                    ViewData["EndDate"] = priceCatalog.EndDate;
                    ViewData["SupportLoyalty"] = priceCatalog.SupportLoyalty;
                    ViewData["IgnoreZeroPrices"] = priceCatalog.IgnoreZeroPrices;
                    ViewData["PriceCatalogID"] = priceCatalogGuid;
                    ViewData["IsEditableAtStore"] = priceCatalog.IsEditableAtStore == null ? "" : priceCatalog.IsEditableAtStore.Description;
                }
            }
            return PartialView();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportPriceCatalogDetail()
        {
            if (Request["PCOid"] == null) { return RedirectToAction("Index", "PriceCatalog"); }
            return RedirectToAction("PriceCatalogDetailReport", "Reports", new { PCOid = Request["PCOid"].ToString() });
        }

        public JsonResult DeletePriceCatalog()
        {
            if (!TableCanDelete) return null;

            try
            {
                GenerateUnitOfWork();
                Guid PriceCatalogID = (Request["PriceCatalogID"] == null || Request["PriceCatalogID"] == "null" || Request["PriceCatalogID"] == "-1") ? Guid.Empty : Guid.Parse(Request["PriceCatalogID"]);
                PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(PriceCatalogID);
                pc.Delete();
                XpoHelper.CommitTransaction(uow);
                return Json(new { Success = "" });
            }
            catch (Exception e)
            {
                Session["Error"] = e.InnerException != null ? e.InnerException.Message : e.Message + Environment.NewLine + e.StackTrace;
                return Json(new { Error = "" });
            }
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditPriceCatalog;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }


        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            return PartialView();

        }

        public ActionResult EditView(string Oid)
        {
            this.CustomJSProperties.AddJSProperty("treeViewName", "priceCatalogTreeViewCbPanel");

            ViewData["EditMode"] = true;

            GenerateUnitOfWork();
            Guid priceCatalogGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            if (priceCatalogGuid == Guid.Empty && TableCanInsert == false)
            {
                Response.Redirect("Index");
            }
            else if (priceCatalogGuid != Guid.Empty && TableCanUpdate == false)
            {
                Response.Redirect("Index");
            }

            Session["currentPriceCatalogID"] = priceCatalogGuid;
            PriceCatalog priceCatalog;
            if (Session["UnsavedPriceCatalog"] == null)
            {
                if (priceCatalogGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditPriceCatalog;
                    priceCatalog = uow.GetObjectByKey<PriceCatalog>(priceCatalogGuid);
                    Session["IsNewPriceCatalog"] = false;
                    if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                      && (priceCatalog.IsEditableAtStore == null || priceCatalog.IsEditableAtStore.Oid != StoreControllerAppiSettings.CurrentStoreOid))
                    {
                        Session["Error"] = Resources.YouCannotEditThisElement;
                        Response.Redirect("Index");
                    }
                }
                else
                {
                    ViewBag.Mode = Resources.AddPriceCatalog;
                    priceCatalog = new PriceCatalog(uow);
                    Session["IsNewPriceCatalog"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (priceCatalogGuid != Guid.Empty && (Session["UnsavedPriceCatalog"] as PriceCatalog).Oid == priceCatalogGuid)
                {
                    Session["IsRefreshed"] = true;
                    priceCatalog = (PriceCatalog)Session["UnsavedPriceCatalog"];
                }
                else if (priceCatalogGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    priceCatalog = (PriceCatalog)Session["UnsavedPriceCatalog"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    priceCatalog = uow.GetObjectByKey<PriceCatalog>(priceCatalogGuid);
                }

            }
            FillLookupComboBoxes();
            ViewData["PriceCatalogID"] = priceCatalog.Oid.ToString();
            ViewData["StorePriceList"] = priceCatalog.StorePriceLists;
            Session["UnsavedPriceCatalog"] = priceCatalog;
            Session["PriceCatalogDetailFilter"] = priceCatalog.PriceCatalogDetails.Filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");

            return PartialView("EditView", priceCatalog);
        }

        public ActionResult PriceCatalogChildsGrid(string PriceCatalogID, bool editMode)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = editMode;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                FillLookupComboBoxes();
                return PartialView("PriceCatalogChildsGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogs);
            }
            else //view mode
            {
                Guid PriceCatalogGuid = PriceCatalogID == null || PriceCatalogID == "null" || PriceCatalogID == "" || PriceCatalogID == "-1" ? Guid.Empty : Guid.Parse(PriceCatalogID);
                PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(PriceCatalogGuid);
                ViewData["PriceCatalogID"] = PriceCatalogID;

                return PartialView("PriceCatalogChildsGrid", pc.PriceCatalogs);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceCatalogChildAddNewPartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalog ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            if (ModelState.IsValid)
            {
                try
                {
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    Guid comboBoxChildPriceCatalogID = Request["ComboBoxChildPriceCatalogID"] != null || Request["ComboBoxChildPriceCatalogID"] != "null" ? Guid.Parse(Request["ComboBoxChildPriceCatalogID"]) : Guid.Empty;
                    PriceCatalog comboBoxChildPriceCatalog = uow.GetObjectByKey<PriceCatalog>(comboBoxChildPriceCatalogID);
                    if (comboBoxChildPriceCatalog.ParentCatalog != null && comboBoxChildPriceCatalog.ParentCatalog.Oid == pc.Oid)
                    {
                        Session["Error"] = Resources.PriceCatalogAlreadyExists;
                    }
                    else
                    {
                        comboBoxChildPriceCatalog.ParentCatalog = pc;
                        Session["UnsavedPriceCatalog"] = pc;
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("PriceCatalogChildsGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogs);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PriceCatalogChildDeletePartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalog ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            try
            {
                Guid ChildPriceCatalogID = ct.Oid;
                PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                foreach (PriceCatalog cpc in pc.PriceCatalogs)
                {
                    if (cpc.Oid == ChildPriceCatalogID)
                    {
                        //pc.PriceCatalogs.Remove(cpc);
                        cpc.ParentCatalog = null;
                        cpc.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("PriceCatalogChildsGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogs);
        }

        public JsonResult Save(PriceCatalogViewModel priceCatalogViewModel)
        {
            GenerateUnitOfWork();
            Guid priceCatalogGuid = Guid.Empty;

            bool correctpriceCatalogGuid = Request["PriceCatalogID"] != null && Guid.TryParse(Request["PriceCatalogID"].ToString(), out priceCatalogGuid);
            if (correctpriceCatalogGuid && ModelState.IsValid)
            {
                PriceCatalog priceCatalog = (PriceCatalog)Session["UnsavedPriceCatalog"];
                if (priceCatalog != null)
                {
                    if ((bool)Session["IsNewPriceCatalog"])
                    {
                        AssignOwner(priceCatalog);
                    }

                    priceCatalog.Code = priceCatalogViewModel.Code;
                    priceCatalog.Description = priceCatalogViewModel.Description;
                    priceCatalog.StartDate = priceCatalogViewModel.StartDate;
                    priceCatalog.EndDate = priceCatalogViewModel.EndDate;
                    priceCatalog.SupportLoyalty = priceCatalogViewModel.SupportLoyalty;
                    priceCatalog.IgnoreZeroPrices = priceCatalogViewModel.IgnoreZeroPrices;
                    priceCatalog.ParentCatalogOid = priceCatalogViewModel.ParentCatalogOid;

                    priceCatalog.IsRoot = priceCatalog.ParentCatalog == null;


                    if (priceCatalog.ParentCatalog != null && priceCatalog.PriceCatalogs.Count > 0)
                    {
                        IEnumerable<Guid> subCatalogs = priceCatalog.PriceCatalogs.Select(subCatalog => subCatalog.Oid);
                        if (subCatalogs.Contains(priceCatalog.ParentCatalog.Oid))
                        {
                            uow.RollbackTransaction();
                            Session["Error"] = Resources.PriceCatalogCannotBeBothParentCatalogAndSubCatalog;
                            return Json(new { error = Session["Error"].ToString() });
                        }
                    }

                    if (MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
                    {
                        Guid isEdditableAtStore = Guid.Empty;
                        Guid.TryParse(Request["IsEditableAtStore_VI"], out isEdditableAtStore);
                        priceCatalog.IsEditableAtStore = priceCatalog.Session.GetObjectByKey<Store>(isEdditableAtStore);
                    }

                    //Error no owner
                    if (priceCatalog.Owner == null)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ": Price Catalog Owner is not defined.";
                        return Json(new { error = Session["Error"] });
                    }

                    #region Validate Time Values
                    PlatformPriceCatalogDetailService platformPriceCatalogDetailService = new PlatformPriceCatalogDetailService();
                    bool timeValuesResultValid = true;
                    string timeValuesMessage = "";
                    foreach (PriceCatalogDetail currentPriceCatalogDetail in priceCatalog.PriceCatalogDetails)
                    {
                        ValidationPriceCatalogDetailTimeValuesResult validationResult = platformPriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(currentPriceCatalogDetail.TimeValues);
                        if (validationResult != null)
                        {
                            timeValuesResultValid = false;
                            timeValuesMessage = (validationResult.PartialOverlap ? Resources.PARTIALLY_OVERLAPPING_TIME_VALUES : Resources.Error)
                                                                    + Environment.NewLine + Resources.FromDate + ": " + validationResult.From.ToString()
                                                                    + Environment.NewLine + Resources.ToDate + ": " + validationResult.To.ToString();
                        }
                    }
                    if (!timeValuesResultValid)
                    {
                        return Json(new { success = false, error = timeValuesMessage });
                    }
                    #endregion
                    string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                    try
                    {
                        if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
                        {
                            if (MvcApplication.Status != ApplicationStatus.ONLINE)
                            {
                                throw new Exception(Resources.ApplicationMustBeConnectedToHeadQuartersToEditPrices);
                            }
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
                            {
                                webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                                webService.Url = StoreControllerAppiSettings.MasterServiceURL;
                                string errorMessage = string.Empty;

                                foreach (PriceCatalogDetail storeControllerPriceCatalogDetail in priceCatalog.PriceCatalogDetails.Where(priceDetail =>
                                                                                                                                        priceDetail.HasChangedOrHasTimeValueChanges
                                                                                                                                        )
                                        )
                                {
                                    string jsonItem = storeControllerPriceCatalogDetail.JsonWithDetails(PlatformConstants.JSON_SERIALIZER_SETTINGS, false);
                                    if (webService.InsertOrUpdateRecord(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid, "PriceCatalogDetail", jsonItem, out errorMessage)
                                        == false
                                       )
                                    {
                                        Session["Error"] = errorMessage;
                                        return Json(new { reloadEdit = true, data = priceCatalog, error = Session["Error"] });
                                    }
                                }
                                #region manage deletion of priceCatalog Details
                                List<object> details = priceCatalog.Session.GetObjectsToSave().Cast<object>().Where(priceCatalogDetailObject => priceCatalogDetailObject is BaseObj).ToList();
                                foreach (BaseObj currentBaseObject in details)
                                {
                                    if (currentBaseObject.IsDeleted)
                                    {
                                        webService.DeleteRecord(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid,
                                            currentBaseObject.GetType().Name,
                                            currentBaseObject.Oid,
                                            out errorMessage
                                            );
                                    }
                                }
                                #endregion
                                priceCatalog.Save();
                                XpoHelper.CommitTransaction(uow);
                                Session["Notice"] = Resources.SavedSuccesfully;
                            }
                        }

                        priceCatalog.Save();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ": " + (e.InnerException == null ? e.Message : e.InnerException.Message);
                    }
                    finally
                    {

                        Session["IsNewPriceCatalog"] = null;
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["uow"] = null;
                        Session["UnsavedPriceCatalog"] = null;
                        Session["IsRefreshed"] = null;
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                    }

                }
            }
            else
            {
                Session["Error"] = Resources.FillAllMissingFields;
                return Json(new { success = false });
            }
            return Json(new { success = Session["Error"] == null || string.IsNullOrEmpty(Session["Error"].ToString()) });
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            bool isRefreshed;
            if (Boolean.TryParse(Session["IsRefreshed"].ToString(), out isRefreshed) && isRefreshed == false)
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewPriceCatalog"] = null;
                Session["UnsavedPriceCatalog"] = null;
            }
            return null;
        }

        public static object PriceCatalogRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                PriceCatalog obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PriceCatalog>(e.Value);
                return obj;
            }
            return null;

        }

        public static object GetPriceCatalogByValue(object value)
        {
            return GetObjectByValue<PriceCatalog>(value);
        }

        public static object PriceCatalogsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            PriceCatalog priceCatalog = (PriceCatalog)System.Web.HttpContext.Current.Session["UnsavedPriceCatalog"];
            CriteriaOperator criteria = CriteriaOperator.And(CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                 //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                 //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                 ),
                                                              new BinaryOperator("Oid", priceCatalog.Oid, BinaryOperatorType.NotEqual)
                                                             );

            XPCollection<PriceCatalog> collection = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(), criteria, "Description");

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult PriceCatalogsComboBoxPartial()
        {
            return PartialView();
        }

        public ActionResult ChildPriceCatalogsComboBoxPartial()
        {
            return PartialView();
        }

        public JsonResult jsonCheckForExistingPriceCatalog(string Code)
        {
            GenerateUnitOfWork();

            PriceCatalog unsavedPriceCatalog = (PriceCatalog)System.Web.HttpContext.Current.Session["UnsavedPriceCatalog"];
            CompanyNew owner = (bool)System.Web.HttpContext.Current.Session["IsNewPriceCatalog"] == true ? CurrentOwner : unsavedPriceCatalog.Owner;
            PriceCatalog priceCatalog = uow.FindObject<PriceCatalog>(CriteriaOperator.And(new BinaryOperator("Code", Code),
                                                                                          new BinaryOperator("Owner.Oid", owner.Oid),
                                                                                          new BinaryOperator("Oid", unsavedPriceCatalog.Oid, BinaryOperatorType.NotEqual)
                                                                                         ));

            return (priceCatalog == null)
                   ? Json(new { Error = "" })
                   : Json(new { PriceCatalogID = priceCatalog.Oid, PriceCatalogDescription = priceCatalog.Description, PriceCatalogCode = priceCatalog.Code });
        }

        [Security(ReturnsPartial = false)]
        public ActionResult LoadExistingPriceCatalog(string PriceCatalogID)
        {
            GenerateUnitOfWork();
            uow.RollbackTransaction();

            Guid pcGuid;
            PriceCatalog pc = null;
            if (Guid.TryParse(PriceCatalogID, out pcGuid))
            {
                pc = uow.GetObjectByKey<PriceCatalog>(pcGuid);
                if (pc != null)
                {
                    Session["IsNewPriceCatalog"] = false;
                    Session["UnsavedPriceCatalog"] = pc;
                    ViewData["PriceCatalogID"] = pc.Oid.ToString();
                    ViewData["StorePriceList"] = pc.StorePriceLists;
                    ViewData["EditMode"] = true;
                    pc.PriceCatalogDetails.Filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                }
            }

            FillLookupComboBoxes();

            return View("EditView", pc);
        }

        public ActionResult CustomerTabPage()
        {

            return PartialView();
        }

        public ActionResult SupplierTabPage()
        {

            return PartialView();
        }

        public ActionResult PriceCatalogDetailGrid(string PriceCatalogID, bool editMode, string TabPage)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = editMode;
            if (editMode == true)  //Edit mode
            {
                FillLookupComboBoxes();

                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedPriceCatalogDetail"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid PriceCatalogDetailID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);

                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    foreach (PriceCatalogDetail pcd in pc.PriceCatalogDetails)
                    {
                        if (pcd.Oid == PriceCatalogDetailID)
                        {
                            Session["UnsavedPriceCatalogDetail"] = pcd;
                            break;
                        }
                    }
                }
                else if (Request["DXCallbackArgument"].Contains("CLEARFILTERS"))
                {
                    Session["PriceCatalogDetailFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
                }
                else if (Request["DXCallbackArgument"].Contains("SEARCH"))
                {
                    Session["PriceCatalogDetailFilter"] = PriceCatalogDetailsFilter();
                }

                ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
                if (((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Count > 0)
                {
                    ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Sorting = new SortingCollection(new SortProperty("CreatedOnTicks", SortingDirection.Descending));
                }
                return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
            }
            else  //View Mode or Customer Mode
            {
                if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("CLEARFILTERS"))
                {
                    Session["PriceCatalogDetailViewFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
                }
                else if (Request["DXCallbackArgument"] != null && Request["DXCallbackArgument"].Contains("SEARCH"))
                {
                    Session["PriceCatalogDetailViewFilter"] = PriceCatalogDetailsFilter();
                }
                else if (Request["DXCallbackArgument"] != null
                     && !Request["DXCallbackArgument"].Contains("PAGERONCLICK")
                     && !Request["DXCallbackArgument"].Contains("COLUMNMOVE")
                     && !Request["DXCallbackArgument"].Contains("SORT")
                     && !Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER")
                     && !Request["DXCallbackArgument"].Contains("APPLYFILTER")
                     && !Request["DXCallbackArgument"].Contains("SHOWDETAILROW")
                     && !Request["DXCallbackArgument"].Contains("HIDEDETAILROW")
                     )
                {
                    Session["PriceCatalogDetailViewFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                }
                else if (Request["DXCallbackArgument"] == null)
                {
                    Session["PriceCatalogDetailViewFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                }

                IEnumerable<PriceCatalogDetail> details = null;

                if (TabPage == "Customer")
                {
                    Store store = uow.GetObjectByKey<Store>(CurrentStore.Oid);
                    Customer userCustomer = UserHelper.GetCustomer(CurrentUser);
                    details = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(store, (CriteriaOperator)Session["PriceCatalogDetailViewFilter"], userCustomer).OrderByDescending(detail => detail.CreatedOnTicks);
                }
                else
                {
                    Guid PriceCatalogGuid;
                    PriceCatalog priceCatalog = null;
                    if (Guid.TryParse(PriceCatalogID, out PriceCatalogGuid))
                    {
                        priceCatalog = uow.GetObjectByKey<PriceCatalog>(PriceCatalogGuid);
                        ViewData["PriceCatalogID"] = PriceCatalogID;
                        if (priceCatalog != null)
                        {
                            details = GetList<PriceCatalogDetail>(this.uow, CriteriaOperator.And((CriteriaOperator)Session["PriceCatalogDetailViewFilter"], new BinaryOperator("PriceCatalog.Oid", priceCatalog.Oid))).OrderByDescending(detail => detail.CreatedOnTicks);
                        }
                    }
                }

                ViewData["TabPage"] = TabPage;
                return PartialView("PriceCatalogDetailGrid", details);
            }
        }

        [HttpPost]
        public ActionResult PriceCatalogDetailInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];

                    Guid ItemBarcodeID = Request["ItemBarcodeID"] == null || Request["ItemBarcodeID"] == "-1" || Request["ItemBarcodeID"] == "null" ? Guid.Empty : Guid.Parse(Request["ItemBarcodeID"]);
                    ItemBarcode itemBarcode = uow.GetObjectByKey<ItemBarcode>(ItemBarcodeID);
                    pc.PriceCatalogDetails.Filter = CriteriaOperator.And(new BinaryOperator("Item.Oid", itemBarcode.Item.Oid),
                                                new BinaryOperator("Barcode.Oid", itemBarcode.Barcode.Oid));

                    if (pc.PriceCatalogDetails.Count != 0)
                    {
                        Session["Error"] = Resources.DuplicatePriceCatalogDetail;
                        FillLookupComboBoxes();
                        Session["UnsavedPriceCatalog"] = pc;
                        ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
                        return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
                    }


                    PriceCatalogDetail pcd = new PriceCatalogDetail(uow);
                    pcd.Barcode = itemBarcode.Barcode;
                    pcd.VATIncluded = Request["VatIncluded"] == "true" ? true : false;
                    pcd.DatabaseValue = ct.DatabaseValue;
                    pcd.Discount = ct.Discount / 100;
                    pcd.MarkUp = ct.MarkUp / 100;
                    pcd.VATIncluded = ct.VATIncluded;
                    pcd.Item = itemBarcode.Item;
                    pcd.IsActive = Request["IsActiveValue"] == "C";
                    pcd.TimeValue = ct.TimeValue;
                    pcd.TimeValueValidFrom = ct.TimeValueValidFrom;
                    pcd.TimeValueValidUntil = ct.TimeValueValidUntil;
                    if (!pcd.TimeValueIsValid)
                    {
                        ModelState.AddModelError("TimeValue", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidFromDate", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidUntilDate", Resources.InvalidTimeValue);
                        ViewBag.CurrentItem = pcd;
                    }
                    else
                    {
                        pc.PriceCatalogDetails.Add(pcd);
                        Session["UnsavedPriceCatalog"] = pc;
                        Session["UnsavedPriceCatalogDetail"] = null;
                        ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
                    }
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
            return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
        }

        [HttpPost]
        public ActionResult PriceCatalogDetailInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            if (ModelState.IsValid)
            {
                try
                {
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    PriceCatalogDetail pcd = (PriceCatalogDetail)Session["UnsavedPriceCatalogDetail"];

                    Guid ItemBarcodeID = Request["ItemBarcodeID"] == null || Request["ItemBarcodeID"] == "-1" || Request["ItemBarcodeID"] == "null" ? Guid.Empty : Guid.Parse(Request["ItemBarcodeID"]);
                    ItemBarcode itemBarcode = uow.GetObjectByKey<ItemBarcode>(ItemBarcodeID);

                    if (pc.PriceCatalogDetails.FirstOrDefault(x => x.Item.Oid == itemBarcode.Item.Oid && x.Barcode.Oid == itemBarcode.Barcode.Oid && x.Oid != pcd.Oid) != null)
                    {
                        Session["Error"] = Resources.DuplicatePriceCatalogDetail;
                        FillLookupComboBoxes();
                        Session["UnsavedPriceCatalog"] = pc;
                        ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
                        return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
                    }

                    pcd.Barcode = itemBarcode.Barcode;
                    pcd.VATIncluded = Request["VatIncluded"] == "true" ? true : false;
                    pcd.DatabaseValue = ct.DatabaseValue;
                    pcd.Discount = ct.Discount / 100;
                    pcd.MarkUp = ct.MarkUp / 100;
                    pcd.VATIncluded = ct.VATIncluded;
                    pcd.IsActive = Request["IsActiveValue"] == "C";
                    pcd.Item = itemBarcode.Item;
                    decimal oldTimeValue = pcd.TimeValue;
                    long oldTimeValueValidFrom = pcd.TimeValueValidFrom;
                    long oldTimeValueValidUntil = pcd.TimeValueValidUntil;

                    pcd.TimeValue = ct.TimeValue;
                    pcd.TimeValueValidFrom = ct.TimeValueValidFrom;
                    pcd.TimeValueValidUntil = ct.TimeValueValidUntil;
                    if (!pcd.TimeValueIsValid)
                    {
                        ModelState.AddModelError("TimeValue", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidFromDate", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidUntilDate", Resources.InvalidTimeValue);
                        ViewBag.CurrentItem = pcd;
                    }
                    else
                    {

                        pcd.PriceCatalog = pc;
                        Session["UnsavedPriceCatalog"] = pc;
                        Session["UnsavedPriceCatalogDetail"] = null;
                        ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
        }


        [HttpPost]
        public ActionResult PriceCatalogDetailInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            try
            {
                PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                PriceCatalogDetail priceCatalogDetail = pc.Session.GetObjectByKey<PriceCatalogDetail>(ct.Oid);
                priceCatalogDetail.Delete();
                //foreach (PriceCatalogDetail pcd in pc.PriceCatalogDetails)
                //{
                //    if (pcd.Oid == ct.Oid)
                //    {
                //        pc.PriceCatalogDetails.Remove(pcd);
                //        pcd.Delete();
                //        break;
                //    }
                //}
                ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails.Filter = (CriteriaOperator)Session["PriceCatalogDetailFilter"];
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }
            FillLookupComboBoxes();
            return PartialView("PriceCatalogDetailGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).PriceCatalogDetails);
        }

        public static object ItemBarcodesRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Filter))
            {
                return null;
            }
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');

            if (OwnerApplicationSettings.PadBarcodes && !codefilter.Contains('%'))
            {
                codefilter = codefilter.PadLeft(OwnerApplicationSettings.BarcodeLength, OwnerApplicationSettings.BarcodePaddingCharacter[0]);
            }
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();

            XPCollection<ItemBarcode> collection = GetList<ItemBarcode>(uowLocal,
                                                                CriteriaOperator.Or(new BinaryOperator("Item.Name", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Barcode.Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)),
                                                                "Barcode.Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult BarcodesComboBoxPartial()
        {
            return PartialView();
        }


        public ActionResult StorePriceListGrid(string PriceCatalogID, bool editMode, string TabPage)
        {
            ViewData["EditMode"] = editMode;
            GenerateUnitOfWork();
            if (editMode == true)
            {
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    if (Session["UnsavedPriceCatalog"] != null)
                    {
                        StorePriceList spl = new StorePriceList(uow);
                        Session["UnsavedStorePriceList"] = spl;
                    }
                    else
                    {
                        StorePriceList spl = new StorePriceList(uow);
                        spl.PriceList = uow.GetObjectByKey<PriceCatalog>(PriceCatalogID);
                        Session["UnsavedStorePriceList"] = spl;
                    }

                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedStorePriceList"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid StorePriceListID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    Session["UnsavedStorePriceList"] = uow.GetObjectByKey<StorePriceList>(StorePriceListID);
                }

                return PartialView("StorePriceListGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).StorePriceLists);
            }
            else //view mode
            {
                Guid PriceCatalogGuid = (PriceCatalogID == null || PriceCatalogID == "null" || PriceCatalogID == "-1") ? Guid.Empty : Guid.Parse(PriceCatalogID);
                PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(PriceCatalogGuid);
                ViewData["PriceCatalogID"] = PriceCatalogID;
                if (pc != null)
                {
                    ViewData["StorePriceLists"] = pc.StorePriceLists;
                }
                ViewData["TabPage"] = TabPage;
                return PartialView("StorePriceListGrid", ViewData["StorePriceLists"]);
            }
        }


        [HttpPost]
        public ActionResult StorePriceListInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] StorePriceList ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            if (ModelState.IsValid)
            {
                try
                {
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    StorePriceList spl = (StorePriceList)Session["UnsavedStorePriceList"];
                    Guid StoreID = Request["StoreID"] == null || Request["StoreID"] == "-1" || Request["StoreID"] == "null" ? Guid.Empty : Guid.Parse(Request["StoreID"]);
                    Store store = pc.Session.GetObjectByKey<Store>(StoreID);
                    spl.Store = store;
                    pc.StorePriceLists.Add(spl);
                    Session["UnsavedPriceCatalog"] = pc;
                    Session["UnsavedStorePriceList"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("StorePriceListGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).StorePriceLists);
        }

        [HttpPost]
        public ActionResult StorePriceListInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] StorePriceList ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                StorePriceList storePriceList = pc.Session.GetObjectByKey<StorePriceList>(ct.Oid);
                storePriceList.Delete();
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            FillLookupComboBoxes();
            return PartialView("StorePriceListGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).StorePriceLists);
        }


        public static object StoresRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            PriceCatalog priceCatalog = (PriceCatalog)System.Web.HttpContext.Current.Session["UnsavedPriceCatalog"];

            CriteriaOperator criteria = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), e.Filter),
                                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.City"), e.Filter),
                                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.Description"), e.Filter)
                                                            //new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                            //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                            //new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                            //new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                            );

            XPCollection<Store> collection = GetList<Store>(priceCatalog.Session,
                                                            CriteriaOperator.And(criteria,
                                                                                 new NotOperator(new InOperator("Oid", priceCatalog.StorePriceLists.Select(list => list.Store)))
                                                                                )
                                                            );

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult StoresComboBoxPartial()
        {
            return PartialView();
        }

        public static object ItemCategoriesRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<ItemCategory> collection = GetList<ItemCategory>(XpoHelper.GetNewUnitOfWork(),
                                                    CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                                        new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter)
                                                                        //new BinaryOperator("Code", String.Format("{0}", e.Filter), BinaryOperatorType.Like),
                                                                        //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                        ),
                                                    "Code");

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult ItemCategoriesComboBox()
        {
            return PartialView();
        }

        public ActionResult CustomerCategoryDiscountGrid(string PriceCatalogID, bool editMode, string TabPage)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = editMode;
            if (editMode == true)  //edit mode
            {
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedCustomerCategoryDiscount"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid CustomerCategoryDiscountID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    foreach (CustomerCategoryDiscount discount in pc.CustomerCategoryDiscounts)
                    {
                        if (discount.Oid == CustomerCategoryDiscountID)
                        {
                            Session["UnsavedCustomerCategoryDiscount"] = discount;
                            break;
                        }
                    }
                }
                FillLookupComboBoxes();
                return PartialView("CustomerCategoryDiscountGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).CustomerCategoryDiscounts);
            }
            else //view mode
            {
                Guid PriceCatalogGuid = (PriceCatalogID == null || PriceCatalogID == "null" || PriceCatalogID == "-1") ? Guid.Empty : Guid.Parse(PriceCatalogID);
                PriceCatalog pc = uow.GetObjectByKey<PriceCatalog>(PriceCatalogGuid);
                ViewData["PriceCatalogID"] = PriceCatalogID;
                ViewData["TabPage"] = TabPage;
                if (pc != null)
                {
                    return PartialView("CustomerCategoryDiscountGrid", pc.CustomerCategoryDiscounts);
                }
                return PartialView("CustomerCategoryDiscountGrid", GetList<CustomerCategoryDiscount>(uow, new BinaryOperator("Oid", Guid.Empty)));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerCategoryDiscountAddNew([ModelBinder(typeof(RetailModelBinder))] CustomerCategoryDiscount ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            if (ModelState.IsValid)
            {
                try
                {
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    CustomerCategoryDiscount discount = new CustomerCategoryDiscount(uow);

                    Guid itemCategoryID = Request["ItemCategoryID"] != null || Request["ItemCategoryID"] != "null" ? Guid.Parse(Request["ItemCategoryID"]) : Guid.Empty;
                    Guid customerCategoryID = Request["CustomerCategoryID"] != null || Request["CustomerCategoryID"] != "null" ? Guid.Parse(Request["CustomerCategoryID"]) : Guid.Empty;
                    discount.ItemCategory = uow.GetObjectByKey<ItemCategory>(itemCategoryID);
                    discount.CustomerCategory = uow.GetObjectByKey<CustomerCategory>(customerCategoryID);
                    discount.Discount = ct.Discount / 100;

                    pc.CustomerCategoryDiscounts.Add(discount);
                    Session["UnsavedPriceCatalog"] = pc;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("CustomerCategoryDiscountGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).CustomerCategoryDiscounts);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerCategoryDiscountUpdate([ModelBinder(typeof(RetailModelBinder))] CustomerCategoryDiscount ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            if (ModelState.IsValid)
            {
                try
                {
                    PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                    CustomerCategoryDiscount discount = (CustomerCategoryDiscount)Session["UnsavedCustomerCategoryDiscount"];

                    Guid itemCategoryID = Request["ItemCategoryID"] != null || Request["ItemCategoryID"] != "null" ? Guid.Parse(Request["ItemCategoryID"]) : Guid.Empty;
                    Guid customerCategoryID = Request["CustomerCategoryID"] != null || Request["CustomerCategoryID"] != "null" ? Guid.Parse(Request["CustomerCategoryID"]) : Guid.Empty;
                    discount.ItemCategory = uow.GetObjectByKey<ItemCategory>(itemCategoryID);
                    discount.CustomerCategory = uow.GetObjectByKey<CustomerCategory>(customerCategoryID);
                    discount.Discount = ct.Discount / 100;

                    pc.CustomerCategoryDiscounts.Remove(discount);
                    pc.CustomerCategoryDiscounts.Add(discount);
                    Session["UnsavedPriceCatalog"] = pc;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("CustomerCategoryDiscountGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).CustomerCategoryDiscounts);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerCategoryDiscountDelete([ModelBinder(typeof(RetailModelBinder))] CustomerCategoryDiscount ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            try
            {
                Guid ChildPriceCatalogID = ct.Oid;
                PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                CustomerCategoryDiscount customerCategoryDiscount = pc.Session.GetObjectByKey<CustomerCategoryDiscount>(ct.Oid);
                customerCategoryDiscount.Delete();
                //foreach (CustomerCategoryDiscount ccd in pc.CustomerCategoryDiscounts)
                //{
                //    if (ccd.Oid == ChildPriceCatalogID)
                //    {
                //        pc.CustomerCategoryDiscounts.Remove(ccd);
                //        ccd.Delete();
                //        break;
                //    }
                //}
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("CustomerCategoryDiscountGrid", ((PriceCatalog)Session["UnsavedPriceCatalog"]).CustomerCategoryDiscounts);
        }

        public CriteriaOperator PriceCatalogDetailsFilter()
        {
            CriteriaOperator filter = null;
            if (Request.HttpMethod == "POST")
            {
                string fcode = Request["Fcode"] == null || Request["Fcode"] == "null" ? "" : Request["Fcode"];
                string fname = Request["Fname"] == null || Request["Fname"] == "null" ? "" : Request["Fname"];
                string fBarcode = Request["Fbarcode"] == null || Request["Fbarcode"] == "null" ? "" : Request["Fbarcode"];
                string factive = Request["Factive"] == null || Request["Factive"] == "null" ? "" : Request["Factive"];
                string Fcategory = Request["Fcategory"] == null || Request["Fcategory"] == "null" ? "" : Request["Fcategory"];
                string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];
                string Fbuyer = Request["Fbuyer"] == null || Request["Fbuyer"] == "null" ? "" : Request["Fbuyer"];
                string Fseasonality = Request["Fseasonality"] == null || Request["Fseasonality"] == "null" ? "" : Request["Fseasonality"];
                string Fmothercode = Request["Fmothercode"] == null || Request["Fmothercode"] == "null" ? "" : Request["Fmothercode"];
                string FitemSupplier = Request["FitemSupplier"] == null || Request["FitemSupplier"] == "null" ? "" : Request["FitemSupplier"];

                if (OwnerApplicationSettings.PadBarcodes)
                {
                    fBarcode = fBarcode != "" && !fBarcode.Contains("*") && !fBarcode.Contains("%") ? fBarcode.PadLeft(OwnerApplicationSettings.BarcodeLength, OwnerApplicationSettings.BarcodePaddingCharacter[0]) : fBarcode;
                }

                if (OwnerApplicationSettings.PadItemCodes)
                {
                    fcode = fcode != "" && !fcode.Contains("*") && !fcode.Contains("%") ? fcode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : fcode;
                    Fmothercode = Fmothercode != "" && !Fmothercode.Contains("*") && !Fmothercode.Contains("%") ? Fmothercode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fmothercode;
                }

                CriteriaOperator codeFilter = null;
                if (fcode != null && fcode.Trim() != "")
                {
                    if (fcode.Replace('%', '*').Contains("*"))
                    {
                        codeFilter = new BinaryOperator("Item.Code", fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                    }
                    else
                    {
                        codeFilter = new BinaryOperator("Item.Code", fcode);
                    }
                }

                CriteriaOperator nameFilter = null;
                if (fname != null && fname.Trim() != "")
                {
                    nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Name"), fname);
                    // new BinaryOperator("Item.Name", "%" + fname + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator barfilter = null;
                if (fBarcode != null && fBarcode.Trim() != "")
                {
                    if (fBarcode.Replace('%', '*').Contains("*"))
                    {
                        barfilter = new BinaryOperator("Barcode.Code", fBarcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                    }
                    else
                    {
                        barfilter = new BinaryOperator("Barcode.Code", fBarcode);
                    }
                }

                CriteriaOperator activefilter = null;
                if (factive == "0" || factive == "1")
                {
                    activefilter = new BinaryOperator("Item.IsActive", factive == "1");
                }

                CriteriaOperator categoryfilter = null;
                if (Fcategory != "" && Fcategory != "-1")
                {
                    Guid categoryGuid;
                    if (Guid.TryParse(Fcategory, out categoryGuid))
                    {
                        ItemCategory ic = uow.GetObjectByKey<ItemCategory>(categoryGuid);
                        categoryfilter = ic.GetAllNodeTreeFilter("Item.ItemAnalyticTrees");
                    }
                }

                CriteriaOperator createdOnFilter = null;
                if (FcreatedOn != "")
                {
                    createdOnFilter = CriteriaOperator.Or(new BinaryOperator("Item.InsertedDate", DateTime.Parse(FcreatedOn), BinaryOperatorType.GreaterOrEqual));
                }

                CriteriaOperator updatedOnFilter = null;
                if (FupdatedOn != "")
                {
                    updatedOnFilter = CriteriaOperator.Or(new BinaryOperator("Item.UpdatedOn", DateTime.Parse(FupdatedOn), BinaryOperatorType.GreaterOrEqual));
                }

                CriteriaOperator buyerFilter = null;
                if (Fbuyer != null && Fbuyer.Trim() != "")
                {
                    buyerFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Buyer.Description"), Fbuyer);
                    // new BinaryOperator("Item.Buyer.Description", "%" + Fbuyer + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator seasonalityFilter = null;
                if (Fseasonality != null && Fseasonality.Trim() != "")
                {
                    seasonalityFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Seasonality.Description"), Fseasonality);
                    // new BinaryOperator("Item.Seasonality.Description", "%" + Fseasonality + "%", BinaryOperatorType.Like);
                }

                CriteriaOperator mothercodeFilter = null;
                if (Fmothercode != null && Fmothercode.Trim() != "")
                {
                    if (Fmothercode.Replace('%', '*').Contains("*"))
                    {
                        mothercodeFilter = //new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.MotherCode.Code"), );
                        new BinaryOperator("Item.MotherCode.Code", Fmothercode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                    }
                    else
                    {
                        mothercodeFilter = new BinaryOperator("Item.MotherCode.Code", Fmothercode);
                    }
                }

                CriteriaOperator itemSupplierFilter = null;
                if (FitemSupplier != null && FitemSupplier.Trim() != "")
                {
                    itemSupplierFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.DefaultSupplier.CompanyName"), FitemSupplier);
                    // new BinaryOperator("Item.DefaultSupplier.CompanyName", "%" + FitemSupplier + "%", BinaryOperatorType.Like);
                }

                filter = CriteriaOperator.And(codeFilter, nameFilter, barfilter, activefilter, categoryfilter, createdOnFilter, updatedOnFilter, buyerFilter, seasonalityFilter, mothercodeFilter, itemSupplierFilter);
            }
            else
            {
                filter = new BinaryOperator("Oid", Guid.Empty);
            }
            return filter;
        }


        public ActionResult PriceCatalogDetailTimeValueGrid(Guid PriceCatalogDetailOid, bool EditMode)
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            ViewData["EditMode"] = EditMode;
            ViewData["PriceCatalogDetailOid"] = PriceCatalogDetailOid;
            PriceCatalogDetail pcd = null;
            if (EditMode)
            {
                PriceCatalog pc = (PriceCatalog)Session["UnsavedPriceCatalog"];
                if (pc != null)
                {
                    pcd = pc.PriceCatalogDetails.FirstOrDefault(x => x.Oid == PriceCatalogDetailOid);
                }
            }
            else
            {
                pcd = uow.GetObjectByKey<PriceCatalogDetail>(PriceCatalogDetailOid);
            }
            if (pcd != null)
            {
                return PartialView(pcd.TimeValues);
            }
            return null;
        }

        [ValidateInput(false)]
        public ActionResult PriceCatalogDetailTimeValueBatchUpdateGrid(MVCxGridViewBatchUpdateValues<PriceCatalogDetailTimeValue, Guid> updateValues, Guid PriceCatalogDetailOid)
        {
            //return null;
            ViewData["EditMode"] = true;
            ViewData["PriceCatalogDetailOid"] = PriceCatalogDetailOid;

            PriceCatalogDetail priceCatalogDetail = null;
            PriceCatalog priceCatalog = (PriceCatalog)Session["UnsavedPriceCatalog"];
            if (priceCatalog != null)
            {
                priceCatalogDetail = priceCatalog.PriceCatalogDetails.FirstOrDefault(priceDetail => priceDetail.Oid == PriceCatalogDetailOid);

                //Deletions
                priceCatalogDetail.TimeValues.Where(timeValue => updateValues.DeleteKeys.Contains(timeValue.Oid)).ToList().ForEach(timeValue => timeValue.Delete());
                //Insertions
                updateValues.Insert.ForEach(timeValue =>
                {
                    PriceCatalogDetailTimeValue newValue = new PriceCatalogDetailTimeValue(priceCatalogDetail.Session);
                    newValue.GetData(timeValue);
                    priceCatalogDetail.TimeValues.Add(newValue);
                });

                //Updates
                updateValues.Update.ForEach(updatedTimeValue =>
                {
                    PriceCatalogDetailTimeValue newValue = priceCatalogDetail.TimeValues.FirstOrDefault(timeValue => timeValue.Oid == updatedTimeValue.Oid);
                    if (newValue != null)
                    {
                        newValue.TimeValueValidFrom = updatedTimeValue.TimeValueValidFrom;
                        newValue.TimeValueValidUntil = updatedTimeValue.TimeValueValidUntil;
                        newValue.TimeValue = updatedTimeValue.TimeValue;
                        newValue.IsActive = updatedTimeValue.IsActive;
                    }
                });
                #region Validate Time Values
                PlatformPriceCatalogDetailService platformPriceCatalogDetailService = new PlatformPriceCatalogDetailService();
                bool timeValuesResultValid = true;
                string timeValuesMessage = "";
                foreach (PriceCatalogDetail currentPriceCatalogDetail in priceCatalog.PriceCatalogDetails)
                {
                    ValidationPriceCatalogDetailTimeValuesResult validationResult = platformPriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(currentPriceCatalogDetail.TimeValues);
                    if (validationResult != null)
                    {
                        timeValuesResultValid = false;
                        timeValuesMessage = (validationResult.PartialOverlap ? Resources.PARTIALLY_OVERLAPPING_TIME_VALUES : Resources.Error)
                                                                + Environment.NewLine + Resources.FromDate + ": " + validationResult.From.ToString()
                                                                + Environment.NewLine + Resources.ToDate + ": " + validationResult.To.ToString();
                    }
                }
                if (!timeValuesResultValid)
                {
                    Session["Error"] = timeValuesMessage;
                }
                #endregion
            }
            if (priceCatalogDetail != null)
            {
                return PartialView("PriceCatalogDetailTimeValueGrid", priceCatalogDetail.TimeValues);
            }
            return null;

        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.Store = GetList<Store>(XpoSession);
        }
    }
}
