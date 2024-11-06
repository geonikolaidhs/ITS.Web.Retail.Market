//-----------------------------------------------------------------------
// <copyright file="DocumentDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.ResourcesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.Model
{
    [Indices("GCRecord;Item;IsCanceled", "GCRecord;DocumentHeader;IsCanceled", "GCRecord;DocumentHeader;Oid",
             "GCRecord;DocumentHeader;Item;IsCanceled", "GCRecord;IsCanceled;Item;Qty;DocumentHeader",
             "DocumentHeader;GCRecord;IsCanceled;Item;Qty", "DocumentHeader;GCRecord;CentralStore"
             )]
    public class
        DocumentDetail : BaseObj, IDocumentDetail
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
            _HasCustomDescription = false;
            _HasCustomPrice = false;
            NetTotalDeviation =
            TotalVatAmountDeviation =
            GrossTotalDeviation = .0m;

        }


        private MeasurementUnit _PackingMeasurementUnit;
        private Store _CentralStore;
        private Guid _LinkedLine;
        private Item _Item;
        private SpecialItem _SpecialItem;
        private MeasurementUnit _MeasurementUnit;
        private Barcode _Barcode;
        private DocumentHeader _DocumentHeader;
        private double _PackingMeasurementUnitRelationFactor;
        private decimal _PackingQuantity;
        private int _LineNumber;
        private decimal _GrossTotalBeforeDocumentDiscount;
        private decimal _GrossTotalDeviation;
        private decimal _TotalVatAmountDeviation;
        private decimal _NetTotalDeviation;
        private Guid _VatFactorGuid;
        private decimal _CustomUnitPrice;
        private string _CustomMeasurementUnit;
        //private decimal _GrossTotalAfterFirstDiscount;
        private string _Remarks;
        private string _ItemCode;
        private string _VatFactorCode;
        private string _MeasurementUnitCode;
        private string _BarcodeCode;
        private decimal _GrossTotalBeforeDiscount;
        private bool _HasCustomDescription;
        private bool _HasCustomPrice;
        private string _CustomDescription;
        private bool _IsCanceled;
        private decimal _PriceListUnitPrice;
        private decimal _UnitPrice;
        private decimal _Qty;
        private decimal _Points;
        private decimal _VatFactor;
        private decimal _GrossTotal;
        private decimal _FinalUnitPrice;
        private decimal _TotalDiscount;
        private decimal _TotalVatAmount;
        private decimal _NetTotal;
        private int _IsOffer;
        private string _OfferDescription;
        private bool _IsReturn;
        private decimal _TotalVatAmountBeforeDiscount;
        private decimal _NetTotalBeforeDiscount;
        private string _MeasurementUnit2Code;
        private decimal _FirstDiscount;
        private decimal _SecondDiscount;
        private Guid _DocumentHeaderOid;
        private bool _FromScanner;
        private string _POSGeneratedPriceCatalogDetailSerialized;
        private bool _IsPOSGeneratedPriceCatalogDetailApplied;
        private decimal _TotalDiscountAmountWithVAT;
        private decimal _TotalDiscountAmountWithoutVAT;
        private decimal _CurrentPromotionDiscountValue;
        private decimal _PromotionsLineDiscountsAmount;
        private string _WithdrawDepositTaxCode;
        private Reason _Reason;
        private decimal _PriceListUnitPriceWithVAT;
        private decimal _PriceListUnitPriceWithoutVAT;
        private Guid _PriceCatalog;
        private double _BaseQuantity;
        private MeasurementUnit _BaseMeasurementUnit;
        private double _PreviousBaseQuantity;
        private bool _LockPreviousBaseQuantity;
        private bool _DoesNotAllowDiscount;
        private bool _IsTax;

        public SpecialItem SpecialItem
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

        [Indexed("GCRecord", Unique = false)]
        public Store CentralStore
        {
            get
            {
                return _CentralStore;
            }
            set
            {
                SetPropertyValue("CentralStore", ref _CentralStore, value);
            }
        }
        [Indexed(Unique = false), Association("Item-DocumentDetails")]
        public Item Item
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

        [Indexed(Unique = false)]
        public Barcode Barcode
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

        public decimal PriceListUnitPriceWithVAT
        {
            get
            {
                return _PriceListUnitPriceWithVAT;
            }
            set
            {
                SetPropertyValue("PriceListUnitPriceWithVAT", ref _PriceListUnitPriceWithVAT, value);
            }
        }

        public decimal PriceListUnitPriceWithoutVAT
        {
            get
            {
                return _PriceListUnitPriceWithoutVAT;
            }
            set
            {
                SetPropertyValue("PriceListUnitPriceWithoutVAT", ref _PriceListUnitPriceWithoutVAT, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "UnitPrice", ResourceType = typeof(Resources))]
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


        [System.ComponentModel.DataAnnotations.Display(Name = "Quantity", ResourceType = typeof(Resources))]
        [Indexed(Unique = false)]
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



        [UpdaterIgnoreField]
        public bool IsPOSGeneratedPriceCatalogDetailApplied
        {
            get
            {
                return _IsPOSGeneratedPriceCatalogDetailApplied;
            }
            set
            {
                SetPropertyValue("IsPOSGeneratedPriceCatalogDetailApplied", ref _IsPOSGeneratedPriceCatalogDetailApplied, value);
            }
        }

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

        public MeasurementUnit MeasurementUnit
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

        public string MeasurementUnit2Code
        {
            get
            {
                return _MeasurementUnit2Code;
            }
            set
            {
                SetPropertyValue("MeasurementUnit2Code", ref _MeasurementUnit2Code, value);
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

        public decimal FirstDiscount
        {
            get
            {
                return _FirstDiscount;
            }
            set
            {
                SetPropertyValue("FirstDiscount", ref _FirstDiscount, value);
            }
        }


        public decimal SecondDiscount
        {
            get
            {
                return _SecondDiscount;
            }
            set
            {
                SetPropertyValue("SecondDiscount", ref _SecondDiscount, value);
            }
        }


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


        public Guid VatFactorGuid
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

        [System.ComponentModel.DataAnnotations.Display(Name = "TotalDiscount", ResourceType = typeof(Resources))]
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



        [Association("DocumentHeader-DocumentDetails"), Indexed(Unique = false)]
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

        [Aggregated, Association("DocumentDetail-RelativeDocumentDetail-DerivedDocumentDetail", typeof(RelativeDocumentDetail))]
        public XPCollection<RelativeDocumentDetail> ReferencedRelativeDocumentDetails
        {
            get
            {
                return GetCollection<RelativeDocumentDetail>("ReferencedRelativeDocumentDetails");
            }
        }

        public IEnumerable<DocumentDetail> InitialDocumentDetails
        {
            get
            {
                if (ReferencedRelativeDocumentDetails.Count > 0)
                {
                    return ReferencedRelativeDocumentDetails.Select(rdd => rdd.InitialDocumentDetail);
                }
                else
                {
                    return new List<DocumentDetail>();
                }
            }
        }

        [Aggregated, Association("DocumentDetail-RelativeDocumentDetail-InitialDocumentDetail")]
        public XPCollection<RelativeDocumentDetail> DerivedRelativeDocumentDetails
        {
            get
            {
                return GetCollection<RelativeDocumentDetail>("DerivedRelativeDocumentDetails");
            }
        }


        public IEnumerable<DocumentDetail> DerivedDocumentDetails
        {
            get
            {
                if (DerivedRelativeDocumentDetails.Count > 0)
                {
                    return DerivedRelativeDocumentDetails.Select(rdd => rdd.DerivedDocumentDetail);
                }
                else
                {
                    return new List<DocumentDetail>();
                }
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Description", ResourceType = typeof(Resources))]
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

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "DocumentHeader" || propertyName == "Item" || propertyName == "Qty")
            {
                if (DocumentHeader == null)
                {
                    return;
                }

                if (propertyName == "Item" && Item != null)
                {
                    _HasCustomDescription = Item.AcceptsCustomDescription;
                    _HasCustomPrice = Item.AcceptsCustomPrice;
                }

                _CentralStore = DocumentHeader.Store;
                if (DocumentHeader.Division != Platform.Enumerations.eDivision.Sales ||
                    (DocumentHeader.Customer != null && !DocumentHeader.Customer.BreakOrderToCentral))
                {
                    return;
                }

                if (Item != null)
                {
                    if (LinkedLine == Guid.Empty)
                    {
                        if (Item.IsCentralStored && Qty >= (decimal)Item.OrderQty)
                        {
                            Store itemCentralStore = Item.GetCentralStore(DocumentHeader.Store.Owner);
                            if (itemCentralStore != null)
                            {
                                _CentralStore = itemCentralStore;
                            }
                        }
                    }
                    else
                    {
                        BindingSource details = new BindingSource(DocumentHeader.DocumentDetails, "");
                        int pos = details.Find("Oid", LinkedLine);
                        if (pos >= 0)
                        {
                            _CentralStore = DocumentHeader.DocumentDetails[pos].CentralStore;
                        }
                    }
                    return;
                }

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

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);
            if (includeDetails)
            {
                dictionary.Add("DocumentDetailDiscounts", DocumentDetailDiscounts.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
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

        public decimal GetCustomDiscountsPercentage(bool isWholeSale)
        {
            return isWholeSale ? this.CustomDiscountsPercentageWholeSale : this.CustomDiscountsPercentageRetail;
        }

        /// <summary>
        /// Calculated. Gets all the discounts that must be shown as "Document Header Discounts".
        /// </summary>
        public decimal AllDocumentHeaderDiscounts
        {
            get
            {
                return this.PointsDiscountAmount + this.DocumentDiscountAmount + this.CustomerDiscountAmount;
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
                if (Qty == 0)
                {
                    return 0;
                }
                return GrossTotalBeforeDiscount / (decimal)Qty;

            }
        }

        /// <summary>
        /// Calculated. All the line discounts that are not applied to the document header
        /// </summary>
        public decimal TotalNonDocumentDiscount
        {
            get
            {
                var nonDocumentDiscounts = this.DocumentDetailDiscounts
                    .Where(x => x.DiscountSource != eDiscountSource.DOCUMENT &&
                                x.DiscountSource != eDiscountSource.POINTS &&
                                x.DiscountSource != eDiscountSource.CUSTOMER &&
                                x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);
                if (nonDocumentDiscounts.Count() > 0)
                {
                    return nonDocumentDiscounts.Sum(x => x.Value);
                }

                return 0;
            }
        }



        /// <summary>
        /// Calculated
        /// </summary>
        public decimal TotalDiscountPercentageWithVat
        {
            get
            {
                if (GrossTotal + TotalDiscountAmountWithVAT == 0)
                {
                    return 0;
                }
                return (this.TotalDiscountAmountWithVAT) / (GrossTotal + TotalDiscountAmountWithVAT);
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
        public decimal UnreferencedQuantity
        {
            get
            {
                decimal unreferenceQuantity = Qty;

                if (DerivedRelativeDocumentDetails != null && DerivedRelativeDocumentDetails.Count > 0)
                {
                    foreach (RelativeDocumentDetail derdt in DerivedRelativeDocumentDetails)
                    {
                        unreferenceQuantity -= derdt.Qty;
                    }
                }

                return unreferenceQuantity;
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
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.Sum(x => x.Value);
                }

                return 0;
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
        public decimal NetTotalBeforeDocumentDiscount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);

                if (docDiscount != null)
                {
                    return this.NetTotal - docDiscount.Value;
                }
                else
                {
                    return this.NetTotal;
                }
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
                IEnumerable<DocumentDetailDiscount> customerDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOMER);

                if (customerDiscounts.Count() > 0)
                {
                    return customerDiscounts.Sum(x => x.Value);
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

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal CustomDiscountsPercentageWholeSale
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.OrderBy(x => x.Priority).Select(x => x.Percentage).Aggregate((first, second) => (1 + first) * (1 + second) - 1);
                }
                return 0;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        public decimal CustomDiscountsPercentageRetail
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.OrderBy(x => x.Priority).Select(x => x.Percentage).Aggregate((first, second) => (1 + first) * (1 + second) - 1);
                }
                return 0;
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


        public decimal PackingQuantity
        {
            get
            {
                return _PackingQuantity;
            }
            set
            {
                SetPropertyValue("PackingQuantity", ref _PackingQuantity, value);
            }
        }


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

        public string MeasurementUnitsQuantities
        {
            get
            {
                if ( //this.DocumentHeader!=null && this.DocumentHeader.UsesPackingQuantities
                     //&& 
                    this.MeasurementUnit != null
                    && this.PackingMeasurementUnit != null
                    && this.MeasurementUnit.Oid != this.PackingMeasurementUnit.Oid
                   )
                {
                    string quantitiesString = "";
                    quantitiesString = this.PackingQuantity.ToString();
                    if (this.PackingMeasurementUnit != null)
                    {
                        quantitiesString += " " + this.PackingMeasurementUnit.Description;
                    }
                    quantitiesString += String.Format(" ~ {0} {1}", this.Qty, this.MeasurementUnit.Description);
                    return quantitiesString;
                }
                return "";
            }
        }

        public bool ShouldBeSummarised
        {
            get
            {
                return !this.IsCanceled;
            }
        }

        public bool CanBeTransformed
        {
            get
            {
                return !this.IsLinkedLine && !this.IsCanceled && !this.IsReturn && this.SpecialItem == null && UnreferencedQuantity > 0;
            }
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


        public decimal CurrentPromotionDiscountValue
        {
            get
            {
                return _CurrentPromotionDiscountValue;
            }
            set
            {
                SetPropertyValue("CurrentPromotionDiscountValue", ref _CurrentPromotionDiscountValue, value);
            }
        }


        public decimal PromotionsLineDiscountsAmount
        {
            get
            {
                return _PromotionsLineDiscountsAmount;
            }


            set
            {
                SetPropertyValue("PromotionsLineDiscountsAmount", ref _PromotionsLineDiscountsAmount, value);
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

        public PriceCatalog PriceCatalogObject
        {
            get
            {
                if (this.PriceCatalog != Guid.Empty)
                {
                    return this.Session.GetObjectByKey<PriceCatalog>(this.PriceCatalog);
                }
                return null;
            }
        }

        public Guid ItemOid
        {
            get
            {
                if (this.Item == null)
                {
                    return Guid.Empty;
                }
                return this.Item.Oid;
            }
        }

        IEnumerable<IDocumentDetailDiscount> IDocumentDetail.DocumentDetailDiscounts
        {
            get
            {
                return DocumentDetailDiscounts;
            }
        }

        public Reason Reason
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

        public List<DocumentDetail> LinkedLines
        {
            get
            {
                return this.DocumentHeader.DocumentDetails.Where(x => x.LinkedLine == this.Oid).ToList();
            }
        }

        public double BaseQuantity
        {
            get
            {
                return _BaseQuantity;
            }
            set
            {
                if ( //this.Session.IsNewObject(this) == false
                     /*&&*/ this.LockPreviousBaseQuantity == false
                    && this.ChangedMembers.Contains(this.ClassInfo.GetMember("Qty"))
                   )
                {
                    this.PreviousBaseQuantity = this._BaseQuantity;
                    this.LockPreviousBaseQuantity = true;
                }
                SetPropertyValue("BaseQuantity", ref _BaseQuantity, value);
            }
        }

        public double PreviousBaseQuantity
        {
            get
            {
                return _PreviousBaseQuantity;
            }
            set
            {
                SetPropertyValue("PreviousBaseQuantity", ref _PreviousBaseQuantity, value);
            }
        }

        [NonPersistent, UpdaterIgnoreField]
        public bool LockPreviousBaseQuantity
        {
            get
            {
                return _LockPreviousBaseQuantity;
            }
            set
            {
                SetPropertyValue("LockPreviousBaseQuantity", ref _LockPreviousBaseQuantity, value);
            }
        }

        public MeasurementUnit BaseMeasurementUnit
        {
            get
            {
                return _BaseMeasurementUnit;
            }
            set
            {
                SetPropertyValue("BaseMeasurementUnit", ref _BaseMeasurementUnit, value);
            }
        }
    }
}
