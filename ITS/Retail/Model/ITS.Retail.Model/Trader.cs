//-----------------------------------------------------------------------
// <copyright file="Trader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using Newtonsoft.Json;
using ITS.WRM.Model.Interface;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 10, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS),
     UniqueFields(UniqueFields = new string[] { "TaxCode" }), Indices("TaxCode;Oid")]
    [EntityDisplayName("Trader", typeof(ResourcesLib.Resources))]

    public class Trader : BaseObj, IPersistentObject
    {
        private string _TaxOffice;
        private string _Code;
        private string _FirstName;
        private string _LastName;
        private string _TaxCode;
        private eTraderType _TraderType;
        private Guid? _TaxOfficeLookUpOid;


        public Trader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Trader(Session session)
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
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null || store == null)
                    {
                        throw new Exception("Trader.GetUpdaterCriteria(); Error: Supplier or Store is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Oid", owner.Trader.Oid),
                                              //new BinaryOperator("Oid", store.ReferenceCompany == null ? Guid.Empty :store.ReferenceCompany.Trader.Oid),
                                              new ContainsOperator("Suppliers", new BinaryOperator("Owner.Oid", owner.Oid)),
                                              new ContainsOperator("Customers", new BinaryOperator("Owner.Oid", owner.Oid)));
                    IEnumerable<Guid> referenceCompaniesTraders = owner.Stores.Where(stor => stor.ReferenceCompany != null)
                                                                                .Select(stor => stor.ReferenceCompany.Trader.Oid)
                                                                                .Distinct();
                    if (referenceCompaniesTraders.Count() > 0)
                    {
                        crop = CriteriaOperator.Or(crop, new InOperator("Oid", referenceCompaniesTraders));
                    }
                    break;

            }

            return crop;
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

        [GDPR]
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

        [GDPR]
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


        [Indexed("CreatedByDevice", Unique = false)]
        [DescriptionField]
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


        public eTraderType TraderType
        {
            get
            {
                return _TraderType;
            }
            set
            {
                SetPropertyValue("TraderType", ref _TraderType, value);
            }
        }


        [Association("Trader-Customers")]
        public XPCollection<Customer> Customers
        {
            get
            {
                return GetCollection<Customer>("Customers");
            }
        }

        [Association("Trader-Suppliers")]
        public XPCollection<CompanyNew> Companies
        {
            get
            {
                return GetCollection<CompanyNew>("Companies");
            }
        }

        [Association("Trader-Companies")]
        public XPCollection<SupplierNew> Suppliers
        {
            get
            {
                return GetCollection<SupplierNew>("Suppliers");
            }
        }

        public bool IsCustomer
        {
            get
            {
                return this.Customers.Count != 0 ? true : false;
            }
        }
        public bool IsSupplier
        {
            get
            {
                return this.Companies.Count != 0 ? true : false;
            }
        }
        public bool IsCompany
        {
            get
            {
                return this.Suppliers.Count != 0 ? true : false;
            }
        }

        [Association("Trader-Addresses")]
        public XPCollection<Address> Addresses
        {
            get
            {
                return GetCollection<Address>("Addresses");
            }
        }

        public XPCollection<Store> GetStores()
        {
            return new XPCollection<Store>(this.Session, new BinaryOperator("Trader.Oid", Oid, BinaryOperatorType.Equal));
        }


        public string TaxOffice
        {
            get
            {
                return _TaxOffice;
            }
            set
            {
                SetPropertyValue("TaxOffice", ref _TaxOffice, value);
            }
        }

        [Persistent("TaxOfficeLookUp")]
        public Guid? TaxOfficeLookUpOid
        {
            get
            {
                return _TaxOfficeLookUpOid;
            }
            set
            {
                SetPropertyValue("TaxOfficeLookUpOid", ref _TaxOfficeLookUpOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public TaxOffice TaxOfficeLookUp
        {
            get
            {
                return this.Session.FindObject<TaxOffice>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.TaxOfficeLookUpOid));
            }
            set
            {
                if (value == null)
                {
                    this.TaxOffice = "";
                    this.TaxOfficeLookUpOid = null;
                }
                else
                {
                    this.TaxOffice = value.Description;
                    this.TaxOfficeLookUpOid = value.Oid;
                }
            }
        }

        public string FullJson(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, object> dict = this.GetDict(settings, includeType, true);
            if (dict.ContainsKey("Addresses") == false)
            {
                dict.Add("Addresses", this.Addresses.Select(x => x.FullJson(settings, includeType)));
            }
            else
            {
                dict["Addresses"] = this.Addresses.Select(x => x.FullJson(settings, includeType));
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }
        private DateTime? _GDPRRegistrationDate;
        private string _GDPRProtocolNumber;
        public DateTime? GDPRRegistrationDate
        {
            get
            {
                return _GDPRRegistrationDate;
            }
            set
            {
                SetPropertyValue("GDPRRegistrationDate", ref _GDPRRegistrationDate, value);
            }
        }
        public string GDPRProtocolNumber
        {
            get
            {
                return _GDPRProtocolNumber;
            }
            set
            {
                SetPropertyValue("GDPRProtocolNumber", ref _GDPRProtocolNumber, value);
            }
        }
    }

}
