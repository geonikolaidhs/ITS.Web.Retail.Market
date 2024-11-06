using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class PriceCatalogPriority: XPBaseObject
    {
        private PriceCatalog _PriceCatalog;
        private decimal _Order;
        private PriceCatalogSearchMethod _Method;
        private Guid _Oid;
        private long _CreatedOnTicks;
        private Guid _BatchID;

        public PriceCatalogPriority()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPriority(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            CreatedOnTicks = DateTime.Now.Ticks;
            Oid = Guid.NewGuid();
        }

        [Key]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        public long CreatedOnTicks
        {
            get
            {
                return _CreatedOnTicks;
            }
            set
            {
                SetPropertyValue("CreatedOnTicks", ref _CreatedOnTicks, value);
            }
        }

        public Guid BatchID
        {
            get
            {
                return _BatchID;
            }
            set
            {
                SetPropertyValue("BatchID", ref _BatchID, value);
            }
        }

        public PriceCatalog PriceCatalog
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

        public decimal Order
        {
            get
            {
                return _Order;
            }
            set
            {
                SetPropertyValue("Order", ref _Order, value);
            }
        }

        public PriceCatalogSearchMethod Method
        {
            get
            {
                return _Method;
            }
            set
            {
                SetPropertyValue("Method", ref _Method, value);
            }
        }

    }
}
