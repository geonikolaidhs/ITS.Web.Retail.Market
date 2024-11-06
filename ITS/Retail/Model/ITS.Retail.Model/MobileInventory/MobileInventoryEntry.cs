using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace ITS.Retail.Model.MobileInventory
{
    public class MobileInventoryEntry: BaseObj
    {
         public MobileInventoryEntry()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public MobileInventoryEntry(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        // Fields...
        private decimal _Qty;
        private Item _Item;

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


        public decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _Qty, value);
            }
        }


    }
}
