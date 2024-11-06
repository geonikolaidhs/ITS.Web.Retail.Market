using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class DocumentStatus : Lookup2Fields
    {
        public DocumentStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            TakeSequence = false;
            ReadOnly = false;
        }
        private bool _TakeSequence;
        public bool TakeSequence
        {
            get
            {
                return _TakeSequence;
            }
            set
            {
                SetPropertyValue("TakeSequence", ref _TakeSequence, value);
            }
        }
        private bool _ReadOnly;
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                SetPropertyValue("ReadOnly", ref _ReadOnly, value);
            }
        }

    }
}
