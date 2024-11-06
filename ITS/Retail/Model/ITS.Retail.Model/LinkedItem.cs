//-----------------------------------------------------------------------
// <copyright file="LinkedItem.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 510,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("LinkedItem", typeof(ResourcesLib.Resources))]
    public class LinkedItem : BaseObj
    {
        public LinkedItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LinkedItem(Session session)
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
					if (supplier == null)
					{
						throw new Exception("LinkedItem.GetUpdaterCriteria(); Error: Supplier is null");
					}
					crop = new BinaryOperator("Item.Owner.Oid", supplier.Oid);
					break;
			}

			return crop;
		}


        private Item _Item;
        [Association("Item-LinkedItems")]
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

        private Item _SubItem;
        [Association("Item-SubItems")]
        public Item SubItem
        {
            get
            {
                return _SubItem;
            }
            set
            {
                SetPropertyValue("SubItem", ref _SubItem, value);
            }
        }

        private double _QtyFactor;
        public double QtyFactor
        {
            get
            {
                return _QtyFactor;
            }
            set
            {
                SetPropertyValue("QtyFactor", ref _QtyFactor, value);
            }
        }

    }

}