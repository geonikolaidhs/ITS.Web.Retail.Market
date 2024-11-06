using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations.ViewModel
{
    public class InsertedCustomerViewModel : INotifyPropertyChanged
    {

        // Fields...
        private string _Phone;
        private string _City;
        private string _PostalCode;
        private string _Street;
        private string _CompanyName;
        private string _TaxCode;
        private string _Code;
        private string _Profession;
        private string _FirstName;
        private string _LastName;
        private Guid _CustomerOid;
        private Guid _AddressOid;
        private bool _IsNew;
        private Guid _TaxOfficeLookup;
        private bool _IsSupplier;
        private string _AddressProfession;
        private string _CardID;
        private bool _AddressIsNew;
        private string _TaxOfficeDescription;
        private string _DeliveryProfession;
        private string _ThirdPartNum;

        public Guid TaxOfficeLookup
        {
            get
            {
                return _TaxOfficeLookup;
            }
            set
            {
                SetPropertyValue("TaxOfficeLookup", ref _TaxOfficeLookup, value);
            }
        }

        public string TaxOfficeDescription
        {
            get
            {
                return _TaxOfficeDescription;
            }
            set
            {
                SetPropertyValue("TaxOfficeDescription", ref _TaxOfficeDescription, value);
            }
        }

        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }

        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref _CompanyName, value);
            }
        }

        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                SetPropertyValue("FirstName", ref _FirstName, value);
            }
        }
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                SetPropertyValue("LastName", ref _LastName, value);
            }
        }

        public string Profession
        {
            get
            {
                return _Profession;
            }
            set
            {
                SetPropertyValue("Profession", ref _Profession, value);
            }
        }

        public string AddressProfession
        {
            get
            {
                return _AddressProfession;
            }
            set
            {
                SetPropertyValue("AddressProfession", ref _AddressProfession, value);
            }
        }

        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                SetPropertyValue("Street", ref _Street, value);
            }
        }

        public string PostalCode
        {
            get
            {
                return _PostalCode;
            }
            set
            {
                SetPropertyValue("PostalCode", ref _PostalCode, value);
            }
        }

        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                SetPropertyValue("City", ref _City, value);
            }
        }

        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                SetPropertyValue("Phone", ref _Phone, value);
            }
        }

        public Guid CustomerOid
        {
            get
            {
                return _CustomerOid;
            }
            set
            {
                SetPropertyValue("CustomerOid", ref _CustomerOid, value);
            }
        }

        public Guid AddressOid
        {
            get
            {
                return _AddressOid;
            }
            set
            {
                SetPropertyValue("AddressOid", ref _AddressOid, value);
            }
        }

   
        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                SetPropertyValue("IsNew", ref _IsNew, value);
            }
        }
        public bool AddressIsNew
        {
            get
            {
                return _AddressIsNew;
            }
            set
            {
                SetPropertyValue("AddressIsNew", ref _AddressIsNew, value);
            }
        }

        public bool IsSupplier
        {
            get
            {
                return _IsSupplier;
            }
            set
            {
                SetPropertyValue("IsSupplier", ref _IsSupplier, value);
            }
        }
        public string CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                SetPropertyValue("CardID", ref _CardID, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetPropertyValue<T>(string propertyName, ref T field, T newvalue)
        {
            field = newvalue;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string DeliveryProfession
        {
            get
            {
                return _DeliveryProfession;
            }
            set
            {
                SetPropertyValue("DeliveryProfession", ref _DeliveryProfession, value);
            }
        }

        public string ThirdPartNum  
        {
            get
            {
                return _ThirdPartNum;
            }
            set
            {
                SetPropertyValue("ThirdPartNum", ref _ThirdPartNum, value);
            }
        }
    }
}
