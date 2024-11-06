using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Master
{
    public class Barcode : BaseObj
    {
        public Barcode()
            : base()
        {
            // This constructor is used when can object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Barcode(Session session)
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

        private string _Code;
        [Indexed("GCRecord",Unique = true)]
        public string Code // item barcode
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }
        

        //private Guid _Item;
        //public Guid Item
        //{
        //    get
        //    {
        //        return _Item;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Item", ref _Item, value);
        //    }
        //}

        //private Guid _MeasurementUnit;
        //public Guid MeasurementUnit
        //{
        //    get
        //    {
        //        return _MeasurementUnit;
        //    }
        //    set
        //    {
        //        SetPropertyValue("MeasurementUnit", ref _MeasurementUnit, value);
        //    }
        //}

        public Guid MeasurementUnit(Guid owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(owner);
            return itemBarcode==null ? Guid.Empty : itemBarcode.MeasurementUnit;
        }

        public ItemBarcode ItemBarcode(Guid owner)
        {
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Barcode", this.Oid), new BinaryOperator("Owner", owner));
            return this.Session.FindObject<ItemBarcode>(crop);
        }

        public Guid Type(Guid owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(owner);
            return itemBarcode == null ? Guid.Empty : itemBarcode.Type;
        }

        public decimal RelationFactor(Guid owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(owner);
            return itemBarcode == null ? 0 : itemBarcode.RelationFactor;
        }

        //private Guid _Type;
        //public Guid Type
        //{
        //    get
        //    {
        //        return _Type;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Type", ref _Type, value);
        //    }
        //}

        public XPCollection<PriceCatalogDetail> PriceCatalogDetails
        {
            get        
            {
                return new XPCollection<PriceCatalogDetail>(this.Session, new BinaryOperator("Barcode", Oid, BinaryOperatorType.Equal));
            }
        }
        //--------------------- Methods -------------------------

    }
}
