using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ITS.Retail.WebClient.RetailAtStoreModel
{

    public class InvLine
    {
        public Guid Oid { get; set; }

        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        private string _prodCode = string.Empty;
        /// <summary>
        /// The taxcode of the product or the scanned code. depends on whether or not we have product information by the 
        /// time the user scans something
        /// </summary>
        //[Indexed("ProdBarcode")]
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

        private string _ProdBarcode = string.Empty;
        /// <summary>
        /// The barcode of the product
        /// </summary>
        //[Indexed("ProdCode")]
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

        private decimal _qty;
        /// <summary>
        /// The entered quantity for this line
        /// </summary>
        public decimal Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _qty, value);
            }
        }

        private string _Descr;
        public string Descr
        {
            get
            {
                return _Descr;
            }
            set
            {
                SetPropertyValue("Descr", ref _Descr, value);
            }
        }

        public string ZeroTrimmedProdCode
        {
            get
            {
                if (string.IsNullOrEmpty(ProdCode))
                    return string.Empty;
                else
                    return ProdCode.TrimStart('0');
            }
        }

        public string ZeroTrimmedProdBarcode
        {
            get
            {
                if (string.IsNullOrEmpty(ProdBarcode))
                    return string.Empty;
                else
                    return ProdBarcode.TrimStart('0');
            }
        }

        private int _Export;
        public int Export
        {
            get
            {
                return _Export;
            }
            set
            {
                SetPropertyValue("Export", ref _Export, value);
            }
        }

        private string _OutputPath;
        public string outputPath
        {
            get
            {
                return _OutputPath;
            }
            set
            {
                SetPropertyValue("outputPath", ref _OutputPath, value);
            }
        }

        protected void SetPropertyValue<T>(string p1, ref T p2, T value)
        {
            p2 = value;
        }
    }
}