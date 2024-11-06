using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface.Model;
using DevExpress.Xpo;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    //[CreateOrUpdaterOrder(Order = 1050, Permissions = eUpdateDirection.NONE)]
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
            _HasCustomDescription = false;
            _HasCustomPrice = false;
            NetTotalDeviation =
            TotalVatAmountDeviation =
            GrossTotalDeviation = .0m;

        }
        private bool _HasCustomDescription;
        private bool _HasCustomPrice;
        
        public Guid Barcode { get; set; }
        
        public string BarcodeCode { get; set; }

        public Guid CentralStore { get; set; }
        

        public decimal CurrentPromotionDiscountValue { get; set; }

        public string CustomDescription { get; set; }

        public string CustomMeasurementUnit { get; set; }

        public decimal CustomUnitPrice { get; set; }

        public Guid DocumentHeader { get; set; }
        
        public decimal FinalUnitPrice { get; set; }

        public decimal FirstDiscount { get; set; }

        public bool FromScanner { get; set; }

        public decimal GrossTotal { get; set; }

        public decimal GrossTotalBeforeDiscount { get; set; }

        public decimal GrossTotalBeforeDocumentDiscount { get; set; }

        public decimal GrossTotalDeviation { get; set; }

        public bool HasCustomDescription { get; set; }

        public bool HasCustomPrice { get; set; }

        public bool IsCanceled { get; set; }

        public int IsOffer { get; set; }

        public bool IsPOSGeneratedPriceCatalogDetailApplied { get; set; }

        public bool IsReturn { get; set; }

        public string ItemCode { get; set; }

        public int LineNumber { get; set; }

        public Guid LinkedLine { get; set; }

        public Guid MeasurementUnit { get; set; }
        
        public string MeasurementUnitCode { get; set; }

        public decimal NetTotal { get; set; }

        public decimal NetTotalBeforeDiscount { get; set; }

        public decimal NetTotalDeviation { get; set; }

        public string OfferDescription { get; set; }

        public Guid PackingMeasurementUnit { get; set; }
        
        public double PackingMeasurementUnitRelationFactor { get; set; }

        public decimal PackingQuantity { get; set; }

        public decimal Points { get; set; }

        public string POSGeneratedPriceCatalogDetailSerialized { get; set; }

        public decimal PriceListUnitPrice { get; set; }

        public decimal PriceListUnitPriceWithoutVAT { get; set; }

        public decimal PriceListUnitPriceWithVAT { get; set; }

        public decimal PromotionsLineDiscountsAmount { get; set; }

        public decimal Qty { get; set; }

        public Guid Reason { get; set; }
       
        public string Remarks { get; set; }

        public decimal SecondDiscount { get; set; }

        public Guid SpecialItem { get; set; }
        
        public decimal TotalDiscount { get; set; }

        public decimal TotalDiscountAmountWithoutVAT { get; set; }

        public decimal TotalDiscountAmountWithVAT { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal TotalVatAmountBeforeDiscount { get; set; }

        public decimal TotalVatAmountDeviation { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal VatFactor { get; set; }

        public string VatFactorCode { get; set; }

        public Guid VatFactorGuid { get; set; }

        public string _easurementUnit2Code { get; set; }

        public string _ithdrawDepositTaxCode { get; set; }
        [NonPersistent]
        public IItem Item
        {
            get
            {
                return this.Item;
            }

            set
            {
                this.Item = value as IItem;
            }
        }
        [NonPersistent]
        public Guid DocumentHeaderOid { get; set; }

        IStore IDocumentDetail.CentralStore
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IMeasurementUnit IDocumentDetail.MeasurementUnit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentHeader IDocumentDetail.DocumentHeader
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IMeasurementUnit IDocumentDetail.PackingMeasurementUnit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ISpecialItem IDocumentDetail.SpecialItem
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IReason IDocumentDetail.Reason
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IBarcode IDocumentDetail.Barcode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}