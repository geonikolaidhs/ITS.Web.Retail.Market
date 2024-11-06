using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class Customer : BasicObj
    {
        public Customer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Customer(Session session)
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


        String companyName;
        public String CompanyName
        {
            get
            {
                return companyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref companyName, value);
            }
        }


        [Association("SerialNumbers-Customer")]
        public XPCollection<SerialNumber> SerialNumbers
        {
            get
            {
                return GetCollection<SerialNumber>("SerialNumbers");
            }
        }

    }
}
