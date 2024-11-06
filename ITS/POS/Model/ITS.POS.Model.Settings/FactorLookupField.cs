using System;
using System.Collections.Generic;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class FactorLookupField : Lookup2Fields
    {
        public FactorLookupField()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public FactorLookupField(Session session)
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

        public FactorLookupField(Session session, string code, string description, decimal factor)
            : base(session, code, description)
        {
            _Factor = factor;
        }
        public FactorLookupField(string code, string description, decimal factor)
            : base(code, description)
        {
            _Factor = factor;
        }

        private decimal _Factor;
        public decimal Factor
        {
            get
            {
                return _Factor;
            }
            set
            {
                SetPropertyValue("Factor", ref _Factor, value);
            }
        }


    }
}
