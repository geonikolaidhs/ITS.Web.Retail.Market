using System;
using DevExpress.Xpo;


namespace ITS.POS.Model.Settings
{
    public class SpecialItem : Lookup2Fields
    {

        public SpecialItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SpecialItem(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public SpecialItem(string code, string description)
            : base(code, description)
        {
        }
        public SpecialItem(Session session, string code, string description)
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
