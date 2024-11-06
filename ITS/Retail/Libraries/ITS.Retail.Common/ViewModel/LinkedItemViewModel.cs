using ITS.Retail.Model;
using System;

namespace ITS.Retail.Common.ViewModel
{
    public class LinkedItemViewModel : BasePersistableViewModel
    {
        //Fields
        private double _QtyFactor;
        private Guid _Item;
        private Guid _SubItem;

        public override Type PersistedType { get { return typeof(LinkedItem); } }

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

        public Guid SubItem
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
