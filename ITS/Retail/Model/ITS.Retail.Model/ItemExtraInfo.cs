using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    [DataViewParameter]
    [Serializable]
    [Updater(Order = 421,  Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("ItemExtraInfo", typeof(ResourcesLib.Resources))]
    //[Indices("Store;Item;GCRecord")]
    public class ItemExtraInfo : BaseObj, IRequiredOwner
    {
        public ItemExtraInfo()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemExtraInfo(Session session)
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

        private string _Description;
        private Item _Item;
        private string _Ingredients;
        private DateTime _PackedAt;
        private DateTime _ExpiresAt;
        private string _Origin;
        private string _Lot;
        private Store _Store;
        
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Ingredients
        {
            get
            {
                return _Ingredients;
            }
            set
            {
                SetPropertyValue("Ingredients", ref _Ingredients, value);
            }
        }

        public DateTime PackedAt
        {
            get
            {
                return _PackedAt;
            }
            set
            {
                SetPropertyValue("Ingredients", ref _PackedAt, value);
            }
        }

        public DateTime ExpiresAt
        {
            get
            {
                return _ExpiresAt;
            }
            set
            {
                SetPropertyValue("ExpiresAt", ref _ExpiresAt, value);
            }
        }

        [UpdaterIgnoreField]
        public CompanyNew Owner
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.Owner;
            }
        }
        [Association("Item-ItemExtraInfos")]
        //[Indexed("GCRecord", Unique = true)]
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

        public string Origin
        {
            get
            {
                return _Origin;
            }
            set
            {
                SetPropertyValue("Origin", ref _Origin, value);
            }
        }

        public string Lot
        {
            get
            {
                return _Lot;
            }
            set
            {
                SetPropertyValue("Lot", ref _Lot, value);
            }
        }

        [Association("Store-ItemExtraInfos")]
        //[Indexed("GCRecord", Unique = false)]
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            if (owner == null)
            {
                throw new Exception("ItemExtraInfo.GetUpdaterCriteria(); Error: Owner is null");
            }            

            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception("ItemExtraInfo.GetUpdaterCriteria(); Error: Store is not defined");
                    }
                    return CriteriaOperator.And(new BinaryOperator("Item.Owner.Oid", owner.Oid),  new BinaryOperator("Store.Oid", store.Oid));
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (string.IsNullOrEmpty(deviceID))
                    {
                        throw new Exception("ItemExtraInfo.GetUpdaterCriteria(); Error: DeviceID is not defined");
                    }
                    return new BinaryOperator("Item.Owner.Oid", owner.Oid);
                default:
                    return new BinaryOperator("Item.Owner.Oid", owner.Oid);
            }
        }
    }
}
