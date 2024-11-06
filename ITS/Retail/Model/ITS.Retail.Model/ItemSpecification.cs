//-----------------------------------------------------------------------
// <copyright file="ItemFlags.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model {
    [Updater(Order = 480,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class ItemSpecification : BaseObj {
        public ItemSpecification()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemSpecification(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
					if (owner == null)
					{
						throw new Exception("ItemSpecification.GetUpdaterCriteria(); Error: Owner is null");
					}
					crop = new BinaryOperator("Item.Owner.Oid", owner.Oid);
					break;
			}

			return crop;
		}

        // Fields...
        private Item _Item;
        private bool _CanBenSold;
        private bool _RequiresPrice;


        public Item Item {
            get {
                return _Item;
            }
            set {
                SetPropertyValue("Item", ref _Item, value);
            }
        }
        public bool RequiresPrice {
            get {
                return _RequiresPrice;
            }
            set {
                SetPropertyValue("RequiresPrice", ref _RequiresPrice, value);
            }
        }

        public bool CanBenSold {
            get {
                return _CanBenSold;
            }
            set {
                SetPropertyValue("CanBenSold", ref _CanBenSold, value);
            }
        }

    }
}
