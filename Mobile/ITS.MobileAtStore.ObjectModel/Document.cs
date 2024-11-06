using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;


namespace ITS.MobileAtStore.ObjectModel
{

    public enum DOC_TYPES : byte
    {
        ALL_TYPES = 0,
        ORDER = 1,
        NULL2 = 2,
        INVOICE = 3,
        NULL4 = 4,
        RECEPTION = 5,
        INVENTORY = 6,
        MATCHING = 7,
        PICKING = 8,
        TRANSFER = 9,
        TAG = 10,
        PRICE_CHECK = 11,
        COMPETITION = 12,
        /// <summary>
        /// Not used in documents
        /// </summary>
        ESL_INV = 13,
        //
        NULL14 = 14,
        NULL15 = 15,
        INVOICE_SALES = 16,
        DECOMPOSITION = 17,
        COMPOSITION = 18,
        QUEUE_QR = 19
    }
        

    public enum DOC_STATUS : byte
    {
        OPEN = 0,
        CLOSED = 1,
        EXPORTED = 2
    }

    public class Document : IDisposable
    {
        #region Data Members
        private Session _session;

        private IDataLayer _dataLayer;
        public IDataLayer DataLayer
        {
            get
            {
                return _dataLayer;
            }
            set
            {
                _dataLayer = value;
            }
        }

        private Header _header;
        public Header Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        private Line _currentLine;
        public Line CurrentLine
        {
            get
            {
                return _currentLine;
            }
            set
            {
                _currentLine = value;
            }
        }

