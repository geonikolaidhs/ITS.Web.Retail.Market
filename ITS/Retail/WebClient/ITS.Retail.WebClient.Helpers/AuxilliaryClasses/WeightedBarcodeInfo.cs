using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{
    public class WeightedBarcodeInfo
    {
        public string DecodedCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public string PluCode { get; set; }
        public BarcodeParsingResult BarcodeParsingResult { get; set; }
        public Guid? ItemBarcode { get; set; }
        public Guid? Barcode { get; set; }
        public Guid? PriceCatalogDetail { get; set; }

        public WeightedBarcodeInfo(BarcodeParseResult barcodeParseResult)
        {
            this.DecodedCode = barcodeParseResult.DecodedCode;
            this.Quantity = barcodeParseResult.Quantity;
            this.Value = barcodeParseResult.CodeValue;
            this.PluCode = barcodeParseResult.PLU;
            this.BarcodeParsingResult = barcodeParseResult.BarcodeParsingResult;
            this.ItemBarcode = null;
            this.Barcode = null;
            this.PriceCatalogDetail = null;
        }
    }
}
