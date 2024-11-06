using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using DevExpress.Xpo.Metadata;

namespace ITS.POS.Model.Master
{
    // ITS(c)
    // Item Class : Περιγράφει την κλαση των ειδών
    //
    // kda
    [Indices("Barcode;GCRecord")]
    public class ItemBarcode: BaseObj
    {
        public ItemBarcode()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemBarcode(Session session)
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

        [Indexed("Owner;GCRecord", Unique = false)]
        public Guid Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

		[Indexed(Unique = false)]
        public Guid Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }
        
        public decimal RelationFactor
        {
            get
            {
                return _RelationFactor;
            }
            set
            {
                SetPropertyValue("RelationFactor", ref _RelationFactor, value);
            }
        }
        
        public Guid MeasurementUnit
        {
            get
            {
                return _MeasurementUnit;
            }
            set
            {
                SetPropertyValue("MeasurementUnit", ref _MeasurementUnit, value);
            }
        }
        
        public Guid Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }

        [Indexed("Owner;GCRecord", Unique = false)]
        public string PluCode
        {
            get
            {
                return _PluCode;
            }
            set
            {
                SetPropertyValue("PluCode", ref _PluCode, value);
            }
        }


        public string PluPrefix
        {
            get
            {
                return _PluPrefix;
            }
            set
            {
                SetPropertyValue("PluPrefix", ref _PluPrefix, value);
            }
        }

        // Fields...
        private string _PluPrefix;
        private string _PluCode;
        private Guid _Owner;
        private Guid _Barcode;
        private Guid _Item;
        private decimal _RelationFactor;
        private Guid _MeasurementUnit;
        private Guid _Type;
    }
}
