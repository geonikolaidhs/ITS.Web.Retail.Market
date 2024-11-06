using ITS.Retail.Model;
using System;

namespace ITS.Retail.Common.ViewModel
{
    public class ItemStoreViewModel : BasePersistableViewModel
    {
        private Guid _Item;
        private Guid _Store;

        public override Type PersistedType { get { return typeof(ItemStore); } }

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
    }
}
