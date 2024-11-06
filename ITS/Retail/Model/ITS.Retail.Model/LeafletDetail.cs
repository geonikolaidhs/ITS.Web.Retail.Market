//-----------------------------------------------------------------------
// <copyright file="LeafletDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
namespace ITS.Retail.Model
{
    [Updater(Order =1008,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("LeafletDetail", typeof(ResourcesLib.Resources))]
        [Indices("Leaflet;Barcode;Oid;GCRecord")]
    public class LeafletDetail : BaseObj
    {
        public LeafletDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LeafletDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsActive = true;
        }

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
					if (owner == null)
					{
						throw new Exception("LeafletDetail.GetUpdaterCriteria(); Error: Owner is null");
					}
					crop = new BinaryOperator("Leaflet.Owner.Oid", owner.Oid);
					break;
			}
			return crop;
		}

        private Leaflet _Leaflet;
        [Association("Leaflet-LeafletDetails")]
        public Leaflet Leaflet
        {
            get
            {
                return _Leaflet;
            }
            set
            {
                SetPropertyValue("Leaflet", ref _Leaflet, value);
            }
        }

        private Item _Item;
        [Association("Item-LeafletDetails")]
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

        private decimal _Value;
        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        [NonPersistent]
        public ItemBarcode ItemBarcode
        {
            get
            {
                return this.Item != null ? this.Item.ItemBarcodes.FirstOrDefault(x => x.Barcode == this.Barcode) : null;
            }
            set
            {
                if (value != null && value.Owner != null && this.Leaflet != null && this.Leaflet.Owner != null
                    && value.Owner.Oid == this.Leaflet.Owner.Oid)
                {
                    this.Item = value.Item;
                    this.Barcode = value.Barcode;
                }
            }
        }

        private Barcode _Barcode;
        [Indexed(Unique = false), Association("Barcode-LeafletDetails")]
        public Barcode Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }
    }
}
