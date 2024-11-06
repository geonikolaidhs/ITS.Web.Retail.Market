using System;
using DevExpress.Xpo;


namespace ITS.POS.Model.Settings
{
    public class MeasurementUnit : Lookup2Fields
    {
        private bool _SupportDecimal;

        public MeasurementUnit()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MeasurementUnit(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public MeasurementUnit(string code, string description)
            : base(code, description)
        {
        }
        public MeasurementUnit(Session session, string code, string description)
            : base(session, code, description)
        {
        }

        public bool SupportDecimal
        {
            get
            {
                return _SupportDecimal;
            }
            set
            {
                SetPropertyValue("SupportDecimal", ref _SupportDecimal, value);
            }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.

        }
    }
}
