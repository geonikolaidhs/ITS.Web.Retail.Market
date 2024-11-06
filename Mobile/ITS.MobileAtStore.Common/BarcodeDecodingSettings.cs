using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{
    public class BarcodeDecodingSettings : IXmlSubitems
    {
        public bool DecodeBarcodes { get; set; }
        public int MinimumDecodingLength { get; set; }
        public int PriceDecimalDigits { get; set; }
        public int WeightDecimalDigits { get; set; }
        public List<DecodingPattern> DecodingPrefixes { get; protected set; }
        //public String DecodingPattern { get; set; }
        public BarcodeDecodingSettings()
        {
            DecodingPrefixes = new List<DecodingPattern>();
        }
    }
}
