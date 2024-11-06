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
using ITS.Retail.Model.Attributes;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITS.Retail.Model
{
    [Updater(Order = 40,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Address", typeof(ResourcesLib.Resources))]
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null || store == null)
                    {
                        throw new Exception("Address.GetUpdaterCriteria(); Error: Owner or Store is null");
                    }

                    crop = CriteriaOperator.Or(new BinaryOperator("Trader.Oid", owner.Trader.Oid),
                                               new ContainsOperator("Trader.Customers", new BinaryOperator("Owner", owner.Oid)),
                                               new ContainsOperator("Trader.Suppliers", new BinaryOperator("Owner", owner.Oid))
                                              );

                    if (store.ReferenceCompanyOid != Guid.Empty)
                    {
                        CriteriaOperator storeCriteria = new ContainsOperator("Trader.Companies", new BinaryOperator("Oid", store.ReferenceCompanyOid));
                        crop = CriteriaOperator.Or(crop, storeCriteria);
                    }
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (store == null)
                    {
                        throw new Exception("Address.GetUpdaterCriteria(); Error: Owner or Store is null");
                    }

                    crop =
                        CriteriaOperator.Or(new BinaryOperator("Trader.Oid", owner.Trader.Oid),
                                               new BinaryOperator("Store.Oid", store.Oid),
                                               new ContainsOperator("Trader.Customers", new BinaryOperator("Owner", owner.Oid)),
                                               new ContainsOperator("Trader.Suppliers", new BinaryOperator("Owner", owner.Oid))
                                               );

                    break;
            }
            return crop;
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

        private AddressType _AddressType;
        [Association("AddressType-Addresss")]
        public AddressType AddressType
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

        [NonPersistent]
        [DescriptionField]
        public string Description
        {
            get
            {
                string address = "";
                if (AddressType != null && String.IsNullOrEmpty(AddressType.Description) == false)
                {
                    address += AddressType.Description;
                }

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

        private Guid? _DefaultPhoneOid;
        [Persistent("DefaultPhone")]
        public Guid? DefaultPhoneOid
        {
            get
            {
                return _DefaultPhoneOid;
            }
            set
            {
                SetPropertyValue("DefaultPhoneOid", ref _DefaultPhoneOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        [NoForeignKey]
        public Phone DefaultPhone
        {
            get
            {
                if (this.DefaultPhoneOid.HasValue == false)
                {
                    return null;
                }
                return this.Session.FindObject<Phone>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.DefaultPhoneOid.Value));
            }
            set
            {
                if (value == null)
                {
                    DefaultPhoneOid = null;
                }
                else
                {
                    DefaultPhoneOid = value.Oid;
                }
            }
        }

        [Aggregated, Association("Address-Phones")]
        public XPCollection<Phone> Phones
        {
            get
            {
                return GetCollection<Phone>("Phones");
            }
        }

        private Trader _Trader;
        [Association("Trader-Addresses"), Indexed(Unique = false)]

        public Trader Trader
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

        private Store _Store;
        [UpdaterIgnoreField]
        //[Association("Address-Store")]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                if (_Store == value)
                {
                    return;
                }

                // Store a reference to the person's former house. 
                Store prevHouse = _Store;
                _Store = value;

                if (IsLoading)
                {
                    return;
                }

                // Remove a reference to the house's owner, if the person is its owner. 
                if (prevHouse != null && prevHouse.Address == this)
                {
                    prevHouse.Address = null;
                }

                // Specify the person as a new owner of the house. 
                if (_Store != null)
                {
                    _Store.Address = this;
                }

                OnChanged("Store");
            }
        }

        public bool IsStore
        {
            get
            {
                return Store != null;
            }
        }
        [GDPR]
        public string email
        {
            get
            {
                return _Email;
            }
            set
            {
                SetPropertyValue("email", ref _Email, value);
            }
        }

        private VatLevel _VatLevel;
        public VatLevel VatLevel
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

        private string _Region;
        private string _Profession;

        public string Region
        {
            get
            {
                return _Region;
            }
            set
            {
                SetPropertyValue("Region", ref _Region, value);
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

        private double _Latitude;

        public double Latitude
        {
            get
            {
                return _Latitude;
            }
            set
            {
                SetPropertyValue("Latitude", ref _Latitude, value);
            }
        }


        private double _Longitude;

        public double Longitude
        {
            get
            {
                return _Longitude;
            }
            set
            {
                SetPropertyValue("Longitude", ref _Longitude, value);
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }
            if (this.IsDeleted)
            {
                this.AutomaticNumbering = null;
            }
            else if (this.Trader != null && this.AutomaticNumbering == null)
            {
                int? maxAutomaticNumbering = (int?)Session.EvaluateInTransaction(this.Session.GetClassInfo(this),
                                                    CriteriaOperator.Parse("Max(AutomaticNumbering)"),
                                                    new BinaryOperator("Trader.Oid", this.Trader.Oid));
                if (maxAutomaticNumbering == null)
                {
                    maxAutomaticNumbering = 0;
                }

                maxAutomaticNumbering++;
                this.AutomaticNumbering = maxAutomaticNumbering;
            }
            base.OnSaving();
        }

        public string FullJson(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, object> dict = this.GetDict(settings, includeType, true);
            if (dict.ContainsKey("Phones") == false)
            {
                dict.Add("Phones", this.Phones.Select(x => x.ToJson(settings, includeType, true)));
            }
            else
            {
                dict["Phones"] = this.Phones.Select(x => x.ToJson(settings, includeType, true));
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }


    }
}
