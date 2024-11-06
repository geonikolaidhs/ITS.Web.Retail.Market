using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;


namespace ITS.POS.Model.Settings
{
    [SyncInfoIgnore]
    public class DocumentSequence : LookupField
    {
        public DocumentSequence()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSequence(Session session)
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

        public DocumentSequence(Session session, Guid series)
            : base(session)
        {
            this.DocumentSeries = series;
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private Guid _DocumentSeries;
        [Indexed(Unique = false)]
        public Guid DocumentSeries
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

        private Int32 _DocumentNumber;
        public Int32 DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }



    }
}
