using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class License : BasicObj
    {

        public License()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public License(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            //Oid = Guid.NewGuid();
        }


        String _MachineID, _MachineApplicationUniqueKey, _ActivationKey;

        public String MachineID
        {
            get { return _MachineID; }
            set { SetPropertyValue("MachineID", ref _MachineID, value);; }
        }



        public String MachineApplicationUniqueKey
        {
            get { return _MachineApplicationUniqueKey; }
            set { SetPropertyValue("MachineApplicationUniqueKey", ref _MachineApplicationUniqueKey, value); }
        }

        public String ActivationKey
        {
            get { return _ActivationKey; }
            set { SetPropertyValue("ActivationKey", ref _ActivationKey, value); }
        }


        protected DateTime installedVersionDateTime;
        public DateTime InstalledVersionDateTime
        {
            get
            {
                return installedVersionDateTime;
            }
            set
            {
                SetPropertyValue("InstalledVersionDateTime", ref installedVersionDateTime, value);
            }
        }

        protected SerialNumber serialNumber;
        [Association("Licenses-SerialNumber")]
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
    }
}
