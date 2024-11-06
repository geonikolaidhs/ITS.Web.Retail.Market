using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.MobileAtStore.ObjectModel
{
    [Persistent("ITS_DATALOGGER_ESL_INV_LINE")]
    public class ESLInvLine : BaseXPObject
    {
        public ESLInvLine()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ESLInvLine(Session session)
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

        private string _prodCode = string.Empty;
        /// <summary>
        /// The taxcode of the product or the scanned code. depends on whether or not we have product information by the 
        /// time the user scans something
        /// </summary>
        [Indexed("ProdBarcode", "InventoryNumber")]
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
        [Indexed("ProdCode")]
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

        private string _InventoryNumber;
        [Indexed]
        public string InventoryNumber
        {
            get
            {
                return _InventoryNumber;
            }
            set
            {
                SetPropertyValue("InventoryNumber", ref _InventoryNumber, value);
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

        [NonPersistent]
        public decimal TotalQty
        {
            get
            {
                object res;
                res = Session.Evaluate<ESLInvLine>(CriteriaOperator.Parse("SUM(Qty)"), CriteriaOperator.Parse("ProdCode = ?", ProdCode));
                return res == null ? 0 : Convert.ToDecimal(res);
            }
        }
    }
}
