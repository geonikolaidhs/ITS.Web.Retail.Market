using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;

namespace ITS.POS.Model.Master
{
    public class ItemExtraInfo : BaseObj
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
        private Guid _Item;
        private string _Ingredients;
        private DateTime _PackedAt;
        private DateTime _ExpiresAt;
        private string _Origin;
        private string _Lot;

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

        //public Guid Owner
        //{
        //    get
        //    {
        //        if (Item == Guid.Empty)
        //        {
        //            return Guid.Empty;
        //        }
        //        return SessionMan;
        //    }
        //}

        [Indexed(Unique = false)]
        public Guid Item
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
    }
}
