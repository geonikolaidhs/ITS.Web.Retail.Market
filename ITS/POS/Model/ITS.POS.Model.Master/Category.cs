using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    [NonPersistent]
    public class Category : LookupField
    {
        public Category()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Category(Session session)
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

        private string _Code;
        [Indexed("GCRecord;ObjectType", Unique = true)]
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
