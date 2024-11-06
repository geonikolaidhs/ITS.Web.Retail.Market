using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using ITS.POS.Hardware.Common;
using ITS.Retail.Common;
using ITS.Retail.Common.Helpers;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.ViewModel;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace ITS.Retail.WebClient.Helpers
{
    public class DocumentHelper
    {
        public static int QUANTITY_MULTIPLIER
        {
            get
            {
                return 10000;
            }
        }

        public static bool fromDesktopClient { get; set; } = false;
        public static bool fromTransformation { get; set; } = false;
        public static bool recalculateLines { get; set; } = false;


        public static CriteriaOperator CustomerCriteria(string proccessed_filter, DocumentHeader docHeader, CompanyNew owner)
        {
            Guid ownerGuid = docHeader != null ? docHeader.Owner.Oid : owner.Oid;

            CriteriaOperator crop;
            crop = CriteriaOperator.And(new BinaryOperator("Owner.Oid", ownerGuid),
                                        new BinaryOperator("IsActive", true),
                                        CriteriaOperator.Or(// new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), proccessed_filter),
                                                            new BinaryOperator("CompanyName", proccessed_filter, BinaryOperatorType.Like),
                                                            //new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), proccessed_filter),
                                                            new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like),
                                                            //new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), proccessed_filter)));
                                                            new BinaryOperator("Trader.TaxCode", proccessed_filter, BinaryOperatorType.Like)));

            if (docHeader != null && docHeader.DocumentType != null)
            {
                crop = CriteriaOperator.And(crop, DocumentTypeSupportedCustomersCriteria(docHeader));
            }
            return crop;
        }

        public static bool IsDocumentReadyForSaving(DocumentHeader documentHeader, out string message, out int displayTab, out string displayTabTag)
        {
            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;
            int num_of_details = documentHeader.DocumentDetails.Count;
            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
            displayTab = -1;
            message = "";
            displayTabTag = "";
            if (documentHeader.Division == eDivision.Financial
                && (documentHeader.DocumentPayments == null
                    || documentHeader.DocumentPayments.Count <= 0
                   )
              )
            {
                message = Resources.PleaseAddPayments;
                displayTab = 1;
                displayTabTag = "PaymentMethods";
                return false;
            }

            if (documentHeader.Division != eDivision.Financial && num_of_details < 1)
            {
                message = Resources.PleaseAddItems;
                displayTab = 0;
                displayTabTag = "DocumentDetails";
                return false;
            }

            if (documentHeader.DocumentType.Division.Section != eDivision.Financial
                && documentHeader.DocumentType.ManualLinkedLineInsertion
                && documentHeader.DocumentDetails.Any(x => x.LinkedLine == Guid.Empty && x.LinkedLines.Count == 0))
            {
                message = Resources.FillAllMissingFields + ":" + Resources.LinkedItems;
                displayTab = 0;
                displayTabTag = "DocumentDetailsDouble";
                return false;
            }
            switch (documentHeader.Division)
            {
                case eDivision.Store:
                    if (documentHeader.SecondaryStore == null)
                    {
                        message = Resources.PleaseSelectAStore;
                        return false;
                    }
                    break;

                case eDivision.Sales:
                    if (documentHeader.Customer == null)
                    {
                        message = Resources.PleaseSelectACustomer;
                        return false;
                    }
                    else if (!DocTypeSupportsCustomer(documentHeader, documentHeader.Customer))
                    {
                        message = Resources.DocTypeNoSupportCustCat;
                        return false;
                    }

                    if (String.IsNullOrWhiteSpace(documentHeader.DeliveryAddress))
                    {
                        message = Resources.PleaseFillInDeliveryAddress + ".";
                        displayTab = 1;
                        displayTabTag = "Customer";
                        return false;
                    }
                    break;

                case eDivision.Purchase:
                    if (documentHeader.Supplier == null)
                    {
                        message = Resources.PleaseSelectASupplier;
                    }
                    break;

                default:
                    break;
            }

            if (documentHeader.DocumentType == null || documentHeader.DocumentSeries == null)
            {
                message = Resources.PleaseFillInDocumentHeaderData;
                return false;
            }
            if (documentHeader.DocumentSeries.HasAutomaticNumbering == false && documentHeader.Status.TakeSequence && documentHeader.DocumentNumber <= 0)
            {
                message = Resources.InvalidDocumentNumber;
            }
            if (documentHeader.HasBeenExecuted && (documentHeader.InvoicingDate == null || documentHeader.InvoicingDate == DateTime.MinValue))
            {
                message = Resources.PleaseFillInInvoiceDate;
                return false;
            }

            message = CreateDefaultDocumentPayments(documentHeader);
            if (message != String.Empty)
            {
                displayTab = 5;
                displayTabTag = "PaymentMethods";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Σαρώνει το συγκεκριμένο παραστατικό και επιστρέφει μία λίστα με τις υπάρχουσες ποσότητες κάθε είδους συνολικά στην παραγγελία
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <returns>List<SelectedItemsQty></returns>
        public static List<SelectedItemsQty> SelectedItemsTotalQuantity(DocumentHeader documentHeader)
        {
            var item_qtys = from itm in documentHeader.DocumentDetails
                            group itm by new { itm.Item.Oid } into gr
                            select new { item = gr.Key.Oid, Qty = .0, order_qty = gr.Sum(t => t.Qty) };

            var item_oids = from obj in documentHeader.DocumentDetails
                            select obj.Item.Oid;
            XPCollection<Item> items = new XPCollection<Item>(documentHeader.Session, new InOperator("Oid", item_oids.ToList()));
            IEnumerable<SelectedItemsQty> help = from it in item_qtys
                                                 join it2 in items on it.item equals it2.Oid
                                                 select new SelectedItemsQty(it2, .0m, it.order_qty);
            return help.ToList();
        }

        /// <summary>
        /// Ελέγχει εάν ο συγκεκριμένος User μπορεί να επεξεργαστεί μια παραγγελία με τα συγκεκριμένα στοιχεία
        /// </summary>
        /// <param name="document_number"></param>
        /// <param name="has_been_checked"></param>
        /// <param name="has_been_executed"></param>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool CanEdit(DocumentHeader documentHeader, User user)
        {
            if (UserHelper.IsSystemAdmin(user))
            {
                return true;
            }

            if (documentHeader.IsCanceled || documentHeader.IsCancelingAnotherDocument
              || (documentHeader.StoreDocumentSeriesType != null && documentHeader.DocumentSeries.eModule == eModule.POS)
              || documentHeader.POS != null
             || (documentHeader.HasBeenExecuted || (documentHeader.DerivedDocuments != null && documentHeader.DerivedDocuments.Count > 0))
             || (documentHeader.DocumentNumber > 0 && documentHeader.Status.TakeSequence
                    && UserHelper.IsSystemAdmin(user) == false)
             || (documentHeader.DocumentNumber > 0 && documentHeader.DocumentType.TakesDigitalSignature == true)
                )
            {
                return false;
            }
            return DocumentPermissionHelper.CanEditDocument(documentHeader.Oid.ToString());
        }

        public static bool CanCopy(DocumentHeader documentHeader, User user)
        {
            if (documentHeader.IsCanceled)
            {
                return false;
            }
            if (documentHeader.DocumentType.DocumentHeaderCanBeCopied == false)
            {
                return false;
            }
            if (user.Role.RoleEntityAccessPermisions.Count() == 0)
            {
                return true;
            }

            return documentHeader.StoreDocumentSeriesType == null || !(documentHeader.DocumentSeries.eModule == eModule.POS);
        }

        public static void RecalculateDocumentCosts(ref DocumentHeader documentHeader, bool recompute_document_lines = true, bool ignoreOwnerSettings = false)
        {
            switch (documentHeader.Division)
            {
                case eDivision.Purchase:
                    RecalculateDocumentCostsPurchase(ref documentHeader);
                    break;

                case eDivision.Sales:
                    RecalculateDocumentCostsSales(ref documentHeader, recompute_document_lines, ignoreOwnerSettings);
                    break;

                case eDivision.Store:
                    // RecalculateDocumentCostsSales(ref documentHeader, recompute_document_lines, ignoreOwnerSettings);
                    break;

                case eDivision.Financial:
                    RecalculateDocumentCostsFinancial(ref documentHeader, recompute_document_lines, ignoreOwnerSettings);
                    break;

                default:
                    throw new InvalidOperationException("Unreachable code exception - DocumentHelper.RecalculateDocumentCosts()");
            }
        }

        /// <summary>
        /// Επαναυπολογίζει τα κόστη του Παραστατικού Αγοράς.
        /// </summary>
        /// <param name="documentHeader"></param>
        private static void RecalculateDocumentCostsPurchase(ref DocumentHeader documentHeader)
        {
            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;
            documentHeader.GrossTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotal);
            documentHeader.NetTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotal);
            documentHeader.TotalVatAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmount);
            documentHeader.GrossTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDiscount);
            documentHeader.NetTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotalBeforeDiscount);
            documentHeader.TotalVatAmountBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmountBeforeDiscount);
            documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDocumentDiscount);
            documentHeader.TotalDiscountAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalDiscount);
            documentHeader.TotalQty = documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled).Sum(detail => detail.Qty);

            documentHeader.DocumentPoints = 0;
            documentHeader.TotalPoints = 0;
            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
        }

        /// <summary>
        /// Επαναυπολογίζει τα κόστη της παραγγελίας. Ανάλογα με την τιμή της recompute_document_lines
        /// απλώς προσθέτει τα κόστη των γραμμών ή επαναυπολογίζει και τις γραμμές
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="recompute_document_lines"></param>
        /// <param name="ignoreOwnerSettings"></param>
        private static void RecalculateDocumentCostsSales(ref DocumentHeader documentHeader, bool recompute_document_lines, bool ignoreOwnerSettings)
        {
            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;

            if (recompute_document_lines || ignoreOwnerSettings)
            {
                foreach (DocumentDetail documentDetail in documentHeader.DocumentDetails)
                {
                    DocumentDetail tempDocumentLine;
                    if (documentHeader.Owner.OwnerApplicationSettings.RecomputePrices || ignoreOwnerSettings)
                    {
                        tempDocumentLine = ComputeDocumentLine(ref documentHeader,
                            documentDetail.Item, documentDetail.Barcode, documentDetail.PackingQuantity, documentDetail.IsLinkedLine, -1,
                            documentDetail.HasCustomPrice, documentDetail.CustomDescription, documentDetail.DocumentDetailDiscounts, true, documentDetail.CustomMeasurementUnit, documentDetail);
                    }
                    else
                    {
                        tempDocumentLine = ComputeDocumentLine(ref documentHeader,
                            documentDetail.Item, documentDetail.Barcode, documentDetail.PackingQuantity, documentDetail.IsLinkedLine, documentDetail.CustomUnitPrice,
                            true, documentDetail.CustomDescription, documentDetail.DocumentDetailDiscounts, true, documentDetail.CustomMeasurementUnit,
                            documentDetail);
                    }
                }
            }

            documentHeader.GrossTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDiscount);
            documentHeader.GrossTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotal);
            documentHeader.NetTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotalBeforeDiscount);
            documentHeader.NetTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotal);
            documentHeader.TotalVatAmountBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmountBeforeDiscount);
            documentHeader.TotalVatAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmount);
            documentHeader.TotalDiscountAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalDiscount);
            documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDocumentDiscount);
            documentHeader.DocumentPoints = 0;

            if (documentHeader.Owner.OwnerApplicationSettings.SupportLoyalty &&
                documentHeader.Owner.OwnerApplicationSettings.LoyaltyOnDocumentSum &&
                documentHeader.Owner.OwnerApplicationSettings.DocumentSumForLoyalty > 0 &&
                documentHeader.DocumentType != null && documentHeader.DocumentType.SupportLoyalty &&
                documentHeader.PriceCatalogPolicy != null)
            {
                documentHeader.DocumentPoints = ((int)(documentHeader.GrossTotal - documentHeader.DocumentDetails.Where(x => x.DoesNotAllowDiscount == true || x.IsTax == true).Sum(z => z.FinalUnitPrice) / documentHeader.Owner.OwnerApplicationSettings.DocumentSumForLoyalty)) * documentHeader.Owner.OwnerApplicationSettings.LoyaltyPointsPerDocumentSum;
            }

            documentHeader.TotalPoints = documentHeader.DocumentPoints + documentHeader.PromotionPoints + documentHeader.SumarisableDocumentDetails.Sum(x => x.Points);

            documentHeader.TotalQty = documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled).Sum(detail => detail.Qty);

            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
        }

        public static DateTime ConvertToDateTime(string unformated_date)
        {
            if (unformated_date == "" || unformated_date == null)
            {
                return new DateTime(0);
            }

            DateTime dt = new DateTime();
            if (unformated_date.Contains("GMT"))
            {
                dt = Convert.ToDateTime(unformated_date.Substring(0, unformated_date.IndexOf("GMT")));
            }
            if (unformated_date.Contains("UTC"))
            {
                dt = Convert.ToDateTime(unformated_date.Substring(0, unformated_date.IndexOf("UTC") - 9) + unformated_date.Substring(unformated_date.IndexOf("UTC") + 9));
            }
            return dt;
        }

        private class ItemGroup
        {
            public Guid Oid { set; get; }
            public decimal FinalUnitPrice { get; set; }
            public decimal TotalDiscount { set; get; }
        }

        /// <summary>
        /// Αναλαμβάνει το ένωμα των γραμμών του παραστατικού
        /// ακόμη και για τα συνδεδεμένα είδη.
        /// </summary>
        /// <param name="documentHeader"></param>
        public static void MergeDocumentLines(ref DocumentHeader documentHeader)
        {
            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;

            var duplicates = from det in documentHeader.DocumentDetails
                             group det by new { det.Item.Oid, det.FinalUnitPrice, det.CustomDescription } into detGroup
                             where detGroup.Count() > 1
                             let itemGroup = detGroup.FirstOrDefault()
                             select new ItemGroup
                             {
                                 Oid = itemGroup.Item.Oid,
                                 FinalUnitPrice = itemGroup.FinalUnitPrice,
                                 TotalDiscount = itemGroup.TotalDiscount
                             };

            List<ItemGroup> itemGroups = duplicates.ToList();
            foreach (ItemGroup itemGroup in itemGroups)
            {
                List<DocumentDetail> duplicateDocumentDetails = documentHeader.DocumentDetails.Where(docdet => docdet.Item.Oid == itemGroup.Oid
                                                                                                                     && docdet.FinalUnitPrice == itemGroup.FinalUnitPrice).ToList();
                bool dublicateLinkedItems = false;

                foreach (DocumentDetail documentDetail in duplicateDocumentDetails)
                {
                    if (documentDetail.LinkedLine != Guid.Empty)
                    {
                        dublicateLinkedItems = true;
                        break;
                    }
                }

                if (!dublicateLinkedItems)
                {
                    decimal mergedQty = .0m;
                    Barcode theBarcode = duplicateDocumentDetails.Last().Barcode;
                    decimal unitPrice = duplicateDocumentDetails.Last().UnitPrice;
                    string customDescription = duplicateDocumentDetails.Last().CustomDescription;
                    foreach (DocumentDetail documentDetail in duplicateDocumentDetails)
                    {
                        mergedQty += documentDetail.Qty;
                        DeleteItem(ref documentHeader, documentDetail);
                        RecalculateDocumentCosts(ref documentHeader, false, false);
                    }
                    documentHeader.Session.Delete(duplicateDocumentDetails);//** Duplicate lines can only be deleted all together.
                    DocumentDetail mergedDocumentDetail;
                    if (documentHeader.Owner.OwnerApplicationSettings.RecomputePrices)
                    {
                        mergedDocumentDetail = ComputeDocumentLine(ref documentHeader, ItemHelper.GetItemOfSupplier(documentHeader.Session, theBarcode, documentHeader.Store.Owner), theBarcode, mergedQty, false, -1, false, customDescription, duplicateDocumentDetails.Last().DocumentDetailDiscounts);
                    }
                    else
                    {
                        mergedDocumentDetail = ComputeDocumentLine(ref documentHeader, ItemHelper.GetItemOfSupplier(documentHeader.Session, theBarcode, documentHeader.Store.Owner), theBarcode, mergedQty, false, unitPrice, true, customDescription, duplicateDocumentDetails.Last().DocumentDetailDiscounts);
                    }

                    AddItem(ref documentHeader, mergedDocumentDetail);
                }
            }
            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
        }

        /// <summary>
        /// Μηδενίζει τα σύνολα του παραστατικού όχι όμως και των γραμμών
        /// </summary>
        /// <remarks>
        /// Μπορεί να χρησιμοποιηθεί και εκτός αλλά προορίζεται για εσωτερική χρήση κυρίως
        /// </remarks>
        /// <param name="documentHeader"></param>
        public static void SetDocumentHeaderValuesToZero(ref DocumentHeader documentHeader)
        {
            documentHeader.GrossTotal =
            documentHeader.NetTotal =
            documentHeader.TotalDiscountAmount =
            documentHeader.TotalVatAmount =
            documentHeader.GrossTotalBeforeDiscount =
            documentHeader.NetTotalBeforeDiscount =
            documentHeader.TotalVatAmountBeforeDiscount =
            documentHeader.TotalQty =
            documentHeader.GrossTotalBeforeDocumentDiscount = .0M;
        }

        /// <summary>
        /// Διαγράφει τη συγκεκριμένη γραμμή (documentDetail) από το παραστατικό (documentHeader)
        /// μαζί με τα όποια συνδεδεμένα είδη.
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="documentDetail"></param>
        public static void DeleteItem(ref DocumentHeader documentHeader, DocumentDetail documentDetail)
        {
            DeleteLinkedItems(ref documentHeader, documentDetail);
            documentHeader.DocumentDetails.Remove(documentDetail);
            ApplyDiscountsOnDocumentTotal(ref documentHeader);
        }

        /// <summary>
        /// Διαγράφει τα συδεδεμένα είδη της γραμμής documentDetail.
        /// </summary>
        /// <remarks>
        /// Μπορεί να χρησιμοποιηθεί και εκτός αλλά σχεδιάστηκε κυρίως για εσωτερική χρήση.
        /// </remarks>
        /// <param name="documentHeader"></param>
        /// <param name="documentDetail"></param>
        public static void DeleteLinkedItems(ref DocumentHeader documentHeader, DocumentDetail documentDetail)
        {
            IEnumerable<DocumentDetail> linkedLines = documentHeader.DocumentDetails.Where(linkedDocDetail => linkedDocDetail.LinkedLine == documentDetail.Oid);
            documentHeader.Session.Delete(linkedLines.ToList());
        }

        /// <summary>
        /// Προσθέτει τα όποια συνδεδεμένα είδη πρέπει να προστεθούν για το είδος της documentDetail
        /// </summary>
        /// <remarks>
        /// Μπορεί να χρησιμοποιηθεί και εκτός αλλά σχεδιάστηκε κυρίως για εσωτερική χρήση.
        /// </remarks>
        /// <seealso cref="AddItem"/>
        /// <param name="documentHeader"></param>
        /// <param name="documentDetail"></param>
        private static void AddLinkedItems(ref DocumentHeader documentHeader, DocumentDetail documentDetail)
        {
            if (documentDetail.Item == null
              && documentDetail.SpecialItem != null
               )//There is a convention that we do not define linked items on special items!!!
            {
                return;
            }

            int offset = 1;
            EffectivePriceCatalogPolicy currentPriceCatalogPolicy = documentHeader.EffectivePriceCatalogPolicy;
            foreach (LinkedItem linked_item in documentDetail.Item.LinkedItems)
            {
                if (linked_item.QtyFactor <= 0)
                {
                    HttpContext.Current.Session["Error"] = string.Format(Resources.InvalidQuantityFactorForLinkedItemOnItem, linked_item.Item.Name, documentDetail.Item.Name);
                }

                PriceCatalogDetail tempPriceCatalogDetail = null;
                PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = null;

                if (linked_item.SubItem.DefaultBarcode != null)
                {
                    string search_code_str = linked_item.SubItem.DefaultBarcode.Code;
                    if (documentHeader.Owner.OwnerApplicationSettings.PadBarcodes)
                    {
                        search_code_str = search_code_str.PadLeft(documentHeader.Owner.OwnerApplicationSettings.BarcodeLength, documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                    }
                    priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork, currentPriceCatalogPolicy, search_code_str);
                    tempPriceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }

                if (tempPriceCatalogDetail == null && linked_item.SubItem.Code != null)
                {
                    string search_code_str = linked_item.SubItem.Code;
                    if (documentHeader.Owner.OwnerApplicationSettings.PadItemCodes)
                    {
                        search_code_str = search_code_str.PadLeft(documentHeader.Owner.OwnerApplicationSettings.ItemCodeLength, documentHeader.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                    }
                    priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork, currentPriceCatalogPolicy, search_code_str);
                    tempPriceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    if (tempPriceCatalogDetail == null)
                    {
                        search_code_str = linked_item.SubItem.Code;
                        if (documentHeader.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            search_code_str = search_code_str.PadLeft(documentHeader.Owner.OwnerApplicationSettings.BarcodeLength, documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }
                        priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork, currentPriceCatalogPolicy, search_code_str);
                        tempPriceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    }
                }

                if (tempPriceCatalogDetail == null || priceCatalogPolicyPriceResult == null || priceCatalogPolicyPriceResult.PriceCatalogDetail == null)
                {
                    #region Add LinkedItem with zero value

                    decimal pcDiscount = tempPriceCatalogDetail == null ? .0m : tempPriceCatalogDetail.Discount;
                    Barcode search_barcode = linked_item.SubItem.DefaultBarcode;
                    if (search_barcode == null)
                    {
                        string search_code_str = linked_item.SubItem.Code;
                        if (documentHeader.Owner.OwnerApplicationSettings.PadBarcodes)
                        {
                            search_code_str = search_code_str.PadLeft(documentHeader.Owner.OwnerApplicationSettings.BarcodeLength, documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                        }
                        search_barcode = documentHeader.Session.FindObject<Barcode>(new BinaryOperator("Code", search_code_str));
                    }

                    if (search_barcode == null)
                    {
                        DeleteItem(ref documentHeader, documentDetail);
                        HttpContext.Current.Session["Error"] = Resources.LinkedItemWithoutPriceHasBeenFound;
                        return;
                    }
                    DocumentDetail tempDocumentDetail = ComputeDocumentLine(
                        ref documentHeader, linked_item.SubItem, search_barcode, documentDetail.Qty * (decimal)linked_item.QtyFactor, true, -1, false, "", null);
                    tempDocumentDetail.LinkedLine = documentDetail.Oid;

                    if (tempDocumentDetail.UnitPrice < 0)
                    {
                        tempDocumentDetail.UnitPrice = 0;
                    }
                    tempDocumentDetail.LineNumber = documentDetail.LineNumber + offset;
                    offset++;
                    documentHeader.DocumentDetails.Add(tempDocumentDetail);
                    ApplyDiscountsOnDocumentTotal(ref documentHeader);
                    return;

                    #endregion Add LinkedItem with zero value
                }
                DocumentDetail tempDocumentDetail2 = ComputeDocumentLine(
                        ref documentHeader, linked_item.SubItem, priceCatalogPolicyPriceResult.PriceCatalogDetail.Barcode, documentDetail.Qty * (decimal)linked_item.QtyFactor, true, -1, false, "", null);
                tempDocumentDetail2.LinkedLine = documentDetail.Oid;
                if (tempDocumentDetail2.UnitPrice < 0)
                {
                    tempDocumentDetail2.UnitPrice = 0;
                }

                tempDocumentDetail2.LineNumber = documentDetail.LineNumber + offset;
                offset++;
                documentHeader.DocumentDetails.Add(tempDocumentDetail2);
                ApplyDiscountsOnDocumentTotal(ref documentHeader);
            }
        }

        /// <summary>
        /// Returns the Sorted Order for a new DocumentDetail of a DocumentHeader
        /// </summary>
        /// <param name="documentHeader">The DocumentHeader</param>
        /// <returns>1 if the DocumentDetail is the first to be inserted else the next sorted order</returns>
        public static int GetDocumentNextDocumentDetailSortOrder(DocumentHeader documentHeader)
        {
            return documentHeader.DocumentDetails.Count == 0 ? 1 : documentHeader.DocumentDetails.Max(documentDetail => documentDetail.LineNumber) + 1;
        }

        /// <summary>
        /// Προσθέτει το είδος στην παραγγελία μαζί με τα όποια συνδεδέμενα πρέπει να προστεθούν.
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="documentDetail"></param>
        public static void AddItem(ref DocumentHeader documentHeader, DocumentDetail documentDetail, bool add_linked_lines = true, bool applyDerivedDocumentTotalDiscounts = true) //, bool add_linked_lines = true, bool recalculateHeader = true)
        {
            if (documentDetail.LineNumber == 0)
            {
                documentDetail.LineNumber = GetDocumentNextDocumentDetailSortOrder(documentHeader);
            }
            documentHeader.DocumentDetails.Add(documentDetail);
            if (applyDerivedDocumentTotalDiscounts == true)
            {
                ApplyDiscountsOnDocumentTotal(ref documentHeader);
            }
            if (documentHeader.DocumentType.ManualLinkedLineInsertion == false && add_linked_lines == true)
            {
                AddLinkedItems(ref documentHeader, documentDetail);

            }
            if (fromTransformation == true && recalculateLines == true)
            {
                RecalculateDocumentCosts(ref documentHeader, true);
            }
            else
            {
                RecalculateDocumentCosts(ref documentHeader, false);
            }


        }

        /// <summary>
        /// Αντικαθιστά στο παραστιτικό τη γραμμή old_value με τη γραμμή new_value.
        /// Ενημερώνει και τα συνδεδεμένα είδη.
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="old_value"></param>
        /// <param name="new_value"></param>
        public static void ReplaceItem(ref DocumentHeader documentHeader, DocumentDetail old_value, DocumentDetail new_value)
        {
            new_value.LineNumber = old_value.LineNumber;
            TransferRelativeDocumentDetails(old_value, new_value);
            DeleteItem(ref documentHeader, old_value);
            AddItem(ref documentHeader, new_value);
        }

        /// <summary>
        /// It refers to the case where the old DocumentDetail is the result of Document Transformation.
        /// Therefore the connection that is the Relative Document Details MUST be transfered
        /// </summary>
        /// <param name="oldDocumentDetail"></param>
        /// <param name="newDocumentDetail"></param>
        private static void TransferRelativeDocumentDetails(DocumentDetail oldDocumentDetail, DocumentDetail newDocumentDetail)
        {
            foreach (RelativeDocumentDetail relativeDocumentDetail in oldDocumentDetail.ReferencedRelativeDocumentDetails.ToList())
            {
                relativeDocumentDetail.DerivedDocumentDetail = newDocumentDetail;
            }
        }

        /// <summary>
        /// Ελέγχει εάν ο χρήστης user μπορεί να επεργαστεί ή όχι τη συγκεκριμένη γραμμή του παραστατικού.
        /// </summary>
        /// <param name="documentDetail"></param>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool DocumentDetailCanBeEdited(DocumentDetail documentDetail, User user)
        {
            if (documentDetail.IsLinkedLine == false
                && (documentDetail.DerivedRelativeDocumentDetails == null || documentDetail.DerivedRelativeDocumentDetails.Count == 0)
                )
            {
                return true;
            }
            return false;
        }

        public static DocumentHeader CreateDerivativeDocument(List<DocumentDetailAssociation> documentDetailAssociations, User user, DocumentType transformToDocumentType, DocumentSeries transformToDocumentSeries, List<DocumentDetail> linkedlines)
        {
            if (transformToDocumentType == null)
            {
                throw new Exception("No Document type has been selected");
            }
            if (transformToDocumentSeries == null)
            {
                throw new Exception("No Document series has been selected");
            }

            if (documentDetailAssociations == null || documentDetailAssociations.Where(docDetAssoc => docDetAssoc.IsSelected && docDetAssoc.RetrievedQuantity > 0).Count() <= 0)
            {
                throw new Exception("No data have been selected");
            }

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            DocumentHeader newHeader = new DocumentHeader(uow);
            newHeader.DocumentSeries = uow.GetObjectByKey<DocumentSeries>(transformToDocumentSeries.Oid);
            newHeader.DocumentType = uow.GetObjectByKey<DocumentType>(transformToDocumentType.Oid);
            newHeader.CreatedBy = uow.GetObjectByKey<User>(user.Oid);

            List<DocumentDetailAssociation> linkedLinesAssociation = new List<DocumentDetailAssociation>();
            foreach (DocumentDetail entry in linkedlines)
            {
                DocumentDetailAssociation detailAssociation = new DocumentDetailAssociation(entry, entry.UnreferencedQuantity);
                linkedLinesAssociation.Add(detailAssociation);
            }

            List<DocumentDetailAssociation> validDocumentDetailAssociations = documentDetailAssociations.Where(docDetAssoc => docDetAssoc.IsSelected && docDetAssoc.RetrievedQuantity > 0 && DocumentTypeSupportsItem(newHeader, docDetAssoc.documentDetail.Item)).ToList();
            List<DocumentHeader> initialHeaders = validDocumentDetailAssociations.Select(docDetAssoc => docDetAssoc.documentDetail.DocumentHeader).Distinct().ToList();
            if (initialHeaders.Count == 0)
            {
                throw new Exception(Resources.NoItemsSupportedByDocType);
            }

            if (newHeader.DocumentType.ReserveCoupons)
            {
                IEnumerable<string> uniqueCouponCodes = initialHeaders.SelectMany(header => header.TransactionCoupons
                                                                                                    .Where(transCoupon => (transCoupon.Coupon != null && transCoupon.Coupon.IsUnique)
                                                                                                           || (transCoupon.CouponMask != null && transCoupon.CouponMask.IsUnique)
                                                                                                          )
                                                                                 )
                                                                       .Select(transCoupon => transCoupon.CouponCode);

                if (uniqueCouponCodes.Count() > 0)
                {
                    List<string> uniqueCouponCodesThatHaveBeenUsed = new List<string>();

                    foreach (string couponCode in uniqueCouponCodes)
                    {
                        Coupon coupon = newHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", couponCode));
                        if (coupon.NumberOfTimesUsed >= 1)
                        {
                            uniqueCouponCodesThatHaveBeenUsed.Add(couponCode);
                        }
                    }

                    if (uniqueCouponCodesThatHaveBeenUsed.Count() > 0)
                    {
                        throw new Exception(string.Format(Resources.ThereAreUniqueCouponsThatHaveAlreadyBeenUsed, string.Join(",", uniqueCouponCodesThatHaveBeenUsed.ToArray())));
                    }
                }
            }

            DocumentHeader initialHeader = uow.GetObjectByKey<DocumentHeader>(initialHeaders.First().Oid);
            Store store = newHeader.Session.GetObjectByKey<Store>(initialHeader.Store.Oid);
            if (store == null)
            {
                HttpContext.Current.Session["Error"] = Resources.AnErrorOccurred;
                throw new Exception(Resources.AnErrorOccurred);
            }
            newHeader.Store = store;
            newHeader.Customer = newHeader.Session.GetObjectByKey<Customer>(initialHeader.Customer == null ? Guid.Empty : initialHeader.Customer.Oid);
            newHeader.Supplier = newHeader.Session.GetObjectByKey<SupplierNew>(initialHeader.Supplier == null ? Guid.Empty : initialHeader.Supplier.Oid);
            newHeader.PriceCatalogPolicy = initialHeader.PriceCatalogPolicy == null ? null : newHeader.Session.GetObjectByKey<PriceCatalogPolicy>(initialHeader.PriceCatalogPolicy.Oid);
            newHeader.DeliveryAddress = initialHeaders.Select(g => g.DeliveryAddress).Distinct().Aggregate((f, s) => f + Environment.NewLine + s);
            newHeader.FinalizedDate = DateTime.Now;
            IEnumerable<Address> billingAddresses = initialHeaders.Where(doc => doc.BillingAddress != null).Select(doc => doc.BillingAddress);
            if (billingAddresses.Count() > 0)
            {
                Address billingAddress = billingAddresses.First();
                newHeader.BillingAddress = newHeader.Session.GetObjectByKey<Address>(billingAddress.Oid);
            }

            newHeader.Status = newHeader.DocumentType.DefaultDocumentStatus;
            if (newHeader.Status == null)
            {
                DocumentStatus docStatus = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
                if (docStatus == null)
                {
                    XPCollection<DocumentStatus> docStatuses = new XPCollection<DocumentStatus>(uow);
                    if (docStatuses.Count > 0)
                    {
                        docStatus = docStatuses.First();
                    }
                }

                if (docStatus != null)
                {
                    newHeader.Status = docStatus;
                }
            }

            newHeader.TransferMethod = initialHeader.TransferMethod;
            newHeader.TransferPurpose = initialHeader.TransferPurpose;

            TransformationRule transformationRule = initialHeader.DocumentType.TransformsTo.FirstOrDefault(transrule => transrule.DerrivedType.Oid == transformToDocumentType.Oid);
            if (transformationRule == null)
            {
                throw new Exception(Resources.DocumentCannotBeTranformed);
            }

            newHeader.TransformationLevel = transformationRule.TransformationLevel;

            if (initialHeader.DeliveryTo != null)
            {
                newHeader.DeliveryTo = initialHeader.DeliveryTo;
            }
            if (initialHeader.DeliveryType != null)
            {
                newHeader.DeliveryType = initialHeader.DeliveryType;
            }
            newHeader.Division = newHeader.DocumentType.Division == null ? initialHeader.Division : newHeader.DocumentType.Division.Section;

            IEnumerable<DocumentHeader> initialHeadersWhithRemarks = initialHeaders.Where(g => string.IsNullOrEmpty(g.Remarks) == false && g.Remarks.ToLower() != "null");
            if (initialHeadersWhithRemarks.Count() > 0)
            {
                newHeader.Remarks = initialHeadersWhithRemarks.Select(g => g.Remarks).Aggregate((f, s) => f + Environment.NewLine + s);
            }

            List<string> documentPaymentIgnoreCopyFields = new List<string>() { "Oid", "DocumentHeader" };


            foreach (DocumentHeader initHeader in initialHeaders)
            {
                RelativeDocument relativeDocument = new RelativeDocument(uow);
                relativeDocument.InitialDocument = initialHeader;
                relativeDocument.DerivedDocument = newHeader;

                //CopyDocumentPayments
                foreach (DocumentPayment documentPayment in initHeader.DocumentPayments)
                {
                    DocumentPayment copyDocumentPayment = new DocumentPayment(newHeader.Session);
                    copyDocumentPayment.GetData(documentPayment, documentPaymentIgnoreCopyFields);
                    newHeader.DocumentPayments.Add(copyDocumentPayment);
                }

                if (newHeader.TransformationLevel != eTransformationLevel.DEFAULT)
                {
                    validDocumentDetailAssociations.AddRange(linkedLinesAssociation);

                }
                else
                {
                    newHeader.DefaultDocumentDiscount = newHeader.StoreDocumentSeriesType.DefaultDiscountPercentage * 100;
                    if (newHeader.Customer != null)
                    {
                        newHeader.DefaultCustomerDiscount = (decimal)newHeader.Customer.Discount * 100;
                    }
                }

                foreach (DocumentDetailAssociation documentDetailAssociation in validDocumentDetailAssociations.Where(docDetAssoc => docDetAssoc.documentDetail.DocumentHeader.Oid == initHeader.Oid))
                {
                    DocumentDetail documentDetail = uow.GetObjectByKey<DocumentDetail>(documentDetailAssociation.documentDetail.Oid);
                    Item item = uow.GetObjectByKey<Item>(documentDetailAssociation.documentDetail.Item.Oid);
                    Barcode barcode = uow.GetObjectByKey<Barcode>(documentDetailAssociation.documentDetail.Barcode.Oid);
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(initHeader.Store, barcode.Code, initHeader.Customer);
                    PriceCatalogDetail pricecatdet = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    DocumentDetail derivedDocumentDetail;

                    if (!newHeader.DocumentType.AcceptsGeneralItems && item.IsGeneralItem)
                    {
                        throw new Exception(Resources.DocTypeNotUsesGeneralItems);
                    }

                    if (newHeader.TransformationLevel == eTransformationLevel.DEFAULT)
                    {
                        fromTransformation = true;
                        recalculateLines = true;
                        decimal price = pricecatdet != null ?
                            (newHeader.DocumentType.IsForWholesale ? pricecatdet.WholesaleValue : pricecatdet.RetailValue) :
                            (newHeader.DocumentType.IsForWholesale ?
                            (documentDetail.NetTotalBeforeDiscount / documentDetail.Qty) :
                            documentDetail.FinalUnitPriceWithVat);

                        List<DocumentDetailDiscount> documentDetailDiscounts = new List<DocumentDetailDiscount>();
                        foreach (DocumentDetailDiscount documentDetailDiscount in documentDetail.DocumentDetailDiscounts)
                        {
                            DocumentDetailDiscount discount = new DocumentDetailDiscount(newHeader.Session);
                            discount.GetData(documentDetailDiscount, new List<string>() { "Oid", "DocumentDetail" });
                            if (documentDetail.IsTax == false && documentDetail.DoesNotAllowDiscount == false)
                            {
                                documentDetailDiscounts.Add(discount);
                            }

                        }

                        derivedDocumentDetail = ComputeDocumentLine(ref newHeader, item,
                                                                            barcode, documentDetailAssociation.RetrievedQuantity,
                                                                            (documentDetail.LinkedLine != Guid.Empty),
                                                                            price,
                                                                            true,
                                                                            documentDetail.CustomDescription,
                                                                            documentDetailDiscounts,
                                                                            documentDetailAssociation: documentDetailAssociation,
                                                                            UseDetailAssociation: true
                                                                        );
                        AddItem(ref newHeader, derivedDocumentDetail);
                    }
                    else
                    {
                        if (!newHeader.DocumentType.AllowItemZeroPrices && documentDetail.CustomUnitPrice <= 0)
                        {
                            throw new Exception(Resources.ItemsWithZeroPricesAreNotAllowed);
                        }
                        derivedDocumentDetail = new DocumentDetail(newHeader.Session);
                        derivedDocumentDetail.GetData(documentDetail, new List<string>() { "Oid", "DocumentHeader", "LineNumber", "Points" });

                        foreach (DocumentDetailDiscount documentDetailDiscount in documentDetail.DocumentDetailDiscounts)
                        {
                            if (documentDetail.IsTax == false && documentDetail.DoesNotAllowDiscount == false)
                            {
                                DocumentDetailDiscount discount = new DocumentDetailDiscount(newHeader.Session);
                                discount.GetData(documentDetailDiscount, new List<string>() { "Oid", "DocumentDetail" });
                                derivedDocumentDetail.DocumentDetailDiscounts.Add(discount);
                            }
                        }
                        AddItem(ref newHeader, derivedDocumentDetail, false, false);
                    }

                    RelativeDocumentDetail relativeDocumentDetail = new RelativeDocumentDetail(uow);

                    relativeDocumentDetail.Qty = documentDetailAssociation.RetrievedQuantity;
                    relativeDocumentDetail.InitialDocumentDetail = uow.GetObjectByKey<DocumentDetail>(documentDetailAssociation.documentDetail.Oid);
                    relativeDocumentDetail.DerivedDocumentDetail = derivedDocumentDetail;
                    relativeDocument.RelativeDocumentDetails.Add(relativeDocumentDetail);
                }
                if (newHeader.TransformationLevel == eTransformationLevel.FREEZE_EDIT || newHeader.TransformationLevel == eTransformationLevel.FREEZE_VALUES)
                {
                    RecalculateDocumentCosts(ref newHeader, false, false);
                }

                if (newHeader.DocumentDetails.Count() == initHeader.DocumentDetails.Count)
                {
                    FixSmallPaymentDeviations(ref newHeader);
                }
                relativeDocument.Save();
            }

            //Add default DocumentPayment if initialDocumentHeaders have no Payment.
            if (newHeader.DocumentPayments.Count == 0 && newHeader.DocumentType.DefaultPaymentMethod != null)
            {
                DocumentPayment defaultDocumentPayment = new DocumentPayment(newHeader.Session);
                defaultDocumentPayment.PaymentMethod = newHeader.DocumentType.DefaultPaymentMethod;
                defaultDocumentPayment.Amount = newHeader.GrossTotal;
                newHeader.DocumentPayments.Add(defaultDocumentPayment);
            }

            return newHeader;
        }

        public static Dictionary<Guid, List<DocumentDetailDiscount>> GetDiscountsFromLinkedItems(XPCollection<DocumentHeader> documents)
        {
            Dictionary<Guid, List<DocumentDetailDiscount>> dictionary = new Dictionary<Guid, List<DocumentDetailDiscount>>();
            foreach (DocumentHeader document in documents)
            {
                foreach (DocumentDetail detail in document.DocumentDetails)
                {
                    if (detail.LinkedLine != null && detail.LinkedLine != Guid.Empty)
                    {
                        List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
                        foreach (DocumentDetailDiscount disc in detail.DocumentDetailDiscounts)
                        {
                            discounts.Add(disc);
                        }
                        dictionary.Add(detail.ItemOid, discounts);
                    }
                }
            }
            return dictionary;
        }
        public static List<DocumentDetail> GetLinkedLines(XPCollection<DocumentHeader> documents)
        {
            List<DocumentDetail> list = new List<DocumentDetail>();
            foreach (DocumentHeader document in documents)
            {
                foreach (DocumentDetail detail in document.DocumentDetails)
                {
                    if (detail.LinkedLine != null && detail.LinkedLine != Guid.Empty)
                    {
                        list.Add(detail);
                    }
                }
            }
            return list;
        }

        private static void FixSmallPaymentDeviations(ref DocumentHeader header)
        {
            decimal PaymentDeviations = Math.Abs(header.DocumentPayments.Sum(x => x.Amount) - header.GrossTotal);
            if (PaymentDeviations != 0 && PaymentDeviations < (decimal)0.01)
            {
                if (header.DocumentPayments.Sum(x => x.Amount) > header.GrossTotal)
                {
                    header.DocumentPayments.FirstOrDefault().Amount -= PaymentDeviations;
                }
                else
                {
                    header.DocumentPayments.FirstOrDefault().Amount += PaymentDeviations;
                }
            }
        }

        public static string DocumentCanBeTransformed(DocumentHeader documentHeader)
        {
            if (documentHeader.PriceCatalogPolicy == null && documentHeader.Division == eDivision.Sales)
            {
                return Resources.DocumentCannotBeTranformed;
            }
            if (documentHeader.IsCanceled)
            {
                return Resources.DocumentAlreadyCanceled;
            }
            if (documentHeader.UnreferencedDetails == null || documentHeader.UnreferencedDetails.FirstOrDefault() == null)
            {
                return Resources.DocumentAlreadyTransformed;
            }
            else if (documentHeader.DocumentType.TransformsTo == null ||
                      documentHeader.DocumentType.TransformsTo.Where(ttype => ttype.DerrivedType.Division.Section == documentHeader.Division).Count() <= 0)
            {
                return String.Format(Resources.DocTypeHasNoTransformRules, documentHeader.DocumentType.Description);
            }
            return ""; // success
        }

        public static string DocumentsCanBeTransformed(List<DocumentHeader> documentHeaders, bool isFast, eModule module)
        {
            IEnumerable<EffectivePriceCatalogPolicy> documentHeadersPriceCatalogPolicies = documentHeaders.Select(documentHeader => documentHeader.EffectivePriceCatalogPolicy).Distinct();
            IEnumerable<eTransformationLevel> documentHeaderstransformationLevels = documentHeaders.Select(documentHeader => documentHeader.TransformationLevel).Distinct();
            IEnumerable<Address> documentHeadersBillingAddressess = documentHeaders.Select(documentHeader => documentHeader.BillingAddress).Distinct();

            if (documentHeaders.Select(docs => docs.Store).Distinct().Count() > 1)
            {
                return Resources.DocumentsMustHaveSameStore;
            }

            if ((documentHeadersPriceCatalogPolicies.Count() != 1
                || (documentHeadersPriceCatalogPolicies.Count() == 1 && documentHeadersPriceCatalogPolicies.FirstOrDefault() == null))
               || (documentHeaderstransformationLevels.Count() != 1)
               || (documentHeadersBillingAddressess.Count() != 1)
                )
            {
                return Resources.DocumentsCannotBeTranformed;
            }
            foreach (DocumentHeader docHeader in documentHeaders)
            {
                string result = DocumentCanBeTransformed(docHeader);
                if (String.IsNullOrWhiteSpace(result) == false)
                {
                    return result;
                }
            }

            if (documentHeaders.Select(docHeader => docHeader.Customer).Distinct().Count() != 1
                || documentHeaders.Select(docHeader => docHeader.Supplier).Distinct().Count() != 1
                || documentHeaders.Select(docHeader => docHeader.SecondaryStore).Distinct().Count() != 1
                || documentHeaders.Select(docHeader => docHeader.Division).Distinct().Count() != 1
              )
            {
                return Resources.DocumentsToTransformInvalid;
            }

            eDivision section = documentHeaders.Select(docHeader => docHeader.Division).Distinct().FirstOrDefault();

            List<DocumentType> selectedDocumentsDocumentTypes = documentHeaders.Select(docHead => docHead.DocumentType).Distinct().ToList();

            string message;
            List<TransformationRule> allowedTransformationRules = AllowedTransformationRules(selectedDocumentsDocumentTypes,
                 documentHeaders.First().Store, module, section, out message);
            if (allowedTransformationRules == null || allowedTransformationRules.Count() == 0)
            {
                return message;
            }

            if (isFast)
            {
                IEnumerable<Guid> proformaDocumentTypeGuids = (new XPCollection<Model.POS>(documentHeaders.First().Session)).Select(pos => pos.ProFormaInvoiceDocumentType.Oid).Distinct<Guid>();

                if (proformaDocumentTypeGuids.Count() > 0)
                {
                    if (allowedTransformationRules.Count() > 0
                            &&
                        documentHeaders.Where(header => proformaDocumentTypeGuids.Contains(header.DocumentType.Oid)).Count() > 0
                        )
                    {
                        TransformationRule defaultRule = allowedTransformationRules.FirstOrDefault(x => x.IsDefault);
                        if (defaultRule == null && allowedTransformationRules.Count() > 1)
                        {
                            return string.Format(Resources.NoDefaultTransformationHasBeenSelected);
                        }
                        if (defaultRule == null)
                        {
                            defaultRule = allowedTransformationRules.FirstOrDefault();
                        }

                        DocumentType ruleDocumentType = defaultRule.DerrivedType;
                        IEnumerable<DocumentSeries> allowedDocumentSeries = StoreHelper.StoreSeriesForDocumentType(documentHeaders.First().Store, ruleDocumentType, module);
                        int allowedDocumentSeriesCount = allowedDocumentSeries.Count();
                        if (allowedDocumentSeriesCount != 1)
                        {
                            return string.Format(Resources.DocumentTypeHasMoreThanOneDocumentSeries, ruleDocumentType.Description, allowedDocumentSeriesCount);
                        }
                    }
                }
            }

            return ""; //success
        }

        public static List<TransformationRule> AllowedTransformationRules(List<DocumentType> selectedDocumentsDocumentTypes, Store currentstore, eModule module, eDivision section, out string errormessage)
        {
            List<TransformationRule> FirstTransformationRules = selectedDocumentsDocumentTypes.First().TransformsTo.
                Where(transrule => transrule.DerrivedType.Division.Section == section &&
                StoreHelper.StoreHasSeriesForDocumentType(currentstore, transrule.DerrivedType, module)).ToList();
            List<TransformationRule> allowedTransformationRules = new List<TransformationRule>();
            if (FirstTransformationRules.Count() == 0)
            {
                errormessage = Resources.NoSeriesForDerivedDocumentTypes;
                return FirstTransformationRules;
            }

            selectedDocumentsDocumentTypes.RemoveAt(0);

            if (selectedDocumentsDocumentTypes.Count > 0)
            {
                foreach (DocumentType dt in selectedDocumentsDocumentTypes)
                {
                    IEnumerable<TransformationRule> newTransRules = dt.TransformsTo;
                    List<TransformationRule> nonCommonTransRules = new List<TransformationRule>();
                    foreach (TransformationRule allowedtransrule in FirstTransformationRules)
                    {
                        foreach (TransformationRule newtransrule in newTransRules)
                        {
                            if ((allowedtransrule.DerrivedType == newtransrule.DerrivedType) && (allowedtransrule.TransformationLevel == newtransrule.TransformationLevel))
                            {
                                if (!allowedTransformationRules.Contains(allowedtransrule))
                                {
                                    allowedTransformationRules.Add(allowedtransrule);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                allowedTransformationRules = FirstTransformationRules;
            }

            errormessage = "";
            if (allowedTransformationRules.Count <= 0)
            {
                errormessage = Resources.NoCommonTransformationType;
            }
            return allowedTransformationRules;
        }

        /// <summary>
        /// It returns a replica of the Document. BE AWARE of when you Dispose the "uow"!
        /// </summary>
        /// <param name="docToCopyGuid">Guid of document to copy</param>
        /// <param name="uow">The session in which it should be retrieved</param>
        /// <returns></returns>
        public static object CopyDocument(Guid docToCopyGuid, UnitOfWork uow)
        {
            DocumentHeader originalDocument = uow.GetObjectByKey<DocumentHeader>(docToCopyGuid);

            if (originalDocument == null)
            {
                return null;
            }

            DocumentStatus status = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
            if (status == null)
            {
                status = new XPCollection<DocumentStatus>(uow).FirstOrDefault();
            }

            DocumentHeader copy = new DocumentHeader(uow);
            copy.GetData(originalDocument, new List<string>()
                                          {
                                              "Oid",
                                              "GrossTotal",
                                              "GrossTotalBeforeDiscount",
                                              "NetTotal",
                                              "NetTotalBeforeDiscount",
                                              "TotalDiscountAmount",
                                              "TotalQty",
                                              "TotalVatAmount",
                                              "TotalVatAmountBeforeDiscount",
                                              "VatAmount1",
                                              "VatAmount2",
                                              "VatAmount3",
                                              "VatAmount4",
                                              "Signature",
                                              "CreatedOnTicks",
                                              "UpdatedOnTicks",
                                              "CustomerBalanceComputedAtStoreController",
                                              "CustomerBalanceComputedAtHeadquarters"
                                          }
                        );

            copy.DocumentNumber = 0;
            copy.FinalizedDate = DateTime.Now;
            copy.InvoicingDate = originalDocument.InvoicingDate;

            if (originalDocument.DocumentPayments != null && originalDocument.DocumentPayments.Count > 0)
            {
                foreach (DocumentPayment docPayment in originalDocument.DocumentPayments)
                {
                    DocumentPayment docPay = new DocumentPayment(uow);

                    docPay.GetData(docPayment, new List<string>() { "Oid", "DocumentHeader" });
                    copy.DocumentPayments.Add(docPay);

                }
            }

            Dictionary<Guid, Guid> trackOriginalLinesCopy = new Dictionary<Guid, Guid>();

            List<DocumentDetail> lines = originalDocument.DocumentDetails.AsQueryable().ToList();
            //Main Lines
            foreach (DocumentDetail sourceDocumentDetail in lines)
            {
                DocumentDetail targetDocumentDetail = new DocumentDetail(uow);
                string json = sourceDocumentDetail.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                JObject jsonItem = JObject.Parse(json);
                string error;
                targetDocumentDetail.FromJson(jsonItem, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                targetDocumentDetail.Oid = Guid.NewGuid();
                targetDocumentDetail.DocumentHeader = copy;
                targetDocumentDetail.DocumentHeaderOid = copy.Oid;
                copy.DocumentDetails.Add(targetDocumentDetail);
                trackOriginalLinesCopy.Add(sourceDocumentDetail.Oid, targetDocumentDetail.Oid);

                foreach (DocumentDetailDiscount detaildiscount in sourceDocumentDetail.DocumentDetailDiscounts)
                {
                    DocumentDetailDiscount detailDiscount = new DocumentDetailDiscount(uow);
                    string jsonDiscount = sourceDocumentDetail.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                    JObject jDiscount = JObject.Parse(jsonDiscount);
                    string errorDiscount;
                    detailDiscount.FromJson(jDiscount, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out errorDiscount);
                    detailDiscount.Oid = Guid.NewGuid();
                    detailDiscount.DocumentDetail = targetDocumentDetail;
                    targetDocumentDetail.DocumentDetailDiscounts.Add(detailDiscount);

                }
            }
            RecalculateDocumentCosts(ref copy, false);
            return copy;
        }

        /// <summary>
        /// Cancels a document.It creates a new document with quantities reversed.When calling call DocumentSequence from Master
        /// </summary>
        /// <param name="documentGuid">The guid of the document to be canceled</param>
        /// <param name="currentUserOid">The user requesting the document canceling</param>
        /// <returns>The Guid of the canceling document if success, otherwise Guid.Empty</returns>
        public static Guid CancelDocument(Guid documentGuid, Guid currentUserOid, long newUpdatedOnTicks)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader document = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                if (document == null || currentUserOid == (Guid.Empty))
                {
                    HttpContext.Current.Session["Error"] = Resources.AnErrorOccurred;
                    return Guid.Empty;
                }
                else
                {
                    if (document.IsCanceled)
                    {
                        HttpContext.Current.Session["Error"] = Resources.DocumentAlreadyCanceled;
                    }
                    else if (document.DocumentSeries.IsCancelingSeries
                        || document.CanceledByDocument != null
                        || document.DerivedDocuments.Count > 0
                        || document.DocumentSeries.IsCanceledBy == null
                        )
                    {
                        HttpContext.Current.Session["Error"] = Resources.DocumentCannotBeCanceled;
                        return Guid.Empty;
                    }
                    else
                    {
                        DocumentHeader canceledDocument = CopyDocument(document.Oid, document.Session as UnitOfWork) as DocumentHeader;
                        canceledDocument.DocumentNumber = 0;
                        canceledDocument.DocumentType = document.DocumentType;
                        canceledDocument.DocumentSeries = document.DocumentSeries.IsCanceledBy;
                        canceledDocument.CancelsDocumentOid = document.Oid;
                        canceledDocument.CreatedBy = canceledDocument.Session.GetObjectByKey<User>(currentUserOid);

                        ReverseCancellingDocumentPrices(canceledDocument);

                        if (canceledDocument.CreatedBy == null)
                        {
                            HttpContext.Current.Session["Error"] = Resources.Login;
                            return Guid.Empty;
                        }

                        document.CanceledByDocumentOid = canceledDocument.Oid;
                        document.IsCanceled = true;

                        uow.Delete(document.ReferencedDocuments);

                        foreach (TransactionCoupon transactionCoupon in document.TransactionCoupons)
                        {
                            transactionCoupon.IsCanceled = true;
                            if (transactionCoupon.Coupon != null && transactionCoupon.Coupon.NumberOfTimesUsed > 0)
                            {
                                transactionCoupon.Coupon.NumberOfTimesUsed--;
                            }
                            transactionCoupon.Save();
                        }

                        canceledDocument.UpdatedOnTicks = (newUpdatedOnTicks <= 0) ? DateTime.Now.Ticks : newUpdatedOnTicks;

                        canceledDocument.Save();
                        XpoHelper.CommitChanges(uow);
                        return (Guid)document.CanceledByDocumentOid;
                    }
                }
            }
            return Guid.Empty;
        }


        public static string SignDocumentIfNecessary(Guid documentOid, User user, Guid StoreControllerSettingsOid)
        {

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentOid);

                if (documentHeader.DocumentType.TakesDigitalSignature
                && documentHeader.DocumentNumber > 0
                && String.IsNullOrWhiteSpace(documentHeader.Signature)
               )
                {
                    try
                    {
                        StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(StoreControllerSettingsOid);
                        List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                            Where(x =>
                                    x.DocumentSeries.Any(y => y.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid)
                                 && x.TerminalDevice is POSDevice
                                 && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                            ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                        string signature = DocumentHelper.SignDocument(documentHeader, user, documentHeader.Owner, String.Empty/*MvcApplication.OLAPConnectionString*/, posDevices);
                        if (string.IsNullOrWhiteSpace(signature))
                        {
                            return Resources.CannotRetreiveSignature;
                        }
                        documentHeader.Signature = signature;
                        documentHeader.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                    catch (Exception exception)
                    {
                        return exception.GetFullMessage();
                    }
                }
            }
            return String.Empty;
        }




        private static void ReverseCancellingDocumentPrices(DocumentHeader canceledDocument)
        {
            //DocumentDetails
            foreach (DocumentDetail detail in canceledDocument.DocumentDetails)
            {
                detail.Qty *= -1;
                detail.PackingQuantity *= -1;
                detail.TotalDiscount *= -1;
                detail.TotalVatAmountBeforeDiscount *= -1;
                detail.TotalVatAmount *= -1;
                detail.NetTotalBeforeDiscount *= -1;
                detail.NetTotal *= -1;
                detail.GrossTotalBeforeDiscount *= -1;
                detail.GrossTotalBeforeDocumentDiscount *= -1;
                detail.GrossTotal *= -1;
            }

            canceledDocument.TotalQty *= -1;
            canceledDocument.DocumentDiscountAmount *= -1;
            canceledDocument.PointsDiscountAmount *= -1;
            canceledDocument.TotalDiscountAmount *= -1;
            canceledDocument.PromotionsDiscountAmount *= -1;
            canceledDocument.TotalVatAmount *= -1;
            canceledDocument.TotalVatAmountBeforeDiscount *= -1;
            canceledDocument.NetTotalBeforeDiscount *= -1;
            canceledDocument.NetTotal *= -1;
            canceledDocument.GrossTotalBeforeDiscount *= -1;
            canceledDocument.GrossTotalBeforeDocumentDiscount *= -1;
            canceledDocument.GrossTotal *= -1;

            //Payments
            foreach (DocumentPayment payment in canceledDocument.DocumentPayments)
            {
                payment.Amount *= -1;
            }

            //Points
            canceledDocument.DocumentPoints *= -1;
            canceledDocument.PromotionPoints *= -1;
            canceledDocument.TotalPoints = canceledDocument.DocumentPoints + canceledDocument.PromotionPoints + canceledDocument.SumarisableDocumentDetails.Sum(x => x.Points);
            canceledDocument.PointsDiscountAmount *= -1;
            canceledDocument.ConsumedPointsForDiscount *= -1;
        }

        public static bool DocumentCanBeCanceled(Guid documentGuid, User user, out string reason)
        {
            bool canBeCanceled = true;
            reason = string.Empty;

            if (UserHelper.IsCustomer(user))
            {
                reason = Resources.YouCannotCancelThisDocument;
                return false;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader document = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                if (document != null)
                {
                    if (document.IsCanceled)
                    {
                        canBeCanceled = false;
                        reason = Resources.IsCanceled;
                    }
                    else if (document.DocumentNumber <= 0)
                    {
                        reason = Resources.DocumentCannotBeCanceledNoDocumentNumber;
                        return false;
                    }
                    else if (document.DocumentSeries != null && document.DocumentSeries.IsCanceledBy == null)
                    {
                        reason = string.Format(Resources.DocumentSeriesCannotBeCanceledNoIsCancelledSeries, document.DocumentSeries.Description);
                        canBeCanceled = false;
                    }
                    else if ((DateTime.Now - document.FiscalDate).Days > document.Owner.OwnerApplicationSettings.NumberOfDaysDocumentCanBeCanceled)
                    {
                        reason = string.Format(Resources.DocumentIsOlderThanTheAllowedDays, (DateTime.Now - document.FiscalDate).Days, document.Owner.OwnerApplicationSettings.NumberOfDaysDocumentCanBeCanceled);
                        canBeCanceled = false;
                    }

                    WRMApplicationSettings wrmApplicationSettings = uow.FindObject<WRMApplicationSettings>(null);
                    if (wrmApplicationSettings != null
                        && document.DocumentSeries != null
                        && document.DocumentSeries.IsCanceledBy != null
                        && document.DocumentSeries.eModule != eModule.ALL
                       )
                    {
                        switch (wrmApplicationSettings.ApplicationInstance)
                        {
                            case eApplicationInstance.DUAL_MODE:
                                if (document.DocumentSeries.eModule != eModule.DUAL && document.DocumentSeries.eModule != eModule.POS && document.DocumentSeries.eModule != eModule.SFA)
                                {
                                    reason = String.Format(Resources.DocumentCannotBeCanceledSeriesBelongsToDifferentApplicationInstances
                                                           , wrmApplicationSettings.ApplicationInstance
                                                           , document.DocumentSeries.eModule
                                                          );
                                    canBeCanceled = false;
                                }
                                break;

                            case eApplicationInstance.RETAIL:
                                if (document.DocumentSeries.eModule != eModule.HEADQUARTERS && document.DocumentSeries.eModule != eModule.POS && document.DocumentSeries.eModule != eModule.SFA)
                                {
                                    reason = String.Format(Resources.DocumentCannotBeCanceledSeriesBelongsToDifferentApplicationInstances
                                                           , wrmApplicationSettings.ApplicationInstance
                                                           , document.DocumentSeries.eModule
                                                          );
                                    canBeCanceled = false;
                                }
                                break;

                            case eApplicationInstance.STORE_CONTROLER:
                                if (document.DocumentSeries.eModule != eModule.STORECONTROLLER && document.DocumentSeries.eModule != eModule.POS && document.DocumentSeries.eModule != eModule.SFA)
                                {
                                    reason = String.Format(Resources.DocumentCannotBeCanceledSeriesBelongsToDifferentApplicationInstances
                                                           , wrmApplicationSettings.ApplicationInstance
                                                           , document.DocumentSeries.eModule
                                                          );
                                    canBeCanceled = false;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            return canBeCanceled;
        }

        public static bool DocumentCanBeDeleted(Guid documentGuid, User user, out string reason)
        {
            bool canBeDeleted = false;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader document = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                if (document.StoreDocumentSeriesType != null && document.DocumentSeries.eModule == eModule.POS)
                {
                    canBeDeleted = false;
                    reason = Resources.CannotDeleteDocument;
                }
                else if (document.IsCanceled)
                {
                    canBeDeleted = false;
                    reason = Resources.IsCanceled;
                }
                else if (document.HasBeenExecuted)
                {
                    canBeDeleted = false;
                    reason = Resources.HasBeenExecuted;
                }
                else if (document.DerivedDocuments.Count > 0)
                {
                    canBeDeleted = false;
                    reason = Resources.HasBeenTransformed;
                }
                else if (document.DocumentNumber > 0 && document.Status.TakeSequence)
                {
                    canBeDeleted = false;
                    reason = Resources.HasBeenAutoNumbered;
                }
                else
                {
                    canBeDeleted = true;
                    reason = "";
                }
            }
            return canBeDeleted;
        }

        public static object Payments(Guid? currentDocumentHeaderOid, DocumentHeader documentHeader)
        {
            if (documentHeader == null && (currentDocumentHeaderOid == null || currentDocumentHeaderOid == Guid.Empty))
            {
                HttpContext.Current.Session["Error"] = Resources.AnErrorOccurred;
                return null;
            }

            UnitOfWork uow = documentHeader == null ? XpoHelper.GetNewUnitOfWork() : (documentHeader.Session as UnitOfWork);

            if (HttpContext.Current.Request["DXCallbackArgument"] != null && HttpContext.Current.Request["DXCallbackArgument"].Contains("ADDNEW"))
            {
                HttpContext.Current.Session["currentDocumentPayment"] = new DocumentPayment(uow);
            }
            else if (HttpContext.Current.Request["DXCallbackArgument"] != null && HttpContext.Current.Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                int guidLength = Guid.Empty.ToString().Length;
                String docPaymentOid = HttpContext.Current.Request["DXCallbackArgument"].ToString().Substring(HttpContext.Current.Request["DXCallbackArgument"].ToString().Length - (guidLength + 1), guidLength);
                Guid docPaymentGuid;
                if (Guid.TryParse(docPaymentOid, out docPaymentGuid))
                {
                    HttpContext.Current.Session["currentDocumentPayment"] = uow.GetObjectByKey<DocumentPayment>(docPaymentGuid);
                }
                else
                {
                    HttpContext.Current.Session["Error"] = Resources.AnErrorOccurred;
                    return null;
                }
            }
            else
            {
                HttpContext.Current.Session["currentDocumentPayment"] = null;
            }

            return uow;
        }

        public static IEnumerable<DocumentVatInfo> GetDocumentVatAnalysis(DocumentHeader documentHeader)
        {
            List<DocumentVatInfo> vatAnalysis = new List<DocumentVatInfo>();

            if (documentHeader != null && documentHeader.DocumentDetails.Count > 0)
            {
                IEnumerable<Guid> vatFactors = documentHeader.DocumentDetails.Select(documentDetail => documentDetail.VatFactorGuid).Distinct();

                foreach (Guid vatFactorGuid in vatFactors)
                {
                    VatFactor vatFactor = documentHeader.Session.GetObjectByKey<VatFactor>(vatFactorGuid);
                    if (vatFactor != null)
                    {
                        DocumentVatInfo documentVatInfo = new DocumentVatInfo();
                        documentVatInfo.VatFactor = vatFactor.Factor * 100;
                        IEnumerable<DocumentDetail> vatFactorDocumentDetails = documentHeader.DocumentDetails.Where(documentDetail => documentDetail.VatFactorGuid == vatFactor.Oid);
                        documentVatInfo.ItemsQuantity = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.Qty);
                        documentVatInfo.NumberOfItems = vatFactorDocumentDetails.Select(documentDetail => documentDetail.Item.Oid).Distinct().Count();
                        documentVatInfo.TotalVatAmount = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.TotalVatAmount);
                        documentVatInfo.NetTotal = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.NetTotal);
                        documentVatInfo.GrossTotal = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.GrossTotal);
                        documentVatInfo.VatFactorOid = vatFactor.Oid;
                        vatAnalysis.Add(documentVatInfo);
                        eMinistryVatCategoryCode vat_cat_code = vatFactor.VatCategory.MinistryVatCategoryCode;
                        if (vat_cat_code == eMinistryVatCategoryCode.E)
                        {
                            documentHeader.VatAmount1 = documentVatInfo.TotalVatAmount;
                        }
                        else if (vat_cat_code == eMinistryVatCategoryCode.A)
                        {
                            documentHeader.VatAmount2 = documentVatInfo.TotalVatAmount;
                        }
                        else if (vat_cat_code == eMinistryVatCategoryCode.B)
                        {
                            documentHeader.VatAmount3 = documentVatInfo.TotalVatAmount;
                        }
                        else if (vat_cat_code == eMinistryVatCategoryCode.C)
                        {
                            documentHeader.VatAmount4 = documentVatInfo.TotalVatAmount;
                        }
                    }
                }
            }

            return vatAnalysis.OrderByDescending(vat => vat.VatFactor);
        }

        /// <summary>
        /// Σε κάθε παραστατικό που εκδίδεται είναι απαραίτητη και η ύπαρξη μιας γραμμής που περιέχει τα οικονομικά στοιχεία που πρέπει να διαβιβαστούν στην βάση δεδομένων της ΓΓΠΣ.
        ///Αυτά  έχουν περιγραφεί από την ΠΟΛ 1221 και είναι τα κάτωθι:
        ///α/α Πεδίου   Περιεχόμενο             Μήκος (χαρακτήρες)
        ///0        ΑΦΜ Εκδότη                              12
        ///1        ΑΦΜ Παραλήπτη                           12
        ///2        Αριθμός Κάρτας Αποδείξεων Πελάτη  *1    19
        ///3        Ημερομηνία και Ώρα                *4    12
        ///4        Περιγραφή Παραστατικού            *2 (Μεταβλητό)
        ///5        Σειρά Θεώρησης                          10
        ///6        Αριθμός Παραστατικού                    10
        ///7        Καθαρό Ποσό Α                           18:2
        ///8        Καθαρό Ποσό Β                           18:2
        ///9        Καθαρό Ποσό Γ                           18:2
        ///10       Καθαρό Ποσό Δ                           18:2
        ///11       Καθαρό Ποσό Ε                           18:2
        ///12       ΦΠΑ Α                                   18:2
        ///13       ΦΠΑ Β                                   18:2
        ///14       ΦΠΑ Γ                                   18:2
        ///15       ΦΠΑ Δ                                   18:2
        ///16       Γενικό Σύνολο Παρ/κού                   18:2
        ///17       Κωδικός νομίσματος
        ///
        ///*1    ΑΦΜ ή o αριθμός της πιστωτικής κάρτας του πελάτη αν δεν έχει κάρτα αποδείξεων.
        ///*2   Αν είναι γνωστός ο κωδικός του παραστατικού βάση της τυποποίησης των παραστατικών του taxis, τότε μπορεί να χρησιμοποιηθεί απ’ ευθείας στο πεδίο αυτό.
        ///Διαφορετικά μπορεί να χρησιμοποιηθεί η περιγραφή του παραστατικού. Κατόπιν χρειάζεται να προγραμματίσουμε στον driver την αντιστοίχηση της περιγραφής του πεδίου αυτού με τον σωστό κωδικό Taxis.
        ///*4 Η μορφοποίηση της ημερομηνίας είναι : YYYYMMDDHHmm
        ///
        /// YYYY = έτος   ΜΜ = Μήνας  DD = Μέρα  HH = Ώρα  mm = Λεπτά
        /// EXAMPLE: 999999999/123456789/1234567890123456789/020420131200/173/A/1001/3.00/3.00/3.00/3.00/3.00/0.19/0.39/0.69/1.08/14.35/1
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string CreateFiscalInfoLine(DocumentHeader header, string customerReceiptCard = null, int currencyCode = 0, string seperator = "/")
        {
            Store store = header.Store;
            CompanyNew company = header.Company;
            Trader companyTrader = company.Trader;
            string ownerTaxCode = companyTrader.TaxCode;

            Customer customer = header.Customer;
            Trader customerTrader = customer.Trader;
            string customerTaxCode = customerTrader.TaxCode;

            DocumentType docType = header.DocumentType;

            MinistryDocumentType ministryDocType = docType.MinistryDocumentType;

            if (ministryDocType == null)
            {
                throw new Exception("Document Type '" + (docType.Code ?? "") + " - " + (docType.Description ?? "") + "' has no Ministry document type assigned to it");
            }

            DocumentSeries series = header.DocumentSeries;
            string seriesDescription = series.Description;

            decimal vatAmount1 = 0;
            decimal vatAmount2 = 0;
            decimal vatAmount3 = 0;
            decimal vatAmount4 = 0;

            decimal netAmount1 = 0;
            decimal netAmount2 = 0;
            decimal netAmount3 = 0;
            decimal netAmount4 = 0;
            decimal netAmount5 = 0;

            foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
            {
                VatCategory vatCategory = detail.Item.VatCategory;
                if (vatCategory == null)
                    throw new Exception("Item '" + detail.Item.Code + "' has no Vat Category");
                switch (vatCategory.MinistryVatCategoryCode)
                {
                    case eMinistryVatCategoryCode.A:
                        vatAmount1 += detail.TotalVatAmount;
                        netAmount1 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.B:
                        vatAmount2 += detail.TotalVatAmount;
                        netAmount2 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.C:
                        vatAmount3 += detail.TotalVatAmount;
                        netAmount3 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.D:
                        vatAmount4 += detail.TotalVatAmount;
                        netAmount4 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.E:
                        netAmount5 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.NONE:
                        throw new Exception("Vat Category '" + vatCategory.Code ?? "" + " - " + vatCategory.Description ?? "" + "' has no Ministry Vat Category Code assigned to it.");
                }
            }

            //string oldSeperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." };

            string line = String.Format(nfi, "[<]{0}" + seperator + "{1}" + seperator + "{2}" + seperator + "{3:yyyyMMddHHmm}" + seperator + "{4}" + seperator + "{5}" + seperator +
                "{6}" + seperator + "{7:0.00}" + seperator + "{8:0.00}" + seperator + "{9:0.00}" + seperator + "{10:0.00}" + seperator + "{11:0.00}" + seperator + "{12:0.00}" + seperator + "{13:0.00}"
                + seperator + "{14:0.00}" + seperator + "{15:0.00}" + seperator + "{16:0.00}" + seperator + "{17}[>]",
                String.Concat(ownerTaxCode.Take(12)),
                String.Concat(customerTaxCode.Take(12)),
                customerReceiptCard == null ? String.Concat(customerTaxCode.Take(19)) : String.Concat(customerReceiptCard.Take(19)),
                header.FinalizedDate,
                ministryDocType.Code,
                String.Concat(seriesDescription.Take(10)),
                header.DocumentNumber, netAmount1, netAmount2,
                netAmount3, netAmount4, netAmount5,
                vatAmount1,
                vatAmount2,
                vatAmount3,
                vatAmount4,
                header.GrossTotal, currencyCode   //Euro is 0
                );

            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = oldSeperator;
            return line;
        }

        public static void FixDocumentVatDeviations(ref DocumentHeader documentHeader, VatFactor vatFactor, decimal netTotalUserValue, decimal vatAmountUserValue)
        {
            IEnumerable<DocumentDetail> vatFactorDocumentDetails = documentHeader.DocumentDetails.Where(y => y.VatFactorGuid == vatFactor.Oid && y.IsCanceled == false);
            decimal vatCategoryVatTotalAmount = vatFactorDocumentDetails.Sum(z => z.TotalVatAmount);
            decimal vatCategoryGrossTotal = vatFactorDocumentDetails.Sum(z => z.GrossTotal);
            decimal vatCategoryNetTotal = vatFactorDocumentDetails.Sum(z => z.NetTotal);

            #region fix NetTotal

            decimal vatFactorDocumentHeaderNetTotal = vatFactorDocumentDetails.Sum(x => x.NetTotal);
            decimal documentHeaderNetTotalDeviation = netTotalUserValue - vatFactorDocumentHeaderNetTotal;
            int sign = Math.Sign(documentHeaderNetTotalDeviation);
            decimal remainingNetTotalDeviation = Math.Abs(documentHeaderNetTotalDeviation);

            int vatFactorDocumentDetailsCount = vatFactorDocumentDetails.Count();

            vatFactorDocumentDetails.OrderByDescending(dc => dc.NetTotal).ToList().ForEach(delegate (DocumentDetail documentDetail)
            {
                decimal ratio = vatFactorDocumentHeaderNetTotal == 0 ? 1.0m / vatFactorDocumentDetailsCount : documentDetail.NetTotal / vatFactorDocumentHeaderNetTotal;

                documentDetail.NetTotalDeviation = BusinessLogic.Round(documentHeaderNetTotalDeviation * ratio, documentDetail.DocumentHeader.Owner);
                if (Math.Abs(documentDetail.NetTotalDeviation) > remainingNetTotalDeviation)
                {
                    documentDetail.NetTotalDeviation = sign * remainingNetTotalDeviation;
                }
                documentDetail.NetTotal += documentDetail.NetTotalDeviation;
                documentDetail.Save();
                remainingNetTotalDeviation -= Math.Abs(documentDetail.NetTotalDeviation);
            });

            #endregion fix NetTotal

            //// fix VatAmount
            decimal vatFactorDocumentHeaderVatAmount = vatFactorDocumentDetails.Sum(x => x.TotalVatAmount);
            decimal documentHeaderVatAmountDeviation = vatAmountUserValue - vatFactorDocumentHeaderVatAmount;
            sign = Math.Sign(documentHeaderVatAmountDeviation);
            decimal remainingVatAmountDeviation = Math.Abs(documentHeaderVatAmountDeviation);
            vatFactorDocumentDetails.OrderByDescending(dc => dc.TotalVatAmount).ToList().ForEach(delegate (DocumentDetail documentDetail)
            {
                decimal ratio = vatFactorDocumentHeaderVatAmount == 0 ? 1.0m / vatFactorDocumentDetailsCount : documentDetail.TotalVatAmount / vatFactorDocumentHeaderVatAmount;
                documentDetail.TotalVatAmountDeviation = BusinessLogic.Round(documentHeaderVatAmountDeviation * ratio, documentDetail.DocumentHeader.Owner);
                if (Math.Abs(documentDetail.TotalVatAmountDeviation) > remainingVatAmountDeviation)
                {
                    documentDetail.TotalVatAmountDeviation = sign * remainingVatAmountDeviation;
                }
                documentDetail.TotalVatAmount += documentDetail.TotalVatAmountDeviation;
                documentDetail.Save();
                remainingVatAmountDeviation -= Math.Abs(documentDetail.TotalVatAmountDeviation);
            });

            vatFactorDocumentDetails.ToList().ForEach(delegate (DocumentDetail documentDetail)
            {
                documentDetail.GrossTotal = documentDetail.NetTotal + documentDetail.TotalVatAmount;
            });

            RecalculateDocumentCosts(ref documentHeader, false);
        }

        public static eDocumentTypeView CurrentUserDocumentView(User user, DocumentType documentType)
        {
            if (user == null || UserHelper.IsCustomer(user))
            {
                return eDocumentTypeView.Simple;
            }

            if (documentType == null || (documentType.DocumentTypeRoles.Count == 0 && documentType.ManualLinkedLineInsertion == false))
            {
                //Fall back
                return eDocumentTypeView.Advanced;
            }

            if (documentType.ManualLinkedLineInsertion)
            {
                return eDocumentTypeView.CompositionDecomposition;//So far only this form supports manual linked lines
            }

            DocumentTypeRole documentTypeRole = documentType.DocumentTypeRoles.FirstOrDefault(docTypeRole => docTypeRole.Role.Oid == user.Role.Oid);
            if (documentTypeRole == null)
            {
                //Fall back
                return eDocumentTypeView.Advanced;
            }
            return documentTypeRole.DocumentView;
        }

        public static bool IsOrder(DocumentHeader document)
        {
            if (document == null || document.DocumentType == null)
            {
                return false;
            }
            return document.DocumentType.IsDefault;
        }

        public static DocumentDetail GetPreviousDetail(DocumentDetail current)
        {
            return current.DocumentHeader.DocumentDetails.Where(x => x.LineNumber < current.LineNumber && x.DocumentHeader != null && x.IsLinkedLine == false).OrderByDescending(x => x.LineNumber).FirstOrDefault();
        }

        public static DocumentDetail GetNextDetail(DocumentDetail current)
        {
            return current.DocumentHeader.DocumentDetails.Where(x => x.LineNumber > current.LineNumber && x.DocumentHeader != null && x.IsLinkedLine == false).OrderBy(x => x.LineNumber).FirstOrDefault();
        }

        public static void UpdateLinkedItems(ref DocumentHeader documentHeader, DocumentDetail documentdetail)
        {
            if (documentHeader.DocumentType.ManualLinkedLineInsertion == false)
            {
                IEnumerable<DocumentDetail> linkeditems = documentHeader.DocumentDetails.Where(docDetail => docDetail.LinkedLine == documentdetail.Oid);
                if (linkeditems.Count() > 0)
                {
                    foreach (DocumentDetail linkeddetail in linkeditems.ToList())
                    {
                        documentHeader.DocumentDetails.Remove(linkeddetail);
                    }
                    AddLinkedItems(ref documentHeader, documentdetail);
                }
            }
        }

        public static void GetDefaultDocStatus(ref DocumentHeader document, CompanyNew owner)
        {
            document.Status = document.DocumentType.DefaultDocumentStatus;
            BinaryOperator ownercriteria = new BinaryOperator("Owner.Oid", owner.Oid);
            if (document.Status == null)
            {
                document.Status = document.Session.FindObject<DocumentStatus>(CriteriaOperator.And(new BinaryOperator("IsDefault", true), ownercriteria));
                if (document.Status == null)
                {
                    document.Status = document.Session.FindObject<DocumentStatus>(ownercriteria);
                }
            }
        }

        public static DocumentDetail ComputeDocumentLine(ref DocumentHeader documentHeader, Item item, Barcode barcode, decimal qty,
            bool is_linked_line, decimal unitPrice, bool hasCustomPrice, string customDescription, IEnumerable<DocumentDetailDiscount> discounts,
            bool hasCustomMeasurementUnit = false, string customMeasurementUnit = "", DocumentDetail oldDocumentLine = null,
            DocumentDetailAssociation documentDetailAssociation = null, bool UseDetailAssociation = false, VatFactor customVatFactor = null)
        {
            DocumentDetail currentLine;
            if (oldDocumentLine != null)
            {
                currentLine = oldDocumentLine;
            }
            else
            {
                currentLine = new DocumentDetail(documentHeader.Session);
            }

            decimal price;  // -> PriceListUnitPrice
            bool vatIncluded;

            if (qty <= 0 && !documentHeader.IsCancelingAnotherDocument)
            {
                qty = 1;
            }

            //Adding or replacing detail's discounts
            if (discounts != null)
            {
                foreach (DocumentDetailDiscount discount in discounts.ToList())
                {
                    DocumentDetailDiscount DiscountToRemove = currentLine.DocumentDetailDiscounts.Where(detaildiscount => detaildiscount.DiscountSource == discount.DiscountSource).FirstOrDefault();
                    if (DiscountToRemove != null)
                    {
                        currentLine.DocumentDetailDiscounts.Remove(DiscountToRemove);
                    }
                    if (discount.Value != 0 || discount.Percentage != 0)
                    {
                        currentLine.DocumentDetailDiscounts.Add(discount);
                    }
                }
            }

            try
            {
                // Fill in basic details: Item, barcode & description
                currentLine.Item = item;
                currentLine.DoesNotAllowDiscount = item.DoesNotAllowDiscount;
                currentLine.IsTax = item.IsTax;
                currentLine.ItemCode = item == null ? "" : item.Code;
                currentLine.Barcode = barcode;
                currentLine.BarcodeCode = barcode == null ? "" : barcode.Code;
                if (currentLine.Item.AcceptsCustomDescription && !String.IsNullOrEmpty(customDescription))
                {
                    currentLine.CustomDescription = customDescription;
                }
                else
                {
                    currentLine.CustomDescription = item.Name;
                }

                DocumentType initdoctype = documentDetailAssociation != null ? documentDetailAssociation.documentDetail.DocumentHeader.DocumentType : null;
                TransformationRule transformrule = initdoctype == null || documentHeader.DocumentType == null ? null : documentHeader.Session.FindObject<TransformationRule>(CriteriaOperator.And(new BinaryOperator("InitialType", initdoctype.Oid), new BinaryOperator("DerrivedType", documentHeader.DocumentType.Oid)));

                VatFactor vatFactor = customVatFactor;

                if (vatFactor == null)
                {
                    VatCategory vatcat = item.VatCategory;

                    VatLevel vatlev = null;
                    if (documentHeader.DocumentType.SupportCustomerVatLevel)
                    {
                        switch (documentHeader.Division)
                        {
                            case eDivision.Sales:
                                vatlev = documentHeader.Customer.GetVatLevel(documentHeader.BillingAddress);
                                break;

                            case eDivision.Purchase:
                                vatlev = documentHeader.Supplier.GetVatLevel(documentHeader.BillingAddress);
                                break;

                            case eDivision.Store:
                                vatlev = documentHeader.Store.Address.VatLevel;
                                if (vatlev == null)
                                {
                                    throw new Exception("VatLevel For Store is not defined");
                                }
                                break;

                            default:
                                throw new Exception("Wrong Division");
                        }
                    }
                    else
                    {
                        vatlev = documentHeader.Store.Address.VatLevel;
                    }

                    if (item.VatCategory == null)
                    {
                        vatcat = documentHeader.Session.FindObject<VatCategory>(new BinaryOperator("IsDefault", true));
                        throw new Exception(Resources.ItemVatCategoryNotFound + Environment.NewLine + currentLine.ToString());
                    }
                    if (vatlev == null)
                    {
                        vatlev = documentHeader.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                        if (vatlev == null)
                        {
                            throw new Exception(Resources.CustomerVatLevelNotFound + Environment.NewLine + currentLine.ToString());
                        }
                    }

                    vatFactor = documentHeader.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory", vatcat),
                                                                                                  new BinaryOperator("VatLevel", vatlev)));
                    if (vatFactor == null)
                    {
                        throw new Exception(Resources.VatFactorNotFound + Environment.NewLine + currentLine.ToString());
                    }
                }

                VatFactor oldVatFactor = oldDocumentLine != null && currentLine.VatFactorGuid != Guid.Empty
          /* existing document detail */? documentHeader.Session.GetObjectByKey<VatFactor>(currentLine.VatFactorGuid)
          /*      new document detail */: documentHeader.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory", item.VatCategory.Oid),
                                                                                                  new BinaryOperator("VatLevel", documentHeader.Store.Address.VatLevel.Oid)));

                if (oldVatFactor == null)
                {
                    throw new Exception(Resources.VatFactorNotFound + Environment.NewLine + currentLine.ToString());
                }

                currentLine.VatFactor = vatFactor.Factor;
                currentLine.VatFactorCode = vatFactor.Code;
                currentLine.VatFactorGuid = vatFactor.Oid;
                //Measurement Unit
                Guid mm;
                if (Guid.TryParse(customMeasurementUnit, out mm))
                {
                    currentLine.MeasurementUnit = currentLine.Session.GetObjectByKey<MeasurementUnit>(mm);
                }
                if (currentLine.MeasurementUnit == null)
                {
                    currentLine.MeasurementUnit = (barcode == null) ? item.DefaultBarcode.MeasurementUnit(documentHeader.Owner) : barcode.MeasurementUnit(documentHeader.Owner);
                }

                PriceCatalogDetail priceCatalogDetail = null;

                currentLine.HasCustomPrice = hasCustomPrice;

                if (hasCustomPrice || documentHeader.IsCancelingAnotherDocument)
                {
                    if (unitPrice <= 0 && documentHeader.DocumentType != null && documentHeader.DocumentType.AllowItemZeroPrices == false)
                    {
                        unitPrice = 1;
                    }
                    price = unitPrice;
                    vatIncluded = !(documentHeader.DocumentType.IsForWholesale || documentHeader.Division == eDivision.Purchase);
                    currentLine.PriceCatalog = Guid.Empty;
                }
                else
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork, documentHeader.EffectivePriceCatalogPolicy,
                        item, barcode);
                    priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    if (priceCatalogDetail == null && !is_linked_line)
                    {
                        throw new Exception(string.Format(Resources.SelectedPriceCatalogPolicyNotContainsAllItems, currentLine.Item.Name));
                    }
                    else if (is_linked_line && priceCatalogDetail == null)
                    {
                        //priceCatalogDetail = null;
                        //priceCatalogDetail= new PriceCatalogDetail(documentHeader.Session);
                        price = 0;
                        currentLine.PriceCatalog = Guid.Empty;
                        vatIncluded = false;
                        //priceCatalogDetail.Discount = 0;
                    }
                    else
                    {
                        price = priceCatalogDetail.Value;
                        currentLine.PriceCatalog = priceCatalogDetail.PriceCatalog.Oid;
                        vatIncluded = priceCatalogDetail.VATIncluded;
                    }

                    if (priceCatalogDetail != null)
                    {
                        if (priceCatalogDetail.Discount > 0 && currentLine.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG).Count() == 0)
                        {
                            currentLine.DocumentDetailDiscounts.Add(DiscountHelper.CreatePriceCatalogDetailDiscount((UnitOfWork)currentLine.Session, priceCatalogDetail.Discount));
                        }
                    }
                }

                if (is_linked_line && currentLine.UnitPrice < 0)
                {
                    price = .0m;
                }

                if (documentHeader.DocumentType.AllowItemZeroPrices == false && price == 0 && !is_linked_line)
                {
                    throw new Exception(Resources.ItemsWithZeroPricesAreNotAllowed);
                }

                //Compute Packing Qty Measurement Unit
                if (documentDetailAssociation == null && UseDetailAssociation)
                {
                    throw new Exception("The function was incorrectly called");
                }
                else if (UseDetailAssociation)
                {
                    qty *= (decimal)transformrule.QtyTransformationFactor;
                    price *= (decimal)transformrule.ValueTransformationFactor;

                    if (initdoctype.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.DEFAULT))
                    {
                        currentLine.Qty = qty;
                        currentLine.PackingQuantity = qty;
                        currentLine.PackingMeasurementUnit = currentLine.MeasurementUnit = currentLine.Barcode.MeasurementUnit(documentHeader.Owner);
                        currentLine.PackingMeasurementUnitRelationFactor = 1;
                        currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                    }
                    else if (initdoctype.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.PACKING))
                    {
                        if (documentHeader.DocumentType != null && documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.DEFAULT))
                        {
                            currentLine.Qty = qty;
                            currentLine.PackingQuantity = qty;
                            currentLine.PackingMeasurementUnit = currentLine.MeasurementUnit = currentLine.Barcode.MeasurementUnit(documentHeader.Owner);
                            currentLine.PackingMeasurementUnitRelationFactor = 1;
                            currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                        }
                        else if (documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.PACKING))
                        {
                            currentLine.Qty = qty;
                            currentLine.MeasurementUnit = currentLine.Session.GetObjectByKey<MeasurementUnit>(documentDetailAssociation.documentDetail.MeasurementUnit.Oid);
                            currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                            if (currentLine.Item.PackingQty <= 0 || currentLine.Item.PackingMeasurementUnit == null || currentLine.Item.PackingMeasurementUnit == currentLine.MeasurementUnit)
                            {
                                currentLine.PackingQuantity = qty;
                                currentLine.PackingMeasurementUnitRelationFactor = 1;
                                currentLine.PackingMeasurementUnit = currentLine.MeasurementUnit;
                            }
                            else
                            {
                                currentLine.PackingQuantity = qty / (decimal)documentDetailAssociation.documentDetail.Item.PackingQty;
                                currentLine.PackingMeasurementUnit = currentLine.Item.PackingMeasurementUnit;
                                currentLine.PackingMeasurementUnitRelationFactor = documentDetailAssociation.documentDetail.Item.PackingQty;
                            }
                        }
                    }
                }
                else
                {
                    if (documentHeader.DocumentType != null && documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.DEFAULT))
                    {
                        currentLine.Qty = qty;
                        currentLine.PackingQuantity = qty;
                        currentLine.PackingMeasurementUnit = currentLine.MeasurementUnit = currentLine.Barcode.MeasurementUnit(documentHeader.Owner);
                        currentLine.PackingMeasurementUnitRelationFactor = 1;
                        currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                    }
                    else if (documentHeader.DocumentType != null && documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.PACKING))
                    {
                        ItemBarcode taxCodeItemBarcode = BOItemHelper.GetTaxCodeBarcode((UnitOfWork)currentLine.Session, currentLine.Item, documentHeader.Owner);
                        if (taxCodeItemBarcode == null)
                        {
                            throw new Exception(string.Format(Resources.PleaseDefineTaxCodeBarcodeForItem, currentLine.Item.Name));
                        }

                        currentLine.PackingQuantity = qty;
                        currentLine.PackingMeasurementUnit = currentLine.Item.PackingMeasurementUnit;
                        if (currentLine.Item.PackingQty <= 0 || currentLine.PackingMeasurementUnit == null || currentLine.PackingMeasurementUnit == taxCodeItemBarcode.MeasurementUnit)
                        {
                            currentLine.PackingMeasurementUnitRelationFactor = 1;
                            currentLine.PackingMeasurementUnit = taxCodeItemBarcode.MeasurementUnit;
                        }
                        else
                        {
                            currentLine.PackingMeasurementUnitRelationFactor = currentLine.Item.PackingQty;
                        }

                        currentLine.Qty = currentLine.PackingQuantity * (decimal)currentLine.PackingMeasurementUnitRelationFactor;
                        currentLine.MeasurementUnit = taxCodeItemBarcode.MeasurementUnit;
                        currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                    }
                }

                #region Calculate BaseQuantity and BaseMeasurementUnit for ItemStock

                if (currentLine.Item.DefaultBarcode.Oid == currentLine.Barcode.Oid)
                {
                    currentLine.BaseQuantity = (double)currentLine.Qty;
                    currentLine.BaseMeasurementUnit = currentLine.MeasurementUnit;
                }
                else
                {
                    ItemBarcode itemBarcode = currentLine.Barcode.ItemBarcode(documentHeader.Owner);
                    decimal relationFactor = itemBarcode == null ? 1 : (decimal)itemBarcode.RelationFactor;
                    currentLine.BaseQuantity = (double)(relationFactor * currentLine.Qty);
                    currentLine.BaseMeasurementUnit = currentLine.Item.DefaultBarcode.MeasurementUnit(documentHeader.Owner) ?? currentLine.MeasurementUnit;
                }

                #endregion Calculate BaseQuantity and BaseMeasurementUnit for ItemStock

                bool priceHasChanged = oldDocumentLine != null && oldDocumentLine.CustomUnitPrice != price;
                currentLine.CustomUnitPrice = currentLine.PriceListUnitPrice = price;
                if (hasCustomPrice || oldDocumentLine == null || priceHasChanged)
                {
                    if (vatIncluded)
                    {
                        currentLine.PriceListUnitPriceWithoutVAT = price / (1 + oldVatFactor.Factor);
                        currentLine.PriceListUnitPriceWithVAT = (price / (1 + oldVatFactor.Factor)) * (1 + currentLine.VatFactor);
                    }
                    else
                    {
                        currentLine.PriceListUnitPriceWithoutVAT = price;
                        currentLine.PriceListUnitPriceWithVAT = price * (1 + currentLine.VatFactor);
                    }
                }
                else
                {
                    currentLine.PriceListUnitPriceWithoutVAT = oldDocumentLine.PriceListUnitPriceWithoutVAT;
                    currentLine.PriceListUnitPriceWithVAT = currentLine.PriceListUnitPriceWithoutVAT * (1 + currentLine.VatFactor);
                }

                //CHECKING LIMITS
                if (currentLine.Qty > documentHeader.DocumentType.MaxDetailQty && documentHeader.DocumentType.MaxDetailQty > 0)
                {
                    throw new Exception(String.Format(Resources.MaxItemOrderQtyExceeded, currentLine.Qty));
                }

                if (price > documentHeader.DocumentType.MaxDetailValue && documentHeader.DocumentType.MaxDetailValue > 0)
                {
                    throw new Exception(String.Format(Resources.InvalidDetailValue, price));
                }

                if (Math.Abs(currentLine.Qty * price) > documentHeader.DocumentType.MaxDetailTotal && documentHeader.DocumentType.MaxDetailTotal > 0)
                {
                    throw new Exception(String.Format(Resources.InvalidDetailTotal, currentLine.Qty * price));
                }

                if (documentHeader.DocumentType == null)
                {
                    documentHeader.DocumentType = documentHeader.Session.FindObject<DocumentType>(new BinaryOperator("IsDefault", true));
                    if (documentHeader.DocumentType == null)
                    {
                        documentHeader.DocumentType = documentHeader.Session.FindObject<DocumentType>(null);
                    }
                }

                if (documentHeader.DocumentType.IsForWholesale || documentHeader.Division == eDivision.Purchase)
                {
                    //Round στην τιμή, με βάση
                    currentLine.PriceListUnitPrice = RoundDisplayValueDigits(currentLine.PriceListUnitPrice, documentHeader.Owner);
                    currentLine.PriceListUnitPriceWithoutVAT = RoundDisplayValueDigits(currentLine.PriceListUnitPriceWithoutVAT, documentHeader.Owner);
                    WholesaleDocumentDetail(ref currentLine, documentHeader, vatIncluded);
                }
                else
                {
                    currentLine.PriceListUnitPrice = vatIncluded ? RoundDisplayValue(currentLine.PriceListUnitPrice, documentHeader.Owner) : RoundValue(currentLine.PriceListUnitPrice, documentHeader.Owner);
                    currentLine.PriceListUnitPriceWithVAT = RoundDisplayValue(currentLine.PriceListUnitPriceWithVAT, documentHeader.Owner);
                    RetailDocumentDetail(ref currentLine, documentHeader, vatIncluded);
                }

                //Computes points of line if documenet type is Sales, otherwise set points to 0
                if (documentHeader.Division == eDivision.Sales && item.IsTax == false && item.DoesNotAllowDiscount == false)
                {
                    decimal decimalPoints = ItemHelper.GetPointsOfItem(currentLine.Item, documentHeader.DocumentType, documentHeader.Owner.OwnerApplicationSettings) * Math.Floor(currentLine.Qty);
                    int points = 0;
                    if (decimalPoints >= int.MaxValue)
                    {
                        points = int.MaxValue;
                    }
                    else
                    {
                        points = (int)decimalPoints;
                    }
                    currentLine.Points = points;
                }
                else
                {
                    currentLine.Points = 0;
                }

                //SOS Calculate Points
                currentLine.Points = ItemHelper.GetPointsOfItem(currentLine.Item, documentHeader.DocumentType, documentHeader.Owner.OwnerApplicationSettings) * Math.Floor(currentLine.Qty);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + currentLine.Item.Name);
            }
            RecalculateDocumentCosts(ref documentHeader, false, false);
            return currentLine;
        }

        /*
private Guid _LinkedLine;
private double _UnitPriceAfterDiscount;             // Τελικη τιμή άνευ ΦΠΑ μετά έκπτωσης //Εδώ θα εφαρμοστεί η Δεύτερη Έκπτωση στην περίπτωση ΧΟΝΤΡΙΚΗΣ
private double _PriceListUnitPrice;                 // Αρχική τιμή άνευ ΦΠΑ ή τιμή καρφωτή/χρήστη
private double _UnitPrice;                          // PriceListUnitPrice μετά έκπτωσης πελάτη προ ΦΠΑ
private double _Qty;                                // Ποσότητα
private double _FirstDiscount;                      // Εκπτωση τιμοκαταλόγου ανά τεμάχιο
private double _SecondDiscount;                     // 2η Εκπτωση (επιπλέον) ανά τεμάχιο
private double _VatFactor;                          // Ποσοστό ΦΠΑ
private double _VatAmount;                          // Ποσό ΦΠΑ ανά τεμάχιο
private double _NetTotalAfterDiscount;              // Καθαρή αξία (μετά εκπτώσεων)
private double _GrossTotal;                         // Συνολική αξία γραμμής (με ΦΠΑ)
private double _FinalUnitPrice;                     // Τελική τιμή μονάδος με ΦΠΑ //Εδώ θα εφαρμοστεί η Δεύτερη Έκπτωση στην περίπτωση ΛΙΑΝΙΚΗΣ
private double _TotalDiscount;                      // Συνολικό ποσό έκπτωσης
private double _TotalVatAmount;                     // Συνολικό ποσό ΦΠΑ
private double _NetTotal;                           // Καθαρή αξία (προ εκπτώσεων)
*
* Γνωστά
* _FirstDiscount;
* _SecondDiscount;
* _VatFactor
* _Qty
* _PriceListUnitPrice (with or without VAT);
* * */

        private static void RetailDocumentDetail(ref DocumentDetail documentDetail, DocumentHeader header, bool vatIncluded)
        {
            if (header.DocumentType.IsForWholesale)
            {
                throw new Exception("This code has been called incorrectly....");
            }

            if (vatIncluded)
            {
                documentDetail.CustomUnitPrice = documentDetail.PriceListUnitPriceWithoutVAT * (1m + documentDetail.VatFactor);
                documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPriceWithoutVAT, header.Owner);

                //documentDetail.CustomUnitPrice = (documentDetail.PriceListUnitPrice / (1.0m + oldVatFactor)) * (1m + documentDetail.VatFactor);
                //documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPrice / (1.0m + documentDetail.VatFactor), header.Owner);
                //documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPrice / (1.0m + oldVatFactor), header.Owner);
            }
            else
            {
                documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPriceWithoutVAT, header.Owner);
                documentDetail.CustomUnitPrice = RoundDisplayValue(documentDetail.PriceListUnitPriceWithoutVAT, header.Owner);

                //documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPrice, header.Owner);
                //documentDetail.CustomUnitPrice = RoundDisplayValue(documentDetail.PriceListUnitPrice * (1 + documentDetail.VatFactor), header.Owner);
                //documentDetail.CustomUnitPrice = RoundDisplayValue(documentDetail.PriceListUnitPrice * (1 + oldVatFactor), header.Owner);
            }
            documentDetail.GrossTotalBeforeDiscount = documentDetail.CustomUnitPrice * documentDetail.Qty;
            documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount * documentDetail.VatFactor / (1 + documentDetail.VatFactor), header.Owner);
            documentDetail.NetTotalBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalVatAmountBeforeDiscount, header.Owner);

            CalculateDetailTotalDiscount(ref documentDetail, header);

            documentDetail.GrossTotalBeforeDocumentDiscount = documentDetail.GrossTotalBeforeDiscount - RoundValue(documentDetail.TotalNonDocumentDiscount, header.Owner);
            documentDetail.GrossTotal = RoundValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalDiscount, header.Owner);
            documentDetail.TotalVatAmount = RoundDisplayValue(documentDetail.GrossTotal * documentDetail.VatFactor / (1 + documentDetail.VatFactor), header.Owner);
            documentDetail.NetTotal = RoundDisplayValue(documentDetail.GrossTotal - documentDetail.TotalVatAmount, header.Owner);
            documentDetail.FinalUnitPrice = documentDetail.NetTotal / documentDetail.Qty;
        }

        private static void WholesaleDocumentDetail(ref DocumentDetail documentDetail, DocumentHeader header, bool vatIncluded)
        {
            if (!header.DocumentType.IsForWholesale && header.Division != eDivision.Purchase)
            {
                throw new Exception("This code has been called incorrectly....");
            }
            documentDetail.UnitPrice = documentDetail.PriceListUnitPriceWithoutVAT;//documentDetail.UnitPrice = documentDetail.PriceListUnitPrice / (1 + (vatIncluded == true ? documentDetail.VatFactor : 0.0m));
            documentDetail.NetTotalBeforeDiscount = RoundDisplayValue(documentDetail.UnitPrice * documentDetail.Qty, header.Owner);
            documentDetail.GrossTotalBeforeDiscount = RoundDisplayValue(documentDetail.NetTotalBeforeDiscount * (1 + documentDetail.VatFactor), header.Owner);
            documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.NetTotalBeforeDiscount, header.Owner);

            CalculateDetailTotalDiscount(ref documentDetail, header);
            documentDetail.GrossTotalBeforeDocumentDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalNonDocumentDiscount, header.Owner);
            documentDetail.NetTotal = RoundDisplayValue(documentDetail.NetTotalBeforeDiscount - documentDetail.TotalDiscount, header.Owner);
            documentDetail.TotalVatAmount = RoundDisplayValue(documentDetail.NetTotal * documentDetail.VatFactor, header.Owner);
            documentDetail.GrossTotal = RoundDisplayValue(documentDetail.NetTotal + documentDetail.TotalVatAmount, header.Owner);
            documentDetail.FinalUnitPrice = documentDetail.NetTotal / documentDetail.Qty;
            documentDetail.CustomUnitPrice = documentDetail.UnitPrice;
        }

        private static decimal RoundValue(decimal value, CompanyNew owner)
        {
            return Math.Round(value, (int)owner.OwnerApplicationSettings.ComputeDigits, MidpointRounding.AwayFromZero);
        }

        private static decimal RoundDisplayValue(decimal value, CompanyNew owner)
        {
            return Math.Round(value, (int)owner.OwnerApplicationSettings.DisplayDigits, MidpointRounding.AwayFromZero);
        }

        private static decimal RoundDisplayValueDigits(decimal value, CompanyNew owner)
        {
            return Math.Round(value, (int)owner.OwnerApplicationSettings.DisplayValueDigits, MidpointRounding.AwayFromZero);
        }

        private static decimal CalculateDiscountAmount(DocumentDetailDiscount discount, decimal amountToApplyDiscount)
        {
            decimal discountValue = 0;
            if (discount.DocumentDetail.IsTax == false && discount.DocumentDetail.DoesNotAllowDiscount == false)
            {
                if (discount.DiscountType == eDiscountType.PERCENTAGE)
                {
                    discountValue = amountToApplyDiscount * discount.Percentage;
                    discount.Value = discountValue;
                }
                else
                {
                    discountValue = discount.Value;
                }
            }

            return discountValue;
        }

        /// <summary>
        /// Calculates the TotalDiscount field of the detail using the DocumentDetailDiscounts list
        /// </summary>
        /// <param name="documentDetail"></param>
        private static void CalculateDetailTotalDiscount(ref DocumentDetail documentDetail, DocumentHeader header)
        {
            bool isWholeSale = header.DocumentType.IsForWholesale;
            decimal totalDiscount = 0, originalAmount;
            if (!documentDetail.IsTax && !documentDetail.DoesNotAllowDiscount)
            {
                decimal amountToApplyDiscount = originalAmount = isWholeSale ? documentDetail.NetTotalBeforeDiscount : documentDetail.GrossTotalBeforeDiscount;

                DocumentDetailDiscount overridesAllDiscount = documentDetail.DocumentDetailDiscounts.OrderBy(x => x.Priority).FirstOrDefault(x => x.DiscardsOtherDiscounts);
                if (overridesAllDiscount != null)
                {
                    documentDetail.TotalDiscount = CalculateDiscountAmount(overridesAllDiscount, amountToApplyDiscount);
                    if (isWholeSale)
                    {
                        overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value;
                        overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value * (1 + documentDetail.VatFactor);
                    }
                    else
                    {
                        overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value;
                        overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value / (1 + documentDetail.VatFactor);
                    }
                    return;
                }

                foreach (DocumentDetailDiscount discount in documentDetail.DocumentDetailDiscounts.OrderBy(x => x.Priority))
                {
                    if (discount.Percentage == 0)
                    {
                        if (amountToApplyDiscount != 0)
                        {
                            discount.Percentage = discount.Value / amountToApplyDiscount;
                        }
                    }
                    else
                    {
                        discount.Value = discount.Percentage * amountToApplyDiscount;
                    }
                    decimal discountAmount = RoundDisplayValue(CalculateDiscountAmount(discount, amountToApplyDiscount), header.Owner);
                    totalDiscount += discountAmount;
                    amountToApplyDiscount = originalAmount - totalDiscount;
                    if (isWholeSale)
                    {
                        discount.DiscountWithoutVAT = discount.Value;
                        discount.DiscountWithVAT = discount.Value * (1 + documentDetail.VatFactor);
                    }
                    else
                    {
                        discount.DiscountWithVAT = discount.Value;
                        discount.DiscountWithoutVAT = discount.Value / (1 + documentDetail.VatFactor);
                    }
                }

                documentDetail.TotalDiscount = totalDiscount;

                if (isWholeSale)
                {
                    documentDetail.TotalDiscountAmountWithoutVAT = documentDetail.TotalDiscount;
                    documentDetail.TotalDiscountAmountWithVAT = documentDetail.TotalDiscount * (1 + documentDetail.VatFactor);
                }
                else
                {
                    documentDetail.TotalDiscountAmountWithoutVAT = documentDetail.TotalDiscount / (1 + documentDetail.VatFactor);
                    documentDetail.TotalDiscountAmountWithVAT = documentDetail.TotalDiscount;
                }
            }
        }

        public static List<DocumentDetailAssociation> CreateDocumentDetailAssociations(XPCollection<DocumentHeader> documents)
        {
            List<DocumentDetailAssociation> transformationDetails = new List<DocumentDetailAssociation>();
            foreach (DocumentHeader document in documents)
            {
                foreach (DocumentDetail detail in document.TransformableDocumentDetails)
                {
                    DocumentDetailAssociation detailAssociation = new DocumentDetailAssociation(detail, detail.UnreferencedQuantity);
                    transformationDetails.Add(detailAssociation);
                }
            }
            return transformationDetails;
        }

        /// <summary>
        /// Creates document payments based on DocumentType settings
        /// </summary>
        /// <param name="documentHeader">The document for which the payment should be created</param>
        /// <returns>A localised error message if something went wrong, otherwise string.Empty</returns>
        public static string CreateDefaultDocumentPayments(DocumentHeader documentHeader)
        {
            string errorMessage = string.Empty;
            if (documentHeader.DocumentType != null
             && documentHeader.DocumentType.UsesPaymentMethods
             && documentHeader.Division != eDivision.Financial
               )
            {
                if (documentHeader.DocumentPayments.Count == 0)
                {
                    if (documentHeader.DocumentType.DefaultPaymentMethod != null)
                    {
                        DocumentPayment docpayment = new DocumentPayment(documentHeader.Session);
                        docpayment.DocumentHeader = documentHeader;
                        docpayment.PaymentMethod = documentHeader.DocumentType.DefaultPaymentMethod;
                        docpayment.Amount = documentHeader.GrossTotal;
                        documentHeader.DocumentPayments.Add(docpayment);
                    }
                    else
                    {
                        errorMessage = Resources.NoPaymentMethodDefined;
                    }
                }
                else
                {
                    if (documentHeader.DocumentPayments.Sum(docpayment => docpayment.Amount) != documentHeader.GrossTotal)
                    {

                        FixSmallPaymentDeviations(ref documentHeader);
                    }

                    if (documentHeader.DocumentPayments.Sum(docpayment => docpayment.Amount) != documentHeader.GrossTotal)
                    {
                        errorMessage = Resources.PaymentMethodsAmountDiff;
                    }
                }
            }
            return errorMessage;
        }

        public static bool PointDocumentSettingsHaveBeenSet(OwnerApplicationSettings ownerApplicationSettings)
        {
            if (ownerApplicationSettings.PointsDocumentType == null
                 || ownerApplicationSettings.PointsDocumentSeries == null
                 || ownerApplicationSettings.PointsDocumentStatus == null
                )
            {
                return false;
            }
            return true;
        }

        public static bool DocTypeSupportsCustomer(DocumentHeader document, Customer customer)
        {
            return document.Session.FindObject<Customer>(CriteriaOperator.And(DocumentTypeSupportedCustomersCriteria(document), new BinaryOperator("Oid", customer.Oid))) != null;
        }

        public static CriteriaOperator DocumentTypeSupportedCustomersCriteria(DocumentHeader document)
        {
            IEnumerable<Guid> customerCategories = document.DocumentType.DocTypeCustomerCategories.SelectMany(x => x.CustomerCategory.GetNodeIDs());

            if (customerCategories.Count() == 0)
            {
                return null;
            }
            switch (document.DocumentType.DocTypeCustomerCategoryMode)
            {
                case eDocTypeCustomerCategory.EXCLUDE_CUSTOMER_CATEGORIES:
                    return CriteriaOperator.And(new NotOperator(new ContainsOperator("CustomerAnalyticTrees", new InOperator("Node.Oid", customerCategories))));

                case eDocTypeCustomerCategory.INCLUDE_CUSTOMER_CATEGORIES:
                    return CriteriaOperator.And(new ContainsOperator("CustomerAnalyticTrees", new InOperator("Node.Oid", customerCategories)));

                default:
                    return null;
            }
        }

        public static bool AvailableDocumentTypesPerStore(User user, CompanyNew owner, Guid storeOid, eDivision division, eApplicationInstance AppInstance, out List<string> errormessages, bool isOrder = false, bool isProforma = false)
        {
            errormessages = new List<string>();
            CriteriaOperator docTypeCriteria = new BinaryOperator("Division.Section", division);

            docTypeCriteria = RetailHelper.ApplyOwnerCriteria(docTypeCriteria, typeof(DocumentType), owner);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Store store = uow.GetObjectByKey<Store>(storeOid);

                IEnumerable<DocumentType> docTypes = null;
                if (isProforma)
                {
                    docTypes = GetProformaTypes(uow);
                }
                else
                {
                    docTypes = new XPCollection<DocumentType>(uow, docTypeCriteria).Select(doctype => doctype);
                }

                if (docTypes != null)
                {
                    bool seriesFound = false;

                    foreach (DocumentType doctype in docTypes)
                    {
                        IEnumerable<DocumentSeries> docseries = StoreHelper.StoreSeriesForDocumentType(store, doctype, GetDocSeriesModule(AppInstance), onlyForOrder: isOrder);
                        if (docseries.Count() > 0)
                        {
                            seriesFound = true;
                            break;
                        }
                    }

                    if (!seriesFound)
                    {
                        errormessages.Add(isOrder ? Resources.NoSeriesForOrder :
                                       (isProforma ? Resources.NoSeriesForProforma :
                                       (division == eDivision.Sales ? Resources.NoSeriesForSalesDoc : (division == eDivision.Purchase ? Resources.NoSeriesForPurchaseDoc : Resources.NoSeriesForStoreDoc))));
                    }
                    return seriesFound;
                }
                else
                {
                    errormessages.Add(isOrder ? Resources.NoOrderDocumentTypeDefined :
                                   (isProforma ? Resources.NoProformaTypeDefined :
                                   (division == eDivision.Sales ? Resources.NoSalesDocumentTypeDefined : (division == eDivision.Purchase ? Resources.NoPurchaseDocumentTypeDefined : Resources.NoStoreDocumentTypeDefined))));
                    return false;
                }
            }
        }

        public static void SaveMarkUps(List<Reevaluate> reavaluations, UnitOfWork uow, bool saveMarkUpValues)
        {
            foreach (Reevaluate reevaluate in reavaluations.Where(reeval => reeval.Selected))
            {
                PriceCatalogDetail priceCatalogDetail = uow.GetObjectByKey<PriceCatalogDetail>(reevaluate.priceCatalogDetail.Oid);
                if (saveMarkUpValues)
                {
                    priceCatalogDetail.MarkUp = reevaluate.MarkUp;
                }
                priceCatalogDetail.DatabaseValue = reevaluate.UnitPrice;
                priceCatalogDetail.Save();
            };
            uow.CommitTransaction();
        }

        /// <summary>
        /// Checks if an item is supported by current document's type
        /// </summary>
        /// <param name="document">A DocumentHeader object</param>
        /// <param name="item">An Item object</param>
        /// <returns>True if item is supported, false otherwise</returns>
        public static bool DocumentTypeSupportsItem(DocumentHeader document, Item item)
        {
            return document.Session.FindObject<Item>(CriteriaOperator.And(DocumentTypeSupportedItemsCriteria(document), new BinaryOperator("Oid", item.Oid))) != null;
        }

        /// <summary>
        /// Creates the criteria operator used for selecting available items for current document's type
        /// </summary>
        /// <param name="document">A DocumentHeader object</param>
        /// <returns>A CriteriaOperator object</returns>
        public static CriteriaOperator DocumentTypeSupportedItemsCriteria(DocumentHeader document)
        {
            IEnumerable<Guid> itemCategories = document.DocumentType.DocumentTypeItemCategories.SelectMany(x => x.ItemCategory.GetNodeIDs());
            CriteriaOperator itemPriceCriteria = new BinaryOperator("IsActive", true);
            if (!document.DocumentType.AcceptsGeneralItems)
            {
                itemPriceCriteria = CriteriaOperator.And(itemPriceCriteria, CriteriaOperator.Or(new BinaryOperator("IsGeneralItem", true, BinaryOperatorType.NotEqual),
                                                                                               new NullOperator("IsGeneralItem")));
            }

            if (document.Division == eDivision.Sales && document.PriceCatalogPolicy != null)
            {
                List<Guid> pcOids = PriceCatalogHelper.GetPriceCatalogsFromPolicy(document.EffectivePriceCatalogPolicy);

                CriteriaOperator priceCriteria = null;

                if (!document.DocumentType.AllowItemZeroPrices)
                {
                    long nowTicks = DateTime.Now.Ticks;
                    CriteriaOperator timeValueCriteria = new ContainsOperator("TimeValues",
                                CriteriaOperator.And(
                                    new BinaryOperator("TimeValue", 0m, BinaryOperatorType.Greater),
                                    new BinaryOperator("TimeValueValidFrom", nowTicks, BinaryOperatorType.LessOrEqual),
                                    new BinaryOperator("TimeValueValidUntil", nowTicks, BinaryOperatorType.GreaterOrEqual)
                                                                             ));

                    priceCriteria = CriteriaOperator.Or(timeValueCriteria, new BinaryOperator("DatabaseValue", 0m, BinaryOperatorType.Greater));
                }

                itemPriceCriteria = CriteriaOperator.And(itemPriceCriteria,
                                                         new ContainsOperator("PriceCatalogs", CriteriaOperator.And(new BinaryOperator("IsActive", true),
                                                                                                                    priceCriteria,
                                                                                                                    new InOperator("PriceCatalog.Oid", pcOids))));
            }
            if (itemCategories.Count() == 0)
            {
                return itemPriceCriteria;
            }
            switch (document.DocumentType.DocumentTypeItemCategoryMode)
            {
                case eDocumentTypeItemCategory.EXCLUDE_ITEM_CATEGORIES:
                    return CriteriaOperator.And(itemPriceCriteria,
                                                new NotOperator(new ContainsOperator("ItemAnalyticTrees", new InOperator("Node.Oid", itemCategories))));

                case eDocumentTypeItemCategory.INCLUDE_ITEM_CATEGORIES:
                    return CriteriaOperator.And(itemPriceCriteria,
                                                new ContainsOperator("ItemAnalyticTrees", new InOperator("Node.Oid", itemCategories)));

                default:
                    return itemPriceCriteria;
            }
        }

        /// <summary>
        /// Creates the criteria operator used for selecting available price catalog details for current document's type
        /// </summary>
        /// <param name="document">A DocumentHeader object</param>
        /// <returns>A CriteriaOperator object</returns>
        public static CriteriaOperator DocumentTypeSupportedItemsCriteriaForPriceCatalogDetail(DocumentHeader document)
        {
            IEnumerable<Guid> itemCategories = document.DocumentType.DocumentTypeItemCategories.SelectMany(x => x.ItemCategory.GetNodeIDs());
            CriteriaOperator priceCatalogCriteria = CriteriaOperator.And(new BinaryOperator("IsActive", true), new BinaryOperator("Item.IsActive", true));
            if (!document.DocumentType.AcceptsGeneralItems)
            {
                priceCatalogCriteria = CriteriaOperator.And(priceCatalogCriteria, CriteriaOperator.Or(new BinaryOperator("Item.IsGeneralItem", true, BinaryOperatorType.NotEqual),
                                                                                                      new NullOperator("Item.IsGeneralItem")));
            }

            if (document.Division == eDivision.Sales && document.PriceCatalogPolicy != null)
            {
                List<Guid> pcOids = PriceCatalogHelper.GetPriceCatalogsFromPolicy(document.EffectivePriceCatalogPolicy);
                if (pcOids.Count == 0)//There is no valid PriceCatalog in the current Policy
                {
                    return new BinaryOperator("PriceCatalog.Oid", Guid.Empty);
                }
                CriteriaOperator priceCriteria = null;

                if (!document.DocumentType.AllowItemZeroPrices)
                {
                    long nowTicks = DateTime.Now.Ticks;
                    CriteriaOperator timeValueCriteria = CriteriaOperator.And(
                                    new BinaryOperator("TimeValue", 0m, BinaryOperatorType.Greater),
                                    new BinaryOperator("TimeValueValidFrom", nowTicks, BinaryOperatorType.LessOrEqual),
                                    new BinaryOperator("TimeValueValidUntil", nowTicks, BinaryOperatorType.GreaterOrEqual)
                                                                             );

                    priceCriteria = CriteriaOperator.Or(timeValueCriteria, new BinaryOperator("DatabaseValue", 0m, BinaryOperatorType.Greater));
                }

                priceCatalogCriteria = CriteriaOperator.And(priceCatalogCriteria,
                                                            priceCriteria,
                                                            new InOperator("PriceCatalog.Oid", pcOids));
            }
            if (itemCategories.Count() == 0)
            {
                return priceCatalogCriteria;
            }
            switch (document.DocumentType.DocumentTypeItemCategoryMode)
            {
                case eDocumentTypeItemCategory.EXCLUDE_ITEM_CATEGORIES:
                    return CriteriaOperator.And(priceCatalogCriteria,
                                                new NotOperator(new ContainsOperator("Item.ItemAnalyticTrees", new InOperator("Node.Oid", itemCategories))));

                case eDocumentTypeItemCategory.INCLUDE_ITEM_CATEGORIES:
                    return CriteriaOperator.And(priceCatalogCriteria,
                                                new ContainsOperator("Item.ItemAnalyticTrees", new InOperator("Node.Oid", itemCategories)));

                default:
                    return priceCatalogCriteria;
            }
        }

        /// <summary>
        /// Returns the filters of all Document Types defined as order for a specific Store
        /// </summary>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        private static CriteriaOperator GetOrderDocumentTypesCriteria(Store store)
        {
            CriteriaOperator storeDocumentTypeCriteria = CriteriaOperator.And(new BinaryOperator("DocumentSeries.Store", store.Oid), new BinaryOperator("StoreDocumentType", eStoreDocumentType.ORDER));
            CriteriaOperator criteria = CriteriaOperator.And(
                            new BinaryOperator("Division.Section", eDivision.Sales),
                            new ContainsOperator("StoreDocumentSeriesTypes", storeDocumentTypeCriteria)
                            );

            return RetailHelper.ApplyOwnerCriteria(criteria, typeof(DocumentType), store.Owner);
        }

        /// <summary>
        /// Returns the filters of all Store Document Series Types defined as order for a specific Store
        /// </summary>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        private static CriteriaOperator GetOrderStoreDocumentSeriesTypesCriteria(Store store)
        {
            CriteriaOperator criteria = CriteriaOperator.And(
                            new BinaryOperator("StoreDocumentType", eStoreDocumentType.ORDER),
                            new BinaryOperator("DocumentSeries.Store", store.Oid),
                            new BinaryOperator("DocumentType.Division.Section", eDivision.Sales)
                            );

            return RetailHelper.ApplyOwnerCriteria(criteria, typeof(StoreDocumentSeriesType), store.Owner);
        }

        /// <summary>
        /// Returns a collection of all Document Types defined as order for a specific Store
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork where the XPCollection is returned</param>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        public static XPCollection<DocumentType> GetOrderDocumentTypes(UnitOfWork unitOfWork, Store store)
        {
            CriteriaOperator criteria = GetOrderDocumentTypesCriteria(store);
            return new XPCollection<DocumentType>(unitOfWork, criteria);
        }

        /// <summary>
        /// Returns a collection of all Store Document Series Types defined as order for a specific Store
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork where the XPCollection is returned</param>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        public static XPCollection<StoreDocumentSeriesType> GetOrderStoreDocumentSeriesTypes(UnitOfWork unitOfWork, Store store)
        {
            CriteriaOperator criteria = GetOrderStoreDocumentSeriesTypesCriteria(store);
            return new XPCollection<StoreDocumentSeriesType>(unitOfWork, criteria);
        }

        /// <summary>
        /// Returns the default Order DocumentType for the specified Store in the specified  UnitOfWork. If only one DocumentType found return this independendly of it's IsDefault value
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork where the DocumentType is returned</param>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        //public static DocumentType GetFirstOrDefaultOrderDocumentType(UnitOfWork unitOfWork, Store store)
        //{
        //    XPCollection<DocumentType> orderDocumentTypes = GetOrderDocumentTypes(unitOfWork, store);

        //    if (orderDocumentTypes.Count == 0)
        //    {
        //        return null;
        //    }

        //    if (orderDocumentTypes.Count == 1)
        //    {
        //        return orderDocumentTypes.First();
        //    }

        //    return orderDocumentTypes.Where(documentType => documentType.IsDefault).FirstOrDefault();
        //}

        /// <summary>
        /// Returns the number of Order DocumentTypes found for Store store
        /// </summary>
        /// <param name="unitOfWork">The UnitOfWork where DocumentTypes is counted</param>
        /// <param name="store">The Store where the order should have been defined</param>
        /// <returns></returns>
        public static int OrderDocumentTypeHasBeenDefined(UnitOfWork unitOfWork, Store store)
        {
            CriteriaOperator criteria = GetOrderDocumentTypesCriteria(store);
            int ordersCount = (int)unitOfWork.Evaluate(typeof(DocumentType), CriteriaOperator.Parse("Count"), criteria);
            return ordersCount;
        }

        /// <summary>
        /// Returns all the Proforma types
        /// </summary>
        /// <param name="uow">The UnitOfWork where DocumentTypes is counted</param>
        /// <returns></returns>
        public static List<DocumentType> GetProformaTypes(UnitOfWork uow)
        {
            XPCollection<Model.POS> poses = new XPCollection<Model.POS>(uow);
            return poses.Select(pos => pos.ProFormaInvoiceDocumentType).Distinct().ToList();
        }

        /// <summary>
        /// Returns all the Proforma types
        /// </summary>
        /// <param name="uow">The UnitOfWork where DocumentTypes is counted</param>
        /// <returns></returns>
        public static List<DocumentType> GetSpecialProformaTypes(UnitOfWork uow)
        {
            return uow.Query<Model.POS>().Where(x => x.SpecialProformaDocumentType != null).Select(x => x.SpecialProformaDocumentType).Distinct().ToList();
        }

        /// <summary>
        /// Returns the eModule corresponding to current MvcApplication
        /// </summary>
        /// <returns></returns>
        public static eModule GetDocSeriesModule(eApplicationInstance applicationInstance)
        {
            eModule module = eModule.ALL;
            switch (applicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    module = eModule.DUAL;
                    break;

                case eApplicationInstance.RETAIL:
                    module = eModule.HEADQUARTERS;
                    break;

                case eApplicationInstance.STORE_CONTROLER:
                    module = eModule.STORECONTROLLER;
                    break;

                default:
                    break;
            }
            return module;
        }

        /// <summary>
        /// Checks if document's maximum count of lines is exceeded
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="message">The error message to be returned</param>
        /// <returns>True if count is exceeded, false otherwise</returns>
        public static bool MaxCountOfLinesExceeded(DocumentHeader document, out string message)
        {
            message = "";
            if (document.DocumentType.MaxCountOfLines > 0 && document.DocumentDetails.Count >= document.DocumentType.MaxCountOfLines)
            {
                message = Resources.MaxCountOfDocLinesExceeded;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if after adding multiple lines the limit of document.DocumentType.MacCountOfLines will be surpassed
        /// </summary>
        /// <param name="document">The document the lines will be added in</param>
        /// <param name="linesToBeAdded">The number of lines to be added</param>
        /// <returns>True if after adding the lines the limit will be surpassed, otherwise false-</returns>
        public static bool CheckIfMaximumLinesLimitWillExceed(DocumentHeader document, int linesToBeAdded)
        {
            if (document.DocumentType.MaxCountOfLines > 0 && document.DocumentDetails.Count + linesToBeAdded > document.DocumentType.MaxCountOfLines)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the selected PriceCatalog contains all items of the document
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="priceCatalog">The new price catalog selected</param>
        /// <returns>The items not included, if any, otherwise null</returns>
        public static List<DocumentDetail> PriceCatalogNotIncludedItems(DocumentHeader document, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy)
        {
            List<DocumentDetail> notIncludedItemsOids = new List<DocumentDetail>();
            if (document.DocumentDetails != null)
            {
                foreach (DocumentDetail detail in document.DocumentDetails)
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(document.Session as UnitOfWork,
                                                                                            effectivePriceCatalogPolicy,
                                                                                            detail.Item,
                                                                                            detail.Barcode);
                    if (priceCatalogPolicyPriceResult == null || priceCatalogPolicyPriceResult.PriceCatalogDetail == null)
                    {
                        notIncludedItemsOids.Add(detail);
                    }
                }
            }
            return notIncludedItemsOids;
        }



        public static List<MergedDocumentDetail> MergeDetails(List<Guid> docHeaderOids, CriteriaOperator filter)
        {
            List<MergedDocumentDetail> details = new List<MergedDocumentDetail>();
            foreach (Guid docOid in docHeaderOids)
            {
                CriteriaOperator crit = null;
                CriteriaOperator linesCrit = CriteriaOperator.And(new BinaryOperator("DocumentHeader.Oid", docOid, BinaryOperatorType.Equal),
                                        new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
                                        new BinaryOperator("IsReturn", false, BinaryOperatorType.Equal), new OperandProperty("GCRecord").IsNull());
                crit = linesCrit;
                if (filter != null)
                {
                    crit = CriteriaOperator.And(linesCrit, filter);
                }
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    int docTypeFactor = uow.GetObjectByKey<DocumentHeader>(docOid)?.DocumentType?.QuantityFactor ?? 1;
                    List<DocumentDetail> docDetails = new XPCollection<DocumentDetail>(uow, crit)?.ToList();
                    if (docDetails != null && docDetails.Count > 0)
                    {
                        foreach (DocumentDetail dtl in docDetails)
                        {
                            decimal detailQty = dtl.Qty * docTypeFactor;
                            MergedDocumentDetail mergeDetail = details.Where(x => x.ItemOid == dtl.ItemOid).FirstOrDefault();
                            if (mergeDetail != null)
                            {
                                mergeDetail.Qty = mergeDetail.Qty + dtl.Qty * docTypeFactor;
                            }
                            else
                            {
                                mergeDetail = new MergedDocumentDetail(dtl, docOid, docTypeFactor);
                                details.Add(mergeDetail);
                            }
                        }
                    }
                }
            }
            return details;
        }


        public static string SignDocument(DocumentHeader currentDocumentHeader, User currentUser, CompanyNew owner, string olapConnectionString, List<POSDevice> posDevices)
        {
            if (posDevices == null || posDevices.Count <= 0)
            {
                throw new SignatureFailureException(Resources.PleaseCheckFiscalDevicesForStore);
            }

            string rtfTempFile = Path.GetTempFileName();
            string tempFile = Path.GetTempFileName();

            DocumentTypeCustomReport customReport = ReportsHelper.GetValidDocumentTypeCustomReports(currentUser, currentDocumentHeader.DocumentType, currentDocumentHeader.Session as UnitOfWork).Distinct().FirstOrDefault();
            string title, description;
            XtraReport report = null;
            if (customReport != null && customReport.Report != null)
            {
                report = ReportsHelper.GetXtraReport(customReport.Report.Oid, owner, currentUser,
                    olapConnectionString, out title, out description);
                if (report is SingleObjectXtraReport)
                {
                    (report as SingleObjectXtraReport).ObjectOid = currentDocumentHeader.Oid;
                }
                int duplicates = customReport.Duplicates > 0 ? customReport.Duplicates : 1;
                ReportsHelper.DuplicateReport(report as XtraReportBaseExtension, duplicates);
            }

            TextExportOptions options = new TextExportOptions();
            options.Encoding = Encoding.UTF8;
            if (report == null)
            {
                throw new Exception(String.Format(Resources.PleaseDefineAReportForDocumentType, currentDocumentHeader.DocumentType.Description));
            }
            report.ExportToText(tempFile, options);
            report.Dispose();
            string signature = "";
            int deviceCounter = -1;
            string errorMessage = String.Empty;

            while (String.IsNullOrEmpty(signature))
            {
                deviceCounter++;
                if (deviceCounter >= posDevices.Count)
                {
                    break;
                }

                //StoreControllerTerminalDeviceAssociation deviceAssociation = posDevices.OrderByDescending(g => g.IsPrimary).ToList()[i];
                POSDevice posDevice = posDevices[deviceCounter];
                if (posDevice == null)
                {
                    continue;
                }
                if (posDevice.DeviceSettings.DeviceType.GetSupportedConnections().Contains(posDevice.ConnectionType) == false)
                {
                    continue;
                }

                switch (posDevice.DeviceSettings.DeviceType)
                {
                    /* case DeviceType.DataSignESD:
                         DataSignESD dataSign = new DataSignESD(posDevice.ConnectionType, posDevice.Name);
                         if (posDevice.ConnectionType == ConnectionType.ETHERNET)
                         {
                             dataSign.Settings.Ethernet.IPAddress = (posDevice.DeviceSettings as ITS.Retail.Model.EthernetDeviceSettings).IPAddress;
                         }
                         DataSignESD.DataSignResult signResult = dataSign.SignDocument(deviceAssociation.ABCDirectory, tempFile, ref signature);
                         if (signResult != DataSignESD.DataSignResult.ERR_SUCCESS)
                         {
                             signature = "";
                         }
                         break;

                     case DeviceType.AlgoboxNetESD:
                         AlgoboxNetESD algobox = new AlgoboxNetESD(posDevice.ConnectionType, posDevice.Name);

                         if (posDevice.ConnectionType == ConnectionType.ETHERNET)
                         {
                             algobox.Settings.Ethernet.IPAddress = (posDevice.DeviceSettings as ITS.Retail.Model.EthernetDeviceSettings).IPAddress;
                             algobox.Settings.Ethernet.Port = (posDevice.DeviceSettings as ITS.Retail.Model.EthernetDeviceSettings).Port;
                         }
                         else if (posDevice.ConnectionType == ConnectionType.COM)
                         {
                             algobox.Settings.COM.PortName = (posDevice.DeviceSettings as ITS.Retail.Model.COMDeviceSettings).PortName;
                             algobox.Settings.COM.BaudRate = (posDevice.DeviceSettings as ITS.Retail.Model.COMDeviceSettings).BaudRate;
                         }
                         string algoExResult = "";
                         AlgoboxNetESD.AlgoboxNetResult algoSignResult = algobox.SignDocument(tempFile, DocumentHelper.CreateFiscalInfoLine(currentDocumentHeader), ref algoExResult);
                         if (algoSignResult != AlgoboxNetESD.AlgoboxNetResult.SUCCESS)
                         {
                             signature = "";
                         }
                         else
                         {
                             signature = algoExResult;
                         }
                         break;*/
                    case DeviceType.DiSign:
                        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                        DiSign disign = new DiSign(posDevice.ConnectionType, posDevice.Name);

                        if (posDevice.ConnectionType == ConnectionType.ETHERNET)
                        {
                            disign.Settings.Ethernet.IPAddress = (posDevice.DeviceSettings as Model.EthernetDeviceSettings).IPAddress;
                            disign.Settings.Ethernet.Port = (posDevice.DeviceSettings as Model.EthernetDeviceSettings).Port;
                        }

                        disign.AfterLoad(null);
                        string disignExResult = "";

                        int disignResult = disign.SignDocument(File.ReadAllText(tempFile), CreateFiscalInfoLine(currentDocumentHeader), ref disignExResult);

                        if (disignResult != 0)
                        {
                            errorMessage = disignExResult;
                            signature = "";
                        }
                        else
                        {
                            signature = disignExResult;
                        }
                        break;
                }
            }
            try
            {
                File.Delete(tempFile);
                File.Delete(rtfTempFile);
            }
            catch
            {
            }

            if (String.IsNullOrEmpty(signature))
            {
                string exceptionMessage = Resources.CannotRetreiveSignature;
                if (String.IsNullOrWhiteSpace(errorMessage) == false)
                {
                    exceptionMessage += Environment.NewLine + errorMessage;
                }
                throw new SignatureFailureException(exceptionMessage);
            }
            return signature;
        }

        /// <summary>
        /// While saving a document relative customer points are updated
        /// </summary>
        /// <param name="documentHeader">The document</param>
        /// <param name="applicationInstance">Application instance</param>
        public static void UpdateCustomerPoints(DocumentHeader documentHeader, eApplicationInstance applicationInstance)
        {
            bool pointsConsumed = false;
            bool pointsAdded = false;
            if (applicationInstance == eApplicationInstance.DUAL_MODE || applicationInstance == eApplicationInstance.RETAIL)
            {
                pointsConsumed = documentHeader.PointsConsumed;
                pointsAdded = documentHeader.PointsAddedToCustomer;
            }
            else
            {
                ////Store controller
                pointsConsumed = documentHeader.PointsConsumedAtStoreController;
                pointsAdded = documentHeader.PointsAddedToCustomerAtStoreController;
            }

            ////Remove points from the customer
            if (documentHeader.IsCanceled == false && documentHeader.Customer != null && documentHeader.ConsumedPointsForDiscount > 0 && pointsConsumed == false)
            {
                documentHeader.Customer.CollectedPoints -= documentHeader.ConsumedPointsForDiscount;
                documentHeader.Customer.TotalConsumedPoints += documentHeader.ConsumedPointsForDiscount;
                if (applicationInstance == eApplicationInstance.DUAL_MODE || applicationInstance == eApplicationInstance.RETAIL)
                {
                    documentHeader.PointsConsumed = true;
                }
                else
                {
                    documentHeader.PointsConsumedAtStoreController = true;
                }
            }

            ////Points added to the customer
            if (documentHeader.IsCanceled == false && documentHeader.Customer != null && documentHeader.TotalPoints > 0 && pointsAdded == false)
            {
                documentHeader.Customer.CollectedPoints += documentHeader.TotalPoints;
                documentHeader.Customer.TotalEarnedPoints += documentHeader.TotalPoints;
                if (applicationInstance == eApplicationInstance.DUAL_MODE || applicationInstance == eApplicationInstance.RETAIL)
                {
                    documentHeader.PointsAddedToCustomer = true;
                }
                else
                {
                    documentHeader.PointsAddedToCustomerAtStoreController = true;
                }
            }
        }

        /// <summary>
        /// While saving a document check transaction coupons and update relative coupons
        /// </summary>
        /// <param name="documentHeader">The document</param>
        /// <param name="applicationInstance">Application instance</param>
        public static void UpdateDocumentCoupons(DocumentHeader documentHeader, eApplicationInstance applicationInstance)
        {
            if (documentHeader.IsCanceled == false)
            {
                IEnumerable<TransactionCoupon> nonCanceledTransactionCoupons = documentHeader.TransactionCoupons.Where(transactionCoupon => transactionCoupon.IsCanceled == false);
                bool couponsHaveBeenUpdated = false;

                switch (applicationInstance)
                {
                    case eApplicationInstance.DUAL_MODE:
                        if (documentHeader.CouponsHaveBeenUpdatedOnStoreController)
                        {
                            documentHeader.CouponsHaveBeenUpdatedOnMaster = true;
                        }
                        couponsHaveBeenUpdated = documentHeader.CouponsHaveBeenUpdatedOnMaster;
                        break;

                    case eApplicationInstance.RETAIL:
                        couponsHaveBeenUpdated = documentHeader.CouponsHaveBeenUpdatedOnMaster;
                        break;

                    case eApplicationInstance.STORE_CONTROLER:
                        couponsHaveBeenUpdated = documentHeader.CouponsHaveBeenUpdatedOnStoreController;
                        break;
                }

                if (!documentHeader.DocumentType.ReserveCoupons)
                {
                    switch (applicationInstance)
                    {
                        case eApplicationInstance.DUAL_MODE:
                            documentHeader.CouponsHaveBeenUpdatedOnStoreController = true;
                            documentHeader.CouponsHaveBeenUpdatedOnMaster = true;
                            break;

                        case eApplicationInstance.RETAIL:
                            documentHeader.CouponsHaveBeenUpdatedOnMaster = true;
                            break;

                        case eApplicationInstance.STORE_CONTROLER:
                            documentHeader.CouponsHaveBeenUpdatedOnStoreController = true;
                            break;
                    }
                }
                else if (documentHeader.DocumentType.ReserveCoupons && couponsHaveBeenUpdated == false && nonCanceledTransactionCoupons.Count() > 0)
                {
                    switch (applicationInstance)
                    {
                        case eApplicationInstance.DUAL_MODE:
                            foreach (TransactionCoupon transactionCoupon in documentHeader.TransactionCoupons)
                            {
                                Coupon coupon = transactionCoupon.Coupon != null ?
                                                documentHeader.Session.GetObjectByKey<Coupon>(transactionCoupon.Coupon.Oid) ??
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.Coupon.Code)) :
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.CouponCode));
                                if (coupon != null)
                                {
                                    coupon.NumberOfTimesUsed++;
                                    coupon.Save();
                                }
                            }
                            documentHeader.CouponsHaveBeenUpdatedOnStoreController = true;
                            documentHeader.CouponsHaveBeenUpdatedOnMaster = true;
                            break;

                        case eApplicationInstance.RETAIL:
                            foreach (TransactionCoupon transactionCoupon in documentHeader.TransactionCoupons)
                            {
                                Coupon coupon = transactionCoupon.Coupon != null ?
                                                documentHeader.Session.GetObjectByKey<Coupon>(transactionCoupon.Coupon.Oid) ??
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.Coupon.Code)) :
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.CouponCode));

                                if (coupon != null)
                                {
                                    coupon.NumberOfTimesUsed++;
                                    coupon.Save();
                                }
                                else
                                {
                                    coupon = new Coupon(documentHeader.Session)
                                    {
                                        Owner = documentHeader.Session.GetObjectByKey<CompanyNew>(documentHeader.Owner.Oid),
                                        CouponCategory = documentHeader.Session.GetObjectByKey<CouponCategory>(transactionCoupon.CouponMask.CouponCategory.Oid),
                                        CouponMask = documentHeader.Session.GetObjectByKey<CouponMask>(transactionCoupon.CouponMask.Oid),
                                        Amount = CouponHelper.GetMaskAmount(transactionCoupon.CouponCode, transactionCoupon.CouponMask),
                                        Code = transactionCoupon.CouponCode,
                                        CouponAmountIsAppliedAs = transactionCoupon.CouponMask.CouponAmountIsAppliedAs,
                                        CouponAmountType = transactionCoupon.CouponMask.CouponAmountType,
                                        CouponAppliesOn = transactionCoupon.CouponMask.CouponAppliesOn,
                                        Description = transactionCoupon.CouponMask.Description,
                                        DiscountType = transactionCoupon.CouponMask.DiscountType == null ? null : documentHeader.Session.GetObjectByKey<DiscountType>(transactionCoupon.CouponMask.DiscountType.Oid),
                                        IsActiveFrom = transactionCoupon.CouponMask.IsActiveFrom,
                                        IsActiveUntil = transactionCoupon.CouponMask.IsActiveUntil,
                                        IsUnique = transactionCoupon.CouponMask.IsUnique,
                                        NumberOfTimesUsed = 1,
                                        PaymentMethod = transactionCoupon.CouponMask.PaymentMethod == null ? null : documentHeader.Session.GetObjectByKey<PaymentMethod>(transactionCoupon.CouponMask.PaymentMethod.Oid)
                                    };
                                    coupon.TransactionCoupons.Add(transactionCoupon);
                                    coupon.Save();
                                }
                            }
                            documentHeader.CouponsHaveBeenUpdatedOnMaster = true;
                            break;

                        case eApplicationInstance.STORE_CONTROLER:
                            foreach (TransactionCoupon transactionCoupon in documentHeader.TransactionCoupons)
                            {
                                Coupon coupon = transactionCoupon.Coupon != null ?
                                                documentHeader.Session.GetObjectByKey<Coupon>(transactionCoupon.Coupon.Oid) ??
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.Coupon.Code)) :
                                                documentHeader.Session.FindObject<Coupon>(new BinaryOperator("Code", transactionCoupon.CouponCode));

                                if (coupon != null)
                                {
                                    coupon.NumberOfTimesUsed++;
                                    coupon.Save();
                                }
                                else
                                {
                                    GeneratedCoupon generatedCoupon = documentHeader.Session.FindObject<GeneratedCoupon>(new BinaryOperator("Code", transactionCoupon.CouponCode));
                                    if (generatedCoupon.Status != GeneratedCouponStatus.Used)
                                    {
                                        generatedCoupon.Status = GeneratedCouponStatus.Used;
                                        generatedCoupon.Save();
                                    }
                                }
                            }
                            documentHeader.CouponsHaveBeenUpdatedOnStoreController = true;
                            break;
                    }
                }
            }
        }

        public static void ApplyPricesFromPOS(DocumentHeader documentHeader, eApplicationInstance applicationInstance)
        {
            if ((applicationInstance == eApplicationInstance.DUAL_MODE || applicationInstance == eApplicationInstance.RETAIL)
                && (documentHeader.Owner.OwnerApplicationSettings.POSCanChangePrices || documentHeader.Owner.OwnerApplicationSettings.POSCanSetPrices)
                )
            {
                foreach (DocumentDetail documentDetail in documentHeader.DocumentDetails.OrderBy(detail => detail.UpdatedOnTicks)
                                                                                .Where(detail => String.IsNullOrEmpty(detail.POSGeneratedPriceCatalogDetailSerialized) == false
                                                                                       && detail.IsPOSGeneratedPriceCatalogDetailApplied == false
                                                                                       ))
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        JObject jsonPcd = JObject.Parse(documentDetail.POSGeneratedPriceCatalogDetailSerialized);
                        string oid = jsonPcd.Property("Oid").Value.ToString();
                        PriceCatalogDetail priceCatalogDetail = uow.GetObjectByKey<PriceCatalogDetail>(Guid.Parse(oid));

                        if (priceCatalogDetail == null)
                        {
                            priceCatalogDetail = new PriceCatalogDetail(uow);
                        }

                        try
                        {
                            string error;
                            priceCatalogDetail.FromJson(documentDetail.POSGeneratedPriceCatalogDetailSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, true, out error);
                            priceCatalogDetail.UpdatedOnTicks = DateTime.Now.Ticks;
                            priceCatalogDetail.Save();
                            XpoHelper.CommitChanges(uow);
                            documentDetail.IsPOSGeneratedPriceCatalogDetailApplied = true;
                            documentDetail.Save();
                        }
                        catch (Exception exception)
                        {
                            string message = "ApplyPricesFromPOS  Error Creating/Updating PriceCatalogDetail" + exception.GetFullMessage() + Environment.NewLine + exception.StackTrace;
                            LogErrorMessage errorMessage = new LogErrorMessage()
                            {
                                Result = message,
                                Action = "DocumentHelper.ApplyPricesFromPOS method",
                                Controller = string.Empty,
                                Error = message
                            };
                            ITSLogHelper.Log(errorMessage);
                        }
                    }
                }
            }
        }

        public static void CreateCustomerFromPOSDocument(DocumentHeader documentHeader, eApplicationInstance applicationInstance)
        {
            if (applicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                if (string.IsNullOrWhiteSpace(documentHeader.DenormalizedCustomer) == false)
                {
                    InsertedCustomerViewModel customerCreatedOnPOS = JsonConvert.DeserializeObject<InsertedCustomerViewModel>(documentHeader.DenormalizedCustomer);
                    CriteriaOperator customerCriteria = RetailHelper.ApplyOwnerCriteria(
                        CriteriaOperator.Or(
                            new BinaryOperator("Trader.TaxCode", customerCreatedOnPOS.TaxCode),
                            new BinaryOperator("Code", customerCreatedOnPOS.Code)
                            ),
                        typeof(Customer), documentHeader.Owner);

                    Customer customer = documentHeader.Session.FindObject<Customer>(customerCriteria) ??
                        documentHeader.Session.FindObject<Customer>(PersistentCriteriaEvaluationBehavior.InTransaction, customerCriteria);

                    Address billingAddress = null;

                    if (customer == null)
                    {
                        Trader trader = documentHeader.Session.FindObject<Trader>(new BinaryOperator("TaxCode", customerCreatedOnPOS.TaxCode)) ??
                        documentHeader.Session.FindObject<Trader>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("TaxCode", customerCreatedOnPOS.TaxCode));

                        if (trader == null)
                        {
                            //Completely new Customer without supplier
                            trader = new Trader(documentHeader.Session)
                            {
                                TaxCode = customerCreatedOnPOS.TaxCode,
                                FirstName = customerCreatedOnPOS.FirstName,
                                LastName = customerCreatedOnPOS.LastName,
                                TaxOfficeLookUpOid = customerCreatedOnPOS.TaxOfficeLookup
                            };
                            customer = new Customer(documentHeader.Session)
                            {
                                Code = customerCreatedOnPOS.Code,
                                CompanyName = customerCreatedOnPOS.CompanyName,
                                Trader = trader,
                                Owner = documentHeader.Owner,
                                CardID = customerCreatedOnPOS.CardID
                            };
                            billingAddress = new Address(documentHeader.Session)
                            {
                                Oid = customerCreatedOnPOS.AddressOid,
                                Trader = trader,
                                Street = customerCreatedOnPOS.Street,
                                Profession = customerCreatedOnPOS.AddressProfession,
                                City = customerCreatedOnPOS.City,
                                PostCode = customerCreatedOnPOS.PostalCode,
                                DefaultPhoneOid = String.IsNullOrWhiteSpace(customerCreatedOnPOS.Phone) ?
                                    Guid.Empty : (new Phone(documentHeader.Session) { Number = customerCreatedOnPOS.Phone }).Oid,
                                ThirdPartNum = customerCreatedOnPOS.ThirdPartNum
                            };
                            if (billingAddress.DefaultPhoneOid.HasValue && billingAddress.DefaultPhoneOid.Value != Guid.Empty)
                            {
                                billingAddress.DefaultPhone.Address = billingAddress;
                            }
                            customer.DefaultAddress = billingAddress;

                            //Add customer to Default Category

                            if (documentHeader.DocumentType.DocTypeCustomerCategoryMode == eDocTypeCustomerCategory.INCLUDE_CUSTOMER_CATEGORIES)
                            {
                                DocTypeCustomerCategory docTypeCustomerCategory = documentHeader.DocumentType.DocTypeCustomerCategories.FirstOrDefault(docTypecat => docTypecat.DefaultCategoryForNewCustomer);

                                if (docTypeCustomerCategory != null)
                                {
                                    CustomerCategory custCategory = docTypeCustomerCategory.CustomerCategory;
                                    CustomerAnalyticTree custAnalyticTree = new CustomerAnalyticTree(documentHeader.Session);
                                    custAnalyticTree.Object = customer;
                                    custAnalyticTree.Node = custCategory;
                                    custAnalyticTree.Root = custCategory.GetRoot(documentHeader.Session);
                                    custAnalyticTree.Save();
                                }
                            }
                        }
                        else
                        {
                            if (trader.Customers.Count == 0)
                            {
                                //Trader exist but no customer exist
                                customer = new Customer(documentHeader.Session)
                                {
                                    Code = customerCreatedOnPOS.Code,
                                    CompanyName = customerCreatedOnPOS.CompanyName,
                                    Trader = trader,
                                    Owner = documentHeader.Owner
                                };

                                billingAddress = documentHeader.Session.GetObjectByKey<Address>(customerCreatedOnPOS.AddressOid);
                                if (billingAddress == null)
                                {
                                    billingAddress = new Address(documentHeader.Session)
                                    {
                                        Oid = customerCreatedOnPOS.AddressOid,
                                        Trader = trader,
                                        Profession = customerCreatedOnPOS.AddressProfession,
                                        Street = customerCreatedOnPOS.Street,
                                        City = customerCreatedOnPOS.City,
                                        PostCode = customerCreatedOnPOS.PostalCode,
                                        DefaultPhoneOid = String.IsNullOrWhiteSpace(customerCreatedOnPOS.Phone) ?
                                            Guid.Empty : (new Phone(documentHeader.Session) { Number = customerCreatedOnPOS.Phone }).Oid,
                                        ThirdPartNum = customerCreatedOnPOS.ThirdPartNum
                                    };
                                }
                                if (billingAddress.DefaultPhoneOid.HasValue && billingAddress.DefaultPhoneOid.Value != Guid.Empty)
                                {
                                    billingAddress.DefaultPhone.Address = billingAddress;
                                }

                                //Add customer to Default Category

                                if (documentHeader.DocumentType.DocTypeCustomerCategoryMode == eDocTypeCustomerCategory.INCLUDE_CUSTOMER_CATEGORIES)
                                {
                                    DocTypeCustomerCategory docTypeCustomerCategory = documentHeader.DocumentType.DocTypeCustomerCategories.FirstOrDefault(docTypecat => docTypecat.DefaultCategoryForNewCustomer);

                                    if (docTypeCustomerCategory != null)
                                    {
                                        CustomerCategory custCategory = docTypeCustomerCategory.CustomerCategory;
                                        CustomerAnalyticTree custAnalyticTree = new CustomerAnalyticTree(documentHeader.Session);
                                        custAnalyticTree.Object = customer;
                                        custAnalyticTree.Node = custCategory;
                                        custAnalyticTree.Root = custCategory.GetRoot(documentHeader.Session);
                                        custAnalyticTree.Save();
                                    }
                                }
                            }
                            else
                            {
                                customer = trader.Customers.FirstOrDefault();
                                billingAddress = documentHeader.Session.GetObjectByKey<Address>(customerCreatedOnPOS.AddressOid);
                                if (billingAddress == null)
                                {
                                    billingAddress = new Address(documentHeader.Session)
                                    {
                                        Oid = customerCreatedOnPOS.AddressOid,
                                        Trader = trader,
                                        Street = customerCreatedOnPOS.Street,
                                        Profession = customerCreatedOnPOS.AddressProfession,
                                        City = customerCreatedOnPOS.City,
                                        PostCode = customerCreatedOnPOS.PostalCode,
                                        DefaultPhoneOid = String.IsNullOrWhiteSpace(customerCreatedOnPOS.Phone) ?
                                                Guid.Empty : (new Phone(documentHeader.Session) { Number = customerCreatedOnPOS.Phone }).Oid,
                                        ThirdPartNum = customerCreatedOnPOS.ThirdPartNum
                                    };
                                }
                                if (billingAddress.DefaultPhoneOid.HasValue && billingAddress.DefaultPhoneOid.Value != Guid.Empty)
                                {
                                    billingAddress.DefaultPhone.Address = billingAddress;
                                }
                            }
                        }
                        documentHeader.Customer = customer;
                        documentHeader.CustomerCode = customer.Code;
                        documentHeader.CustomerName = customer.CompanyName;
                        documentHeader.BillingAddress = billingAddress;
                        documentHeader.AddressProfession = billingAddress.Profession;
                        documentHeader.ProcessedDenormalizedCustomer = documentHeader.DenormalizedCustomer;
                        documentHeader.DenormalizedCustomer = null;
                    }
                    else // Customer already exists but it was not known in POS // if (newCustomer.CustomerOid != Guid.Empty)
                    {
                        Trader trader = customer.Trader;

                        billingAddress = documentHeader.Session.GetObjectByKey<Address>(customerCreatedOnPOS.AddressOid);

                        if (billingAddress == null)
                        {
                            billingAddress = new Address(documentHeader.Session)
                            {
                                Oid = customerCreatedOnPOS.AddressOid,
                                Trader = trader,
                                Street = customerCreatedOnPOS.Street,
                                Profession = customerCreatedOnPOS.AddressProfession,
                                City = customerCreatedOnPOS.City,
                                PostCode = customerCreatedOnPOS.PostalCode,
                                DefaultPhoneOid = String.IsNullOrWhiteSpace(customerCreatedOnPOS.Phone) ?
                                    Guid.Empty : (new Phone(documentHeader.Session) { Number = customerCreatedOnPOS.Phone }).Oid,
                                ThirdPartNum = customerCreatedOnPOS.ThirdPartNum
                            };
                            if (billingAddress.DefaultPhoneOid.HasValue && billingAddress.DefaultPhoneOid.Value != Guid.Empty)
                            {
                                billingAddress.DefaultPhone.Address = billingAddress;
                            }
                        }

                        documentHeader.Customer = customer;
                        documentHeader.BillingAddress = billingAddress;
                        documentHeader.ProcessedDenormalizedCustomer = documentHeader.DenormalizedCustomer;
                        documentHeader.DenormalizedCustomer = null;
                        documentHeader.AddressProfession = billingAddress.Profession;

                        //just to trigger resend customer
                        documentHeader.Customer.UpdatedOnTicks = DateTime.Now.Ticks;
                        documentHeader.Customer.Save();
                    }
                }
            }
        }

        public static List<Address> GetAddresses(string objectOid, Session session, eDivision division)
        {
            List<Address> result = new List<Address>();
            Guid objectGuid = Guid.Empty;
            if (Guid.TryParse(objectOid, out objectGuid))
            {
                switch (division)
                {
                    case eDivision.Sales:
                        Customer customer = session.GetObjectByKey<Customer>(objectGuid);
                        if (customer != null)
                        {
                            result.AddRange(customer.Trader.Addresses);
                        }
                        break;

                    case eDivision.Purchase:
                        SupplierNew supplier = session.GetObjectByKey<SupplierNew>(objectGuid);
                        if (supplier != null)
                        {
                            result.AddRange(supplier.Trader.Addresses);
                        }
                        break;

                    case eDivision.Store:
                        Store store = session.GetObjectByKey<Store>(objectGuid);
                        if (store != null)
                        {
                            result.Add(store.Address);
                        }
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="priceCatalogDetail"></param>
        /// <param name="qty"></param>
        /// <param name="vatFactor"></param>
        /// <returns>A localised error if any, otherwise string.Empty</returns>
        public static string RecalculateTemporaryDocumentDetail(DocumentHeader currentDocumentHeader, DocumentDetail currentDocumentDetail, decimal qty, VatFactor vatFactor, string customMeasurementUnit, string customDescription, string remarks, string isPercentage, string userDiscount = null, PriceCatalogDetail priceCatalogDetail = null)
        {
            try
            {
                decimal moneyValue = .0m;
                bool IsPercentage = true;
                List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
                Boolean.TryParse(isPercentage, out IsPercentage);
                Item item;
                Barcode barcode;

                if (priceCatalogDetail != null)
                {
                    item = priceCatalogDetail.Item;
                    barcode = priceCatalogDetail.Barcode;
                    if (priceCatalogDetail.Discount > 0 && item.IsTax == false && item.DoesNotAllowDiscount == false)
                    {
                        discounts.Add(DiscountHelper.CreatePriceCatalogDetailDiscount((UnitOfWork)priceCatalogDetail.Session, priceCatalogDetail.Discount));
                    }
                }
                else
                {
                    item = currentDocumentDetail.Item;
                    barcode = currentDocumentDetail.Barcode;
                }

                if (Decimal.TryParse(userDiscount, out moneyValue) == false)
                {
                    moneyValue = 0.0m;
                }

                moneyValue /= QUANTITY_MULTIPLIER;
                if (IsPercentage)
                {
                    moneyValue /= 100;
                }

                if (moneyValue >= 0)
                {
                    if (IsPercentage && moneyValue > 1)
                    {
                        return Resources.InvalidValue;
                    }
                    else if (IsPercentage == false && moneyValue > currentDocumentDetail.GrossTotalBeforeDiscount - discounts.Sum(x => x.Value))
                    {
                        return Resources.InvalidValue;
                    }

                    DocumentDetailDiscount customDiscount = new DocumentDetailDiscount(currentDocumentDetail.Session)
                    {
                        DiscountSource = eDiscountSource.CUSTOM,
                        DiscountType = IsPercentage ? eDiscountType.PERCENTAGE : eDiscountType.VALUE,
                        Percentage = IsPercentage ? moneyValue : 0,
                        Value = IsPercentage ? 0 : moneyValue
                    };
                    if (item.IsTax == false && item.DoesNotAllowDiscount == false)
                    {
                        discounts.Add(customDiscount);
                    }

                }

                if (currentDocumentDetail.CustomUnitPrice == 0 && currentDocumentHeader.DocumentType != null && currentDocumentHeader.DocumentType.AllowItemZeroPrices == false)
                {
                    return Resources.InvalidValue;
                }

                currentDocumentDetail = ComputeDocumentLine(ref currentDocumentHeader, item, barcode, qty, false,
                                                                                       currentDocumentDetail.CustomUnitPrice, currentDocumentDetail.HasCustomPrice, "", discounts,
                                                                                       !String.IsNullOrEmpty(customMeasurementUnit),
                                                                                       customMeasurementUnit,
                                                                                       currentDocumentDetail, customVatFactor: vatFactor);

                currentDocumentDetail.Remarks = remarks;
                if (String.IsNullOrEmpty(customDescription))
                {
                    customDescription = currentDocumentDetail.CustomDescription;
                    if (String.IsNullOrEmpty(customDescription))
                    {
                        customDescription = currentDocumentDetail.Item.Description;
                    }
                }

                currentDocumentDetail.CustomDescription = customDescription;
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.GetFullMessage();
            }
        }

        public static void SetFinancialDocumentDetail(DocumentHeader documentHeader)
        {
            DocumentDetail documentDetail = null;
            if (documentHeader.DocumentDetails.Count == 0)
            {
                documentDetail = new DocumentDetail(documentHeader.Session);
                documentDetail.DocumentHeader = documentHeader;
            }
            else if (documentHeader.DocumentDetails.Count == 1)
            {
                documentDetail = documentHeader.DocumentDetails.First();
            }
            else
            {
                throw new Exception("Document must have one and only one DocumentDetail");
            }

            decimal totalAmount = documentHeader.DocumentPayments.Sum(payment => payment.Amount);
            documentHeader.NetTotal = totalAmount;
            documentHeader.NetTotalBeforeDiscount = totalAmount;
            documentHeader.GrossTotal = totalAmount;
            documentHeader.GrossTotalBeforeDiscount = totalAmount;

            //TODO
            //documentDetail.Item =
            //documentDetail.CustomDescription =
            //documentDetail.Barcode =
            documentDetail.SpecialItem = documentHeader.DocumentType.SpecialItem;
            documentDetail.Qty = 1;
            documentDetail.NetTotal = documentHeader.NetTotal;
            documentDetail.NetTotalBeforeDiscount = documentHeader.NetTotalBeforeDiscount;
            documentDetail.GrossTotal = documentHeader.GrossTotal;
            documentDetail.GrossTotalBeforeDiscount = documentHeader.GrossTotalBeforeDocumentDiscount;

            documentDetail.Save();
        }

        private static void RecalculateDocumentCostsFinancial(ref DocumentHeader documentHeader, bool recompute_document_lines, bool ignoreOwnerSettings)
        {
            CriteriaOperator documentDetaisFilterToRestore = documentHeader.DocumentDetails.Filter;
            documentHeader.DocumentDetails.Filter = null;
            documentHeader.GrossTotal = documentHeader.DocumentPayments.Sum(documentPayment => documentPayment.Amount);
            documentHeader.NetTotal = documentHeader.GrossTotal;
            documentHeader.TotalVatAmount = 0;
            documentHeader.GrossTotalBeforeDiscount = documentHeader.GrossTotal;
            documentHeader.NetTotalBeforeDiscount = documentHeader.GrossTotal;
            documentHeader.TotalVatAmountBeforeDiscount = 0;
            documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.GrossTotal;
            documentHeader.TotalDiscountAmount = 0;
            documentHeader.TotalQty = 1;
            documentHeader.DocumentPoints = 0;
            documentHeader.TotalPoints = 0;
            documentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
        }


        public static DocumentHeader CreateDocumentHeaderForCashier(UnitOfWork uow, List<ItemSales> itemSales, DocumentType documentType,
                                          DocumentSeries documentSeries, DocumentStatus documentStatus,
                                          Customer customer, Store store,
                                          User user, List<CashRegisterPaymentMethods> paymentMethods, POSDevice cashRegisterDevice
                                         )
        {
            try
            {
                DocumentHeader documentHeader = new DocumentHeader(uow);
                documentHeader.Division = documentType.Division.Section;
                documentHeader.DocumentType = uow.GetObjectByKey<DocumentType>(documentType.Oid);
                documentHeader.Customer = uow.GetObjectByKey<Customer>(customer.Oid);
                documentHeader.Store = uow.GetObjectByKey<Store>(store.Oid);
                documentHeader.Status = uow.GetObjectByKey<DocumentStatus>(documentStatus.Oid);
                documentHeader.DocumentSeries = uow.GetObjectByKey<DocumentSeries>(documentSeries.Oid);
                documentHeader.FinalizedDate = DateTime.Now;
                documentHeader.PriceCatalogPolicy = uow.GetObjectByKey<PriceCatalogPolicy>(store.DefaultPriceCatalogPolicy.Oid);
                documentHeader.StoreCode = store.Code;
                documentHeader.CreatedBy = documentHeader.UpdatedBy = uow.GetObjectByKey<User>(user.Oid);

                foreach (ItemSales sale in itemSales)
                {
                    Item item = uow.FindObject<Item>(new BinaryOperator("CashierDeviceIndex", sale.deviceIndex));
                    ItemBarcode itemBarcode = item.ItemBarcodes.FirstOrDefault(itBarcode => itBarcode.Barcode.Code == sale.Code.Trim());
                    MapVatFactor mapVactor = uow.FindObject<MapVatFactor>(new BinaryOperator("DeviceVatLevel", sale.VatCode.ToString()));
                    DocumentDetail documentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader, item, itemBarcode.Barcode, sale.SoldQTY, false, sale.price, true, sale.Description, null, customVatFactor: mapVactor.VatFactor);
                    DocumentHelper.AddItem(ref documentHeader, documentDetail);
                    documentDetail.Save();
                }
                List<DocumentPayment> documentPaymentList = new List<DocumentPayment>();

                foreach (var currentPayment in paymentMethods.Where(x => x.DailySum > 0))
                {
                    DocumentPayment documentPayment = new DocumentPayment(uow);
                    PaymentMethod paymentMethod = uow.FindObject<PaymentMethod>(new BinaryOperator("Code", currentPayment.Code));
                    if (paymentMethod == null)
                    {
                        throw new Exception(String.Format(ResourcesLib.Resources.CantFindThisPaymentCodePleaseCheckCashierRegisterPaymentSettings, currentPayment.Code));
                    }
                    documentPayment.DocumentHeader = documentHeader;
                    documentPayment.DocumentHeaderOid = documentHeader.Oid;
                    documentPayment.PaymentMethodCode = currentPayment.Code;
                    documentPayment.PaymentMethod = paymentMethod;
                    documentPayment.Amount = currentPayment.DailySum;
                    documentPaymentList.Add(documentPayment);
                }
                decimal saleSum = itemSales.Sum(x => x.TotalSalesAmount);
                decimal paymentMethodSum = paymentMethods.Sum(x => x.DailySum);
                decimal diff = saleSum - paymentMethodSum;
                if (diff != 0)
                {
                    DocumentPayment documentPayment = new DocumentPayment(uow);
                    PaymentMethod paymentMethod = uow.FindObject<PaymentMethod>(new BinaryOperator("Oid", cashRegisterDevice.PaymentMethod.Oid));
                    documentPayment.DocumentHeader = documentHeader;
                    documentPayment.DocumentHeaderOid = documentHeader.Oid;
                    documentPayment.PaymentMethodCode = cashRegisterDevice.PaymentMethod.Code;
                    documentPayment.PaymentMethod = paymentMethod;
                    documentPayment.Amount = diff;
                    documentPaymentList.Add(documentPayment);
                }

                documentHeader.DocumentPayments.AddRange(documentPaymentList);

                return documentHeader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        /// Removes the discounts with source DEFAULT_DOCUMENT_DISCOUNT from document lines and recalculates DocumentCost
        /// </summary>
        /// <param name="header">The document</param>        
        public static void RemoveDefaultDocumentDetailsDiscounts(ref DocumentHeader header)
        {
            foreach (DocumentDetail detail in header.DocumentDetails)
            {
                List<DocumentDetailDiscount> listToRemove = new List<DocumentDetailDiscount>();
                foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts)
                {
                    if (discount.DiscountSource == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT)
                    {
                        listToRemove.Add(discount);
                    }
                }
                foreach (DocumentDetailDiscount discount in listToRemove)
                {
                    detail.DocumentDetailDiscounts.Remove(discount);
                }
            }
            RecalculateDocumentCosts(ref header, true, false);
        }

        /// <summary>
        /// Removes the CUSTOMER_DISCOUNT from document lines and recalculates DocumentCost
        /// </summary>
        /// <param name="header">The document</param> 
        public static void RemoveCustomerDetailsDiscounts(ref DocumentHeader header)
        {
            foreach (DocumentDetail detail in header.DocumentDetails)
            {
                List<DocumentDetailDiscount> listToRemove = new List<DocumentDetailDiscount>();
                foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts)
                {
                    if (discount.DiscountSource == eDiscountSource.CUSTOMER)
                    {
                        listToRemove.Add(discount);
                    }
                }
                foreach (DocumentDetailDiscount discount in listToRemove)
                {
                    detail.DocumentDetailDiscounts.Remove(discount);
                }
            }
            RecalculateDocumentCosts(ref header, true, false);
        }

        /// <summary>
        /// Distributes the DEFAULT_DOCUMENT_DISCOUNT at lines eligible to take a discount, recalculates DocumentCost
        /// </summary>
        /// <param name="header">The document</param> 
        /// <param name="percentagePerLine">The percentage of discount</param> 
        public static void AddDefaultDocumentDetailsDiscounts(ref DocumentHeader header, decimal percentagePerLine)
        {
            foreach (DocumentDetail detail in header.DocumentDetails)
            {
                if (detail.DoesNotAllowDiscount == false && detail.IsTax == false && detail.IsCanceled == false && detail.IsReturn == false)
                {
                    DocumentDetailDiscount dicount = new DocumentDetailDiscount(header.Session);
                    dicount.Value = detail.GrossTotal * percentagePerLine;
                    dicount.DiscountSource = eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT;
                    dicount.DiscountType = eDiscountType.VALUE;
                    detail.DocumentDetailDiscounts.Add(dicount);
                }

                RecalculateDocumentCosts(ref header, true, false);
            }
        }

        /// <summary>
        /// Distributes the CUSTOMER_DISCOUNT at lines eligible to take a discount, recalculates DocumentCost
        /// </summary>
        /// <param name="header">The document</param> 
        /// <param name="percentagePerLine">The percentage of discount</param> 
        public static void AddCustomerDetailsDiscounts(ref DocumentHeader header, decimal percentagePerLine)
        {
            foreach (DocumentDetail detail in header.DocumentDetails)
            {
                if (detail.DoesNotAllowDiscount == false && detail.IsTax == false && detail.IsCanceled == false && detail.IsReturn == false)
                {
                    DocumentDetailDiscount dicount = new DocumentDetailDiscount(header.Session);
                    dicount.Value = detail.GrossTotal * percentagePerLine;
                    dicount.DiscountSource = eDiscountSource.CUSTOMER;
                    dicount.DiscountType = eDiscountType.VALUE;
                    detail.DocumentDetailDiscounts.Add(dicount);
                }

                RecalculateDocumentCosts(ref header, true, false);
            }
        }


        /// <summary>
        /// Returns the total amount of DEFAULT_DOCUMENT_DISCOUNT that is already applied to a document Line     
        /// </summary>
        /// <param name="detail">The document line</param>
        public static decimal GetDefaultDocumentDetailDiscountOnly(DocumentDetail detail)
        {
            decimal discountAmount = 0;
            foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts)
            {
                if (discount.DiscountSource == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT)
                {
                    discountAmount += discount.Value;
                }
            }
            return discountAmount;
        }

        /// <summary>
        /// Returns the total amount of CUSTOMER_DISCOUNT that is already applied to a document Line    
        /// </summary>
        /// <param name="detail">The document line</param>
        public static decimal GetCustomerDetailDiscountOnly(DocumentDetail detail)
        {
            decimal discountAmount = 0;
            foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts)
            {
                if (discount.DiscountSource == eDiscountSource.CUSTOMER)
                {
                    discountAmount += discount.Value;
                }
            }
            return discountAmount;
        }

        /// <summary>
        /// Return the gross total from document lines as CUSTOMER_DISCOUNT and DEFAULT_DOCUMENT_DISCOUNT was not applied
        /// </summary>
        /// <param name="header">The document</param> 
        public static decimal GetDocumentDetailsGrossTotalWithoutDocumentAndCustomerDiscount(DocumentHeader header)
        {
            IEnumerable<DocumentDetail> validDetails = header.DocumentDetails.Where(documentDetail => documentDetail.IsCanceled == false && documentDetail.IsReturn == false && documentDetail.IsTax == false && documentDetail.DoesNotAllowDiscount == false);
            decimal defaultDetaislDiscountAmount = 0;
            decimal customerDetailsDiscountAmount = 0;
            foreach (DocumentDetail detail in validDetails)
            {
                defaultDetaislDiscountAmount += GetDefaultDocumentDetailDiscountOnly(detail);
                customerDetailsDiscountAmount += GetCustomerDetailDiscountOnly(detail);
            }
            return validDetails.Sum(docDet => docDet.GrossTotal) + defaultDetaislDiscountAmount + customerDetailsDiscountAmount;
        }

        /// <summary>
        /// Return the gross total from document lines as DEFAULT_DOCUMENT_DISCOUNT was not applied
        /// </summary>
        /// <param name="header">The document</param> 
        public static decimal GetDocumentDetailsGrossTotalWithoutDocumentDiscount(DocumentHeader header)
        {
            IEnumerable<DocumentDetail> validDetails = header.DocumentDetails.Where(documentDetail => documentDetail.IsCanceled == false && documentDetail.IsReturn == false && documentDetail.IsTax == false && documentDetail.DoesNotAllowDiscount == false);
            decimal defaultDetaislDiscountAmount = 0;
            foreach (DocumentDetail detail in validDetails)
            {
                defaultDetaislDiscountAmount += GetDefaultDocumentDetailDiscountOnly(detail);
            }
            return validDetails.Sum(docDet => docDet.GrossTotal) + defaultDetaislDiscountAmount;
        }


        /// <summary>
        /// Calculates the discount percentage of DEFAULT_DOCUMENT_DISCOUNT that should be applied per line
        /// </summary>
        /// <param name="header">The document</param> 
        public static void ApplyDefaultDocumentDiscount(ref DocumentHeader header, decimal discount)
        {
            if (header.DefaultDocumentDiscount >= 0 && header.Division == eDivision.Sales)
            {
                decimal validLinesGrossTotalBeforeDefaultDocumentDiscount = GetDocumentDetailsGrossTotalWithoutDocumentDiscount(header);
                if (validLinesGrossTotalBeforeDefaultDocumentDiscount > 0)
                {
                    decimal desiredDiscountAmount = validLinesGrossTotalBeforeDefaultDocumentDiscount * discount;
                    decimal percentagePerLine = desiredDiscountAmount / validLinesGrossTotalBeforeDefaultDocumentDiscount;
                    RemoveDefaultDocumentDetailsDiscounts(ref header);
                    AddDefaultDocumentDetailsDiscounts(ref header, percentagePerLine);
                }
            }
        }

        /// <summary>
        /// Calculates the discount percentage of CUSTOMER_DISCOUNT that should be applied per line
        /// </summary>
        /// <param name="header">The document</param> 
        public static void ApplyCustomerDiscount(ref DocumentHeader header, decimal discount)
        {
            if (header.Customer != null)
            {
                if (header.Customer.Oid != Guid.Empty && header.DefaultCustomerDiscount >= 0)
                {
                    decimal validLinesGrossTotalBeforeDefaultDocumentDiscount = GetDocumentDetailsGrossTotalWithoutDocumentAndCustomerDiscount(header);

                    if (validLinesGrossTotalBeforeDefaultDocumentDiscount > 0)
                    {
                        decimal desiredDiscountAmount = validLinesGrossTotalBeforeDefaultDocumentDiscount * discount;
                        decimal percentagePerLine = desiredDiscountAmount / validLinesGrossTotalBeforeDefaultDocumentDiscount;
                        RemoveCustomerDetailsDiscounts(ref header);
                        AddCustomerDetailsDiscounts(ref header, percentagePerLine);
                    }
                }
            }

        }

        /// <summary>
        /// Applies CUSTOMER_DISCOUNT & DEFAULT_DOCUMENT_DISCOUNT on document
        /// </summary>
        /// <param name="header">The document</param> 
        /// <param name="fromDesktopClient"> true for Office Manager</param> 
        public static void ApplyDiscountsOnDocumentTotal(ref DocumentHeader header)
        {
            if (header.Division == eDivision.Sales)
            {
                decimal defaultDocumentDiscount = header.DefaultDocumentDiscount;
                decimal defaultCustomerDiscount = header.DefaultCustomerDiscount;
                if (fromDesktopClient == false)
                {
                    defaultDocumentDiscount = header.DefaultDocumentDiscount / 100;
                    defaultCustomerDiscount = header.DefaultCustomerDiscount / 100;
                }

                ApplyDefaultDocumentDiscount(ref header, defaultDocumentDiscount);
                ApplyCustomerDiscount(ref header, defaultCustomerDiscount);

            }

        }
    }
}
