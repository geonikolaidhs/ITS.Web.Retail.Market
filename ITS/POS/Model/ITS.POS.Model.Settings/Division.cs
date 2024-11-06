using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;


namespace ITS.POS.Model.Settings
{
    public class Division : LookupField
    {
        public Division()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Division(Session session)
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
        private eDivision _Section;
        public eDivision Section
        {
            get
            {
                return _Section;
            }
            set
            {
                SetPropertyValue("Section", ref _Section, value);
            }
        }

    }
}
