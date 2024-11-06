using ITS.Retail.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SQLite;
using ITS.Retail.WebClient.Helpers;
using System.Threading;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    [RoleAuthorize]
    public class SFAController : BaseObjController<ITS.Retail.Model.SFA>
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

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.CustomButton.Visible = false;
            this.ToolbarOptions.CustomButton.CCSClass = "pos";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "SFAGuid");
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSs");

            GenerateUnitOfWork();
            FillLookupComboBoxes();
            CriteriaOperator filter = (CriteriaOperator)Session["SFAFilter"];
            return View("Index", GetList<ITS.Retail.Model.SFA>(uow, filter).AsEnumerable<ITS.Retail.Model.SFA>());
        }

        public JsonResult CreateSFADatabase()
        {
            bool success = false;
            try
            {
                Thread thread = new Thread(CreateSFADatabaseThread);
                thread.Start();
                success = Session["Error"] == null;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new { success = success });
        }

        public void CreateSFADatabaseThread(object param)
        {
            if (!SFAMasterDBPreparationHelper.IsProcessing)
            {
                Guid currentStore = Guid.Empty;
                if (MvcApplication.ApplicationInstance == Platform.Enumerations.eApplicationInstance.RETAIL)
                {
                    if (CurrentStore == null)
                    {
                        Session["Error"] = ResourcesLib.Resources.PleaseSelectAStore;
                        return;
                    }
                    currentStore = CurrentStore.Oid;
                }
                else
                {
                    currentStore = StoreControllerAppiSettings.CurrentStoreOid;
                }
                SFAMasterDBPreparationHelper.PrepareSFAMaster(currentStore, Server.MapPath("~/POS"));
            }
        }

        public override ActionResult Dialog(List<string> arguments)
        {
            GenerateUnitOfWork();

            this.DialogOptions.OKButton.Visible = false;
            this.DialogOptions.OKButton.OnClick = "function (s,e) { Dialog.Hide(); }";
            this.DialogOptions.BodyPartialView = "SFADatabaseCreationDialog";
            this.DialogOptions.HeaderText = ResourcesLib.Resources.CreateSFADatabase;
            this.DialogOptions.AdjustSizeOnInit = true;
            this.DialogOptions.CancelButton.Visible = false;
            this.DialogOptions.OnShownEvent = "SFA.DialogOnShown";
            return PartialView();
        }

        public JsonResult jsonCheckSFADatabaseRunningStatus()
        {
            return Json(new { done = !SFAMasterDBPreparationHelper.IsProcessing });
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();
            ViewBag.Title = ResourcesLib.Resources.TabletSettings;
            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            CriteriaOperator filter = null;

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {

                if (Request.HttpMethod == "POST")
                {
                    string fid = Request["fid"] == null || Request["fid"] == "null" ? "" : Request["fid"];
                    string fname = Request["fname"] == null || Request["fname"] == "null" ? "" : Request["fname"];
                    string fstore = Request["fstore"] == null || Request["fstore"] == "null" ? "" : Request["fstore"];


                    CriteriaOperator idFilter = null;
                    if (fid != null && fid.Trim() != "")
                    {
                        if (fid.Replace('%', '*').Contains("*"))
                            idFilter = new BinaryOperator("ID", fid.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            idFilter = new BinaryOperator("ID", fid);
                    }

                    CriteriaOperator nameFilter = null;
                    if (fname != null && fname.Trim() != "")
                    {
                        if (fname.Replace('%', '*').Contains("*"))
                            nameFilter = new BinaryOperator("ID", fname.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            nameFilter = new BinaryOperator("ID", fname);
                    }


                    filter = CriteriaOperator.And(idFilter, nameFilter);
                    Session["SFAFilter"] = filter;
                }
                else
                {
                    filter = new BinaryOperator("Oid", Guid.Empty);
                    Session["SFAFilter"] = filter;
                }
            }
            GridFilter = (CriteriaOperator)Session["SFAFilter"];
            return base.Grid();

        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid sfaGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);
            ViewData["EditMode"] = true;
            if (sfaGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (sfaGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }
            SFA sfa = uow.GetObjectByKey<SFA>(sfaGuid);
            if (sfa == null)
            {
                Session["IsNewSFA"] = true;
                sfa = new SFA(uow);
            }
            else
            {
                Session["IsNewSFA"] = false;
            }
            ViewData["ID"] = sfa.Oid.ToString();
            FillLookupComboBoxes();
            return PartialView("Edit", sfa);
        }

        public JsonResult Save()
        {
            Guid sfaGuid = Guid.Empty;
            bool isNew = false;
            int id = -1;
            Guid.TryParse(Request["SFAGuid"], out sfaGuid);
            try
            {
                if (Int32.TryParse(Request["ID"], out id))
                {
                    if (id >= 0)
                    {
                        using (UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
                        {
                            SFA sfa = uow2.GetObjectByKey<SFA>(sfaGuid);
                            if (sfa == null)
                            {
                                sfa = new SFA(uow2);
                                isNew = true;
                            }
                            if (isNew && CheckForExistingId(uow2, id))
                            {
                                Session["Error"] = ResourcesLib.Resources.CodeAlreadyExists;
                                return Json(new { });
                            }
                            else if (!isNew && CheckForExistingId(uow2, id) && sfa.ID != id)
                            {
                                Session["Error"] = ResourcesLib.Resources.CodeAlreadyExists;
                                return Json(new { });
                            }
                            else
                            {
                                sfa.ID = id;
                                sfa.Name = Request["Name"];
                                sfa.IsActive = Request["IsActive"] == "C";
                                uow2.CommitChanges();
                                Session["Notice"] = ResourcesLib.Resources.SavedSuccesfully;
                                return Json(new { });
                            }

                        }
                    }
                }
                else
                {
                    Session["Error"] = ResourcesLib.Resources.RequiredFieldError;
                    return Json(new { });
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return Json(new { ex.Message });
            }
            return Json(new { });
        }

        private bool CheckForExistingId(UnitOfWork uow2, int id)
        {
            return uow2.FindObject<SFA>(CriteriaOperator.And(new BinaryOperator("ID", id, BinaryOperatorType.Equal))) == null ? false : true;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {

            return null;
        }

    }
}
