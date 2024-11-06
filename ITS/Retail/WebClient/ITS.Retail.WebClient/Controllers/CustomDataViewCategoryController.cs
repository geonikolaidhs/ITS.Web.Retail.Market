using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class CustomDataViewCategoryController : BaseObjController<CustomDataViewCategory>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.CustomJSProperties.AddJSProperty("gridName", "grdCustomDataViewCategories");
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "CustomDataViewCategory.ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "CustomDataViewCategory.ImportButtonOnClick";

            return View("Index", GetList<CustomDataViewCategory>(XpoHelper.GetNewUnitOfWork()));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.DetailsToIgnore.Add("DataViews");
            return ruleset;
        }

        public FileContentResult ExportCustomDataViewCategories(string CustomDataViewCategoriesOids)
        {
            IEnumerable<Guid> CustomDataViewCategoriesGuids = null;
            try
            {
                CustomDataViewCategoriesGuids = CustomDataViewCategoriesOids.Split(',').Select(x => Guid.Parse(x));
                XPCollection<CustomDataViewCategory> dataViewCategories = GetList<CustomDataViewCategory>(XpoSession,
                                                                            new InOperator("Oid", CustomDataViewCategoriesGuids));
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> objectsToSerialize = new List<string>();
                        foreach (CustomDataViewCategory category in dataViewCategories)
                        {
                            objectsToSerialize.Add(category.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        }
                        sr.Write(JsonConvert.SerializeObject(objectsToSerialize, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "import_dataViews.txt");
                    }
                }
            }
            catch(Exception ex)
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
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, CustomDataViewsCategoriesUpload_FileUploadComplete);
            return null;
        }

        private void CustomDataViewsCategoriesUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
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
                            JArray jDataViewCategories = JsonConvert.DeserializeObject(jsonData) as JArray;
                            foreach (JToken jCategory in jDataViewCategories)
                            {
                                JObject jObjCategory = JsonConvert.DeserializeObject(jCategory.ToString()) as JObject;
                                if (jObjCategory is JObject)
                                {
                                    CustomDataViewCategory dataViewCategory = new CustomDataViewCategory(uow);
                                    dataViewCategory.FromJson(jObjCategory, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                                    dataViewCategory.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                                    dataViewCategory.Save();
                                }
                            }
                            XpoHelper.CommitChanges(uow);
                        }
                    }    
                }
                catch(Exception ex)
                {
                    Session["Error"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                }                  
            }
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };
    }
}
