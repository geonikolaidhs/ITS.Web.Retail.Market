using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 670,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class ReportRole : BaseObj
    {
        public ReportRole()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ReportRole(Session session)
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
        private Role _Role;
        private CustomReport _Report;

        [Association("CustomReport-ReportRoles")]
        public CustomReport Report
        {
            get
            {
                return _Report;
            }
            set
            {
                SetPropertyValue("Report", ref _Report, value);
            }
        }


        public Role Role
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
    }
}
