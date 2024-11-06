using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.MobileAtStore.ObjectModel.Attributes;
using ITS.MobileAtStore.WRMMobileAtStore;

namespace ITS.MobileAtStore.ObjectModel
{
    public class Product : BaseXPObject
    {
        public Product()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Product(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            ChainProduct = null;
        }

        private string _measurementUnitText = string.Empty;
        public string MeasurementUnitText
        {
            get
            {
                return _measurementUnitText;
            }
            set
            {
                SetPropertyValue("MeasurementUnitText", ref _measurementUnitText, value);
            }
        }
        private decimal _points;
        public decimal Points
        {
            get
            {
                return _points;
            }
            set
            {
                SetPropertyValue("Points", ref _points, value);
            }
        }
        private decimal _pricePerUnit;
        public decimal PricePerUnit
        {
            get
            {
                return _pricePerUnit;
            }
            set
            {
                SetPropertyValue("PricePerUnit", ref _pricePerUnit, value);
            }
        }
        private decimal _calculatedTotalPrice;
        public decimal CalculatedTotalPrice
        {
            get
            {
                return _calculatedTotalPrice;
            }
            set
            {
                SetPropertyValue("CalculatedTotalPrice", ref _calculatedTotalPrice, value);
            }
        }
        private decimal _pricePerMeasurementUnit;
        public decimal PricePerMeasurementUnit
        {
            get
            {
                return _pricePerMeasurementUnit;
            }
            set
            {
                SetPropertyValue("PricePerMeasurementUnit", ref _pricePerMeasurementUnit, value);
            }
        }
        public Offer[] RelatedActions { get; set; }
        public Product ChainProduct { get; set; }
        private decimal _chainProductQuantity;
        public decimal ChainProductQuantity
        {
            get
            {
                return _chainProductQuantity;
            }
            set
            {
                SetPropertyValue("ChainProductQuantity", ref _chainProductQuantity, value);
            }
        }

