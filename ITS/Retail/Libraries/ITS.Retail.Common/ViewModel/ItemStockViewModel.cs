using ITS.Retail.Model;
using System;

namespace ITS.Retail.Common.ViewModel
{
    public class ItemStockViewModel : BasePersistableViewModel
    {
        private Guid _Store;
        private Guid _Item;
        private double _Stock;
        private double _DesirableStock;

        public override Type PersistedType { get { return typeof(ItemStock); } }

        public Guid Store
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
