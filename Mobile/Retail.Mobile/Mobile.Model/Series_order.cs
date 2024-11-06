using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class Series_order : DevExpress.Xpo.XPBaseObject
    {
        public Series_order()
        {
            
        }
        public Series_order(DevExpress.Xpo.Session session)
            : base(session)
        {
            
        }
        public Series_order(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
            : base(session, classInfo)
        {
            
        }

        [Key(true)]
        public Int32 Oid;

        private Guid _Id;
        public Guid Id
        {
            get
            {
                return _Id;
            }
            set
            {
                SetPropertyValue("Id", ref _Id, value);
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

        private Types_Order _Type_Order;
        [Association("Types_Order-Series_order")]
        public Types_Order Type_Order
        {
            get
            {
                return _Type_Order;
            }
            set
            {
                SetPropertyValue("Type_Order", ref _Type_Order, value);
            }
        }

    }
}
