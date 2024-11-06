using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class Lookup2Fields : LookupField
    {
        public Lookup2Fields()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Lookup2Fields(Session session)
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

        public Lookup2Fields(string code, string description)
            : base(description)
        {
            _Code = code;
        }
        public Lookup2Fields(Session session, string code, string description)
            : base(session, description)
        {
            _Code = code;
        }
        private string _Code;
        [Indexed("GCRecord",Unique = true) ]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }


    }
}
