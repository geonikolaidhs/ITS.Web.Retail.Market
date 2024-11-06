using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Model;
namespace ITS.Retail.Common.ViewModel
{
    public class AddressViewModel: BasePersistableViewModel
    {
        public override Type PersistedType
        {
            get { return typeof(Address); }
        }

        public Guid AddressType
        {
            get
            {
                return _AddressType;
            }
            set
            {
                SetPropertyValue("AddressType", ref _AddressType, value);
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
                Notify("Description");
            }
        }

        public string POBox
        {
            get
            {
                return _POBox;
            }
            set
            {
                SetPropertyValue("POBox", ref _POBox, value);
                Notify("Description");
            }
        }

        public string PostCode
        {
            get
            {
                return _PostCode;
            }
            set
            {
                SetPropertyValue("PostCode", ref _PostCode, value);
                Notify("Description");
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
                Notify("Description");
            }
        }

        public string Description
        {
            get
            {
                string address = "";                

                if (String.IsNullOrEmpty(Street) == false)
                {
                    address = Street;
                }


                if (String.IsNullOrEmpty(City) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ",";
                    }
                    address += City;
                }

                if (String.IsNullOrEmpty(PostCode) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ",";
                    }
                    address += PostCode;
                }

                if (String.IsNullOrEmpty(POBox) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ",";
                    }
                    address += POBox;
                }

                return address;
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
                Notify("Profession");
            }
        }

        private string _POBox;
        private string _Street;
        private Guid _AddressType;
        private string _PostCode;
        private string _City;
        private string _Profession;
    }
}
