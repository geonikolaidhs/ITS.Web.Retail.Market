using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class ApplicationInfo : BasicObj
    {
        public ApplicationInfo()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ApplicationInfo(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            HasDesktop = //false;
            HasMobile = //false;
            HasWeb = false;
        }

        private Guid applicationOid;
        public Guid ApplicationOid
        {
            get
            {
                return applicationOid;
            }
            set
            {
                SetPropertyValue("ApplicationOid", ref applicationOid, value);
            }
        }


        private String name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                SetPropertyValue("Name", ref name, value);
            }
        }

        private bool hasWeb;
        public bool HasWeb
        {
            get
            {
                return hasWeb;
            }
            set
            {
                SetPropertyValue("HasWeb", ref hasWeb, value);
            }
        }
        private bool hasMobile;
        public bool HasMobile
        {
            get
            {
                return hasMobile;
            }
            set
            {
                SetPropertyValue("HasMobile", ref hasMobile, value);
            }
        }
        private bool hasDesktop;
        public bool HasDesktop
        {
            get
            {
                return hasDesktop;
            }
            set
            {
                SetPropertyValue("HasDesktop", ref hasDesktop, value);
            }
        }

        [Aggregated, Association("ApplicationInfo-ApplicationVersions")]
        public XPCollection<ApplicationVersion> ApplicationVersions
        {
            get
            {
                return GetCollection<ApplicationVersion>("ApplicationVersions");
            }
        }

        [Aggregated,Association("Rule-ApplicationInfo")]
        public XPCollection<Rule> Rules
        {
            get
            {
                return GetCollection<Rule>("Rules");
            }
        }

        [Association("ApplicationInfo-SerialNumbers")]
        public XPCollection<SerialNumber> SerialNumbers
        {
            get
            {
                return GetCollection<SerialNumber>("SerialNumbers");
            }
        }
    }
}
