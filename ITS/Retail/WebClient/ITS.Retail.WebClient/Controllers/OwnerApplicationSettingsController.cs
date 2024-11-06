using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Helpers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Common.ViewModel;


namespace ITS.Retail.WebClient.Controllers
{
    public class OwnerApplicationSettingsController : BaseObjController<OwnerApplicationSettings>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true), ValidateInput(false)]
        public ActionResult Index()
        {
            StoreViewModel sessionStore = Session["currentStore"] as StoreViewModel;
            if (CurrentOwner == null)
            {
                Session["Error"] = Resources.PleaseSelectOwner;
                return View();
            }
            ToolbarOptions.ForceVisible = false;


            if (Request.HttpMethod == "POST" && MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                OwnerApplicationSettings ownerApplicationSettings = (OwnerApplicationSettings)Session["UnsavedOwnerApplicationSettings"];
                if (ownerApplicationSettings != null)
                {
                    string errorMessage = string.Empty;
                    bool ownerApplicationSettingsHaveBeenFilled = FillOwnerApplicationSettings(ownerApplicationSettings, this.Request, out errorMessage);

                    if (!ownerApplicationSettingsHaveBeenFilled)
                    {
                        Session["Error"] = errorMessage;
                        return View();
                    }

                    if (Session["UnsavedOwnerImage"] is OwnerImage)
                    {
                        OwnerImage unsavedOwnerImage = (OwnerImage)Session["UnsavedOwnerImage"];
                        OwnerImage ownerImage = ownerApplicationSettings.Session.GetObjectByKey<OwnerImage>(unsavedOwnerImage.Oid) ?? new OwnerImage(ownerApplicationSettings.Session);
                        ownerImage.GetData(unsavedOwnerImage);
                        ownerImage.OwnerApplicationSettingsOid = ownerApplicationSettings.Oid;
                        ownerApplicationSettings.OwnerImageOid = ownerImage.Oid;
                        ownerImage.Save();
                    }
                    ownerApplicationSettings.Save();
                    ownerApplicationSettings.Session.CommitTransaction();
                    Session["OwnerSettings"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<OwnerApplicationSettings>(ownerApplicationSettings.Oid);

                    this.PrepareLoggedInUserVariables();
                }
            }
            if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER || (!UserHelper.IsAdmin(CurrentUser)))
            {
                ViewBag.editMode = false;
            }
            else
            {
                ViewBag.editMode = true;
            }
            if (uow != null || Session["uow"] != null)
            {
                if (uow != null)
                {
                    uow.Dispose();
                    uow = null;
                }
                Session["uow"] = null;
            }
            GenerateUnitOfWork();
            Session["UnsavedOwnerApplicationSettings"] = uow.GetObjectByKey<OwnerApplicationSettings>(OwnerApplicationSettings.Oid);
            Session["UnsavedOwnerImage"] = uow.GetObjectByKey<OwnerImage>(OwnerApplicationSettings.OwnerImageOid);
            ViewBag.PaymentMehtods = GetList<PaymentMethod>(XpoSession).ToList();
            ViewBag.DiscountTypes = GetList<DiscountType>(XpoSession).ToList();
            ViewBag.Fonts = RetailHelper.GetAvailableFonts(Server.MapPath("~/Content/B2C/customize/fonts"));

            ViewBag.PointsDocumentTypes = GetList<DocumentType>(XpoSession, new BinaryOperator("Division.Section", eDivision.Other)).ToList();
            ViewBag.PointsDocumentSeries = GetList<DocumentSeries>(XpoSession, new ContainsOperator("StoreDocumentSeriesTypes",
                new BinaryOperator("DocumentType.Division.Section", eDivision.Other))).ToList();
            ViewBag.PointsDocumentStatus = GetList<DocumentStatus>(XpoSession).ToList();

            return View();
        }

        public ActionResult TermsEditor()
        {
            string companyID = Request["CompanyID"];
            Guid companyOid = Guid.Empty;
            Guid.TryParse(companyID, out companyOid);

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            CompanyNew company = uow.GetObjectByKey<CompanyNew>(companyOid, false);
            return PartialView(company == null ? null : company.OwnerApplicationSettings);

        }

        /// <summary>
        /// Fills Owner Application Settings properties based on user input
        /// </summary>
        /// <param name="ownerApplicationSettings">The owner application settings to be filled</param>
        /// <param name="Request">html form request containing user input</param>
        /// <param name="errorMessage">string.Empty if no error found, otherwise it cotnains an explanation for the relative failure</param>
        /// <returns>True if all owner application settings are valid, otherwise false</returns>
        public static bool FillOwnerApplicationSettings(OwnerApplicationSettings ownerApplicationSettings, HttpRequestBase Request, out string errorMessage)
        {
            errorMessage = string.Empty;

            ownerApplicationSettings.PadBarcodes = Request["PadBarcodes"] == "C";
            ownerApplicationSettings.PadItemCodes = Request["PadItemCodes"] == "C";

            int barcodeLength;
            int.TryParse(Request["BarcodeLength"], out barcodeLength);
            ownerApplicationSettings.BarcodeLength = barcodeLength;

            int itemCodeLength;
            int.TryParse(Request["ItemCodeLength"], out itemCodeLength);
            ownerApplicationSettings.ItemCodeLength = itemCodeLength;

            ownerApplicationSettings.BarcodePaddingCharacter = Request["BarcodePaddingCharacter"];
            ownerApplicationSettings.ItemCodePaddingCharacter = Request["ItemCodePaddingCharacter"];

            if (ownerApplicationSettings.PadBarcodes && (string.IsNullOrWhiteSpace(ownerApplicationSettings.BarcodePaddingCharacter) || ownerApplicationSettings.BarcodeLength <= 0))
            {
                errorMessage = Resources.PleaseSetBarcodePaddingSettings;
                return false;
            }

            if (ownerApplicationSettings.PadItemCodes && (string.IsNullOrWhiteSpace(ownerApplicationSettings.ItemCodePaddingCharacter) || ownerApplicationSettings.ItemCodeLength <= 0))
            {
                errorMessage = Resources.PleaseSetItemCodePaddingSettings;
                return false;
            }

            double computeDigits;
            Double.TryParse(Request["ComputeDigits"], out computeDigits);
            ownerApplicationSettings.ComputeDigits = computeDigits;

            double displayDigits;
            Double.TryParse(Request["DisplayDigits"], out displayDigits);
            ownerApplicationSettings.DisplayDigits = displayDigits;

            Double displayValueDigits;
            Double.TryParse(Request["DisplayValueDigits"], out displayValueDigits);
            ownerApplicationSettings.DisplayValueDigits = displayValueDigits;

            double maxItemOrderQty;
            Double.TryParse(Request["MaxItemOrderQty"], out maxItemOrderQty);

            ownerApplicationSettings.UseBarcodeRelationFactor = Request["UseBarcodeRelationFactor"] == "C";
            ownerApplicationSettings.DiscountPermited = Request["DiscountPermited"] == "C";
            ownerApplicationSettings.RecomputePrices = Request["RecomputePrices"] == "C";
            ownerApplicationSettings.TrimBarcodeOnDisplay = Request["TrimBarcodeOnDisplay"] == "C";
            ownerApplicationSettings.AllowPriceCatalogSelection = Request["AllowPriceCatalogSelection"] == "C";
            ownerApplicationSettings.EnablePurchases = Request["EnablePurchases"] == "C";
            ownerApplicationSettings.SupportLoyalty = Request["SupportLoyalty"] == "C";
            ownerApplicationSettings.Fonts = Request["Fonts"];

            ownerApplicationSettings.POSCanChangePrices = Request["POSCanChangePrices"] == "C";
            ownerApplicationSettings.POSCanSetPrices = Request["POSCanSetPrices"] == "C";

            int numberOfDaysDocumentCanBeCanceled = 0;
            if (int.TryParse(Request["NumberOfDaysDocumentCanBeCanceled"], out numberOfDaysDocumentCanBeCanceled))
            {
                ownerApplicationSettings.NumberOfDaysDocumentCanBeCanceled = numberOfDaysDocumentCanBeCanceled;
            }

            ownerApplicationSettings.PayPalEmail = Request["PayPalEmail"];

            PayPalMode payPalMode;
            if (Enum.TryParse(Request["PayPalMode"], out payPalMode))
            {
                ownerApplicationSettings.PayPalMode = payPalMode;
            }


            Guid B2CPriceCatalogGuid = Guid.Empty;
            PriceCatalog b2cPriceCatalog = null;
            if (Guid.TryParse(Request["B2CPriceCatalog_VI"], out B2CPriceCatalogGuid))
            {
                b2cPriceCatalog = ownerApplicationSettings.Session.GetObjectByKey<PriceCatalog>(B2CPriceCatalogGuid);
            }
            ownerApplicationSettings.B2CPriceCatalog = b2cPriceCatalog;

            Guid B2CDefaultCustomerGuid = Guid.Empty;
            Customer b2CDefaultCustomer = null;
            if (Guid.TryParse(Request["B2CDefaultCustomer_VI"], out B2CDefaultCustomerGuid))
            {
                b2CDefaultCustomer = ownerApplicationSettings.Session.GetObjectByKey<Customer>(B2CDefaultCustomerGuid);
            }
            ownerApplicationSettings.B2CDefaultCustomer = b2CDefaultCustomer;

            Guid B2CStoreGuid = Guid.Empty;
            Store b2CStore = null;
            if (Guid.TryParse(Request["B2CStore_VI"], out B2CStoreGuid))
            {
                b2CStore = ownerApplicationSettings.Session.GetObjectByKey<Store>(B2CStoreGuid);
            }
            ownerApplicationSettings.B2CStore = b2CStore;


            Guid B2CDocumentTypeGuid = Guid.Empty;
            DocumentType b2CDocumentType = null;
            if (Guid.TryParse(Request["B2CDocumentType_VI"], out B2CDocumentTypeGuid))
            {
                b2CDocumentType = ownerApplicationSettings.Session.GetObjectByKey<DocumentType>(B2CDocumentTypeGuid);
            }
            ownerApplicationSettings.B2CDocumentType = b2CDocumentType;

            Guid B2CDocumentSeriesGuid = Guid.Empty;
            DocumentSeries b2CDocumentSeries = null;
            if (Guid.TryParse(Request["B2CDocumentSeries_VI"], out B2CDocumentSeriesGuid))
            {
                b2CDocumentSeries = ownerApplicationSettings.Session.GetObjectByKey<DocumentSeries>(B2CDocumentSeriesGuid);
            }
            ownerApplicationSettings.B2CDocumentSeries = b2CDocumentSeries;


            Guid CashOnDeliveryGuid = Guid.Empty;
            PaymentMethod cashOnDelivery = null;
            if (Guid.TryParse(Request["CashOnDelivery_VI"], out CashOnDeliveryGuid))
            {
                cashOnDelivery = ownerApplicationSettings.Session.GetObjectByKey<PaymentMethod>(CashOnDeliveryGuid);
            }
            ownerApplicationSettings.CashOnDelivery = cashOnDelivery;

            Guid BankDepositGuid = Guid.Empty;
            PaymentMethod bankDeposit = null;
            if (Guid.TryParse(Request["BankDeposit_VI"], out BankDepositGuid))
            {
                bankDeposit = ownerApplicationSettings.Session.GetObjectByKey<PaymentMethod>(BankDepositGuid);
            }
            ownerApplicationSettings.BankDeposit = bankDeposit;

            Guid PayPalGuid = Guid.Empty;
            PaymentMethod payPal = null;
            if (Guid.TryParse(Request["PayPal_VI"], out PayPalGuid))
            {
                payPal = ownerApplicationSettings.Session.GetObjectByKey<PaymentMethod>(PayPalGuid);
            }
            ownerApplicationSettings.PayPal = payPal;

            Guid pointsDocumentType = Guid.Empty;
            if (Guid.TryParse(Request["PointsDocumentType_VI"], out pointsDocumentType) &&
                ownerApplicationSettings.Session.GetObjectByKey<DocumentType>(pointsDocumentType) != null)
            {
                ownerApplicationSettings.PointsDocumentTypeOid = pointsDocumentType;
            }

            int quantityNumberOfDecimalDigits;
            if (int.TryParse(Request["QuantityNumberOfDecimalDigits"], out quantityNumberOfDecimalDigits))
            {
                ownerApplicationSettings.QuantityNumberOfDecimalDigits = quantityNumberOfDecimalDigits;
            }
            int quantityNumberOfIntegralDigits;
            if (int.TryParse(Request["QuantityNumberOfIntegralDigits"], out quantityNumberOfIntegralDigits))
            {
                ownerApplicationSettings.QuantityNumberOfIntegralDigits = quantityNumberOfIntegralDigits;
            }

            Guid pointsDocumentStatus = Guid.Empty;
            if (Guid.TryParse(Request["PointsDocumentStatus_VI"], out pointsDocumentStatus) &&
                ownerApplicationSettings.Session.GetObjectByKey<DocumentStatus>(pointsDocumentStatus) != null)
            {
                ownerApplicationSettings.PointsDocumentStatusOid = pointsDocumentStatus;
            }

            Guid pointsDocumentSeries = Guid.Empty;
            if (Guid.TryParse(Request["PointsDocumentSeries_VI"], out pointsDocumentSeries) &&
                ownerApplicationSettings.Session.GetObjectByKey<DocumentSeries>(pointsDocumentSeries) != null)
            {
                ownerApplicationSettings.PointsDocumentSeriesOid = pointsDocumentSeries;
            }

            decimal refundPoints;
            decimal.TryParse(Request["RefundPoints"], out refundPoints);
            ownerApplicationSettings.RefundPoints = refundPoints;

            decimal discountAmount;
            decimal.TryParse(Request["DiscountAmount"], out discountAmount);
            ownerApplicationSettings.DiscountAmount = discountAmount;

            decimal discountPercentage;
            decimal.TryParse(Request["DiscountPercentage"], out discountPercentage);
            ownerApplicationSettings.DiscountPercentage = discountPercentage;

            ownerApplicationSettings.ApplicationTerms = HtmlEditorExtension.GetHtml("Terms_Html");
            ownerApplicationSettings.B2CProductsShipping = HtmlEditorExtension.GetHtml("B2CProductsShipping");
            ownerApplicationSettings.B2CTransactionsSafety = HtmlEditorExtension.GetHtml("B2CTransactionsSafety");
            ownerApplicationSettings.B2CCompany = HtmlEditorExtension.GetHtml("B2CCompany");
            ownerApplicationSettings.B2CUsefullInfo = HtmlEditorExtension.GetHtml("B2CUsefullInfo");
            ownerApplicationSettings.B2CFAQ = HtmlEditorExtension.GetHtml("B2CFAQ");

            ownerApplicationSettings.eMail = Request["eMail"];
            ownerApplicationSettings.Phone = Request["Phone"];
            ownerApplicationSettings.FAX = Request["FAX"];
            ownerApplicationSettings.Webpage = Request["Webpage"];
            ownerApplicationSettings.TwitterAccount = Request["TwitterAccount"];
            ownerApplicationSettings.FacebookAccount = Request["FacebookAccount"];
            ownerApplicationSettings.LinkedInAccount = Request["LinkedInAccount"];
            ownerApplicationSettings.LocationGoogleID = Request["LocationGoogleID"];

            ownerApplicationSettings.SmtpHost = Request["SmtpHost"];
            ownerApplicationSettings.SmtpPort = Request["SmtpPort"];
            ownerApplicationSettings.SmtpUsername = Request["SmtpUsername"];
            ownerApplicationSettings.SmtpPassword = Request["SmtpPassword"];
            ownerApplicationSettings.SmtpDomain = Request["SmtpDomain"];
            ownerApplicationSettings.SmtpEmailAddress = Request["SmtpEmailAddress"];
            ownerApplicationSettings.SmtpUseSSL = Request["SmtpUseSSL"] == "C";
            ownerApplicationSettings.MetaDescription = Request["MetaDescription"];
            ownerApplicationSettings.GoogleAnalyticsID = Request["GoogleAnalyticsID"];
            ownerApplicationSettings.EmailTemplateColor1 = Request["EmailTemplateColor1"];
            ownerApplicationSettings.EmailTemplateColor2 = Request["EmailTemplateColor2"];

            ownerApplicationSettings.OnlyRefundStore = Request["OnlyRefundStore"] == "C";
            decimal maximumAllowedDiscountPercentage;
            if (Decimal.TryParse(Request["MaximumAllowedDiscountPercentage"], out maximumAllowedDiscountPercentage))
            {
                ownerApplicationSettings.MaximumAllowedDiscountPercentage = maximumAllowedDiscountPercentage;
            }

            Guid gd;
            if (Guid.TryParse(Request["LoyaltyPaymentMethod_VI"], out gd))
            {
                ownerApplicationSettings.LoyaltyPaymentMethod = ownerApplicationSettings.Session.GetObjectByKey<PaymentMethod>(gd);
            }
            ownerApplicationSettings.LoyaltyOnDocumentSum = Request["LoyaltyOnDocumentSum"] == "C";

            eLoyaltyRefundType loyaltyRefundType;
            Enum.TryParse(Request["LoyaltyRefundType_VI"], out loyaltyRefundType);
            ownerApplicationSettings.LoyaltyRefundType = loyaltyRefundType;

            decimal loyaltyPointsPerDocumentSum;
            decimal.TryParse(Request["LoyaltyPointsPerDocumentSum"], out loyaltyPointsPerDocumentSum);
            ownerApplicationSettings.LoyaltyPointsPerDocumentSum = loyaltyPointsPerDocumentSum;

            decimal documentSumForLoyalty;
            decimal.TryParse(Request["DocumentSumForLoyalty"], out documentSumForLoyalty);
            ownerApplicationSettings.DocumentSumForLoyalty = documentSumForLoyalty;

            decimal PointCost;
            decimal.TryParse(Request["PointCost"], out PointCost);
            ownerApplicationSettings.PointCost = PointCost;

            ePromotionExecutionPriority promotionExecutionPriority;
            Enum.TryParse(Request["PromotionExecutionPriority_VI"], out promotionExecutionPriority);
            ownerApplicationSettings.PromotionExecutionPriority = promotionExecutionPriority;

            decimal markupPriceDeviation;
            if (decimal.TryParse(Request["MarkupDefaultValueDifference"], out markupPriceDeviation))
            {
                ownerApplicationSettings.MarkupDefaultValueDifference = markupPriceDeviation;
            }
            ownerApplicationSettings.UseMarginInsteadMarkup = Request["UseMarginInsteadMarkup"] == "C";


            CustomReport CustomerExportProtocolReport = null;
            if (!string.IsNullOrEmpty(Request["CustomerExportProtocolReport_VI"].ToString()))
                CustomerExportProtocolReport = ownerApplicationSettings.Session.GetObjectByKey<CustomReport>(Guid.Parse(Request["CustomerExportProtocolReport_VI"].ToString()));
            ownerApplicationSettings.CustomerExportProtocolReport = CustomerExportProtocolReport;

            CustomReport CustomerAnonymizationProtocolReport = null;
            if (!string.IsNullOrEmpty(Request["CustomerAnonymizationProtocolReport_VI"].ToString()))
                CustomerAnonymizationProtocolReport = ownerApplicationSettings.Session.GetObjectByKey<CustomReport>(Guid.Parse(Request["CustomerAnonymizationProtocolReport_VI"].ToString()));
            ownerApplicationSettings.CustomerAnonymizationProtocolReport = CustomerAnonymizationProtocolReport;

            CustomReport SupplierExportProtocolReport = null;
            if (!string.IsNullOrEmpty(Request["SupplierExportProtocolReport_VI"].ToString()))
                SupplierExportProtocolReport = ownerApplicationSettings.Session.GetObjectByKey<CustomReport>(Guid.Parse(Request["SupplierExportProtocolReport_VI"].ToString()));
            ownerApplicationSettings.SupplierExportProtocolReport = SupplierExportProtocolReport;

            CustomReport SupplierAnonymizationProtocolReport = null;
            if (!string.IsNullOrEmpty(Request["SupplierAnonymizationProtocolReport_VI"].ToString()))
                SupplierAnonymizationProtocolReport = ownerApplicationSettings.Session.GetObjectByKey<CustomReport>(Guid.Parse(Request["SupplierAnonymizationProtocolReport_VI"].ToString()));
            ownerApplicationSettings.SupplierAnonymizationProtocolReport = SupplierAnonymizationProtocolReport;

            return true;
        }

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

        public JsonResult jsonDeleteOwnerImage()
        {
            GenerateUnitOfWork();
            OwnerApplicationSettings ownerApplicationSettings = null;
            if (Session["UnsavedOwnerApplicationSettings"] != null)
            {
                ownerApplicationSettings = (OwnerApplicationSettings)Session["UnsavedOwnerApplicationSettings"];
            }
            else if (Session["Company"] != null)
            {
                CompanyNew owner = (CompanyNew)Session["Company"];
                if (owner != null)
                {
                    ownerApplicationSettings = owner.OwnerApplicationSettings;
                }
            }
            if (Session["UnsavedOwnerImage"] != null)
            {
                OwnerImage im = (Session["UnsavedOwnerImage"] as OwnerImage);
                //fix case 5631
                if (ownerApplicationSettings != null)
                {
                    ownerApplicationSettings.OwnerImageOid = Guid.Empty;
                }
                im.Delete();
                Session["UnsavedOwnerImage"] = null;
            }
            return Json(new { success = true, OwnerId = ownerApplicationSettings == null ? Guid.Empty.ToString() : ownerApplicationSettings.Owner.Oid.ToString() });
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", ItemController.UploadControlValidationSettings, ImageUpload_FileUploadComplete);
            return null;
        }

        [Security(ReturnsPartial = false, DontLogAction = true)]
        [AllowAnonymous]
        public FileContentResult ShowImageId()
        {
            GenerateUnitOfWork();
            OwnerImage im = null;
            OwnerApplicationSettings it = null;
            if (Session["UnsavedOwnerApplicationSettings"] != null)
            {
                it = (OwnerApplicationSettings)Session["UnsavedOwnerApplicationSettings"];
            }
            else if (Session["Company"] != null)
            {
                CompanyNew owner = (CompanyNew)Session["Company"];
                if (owner != null)
                {
                    if (uow != null)
                    {
                        uow.Dispose();
                        uow = null;
                        Session["uow"] = null;
                    }
                    uow = (UnitOfWork)owner.Session;
                    it = owner.OwnerApplicationSettings;
                }
            }

            im = (it != null) ? Session["UnsavedOwnerImage"] as OwnerImage ?? it.Session.FindObject<OwnerImage>(new BinaryOperator("Oid", it.OwnerImageOid)) :
                null;

            if (im != null)
            {
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(im.Image, typeof(byte[]));
                string format = "";

                if (im.Image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    format = "jpeg";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Gif)))
                {
                    format = "gif";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Png)))
                {
                    format = "png";
                }

                return new FileContentResult(imageBytes, "image/" + format);
            }
            else
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/wrm_light.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }
        }


        [Security(ReturnsPartial = false, DontLogAction = true)]
        [AllowAnonymous]
        public FileContentResult ShowImageOid(Guid OwnerGuid)
        {
            GenerateUnitOfWork();
            OwnerImage im = null;

            OwnerApplicationSettings it = uow.FindObject<OwnerApplicationSettings>(new BinaryOperator("Owner.Oid", OwnerGuid));
            if (it != null && it.OwnerImageOid != null)
            {
                im = uow.FindObject<OwnerImage>(new BinaryOperator("Oid", it.OwnerImageOid));
            }


            if (im != null)
            {
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(im.Image, typeof(byte[]));
                string format = "";

                if (im.Image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    format = "jpeg";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Gif)))
                {
                    format = "gif";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Png)))
                {
                    format = "png";
                }

                return new FileContentResult(imageBytes, "image/" + format);
            }
            else
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/wrm_light.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }
        }

        public static void ImageUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                HttpSessionState currentSession = System.Web.HttpContext.Current.Session;
                e.CallbackData = "success";
                if (System.Web.HttpContext.Current.Session["ImageStream"] != null)
                {
                    ((MemoryStream)currentSession["ImageStream"]).Dispose();
                    ((MemoryStream)currentSession["ImageStream"]).Close();
                }
                MemoryStream outMs = new MemoryStream(e.UploadedFile.FileBytes);
                System.Web.HttpContext.Current.Session["ImageStream"] = outMs;  //must keep alive
                Image uploadedImage = Image.FromStream(outMs);
                OwnerApplicationSettings currentOwner = currentSession["Company"] == null ?
                    OwnerApplicationSettings : (currentSession["Company"] as CompanyNew).OwnerApplicationSettings;

                if (currentOwner != null)
                {
                    OwnerImage ownerImage = (currentSession["UnsavedOwnerImage"] as OwnerImage);
                    if (ownerImage == null)
                    {
                        ownerImage = new OwnerImage(currentOwner.Session);
                        currentSession["UnsavedOwnerImage"] = ownerImage;
                    }

                    int[] dimensions = new int[2];
                    dimensions = ImageUtilities.calculateAspectRatioFit(uploadedImage.Width, uploadedImage.Height, 300, 300);
                    ownerImage.Image = ImageUtilities.ResizeImage(uploadedImage, dimensions[0], dimensions[1]);
                }
            }
        }

        public static object PriceCatalogsRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<PriceCatalog> collection = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(),
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

        public static object PriceCatalogRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                PriceCatalog obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PriceCatalog>(e.Value);
                return obj;
            }
            return null;
        }

        public static object StoreRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {

            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),
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

        public static object DocumentTypesRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            Store store = (Store)System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CStore"];
            if (store == null)
            {
                return null;
            }
            XPCollection<DocumentType> collection = GetList<DocumentType>(XpoHelper.GetNewUnitOfWork(),
                                CriteriaOperator.And(new BinaryOperator("Division.Section", eDivision.Sales),
                                                     new ContainsOperator("StoreDocumentSeriesTypes",
                                                                          StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(store,
                                                                                    DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance))),
                                                     CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                         new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like))), "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object DocumentTypeRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                DocumentType obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentType>(e.Value);
                System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CDocumentType"] = obj;
                return obj;
            }
            System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CDocumentType"] = null;
            return null;
        }

        public static object DocumentSeriesRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            HttpSessionState currentSession = System.Web.HttpContext.Current.Session;
            OwnerApplicationSettings currentOwner = currentSession["Company"] == null ? OwnerApplicationSettings : (currentSession["Company"] as CompanyNew).OwnerApplicationSettings;
            DocumentType b2CDocumentType = currentOwner.B2CDocumentType;

            if (System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CDocumentType"] != null)
            {
                b2CDocumentType = (DocumentType)(System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CDocumentType"]);
            }

            if (b2CDocumentType == null)
            {
                return new List<DocumentSeries>() { };
            }

            Store store = currentOwner.B2CStore ?? ((Store)System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CStore"]);

            if (store == null)
            {
                return new List<DocumentSeries>();
            }

            CriteriaOperator criteria = CriteriaOperator.And(new ContainsOperator("StoreDocumentSeriesTypes",
                                                                                   CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", b2CDocumentType.Oid),
                                                                                   StoreHelper.StoreDocumentSeriesTypeForDocTypeCriteria(store,
                                                                                                                                         DocumentHelper.GetDocSeriesModule(MvcApplication.ApplicationInstance)))),
                                                             CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                 new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)));
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            {
                XPCollection<DocumentSeries> collection = GetList<DocumentSeries>(uow, criteria, "Description");
                collection.SkipReturnedObjects = e.BeginIndex;
                collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
                return collection;
            }
        }


        public static object CustomerRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Customer> collection = GetList<Customer>(XpoHelper.GetNewUnitOfWork(),
                                                                  CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                      new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)),
                                                                  "Description"
                                                                 );
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object DocumentSeriesRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                DocumentSeries obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentSeries>(e.Value);
                return obj;
            }
            return null;
        }

        public ActionResult B2CPriceCatalog()
        {
            ViewData["Name"] = Request["name"];
            return PartialView();
        }

        public ActionResult B2CDocumentType(Guid? storeID)
        {
            GenerateUnitOfWork();
            System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CStore"] = uow.GetObjectByKey<Store>(storeID);
            ViewData["Name"] = Request["name"];
            return PartialView();
        }

        public ActionResult B2CStore()
        {
            ViewData["Name"] = Request["name"];
            return PartialView();
        }
        public ActionResult B2CDocumentSeries(Guid? storeID, Guid? documentTypeID)
        {
            GenerateUnitOfWork();
            ViewData["Name"] = Request["name"];
            System.Web.HttpContext.Current.Session["OwnerApplicationSettingsB2CDocumentType"] = uow.GetObjectByKey<DocumentType>(documentTypeID);
            return PartialView();
        }
        public ActionResult B2CDefaultCustomer()
        {
            ViewData["Name"] = Request["name"];
            return PartialView();
        }
        public ActionResult B2CProductsShipping()
        {
            return PartialView();
        }
        public ActionResult B2CTransactionsSafety()
        {
            return PartialView();
        }
        public ActionResult B2CCompany()
        {
            return PartialView();
        }
        public ActionResult B2CUsefullInfo()
        {
            return PartialView();
        }
        public ActionResult B2CFAQ()
        {
            return PartialView();
        }

        public ActionResult PaymentMethodAttribute()
        {
            ViewData["Name"] = Request["name"];
            return PartialView();
        }

        public static object PaymentMethodRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<PaymentMethod> collection = GetList<PaymentMethod>(XpoHelper.GetNewUnitOfWork(),
                                                     CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                         new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)),
                                                     "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object PaymentMethodRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                PaymentMethod obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PaymentMethod>(e.Value);
                return obj;
            }
            return null;
        }
        public static object CustomReportMethodRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<CustomReport> collection = GetList<CustomReport>(XpoHelper.GetNewUnitOfWork(),
                                                     CriteriaOperator.Or(new BinaryOperator("Title", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                         new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)),
                                                     "Title");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object CustomReportMethodRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                CustomReport obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CustomReport>(e.Value);
                return obj;
            }
            return null;
        }
        public ActionResult ReportsComboBox()
        {
            ViewData["Name"] = Request["name"];
            return PartialView();
        }
    }
}
