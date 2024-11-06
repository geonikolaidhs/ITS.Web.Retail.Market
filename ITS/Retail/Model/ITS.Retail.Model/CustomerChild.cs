//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.SupportingClasses;

namespace ITS.Retail.Model
{
    [Updater(Order = 215, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class CustomerChild : BaseObj
    {
        public CustomerChild()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerChild(Session session)
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

        // Fields...
        private Customer _Customer;
        private long _ChildBirthDateTicks;
        private eSex _ChildSex;
        [GDPR]
        public eSex ChildSex
        {
            get
            {
                return _ChildSex;
            }
            set
            {
                SetPropertyValue("ChildSex", ref _ChildSex, value);
            }
        }

        [GDPR]
        public long ChildBirthDateTicks
        {
            get
            {
                return _ChildBirthDateTicks;
            }
            set
            {
                SetPropertyValue("ChildBirthDateTicks", ref _ChildBirthDateTicks, value);
            }
        }
        [NonPersistent]
        public DateTime ChildBirthDate
        {
            get
            {
                return new DateTime(ChildBirthDateTicks);
            }
            set
            {
                ChildBirthDateTicks = value.Ticks;
            }
        }

        [Association("Customer-CustomerChilds")]
        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception(typeof(CustomerChild).Name + ".GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Customer.Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }
    }
}
