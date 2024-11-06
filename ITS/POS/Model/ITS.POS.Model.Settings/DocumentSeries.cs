using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Settings
{
    public class DocumentSeries : Lookup2Fields
    {
        public DocumentSeries()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSeries(Session session)
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
        private bool _HasAutomaticNumbering;
        private Guid? _IsCanceledBy;
        private bool _IsCancelingSeries;
        private eModule _eModule;
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

        public bool IsCancelingSeries
        {
            get
            {
                return _IsCancelingSeries;
            }
            set
            {
                SetPropertyValue("IsCancelingSeries", ref _IsCancelingSeries, value);
            }
        }


        public Guid? IsCanceledBy
        {
            get
            {
                return _IsCanceledBy;
            }
            set
            {
                SetPropertyValue("IsCanceledBy", ref _IsCanceledBy, value);
            }
        }


        public bool HasAutomaticNumbering
        {
            get
            {
                return _HasAutomaticNumbering;
            }
            set
            {
                SetPropertyValue("HasAutomaticNumbering", ref _HasAutomaticNumbering, value);
            }
        }

        public eModule eModule
        {
            get
            {
                return _eModule;
            }
            set
            {
                SetPropertyValue("eModule", ref _eModule, value);
            }
        }
    }
}
