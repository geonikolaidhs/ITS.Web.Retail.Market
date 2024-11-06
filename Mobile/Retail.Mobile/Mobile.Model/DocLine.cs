using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class DocLine : DevExpress.Xpo.XPBaseObject
    {
        public DocLine()
        {
            
        }
        public DocLine(DevExpress.Xpo.Session session)
            : base(session)
        {
            
        }
        public DocLine(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
            : base(session, classInfo)
        {
            
        }

        private double _PackingQty;
        [Key(true)]
        public Int32 Oid;

        private string _ItemCode;
        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
            }
        }


        private bool _EditOffline;
        public bool EditOffline
        {
            get
            {
                return _EditOffline;
            }
            set
            {
                SetPropertyValue("EditOffline", ref _EditOffline, value);
            }
        }


        private string _ItemOid;
        public string ItemOid
        {
            get
            {
                return _ItemOid;
            }
            set
            {
                SetPropertyValue("ItemOid", ref _ItemOid, value);
            }
        }

        private string _ItemName;
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                SetPropertyValue("ItemName", ref _ItemName, value);
            }
        }

        private string _Barcode;
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

        private double _Qty;
        public double Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _Qty, value);
            }
        }

        private DocHead _DocHead;
        [Association("DocHead-DocLines")]
        public DocHead DocHead
        {
            get
            {
                return _DocHead;
            }
            set
            {
                SetPropertyValue("DocHead", ref _DocHead, value);
            }
        }

        double _ItemPrice;
        public double ItemPrice
        {
            get
            {
                return _ItemPrice;
            }
            set
            {
                SetPropertyValue("ItemPrice", ref _ItemPrice, value);
            }
        }

        double _FinalUnitPrice;
        public double FinalUnitPrice
        {
            get
            {
                return _FinalUnitPrice;
            }
            set
            {
                SetPropertyValue("FinalUnitPrice", ref _FinalUnitPrice, value);
            }
        }

        double _GrossTotal;
        public double GrossTotal
        {
            get
            {
                return _GrossTotal;
            }
            set
            {
                SetPropertyValue("GrossTotal", ref _GrossTotal, value);
            }
        }

        double _NetTotal;
        public double NetTotal
        {
            get
            {
                return _NetTotal;
            }
            set
            {
                SetPropertyValue("NetTotal", ref _NetTotal, value);
            }
        }

        double _NetTotalAfterDiscount;
        public double NetTotalAfterDiscount
        {
            get
            {
                return _NetTotalAfterDiscount;
            }
            set
            {
                SetPropertyValue("NetTotalAfterDiscount", ref _NetTotalAfterDiscount, value);
            }
        }

        double _FirstDiscount;
        public double FirstDiscount
        {
            get
            {
                return _FirstDiscount;
            }
            set
            {
                SetPropertyValue("FirstDiscount", ref _FirstDiscount, value);
            }
        }

        double _SecondDiscount;
        public double SecondDiscount
        {
            get
            {
                return _SecondDiscount;
            }
            set
            {
                SetPropertyValue("SecondDiscount", ref _SecondDiscount, value);
            }
        }

        double _TotalDiscount;
        public double TotalDiscount
        {
            get
            {
                return _TotalDiscount;
            }
            set
            {
                SetPropertyValue("TotalDiscount", ref _TotalDiscount, value);
            }
        }

        double _TotalVatAmount;
        public double TotalVatAmount
        {
            get
            {
                return _TotalVatAmount;
            }
            set
            {
                SetPropertyValue("TotalVatAmount", ref _TotalVatAmount, value);
            }
        }

        double _UnitPriceAfterDiscount;
        public double UnitPriceAfterDiscount
        {
            get
            {
                return _UnitPriceAfterDiscount;
            }
            set
            {
                SetPropertyValue("UnitPriceAfterDiscount", ref _UnitPriceAfterDiscount, value);
            }
        }

        double _VatAmount;
        public double VatAmount
        {
            get
            {
                return _VatAmount;
            }
            set
            {
                SetPropertyValue("VatAmount", ref _VatAmount, value);
            }
        }

        double _VatFactor;
        public double VatFactor
        {
            get
            {
                return _VatFactor;
            }
            set
            {
                SetPropertyValue("VatFactor", ref _VatFactor, value);
            }
        }

        DocLine _RefDocLine;
        [Association("DocLine-DocLines")]
        public DocLine RefDocLine
        {
            get
            {
                return _RefDocLine;
            }
            set
            {
                SetPropertyValue("RefDocLine", ref _RefDocLine, value);
            }
        }


        [Association("DocLine-DocLines")]
        public XPCollection<DocLine> RefLines
        {
            get
            {
                return GetCollection<DocLine>("RefLines");
            }
        }


        public double PackingQty
        {
            get
            {
                return _PackingQty;
            }
            set
            {
                SetPropertyValue("PackingQty", ref _PackingQty, value);
            }
        }
//#if DEBUG
//        protected override void OnSaving()
//        {
//            base.OnSaving();
//            if (DocHead == null)
//            {
//                throw new Exception("Dochead is null");
//            }
//        }
//#endif

    }
}
