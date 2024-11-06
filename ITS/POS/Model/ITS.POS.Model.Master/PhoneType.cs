using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    public class PhoneType : Lookup2Fields
    {
        public PhoneType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PhoneType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public PhoneType(string code, string description)
            : base(code, description)
        {

        }
        public PhoneType(Session session, string code, string description)
            : base(session, code, description)
        {

        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.

        }
    }
}
