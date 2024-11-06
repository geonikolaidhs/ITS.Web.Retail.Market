//-----------------------------------------------------------------------
// <copyright file="OfferDetail.cs" company="ITS">
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
    [Updater(Order = 640,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("OfferDetail", typeof(ResourcesLib.Resources))]
        [Indices("Offer;Oid;GCRecord")]
    public class OfferDetail : BaseObj
    {
        public OfferDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public OfferDetail(Session session)
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
						throw new Exception("OfferDetail.GetUpdaterCriteria(); Error: Owner is null");
					}
					crop = new BinaryOperator("Offer.PriceCatalog.Owner.Oid", owner.Oid);
					break;
			}
			return crop;
		}

        private Offer _Offer;
        [Association("Offer-OfferDetails")]
        public Offer Offer
        {
            get
            {
                return _Offer;
            }
            set
            {
                SetPropertyValue("Offer", ref _Offer, value);
            }
        }

        private Item _Item;
        [Association("Item-OfferDetails")]
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
    }
}