        public bool HasCustomer
        {
            get
            {
                if (Header == null)
                    return false;
                else
                    return !string.IsNullOrEmpty(Header.CustomerCode);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the Document by setting the datalayer used and a XPO Session
        /// </summary>
        /// <param name="dataLayer"></param>
        public Document(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
            _session = new Session(_dataLayer);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if a prodcode line exists already in the document
        /// </summary>
        /// <param name="prodCode">The prodcode that we like to search for</param>
        /// <returns></returns>
        public Line Exists(string prodCode)
        {
            Line line = _session.FindObject<Line>(CriteriaOperator.Parse(string.Format("[Header] = '{0}' AND ProdCode = '{1}'", _header.Oid, prodCode)));
            return line;
        }

        /// <summary>
        /// Checks if a line exists with the supplied code and barcode, if barcode is empty then it checks with just the taxcode
        /// </summary>
        /// <param name="prodCode">The prodcode that we like to search for</param>
        /// <param name="barcode">The barcode that we like to search for</param>
        /// <returns></returns>
        public Line Exists(string prodCode, string barcode)
        {
            string criteria = string.Format("[Header] = '{0}' AND [ProdCode] = '{1}' AND [ProdBarcode] = '{2}' ", _header.Oid, prodCode, barcode);
            Line line = _session.FindObject<Line>(CriteriaOperator.Parse(criteria));
            return line;
        }

        /// <summary>
        /// Returns the sum of the quantity scanned for the given Line.ProdCode
        /// </summary>
        /// <returns>The quantity scanned, 0 for none or null</returns>
        public decimal GetTotalQuantityScannedForCode(string code)
        {
            decimal result;
            using (UnitOfWork uow = new UnitOfWork(_dataLayer))
            {
                object objResult = uow.Evaluate(typeof(Line), CriteriaOperator.Parse("Sum([Qty1])"), CriteriaOperator.Parse("[Header] = ? AND [ProdCode] = ?", _header.Oid, code));
                result = objResult == null ? 0 : (decimal)objResult;
            }
            return result;
        }

        /// <summary>
        /// Checks if a line with the provided prodCode and barcode already exists
        /// </summary>
        /// <param name="prod_code"></param>
        /// <returns></returns>
        public Line Exists(Product prod)
        {
            if (prod == null)
                return null;
            Line line = null;
            string criteria = string.Format("[Header] = '{0}' AND [ProdCode] = '{1}' AND [ProdBarcode] = '{2}' ", _header.Oid, prod.Code, prod.Barcode);
            line = _session.FindObject<Line>(CriteriaOperator.Parse(criteria));
            return line;
        }

        /// <summary>
        /// Adds a line in the document by its prodcode and a quantity
        /// </summary>
        /// <param name="prodCode"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public Line Add(string prodCode, decimal qty, int flyer)
        {
            return Add(prodCode, string.Empty, qty, flyer);
        }

        public Line Add(string prodCode, string barcode, decimal qty, int flyer)
        {
            Line line = new Line(_session) {
                                                ProdCode = prodCode,
                                                ProdBarcode = barcode,
                                                Qty1 = qty,
                                                Header = _header.Oid,
                                                Flyer = flyer,
                                                BarcodeParsingResult = ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.NON_WEIGHTED_PRODUCT,
                                                FoundOnline = true,
                                                ScannedCode=barcode
            };
            line.Uniqueid = Guid.NewGuid();
            line.Save();
            _header.Save();
            this.CurrentLine = line;
            return this.CurrentLine;
        }

        public Line Add(Product product,int flyer)
        {
            Line line = new Line(_session) {
                                                ProdCode = product.Code,
                                                ProdBarcode = product.Barcode,
                                                Qty1 = product.Quantity,
                                                Header = _header.Oid,
                                                Flyer = flyer,
                                                BarcodeParsingResult = product.BarcodeParsingResult,
                                                WeightedDecodedBarcode = product.WeightedDecodedBarcode,
                                                FoundOnline=true                                                
            };
            if( product.BarcodeParsingResult == ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.ITEM_CODE_VALUE )
            {
                line.WeightedBarcodeValue = product.CalculatedTotalPrice;
                line.ScannedCode = product.WeightedDecodedBarcode;
            }
            if (product.BarcodeParsingResult == ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.ITEM_CODE_QUANTITY)
            {
                line.ScannedCode = product.WeightedDecodedBarcode;
            }
            else
            {
                line.ScannedCode = product.Barcode;
            }
            line.Uniqueid = Guid.NewGuid();
            line.Save();
            _header.Save();
            this.CurrentLine = line;
            return this.CurrentLine;
        }


        public Line AddOrUpdate(string prodCode, string barCode, decimal qty, int flyer)
        {
            Line line = Exists(prodCode, barCode);
            if (line == null)
            {
                line = Add(prodCode, barCode, qty, flyer);
            }
            else
            {
                line.Qty1 = qty;
                line.Save();
            }
            return line;
        }

        public void SetHeaderCustomer(string code)
        {
            if (_header != null)
            {
                _header.CustomerCode = code;
                _header.Save();
            }          
        }

        /// <summary>
        /// Adds a line in the document by its prodcode and a quantity
        /// </summary>
        /// <param name="prodCode"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public Line AddMatching(string code, string codeMatching, decimal qty, int flyer)
        {
            return Add(code, codeMatching, qty, flyer);
        }
        /// <summary>
        /// Loads a document based on its type if there isn't one it creates a new one
        /// </summary>
        /// <param name="doc_type"></param>
        /// <returns></returns>
        public Document Load(DOC_TYPES doc_type, bool forcedOffline)
        {
            XPCollection<Header> allHeaders = new XPCollection<Header>(_session, CriteriaOperator.Parse(string.Format("[DocType] = {0}", (int)doc_type)));

            //_header = _session.FindObject<Header>(CriteriaOperator.And(CriteriaOperator.Parse(string.Format("[DocType] = {0}", (int)doc_type)),
            //                                                           new BinaryOperator("ForcedOffline",forcedOffline)));

            //backwards compartible
            foreach (Header header in allHeaders)
            {
                if (header.ForcedOffline == forcedOffline)
                {
                    _header = header;
                    break;
                }
            }

            if (_header == null)
            {
                _header = new Header(_session);
                _header.DocType = doc_type;
                _header.ForcedOffline = forcedOffline;
                
                _header.Save();
            }
            return this;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Counts the lines of a specific doctype
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="dl"></param>
        /// <returns></returns>
        public static int GetDocItemCount(DOC_TYPES docType, IDataLayer dl, bool forcedOffline)
        {
            int result = 0;
            try
            {
                using (UnitOfWork uow = new UnitOfWork(dl))
                {
                    /*Header header = uow.FindObject<Header>(CriteriaOperator.And(
                                                            CriteriaOperator.Parse("[DocType] = ?", (int)docType),
                                                            new BinaryOperator("ForcedOffline", forcedOffline)));
                     * */

                    Header header = null;/* = uow.FindObject<Header>(CriteriaOperator.And(
                                                           CriteriaOperator.Parse("[DocType] = ?", (int)docType), new BinaryOperator("ForcedOffline", 
                                                           forcedOffline)));*/
                    XPCollection<Header> allHeaders = new XPCollection<Header>(uow, CriteriaOperator.Parse(string.Format("[DocType] = {0}", (int)docType)));

                    //backwards compartible
                    foreach (Header head in allHeaders)
                    {
                        if (head.ForcedOffline == forcedOffline)
                        {
                            header = head;
                            break;
                        }
                    }
                    if (header != null)
                        result = header.CountLines();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Υπήρξε ένα σφάλμα κάτα την καταμέτρηση των γραμμών ενός παραστατικού", ex);
            }
            return result;
        }

        ///// <summary>
        ///// Checks if a given doc type has lines for export
        ///// </summary>
        ///// <param name="doc_type"></param>
        ///// <param name="dl"></param>
        ///// <returns></returns>
        //public static bool HasRecordForExport(DOC_TYPES doc_type, IDataLayer dl)
        //{
        //    using (UnitOfWork uow = new UnitOfWork(dl))
        //    {
        //        int count = (int)uow.Evaluate(typeof(Header), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[DocType] = ?", doc_type));
        //        return count > 0;
        //    }
        //}
        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            _session.Dispose();
        }

        #endregion
    }
}
