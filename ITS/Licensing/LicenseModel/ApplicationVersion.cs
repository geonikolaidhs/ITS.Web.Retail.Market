using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class ApplicationVersion : BasicObj
    {
        public ApplicationVersion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ApplicationVersion(Session session)
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


        private long buildDateTime;
        public long BuildDateTime
        {
            get
            {
                return buildDateTime;
            }
            set
            {
                SetPropertyValue("BuildDateTime", ref buildDateTime, value);
            }
        }

        private ApplicationInfo _Application;
        [Association("ApplicationInfo-ApplicationVersions")]
        public ApplicationInfo Application
        {
            get
            {
                return _Application;
            }
            set
            {
                SetPropertyValue("Application", ref _Application, value);
            }
        }
    }
}
