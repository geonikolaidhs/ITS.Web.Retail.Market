using System;
using System.Collections.Generic;
using System.Linq;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ImageMagick;
using System.Drawing;
using System.Drawing.Imaging;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.Helpers;

namespace ITS.Retail.WebClient.Helpers
{
    /// <summary>
    /// Supporting functions for Items
    /// </summary>
    public static class ItemHelper
    {
        /// <summary>
        /// Returns the Item of Owner supplier with Barcode <paramref name="bc"/>
        /// </summary>
        /// <param name="uow">The unit of work</param>
        /// <param name="bc">The barcode</param>
        /// <param name="supplier">The owner</param>
        /// <returns>The Item</returns>
        public static Item GetItemOfSupplier(Session uow, Barcode bc, CompanyNew supplier)
        {
            return uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Owner.Oid", supplier.Oid), new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Oid", bc.Oid))));
        }

        public static Item GetItemOfSupplier(Session uow, Barcode bc, Guid supplier)
        {
            return uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Owner.Oid", supplier), new ContainsOperator("ItemBarcodes", new BinaryOperator("Barcode.Oid", bc.Oid))));
        }
        /// <summary>
        /// Returns the XPCollection that contain the barcodes associated with the Item
        /// </summary>
        /// <param name="item">The Item</param>
        /// <returns>The Barcodes of the Item</returns>
		public static XPCollection<Barcode> GetBarcodesOfItem(Item item)
        {
            if (item.ItemBarcodes == null)
            {
                return new XPCollection<Barcode>(item.Session, new BinaryOperator("Oid", Guid.Empty));
            }
            return new XPCollection<Barcode>(item.Session, item.ItemBarcodes.Select(g => g.Barcode).ToList());
        }

        /// <summary>
        /// Recursively finds the PriceCatalogDetail of an Item in a PriceCatalog and its parents. 
        /// </summary>
        /// <param name="priceCatalog">The PriceCatalog</param>
        /// <param name="item">The Item</param>
        /// <returns>The PriceCatalogDetail</returns>
        public static PriceCatalogDetail GetItemPrice(PriceCatalog priceCatalog, Item item)
        {
            item.PriceCatalogs.Filter = CriteriaOperator.And(new BinaryOperator("PriceCatalog.Oid", priceCatalog.Oid), new BinaryOperator("IsActive", true));
            PriceCatalogDetail detail = null;

            if (item.PriceCatalogs.Count == 0)
            {
                if (priceCatalog.ParentCatalog != null)
                {
                    detail = GetItemPrice(priceCatalog.ParentCatalog, item);
                }
            }
            else
            {
                detail = item.PriceCatalogs.First();
            }
            item.PriceCatalogs.Filter = null;
            return detail;
        }

        /// <summary>
        /// Recursively finds the PriceCatalogDetails of Items in a PriceCatalog and its parents. 
        /// </summary>
        /// <param name="priceCatalog">The PriceCatalog</param>
        /// <param name="items">The List of Items</param>
        /// <returns>The List of PriceCatalogDetails</returns>
        //public static List<PriceCatalogDetail> GetPricesForItems(PriceCatalog priceCatalog, List<Item> items)
        //{
        //    List<PriceCatalogDetail> priceCatalogDetails = new List<PriceCatalogDetail>();
        //    foreach (Item item in items)
        //    {
        //        PriceCatalogDetail tempPriceCatalogDetail = GetItemPrice(priceCatalog, item);
        //        if (tempPriceCatalogDetail != null)
        //        {
        //            priceCatalogDetails.Add(tempPriceCatalogDetail);
        //        }
        //    }
        //    return priceCatalogDetails;
        //}

        ///// <summary>
        ///// Recursively finds the PriceCatalogDetails of Items in a PriceCatalog and its parents. 
        ///// </summary>
        ///// <param name="pc">The PriceCatalog</param>
        ///// <param name="items">The List of Items</param>
        ///// <returns>The List of PriceCatalogDetails</returns>
        //public static IEnumerable<PriceCatalogDetail> GetPricesForItems2(PriceCatalog pc, IEnumerable<Item> items)
        //{
        //    IEnumerable<PriceCatalogDetail> pdd = items.Select(g => g.PriceCatalogs.Where(pd => pd.PriceCatalog.Oid == pc.Oid).FirstOrDefault()).Where(g => g != null);

        //    if (pc.ParentCatalog != null && pc.ParentCatalog != pc)
        //    {
        //        IEnumerable<Item> includedItems = pdd.Select(g => g.Item);
        //        IEnumerable<Item> restItems = items.Except(includedItems);
        //        IEnumerable<PriceCatalogDetail> parentDetails = GetPricesForItems2(pc.ParentCatalog, restItems);
        //        return pdd.Union(parentDetails);
        //    }

        //    return pdd;
        //}

        /// <summary>
        /// Returns the Unit Price without vat after price catalog discount
        /// </summary>
        /// <param name="pcDetail">The PriceCatalogDetail</param>
        /// <returns>The Price</returns>
        public static decimal GetItemPriceWithoutTax(PriceCatalogDetail pcDetail)
        {
            decimal displayValue = 0;
            decimal discount = pcDetail.Discount;
            if (discount > 1)
            {
                discount /= 100;
            }

            if (pcDetail.VATIncluded)
            {
                displayValue = (1 - discount) * pcDetail.GetUnitPrice();
            }
            else
            {
                displayValue = (1 - discount) * pcDetail.Value;
            }
            return displayValue;
        }

        /// <summary>
        /// Returns the Unit Price without vat after price catalog discount for an <paramref name="item"/> in price catalog <paramref name="priceCatalog"/>
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="priceCatalog">The Price Catalog</param>
        /// <returns>The Price</returns>
        public static decimal GetItemPriceWithoutTax(Store store, Item item, Customer customer)
        {
            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(store, item.Code, customer);
            PriceCatalogDetail priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
            if (priceCatalogDetail == null)
            {
                return -1;
            }
            return GetItemPriceWithoutTax(priceCatalogDetail);
        }

        public static ItemBarcode GetTaxCodeBarcode(UnitOfWork uow, Item item, CompanyNew owner)
        {

            if (item == null || owner == null)
            {
                throw new Exception(string.Format(ResourcesLib.Resources.ItemAndOwnerMustBeDefined, item == null ? "" : item.Name, owner == null ? "" : owner.CompanyName));
            }

            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Item.Oid", item.Oid), new BinaryOperator("Owner.Oid", owner.Oid));
            return uow.FindObject<ItemBarcode>(crop);
        }

        /// <summary>
        /// Returns all the points corresponding to an Item by suming recursively the points of all Item's ItemCategories
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="type">The document type</param>
        /// <param name="priceCatalog">The price catalog</param>
        /// <returns>The sum of Points</returns>
        public static decimal GetPointsOfItem(Item item, DocumentType type, OwnerApplicationSettings settings)
        {
            if (type.SupportLoyalty /*&& priceCatalog.SupportLoyalty*/ && settings.SupportLoyalty)
            {
                decimal points = item.Points;
                foreach (ItemAnalyticTree iat in item.ItemAnalyticTrees)
                {
                    if (iat.Node != null)
                    {
                        ItemCategory CurrentCategory = item.Session.GetObjectByKey<ItemCategory>(iat.Node.Oid);
                        while (CurrentCategory != null)
                        {
                            points += CurrentCategory.Points;
                            CurrentCategory = item.Session.GetObjectByKey<ItemCategory>(CurrentCategory.ParentOid);
                        }
                    }
                }
                return points;
            }
            return 0;
        }

        public static decimal GetSupplierLastPrice(Item item, SupplierNew supplier)
        {
            IEnumerable<Guid> statusOids = new XPQuery<DocumentStatus>(item.Session).Where(x => x.TakeSequence).Select(x => x.Oid);
            CriteriaOperator supplierCriteria = supplier == null ? null : new BinaryOperator("DocumentHeader.Supplier.Oid", supplier.Oid);
            CriteriaOperator crop = CriteriaOperator.And(
                new InOperator("DocumentHeader.Status.Oid", statusOids),
                new BinaryOperator("DocumentHeader.IsCanceled", false),
                new BinaryOperator("IsCanceled", false),
                supplierCriteria,
                new BinaryOperator("Item.Oid", item.Oid));
            using (XPCollection<DocumentDetail> details = new XPCollection<DocumentDetail>(item.Session, crop, new SortProperty("DocumentHeader.FiscalDate", DevExpress.Xpo.DB.SortingDirection.Descending)))
            {
                details.TopReturnedObjects = 1;
                if (details.Count > 0)
                {
                    return details.First().CustomUnitPrice;
                }
            }
            return 0m;
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

        public static WeightedBarcodeInfo GetWeightedBarcodeInfo(string code, DocumentHeader documentHeader)
        {
            if (string.IsNullOrEmpty(code) || documentHeader == null)
            {
                return null;
            }


            WeightedBarcodeInfo weightedBarcodeInfo = null;
            CriteriaOperator barcodeTypeCriteria = new ContainsOperator("DocumentTypeBarcodeTypes", new BinaryOperator("DocumentType", documentHeader.DocumentType.Oid));
            //XPCollection<BarcodeType> barcodeTypes = new XPCollection<BarcodeType>(documentHeader.Session, RetailHelper.ApplyOwnerCriteria(barcodeTypeCriteria, typeof(BarcodeType), documentHeader.Owner));
            List<BarcodeType> barcodeTypes = MaskHelper.GetMatchingMasks(code, null);
            BarcodeParseResult barcodeParseResult = null;

            switch (documentHeader.Division)
            {
                case eDivision.Financial:
                    return null;
                case eDivision.Other:
                    return null;
                case eDivision.Purchase:
                    barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode(barcodeTypes,
                                                                        code,
                                                                        documentHeader.Owner.OwnerApplicationSettings.PadBarcodes,
                                                                        documentHeader.Owner.OwnerApplicationSettings.BarcodeLength,
                                                                        documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                        );
                    switch (barcodeParseResult.BarcodeParsingResult)
                    {
                        case BarcodeParsingResult.ITEM_CODE_QUANTITY:
                            PriceCatalogDetail itemCodeQuantityPriceCatalogDetail = null;
                            weightedBarcodeInfo = ItemCodePurchaseParseResult(documentHeader, barcodeParseResult, out itemCodeQuantityPriceCatalogDetail);
                            if (itemCodeQuantityPriceCatalogDetail != null)
                            {
                                weightedBarcodeInfo.Value = weightedBarcodeInfo.Quantity * itemCodeQuantityPriceCatalogDetail.Value;
                            }
                            break;
                        case BarcodeParsingResult.ITEM_CODE_VALUE:
                            PriceCatalogDetail itemCodeValuePriceCatalogDetail = null;
                            weightedBarcodeInfo = ItemCodePurchaseParseResult(documentHeader, barcodeParseResult, out itemCodeValuePriceCatalogDetail);
                            if (itemCodeValuePriceCatalogDetail != null && itemCodeValuePriceCatalogDetail.Value != 0)
                            {
                                weightedBarcodeInfo.Quantity = weightedBarcodeInfo.Value / itemCodeValuePriceCatalogDetail.Value;
                            }
                            break;
                        case BarcodeParsingResult.CUSTOMER://we are looking for an item
                        case BarcodeParsingResult.NONE:
                        case BarcodeParsingResult.NON_WEIGHTED_PRODUCT://a non weighted product should have been already found in this method
                        default:
                            weightedBarcodeInfo = null;
                            break;
                    }
                    break;
                case eDivision.Sales:
                    barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode(barcodeTypes,
                                                                        code,
                                                                        documentHeader.Owner.OwnerApplicationSettings.PadBarcodes,
                                                                        documentHeader.Owner.OwnerApplicationSettings.BarcodeLength,
                                                                        documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                        );

                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                                               documentHeader.EffectivePriceCatalogPolicy,
                                                                                                               barcodeParseResult.DecodedCode);
                    PriceCatalogDetail pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    if (pricecatalogdetail != null
                        && (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY
                           || barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE
                          )
                       )
                    {
                        weightedBarcodeInfo = new WeightedBarcodeInfo(barcodeParseResult);
                        weightedBarcodeInfo.PriceCatalogDetail = pricecatalogdetail.Oid;
                        if (weightedBarcodeInfo.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE && pricecatalogdetail.RetailValue != 0)
                        {
                            weightedBarcodeInfo.Quantity = weightedBarcodeInfo.Value / pricecatalogdetail.RetailValue;
                        }

                        Barcode salesBarcode = documentHeader.Session.FindObject<Barcode>(
                                                                        RetailHelper.ApplyOwnerCriteria(
                                                                                        new BinaryOperator("Code", weightedBarcodeInfo.DecodedCode),
                                                                                        typeof(Barcode),
                                                                                        documentHeader.Owner
                                                                          ));
                        if (salesBarcode != null)
                        {
                            weightedBarcodeInfo.Barcode = salesBarcode.Oid;
                            CriteriaOperator itemBarcodeCriteria = RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(
                                                                                                                        new BinaryOperator("Item", pricecatalogdetail.Item),
                                                                                                                        new BinaryOperator("Barcode", salesBarcode.Oid)
                                                                                                                       ),
                                                                                                   typeof(ItemBarcode),
                                                                                                   documentHeader.Owner
                                                                                                  );
                            ItemBarcode salesItemBarcode = documentHeader.Session.FindObject<ItemBarcode>(itemBarcodeCriteria);
                            if (salesItemBarcode != null)
                            {
                                weightedBarcodeInfo.ItemBarcode = salesItemBarcode.Oid;
                            }
                        }
                    }
                    break;
                case eDivision.Store:
                    barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode(barcodeTypes,
                                                                            code,
                                                                            documentHeader.Owner.OwnerApplicationSettings.PadBarcodes,
                                                                            documentHeader.Owner.OwnerApplicationSettings.BarcodeLength,
                                                                            documentHeader.Owner.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                            );
                    switch (barcodeParseResult.BarcodeParsingResult)
                    {
                        case BarcodeParsingResult.ITEM_CODE_QUANTITY:
                            PriceCatalogDetail itemCodeQuantityPriceCatalogDetail = null;
                            weightedBarcodeInfo = ItemCodeStoreParseResult(documentHeader, barcodeParseResult, out itemCodeQuantityPriceCatalogDetail);
                            if (itemCodeQuantityPriceCatalogDetail != null)
                            {
                                weightedBarcodeInfo.Value = weightedBarcodeInfo.Quantity * itemCodeQuantityPriceCatalogDetail.Value;
                            }
                            break;
                        case BarcodeParsingResult.ITEM_CODE_VALUE:
                            PriceCatalogDetail itemCodeValuePriceCatalogDetail = null;
                            weightedBarcodeInfo = ItemCodeStoreParseResult(documentHeader, barcodeParseResult, out itemCodeValuePriceCatalogDetail);
                            if (itemCodeValuePriceCatalogDetail != null && itemCodeValuePriceCatalogDetail.Value != 0)
                            {
                                weightedBarcodeInfo.Quantity = weightedBarcodeInfo.Value / itemCodeValuePriceCatalogDetail.Value;
                            }
                            break;
                        case BarcodeParsingResult.CUSTOMER://we are looking for an item
                        case BarcodeParsingResult.NONE:
                        case BarcodeParsingResult.NON_WEIGHTED_PRODUCT://a non weighted product should have been already found in this method
                        default:
                            weightedBarcodeInfo = null;
                            break;
                    }
                    break;
                default:
                    return null;
            }
            return weightedBarcodeInfo;
        }

        private static WeightedBarcodeInfo ItemCodeStoreParseResult(DocumentHeader documentHeader, BarcodeParseResult barcodeParseResult, out PriceCatalogDetail priceCatalogDetail)
        {
            priceCatalogDetail = null;
            WeightedBarcodeInfo weightedBarcodeInfo = new WeightedBarcodeInfo(barcodeParseResult);
            Barcode storeBarcode = documentHeader.Session.FindObject<Barcode>(
                RetailHelper.ApplyOwnerCriteria(
                    new BinaryOperator("Code", weightedBarcodeInfo.DecodedCode),
                    typeof(Barcode),
                    documentHeader.Owner
                ));
            if (storeBarcode != null)
            {
                weightedBarcodeInfo.Barcode = storeBarcode.Oid;
            }

            ItemBarcode storeItemBarcode = documentHeader.Session.FindObject<ItemBarcode>(
                                                            RetailHelper.ApplyOwnerCriteria(
                                                                new BinaryOperator("Barcode.Code", weightedBarcodeInfo.DecodedCode),
                                                                typeof(ItemBarcode),
                                                                documentHeader.Owner
                                                             ));

            if (storeItemBarcode != null)
            {
                weightedBarcodeInfo.ItemBarcode = storeItemBarcode.Oid;
                if (documentHeader.Store.DefaultPriceCatalogPolicy != null)
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                            documentHeader.EffectivePriceCatalogPolicy,
                                                                                            storeItemBarcode.Item);
                    priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    //GetItemPrice(documentHeader.Store.DefaultPriceCatalog, storeItemBarcode.Item);
                }
            }

            return weightedBarcodeInfo;
        }

        private static WeightedBarcodeInfo ItemCodePurchaseParseResult(DocumentHeader documentHeader, BarcodeParseResult barcodeParseResult, out PriceCatalogDetail priceCatalogDetail)
        {
            priceCatalogDetail = null;
            WeightedBarcodeInfo weightedBarcodeInfo = new WeightedBarcodeInfo(barcodeParseResult);
            Barcode purchaseBarcode = documentHeader.Session.FindObject<Barcode>(
                RetailHelper.ApplyOwnerCriteria(
                    new BinaryOperator("Code", weightedBarcodeInfo.DecodedCode),
                    typeof(Barcode),
                    documentHeader.Owner
                    ));
            if (purchaseBarcode != null)
            {
                weightedBarcodeInfo.Barcode = purchaseBarcode.Oid;
            }

            ItemBarcode itemBarcode = documentHeader.Session.FindObject<ItemBarcode>(
                                                            RetailHelper.ApplyOwnerCriteria(
                                                                new BinaryOperator("Barcode.Code", weightedBarcodeInfo.DecodedCode),
                                                                typeof(ItemBarcode),
                                                                documentHeader.Owner
                                                             ));
            if (itemBarcode != null)
            {
                weightedBarcodeInfo.ItemBarcode = itemBarcode.Oid;

                if (documentHeader.Store.DefaultPriceCatalogPolicy != null)
                {
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                                            documentHeader.EffectivePriceCatalogPolicy,
                                                                                            itemBarcode.Item);
                    priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }
            }

            return weightedBarcodeInfo;
        }

        public static string GetBarcodeCodeForCashRegister(Item item, POSDevice cashRegister, CompanyNew owner)
        {
            string result = String.Empty;
            ItemBarcode itemBarcode = null;
            item.ItemBarcodes.ToList().ForEach(itb =>
            {
                if (itemBarcode == null)
                {
                    BarcodeType barcodeType = itb.Barcode.Type(owner);
                    if (barcodeType != null && barcodeType.Oid == cashRegister.BarcodeType.Oid)
                    {
                        itemBarcode = itb;
                    }
                }
            });

            if (itemBarcode != null)
            {
                result = itemBarcode.Barcode.Code;
            }
            else if (item.DefaultBarcode != null)
            {
                result = item.DefaultBarcode.Code;
            }
            return result;
        }
    }
}