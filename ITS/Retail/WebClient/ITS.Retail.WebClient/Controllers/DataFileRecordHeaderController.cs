using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using System.Reflection;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using System.IO;
using Newtonsoft.Json;
using ITS.Retail.Platform;
using DevExpress.Web;
using Newtonsoft.Json.Linq;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class DataFileRecordHeaderController : BaseObjController<DataFileRecordHeader>
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

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.Add("DecocedData");
            ruleset.PropertiesToIgnore.Add("Description");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.DetailPropertiesToIgnore.Add(typeof(DataFileRecordDetail), new List<string>() { "IsDefault", "CreatedOn", "UpdatedOn", "IsActive" });
            ruleset.NumberOfColumns = 3;
            return ruleset;
        }


        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.BridgeSettings;

            return PartialView("LoadEditPopup");
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            GenerateUnitOfWork();

            this.ToolbarOptions.ExportToButton.Visible = false;

            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";

            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "ImportButtonOnClick";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "DataFileRecordHeaderID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdDataFileRecordHeaders");


            return View("Index", GetList<DataFileRecordHeader>(uow).AsEnumerable<DataFileRecordHeader>());
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
            if(HeadersGuidsArray != null && HeadersGuidsArray.Count() > 0 )
            {
                XPCollection<DataFileRecordHeader> heads = GetList<DataFileRecordHeader>(XpoSession, new InOperator("Oid", HeadersGuidsArray));
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sr = new StreamWriter(ms))
                {
                    sr.Write(JsonConvert.SerializeObject(heads.Select(x => x.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)).ToList(), PlatformConstants.JSON_SERIALIZER_SETTINGS));
                    sr.Flush();
                    return File(ms.ToArray(), "text/plain", "import_settings.txt");
                }
            }
            return null;
        }

        public ActionResult Edit(string Oid)
        {

            GenerateUnitOfWork();
            Guid DataFileRecordHeaderGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            if (DataFileRecordHeaderGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (DataFileRecordHeaderGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            DataFileRecordHeader DataFileRecordHeader;
            ViewData["EditMode"] = true;
            if (Session["UnsavedDataFileRecordHeader"] == null)
            {
                if (DataFileRecordHeaderGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditDataFileRecordHeader;
                    DataFileRecordHeader = uow.FindObject<DataFileRecordHeader>(new BinaryOperator("Oid", DataFileRecordHeaderGuid, BinaryOperatorType.Equal));
                    Session["IsNewDataFileRecordHeader"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewDataFileRecordHeader;
                    DataFileRecordHeader = new DataFileRecordHeader(uow);
                    DataFileRecordHeader.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                    Session["IsNewDataFileRecordHeader"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (DataFileRecordHeaderGuid != Guid.Empty && (Session["UnsavedDataFileRecordHeader"] as DataFileRecordHeader).Oid == DataFileRecordHeaderGuid)
                {
                    Session["IsRefreshed"] = true;
                    DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                }
                else if (DataFileRecordHeaderGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    DataFileRecordHeader = uow.FindObject<DataFileRecordHeader>(new BinaryOperator("Oid", DataFileRecordHeaderGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["DataFileRecordHeaderID"] = DataFileRecordHeader.Oid.ToString();
            Session["UnsavedDataFileRecordHeader"] = DataFileRecordHeader;
            ViewBag.KeyPropertyComboBox = DataFileRecordHeader.DataFileRecordDetails;
            ViewBag.ReferencePropertyComboBox = DataFileRecordHeader.DataFileRecordDetails;

            if ((bool)Session["IsNewDataFileRecordHeader"] == true)
            {
                ViewData["UserSupplier"] = Session["currentSupplier"]; //Used for combobox binding
                (Session["UnsavedDataFileRecordHeader"] as DataFileRecordHeader).Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner == null ? Guid.Empty : EffectiveOwner.Oid);// Session["currentSupplier"] as CompanyNew == null ? null : uow.GetObjectByKey<CompanyNew>((Session["currentSupplier"] as CompanyNew).Oid);
            }
            else
            {
                ViewData["UserSupplier"] = (Session["UnsavedDataFileRecordHeader"] as DataFileRecordHeader).Owner; //Used for combobox binding
                ViewData["EnableOwnersComboBox"] = false;
            }
            return PartialView("Edit", DataFileRecordHeader);
        }

        protected void FillDetailLookupComboBoxes(DataFileRecordHeader header)
        {
            IEnumerable<PropertyInfo> tempRowProperties = typeof(DecodedRawData).GetProperties().Where(x =>
            {
                IEnumerable<ImportEntityAttribute> attributes = x.GetCustomAttributes(typeof(ImportEntityAttribute), true).Cast<ImportEntityAttribute>();
                return attributes.Count() > 0 && attributes.Where(att => att.EntityType.Name == header.EntityName).Count() > 0;
            });

            List<string> existing = header.DataFileRecordDetails.Select(x => x.PropertyName).ToList();
            List<string> selectedTempRowProperties = tempRowProperties.Where(x => existing.Contains(x.Name) == false).Select(x => x.Name).ToList();
            if (existing.Contains("IsActive") == false)
            {
                selectedTempRowProperties.Add("IsActive");
            }

            ViewBag.PropertyNameComboBox = selectedTempRowProperties.OrderBy(x => x).ToList();
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            Dictionary<string, string> localizedEntityNames = new Dictionary<string, string>();
            IEnumerable<string> entities = typeof(DecodedRawData).GetProperties().SelectMany(prop => prop.GetCustomAttributes(typeof(ImportEntityAttribute), true).Select(att => (att as ImportEntityAttribute).EntityType.Name)).Distinct();
            foreach (string entity in entities)
            {
                Type entityType = typeof(Item).Assembly.GetType(typeof(Item).FullName.Replace(typeof(Item).Name, entity));
                if (entityType != null)
                {
                    localizedEntityNames.Add(entityType.Name,entityType.ToLocalizedString());
                }
            }
            ViewBag.EntityComboBox = localizedEntityNames;
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid DataFileRecordHeaderGuid = Guid.Empty;

            bool correctDataFileRecordHeaderGuid = Request["DataFileRecordHeaderID"] != null && Guid.TryParse(Request["DataFileRecordHeaderID"].ToString(), out DataFileRecordHeaderGuid);
            if (correctDataFileRecordHeaderGuid)
            {
                DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                if (DataFileRecordHeader != null)
                {
                    DataFileRecordHeader.EntityName = Request["EntityName_VI"];
                    DataFileRecordHeader.HeaderCode = Request["HeaderCode"];

                    DataFileRecordHeader.KeyProperty = null;
                    Guid keyDetailGuid = (Request.Params["KeyProperty_VI"] == null || Request.Params["KeyProperty_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["KeyProperty_VI"]);
                    foreach (DataFileRecordDetail detail in DataFileRecordHeader.DataFileRecordDetails)
                    {
                        if (detail.Oid == keyDetailGuid)
                        {
                            DataFileRecordHeader.KeyProperty = detail;
                            break;
                        }
                    }

                    //ReferenceProperty
                    Guid referencePropertyGuid = Request.Params["ReferenceProperty_VI"] == null || Request.Params["ReferenceProperty_VI"] == ""
                                                 ? Guid.Empty
                                                 : Guid.Parse(Request.Params["ReferenceProperty_VI"]);
                    DataFileRecordHeader.ReferenceProperty = DataFileRecordHeader.DataFileRecordDetails.FirstOrDefault( dataFileRecordDetail => dataFileRecordDetail.Oid == referencePropertyGuid);

                    int length = 0;
                    int.TryParse(Request["Length"], out length);
                    DataFileRecordHeader.Length = length;

                    int order = 0;
                    int.TryParse(Request["Order"], out order);
                    DataFileRecordHeader.Order = order;

                    int position = 0;
                    int.TryParse(Request["Position"], out position);
                    DataFileRecordHeader.Position = position;

                    if (Request["IsCharacterDelimitedCheckbox"] != null || Request["IsCharacterDelimitedCheckbox"] != "null")
                    {
                        if (Request["IsCharacterDelimitedCheckbox"].ToString().Equals("C"))
                        {
                            DataFileRecordHeader.IsTabDelimited = true;
                        }
                        else if (Request["IsCharacterDelimitedCheckbox"].ToString().Equals("U"))
                        {
                            DataFileRecordHeader.IsTabDelimited = false;
                        }
                    }

                    DataFileRecordHeader.TabDelimitedString = Request["TabDelimitedString"];

                    AssignOwner<DataFileRecordHeader>(DataFileRecordHeader);
                    UpdateLookupObjects(DataFileRecordHeader);
                    DataFileRecordHeader.Save();
                    XpoHelper.CommitTransaction(uow);
                    Session["IsNewDataFileRecordHeader"] = null;
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                    Session["UnsavedDataFileRecordHeader"] = null;
                    Session["IsRefreshed"] = null;

                }
            }
            return Json(new {});
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (!Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewDataFileRecordHeader"] = null;
                Session["UnsavedDataFileRecordHeader"] = null;
            }
            return null;
        }

        public JsonResult jsonEntityChanged(string EntityName)
        {
            if (Session["UnsavedDataFileRecordHeader"] != null && Session["UnsavedDataFileRecordHeader"] is DataFileRecordHeader)
            {
                ((DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"]).EntityName = EntityName;
            }
            return Json(new { });
        }

        public ActionResult DataFileRecordDetailGrid(string DataFileRecordHeaderID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();

                //FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedDataFileRecordDetail"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid DataFileRecordDetailID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                    foreach (DataFileRecordDetail DataFileRecordDetail in DataFileRecordHeader.DataFileRecordDetails)
                    {
                        if (DataFileRecordDetail.Oid == DataFileRecordDetailID)
                        {
                            Session["UnsavedDataFileRecordDetail"] = DataFileRecordDetail;
                            break;
                        }
                    }
                }

                DataFileRecordHeader dataFileRecordHeader = ((DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"]);
                ViewBag.KeyPropertyComboBox = dataFileRecordHeader.DataFileRecordDetails;
                ViewBag.ReferencePropertyComboBox = dataFileRecordHeader.DataFileRecordDetails;
                FillDetailLookupComboBoxes(dataFileRecordHeader);
                return PartialView("DataFileRecordDetailGrid", dataFileRecordHeader.DataFileRecordDetails);
            }
            else  //view mode
            {
                Guid DataFileRecordHeaderGuid = Guid.Parse(DataFileRecordHeaderID);
                DataFileRecordHeader DataFileRecordHeader = XpoHelper.GetNewUnitOfWork().FindObject<DataFileRecordHeader>(new BinaryOperator("Oid", DataFileRecordHeaderGuid, BinaryOperatorType.Equal));
                ViewData["DataFileRecordHeaderID"] = DataFileRecordHeaderID;

                return PartialView("DataFileRecordDetailGrid", DataFileRecordHeader.DataFileRecordDetails);
            }
        }


        public ActionResult InsertDataFileRecordDetail([ModelBinder(typeof(RetailModelBinder))] DataFileRecordDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                    FillDetailLookupComboBoxes(DataFileRecordHeader);
                    DataFileRecordDetail DataFileRecordDetail = new DataFileRecordDetail(uow);
                    bool Padding = bool.Parse(Request["Padding"]);
                    bool Trim = bool.Parse(Request["Trim"]);
                    bool AllowNull = bool.Parse(Request["AllowNull"]);
                    bool UseThirdPartNum = bool.Parse(Request["UseThirdPartNum"]);

                    DataFileRecordDetail.DefaultValue = ct.DefaultValue;
                    int length = 0;
                    int.TryParse(Request["DetailLength"], out length);
                    DataFileRecordDetail.Length = length;
                    DataFileRecordDetail.Padding = Padding;
                    DataFileRecordDetail.PaddingCharacter = ct.PaddingCharacter;
                    int position = 0;
                    int.TryParse(Request["DetailPosition"], out position);
                    DataFileRecordDetail.Position = position;

                    double multiplier = 0;
                    double.TryParse(Request["MultiplierSpinEdit"], out multiplier);
                    DataFileRecordDetail.Multiplier = multiplier;

                    DataFileRecordDetail.PropertyName = ct.PropertyName;
                    DataFileRecordDetail.Trim = Trim;
                    DataFileRecordDetail.AllowNull = AllowNull;
                    DataFileRecordDetail.UseThirdPartNum = UseThirdPartNum;
                    DataFileRecordDetail.ConstantValue = Request["ConstantValue"];

                    DataFileRecordHeader.DataFileRecordDetails.Add(DataFileRecordDetail);
                    Session["UnsavedDataFileRecordHeader"] = DataFileRecordHeader;
                    Session["UnsavedDataFileRecordDetail"] = null;

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

            //FillLookupComboBoxes();
            return PartialView("DataFileRecordDetailGrid", ((DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"]).DataFileRecordDetails);
        }

        [HttpPost]
        public ActionResult UpdateDataFileRecordDetail([ModelBinder(typeof(RetailModelBinder))] DataFileRecordDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                    FillDetailLookupComboBoxes(DataFileRecordHeader);
                    DataFileRecordDetail DataFileRecordDetail = (DataFileRecordDetail)Session["UnsavedDataFileRecordDetail"];
                    bool Padding = bool.Parse(Request["Padding"]);
                    bool Trim = bool.Parse(Request["Trim"]);
                    bool AllowNull = bool.Parse(Request["AllowNull"]);
                    bool UseThirdPartNum = bool.Parse(Request["UseThirdPartNum"]);

                    DataFileRecordDetail.DefaultValue = ct.DefaultValue;
                    int length = 0;
                    int.TryParse(Request["DetailLength"], out length);
                    DataFileRecordDetail.Length = length;
                    DataFileRecordDetail.Padding = Padding;
                    DataFileRecordDetail.PaddingCharacter = ct.PaddingCharacter;

                    int position = 0;
                    int.TryParse(Request["DetailPosition"], out position);
                    DataFileRecordDetail.Position = position;

                    double multiplier = 0;
                    double.TryParse(Request["MultiplierSpinEdit"], out multiplier);
                    DataFileRecordDetail.Multiplier = multiplier;

                    DataFileRecordDetail.PropertyName = ct.PropertyName;
                    DataFileRecordDetail.Trim = Trim;
                    DataFileRecordDetail.ConstantValue = Request["ConstantValue"];
                    DataFileRecordDetail.AllowNull = AllowNull;
                    DataFileRecordDetail.UseThirdPartNum = UseThirdPartNum;
                    
                    DataFileRecordHeader.DataFileRecordDetails.Remove(DataFileRecordDetail);
                    DataFileRecordHeader.DataFileRecordDetails.Add(DataFileRecordDetail);
                    Session["UnsavedDataFileRecordHeader"] = DataFileRecordHeader;
                    Session["UnsavedDataFileRecordDetail"] = null;
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
            return PartialView("DataFileRecordDetailGrid", ((DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"]).DataFileRecordDetails);
        }

        [HttpPost]
        public ActionResult DeleteDataFileRecordDetail([ModelBinder(typeof(RetailModelBinder))] DataFileRecordDetail ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
                FillDetailLookupComboBoxes(DataFileRecordHeader);
                foreach (DataFileRecordDetail DataFileRecordDetail in DataFileRecordHeader.DataFileRecordDetails)
                {
                    if (DataFileRecordDetail.Oid == ct.Oid)
                    {
                        DataFileRecordHeader.DataFileRecordDetails.Remove(DataFileRecordDetail);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }

            return PartialView("DataFileRecordDetailGrid", ((DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"]).DataFileRecordDetails);
        }


        public ActionResult KeyPropertyComboBox()
        {
            GenerateUnitOfWork();
            DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
            if (DataFileRecordHeader != null && DataFileRecordHeader.DataFileRecordDetails != null)
            {
                ViewBag.KeyPropertyComboBox = DataFileRecordHeader.DataFileRecordDetails;
            }
            Guid currentKeyPropertyID;
            if (Session["CurrentKeyProperty"] != null && Guid.TryParse(Session["CurrentKeyProperty"].ToString(), out currentKeyPropertyID))
            {
                return PartialView("KeyPropertyComboBox", DataFileRecordHeader.DataFileRecordDetails.FirstOrDefault(dataFileRecordDetail=> dataFileRecordDetail.Oid == currentKeyPropertyID));
            }
            return PartialView("KeyPropertyComboBox", null);
        }

        public ActionResult PropertyNameComboBox()
        {
            FillLookupComboBoxes();
            return PartialView();
        }

        public ActionResult EntityComboBox()
        {
            FillLookupComboBoxes();
            return PartialView();
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
            if(e.UploadedFile.IsValid)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
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
                                DataFileRecordHeader dataFileRecordHeader = new DataFileRecordHeader(uow);
                                dataFileRecordHeader.FromJson(jobj as JObject, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                                dataFileRecordHeader.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                                dataFileRecordHeader.Save();
                            }
                        }
                        XpoHelper.CommitChanges(uow);
                    }
                }
            }
        }


        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };


        public ActionResult ReferencePropertyComboBox()
        {
            GenerateUnitOfWork();
            DataFileRecordHeader DataFileRecordHeader = (DataFileRecordHeader)Session["UnsavedDataFileRecordHeader"];
            if (DataFileRecordHeader != null && DataFileRecordHeader.DataFileRecordDetails != null)
            {
                ViewBag.ReferencePropertyComboBox = DataFileRecordHeader.DataFileRecordDetails;
            }
            Guid referencePropertyID;
            if (Session["ReferenceProperty"] != null && Guid.TryParse(Session["ReferenceProperty"].ToString(), out referencePropertyID))
            {
                return PartialView(DataFileRecordHeader.DataFileRecordDetails.FirstOrDefault(dataFileRecordDetail => dataFileRecordDetail.Oid == referencePropertyID));
            }
            return PartialView(null);
        }
    }
}
