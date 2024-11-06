using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class DocHead : DevExpress.Xpo.XPBaseObject
    {
        public DocHead()
        {

        }
        public DocHead(DevExpress.Xpo.Session session)
            : base(session)
        {

        }
        public DocHead(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
            : base(session, classInfo)
        {

        }

        [Key(true)]
        public Int32 Oid;


        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }


        private string _headOid;
        public string HeadOid
        {
            get
            {
                return _headOid;
            }
            set
            {
                SetPropertyValue("ItemOid", ref _headOid, value);
            }
        }

        [Aggregated, Association("DocHead-DocLines")]
        public XPCollection<DocLine> DocLines
        {
            get
            {
                return GetCollection<DocLine>("DocLines");
            }
        }

        private Series_order _Series;
        [Association("Series_order-DocHeads")]
        public Series_order Series
        {
            get
            {
                return _Series;
            }
            set
            {
                SetPropertyValue("Series", ref _Series, value);
            }
        }

        private Types_Order _Type;
        [Association("Types_Order-DocHeads")]
        public Types_Order Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }

        //private DOC_TYPES _docType;
        //public DOC_TYPES DocType
        //{
        //    get
        //    {
        //        return _docType;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DocType", ref _docType, value);
        //    }
        //}


        double _DocumentDiscount;
        public double DocumentDiscount
        {
            get
            {
                return _DocumentDiscount;
            }
            set
            {
                SetPropertyValue("DocumentDiscount", ref _DocumentDiscount, value);
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

        double _VatAmount1;
        public double VatAmount1
        {
            get
            {
                return _VatAmount1;
            }
            set
            {
                SetPropertyValue("VatAmount1", ref _VatAmount1, value);
            }
        }


        double _VatAmount2;
        public double VatAmount2
        {
            get
            {
                return _VatAmount2;
            }
            set
            {
                SetPropertyValue("VatAmount2", ref _VatAmount2, value);
            }
        }

        double _VatAmount3;
        public double VatAmount3
        {
            get
            {
                return _VatAmount3;
            }
            set
            {
                SetPropertyValue("VatAmount3", ref _VatAmount3, value);
            }
        }


        double _VatAmount4;
        public double VatAmount4
        {
            get
            {
                return _VatAmount4;
            }
            set
            {
                SetPropertyValue("VatAmount4", ref _VatAmount4, value);
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

        double _TotalDiscountAmount;
        public double TotalDiscountAmount
        {
            get
            {
                return _TotalDiscountAmount;
            }
            set
            {
                SetPropertyValue("TotalDiscountAmount", ref _TotalDiscountAmount, value);
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


        public void UpdateDocumentHeader()
        {

            _VatAmount4 = _VatAmount3 = _VatAmount2 = _VatAmount1 = _TotalVatAmount = _TotalDiscountAmount = _NetTotal = _GrossTotal = 0;

            foreach (DocLine docline1 in this.DocLines)
            {

                _NetTotal += docline1.NetTotal;
                _GrossTotal += docline1.GrossTotal;
                _TotalDiscountAmount += docline1.TotalDiscount;
                _TotalVatAmount += docline1.TotalVatAmount;

            }
        }
    }
}
