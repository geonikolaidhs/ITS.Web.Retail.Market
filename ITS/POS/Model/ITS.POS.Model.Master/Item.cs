using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Master
{
    // ITS(c)
    // Item Class : Περιγράφει την κλαση των ειδών
    //
    // kda
    public class Item : BaseObj
    {
        public Item()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Item(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            MotherCodeOid = Guid.Empty;
            // Place here your initialization code.
        }

        private double _MinOrderQty;
        private string _Remarks;
        private decimal _Points;
        private Guid _Owner;
        private string _ExtraDescription;
        private eItemCustomPriceOptions _CustomPriceOptions;
        private bool _AcceptsCustomDescription;
        private bool _AcceptsCustomPrice;
        private Guid _DefaultSupplier;
        private bool _AreDiscountsAllowed;
        private bool _IsCentralStored;
        private bool _DoesNotAllowDiscount;
        private bool _IsTax;

        public decimal Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Points", ref _Points, value);
            }
        }


        public bool DoesNotAllowDiscount
        {
            get
            {
                return _DoesNotAllowDiscount;
            }
            set
            {
                SetPropertyValue("DoesNotAllowDiscount", ref _DoesNotAllowDiscount, value);
            }
        }

        public bool IsTax
        {
            get
            {
                return _IsTax;
            }
            set
            {
                SetPropertyValue("IsTax", ref _IsTax, value);
            }
        }


        public bool AreDiscountsAllowed
        {
            get
            {
                return _AreDiscountsAllowed;
            }
            set
            {
                SetPropertyValue("AreDiscountsAllowed", ref _AreDiscountsAllowed, value);
            }

        }

        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        public double MinOrderQty
        {
            get
            {
                return _MinOrderQty;
            }
            set
            {
                SetPropertyValue("MinOrderQty", ref _MinOrderQty, value);
            }
        }

        [Indexed(Unique = false)]
        public string ExtraDescription
        {
            get
            {
                return _ExtraDescription;
            }
            set
            {
                SetPropertyValue("ExtraDescription", ref _ExtraDescription, value);
            }
        }

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


        private string _Code;
        [Indexed("Owner;GCRecord", Unique = true)]
        public string Code // Item Code
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

        private string _Name;
        [Indexed(Unique = false)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private Guid _DefaultBarcode;
        public Guid DefaultBarcode
        {
            get
            {
                return _DefaultBarcode;
            }
            set
            {
                SetPropertyValue("DefaultBarcode", ref _DefaultBarcode, value);
            }
        }


        private Guid _VatCategory;
        public Guid VatCategory
        {
            get
            {
                return _VatCategory;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatCategory, value);
            }
        }



        private Guid? _MotherCodeOid;
        public Guid? MotherCodeOid
        {
            get
            {
                return _MotherCodeOid;
            }
            set
            {
                SetPropertyValue("MotherCodeOid", ref _MotherCodeOid, value);
            }
        }

        private DateTime _InsertedDate;
        public DateTime InsertedDate
        {
            get
            {
                return _InsertedDate;
            }
            set
            {
                SetPropertyValue("InsertedDate", ref _InsertedDate, value);
            }
        }

        private Guid _Seasonality;
        public Guid Seasonality
        {
            get
            {
                return _Seasonality;
            }
            set
            {
                SetPropertyValue("Seasonality", ref _Seasonality, value);
            }
        }

        private Guid _Buyer;
        public Guid Buyer
        {
            get
            {
                return _Buyer;
            }
            set
            {
                SetPropertyValue("Buyer", ref _Buyer, value);
            }
        }

        private double _PackingQty;
        public double PackingQty
        {
            get
            {
                return _PackingQty;
            }
            set
            {
                SetPropertyValue("PackingQty", ref _PackingQty, value);
            }
        }

        private double _OrderQty;
        public double OrderQty
        {
            get
            {
                return _OrderQty;
            }
            set
            {
                SetPropertyValue("OrderQty", ref _OrderQty, value);
            }
        }

        private double _MaxOrderQty;
        private bool _IsGeneralItem;
        private Guid _ItemExtraInfo;

        public double MaxOrderQty
        {
            get
            {
                return _MaxOrderQty;
            }
            set
            {
                SetPropertyValue("MaxOrderQty", ref _MaxOrderQty, value);
            }
        }

        public bool IsCentralStored
        {
            get
            {
                return _IsCentralStored;
            }
            set
            {
                SetPropertyValue("IsCentralStored", ref _IsCentralStored, value);
            }
        }

        public Guid DefaultSupplier
        {
            get
            {
                return _DefaultSupplier;
            }
            set
            {
                SetPropertyValue("DefaultSupplier", ref _DefaultSupplier, value);
            }
        }


        public XPCollection<ItemBarcode> ItemBarcodes
        {
            get
            {
                return new XPCollection<ItemBarcode>(this.Session, new BinaryOperator("Item", Oid, BinaryOperatorType.Equal));
                //GetCollection<Barcode>("Barcodes");
            }
        }

        public XPCollection<Item> ChildItems
        {
            get
            {
                return new XPCollection<Item>(this.Session, new BinaryOperator("MotherCodeOid", Oid, BinaryOperatorType.Equal));
            }
        }

        public XPCollection<ItemAnalyticTree> ItemAnalyticTrees
        {
            get
            {
                return new XPCollection<ItemAnalyticTree>(this.Session, new BinaryOperator("Object", Oid, BinaryOperatorType.Equal));
                //GetCollection<ItemAnalyticTree>("ItemAnalyticTrees");
            }
        }

        public XPCollection<PriceCatalogDetail> PriceCatalogs
        {
            get
            {
                return new XPCollection<PriceCatalogDetail>(this.Session, new BinaryOperator("Item", Oid, BinaryOperatorType.Equal));
                //GetCollection<PriceCatalogDetail>("PriceCatalogs");
            }
        }

        public XPCollection<LinkedItem> LinkedItems
        {
            get
            {
                return new XPCollection<LinkedItem>(this.Session, new BinaryOperator("Item", Oid, BinaryOperatorType.Equal));
                //GetCollection<LinkedItem>("LinkedItems");
            }
        }

        public XPCollection<LinkedItem> SubItems
        {
            get
            {
                return new XPCollection<LinkedItem>(this.Session, new BinaryOperator("SubItem", Oid, BinaryOperatorType.Equal));
                //GetCollection<LinkedItem>("SubItems");
            }
        }

        public bool AcceptsCustomPrice
        {
            get
            {
                return _AcceptsCustomPrice;
            }
            set
            {
                SetPropertyValue("AcceptsCustomPrice", ref _AcceptsCustomPrice, value);
            }
        }


        public bool AcceptsCustomDescription
        {
            get
            {
                return _AcceptsCustomDescription;
            }
            set
            {
                SetPropertyValue("AcceptsCustomDescription", ref _AcceptsCustomDescription, value);
            }
        }



        public eItemCustomPriceOptions CustomPriceOptions
        {
            get
            {
                return _CustomPriceOptions;
            }
            set
            {
                SetPropertyValue("CustomPriceOptions", ref _CustomPriceOptions, value);
            }
        }

        public bool IsGeneralItem
        {
            get
            {
                return _IsGeneralItem;
            }
            set
            {
                SetPropertyValue("IsGeneralItem", ref _IsGeneralItem, value);
            }
        }




        //----------------------------- Methods -----------------------------------

        public T GetObject<T>(int id)
        {
            return Session.GetObjectByKey<T>(id);
        }

        public XPCollection<PriceCatalogDetail> GetAllPrices(Session session)
        {
            DateTime now = DateTime.Now;
            CriteriaOperator cop = CriteriaOperator.And(new BinaryOperator("Item", Oid),
            new BinaryOperator("StartDate", now, BinaryOperatorType.LessOrEqual),
            new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual));

            return new XPCollection<PriceCatalogDetail>(session, cop, new SortProperty("PriceCatalog.Code", SortingDirection.Ascending));
        }

        //public Guid ItemExtraInfo
        //{
        //    get
        //    {
        //        return _ItemExtraInfo;
        //    }
        //    set
        //    {
        //        SetPropertyValue("ItemExtraInfo", ref _ItemExtraInfo, value);
        //    }
        //}

        public ItemExtraInfo ItemExtraInfoObject
        {
            get
            {
                //return this.Session.GetObjectByKey<ItemExtraInfo>(ItemExtraInfo) ?? this.Session.FindObject<ItemExtraInfo>(new BinaryOperator("Item", this.Oid));
                return this.Session.FindObject<ItemExtraInfo>(new BinaryOperator("Item", this.Oid));
            }
        }


        public string NameWithExtraInfo
        {
            get
            {
                return Name + (ItemExtraInfoObject == null ? "" : " " + ItemExtraInfoObject.Description);
            }
        }
    }
}


