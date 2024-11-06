using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class LinkedItem : BaseObj
    {
        public LinkedItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LinkedItem(Session session)
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


        //Βασικό είδος 
        private Guid _Item;
        [Indexed("GCRecord", Unique = false)]
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

        // Είδος εγγυοδοσίας
        private Guid _SubItem;
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

        private double _QtyFactor;
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
