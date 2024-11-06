using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.RetailAtStoreModel
{
    public class Line
    {

        private decimal _WeightedBarcodeValue;
        private string _WeightedDecodedBarcode;
        private Guid _LinkedLine;
        private BarcodeParsingResult _BarcodeParsingResult;
        private int _counter = -1;
        private decimal _qty1;
        private int _Flyer;
        private Guid _Uniqueid;
        private Guid _header;
        private string _prodCode = string.Empty;
        private string _ProdBarcode = string.Empty;

        public Guid Oid { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        /// <summary>
        /// The incremental number of the header lines for this line.
        /// </summary>
        public int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                SetPropertyValue("Counter", ref _counter, value);
            }
        }

        
        /// <summary>
        /// The Line item is related with a single Header on the other hand Headers have multiple Lines
        /// </summary>
        /// 

        public Guid Header
        {
            get
            {
                return _header;
            }
            set
            {
                SetPropertyValue("Header", ref _header, value);
            }
        }

        
        /// <summary>
        /// The taxcode of the product or the scanned code. depends on whether or not we have product information by the 
        /// time the user scans something
        /// </summary>
        public string ProdCode
        {
            get
            {
                return _prodCode;
            }
            set
            {
                SetPropertyValue("ProdCode", ref _prodCode, value);
            }
        }

        
        /// <summary>
        /// The barcode of the product
        /// </summary>
        public string ProdBarcode
        {
            get
            {
                return _ProdBarcode;
            }
            set
            {
                SetPropertyValue("ProdBarcode", ref _ProdBarcode, value);
            }
        }


        /// <summary>
        /// The entered quantity for this line
        /// </summary>
        public decimal Qty1
        {
            get
            {
                return _qty1;
            }
            set
            {
                SetPropertyValue("Qty1", ref _qty1, value);
            }
        }

        public Guid Uniqueid
        {
            get
            {
                return _Uniqueid;
            }
            set
            {
                SetPropertyValue("Uniqueid", ref _Uniqueid, value);
            }
        }


        public int Flyer
        {
            get
            {
                return _Flyer;
            }
            set
            {
                SetPropertyValue("Flyer", ref _Flyer, value);
            }
        }


        public BarcodeParsingResult BarcodeParsingResult
        {
            get
            {
                return _BarcodeParsingResult;
            }
            set
            {
                SetPropertyValue("BarcodeParsingResult", ref _BarcodeParsingResult, value);
            }
        }


        public string WeightedDecodedBarcode
        {
            get
            {
                return _WeightedDecodedBarcode;
            }
            set
            {
                SetPropertyValue("WeightedDecodedBarcode", ref _WeightedDecodedBarcode, value);
            }
        }


        public decimal WeightedBarcodeValue
        {
            get
            {
                return _WeightedBarcodeValue;
            }
            set
            {
                SetPropertyValue("WeightedBarcodeValue", ref _WeightedBarcodeValue, value);
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

        private void SetPropertyValue<T>(string p1, ref T p2, T value)
        {
            p2 = value;
        }
    }
}