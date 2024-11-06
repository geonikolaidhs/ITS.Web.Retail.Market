using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using DevExpress.Data.Filtering;
using System.IO;
using Newtonsoft.Json;
using ITS.Retail.Platform;
using DevExpress.Web;
using Newtonsoft.Json.Linq;

namespace ITS.Retail.WebClient.Controllers
{
    public class CustomEnumerationController : BaseObjController<CustomEnumerationDefinition>
    {
        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.CustomEnumeration;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";         
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;
            ToolbarOptions.ViewButton.Visible = true;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "objectSid");
            CustomJSProperties.AddJSProperty("gridName", "grdCustomEnumeration");
            return View(GetList<CustomEnumerationDefinition>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsActive" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(CustomEnumerationValue), new List<string>() { "IsActive", "CustomEnumerationDefinition" });
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        
        public ActionResult Edit(String Oid)
        {
            Guid objectId;
            if (Guid.TryParse(Oid, out objectId)==false)
            {
                objectId = Guid.Empty;
            }
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            CustomEnumerationDefinition obj;
            if (objectId == Guid.Empty)
            {
                obj = new CustomEnumerationDefinition(uow);
            }
            else
            {
                obj = uow.GetObjectByKey<CustomEnumerationDefinition>(objectId);                
            }
            Session["EditingCustomEnumeration"] = obj;
            return PartialView(obj);
        }

        public ActionResult ValueGrid()
        {
            XPCollection<CustomEnumerationValue> modelValues = (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition)==null ? null : (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition).CustomEnumerationValues;
            return PartialView(modelValues);
        }

        public ActionResult InsertValue([ModelBinder(typeof(RetailModelBinder))] CustomEnumerationValue ct)
        {
            
            CustomEnumerationDefinition customEnumerationDefinition = (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition);
            if (customEnumerationDefinition.CustomEnumerationValues.Select(definition => definition.Ordering).Contains(ct.Ordering))
            {
                ModelState.AddModelError("Ordering", Resources.OrderingAlreadyExists);
            }
            if (ModelState.IsValid)
            {
                CustomEnumerationValue newValue = new CustomEnumerationValue(customEnumerationDefinition.Session);
                newValue.Description = ct.Description;
                newValue.Ordering = ct.Ordering;
                customEnumerationDefinition.CustomEnumerationValues.Add(newValue);
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            return PartialView("ValueGrid",customEnumerationDefinition.CustomEnumerationValues);
        }

        public ActionResult UpdateValue([ModelBinder(typeof(RetailModelBinder))] CustomEnumerationValue ct)
        {

            CustomEnumerationDefinition customEnumerationDefinition = (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition);
            if (customEnumerationDefinition.CustomEnumerationValues.Where(customDefinition=> customDefinition.Oid != ct.Oid).Select(definition => definition.Ordering).Contains(ct.Ordering))
            {
                ModelState.AddModelError("Ordering", Resources.OrderingAlreadyExists);
            }
            if (ModelState.IsValid)
            {
                CustomEnumerationValue newValue = customEnumerationDefinition.CustomEnumerationValues.FirstOrDefault(g => g.Oid == ct.Oid);
                if (newValue != null)
                {
                    newValue.Description = ct.Description;
                    newValue.Ordering = ct.Ordering;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            return PartialView("ValueGrid", customEnumerationDefinition.CustomEnumerationValues);
        }

        public ActionResult DeleteValue([ModelBinder(typeof(RetailModelBinder))] CustomEnumerationValue ct)
        {

            CustomEnumerationDefinition customEnumerationDefinition = (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition);
            if (ModelState.IsValid)
            {
                CustomEnumerationValue newValue = customEnumerationDefinition.CustomEnumerationValues.FirstOrDefault(g => g.Oid == ct.Oid);
                if (newValue != null)
                {
                    newValue.Delete();
                }
            }
            return PartialView("ValueGrid", customEnumerationDefinition.CustomEnumerationValues);
        }
                
        public JsonResult Save()
        {            
            CustomEnumerationDefinition editingObject = (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition);
            editingObject.IsDefault = Request["IsDefaultCheckbox"].Equals("C");
            editingObject.Code = Request["Code"].ToString();
            editingObject.Description = Request["CustomEnumerationDescription"].ToString();
            AssignOwner(editingObject);

            if (HasDuplicate<CustomEnumerationDefinition>(editingObject))
            {
                ModelState.AddModelError("Code", Resources.CodeAlreadyExists);
                Session["Error"] += Resources.CodeAlreadyExists;
                return Json(new { error = Session["Error"] });
            }


            if (HasIsDefaultDuplicates(editingObject))
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
                Session["Error"] += Resources.DefaultAllreadyExists;
                return Json(new { error = Session["Error"] });
                
            }
            editingObject.Save();
            XpoHelper.CommitChanges(editingObject.Session as UnitOfWork);//XpoHelper.CommitChanges(Session["CustomEnumerationUow"] as UnitOfWork);
            return Json(new { });   
        }


        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if ((Session["EditingCustomEnumeration"] as UnitOfWork) != null)
            {
                (Session["EditingCustomEnumeration"] as CustomEnumerationDefinition).Session.Dispose();
            }
            Session["EditingCustomEnumeration"] = /*Session["CustomEnumerationUow"]= */null;
            return null;
        }

        public FileContentResult Export(string HeadersGuids)
        {
            IEnumerable<Guid> HeadersGuidsArray = null;
            try
            {
                HeadersGuidsArray = HeadersGuids.Split(',').Select(x => Guid.Parse(x));
            }
            catch
            {
                HeadersGuidsArray = null;
            }
            if (HeadersGuidsArray != null && HeadersGuidsArray.Count() > 0)
            {
                XPCollection<CustomEnumerationDefinition> heads = GetList<CustomEnumerationDefinition>(XpoSession, new InOperator("Oid", HeadersGuidsArray));
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sr = new StreamWriter(ms))
                {
                    sr.Write(JsonConvert.SerializeObject(heads.Select(x => x.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)).ToList(), PlatformConstants.JSON_SERIALIZER_SETTINGS));
                    sr.Flush();
                    return File(ms.ToArray(), "text/plain", "CustomEnumeration.txt");
                }
            }
            return null;
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
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, DevicesUpload_FileUploadComplete);
            return null;
        }

        private void DevicesUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                using (StreamReader reader = new StreamReader(e.UploadedFile.FileContent))
                {
                    String jsonData = reader.ReadToEnd();
                    string error;
                    var jArray = JsonConvert.DeserializeObject(jsonData) as JArray;
                    foreach (var obj in jArray)
                    {
                        JObject jobj = JsonConvert.DeserializeObject(obj.ToString()) as JObject;
                        if (jobj is JObject)
                        {
                            CustomEnumerationDefinition head = new CustomEnumerationDefinition(uow);
                            head.FromJson(jobj as JObject, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                            head.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                            head.Save();
                        }
                    }
                    XpoHelper.CommitChanges(uow);
                }
            }
        }
    }
}
