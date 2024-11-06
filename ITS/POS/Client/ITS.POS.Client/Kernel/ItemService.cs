using System;
using System.Collections.Generic;
using System.Linq;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Master;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
namespace ITS.POS.Client.Kernel
{
    public class ItemInfo
    {
        public Item Item { get; set; }
        public Barcode SelectedBarcode { get; set; }
        public Store Store { get; set; }
        public Customer Customer { get; set; }
        public PriceCatalog PriceList { get; set; }
        public double Price { get; set; }
        public bool VatIncluded { get; set; }
        public ItemInfo()
        {
        }
    }
    /// <summary>
    /// Provides helper functions for handling item searching logic.
    /// </summary>
    public class ItemService : IItemService
    {
        private ISessionManager SessionManager { get; set; }
        private IConfigurationManager ConfigurationManager { get; set; }
        private OwnerApplicationSettings AppSettings
        {
            get
            {
                return ConfigurationManager == null ? null : ConfigurationManager.GetAppSettings();
            }
        }
        public ItemService(ISessionManager sessionManager, IConfigurationManager config)
        {
            this.SessionManager = sessionManager;
            this.ConfigurationManager = config;
        }
        /// <summary>
        /// Gets an item-barcode pair by code.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeInactive"></param>
        /// <param name="foundButInactive"></param>
        /// <param name="plu"></param>
        /// <returns></returns>
        public KeyValuePair<Item, Barcode> GetItemAndBarcodeByCode(string code, bool includeInactive, out bool foundButInactive, string plu = null)
        {
            foundButInactive = false;
            if (String.IsNullOrWhiteSpace(code))
            {
                return new KeyValuePair<Item, Barcode>(null, null);
            }
            string barcodeCode;
            string itemCode;
            if (AppSettings.PadItemCodes)
            {
                itemCode = code.Trim().PadLeft(AppSettings.ItemCodeLength, AppSettings.ItemCodePaddingCharacter[0]);
            }
            else
            {
                itemCode = code.Trim();
            }
            if (AppSettings.PadBarcodes)
            {
                barcodeCode = code.Trim().PadLeft(AppSettings.BarcodeLength, AppSettings.BarcodePaddingCharacter[0]);
            }
            else
            {
                barcodeCode = code.Trim();
            }
            Barcode bc = null;
            if (plu != null)
            {
                ItemBarcode ibc = SessionManager.FindObject<ItemBarcode>(new BinaryOperator("PluCode", plu.Trim()));
                if (ibc == null)
                {
                    ibc = SessionManager.FindObject<ItemBarcode>(new BinaryOperator("PluCode", plu.Trim().PadLeft(5, '0')));
                }
                if (ibc == null)
                {
                    ibc = SessionManager.FindObject<ItemBarcode>(new BinaryOperator("PluCode", plu.Trim().TrimStart('0')));
                }
                if (ibc != null)
                {
                    bc = SessionManager.FindObject<Barcode>(new BinaryOperator("Oid", ibc.Barcode));
                }
            }
            if (bc == null)
            {
                bc = SessionManager.FindObject<Barcode>(new BinaryOperator("Code", barcodeCode));
            }
            if (bc == null)
            {
                bc = SessionManager.FindObject<Barcode>(new BinaryOperator("Code", itemCode));
            }
            Item item = null;
            if (bc != null)
            {
                item = this.GetItemFromBarcode(bc, includeInactive, out foundButInactive);
            }
            else
            {
                item = SessionManager.FindObject<Item>(new BinaryOperator("Code", itemCode));
                if (!includeInactive && item != null && item.IsActive == false)
                {
                    item = null;
                    foundButInactive = true;
                }
            }
            return new KeyValuePair<Item, Barcode>(item, bc);
        }
        protected Item GetItemFromBarcode(Barcode bc, bool includeInactive, out bool foundButInactive)
        {
            XPCollection<ItemBarcode> ibcs = new XPCollection<ItemBarcode>(bc.Session, new BinaryOperator("Barcode", bc.Oid));
            foundButInactive = false;
            if (ibcs.Count == 0)
            {
                return null;
            }
            if (ibcs.Count == 1)
            {
                //CriteriaOperator crop = new BinaryOperator("Oid", ibcs[0].Item);
                Item item = bc.Session.GetObjectByKey<Item>(ibcs[0].Item);
                //.FindObject<Item>(crop);
                if (!includeInactive && item != null && item.IsActive == false)
                {
                    item = null;
                    foundButInactive = true;
                    //crop = CriteriaOperator.And(crop, new BinaryOperator("IsActive", true));
                }
                return item;
            }
            throw new Exception("Barcode '" + bc.Code + "' has more than one associations with items.");
        }
        /// <summary>
        /// Gets the unit price of the given item for the given price catalog.
        /// </summary>
        /// <param name="pricecatalog"></param>
        /// <param name="item"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private decimal GetUnitPrice(List<PriceCatalogDetail> itemPriceCatalogDetails, PriceCatalog pricecatalog, Item item, out PriceCatalogDetail priceCatalogDetail, Barcode barcode = null, bool includeVat = true, bool includeInactivePrices = false, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            DateTime now = DateTime.Now;
            long nowTicks = now.Ticks;
            //PriceCatalogDetail priceCatalogDetail;
            priceCatalogDetail = null;
            bool foundButInactive;
            string itemCode = item.Code;
            string fkBarcode = item.Code;
            if (pricecatalog == null)
            {
                throw new Exception("Price Catalog NOT Defined");
            }
            if (barcode == null)
            {
                // seek by ItemCode
                if (item == null)
                {
                    throw new Exception("Item Not Found");
                }

                if (AppSettings.PadItemCodes)
                {
                    itemCode = item.Code.PadLeft(AppSettings.ItemCodeLength, AppSettings.ItemCodePaddingCharacter[0]);
                }
                if (AppSettings.PadBarcodes)
                {
                    fkBarcode = itemCode.PadLeft(AppSettings.BarcodeLength, AppSettings.BarcodePaddingCharacter[0]);
                }
                barcode = SessionManager.FindObject<Barcode>(new BinaryOperator("Code", fkBarcode));
                if (barcode == null)
                {
                    throw new Exception("Item Not Found");
                }
            }
            else if (item == null)
            {
                item = this.GetItemFromBarcode(barcode, true, out foundButInactive);
            }
            CriteriaOperator isActiveCriteria = includeInactivePrices ? null : new BinaryOperator("IsActive", true);
            if (pricecatalog.IsActive && pricecatalog.StartDate.Ticks <= now.Ticks || pricecatalog.EndDate.Ticks >= now.Ticks)
            {
                priceCatalogDetail = itemPriceCatalogDetails.FirstOrDefault(itemPcd => itemPcd.IsActive
                                                                           && itemPcd.PriceCatalog == pricecatalog.Oid
                                                                           && itemPcd.Barcode == barcode.Oid);
                //    SessionManager.FindObject<PriceCatalogDetail>(CriteriaOperator.And(new BinaryOperator("PriceCatalog", pricecatalog.Oid),
                //new BinaryOperator("Item", item),
                //new BinaryOperator("Barcode", barcode),
                //isActiveCriteria));
                if (priceCatalogDetail == null
                    && barcode.Code != item.Code
                  )//search by FK - FK
                {
                    barcode = SessionManager.FindObject<Barcode>(new BinaryOperator("Code", fkBarcode));
                    if (barcode != null)
                    {
                        priceCatalogDetail = itemPriceCatalogDetails.FirstOrDefault(itemPcd => itemPcd.IsActive
                                                                           && itemPcd.PriceCatalog == pricecatalog.Oid
                                                                           && itemPcd.Barcode == barcode.Oid);
                    }
                }
                if (priceCatalogDetail != null && pricecatalog.IgnoreZeroPrices && priceCatalogDetail.Value == 0)//Ignore this priceCAtalogDetail
                {
                    priceCatalogDetail = null;
                }
            }
            if (priceCatalogDetail == null || (pricecatalog.IgnoreZeroPrices && priceCatalogDetail.Value == 0))
            {
                // Check if item has price in parent Catalog
                if ((!pricecatalog.IsRoot || pricecatalog.ParentCatalogOid != null) && priceCatalogSearchMethod == PriceCatalogSearchMethod.PRICECATALOG_TREE)
                {
                    PriceCatalog parent = pricecatalog.Session.GetObjectByKey<PriceCatalog>(pricecatalog.ParentCatalogOid);
                    if (parent != null)
                    {
                        if (pricecatalog.Code != parent.Code)
                        {
                            return GetUnitPrice(itemPriceCatalogDetails, parent, item, out priceCatalogDetail, barcode, includeVat, includeInactivePrices); //  seek unitprice into parentCatalog
                        }
                    }
                }
                //throw new PriceNotFoundException(POSClientResources.PRICE_DOES_NOT_EXIST);
            }
            if (priceCatalogDetail != null && pricecatalog.IgnoreZeroPrices && priceCatalogDetail.Value == 0)
            {
                priceCatalogDetail = null;
            }
            decimal unitPrice = -1;
            // Compute Net Unit Price 
            if (priceCatalogDetail != null)
            {
                if (includeVat && priceCatalogDetail.VATIncluded)
                {
                    return priceCatalogDetail.Value;
                }
                VatCategory vatCategory = SessionManager.FindObject<VatCategory>(new BinaryOperator("Oid", item.VatCategory, BinaryOperatorType.Equal));
                VatLevel vatLevel = SessionManager.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                VatFactor vatFactor = SessionManager.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory", vatCategory.Oid), new BinaryOperator("VatLevel", vatLevel.Oid)));
                if (!priceCatalogDetail.VATIncluded || vatCategory == null || vatLevel == null || vatFactor == null)
                {
                    unitPrice = priceCatalogDetail.Value;
                }
                else
                {
                    unitPrice = priceCatalogDetail.Value / (1 + Math.Abs(vatFactor.Factor)); // remove VAT 
                }
            }
            return unitPrice;
        }
        /// <summary>
        /// Returns all the points corresponding to an Item by suming recursively the points of all the categories of the item.
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="type">The document type</param>
        /// <param name="pc">The price catalog</param>
        /// <returns>The sum of Points</returns>
        public decimal GetPointsOfItem(Item item, DocumentType type)
        {
            if (type.SupportLoyalty && AppSettings.SupportLoyalty && item.IsTax == false && item.DoesNotAllowDiscount == false)
            {
                decimal points = item.Points;
                foreach (ItemAnalyticTree iat in item.ItemAnalyticTrees)
                {
                    ItemCategory CurrentCategory = SessionManager.GetObjectByKey<ItemCategory>(iat.Node);
                    while (CurrentCategory != null)
                    {
                        points += CurrentCategory.Points;
                        CurrentCategory = SessionManager.GetObjectByKey<ItemCategory>(CurrentCategory.ParentOid);
                    }
                }
                return points;
            }
            return 0;
        }
        public bool CheckIfItemIsValidForDocumentType(Item item, DocumentType docType)
        {
            if (docType.DocumentTypeItemCategoryMode != eDocumentTypeItemCategory.NONE)
            {
                //Check if Item can be added to this Document
                XPCollection<DocumentTypeItemCategory> documentTypeItemCategories =
                    new XPCollection<DocumentTypeItemCategory>(SessionManager.GetSession<DocumentTypeItemCategory>(),
                    new BinaryOperator("DocumentType", docType.Oid));
                XPCollection<ItemAnalyticTree> itemAnalyticTrees = new XPCollection<ItemAnalyticTree>(SessionManager.GetSession<ItemAnalyticTree>(),
                    new BinaryOperator("Object", item.Oid));
                List<Guid> itemCategoriesOids = itemAnalyticTrees.Select(x => x.Node).ToList();
                XPCollection<ItemCategory> itemCategoriesCurrentLevelObjects = new XPCollection<ItemCategory>(SessionManager.GetSession<ItemCategory>(), new InOperator("Oid", itemCategoriesOids));
                foreach (ItemCategory itemCategory in itemCategoriesCurrentLevelObjects)
                {
                    ItemCategory currentLevel = itemCategory;
                    do
                    {
                        if (itemCategoriesOids.Contains(currentLevel.Oid) == false)
                        {
                            itemCategoriesOids.Add(currentLevel.Oid);
                        }
                        currentLevel = currentLevel.Session.GetObjectByKey<ItemCategory>(currentLevel.ParentOid);
                    } while (currentLevel != null && currentLevel.ParentOid != null && currentLevel.ParentOid != Guid.Empty);
                }
                List<Guid> documentTypeItemCategoriesGuids = documentTypeItemCategories.Select(x => x.ItemCategory).ToList();
                IEnumerable<Guid> commonCategories = documentTypeItemCategoriesGuids.Intersect(itemCategoriesOids);
                int commonCount = commonCategories.Count();
                if (commonCount == 0 && docType.DocumentTypeItemCategoryMode == eDocumentTypeItemCategory.INCLUDE_ITEM_CATEGORIES)
                {
                    //throw new POSException(POSClientResources.DOCUMENT_TYPE + ":" + POSClientResources.ITEM_IS_INACTIVE);
                    return false;
                }
                if (commonCount != 0 && docType.DocumentTypeItemCategoryMode == eDocumentTypeItemCategory.EXCLUDE_ITEM_CATEGORIES)
                {
                    //throw new POSException(POSClientResources.DOCUMENT_TYPE + ":" + POSClientResources.ITEM_IS_INACTIVE);
                    return false;
                }
            }
            return true;
        }
        private decimal GetUnitPriceFromPolicy(PriceCatalogPolicy priceCatalogPolicy, Item item, out PriceCatalogDetail priceCatalogDetail, Barcode barcode = null, bool includeVat = true, bool includeInactivePrices = false, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            priceCatalogDetail = null;
            decimal priceCatalogDetailValue = -1;
            CriteriaOperator priceCatalogDetailCriteria = CriteriaOperator.And(new BinaryOperator("Item", item.Oid),
                                                                                new BinaryOperator("IsActive", true)
                                                                              );
            List<PriceCatalogDetail> itemPriceCatalogDetails = SessionManager.GetSession<PriceCatalogDetail>().Query<PriceCatalogDetail>()
                                                                         .Where(pcd => pcd.IsActive && pcd.Item == item.Oid).ToList();
            if (priceCatalogPolicy != null)
            {
                List<Guid> activePriceLists = SessionManager.GetSession<StorePriceList>().Query<StorePriceList>()
                                                        .Where(spl => spl.Store == ConfigurationManager.CurrentStoreOid)
                                                        .Select(spl => spl.PriceList).ToList();
                foreach (PriceCatalogPolicyDetail priceCatalogPolicyDetail in priceCatalogPolicy.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort))
                {
                    if (activePriceLists.Contains(priceCatalogPolicyDetail.PriceCatalog))
                    {
                        priceCatalogDetailValue = GetUnitPrice(itemPriceCatalogDetails, priceCatalogPolicyDetail.GetPriceCatalog, item, out priceCatalogDetail, barcode, includeVat, includeInactivePrices, priceCatalogPolicyDetail.PriceCatalogSearchMethod);
                        if (priceCatalogDetail != null)
                        {
                            return priceCatalogDetailValue;
                        }
                    }
                }
            }
            return priceCatalogDetailValue;
        }
        public decimal GetUnitPriceFromPolicies(PriceCatalogPolicy customerPriceCatalogPolicy, PriceCatalogPolicy storePriceCatalogPolicy, Item item, out PriceCatalogDetail priceCatalogDetail, Barcode barcode = null, bool includeVat = true, bool includeInactivePrices = false, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            priceCatalogDetail = null;
            decimal priceCatalogDetailValue = -1;
            if (customerPriceCatalogPolicy != null)
            {
                priceCatalogDetailValue = GetUnitPriceFromPolicy(customerPriceCatalogPolicy, item, out priceCatalogDetail, barcode, includeVat, includeInactivePrices, priceCatalogSearchMethod);
            }
            if (priceCatalogDetail == null && storePriceCatalogPolicy != null)
            {
                priceCatalogDetailValue = GetUnitPriceFromPolicy(storePriceCatalogPolicy, item, out priceCatalogDetail, barcode, includeVat, includeInactivePrices, priceCatalogSearchMethod);
            }
            return priceCatalogDetailValue;
        }
        /// <summary>
        /// Gets an item-barcode pair by description.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compareOperator"></param>
        /// <returns></returns>
        public List<Item> GetItemFromDescription(string name)
        {
            XPCollection<Item> itemCollection = null;
            if ((name.Replace("%","*")).Contains("*"))
            {
                itemCollection = new XPCollection<Item>(SessionManager.GetSession<Item>(), new FunctionOperator("Like", new OperandProperty("Name"), new OperandValue(name.Replace("*","%"))));// CriteriaOperator.And(new FunctionOperator("Like", new OperandProperty("Name"), new OperandValue(name)), new BinaryOperator("IsActive", true)));
            }
            else
            {
                itemCollection = new XPCollection<Item>(SessionManager.GetSession<Item>(), new BinaryOperator("Name", name));
            }
            if (itemCollection.Count == 0)
            {
                return new List<Item>();
            }
            return itemCollection.ToList();
        }
    }
}