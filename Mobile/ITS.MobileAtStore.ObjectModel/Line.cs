using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.MobileAtStore.ObjectModel
{
    //[Persistent("ITS_DATALOGGER_LINE")]
    //[Persistent("ORDER_MOB_ART")]
    public class Line : BaseXPObject
    {
        public Line()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Line(Session session)
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

        /// <summary>
        /// Implements additional logic that runs before been saved
        /// </summary>
        protected override void OnSaving()
        {
            if (_counter == -1)
            {
                int? max = (int?)Session.Evaluate<Line>(CriteriaOperator.Parse("Max(Counter)"), CriteriaOperator.Parse("Header = ?", _header));
                _counter = max == null ? 1 : max.Value + 1;
            }

            base.OnSaving();
        }


        public string ToString(String format)
        {
            Header header = this.GetHeader();
            string TraderCode = "";
            string TraderAFM = "";
            string TraderCompanyName = "";
            if(header!=null)
            {
                TraderCode = header.CustomerCode;
                TraderAFM = header.CustomerAFM;
                TraderCompanyName = header.CustomerName;
            }
            return string.Format(
                new PaddedStringFormatInfo(),
                format,
                this.Flyer, 
                this.ProdBarcode, 
                this.ProdCode, 
                this.Qty1, 
                TraderCode, 
                TraderAFM, 
                TraderCompanyName);
        }



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

        [Indexed(Unique = false)]        
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

        [DevExpress.Xpo.Indexed(Unique = false)]
        public  Guid Uniqueid
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

        public ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult BarcodeParsingResult
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
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        private bool _FoundOnline;
        public bool FoundOnline
        {
            get
            {
                return _FoundOnline;
            }
            set
            {
                SetPropertyValue("FoundOnline", ref _FoundOnline, value);
            }
        }

        private string _ScannedCode;
        public string ScannedCode
        {
            get
            {
                return _ScannedCode;
            }
            set
            {
                SetPropertyValue("ScannedCode", ref _ScannedCode, value);
            }
        }


        public string DetailDescription
        {
            get
            {
                return ProdCode + ": " + Description + "\r\nΠοσοτητα: " + Qty1.ToString();
            }
        }

        private string _Description;
        private decimal _WeightedBarcodeValue;
        private Guid _LinkedLine;
        private string _WeightedDecodedBarcode;
        private ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult _BarcodeParsingResult;
        private int _Flyer;
        private Guid _Uniqueid;
        private decimal _qty1;
        private string _ProdBarcode = string.Empty;
        private string _prodCode = string.Empty;
        private Guid _header;
        private int _counter = -1;

        private Header GetHeader()
        {
            return this.Session.GetObjectByKey<Header>(this.Header);
        }
    }
}
