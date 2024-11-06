using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class StoreDocumentSeriesType : BaseObj
    {
        public StoreDocumentSeriesType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDocumentSeriesType(Session session)
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

        private Guid _DocumentSeries;
        [Indexed("DocumentType", Unique = false)]
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

        private decimal _DefaultDiscountPercentage;
        public decimal DefaultDiscountPercentage
        {
            get
            {
                return _DefaultDiscountPercentage;
            }
            set
            {
                SetPropertyValue("DefaultDiscountPercentage", ref _DefaultDiscountPercentage, value);
            }
        }

        private Guid _DocumentType;
        [Indexed("DocumentSeries;GCRecord", Unique=false)]
        public Guid DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        private Guid _DefaultCustomer;
        

        public Guid DefaultCustomer
        {
            get
            {
                return _DefaultCustomer;
            }
            set
            {
                SetPropertyValue("DefaultCustomer", ref _DefaultCustomer, value);
            }
        }


        public bool HasAutomaticNumbering
        {
            get
            {
                DocumentSeries docSeries = this.Session.GetObjectByKey<DocumentSeries>(DocumentSeries);
                if(docSeries==null){
                    return false;
                }

                return docSeries.HasAutomaticNumbering;
            }
        }

        private StoreDocumentSeriesTypePrintServiceSettings _PrintServiceSettings;
        public StoreDocumentSeriesTypePrintServiceSettings PrintServiceSettings
        {
            get
            {
                return _PrintServiceSettings;
            }
            set
            {
                if (_PrintServiceSettings == value)
                {
                    return;
                }

                StoreDocumentSeriesTypePrintServiceSettings previousPrintServiceSettings = _PrintServiceSettings;
                _PrintServiceSettings = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousPrintServiceSettings != null && previousPrintServiceSettings.StoreDocumentSeriesType == this)
                {
                    previousPrintServiceSettings.StoreDocumentSeriesType = null;
                }


                if (_PrintServiceSettings != null)
                {
                    _PrintServiceSettings.StoreDocumentSeriesType = this;
                }

                OnChanged("PrintServiceSettings");
            }
        }

    }
}
