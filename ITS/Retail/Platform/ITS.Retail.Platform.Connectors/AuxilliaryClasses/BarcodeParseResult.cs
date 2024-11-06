using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.AuxilliaryClasses
{
    public class BarcodeParseResult
    {
        public BarcodeParsingResult BarcodeParsingResult { get; set; }
        public string DecodedCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal CodeValue { get; set; }
        public string PLU { get; set; }
        public Guid? BarcodeType { get; set; }

        public BarcodeParseResult()
        {
            SetDefaultValues();
        }

        internal void SetDefaultValues()
        {
            this.BarcodeParsingResult = BarcodeParsingResult.NONE;
            this.DecodedCode = null;
            this.Quantity = 0;
            this.CodeValue = 0;
            this.PLU = null;
            this.BarcodeType = null;
        }
    }
}
