//-----------------------------------------------------------------------
// <copyright file="Phone.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 650,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class Phone : BaseObj, IOwner
    {
        public Phone()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Phone(Session session)
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
                        throw new Exception("Phone.GetUpdaterCriteria(); Error: Owner or Store is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Address.Trader.Oid", owner.Trader.Oid),
                                               new ContainsOperator("Address.Trader.Suppliers", new BinaryOperator("Owner.Oid", owner.Oid)),
                                               new ContainsOperator("Address.Trader.Customers", new BinaryOperator("Owner.Oid", owner.Oid))
                                              );

                    if (store.ReferenceCompanyOid != Guid.Empty)
                    {
                        CriteriaOperator storeCriteria = new ContainsOperator("Address.Trader.Companies", new BinaryOperator("Oid", store.ReferenceCompanyOid));
                        crop = CriteriaOperator.Or(crop, storeCriteria);
                    }

                    crop = CriteriaOperator.And(crop, CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner")));

                    break;
            }

            return crop;
        }

        [DescriptionField]
        public String Description
        {
            get
            {
                return (PhoneType == null ? "" : PhoneType.Description + ":") + Number;
            }
        }

        private PhoneType _PhoneType;
        [Association("PhoneType-Phones")]
        public PhoneType PhoneType
        {
            get
            {
                return _PhoneType;
            }
            set
            {
                SetPropertyValue("PhoneType", ref _PhoneType, value);
            }
        }

        private string _Number;
        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                SetPropertyValue("Number", ref _Number, value);
            }
        }

        private Address _Address;
        [Association("Address-Phones")]
        public Address Address
        {
            get
            {
                return _Address;
            }
            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

        private CompanyNew _Owner;

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }
    }

}