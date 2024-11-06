using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class UserRule : BasicObj
    {
        public UserRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserRule(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            Oid = Guid.NewGuid();
        }

        protected String userType;
        public String UserType
        {
            get
            {
                return userType;
            }
            set
            {
                SetPropertyValue("UserType", ref userType, value);
            }
        }

        protected int limit;
        public int Limit
        {
            get
            {
                return limit;
            }
            set
            {
                SetPropertyValue("Limit", ref limit, value);
            }
        }

        private SerialNumber _SerialNumber;
        [Association("SerialNumber-UserRules")]
        public SerialNumber SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                SetPropertyValue("SerialNumber", ref _SerialNumber, value);
            }
        }
    }
}
