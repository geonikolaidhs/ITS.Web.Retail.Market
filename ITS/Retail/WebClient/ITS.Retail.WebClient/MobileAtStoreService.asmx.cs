using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.Helpers;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml;


#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
namespace ITS.Retail.WebClient
{
#if _RETAIL_STORECONTROLLER
    public enum InactiveItemReason
    {
        INACTIVE,
        NOPRICE,
        NOT_FOUND
    }
#endif

    public struct InvalidItem
    {
        public Guid ItemOid;
        public string ItemCode;
        public string Barcode;
        public InactiveItemReason Reason;
    }

    /// <summary>
    /// Summary description for MobileAtStoreService
    /// </summary>
    [WebService(Namespace = "http://www.its.net.gr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MobileAtStoreService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<InvalidItem> ValidateOrderItems(string xmlString)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);
            List<InvalidItem> invalidItems = new List<InvalidItem>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Customer customer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomer.Oid);
                CompanyNew company = uow.GetObjectByKey<CompanyNew>(StoreControllerAppiSettings.Owner.Oid);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Document/DocumentDetails/DocumentDetail");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    string code = xmlNode.SelectSingleNode("Code").InnerText;
                    string padded_code = code;

                    if (StoreControllerAppiSettings.OwnerApplicationSettings.PadBarcodes)
                    {
                        if (String.IsNullOrWhiteSpace(code))
                        {
                            invalidItems.Add(new InvalidItem { ItemOid = Guid.Empty, ItemCode = code, Barcode = code, Reason = InactiveItemReason.NOT_FOUND });
                            continue;//break;
                        }
                        else if (code.Length <= StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength && string.IsNullOrEmpty(StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter) == false)
                        {
                            padded_code = code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength, StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter.First());
                        }
                    }

                    Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", padded_code));
                    Item item = (barcode == null) ? null : ItemHelper.GetItemOfSupplier(uow, barcode, company);
                    if (item == null)
                    {
                        padded_code = code;
                        if (StoreControllerAppiSettings.OwnerApplicationSettings.PadItemCodes)
                        {
                            if (String.IsNullOrWhiteSpace(code))
                            {
                                invalidItems.Add(new InvalidItem { ItemOid = Guid.Empty, ItemCode = code, Barcode = code, Reason = InactiveItemReason.NOT_FOUND });
                                continue;
                            }
                            else if (code.Length <= StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength && string.IsNullOrEmpty(StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodePaddingCharacter) == false)
                            {
                                padded_code = code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength, StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodePaddingCharacter.First());
                            }
                        }
                        item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", padded_code), new BinaryOperator("Owner.Oid", company.Oid)));
                        if (item == null)
                        {
                            invalidItems.Add(new InvalidItem { ItemOid = Guid.Empty, ItemCode = code, Barcode = code, Reason = InactiveItemReason.NOT_FOUND });
                            continue;
                        }
                        barcode = item.DefaultBarcode;
                    }
                    if (item.IsActive == false)
                    {
                        invalidItems.Add(new InvalidItem { ItemOid = item.Oid, ItemCode = item.Code, Barcode = barcode.Code, Reason = InactiveItemReason.INACTIVE });
                    }
                    else if (PriceCatalogHelper.GetUnitPrice(StoreControllerAppiSettings.CurrentStore, customer, barcode.Code) < 0)
                    {
                        invalidItems.Add(new InvalidItem { ItemOid = item.Oid, ItemCode = /*item.Code*/ code, Barcode = /*barcode.Code*/ code, Reason = InactiveItemReason.NOPRICE });
                    }
                }
            }
            return invalidItems;
        }

        [WebMethod]
        public string PostDocument(string data)
        {
            Guid? userId = null;
            const decimal QUANTITY_MULTIPLIER = 1000.0m;// Υπάρχει και το int DocumentHelper.QUANTITY_MULTIPLIER = 10000 !!!
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(data);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader head = new DocumentHeader(uow);
                    User createdBy = null;
                    if (!string.IsNullOrEmpty(XmlHelper.GetNodeValue(xmlDocument, "User")))
                    {
                        Guid userGuid;
                        Guid.TryParse(XmlHelper.GetNodeValue(xmlDocument, "User"), out userGuid);
                        createdBy = uow.GetObjectByKey<User>(userGuid);
                        head.CreatedBy = createdBy;
                        head.UpdatedBy = createdBy;
                        userId = createdBy == null ? null : (Guid?)createdBy.Oid;
                    }
                    eDivision division;
                    XmlNode documentXMLNode = xmlDocument.SelectSingleNode("Document");
                    Enum.TryParse<eDivision>(XmlHelper.GetNodeValue(documentXMLNode, "Division"), out division);
                    head.Division = division;
                    eDocumentType edocumentType;

                    if (Enum.TryParse<eDocumentType>(XmlHelper.GetNodeValue(documentXMLNode, "DocumentType"), out edocumentType) == false)
                    {
                        throw new Exception(ResourcesLib.Resources.PleaseSelectADocumentTypeForMobile);
                    }

                    head.DocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Type", edocumentType));
                    if (head.DocumentType == null)
                    {
                        throw new Exception(ResourcesLib.Resources.PleaseSelectADocumentTypeForMobile);
                    }

                    head.Division = head.DocumentType.Division.Section;
                    head.Store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStore.Oid);
                    if (edocumentType == eDocumentType.TAG)
                    {
                        head.Customer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomer.Oid);
                        head.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(head.Store, head.Customer);
                    }
                    else
                    {
                        string TraderTaxCode = XmlHelper.GetNodeValue(documentXMLNode, "TraderTaxCode");
                        string TraderCode = XmlHelper.GetNodeValue(documentXMLNode, "TraderCode");
                        if (string.IsNullOrEmpty(TraderTaxCode) && string.IsNullOrEmpty(TraderCode))
                        {
                            throw new Exception(string.Format(ResourcesLib.Resources.CouldNotFindTrader, TraderTaxCode, TraderCode));
                        }

                        if (head.DocumentType.Division.Section.Equals(eDivision.Purchase))
                        {
                            SupplierNew supplier = uow.FindObject<SupplierNew>(CriteriaOperator.Or(
                                new BinaryOperator("Trader.TaxCode", TraderTaxCode),
                                new BinaryOperator("Code", TraderCode)
                                ));
                            head.Supplier = supplier;
                            if (head.Supplier == null)
                            {
                                throw new Exception(string.Format(ResourcesLib.Resources.CouldNotFindTrader, TraderTaxCode, TraderCode));
                            }
                            string deliveryAddress = XmlHelper.GetNodeValue(xmlDocument, "deliveryAddress");
                            if (string.IsNullOrEmpty(deliveryAddress))
                            {
                                deliveryAddress = head.Supplier.DefaultAddress == null ? "" : head.Supplier.DefaultAddress.Description;
                            }
                            head.DeliveryAddress = deliveryAddress;
                        }
                        else
                        {
                            Customer supplier = uow.FindObject<Customer>(CriteriaOperator.And(
                                new BinaryOperator("Trader.TaxCode", TraderTaxCode),
                                new BinaryOperator("Code", TraderCode)
                                ));
                            head.Customer = supplier;
                            if (head.Customer == null)
                            {
                                head.Customer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomer.Oid);
                            }
                            head.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(head.Store, head.Customer);
                            string deliveryAddress = XmlHelper.GetNodeValue(xmlDocument, "deliveryAddress");
                            if (string.IsNullOrEmpty(deliveryAddress))
                            {
                                deliveryAddress = head.Customer.DefaultAddress == null ? "" : head.Customer.DefaultAddress.Description;
                            }
                            head.DeliveryAddress = deliveryAddress;
                        }
                    }

                    head.Remarks = "";



                    string finalizedDate = XmlHelper.GetNodeValue(xmlDocument, "finalizeDate");
                    DateTime FinalizedDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(finalizedDate))
                    {
                        try
                        {
                            long l = long.Parse(finalizedDate);
                            FinalizedDate = new DateTime(l);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    head.FinalizedDate = FinalizedDate;
                    try
                    {
                        head.Remarks = XmlHelper.GetNodeValue(xmlDocument, "comments");
                    }
                    catch (Exception e)
                    {
                        string errorMessage = RetailHelper.GetErrorMessage(e);
                    }

                    DocumentSeries docSeries = StoreHelper.StoreSeriesForDocumentType(head.Store, head.DocumentType).FirstOrDefault();

                    if (docSeries != null)
                    {
                        head.DocumentSeries = docSeries;
                    }
                    else
                    {
                        MvcApplication.WRMLogModule.Log(null,
                             "No Document Series has been set up for specified store. Customer:" + head.Customer.CompanyName + ", Store:" + head.Store != null ? head.Store.Name : "No store defined",
                             "MobileAtStoreService", "PostDocument", this.Context.Request.UserAgent, this.Context.Request.UserHostAddress, "",
                             userId, KernelLogLevel.Info);
                        NewXmlCreator a = new NewXmlCreator("1", "Δεν εχουν καθοριστεί σειρές παραστατικών");
                        a.Xmlclose();
                        return a.MyXml;
                    }


                    Guid headstatoid;
                    head.Status = null;
                    if (Guid.TryParse(XmlHelper.GetNodeValue(xmlDocument, "Status"), out headstatoid))
                    {
                        head.Status = uow.GetObjectByKey<DocumentStatus>(headstatoid);
                    }
                    if (head.Status == null)
                    {
                        head.Status = head.DocumentType.DefaultDocumentStatus;
                    }
                    if (head.Status == null)
                    {
                        head.Status = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
                    }
                    if (head.Status == null)
                    {
                        head.Status = uow.FindObject<DocumentStatus>(null);
                    }

                    CompanyNew company = head.Store.Owner;

                    XmlNodeList nodeList = xmlDocument.SelectSingleNode("Document").SelectSingleNode("DocumentDetails").SelectNodes("DocumentDetail");

                    foreach (XmlNode xmlNode in nodeList)
                    {
                        DocumentDetail line;
                        string code = XmlHelper.GetNodeValue(xmlNode, "Code");
                        string padded_code = code;

                        if (String.IsNullOrWhiteSpace(code))
                        {
                            break;
                        }
                        if (StoreControllerAppiSettings.OwnerApplicationSettings.PadBarcodes)
                        {
                            if (code.Length <= StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength && string.IsNullOrEmpty(StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter) == false)
                            {
                                padded_code = code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength, StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter.First());
                            }
                        }
                        Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", padded_code));
                        Item item = (barcode == null) ? null : ItemHelper.GetItemOfSupplier(uow, barcode, company);
                        if (item == null)
                        {
                            padded_code = code;
                            if (StoreControllerAppiSettings.OwnerApplicationSettings.PadItemCodes)
                            {
                                if (code.Length <= StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength && string.IsNullOrEmpty(StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodePaddingCharacter) == false)
                                {
                                    padded_code = code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength, StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodePaddingCharacter.First());
                                }
                            }
                            item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", padded_code),
                                                                             new BinaryOperator("Owner.Oid", company.Oid))
                                                        );
                            if (item != null && barcode == null)
                            {
                                barcode = item.DefaultBarcode;
                            }
                        }
                        decimal Qty = Convert.ToDecimal(XmlHelper.GetNodeValue(xmlNode, "Qty")) / QUANTITY_MULTIPLIER;
                        decimal discount = 0;
                        List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
                        try
                        {
                            decimal.TryParse(XmlHelper.GetNodeValue(xmlNode, "discount"), out discount);
                            discount /= QUANTITY_MULTIPLIER;
                            if (discount > 0)
                            {
                                DocumentDetailDiscount discountNonPerst = new DocumentDetailDiscount(uow)
                                {
                                    DiscountSource = eDiscountSource.CUSTOM,
                                    DiscountType = eDiscountType.PERCENTAGE,
                                    Percentage = discount,
                                    Priority = 1
                                };
                                discounts.Add(discountNonPerst);
                            }
                        }
                        catch (Exception ex)
                        {
                            //no discount found
                            string error = ex.GetFullMessage();
                        }
                        decimal unitPrice = -1;
                        try
                        {
                            decimal.TryParse(XmlHelper.GetNodeValue(xmlNode, "unitPrice"), out unitPrice);
                            unitPrice /= (10 * QUANTITY_MULTIPLIER);
                        }
                        catch
                        {
                            //No custom price found
                        }

                        bool addLinkedItem = head.DocumentType.Type != eDocumentType.TAG;
                        if (head.Division == eDivision.Sales)
                        {
                            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(head.Store, barcode.Code, head.Customer);
                            PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                            if (head.Customer.Owner.OwnerApplicationSettings.RecomputePrices)
                            {
                                line = DocumentHelper.ComputeDocumentLine(ref head, item, barcode, Qty, false, -1, false, "", discounts);
                            }
                            else
                            {
                                bool foundCustomPrice = unitPrice > 0;
                                if (pcdt.Discount > 0)
                                {
                                    discounts.Add(DiscountHelper.CreatePriceCatalogDetailDiscount(uow, pcdt.Discount));
                                }

                                line = DocumentHelper.ComputeDocumentLine(ref head, item, barcode, Qty, false, unitPrice, foundCustomPrice, "", discounts);
                            }
                            head.BillingAddress = head.Customer.DefaultAddress ?? head.Customer.Trader.Addresses.FirstOrDefault();
                            DocumentHelper.AddItem(ref head, line);
                        }
                        else
                        {
                            DocumentDetail documentDetail = DocumentHelper.ComputeDocumentLine(ref head, item, barcode, Qty, false, 0, true, null, null);
                            DocumentHelper.AddItem(ref head, documentDetail);
                        }

                    }
                    if (head.DocumentType.MergedSameDocumentLines)
                    {
                        DocumentHelper.MergeDocumentLines(ref head);
                    }
                    head.Save();
                    XpoHelper.CommitChanges(uow);
                    MvcApplication.WRMLogModule.Log(null,
                             "Document has been successfully created",
                             "MobileAtStoreService", "PostDocument", this.Context.Request.UserAgent, this.Context.Request.UserHostAddress, "",
                             userId, KernelLogLevel.Info);
                    return "1";
                }
            }
            catch (Exception e)
            {

                MvcApplication.WRMLogModule.Log(e, "Post Document Error", "MobileAtStoreService", "PostDocument",
                    this.Context.Request.UserAgent, this.Context.Request.UserHostAddress, "",
                             userId, KernelLogLevel.Info);
                NewXmlCreator a = new NewXmlCreator("1", e.GetFullMessage() + Environment.NewLine + e.GetFullStackTrace());
                a.Xmlclose();
                return a.MyXml;
            }
        }

    }
}
#endif