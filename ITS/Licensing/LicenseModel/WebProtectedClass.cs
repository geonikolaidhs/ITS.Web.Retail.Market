using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class WebProtectedClass : BasicObj
    {
        public WebProtectedClass()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public WebProtectedClass(Session session)
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

        String _ClassName;
        public String ClassName
        {
            get
            {
                return _ClassName;
            }
            set
            {
                SetPropertyValue("ClassName", ref _ClassName, value);
            }
        }

        String _Criteria;
        public String Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                SetPropertyValue("Criteria", ref _Criteria, value);
            }
        }

        int _MaximumNumberOfEntries;
        public int MaximumNumberOfEntries
        {
            get
            {
                return _MaximumNumberOfEntries;
            }
            set
            {
                SetPropertyValue("MaximumNumberOfEntries", ref _MaximumNumberOfEntries, value);
            }
        }

        protected SerialNumber serialNumber;
        [Association("WebProtectedClasses-SerialNumber")]
        public SerialNumber SerialNumber
        {
            get
            {
                return serialNumber;
            }
            set
            {
                SetPropertyValue("SerialNumber", ref serialNumber, value);
            }
        }
    }
}
