using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class Types_Order : DevExpress.Xpo.XPBaseObject
    {
        public Types_Order()
        {
            
        }
        public Types_Order(DevExpress.Xpo.Session session)
            : base(session)
        {
            
        }
        public Types_Order(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
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

        [Association("Types_Order-Series_order")]
        public XPCollection<Series_order> Series_order
        {
            get
            {
                return GetCollection<Series_order>("Series_order");
            }
        }
    }
}
