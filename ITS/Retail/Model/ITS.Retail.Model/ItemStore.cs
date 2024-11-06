//-----------------------------------------------------------------------
// <copyright file="ItemStore.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 500,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class ItemStore : BaseObj
    {
        public ItemStore()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemStore(Session session)
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

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
					if (store == null)
					{
						throw new Exception("ItemStore.GetUpdaterCriteria(); Error: Store is null");
					}
					crop = new BinaryOperator("Store.Oid", store.Oid);
					break;
			}

			return crop;
		}

        private Item _Item;
        [Association("ItemStores-Item")]
        public Item Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        private Store _Store;
        [Association("ItemStores-Store")]
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
    }

}