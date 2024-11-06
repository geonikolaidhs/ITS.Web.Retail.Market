using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class ApplicationUser : BasicObj
    {
        public ApplicationUser()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ApplicationUser(Session session)
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

        private Guid _UserApplicationOid;
        public Guid UserApplicationOid
        {
            get
            {
                return _UserApplicationOid;
            }
            set
            {
                SetPropertyValue("UserApplicationOid", ref _UserApplicationOid, value);
            }
        }

        private String _Name;
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private String _Key;
        public String Key
        {
            get
            {
                return _Key;
            }
            set
            {
                SetPropertyValue("Key", ref _Key, value);
            }
        }

        private String _Type;
        public String Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }


        private SerialNumber _ApplicationUsers;
        [Association("SerialNumber-ApplicationUsers")]
        public SerialNumber ApplicationUsersSerialNumber
        {
            get
            {
                return _ApplicationUsers;
            }
            set
            {
                SetPropertyValue("Application Users", ref _ApplicationUsers, value);
            }
        }

        public String VersionDate
        {
            get
            {
                return DateTime.Parse(this.Version.ToString()).ToString();
            }
        }
    }
}
