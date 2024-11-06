//-----------------------------------------------------------------------
// <copyright file="Supplier.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using System.Collections.Generic;
using ITS.Retail.Platform.Kernel.Model;


namespace ITS.Retail.Model
{
    [Updater(Order = 20,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CompanyNew", typeof(ResourcesLib.Resources))]
    public class CompanyNew : BaseObj, ITS.Retail.Platform.Kernel.Model.ICompany
    {
        public CompanyNew()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CompanyNew(Session session)
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

        public string Description
        {
            get
            {
                return this.CompanyName;
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("Company.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    if (store == null)
                    {
                        throw new Exception("Company.GetUpdaterCriteria(); Error: store is null");
                    }
                    CriteriaOperator ownerCriteria = new BinaryOperator("Oid", owner.Oid);
                    IEnumerable<Guid> referenceCompanies = owner.Stores.Select(x => x.ReferenceCompanyOid).Distinct();

                    crop = (referenceCompanies.Count() == 0) ? ownerCriteria : CriteriaOperator.Or(ownerCriteria, new InOperator("Oid", referenceCompanies));
                    break;
            }

            return crop;
        }

        private string _B2CURL;
        private double _Balance;
        private Trader _Trader;
        [Association("Trader-Suppliers")]
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

        private string _Code;
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

        private string _CompanyName;
        [DescriptionField]
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

        private Guid? _DefaultAddressOid;
        [Persistent("DefaultAddress")]
        public Guid? DefaultAddressOid
        {
            get
            {
                return _DefaultAddressOid;
            }
            set
            {
                SetPropertyValue("DefaultAddressOid", ref _DefaultAddressOid, value);
            }
        }

        //private Address _DefaultAddress;
        [NonPersistent]
        [UpdaterIgnoreField]
        public Address DefaultAddress
        {
            get
            {
                return this.Session.FindObject<Address>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", DefaultAddressOid));
            }
            set
            {
                if (value == null)
                    DefaultAddressOid = null;
                else
                    DefaultAddressOid = value.Oid;
            }
        }


        private string _Profession;
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


        public double Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                SetPropertyValue("Balance", ref _Balance, value);
            }
        }

        public string B2CURL
        {
            get
            {
                return _B2CURL;
            }
            set
            {
                SetPropertyValue("B2CURL", ref _B2CURL, value);
            }
        }


        [Aggregated, Association("Owner-Stores")]
        public XPCollection<Store> Stores
        {
            get
            {
                return GetCollection<Store>("Stores");
            }
        }



        [Association("Owner-Items")]
        public XPCollection<Item> ItemsCollection
        {
            get
            {
                return GetCollection<Item>("ItemsCollection");
            }
        }

        [Association("Supplier-PriceCatalogs")]
        public XPCollection<PriceCatalog> PriceCatalogs
        {
            get
            {
                return GetCollection<PriceCatalog>("PriceCatalogs");
            }
        }



        [Aggregated, Association("Supplier-FormMessages")]
        public XPCollection<FormMessage> FormMessages
        {
            get
            {
                return GetCollection<FormMessage>("FormMessages");
            }
        }

        [Aggregated, Association("Supplier-OwnerCategories")]
        public XPCollection<OwnerCategories> OwnerCategories
        {
            get
            {
                return GetCollection<OwnerCategories>("OwnerCategories");
            }
        }

        private OwnerApplicationSettings _ownerApplicationSettings;
        public OwnerApplicationSettings OwnerApplicationSettings
        {
            get
            {
                if (_ownerApplicationSettings == null)
                {
                    _ownerApplicationSettings = this.Session.FindObject<OwnerApplicationSettings>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, new BinaryOperator("Owner.Oid", this.Oid));
                    if (_ownerApplicationSettings == null)
                    {
                        _ownerApplicationSettings = this.Session.FindObject<OwnerApplicationSettings>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Owner.Oid", this.Oid));
                    }
                }
                return _ownerApplicationSettings;
            }
        }

        protected override void OnDeleting()
        {
            if (Stores.Where(g => g.DocumentHeaders.Count > 0).Count() > 0)
            {
                throw new Exception("There are Documents for this supplier");
            }
            base.OnDeleting();
        }

    }
}
