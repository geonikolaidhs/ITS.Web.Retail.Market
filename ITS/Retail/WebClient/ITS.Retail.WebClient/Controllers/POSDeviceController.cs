//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using DevExpress.Web.Mvc;
using System.IO.Ports;
using System.IO;
using Newtonsoft.Json;
using DevExpress.Web;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations.Attributes;
using ITS.Retail.WebClient.Providers;
using System.Linq;
using ITS.Retail.WebClient.Helpers;
using System.Threading;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSDeviceController : BaseObjController<POSDevice>
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
            this.CustomJSProperties.AddJSProperty("editIDParameter", "POSDeviceGuid");
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSDevices");

            GenerateUnitOfWork();
            FillLookupComboBoxes();
            return View("Index", GetList<POSDevice>(uow, new BinaryOperator("Oid", Guid.Empty)));
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
                    string fconnectionType = Request["fconnectionType"] == null || Request["fconnectionType"] == "null" ? "" : Request["fconnectionType"];
                    string fname = Request["fname"] == null || Request["fname"] == "null" ? "" : Request["fname"];
                    string fdeviceType = Request["fdeviceType"] == null || Request["fdeviceType"] == "null" ? "" : Request["fdeviceType"];

                    CriteriaOperator nameFilter = null;
                    if (fname != null && fname.Trim() != "")
                    {
                        if (fname.Replace('%', '*').Contains("*"))
                            nameFilter = new BinaryOperator("Name", fname.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            nameFilter = new BinaryOperator("Name", fname, BinaryOperatorType.Equal);
                    }

                    ConnectionType connectionType;
                    CriteriaOperator connectionTypeFilter = null;
                    if (Enum.TryParse<ConnectionType>(fconnectionType, true, out connectionType))
                    {
                        connectionTypeFilter = new BinaryOperator("ConnectionType", connectionType);
                    }

                    DeviceType deviceType;
                    CriteriaOperator deviceTypeFilter = null;
                    if (Enum.TryParse<DeviceType>(fdeviceType, true, out deviceType))
                    {
                        connectionTypeFilter = new BinaryOperator("DeviceSettings.DeviceType", deviceType);
                    }


                    filter = CriteriaOperator.And(nameFilter, connectionTypeFilter, deviceTypeFilter);
                    Session["POSDeviceFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["POSDeviceFilter"] = filter;
                }
            }
            GridFilter = (CriteriaOperator)Session["POSDeviceFilter"];
            return base.Grid();

        }

        public ActionResult ImportDevicesUpload()
        {
            return PartialView();
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult POSDevicesUploadForm()
        {
            return PartialView("ImportDevicesUpload");
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, DevicesUpload_FileUploadComplete);
            return null;
        }

        public static void DevicesUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                try
                {
                    ImportDevicesFromFile(e.UploadedFile.FileContent);
                    e.CallbackData = "success";
                }
                catch (Exception ex)
                {
                    e.CallbackData = "failure";
                    e.ErrorText = ex.Message;
                }
            }
        }


        public override ActionResult Dialog(List<string> arguments)
        {
            if (arguments.Contains("DEVICE_DATABASE_PROCESS_DIALOG"))
            {
                this.DialogOptions.OKButton.Visible = false;
                this.DialogOptions.OKButton.OnClick = "function (s,e) { Dialog.Hide(); }";
                this.DialogOptions.BodyPartialView = "POSDeviceDatabaseCreationDialog";
                this.DialogOptions.HeaderText = Resources.CreatePOSDeviceDatabase;
                this.DialogOptions.AdjustSizeOnInit = true;
                this.DialogOptions.CancelButton.Visible = false;
                this.DialogOptions.OnShownEvent = "Dialog_OnShown";
            }
            else  //POS Status command Dialogs
            {
                this.DialogOptions.AdjustSizeOnInit = true;
                this.DialogOptions.HeaderText = Resources.Import;
                this.DialogOptions.BodyPartialView = "ImportDevicesUpload";
                this.DialogOptions.OKButton.OnClick = @"function (s,e) { UploadControl.Upload();}";
                this.DialogOptions.CancelButton.OnClick = "function (s,e) { Dialog.Hide();}";

            }
            return PartialView();
        }
        public JsonResult jsonCheckPOSDeviceDatabaseRunning()
        {
            Session["KeepAlive"] = DateTime.Now;
            if (POSDeviceDBCreationHelper.IsProcessing)
            {
                return Json(new { done = false });
            }
            else
            {
                return Json(new { done = true });
            }
        }
        private static void ImportDevicesFromFile(Stream fileStream)
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
                    string type = jsonItem.Property("Type").Value.ToString();
                    string key = jsonItem.Property("Oid").Value.ToString();
                    Type typeObj = typeof(Item).Assembly.GetType("ITS.Retail.Model." + type);
                    BaseObj currentObject = uow.GetObjectByKey(typeObj, Guid.Parse(key)) as BaseObj;
                    if (currentObject == null)
                    {
                        currentObject = Activator.CreateInstance(typeObj, new object[] { uow }) as BaseObj;
                    }
                    string error;
                    currentObject.FromJson(item, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    currentObject.Save();

                    XpoHelper.CommitChanges(uow);// prepei na ginei commit prin paei sto epomeno
                }
            }
        }

        [Security(OverrideSecurity = true, ReturnsPartial = false)]
        public FileContentResult ExportDevices()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string allOids = Request["POSDeviceGuids"];
                if (!String.IsNullOrWhiteSpace(allOids))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();
                    foreach (string strOid in strOids)
                    {
                        oids.Add(Guid.Parse(strOid));
                    }

                    XPCollection<POSDevice> devices = GetList<POSDevice>(uow, new InOperator("Oid", oids));
                    using (MemoryStream ms = new MemoryStream())
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> items = new List<string>();
                        foreach (POSDevice device in devices)
                        {
                            items.Add(device.DeviceSettings.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true));
                            items.Add(device.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true));
                        }
                        sr.WriteLine(JsonConvert.SerializeObject(items, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "devices.txt");
                    }
                }
                else
                {
                    return null;
                }
            }

        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid posDeviceGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            ViewData["EditMode"] = true;

            if (posDeviceGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (posDeviceGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            POSDevice posDevice;
            if (Session["UnsavedPOSDevice"] == null)
            {
                if (posDeviceGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditDevice;
                    posDevice = uow.FindObject<POSDevice>(new BinaryOperator("Oid", posDeviceGuid, BinaryOperatorType.Equal));
                    Session["IsNewPOSDevice"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewDevice;
                    posDevice = new POSDevice(uow);
                    Session["IsNewPOSDevice"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (posDeviceGuid != Guid.Empty && (Session["UnsavedPOSDevice"] as POSDevice).Oid == posDeviceGuid)
                {
                    Session["IsRefreshed"] = true;
                    posDevice = (POSDevice)Session["UnsavedPOSDevice"];
                }
                else if (posDeviceGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    posDevice = (POSDevice)Session["UnsavedPOSDevice"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    posDevice = uow.FindObject<POSDevice>(new BinaryOperator("Oid", posDeviceGuid, BinaryOperatorType.Equal));
                }
            }
            if (posDevice.DeviceSettings != null)
            {
                ViewData["ConnectionTypes"] = posDevice.DeviceSettings.DeviceType.GetSupportedConnections();
                ViewData["DeviceSpecificTypes"] = GetSupportedDeviceSpecificTypes(posDevice.DeviceSettings.DeviceType).ToArray();
            }

            FillLookupComboBoxes();
            ViewData["POSDeviceGuid"] = posDevice.Oid.ToString();
            Session["UnsavedPOSDevice"] = posDevice;

            XPCollection<BarcodeType> barcodeType = GetList<BarcodeType>(uow);
            Session["CashRegisterBarcodeTypes"] = barcodeType;
            XPCollection<ItemCategory> itemCategory = GetList<ItemCategory>(uow);
            Session["CashRegisterItemCategory"] = itemCategory;
            XPCollection<PriceCatalog> priceCatalog = GetList<PriceCatalog>(uow);
            Session["CashRegisterPriceCatalog"] = priceCatalog;
            XPCollection<VatFactor> vatFactor = GetList<VatFactor>(uow);
            Session["CashRegisterVatFactor"] = vatFactor;
            XPCollection<DocumentType> documentType = GetList<DocumentType>(uow);
            Session["CashRegisterDocumentType"] = documentType;
            XPCollection<DocumentSeries> documentSeries = GetList<DocumentSeries>(uow);
            Session["CashRegisterDocumentSeries"] = documentSeries;
            XPCollection<DocumentStatus> documentStatus = GetList<DocumentStatus>(uow);
            Session["CashRegisterDocumentStatus"] = documentStatus;
            XPCollection<PaymentMethod> paymentMethod = GetList<PaymentMethod>(uow);
            Session["CashRegisterPaymentMethod"] = paymentMethod;
            if (posDevice.BarcodeType != null && posDevice.PriceCatalog != null && posDevice.PriceCatalog != null)
            {
                ViewData["IsCashRegister"] = 1;
            }
            else
            {
                ViewData["IsCashRegister"] = 0;
            }
            //posDevice.DeviceSettings.DeviceType.GetType()
            if (posDevice.DeviceSettings != null)
            {

                DeviceType deviceType = posDevice.DeviceSettings.DeviceType;
                var member = deviceType.GetType().GetMember(posDevice.DeviceSettings.DeviceType.ToString());
                IsCashRegisterAttribute[] attribute = member[0].GetCustomAttributes(typeof(IsCashRegisterAttribute), false) as IsCashRegisterAttribute[];
                if (attribute != null && attribute.Length > 0)
                {
                    ViewData["IsCashRegister"] = 1;
                }
                else
                {
                    ViewData["IsCashRegister"] = 0;
                }
                ViewData["DeviceSpecificTypes"] = GetSupportedDeviceSpecificTypes(deviceType).ToArray();
            }
            return PartialView("Edit", posDevice);
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid posDeviceGuid = Guid.Empty;

            bool correctPOSDeviceGuid = Request["POSDeviceGuid"] != null && Guid.TryParse(Request["POSDeviceGuid"].ToString(), out posDeviceGuid);
            if (correctPOSDeviceGuid)
            {
                POSDevice posDevice = (Session["UnsavedPOSDevice"] as POSDevice);
                if (posDevice != null)
                {
                    //For CashRegister Device
                    Guid barcodeTypeOid = Guid.Empty;
                    Guid.TryParse(Request["BarcodeType_VI"], out barcodeTypeOid);
                    posDevice.BarcodeType = posDevice.Session.GetObjectByKey<BarcodeType>(barcodeTypeOid);

                    Guid ItemCategoryOid = Guid.Empty;
                    Guid.TryParse(Request["ItemCategory_VI"], out ItemCategoryOid);
                    posDevice.ItemCategory = posDevice.Session.GetObjectByKey<ItemCategory>(ItemCategoryOid);

                    Guid PriceCatalogOid = Guid.Empty;
                    Guid.TryParse(Request["PriceCatalog_VI"], out PriceCatalogOid);
                    posDevice.PriceCatalog = posDevice.Session.GetObjectByKey<PriceCatalog>(PriceCatalogOid);

                    Guid DocumentTypeOid = Guid.Empty;
                    Guid.TryParse(Request["DocumentType_VI"], out DocumentTypeOid);
                    posDevice.DocumentType = posDevice.Session.GetObjectByKey<DocumentType>(DocumentTypeOid);

                    Guid DocumentSeriesOid = Guid.Empty;
                    Guid.TryParse(Request["DocumentSeries_VI"], out DocumentSeriesOid);
                    posDevice.DocumentSeries = posDevice.Session.GetObjectByKey<DocumentSeries>(DocumentSeriesOid);

                    Guid DocumentStatusOid = Guid.Empty;
                    Guid.TryParse(Request["DocumentStatus_VI"], out DocumentStatusOid);
                    posDevice.DocumentStatus = posDevice.Session.GetObjectByKey<DocumentStatus>(DocumentStatusOid);

                    Guid PaymentMethodOid = Guid.Empty;
                    Guid.TryParse(Request["PaymentMethod_VI"], out PaymentMethodOid);
                    posDevice.PaymentMethod = posDevice.Session.GetObjectByKey<PaymentMethod>(PaymentMethodOid);

                    int maxItems = 0;
                    int.TryParse(Request["MaxItemsAdd"], out maxItems);
                    posDevice.MaxItemsAdd = maxItems;
                    //-----------------------
                    posDevice.ConnectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), Request["ConnectionType_VI"]);
                    posDevice.Name = Request["Name"];
                    posDevice.DeviceSpecificType = (eDeviceSpecificType)Enum.Parse(typeof(eDeviceSpecificType), String.IsNullOrWhiteSpace(Request["DeviceSpecificType_VI"]) ? eDeviceSpecificType.None.ToString() : Request["DeviceSpecificType_VI"]);
                    DeviceType deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), Request["DeviceType_VI"]);
                    if (posDevice.DeviceSettings != null)
                    {
                        DeviceSettings oldSettings = posDevice.DeviceSettings;
                        oldSettings.Delete();
                        posDevice.DeviceSettings = null;
                    }

                    try
                    {
                        switch (posDevice.ConnectionType)
                        {
                            case ConnectionType.NONE:
                            case ConnectionType.OPERATING_SYSTEM_DRIVER:
                            case ConnectionType.EMULATED:
                                posDevice.DeviceSettings = new DeviceSettings(uow);
                                break;
                            case ConnectionType.COM:
                                switch (deviceType)
                                {
                                    case DeviceType.Scale:
                                        posDevice.DeviceSettings = new COMScaleSettings(uow);
                                        ScaleCommunicationType scaleCommunicationType;
                                        if (Enum.TryParse(Request["CommunicationType"], out scaleCommunicationType))
                                        {
                                            (posDevice.DeviceSettings as COMScaleSettings).CommunicationType = scaleCommunicationType;
                                        }
                                        (posDevice.DeviceSettings as COMScaleSettings).ScaleReadPattern = Request["ScaleReadPattern"];
                                        break;
                                    default:
                                        posDevice.DeviceSettings = new COMDeviceSettings(uow);
                                        break;
                                }

                                (posDevice.DeviceSettings as COMDeviceSettings).PortName = Request["COMPortName"];

                                int baudRate;
                                if (Int32.TryParse(Request["BaudRate"], out baudRate))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).BaudRate = baudRate;
                                }
                                int dataBits;
                                if (Int32.TryParse(Request["DataBits"], out dataBits))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).DataBits = dataBits;
                                }
                                Handshake handshake;
                                if (Enum.TryParse(Request["Handshake"], out handshake))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).Handshake = handshake;
                                }

                                Parity parity;
                                if (Enum.TryParse(Request["Parity"], out parity))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).Parity = parity;
                                }

                                StopBits stopBits;
                                if (Enum.TryParse(Request["StopBits"], out stopBits))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).StopBits = stopBits;
                                }

                                int writeTimeOut;
                                if (Int32.TryParse(Request["WriteTimeOut"], out writeTimeOut))
                                {
                                    (posDevice.DeviceSettings as COMDeviceSettings).WriteTimeOut = writeTimeOut;
                                }
                                break;
                            case ConnectionType.LPT:
                                posDevice.DeviceSettings = new LPTDeviceSettings(uow);

                                (posDevice.DeviceSettings as LPTDeviceSettings).PortName = Request["LPTPortName"];
                                break;
                            case ConnectionType.OPOS:
                                switch (deviceType)
                                {
                                    case DeviceType.Printer:
                                        posDevice.DeviceSettings = new OPOSPrinterSettings(uow);
                                        (posDevice.DeviceSettings as OPOSPrinterSettings).LogicalDeviceName = Request["LogicalDeviceName"];
                                        PrinterLogoLocation logoLocation;
                                        if (Enum.TryParse(Request["LogoLocation"], out logoLocation))
                                        {
                                            (posDevice.DeviceSettings as OPOSPrinterSettings).LogoLocation = logoLocation;
                                        }

                                        (posDevice.DeviceSettings as OPOSPrinterSettings).LogoText = Request["LogoText"];

                                        PrinterStation printerStation;
                                        if (Enum.TryParse(Request["PrinterStation"], out printerStation))
                                        {
                                            (posDevice.DeviceSettings as OPOSPrinterSettings).PrinterStation = printerStation;
                                        }

                                        break;
                                    default:
                                        posDevice.DeviceSettings = new OPOSDeviceSettings(uow);
                                        (posDevice.DeviceSettings as OPOSDeviceSettings).LogicalDeviceName = Request["LogicalDeviceName"];
                                        break;
                                }
                                break;
                            case ConnectionType.ETHERNET:
                                posDevice.DeviceSettings = new EthernetDeviceSettings(uow);

                                (posDevice.DeviceSettings as EthernetDeviceSettings).IPAddress = Request["IPAddress"];
                                int portValue;
                                if (Int32.TryParse(Request["Port"], out portValue))
                                {
                                    (posDevice.DeviceSettings as EthernetDeviceSettings).Port = portValue;
                                }
                                break;
                            case ConnectionType.INDIRECT:
                                posDevice.DeviceSettings = new IndirectDeviceSettings(uow);

                                (posDevice.DeviceSettings as IndirectDeviceSettings).ParentDeviceName = Request["ParentDeviceName"];
                                (posDevice.DeviceSettings as IndirectDeviceSettings).OpenCommandString = SpecialCharacterReplacement(Request["OpenCommandString"]);
                                (posDevice.DeviceSettings as IndirectDeviceSettings).KeyPosition0CommandString = SpecialCharacterReplacement(Request["KeyPosition0CommandString"]);
                                (posDevice.DeviceSettings as IndirectDeviceSettings).KeyPosition1CommandString = SpecialCharacterReplacement(Request["KeyPosition1CommandString"]);
                                (posDevice.DeviceSettings as IndirectDeviceSettings).KeyPosition2CommandString = SpecialCharacterReplacement(Request["KeyPosition2CommandString"]);
                                (posDevice.DeviceSettings as IndirectDeviceSettings).KeyPosition3CommandString = SpecialCharacterReplacement(Request["KeyPosition3CommandString"]);
                                (posDevice.DeviceSettings as IndirectDeviceSettings).KeyPosition4CommandString = SpecialCharacterReplacement(Request["KeyPosition4CommandString"]);

                                break;
                        }

                        if (posDevice.DeviceSettings != null)
                        {
                            posDevice.DeviceSettings.DeviceType = deviceType;

                            int characterSet;
                            if (Int32.TryParse(Request["CharacterSet"], out characterSet))
                            {
                                posDevice.DeviceSettings.CharacterSet = characterSet;
                            }

                            int lineChars;
                            if (Int32.TryParse(Request["LineChars"], out lineChars))
                            {
                                posDevice.DeviceSettings.LineChars = lineChars;
                            }

                            posDevice.DeviceSettings.NewLine = Request["NewLine"];

                            int numberOfLines;
                            if (Int32.TryParse(Request["NumberOfLines"], out numberOfLines))
                            {
                                posDevice.DeviceSettings.NumberOfLines = numberOfLines;
                            }

                            int CommandChars;
                            if (Int32.TryParse(Request["CommandChars"], out CommandChars))
                            {
                                posDevice.DeviceSettings.CommandChars = CommandChars;
                            }

                            posDevice.DeviceSettings.ConvertCharset = Request["ConvertCharset"] == "C";
                            int codePageFrom, codePageTo;
                            if (int.TryParse(Request["ConvertCharsetFrom"], out codePageFrom) && int.TryParse(Request["ConvertCharsetTo"], out codePageTo))
                            {
                                posDevice.DeviceSettings.ConvertCharsetFrom = codePageFrom;
                                posDevice.DeviceSettings.ConvertCharsetTo = codePageTo;
                            }


                        }
                        posDevice.Save();
                        //uow.CommitTransaction();
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
                        Session["IsNewPOSDevice"] = null;
                        Session["uow"] = null;
                        Session["UnsavedPOSDevice"] = null;
                        Session["IsRefreshed"] = null;
                    }

                }
            }
            return Json(new { });

        }

        public static string SpecialCharacterReplacement(string input, bool reverse = false)
        {
            string[][] map = new string[][] { new string[] { "\\x1b", "\x1b" }, new string[] { "\\x1B", "\x1b" },
                                              new string[] { "\\x0", "\x0" } , new string[] { "\\r", "\r" }, new string[] { "\\n", "\n" }};
            string output = input;
            if (output != null)
            {
                foreach (string[] el in map)
                {
                    if (reverse)
                    {
                        output = output.Replace(el[1], el[0]);
                    }
                    else
                    {
                        output = output.Replace(el[0], el[1]);
                    }
                }
            }
            return output;
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
                Session["IsNewPOSDevice"] = null;
                Session["UnsavedPOSDevice"] = null;

            }
            return null;
        }

        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
        }

        private List<eDeviceSpecificType> GetSupportedDeviceSpecificTypes(DeviceType deviceType)
        {
            List<eDeviceSpecificType> supportedDeviceSpecificTypes = new List<eDeviceSpecificType>();
            supportedDeviceSpecificTypes.Add(eDeviceSpecificType.None);
            foreach (eDeviceSpecificType deviceSpecificType in Enum.GetValues(typeof(eDeviceSpecificType)))
            {
                if (deviceSpecificType.GetDeviceType() == deviceType)
                {
                    supportedDeviceSpecificTypes.Add(deviceSpecificType);
                }

            }
            return supportedDeviceSpecificTypes;
        }

        public ActionResult DeviceSpecificTypeCallbackPanel()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                string deviceTypeStr = Request["DXCallbackArgument"].Split(':')[1];
                DeviceType deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), deviceTypeStr);
                var member = deviceType.GetType().GetMember(deviceTypeStr);
                IsCashRegisterAttribute[] attribute = member[0].GetCustomAttributes(typeof(IsCashRegisterAttribute), false) as IsCashRegisterAttribute[];
                if (attribute != null && attribute.Length > 0)
                {
                    ViewData["IsCashRegister"] = 1;
                }
                else
                {
                    ViewData["IsCashRegister"] = 0;
                }
                ViewData["DeviceSpecificTypes"] = GetSupportedDeviceSpecificTypes(deviceType).ToArray();
            }

            return PartialView();
        }

        public ActionResult ConnectionTypeCallbackPanel()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                string deviceTypeStr = Request["DXCallbackArgument"].Split(':')[1];
                DeviceType deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), deviceTypeStr);
                ViewData["ConnectionTypes"] = deviceType.GetSupportedConnections();
            }
            return PartialView();
        }

        public ActionResult CashierVatFactorTabSettings()
        {
            try
            {
                return PartialView("CashierVatFactorTabSettings", ((ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"]).MapVatFactors);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult AddGridRow([ModelBinder(typeof(RetailModelBinder))] MapVatFactor mapVatFactor)
        {
            ViewData["EditMode"] = true;

            ITS.Retail.Model.POSDevice posDevice = (ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"];

            if (ModelState.IsValid)
            {
                try
                {
                    MapVatFactor mVatFactor = new MapVatFactor(posDevice.Session);
                    Guid ItemOid = Guid.Empty;
                    Guid.TryParse(Request["ItemCombobox_VI"], out ItemOid);
                    mVatFactor.GetData(mapVatFactor, new List<string>() { "Session" });
                    mVatFactor.Item = posDevice.Session.GetObjectByKey<Item>(ItemOid);
                    ((ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"]).MapVatFactors.Add(mVatFactor);
                    mVatFactor.Save();
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
            return PartialView("CashierVatFactorTabSettings", posDevice.MapVatFactors);
        }
        public ActionResult SelectItem()
        {
            return PartialView();
        }
        public static object ItemRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
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

            XPCollection<Item> collection = GetList<Item>(uowLocal,
                                                                CriteriaOperator.Or(new BinaryOperator("Name", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)));
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }


        [HttpPost]
        public ActionResult UpdateGridRow([ModelBinder(typeof(RetailModelBinder))] MapVatFactor mapVatFactor)
        {
            ViewData["EditMode"] = true;

            ITS.Retail.Model.POSDevice posDevice = (ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"];

            if (ModelState.IsValid)
            {
                try
                {

                    MapVatFactor mVatFactor = posDevice.MapVatFactors.Where(x => x.Oid == mapVatFactor.Oid).FirstOrDefault();
                    Guid ItemOid = Guid.Empty;
                    Guid.TryParse(Request["ItemCombobox_VI"], out ItemOid);
                    mVatFactor.GetData(mapVatFactor, new List<string>() { "Session" });
                    mVatFactor.Item = posDevice.Session.GetObjectByKey<Item>(ItemOid);
                    ((ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"]).MapVatFactors.Add(mVatFactor);
                    mVatFactor.Save();
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
            return PartialView("CashierVatFactorTabSettings", posDevice.MapVatFactors);
        }
        [HttpPost]
        public ActionResult DeleteGridRow([ModelBinder(typeof(RetailModelBinder))] MapVatFactor mapVatFactor)
        {
            ViewData["EditMode"] = true;

            ITS.Retail.Model.POSDevice posDevice = (ITS.Retail.Model.POSDevice)Session["UnsavedPOSDevice"];

            if (ModelState.IsValid)
            {
                try
                {
                    posDevice.MapVatFactors.First(factors => factors.Oid == mapVatFactor.Oid).Delete();

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
            return PartialView("CashierVatFactorTabSettings", posDevice.MapVatFactors);
        }
        public void CreatePOSDeviceDatabaseThread(object param)
        {
            if (!POSDeviceDBCreationHelper.IsProcessing)
            {
                POSDeviceDBCreationHelper.CreateFile(StoreControllerAppiSettings.DefaultCustomer.VatLevel.Oid, Server.MapPath("~/POS"));
            }
        }
        public JsonResult CreatePOSDeviceDatabase()
        {
            bool success = false;
            try
            {
                Thread thread = new Thread(CreatePOSDeviceDatabaseThread);
                thread.Start();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new { success = success });
        }
    }
}
