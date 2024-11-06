//-----------------------------------------------------------------------
// <copyright file="Address.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class Address : BaseObj
    {
        public Address()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Address(Session session)
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

        private string _Email;
        private bool _IsDefault;
        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }

        //private AddressType _AddressType;
        //[Association("AddressType-Addresss")]
        //public AddressType AddressType
        //{
        //    get
        //    {
        //        return _AddressType;
        //    }
        //    set
        //    {
        //        SetPropertyValue("AddressType", ref _AddressType, value);
        //    }
        //}

        [NonPersistent]
        public string Description
        {
            get
            {
                string address = "";                

                if (String.IsNullOrEmpty(Street) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += Street;
                }


                if (String.IsNullOrEmpty(City) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += City;
                }

                if (String.IsNullOrEmpty(PostCode) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += PostCode;
                }

                if (String.IsNullOrEmpty(POBox) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += POBox;
                }

                return address;
            }
        }

        private string _Street;
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

        private string _POBox;
        public string POBox
        {
            get
            {
                return _POBox;
            }
            set
            {
                SetPropertyValue("POBox", ref _POBox, value);
            }
        }

        private string _PostCode;
        public string PostCode
        {
            get
            {
                return _PostCode;
            }
            set
            {
                SetPropertyValue("PostCode", ref _PostCode, value);
            }
        }
        private string _City;
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


        //private Guid? _DefaultPhoneOid;
        //[Persistent("DefaultPhone")]        
        //public Guid? DefaultPhoneOid
        //{
        //    get
        //    {
        //        return _DefaultPhoneOid;                
        //    }
        //    set
        //    {
        //        SetPropertyValue("DefaultPhoneOid", ref _DefaultPhoneOid, value);
        //    }
        //}

        //[NonPersistent]
        //[UpdaterIgnoreField]
        //[NoForeignKey]
        //public Phone DefaultPhone
        //{
        //    get
        //    {
        //        return this.Session.FindObject<Phone>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.DefaultPhoneOid));
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            DefaultPhoneOid = null;
        //        }
        //        else
        //        {
        //            DefaultPhoneOid = value.Oid;
        //        }
        //    }
        //}

        //[Aggregated,Association("Address-Phones")]
        //public XPCollection<Phone> Phones
        //{
        //    get
        //    {
        //        return GetCollection<Phone>("Phones");
        //    }
        //}

        private Guid _Trader;
        public Guid Trader
        {
            get
            {
                return _Trader;
            }
            set
            {
                SetPropertyValue("Trader", ref _Trader, value);
            }
        }

        private Guid _Store;
        //[Association("Address-Store")]
        public Guid Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        public bool IsStore
        {
            get
            {
                return Store != Guid.Empty;
            }
        }


        public string email {
            get {
                return _Email;
            }
            set {
                SetPropertyValue("email", ref _Email, value);
            }
        }

        private Guid _VatLevel;
        public Guid VatLevel
        {
            get
            {
                return _VatLevel;
            }
            set
            {
                SetPropertyValue("VatLevel", ref _VatLevel, value);
            }
        }

        private int? _AutomaticNumbering;
        private string _Profession;

        public int? AutomaticNumbering
        {
            get
            {
                return _AutomaticNumbering;
            }
            set
            {
                SetPropertyValue("AutomaticNumbering", ref _AutomaticNumbering, value);
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
    }
}