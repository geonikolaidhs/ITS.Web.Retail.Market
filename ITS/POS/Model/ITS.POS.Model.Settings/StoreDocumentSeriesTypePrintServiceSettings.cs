using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class StoreDocumentSeriesTypePrintServiceSettings : PrintServiceSettings
    {
        public StoreDocumentSeriesTypePrintServiceSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDocumentSeriesTypePrintServiceSettings(Session session)
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

        private StoreDocumentSeriesType _StoreDocumentSeriesType;

        public StoreDocumentSeriesType StoreDocumentSeriesType
        {
            get
            {
                return _StoreDocumentSeriesType;
            }
            set
            {
                if (_StoreDocumentSeriesType == value)
                {
                    return;
                }

                StoreDocumentSeriesType previousStoreDocumentSeriesType = _StoreDocumentSeriesType;
                _StoreDocumentSeriesType = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousStoreDocumentSeriesType != null && previousStoreDocumentSeriesType.PrintServiceSettings == this)
                {
                    previousStoreDocumentSeriesType.PrintServiceSettings = null;
                }


                if (_StoreDocumentSeriesType != null)
                {
                    _StoreDocumentSeriesType.PrintServiceSettings = this;
                }

                OnChanged("StoreDocumentSeriesType");
            }
        }
    }
}
