using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class OrderStatus : DevExpress.Xpo.XPBaseObject
    {
        public OrderStatus()
        {

        }
        public OrderStatus(DevExpress.Xpo.Session session)
            : base(session)
        {

        }
        public OrderStatus(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
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

        private bool isDefault;
        public bool IsDefault
        {
            get
            {
                return isDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref isDefault, value);
            }
        }

        
    }
}
