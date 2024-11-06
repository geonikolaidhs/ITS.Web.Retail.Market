//-----------------------------------------------------------------------
// <copyright file="CustomerStorePriceList.cs" company="ITS">
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
    [Updater(Order = 600,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CustomerStorePriceList", typeof(ResourcesLib.Resources))]
    public class CustomerStorePriceList : BaseObj, IOwner
    {
        public CustomerStorePriceList()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerStorePriceList(Session session)
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

        private int _Gravity;
        private Customer _Customer;
        private StorePriceList _StorePriceList;
        private bool _IsDefault;

        [DescriptionField]
        public string Description
        {
            get
            {
                return Customer.CompanyName + " " + StorePriceList.Store.Name;
            }
        }
        
        [Association("Customer-CustomerStorePriceLists")]
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
        
        [Association("StorePriceList-CustomerStorePriceLists")]
        [Indexed("GCRecord", Unique = false)]
        public StorePriceList StorePriceList
        {
            get
            {
                return _StorePriceList;
            }
            set
            {
                SetPropertyValue("StorePriceList", ref _StorePriceList, value);
            }
        }

        public int Gravity
        {
            get
            {
                return _Gravity;
            }
            set
            {
                SetPropertyValue("Gravity", ref _Gravity, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                if (StorePriceList == null || StorePriceList.Store == null)
                {
                    return null;
                }
                return StorePriceList.Store.Owner;
            }
        }

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


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:

                    if (store == null)
                    {
                        throw new Exception("CustomerStorePriceList.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = CriteriaOperator.And(new BinaryOperator("StorePriceList.Store.Owner.Oid", supplier.Oid),
                        new BinaryOperator("StorePriceList.Store.Oid", store.Oid));

                    break;
            }
            return crop;
        }
    }

}