using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ActionTypeController : BaseObjController<ActionType>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.CustomButton.Visible = true;
            this.ToolbarOptions.CustomButton.Name = "RecalculateActionTypes";
            this.ToolbarOptions.CustomButton.OnClick = "ActionType.RecalculateActionTypes";
            this.ToolbarOptions.CustomButton.CCSClass = "fake";
            this.ToolbarOptions.CustomButton.Title = Resources.Recalculate;
            this.CustomJSProperties.AddJSProperty("gridName", "grdActionType");

            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "ActionType.ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "ActionType.ImportButtonOnClick";

            return View("Index", GetList<ActionType>(XpoSession).AsEnumerable());
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Session["ActionType"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<ActionType>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]));
                }
                else if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["ActionType"] = new ActionType(XpoHelper.GetNewUnitOfWork());
                    ViewBag.IsNewAction = true;
                }
                else if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
                {
                    foreach (Guid actionTypeOid in RetailHelper.GetOidsToDeleteFromDxCallbackArgument(Request["DXCallbackArgument"]))
                    {
                        ActionType actionType = XpoHelper.GetNewUnitOfWork().GetObjectByKey<ActionType>(actionTypeOid);
                        if (actionType.ActionTypeEntities.Count > 0)
                        {
                            Session["Error"] = string.Format(Resources.ActionTypeUsedByEntities, actionType.Description);
                            return null;
                        }
                    }
                }
            }
            return base.Grid();
        }

        [HttpPost]
        public ActionResult InlineEditingUpDatePartial([ModelBinder(typeof(RetailModelBinder))] ActionType ct)
        {
            try
            {
                ActionType actionType = (ActionType)Session["ActionType"];
                if (ModelState.IsValid)
                {
                    actionType.GetData(ct, new List<string>() { "Session" });
                    Guid storeGuid;
                    if (Guid.TryParse(Request["StoreCombobox_VI"], out storeGuid))
                    {
                        actionType.Store = actionType.Session.GetObjectByKey<Store>(storeGuid);
                    }
                    AssignOwner(actionType);
                    actionType.Save();
                    actionType.Session.CommitTransaction();
                }
                Session["ActionType"] = null;
                return base.Grid();
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return base.Grid();
            }
        }

        [HttpPost]
        public ActionResult InlineUpdateVariablePartial([ModelBinder(typeof(RetailModelBinder))] VariableActionType ct)
        {
            ActionType actionType = (ActionType)Session["ActionType"];
            if (ModelState.IsValid)
            {
                VariableActionType varActionType = actionType.VariableActionTypes.FirstOrDefault(varActType => varActType.Oid == ct.Oid);
                if (varActionType == null)
                {
                    varActionType = new VariableActionType(actionType.Session);
                }
                varActionType.GetData(ct, new List<string>() { "Session" });
                Guid varOid;
                varActionType.Variable = Guid.TryParse(Request["VariableCb_VI"], out varOid) ? varActionType.Session.GetObjectByKey<Variable>(varOid) : null;
                varActionType.VariableName = varActionType.Variable == null ? "" : varActionType.Variable.Description;
                varActionType.ActionType = actionType;
                actionType.VariableActionTypes.Add(varActionType);
            }
            return PartialView("VariablesPartialGrid", actionType.VariableActionTypes);
        }

        [HttpPost]
        public ActionResult InlineDeleteVariablePartial([ModelBinder(typeof(RetailModelBinder))] VariableActionType ct)
        {
            ActionType actionType = (ActionType)Session["ActionType"];
            VariableActionType varActionType = actionType.VariableActionTypes.Where(varActType => varActType.Oid == ct.Oid).FirstOrDefault();
            varActionType.Delete();
            return PartialView("VariablesPartialGrid", actionType.VariableActionTypes);
        }

        [HttpPost]
        public ActionResult VariablesPartialGrid(string category)
        {
            ActionType actionType = (ActionType)Session["ActionType"];
            ActionEntityCategory actionCategory;
            if (Enum.TryParse(category, out actionCategory))
            {
                ViewBag.VariablesComboBox = GetList<Variable>(actionType.Session, new BinaryOperator("Category", actionCategory), "Description")
                .Except(actionType.VariableActionTypes.Select(varActionType => varActionType.Variable));
            }
            return PartialView("VariablesPartialGrid", actionType.VariableActionTypes);
        }

        public static object VariableRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();
            ActionType actionType = (ActionType)System.Web.HttpContext.Current.Session["ActionType"];

            CriteriaOperator crop = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                         //new BinaryOperator("Description", e.Filter, BinaryOperatorType.Like),
                                                         new NotOperator(new ContainsOperator("VariableActionTypes",
                                                                                             new BinaryOperator("ActionType", actionType.Oid)
                                                                                             )
                                                                         )
                                                        );
            XPCollection<Variable> collection = GetList<Variable>(uowLocal, crop);
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            collection.Sorting.Add(new SortProperty("Description", SortingDirection.Ascending));

            return collection;
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.DetailsToIgnore.Add("ActionTypeEntities");
            ruleset.DetailPropertiesToIgnore.Add(typeof(VariableActionType), new List<string>() { "ActionType", "VariableAction", "Variable", "IsActive" });
            return ruleset;
        }

        public ActionResult ActionTypeRecalculateForm()
        {
            List<Guid> actionTypeOids = new List<Guid>();
            if (Request["ActionTypes"] != null)
            {
                List<string> oids = Request["ActionTypes"].ToString().Split(',').ToList();
                foreach (string oid in oids)
                {
                    Guid guid = Guid.Empty;
                    if (Guid.TryParse(oid, out guid))
                    {
                        actionTypeOids.Add(guid);
                    }
                }
            }
            ViewBag.ActionTypes = GetList<ActionType>(XpoHelper.GetNewUnitOfWork());
            return PartialView("../Shared/_ActionTypeRecalculateForm", new ActionRecalculateCriteria() { ActionTypeOids = actionTypeOids });
        }



        [HttpPost]
        public JsonResult RecalculateActionTypes()
        {
            List<string> inputNames = new List<string>()
            {
                "FromDate",
                "ToDate",
                "ActionTypesString_selected"
            };

            foreach (string input in inputNames)
            {
                if (string.IsNullOrEmpty(Request[input]))
                {
                    return Json(new { error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS });
                }
            }

            DateTime fromDate;
            if (DateTime.TryParse(Request["FromDate"], out fromDate) == false)
            {
                return Json(new { error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS });
            }

            DateTime toDate;
            if (DateTime.TryParse(Request["ToDate"], out toDate) == false)
            {
                return Json(new { error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS });
            }

            List<Guid> actionTypeGuids = new List<Guid>();

            foreach (string oid in Request["ActionTypesString_selected"].ToString().Split(','))
            {
                Guid actionTypeGuid;
                if (Guid.TryParse(oid, out actionTypeGuid))
                {
                    actionTypeGuids.Add(actionTypeGuid);
                }
                else
                {
                    return Json(new { error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS });
                }
            }

            if (actionTypeGuids.Count == 0)
            {
                return Json(new { error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS });
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator actionTypeCriteria = new InOperator("Oid", actionTypeGuids);
                XPCollection<ActionType> actionTypes = GetList<ActionType>(uow, actionTypeCriteria);
                CriteriaOperator basicObjectCriteria = new BetweenOperator("UpdatedOnTicks", fromDate.Ticks, toDate.Ticks);
                foreach (ActionType actionType in actionTypes)
                {
                    ActionTypeHelper.RecalculateActionType(actionType, basicObjectCriteria, EffectiveOwner);
                }
            }

            return Json(new { });
        }

        public FileContentResult ExportActionTypes(string ActionTypesOids)
        {
            IEnumerable<Guid> ActionTypesGuids = null;
            try
            {
                ActionTypesGuids = ActionTypesOids.Split(',').Select(x => Guid.Parse(x));
                XPCollection<ActionType> actionTypes = GetList<ActionType>(XpoSession, new InOperator("Oid", ActionTypesGuids));
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> objectsToSerialize = new List<string>();
                        foreach (ActionType actionType in actionTypes)
                        {
                            objectsToSerialize.Add(actionType.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        }
                        sr.Write(JsonConvert.SerializeObject(objectsToSerialize, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "import_ActionTypes.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return null;
            }

        }

        public override ActionResult Dialog(List<string> arguments)
        {
            this.DialogOptions.AdjustSizeOnInit = true;
            this.DialogOptions.HeaderText = Resources.Import;
            this.DialogOptions.BodyPartialView = "ImportSettingsUpload";
            this.DialogOptions.OKButton.OnClick = @"function (s,e) { UploadControl.Upload();}";
            this.DialogOptions.CancelButton.OnClick = "function (s,e) { Dialog.Hide();}";
            return PartialView();
        }

        public ActionResult ImportSettingsUploadForm()
        {
            return PartialView("ImportSettingsUpload");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, ActionTypesUpload_FileUploadComplete);
            return null;
        }

        private void ActionTypesUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        using (StreamReader reader = new StreamReader(e.UploadedFile.FileContent))
                        {
                            string jsonData = reader.ReadToEnd();
                            string error;
                            JArray jActionTypes = JsonConvert.DeserializeObject(jsonData) as JArray;
                            foreach (JToken jActionType in jActionTypes)
                            {
                                JObject jObjActionType = JsonConvert.DeserializeObject(jActionType.ToString()) as JObject;
                                if (jObjActionType is JObject)
                                {
                                    ActionType actionType = new ActionType(uow);
                                    actionType.FromJson(jObjActionType, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                                    foreach (VariableActionType varActionType in actionType.VariableActionTypes)
                                    {
                                        varActionType.Variable = actionType.Session.FindObject<Variable>(new BinaryOperator("Description", varActionType.VariableName));
                                        if (varActionType.Variable == null)
                                        {
                                            throw new Exception(string.Format(Resources.ActionTypeVariableNotFound, varActionType.VariableName, actionType.Description));
                                        }
                                    }
                                    foreach (ActionTypeEntity entity in actionType.ActionTypeEntities)
                                    {
                                        DocumentType docType = actionType.Session.FindObject<DocumentType>(new BinaryOperator("Code", entity.EntityCode));
                                        entity.EntityOid = docType != null ? docType.Oid : Guid.Empty;
                                        foreach (ActionTypeDocStatus actionTypeDocStatus in entity.ActionTypeDocStatuses)
                                        {
                                            actionTypeDocStatus.DocumentStatus = actionType.Session.FindObject<DocumentStatus>(new BinaryOperator("Code", actionTypeDocStatus.DocStatusCode));
                                        }
                                    }
                                    actionType.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                                    actionType.Save();
                                }
                            }
                            XpoHelper.CommitChanges(uow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["Error"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                }
            }
        }

        public ActionResult StoresComboBoxPartial(eTotalizersUpdateMode? updatemode)
        {
            ViewData["UpdateMode"] = updatemode;
            return PartialView("StoresComboBoxPartial", Session["ActionType"]);
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };
    }
}
