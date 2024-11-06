using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class BillingPackage : BasicObj
    {
        public BillingPackage()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BillingPackage(Session session)
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

        // Fields...
        private string _ApplicationURL;

        public string ApplicationURL
        {
            get
            {
                return _ApplicationURL;
            }
            set
            {
                SetPropertyValue("ApplicationURL", ref _ApplicationURL, value);
            }
        }

        //xps
    }
}
