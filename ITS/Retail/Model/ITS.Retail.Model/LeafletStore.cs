using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 1003,
      Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class LeafletStore : BaseObj
    {
        public LeafletStore()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LeafletStore(Session session)
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
                        throw new Exception("LeafletStore.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = new BinaryOperator("Store.Oid", store.Oid);
                    break;
            }

            return crop;
        }

        private Leaflet _Leaflet;
        [Association("LeafletStores-Leaflet")]
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

        private Store _Store;
        [Association("LeafletStores-Store")]
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
