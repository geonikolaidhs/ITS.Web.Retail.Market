using System;

using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace Retail.Mobile_Model
{
    public class Settings : DevExpress.Xpo.XPBaseObject
    {
        public Settings()
        {
            
        }
        public Settings(DevExpress.Xpo.Session session)
            : base(session)
        {
            
        }
        public Settings(DevExpress.Xpo.Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
            : base(session, classInfo)
        {
            
        }

        [Key(true)]
        public Int32 Oid;

        private string _CustomerName;
        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                SetPropertyValue("CustomerName", ref _CustomerName, value);
            }
        }

        private string _CustomerID;
        public string CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                SetPropertyValue("CustomerID", ref _CustomerID, value);
            }
        }

        private string _StoreName;
        public string StoreName
        {
            get
            {
                return _StoreName;
            }
            set
            {
                SetPropertyValue("StoreName", ref _StoreName, value);
            }
        }

        private string _StoreID;
        public string StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                SetPropertyValue("StoreID", ref _StoreID, value);
            }
        }

        private string _CompanyName;
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref _CompanyName, value);
            }
        }
        private string _CompanyID;
        public string CompanyID
        {
            get
            {
                return _CompanyID;
            }
            set
            {
                SetPropertyValue("CompanyID", ref _CompanyID, value);
            }
        }

        private string _User;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        private string _Pass;
        public string Pass
        {
            get
            {
                return _Pass;
            }
            set
            {
                SetPropertyValue("Pass", ref _Pass, value);
            }
        }

        private string _UserID;
        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                SetPropertyValue("UserID", ref _UserID, value);
            }
        }
    }
}
