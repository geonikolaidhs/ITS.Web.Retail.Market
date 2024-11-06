using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class FiscalDeviceDocumentSeries: BaseObj
    {
         public FiscalDeviceDocumentSeries()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public FiscalDeviceDocumentSeries(Session session)
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

        // Fields...
        private StoreControllerTerminalDeviceAssociation _FiscalDevice;
        private DocumentSeries _DocumentSeries;

        public DocumentSeries DocumentSeries
        {
            get
            {
                return _DocumentSeries;
            }
            set
            {
                SetPropertyValue("DocumentSeries", ref _DocumentSeries, value);
            }
        }

        [Association("FiscalDevice-DocumentSeries")]
        public StoreControllerTerminalDeviceAssociation FiscalDevice
        {
            get
            {
                return _FiscalDevice;
            }
            set
            {
                SetPropertyValue("FiscalDevice", ref _FiscalDevice, value);
            }
        }
    }
}
