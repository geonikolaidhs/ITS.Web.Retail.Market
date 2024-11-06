using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class VatLevel : Lookup2Fields
    {
        public VatLevel()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatLevel(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatLevel(string code, string description)
            : base(code, description)
        {
        }
        public VatLevel(Session session, string code, string description)
            : base(session, code, description)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private bool _IsDefaultLevel;

        public bool IsDefaultLevel
        {
            get
            {
                return _IsDefaultLevel;
            }
            set
            {
                SetPropertyValue("IsDefaultLevel", ref _IsDefaultLevel, value);
            }
        }

    }
}
