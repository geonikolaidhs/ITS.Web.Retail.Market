using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;


namespace ITS.Retail.WebClient.ViewModel
{
    public class DocumentDetailViewModel : IPersistableViewModel
    {
        public DocumentDetailViewModel()
        {
            Oid = Guid.NewGuid();
        }

        public Guid Oid { get; set; }

        public Type PersistedType { get { return typeof(DocumentHeader); } }

        public bool IsDeleted { get; set; }

        public Guid? PackingMeasurementUnit { get; set; }

        public string PackingMeasurementUnitDescription { get; set; }

        public Guid CentralStore { get; set; }

        public Guid LinkedLine { get; set; }

        public Guid Item { get; set; }

        public Guid SpecialItem { get; set; }

        public Guid? MeasurementUnit { get; set; }

        public string MeasurementUnitDescription { get; set; }

        public Guid Barcode { get; set; }

        public Guid DocumentHeader { get; set; }

        public double PackingMeasurementUnitRelationFactor{ get; set; }
        public decimal PackingQuantity{ get; set; }
        public int LineNumber{ get; set; }
        public decimal GrossTotalBeforeDocumentDiscount{ get; set; }
        public decimal GrossTotalDeviation{ get; set; }
        public decimal TotalVatAmountDeviation{ get; set; }
        public decimal NetTotalDeviation{ get; set; }
        public Guid VatFactorGuid{ get; set; }
        public decimal CustomUnitPrice{ get; set; }
        public string CustomMeasurementUnit{ get; set; }
        public decimal GrossTotalAfterFirstDiscount{ get; set; }
        public string Remarks{ get; set; }

        public string ItemCode{ get; set; }
        public string ItemDescription { get; set; }

        public string VatFactorCode{ get; set; }
        public string MeasurementUnitCode{ get; set; }
        public string BarcodeCode{ get; set; }
        public decimal GrossTotalBeforeDiscount{ get; set; }
        public bool HasCustomDescription{ get; set; }
        public bool HasCustomPrice{ get; set; }
        public string CustomDescription{ get; set; }
        public bool IsCanceled{ get; set; }
        public decimal PriceListUnitPrice{ get; set; }
        public decimal UnitPrice{ get; set; }
        public decimal Qty{ get; set; }
        public decimal Points{ get; set; }
        public decimal VatFactor{ get; set; }
        public decimal GrossTotal{ get; set; }
        public decimal FinalUnitPrice{ get; set; }
        public decimal TotalDiscount{ get; set; }
        public decimal TotalVatAmount{ get; set; }
        public decimal NetTotal{ get; set; }
        public int IsOffer{ get; set; }
        public string OfferDescription{ get; set; }
        public bool IsReturn{ get; set; }
        public decimal TotalVatAmountBeforeDiscount{ get; set; }
        public decimal NetTotalBeforeDiscount{ get; set; }
        public string MeasurementUnit2Code{ get; set; }
        public decimal FirstDiscount{ get; set; }
        public decimal SecondDiscount{ get; set; }

        public List<DocumentDetailDiscountViewModel> DocumentDetailDiscounts { get; set; }

        public void UpdateModel(Session uow)
        {
            this.UpdateProperties(uow);
            DocumentDetailDiscounts.ForEach(x => { x.UpdateModel(uow); x.DocumentDetail = this.Oid; });
        }

        public bool Validate(out string message)
        {
            throw new NotImplementedException();
        }


        public string MeasurementUnitsQuantities
        {
            get
            {
                if ( this.MeasurementUnit.HasValue && this.PackingMeasurementUnit.HasValue && this.MeasurementUnit.Value != this.PackingMeasurementUnit.Value)
                {
                    return String.Format("{2} {3} ~ {0} {1}", this.Qty, this.MeasurementUnitDescription, this.PackingQuantity, this.PackingMeasurementUnitDescription); 
                }
                return "";
            }
        }

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


        public decimal TotalDiscountPercentage
        {
            get
            {
                if (NetTotal == 0)
                {
                    return 0;
                }
                return (this.TotalDiscount) / (NetTotal + TotalDiscount);
            }
        }

        public decimal TotalDiscountIncludingVAT
        {
            get
            {
                return GrossTotalBeforeDiscount - GrossTotal;
            }
        }

        public decimal TotalDiscountPercentageWithVat
        {
            get
            {
                if (this.GrossTotal == 0 || this.GrossTotalBeforeDiscount == 0)
                {
                    return 0;
                }
                //return (this.GrossTotalBeforeDiscount - GrossTotal) / (GrossTotalBeforeDiscount);
                return (this.TotalDiscount) / (GrossTotal + TotalDiscount);
            }
        }

    }
}