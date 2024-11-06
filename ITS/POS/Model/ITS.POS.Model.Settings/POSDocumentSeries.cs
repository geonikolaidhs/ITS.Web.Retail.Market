using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class POSDocumentSeries: BaseObj
    {
        public POSDocumentSeries()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSDocumentSeries(Session session)
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
        private Guid _StoreDocumentSeriesType;
        private Guid _POS;

        public Guid POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }

        public Guid StoreDocumentSeriesType
        {
            get
            {
                return _StoreDocumentSeriesType;
            }
            set
            {
                SetPropertyValue("StoreDocumentSeriesType", ref _StoreDocumentSeriesType, value);
            }
        }
    }
}
