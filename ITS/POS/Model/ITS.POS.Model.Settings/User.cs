using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Settings
{
    public class User : BaseObj
    {
        public User()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public User(Session session)
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

        public User(User u)
            : base()
        {
            _FullName = u.FullName;
            _Password = u.Password;
            _UserName = u.UserName;
        }

        private string _UserName;
        [Indexed("GCRecord",Unique = true)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }
        private string _FullName;
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                SetPropertyValue("FullName", ref _FullName, value);
            }
        }
        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }
        private string _TaxCode;
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }

        public XPCollection<UserTypeAccess> UserTypeAccesses
        {
            get
            {
                      return  new XPCollection<UserTypeAccess>(this.Session, new BinaryOperator("UserTypeAccesses", Oid, BinaryOperatorType.Equal));
             }
        }

        private Guid _Role;
        public Guid Role
        {
            get
            {
                return _Role;
            }
            set
            {
                SetPropertyValue("Role", ref _Role, value);
            }
        }

        private string _POSUserName;
        public string POSUserName
        {
            get
            {
                return _POSUserName;
            }
            set
            {
                SetPropertyValue("POSUserName", ref _POSUserName, value);
            }
        }

        private string _POSPassword;
        public string POSPassword
        {
            get
            {
                return _POSPassword;
            }
            set
            {
                SetPropertyValue("POSPassword", ref _POSPassword, value);
            }
        }

        private ePOSUserLevel _POSUserLevel;
        public ePOSUserLevel POSUserLevel
        {
            get
            {
                return _POSUserLevel;
            }
            set
            {
                SetPropertyValue("POSUserLevel", ref _POSUserLevel, value);
            }
        }


    }
}
