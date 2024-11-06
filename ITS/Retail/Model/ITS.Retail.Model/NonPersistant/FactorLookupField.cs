using System;
using System.Collections.Generic;
using DevExpress.Xpo;

namespace ITS.Retail.Model
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

        public FactorLookupField(Session session, string code, string description, double factor)
            : base(session, code, description)
        {
            _Factor = factor;
        }
        public FactorLookupField(string code, string description, double factor)
            : base(code, description)
        {
            _Factor = factor;
        }



        //public override void GetData(Session myses, object item)
        //{
        //    FactorLookupField obj = (FactorLookupField)item;
        //    base.GetData(myses, item);
        //    Factor = obj.Factor;
        //}

    }

}