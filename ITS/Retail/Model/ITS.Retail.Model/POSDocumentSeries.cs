using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Model
{
    [Updater(Order = 901,
        Permissions =  eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class POSDocumentSeries : BaseObj
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
            // Place here your initialization code.
        }


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {               
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop = new BinaryOperator("POS.Oid", deviceID);
                    break;
            }
            return crop;
        }


        // Fields...
        private StoreDocumentSeriesType _StoreDocumentSeriesType;
        private POS _POS;

        [Association("POS-StoreDocumentSeriesType")]
        public POS POS
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

        [Association("SDST-StoreDocumentSeriesType")]
        public StoreDocumentSeriesType StoreDocumentSeriesType
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
