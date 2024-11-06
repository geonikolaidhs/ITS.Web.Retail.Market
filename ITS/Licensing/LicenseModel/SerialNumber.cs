using System;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{

    public class SerialNumber : BasicObj
    {

        public SerialNumber()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SerialNumber(Session session)
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

        protected String number;
        public String Number
        {
            get
            {
                return number;
            }
            set
            {
                SetPropertyValue("Number", ref number, value);
            }
        }

        protected int numberOfLicenses;
        public int NumberOfLicenses
        {
            get
            {
                return numberOfLicenses;
            }
            set
            {
                SetPropertyValue("NumberOfLicenses", ref numberOfLicenses, value);
            }
        }


        protected int numberOfUsers;
        public int NumberOfUsers
        {
            get
            {
                return numberOfUsers;
            }
            set
            {
                SetPropertyValue("NumberOfUsers", ref numberOfUsers, value);
            }
        }

        [Association("Licenses-SerialNumber")]
        public XPCollection<License> Licences
        {
            get
            {
                return GetCollection<License>("Licences");
            }
        }

        [Association("SerialNumber-ApplicationUsers")]
        public XPCollection<ApplicationUser> ApplicationUsers
        {
            get
            {
                return GetCollection<ApplicationUser>("ApplicationUsers");
            }
        }
        
        ApplicationInfo _Application;
        [Association("ApplicationInfo-SerialNumbers")]
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

        DateTime _StartDate, _FinalDate;

        public DateTime FinalDate
        {
            get { return _FinalDate; }
            set { SetPropertyValue("FinalDate", ref _FinalDate, value); }
        }

        public DateTime StartDate
        {
            get { return _StartDate; }
            set { SetPropertyValue("StartDate", ref _StartDate, value); }
        }

        Customer customer;
        [Association("SerialNumbers-Customer")]
        public Customer Customer
        {
            get
            {
                return customer;
            }
            set
            {
                SetPropertyValue("Customer", ref customer, value);
            }
        }

        [Association("WebProtectedClasses-SerialNumber")]
        public XPCollection<WebProtectedClass> WebProtectedClasses
        {
            get
            {
                return GetCollection<WebProtectedClass>("WebProtectedClasses");
            }

        }

        [Aggregated, Association("SerialNumber-ValidationRules")]
        public XPCollection<ValidationRule> ValidationRules
        {
            get
            {
                return GetCollection<ValidationRule>("ValidationRules");
            }
        }


        [Aggregated, Association("SerialNumber-UserRules")]
        public XPCollection<UserRule> UserRules
        {
            get
            {
                return GetCollection<UserRule>("UserRules");
            }
        }
    }
}