using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model;
using ITS.Retail.Model.MobileInventory;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.Helpers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.RetailAtStoreModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Hosting;
using ITS.Retail.Model.NonPersistant;
using System.ServiceModel.Web;
using System.Net;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WRMMobileAtStore" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WRMMobileAtStore.svc or WRMMobileAtStore.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WRMMobileAtStore : IWRMMobileAtStore
    {
        /// <summary>
        /// Returns a completed product if an applicable has been found 
        /// when searched by <paramref name="code"/>, using supplier with code
        /// <paramref name="suppcode"/>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="suppcode"></param>
        /// <param name="compCode"></param>
        /// <param name="priceCatalogPolicy">Ignored parameter. In earlier version this was a selected PriceCatalog but now this has no point at all.</param>
        /// <returns></returns>
        public RetailAtStoreModel.Product GetProduct(int id, string ip, string code, string suppcode, string compCode, string priceCatalogPolicy, eDocumentType eDocumentType)
        {
            TerminalIsAuthorized(id, ip);

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                bool foundPriceCatalog = true;

                string codeSearch = code, barcodeSearch = code;
                if (StoreControllerAppiSettings.Owner.OwnerApplicationSettings.PadBarcodes)
                {
                    barcodeSearch = code.PadLeft(StoreControllerAppiSettings.Owner.OwnerApplicationSettings.BarcodeLength,
                        StoreControllerAppiSettings.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                }
                if (StoreControllerAppiSettings.Owner.OwnerApplicationSettings.PadItemCodes)
                {
                    codeSearch = code.PadLeft(StoreControllerAppiSettings.Owner.OwnerApplicationSettings.ItemCodeLength,
                        StoreControllerAppiSettings.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                }
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(StoreControllerAppiSettings.CurrentStore);
                PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, barcodeSearch);
                PriceCatalogDetail pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

                MeasurementUnit measurementUnit = null;
                if (pricecatalogdetail == null)
                {
                    priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, codeSearch);
                    pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }
                if (pricecatalogdetail == null)
                {
                    CompanyNew owner = uow.GetObjectByKey<CompanyNew>(StoreControllerAppiSettings.Owner.Oid);

                    List<BarcodeType> barcodeTypes = MaskHelper.GetMatchingMasks(code, null);

                    BarcodeParseResult barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode<BarcodeType>(barcodeTypes,
                                                                        code,
                                                                        StoreControllerAppiSettings.OwnerApplicationSettings.PadBarcodes,
                                                                        StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength,
                                                                        StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                        );
                    priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, barcodeParseResult.DecodedCode);
                    pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

                    if (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.CUSTOMER)
                    {
                        MobileAtStore.ObjectModel.Customer cust = GetCustomer(1, "", code);
                        if (cust == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new RetailAtStoreModel.Product()
                            {
                                AverageMonthSales = 0,
                                Barcode = code,
                                BasicSupplier = "",
                                BasicSupplierColor = 1,
                                CalculatedTotalPrice = 0,
                                ChainProduct = null,
                                ChainProductQuantity = 0,
                                Code = cust.Code,
                                Description = cust.Name,
                                IsActive = true,
                                IsActiveOnSupplier = true,
                                MeasurementUnitText = "",
                                Price = 0,
                                Quantity = 0,
                                BarcodeParsingResult = barcodeParseResult.BarcodeParsingResult,
                                WeightedDecodedBarcode = barcodeParseResult.DecodedCode,
                                SupportsDecimalQuantities = false
                            };
                        }
                    }
                    if (pricecatalogdetail == null)
                    {
                        return null;
                    }

                    Barcode weightedBarcode = pricecatalogdetail.Barcode;
                    if (weightedBarcode.Code != barcodeParseResult.DecodedCode)
                    {
                        ItemBarcode weightedItemBarcode = pricecatalogdetail.Item.ItemBarcodes.Where(itemBar => itemBar.Barcode.Code == barcodeParseResult.DecodedCode).FirstOrDefault();

                        if (weightedItemBarcode != null)
                        {
                            weightedBarcode = weightedItemBarcode.Barcode;
                        }
                    }

                    if (weightedBarcode == null)
                    {
                        return null;// This shouls be unreachable code
                    }

                    measurementUnit = weightedBarcode.MeasurementUnit(pricecatalogdetail.Item.Owner);
                    if (measurementUnit == null)
                    {
                        return null;
                    }

                    bool documentTypeSupportsBarcodeType = false;

                    CriteriaOperator documentTypeCriteria = CriteriaOperator.And(
                                                                                    new NotOperator(new NullOperator("DocumentTypeMapping")),
                                                                                    new BinaryOperator("DocumentTypeMapping.eDocumentType", eDocumentType)
                                                                                );
                    DocumentType documentType = pricecatalogdetail.Session.FindObject<DocumentType>(
                                                                            RetailHelper.ApplyOwnerCriteria(
                                                                                    documentTypeCriteria,
                                                                                    typeof(DocumentType),
                                                                                    StoreControllerAppiSettings.Owner
                                                                            )
                                                                           );
                    if (documentType == null)
                    {
                        documentTypeCriteria = new BinaryOperator("Type", eDocumentType);
                        documentType = pricecatalogdetail.Session.FindObject<DocumentType>(
                                                                            RetailHelper.ApplyOwnerCriteria(
                                                                                    documentTypeCriteria,
                                                                                    typeof(DocumentType),
                                                                                    StoreControllerAppiSettings.Owner
                                                                            )
                                                                           );
                    }

                    if (documentType != null)
                    {
                        BarcodeType measurementUnitBarcodeType = weightedBarcode.Type(StoreControllerAppiSettings.Owner);
                        if (measurementUnitBarcodeType != null)
                        {
                            DocumentTypeBarcodeType documentTypeBarcodeType = documentType.DocumentTypeBarcodeTypes.Where(docTypeBarType => docTypeBarType.BarcodeType.Oid == measurementUnitBarcodeType.Oid).FirstOrDefault();
                            if (documentTypeBarcodeType != null)
                            {
                                documentTypeSupportsBarcodeType = documentTypeBarcodeType.BarcodeType.IsWeighed;
                            }
                        }
                    }

                    if (!documentTypeSupportsBarcodeType && eDocumentType != eDocumentType.QUEUE_QR)
                    {
                        return null;
                    }


                    switch (barcodeParseResult.BarcodeParsingResult)
                    {
                        case BarcodeParsingResult.CUSTOMER:
                            return null;//This should be a product!!!
                        case BarcodeParsingResult.ITEM_CODE_QUANTITY:
                            return new RetailAtStoreModel.Product()
                            {
                                AverageMonthSales = 0,
                                Barcode = code,
                                BasicSupplier = pricecatalogdetail.Item.DefaultSupplier == null ? null : pricecatalogdetail.Item.DefaultSupplier.CompanyName,
                                BasicSupplierColor = 1,
                                CalculatedTotalPrice = barcodeParseResult.Quantity * pricecatalogdetail.RetailValue,
                                ChainProduct = null,
                                ChainProductQuantity = 0,
                                Code = pricecatalogdetail.Item.Code,
                                Description = pricecatalogdetail.Item.Name,
                                IsActive = pricecatalogdetail.Item.IsActive,
                                IsActiveOnSupplier = true,
                                MeasurementUnitText = measurementUnit.Description,
                                Price = pricecatalogdetail.RetailValue,
                                Quantity = barcodeParseResult.Quantity,
                                BarcodeParsingResult = barcodeParseResult.BarcodeParsingResult,
                                WeightedDecodedBarcode = barcodeParseResult.DecodedCode,
                                SupportsDecimalQuantities = measurementUnit.SupportDecimal
                            };
                        case BarcodeParsingResult.ITEM_CODE_VALUE:
                            return new RetailAtStoreModel.Product()
                            {
                                AverageMonthSales = 0,
                                Barcode = code,
                                BasicSupplier = pricecatalogdetail.Item.DefaultSupplier == null ? null : pricecatalogdetail.Item.DefaultSupplier.CompanyName,
                                BasicSupplierColor = 1,
                                CalculatedTotalPrice = barcodeParseResult.CodeValue,
                                ChainProduct = null,
                                ChainProductQuantity = 0,
                                Code = pricecatalogdetail.Item.Code,
                                Description = pricecatalogdetail.Item.Name,
                                IsActive = pricecatalogdetail.Item.IsActive,
                                IsActiveOnSupplier = true,
                                MeasurementUnitText = measurementUnit.Description,
                                Price = barcodeParseResult.CodeValue,
                                Quantity = barcodeParseResult.Quantity,
                                BarcodeParsingResult = barcodeParseResult.BarcodeParsingResult,
                                WeightedDecodedBarcode = barcodeParseResult.DecodedCode,
                                SupportsDecimalQuantities = measurementUnit.SupportDecimal
                            };
                        case BarcodeParsingResult.NONE:
                            return null;
                        case BarcodeParsingResult.NON_WEIGHTED_PRODUCT:
                            return null;//We should not be here in the first place
                        default:
                            return null;
                    }
                }


                if (measurementUnit == null)
                {
                    measurementUnit = pricecatalogdetail.Barcode.MeasurementUnit(pricecatalogdetail.Item.Owner);
                }
                if (measurementUnit == null)
                {
                    return null;
                }

                RetailAtStoreModel.Product product = new RetailAtStoreModel.Product()
                {
                    AverageMonthSales = 0,
                    Barcode = code,
                    BasicSupplier = pricecatalogdetail.Item.DefaultSupplier == null ? null : pricecatalogdetail.Item.DefaultSupplier.CompanyName,
                    BasicSupplierColor = 1,
                    CalculatedTotalPrice = pricecatalogdetail.RetailValue,
                    ChainProduct = null,
                    ChainProductQuantity = 0,
                    Code = pricecatalogdetail.Item.Code,
                    Description = pricecatalogdetail.Item.Name,
                    IsActive = pricecatalogdetail.Item.IsActive,
                    IsActiveOnSupplier = true, //pricecatalogdetail.Item.DefaultSupplier != null && pricecatalogdetail.Item.DefaultSupplier.IsActive,
                    MeasurementUnitText = measurementUnit.Description,
                    Price = pricecatalogdetail.RetailValue,
                    Quantity = 0,
                    BarcodeParsingResult = BarcodeParsingResult.NON_WEIGHTED_PRODUCT,
                    WeightedDecodedBarcode = code,
                    SupportsDecimalQuantities = measurementUnit.SupportDecimal
                };
                if (!foundPriceCatalog)
                {
                    product.ErrorMessage = "Price Catalog with code " + priceCatalogPolicy + " not found";
                }
                return product;
            }
        }

        public RetailAtStoreModel.Product GetReceiptProduct(int id, string ip, string code, string suppcode, string compCode, string priceCatalogPolicy, eDocumentType eDocumentType)
        {
            TerminalIsAuthorized(id, ip);
            return GetProduct(id, ip, code, suppcode, compCode, priceCatalogPolicy, eDocumentType);
        }

        public MobileAtStore.ObjectModel.Offer[] GetOffers(int id, string ip, string code, string compCode)
        {
            TerminalIsAuthorized(id, ip);

            List<MobileAtStore.ObjectModel.Offer> offers = new List<MobileAtStore.ObjectModel.Offer>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Item item = uow.FindObject<Item>(new BinaryOperator("Code", code));
                if (item != null)
                {

                    XPCollection<PromotionItemApplicationRule> promoItemRules = new XPCollection<PromotionItemApplicationRule>(uow,
                        CriteriaOperator.And(
                            new BinaryOperator("Item.Oid", item.Oid),
                            new BinaryOperator("PromotionApplicationRuleGroup.Promotion.StartDate", DateTime.Now, BinaryOperatorType.LessOrEqual),
                            new BinaryOperator("PromotionApplicationRuleGroup.Promotion.EndDate", DateTime.Now, BinaryOperatorType.GreaterOrEqual)
                        ));
                    if (promoItemRules.Count > 0)
                    {
                        promoItemRules.ToList().ForEach(x =>
                        {
                            offers.Add(new MobileAtStore.ObjectModel.Offer()
                            {
                                AA = offers.Count + 1,
                                Description = x.PromotionApplicationRuleGroup.Promotion.Description,
                                DescriptionProcessed = x.PromotionApplicationRuleGroup.Promotion.Description,
                                ValidForMembers = x.PromotionApplicationRuleGroup.Promotion.HasCustomerRule || x.PromotionApplicationRuleGroup.Promotion.HasCustomerGroupRule
                            });
                        });
                    }
                    if (item.ItemAnalyticTrees.Count > 0)
                    {
                        XPCollection<PromotionItemCategoryApplicationRule> promoItemCategoriesRules = new XPCollection<PromotionItemCategoryApplicationRule>(uow,
                            CriteriaOperator.And(
                                new InOperator("ItemCategory.Oid", item.ItemAnalyticTrees.Select(x => x.Node.Oid)),
                                new BinaryOperator("PromotionApplicationRuleGroup.Promotion.StartDate", DateTime.Now, BinaryOperatorType.LessOrEqual),
                                new BinaryOperator("PromotionApplicationRuleGroup.Promotion.EndDate", DateTime.Now, BinaryOperatorType.GreaterOrEqual)
                            ));
                        if (promoItemCategoriesRules.Count > 0)
                        {
                            promoItemCategoriesRules.ToList().ForEach(x =>
                            {
                                offers.Add(new MobileAtStore.ObjectModel.Offer()
                                {
                                    AA = offers.Count + 1,
                                    Description = x.PromotionApplicationRuleGroup.Promotion.Description,
                                    DescriptionProcessed = x.PromotionApplicationRuleGroup.Promotion.Description,
                                    ValidForMembers = x.PromotionApplicationRuleGroup.Promotion.HasCustomerRule || x.PromotionApplicationRuleGroup.Promotion.HasCustomerGroupRule
                                });
                            });
                        }
                    }
                }
            }
            return offers.ToArray();
        }

        public MobileAtStore.ObjectModel.Customer GetSupplier(int id, string ip, string searchString)
        {
            TerminalIsAuthorized(id, ip);

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                SupplierNew supplier = uow.FindObject<SupplierNew>(CriteriaOperator.Or(new BinaryOperator("Code", searchString), new BinaryOperator("Trader.TaxCode", searchString)));
                if (supplier == null)
                {
                    return null;
                }
                return new MobileAtStore.ObjectModel.Customer()
                {
                    AFM = supplier.Trader.TaxCode,
                    Code = supplier.Code,
                    Name = supplier.CompanyName
                };
            }
        }

        public MobileAtStore.ObjectModel.Customer GetCustomer(int id, string ip, string searchString)
        {
            TerminalIsAuthorized(id, ip);

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Customer customer = uow.FindObject<Customer>(CriteriaOperator.Or(new BinaryOperator("Code", searchString), new BinaryOperator("Trader.TaxCode", searchString)));
                if (customer == null)
                {
                    return null;
                }
                return new MobileAtStore.ObjectModel.Customer()
                {
                    AFM = customer.Trader.TaxCode,
                    Code = customer.Code,
                    Name = customer.CompanyName
                };
            }
        }



        public DateTime GetNow(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            return DateTime.Now;
        }

        public string GetWebServiceVersion(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            return this.GetType().Assembly.GetName().Version.ToString();
        }

        public int CountInvLines(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator criteria = CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEntry).FullName);
                return new XPCollection<MobileInventoryEntry>(uow, criteria).Count;
            }
        }

        public bool UpdateInvLine(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            return true;
        }

        public List<RetailAtStoreModel.InvLine> GetInvLines(int id, string ip, string taxCode)
        {
            TerminalIsAuthorized(id, ip);
            return GetInvLinesCommon<Model.MobileInventory.MobileInventoryEntry, RetailAtStoreModel.InvLine>(taxCode);
        }

        public RetailAtStoreModel.InvLine UploadInvLine(int id, string ip, RetailAtStoreModel.InvLine line, decimal quantity, bool add, string outputPath)
        {
            TerminalIsAuthorized(id, ip);
            return CommonUploadInvLine<Model.MobileInventory.MobileInventoryEntry, RetailAtStoreModel.InvLine>(line, quantity, add);
        }

        public List<RetailAtStoreModel.ESLInvLine> GetESLInvLines(int id, string ip, string taxCode, string invNumber)
        {
            TerminalIsAuthorized(id, ip);
            return GetInvLinesCommon<Model.MobileInventory.MobileInventoryEslEntry, RetailAtStoreModel.ESLInvLine>(taxCode, invNumber);
        }

        public RetailAtStoreModel.ESLInvLine UploadESLInvLine(int id, string ip, RetailAtStoreModel.ESLInvLine line, decimal quantity, bool add)
        {
            TerminalIsAuthorized(id, ip);
            return CommonUploadInvLine<Model.MobileInventory.MobileInventoryEslEntry, RetailAtStoreModel.ESLInvLine>(line, quantity, add);
        }

        public bool ProductCheckAdd(int id, string ip, string code, string compcode)
        {
            TerminalIsAuthorized(id, ip);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Item item = uow.FindObject<Item>(new BinaryOperator("Code", code));
                if (item == null)
                {
                    return false;
                }
                ItemCheck itemCheck = uow.FindObject<ItemCheck>(new BinaryOperator("Item.Oid", item.Oid));
                if (itemCheck == null)
                {
                    itemCheck = new ItemCheck(uow) { Item = item };
                    itemCheck.Save();
                    uow.CommitChanges();
                }
                return true;
            }
        }

        public bool ProductCheckRemove(int id, string ip, string code, string compcode)
        {
            TerminalIsAuthorized(id, ip);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Item item = uow.FindObject<Item>(new BinaryOperator("Code", code));
                if (item == null)
                {
                    return false;
                }
                ItemCheck itemCheck = uow.FindObject<ItemCheck>(new BinaryOperator("Item.Oid", item.Oid));
                if (itemCheck != null)
                {
                    itemCheck.Delete();
                    uow.CommitChanges();
                }
                return true;
            }
        }

        private string PadBarcode(string code)
        {
            if (StoreControllerAppiSettings.OwnerApplicationSettings.PadBarcodes && code.Length <=
                StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength)
            {
                return code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.BarcodeLength,
                    StoreControllerAppiSettings.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
            }
            return code;
        }


        private string PadItemCode(string code)
        {
            if (StoreControllerAppiSettings.OwnerApplicationSettings.PadItemCodes && code.Length <= StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength)
            {
                return code.PadLeft(StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodeLength, StoreControllerAppiSettings.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
            }
            return code;
        }

        private DocumentDetail CreateAndAddDocumentDetail(ref string errorMessage, UnitOfWork uow, ref DocumentHeader docheader, CompanyNew owner, RetailAtStoreModel.Line line)
        {
            Barcode barcode = null;
            Item item = null;
            DocumentDetail documentDetail;
            switch (line.BarcodeParsingResult)
            {
                case BarcodeParsingResult.CUSTOMER:
                    throw new NotSupportedException("ExportDocument()");
                case BarcodeParsingResult.ITEM_CODE_QUANTITY:
                    GetItemAndBarcode(uow, owner, line, out barcode, out item);
                    if (item == null || barcode == null)
                    {
                        errorMessage += string.Format("{0} ({1} - {2}){3}", Resources.ItemNotFound, line.ProdCode, line.ProdBarcode, Environment.NewLine);
                        return null;
                    }
                    switch (docheader.Division)
                    {
                        case Platform.Enumerations.eDivision.Sales:
                            //PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(docheader.Store, barcode.Code, docheader.Customer);
                            //PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                           item,
                                                                                           barcode,
                                                                                           line.Qty1,
                                                                                           false,
                                                                                           -1,
                                                                                           false,
                                                                                           "",
                                                                                           new List<DocumentDetailDiscount>()
                                                                                        );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                        default:
                        case Platform.Enumerations.eDivision.Purchase:
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                                   item,
                                                                                                   barcode,
                                                                                                   line.Qty1,
                                                                                                   false,
                                                                                                   0,
                                                                                                   true,
                                                                                                   null,
                                                                                                   null
                                                                                               );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                    }
                    break;
                case BarcodeParsingResult.ITEM_CODE_VALUE:
                    GetItemAndBarcode(uow, owner, line, out barcode, out item);
                    if (item == null || barcode == null)
                    {
                        errorMessage += string.Format("{0} ({1} - {2}){3}", Resources.ItemNotFound, line.ProdCode, line.ProdBarcode, Environment.NewLine);
                        return null;
                    }
                    switch (docheader.Division)
                    {
                        case Platform.Enumerations.eDivision.Sales:
                            //PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult =  PriceCatalogHelper.GetPriceCatalogDetail(docheader.Store, barcode.Code, docheader.Customer);
                            //PriceCatalogDetail pcdt = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                           item,
                                                                                           barcode,
                                                                                           line.Qty1,
                                                                                           false,
                                                                                           line.WeightedBarcodeValue,
                                                                                           true,
                                                                                           "",
                                                                                           new List<DocumentDetailDiscount>()
                                                                                        );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                        default:
                        case Platform.Enumerations.eDivision.Purchase:
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                                   item,
                                                                                                   barcode,
                                                                                                   line.Qty1,
                                                                                                   false,
                                                                                                   line.WeightedBarcodeValue,
                                                                                                   true,
                                                                                                   null,
                                                                                                   null
                                                                                               );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                    }
                    break;
                case BarcodeParsingResult.NONE:
                case BarcodeParsingResult.NON_WEIGHTED_PRODUCT:
                default:
                    CriteriaOperator itemCriteria = null;
                    CriteriaOperator barcodeCriteria = null;
                    if (String.IsNullOrWhiteSpace(line.ProdBarcode) == false)
                    {
                        barcodeCriteria = RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", PadBarcode(line.ProdBarcode)), typeof(Barcode), owner);
                        barcode = uow.FindObject<Barcode>(barcodeCriteria);
                    }

                    if (barcode == null)
                    {
                        barcodeCriteria = RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", PadBarcode(line.ProdCode)), typeof(Barcode), owner);
                        barcode = uow.FindObject<Barcode>(barcodeCriteria);
                    }

                    itemCriteria = RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", PadItemCode(line.ProdCode)), typeof(Item), owner);
                    item = (barcode == null)
                                ? uow.FindObject<Item>(itemCriteria)
                                : ItemHelper.GetItemOfSupplier(uow, barcode, owner);

                    if (item == null)
                    {
                        errorMessage += string.Format("{0} ({1} - {2}){3}", Resources.ItemNotFound, line.ProdCode, line.ProdBarcode, Environment.NewLine);
                        return null;
                    }
                    if (barcode == null)
                    {
                        barcode = item.DefaultBarcode;
                    }

                    ItemBarcode itemBarcode = item.ItemBarcodes.FirstOrDefault(itemBc => itemBc.Barcode.Oid == barcode.Oid);
                    decimal relationFactor = itemBarcode.RelationFactor > 0 ? (decimal)itemBarcode.RelationFactor : 1;
                    switch (docheader.Division)
                    {
                        case Platform.Enumerations.eDivision.Sales:
                            //PriceCatalogDetail pcdt = PriceCatalogHelper.GetPriceCatalogDetail(docheader.Store, barcode.Code, docheader.Customer);
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                            item,
                                                                                            barcode,
                                                                                            line.Qty1 * relationFactor,
                                                                                            false,
                                                                                            -1,
                                                                                            false,
                                                                                            "",
                                                                                            new List<DocumentDetailDiscount>()
                                                                                        );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                        default:
                        case Platform.Enumerations.eDivision.Purchase:
                            documentDetail = DocumentHelper.ComputeDocumentLine(ref docheader,
                                                                                                   item,
                                                                                                   barcode,
                                                                                                   line.Qty1 * relationFactor,
                                                                                                   false,
                                                                                                   0,
                                                                                                   true,
                                                                                                   null,
                                                                                                   null
                                                                                               );
                            DocumentHelper.AddItem(ref docheader, documentDetail);
                            break;
                    }

                    break;
            }
            return documentDetail;
        }
        public bool ExportDocument(int id, string ip, RetailAtStoreModel.Header header, out string errorMessage)
        {
            TerminalIsAuthorized(id, ip);
            errorMessage = string.Empty;
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader docheader = CreateDocumentHeader(header, uow);
                    CompanyNew owner = uow.GetObjectByKey<CompanyNew>(docheader.Owner.Oid);
                    bool addLinkedItem = header.DocType != Mobile.AuxilliaryClasses.eDocumentType.TAG;


                    foreach (RetailAtStoreModel.Line line in header.Details.Where(ln => ln.LinkedLine == Guid.Empty).OrderBy(ln => ln.Counter))
                    {
                        try
                        {
                            DocumentDetail documentDetail = CreateAndAddDocumentDetail(ref errorMessage, uow, ref docheader, owner, line);
                            foreach (RetailAtStoreModel.Line innerLine in header.Details.Where(ln => ln.LinkedLine == line.Oid).OrderBy(ln => ln.Counter))
                            {

                                try
                                {

                                    DocumentDetail innerDetail = CreateAndAddDocumentDetail(ref errorMessage, uow, ref docheader, owner, innerLine);
                                    innerDetail.LinkedLine = documentDetail.Oid;
                                }
                                catch (Exception innerException)
                                {
                                    string error = string.Format("{0} ({1} - {2}){3} {4}{3}",
                                                          Resources.CannotInsertItemWithCode,
                                                          innerLine.ProdCode,
                                                          innerLine.ProdBarcode,
                                                          Environment.NewLine,
                                                          innerException.GetFullMessage()
                                                        );
                                    errorMessage += error;
                                    MvcApplication.WRMLogModule.Log(innerException, error, KernelLogLevel.Info);
                                    continue;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            string error = string.Format("{0} ({1} - {2}){3} {4}{3}",
                                                          Resources.CannotInsertItemWithCode,
                                                          line.ProdCode,
                                                          line.ProdBarcode,
                                                          Environment.NewLine,
                                                          exception.GetFullMessage()
                                                        );
                            errorMessage += error;
                            MvcApplication.WRMLogModule.Log(exception, error, KernelLogLevel.Info);
                            continue;
                        }
                    }
                    if (docheader.DocumentType.MergedSameDocumentLines)
                    {
                        DocumentHelper.MergeDocumentLines(ref docheader);
                    }
                    if (docheader.DocumentDetails.Count > 0)
                    {
                        docheader.Save();
                        XpoHelper.CommitChanges(uow);
                    }
                    else
                    {
                        errorMessage += Resources.Document + ":" + Resources.NotSaved + Environment.NewLine + Resources.DocumentContainsNoItems;
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
            finally
            {
                if (String.IsNullOrEmpty(errorMessage) == false || String.IsNullOrWhiteSpace(errorMessage) == false)
                {
                    RetailHelper.Log(new Exception(errorMessage));
                }
            }
            return true;
        }

        private void GetItemAndBarcode(UnitOfWork uow, CompanyNew owner, Line line, out Barcode barcode, out Item item)
        {
            CriteriaOperator itemCriteria = new BinaryOperator("Code", PadItemCode(line.ProdCode));
            item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(itemCriteria, typeof(Item), owner));
            CriteriaOperator barcodeCriteria = RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", line.WeightedDecodedBarcode), typeof(Barcode), owner);
            barcode = uow.FindObject<Barcode>(barcodeCriteria);
        }

        // TODO -> Εξαγωγή απογραφής
        // Επιστρέφουμε αληθές για να μην σκάει 
        // αλλά θέλει δουλίτσα.
        public bool PerformInventoryExport(int id, string ip, out string errorMessage)
        {
            TerminalIsAuthorized(id, ip);
            try
            {
                errorMessage = string.Empty;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<Model.MobileInventory.MobileInventoryEntry> collection = new XPCollection<MobileInventoryEntry>(uow,
                        CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEntry).FullName)
                        );

                    DocumentHeader documentHeader = CreateDocumentHeader(Mobile.AuxilliaryClasses.eDocumentType.INVENTORY, "", "", "", uow);
                    CompanyNew owner = documentHeader.Owner;

                    foreach (Model.MobileInventory.MobileInventoryEntry line in collection)
                    {

                        Item item = line.Item;
                        Barcode barcode = item.DefaultBarcode;
                        DocumentDetail documentDetail = null;
                        switch (documentHeader.Division)
                        {
                            case Platform.Enumerations.eDivision.Sales:
                                //PriceCatalogDetail pricdeCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetail(documentHeader.Store, barcode.Code, documentHeader.Customer);
                                DocumentDetail detail = DocumentHelper.ComputeDocumentLine(ref documentHeader, item, barcode, line.Qty, false, -1, false, "", new List<DocumentDetailDiscount>());
                                DocumentHelper.AddItem(ref documentHeader, detail);
                                break;
                            case Platform.Enumerations.eDivision.Purchase:
                                documentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader, item, barcode, line.Qty, false, 0, true, null, null);
                                DocumentHelper.AddItem(ref documentHeader, documentDetail);
                                break;
                            case Platform.Enumerations.eDivision.Store:
                                documentDetail = DocumentHelper.ComputeDocumentLine(ref documentHeader, item, barcode, line.Qty, false, 0, true, null, null);
                                DocumentHelper.AddItem(ref documentHeader, documentDetail);
                                break;
                        }
                    }
                    if (documentHeader.DocumentType.MergedSameDocumentLines)
                    {
                        DocumentHelper.MergeDocumentLines(ref documentHeader);
                    }
                    if (documentHeader.DocumentDetails.Count > 0)
                    {
                        documentHeader.Save();
                        uow.Delete(collection);
                        XpoHelper.CommitChanges(uow);
                    }
                    else
                    {
                        errorMessage += Resources.Document + ":" + Resources.NotSaved;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.GetFullMessage();
                return false;
            }

        }

        //TODO --> Λίστα αποθηκευτικών χώρων
        public List<MobileAtStore.ObjectModel.Warehouse> GetWarehouses(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            MobileAtStore.ObjectModel.Warehouse warehouse = new MobileAtStore.ObjectModel.Warehouse()
            {
                CompCode = StoreControllerAppiSettings.CurrentStore.Code,
                Description = StoreControllerAppiSettings.CurrentStore.Description
            };
            return new List<MobileAtStore.ObjectModel.Warehouse>() { warehouse };
        }

        public List<MobileAtStore.ObjectModel.Warehouse> GetStores(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                return uow.Query<Store>().Select(x => new MobileAtStore.ObjectModel.Warehouse()
                {
                    CompCode = x.Code,
                    Description = x.Name
                }).ToList();
            }
            //MobileAtStore.ObjectModel.Warehouse warehouse = new MobileAtStore.ObjectModel.Warehouse()
            //{
            //    CompCode = StoreControllerAppiSettings.CurrentStore.Code,
            //    Description = StoreControllerAppiSettings.CurrentStore.Description
            //};
            //return new List<MobileAtStore.ObjectModel.Warehouse>() { warehouse };
        }

        //TODO --> List of PriceCatalog Policies
        public MobileAtStore.ObjectModel.PriceCatalogPolicy[] GetPriceCatalogPolicies(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            return StoreControllerAppiSettings.CurrentStore.StorePriceCatalogPolicies
                    .Select(storePolicy => new MobileAtStore.ObjectModel.PriceCatalogPolicy()
                    {
                        ID = storePolicy.PriceCatalogPolicy.Code,
                        Description = storePolicy.PriceCatalogPolicy.Description
                    }).ToArray();
        }

        //TODO --> Λίστα αρχείων που κατεβάζει το φορητό
        public string[][] GetMobileFilelist(int id, string ip)
        {
            TerminalIsAuthorized(id, ip);
            return this.GetFileList("Mobile");
        }

        //TODO --> Το config του φορητού
        public bool GetMobileConfig(int id, string ip, out string fileContent)
        {
            TerminalIsAuthorized(id, ip);
            fileContent = "";
            return false;
        }

        ////TODO Λίστα τιμοκαταλόγων
        //// Για κάποιο λόγο είναι διπλό, απο παλαιότερα. Θέλει διερεύνηση με βάση τον αρχικό κώδικα ώστε
        //// να δούμε αν χρειάζεται πραγματικά ή μπορεί να αντικατασταθεί από την GetPriceLists()
        //public MobileAtStore.ObjectModel.PriceCatalogPolicy[] GetPriceListsFromWebService(int id, string ip)
        //{
        //    TerminalIsAuthorized(id, ip);
        //    throw new NotImplementedException();
        //}


        // Private Functions

        private T CommonUploadInvLine<W, T>(T line, decimal quantity, bool add)
            where W : Model.MobileInventory.MobileInventoryEntry, new()
            where T : RetailAtStoreModel.InvLine, new()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                W dbLine = uow.GetObjectByKey<W>(line.Oid);
                if (dbLine == null)
                {
                    Item item = uow.FindObject<Item>(new BinaryOperator("Code", line.ProdCode));
                    dbLine = Activator.CreateInstance(typeof(W), uow) as W;
                    dbLine.Item = item;
                    dbLine.Qty = 0;
                    if (dbLine is ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry)
                    {
                        (dbLine as ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry).InventoryNumber = (line as RetailAtStoreModel.ESLInvLine).InventoryNumber;
                    }
                }
                dbLine.Qty = quantity + (add ? dbLine.Qty : 0);
                dbLine.Save();
                uow.CommitChanges();
                T newT = new T()
                {
                    CreatedOn = dbLine.CreatedOn,
                    Descr = dbLine.Item.Name,
                    Export = 0,
                    Oid = dbLine.Oid,
                    ProdBarcode = dbLine.Item.Code,
                    ProdCode = dbLine.Item.Code,
                    Qty = dbLine.Qty,
                    UpdatedOn = dbLine.UpdatedOn
                };
                if (dbLine is ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry)
                {
                    (newT as RetailAtStoreModel.ESLInvLine).InventoryNumber = (dbLine as ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry).InventoryNumber;
                }
                return newT;
            }
        }

        private List<T> GetInvLinesCommon<W, T>(string taxCode, string invNumber = null)
            where W : Model.MobileInventory.MobileInventoryEntry
            where T : RetailAtStoreModel.InvLine, new()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                List<T> list = new List<T>();
                CriteriaOperator crop = string.IsNullOrWhiteSpace(invNumber) ? new BinaryOperator("Item.Code", taxCode) :
                    CriteriaOperator.And(new BinaryOperator("Item.Code", taxCode), new BinaryOperator("InventoryNumber", invNumber));
                List<W> dblist = new XPCollection<W>(uow, CriteriaOperator.And(
                    CriteriaOperator.Parse("IsExactType(This,?)", typeof(W).FullName), crop)).ToList();
                if (dblist.Count > 0)
                {
                    dblist.ForEach(
                       x =>
                       {
                           T newT = new T()
                           {
                               CreatedOn = x.CreatedOn,
                               Descr = x.Item.Name,
                               Export = 0,
                               Oid = x.Oid,
                               ProdBarcode = x.Item.Code,
                               ProdCode = x.Item.Code,
                               Qty = x.Qty,
                               UpdatedOn = x.UpdatedOn
                           };
                           if (x is ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry)
                           {
                               (newT as RetailAtStoreModel.ESLInvLine).InventoryNumber = (x as ITS.Retail.Model.MobileInventory.MobileInventoryEslEntry).InventoryNumber;
                           }
                           list.Add(newT);
                       });
                }
                return list;
            }
        }


        private DocumentHeader CreateDocumentHeader(RetailAtStoreModel.Header header, UnitOfWork uow)
        {
            return CreateDocumentHeader(header.DocType, header.CustomerAFM, header.CustomerCode, header.DocNumber.ToString(), uow);
        }

        private DocumentHeader CreateDocumentHeader(ITS.Retail.Mobile.AuxilliaryClasses.eDocumentType DocType,
            string traderTaxCode, string traderCode, string remarks, UnitOfWork uow)
        {
            DocumentHeader document = new DocumentHeader(uow);
            document.DocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Type", DocType));
            if (document.DocumentType == null)
            {
                throw new Exception(Resources.PleaseSelectDefaultDocumentType + " " + DocType);
            }
            document.Division = document.DocumentType.Division.Section;
            document.Store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStore.Oid);
            document.Remarks = remarks;
            document.FinalizedDate = DateTime.Now;


            StoreDocumentSeriesType storeDocumentSeriesType = uow.FindObject<StoreDocumentSeriesType>(
                            CriteriaOperator.And(
                                new BinaryOperator("DocumentType", document.DocumentType),
                                new InOperator("DocumentSeries", document.Store.DocumentSeries),
                                new BinaryOperator("DocumentSeries.eModule", eModule.POS, BinaryOperatorType.NotEqual),
                                new BinaryOperator("DocumentSeries.IsCancelingSeries", false)
                            )
                    );

            if (storeDocumentSeriesType != null && storeDocumentSeriesType.DocumentSeries != null)
            {
                document.DocumentSeries = storeDocumentSeriesType.DocumentSeries;
            }
            else
            {
                throw new Exception(Resources.StoreHasNoSeriesTypes);
            }

            switch (DocType)
            {
                case Mobile.AuxilliaryClasses.eDocumentType.TAG:
                    document.Customer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomer.Oid);
                    document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                    break;
                default:
                    switch (document.Division)
                    {
                        case Platform.Enumerations.eDivision.Sales:
                            document.Customer = uow.FindObject<Customer>(new BinaryOperator("Trader.TaxCode", traderTaxCode)) ?? storeDocumentSeriesType.DefaultCustomer;
                            if (document.Customer == null)
                            {
                                throw new Exception(string.Format(ResourcesLib.Resources.CouldNotFindTrader, traderTaxCode, traderCode));
                            }
                            if (DocumentHelper.DocTypeSupportsCustomer(document, document.Customer) == false)
                            {
                                throw new Exception(Resources.TraderIsNotAllowed);
                            }

                            document.DeliveryAddress = document.Customer.DefaultAddress == null ? string.Empty : document.Customer.DefaultAddress.Description;
                            document.BillingAddress = document.Customer.DefaultAddress == null ? null : document.Customer.DefaultAddress;
                            document.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(document.Store, document.Customer);
                            break;
                        case Platform.Enumerations.eDivision.Purchase:
                            document.Supplier = uow.FindObject<SupplierNew>(new BinaryOperator("Trader.TaxCode", traderTaxCode)) ?? storeDocumentSeriesType.DefaultSupplier;
                            if (document.Supplier == null)
                            {
                                throw new Exception(string.Format(ResourcesLib.Resources.CouldNotFindTrader, traderTaxCode, traderCode));
                            }
                            document.DeliveryAddress = document.Supplier.DefaultAddress == null ? string.Empty : document.Supplier.DefaultAddress.Description;
                            break;
                        case Platform.Enumerations.eDivision.Store:
                            document.SecondaryStore = uow.FindObject<Store>(new BinaryOperator("Code", traderCode)) ?? document.Store;

                            break;
                    }

                    break;
            }
            document.Status = storeDocumentSeriesType.DocumentType.DefaultDocumentStatus;
            if (document.Status == null)
            {
                document.Status = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
                if (document.Status == null)
                {
                    document.Status = uow.FindObject<DocumentStatus>(null);
                }
            }
            return document;
        }


        protected String[][] GetFileList(string contentfolder)
        {
            DirectoryInfo df = new DirectoryInfo(HostingEnvironment.MapPath("~/" + contentfolder));
            FileInfo[] filesInfo = df.GetFiles();
            String[][] fileList =
                filesInfo.Select(x => new String[]{
                    x.Name,
                    GetFileHash(HostingEnvironment.MapPath("~/" + contentfolder) + "\\" + x.Name),
                     OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().Substring(0,
                      OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().IndexOf("/WRMMobileAtStore.svc")+1)
                    + contentfolder + "/" + x.Name
                }).ToArray();

            return fileList.ToArray();
        }

        protected string GetFileHash(String filename)
        {
            string result = "";
            byte[] arrbytHashValue;
            using (MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider())
            {
                try
                {
                    using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        arrbytHashValue = oMD5Hasher.ComputeHash(fileStream);
                        fileStream.Close();
                        result = System.BitConverter.ToString(arrbytHashValue);
                        return result;
                    }
                }
                catch (System.Exception ex)
                {
                    MvcApplication.WRMLogModule.Log(ex, "Exception during File Hash Computation:", KernelLogLevel.Info);
                    return "-1";
                }
            }
        }

        public MobileAtStoreSettings GetSettings()
        {
            return new MobileAtStoreSettings()
            {
                QuantityNumberOfDecimalDigits = StoreControllerAppiSettings.OwnerApplicationSettings.QuantityNumberOfDecimalDigits,
                QuantityNumberOfIntegralDigits = StoreControllerAppiSettings.OwnerApplicationSettings.QuantityNumberOfIntegralDigits
            };
        }

        private void TerminalIsAuthorized(int id, string ip)
        {
            //if (MvcApplication.USES_LICENSE)
            //{
            //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            //    {
            //        CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("ID", id),
            //                                                         new BinaryOperator("IPAddress", ip),
            //                                                         new BinaryOperator("IsActive", true)
            //                                                        );
            //        MobileTerminal mobileTerminal = uow.FindObject<MobileTerminal>(criteria);
            //        if (mobileTerminal == null)
            //        {
            //            throw new WebFaultException<string>(Resources.PermissionDenied, HttpStatusCode.PaymentRequired);
            //        }
            //    }
            //}
        }
    }
}
