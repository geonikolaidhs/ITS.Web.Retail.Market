using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Licensing.Common.Implementations;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.PrintServer.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class DocumentTypeController : BaseObjController<DocumentType>
    {

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.Edit;

            if (Request["Copy"] != null)
            {
                ViewData["Copy"] = Request["Copy"];
            }

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            ViewData["Copy"] = Request["Copy"];

            return PartialView();

        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            const string insertItemJSFunction = "AddNewCustomV2";
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CopyButton.Visible = ToolbarOptions.NewButton.Visible;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.CopyButton.OnClick = "CopySelectedRow";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.NewButton.OnClick = insertItemJSFunction;
            ToolbarOptions.CopyButton.OnClick = "CopySelectedRow";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("objectToBeCopied", Guid.Empty.ToString());
            CustomJSProperties.AddJSProperty("gridName", "grdDocumentType");
            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "documentTypeOid");
            CustomJSProperties.AddJSProperty("insertItemJSFunction", insertItemJSFunction);


            FillLookupComboBoxes();

            return View("Index", GetList<DocumentType>(XpoHelper.GetNewUnitOfWork()).AsEnumerable());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsActive", "DocumentTypeMapping", "DeficiencySettings", "DisplaysMarkUpForm", "IsDefault" });
            ruleset.DetailsToIgnore.Add("TransformsFrom");
            ruleset.DetailPropertiesToIgnore.Add(typeof(TransformationRule), new List<string>() { "Description", "IsActive" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(DocumentTypeCustomReport), new List<string>() { "IsActive" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(DocumentTypeRole), new List<string>() { "IsActive" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(StoreDocumentSeriesType), new List<string>() { "IsActive", "Duplicates", "DefaultSupplier", "DefaultCustomReport", "MenuDescription" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(ActionTypeEntity), new List<string>() { "IsActive", "EntityType", "EntityOid", "EntityCode", "UpdateMode", "ValidVariableActionTypes", "Store" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(DocumentTypeBarcodeType), new List<string>() { "IsActive", "DocumentType" });
            ruleset.NumberOfColumns = 4;
            return ruleset;
        }

        public override ActionResult Grid()
        {
            FillLookupComboBoxes();
            return base.Grid();
        }

        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] DocumentType ct)
        {
            if (!TableCanInsert)
            {
                return null;
            }
            AddModelErrors(ct);

            if (ModelState.IsValid)
            {
                Session["Error"] = null;
                try
                {
                    AssignOwner(ct);
                    Save(ct);
                    Session["Notice"] = Resources.SavedSuccesfully;

                }
                catch (Exception e)
                {
                    if (Session["Error"] != null)
                    {
                        if (Session["Error"].ToString().Contains("Default value has already been selected"))
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                        }
                        else
                        {
                            Session["Error"] = Resources.CodeAlreadyExists;
                        }
                    }
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<DocumentType>(XpoHelper.GetNewUnitOfWork()).AsEnumerable());
        }

        public ActionResult Edit(string Oid, Guid? Copy = null)
        {
            DocumentType documentType = null;

            if (Copy != Guid.Empty)
            {
                Guid docTypeGuid;
                if (Guid.TryParse(Copy.ToString(), out docTypeGuid))
                {
                    documentType = ViewBag.CurrentItem = DocumentTypeHelper.CopyDocumentType(docTypeGuid);
                }
            }
            else
            {
                Guid documentTypeGuid;
                if (Guid.TryParse(Oid, out documentTypeGuid))
                {
                    if (documentTypeGuid == Guid.Empty)
                    {
                        documentType = new DocumentType(XpoHelper.GetNewUnitOfWork());
                    }
                    else
                    {
                        documentType = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentType>(documentTypeGuid);
                    }
                }
                if (documentType == null)
                {
                    return null;
                }
            }
            Session["currentDocumentType"] = documentType;

            FillLookupComboBoxes();
            return PartialView(documentType);
        }

        public JsonResult GetPurchaseOid()
        {
            Guid resultGuid = Guid.Empty;
            resultGuid = XpoSession.FindObject<Division>(new BinaryOperator("Section", eDivision.Purchase)).Oid;
            DocumentType documentType = Session["currentDocumentType"] as DocumentType;
            Guid divisionGuid;
            if (documentType != null && Request["itemOid"] is string && Guid.TryParse(Request["itemOid"], out divisionGuid))
            {
                documentType.Division = documentType.Session.Query<Division>().FirstOrDefault(division => division.Oid == divisionGuid);
            }
            return Json(new { result = resultGuid });
        }

        public JsonResult Save()
        {
            Session["Error"] = "";
            DocumentType documentType = Session["currentDocumentType"] as DocumentType;

            int qtyFactor, valFactor, linkedLineQuantityFactor, linkedLineValueFactor;
            uint maxCountOfLines;
            decimal maxDetailQty, maxDetailVal, maxDetailTotal, maxPaymentAmount;

            Guid mdtGuid;
            Guid divisionGuid;
            Guid docstatusGuid;
            Guid paymentMethodGuid;
            Guid reasonCategoryGuid;

            string code = Request["Code"];
            string ReferenceCode = Request["ReferenceCode"];
            string description = Request["Description"];
            string qtyFactorString = Request["QuantityFactor"];
            string valFactorString = Request["ValueFactor"];
            string linkedLineQuantityFactorString = Request["LinkedLineQuantityFactor"];
            string linkedLineValueFactorQuantityString = Request["LinkedLineValueFactor"];

            bool mergedSameDocumentLines = Request["MergedSameDocumentLines"].Equals("C");
            bool usesPrices = Request["UsesPrices"].Equals("C");
            bool takesDigitalSignature = Request["TakesDigitalSignature"].Equals("C");
            bool isForWholesale = Request["IsForWholesale"] != null ? Request["IsForWholesale"].Equals("C") : true;
            bool AllowItemZeroPrices = Request["AllowItemZeroPrices"].Equals("C");
            bool IncreaseVatAndSales = Request["IncreaseVatAndSales"] != null ? Request["IncreaseVatAndSales"].Equals("C") : true;
            bool JoinInTotalizers = Request["JoinInTotalizers"] != null ? Request["JoinInTotalizers"].Equals("C") : true;
            //bool IsOfValues = Request["IsOfValues"].Equals("C");
            //bool IsQuantitative = Request["IsQuantitative"].Equals("C");
            bool UsesMarkUp = Request["UsesMarkUp"].Equals("C");
            bool UsesMarkUpForm = Request["UsesMarkUpForm"].Equals("C");
            bool UsesPaymentMethods = Request["UsesPaymentMethods"].Equals("C");
            bool SupportLoyalty = Request["SupportLoyalty"].Equals("C");
            bool documentHeaderCanBeCopied = Request["DocumentHeaderCanBeCopied"].Equals("C");
            bool recalculateDocumentDetailsOnTraderChange = Request["RecalculatePricesOnTraderChange"].Equals("C");
            bool isPrintedOnStoreController = Request["IsPrintedOnStoreController"].Equals("C");
            bool chargeToUser = Request["ChargeToUser"].Equals("C");
            bool supportCustomerVatLevel = Request["SupportCustomerVatLevel"].Equals("C");
            bool updateSalesRecords = Request["UpdateSalesRecords"].Equals("C");



            eDocumentType eDocumentType = eDocumentType.NONE;
            Enum.TryParse(Request["Type_VI"], out eDocumentType);
            eDocTypeCustomerCategory eDocTypeCustomerCategory = eDocTypeCustomerCategory.NONE;
            Enum.TryParse(Request["DocTypeCustomerCategoryMode_VI"], out eDocTypeCustomerCategory);
            eDocumentTypeMeasurementUnit eDocumentTypeMeasurementUnit = eDocumentTypeMeasurementUnit.DEFAULT;
            Enum.TryParse(Request["MeasurementUnitMode_VI"], out eDocumentTypeMeasurementUnit);
            eDocumentTypeItemCategory eDocumentTypeItemCategory = eDocumentTypeItemCategory.NONE;
            Enum.TryParse(Request["DocumentTypeItemCategoryMode_VI"], out eDocumentTypeItemCategory);
            ePriceSuggestionType epriceSuggestionType;
            Enum.TryParse(Request["PriceSuggestionType_VI"], out epriceSuggestionType);
            eDocumentTraderType traderType;
            Enum.TryParse(Request["TraderTypeCombobox_VI"], out traderType);
            ItemStockAffectionOptions itemStockAffectionOptions = ItemStockAffectionOptions.NO_AFFECTION;
            Enum.TryParse(Request["ItemStockAffectionOptions_VI"], out itemStockAffectionOptions);

            switch (qtyFactorString)
            {
                case "0":
                    qtyFactor = -1;
                    break;
                case "1":
                    qtyFactor = 0;
                    break;
                case "2":
                    qtyFactor = 1;
                    break;
                default:
                    qtyFactor = 0;
                    break;
            }

            switch (valFactorString)
            {
                case "0":
                    valFactor = -1;
                    break;
                case "1":
                    valFactor = 0;
                    break;
                case "2":
                    valFactor = 1;
                    break;
                default:
                    valFactor = 0;
                    break;
            }

            switch (linkedLineQuantityFactorString)
            {
                case "0":
                    linkedLineQuantityFactor = -1;
                    break;
                case "1":
                    linkedLineQuantityFactor = 0;
                    break;
                case "2":
                    linkedLineQuantityFactor = 1;
                    break;
                default:
                    linkedLineQuantityFactor = 0;
                    break;
            }

            switch (linkedLineValueFactorQuantityString)
            {
                case "0":
                    linkedLineValueFactor = -1;
                    break;
                case "1":
                    linkedLineValueFactor = 0;
                    break;
                case "2":
                    linkedLineValueFactor = 1;
                    break;
                default:
                    linkedLineValueFactor = 0;
                    break;
            }


            if (String.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("Code", Resources.CodeIsEmpty);
                Session["Error"] += Resources.CodeIsEmpty;
            }
            if (String.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("Description", Resources.DescriptionIsEmpty);
                Session["Error"] += Resources.DescriptionIsEmpty;
            }


            documentType.Code = code;
            documentType.IncreaseVatAndSales = IncreaseVatAndSales;
            documentType.JoinInTotalizers = JoinInTotalizers;
            documentType.ReferenceCode = ReferenceCode;
            documentType.Description = description;
            documentType.MergedSameDocumentLines = mergedSameDocumentLines;
            documentType.UsesPrices = usesPrices;
            documentType.TakesDigitalSignature = takesDigitalSignature;
            documentType.IsForWholesale = isForWholesale;
            documentType.AllowItemZeroPrices = AllowItemZeroPrices;
            documentType.QuantityFactor = qtyFactor;
            documentType.LinkedLineQuantityFactor = linkedLineQuantityFactor;
            //documentType.IsOfValues = IsOfValues;
            //documentType.IsQuantitative = IsQuantitative;
            documentType.UsesMarkUp = UsesMarkUp;
            documentType.UsesMarkUpForm = UsesMarkUpForm;
            documentType.UsesPaymentMethods = UsesPaymentMethods;
            documentType.ValueFactor = valFactor;
            documentType.LinkedLineValueFactor = linkedLineValueFactor;
            documentType.SupportLoyalty = SupportLoyalty;
            documentType.Type = eDocumentType;
            documentType.MeasurementUnitMode = eDocumentTypeMeasurementUnit;
            documentType.DocumentHeaderCanBeCopied = documentHeaderCanBeCopied;
            documentType.RecalculatePricesOnTraderChange = recalculateDocumentDetailsOnTraderChange;
            documentType.DocTypeCustomerCategoryMode = eDocTypeCustomerCategory;
            documentType.DocumentTypeItemCategoryMode = eDocumentTypeItemCategory;
            documentType.PriceSuggestionType = epriceSuggestionType;
            documentType.AcceptsGeneralItems = Request["AcceptsGeneralItems"].Equals("C");
            documentType.ReserveCoupons = Request["ReserveCoupons"].Equals("C");
            documentType.FormDescription = Request["FormDescription"];
            documentType.IsPrintedOnStoreController = isPrintedOnStoreController;
            documentType.ManualLinkedLineInsertion = Request["ManualLinkedLineInsertion"].Equals("C");
            documentType.InitialisesQuantities = Request["InitialisesQuantities"].Equals("C");
            documentType.InitialisesValues = Request["InitialisesValues"].Equals("C");
            documentType.AffectsCustomerBalance = Request["AffectsCustomerBalance"].Equals("C");
            documentType.ItemStockAffectionOptions = itemStockAffectionOptions;
            documentType.AllowItemValueEdit = Request["AllowItemValueEdit"].Equals("C");
            documentType.DisplayInCashCount = Request["DisplayInCashCount"].Equals("C");
            documentType.ChargeToUser = chargeToUser;
            documentType.SupportCustomerVatLevel = supportCustomerVatLevel;
            documentType.UpdateSalesRecords = updateSalesRecords;

            documentType.MinistryDocumentType = Guid.TryParse(Request["MinistryDocumentTypeComboBox_VI"], out mdtGuid) ? documentType.Session.GetObjectByKey<MinistryDocumentType>(mdtGuid) : null;

            if (takesDigitalSignature && mdtGuid == Guid.Empty)
            {
                ModelState.AddModelError("MinistryDocumentType", Resources.YouHaveChosenAnDigitalSignatureButYouHaveNoteSelectedAnMinistryDocumentType);
                Session["Error"] += Resources.YouHaveChosenAnDigitalSignatureButYouHaveNoteSelectedAnMinistryDocumentType;
            }

            documentType.Division = Guid.TryParse(Request["Division_VI"], out divisionGuid) ? documentType.Session.GetObjectByKey<Division>(divisionGuid) : null;

            documentType.DefaultDocumentStatus = Guid.TryParse(Request["DefaultDocumentStatus_VI"], out docstatusGuid) ? documentType.Session.GetObjectByKey<DocumentStatus>(docstatusGuid) : null;

            documentType.DefaultPaymentMethod = Guid.TryParse(Request["DefaultPaymentMethod_VI"], out paymentMethodGuid) ? documentType.Session.GetObjectByKey<PaymentMethod>(paymentMethodGuid) : null;

            documentType.ReasonCategory = Guid.TryParse(Request["ReasonCategory_VI"], out reasonCategoryGuid) ? documentType.Session.GetObjectByKey<ReasonCategory>(reasonCategoryGuid) : null;

            if (uint.TryParse(Request["MaxCountOfLines"], out maxCountOfLines))
            {
                documentType.MaxCountOfLines = maxCountOfLines;
            }
            else
            {
                documentType.MaxCountOfLines = 0;
            }

            if (decimal.TryParse(Request["MaxDetailQty"], out maxDetailQty))
            {
                documentType.MaxDetailQty = maxDetailQty;
            }

            if (decimal.TryParse(Request["MaxDetailValue"], out maxDetailVal))
            {
                documentType.MaxDetailValue = maxDetailVal;
            }

            if (decimal.TryParse(Request["MaxDetailTotal"], out maxDetailTotal))
            {
                documentType.MaxDetailTotal = maxDetailTotal;
            }

            if (decimal.TryParse(Request["MaxPaymentAmount"], out maxPaymentAmount))
            {
                documentType.MaxPaymentAmount = maxPaymentAmount;
            }

            if (decimal.TryParse(Request["ReasonCategory"], out maxPaymentAmount))
            {
                documentType.MaxPaymentAmount = maxPaymentAmount;
            }

            if (HasDuplicate(documentType))
            {
                ModelState.AddModelError("Code", Resources.CodeAlreadyExists);
                Session["Error"] += Resources.CodeAlreadyExists;
            }

            if (HasIsDefaultDuplicates(documentType))
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
                Session["Error"] += Resources.DefaultAllreadyExists;
            }

            if (documentType.Type != eDocumentType.NONE)
            {
                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Type", documentType.Type), new BinaryOperator("Oid", documentType.Oid, BinaryOperatorType.NotEqual));
                if (GetList<DocumentType>(documentType.Session, crop).Count > 0)
                {
                    ModelState.AddModelError("Type", Resources.DocumentTypeHasAlreadyBeenSet);
                    Session["Error"] += Resources.DocumentTypeHasAlreadyBeenSet;
                }
            }

            if (documentType.Division == null)
            {
                ModelState.AddModelError("Division", Resources.DivisionIsEmpty);
                Session["Error"] += Resources.DivisionIsEmpty;
            }
            else
            {
                List<eDocumentTraderType> availableTypes = documentType.Division.Section.GetAvailableTraderTypes().ToEnumValuesEnumerable().ToList();
                if (availableTypes.Contains(traderType))
                {
                    documentType.TraderType = traderType;
                }
                else if (availableTypes.Count == 1)
                {
                    documentType.TraderType = availableTypes[0];
                }
                else
                {
                    ModelState.AddModelError("TraderTypeCombobox", Resources.InvalidValue);
                    Session["Error"] += Resources.InvalidValue;
                }
            }

            Guid specialItemGuid = Guid.Empty;
            Guid.TryParse(Request["DocumentTypeSpecialItemComboBox_VI"], out specialItemGuid);
            documentType.SpecialItem = documentType.Session.GetObjectByKey<SpecialItem>(specialItemGuid);

            if (documentType.Division.Section == eDivision.Financial && documentType.SpecialItem == null)
            {
                string errorMessage = Resources.PleaseSelectAnItem;
                ModelState.AddModelError("SpecialItem", errorMessage);
                Session["Error"] = errorMessage;
            }

            if (ModelState.IsValid)
            {
                Session["Error"] = null;
                try
                {
                    AssignOwner(documentType);
                    UpdateLookupObjects(documentType);
                    if (documentType.ShouldResetMenu || documentType.StoreDocumentSeriesTypes.Count == 0)
                    {
                        Session["Menu"] = null;
                    }
                    documentType.Save();
                    XpoHelper.CommitTransaction(documentType.Session);
                    Session["currentDocumentType"] = null;
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }

            if (ModelState.IsValid == false || Session["Error"] != null)
            {
                FillLookupComboBoxes();
                return Json(new { error = Session["Error"] });
            }

            return Json(new { });
        }

        public JsonResult Cancel()
        {
            Session["currentDocumentType"] = null;
            return Json(new { success = (Session["Error"] == null || String.IsNullOrEmpty(Session["Error"].ToString())) });
        }

        public ActionResult MinistryDocumentTypeComboBox()
        {
            return PartialView();
        }

        public ActionResult TraderTypeCombobox()
        {
            FillLookupComboBoxes();
            return PartialView(Session["currentDocumentType"] as DocumentType);
        }

        public static object ItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                MinistryDocumentType obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<MinistryDocumentType>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetItemByValue(object value)
        {
            return GetObjectByValue<MinistryDocumentType>(value);
        }

        public static object ItemsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();

            CriteriaOperator crop = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                        new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Title"), e.Filter),
                                                        new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter)
                                                       //new BinaryOperator("Code", nameFilter, BinaryOperatorType.Like),
                                                       //new BinaryOperator("Title", nameFilter, BinaryOperatorType.Like),
                                                       //new BinaryOperator("Description", nameFilter, BinaryOperatorType.Like)
                                                       );

            XPCollection<MinistryDocumentType> collection = GetList<MinistryDocumentType>(uowLocal, crop, "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return collection;
        }

        public ActionResult CustomerCategoryGrid()
        {
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            return PartialView("CustomerCategoryGrid", docType.DocTypeCustomerCategories);
        }

        public ActionResult ItemCategoryGrid()
        {
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            return PartialView("ItemCategoryGrid", docType.DocumentTypeItemCategories);
        }

        [HttpPost]
        public ActionResult CustomerCategoryUpdatePartial([ModelBinder(typeof(RetailModelBinder))] DocTypeCustomerCategory ct)
        {
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SelectedNodeID = Guid.Empty;
                    if (Guid.TryParse(Request["SelectedNodeOid"], out SelectedNodeID))
                    {
                        IEnumerable<Guid> docCategoriesOids = docType.DocTypeCustomerCategories.SelectMany(doctypecustcat => doctypecustcat.CustomerCategory.GetNodeIDs());
                        if (!docCategoriesOids.Contains(SelectedNodeID))
                        {
                            DocTypeCustomerCategory docTypeCustCategory = docType.Session.GetObjectByKey<DocTypeCustomerCategory>(ct.Oid) ??
                                                                      new DocTypeCustomerCategory(docType.Session);
                            docTypeCustCategory.GetData(ct, new List<string>() { "Session" });
                            docTypeCustCategory.CustomerCategory = docTypeCustCategory.Session.GetObjectByKey<CustomerCategory>(SelectedNodeID);
                            foreach (DocTypeCustomerCategory docCat in docType.DocTypeCustomerCategories.ToList())
                            {
                                if (docTypeCustCategory.CustomerCategory.GetNodeIDs().Contains(docCat.CustomerCategory.Oid) && docTypeCustCategory.CustomerCategory.Oid != docCat.CustomerCategory.Oid)
                                {
                                    docType.DocTypeCustomerCategories.Remove(docCat);
                                    docType.Session.Delete(docCat);
                                }
                            }
                            docTypeCustCategory.DocumentType = docType;
                            docType.DocTypeCustomerCategories.Add(docTypeCustCategory);
                        }
                        else
                        {
                            Session["Error"] = Resources.DuplicateCustomerCategory;
                        }
                    }
                    else
                    {
                        Session["Error"] = Resources.AnErrorOccurred;
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }

            FillLookupComboBoxes();
            return PartialView("CustomerCategoryGrid", docType.DocTypeCustomerCategories);
        }

        [HttpPost]
        public ActionResult CustomerCategoryDeletePartial([ModelBinder(typeof(RetailModelBinder))] DocTypeCustomerCategory ct)
        {
            ViewData["EditMode"] = true;
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            try
            {
                docType.Session.Delete(docType.DocTypeCustomerCategories.Where(docTypeCustCategory => docTypeCustCategory.Oid == ct.Oid).FirstOrDefault());
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            FillLookupComboBoxes();
            return PartialView("CustomerCategoryGrid", docType.DocTypeCustomerCategories);
        }

        [HttpPost]
        public ActionResult ItemCategoryUpdatePartial([ModelBinder(typeof(RetailModelBinder))] DocumentTypeItemCategory ct)
        {
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            if (ModelState.IsValid)
            {
                try
                {
                    Guid SelectedNodeID = Guid.Empty;
                    if (Guid.TryParse(Request["SelectedNodeOid"], out SelectedNodeID))
                    {
                        IEnumerable<Guid> docItemCategories = docType.DocumentTypeItemCategories.SelectMany(doctypeitemcat => doctypeitemcat.ItemCategory.GetNodeIDs());
                        DocumentTypeItemCategory docTypeItemCategory = docType.Session.GetObjectByKey<DocumentTypeItemCategory>(ct.Oid) ??
                                                                      new DocumentTypeItemCategory(docType.Session);

                        if (!docItemCategories.Contains(SelectedNodeID))
                        {
                            docTypeItemCategory.GetData(ct, new List<string>() { "Session" });
                            docTypeItemCategory.ItemCategory = docTypeItemCategory.Session.GetObjectByKey<ItemCategory>(SelectedNodeID); ;
                            foreach (DocumentTypeItemCategory docItemCat in docType.DocumentTypeItemCategories.ToList())
                            {
                                if (docTypeItemCategory.ItemCategory.GetNodeIDs().Contains(docItemCat.ItemCategory.Oid) && docTypeItemCategory.ItemCategory.Oid != docItemCat.ItemCategory.Oid)
                                {
                                    docType.DocumentTypeItemCategories.Remove(docItemCat);
                                    docType.Session.Delete(docItemCat);
                                }
                            }
                            docTypeItemCategory.DocumentType = docType;
                            docType.DocumentTypeItemCategories.Add(docTypeItemCategory);
                        }
                        else
                        {
                            Session["Error"] = Resources.DuplicateItemCategory;
                        }
                    }
                    else
                    {
                        Session["Error"] = Resources.AnErrorOccurred;
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
            return PartialView("ItemCategoryGrid", docType.DocumentTypeItemCategories);
        }

        [HttpPost]
        public ActionResult ItemCategoryDeletePartial([ModelBinder(typeof(RetailModelBinder))] DocumentTypeItemCategory ct)
        {
            ViewData["EditMode"] = true;
            DocumentType docType = (DocumentType)Session["currentDocumentType"];
            try
            {
                docType.Session.Delete(docType.DocumentTypeItemCategories.Where(docTypeItemCategory => docTypeItemCategory.Oid == ct.Oid).FirstOrDefault());
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            FillLookupComboBoxes();
            return PartialView("ItemCategoryGrid", docType.DocumentTypeItemCategories);
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.DivisionComboBox = GetList<Division>(XpoSession);
            ViewBag.DocumentStatusComboBox = GetList<DocumentStatus>(XpoSession);
            ViewBag.PaymentMethodComboBox = GetList<PaymentMethod>(XpoSession);
            ViewBag.ReasonCategoryComboBox = GetList<ReasonCategory>(XpoSession);

            DocumentType documentType = Session["currentDocumentType"] as DocumentType;

            List<eDocumentTraderType> acceptableDocumentTraderTypes = (documentType != null && documentType.Division != null) ?
                                                        documentType.Division.Section.GetAvailableTraderTypes().ToEnumValuesEnumerable().ToList()
                                                       : new List<eDocumentTraderType>();

            Dictionary<eDocumentTraderType, string> availableTraderTypes = new Dictionary<eDocumentTraderType, string>();

            acceptableDocumentTraderTypes.ForEach(traderType =>
            {
                availableTraderTypes.Add(traderType, Enum<eDocumentTraderType>.ToLocalizedString(traderType));
            });

            ViewBag.AvailableTraderTypes = availableTraderTypes;
        }

        public ActionResult CustomerCategoryTreeView()
        {
            return PartialView("CustomerCategoryTreeView");
        }

        public ActionResult ItemCategoryTreeView()
        {
            return PartialView("ItemCategoryTreeView");
        }



        public ActionResult ActionTypeGrid()
        {
            DocumentType documentType = (DocumentType)Session["currentDocumentType"];
            ViewBag.DocStatuses = GetList<DocumentStatus>(documentType.Session);
            return PartialView(documentType == null ? null : documentType.ActionTypeEntities);
        }

        [HttpPost]
        public ActionResult UpdateActionTypeEntity([ModelBinder(typeof(RetailModelBinder))] ActionTypeEntityViewModel actionTypeEntityViewModel)
        {
            DocumentType documentType = (DocumentType)Session["currentDocumentType"];

            if (ModelState.IsValid)
            {
                ActionTypeEntity actionTypeEntity = documentType.ActionTypeEntities.FirstOrDefault(ate => ate.Oid == actionTypeEntityViewModel.Oid) ?? new ActionTypeEntity(documentType.Session);
                if (actionTypeEntity != null)
                {
                    actionTypeEntityViewModel.Persist(actionTypeEntity);
                    if (documentType.ActionTypeEntities.Select(actTypeEnt => actTypeEnt.ActionType).Contains(actionTypeEntity.ActionType))
                    {
                        Session["Error"] = Resources.DuplicateActionType;
                    }
                    else
                    {
                        actionTypeEntity.EntityOid = documentType.Oid;
                        actionTypeEntity.EntityCode = documentType.Code;
                        actionTypeEntity.EntityType = documentType.GetType().FullName;
                        actionTypeEntity.Owner = actionTypeEntity.ActionType.Owner;
                        actionTypeEntity.Store = actionTypeEntity.ActionType.Store;
                        actionTypeEntity.UpdateMode = actionTypeEntity.ActionType.UpdateMode;
                        List<string> statusesOids = Request["lstDocStatus"].Split('|').ToList();
                        statusesOids.Remove(statusesOids.First());
                        statusesOids = statusesOids.Select(oid => oid.Substring(0, Guid.Empty.ToString().Length)).ToList();
                        actionTypeEntity.Session.Delete(actionTypeEntity.ActionTypeDocStatuses);
                        foreach (string oid in statusesOids)
                        {
                            Guid guid;
                            if (Guid.TryParse(oid, out guid))
                            {
                                ActionTypeDocStatus actionTypeDocStatus = new ActionTypeDocStatus(actionTypeEntity.Session);
                                actionTypeDocStatus.DocumentStatus = documentType.Session.GetObjectByKey<DocumentStatus>(guid);
                                actionTypeDocStatus.DocStatusCode = actionTypeDocStatus.DocumentStatus.Code;
                                actionTypeDocStatus.ActionTypeEntity = actionTypeEntity;
                                actionTypeEntity.ActionTypeDocStatuses.Add(actionTypeDocStatus);
                            }
                        }
                    }
                }
            }
            return PartialView("ActionTypeGrid", documentType == null ? null : documentType.ActionTypeEntities);
        }

        [HttpPost]
        public ActionResult DeleteActionTypeEntity([ModelBinder(typeof(RetailModelBinder))] ActionTypeEntityViewModel actionTypeEntityViewModel)
        {
            DocumentType documentType = (DocumentType)Session["currentDocumentType"];

            if (documentType != null)
            {
                ActionTypeEntity actionTypeEntity = documentType.ActionTypeEntities.FirstOrDefault(ate => ate.Oid == actionTypeEntityViewModel.Oid);
                if (actionTypeEntity != null)
                {
                    actionTypeEntityViewModel.Persist(actionTypeEntity);
                    if (actionTypeEntity != null)
                    {
                        documentType.Session.Delete(actionTypeEntity);
                    }
                }
            }
            return PartialView("ActionTypeGrid", documentType == null ? null : documentType.ActionTypeEntities);
        }

        public ActionResult ActionTypeComboBox()
        {
            return PartialView();
        }

        public static object ActionTypeRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            DocumentType documentType = (DocumentType)System.Web.HttpContext.Current.Session["currentDocumentType"];

            CriteriaOperator applicationInstanceCriteria = MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                                                           ? new BinaryOperator("UpdateMode", eTotalizersUpdateMode.STORE)
                                                           : null;

            CriteriaOperator crop = CriteriaOperator.And(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                         //new BinaryOperator("Description", nameFilter, BinaryOperatorType.Like),
                                                         applicationInstanceCriteria
                                                        );

            XPCollection<ActionType> collection = GetList<ActionType>(documentType.Session, crop, "Description", PersistentCriteriaEvaluationBehavior.InTransaction);
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult BarcodeTypes()
        {
            DocumentType documentType = (DocumentType)Session["currentDocumentType"];
            return PartialView(documentType == null ? null : documentType.DocumentTypeBarcodeTypes);
        }

        public ActionResult BarcodeTypesComboBox()
        {
            return PartialView();
        }

        public static object BarcodeTypeRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            DocumentType documentType = (DocumentType)System.Web.HttpContext.Current.Session["currentDocumentType"];

            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("IsWeighed", true),
                                                          new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                          //new BinaryOperator("Description", nameFilter, BinaryOperatorType.Like),
                                                          new NotOperator(new ContainsOperator("DocumentTypeBarcodeTypes", new BinaryOperator("DocumentType", documentType.Oid)))
                                                        );

            XPCollection<BarcodeType> collection = new XPCollection<BarcodeType>(PersistentCriteriaEvaluationBehavior.InTransaction, documentType.Session, crop);
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            collection.Sorting.Add(new SortProperty("Description", DevExpress.Xpo.DB.SortingDirection.Ascending));
            return collection;
        }

        [HttpPost]
        public ActionResult AddBarcodeType(Guid? BarcodeType_VI)
        {
            DocumentType documentType = (DocumentType)Session["currentDocumentType"];
            if (BarcodeType_VI.HasValue
                && documentType.DocumentTypeBarcodeTypes.Where(docTypeBarType => docTypeBarType.BarcodeType.Oid == BarcodeType_VI).Count() == 0
               )
            {
                BarcodeType barcodeType = documentType.Session.GetObjectByKey<BarcodeType>(BarcodeType_VI);
                if (barcodeType != null)
                {
                    DocumentTypeBarcodeType documentTypeBarcodeType = new DocumentTypeBarcodeType(documentType.Session) { DocumentType = documentType, BarcodeType = barcodeType };
                    documentTypeBarcodeType.Save();
                }
            }
            return PartialView("BarcodeTypes", documentType == null ? null : documentType.DocumentTypeBarcodeTypes);
        }

        public JsonResult SetDefaultCategory()
        {
            try
            {
                Guid docTypeCustCategoryGuid = Guid.Empty;
                if (Guid.TryParse(Request["itemOid"], out docTypeCustCategoryGuid))
                {
                    DocumentType documentType = (DocumentType)Session["currentDocumentType"];
                    DocTypeCustomerCategory docTypeItemCategory = documentType.DocTypeCustomerCategories.FirstOrDefault(docTypecat => docTypecat.Oid == docTypeCustCategoryGuid);
                    if (docTypeItemCategory != null)
                    {
                        if (!docTypeItemCategory.DefaultCategoryForNewCustomer && documentType.DocTypeCustomerCategories.FirstOrDefault(docTypecat => docTypecat.DefaultCategoryForNewCustomer) != null)
                        {
                            Session["Error"] = Resources.DefaultAllreadyExists;
                            return Json(new { error = Resources.DefaultAllreadyExists });
                        }
                        else
                        {
                            docTypeItemCategory.DefaultCategoryForNewCustomer = !docTypeItemCategory.DefaultCategoryForNewCustomer;
                            docTypeItemCategory.Save();
                        }

                    }
                }

                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }
        }

        public ActionResult DocumentTypeSpecialItemComboBox()
        {
            return PartialView();
        }

        public static object SpecialItemsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string barcodeFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();
            XPCollection<SpecialItem> collection = GetList<SpecialItem>(uowLocal,
                                                                CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Code", String.Format("{0}", codefilter), BinaryOperatorType.Like)),
                                                                "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object SpecialItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                return XpoHelper.GetNewUnitOfWork().GetObjectByKey<SpecialItem>(e.Value);
            }
            return null;
        }

        public static object GetSpecialItemByValue(object value)
        {
            return GetObjectByValue<SpecialItem>(value);
        }
    }
}
