//-----------------------------------------------------------------------
// <copyright file="StorePriceList.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 580, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("StorePriceList", typeof(ResourcesLib.Resources))]

    public class StorePriceList : BaseObj
    {
        public StorePriceList()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StorePriceList(Session session)
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

        private Store _Store;
        private PriceCatalog _PriceList;       

        [Association("Store-StorePriceLists"),Indexed("GCRecord", Unique=false)]
        public Store Store
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
        
        [Association("PriceCatalog-StorePriceLists"), Indexed("Store", "GCRecord", Unique = true)]
        public PriceCatalog PriceList
        {
            get { return _PriceList; }
            set
            {
                SetPropertyValue("PriceList", ref _PriceList, value);
            }
        }       

        [Aggregated, Association("StorePriceList-CustomerStorePriceLists")]
        public XPCollection<CustomerStorePriceList> CustomerStorePriceLists
        {
            get
            {
                return GetCollection<CustomerStorePriceList>("CustomerStorePriceLists");
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception("StorePriceList.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = new BinaryOperator("Store.Oid", store.Oid);
                    break;
            }
            return crop;
        }
    }
}