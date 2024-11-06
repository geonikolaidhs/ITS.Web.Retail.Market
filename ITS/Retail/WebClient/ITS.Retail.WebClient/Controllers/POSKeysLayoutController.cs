//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Web.Mvc;
using System.Windows.Forms;
using System.IO;
using DevExpress.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSKeysLayoutController : BaseObjController<POSKeysLayout>
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
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "ImportButtonOnClick";

            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "POSKeysLayoutGuid");
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSKeysLayouts");

            GenerateUnitOfWork();
            FillLookupComboBoxes();
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            return View("Index", GetList<POSKeysLayout>(uow, filter).AsEnumerable<POSKeysLayout>());
        }

        public override ActionResult Dialog(List<string> arguments)
        {
            this.DialogOptions.AdjustSizeOnInit = true;
            this.DialogOptions.HeaderText = Resources.Import;
            this.DialogOptions.BodyPartialView = "ImportKeyMappingsUpload";
            this.DialogOptions.OKButton.OnClick = @"function (s,e) { UploadControl.Upload();}";
            this.DialogOptions.CancelButton.OnClick = "function (s,e) { Dialog.Hide();}";
            return PartialView();
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
                    string fcode = Request["fcode"] == null || Request["fcode"] == "null" ? "" : Request["fcode"];
                    string fdescription = Request["fdescription"] == null || Request["fdescription"] == "null" ? "" : Request["fdescription"];

                    CriteriaOperator codeFilter = null;
                    if (fcode != null && fcode.Trim() != "")
                    {
                        if (fcode.Replace('%', '*').Contains("*"))
                            codeFilter = new BinaryOperator("Code", fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            codeFilter = new BinaryOperator("Code", fcode, BinaryOperatorType.Equal);
                    }
                    CriteriaOperator descriptionFilter = null;
                    if (fdescription != null && fdescription.Trim() != "")
                    {
                        descriptionFilter = new BinaryOperator("Description", "%" + fdescription + "%", BinaryOperatorType.Like);
                    }

                    filter = CriteriaOperator.And(codeFilter, descriptionFilter);
                    Session["POSKeysLayoutFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["POSKeysLayoutFilter"] = filter;
                }
            }
            GridFilter = (CriteriaOperator)Session["POSKeysLayoutFilter"];
            return base.Grid();

        }

        public ActionResult ImportKeyMappingsUpload()
        {
            return PartialView();
        }

        private static void ImportKeysLayoutFromFile(Stream fileStream)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string data = reader.ReadToEnd();
                List<string> items = new List<string>();
                try
                {
                    items = JsonConvert.DeserializeObject<List<string>>(data);
                }
                catch
                {
                    throw new Exception(Resources.InvalidFileFormat);
                }
                foreach (string item in items)
                {
                    JObject jsonItem = JObject.Parse(item);
                    IEnumerable<JProperty> itemValues = jsonItem.Properties();
                    string type = jsonItem.Property("Type").Value.ToString();
                    string key = jsonItem.Property("Oid").Value.ToString();
                    Type typeObj = typeof(Item).Assembly.GetType("ITS.Retail.Model." + type);
                    BaseObj currentObject = uow.GetObjectByKey(typeObj, Guid.Parse(key)) as BaseObj;
                    if (currentObject == null)
                    {
                        currentObject = Activator.CreateInstance(typeObj, new object[] { uow }) as BaseObj;
                    }
                    else if(currentObject.IsDeleted)
                    {
                        currentObject.SetMemberValue("GCRecord", null);
                    }
                    string error;
                    currentObject.FromJson(item, PlatformConstants.JSON_SERIALIZER_SETTINGS, true,false,out error);
                    currentObject.Save();
                    
                    XpoHelper.CommitChanges(uow);// prepei na ginei commit prin paei sto epomeno
                }
                
            }
        }


         [Security(OverrideSecurity = true, ReturnsPartial = false)]
        public FileContentResult ExportKeysLayout()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string allOids = Request["POSKeysLayoutGuids"];
                if (!String.IsNullOrWhiteSpace(allOids))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();
                    foreach (string strOid in strOids)
                    {
                        oids.Add(Guid.Parse(strOid));
                    }

                    XPCollection<POSKeysLayout> posKeysLayouts = GetList<POSKeysLayout>(uow, new InOperator("Oid", oids));
                    using (MemoryStream ms = new MemoryStream())
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> items = new List<string>();
                        foreach (POSKeysLayout keysLayout in posKeysLayouts)
                        {
                            items.Add(keysLayout.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true));
                            foreach (POSKeyMapping keyMapping in keysLayout.POSKeyMappings)
                            {
                                items.Add(keyMapping.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true));
                            }
                        }
                        sr.WriteLine(JsonConvert.SerializeObject(items, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "keymappings.txt");
                    }
                }
                else
                {
                    return null;
                }
            }

        }


        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult KeyMappingsUploadForm()
        {
            return PartialView("ImportKeyMappingsUpload");
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] {  ".txt" },
            MaxFileSize = 200971520
        };


        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, KeyMappingsUpload_FileUploadComplete);
            return null;
        }



        public static void KeyMappingsUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                try
                {
                    ImportKeysLayoutFromFile(e.UploadedFile.FileContent);
                    e.CallbackData = "success";
                }
                catch(Exception ex)
                {
                    e.CallbackData = "failure";
                    e.ErrorText = ex.Message;
                }
            }
        }
                
        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid posKeysLayoutGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            ViewData["EditMode"] = true;

            if (posKeysLayoutGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (posKeysLayoutGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            POSKeysLayout posKeysLayout;
            if (Session["UnsavedPOSKeysLayout"] == null)
            {
                if (posKeysLayoutGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditPOSKeysLayout;
                    posKeysLayout = uow.FindObject<POSKeysLayout>(new BinaryOperator("Oid", posKeysLayoutGuid, BinaryOperatorType.Equal));
                    Session["IsNewPOSKeysLayout"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewPOSKeysLayout;
                    posKeysLayout = new POSKeysLayout(uow);
                    Session["IsNewPOSKeysLayout"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (posKeysLayoutGuid != Guid.Empty && (Session["UnsavedPOSKeysLayout"] as POSKeysLayout).Oid == posKeysLayoutGuid)
                {
                    Session["IsRefreshed"] = true;
                    posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                }
                else if (posKeysLayoutGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    posKeysLayout = uow.FindObject<POSKeysLayout>(new BinaryOperator("Oid", posKeysLayoutGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["POSKeysLayoutGuid"] = posKeysLayout.Oid.ToString();
            Session["UnsavedPOSKeysLayout"] = posKeysLayout;

            User currentUser = CurrentUser;
            return PartialView("Edit", posKeysLayout);
        }

        public JsonResult Save()
        {

            GenerateUnitOfWork();
            Guid posKeysLayoutGuid = Guid.Empty;

            bool correctPOSKeysLayoutGuid = Request["POSKeysLayoutGuid"] != null && Guid.TryParse(Request["POSKeysLayoutGuid"].ToString(), out posKeysLayoutGuid);
            if (correctPOSKeysLayoutGuid)
            {
                POSKeysLayout posKeysLayout = (Session["UnsavedPOSKeysLayout"] as POSKeysLayout);
                if (posKeysLayout != null)
                {
                    posKeysLayout.Code = Request["Code"];
                    posKeysLayout.Description = Request["Description"];
                    try
                    {
                        AssignOwner<POSKeysLayout>(posKeysLayout);
                        UpdateLookupObjects(posKeysLayout);
                        posKeysLayout.Save();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                        return Json(new { error = Session["Error"]});
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewPOSKeysLayout"] = null;
                        Session["uow"] = null;
                        Session["UnsavedPOSKeysLayout"] = null;
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
                Session["IsNewPOSKeysLayout"] = null;
                Session["UnsavedPOSKeysLayout"] = null;

            }
            return null;
        }



        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
            ViewBag.KeyCodes = Enum.GetValues(typeof(Keys)).OfType<Keys>().Where(x=> x != Keys.None); //remove None

            ViewBag.ExternalActions = POSHelper.GetExternalActions().OrderBy(x => x.ToLocalizedString()).ToDictionary(x => x, y => y.ToLocalizedString());
        }

        public ActionResult POSKeyMappingGrid(string POSKeysLayoutGuid, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (/*editMode == null ||*/ editMode == true)  //edit mode
            {

                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    POSKeyMapping tda = new POSKeyMapping(uow);
                    Session["UnsavedPOSKeyMapping"] = tda;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedPOSKeyMapping"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    POSKeysLayout posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                    foreach (POSKeyMapping pkm in posKeysLayout.POSKeyMappings)
                    {
                        Guid POSKeyMappingID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (pkm.Oid == POSKeyMappingID)
                        {
                            Session["UnsavedPOSKeyMapping"] = pkm;
                            break;
                        }
                    }
                }

                return PartialView("POSKeyMappingsGrid", ((POSKeysLayout)Session["UnsavedPOSKeysLayout"]).POSKeyMappings);
            }
            else  //view mode
            {
                Guid POSKeysLayoutGuidParsed = (POSKeysLayoutGuid == null || POSKeysLayoutGuid == "null" || POSKeysLayoutGuid == "-1") ? Guid.Empty : Guid.Parse(POSKeysLayoutGuid);
                POSKeysLayout posKeysLayout = XpoHelper.GetNewUnitOfWork().FindObject<POSKeysLayout>(new BinaryOperator("Oid", POSKeysLayoutGuidParsed, BinaryOperatorType.Equal));
                ViewData["POSKeysLayoutGuid"] = POSKeysLayoutGuid;
                return PartialView("POSKeyMappingsGrid", posKeysLayout.POSKeyMappings);
            }

        }

        [HttpPost]
        public ActionResult POSKeyMappingAddNew([ModelBinder(typeof(RetailModelBinder))] POSKeyMapping ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    POSKeysLayout posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                    POSKeyMapping posKeyMapping = (POSKeyMapping)Session["UnsavedPOSKeyMapping"];
                    posKeysLayout.POSKeyMappings.Add(posKeyMapping);
                    Session["UnsavedPOSKeysLayout"] = posKeysLayout;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("POSKeyMappingsGrid", ((POSKeysLayout)Session["UnsavedPOSKeysLayout"]).POSKeyMappings);
        }

        [HttpPost]
        public ActionResult POSKeyMappingUpdate([ModelBinder(typeof(RetailModelBinder))] POSKeyMapping ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    POSKeysLayout posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                    POSKeyMapping posKeyMapping = (POSKeyMapping)Session["UnsavedPOSKeyMapping"];
                    posKeysLayout.POSKeyMappings.Remove(posKeyMapping);
                    posKeysLayout.POSKeyMappings.Add(posKeyMapping);
                    Session["UnsavedPOSKeysLayout"] = posKeysLayout;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("POSKeyMappingsGrid", ((POSKeysLayout)Session["UnsavedPOSKeysLayout"]).POSKeyMappings);
        }

        [HttpPost]
        public ActionResult POSKeyMappingDelete([ModelBinder(typeof(RetailModelBinder))] POSKeyMapping ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                POSKeysLayout posKeysLayout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];
                foreach (POSKeyMapping pkm in posKeysLayout.POSKeyMappings)
                {
                    if (pkm.Oid == ct.Oid)
                    {
                        pkm.Delete();
                        //posKeysLayout.POSKeyMappings.Remove(pkm);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("POSKeyMappingsGrid", ((POSKeysLayout)Session["UnsavedPOSKeysLayout"]).POSKeyMappings);

        }

        public ActionResult ActionParametersCallbackPanel()
        {
            try
            {
                string actionCode = Request["DXCallbackArgument"].Split(':')[1];
                eActions action = (eActions)Enum.Parse(typeof(eActions), actionCode);

                Dictionary<string,Type> parameters = action.GetActionKeybindParameters();

                ViewData["ActionParameters"] = parameters;
            }
            catch
            {

            }
            return PartialView();
        }

        public JsonResult jsonCheckActionForKeybindParameters(string ActionCode)
        {
            eActions action = (eActions)Enum.Parse(typeof(eActions), ActionCode);
            if (action.GetActionKeybindParameters().Count > 0)
            {
                return Json(new { hasParameters = true });
            }
            else
            {
                return Json(new { hasParameters = false });
            }
        }


        public JsonResult jsonAddKeyMapping(string ActionCode, string EntryMode,
            string KeyData, string KeyCode, bool? isCtrlChecked, bool? isShiftChecked, bool? isAltChecked,
            string RedirectKeyData, string RedirectKeyCode, bool? isRedirectCtrlChecked, bool? isRedirectShiftChecked, bool? isRedirectAltChecked, string actionParameters)
        {
            try
            {
                eActions action = (eActions)Enum.Parse(typeof(eActions), ActionCode);
                eEntryMode entryMode = (eEntryMode)Enum.Parse(typeof(eEntryMode), EntryMode);
                POSKeysLayout layout = (POSKeysLayout)Session["UnsavedPOSKeysLayout"];

                Keys keyData = Keys.None;
                Keys redirectKeyData = Keys.None;

                if (entryMode == eEntryMode.KeyData)
                {
                    keyData = (Keys)int.Parse(KeyData);
                    redirectKeyData = String.IsNullOrWhiteSpace(RedirectKeyData) ? Keys.None : (Keys)int.Parse(RedirectKeyData);
                }
                else if (entryMode == eEntryMode.Keys)
                {
                    keyData = (Keys)Enum.Parse(typeof(Keys), KeyCode);
                    if (isCtrlChecked.HasValue && (bool)isCtrlChecked)
                    {
                        keyData = keyData | Keys.Control;
                    }
                    if (isShiftChecked.HasValue && (bool)isShiftChecked)
                    {
                        keyData = keyData | Keys.Shift;
                    }
                    if (isAltChecked.HasValue && (bool)isAltChecked)
                    {
                        keyData = keyData | Keys.Alt;
                    }
                    if (!String.IsNullOrWhiteSpace(RedirectKeyCode))
                    {
                        redirectKeyData = (Keys)Enum.Parse(typeof(Keys), RedirectKeyCode);
                        if (isCtrlChecked.HasValue && (bool)isRedirectCtrlChecked)
                        {
                            redirectKeyData = redirectKeyData | Keys.Control;
                        }
                        if (isShiftChecked.HasValue && (bool)isRedirectShiftChecked)
                        {
                            redirectKeyData = redirectKeyData | Keys.Shift;
                        }
                        if (isAltChecked.HasValue && (bool)isRedirectAltChecked)
                        {
                            redirectKeyData = redirectKeyData | Keys.Alt;
                        }
                    }
                }
                //bool keyCodeAlreadyExists = false;
                POSKeyMapping keyMapping = null;
                foreach (POSKeyMapping keymap in layout.POSKeyMappings)
                {
                    if (keymap.KeyData == keyData)
                    {
                        keyMapping = keymap;
                        break;
                    }
                }

                if (keyMapping == null)
                {
                    foreach (POSKeyMapping keymap in layout.Session.GetObjectsToSave().OfType<POSKeyMapping>().ToList()) //Deleted and unsaved with the same key data
                    {
                        if (keymap.KeyData == keyData)
                        {
                            keyMapping = keymap;
                            keyMapping.SetMemberValue("GCRecord", null);
                            break;
                        }
                    }
                }

                if (keyMapping == null)
                {
                    keyMapping = new POSKeyMapping(layout.Session);
                }
                keyMapping.KeyData = keyData;
                layout.POSKeyMappings.Add(keyMapping);
                keyMapping.ActionCode = action;
                keyMapping.NotificationType = action == eActions.NONE ? eNotificationsTypes.KEY : eNotificationsTypes.ACTION;
                if (action == eActions.NONE && redirectKeyData != Keys.None)
                {
                    keyMapping.RedirectTo = redirectKeyData;
                }

                if (action.GetActionKeybindParameters().Count > 0)
                {
                    keyMapping.ActionParameters = actionParameters;
                }
                else
                {
                    keyMapping.ActionParameters = null;
                }

                //}
                //else
                //{
                //    return Json(new { error = Resources.KeyCodeAlreadyExists });
                //}

                return Json(new { success = true, error = ""});
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                return Json(new { success = false, error = Resources.POSKeyDataMissing });
            }
        }
    }
}

//#endif