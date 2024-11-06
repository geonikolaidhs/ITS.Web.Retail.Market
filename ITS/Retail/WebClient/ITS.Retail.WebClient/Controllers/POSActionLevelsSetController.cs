using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSActionLevelsSetController : BaseObjController<POSActionLevelsSet>
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

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.POSInfo;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("editAction", "EditView");
            CustomJSProperties.AddJSProperty("editIDParameter", "POSActionLevelsSetGuid");
            CustomJSProperties.AddJSProperty("gridName", "grdActionLevelsSets");
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            return View("Index", GetList<POSActionLevelsSet>(uow).AsEnumerable<POSActionLevelsSet>());
        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid posActionLevelsGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            ViewData["EditMode"] = true;

            if (posActionLevelsGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (posActionLevelsGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            POSActionLevelsSet set;
            if (Session["UnsavedPOSActionLevelsSet"] == null)
            {
                if (posActionLevelsGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditPOSActionLevelsSet;
                    set = uow.FindObject<POSActionLevelsSet>(new BinaryOperator("Oid", posActionLevelsGuid, BinaryOperatorType.Equal));
                    Session["IsNewPOSActionLevelsSet"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewPOSActionLevelsSet;
                    set = new POSActionLevelsSet(uow);
                    Session["IsNewPOSActionLevelsSet"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (posActionLevelsGuid != Guid.Empty && (Session["UnsavedPOSActionLevelsSet"] as POSActionLevelsSet).Oid == posActionLevelsGuid)
                {
                    Session["IsRefreshed"] = true;
                    set = (POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"];
                }
                else if (posActionLevelsGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    set = (POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    set = uow.FindObject<POSActionLevelsSet>(new BinaryOperator("Oid", posActionLevelsGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["POSActionLevelsSetGuid"] = set.Oid.ToString();
            Session["UnsavedPOSActionLevelsSet"] = set;
            return PartialView("Edit", set);
        }

        public JsonResult Save()
        {

            GenerateUnitOfWork();
            Guid posDeviceGuid = Guid.Empty;

            bool correctPOSActionLevelsSetGuid = Request["POSActionLevelsSetGuid"] != null && Guid.TryParse(Request["POSActionLevelsSetGuid"].ToString(), out posDeviceGuid);
            if (correctPOSActionLevelsSetGuid)
            {
                POSActionLevelsSet posActionLevelsSet = (Session["UnsavedPOSActionLevelsSet"] as POSActionLevelsSet);
                if (posActionLevelsSet != null)
                {
                    posActionLevelsSet.Code = Request["Code"];
                    posActionLevelsSet.Description = Request["Description"];
                    try
                    {
                        AssignOwner<POSActionLevelsSet>(posActionLevelsSet);
                        UpdateLookupObjects(posActionLevelsSet);
                        posActionLevelsSet.Save();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                        return Json(new { error = Session["Error"] });
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewPOSActionLevelsSet"] = null;
                        Session["uow"] = null;
                        Session["UnsavedPOSActionLevelsSet"] = null;
                        Session["IsRefreshed"] = null;
                    }

                }
            }
            return Json(new { });

        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (Session["IsRefreshed"] != null && !Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewPOSActionLevelsSet"] = null;
                Session["UnsavedPOSActionLevelsSet"] = null;

            }
            return null;
        }

        public ActionResult ActionLevelGrid(string POSActionLevelsSetGuid, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    POSActionLevel level = new POSActionLevel(uow);
                    Session["UnsavedPOSActionLevel"] = level;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedPOSActionLevel"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    POSActionLevelsSet set = (POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"];
                    foreach (POSActionLevel level in set.POSActionLevels)
                    {
                        Guid POSActionLevelID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (level.Oid == POSActionLevelID)
                        {
                            Session["UnsavedPOSActionLevel"] = level;
                            break;
                        }
                    }
                }

                return PartialView("ActionLevelsGrid", ((POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"]).POSActionLevels);
            }
            else  //view mode
            {
                Guid guidParsed = (POSActionLevelsSetGuid == null || POSActionLevelsSetGuid == "null" || POSActionLevelsSetGuid == "-1") ? Guid.Empty : Guid.Parse(POSActionLevelsSetGuid);
                POSActionLevelsSet set = XpoHelper.GetNewUnitOfWork().FindObject<POSActionLevelsSet>(new BinaryOperator("Oid", guidParsed, BinaryOperatorType.Equal));
                ViewData["POSActionLevelsSetGuid"] = POSActionLevelsSetGuid;
                return PartialView("ActionLevelsGrid", set.POSActionLevels);
            }
        }

        [HttpPost]
        public ActionResult ActionLevelAddOrUpdate([ModelBinder(typeof(RetailModelBinder))] POSActionLevel ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    POSActionLevelsSet set = (POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"];
                    POSActionLevel posActionLevel = (POSActionLevel)Session["UnsavedPOSActionLevel"];
                    posActionLevel.KeyLevel = (eKeyStatus)Enum.Parse(typeof(eKeyStatus), Request["KeyLevel_VI"]);
                    posActionLevel.ActionCode = (eActions)Enum.Parse(typeof(eActions), Request["ActionCode_VI"]);
                    posActionLevel.POSActionLevelsSet = set;
                    Session["UnsavedPOSActionLevelsSet"] = set;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("ActionLevelsGrid", ((POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"]).POSActionLevels);
        }

        [HttpPost]
        public ActionResult ActionLevelDelete([ModelBinder(typeof(RetailModelBinder))] POSActionLevel ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                POSActionLevelsSet set = (POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"];
                foreach (POSActionLevel level in set.POSActionLevels)
                {
                    if (level.Oid == ct.Oid)
                    {
                        set.POSActionLevels.Remove(level);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("ActionLevelsGrid", ((POSActionLevelsSet)Session["UnsavedPOSActionLevelsSet"]).POSActionLevels);
        }


        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
            ViewBag.ExternalActions = POSHelper.GetExternalActions().OrderBy(x => x.ToLocalizedString()).ToDictionary(x => x, y => y.ToLocalizedString());
        }


    }
}
