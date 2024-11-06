using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using ITS.Retail.Platform.Kernel.Model;
using ITS.POS.Model.Master;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnore]
    public class DocumentDetail : BaseObj, IDocumentDetail
    {
        public DocumentDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentDetail(Session session)
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


        private Guid? _PriceCatalogDetail;
        private decimal _GrossTotalDeviation;
        private decimal _TotalVatAmountDeviation;
        private decimal _NetTotalDeviation;
        private decimal _CustomUnitPrice;
        private string _CustomMeasurementUnit;
        private string _Remarks;
        private bool _PriceCatalogValueVatIncluded;
        private string _VatFactorCode;
        private string _MeasurementUnitCode;
        private string _BarcodeCode;
        private decimal _GrossTotalBeforeDiscount;
        private bool _HasCustomDescription;
        private bool _HasCustomPrice;
        private string _CustomDescription;
        private Guid _LinkedLine;
        private Guid? _VatFactorGuid;
        private DocumentHeader _DocumentHeader;
        private decimal _GrossTotal;
        private decimal _TotalVatAmount;
        private decimal _NetTotal;
        private bool _IsReturn;
        private decimal _NetTotalBeforeDiscount;
        private decimal _Points;
        private decimal _GrossTotalBeforeDocumentDiscount;
        private decimal _PackingQuantity;
        private Guid _PackingMeasurementUnit;
        private double _PackingMeasurementUnitRelationFactor;
        private int _LineNumber;
        private string _SpecialItemCode;
        private string _ItemCode;
        private Guid _Item;
        private Guid _SpecialItem;
        private string _POSGeneratedPriceCatalogDetailSerialized;
        private bool _IsCanceled;
        private Guid _DocumentHeaderOid;
        private bool _FromScanner;
        private decimal _TotalDiscountAmountWithVAT;
        private decimal _TotalDiscountAmountWithoutVAT;
        private string _ItemName;
        private string _ItemVatCategory;
        private Guid _Barcode;
        private decimal _PriceListUnitPrice;
        private decimal _UnitPrice;
        private decimal _Qty;
        private Guid _MeasurementUnit;
        private decimal _TotalVatAmountBeforeDiscount;
        private decimal _VatFactor;
        private decimal _FinalUnitPrice;
        private decimal _TotalDiscount;
        private eMinistryVatCategoryCode _ItemVatCategoryMinistryCode;
        private int _IsOffer;
        private string _OfferDescription;
        private bool _IsInvalid;
        private Guid? _Reason;
        private string _WithdrawDepositTaxCode;
        private Guid _PriceCatalog;
        private bool _DoesNotAllowDiscount;
        private bool _IsTax;
        private string _ItemExtraInfoDescription;



        /// <summary>
        /// Gets or sets the Document Header of the current detail
        /// </summary>
        [Association("DocumentHeader-DocumentDetails")]
        [Indexed(Unique = false)]
        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
                if (_DocumentHeader != null)
                {
                    this.DocumentHeaderOid = _DocumentHeader.Oid;
                }
            }
        }

        public Guid DocumentHeaderOid
        {
            get
            {
                return _DocumentHeaderOid;
            }
            set
            {
                SetPropertyValue("DocumentHeaderOid", ref _DocumentHeaderOid, value);
            }
        }


        /// <summary>
        /// Gets or sets the Code of the special item
        /// </summary>
        public string SpecialItemCode
        {
            get
            {
                return _SpecialItemCode;
            }
            set
            {
                SetPropertyValue("SpecialItemCode", ref _SpecialItemCode, value);
            }
        }


        /// <summary>
        /// Gets or sets the unique identifier of the special Item
        /// </summary>
        [DenormalizedField("SpecialItemCode", typeof(ITS.POS.Model.Master.Item), "ITS.Retail.Model.SpecialItem", "Code")]
        public Guid SpecialItem
        {
            get
            {
                return _SpecialItem;
            }
            set
            {
                SetPropertyValue("SpecialItem", ref _SpecialItem, value);
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the Item
        /// </summary>
        [DenormalizedField("ItemCode", typeof(ITS.POS.Model.Master.Item), "ITS.Retail.Model.Item", "Code")]
        public Guid Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        /// <summary>
        /// Gets or sets the code of the Item
        /// </summary>
        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
            }
        }


        /// <summary>
        /// Gets  the Item
        /// </summary>
        public Item ItemRelation(UnitOfWork uow)
        {
            return uow.GetObjectByKey<Item>(this.Item);
        }


        /// <summary>
        /// Gets or sets the description of the Item
        /// </summary>
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                SetPropertyValue("ItemName", ref _ItemName, value);
            }
        }


        /// <summary>
        /// Gets or sets the Ministry vat category code
        /// </summary>
        public eMinistryVatCategoryCode ItemVatCategoryMinistryCode
        {
            get
            {
                return _ItemVatCategoryMinistryCode;
            }
            set
            {
                SetPropertyValue("ItemVatCategoryMinistryCode", ref _ItemVatCategoryMinistryCode, value);
            }
        }


        /// <summary>
        /// Gets or sets the description of the Item's vat category
        /// </summary>
        public string ItemVatCategoryDescription
        {
            get
            {
                return _ItemVatCategory;
            }
            set
            {
                SetPropertyValue("ItemVatCategory", ref _ItemVatCategory, value);
            }
        }



        /// <summary>
        /// Gets or sets the unique identifier of the Barcode
        /// </summary>
        [DenormalizedField("BarcodeCode", typeof(ITS.POS.Model.Master.Barcode), "ITS.Retail.Model.Barcode", "Code")]
        public Guid Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

        /// <summary>
        /// Gets or sets the Barcode
        /// </summary>
        public string BarcodeCode
        {
            get
            {
                return _BarcodeCode;
            }
            set
            {
                SetPropertyValue("BarcodeCode", ref _BarcodeCode, value);
            }
        }


        /// <summary>
        /// Gets or sets the Price derived form the pricelist 
        /// </summary>
        public decimal PriceListUnitPrice
        {
            get
            {
                return _PriceListUnitPrice;
            }
            set
            {
                SetPropertyValue("PriceListUnitPrice", ref _PriceListUnitPrice, value);
            }
        }

        /// <summary>
        /// Gets or sets the Unit price of the item
        /// </summary>
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
            }
        }


        /// <summary>
        /// Gets or sets the 
        /// Quantity
        /// </summary>
        public decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _Qty, value);
            }
        }

        [DenormalizedField("MeasurementUnitCode", typeof(MeasurementUnit), "ITS.Retail.Model.MeasurementUnit", "Code")]

        /// <summary>
        /// Gets or sets the unique identifier of the Measurement Unit
        /// </summary>
        public Guid MeasurementUnit
        {
            get
            {
                return _MeasurementUnit;
            }
            set
            {
                SetPropertyValue("MeasurementUnit", ref _MeasurementUnit, value);
            }
        }

        /// <summary>
        /// Gets or sets the Code of the Measurement Unit
        /// </summary>
        public string MeasurementUnitCode
        {
            get
            {
                return _MeasurementUnitCode;
            }
            set
            {
                SetPropertyValue("MeasurementUnitCode", ref _MeasurementUnitCode, value);
            }
        }


        /// <summary>
        /// Gets or sets the Total vat amount before any discount
        /// </summary>
        public decimal TotalVatAmountBeforeDiscount
        {
            get
            {
                return _TotalVatAmountBeforeDiscount;
            }
            set
            {
                SetPropertyValue("TotalVatAmountBeforeDiscount", ref _TotalVatAmountBeforeDiscount, value);
            }
        }


        /// <summary>
        /// Gets or sets the vat factor in the range 0..1
        /// </summary>
        public decimal VatFactor
        {
            get
            {
                return _VatFactor;
            }
            set
            {
                SetPropertyValue("VatFactor", ref _VatFactor, value);
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier Vat factor
        /// </summary>
        [DenormalizedField("VatFactorCode", typeof(VatFactor), "ITS.Retail.Model.VatFactor", "Code")]
        public Guid? VatFactorGuid
        {
            get
            {
                return _VatFactorGuid;
            }
            set
            {
                SetPropertyValue("VatFactorGuid", ref _VatFactorGuid, value);
            }
        }

        /// <summary>
        /// Gets or sets the code of the vat factor
        /// </summary>
        public string VatFactorCode
        {
            get
            {
                return _VatFactorCode;
            }
            set
            {
                SetPropertyValue("VatFactorCode", ref _VatFactorCode, value);
            }
        }

        /// <summary>
        /// Gets or sets the price from the price catalog, 
        /// </summary>
        public bool PriceCatalogValueVatIncluded
        {
            get
            {
                return _PriceCatalogValueVatIncluded;
            }
            set
            {
                SetPropertyValue("PriceCatalogValueVatIncluded", ref _PriceCatalogValueVatIncluded, value);
            }
        }

        /// <summary>
        /// Gets or sets the price from the price catalog, 
        /// </summary>
        public bool FromScanner
        {
            get
            {
                return _FromScanner;
            }
            set
            {
                SetPropertyValue("FromScanner", ref _FromScanner, value);
            }
        }



        /// <summary>
        /// Gets or sets the Final unit price
        /// </summary>
        public decimal FinalUnitPrice
        {
            get
            {
                return _FinalUnitPrice;
            }
            set
            {
                SetPropertyValue("FinalUnitPrice", ref _FinalUnitPrice, value);
            }
        }


        /// <summary>
        /// Gets or sets the Total Discount in the document detail
        /// </summary>
        public decimal TotalDiscount
        {
            get
            {
                return _TotalDiscount;
            }
            set
            {
                SetPropertyValue("TotalDiscount", ref _TotalDiscount, value);
            }
        }


        /// <summary>
        /// Gets or sets if the current line is offer 
        /// </summary>
        public int IsOffer
        {
            get
            {
                return _IsOffer;
            }
            set
            {
                SetPropertyValue("IsOffer", ref _IsOffer, value);
            }
        }


        /// <summary>
        /// Gets or sets the offer description
        /// </summary>
        public string OfferDescription
        {
            get
            {
                return _OfferDescription;
            }
            set
            {
                SetPropertyValue("OfferDescription", ref _OfferDescription, value);
            }
        }


        /// <summary>
        [Size(SizeAttribute.Unlimited)]
        public string POSGeneratedPriceCatalogDetailSerialized
        {
            get
            {
                return _POSGeneratedPriceCatalogDetailSerialized;
            }
            set
            {
                SetPropertyValue("POSGeneratedPriceCatalogDetailSerialized", ref _POSGeneratedPriceCatalogDetailSerialized, value);
            }
        }


        /// Gets or sets if the current document detail is canceled
        /// </summary>
        [Indexed(Unique = false)]
        public bool IsCanceled
        {
            get
            {
                return _IsCanceled;
            }
            set
            {
                SetPropertyValue("IsCanceled", ref _IsCanceled, value);
            }
        }

        /// <summary>
        /// Gets or sets if the current document detail is a returned object
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return _IsReturn;
            }
            set
            {
                SetPropertyValue("IsReturn", ref _IsReturn, value);
            }
        }

        /// <summary>
        /// Gets or sets the Description of the item 
        /// </summary>
        public string CustomDescription
        {
            get
            {
                return _CustomDescription;
            }
            set
            {
                SetPropertyValue("CustomDescription", ref _CustomDescription, value);
            }
        }

        /// <summary>
        /// Gets or sets if the document detail has custom price
        /// </summary>
        public bool HasCustomPrice
        {
            get
            {
                return _HasCustomPrice;
            }
            set
            {
                SetPropertyValue("HasCustomPrice", ref _HasCustomPrice, value);
            }
        }

        /// <summary>
        /// Gets or sets if the document detail has custom description
        /// </summary>
        public bool HasCustomDescription
        {
            get
            {
                return _HasCustomDescription;
            }
            set
            {
                SetPropertyValue("HasCustomDescription", ref _HasCustomDescription, value);
            }
        }

        /// <summary>
        /// Gets or sets the unique
        /// </summary>
        public Guid LinkedLine
        {
            get
            {
                return _LinkedLine;
            }
            set
            {
                SetPropertyValue("LinkedLine", ref _LinkedLine, value);
            }
        }

        /// <summary>
        /// Gets or sets the Net Total amount before any Discount
        /// </summary>
        public decimal NetTotalBeforeDiscount
        {
            get
            {
                return _NetTotalBeforeDiscount;
            }
            set
            {
                SetPropertyValue("NetTotalBeforeDiscount", ref _NetTotalBeforeDiscount, value);
            }
        }

        /// <summary>
        /// Gets or sets the Gross Total amount before any Discount
        /// </summary>
        public decimal GrossTotalBeforeDiscount
        {
            get
            {
                return _GrossTotalBeforeDiscount;
            }
            set
            {
                SetPropertyValue("GrossTotalBeforeDiscount", ref _GrossTotalBeforeDiscount, value);
            }
        }


        /// <summary>
        /// Gets or sets the Net Total amount after all Discounts (including the document discounts)
        /// </summary>
        public decimal NetTotal
        {
            get
            {
                return _NetTotal;
            }
            set
            {
                SetPropertyValue("NetTotal", ref _NetTotal, value);
            }
        }

        /// <summary>
        /// Gets or sets the Total vat amount after all Discounts (including the document discounts)
        /// </summary>
        public decimal TotalVatAmount
        {
            get
            {
                return _TotalVatAmount;
            }
            set
            {
                SetPropertyValue("TotalVatAmount", ref _TotalVatAmount, value);
            }
        }

        /// <summary>
        /// Gets or sets the Gross Total amount after all Discounts (including the document discounts)
        /// </summary>
        public decimal GrossTotal
        {
            get
            {
                return _GrossTotal;
            }
            set
            {
                SetPropertyValue("GrossTotal", ref _GrossTotal, value);
            }
        }

        /// <summary>
        /// Gets or sets the Remarks of the document line
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Custom measurement unit
        /// </summary>
        public string CustomMeasurementUnit
        {
            get
            {
                return _CustomMeasurementUnit;
            }
            set
            {
                SetPropertyValue("CustomMeasurementUnit", ref _CustomMeasurementUnit, value);
            }
        }

        /// <summary>
        /// Gets or sets the Unit price of the document detail
        /// </summary>
        public decimal CustomUnitPrice
        {
            get
            {
                return _CustomUnitPrice;
            }
            set
            {
                SetPropertyValue("CustomUnitPrice", ref _CustomUnitPrice, value);
            }
        }

        public decimal NetTotalDeviation
        {
            get
            {
                return _NetTotalDeviation;
            }
            set
            {
                SetPropertyValue("NetTotalDeviation", ref _NetTotalDeviation, value);
            }
        }

        public decimal TotalVatAmountDeviation
        {
            get
            {
                return _TotalVatAmountDeviation;
            }
            set
            {
                SetPropertyValue("TotalVatAmountDeviation", ref _TotalVatAmountDeviation, value);
            }
        }

        public decimal GrossTotalDeviation
        {
            get
            {
                return _GrossTotalDeviation;
            }
            set
            {
                SetPropertyValue("GrossTotalDeviation", ref _GrossTotalDeviation, value);
            }
        }


        public decimal GrossTotalBeforeDocumentDiscount
        {
            get
            {
                return _GrossTotalBeforeDocumentDiscount;
            }
            set
            {
                SetPropertyValue("GrossTotalBeforeDocumentDiscount", ref _GrossTotalBeforeDocumentDiscount, value);
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType);
            dictionary.Add("DocumentDetailDiscounts", DocumentDetailDiscounts.Select(g => g.GetDict(settings, includeType)).ToList());
            return dictionary;
        }

        [Aggregated, Association("DocumentDetail-DocumentDetailDiscounts")]
        public XPCollection<DocumentDetailDiscount> DocumentDetailDiscounts
        {
            get
            {
                return GetCollection<DocumentDetailDiscount>("DocumentDetailDiscounts");
            }
        }

        public IEnumerable<DocumentDetailDiscount> NonHeaderDocumentDetailDiscounts
        {
            get
            {
                return this.DocumentDetailDiscounts.Where(x =>
                                x.DiscountSource != eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT &&
                                x.DiscountSource != eDiscountSource.DOCUMENT &&
                                x.DiscountSource != eDiscountSource.POINTS &&
                                x.DiscountSource != eDiscountSource.CUSTOMER &&
                                x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);
            }

        }

        /// <summary>
        /// Calculated. All the line discounts that are not applied to the document header
        /// </summary>
        public decimal TotalNonDocumentDiscount
        {
            get
            {

                var nonDocumentDiscounts = this.NonHeaderDocumentDetailDiscounts;
                if (nonDocumentDiscounts.Any())
                {
                    return nonDocumentDiscounts.Sum(x => x.Value);
                }

                return 0;

            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal TotalDiscountIncludingVAT
        {
            get
            {
                return GrossTotalBeforeDiscount - GrossTotal;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal FinalUnitPriceWithVat
        {
            get
            {
                return FinalUnitPrice;
            }
        }


        /// <summary>
        /// Calculated
        /// </summary>
        public decimal FinalUnitPriceWithVatBeforeDocumentDiscount
        {
            get
            {
                if (Qty == 0)
                {
                    return 0;
                }
                return GrossTotalBeforeDocumentDiscount / (decimal)Qty;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal TotalDiscountPercentageWithVat
        {
            get
            {
                if (this.GrossTotal == 0 || this.GrossTotalBeforeDiscount == 0)
                {
                    return 0;
                }
                return (this.GrossTotalBeforeDiscount - GrossTotal) / (GrossTotalBeforeDiscount);

            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PointsDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.POINTS);

                if (docDiscount != null)
                {
                    return docDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PointsDiscountAmount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.POINTS);

                if (docDiscount != null)
                {
                    return docDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PromotionsDocumentDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);

                if (docDiscount != null)
                {
                    return docDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PromotionsDocumentDiscountAmount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);

                if (docDiscount != null)
                {
                    return docDiscount.Value;
                }

                return 0.0m;
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

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PromotionsLineDiscountsAmount
        {
            get
            {
                var promotionLineDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.PROMOTION_LINE_DISCOUNT);
                if (promotionLineDiscounts.Any())
                {
                    return promotionLineDiscounts.Sum(x => x.Value);
                }

                return 0;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal NetTotalBeforeDocumentDiscount
        {
            get
            {

                return this.NetTotalBeforeDiscount - this.TotalDiscount + this.DocumentDiscountAmount;
            }
        }

        /// <summary>
        /// Calculated. All the line discounts except the discounts that apply to the header
        /// </summary>
        public decimal TotalNonDocumentDiscountPercentage
        {
            get
            {

                if (this.NetTotal == 0)
                {
                    return 0;
                }
                decimal totalDiscountWithoutDoc = this.TotalDiscount - this.DocumentDiscountAmount - this.PointsDiscountAmount - this.CustomerDiscountAmount - this.DefaultDocumentDiscountAmount;
                return (totalDiscountWithoutDoc) / (NetTotal + TotalDiscount);

            }
        }

        /// <summary>
        /// Calculated. All the line discounts except the document discount with vat included
        /// </summary>
        public decimal TotalNonDocumentDiscountPercentageWithVat
        {
            get
            {

                if (this.GrossTotal == 0)
                {
                    return 0;
                }
                decimal totalDiscountWithoutDoc = this.TotalDiscount - this.DocumentDiscountAmount - this.PointsDiscountAmount - this.CustomerDiscountAmount - this.DefaultDocumentDiscountAmount;
                return Math.Round((totalDiscountWithoutDoc) / (GrossTotal + TotalDiscount), 4, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Calculated. Gets all the discounts that must be shown as "Document Header Discounts".
        /// </summary>
        public decimal AllDocumentHeaderDiscounts
        {
            get
            {
                return this.PointsDiscountAmount + this.DocumentDiscountAmount + this.CustomerDiscountAmount + this.PromotionsDocumentDiscountAmount + DefaultDocumentDiscountAmount;

            }

        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal TotalDiscountPercentage
        {
            get
            {
                if (NetTotal + TotalDiscountAmountWithoutVAT == 0)
                {
                    return 0;
                }
                return (this.TotalDiscountAmountWithoutVAT) / (NetTotal + TotalDiscountAmountWithoutVAT);

            }
        }


        /// <summary>
        /// Calculated
        /// </summary>

        public bool IsLinkedLine
        {
            get
            {
                return LinkedLine != Guid.Empty;

            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal CustomDiscountsAmount
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Any())
                {
                    return customDiscounts.Sum(x => x.Value);
                }

                return 0;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal CustomerDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount custDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.CUSTOMER);

                if (custDiscount != null)
                {
                    return custDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal CustomerDiscountAmount
        {
            get
            {
                DocumentDetailDiscount customerDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.CUSTOMER);

                if (customerDiscount != null)
                {
                    return customerDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PriceCatalogDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount pcDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG);

                if (pcDiscount != null)
                {
                    return pcDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal PriceCatalogDiscountAmount
        {
            get
            {
                DocumentDetailDiscount pcDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG);

                if (pcDiscount != null)
                {
                    return pcDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal DocumentDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);

                if (docDiscount != null)
                {
                    return docDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal DocumentDiscountAmount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);

                if (docDiscount != null)
                {
                    return docDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal DefaultDocumentDiscountAmount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);

                if (docDiscount != null)
                {
                    return docDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal UnitPriceAfterDiscount
        {
            get
            {
                if (this.Qty == 0)
                {
                    return 0;
                }
                return this.NetTotal / this.Qty;
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

        public decimal PackingQuantity
        {
            get
            {
                return _PackingQuantity;
            }
            set
            {
                SetPropertyValue("PackingMeasurementUnit", ref _PackingQuantity, value);
            }
        }


        public Guid PackingMeasurementUnit
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


        public double PackingMeasurementUnitRelationFactor
        {
            get
            {
                return _PackingMeasurementUnitRelationFactor;
            }
            set
            {
                SetPropertyValue("PackingMeasurementUnitRelationFactor", ref _PackingMeasurementUnitRelationFactor, value);
            }
        }

        public int LineNumber
        {
            get
            {
                return _LineNumber;
            }
            set
            {
                SetPropertyValue("LineNumber", ref _LineNumber, value);
            }
        }


        public Guid? PriceCatalogDetail
        {
            get
            {
                return _PriceCatalogDetail;
            }
            set
            {
                SetPropertyValue("PriceCatalogDetail", ref _PriceCatalogDetail, value);
            }
        }

        [NonPersistent]
        public decimal CurrentPromotionDiscountValue { get; set; }


        IEnumerable<IDocumentDetailDiscount> IDocumentDetail.DocumentDetailDiscounts
        {
            get { return DocumentDetailDiscounts; }
        }

        public Guid ItemOid
        {
            get { return Item; }
        }

        public decimal TotalDiscountAmountWithVAT
        {
            get
            {
                return _TotalDiscountAmountWithVAT;
            }
            set
            {
                SetPropertyValue("TotalDiscountAmountWithVAT", ref _TotalDiscountAmountWithVAT, value);
            }
        }

        public decimal TotalDiscountAmountWithoutVAT
        {
            get
            {
                return _TotalDiscountAmountWithoutVAT;
            }
            set
            {
                SetPropertyValue("TotalDiscountAmountWithoutVAT", ref _TotalDiscountAmountWithoutVAT, value);
            }
        }

        public DocumentDetailDiscount PriceCatalogDetailDiscount
        {
            get
            {
                return DocumentDetailDiscounts.Where(documentDetailDiscount => documentDetailDiscount.DiscountSource == eDiscountSource.PRICE_CATALOG).FirstOrDefault();
            }
        }

        public IEnumerable<DocumentDetailDiscount> UserDetailDiscounts
        {
            get
            {
                return DocumentDetailDiscounts.Where(documentDetailDiscount => documentDetailDiscount.DiscountSource == eDiscountSource.CUSTOM);
            }
        }

        public DocumentDetailDiscount UserFirstDetailDiscount
        {
            get
            {
                return UserDetailDiscounts.FirstOrDefault();
            }
        }

        [NonPersistent]
        public bool IsInvalid
        {
            get
            {
                return _IsInvalid;
            }
            set
            {
                SetPropertyValue("IsInvalid", ref _IsInvalid, value);
            }
        }


        public string ItemExtraInfoDescription
        {
            get
            {
                return _ItemExtraInfoDescription;
            }
            set
            {
                SetPropertyValue("ItemExtraInfoDescription", ref _ItemExtraInfoDescription, value);
            }
        }

        public Guid? Reason
        {
            get
            {
                return _Reason;
            }
            set
            {
                SetPropertyValue("Reason", ref _Reason, value);
            }
        }

        public string WithdrawDepositTaxCode
        {
            get
            {
                return _WithdrawDepositTaxCode;
            }
            set
            {
                SetPropertyValue("WithdrawDepositTaxCode", ref _WithdrawDepositTaxCode, value);
            }
        }


        public Guid PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }


            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }

    }
}
