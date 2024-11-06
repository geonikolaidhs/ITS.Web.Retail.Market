//-----------------------------------------------------------------------
// <copyright file="Item.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using System.Drawing;
using DevExpress.Xpo.Metadata;
using ITS.Retail.ResourcesLib;
using System.Collections.Generic;

namespace ITS.Retail.Model
{


    [DataViewParameter]
    [Serializable]
    [Updater(Order = 420, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Item", typeof(Resources))]
    [Indices("Oid;IsActive", "IsActive", "Oid;InsertedDate", "Oid;Owner;InsertedDate")]
    public class Item : BaseObj, IRequiredOwner
    {
        public Item()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public Item(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("Item.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }


        private MeasurementUnit _PackingMeasurementUnit;
        private string _ImageDescription;
        private string _ImageInfo;
        private eItemCustomPriceOptions _CustomPriceOptions;
        private bool _AcceptsCustomDescription;
        private CompanyNew _Owner;
        private decimal _ReferenceUnit = 0m;
        private string _Remarks;
        private decimal _Points;
        private double _MinOrderQty;
        private double _PackingQty;
        private double _OrderQty;
        private double _MaxOrderQty;
        private string _Code;
        private string _ExtraDescription;
        [NonSerialized]
        private Barcode _DefaultBarcode;
        private VatCategory _VatCategory;
        private Guid? _MotherCodeOid;
        private DateTime _InsertedDate;
        private Seasonality _Seasonality;
        private Buyer _Buyer;
        private bool _IsCentralStored;
        private SupplierNew _DefaultSupplier;
        private string _Name;
        private bool _AreDiscountsAllowed;
        private decimal _ContentUnit;
        private byte[] _ExtraFile;
        private string _ExtraFilename;
        private string _ExtraMimeType;
        private string _ExtraHtml;
        private bool _IsGeneralItem;
        private string _CashierDeviceIndex;
        private bool _DoesNotAllowDiscount;
        private bool _IsTax;

        [DescriptionField]
        public string Description
        {
            get
            {
                return Name;
            }
        }

        [Indexed(Unique = false)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }


        public bool DoesNotAllowDiscount
        {
            get
            {
                return _DoesNotAllowDiscount;
            }
            set
            {
                SetPropertyValue("DoesNotAllowDiscount", ref _DoesNotAllowDiscount, value);
            }
        }

        public bool IsTax
        {
            get
            {
                return _IsTax;
            }
            set
            {
                SetPropertyValue("IsTax", ref _IsTax, value);
            }
        }

        [Indexed("Owner;GCRecord", Unique = true)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Code", ResourceType = typeof(Resources))]
        public string Code   // Item Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        [Association("Owner-Items")]
        [Indexed("GCRecord", Unique = false)]
        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }


        [Indexed(Unique = false)]
        public string ExtraDescription
        {
            get
            {
                return _ExtraDescription;
            }
            set
            {
                SetPropertyValue("ExtraDescription", ref _ExtraDescription, value);
            }
        }

        public Barcode DefaultBarcode
        {
            get
            {
                return _DefaultBarcode;
            }
            set
            {
                SetPropertyValue("DefaultBarcode", ref _DefaultBarcode, value);
            }
        }

        [Association("VatCategory-Items")]
        public VatCategory VatCategory
        {
            get
            {
                return _VatCategory;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatCategory, value);
            }
        }

        [Persistent("MotherCode")]
        public Guid? MotherCodeOid
        {
            get
            {
                return _MotherCodeOid;
            }
            set
            {
                SetPropertyValue("MotherCodeOid", ref _MotherCodeOid, value);
            }
        }

        [NonPersistent, UpdaterIgnoreField]
        [Indexed("GCRecord", Unique = false)]
        public Item MotherCode
        {
            get
            {
                return this.Session.GetObjectByKey<Item>(this.MotherCodeOid);
            }
            set
            {
                if (value != null)
                {
                    MotherCodeOid = value.Oid;
                }
                else
                {
                    MotherCodeOid = null;
                }
            }
        }
        public DateTime InsertedDate
        {
            get
            {
                return _InsertedDate;
            }
            set
            {
                SetPropertyValue("InsertedDate", ref _InsertedDate, value);
            }
        }


        [Association("Seasonality-Items")]
        public Seasonality Seasonality
        {
            get
            {
                return _Seasonality;
            }
            set
            {
                SetPropertyValue("Seasonality", ref _Seasonality, value);
            }
        }

        [Association("Buyer-Items")]
        public Buyer Buyer
        {
            get
            {
                return _Buyer;
            }
            set
            {
                SetPropertyValue("Buyer", ref _Buyer, value);
            }
        }
        [System.ComponentModel.DataAnnotations.Display(Name = "PackingQty", ResourceType = typeof(Resources))]
        public double PackingQty
        {
            get
            {
                return _PackingQty;
            }
            set
            {
                SetPropertyValue("PackingQty", ref _PackingQty, value);
            }
        }


        public double OrderQty
        {
            get
            {
                return _OrderQty;
            }
            set
            {
                SetPropertyValue("OrderQty", ref _OrderQty, value);
            }
        }


        public double MinOrderQty
        {
            get
            {
                return _MinOrderQty;
            }
            set
            {
                SetPropertyValue("MinOrderQty", ref _MinOrderQty, value);
            }
        }

        public double MaxOrderQty
        {
            get
            {
                return _MaxOrderQty;
            }
            set
            {
                SetPropertyValue("MaxOrderQty", ref _MaxOrderQty, value);
            }
        }

        public bool IsCentralStored
        {
            get
            {
                return _IsCentralStored;
            }
            set
            {
                SetPropertyValue("IsCentralStored", ref _IsCentralStored, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [Association("Supplier-Items")]
        [Indexed("GCRecord", Unique = false)]
        public SupplierNew DefaultSupplier
        {
            get
            {
                return _DefaultSupplier;
            }
            set
            {
                SetPropertyValue("DefaultSupplier", ref _DefaultSupplier, value);
            }
        }

        [ValueConverter(typeof(ImageValueConverter)), Delayed]
        [UpdaterIgnoreField(eUpdateDirection.STORECONTROLLER_TO_POS)]
        public Image ImageLarge
        {
            get
            {
                return GetDelayedPropertyValue<Image>("ImageLarge");
            }
            set
            {
                SetDelayedPropertyValue("ImageLarge", value);
            }
        }

        [ValueConverter(typeof(ImageValueConverter)), Delayed]
        [UpdaterIgnoreField]
        public Image ImageMedium
        {
            get
            {
                return GetDelayedPropertyValue<Image>("ImageMedium");
            }
            set
            {
                SetDelayedPropertyValue("ImageMedium", value);
            }
        }

        [ValueConverter(typeof(ImageValueConverter)), Delayed]
        [UpdaterIgnoreField]
        public Image ImageSmall
        {
            get
            {
                return GetDelayedPropertyValue<Image>("ImageSmall");
            }
            set
            {
                SetDelayedPropertyValue("ImageSmall", value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string ImageInfo
        {
            get
            {
                return _ImageInfo;
            }
            set
            {
                SetPropertyValue("ImageInfo", ref _ImageInfo, value);
            }
        }


        public string ImageDescription
        {
            get
            {
                return _ImageDescription;
            }
            set
            {
                SetPropertyValue("ImageDescription", ref _ImageDescription, value);
            }
        }

        public decimal Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Points", ref _Points, value);
            }
        }

        public bool AreDiscountsAllowed
        {
            get
            {
                return _AreDiscountsAllowed;
            }
            set
            {
                SetPropertyValue("AreDiscountsAllowed", ref _AreDiscountsAllowed, value);
            }

        }


        public bool AcceptsCustomPrice
        {
            get
            {
                return CustomPriceOptions != eItemCustomPriceOptions.NONE;
            }
        }


        public bool AcceptsCustomDescription
        {
            get
            {
                return _AcceptsCustomDescription;
            }
            set
            {
                SetPropertyValue("AcceptsCustomDescription", ref _AcceptsCustomDescription, value);
            }
        }

        public eItemCustomPriceOptions CustomPriceOptions
        {
            get
            {
                return _CustomPriceOptions;
            }
            set
            {
                SetPropertyValue("CustomPriceOptions", ref _CustomPriceOptions, value);
            }
        }

        public decimal ReferenceUnit
        {
            get
            {
                return _ReferenceUnit;
            }
            set
            {
                SetPropertyValue("ReferenceUnit", ref _ReferenceUnit, value);
            }
        }

        public decimal ContentUnit
        {
            get
            {
                return _ContentUnit;
            }
            set
            {
                SetPropertyValue("ContentUnit", ref _ContentUnit, value);
            }
        }

        [Association("MeasurementUnits-Items")]
        public MeasurementUnit PackingMeasurementUnit
        {
            get
            {
                return _PackingMeasurementUnit;
            }
            set
            {
                SetPropertyValue("PackingMeasurementUnit", ref _PackingMeasurementUnit, value);
            }
        }

        public byte[] ExtraFile
        {
            get
            {
                return _ExtraFile;
            }
            set
            {
                SetPropertyValue("ExtraFile", ref _ExtraFile, value);
            }
        }

        public string ExtraFilename
        {
            get
            {
                return _ExtraFilename;
            }
            set
            {
                SetPropertyValue("ExtraFilename", ref _ExtraFilename, value);
            }
        }

        public string ExtraMimeType
        {
            get
            {
                return _ExtraMimeType;
            }
            set
            {
                SetPropertyValue("ExtraMimeType ", ref _ExtraMimeType, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string ExtraHtml
        {
            get
            {
                return _ExtraHtml;
            }
            set
            {
                SetPropertyValue("ExtraHtml", ref _ExtraHtml, value);
            }
        }

        public bool IsGeneralItem
        {
            get
            {
                return _IsGeneralItem;
            }
            set
            {
                SetPropertyValue("IsGeneralItem", ref _IsGeneralItem, value);
            }
        }

        [Association("Item-OfferDetails"), Aggregated]
        public XPCollection<OfferDetail> OfferDetails
        {
            get
            {
                return GetCollection<OfferDetail>("OfferDetails");
            }
        }

        public XPCollection<Item> ChildItems
        {
            get
            {
                return new XPCollection<Item>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, new BinaryOperator("MotherCodeOid", this.Oid));
            }
        }
        [Association("Item-ItemAnalyticTrees"), Aggregated]
        public XPCollection<ItemAnalyticTree> ItemAnalyticTrees
        {
            get
            {
                return GetCollection<ItemAnalyticTree>("ItemAnalyticTrees");
            }
        }
        [Association("Item-PriceCatalogs"), Aggregated]
        public XPCollection<PriceCatalogDetail> PriceCatalogs
        {
            get
            {
                return GetCollection<PriceCatalogDetail>("PriceCatalogs");
            }
        }
        [Association("Item-ItemStocks"), Aggregated]
        public XPCollection<ItemStock> ItemStocks
        {
            get
            {
                return GetCollection<ItemStock>("ItemStocks");
            }
        }
        [Association("Item-LinkedItems"), Aggregated]
        public XPCollection<LinkedItem> LinkedItems
        {
            get
            {
                return GetCollection<LinkedItem>("LinkedItems");
            }
        }
        [Association("Item-SubItems")]
        public XPCollection<LinkedItem> SubItems
        {
            get
            {
                return GetCollection<LinkedItem>("SubItems");
            }
        }
        [Association("ItemStores-Item"), Aggregated]
        public XPCollection<ItemStore> Stores
        {
            get
            {
                return GetCollection<ItemStore>("Stores");
            }
        }

        [Association("Item-DocumentDetails")]
        public XPCollection<DocumentDetail> DocumentDetails
        {
            get
            {
                return GetCollection<DocumentDetail>("DocumentDetails");
            }
        }

        //----------------------------- Methods -----------------------------------
        public Store GetCentralStore(CompanyNew supplier)
        {
            ItemStore st = this.Session.FindObject<ItemStore>(CriteriaOperator.And(new BinaryOperator("Item", this, BinaryOperatorType.Equal), new BinaryOperator("Store.Owner", supplier.Oid, BinaryOperatorType.Equal)));
            return st != null ? st.Store : null;
        }
        public T GetObject<T>(int id)
        {
            return Session.GetObjectByKey<T>(id);
        }

        [Association("Item-ItemBarcodes"), Aggregated]
        public XPCollection<ItemBarcode> ItemBarcodes
        {
            get
            {
                return GetCollection<ItemBarcode>("ItemBarcodes");
            }
        }



        public decimal GetUnitPrice(PriceCatalog priceCatalog, Barcode barcode = null, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            try
            {
                PriceCatalogDetail priceCatalogDetail = GetPriceCatalogDetail(priceCatalog, barcode);
                if (priceCatalogDetail == null)
                {
                    return -1;
                }
                return priceCatalogDetail.GetUnitPrice();
            }
            catch
            {
                return -1;
            }
        }
        public decimal GetUnitPriceWithVat(PriceCatalog priceCatalog, Barcode barcode = null, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            try
            {
                PriceCatalogDetail priceCatalogDetail = GetPriceCatalogDetail(priceCatalog, barcode);
                if (priceCatalogDetail == null)
                {
                    return -1;
                }
                return priceCatalogDetail.GetUnitPriceWithVat();
            }
            catch
            {
                return -1;
            }
        }
        public PriceCatalogDetail GetPriceCatalogDetail(PriceCatalog priceCatalog, Barcode barcode = null, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            DateTime now = DateTime.Now;
            PriceCatalogDetail pcdt = null;
            if (traces != null)
            {
                traces.Add(new PriceSearchTraceStep()
                {
                    PriceCatalogDescription = priceCatalog.Description,
                    SearchMethod = priceCatalogSearchMethod,
                    Number = traces.Count + 1
                });
            }

            if (barcode != null)
            {
                pcdt = priceCatalog.Session.FindObject<PriceCatalogDetail>(CriteriaOperator.And(new BinaryOperator("PriceCatalog.Oid", priceCatalog.Oid),
                                                                                                new BinaryOperator("PriceCatalog.EndDate", now, BinaryOperatorType.GreaterOrEqual),
                                                                                                new BinaryOperator("PriceCatalog.StartDate", now, BinaryOperatorType.Less),
                                                                                                new BinaryOperator("Item.Oid", Oid),
                                                                                                new BinaryOperator("Barcode.Oid", barcode.Oid),
                                                                                                new BinaryOperator("IsActive", true)
                                                                                                )
                                                                                           );
            }

            if (pcdt == null || (priceCatalog.IgnoreZeroPrices && pcdt.Value == 0))
            {
                OwnerApplicationSettings ownAppSet = this.Session.FindObject<OwnerApplicationSettings>(new BinaryOperator("Owner.Oid", priceCatalog.Owner.Oid));
                string paddedCode = (ownAppSet.PadBarcodes) ? Code.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : Code;
                Barcode fkOid = priceCatalog.Session.FindObject<Barcode>(new BinaryOperator("Code", paddedCode));  // Do Padding

                if (fkOid == null)
                {
                    return null;
                }
                pcdt = priceCatalog.Session.FindObject<PriceCatalogDetail>(CriteriaOperator.And(new BinaryOperator("PriceCatalog.Oid", priceCatalog.Oid),
                                                                                                new BinaryOperator("PriceCatalog.EndDate", now, BinaryOperatorType.GreaterOrEqual),
                                                                                                new BinaryOperator("PriceCatalog.StartDate", now, BinaryOperatorType.Less),
                                                                                                new BinaryOperator("Item.Oid", Oid),
                                                                                                new BinaryOperator("Barcode.Oid", fkOid.Oid),
                                                                                                new BinaryOperator("IsActive", true)));
                if (pcdt == null || (priceCatalog.IgnoreZeroPrices && pcdt.Value == 0))
                {
                    if ((!priceCatalog.IsRoot || priceCatalog.ParentCatalog != null)
                        && priceCatalogSearchMethod == PriceCatalogSearchMethod.PRICECATALOG_TREE
                      )
                    {
                        return GetPriceCatalogDetail(priceCatalog.ParentCatalog, barcode, traces: traces);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return pcdt;
        }


        public XPCollection<PriceCatalogDetail> GetAllPrices(Session session)
        {
            DateTime now = DateTime.Now;
            CriteriaOperator cop = CriteriaOperator.And(new BinaryOperator("Item.Oid", Oid, BinaryOperatorType.Equal),
                                                        new BinaryOperator("StartDate", now, BinaryOperatorType.LessOrEqual),
                                                        new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual));
            return new XPCollection<PriceCatalogDetail>(session, cop, new SortProperty("PriceCatalog.Code", SortingDirection.Ascending));
        }

        public void AddBarcode(Barcode barcode)
        {
            barcode.Save();
            ItemBarcode itemBarcode = new ItemBarcode(this.Session);
            itemBarcode.Barcode = barcode;
            itemBarcode.Owner = this.Owner;
            this.ItemBarcodes.Add(itemBarcode);
            itemBarcode.Save();
        }


        public void RemoveBarcode(Barcode barcode)
        {
            ItemBarcode itemBarcode = this.ItemBarcodes.Where(x => x.Barcode.Oid == barcode.Oid).FirstOrDefault();
            this.ItemBarcodes.Remove(itemBarcode);
        }


        [Association("Item-ItemExtraInfos"), Aggregated]
        public XPCollection<ItemExtraInfo> ItemExtraInfos
        {
            get
            {
                return GetCollection<ItemExtraInfo>("ItemExtraInfos");
            }
        }

        /// <summary>
        /// Returns the ItemStock of Item for Store store at UnityOfWork uow.
        /// If none is found then null is returned.
        /// </summary>
        /// <param name="store">The Store for which we seatch the ItemStock</param>
        /// <returns>The ItemStock of Item for Store store at Item's UnitOfWork.If none is found then null is returned.</returns>
        public ItemStock GetItemStockForStore(Store store)
        {
            ItemStock itemStock = null;
            CriteriaOperator itemStockCriteria = CriteriaOperator.And(new BinaryOperator("Item", this.Oid),
                                                                      new BinaryOperator("Store", store.Oid)
                                                                     );
            itemStock = this.Session.FindObject<ItemStock>(itemStockCriteria);
            return itemStock;
        }

        public DocumentHeader LastInventoryDocumentHeader(DocumentHeader excludeDocumentHeader)
        {
            return this.DocumentDetails.Where(documentDetail => documentDetail.DocumentHeader.Oid != excludeDocumentHeader.Oid
                                                             && documentDetail.DocumentHeader.IsCanceled == false
                                                             && documentDetail.DocumentHeader.Store.Oid == excludeDocumentHeader.Store.Oid
                                                             && documentDetail.DocumentHeader.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.INITIALISES
                                                             && documentDetail.DocumentHeader.FinalizedDate.Ticks >= excludeDocumentHeader.FinalizedDate.Ticks
                                             )
                                             .OrderByDescending(documentDetail => documentDetail.DocumentHeader.FinalizedDate)
                                             .Select(documentDetail => documentDetail.DocumentHeader)
                                             .FirstOrDefault();
        }

        private IEnumerable<DocumentDetail> DocumentDetailsAfterInventory(DocumentHeader inventory)
        {
            return this.DocumentDetails
                       .Where(documentDetail => documentDetail.DocumentHeader.Oid != inventory.Oid
                                             && documentDetail.DocumentHeader.IsCanceled == false
                                             && documentDetail.DocumentHeader.Store.Oid == inventory.Store.Oid
                                             && documentDetail.DocumentHeader.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS
                                             && documentDetail.DocumentHeader.FinalizedDate.Ticks >= inventory.FinalizedDate.Ticks
                             );
        }

        public double RecalculateItemStockAfterInventory(DocumentHeader lastInventoryDocumentHeader)
        {
            double stock = lastInventoryDocumentHeader.DocumentType.QuantityFactor * lastInventoryDocumentHeader.DocumentDetails.Where(documentLine => documentLine.Item.Oid == this.Oid).Sum(documentLine => documentLine.BaseQuantity);
            IEnumerable<DocumentDetail> documentDetailsAfterInventory = this.DocumentDetailsAfterInventory(lastInventoryDocumentHeader);
            foreach (DocumentDetail documentDetail in documentDetailsAfterInventory)
            {
                stock += documentDetail.DocumentHeader.DocumentType.QuantityFactor * documentDetail.BaseQuantity;
            }
            return stock;
        }

        public double RecalculateItemStockForItemWithoutInventory(DocumentHeader excludeDocumentHeader)
        {
            double stock = 0;
            foreach (DocumentDetail documentDetail in this.DocumentDetails.Where(documentDetail => documentDetail.DocumentHeader.Oid != excludeDocumentHeader.Oid
                                                                                                && documentDetail.DocumentHeader.IsCanceled == false
                                                                                                && documentDetail.DocumentHeader.Store.Oid == excludeDocumentHeader.Store.Oid
                                                                                                && documentDetail.DocumentHeader.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS
                                                                                              )
                    )
            {
                stock += documentDetail.DocumentHeader.DocumentType.QuantityFactor * this.DocumentDetails
                                                                                         .Where(documentLine => documentLine.Item.Oid == this.Oid)
                                                                                         .Sum(documentLine => documentLine.BaseQuantity);
            }
            return stock;
        }

        public double RecalculateItemStockForItemWithoutInventory(Store store)
        {
            double stock = 0;
            foreach (DocumentDetail documentDetail in this.DocumentDetails.Where(documentDetail => documentDetail.DocumentHeader.IsCanceled == false
                                                                                                && documentDetail.DocumentHeader.Store.Oid == store.Oid
                                                                                                && documentDetail.DocumentHeader.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS
                                                                                              )
                    )
            {
                stock += documentDetail.DocumentHeader.DocumentType.QuantityFactor * this.DocumentDetails
                                                                                         .Where(documentLine => documentLine.Item.Oid == this.Oid)
                                                                                         .Sum(documentLine => documentLine.BaseQuantity);
            }
            return stock;
        }
        [Indexed(Unique = false)]
        public string CashierDeviceIndex
        {
            get
            {
                return _CashierDeviceIndex;
            }
            set
            {
                SetPropertyValue("CashierDeviceIndex", ref _CashierDeviceIndex, value);
            }
        }
        [Association("Item-LeafletDetails"), Aggregated]
        public XPCollection<LeafletDetail> LeafletDetails
        {
            get
            {
                return GetCollection<LeafletDetail>("LeafletDetails");
            }
        }
    }
}


