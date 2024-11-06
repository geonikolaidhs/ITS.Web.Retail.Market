using System;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageMagick;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Web;

namespace ITS.Retail.WebClient.Controllers
{
    public class LeafletController : BaseObjController<Leaflet>
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

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 200971520
        };
        public ActionResult Index()
        {

            GenerateUnitOfWork();

            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            Session["LeafletFilter"] = filter;

            //this.ToolbarOptions.Visible = true;
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.PrintButton.Visible = this.TableCanExport;

            if (UserHelper.IsCustomer(CurrentUser))
            {
                this.ToolbarOptions.OptionsButton.Visible = false;
            }
            this.ToolbarOptions.PrintButton.OnClick = "ExportSelectedLeaflets";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER ? "AddNewCustomV2" : "";
            this.ToolbarOptions.NewButton.Visible = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER;
            this.ToolbarOptions.EditButton.OnClick = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER ? "EditSelectedRowsCustomV2" : "";
            this.ToolbarOptions.EditButton.Visible = MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER;
            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "LeafletID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdLeaflets");
            
            return View("Index", GetList<Leaflet>(uow, filter).AsEnumerable<Leaflet>());


        }


        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            CriteriaOperator filter = null;

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                ViewData["CallbackMode"] = "SEARCH";
                if (Request.HttpMethod == "POST")
                {
                    string fcode = Request["fcode"] == null || Request["fcode"] == "null" ? "" : Request["fcode"];
                    string fdescription = Request["fdescription"] == null || Request["fdescription"] == "null" ? "" : Request["fdescription"];
                    string fstartDate = Request["fstartDate"] == null || Request["fstartDate"] == "null" ? "" : Request["fstartDate"];
                    string fendDate = Request["fendDate"] == null || Request["fendDate"] == "null" ? "" : Request["fendDate"];

                    CriteriaOperator codeFilter = null;
                    if (fcode != null && fcode.Trim() != "")
                    {
                        if (fcode.Replace('%', '*').Contains("*"))
                        {
                            codeFilter = new BinaryOperator("Code", fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            codeFilter = new BinaryOperator("Code", fcode, BinaryOperatorType.Equal);
                        }
                    }

                    CriteriaOperator descriptionFilter = null;
                    if (fdescription != null && fdescription.Trim() != "")
                    {
                        descriptionFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), fdescription);
                        // new BinaryOperator("Description", "%" + fdescription + "%", BinaryOperatorType.Like);
                    }
                    
                    CriteriaOperator startDateFilter = null;
                    if (fstartDate != "")
                    {
                        startDateFilter = new BinaryOperator("EndDate", DateTime.Parse(fstartDate), BinaryOperatorType.GreaterOrEqual);
                    }

                    CriteriaOperator endDateFilter = null;
                    if (fendDate != "")
                    {
                        endDateFilter = new BinaryOperator("StartDate", DateTime.Parse(fendDate), BinaryOperatorType.LessOrEqual);
                    }




                    CriteriaOperator dateFilter = null;
                    if (UserHelper.IsCustomer(CurrentUser) || UserHelper.IsCompanyUser(CurrentUser))
                    {
                        DateTime now = DateTime.Now;

                        dateFilter = CriteriaOperator.And(new BinaryOperator("StartDate", now, BinaryOperatorType.LessOrEqual),
                                                                     new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual));
                    }

                    filter = CriteriaOperator.And(codeFilter, descriptionFilter,  startDateFilter, endDateFilter,  dateFilter);
                    Session["LeafletFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["LeafletFilter"] = filter;
                }
            }
            this.GridFilter = (CriteriaOperator)Session["LeafletFilter"];
            return base.Grid();

        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.Edit;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public ActionResult EditView(string Oid)
        {
            GenerateUnitOfWork();
            Guid LeafletGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            if (LeafletGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (LeafletGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            Leaflet Leaflet;
            ViewData["EditMode"] = true;
            if (Session["UnsavedLeaflet"] == null)
            {
                if (LeafletGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.Edit;
                    Leaflet = uow.FindObject<Leaflet>(new BinaryOperator("Oid", LeafletGuid, BinaryOperatorType.Equal));
                    Session["IsNewLeaflet"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.New;
                    Leaflet = new Leaflet(uow);
                    Session["IsNewLeaflet"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (LeafletGuid != Guid.Empty && (Session["UnsavedLeaflet"] as Leaflet).Oid == LeafletGuid)
                {
                    Session["IsRefreshed"] = true;
                    Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                }
                else if (LeafletGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    Leaflet = uow.FindObject<Leaflet>(new BinaryOperator("Oid", LeafletGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["LeafletID"] = Leaflet.Oid.ToString();
            Session["UnsavedLeaflet"] = Leaflet;
            if ((bool)Session["IsNewLeaflet"] != true)
            {
                if (Leaflet != null && Leaflet.Image != null)
                {
                    ViewData["LeafletImageDescription"] = Leaflet.ImageDescription;
                    ViewData["LeafletImageInfo"] = Leaflet.ImageInfo;
                }
                else
                {
                    ViewData["LeafletImageDescription"] = "";
                    ViewData["LeafletImageInfo"] = "";
                }
            }
            //ViewData["LeafletDetail"] = Leaflet.LeafletDetails;
            //ViewData["TraderID"] = ct.Trader.Oid.ToString();
            ViewBag.DisplayItemExtraInfo = MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            return PartialView("EditView", Leaflet);
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid LeafletGuid = Guid.Empty;

            bool correctLeafletGuid = Request["LeafletID"] != null && Guid.TryParse(Request["LeafletID"].ToString(), out LeafletGuid);
            if (correctLeafletGuid)
            {
                Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                if (Leaflet != null)
                {
                    if ((bool)Session["IsNewLeaflet"])
                    {
                        AssignOwner(Leaflet);
                    }

                    if (String.IsNullOrWhiteSpace(Request["Code"]))
                    {
                        Session["Error"] = String.Format(Resources.RequiredFieldError, Resources.Code);
                        return Json(new { error = Session["Error"].ToString() });
                    }

                    if (String.IsNullOrWhiteSpace(Request["Description"]))
                    {
                        Session["Error"] = String.Format(Resources.RequiredFieldError, Resources.Description);
                        return Json(new { error = Session["Error"].ToString() });
                    }
                    
                 
                    Leaflet.Code = Request["Code"];
                    Leaflet.Description = Request["Description"];

                    if (Request["StartDate"] != null && Request["StartDate"] != "")
                    {
                        Leaflet.StartDate = DateTime.Parse(Request["StartDate"]);
                    }
                    if (Request["EndDate"] != null && Request["EndDate"] != "")
                    {
                        Leaflet.EndDate = DateTime.Parse(Request["EndDate"]);
                    }
                    if (Request["StartTime"] != null && Request["StartTime"] != "")
                    {
                        Leaflet.StartTime = DateTime.Parse(Request["StartTime"]);
                        Leaflet.StartTime = Leaflet.StartDate.Date + Leaflet.StartTime.TimeOfDay ;
                    }
                    if (Request["EndTime"] != null && Request["EndTime"] != "")
                    {
                        Leaflet.EndTime = DateTime.Parse(Request["EndTime"]);
                        Leaflet.EndTime = Leaflet.EndDate.Date + Leaflet.EndTime.TimeOfDay;
                    }

                    Leaflet.IsActive = Request["IsActive"] != null && !String.IsNullOrEmpty(Request["IsActive"]) && Request["IsActive"] == "C";
                    Leaflet.Save();
                    //uow.CommitTransaction();
                    XpoHelper.CommitTransaction(uow);
                    Session["IsNewLeaflet"] = null;
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                    Session["UnsavedLeaflet"] = null;
                    Session["IsRefreshed"] = null;
                    Session["UnsavedLeafletImage"] = null;
                    if (Session["ImageStream"] != null)
                    {
                        ((MemoryStream)Session["ImageStream"]).Dispose();
                        ((MemoryStream)Session["ImageStream"]).Close();
                        Session["ImageStream"] = null;
                    }
                }
            }
            return Json(new { });

        }

        public JsonResult CancelEdit()
        {
            try
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
                    if (Session["ImageStream"] != null)
                    {
                        ((MemoryStream)Session["ImageStream"]).Dispose();
                        ((MemoryStream)Session["ImageStream"]).Close();
                        Session["ImageStream"] = null;
                    }
                    Session["IsRefreshed"] = null;
                    Session["IsNewLeaflet"] = null;
                    Session["UnsavedLeaflet"] = null;
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult LeafletDetailGrid(string LeafletID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();

                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedLeafletDetail"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid LeafletDetailID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    ViewBag.OwnerApplicationSettings = Leaflet.Owner.OwnerApplicationSettings;
                    foreach (LeafletDetail LeafletDetail in Leaflet.LeafletDetails)
                    {
                        if (LeafletDetail.Oid == LeafletDetailID)
                        {
                            Session["UnsavedLeafletDetail"] = LeafletDetail;
                            break;
                        }
                    }
                }
                //ViewData["LeafletDetail"] = ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails;
                return PartialView("LeafletDetailGrid", ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails);
            }
            else  //view mode
            {
                Guid LeafletGuid = Guid.Parse(LeafletID);
                Leaflet Leaflet = XpoHelper.GetNewUnitOfWork().FindObject<Leaflet>(new BinaryOperator("Oid", LeafletGuid, BinaryOperatorType.Equal));
                ViewData["LeafletID"] = LeafletID;
                ViewBag.OwnerApplicationSettings = Leaflet.Owner.OwnerApplicationSettings;
                return PartialView("LeafletDetailGrid", Leaflet.LeafletDetails);
            }
        }
        
        public ActionResult LeafletDetailAddNew([ModelBinder(typeof(RetailModelBinder))] LeafletDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    Guid ItemBarcodeID = Request["barcodesComboBoxPartialID"] == null || Request["barcodesComboBoxPartialID"] == "-1" || Request["barcodesComboBoxPartialID"] == "null" ? Guid.Empty : Guid.Parse(Request["barcodesComboBoxPartialID"]);
                    ItemBarcode itemBarcode = uow.GetObjectByKey<ItemBarcode>(ItemBarcodeID);
                    LeafletDetail LeafletDetail = new LeafletDetail(uow);
                    
                    LeafletDetail.Item = itemBarcode.Item;
                    LeafletDetail.Barcode = itemBarcode.Barcode;
                    LeafletDetail.Value = ct.Value;
                    Leaflet.LeafletDetails.Add(LeafletDetail);
                    Session["UnsavedLeaflet"] = Leaflet;
                    Session["UnsavedLeafletDetail"] = null;
                    //ViewData["LeafletDetail"] = ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("LeafletDetailGrid", ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails);
        }
        public ActionResult LeafletDetailUpdate([ModelBinder(typeof(RetailModelBinder))] LeafletDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    Guid ItemBarcodeID = Request["barcodesComboBoxPartialID"] == null || Request["barcodesComboBoxPartialID"] == "-1" || Request["barcodesComboBoxPartialID"] == "null" ? Guid.Empty : Guid.Parse(Request["barcodesComboBoxPartialID"]);
                    ItemBarcode itemBarcode = uow.GetObjectByKey<ItemBarcode>(ItemBarcodeID);
                    LeafletDetail LeafletDetail = (LeafletDetail)Session["UnsavedLeafletDetail"];

                    if(Leaflet.LeafletDetails.FirstOrDefault(x => x.Item.Oid == itemBarcode.Item.Oid && x.Barcode.Oid == itemBarcode.Barcode.Oid && x.Oid != LeafletDetail.Oid) != null)
                    {
                        Session["Error"] = Resources.DuplicateLeafletDetail;
                        FillLookupComboBoxes();
                        Session["UnsavedLeaflet"] = Leaflet;
                        ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails.Filter = (CriteriaOperator)Session["LeafletDetailFilter"];
                        return PartialView("LeafletDetailGrid", ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails);
                    }

                    LeafletDetail.Item = itemBarcode.Item;
                    LeafletDetail.Barcode = itemBarcode.Barcode;
                    LeafletDetail.Value = ct.Value;
                    Leaflet.LeafletDetails.Remove(LeafletDetail);
                    Leaflet.LeafletDetails.Add(LeafletDetail);
                    Session["UnsavedLeaflet"] = Leaflet;
                    Session["UnsavedLeafletDetail"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("LeafletDetailGrid", ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails);
        }
        
        public ActionResult LeafletDetailDelete([ModelBinder(typeof(RetailModelBinder))] LeafletDetail ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                LeafletDetail ld = Leaflet.Session.GetObjectByKey<LeafletDetail>(ct.Oid);
                ld.Delete();
                ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails.Filter = (CriteriaOperator)Session["LeafletDetailFilter"];
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("LeafletDetailGrid", ((Leaflet)Session["UnsavedLeaflet"]).LeafletDetails);
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                GenerateUnitOfWork();
                ViewData["EditMode"] = true;

                Leaflet Leaflet = uow.FindObject<Leaflet>(new BinaryOperator("Oid", ViewData["ID"]));
                ViewData["Code"] = Leaflet.Code;
                ViewData["Description"] = Leaflet.Description;
                ViewData["StartDate"] = Leaflet.StartDate.ToShortDateString();
                ViewData["EndDate"] = Leaflet.EndDate.ToShortDateString();
                ViewData["StartTime"] = Leaflet.StartTime.ToShortTimeString();
                ViewData["EndTime"] = Leaflet.EndTime.ToShortTimeString();
                ViewData["IsActive"] = Leaflet.IsActive;
            }
            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }

       
        public static object ItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
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

        public static object ItemsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            Leaflet Leaflet = (Leaflet)System.Web.HttpContext.Current.Session["UnsavedLeaflet"];
            OwnerApplicationSettings settings = (bool)System.Web.HttpContext.Current.Session["IsNewLeaflet"] == true ? OwnerApplicationSettings : Leaflet.Owner.OwnerApplicationSettings;
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            if (settings.PadItemCodes && !codefilter.Contains('%'))
            {
                codefilter = codefilter.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]);
            }

            XPCollection<Item> collection = GetList<Item>(XpoHelper.GetNewUnitOfWork(),
                                                          CriteriaOperator.Or(new BinaryOperator("Name", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                              new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)),
                                                          "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult ItemsComboBox()
        {
            return PartialView();
        }
        
        
        public ActionResult ExportTo()
        {
            return ExportToFile<Leaflet>(Session["LeafletGridSettings"] as GridViewSettings, (CriteriaOperator)Session["LeafletFilter"]);
        }

        public ActionResult LeafletImagePopup()
        {
            Leaflet Leaflet = Session["UnsavedLeaflet"] as Leaflet;
            if (Leaflet != null)
            {
                ViewData["LeafletImageDescription"] = Leaflet.ImageDescription;
                ViewData["LeafletImageInfo"] = Leaflet.ImageInfo;
            }
            else
            {
                ViewData["LeafletImageDescription"] = "";
                ViewData["LeafletImageInfo"] = "";
            }
            return PartialView();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, ImageUpload_FileUploadComplete);
            return null;
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

                Leaflet currentLeaflet = (System.Web.HttpContext.Current.Session["UnsavedLeaflet"] as Leaflet);

                if (currentLeaflet != null)
                {
                    currentLeaflet.Image = PrepareImage(uploadedImage, 300, 300);
                }
            }
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult ImageUploadForm()
        {
            return PartialView("LeafletImagePopup");
        }

        [AllowAnonymous, ActionLog(LogLevel = LogLevel.None)]
        public FileContentResult ShowImageId(String Id, int imageSize = 0)
        {
            GenerateUnitOfWork();
            Guid LeafletGuid;
            ImageConverter converter = new ImageConverter();
            byte[] imageBytes = null;
            string format = "png";
            if (Guid.TryParse(Id, out LeafletGuid))
            {
                Leaflet it = uow.FindObject<Leaflet>(new BinaryOperator("Oid", LeafletGuid));
                if (it != null)
                {
                    Image img;
                    img = it.Image;
                
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
            Leaflet Leaflet = Session["UnsavedLeaflet"] as Leaflet;
            if (Leaflet != null && Leaflet.Image != null && Leaflet.Image.Width > 0)
            {
                Image im = Leaflet.Image;
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

        public JsonResult jsonDeleteLeafletImage()
        {
            if (Session["UnsavedLeaflet"] != null)
            {
                Leaflet Leaflet = Session["UnsavedLeaflet"] as Leaflet;
                Leaflet.Image = null;
            }
            return Json(new { success = true });
        }

        public static Image PrepareImage(MagickImage uploadedImage, int width, int height)
        {
            MagickGeometry geometry = new MagickGeometry(width, height);

            using (MagickImage resizedImage = new MagickImage(uploadedImage))
            {
                resizedImage.Resize(geometry);
                resizedImage.Sharpen();
                resizedImage.Extent(geometry, Gravity.Center, new MagickColor(Color.White));

                ImageFormat format = ImageFormat.Png;
                switch (uploadedImage.Format)
                {
                    case MagickFormat.Jpeg:
                    case MagickFormat.Jpg:
                        format = ImageFormat.Jpeg;
                        break;
                }
                return resizedImage.ToBitmap(format);
            }
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

        public static object CentralStoresRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.City"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.Description"), e.Filter)
                                                                                //new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                )
                                                           );

            //collection.SkipReturnedObjects = e.BeginIndex;
            //collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult StoresComboBoxPartial(string Tab)
        {
            ViewData["Tab"] = Tab;
            return PartialView();
        }

        [HttpPost]
        public ActionResult LeafletStoreAddNewPartial([ModelBinder(typeof(RetailModelBinder))] LeafletStore ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid StoreID = Request["StoreID"] != null || Request["StoreID"] != "null" ? Guid.Parse(Request["StoreID"]) : Guid.Empty;

                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    LeafletStore LeafletStore = (LeafletStore)Session["UnsavedLeafletStore"];
                    LeafletStore.Store = LeafletStore.Session.FindObject<Store>(new BinaryOperator("Oid", StoreID, BinaryOperatorType.Equal));
                    Leaflet.Stores.Add(LeafletStore);
                    Session["UnsavedLeaflet"] = Leaflet;
                    Session["UnsavedLeafletStore"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
                Session["Error"] = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("LeafletStoreGrid", ((Leaflet)Session["UnsavedLeaflet"]).Stores);
        }

        [HttpPost]
        public ActionResult LeafletStoreUpdatePartial([ModelBinder(typeof(RetailModelBinder))] LeafletStore ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (ModelState.IsValid)
            {
                try
                {
                    Guid StoreID = Request["StoreID"] != null || Request["StoreID"] != "null" ? Guid.Parse(Request["StoreID"]) : Guid.Empty;

                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    LeafletStore LeafletStore = (LeafletStore)Session["UnsavedLeafletStore"];
                    LeafletStore.Store = LeafletStore.Session.FindObject<Store>(new BinaryOperator("Oid", StoreID, BinaryOperatorType.Equal));

                    Leaflet.Stores.Add(LeafletStore);
                    Session["UnsavedLeaflet"] = Leaflet;
                    Session["UnsavedLeafletStore"] = null;
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
            return PartialView("LeafletStoreGrid", ((Leaflet)Session["UnsavedLeaflet"]).Stores);
        }

        [HttpPost]
        public ActionResult LeafletStoreDeletePartial([ModelBinder(typeof(RetailModelBinder))] LeafletStore ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            try
            {
                Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                foreach (LeafletStore LeafletStore in Leaflet.Stores)
                {
                    if (LeafletStore.Oid == ct.Oid)
                    {
                        Leaflet.Stores.Remove(LeafletStore);
                        LeafletStore.Delete();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("LeafletStoreGrid", ((Leaflet)Session["UnsavedLeaflet"]).Stores);
        }

        public ActionResult LeafletStoreGrid(string LeafletID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            ViewBag.ApplicationInstance = MvcApplication.ApplicationInstance;
            if (editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    LeafletStore LeafletStore = new LeafletStore(uow);
                    Session["UnsavedLeafletStore"] = LeafletStore;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedLeafletStore"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Leaflet Leaflet = (Leaflet)Session["UnsavedLeaflet"];
                    foreach (LeafletStore LeafletStore in Leaflet.Stores)
                    {
                        Guid LeafletStoreID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (LeafletStore.Oid == LeafletStoreID)
                        {
                            Session["UnsavedLeafletStore"] = LeafletStore;
                            break;
                        }
                    }
                }
                return PartialView("LeafletStoreGrid", ((Leaflet)Session["UnsavedLeaflet"]).Stores);
            }
            else  //view mode
            {
                Guid LeafletGuid = (LeafletID == null || LeafletID == "null" || LeafletID == "-1") ? Guid.Empty : Guid.Parse(LeafletID);
                Leaflet Leaflet = XpoHelper.GetNewUnitOfWork().FindObject<Leaflet>(new BinaryOperator("Oid", LeafletGuid, BinaryOperatorType.Equal));
                ViewData["LeafletID"] = LeafletID;
                return PartialView("LeafletStoreGrid", Leaflet.Stores);
            }
        }

        public JsonResult jsonCheckForExistingLeafletStore(String StoreID)
        {
            GenerateUnitOfWork();
            Leaflet unsavedLeaflet = ((Leaflet)Session["UnsavedLeaflet"]);
            LeafletStore unsavedLeafletStore = (LeafletStore)Session["UnsavedLeafletStore"];
            Store selectedStore = uow.FindObject<Store>(new BinaryOperator("Oid", StoreID));
            bool allow = true;
            foreach (LeafletStore LeafletStore in unsavedLeaflet.Stores)
            {
                if (LeafletStore.Store.Owner == selectedStore.Owner && LeafletStore.Oid != unsavedLeafletStore.Oid)
                {
                    allow = false;
                    break;
                }
            }

            return Json(new { allow = allow });
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

        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            return PartialView();

        }
    }
}
