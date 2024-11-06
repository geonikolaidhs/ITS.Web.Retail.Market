using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 490,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class ItemStock : BaseObj
    {
        public ItemStock()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemStock(Session session)
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

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
					if (store == null)
					{
						throw new Exception("ItemStock.GetUpdaterCriteria(); Error: Store is null");
					}
					crop = new BinaryOperator("Store.Oid", store.Oid);
					break;
			}
			return crop;
		}

        private Store _Store;
        [Association("Store-ItemStocks")]
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
        private Item _Item;
        [Association("Item-ItemStocks")]
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

        private double _Stock;
        private DateTime _LastDocumentHeaderFinalizedDate;
        private DocumentHeader _LastDocumentHeaderInventory;
        private double _DesirableStock;

        public double Stock
        {
            get
            {
                return _Stock;
            }
            set
            {
                SetPropertyValue("Stock", ref _Stock, value);
            }
        }

        public DateTime LastDocumentHeaderFinalizedDate
        {
            get
            {
                return _LastDocumentHeaderFinalizedDate;
            }
            set
            {
                SetPropertyValue("LastDocumentHeaderFinalizedDate", ref _LastDocumentHeaderFinalizedDate, value);
            }
        }

        public DocumentHeader LastDocumentHeaderInventory
        {
            get
            {
                return _LastDocumentHeaderInventory;
            }
            set
            {
                SetPropertyValue("LastDocumentHeaderInventory", ref _LastDocumentHeaderInventory, value);
            }
        }

        public double DesirableStock
        {
            get
            {
                return _DesirableStock;
            }
            set
            {
                SetPropertyValue("DesirableStock", ref _DesirableStock, value);
            }
        }
    }

}