        private string _code = string.Empty;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                SetPropertyValue("Code", ref _code, value);
            }
        }

        private string _barcode = string.Empty;
        public string Barcode
        {
            get
            {
                return _barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _barcode, value);
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetPropertyValue("Description", ref _description, value);
            }
        }

        private decimal _price;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                SetPropertyValue("Price", ref _price, value);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                SetPropertyValue("IsActive", ref _isActive, value);
            }
        }

        private int _vatCat;
        public int VatCat
        {
            get
            {
                return _vatCat;
            }
            set
            {
                SetPropertyValue("VatCat", ref _vatCat, value);
            }
        }



        private string _supplier;
        public string Supplier
        {
            get
            {
                return _supplier;
            }
            set
            {
                SetPropertyValue("Supplier", ref _supplier, value);
            }
        }

        private string _basicsupplier;
        public string BasicSupplier
        {
            get
            {
                return _basicsupplier;
            }
            set
            {
                SetPropertyValue("BasicSupplier", ref _basicsupplier, value);
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                SetPropertyValue("ErrorMessage", ref _errorMessage, value);
            }
        }

        private bool _isActiveOnSupplier;
        public bool IsActiveOnSupplier
        {
            get
            {
                return _isActiveOnSupplier;
            }
            set
            {
                SetPropertyValue("IsActiveOnSupplier", ref _isActiveOnSupplier, value);
            }
        }

        private string _extraCode;
        /// <summary>
        /// The EXTRACODE from SRS
        /// </summary>
        public string ExtraCode
        {
            get
            {
                return _extraCode;
            }
            set
            {
                _extraCode = value;
            }
        }

        private bool _extraCodeIsActive;
        /// <summary>
        /// The EXTRACODE from SRS
        /// </summary>
        public bool ExtraCodeIsActive
        {
            get
            {
                return _extraCodeIsActive;
            }
            set
            {
                _extraCodeIsActive = value;
            }
        }

        /// <summary>
        /// Returns either the taxcode or barcode depending on what was given to this product with priority to Barcode
        /// </summary>
        public string GivenCode
        {
            get
            {
                return !string.IsNullOrEmpty(_barcode) ? _barcode : _code;
            }
        }
        private decimal _requiredQuantity = 0;
        /// <summary>
        /// The required quantity of this entry in an document
        /// </summary>
        public decimal RequiredQuantity
        {
            get
            {
                return _requiredQuantity;
            }
            set
            {
                _requiredQuantity = value;
            }
        }

        private decimal _restInvQty = 0;
        /// <summary>
        /// The quantity left for invoices that haven't been accepted yet
        /// Papapostolou:
        /// Είναι το υπόλοιπο των δελτίων αποστολής που δεν έχει παραληφθεί ακόμα
        /// </summary>
        public decimal RestInvQty
        {
            get
            {
                return _restInvQty;
            }
            set
            {
                _restInvQty = value;
            }
        }

        private decimal _stock = 0;
        /// <summary>
        /// Stock left for this item
        /// </summary>
        public decimal Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                _stock = value;
            }
        }

        private decimal _AverageMonthSales = 0;
        /// <summary>
        ///  ΜΟ ημερήσιων πωλήσεων προηγούμενου μήνα
        /// </summary>
        public decimal AverageMonthSales
        {
            get
            {
                return _AverageMonthSales;
            }
            set
            {
                _AverageMonthSales = value;
            }
        }


        private decimal _OrderMM = 0;
        /// <summary>
        ///  Μονάδα Μέτρησης Παραγγελίας
        /// </summary>
        public decimal OrderMM
        {
            get
            {
                return _OrderMM;
            }
            set
            {
                _OrderMM = value;
            }
        }


        private string _Extrainfo;
        /// <summary>
        /// 
        /// </summary>
        public string extrainfo
        {
            get
            {
                return _Extrainfo;
            }
            set
            {
                SetPropertyValue("extrainfo", ref _Extrainfo, value);
            }
        }

        private bool _SupportsDecimalQuantities;
        public bool SupportsDecimalQuantities
        {
            get
            {
                return _SupportsDecimalQuantities;
            }
            set
            {
                SetPropertyValue("SupportsDecimalQuantities", ref _SupportsDecimalQuantities, value);
            }
        }
        


        private List<ProductQuantity> _productQuantities;
        /// <summary>
        /// The required quantities that have the same mitriko in the document.
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

        /// <summary>
        /// Searches for a product by its given taxcode
        /// </summary>
        /// <param name="taxcode"></param>
        /// <param name="dl"></param>
        /// <returns></returns>
        public static Product Search(string taxcode, IDataLayer dl)
        {
            Session session = new Session(dl);
            return session.FindObject<Product>(CriteriaOperator.Parse(string.Format("[Code] = '{0}'", taxcode)));
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_barcode))
                return string.Format("{0}\r\n{1}", new object[] { (!string.IsNullOrEmpty(_code) ? this._code.TrimStart('0') : string.Empty), this.Description });
            else
            {
                return string.Format(
                    "{0}\r\n{1}\r\n{2}", 
                    new object[] { 
                        (!string.IsNullOrEmpty(_code) ? this._code.TrimStart('0') : string.Empty),
                        (!string.IsNullOrEmpty(_barcode) ? this._barcode.TrimStart('0') : string.Empty),
                        this.Description 
                    });
            }
        }

        /// <summary>
        /// Counts the products that are in the database
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        public static int CountProducts(IDataLayer dl)
        {
            int result;
            using (UnitOfWork uow = new UnitOfWork(dl))
            {
                result = (int)uow.Evaluate(typeof(Product), CriteriaOperator.Parse("Count()"), null);
            }
            return result;
        }


        private int _BasicSupplierColor;
        public int BasicSupplierColor
        {
            get
            {
                return _BasicSupplierColor;
            }
            set
            {
                SetPropertyValue("BasicSupplierColor", ref _BasicSupplierColor, value);
            }
        }

        private decimal _Quantity;
        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        private BarcodeParsingResult _BarcodeParsingResult;
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

        private string _WeightedDecodedBarcode;
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
    }
}
