using ITS.Retail.Model;
using System;
using DevExpress.Xpo;
namespace ITS.Retail.Common.ViewModel
{
    public class ItemBarcodeViewModel : BasePersistableViewModel
    {
        public override Type PersistedType { get { return typeof(ItemBarcode); } }

        public ItemBarcodeViewModel()
        {
            Barcode = new BarcodeViewModel();
        }

        // Fields...
        private Guid _Item;
        private BarcodeViewModel _Barcode;
        private Guid _Type;
        private Guid _MeasurementUnit;
        private double _RelationFactor;
        private string _PluPrefix;
        private string _PluCode;

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
        public BarcodeViewModel Barcode
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

       

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
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


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
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


        public double RelationFactor
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

        public override bool Validate(out string message)
        {
            return base.Validate(out message);
        }
    }
}
