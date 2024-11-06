using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class VatFactor : FactorLookupField
    {
        public VatFactor()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatFactor(Session session)
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

        private Guid _VatLevel;
        public Guid VatLevel
        {
            get
            {
                return _VatLevel;
            }
            set
            {
                SetPropertyValue("VatLevel", ref _VatLevel, value);
            }
        }

        private Guid _VatCategory;
        [Indexed("GCRecord;VatLevel")]
        public Guid VatCategory
        {
            get
            {
                return _VatCategory;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatCategory, value);
            }
        }


    }
}
