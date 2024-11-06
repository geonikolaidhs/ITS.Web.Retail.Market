using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.MobileInventory
{

    public class MobileInventoryEslEntry: MobileInventoryEntry
    {
         public MobileInventoryEslEntry()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public MobileInventoryEslEntry(Session session)
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
        private string _InventoryNumber;

        public string InventoryNumber
        {
            get
            {
                return _InventoryNumber;
            }
            set
            {
                SetPropertyValue("InventoryNumber", ref _InventoryNumber, value);
            }
        }
   
    }
}
