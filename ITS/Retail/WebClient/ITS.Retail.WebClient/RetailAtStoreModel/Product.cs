using ITS.MobileAtStore.ObjectModel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.RetailAtStoreModel
{
    public class Product
    {

        public string MeasurementUnitText { get; set; }
        public decimal Points { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal CalculatedTotalPrice { get; set; }
        public decimal PricePerMeasurementUnit { get; set; }

        public Offer[] RelatedActions { get; set; }
        public Product ChainProduct { get; set; }
        //private decimal _chainProductQuantity;
        public decimal ChainProductQuantity { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int VatCat { get; set; }
        public string Supplier { get; set; }
        public string BasicSupplier { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsActiveOnSupplier { get; set; }
        public string ExtraCode { get; set; }
        public bool ExtraCodeIsActive { get; set; }
        public string GivenCode
        {
            get
            {
                return !string.IsNullOrEmpty(Barcode) ? Barcode : Code;
            }
        }


        public decimal RequiredQuantity { get; set; }
        public decimal RestInvQty { get; set; }
        public decimal Stock { get; set; }
        public decimal AverageMonthSales { get; set; }
        public decimal OrderMM { get; set; }
        public string extrainfo { get; set; }
        public int BasicSupplierColor { get; set; }
        public decimal Quantity { get; set; }
        public BarcodeParsingResult BarcodeParsingResult { get; set; }
        //private string _WeightedDecodedBarcode;
        public string WeightedDecodedBarcode { get; set; }

        private List<ProductQuantity> _productQuantities;
        /// <summary>
        /// The required quantities that have the same maternal code in the document.
        /// </summary>
        public List<ProductQuantity> ProductQuantities
        {
            get
            {
                return _productQuantities;
            }
            set
            {
                _productQuantities = value;
            }
        }

        public bool SupportsDecimalQuantities { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Barcode))
            {
                return string.Format("{0}\r\n{1}", new object[] { (!string.IsNullOrEmpty(Code) ? this.Code.TrimStart('0') : string.Empty), this.Description });
            }
            else
            {
                return string.Format(
                    "{0}\r\n{1}\r\n{2}",
                    new object[] {
                        (!string.IsNullOrEmpty(Code) ? this.Code.TrimStart('0') : string.Empty),
                        (!string.IsNullOrEmpty(Barcode) ? this.Barcode.TrimStart('0') : string.Empty),
                        this.Description
                    });
            }
        }
    }
}