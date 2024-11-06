using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
        [Indices(
        "PriceCatalog;Item;Barcode;GCRecord;IsActive;Oid",
        "PriceCatalog;Barcode;GCRecord;IsActive;Oid",
        "PriceCatalog;Item;GCRecord;IsActive;Oid",
        "GCRecord;IsActive;PriceCatalog;Item",
        "PriceCatalog;Item;Barcode;GCRecord;IsActive",
        "PriceCatalog;Barcode;GCRecord;IsActive"
        )]  
    public class PriceCatalogDetail : BaseObj
    {
        public PriceCatalogDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogDetail(Session session)
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

        private Guid _PriceCatalog;
        [Indexed("Item;Barcode;GCRecord", Unique = false)]
        public Guid PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }

        private Guid _Item;
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

        private Guid _Barcode;
        [Indexed(Unique = false)]
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

        private decimal _Discount;
        public decimal Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }

        private bool _VATIncluded;
        public bool VATIncluded
        {
            get
            {
                return _VATIncluded;
            }
            set
            {
                SetPropertyValue("VATIncluded", ref _VATIncluded, value);
            }
        }

        [NonPersistent]
        public decimal Value
        {
            get
            {
                long now = DateTime.Now.Ticks;
                PriceCatalogDetailTimeValue effectiveTimeValueObject = TimeValues
                                .Where(x => x.IsActive && x.TimeValueValidFrom <= now && x.TimeValueValidUntil >= now && x.TimeValue > 0)
                                .OrderBy(x => x.TimeValueRange).FirstOrDefault();
                if (effectiveTimeValueObject != null)
                {
                    return effectiveTimeValueObject.TimeValue;
                }
                return DatabaseValue;
            }
        }

        [Persistent("Value")]
        public decimal DatabaseValue
        {
            get
            {
                return _DatabaseValue;
            }
            set
            {
                SetPropertyValue("DatabaseValue", ref _DatabaseValue, value);
            }
        }

        public decimal TimeValue
        {
            get
            {
                return _TimeValue;
            }
            set
            {
                SetPropertyValue("TimeValue", ref _TimeValue, value);
            }
        }



        private string _TimeValueValidUntilStr;
        [Persistent("TimeValueValidUntil")]
        /// <summary>
        /// SQLite Fix ....
        /// </summary>
        public string TimeValueValidUntilStr
        {
            get
            {
                return _TimeValueValidUntilStr;
            }
            set
            {
                SetPropertyValue("TimeValueValidUntilStr", ref _TimeValueValidUntilStr, value);
            }
        }

        [NonPersistent]
        public long TimeValueValidUntil
        {
            get
            {
                long parsed = 0;
                long.TryParse(this.TimeValueValidUntilStr, out parsed);
                return parsed;
            }
            set
            {
                this.TimeValueValidUntilStr = value.ToString();
            }
        }


        [NonPersistent]
        public DateTime TimeValueValidUntilDate
        {
            get
            {
                return new DateTime(TimeValueValidUntil);
            }
            set
            {
                TimeValueValidUntil = value.Ticks;
            }
        }

        private string _TimeValueValidFromStr;
        private decimal _DatabaseValue;
        private decimal _TimeValue;

        [Persistent("TimeValueValidFrom")]
        /// <summary>
        /// SQLite Fix ....
        /// </summary>
        public string TimeValueValidFromStr
        {
            get
            {
                return _TimeValueValidFromStr;
            }
            set
            {
                SetPropertyValue("TimeValueValidFromStr", ref _TimeValueValidFromStr, value);
            }
        }

        [NonPersistent]
        public long TimeValueValidFrom
        {
            get
            {
                long parsed = 0;
                long.TryParse(this.TimeValueValidFromStr, out parsed);
                return parsed;
            }
            set
            {
                this.TimeValueValidFromStr = value.ToString();
            }
        }


        [NonPersistent]
        public DateTime TimeValueValidFromDate
        {
            get
            {
                return new DateTime(TimeValueValidFrom);
            }
            set
            {
                TimeValueValidFrom = value.Ticks;
            }
        }


        public bool TimeValueIsValid
        {
            get
            {
                if (TimeValue > 0)
                {
                    if (TimeValueValidFrom > TimeValueValidUntil)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected override bool UpdateTicksOnSaving
        {
            get { return false; }
        }

        public XPCollection<PriceCatalogDetailTimeValue> TimeValues
        {
            get
            {
                return new XPCollection<PriceCatalogDetailTimeValue>(this.Session, new BinaryOperator("PriceCatalogDetail", this.Oid));
            }
        }
        [NonPersistent]
        public PriceCatalogDetailTimeValue FirstTimeValue { get; set; }
        [NonPersistent]
        public PriceCatalogDetailTimeValue SecondTimeValue { get; set; }
    }
}
