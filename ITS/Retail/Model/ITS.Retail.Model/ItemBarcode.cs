using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 460,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("ItemBarcode", typeof(ResourcesLib.Resources))]
    [Indices("Barcode;Owner;GCRecord;Oid", "GCRecord;Owner")]
    public class ItemBarcode : BaseObj, IRequiredOwner
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


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("ItemBarcode.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        // Fields...
        private string _PluPrefix;
        private string _PluCode;
        private MeasurementUnit _MeasurementUnit;
        private double _RelationFactor;
        private BarcodeType _Type;
        private CompanyNew _Owner;
        private Barcode _Barcode;
        private Item _Item;

        [Association("Item-ItemBarcodes")]
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

        [Indexed("Owner;GCRecord", Unique = true), Association("Barcode-ItemBarcodes")]
        public Barcode Barcode
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
        public CompanyNew Owner
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

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }
            if (this.Item != null && this.Item.Owner != Owner)
            {
                throw new Exception("Item Owner " + Item.Owner + " is not equal with ItemBarcode.Owner " + Owner);
            }
            base.OnSaving();
        }

        [DescriptionField]
        public string Description
        {
            get { return (this.Item != null && this.Item.Name != null ? this.Item.Name : "") + " - " + (this.Barcode != null && this.Barcode.Code != null ? this.Barcode.Code : ""); }
        }


        public BarcodeType Type
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


        [Association("MesurmentUnits-ItemBarcodes")]
        public MeasurementUnit MeasurementUnit
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

        public string FullDescription
        {
            get
            {
                return string.Format("{0} - ({1})", this.Item.Name, this.Barcode.Code);
            }
        }
    }
}
