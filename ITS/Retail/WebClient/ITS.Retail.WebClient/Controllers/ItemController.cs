using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using ImageMagick;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Attributes;
using ITS.Retail.Platform.Kernel;
using System.Threading;
using System.Globalization;
using ITS.Retail.Platform;

namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    [StoreControllerEditable]
    public class ItemController : BaseObjController<Item>
    {
        private UnitOfWork uow;
        private CriteriaOperator filter = null;

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

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 200971520
        };

        public static readonly UploadControlValidationSettings UploadExtraFileControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png", ".doc", ".docx", ".pdf", ".html" },
            MaxFileSize = 200971520
        };

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return base.ExportToFile<Item>(Session["ItemGridSettings"] as GridViewSettings, (CriteriaOperator)Session["ItemFilter"]);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            User currentUser = CurrentUser;

            Session["customerDetails"] = null;
            //ViewData["ItemAnalyticTree"] = GetList<ItemAnalyticTree>(XpoHelper.GetNewSession());
            Session["ItemAnalyticTree_Item"] = new List<ItemAnalyticTree>();//GetList<ItemAnalyticTree>(XpoHelper.GetNewSession());
            Session["ItemFilter"] = filter = new BinaryOperator("Oid", Guid.Empty);
            ViewData["IsAdministrator"] = UserHelper.IsSystemAdmin(currentUser);
            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);
            ViewData["IsSupplier"] = userSupplier != null;
            ViewData["IsCustomer"] = UserHelper.IsCustomer(currentUser);

            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER ? "AddNewCustomV2" : "";
            this.ToolbarOptions.NewButton.Visible = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER;
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.EditButton.Visible = !UserHelper.IsCustomer(CurrentUser);
            this.ToolbarOptions.OptionsButton.Visible = MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER || UserHelper.IsCustomer(CurrentUser) == true ? false : true;
            this.ToolbarOptions.VariableValuesButton.Visible = this.GetType().GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null;
            this.ToolbarOptions.VariableValuesButton.OnClick = "VariableValuesDisplay.ShowVariableValuesPopUp";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "ItemID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdItems");

            ViewBag.Seasonality = GetList<Seasonality>(XpoSession);
            ViewBag.Buyers = GetList<Buyer>(XpoSession);

            return View("Index", new List<Item>());
        }

        public ActionResult ItemsOfMotherCodeGrid(string ItemID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (/*editMode == null ||*/ editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                Guid ChildItemID = Request["ChildItemID"] == null || Request["ChildItemID"] == "null" || Request["ChildItemID"] == "-1" ? Guid.Empty : Guid.Parse(Request["ChildItemID"]);

                FillLookupComboBoxes();
                return PartialView("ItemsOfMotherCodeGrid", ((Item)Session["UnsavedItem"]).ChildItems);
            }
            else  //view mode
            {
                Guid ItemGuid = ItemID == null || ItemID == "null" || ItemID == "" || ItemID == "-1" ? Guid.Empty : Guid.Parse(ItemID);
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
                ViewData["ItemID"] = ItemID;

                return PartialView("ItemsOfMotherCodeGrid", item.ChildItems);
            }
        }

        public override ActionResult Grid()   //_ItemPage()
        {
            GenerateUnitOfWork();
            Guid ItemID = Request["ItemID"] == null || Request["ItemID"] == "null" || Request["ItemID"] == "-1" ? Guid.Empty : Guid.Parse(Request["ItemID"]);
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                ViewData["CallbackMode"] = "SEARCH";

                if (Request.HttpMethod == "POST")
                {
                    string Fcode = Request["Fcode"] == null || Request["Fcode"] == "null" ? "" : Request["Fcode"];
                    string Fname = Request["Fname"] == null || Request["Fname"] == "null" ? "" : Request["Fname"];
                    string FBarcode = Request["Fbarcode"] == null || Request["Fbarcode"] == "null" ? "" : Request["Fbarcode"];
                    string Factive = Request["Factive"] == null || Request["Factive"] == "null" ? "" : Request["Factive"];
                    string Fcategory = Request["Fcategory"] == null || Request["Fcategory"] == "null" ? "" : Request["Fcategory"];
                    string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                    string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];
                    string FitemSupplier = Request["FitemSupplier"] == null || Request["FitemSupplier"] == "null" ? "" : Request["FitemSupplier"];
                    string Fbuyer = Request["Fbuyer"] == null || Request["Fbuyer"] == "null" ? "" : Request["Fbuyer"];
                    string Fseasonality = Request["Fseasonality"] == null || Request["Fseasonality"] == "null" ? "" : Request["Fseasonality"];
                    string Fmothercode = Request["Fmothercode"] == null || Request["Fmothercode"] == "null" ? "" : Request["Fmothercode"];

                    bool? doesnotallowdiscount = null;
                    if (Request["DoesNotAllowDiscount"] == "true")
                        doesnotallowdiscount = true;
                    else if (Request["DoesNotAllowDiscount"] == "false")
                        doesnotallowdiscount = false;


                    if (OwnerApplicationSettings != null)
                    {
                        if (OwnerApplicationSettings.PadBarcodes)
                        {
                            FBarcode = (FBarcode != "" && !FBarcode.Contains("*") && !FBarcode.Contains("%")) ? FBarcode.PadLeft(OwnerApplicationSettings.BarcodeLength, OwnerApplicationSettings.BarcodePaddingCharacter[0]) : FBarcode;
                        }

                        if (OwnerApplicationSettings.PadItemCodes)
                        {
                            Fcode = Fcode != "" && !Fcode.Contains("*") && !Fcode.Contains("%") ? Fcode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fcode;
                            Fmothercode = Fmothercode != "" && !Fmothercode.Contains("*") && !Fmothercode.Contains("%") ? Fmothercode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fmothercode;
                        }
                    }

                    if ((Fname.Contains("%") || Fname.Contains("*")) && Fname.Contains("_"))
                    {
                        Session["Error"] = Resources.InvalidFilter;
                        return PartialView("Grid", new XPQuery<Item>(uow).AppendWhere(conv, (CriteriaOperator)Session["ItemFilter"]).MakeOrderBy(conv, new ServerModeOrderDescriptor(new OperandProperty("Name"), false)));
                    }

                    if (Fcode != null && Fcode.Trim() != "")
                    {
                        if (Fcode.Contains("*") || Fcode.Contains("%"))
                        {
                            filter = CriteriaOperator.And(filter, new BinaryOperator("Code", Fcode.Replace('*', '%'), BinaryOperatorType.Like));
                        }
                        else
                        {
                            filter = CriteriaOperator.And(filter, new BinaryOperator("Code", Fcode, BinaryOperatorType.Equal));
                        }
                    }

                    if (doesnotallowdiscount == true)
                    {
                        filter = CriteriaOperator.And(filter, new BinaryOperator("DoesNotAllowDiscount", doesnotallowdiscount, BinaryOperatorType.Equal));
                    }
                    else if (doesnotallowdiscount == false)
                    {
                        filter = CriteriaOperator.And(filter, CriteriaOperator.Or(new NullOperator("DoesNotAllowDiscount"), new BinaryOperator("DoesNotAllowDiscount", false, BinaryOperatorType.Equal)));
                    }


                    if (Fname != null && Fname.Trim() != "")
                    {
                        filter = CriteriaOperator.And(filter, new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), Fname));
                        // CriteriaOperator.And(filter, new BinaryOperator("Name", "%" + Fname + "%", BinaryOperatorType.Like));
                    }

                    if (FBarcode != null && FBarcode.Trim() != "")
                    {
                        if (FBarcode.Contains("*") || FBarcode.Contains("%"))
                        {
                            filter = CriteriaOperator.And(filter, new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Code", FBarcode.Replace('*', '%'), BinaryOperatorType.Like)));
                        }
                        else
                        {
                            filter = CriteriaOperator.And(filter, new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Code", FBarcode, BinaryOperatorType.Equal)));
                        }
                    }

                    if (Factive == "0" || Factive == "1")
                    {
                        filter = CriteriaOperator.And(filter, new BinaryOperator("IsActive", Factive == "1"));
                    }

                    if (Fcategory != "" && Fcategory != "-1")
                    {
                        Guid Fcategory_guid = Guid.Parse(Fcategory);
                        ItemCategory ic = uow.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + Fcategory_guid + "'", ""));
                        filter = CriteriaOperator.And(filter, ic.GetAllNodeTreeFilter());
                    }

                    if (FcreatedOn != "")
                    {
                        filter = CriteriaOperator.And(filter, new BinaryOperator("InsertedDate", DateTime.Parse(FcreatedOn), BinaryOperatorType.GreaterOrEqual));
                    }

                    if (FupdatedOn != "")
                    {
                        filter = CriteriaOperator.And(filter, new BinaryOperator("UpdatedOn", DateTime.Parse(FupdatedOn), BinaryOperatorType.GreaterOrEqual));
                    }

                    if (FitemSupplier != null && FitemSupplier.Trim() != "")
                    {
                        filter = CriteriaOperator.And(filter, new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("DefaultSupplier.CompanyName"), FitemSupplier));
                        // CriteriaOperator.And(filter, new BinaryOperator("DefaultSupplier.CompanyName", "%" + FitemSupplier + "%", BinaryOperatorType.Like));
                    }

                    if (Fbuyer != null && Fbuyer.Trim() != "")
                    {
                        filter = CriteriaOperator.And(filter, new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Buyer.Oid"), Fbuyer));
                        // CriteriaOperator.And(filter, new BinaryOperator("Buyer.Oid", "%" + Fbuyer + "%", BinaryOperatorType.Like));
                    }

                    if (Fseasonality != null && Fseasonality.Trim() != "")
                    {
                        filter = CriteriaOperator.And(filter, new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Seasonality.Oid"), Fseasonality));
                        // CriteriaOperator.And(filter, new BinaryOperator("Seasonality.Oid", "%" + Fseasonality + "%", BinaryOperatorType.Like));
                    }

                    if (Fmothercode != null && Fmothercode.Trim() != "")
                    {
                        if (Fmothercode.Contains("%") || Fmothercode.Contains("*"))
                        {
                            filter = CriteriaOperator.And(filter, new BinaryOperator("MotherCode.Code", Fmothercode.Replace('*', '%'), BinaryOperatorType.Like));
                        }
                        else
                        {
                            filter = CriteriaOperator.And(filter, new BinaryOperator("MotherCode.Code", Fmothercode, BinaryOperatorType.Equal));
                        }
                    }

                    filter = ApplyOwnerCriteria(filter, typeof(Item));
                    Session["ItemFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["ItemFilter"] = filter;
                }
            }
            else if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                ViewData["CallbackMode"] = "DELETESELECTED";
                if (TableCanDelete)
                {
                    List<Guid> oids = new List<Guid>();
                    string allOids = Request["DXCallbackArgument"].Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
                    string[] unparsed = allOids.Split(',');
                    foreach (string unparsedOid in unparsed)
                    {
                        Guid parsedOid;
                        if (Guid.TryParse(unparsedOid, out parsedOid))
                        {
                            oids.Add(parsedOid);
                        }
                    }
                    if (oids.Count > 0)
                    {
                        try
                        {
                            DeleteAll(uow, oids);
                        }
                        catch (ConstraintViolationException)
                        {
                            Session["Error"] = Resources.CannotDeleteObject;
                        }
                        catch (Exception e)
                        {
                            Session["Error"] = e.Message;
                        }
                    }
                }
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            try
            {
                return PartialView("Grid", new XPQuery<Item>(uow).AppendWhere(conv, (CriteriaOperator)Session["ItemFilter"]).MakeOrderBy(conv, new ServerModeOrderDescriptor(new OperandProperty("Name"), false)));
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                return PartialView("Grid", GetList<Item>(uow, (CriteriaOperator)Session["ItemFilter"], "Name"));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewPartialMotherCode([ModelBinder(typeof(RetailModelBinder))] Item ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid comboBoxChildItemGuid;
                    if (Guid.TryParse(Request["ComboBoxChildItemID"], out comboBoxChildItemGuid))
                    {
                        Item comboBoxChildItem = uow.FindObject<Item>(new BinaryOperator("Oid", comboBoxChildItemGuid, BinaryOperatorType.Equal));
                        Item item = (Item)Session["UnsavedItem"];
                        if (comboBoxChildItem.MotherCode != item)
                        {
                            comboBoxChildItem.MotherCode = item;
                        }
                        else
                        {
                            Session["Error"] = Resources.ItemAlreadyExists;
                        }
                        comboBoxChildItem.Save();
                        Session["UnsavedItem"] = item;
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
            return PartialView("ItemsOfMotherCodeGrid", ((Item)Session["UnsavedItem"]).ChildItems);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            Item item = Session["UnsavedItem"] as Item;
            if (item != null)
            {
                if (Session["IsNewItem"] != null && (bool)Session["IsNewItem"] == true)
                {
                    ViewBag.OwnerApplicationSettings = OwnerApplicationSettings;
                }
                else
                {
                    ViewBag.OwnerApplicationSettings = item.Owner.OwnerApplicationSettings;
                }
            }
            else if (Request["ItemID"] != null && Request["editMode"] != null && Boolean.Parse(Request["editMode"]) == false)  //view mode request
            {
                item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", Request["ItemID"], BinaryOperatorType.Equal));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeletePartialMotherCode([ModelBinder(typeof(RetailModelBinder))] Item ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (Item ci in item.ChildItems)
                {
                    if (ci.Oid == ct.Oid)
                    {
                        ci.MotherCode = null;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("ItemsOfMotherCodeGrid", ((Item)Session["UnsavedItem"]).ChildItems);
        }

        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
            base.FillLookupComboBoxes();
            ViewBag.BarcodeTypeComboBox = GetList<BarcodeType>(uow);
            ViewBag.MesurmentUnitComboBox = GetList<MeasurementUnit>(uow, CriteriaOperator.Or(new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.ORDER),
                                                                                             new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING_AND_ORDER),
                                                                                             new NullOperator("MeasurementUnitType")));
            ViewBag.VatCategoryComboBox = GetList<VatCategory>(uow);
            ViewBag.PackingMeasurementUnitComboBox = GetList<MeasurementUnit>(uow, CriteriaOperator.Or(new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING),
                                                                                             new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING_AND_ORDER)));
        }

        public ActionResult DefaultBarcodeComboBox()
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            Item item = (Item)Session["UnsavedItem"];
            if (item != null)
            {
                ViewBag.BarcodeComboBox = ItemHelper.GetBarcodesOfItem(item);
            }
            Guid currentDefaultBarcodeID;
            if (Session["CurrentDefaultBarcode"] != null && Guid.TryParse(Session["CurrentDefaultBarcode"].ToString(), out currentDefaultBarcodeID))
            {
                foreach (Barcode bc in ItemHelper.GetBarcodesOfItem(item))
                {
                    if (bc.Oid == currentDefaultBarcodeID)
                    {
                        return PartialView("DefaultBarcodeComboBox", bc);
                    }
                }
            }
            return PartialView("DefaultBarcodeComboBox", null);
        }

        protected object UpdateMotherLookupObects<T>(T ct)
        {
            Item a = ct as Item;
            a.VatCategory = GetObjectByArgument<VatCategory>(XpoSession, "VatCategoryId") as VatCategory;
            a.DefaultBarcode = GetObjectByArgument<Barcode>(XpoSession, "DefaultBarcodeID") as Barcode;
            a.MotherCode = GetObjectByArgument<Item>(XpoSession, "MotherCodeID") as Item;
            a.DefaultSupplier = GetObjectByArgument<SupplierNew>(XpoSession, "ItemSupplierID");
            a.Name = Request.Params["mName"];
            a.Code = Request.Params["mCode"];
            a.IsActive = Request.Params["mIsActive"] == null ? false : Request.Params["mIsActive"].ToString() == "C" ? true : false;
            return a;
        }

        public ActionResult ItemViewPopup()
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView();
        }

        public ActionResult PrintLabelPopUp()
        {
            GenerateUnitOfWork();
            Customer defaultCustomer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomerOid);
            if (defaultCustomer == null)
            {
                Session["Error"] = Resources.DefaultCustomerNotFound;
                return View("Error");
            }
            else
            {
                ViewBag.Labels = GetList<Label>(XpoSession);
                return PartialView();
            }
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                FillItem();
            }

            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditItem;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public ActionResult ItemImagePopup()
        {
            Item item = Session["UnsavedItem"] as Item;
            if (item != null)
            {
                ViewData["ItemImageDescription"] = item.ImageDescription;
                ViewData["ItemImageInfo"] = item.ImageInfo;
            }
            else
            {
                ViewData["ItemImageDescription"] = "";
                ViewData["ItemImageInfo"] = "";
            }
            return PartialView();
        }

        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            return PartialView();
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult ImageUploadForm()
        {
            return PartialView("ItemImagePopup");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, ImageUpload_FileUploadComplete);
            return null;
        }

        public ActionResult UploadExtraFileControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadExtraFileControl", UploadExtraFileControlValidationSettings, ExtraFileUpload_FileUploadComplete);
            return null;
        }

        public void ExtraFileUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                Item currentItem = (System.Web.HttpContext.Current.Session["UnsavedItem"] as Item);
                if (currentItem != null)
                {
                    currentItem.ExtraFile = e.UploadedFile.FileBytes;
                    currentItem.ExtraFilename = e.UploadedFile.FileName;
                    currentItem.ExtraMimeType = e.UploadedFile.ContentType;
                    e.CallbackData = e.UploadedFile.FileName;
                }
            }
        }

        public static void ImageUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                e.CallbackData = "success";
                if (System.Web.HttpContext.Current.Session["ImageStream"] != null)
                {
                    ((MemoryStream)System.Web.HttpContext.Current.Session["ImageStream"]).Dispose();
                    ((MemoryStream)System.Web.HttpContext.Current.Session["ImageStream"]).Close();
                }

                MagickImage uploadedImage = new MagickImage(e.UploadedFile.FileBytes);

                Item currentItem = (System.Web.HttpContext.Current.Session["UnsavedItem"] as Item);

                if (currentItem != null)
                {
                    currentItem.ImageLarge = ItemHelper.PrepareImage(uploadedImage, 600, 600);
                    currentItem.ImageMedium = ItemHelper.PrepareImage(uploadedImage, 300, 300);
                    currentItem.ImageSmall = ItemHelper.PrepareImage(uploadedImage, 150, 150);
                }
            }
        }

        [AllowAnonymous, ActionLog(LogLevel = LogLevel.None)]
        public FileContentResult ShowImageId(String Id, int imageSize = 0)
        {
            GenerateUnitOfWork();
            Guid itemGuid;
            ImageConverter converter = new ImageConverter();
            byte[] imageBytes = null;
            string format = "png";
            if (Guid.TryParse(Id, out itemGuid))
            {
                Item it = uow.FindObject<Item>(new BinaryOperator("Oid", itemGuid));
                if (it != null)
                {
                    Image img;
                    switch (imageSize)
                    {
                        case 1:
                            img = it.ImageMedium;
                            break;

                        case 2:
                            img = it.ImageLarge;
                            break;

                        default:
                            img = it.ImageSmall;
                            break;
                    }
                    if (img != null)
                    {
                        imageBytes = (byte[])converter.ConvertTo(img, typeof(byte[]));

                        if (img.RawFormat.Equals(ImageFormat.Jpeg))
                        {
                            format = "jpeg";
                        }
                        else if ((img.RawFormat.Equals(ImageFormat.Gif)))
                        {
                            format = "gif";
                        }
                        else if ((img.RawFormat.Equals(ImageFormat.Png)))
                        {
                            format = "png";
                        }
                    }
                }
            }
            if (imageBytes == null)
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/img/no_image.png"));
                imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
            }
            return new FileContentResult(imageBytes, "image/" + format);
        }

        [Security(ReturnsPartial = false, DontLogAction = true)]
        public FileContentResult ShowImage()
        {
            GenerateUnitOfWork();
            Item item = Session["UnsavedItem"] as Item;
            if (item != null && item.ImageSmall != null && item.ImageSmall.Width > 0)
            {
                Image im = item.ImageSmall;
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

        public ActionResult ItemsOfManufacturerGrid()
        {
            return PartialView();
        }

        public ActionResult EditView(string Oid)
        {
            CancelEdit();

            GenerateUnitOfWork();

            User currentUser = CurrentUser;
            ViewData["IsAdministrator"] = UserHelper.IsSystemAdmin(currentUser);
            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);
            ViewData["IsSupplier"] = userSupplier != null;
            ViewData["IsCustomer"] = UserHelper.GetCustomer(currentUser) != null;

            Guid itemGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            ViewData["EditMode"] = true;

            if (itemGuid == Guid.Empty && TableCanInsert == false)
            {
                Session["Error"] = Resources.YouCannotEditThisElement;
                return null;
            }
            else if (itemGuid != Guid.Empty && TableCanUpdate == false)
            {
                Session["Error"] = Resources.YouCannotEditThisElement;
                return null;
            }

            Item item;

            if (itemGuid != Guid.Empty)
            {
                ViewBag.Mode = Resources.EditItem;
                item = uow.FindObject<Item>(new BinaryOperator("Oid", itemGuid, BinaryOperatorType.Equal));
                if (item == null) //then guid must be an ItemBarcode
                {
                    ItemBarcode ibc = uow.FindObject<ItemBarcode>(new BinaryOperator("Oid", itemGuid));
                    if (ibc != null)
                    {
                        item = ibc.Item;
                    }
                }
                Session["IsNewItem"] = false;
            }
            else
            {
                ViewBag.Mode = Resources.NewItem;
                item = new Item(uow);
                AssignOwner(item);
                Session["IsNewItem"] = true;
            }
            Session["IsRefreshed"] = false;

            FillLookupComboBoxes();
            ViewData["ItemID"] = item.Oid.ToString();
            Session["UnsavedItem"] = item;
            if ((bool)Session["IsNewItem"] != true)
            {
                if (item != null && item.ImageSmall != null)
                {
                    ViewData["ItemImageDescription"] = item.ImageDescription;
                    ViewData["ItemImageInfo"] = item.ImageInfo;
                }
                else
                {
                    ViewData["ItemImageDescription"] = "";
                    ViewData["ItemImageInfo"] = "";
                }
            }
            ViewBag.DisplayItemExtraInfo = MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView("EditView", item);
        }

        public ActionResult ItemAnalyticTreeGrid(string ItemID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    ItemAnalyticTree iat = new ItemAnalyticTree(uow);
                    Session["UnsavedItemAnalyticTree"] = iat;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedItemAnalyticTree"] = null;
                }
                return PartialView("ItemAnalyticTreeGrid", ((Item)Session["UnsavedItem"]).ItemAnalyticTrees);
            }
            else  //view mode
            {
                Guid ItemGuid = (ItemID == null || ItemID == "null" || ItemID == "-1") ? Guid.Empty : Guid.Parse(ItemID);
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
                ViewData["ItemID"] = ItemID;
                return PartialView("ItemAnalyticTreeGrid", item.ItemAnalyticTrees);
            }
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid itemGuid = Guid.Empty;

            bool correctItemGuid = Request["ItemID"] != null && Guid.TryParse(Request["ItemID"].ToString(), out itemGuid);
            if (correctItemGuid)
            {
                Item item = (Session["UnsavedItem"] as Item);
                if (item != null)
                {
                    if (MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
                    {
                        if ((bool)Session["IsNewItem"])
                        {
                            AssignOwner(item);
                        }
                        if (item.Owner.OwnerApplicationSettings.PadItemCodes)
                        {
                            item.Code = Request["Code"].PadLeft(item.Owner.OwnerApplicationSettings.ItemCodeLength, item.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                        }
                        else
                        {
                            item.Code = Request["Code"];
                        }
                        item.ExtraDescription = Request["ExtraDescription"];

                        item.Name = Request["Name"];

                        //Error no owner
                        if (item.Owner == null)
                        {
                            uow.RollbackTransaction();
                            Session["Error"] = Resources.AnErrorOccurred + ": Item Owner is not defined.";
                            return Json(new { error = Session["Error"] });
                        }

                        item.VatCategory = uow.FindObject<VatCategory>(new BinaryOperator("Oid", (Request.Params["VatCategory_VI"] == null || Request.Params["VatCategory_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["VatCategory_VI"])));
                        item.DefaultBarcode = uow.FindObject<Barcode>(new BinaryOperator("Oid", (Request.Params["DefaultBarcode_VI"] == null || Request.Params["DefaultBarcode_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["DefaultBarcode_VI"])));
                        item.MotherCode = uow.FindObject<Item>(new BinaryOperator("Oid", (Request.Params["MotherCode_VI"] == null || Request.Params["MotherCode_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["MotherCode_VI"])));
                        item.DefaultSupplier = uow.FindObject<SupplierNew>(new BinaryOperator("Oid", (Request.Params["ItemSupplier_VI"] == null || Request.Params["ItemSupplier_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["ItemSupplier_VI"])));
                        item.Buyer = uow.FindObject<Buyer>(new BinaryOperator("Oid", (Request.Params["Buyer_VI"] == null || Request.Params["Buyer_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["Buyer_VI"])));
                        item.Seasonality = uow.FindObject<Seasonality>(new BinaryOperator("Oid", (Request.Params["Seasonality_VI"] == null || Request.Params["Seasonality_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["Seasonality_VI"])));
                        item.PackingMeasurementUnit = uow.FindObject<MeasurementUnit>(new BinaryOperator("Oid", (Request.Params["PackingMeasurementUnit_VI"] == null || Request.Params["PackingMeasurementUnit_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["PackingMeasurementUnit_VI"])));

                        DateTime inserted_date_time = DateTime.Now;
                        if (DateTime.TryParse(Request["FInsertedOn"], out inserted_date_time))
                        {
                            item.InsertedDate = inserted_date_time;
                        }
                        else if (item.InsertedDate == DateTime.MinValue)//legacy code
                        {
                            item.InsertedDate = item.CreatedOn;
                        }

                        double maxOrderQty = 0.0;
                        Double.TryParse(Request["MaxOrderQty"], out maxOrderQty);
                        item.MaxOrderQty = maxOrderQty;
                        double packingQty = 0.0;
                        Double.TryParse(Request["PackingQty"], out packingQty);
                        item.PackingQty = packingQty;
                        double orderQty = 0.0;
                        Double.TryParse(Request["OrderQty"], out orderQty);
                        item.OrderQty = orderQty;
                        double minOrderQty = 0.0;
                        Double.TryParse(Request["minOrderQty"], out minOrderQty);
                        item.MinOrderQty = minOrderQty;
                        decimal points = 0;
                        decimal ReferenceUnit = 0;
                        decimal ContentUnit = 0;
                        Decimal.TryParse(Request["Points"], out points);

                        item.Points = points;
                        Decimal.TryParse(Request["ref_unit"], out ReferenceUnit);
                        item.ReferenceUnit = ReferenceUnit;
                        Decimal.TryParse(Request["content_unit"], out ContentUnit);
                        item.ContentUnit = ContentUnit;

                        item.Remarks = Request["Remarks"];

                        if (Request["IsActive"] != null || Request["IsActive"] != "null")
                        {
                            if (Request["IsActive"].ToString().Equals("C"))
                            {
                                item.IsActive = true;
                            }
                            else if (Request["IsActive"].ToString().Equals("U"))
                            {
                                item.IsActive = false;
                            }
                        }
                        if (Request["IsCentralStored"] != null || Request["IsCentralStored"] != "null")
                        {
                            if (Request["IsCentralStored"].ToString().Equals("C"))
                            {
                                item.IsCentralStored = true;
                            }
                            else if (Request["IsCentralStored"].ToString().Equals("U"))
                            {
                                item.IsCentralStored = false;
                            }
                        }

                        eItemCustomPriceOptions CustomOptions;
                        if (Enum.TryParse(Request["customPriceOptions_VI"], out CustomOptions))
                        {
                            item.CustomPriceOptions = CustomOptions;
                        }

                        if (Request["AcceptsCustomDescription"] != null || Request["AcceptsCustomDescription"] != "null")
                        {
                            if (Request["AcceptsCustomDescription"].ToString().Equals("C"))
                            {
                                item.AcceptsCustomDescription = true;
                            }
                            else if (Request["AcceptsCustomDescription"].ToString().Equals("U"))
                            {
                                item.AcceptsCustomDescription = false;
                            }
                        }

                        item.IsTax = Request["IsTax"] != null && Request["IsTax"] == "C";

                        item.DoesNotAllowDiscount = Request["DoesNotAllowDiscount2"] == "C" ? true : false;

                        if (Request["IsGeneralItem"] != null || Request["IsGeneralItem"] != "null")
                        {
                            if (Request["IsGeneralItem"].ToString().Equals("C"))
                            {
                                item.IsGeneralItem = true;
                            }
                            else if (Request["IsGeneralItem"].ToString().Equals("U"))
                            {
                                item.IsGeneralItem = false;
                            }
                        }
                    }

                    if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                      || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE)
                    {
                        ItemExtraInfo itemExtraInfo = item.ItemExtraInfos.FirstOrDefault(x => x.Store.Oid == CurrentStore.Oid);
                        if (itemExtraInfo == null)
                        {
                            itemExtraInfo = new Model.ItemExtraInfo(item.Session)
                            {
                                Store = item.Session.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid),
                                Item = item
                            };
                        }
                        //Save item.ItemExtraInfo independendly of application instance type
                        if (!string.IsNullOrWhiteSpace(Request["ItemExtraInfoDescription"]))
                        {
                            itemExtraInfo.Description = Request["ItemExtraInfoDescription"];
                        }
                        if (!string.IsNullOrWhiteSpace(Request["ItemExtraInfoIngredients"]))
                        {
                            itemExtraInfo.Description = Request["ItemExtraInfoIngredients"];
                        }

                        DateTime packedAt;
                        if (DateTime.TryParse(Request["ItemExtraInfoPackedAt"], out packedAt))
                        {
                            if (packedAt != DateTime.MinValue)
                            {
                                itemExtraInfo.PackedAt = packedAt;
                            }
                        }
                        DateTime expiresAt;
                        if (DateTime.TryParse(Request["ItemExtraInfoExpiresAt"], out expiresAt))
                        {
                            if (expiresAt != DateTime.MinValue)
                            {
                                itemExtraInfo.ExpiresAt = expiresAt;
                            }
                        }
                        itemExtraInfo.Save();
                    }

                    if (item.IsTax == true || item.DoesNotAllowDiscount == true)
                    {
                        IEnumerable<PriceCatalogDetail> discounts = item.PriceCatalogs.Where(priceDetail => priceDetail.Discount > 0).ToList();
                        if (discounts.Count() > 0)
                        {
                            List<PriceCatalogDetail> asList = discounts.ToList();
                            asList.ForEach(x => x.Discount = 0);
                        }
                    }

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

                                foreach (PriceCatalogDetail storeControllerPriceCatalogDetail in item.PriceCatalogs.Where(priceDetail => priceDetail.HasChangedOrHasTimeValueChanges))
                                {
                                    string jsonItem = storeControllerPriceCatalogDetail.JsonWithDetails(PlatformConstants.JSON_SERIALIZER_SETTINGS, false);
                                    if (webService.InsertOrUpdateRecord(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid, "PriceCatalogDetail", jsonItem, out errorMessage)
                                        == false
                                       )
                                    {
                                        Session["Error"] = errorMessage;
                                        return Json(new { reloadEdit = true, data = item, error = Session["Error"] });
                                    }
                                }

                                #region manage deletion of item details

                                List<object> itemDetails = item.Session.GetObjectsToSave().Cast<object>().Where(itemDetailObject => itemDetailObject is BaseObj).ToList();
                                foreach (BaseObj currentBaseObject in itemDetails)
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

                                #endregion manage deletion of item details

                                item.Save();
                                XpoHelper.CommitTransaction(uow);
                                Session["Notice"] = Resources.SavedSuccesfully;
                            }
                        }
                        else
                        {
                            item.Save();
                            XpoHelper.CommitTransaction(uow);
                            Session["Notice"] = Resources.SavedSuccesfully;
                        }
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewItem"] = null;
                        Session["uow"] = null;
                        Session["UnsavedItem"] = null;
                        Session["IsRefreshed"] = null;
                        Session["UnsavedItemImage"] = null;
                        if (Session["ImageStream"] != null)
                        {
                            ((MemoryStream)Session["ImageStream"]).Dispose();
                            ((MemoryStream)Session["ImageStream"]).Close();
                            Session["ImageStream"] = null;
                        }
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                    }
                }
            }
            return Json(new { });
        }

        public JsonResult CancelEdit()
        {
            try
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewItem"] = null;
                Session["UnsavedItem"] = null;
                Session["UnsavedItemImage"] = null;
                if (Session["ImageStream"] != null)
                {
                    ((MemoryStream)Session["ImageStream"]).Dispose();
                    ((MemoryStream)Session["ImageStream"]).Close();
                    Session["ImageStream"] = null;
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult ItemsComboBox()
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView();
        }

        public ActionResult LinkedSubItemsComboBox()
        {
            return PartialView();
        }

        public ActionResult LinkedToItemsComboBox()
        {
            return PartialView();
        }

        public ActionResult ItemSuppliersComboBoxPartial()
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView();
        }

        public ActionResult SeasonalityComboBoxPartial()
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Guid seasonalityGuid;
                string oid = Request["DXCallbackArgument"].ToString().Split(';')[1];//There must be a better way to do this...
                if (Guid.TryParse(oid, out seasonalityGuid))
                {
                    Seasonality seasonality = ((Item)Session["UnsavedItem"]).Session.GetObjectByKey<Seasonality>(seasonalityGuid);
                    if (seasonality != null)
                    {
                        ((Item)Session["UnsavedItem"]).Seasonality = seasonality;
                        return PartialView(Session["UnsavedItem"]);
                    }
                }
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }
            return PartialView();
        }

        public ActionResult BuyerComboBoxPartial()
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Guid buyerGuid;
                string oid = Request["DXCallbackArgument"].ToString().Split(';')[1];//There must be a better way to do this...
                if (Guid.TryParse(oid, out buyerGuid))
                {
                    Buyer buyer = ((Item)Session["UnsavedItem"]).Session.GetObjectByKey<Buyer>(buyerGuid);
                    if (buyer != null)
                    {
                        ((Item)Session["UnsavedItem"]).Buyer = buyer;
                        return PartialView(Session["UnsavedItem"]);
                    }
                }
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }
            return PartialView((Item)Session["UnsavedItem"]);
        }

        public ActionResult ChildItemComboBox()
        {
            return PartialView();
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

        public static object GetItemByValue(object value)
        {
            return GetObjectByValue<Item>(value);
        }

        public static object ItemsRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();
            if (String.IsNullOrWhiteSpace(e.Filter) || String.IsNullOrEmpty(e.Filter) || e.Filter.Length < 3)//search start for more than three characters
            {
                return new XPCollection<Item>(uowLocal, new BinaryOperator("Oid", Guid.Empty));
            }

            Item item = (Item)System.Web.HttpContext.Current.Session["UnsavedItem"];
            OwnerApplicationSettings settings = (bool)System.Web.HttpContext.Current.Session["IsNewItem"] == true ? OwnerApplicationSettings : item.Owner.OwnerApplicationSettings;

            string nameFilter = e.Filter.Replace('*', '%');
            string codefilter = e.Filter.Replace('*', '%');
            string barcodeFilter = e.Filter.Replace('*', '%');

            if (settings.PadItemCodes && !codefilter.Contains('%'))
            {
                codefilter = codefilter.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]);
            }

            if (settings.PadBarcodes && !barcodeFilter.Contains('%'))
            {
                barcodeFilter = barcodeFilter.PadLeft(settings.BarcodeLength, settings.BarcodePaddingCharacter[0]);
            }

            CriteriaOperator nameFilterCrop = null;
            foreach (string filt in nameFilter.Split(' '))
            {
                string flt = (filt.Contains('%')) ? filt : String.Format("%{0}%", filt);
                nameFilterCrop = CriteriaOperator.And(nameFilterCrop, new BinaryOperator("Name", flt, BinaryOperatorType.Like));
            }

            CriteriaOperator removeSelf = null;
            if (item != null)
            {
                removeSelf = new BinaryOperator("Oid", item.Oid, BinaryOperatorType.NotEqual);
            }

            XPCollection<Item> collection = GetList<Item>(uowLocal,
                                    CriteriaOperator.And(CriteriaOperator.Or(nameFilterCrop,
                                                            new BinaryOperator("Code", String.Format("{0}", codefilter), BinaryOperatorType.Like),
                                                            new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Code", barcodeFilter))),
                                                            removeSelf), "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return collection;
        }

        public ActionResult BarcodesComboBoxPartial()
        {
            ViewBag.BarcodeComboBox = ItemHelper.GetBarcodesOfItem((Item)Session["UnsavedItem"]);
            return PartialView();
        }

        public ActionResult PriceCatalogDetailGrid(string ItemID, bool editMode)
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            ViewData["EditMode"] = editMode;
            if (editMode == true)  //edit mode
            {
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedPriceCatalogDetail"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid PriceCatalogDetailID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    Item item = (Item)Session["UnsavedItem"];
                    PriceCatalogDetail currentPriceCatalogDetail = null;
                    Session["UnsavedPriceCatalogDetail"] = currentPriceCatalogDetail = item.PriceCatalogs.FirstOrDefault(priceDetail => priceDetail.Oid == PriceCatalogDetailID);

                    //if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                    //    && currentPriceCatalogDetail.PriceCatalog.IsEditableAtStore != null
                    //    && currentPriceCatalogDetail.PriceCatalog.IsEditableAtStore.Oid != StoreControllerAppiSettings.CurrentStoreOid
                    //    )
                    //{
                    //    Session["Error"] = Resources.YouCannotEditThisElement;
                    //}
                }
                ViewBag.BarcodeComboBox = ItemHelper.GetBarcodesOfItem((Item)Session["UnsavedItem"]);
                User currentUser = CurrentUser;
                CriteriaOperator visiblePriceCatalogDetailsFilter = null;
                if (!Boolean.Parse(Session["IsAdministrator"].ToString()))
                {
                    List<CriteriaOperator> visiblePriceCatalogDetailsFilterList = UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid");
                    if (visiblePriceCatalogDetailsFilterList.Count != 0)
                    {
                        visiblePriceCatalogDetailsFilter = CriteriaOperator.Or(visiblePriceCatalogDetailsFilterList);
                    }
                    else
                    {
                        visiblePriceCatalogDetailsFilter = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.Equal);
                    }
                }

                ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = visiblePriceCatalogDetailsFilter;
                return PartialView("PriceCatalogDetailGrid", ((Item)Session["UnsavedItem"]).PriceCatalogs);
            }
            else  //view mode
            {
                Guid ItemGuid = (ItemID == null || ItemID == "null" || ItemID == "-1") ? Guid.Empty : Guid.Parse(ItemID);
                Item item = uow.FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));

                ViewBag.OwnerApplicationSettings = item.Owner.OwnerApplicationSettings;

                ViewData["ItemID"] = ItemID;
                User currentUser = CurrentUser;
                XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
                List<CriteriaOperator> visiblePriceCatalogDetailsFilterList = new List<CriteriaOperator>();
                CriteriaOperator visiblePriceCatalogDetailsFilter = null;

                visiblePriceCatalogDetailsFilterList.Add(new BinaryOperator("Item.Oid", ItemID));
                visiblePriceCatalogDetailsFilterList.AddRange(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));

                IEnumerable<PriceCatalogDetail> customerDetails = null;
                if (visiblePriceCatalogDetailsFilterList.Count != 0)
                {
                    visiblePriceCatalogDetailsFilter = CriteriaOperator.Or(visiblePriceCatalogDetailsFilterList);
                }

                if (!Boolean.Parse(Session["IsAdministrator"].ToString()))
                {
                    if (userCustomers.Count != 0)
                    {
                        Customer userCustomer = userCustomers.First();
                        try
                        {
                            if (this.CurrentStore == null)
                            {
                                Session["Error"] = Resources.PleaseSelectAStore;
                                visiblePriceCatalogDetailsFilterList.Add(new BinaryOperator("Oid", Guid.Empty));
                                customerDetails = GetList<PriceCatalogDetail>(uow, new BinaryOperator("Oid", Guid.Empty));
                            }
                            else
                            {
                                Store store = userCustomer.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);
                                //PriceCatalog pcs = userCustomer.GetPriceCatalog(store);
                                //if (pcs != null)
                                //{
                                //    //customerDetails = PriceCatalogHelper.GetTreePriceCatalogDetails(pcs, visiblePriceCatalogDetailsFilter);
                                //    customerDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(store, visiblePriceCatalogDetailsFilter, userCustomer);
                                //}
                                //else
                                //{
                                //    Session["Error"] = Resources.CustomerHasNoPriceCatalog;
                                //    visiblePriceCatalogDetailsFilterList.Add(new BinaryOperator("Oid", Guid.Empty));
                                //    customerDetails = GetList<PriceCatalogDetail>(uow, new BinaryOperator("Oid", Guid.Empty));
                                //}
                                customerDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(store, visiblePriceCatalogDetailsFilter, userCustomer);
                            }
                        }
                        catch (Exception)
                        {
                            Session["Error"] = Resources.CustomerHasNoPriceCatalog;
                            visiblePriceCatalogDetailsFilterList.Add(new BinaryOperator("Oid", Guid.Empty));
                            customerDetails = GetList<PriceCatalogDetail>(uow, new BinaryOperator("Oid", Guid.Empty));
                        }
                    }
                }

                if (customerDetails != null)
                {
                    return PartialView("PriceCatalogDetailGrid", customerDetails);
                }
                else
                {
                    item.PriceCatalogs.Filter = CriteriaOperator.And(visiblePriceCatalogDetailsFilter, new BinaryOperator("IsActive", true));
                    return PartialView("PriceCatalogDetailGrid", item.PriceCatalogs);
                }
            }
        }

        [HttpPost]
        public ActionResult PriceCatalogDetailAddNewPartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            GenerateUnitOfWork();
            User currentUser = CurrentUser;
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid && MvcApplication.ApplicationInstanceAllowsPriceEdit)
            {
                try
                {
                    Guid BarcodeId = Request["BarcodeComboBoxID"] != null && Request["BarcodeComboBoxID"] != "null" ? Guid.Parse(Request["BarcodeComboBoxID"]) : Guid.Empty;
                    Guid PriceCatalogID = Request["PriceCatalogComboBoxID"] != null && Request["PriceCatalogComboBoxID"] != "null" ? Guid.Parse(Request["PriceCatalogComboBoxID"]) : Guid.Empty;
                    bool VatIncluded = Request["VatIncluded"] == "true" ? true : false;

                    Item item = (Item)Session["UnsavedItem"];

                    item.PriceCatalogs.Filter = CriteriaOperator.And(new BinaryOperator("PriceCatalog.Oid", PriceCatalogID, BinaryOperatorType.Equal),
                                                new BinaryOperator("Barcode.Oid", BarcodeId, BinaryOperatorType.Equal));
                    if (item.PriceCatalogs.Count != 0)
                    {
                        Session["Error"] = Resources.DuplicatePriceCatalogDetail;
                        FillLookupComboBoxes();
                        Session["UnsavedItem"] = item;
                        //Τα details που ανήκουν στους τιμοκαταλόγους του supplier
                        ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
                        return PartialView("PriceCatalogDetailGrid", item.PriceCatalogs);
                    }
                    item.PriceCatalogs.Filter = null;
                    PriceCatalogDetail pcd = new PriceCatalogDetail(uow);
                    PriceCatalog pc = item.Session.FindObject<PriceCatalog>(new BinaryOperator("Oid", PriceCatalogID, BinaryOperatorType.Equal));
                    pcd.PriceCatalog = pc;
                    foreach (Barcode bc in ItemHelper.GetBarcodesOfItem(item))
                    {
                        if (bc.Oid == BarcodeId)
                        {
                            pcd.Barcode = bc;
                            break;
                        }
                    }
                    pcd.IsActive = Request["IsActiveValue"] == "C";
                    pcd.DatabaseValue = ct.DatabaseValue;
                    pcd.Discount = ct.Discount / 100;
                    pcd.MarkUp = ct.MarkUp / 100;
                    pcd.VATIncluded = VatIncluded;

                    pcd.TimeValue = ct.TimeValue;
                    pcd.TimeValueValidFrom = ct.TimeValueValidFrom;
                    pcd.TimeValueValidUntil = ct.TimeValueValidUntil;
                    if (!pcd.TimeValueIsValid)
                    {
                        ModelState.AddModelError("TimeValue", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidFromDate", Resources.InvalidTimeValue);
                        ModelState.AddModelError("TimeValueValidUntilDate", Resources.InvalidTimeValue);
                        ViewBag.BarcodeComboBox = item.ItemBarcodes.Select(itemBarcode => itemBarcode.Barcode);
                        ViewBag.CurrentItem = pcd;
                    }
                    else
                    {
                        item.PriceCatalogs.Add(pcd);
                        Session["UnsavedItem"] = item;
                        Session["UnsavedPriceCatalogDetail"] = null;
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else if (!MvcApplication.ApplicationInstanceAllowsPriceEdit)
            {
                Session["Error"] = Resources.ApplicationMustBeConnectedToHeadQuartersToEditPrices;
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            FillLookupComboBoxes();
            //Τα details που ανήκουν στους τιμοκαταλόγους του supplier
            ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
            return PartialView("PriceCatalogDetailGrid", ((Item)Session["UnsavedItem"]).PriceCatalogs);
        }

        [HttpPost]
        public ActionResult PriceCatalogDetailUpdatePartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            GenerateUnitOfWork();
            User currentUser = CurrentUser;
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            PriceCatalogDetail pcd = (PriceCatalogDetail)Session["UnsavedPriceCatalogDetail"];

            if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                && pcd.PriceCatalog.IsEditableAtStore != null
                && pcd.PriceCatalog.IsEditableAtStore.Oid != StoreControllerAppiSettings.CurrentStoreOid
               )
            {
                ModelState.AddModelError("PriceCatalog", Resources.YouCannotEditThisElement);
                Session["Error"] = Resources.YouCannotEditThisElement;
                ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
                return PartialView("PriceCatalogDetailGrid", ((Item)Session["UnsavedItem"]).PriceCatalogs);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Guid BarcodeId = Request["BarcodeComboBoxID"] != null || Request["BarcodeComboBoxID"] != "null" ? Guid.Parse(Request["BarcodeComboBoxID"]) : Guid.Empty;
                    Guid PriceCatalogID = Request["PriceCatalogComboBoxID"] != null || Request["PriceCatalogComboBoxID"] != "null" ? Guid.Parse(Request["PriceCatalogComboBoxID"]) : Guid.Empty;
                    bool VatIncluded = Request["VatIncluded"] == "true" ? true : false;

                    Item item = (Item)Session["UnsavedItem"];

                    item.PriceCatalogs.Filter = CriteriaOperator.And(new BinaryOperator("PriceCatalog.Oid", PriceCatalogID, BinaryOperatorType.Equal),
                                                new BinaryOperator("Barcode.Oid", BarcodeId, BinaryOperatorType.Equal),
                                                new BinaryOperator("Oid", pcd.Oid, BinaryOperatorType.NotEqual));
                    if (item.PriceCatalogs.Count != 0)
                    {
                        Session["Error"] = Resources.DuplicatePriceCatalogDetail;
                        FillLookupComboBoxes();
                        Session["UnsavedItem"] = item;
                        //Τα details που ανήκουν στους τιμοκαταλόγους του supplier
                        ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
                        return PartialView("PriceCatalogDetailGrid", item.PriceCatalogs);
                    }

                    float disc;
                    float.TryParse(Request["Discount"].Replace("%", ""), out disc);
                    if (disc > 0)
                    {
                        if (pcd.Item.IsTax == true || pcd.Item.DoesNotAllowDiscount == true || Request["IsTax"] == "C" || Request["DoesNotAllowDiscount"] == "C")
                        {
                            Session["Error"] = Resources.NotAllowDiscounts;
                            ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
                            return PartialView("PriceCatalogDetailGrid", item.PriceCatalogs);
                        }
                    }

                    item.PriceCatalogs.Filter = null;
                    PriceCatalog pc = item.Session.FindObject<PriceCatalog>(new BinaryOperator("Oid", PriceCatalogID, BinaryOperatorType.Equal));
                    pcd.PriceCatalog = pc;
                    foreach (Barcode bc in ItemHelper.GetBarcodesOfItem(item))
                    {
                        if (bc.Oid == BarcodeId)
                        {
                            pcd.Barcode = bc;
                            item.PriceCatalogs.Add(pcd);
                            break;
                        }
                    }
                    pcd.IsActive = Request["IsActiveValue"] == "C";
                    pcd.DatabaseValue = ct.DatabaseValue;
                    pcd.MarkUp = ct.MarkUp / 100;
                    pcd.Discount = ct.Discount / 100;
                    pcd.VATIncluded = VatIncluded;

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
                        ViewBag.BarcodeComboBox = item.ItemBarcodes.Select(itemBarcode => itemBarcode.Barcode);
                        ViewBag.CurrentItem = pcd;
                    }
                    else
                    {
                        item.PriceCatalogs.Remove(pcd);
                        item.PriceCatalogs.Add(pcd);
                        Session["UnsavedItem"] = item;
                        Session["UnsavedPriceCatalogDetail"] = null;
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
            //Τα details που ανήκουν στους τιμοκαταλόγους του supplier
            ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));

            return PartialView("PriceCatalogDetailGrid", ((Item)Session["UnsavedItem"]).PriceCatalogs);
        }

        [HttpPost]
        public ActionResult PriceCatalogDetailDeletePartial([ModelBinder(typeof(RetailModelBinder))] PriceCatalogDetail ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (PriceCatalogDetail pcd in item.PriceCatalogs)
                {
                    if (pcd.Oid == ct.Oid)
                    {
                        item.PriceCatalogs.Remove(pcd);
                        pcd.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();

            User currentUser = CurrentUser;
            //Τα details που ανήκουν στους τιμοκαταλόγους του supplier
            ((Item)Session["UnsavedItem"]).PriceCatalogs.Filter = CriteriaOperator.Or(UserHelper.GetUserSupplierPriceCatalogsFilter(currentUser, "PriceCatalog.Oid"));
            return PartialView("PriceCatalogDetailGrid", ((Item)Session["UnsavedItem"]).PriceCatalogs);
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

        public static object PriceCatalogsRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            Item item = (Item)System.Web.HttpContext.Current.Session["UnsavedItem"];

            XPCollection<PriceCatalog> collection = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(),
                                                                CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                    //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                    //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like))
                                                                                    ),
                                                                "Description");

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult PriceCatalogsComboBoxPartial()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ItemAnalyticTreeAddNewPartial([ModelBinder(typeof(RetailModelBinder))] ItemAnalyticTree ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SelectedNodeID = Request["SelectedNodeID"] != null || Request["SelectedNodeID"] != "null" ? Guid.Parse(Request["SelectedNodeID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    ItemAnalyticTree iat = (ItemAnalyticTree)Session["UnsavedItemAnalyticTree"];
                    iat.Node = iat.Session.FindObject<ItemCategory>(new BinaryOperator("Oid", SelectedNodeID, BinaryOperatorType.Equal));
                    iat.Object = item;
                    iat.Root = iat.Node.GetRoot(iat.Session);

                    item.ItemAnalyticTrees.Add(iat);
                    Session["UnsavedItem"] = item;
                    Session["UnsavedItemAnalyticTree"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("ItemAnalyticTreeGrid", ((Item)Session["UnsavedItem"]).ItemAnalyticTrees);
        }

        [HttpPost]
        public ActionResult ItemAnalyticTreeDeletePartial([ModelBinder(typeof(RetailModelBinder))] ItemAnalyticTree ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (ItemAnalyticTree iat in item.ItemAnalyticTrees)
                {
                    if (iat.Oid == ct.Oid)
                    {
                        item.ItemAnalyticTrees.Remove(iat);
                        iat.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("ItemAnalyticTreeGrid", ((Item)Session["UnsavedItem"]).ItemAnalyticTrees);
        }

        public ActionResult LinkedSubItemsGrid(string ItemID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();

                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedLinkedItem"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Item item = (Item)Session["UnsavedItem"];
                    foreach (LinkedItem li in item.LinkedItems)
                    {
                        Guid LinkedItemID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (li.Oid == LinkedItemID)
                        {
                            Session["UnsavedLinkedItem"] = li;
                            break;
                        }
                    }
                }
                return PartialView("LinkedSubItemsGrid", ((Item)Session["UnsavedItem"]).LinkedItems);
            }
            else
            {
                Guid ItemGuid = (ItemID == null || ItemID == "null" || ItemID == "-1") ? Guid.Empty : Guid.Parse(ItemID);
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
                ViewData["ItemID"] = ItemID;
                return PartialView("LinkedSubItemsGrid", item.LinkedItems);
            }
        }

        [HttpPost]
        public ActionResult AddNewLinkedSubItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SubItemID = Request["SubItemID"] != null && Request["SubItemID"] != "null" ? Guid.Parse(Request["SubItemID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    Item subItem = item.Session.FindObject<Item>(new BinaryOperator("Oid", SubItemID, BinaryOperatorType.Equal));
                    if (item.LinkedItems.FirstOrDefault(x => x.SubItem.Oid == subItem.Oid) == null)
                    {
                        LinkedItem li = new LinkedItem(uow);

                        li.SubItem = subItem;
                        li.QtyFactor = ct.QtyFactor;

                        item.LinkedItems.Add(li);
                        Session["UnsavedItem"] = item;
                        Session["UnsavedLinkedItem"] = null;
                    }
                    else
                    {
                        Session["Error"] = Resources.ItemAlreadyExists;
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
            return PartialView("LinkedSubItemsGrid", ((Item)Session["UnsavedItem"]).LinkedItems);
        }

        [HttpPost]
        public ActionResult UpdateLinkedSubItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SubItemID = Request["SubItemID"] != null && Request["SubItemID"] != "null" ? Guid.Parse(Request["SubItemID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    LinkedItem li = (LinkedItem)Session["UnsavedLinkedItem"];
                    Item subItem = item.Session.FindObject<Item>(new BinaryOperator("Oid", SubItemID, BinaryOperatorType.Equal));

                    li.SubItem = subItem;
                    li.QtyFactor = ct.QtyFactor;

                    item.LinkedItems.Remove(li);
                    item.LinkedItems.Add(li);
                    Session["UnsavedItem"] = item;
                    Session["UnsavedLinkedItem"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("LinkedSubItemsGrid", ((Item)Session["UnsavedItem"]).LinkedItems);
        }

        [HttpPost]
        public ActionResult DeleteLinkedSubItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (LinkedItem li in item.LinkedItems)
                {
                    if (li.Oid == ct.Oid)
                    {
                        item.LinkedItems.Remove(li);
                        li.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("LinkedSubItemsGrid", ((Item)Session["UnsavedItem"]).LinkedItems);
        }

        //------------------

        public ActionResult LinkedToItemsGrid(string ItemID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();

                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedLinkedItem"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Item item = (Item)Session["UnsavedItem"];
                    foreach (LinkedItem li in item.SubItems)
                    {
                        Guid LinkedItemID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (li.Oid == LinkedItemID)
                        {
                            Session["UnsavedLinkedItem"] = li;
                            break;
                        }
                    }
                }
                return PartialView("LinkedToItemsGrid", ((Item)Session["UnsavedItem"]).SubItems);
            }
            else
            {
                Guid ItemGuid = (ItemID == null || ItemID == "null" || ItemID == "-1") ? Guid.Empty : Guid.Parse(ItemID);
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
                ViewData["ItemID"] = ItemID;
                return PartialView("LinkedToItemsGrid", item.SubItems);
            }
        }

        [HttpPost]
        public ActionResult AddNewLinkedToItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid BaseItemID = Request["BaseItemID"] != null && Request["BaseItemID"] != "null" ? Guid.Parse(Request["BaseItemID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    Item baseItem = item.Session.FindObject<Item>(new BinaryOperator("Oid", BaseItemID, BinaryOperatorType.Equal));

                    if (item.SubItems.FirstOrDefault(x => x.Item.Oid == baseItem.Oid) == null)
                    {
                        LinkedItem li = new LinkedItem(uow);
                        li.Item = baseItem;
                        li.QtyFactor = ct.QtyFactor;
                        item.SubItems.Add(li);
                        Session["UnsavedItem"] = item;
                        Session["UnsavedLinkedItem"] = null;
                    }
                    else
                    {
                        Session["Error"] = Resources.ItemAlreadyExists;
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
            return PartialView("LinkedToItemsGrid", ((Item)Session["UnsavedItem"]).SubItems);
        }

        [HttpPost]
        public ActionResult UpdateLinkedToItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid BaseItemID = Request["BaseItemID"] != null && Request["BaseItemID"] != "null" ? Guid.Parse(Request["BaseItemID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    LinkedItem li = (LinkedItem)Session["UnsavedLinkedItem"];
                    Item baseItem = item.Session.FindObject<Item>(new BinaryOperator("Oid", BaseItemID, BinaryOperatorType.Equal));

                    li.Item = baseItem;
                    li.QtyFactor = ct.QtyFactor;

                    item.SubItems.Remove(li);
                    item.SubItems.Add(li);
                    Session["UnsavedItem"] = item;
                    Session["UnsavedLinkedItem"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("LinkedToItemsGrid", ((Item)Session["UnsavedItem"]).SubItems);
        }

        [HttpPost]
        public ActionResult DeleteLinkedToItem([ModelBinder(typeof(RetailModelBinder))] LinkedItem ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (LinkedItem li in item.SubItems)
                {
                    if (li.Oid == ct.Oid)
                    {
                        item.SubItems.Remove(li);
                        li.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("LinkedToItemsGrid", ((Item)Session["UnsavedItem"]).SubItems);
        }

        //-------------------
        public JsonResult jsonCheckForExistingItem(string Code)
        {
            string Code1 = (OwnerApplicationSettings.PadItemCodes) ? Code.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Code;
            User currentUser = CurrentUser;
            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);

            if (userSupplier == null && !UserHelper.IsSystemAdmin(currentUser))
            {
                throw new Exception("User is not Supplier");
            }
            Item item;
            item = XpoSession.FindObject<Item>(ApplyOwnerCriteria(new BinaryOperator("Code", Code1, BinaryOperatorType.Equal), typeof(Item)));
            return (item == null) ? Json(new { Error = "" }) : Json(new { ItemID = item.Oid, ItemName = item.Name, ItemCode = item.Code });
        }

        public JsonResult jsonCheckForDuplicateCategory()
        {
            GenerateUnitOfWork();
            Guid SelectedNodeID = Request["SelectedNodeID"] != null || Request["SelectedNodeID"] != "null" ? Guid.Parse(Request["SelectedNodeID"]) : Guid.Empty;
            ItemCategory SelectedNode = uow.FindObject<ItemCategory>(new BinaryOperator("Oid", SelectedNodeID, BinaryOperatorType.Equal));

            Item item = (Item)Session["UnsavedItem"];
            foreach (ItemAnalyticTree iat in item.ItemAnalyticTrees)
            {
                if (SelectedNode.GetRoot(SelectedNode.Session) == iat.Root)
                {
                    return Json(new
                    {
                        hasDuplicate = true
                    });
                }
            }

            return Json(new
            {
                hasDuplicate = false
            });
        }

        public JsonResult jsonCheckForExistingItemStore(String StoreID)
        {
            GenerateUnitOfWork();
            Item unsavedItem = ((Item)Session["UnsavedItem"]);
            ItemStore unsavedItemStore = (ItemStore)Session["UnsavedItemStore"];
            Store selectedStore = uow.FindObject<Store>(new BinaryOperator("Oid", StoreID));
            bool allow = true;
            foreach (ItemStore itemStore in unsavedItem.Stores)
            {
                if (itemStore.Store.Owner == selectedStore.Owner && itemStore.Oid != unsavedItemStore.Oid)
                {
                    allow = false;
                    break;
                }
            }

            return Json(new { allow = allow });
        }

        public JsonResult jsonDeleteItemImage()
        {
            if (Session["UnsavedItem"] != null)
            {
                Item item = Session["UnsavedItem"] as Item;
                item.ImageLarge = null;
                item.ImageSmall = null;
                item.ImageMedium = null;
            }
            return Json(new { success = true });
        }

        public JsonResult jsonDeleteExtraFile()
        {
            if (Session["UnsavedItem"] != null)
            {
                Item item = Session["UnsavedItem"] as Item;
                item.ExtraDescription = null;
                item.ExtraFile = null;
                item.ExtraFilename = null;
                item.ExtraMimeType = null;
            }
            return Json(new { success = true });
        }

        public JsonResult jsonCheckForBarcodeFromItemCode()
        {
            GenerateUnitOfWork();
            Item item = (Item)Session["UnsavedItem"];
            OwnerApplicationSettings settings = (bool)Session["IsNewItem"] == true ? OwnerApplicationSettings : item.Owner.OwnerApplicationSettings;
            bool needsBarcodeFromCode = true;
            bool replaceExisting = false;
            Guid existingBarcodeID = Guid.Empty;
            string previousCode = Request["PreviousItemCode"] ?? "";
            string itemCode = Request["ItemCode"] ?? "";

            string codeBarcode = itemCode;
            if (settings.PadItemCodes)
            {
                codeBarcode = itemCode.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]);
            }
            if (settings.PadBarcodes)
            {
                codeBarcode = itemCode.PadLeft(settings.BarcodeLength, settings.BarcodePaddingCharacter[0]);
            }

            foreach (ItemBarcode ibc in item.ItemBarcodes)
            {
                previousCode = settings.PadItemCodes ? previousCode.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]) : previousCode;

                itemCode = settings.PadItemCodes ? itemCode.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]) : itemCode;

                if (settings.PadBarcodes)
                {
                    if (ibc.Barcode.Code == previousCode.PadLeft(settings.BarcodeLength, settings.BarcodePaddingCharacter[0]))
                    {
                        existingBarcodeID = ibc.Oid;
                    }
                    if (ibc.Barcode.Code == itemCode.PadLeft(settings.BarcodeLength, settings.BarcodePaddingCharacter[0]))
                    {
                        needsBarcodeFromCode = false;
                        break;
                    }
                }
                else
                {
                    if (ibc.Barcode.Code == previousCode)
                    {
                        existingBarcodeID = ibc.Oid;
                    }
                    if (ibc.Barcode.Code == itemCode)
                    {
                        needsBarcodeFromCode = false;
                        break;
                    }
                }
            }

            if (needsBarcodeFromCode)
            {
                if ((previousCode == "") || (itemCode == previousCode) || existingBarcodeID == Guid.Empty)
                {
                    replaceExisting = false;
                }
                else
                {
                    replaceExisting = true;
                }
            }

            #region Validate Time Values

            PlatformPriceCatalogDetailService platformPriceCatalogDetailService = new PlatformPriceCatalogDetailService();
            bool timeValuesResultValid = true;
            string timeValuesMessage = "";
            foreach (PriceCatalogDetail priceCatalogDetail in item.PriceCatalogs)
            {
                ValidationPriceCatalogDetailTimeValuesResult validationResult = platformPriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(priceCatalogDetail.TimeValues);
                if (validationResult != null)
                {
                    timeValuesResultValid = false;
                    timeValuesMessage = (validationResult.PartialOverlap ? Resources.PARTIALLY_OVERLAPPING_TIME_VALUES : Resources.Error)
                                                            + Environment.NewLine + Resources.FromDate + ": " + validationResult.From.ToString()
                                                            + Environment.NewLine + Resources.ToDate + ": " + validationResult.To.ToString();
                }
            }

            #endregion Validate Time Values

            return Json(new
            {
                needsBarcodeFromCode = needsBarcodeFromCode,
                replaceExisting = replaceExisting,
                existingBarcodeID = existingBarcodeID,
                codeBarcode = codeBarcode,
                timeValuesResultValid = timeValuesResultValid,
                timeValuesMessage = timeValuesMessage
            });
        }

        public static object ItemSupplierRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                SupplierNew obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<SupplierNew>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetItemSupplierByValue(object value)
        {
            return GetObjectByValue<SupplierNew>(value);
        }

        public static object ItemSuppliersRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string[] companyNameParts = e.Filter.Split(' ');
            CriteriaOperator companyNameFilterCrop = null;
            foreach (string filt in companyNameParts)
            {
                string flt = (filt.Contains('%')) ? filt : String.Format("%{0}%", filt);
                companyNameFilterCrop = CriteriaOperator.And(companyNameFilterCrop, new BinaryOperator("CompanyName", flt, BinaryOperatorType.Like));
            }

            XPCollection<SupplierNew> collection = GetList<SupplierNew>(XpoHelper.GetNewUnitOfWork(),
                                                                        CriteriaOperator.And(
                                                                            CriteriaOperator.Or(companyNameFilterCrop,
                                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                                //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                                ),
                                                                            new BinaryOperator("IsActive", true)
                                                                                            ),
                                                                        "CompanyName");

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return collection;
        }

        public static object SeasonalityRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                Seasonality obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Seasonality>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetSeasonalityByValue(object value)
        {
            return GetObjectByValue<Seasonality>(value);
        }

        public static object SeasonalityRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Seasonality> collection = GetList<Seasonality>(XpoHelper.GetNewUnitOfWork(),
                                                                        CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                            //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                            //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                            ),
                                                                        "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object BuyerRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                Buyer obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Buyer>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetBuyerByValue(object value)
        {
            return GetObjectByValue<Buyer>(value);
        }

        public static object BuyerRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Buyer> collection = GetList<Buyer>(XpoHelper.GetNewUnitOfWork(),
                                                            CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                ),
                                                            "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult LoadExistingItem(string ItemID)
        {
            GenerateUnitOfWork();
            uow.ReloadChangedObjects();
            uow.RollbackTransaction();

            Guid itemGuid = Guid.Parse(ItemID);
            Item item = uow.FindObject<Item>(new BinaryOperator("Oid", itemGuid, BinaryOperatorType.Equal));

            FillLookupComboBoxes();
            Session["IsNewItem"] = false;
            Session["UnsavedItem"] = item;
            ViewData["EditMode"] = true;
            ViewData["ItemID"] = item.Oid.ToString();
            return View("EditView", item);
        }

        public ActionResult BarcodeGrid(string ItemID, bool editMode)
        {
            Guid ItemOid;
            Item item = (Item)Session["UnsavedItem"] ?? (Guid.TryParse(ItemID, out ItemOid) ? XpoHelper.GetNewUnitOfWork().GetObjectByKey<Item>(ItemOid) : null);
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") || Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                FillLookupComboBoxes();
            }
            ViewData["EditMode"] = editMode;
            ViewData["ItemID"] = ItemID;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView("BarcodeGrid", (item != null ? item.ItemBarcodes : null));
        }

        [HttpPost]
        public ActionResult BarcodeInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] ItemBarcode ct)
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            Item item = (Item)Session["UnsavedItem"];
            OwnerApplicationSettings ownerApplicationSettings = (bool)Session["IsNewItem"] == true ? OwnerApplicationSettings : item.Owner.OwnerApplicationSettings;
            string barcodeCode = Request["BarcodeCode"] != null || Request["BarcodeCode"] != "null" ? Request["BarcodeCode"] : "";
            if (ownerApplicationSettings.PadBarcodes)
            {
                barcodeCode = barcodeCode.PadLeft(ownerApplicationSettings.BarcodeLength, ownerApplicationSettings.BarcodePaddingCharacter[0]);
            }

            CriteriaOperator duplicateCriteria = new BinaryOperator("Barcode.Code", barcodeCode);
            if (!String.IsNullOrEmpty(ct.PluCode))
            {
                duplicateCriteria = CriteriaOperator.Or(duplicateCriteria, new BinaryOperator("PluCode", ct.PluCode));
            }

            ItemBarcode existingIbc = uow.FindObject<ItemBarcode>(PersistentCriteriaEvaluationBehavior.InTransaction,
                                                                  RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(duplicateCriteria,
                                                                                                  new BinaryOperator("Oid", ct.Oid, BinaryOperatorType.NotEqual)),
                                                                                                  typeof(ItemBarcode), item.Owner));
            if (existingIbc != null)
            {
                if (existingIbc.Barcode.Code == barcodeCode)
                {
                    ModelState.AddModelError("BarcodeCode", Resources.BarcodeAlreadyAttached + ": (" + existingIbc.Item.Code + "-" + existingIbc.Item.Name + ")");
                }
                if (existingIbc.PluCode == ct.PluCode)
                {
                    ModelState.AddModelError("PluCode", Resources.PluCodeAlreadyAttached + ": (" + existingIbc.Item.Code + "-" + existingIbc.Item.Name + ")");
                }
            }

            ViewData["EditMode"] = true;
            if (ModelState.IsValid)
            {
                try
                {
                    ItemBarcode ibc = item.ItemBarcodes.FirstOrDefault(itembarcode => itembarcode.Oid == ct.Oid) ?? new ItemBarcode(item.Session);
                    ibc.GetData(ct, new List<string>() { "Session" });
                    ibc.Barcode = ibc.Session.FindObject<Barcode>(new BinaryOperator("Code", barcodeCode, BinaryOperatorType.Equal));
                    if (ibc.Barcode == null)
                    {
                        ibc.Barcode = new Barcode(uow);
                    }

                    ibc.Type = GetObjectByArgument<BarcodeType>(uow, "TypeID") as BarcodeType;
                    ibc.MeasurementUnit = GetObjectByArgument<MeasurementUnit>(uow, "MeasurementUnitID") as MeasurementUnit;
                    ibc.Barcode.Code = barcodeCode;
                    ibc.PluCode = ct.PluCode;
                    ibc.PluPrefix = ct.PluPrefix;
                    AssignOwner(ibc);
                    Session["UnsavedItem"] = ibc.Item = item;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
                ViewBag.CurrentItem = ct;
                FillLookupComboBoxes();
            }
            return PartialView("BarcodeGrid", ((Item)Session["UnsavedItem"]).ItemBarcodes);
        }

        [HttpPost]
        public ActionResult BarcodeInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] ItemBarcode ct)
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            ViewData["EditMode"] = true;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (ItemBarcode ibc in item.ItemBarcodes)
                {
                    if (ibc.Oid == ct.Oid)
                    {
                        foreach (PriceCatalogDetail pcd in ibc.Barcode.PriceCatalogDetails)
                        {
                            item.PriceCatalogs.Remove(pcd);
                            pcd.Delete();
                        }
                        ibc.Delete();
                        ibc.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("BarcodeGrid", ((Item)Session["UnsavedItem"]).ItemBarcodes);
        }

        public ActionResult ItemStockGrid(string ItemID, bool editMode)
        {
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            Item item = null;
            if (editMode)
            {
                item = (Item)Session["UnsavedItem"];
            }
            else
            {
                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(ItemID, out ItemGuid);
                item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
            }
            ViewData["ItemID"] = item.Oid.ToString();
            ViewData["EditMode"] = editMode;
            return PartialView("ItemStockGrid", item.ItemStocks);
        }

        public ActionResult ItemStoreGrid(string ItemID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    ItemStore itemStore = new ItemStore(uow);
                    Session["UnsavedItemStore"] = itemStore;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedItemStore"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Item item = (Item)Session["UnsavedItem"];
                    foreach (ItemStore itemStore in item.Stores)
                    {
                        Guid ItemStoreID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (itemStore.Oid == ItemStoreID)
                        {
                            Session["UnsavedItemStore"] = itemStore;
                            break;
                        }
                    }
                }
                return PartialView("ItemStoreGrid", ((Item)Session["UnsavedItem"]).Stores);
            }
            else  //view mode
            {
                Guid ItemGuid = (ItemID == null || ItemID == "null" || ItemID == "-1") ? Guid.Empty : Guid.Parse(ItemID);
                Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid, BinaryOperatorType.Equal));
                ViewData["ItemID"] = ItemID;
                return PartialView("ItemStoreGrid", item.Stores);
            }
        }

        [HttpPost]
        public ActionResult ItemStoreAddNewPartial([ModelBinder(typeof(RetailModelBinder))] ItemStore ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid StoreID = Request["StoreID"] != null || Request["StoreID"] != "null" ? Guid.Parse(Request["StoreID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    ItemStore itemStore = (ItemStore)Session["UnsavedItemStore"];
                    itemStore.Store = itemStore.Session.FindObject<Store>(new BinaryOperator("Oid", StoreID, BinaryOperatorType.Equal));
                    item.Stores.Add(itemStore);
                    Session["UnsavedItem"] = item;
                    Session["UnsavedItemStore"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("ItemStoreGrid", ((Item)Session["UnsavedItem"]).Stores);
        }

        [HttpPost]
        public ActionResult ItemStoreUpdatePartial([ModelBinder(typeof(RetailModelBinder))] ItemStore ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid StoreID = Request["StoreID"] != null || Request["StoreID"] != "null" ? Guid.Parse(Request["StoreID"]) : Guid.Empty;

                    Item item = (Item)Session["UnsavedItem"];
                    ItemStore itemStore = (ItemStore)Session["UnsavedItemStore"];
                    itemStore.Store = itemStore.Session.FindObject<Store>(new BinaryOperator("Oid", StoreID, BinaryOperatorType.Equal));

                    item.Stores.Add(itemStore);
                    Session["UnsavedItem"] = item;
                    Session["UnsavedItemStore"] = null;
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
            return PartialView("ItemStoreGrid", ((Item)Session["UnsavedItem"]).Stores);
        }

        [HttpPost]
        public ActionResult ItemStoreDeletePartial([ModelBinder(typeof(RetailModelBinder))] ItemStore ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Item item = (Item)Session["UnsavedItem"];
                foreach (ItemStore itemStore in item.Stores)
                {
                    if (itemStore.Oid == ct.Oid)
                    {
                        item.Stores.Remove(itemStore);
                        itemStore.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("ItemStoreGrid", ((Item)Session["UnsavedItem"]).Stores);
        }

        public static object CentralStoresRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),
                                        CriteriaOperator.And(CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.City"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.Description"), e.Filter)
                                                                                //new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                ),
                                                            new BinaryOperator("IsCentralStore", true, BinaryOperatorType.Equal)
                                                            )
                                                           );

            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object StoresRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),
                                                            CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.City"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.Description"), e.Filter)
                                                                                //new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                )
                                                            );
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult StoresComboBoxPartial(string Tab)
        {
            ViewData["Tab"] = Tab;
            return PartialView();
        }

        public ActionResult ItemCategoriesPopup()
        {
            return PartialView();
        }

        public static object BarcodeRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                Barcode obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Barcode>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetBarcodeByValue(object value)
        {
            return GetObjectByValue<Barcode>(value);
        }

        public static object BarcodesRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();
            string nameFilter = e.Filter.Replace('*', '%');
            string codefilter = e.Filter.Replace('*', '%');
            Item item = (Item)System.Web.HttpContext.Current.Session["UnsavedItem"];
            OwnerApplicationSettings settings = (bool)System.Web.HttpContext.Current.Session["IsNewItem"] == true ? OwnerApplicationSettings : item.Owner.OwnerApplicationSettings;
            if (settings.PadBarcodes && !codefilter.Contains('%'))
            {
                codefilter = codefilter.PadLeft(settings.BarcodeLength, settings.BarcodePaddingCharacter[0]);
            }
            XPCollection<Barcode> collection = GetList<Barcode>(uowLocal,
                                                                CriteriaOperator.Or(new BinaryOperator("Item.Name", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Code", String.Format("{0}", codefilter), BinaryOperatorType.Like)),
                                                                "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public override ActionResult Dialog(List<string> arguments)
        {
            if (arguments != null)
            {
                if (arguments[0].ToUpper() == "SEASONALITY")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.Seasonality;

                    //-- The name of the partial to render in the Dialog
                    this.DialogOptions.BodyPartialView = "../Seasonality/SeasonalityEditForm";
                    this.DialogOptions.OKButton.OnClick = "SeasonalityDialogOkButton_OnClick";
                }
                else if (arguments[0].ToUpper() == "BUYER")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.Buyer;

                    this.DialogOptions.BodyPartialView = "../Buyer/BuyerEditForm";
                    this.DialogOptions.OKButton.OnClick = "BuyerDialogOkButton_OnClick";
                }
            }
            return PartialView();
        }

        public JsonResult jsonNewSeasonality()
        {
            Guid newSeasonalityGuid = Guid.Empty;

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User currentUser = CurrentUser;
                ITS.Retail.WebClient.Helpers.ePermition eap = UserHelper.GetUserEntityPermition(currentUser, "Seasonality");
                if (eap.HasFlag(ITS.Retail.WebClient.Helpers.ePermition.Insert))
                {
                    Session["Error"] = "";
                    if (String.IsNullOrWhiteSpace(Request["Code"]))
                    {
                        Session["Error"] += Resources.CodeIsEmpty + Environment.NewLine;
                    }
                    if (String.IsNullOrWhiteSpace(Request["Description"]))
                    {
                        Session["Error"] += Resources.DescriptionIsEmpty + Environment.NewLine;
                    }

                    if (HasDuplicate<Seasonality>(Request["Code"].ToString()))
                    {
                        Session["Error"] += Resources.CodeAlreadyExists + Environment.NewLine;
                    }

                    try
                    {
                        Seasonality seasonality = new Seasonality(uow);
                        seasonality.Code = Request["Code"];
                        seasonality.Description = Request["Description"];
                        seasonality.Owner = seasonality.Session.GetObjectByKey<CompanyNew>((Session["UnsavedItem"] as Item).Owner.Oid);
                        seasonality.Save();
                        XpoHelper.CommitTransaction(uow);
                        newSeasonalityGuid = seasonality.Oid;
                    }
                    catch (Exception e)
                    {
                        string errorMessage = e.GetFullMessage();
                        uow.RollbackTransaction();
                    }
                }

                return Json(new { success = true, seasonalityGuid = newSeasonalityGuid == Guid.Empty ? "" : newSeasonalityGuid.ToString() });
            }
        }

        public JsonResult jsonNewBuyer()
        {
            Guid buyerGuid = Guid.Empty;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User currentUser = CurrentUser;
                ITS.Retail.WebClient.Helpers.ePermition eap = UserHelper.GetUserEntityPermition(currentUser, "Buyer");
                if (eap.HasFlag(ITS.Retail.WebClient.Helpers.ePermition.Insert))
                {
                    Session["Error"] = "";
                    if (String.IsNullOrWhiteSpace(Request["Code"]))
                    {
                        Session["Error"] += Resources.CodeIsEmpty + Environment.NewLine;
                    }
                    if (String.IsNullOrWhiteSpace(Request["Description"]))
                    {
                        Session["Error"] += Resources.DescriptionIsEmpty + Environment.NewLine;
                    }

                    if (HasDuplicate<Buyer>(Request["Code"].ToString()))
                    {
                        Session["Error"] += Resources.CodeAlreadyExists + Environment.NewLine;
                    }

                    try
                    {
                        Buyer buyer = new Buyer(uow);
                        buyer.Code = Request["Code"];
                        buyer.Owner = buyer.Session.GetObjectByKey<CompanyNew>((Session["UnsavedItem"] as Item).Owner.Oid);
                        buyer.Description = Request["Description"];
                        buyer.Save();
                        XpoHelper.CommitTransaction(uow);
                        buyerGuid = buyer.Oid;
                        (Session["UnsavedItem"] as Item).Buyer = (Session["UnsavedItem"] as Item).Session.GetObjectByKey<Buyer>(buyer.Oid, true);
                    }
                    catch (Exception e)
                    {
                        string errorMessage = e.GetFullMessage();
                        uow.RollbackTransaction();
                    }
                }

                return Json(new { success = buyerGuid != Guid.Empty, buyerGuid = buyerGuid == Guid.Empty ? "" : buyerGuid.ToString() });
            }
        }

        [Security(OverrideSecurity = true, ReturnsPartial = false)]
        public ActionResult ExportLabels()
        {
            if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
            {
                Session["Error"] = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description);
                return View("Index");
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string allOids = Request["ItemGuids"];
                string type = Request["Type"];

                if (!String.IsNullOrWhiteSpace(allOids) && !String.IsNullOrWhiteSpace(type))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();
                    foreach (string strOid in strOids)
                    {
                        Guid oid;
                        if (Guid.TryParse(strOid, out oid))
                        {
                            Item item = uow.GetObjectByKey<Item>(oid);
                            oids.Add(item.DefaultBarcode.Oid);
                        }
                    }

                    IEnumerable<PriceCatalogDetail> priceCatalogDetails =
                        PriceCatalogHelper.GetAllSortedPriceCatalogDetails(StoreControllerAppiSettings.CurrentStore, new InOperator("Barcode.Oid", oids));
                    //PriceCatalogHelper.GetTreePriceCatalogDetails(StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalog,new InOperator("Barcode.Oid", oids));

                    using (MemoryStream ms = new MemoryStream())
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> lines = GridExportHelper.GetLabelExportLines(priceCatalogDetails, type, EffectiveOwner);
                        if (lines == null)
                        {
                            Session["Error"] = Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                            return null;
                        }
                        if (lines == null)
                        {
                            Session["Error"] = Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                            return null;
                        }
                        foreach (string line in lines)
                        {
                            sr.WriteLine(line);
                        }
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "labels_export.txt");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public ActionResult ItemExtraInfo()
        {
            Item item = Session["UnsavedItem"] as Item;
            List<ItemExtraInfo> itemExtraInfo = new List<ItemExtraInfo>();
            if (item != null)
            {
                if (MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL)
                {
                    itemExtraInfo = item.ItemExtraInfos.ToList();
                }
                else
                {
                    itemExtraInfo.Add(item.ItemExtraInfos.FirstOrDefault(x => x.Store.Oid == CurrentStore.Oid));
                }
            }
            return PartialView(itemExtraInfo);
        }

        public ActionResult PriceCatalogDetailTimeValueGrid(Guid PriceCatalogDetailOid, bool EditMode)
        {
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            ViewData["EditMode"] = EditMode;
            //ViewData["PriceCatalogDetailOid"] = PriceCatalogDetailOid;

            PriceCatalogDetail priceCatalogDetail = null;
            if (EditMode)
            {
                Item item = Session["UnsavedItem"] as Item;
                if (item != null)
                {
                    priceCatalogDetail = item.PriceCatalogs.FirstOrDefault(x => x.Oid == PriceCatalogDetailOid);
                }
            }
            else
            {
                priceCatalogDetail = uow.GetObjectByKey<PriceCatalogDetail>(PriceCatalogDetailOid);
            }
            ViewData["PriceCatalogDetail"] = priceCatalogDetail;
            if (priceCatalogDetail != null)
            {
                return PartialView(priceCatalogDetail.TimeValues);
            }
            return null;
        }

        [ValidateInput(false)]
        public ActionResult PriceCatalogDetailTimeValueBatchUpdateGrid(MVCxGridViewBatchUpdateValues<PriceCatalogDetailTimeValue, Guid> updateValues, Guid PriceCatalogDetailOid)
        {
            //return null;
            ViewData["EditMode"] = true;
            //ViewData["PriceCatalogDetailOid"] = PriceCatalogDetailOid;

            PriceCatalogDetail priceCatalogDetail = null;
            Item item = Session["UnsavedItem"] as Item;
            if (item != null)
            {
                priceCatalogDetail = item.PriceCatalogs.FirstOrDefault(x => x.Oid == PriceCatalogDetailOid);

                //Deletions
                priceCatalogDetail.TimeValues.Where(x => updateValues.DeleteKeys.Contains(x.Oid)).ToList().ForEach(x => x.Delete());
                //Insertions
                updateValues.Insert.ForEach(x =>
                {
                    PriceCatalogDetailTimeValue newValue = new PriceCatalogDetailTimeValue(priceCatalogDetail.Session);
                    newValue.GetData(x);
                    priceCatalogDetail.TimeValues.Add(newValue);
                });

                //Updates
                updateValues.Update.ForEach(x =>
                {
                    PriceCatalogDetailTimeValue newValue = priceCatalogDetail.TimeValues.FirstOrDefault(y => y.Oid == x.Oid);
                    if (newValue != null)
                    {
                        newValue.TimeValueValidFrom = x.TimeValueValidFrom;
                        newValue.TimeValueValidUntil = x.TimeValueValidUntil;
                        newValue.TimeValue = x.TimeValue;
                        newValue.IsActive = x.IsActive;
                    }
                });
            }
            ViewData["PriceCatalogDetail"] = priceCatalogDetail;
            if (priceCatalogDetail != null)
            {
                return PartialView("PriceCatalogDetailTimeValueGrid", priceCatalogDetail.TimeValues);
            }
            return null;
        }

        public ActionResult ItemExtraInfoGrid(string ItemID, bool editMode)
        {
            Guid ItemOid;
            Item item = (Item)Session["UnsavedItem"] ?? (Guid.TryParse(ItemID, out ItemOid) ? XpoHelper.GetNewUnitOfWork().GetObjectByKey<Item>(ItemOid) : null);
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") || Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                FillLookupComboBoxes();
            }
            ViewData["EditMode"] = editMode;
            ViewData["ItemID"] = ItemID;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (Request["DXCallbackArgument"].Contains("ADDNEW"))
            {
                ItemExtraInfo itemExtraInfo = new ItemExtraInfo(uow);
                Session["UnsavedItemExtraInfo"] = itemExtraInfo;
            }
            else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
            {
                Session["UnsavedItemExtraInfo"] = null;
            }
            else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                item = (Item)Session["UnsavedItem"];
                foreach (ItemExtraInfo itemExtraInfo in item.ItemExtraInfos)
                {
                    Guid ItemExtraInfoID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    if (itemExtraInfo.Oid == ItemExtraInfoID)
                    {
                        Session["UnsavedItemExtraInfo"] = itemExtraInfo;
                        break;
                    }
                }
            }
            return PartialView("ItemExtraInfoGrid", (item != null ? item.ItemExtraInfos : null));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddItemExtraInfo([ModelBinder(typeof(RetailModelBinder))] ItemExtraInfo ct)
        {
            Guid StoreID = Guid.Empty;
            if (MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE || MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
            {
                StoreID = StoreControllerAppiSettings.CurrentStoreOid;
            }
            else
            {
                StoreID = Request["StoreID"] != null || Request["StoreID"] != "null" ? Guid.Parse(Request["StoreID"]) : Guid.Empty;
            }
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;

            Item item = (Item)Session["UnsavedItem"];
            ItemExtraInfo itemExtraInfo = (ItemExtraInfo)Session["UnsavedItemExtraInfo"];
            if (item.ItemExtraInfos.FirstOrDefault(it => it.Store.Oid == StoreID) != null)
            {
                Session["Error"] = Resources.InvalidValue + ":" + Resources.Store;
            }
            else
            {
                ct.Store = ct.Session.GetObjectByKey<Store>(StoreID);
                itemExtraInfo.GetData(ct);
                itemExtraInfo.Item = item;
                itemExtraInfo.Description = Request["ItemExtraInfoDescription"];
                FillItemExtraInfoFields(itemExtraInfo);
            }
            return PartialView("ItemExtraInfoGrid", ((Item)Session["UnsavedItem"]).ItemExtraInfos);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateItemExtraInfo([ModelBinder(typeof(RetailModelBinder))] ItemExtraInfo ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            Item item = (Item)Session["UnsavedItem"];
            try
            {
                ItemExtraInfo itemExtrainfo = item.ItemExtraInfos.FirstOrDefault(it => it.Oid == ct.Oid);
                FillItemExtraInfoFields(itemExtrainfo);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }

            return PartialView("ItemExtraInfoGrid", ((Item)Session["UnsavedItem"]).ItemExtraInfos);
        }

        private void FillItemExtraInfoFields(ItemExtraInfo itemExtrainfo)
        {
            //    "store": getValue("StoresComboBox_ItemExtraInfo")

            itemExtrainfo.Description = Request["ItemExtraInfoDescription"];
            itemExtrainfo.Lot = Request["ItemExtraInfoLot"];
            itemExtrainfo.Ingredients = Request["ItemExtraInfoIngredients"];
            DateTime dateTime;
            if (Request["ItemExtraInfoPackedAt"] == null || String.IsNullOrEmpty(Request["ItemExtraInfoPackedAt"]))
            {
            }
            else if (DateTime.TryParse(Request["ItemExtraInfoPackedAt"].ToString(), out dateTime))
            {
                itemExtrainfo.PackedAt = dateTime;
            }

            if (Request["ItemExtraInfoExpiresAt"] == null || String.IsNullOrEmpty(Request["ItemExtraInfoExpiresAt"]))
            {
            }
            else if (DateTime.TryParse(Request["ItemExtraInfoExpiresAt"].ToString(), out dateTime))
            {
                itemExtrainfo.ExpiresAt = dateTime;
            }

            itemExtrainfo.Origin = Request["ItemExtraInfoOrigin"];
        }

        [HttpPost]
        public ActionResult ItemStockUpdatePartial([ModelBinder(typeof(RetailModelBinder))] ItemStock itemStockView)
        {
            Item item = (Item)Session["UnsavedItem"];
            ItemStock itemStock = item.ItemStocks.FirstOrDefault(stock => stock.Oid == itemStockView.Oid);
            itemStock.DesirableStock = itemStockView.DesirableStock;
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            ViewData["ItemID"] = item.Oid.ToString();
            return PartialView("ItemStockGrid", item.ItemStocks);
        }
    }
}
